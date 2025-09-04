using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TakiGame {
	/// <summary> 
	/// Manages hand display and card prefab instantiation
	/// Handles both player and computer hands with different facing
	/// Manual positioning system with adaptive spacing
	/// LOGGING: Reduced spam - only essential info and errors
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

		// Events
		public System.Action<CardController> OnCardSelected;
		public System.Action<List<CardData>> OnHandUpdated;

		// Internal state
		private List<CardController> cardControllers = new List<CardController> ();
		private CardController selectedCard = null;
		private List<CardData> currentHand = new List<CardData> ();

		// Integration references
		private GameManager gameManager;

		void Awake () {
			// Validate components
			if (handContainer == null) {
				handContainer = transform;
			}

			if (cardPrefab == null) {
				TakiLogger.LogError ($"HandManager {gameObject.name}: Card prefab not assigned!", TakiLogger.LogCategory.System);
			}
		}

		void Start () {
			// Find GameManager reference
			gameManager = FindObjectOfType<GameManager> ();
			if (gameManager == null) {
				TakiLogger.LogWarning ($"HandManager {gameObject.name}: GameManager not found!", TakiLogger.LogCategory.System);
			}
		}

		/// <summary>
		/// Update hand display with new card list
		/// This is the main method called by GameManager
		/// </summary>
		/// <param name="newHand">Updated hand of CardData</param>
		public void UpdateHandDisplay (List<CardData> newHand) {
			if (newHand == null) {
				TakiLogger.LogWarning ("UpdateHandDisplay called with null hand", TakiLogger.LogCategory.System);
				return;
			}

			// Store current hand
			currentHand = new List<CardData> (newHand);

			// Clear existing card prefabs
			ClearAllCards ();

			// Create new card prefabs
			CreateCardPrefabs (newHand);

			// Arrange cards with proper spacing
			ArrangeCards ();

			// Update playable states if this is player hand
			if (showFaceUpCards) {
				UpdatePlayableStates ();
			}

			OnHandUpdated?.Invoke (currentHand);
		}

		/// <summary>
		/// Add a single card to the hand (always at the end)
		/// </summary>
		/// <param name="cardData">Card to add</param>
		public void AddCard (CardData cardData) {
			if (cardData == null) return;

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
			if (showFaceUpCards) {
				UpdatePlayableStates ();
			}
		}

		/// <summary>
		/// Remove a specific card from the hand
		/// </summary>
		/// <param name="cardData">Card to remove</param>
		/// <returns>True if card was found and removed</returns>
		public bool RemoveCard (CardData cardData) {
			if (cardData == null) return false;

			// Find card in current hand
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
		/// Handle card selection from CardController
		/// </summary>
		/// <param name="cardController">Selected card controller</param>
		public void HandleCardSelection (CardController cardController) {
			if (cardController == null) return;

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
		/// Create card prefabs for entire hand
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
		/// Create a single card prefab
		/// </summary>
		/// <param name="cardData">CardData to represent</param>
		/// <param name="index">Index in hand</param>
		/// <returns>Created GameObject</returns>
		GameObject CreateCardPrefab (CardData cardData, int index) {
			if (cardPrefab == null || cardData == null) return null;

			// Instantiate prefab
			GameObject cardObj = Instantiate (cardPrefab, handContainer);
			cardObj.name = $"Card_{cardData.GetDisplayText ()}_{index}";

			// Get CardController component
			CardController controller = cardObj.GetComponent<CardController> ();
			if (controller == null) {
				TakiLogger.LogError ($"Card prefab missing CardController component!", TakiLogger.LogCategory.System);
				Destroy (cardObj);
				return null;
			}

			// Initialize controller
			controller.InitializeCard (cardData, this, showFaceUpCards);

			return cardObj;
		}

		/// <summary>
		/// Arrange cards with adaptive spacing - Manual positioning system
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
		/// Calculate spacing between cards based on hand size
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
		/// Update which cards are playable - REDUCED LOGGING
		/// </summary>
		void UpdatePlayableStates () {
			if (!showFaceUpCards || gameManager == null) {
				return;
			}

			// Get top discard card for rule validation
			CardData topCard = gameManager.GetTopDiscardCard ();
			if (topCard == null) {
				TakiLogger.LogWarning ($"HandManager {gameObject.name}: Cannot update playable states - no top discard card", TakiLogger.LogCategory.Rules);
				return;
			}

			int playableCount = 0;

			// Check each card's playability - MINIMAL LOGGING
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
		/// Delayed playable state update (for timing issues)
		/// </summary>
		public void DelayedPlayableUpdate () {
			UpdatePlayableStates ();
		}

		/// <summary>
		/// Clear all existing card prefabs
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
		/// Get currently selected CardData (for GameManager integration)
		/// </summary>
		/// <returns>Selected CardData or null</returns>
		public CardData GetSelectedCard () {
			return selectedCard?.CardData;
		}

		/// <summary>
		/// Clear current selection
		/// </summary>
		public void ClearSelection () {
			if (selectedCard != null) {
				selectedCard.SetSelected (false);
				selectedCard = null;
			}
		}

		/// <summary>
		/// Force update of playable states (called by GameManager after game state changes)
		/// </summary>
		public void RefreshPlayableStates () {
			if (showFaceUpCards) {
				UpdatePlayableStates ();

				// Also force visual refresh on all selected cards
				foreach (CardController controller in cardControllers) {
					if (controller != null && controller.IsSelected) {
						controller.ForceVisualRefresh ();
					}
				}
			}
		}

		/// <summary>
		/// Get debug information about current hand
		/// </summary>
		public void LogHandDebugInfo () {
			TakiLogger.LogDiagnostics ($"HandManager {gameObject.name} Debug Info:");
			TakiLogger.LogDiagnostics ($"  Cards in hand: {currentHand.Count}");
			TakiLogger.LogDiagnostics ($"  Card controllers: {cardControllers.Count}");
			TakiLogger.LogDiagnostics ($"  Selected card: {selectedCard?.CardData?.GetDisplayText () ?? "None"}");
			TakiLogger.LogDiagnostics ($"  Show face up: {showFaceUpCards}");

			// Log each card
			for (int i = 0; i < currentHand.Count; i++) {
				string cardInfo = currentHand [i]?.GetDisplayText () ?? "NULL";
				string controllerInfo = (i < cardControllers.Count && cardControllers [i] != null) ? "OK" : "MISSING";
				TakiLogger.LogDiagnostics ($"    [{i}] {cardInfo} - Controller: {controllerInfo}");
			}
		}

		// Public Properties
		public int HandSize => currentHand.Count;
		public bool HasSelectedCard => selectedCard != null;
		public CardController SelectedCardController => selectedCard;
		public List<CardData> CurrentHand => new List<CardData> (currentHand);
		public bool IsPlayerHand => showFaceUpCards;

		/// <summary>
		/// Clean up when component is destroyed
		/// </summary>
		void OnDestroy () {
			ClearAllCards ();
		}
	}
}