using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TakiGame {
	/// <summary>
	/// Manages hand display and card prefab instantiation
	/// Handles both player and computer hands with different facing
	/// Manual positioning system with adaptive spacing
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
				Debug.LogError ($"HandManager {gameObject.name}: Card prefab not assigned!");
			}
		}

		void Start () {
			// Find GameManager reference
			gameManager = FindObjectOfType<GameManager> ();
			if (gameManager == null) {
				Debug.LogWarning ($"HandManager {gameObject.name}: GameManager not found!");
			}
		}

		/// <summary>
		/// Update hand display with new card list
		/// This is the main method called by GameManager
		/// </summary>
		/// <param name="newHand">Updated hand of CardData</param>
		public void UpdateHandDisplay (List<CardData> newHand) {
			if (newHand == null) {
				Debug.LogWarning ("UpdateHandDisplay called with null hand");
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

			Debug.Log ($"HandManager {gameObject.name}: Updated display with {newHand.Count} cards");
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

			Debug.Log ($"HandManager {gameObject.name}: Added card {cardData.GetDisplayText ()}");
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
				Debug.LogWarning ($"Card not found in hand: {cardData.GetDisplayText ()}");
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

			Debug.Log ($"HandManager {gameObject.name}: Removed card {cardData.GetDisplayText ()}");
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

			Debug.Log ($"HandManager {gameObject.name}: Card selection - {cardController.CardData?.GetDisplayText ()}, Selected: {!wasSelected}");
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
				Debug.LogError ($"Card prefab missing CardController component!");
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

			Debug.Log ($"HandManager {gameObject.name}: Arranged {cardControllers.Count} cards with {spacing:F1}px spacing");
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
		/// Update which cards are playable (only for player hands)
		/// </summary>
		void UpdatePlayableStates () {
			if (!showFaceUpCards || gameManager == null) return;

			// Get top discard card for rule validation
			CardData topCard = gameManager.GetTopDiscardCard ();
			if (topCard == null) return;

			// Check each card's playability
			for (int i = 0; i < cardControllers.Count && i < currentHand.Count; i++) {
				CardController controller = cardControllers [i];
				CardData cardData = currentHand [i];

				if (controller != null && cardData != null) {
					// Use GameStateManager's validation logic
					bool isPlayable = gameManager.gameState?.IsValidMove (cardData, topCard) ?? false;
					controller.SetPlayable (isPlayable);
				}
			}
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
			}
		}

		/// <summary>
		/// Get debug information about current hand
		/// </summary>
		public void LogHandDebugInfo () {
			Debug.Log ($"HandManager {gameObject.name} Debug Info:");
			Debug.Log ($"  Cards in hand: {currentHand.Count}");
			Debug.Log ($"  Card controllers: {cardControllers.Count}");
			Debug.Log ($"  Selected card: {selectedCard?.CardData?.GetDisplayText () ?? "None"}");
			Debug.Log ($"  Show face up: {showFaceUpCards}");

			// Log each card
			for (int i = 0; i < currentHand.Count; i++) {
				string cardInfo = currentHand [i]?.GetDisplayText () ?? "NULL";
				string controllerInfo = (i < cardControllers.Count && cardControllers [i] != null) ? "OK" : "MISSING";
				Debug.Log ($"    [{i}] {cardInfo} - Controller: {controllerInfo}");
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