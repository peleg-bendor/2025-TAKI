using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// Manages visual display of draw and discard pile cards
	/// NO animations, instant updates only
	/// Uses CardController for consistency with hand system
	/// LOGGING: Reduced spam - only errors and warnings
	/// </summary>
	public class PileManager : MonoBehaviour {

		[Header ("Pile Visual Settings")]
		[Tooltip ("Prefab for creating pile card displays")]
		public GameObject cardPrefab;

		[Header ("Singleplayer Pile Containers")]
		[Tooltip ("Singleplayer Draw pile container - optional, falls back to drawPileContainer")]
		public Transform singlePlayerDrawPileContainer;

		[Tooltip ("Singleplayer Discard pile container - optional, falls back to discardPileContainer")]
		public Transform singlePlayerDiscardPileContainer;

		[Header ("Multiplayer Pile Containers")]
		[Tooltip ("Multiplayer Draw pile container - required for multiplayer mode")]
		public Transform multiPlayerDrawPileContainer;

		[Tooltip ("Multiplayer Discard pile container - required for multiplayer mode")]
		public Transform multiPlayerDiscardPileContainer;


		// Internal references
		private CardController drawPileCardController;
		private CardController discardPileCardController;

		// For draw pile visual
		private static CardData drawPileVisualCard;

		// Mode tracking for timing issue fix
		private bool lastKnownMultiplayerMode = false;
		private bool hasInitializedVisuals = false;

		// MILESTONE: Mode-aware container selection
		/// <summary>
		/// Get the appropriate draw pile container based on current game mode
		/// </summary>
		private Transform GetActiveDrawPileContainer() {
			bool isMultiplayer = IsMultiplayerMode();

			if (isMultiplayer && multiPlayerDrawPileContainer != null) {
				return multiPlayerDrawPileContainer;
			} else if (!isMultiplayer && singlePlayerDrawPileContainer != null) {
				return singlePlayerDrawPileContainer;
			}

			TakiLogger.LogError ("PileManager: GetActiveDrawPileContainer - One/Both of the DrawPileContainers is NULL!", TakiLogger.LogCategory.Deck);
			return null;
		}

		/// <summary>
		/// Get the appropriate discard pile container based on current game mode
		/// </summary>
		private Transform GetActiveDiscardPileContainer() {
			bool isMultiplayer = IsMultiplayerMode();

			if (isMultiplayer && multiPlayerDiscardPileContainer != null) {
				return multiPlayerDiscardPileContainer;
			} else if (!isMultiplayer && singlePlayerDiscardPileContainer != null) {
				return singlePlayerDiscardPileContainer;
			}

			TakiLogger.LogError ("PileManager: GetActiveDiscardPileContainer - One/Both of the DiscardPileContainers is NULL!", TakiLogger.LogCategory.Deck);
			return null;
		}

		/// <summary>
		/// Detect if we're in multiplayer mode by checking GameManager
		/// </summary>
		private bool IsMultiplayerMode() {
			// Try to find GameManager in the scene
			GameManager gameManager = FindObjectOfType<GameManager>();
			return gameManager != null && gameManager.IsMultiplayerMode;
		}

		void Start () {
			// Delay initial visual creation to avoid timing issues
			// Visual will be created in UpdateDrawPileDisplay when needed
		}

		void Update () {
			// Check if multiplayer mode has changed and recreate visuals if needed
			bool currentMultiplayerMode = IsMultiplayerMode();
			if (hasInitializedVisuals && currentMultiplayerMode != lastKnownMultiplayerMode) {
				TakiLogger.LogInfo ($"PileManager: Mode changed from {lastKnownMultiplayerMode} to {currentMultiplayerMode} - recreating visuals", TakiLogger.LogCategory.Deck);
				RecreateVisualsForModeChange();
			}
			lastKnownMultiplayerMode = currentMultiplayerMode;
		}

		/// <summary>
		/// Update draw pile display based on count
		/// </summary>
		/// <param name="cardCount">Number of cards in draw pile</param>
		public void UpdateDrawPileDisplay (int cardCount) {
			if (cardCount > 0) {
				// Show card back
				if (drawPileCardController == null) {
					CreateDrawPileVisual ();
					hasInitializedVisuals = true;
					lastKnownMultiplayerMode = IsMultiplayerMode();
				} else {
					// Make sure it's visible and face-down
					drawPileCardController.gameObject.SetActive (true);
					drawPileCardController.SetCardFacing (false); // Face-down
				}
			} else {
				// Hide draw pile card when empty
				if (drawPileCardController != null) {
					drawPileCardController.gameObject.SetActive (false);
				}
			}

			// Only log when pile becomes empty (potential issue)
			if (cardCount == 0) {
				TakiLogger.LogInfo ("Draw pile is now empty", TakiLogger.LogCategory.Deck);
			}
		}

		/// <summary>
		/// Update discard pile display with top card
		/// </summary>
		/// <param name="topCard">Current top discard card</param>
		public void UpdateDiscardPileDisplay (CardData topCard) {
			if (topCard == null) {
				// Hide discard card if no cards
				if (discardPileCardController != null) {
					discardPileCardController.gameObject.SetActive (false);
				}
				return;
			}

			// Create or update discard pile visual
			if (discardPileCardController == null) {
				CreateDiscardPileVisual ();
			}

			// Initialize with current top card
			if (discardPileCardController != null) {
				discardPileCardController.InitializeCard (topCard, null, true); // Face-up, no hand manager
				discardPileCardController.gameObject.SetActive (true);
				discardPileCardController.SetCardFacing (true); // Face-up
			}
		}

		/// <summary>
		/// Recreate visuals when mode changes to fix timing issues
		/// </summary>
		void RecreateVisualsForModeChange() {
			// Clear existing visuals
			ClearPileVisuals();

			// Recreate draw pile visual in correct container
			CreateDrawPileVisual();

			// If there was a discard pile, recreate it too
			if (discardPileCardController != null) {
				CreateDiscardPileVisual();
			}
		}

		/// <summary>
		/// Create visual card for draw pile (face-down card back)
		/// </summary>
		void CreateDrawPileVisual () {
			// MILESTONE: Use mode-aware container selection
			Transform activeDrawPileContainer = GetActiveDrawPileContainer();

			if (cardPrefab == null || activeDrawPileContainer == null) {
				TakiLogger.LogWarning ("PileManager: Cannot create draw pile visual - missing prefab or container", TakiLogger.LogCategory.System);
				return;
			}

			// Create dummy card data for visual representation
			if (drawPileVisualCard == null) {
				drawPileVisualCard = ScriptableObject.CreateInstance<CardData> ();
				drawPileVisualCard.cardName = "Draw Pile Back";
				drawPileVisualCard.color = CardColor.Red; // Valid color to prevent path errors
				drawPileVisualCard.cardType = CardType.Number;
				drawPileVisualCard.number = 1; // Valid number to prevent path errors
			}

			// Instantiate prefab using mode-aware container
			GameObject cardObj = Instantiate (cardPrefab, activeDrawPileContainer);
			cardObj.name = "DrawPileCard";

			// Get controller and initialize
			drawPileCardController = cardObj.GetComponent<CardController> ();
			if (drawPileCardController != null) {
				drawPileCardController.InitializeCard (drawPileVisualCard, null, false); // Face-down, no hand manager
				drawPileCardController.SetCardFacing (false); // Ensure face-down
			} else {
				TakiLogger.LogError ("PileManager: CardPrefab missing CardController component!", TakiLogger.LogCategory.System);
				Destroy (cardObj);
			}
		}

		/// <summary>
		/// Create visual card for discard pile
		/// </summary>
		void CreateDiscardPileVisual () {
			// MILESTONE: Use mode-aware container selection
			Transform activeDiscardPileContainer = GetActiveDiscardPileContainer();

			if (cardPrefab == null || activeDiscardPileContainer == null) {
				TakiLogger.LogWarning ("PileManager: Cannot create discard pile visual - missing prefab or container", TakiLogger.LogCategory.System);
				return;
			}

			// Instantiate prefab using mode-aware container
			GameObject cardObj = Instantiate (cardPrefab, activeDiscardPileContainer);
			cardObj.name = "DiscardPileCard";

			// Get controller (will be initialized when UpdateDiscardPileDisplay is called)
			discardPileCardController = cardObj.GetComponent<CardController> ();
			if (discardPileCardController != null) {
				// Start inactive - will be activated when first card is discarded
				cardObj.SetActive (false);
			} else {
				TakiLogger.LogError ("PileManager: CardPrefab missing CardController component!", TakiLogger.LogCategory.System);
				Destroy (cardObj);
			}
		}

		/// <summary>
		/// Clear both pile visuals
		/// </summary>
		public void ClearPileVisuals () {
			if (drawPileCardController != null) {
				Destroy (drawPileCardController.gameObject);
				drawPileCardController = null;
			}

			if (discardPileCardController != null) {
				Destroy (discardPileCardController.gameObject);
				discardPileCardController = null;
			}

			// Reset flags when clearing visuals
			hasInitializedVisuals = false;
		}

		/// <summary>
		/// Reset piles for new game
		/// </summary>
		public void ResetPiles () {
			ClearPileVisuals ();

			// Reset mode tracking flags to handle menu transitions safely
			hasInitializedVisuals = false;
			lastKnownMultiplayerMode = IsMultiplayerMode();

			// Visual will be created on-demand in UpdateDrawPileDisplay
		}

		// Properties for debugging
		public bool HasDrawPileVisual => drawPileCardController != null && drawPileCardController.gameObject.activeSelf;
		public bool HasDiscardPileVisual => discardPileCardController != null && discardPileCardController.gameObject.activeSelf;
		public CardData CurrentTopDiscard => discardPileCardController?.CardData;

		/// <summary>
		/// Clean up when destroyed
		/// </summary>
		void OnDestroy () {
			ClearPileVisuals ();

			// Clean up the dummy draw pile card
			if (drawPileVisualCard != null) {
				DestroyImmediate (drawPileVisualCard);
				drawPileVisualCard = null;
			}
		}
	}
}