using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TakiGame {
	/// <summary> 
	/// MILESTONE 1: Enhanced HandManager with Network Privacy System
	/// Preserves all existing functionality while adding opponent hand privacy
	/// </summary>
	public class HandManager : MonoBehaviour {

		[Header ("Hand Settings")]
		[Tooltip ("Parent transform for card prefabs")]
		public Transform handContainer;

		[Tooltip ("Card prefab to instantiate")]
		public GameObject cardPrefab;

		[Tooltip ("Whether this hand shows face-up cards (player) or face-down (computer)")]
		public bool showFaceUpCards = true;

		[Header ("Layout Settings")]
		[Tooltip ("Maximum spacing between cards when few cards")]
		public float maxSpacing = 120f;

		[Tooltip ("Minimum spacing between cards when many cards")]
		public float minSpacing = 40f;

		[Tooltip ("Threshold for switching to tight spacing")]
		public int tightSpacingThreshold = 8;

		[Tooltip ("Horizontal padding from edges")]
		public float horizontalPadding = 20f;

		[Header ("MILESTONE 1: Network Privacy System")]
		[Tooltip ("Is this a network multiplayer game?")]
		public bool isNetworkGame = false;

		// Events
		public System.Action<CardController> OnCardSelected;
		public System.Action<List<CardData>> OnHandUpdated;

		// Internal state - PRESERVED from original
		private List<CardController> cardControllers = new List<CardController> ();
		private CardController selectedCard = null;
		private List<CardData> currentHand = new List<CardData> ();

		// MILESTONE 1: Network privacy state
		private int networkOpponentHandCount = 0;
		private bool isDisplayingOpponentHand = false;

		// Integration references
		private GameManager gameManager;
		private BaseGameplayUIManager activeUI;

		void Awake () {
			// DIAGNOSTIC: Log that HandManager is initializing
			TakiLogger.LogSystem ($"HandManager {gameObject.name}: Awake() called - HandManager initializing...");

			// Validate components
			if (handContainer == null) {
				handContainer = transform;
			}

			if (cardPrefab == null) {
				TakiLogger.LogError ($"HandManager {gameObject.name}: Card prefab not assigned!", TakiLogger.LogCategory.System);
			}
		}

		void Start () {
			// DIAGNOSTIC: Log that Start is being called
			TakiLogger.LogSystem ($"HandManager {gameObject.name}: Start() called - Looking for GameManager...");

			// Find GameManager reference
			gameManager = FindObjectOfType<GameManager> ();
			if (gameManager == null) {
				TakiLogger.LogWarning ($"HandManager {gameObject.name}: GameManager not found!", TakiLogger.LogCategory.System);
				return;
			}

			// DIAGNOSTIC: Check GameManager UI architecture setup
			TakiLogger.LogSystem ($"HandManager {gameObject.name}: DIAGNOSTIC - GameManager found, checking UI architecture...");
			TakiLogger.LogSystem ($"  - useNewUIArchitecture: {gameManager.useNewUIArchitecture}");
			TakiLogger.LogSystem ($"  - singlePlayerUI: {(gameManager.singlePlayerUI != null ? "ASSIGNED" : "NULL")}");
			TakiLogger.LogSystem ($"  - multiPlayerUI: {(gameManager.multiPlayerUI != null ? "ASSIGNED" : "NULL")}");

			// Get active UI manager from GameManager (fixes screen hierarchy issue)
			activeUI = gameManager.GetActiveUI();
			if (activeUI == null) {
				TakiLogger.LogWarning ($"HandManager {gameObject.name}: No active UI manager available from GameManager!", TakiLogger.LogCategory.System);
				TakiLogger.LogWarning ($"  DIAGNOSTIC: This means GetActiveUI() returned null - check the diagnostic info above!", TakiLogger.LogCategory.System);
			} else {
				TakiLogger.LogUI ($"HandManager {gameObject.name}: Connected to active UI manager: {activeUI.GetType().Name}");
			}
		}

		/// <summary>
		/// Ensure activeUI connection is established (called on-demand)
		/// Fixes issue where HandManager is used before Start() runs
		/// </summary>
		void EnsureUIManagerConnection () {
			// Only initialize if not already done
			if (activeUI != null) {
				return;
			}

			TakiLogger.LogSystem ($"HandManager {gameObject.name}: EnsureUIManagerConnection() - On-demand UI manager setup...");

			// Find GameManager if not already found
			if (gameManager == null) {
				gameManager = FindObjectOfType<GameManager> ();
				if (gameManager == null) {
					TakiLogger.LogWarning ($"HandManager {gameObject.name}: GameManager not found during on-demand setup!", TakiLogger.LogCategory.System);
					return;
				}
			}

			// DIAGNOSTIC: Check GameManager UI architecture setup
			TakiLogger.LogSystem ($"HandManager {gameObject.name}: DIAGNOSTIC - GameManager found, checking UI architecture...");
			TakiLogger.LogSystem ($"  - useNewUIArchitecture: {gameManager.useNewUIArchitecture}");
			TakiLogger.LogSystem ($"  - singlePlayerUI: {(gameManager.singlePlayerUI != null ? "ASSIGNED" : "NULL")}");
			TakiLogger.LogSystem ($"  - multiPlayerUI: {(gameManager.multiPlayerUI != null ? "ASSIGNED" : "NULL")}");

			// Get active UI manager from GameManager
			activeUI = gameManager.GetActiveUI();
			if (activeUI == null) {
				TakiLogger.LogWarning ($"HandManager {gameObject.name}: No active UI manager available from GameManager during on-demand setup!", TakiLogger.LogCategory.System);
				TakiLogger.LogWarning ($"  DIAGNOSTIC: This means GetActiveUI() returned null - check the diagnostic info above!", TakiLogger.LogCategory.System);
			} else {
				TakiLogger.LogUI ($"HandManager {gameObject.name}: Connected to active UI manager on-demand: {activeUI.GetType().Name}");
			}
		}

		#region MILESTONE 1: Network Privacy System

		/// <summary>
		/// MILESTONE 1: Set network mode and configure privacy display
		/// </summary>
		/// <param name="isNetwork">True if this is a network game</param>
		public void SetNetworkMode (bool isNetwork) {
			isNetworkGame = isNetwork;
			TakiLogger.LogNetwork ($"HandManager {gameObject.name}: Network mode = {isNetwork}");

			// Determine if this hand should show opponent privacy
			if (isNetwork && !showFaceUpCards) {
				TakiLogger.LogNetwork ($"HandManager {gameObject.name}: Configured for opponent hand privacy");
				isDisplayingOpponentHand = true;
			} else {
				isDisplayingOpponentHand = false;
			}
		}

		/// <summary>
		/// MILESTONE 1: Update opponent hand count for network display
		/// FIXED: Uses centralized UI manager instead of direct UI access
		/// </summary>
		/// <param name="opponentCount">Number of cards opponent has</param>
		public void UpdateNetworkOpponentHandCount (int opponentCount) {
			networkOpponentHandCount = opponentCount;

			// Ensure activeUI is initialized (in case Start() hasn't run yet)
			EnsureUIManagerConnection ();

			// Use centralized UI manager for hand size updates
			if (activeUI != null) {
				// Get local hand count from GameManager or assume this is opponent hand
				int localHandCount = gameManager != null ? gameManager.PlayerHandSize : 0;
				activeUI.UpdateHandSizeDisplay (localHandCount, opponentCount);
				TakiLogger.LogNetwork ($"Opponent count updated via centralized UI: {opponentCount}");
			} else {
				TakiLogger.LogWarning ($"HandManager {gameObject.name}: Cannot update UI - Active UI manager not found (even after EnsureUIManagerConnection)", TakiLogger.LogCategory.System);
			}

			// FIXED: Only update card backs if we're actually displaying opponent hand AND count changed
			if (isDisplayingOpponentHand && opponentCount != currentHand.Count) {
				TakiLogger.LogNetwork ($"Updating card back display: {currentHand.Count} -> {opponentCount}");
				ShowOpponentHandAsCardBacks (opponentCount);
			} else if (isDisplayingOpponentHand) {
				TakiLogger.LogNetwork ($"Card back count unchanged: {opponentCount}");
			}

			TakiLogger.LogNetwork ($"HandManager {gameObject.name}: Opponent hand count updated to {opponentCount}");
		}

		/// <summary>
		/// MILESTONE 1: Show opponent hand as card backs with proper state management
		/// ENHANCED: Better state tracking to prevent unnecessary updates
		/// </summary>
		/// <param name="cardCount">Number of card backs to show</param>
		void ShowOpponentHandAsCardBacks (int cardCount) {
			TakiLogger.LogNetwork ($"Showing {cardCount} card backs for opponent hand");

			// Create list of null cards for card backs
			List<CardData> cardBacks = new List<CardData> ();
			for (int i = 0; i < cardCount; i++) {
				cardBacks.Add (null); // null = card back in our system (now properly handled by CardController)
			}

			// Update display using existing system - CardController now handles null properly
			currentHand = cardBacks;

			// Clear existing cards first
			ClearAllCards ();

			// Create new card prefabs (will be card backs due to null CardData)
			CreateCardPrefabs (cardBacks);

			// Arrange cards with proper spacing
			ArrangeCards ();

			TakiLogger.LogNetwork ($"Opponent hand displayed as {cardCount} card backs");
		}

		/// <summary>
		/// MILESTONE 1: Initialize hands for network multiplayer
		/// FIXED: Uses centralized UI manager for initialization
		/// </summary>
		/// <param name="isLocalPlayerHand">True if this is the local player's hand</param>
		public void InitializeNetworkHands (bool isLocalPlayerHand) {
			TakiLogger.LogNetwork ($"Initializing network hand: Local={isLocalPlayerHand}, FaceUp={showFaceUpCards}");

			if (isLocalPlayerHand) {
				// Local player hand - always face up
				showFaceUpCards = true;
				isDisplayingOpponentHand = false;
			} else {
				// Opponent hand - always face down with count
				showFaceUpCards = false;
				isDisplayingOpponentHand = true;

				// Initialize opponent count through centralized UI
				if (activeUI != null) {
					int localHandCount = gameManager != null ? gameManager.PlayerHandSize : 0;
					activeUI.UpdateHandSizeDisplay (localHandCount, 0);
				}
			}

			TakiLogger.LogNetwork ($"Network hand initialized: FaceUp={showFaceUpCards}, OpponentDisplay={isDisplayingOpponentHand}");
		}

		#endregion

		#region PRESERVED ORIGINAL FUNCTIONALITY - Enhanced for Network

		/// <summary>
		/// Update hand display with new card list - ENHANCED for network privacy
		/// </summary>
		/// <param name="newHand">Updated hand of CardData</param>
		public void UpdateHandDisplay (List<CardData> newHand) {
			if (newHand == null) {
				TakiLogger.LogWarning ("UpdateHandDisplay called with null hand", TakiLogger.LogCategory.System);
				return;
			}

			// MILESTONE 1: Handle network opponent hand privacy
			if (isNetworkGame && isDisplayingOpponentHand) {
				// For opponent hands in network mode, don't update with actual cards
				// Instead, show card backs based on hand count
				int handCount = newHand.Count;
				UpdateNetworkOpponentHandCount (handCount);
				return;
			}

			// Normal hand display for own hand or singleplayer
			currentHand = new List<CardData> (newHand);

			// Clear existing card prefabs
			ClearAllCards ();

			// Create new card prefabs
			CreateCardPrefabs (newHand);

			// Arrange cards with proper spacing
			ArrangeCards ();

			// Update playable states if this is player hand
			if (showFaceUpCards && !isDisplayingOpponentHand) {
				UpdatePlayableStates ();
			}

			OnHandUpdated?.Invoke (currentHand);

			TakiLogger.LogNetwork ($"Hand display updated: {newHand.Count} cards, Network={isNetworkGame}, Opponent={isDisplayingOpponentHand}");
		}

		/// <summary>
		/// Add a single card to the hand (always at the end) - ENHANCED for network
		/// </summary>
		/// <param name="cardData">Card to add</param>
		public void AddCard (CardData cardData) {
			if (cardData == null) return;

			// MILESTONE 1: Handle network opponent privacy
			if (isNetworkGame && isDisplayingOpponentHand) {
				// For opponent hands, just update count
				networkOpponentHandCount++;
				UpdateNetworkOpponentHandCount (networkOpponentHandCount);
				return;
			}

			// Normal add card logic for own hand
			currentHand.Add (cardData);

			// Create prefab for new card
			GameObject cardObj = CreateCardPrefab (cardData, currentHand.Count - 1);
			if (cardObj != null) {
				CardController controller = cardObj.GetComponent<CardController> ();
				if (controller != null) {
					cardControllers.Add (controller);
				}
			}

			// Rearrange all cards
			ArrangeCards ();

			// Update playable states if player hand
			if (showFaceUpCards && !isDisplayingOpponentHand) {
				UpdatePlayableStates ();
			}
		}

		/// <summary>
		/// Remove a specific card from the hand - ENHANCED for network
		/// </summary>
		/// <param name="cardData">Card to remove</param>
		/// <returns>True if card was found and removed</returns>
		public bool RemoveCard (CardData cardData) {
			if (cardData == null) return false;

			// MILESTONE 1: Handle network opponent privacy
			if (isNetworkGame && isDisplayingOpponentHand) {
				// For opponent hands, just update count
				if (networkOpponentHandCount > 0) {
					networkOpponentHandCount--;
					UpdateNetworkOpponentHandCount (networkOpponentHandCount);
					return true;
				}
				return false;
			}

			// Normal remove card logic for own hand
			int cardIndex = currentHand.FindIndex (card => card == cardData);
			if (cardIndex < 0) {
				TakiLogger.LogWarning ($"Card not found in hand: {cardData.GetDisplayText ()}", TakiLogger.LogCategory.System);
				return false;
			}

			// Remove from hand list
			currentHand.RemoveAt (cardIndex);

			// Remove corresponding controller
			if (cardIndex < cardControllers.Count) {
				CardController controller = cardControllers [cardIndex];
				cardControllers.RemoveAt (cardIndex);

				// Clear selection if this card was selected
				if (controller == selectedCard) {
					selectedCard = null;
				}

				// Destroy prefab
				if (controller != null && controller.gameObject != null) {
					Destroy (controller.gameObject);
				}
			}

			// Rearrange remaining cards
			ArrangeCards ();

			return true;
		}

		/// <summary>
		/// Handle card selection from CardController - PRESERVED with network awareness
		/// </summary>
		/// <param name="cardController">Selected card controller</param>
		public void HandleCardSelection (CardController cardController) {
			if (cardController == null) return;

			// MILESTONE 1: Prevent selection of opponent cards
			if (isNetworkGame && isDisplayingOpponentHand) {
				TakiLogger.LogNetwork ("Cannot select opponent cards in network game");
				return;
			}

			// Clear previous selection
			if (selectedCard != null && selectedCard != cardController) {
				selectedCard.SetSelected (false);
			}

			// Toggle selection on clicked card
			bool wasSelected = (selectedCard == cardController);
			if (wasSelected) {
				// Deselect
				selectedCard = null;
				cardController.SetSelected (false);
			} else {
				// Select new card
				selectedCard = cardController;
				cardController.SetSelected (true);
			}

			OnCardSelected?.Invoke (selectedCard);
		}

		/// <summary>
		/// Create card prefabs for entire hand - PRESERVED
		/// </summary>
		void CreateCardPrefabs (List<CardData> hand) {
			cardControllers.Clear ();

			for (int i = 0; i < hand.Count; i++) {
				GameObject cardObj = CreateCardPrefab (hand [i], i);
				if (cardObj != null) {
					CardController controller = cardObj.GetComponent<CardController> ();
					if (controller != null) {
						cardControllers.Add (controller);
					}
				}
			}
		}

		/// <summary>
		/// Create a single card prefab - PRESERVED (already handles null as card back)
		/// </summary>
		/// <param name="cardData">CardData to represent (null = card back)</param>
		/// <param name="index">Index in hand</param>
		/// <returns>Created GameObject</returns>
		GameObject CreateCardPrefab (CardData cardData, int index) {
			if (cardPrefab == null) return null;

			// Instantiate prefab
			GameObject cardObj = Instantiate (cardPrefab, handContainer);

			if (cardData != null) {
				cardObj.name = $"Card_{cardData.GetDisplayText ()}_{index}";
			} else {
				cardObj.name = $"CardBack_{index}";
			}

			// Get CardController component
			CardController controller = cardObj.GetComponent<CardController> ();
			if (controller == null) {
				TakiLogger.LogError ($"Card prefab missing CardController component!", TakiLogger.LogCategory.System);
				Destroy (cardObj);
				return null;
			}

			// Initialize controller - existing system handles null cardData as card back!
			bool shouldShowFaceUp = showFaceUpCards && !isDisplayingOpponentHand;
			controller.InitializeCard (cardData, this, shouldShowFaceUp);

			return cardObj;
		}

		/// <summary>
		/// Arrange cards with adaptive spacing - PRESERVED
		/// </summary>
		void ArrangeCards () {
			if (cardControllers.Count == 0) return;

			// Calculate spacing based on number of cards
			float spacing = CalculateSpacing (cardControllers.Count);

			// Calculate total width needed
			float totalWidth = (cardControllers.Count - 1) * spacing;

			// Calculate starting X position (center alignment)
			float startX = -totalWidth / 2f;

			// Position each card
			for (int i = 0; i < cardControllers.Count; i++) {
				CardController controller = cardControllers [i];
				if (controller == null) continue;

				// Calculate position
				Vector3 position = new Vector3 (
					startX + (i * spacing),
					0f,
					0f
				);

				// Update controller's original position
				controller.UpdateOriginalPosition (position);
			}
		}

		/// <summary>
		/// Calculate spacing between cards based on hand size - PRESERVED
		/// </summary>
		/// <param name="cardCount">Number of cards in hand</param>
		/// <returns>Spacing in pixels</returns>
		float CalculateSpacing (int cardCount) {
			if (cardCount <= 1) return maxSpacing;

			// Use minimum spacing for large hands
			if (cardCount >= tightSpacingThreshold) {
				return minSpacing;
			}

			// Interpolate between min and max for medium hands
			float t = (float)(cardCount - 1) / (tightSpacingThreshold - 1);
			return Mathf.Lerp (maxSpacing, minSpacing, t);
		}

		/// <summary>
		/// Update which cards are playable - ENHANCED with network awareness
		/// </summary>
		void UpdatePlayableStates () {
			if (!showFaceUpCards || gameManager == null || isDisplayingOpponentHand) {
				return;
			}

			// Get top discard card for rule validation
			CardData topCard = gameManager.GetTopDiscardCard ();
			if (topCard == null) {
				TakiLogger.LogWarning ($"HandManager {gameObject.name}: Cannot update playable states - no top discard card", TakiLogger.LogCategory.Rules);
				return;
			}

			int playableCount = 0;

			// Check each card's playability
			for (int i = 0; i < cardControllers.Count && i < currentHand.Count; i++) {
				CardController controller = cardControllers [i];
				CardData cardData = currentHand [i];

				if (controller != null && cardData != null) {
					// Use GameStateManager's validation logic
					bool isPlayable = gameManager.gameState?.IsValidMove (cardData, topCard) ?? false;
					controller.SetPlayable (isPlayable);

					if (isPlayable) playableCount++;
				} else {
					TakiLogger.LogWarning ($"HandManager {gameObject.name}: Missing controller or card data at index {i}", TakiLogger.LogCategory.System);
				}
			}

			// Only log summary if no playable cards (potential issue)
			if (playableCount == 0 && showFaceUpCards) {
				TakiLogger.LogInfo ($"HandManager {gameObject.name}: No playable cards found (0/{cardControllers.Count})", TakiLogger.LogCategory.Rules);
			}
		}

		/// <summary>
		/// Clear all existing card prefabs - PRESERVED
		/// </summary>
		void ClearAllCards () {
			// Destroy all card objects
			foreach (CardController controller in cardControllers) {
				if (controller != null && controller.gameObject != null) {
					Destroy (controller.gameObject);
				}
			}

			cardControllers.Clear ();
			selectedCard = null;
		}

		/// <summary>
		/// Get currently selected CardData - ENHANCED with network protection
		/// </summary>
		/// <returns>Selected CardData or null</returns>
		public CardData GetSelectedCard () {
			// MILESTONE 1: Prevent selection access for opponent hands
			if (isNetworkGame && isDisplayingOpponentHand) {
				return null;
			}

			return selectedCard?.CardData;
		}

		/// <summary>
		/// Clear current selection - PRESERVED
		/// </summary>
		public void ClearSelection () {
			if (selectedCard != null) {
				selectedCard.SetSelected (false);
				selectedCard = null;
			}
		}

		/// <summary>
		/// Force update of playable states - ENHANCED with network awareness
		/// </summary>
		public void RefreshPlayableStates () {
			if (showFaceUpCards && !isDisplayingOpponentHand) {
				UpdatePlayableStates ();

				// Also force visual refresh on all selected cards
				foreach (CardController controller in cardControllers) {
					if (controller != null && controller.IsSelected) {
						controller.ForceVisualRefresh ();
					}
				}
			}
		}

		#endregion

		#region PHASE 2: Enhanced Privacy System - Real Cards with Privacy Mode

		/// <summary>
		/// ENHANCED: Show opponent hand using REAL cards with privacy mode
		/// REPLACES: ShowOpponentHandAsCardBacks() null approach
		/// SAFETY: Additive method, old method preserved for compatibility
		/// </summary>
		/// <param name="realOpponentCards">Real cards from network (for synchronization)</param>
		public void ShowOpponentHandWithPrivacy (List<CardData> realOpponentCards) {
			if (realOpponentCards == null) {
				TakiLogger.LogError ("ShowOpponentHandWithPrivacy called with null cards", TakiLogger.LogCategory.Multiplayer);
				return;
			}

			TakiLogger.LogNetwork ($"Showing {realOpponentCards.Count} opponent cards with privacy mode");

			// Store the real cards (but they won't be visible due to privacy)
			currentHand = new List<CardData> (realOpponentCards);

			// Clear existing cards first
			ClearAllCards ();

			// Create new card prefabs using REAL cards with privacy mode
			CreateCardPrefabsWithPrivacy (realOpponentCards);

			// Arrange cards with proper spacing
			ArrangeCards ();

			// Update opponent count display through centralized UI
			networkOpponentHandCount = realOpponentCards.Count;
			if (activeUI != null) {
				int localHandCount = gameManager != null ? gameManager.PlayerHandSize : 0;
				activeUI.UpdateHandSizeDisplay (localHandCount, realOpponentCards.Count);
			}

			TakiLogger.LogNetwork ($"Opponent hand displayed with privacy: {realOpponentCards.Count} real cards as card backs");
		}

		/// <summary>
		/// ENHANCED: Create card prefabs using real cards with privacy mode
		/// REPLACES: CreateCardPrefabs() with null handling
		/// SAFETY: Private method, does not affect existing functionality
		/// </summary>
		/// <param name="realCards">Real CardData objects to display privately</param>
		private void CreateCardPrefabsWithPrivacy (List<CardData> realCards) {
			cardControllers.Clear ();

			for (int i = 0; i < realCards.Count; i++) {
				CardData realCard = realCards [i];

				// Create prefab for real card
				GameObject cardObj = CreateCardPrefabEnhanced (realCard, i, true); // true = privacy mode
				if (cardObj != null) {
					CardController controller = cardObj.GetComponent<CardController> ();
					if (controller != null) {
						cardControllers.Add (controller);
					}
				}
			}
		}

		/// <summary>
		/// ENHANCED: Create a single card prefab with enhanced privacy support
		/// SAFETY: Additive method, does not replace existing CreateCardPrefab
		/// </summary>
		/// <param name="cardData">Real CardData to represent</param>
		/// <param name="index">Index in hand</param>
		/// <param name="privacyMode">Enable privacy mode for opponent cards</param>
		/// <returns>Created GameObject</returns>
		private GameObject CreateCardPrefabEnhanced (CardData cardData, int index, bool privacyMode = false) {
			if (cardPrefab == null || cardData == null) return null;

			// Instantiate prefab
			GameObject cardObj = Instantiate (cardPrefab, handContainer);
			cardObj.name = privacyMode ? $"OpponentCard_{index}" : $"Card_{cardData.GetDisplayText ()}_{index}";

			// Get CardController component
			CardController controller = cardObj.GetComponent<CardController> ();
			if (controller == null) {
				TakiLogger.LogError ($"Card prefab missing CardController component!", TakiLogger.LogCategory.System);
				Destroy (cardObj);
				return null;
			}

			// Initialize using enhanced method with privacy mode
			bool shouldShowFaceUp = showFaceUpCards && !isDisplayingOpponentHand && !privacyMode;
			controller.InitializeCardEnhanced (cardData, this, shouldShowFaceUp, privacyMode);

			return cardObj;
		}

		/// <summary>
		/// ENHANCED: Update hand display using real cards with privacy (network-safe)
		/// INTEGRATES: With existing UpdateHandDisplay logic
		/// SAFETY: Additive logic path, existing paths preserved
		/// </summary>
		/// <param name="realHand">Real card data to display</param>
		/// <param name="usePrivacyMode">Force privacy mode for opponent hands</param>
		public void UpdateHandDisplayEnhanced (List<CardData> realHand, bool usePrivacyMode = false) {
			if (realHand == null) {
				TakiLogger.LogWarning ("UpdateHandDisplayEnhanced called with null hand", TakiLogger.LogCategory.System);
				return;
			}

			// Determine if this should use privacy mode
			bool shouldUsePrivacy = usePrivacyMode || (isNetworkGame && isDisplayingOpponentHand);

			if (shouldUsePrivacy) {
				// ENHANCED PATH: Use real cards with privacy
				ShowOpponentHandWithPrivacy (realHand);
			} else {
				// EXISTING PATH: Normal hand display (preserve existing functionality)
				UpdateHandDisplay (realHand); // Call existing method
			}

			TakiLogger.LogNetwork ($"Hand display updated (enhanced): {realHand.Count} cards, Privacy={shouldUsePrivacy}");
		}

		/// <summary>
		/// ENHANCED: Initialize network hands with real card support
		/// REPLACES: InitializeNetworkHands() with enhanced privacy features
		/// SAFETY: Enhanced version of existing method
		/// </summary>
		/// <param name="isLocalPlayerHand">True if this is the local player's hand</param>
		/// <param name="initialCards">Initial real cards for the hand (optional)</param>
		public void InitializeNetworkHandsEnhanced (bool isLocalPlayerHand, List<CardData> initialCards = null) {
			TakiLogger.LogNetwork ($"Initializing enhanced network hand: Local={isLocalPlayerHand}, Cards={initialCards?.Count ?? 0}");

			if (isLocalPlayerHand) {
				// Local player hand - always face up, no privacy
				showFaceUpCards = true;
				isDisplayingOpponentHand = false;

				// Initialize with real cards if provided
				if (initialCards != null && initialCards.Count > 0) {
					// Call the standard UpdateHandDisplay directly to ensure local hand is shown properly
					UpdateHandDisplay (initialCards);
					TakiLogger.LogNetwork ($"Local player hand directly displayed: {initialCards.Count} face-up cards");
				}
			} else {
				// Opponent hand - privacy mode with real cards
				showFaceUpCards = false;
				isDisplayingOpponentHand = true;

				// Use centralized UI to initialize opponent count
				if (activeUI != null) {
					int localHandCount = gameManager != null ? gameManager.PlayerHandSize : 0;
					int opponentCount = initialCards?.Count ?? 0;
					activeUI.UpdateHandSizeDisplay (localHandCount, opponentCount);
				}

				// Use enhanced privacy display for opponent hands
				if (initialCards != null && initialCards.Count > 0) {
					UpdateHandDisplayEnhanced (initialCards, true); // Force privacy for opponent hand
					TakiLogger.LogNetwork ($"Opponent hand displayed with privacy: {initialCards.Count} cards as card backs");
				}
			}

			TakiLogger.LogNetwork ($"Enhanced network hand initialized: FaceUp={showFaceUpCards}, OpponentDisplay={isDisplayingOpponentHand}");
		}

		/// <summary>
		/// ENHANCED: Add real card with automatic privacy detection
		/// SAFETY: Extends existing AddCard functionality
		/// </summary>
		/// <param name="cardData">Real card to add</param>
		/// <param name="usePrivacyMode">Force privacy mode (auto-detected if not specified)</param>
		public void AddCardEnhanced (CardData cardData, bool? usePrivacyMode = null) {
			if (cardData == null) return;

			// Auto-detect privacy mode if not specified
			bool privacyMode = usePrivacyMode ?? (isNetworkGame && isDisplayingOpponentHand);

			if (privacyMode) {
				// ENHANCED PATH: Add with privacy mode
				currentHand.Add (cardData);
				networkOpponentHandCount++;

				// Create prefab with privacy
				GameObject cardObj = CreateCardPrefabEnhanced (cardData, currentHand.Count - 1, true);
				if (cardObj != null) {
					CardController controller = cardObj.GetComponent<CardController> ();
					if (controller != null) {
						cardControllers.Add (controller);
					}
				}

				// Update count display through centralized UI
				if (activeUI != null) {
					int localHandCount = gameManager != null ? gameManager.PlayerHandSize : 0;
					activeUI.UpdateHandSizeDisplay (localHandCount, networkOpponentHandCount);
				}

				// Rearrange all cards
				ArrangeCards ();
			} else {
				// EXISTING PATH: Normal add card (preserve existing functionality)
				AddCard (cardData); // Call existing method
			}

			TakiLogger.LogNetwork ($"Card added (enhanced): {cardData.GetDisplayText ()}, Privacy={privacyMode}");
		}

		/// <summary>
		/// ENHANCED: Remove real card with automatic privacy detection
		/// SAFETY: Extends existing RemoveCard functionality
		/// </summary>
		/// <param name="cardData">Real card to remove</param>
		/// <returns>True if card was found and removed</returns>
		public bool RemoveCardEnhanced (CardData cardData) {
			if (cardData == null) return false;

			bool privacyMode = isNetworkGame && isDisplayingOpponentHand;

			if (privacyMode) {
				// ENHANCED PATH: Remove with privacy mode
				int cardIndex = currentHand.FindIndex (card => card?.cardType == cardData.cardType &&
															card?.color == cardData.color);
				if (cardIndex < 0) {
					TakiLogger.LogWarning ($"Card not found in private hand: {cardData.GetDisplayText ()}", TakiLogger.LogCategory.Multiplayer);
					return false;
				}

				// Remove from hand list
				currentHand.RemoveAt (cardIndex);
				networkOpponentHandCount--;

				// Remove corresponding controller
				if (cardIndex < cardControllers.Count) {
					CardController controller = cardControllers [cardIndex];
					cardControllers.RemoveAt (cardIndex);

					// Destroy prefab
					if (controller != null && controller.gameObject != null) {
						Destroy (controller.gameObject);
					}
				}

				// Update count display through centralized UI
				if (activeUI != null) {
					int localHandCount = gameManager != null ? gameManager.PlayerHandSize : 0;
					activeUI.UpdateHandSizeDisplay (localHandCount, networkOpponentHandCount);
				}

				// Rearrange remaining cards
				ArrangeCards ();

				TakiLogger.LogNetwork ($"Card removed (enhanced): {cardData.GetDisplayText ()}, Privacy={privacyMode}");
				return true;
			} else {
				// EXISTING PATH: Normal remove card (preserve existing functionality)
				return RemoveCard (cardData); // Call existing method
			}
		}

		#endregion

		#region PHASE 2: Integration Methods

		/// <summary>
		/// ENHANCED: Set network mode with automatic privacy configuration
		/// REPLACES: SetNetworkMode() with enhanced features
		/// SAFETY: Enhanced version of existing method
		/// </summary>
		/// <param name="isNetwork">True if this is a network game</param>
		/// <param name="forceOpponentMode">Force opponent hand mode regardless of showFaceUpCards</param>
		public void SetNetworkModeEnhanced (bool isNetwork, bool forceOpponentMode = false) {
			isNetworkGame = isNetwork;
			TakiLogger.LogNetwork ($"HandManager {gameObject.name}: Enhanced network mode = {isNetwork}");

			// Determine if this hand should show opponent privacy
			if (isNetwork && (!showFaceUpCards || forceOpponentMode)) {
				TakiLogger.LogNetwork ($"HandManager {gameObject.name}: Configured for enhanced opponent hand privacy");
				isDisplayingOpponentHand = true;
			} else {
				isDisplayingOpponentHand = false;
			}
		}

		/// <summary>
		/// ENHANCED: Synchronize opponent hand count with real card tracking
		/// REPLACES: UpdateNetworkOpponentHandCount() with enhanced features
		/// SAFETY: Enhanced version of existing method
		/// </summary>
		/// <param name="opponentCount">Number of cards opponent has</param>
		/// <param name="realCards">Actual opponent cards for synchronization (optional)</param>
		public void SynchronizeOpponentHandEnhanced (int opponentCount, List<CardData> realCards = null) {
			networkOpponentHandCount = opponentCount;

			// Update count display through centralized UI
			if (activeUI != null) {
				int localHandCount = gameManager != null ? gameManager.PlayerHandSize : 0;
				activeUI.UpdateHandSizeDisplay (localHandCount, opponentCount);
				TakiLogger.LogNetwork ($"Opponent count updated via centralized UI: {opponentCount}");
			}

			// If we have real cards, use enhanced privacy display
			if (realCards != null && realCards.Count == opponentCount) {
				TakiLogger.LogNetwork ($"Synchronizing with {realCards.Count} real opponent cards");
				ShowOpponentHandWithPrivacy (realCards);
			} else if (isDisplayingOpponentHand && opponentCount != currentHand.Count) {
				// Fallback to old method for compatibility (when real cards not available)
				TakiLogger.LogNetwork ($"Falling back to card backs: {currentHand.Count} -> {opponentCount}");
				ShowOpponentHandAsCardBacks (opponentCount);
			}

			TakiLogger.LogNetwork ($"HandManager {gameObject.name}: Enhanced opponent synchronization complete: {opponentCount} cards");
		}

		#endregion

		#region PHASE 2: Public Properties - Enhanced

		/// <summary>
		/// Get current hand with privacy awareness
		/// Returns empty list for opponent hands to maintain privacy
		/// </summary>
		public List<CardData> GetVisibleHand () {
			return (isNetworkGame && isDisplayingOpponentHand) ? new List<CardData> () : new List<CardData> (currentHand);
		}

		/// <summary>
		/// Get actual hand count (even for opponent hands)
		/// Useful for network synchronization
		/// </summary>
		public int GetActualHandCount () {
			return isDisplayingOpponentHand ? networkOpponentHandCount : currentHand.Count;
		}

		/// <summary>
		/// Check if this hand is using enhanced privacy features
		/// </summary>
		public bool IsUsingEnhancedPrivacy () {
			return isNetworkGame && isDisplayingOpponentHand &&
				cardControllers.Any (controller => controller != null && controller.IsPrivacyMode);
		}

		#endregion

		#region Debug and Diagnostics - Enhanced with Network Info

		/// <summary>
		/// Get debug information about current hand - ENHANCED with network info
		/// </summary>
		public void LogHandDebugInfo () {
			TakiLogger.LogDiagnostics ($"HandManager {gameObject.name} Debug Info:");
			TakiLogger.LogDiagnostics ($"  Network game: {isNetworkGame}");
			TakiLogger.LogDiagnostics ($"  Show face up: {showFaceUpCards}");
			TakiLogger.LogDiagnostics ($"  Displaying opponent: {isDisplayingOpponentHand}");
			TakiLogger.LogDiagnostics ($"  Cards in hand: {currentHand.Count}");
			TakiLogger.LogDiagnostics ($"  Card controllers: {cardControllers.Count}");
			TakiLogger.LogDiagnostics ($"  Network opponent count: {networkOpponentHandCount}");
			TakiLogger.LogDiagnostics ($"  Selected card: {selectedCard?.CardData?.GetDisplayText () ?? "None"}");

			// Log each card (respecting privacy)
			if (!isDisplayingOpponentHand) {
				for (int i = 0; i < currentHand.Count; i++) {
					string cardInfo = currentHand [i]?.GetDisplayText () ?? "NULL (Card Back)";
					string controllerInfo = (i < cardControllers.Count && cardControllers [i] != null) ? "OK" : "MISSING";
					TakiLogger.LogDiagnostics ($"    [{i}] {cardInfo} - Controller: {controllerInfo}");
				}
			} else {
				TakiLogger.LogDiagnostics ($"    Opponent hand: {networkOpponentHandCount} card backs");
			}
		}

		#endregion

		#region Public Properties - Enhanced with Network

		public int HandSize => currentHand.Count;
		public bool HasSelectedCard => selectedCard != null && !isDisplayingOpponentHand;
		public CardController SelectedCardController => isDisplayingOpponentHand ? null : selectedCard;
		public List<CardData> CurrentHand => isDisplayingOpponentHand ? new List<CardData> () : new List<CardData> (currentHand);
		public bool IsPlayerHand => showFaceUpCards && !isDisplayingOpponentHand;

		// MILESTONE 1: Enhanced network properties 
		public bool IsNetworkGame => isNetworkGame;
		public int NetworkOpponentHandCount => networkOpponentHandCount;
		public bool IsOpponentHand => isDisplayingOpponentHand;
		public bool IsDisplayingCardBacks => isDisplayingOpponentHand;

		#endregion

		/// <summary>
		/// Clean up when component is destroyed - PRESERVED
		/// </summary>
		void OnDestroy () {
			ClearAllCards ();
		}
	}
}