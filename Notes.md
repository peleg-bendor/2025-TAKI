```
using UnityEngine.UI;

namespace TakiGame {
	/// <summary>
	/// Handles individual card prefab behavior and visual representation
	/// FIXED: Uses correct image paths matching actual folder structure
	/// </summary>
	public class CardController : MonoBehaviour {

		[Header ("Visual Components")]
		[Tooltip ("Button component for card interaction")]
		public Button cardButton;

		[Tooltip ("Image for face-down display")]
		public Image cardBackImage;

		[Tooltip ("Image for face-up display")]
		public Image cardFrontImage;

		[Header ("Visual Settings")]
		[Tooltip ("Card dimensions - height in pixels")]
		public float cardHeight = 100f;

		[Tooltip ("Selection Y-offset when card is selected")]
		public float selectionOffset = 10f;

		[Tooltip ("Colors for visual feedback when selected")]
		public Color validCardTint = new Color (1f, 0.8f, 0f, 0.3f); // Gold overlay
		public Color invalidCardTint = new Color (1f, 0f, 0f, 0.3f);  // Red overlay

		// Events
		public System.Action<CardController> OnCardClicked;

		// Internal state
		private CardData cardData;
		private bool isSelected = false;
		private bool isFaceUp = true;
		private bool isPlayable = true;
		private Vector3 originalPosition;
		private HandManager parentHandManager;

		// Original card colors (for tint overlay)
		private Color originalFrontColor;
		private Color originalBackColor;

		void Awake () {
			// Set up card dimensions
			SetupCardDimensions ();

			// Connect button event
			if (cardButton != null) {
				cardButton.onClick.AddListener (OnCardButtonClicked);
			}

			// Store original position
			originalPosition = transform.localPosition;

			// Store original image colors
			if (cardFrontImage != null) {
				originalFrontColor = cardFrontImage.color;
			}
			if (cardBackImage != null) {
				originalBackColor = cardBackImage.color;
			}
		}

		/// <summary>
		/// Initialize card with CardData and parent hand manager
		/// </summary>
		/// <param name="data">CardData to represent</param>
		/// <param name="handManager">Parent HandManager</param>
		/// <param name="faceUp">Whether card should be face-up</param>
		public void InitializeCard (CardData data, HandManager handManager, bool faceUp = true) {
			cardData = data;
			parentHandManager = handManager;
			isFaceUp = faceUp;

			// Load actual scanned card images
			LoadCardImages ();

			// Set initial facing
			SetCardFacing (faceUp);

			// Reset selection state
			SetSelected (false);
			SetPlayable (true);

			Debug.Log ($"CardController initialized: {cardData?.GetDisplayText ()}");
		}

		/// <summary>
		/// Set up card dimensions based on cardHeight
		/// </summary>
		void SetupCardDimensions () {
			// Calculate width as (2/3) of height
			float cardWidth = cardHeight * (2f / 3f);

			// Apply to RectTransform
			RectTransform rectTransform = GetComponent<RectTransform> ();
			if (rectTransform != null) {
				rectTransform.sizeDelta = new Vector2 (cardWidth, cardHeight);
			}
		}

		/// <summary>
		/// Load actual scanned card images from Resources folder
		/// </summary>
		void LoadCardImages () {
			if (cardData == null) return;

			// Load card front image
			LoadCardFrontImage ();

			// Load card back image
			LoadCardBackImage ();
		}

		/// <summary>
		/// Load the front image for this specific card
		/// FIXED: Uses correct paths matching folder structure
		/// </summary>
		void LoadCardFrontImage () {
			if (cardFrontImage == null || cardData == null) return;

			string imagePath = GetCardFrontImagePath (cardData);
			Sprite cardSprite = Resources.Load<Sprite> (imagePath);

			if (cardSprite != null) {
				cardFrontImage.sprite = cardSprite;
				cardFrontImage.color = Color.white; // No tinting by default
				Debug.Log ($"Loaded card front image: {imagePath}");
			} else {
				Debug.LogWarning ($"Could not load card front image: {imagePath}");
				// Fallback to colored rectangle
				cardFrontImage.sprite = null;
				cardFrontImage.color = GetFallbackColor (cardData.color);
			}
		}

		/// <summary>
		/// Load the back image for cards
		/// FIXED: Uses correct path for card back
		/// </summary>
		void LoadCardBackImage () {
			if (cardBackImage == null) return;

			// FIXED: Use correct path based on actual folder structure
			string backImagePath = "Sprites/Cards/Backs/card_back";
			Sprite backSprite = Resources.Load<Sprite> (backImagePath);

			if (backSprite != null) {
				cardBackImage.sprite = backSprite;
				cardBackImage.color = Color.white;
				Debug.Log ($"Loaded card back image: {backImagePath}");
			} else {
				Debug.LogWarning ($"Could not load card back image: {backImagePath}");
				// Fallback to gray color
				cardBackImage.sprite = null;
				cardBackImage.color = Color.gray;
			}
		}

		/// <summary>
		/// Get the resource path for a card's front image
		/// SIMPLIFIED: All cards follow same folder pattern now
		/// </summary>
		/// <param name="card">CardData to get path for</param>
		/// <returns>Resource path string</returns>
		string GetCardFrontImagePath (CardData card) {
			// All cards follow same pattern: Sprites/Cards/Fronts/{Color}/
			string colorFolder = card.color.ToString ();

			if (card.cardType == CardType.Number) {
				// Number cards: Sprites/Cards/Fronts/{Color}/{number}_{color}
				string numberName = GetNumberName (card.number);
				string colorLower = card.color.ToString ().ToLower ();
				return $"Sprites/Cards/Fronts/{colorFolder}/{numberName}_{colorLower}";
			} else {
				// Special cards
				string specialName = GetSpecialCardName (card.cardType);

				if (card.color == CardColor.Wild) {
					// Wild cards: Sprites/Cards/Fronts/Wild/{specialName} (no color suffix)
					return $"Sprites/Cards/Fronts/{colorFolder}/{specialName}";
				} else {
					// Colored special cards: Sprites/Cards/Fronts/{Color}/{specialName}_{color}
					string colorLower = card.color.ToString ().ToLower ();
					return $"Sprites/Cards/Fronts/{colorFolder}/{specialName}_{colorLower}";
				}
			}
		}

		/// <summary>
		/// Convert number to word name for image files
		/// </summary>
		/// <param name="number">Card number</param>
		/// <returns>Word representation</returns>
		string GetNumberName (int number) {
			switch (number) {
				case 1: return "one";
				case 2: return "two";
				case 3: return "three";
				case 4: return "four";
				case 5: return "five";
				case 6: return "six";
				case 7: return "seven";
				case 8: return "eight";
				case 9: return "nine";
				default: return number.ToString ();
			}
		}

		/// <summary>
		/// Get special card name for image files
		/// </summary>
		/// <param name="cardType">Special card type</param>
		/// <returns>Image file name</returns>
		string GetSpecialCardName (CardType cardType) {
			switch (cardType) {
				case CardType.Plus:
					return "plus";
				case CardType.Stop:
					return "stop";
				case CardType.ChangeDirection:
					return "changeDirection";
				case CardType.ChangeColor:
					return "changeColor";
				case CardType.PlusTwo:
					return "plusTwo";
				case CardType.Taki:
					return "taki";
				case CardType.SuperTaki:
					return "superTaki";
				default:
					return cardType.ToString ().ToLower ();
			}
		}

		/// <summary>
		/// Get fallback color if image loading fails
		/// </summary>
		Color GetFallbackColor (CardColor cardColor) {
			switch (cardColor) {
				case CardColor.Red:
					return new Color (0.8f, 0.2f, 0.2f, 1f);
				case CardColor.Blue:
					return new Color (0.2f, 0.2f, 0.8f, 1f);
				case CardColor.Green:
					return new Color (0.2f, 0.8f, 0.2f, 1f);
				case CardColor.Yellow:
					return new Color (0.8f, 0.8f, 0.2f, 1f);
				case CardColor.Wild:
					return new Color (0.5f, 0.5f, 0.5f, 1f);
				default:
					return Color.white;
			}
		}

		/// <summary>
		/// Set card facing (face-up or face-down)
		/// </summary>
		/// <param name="faceUp">True for face-up, false for face-down</param>
		public void SetCardFacing (bool faceUp) {
			isFaceUp = faceUp;

			// Instant image swap - NO animations
			if (cardFrontImage != null) {
				cardFrontImage.gameObject.SetActive (faceUp);
			}

			if (cardBackImage != null) {
				cardBackImage.gameObject.SetActive (!faceUp);
			}

			// Disable interaction for face-down cards
			if (cardButton != null) {
				cardButton.interactable = faceUp;
			}
		}

		/// <summary>
		/// Set whether this card is currently selected - Set selection with proper visual feedback
		/// </summary>
		/// <param name="selected">Selection state</param>
		public void SetSelected (bool selected) {
			if (isSelected == selected) return; // No change needed

			isSelected = selected;

			// Move card up/down by selection offset
			Vector3 targetPosition = originalPosition;
			if (selected) {
				targetPosition.y += selectionOffset;
			}

			// Instant position change
			transform.localPosition = targetPosition;

			// Always update visual feedback when selection changes
			UpdateVisualFeedback ();

			Debug.Log ($"Card {cardData?.GetDisplayText ()} selection: {selected}, playable: {isPlayable}");
		}

		/// <summary>
		/// Debug method to test tint colors manually
		/// </summary>
		[ContextMenu ("Test Tint Colors")]
		public void TestTintColors () {
			Debug.Log ("=== TESTING TINT COLORS ===");
			Debug.Log ($"Card: {cardData?.GetDisplayText ()}");
			Debug.Log ($"Selected: {isSelected}");
			Debug.Log ($"Playable: {isPlayable}");
			Debug.Log ($"Face Up: {isFaceUp}");

			if (cardFrontImage != null) {
				Debug.Log ($"Front Image Color: {cardFrontImage.color}");
			}

			// Force visual feedback update
			UpdateVisualFeedback ();
			Debug.Log ("Tint test complete - check card appearance");
		}

		/// <summary>
		/// Force visual state refresh
		/// </summary>
		public void ForceVisualRefresh () {
			UpdateVisualFeedback ();
			Debug.Log ($"Visual refresh forced for {cardData?.GetDisplayText ()}");
		}

		/// <summary>
		/// Set whether this card is playable with immediate visual update
		/// </summary>
		/// <param name="playable">Whether card can be played</param>
		public void SetPlayable (bool playable) {
			if (isPlayable == playable) return; // No change needed

			isPlayable = playable;
			Debug.Log ($"Card {cardData?.GetDisplayText ()} playability set to: {playable}");

			// FIXED: Immediate visual feedback update
			UpdateVisualFeedback ();
		}

		/// <summary>
		/// Update visual feedback based on current state
		/// Only apply tinting when selected
		/// </summary>
		void UpdateVisualFeedback () {
			if (isSelected) {
				// FIXED: Properly check playable flag for tint color selection
				Color tintColor;
				if (isPlayable) {
					tintColor = validCardTint; // Gold for valid cards
					Debug.Log ($"Card {cardData?.GetDisplayText ()}: SELECTED + PLAYABLE = GOLD tint");
				} else {
					tintColor = invalidCardTint; // Red for invalid cards  
					Debug.Log ($"Card {cardData?.GetDisplayText ()}: SELECTED + INVALID = RED tint");
				}

				if (isFaceUp && cardFrontImage != null) {
					// Apply the correct tint
					cardFrontImage.color = Color.Lerp (Color.white, tintColor, tintColor.a);
				}
			} else {
				// FIXED: Remove tinting when not selected (always white)
				if (cardFrontImage != null) {
					cardFrontImage.color = Color.white;
				}
				if (cardBackImage != null) {
					cardBackImage.color = Color.white;
				}
				Debug.Log ($"Card {cardData?.GetDisplayText ()}: NOT SELECTED = WHITE (no tint)");
			}
		}

		/// <summary>
		/// Handle card button clicked
		/// </summary>
		void OnCardButtonClicked () {
			if (!isFaceUp) return; // Can't select face-down cards

			Debug.Log ($"Card clicked: {cardData?.GetDisplayText ()}");

			// Notify parent hand manager
			if (parentHandManager != null) {
				parentHandManager.HandleCardSelection (this);
			}

			// Fire event
			OnCardClicked?.Invoke (this);
		}

		/// <summary>
		/// Update original position (called by HandManager during layout)
		/// </summary>
		/// <param name="newPosition">New base position</param>
		public void UpdateOriginalPosition (Vector3 newPosition) {
			originalPosition = newPosition;

			// Update actual position if not selected
			if (!isSelected) {
				transform.localPosition = originalPosition;
			} else {
				// Maintain selection offset
				transform.localPosition = originalPosition + Vector3.up * selectionOffset;
			}
		}

		// Public Properties
		public CardData CardData => cardData;
		public bool IsSelected => isSelected;
		public bool IsFaceUp => isFaceUp;
		public bool IsPlayable => isPlayable;
		public Vector3 OriginalPosition => originalPosition;

		/// <summary>
		/// Clean up when card is destroyed
		/// </summary>
		void OnDestroy () {
			if (cardButton != null) {
				cardButton.onClick.RemoveAllListeners ();
			}
		}
	}
}
```


```
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
		/// Update which cards are playable with better logging and validation
		/// </summary>
		void UpdatePlayableStates () {
			if (!showFaceUpCards || gameManager == null) {
				Debug.Log ($"HandManager {gameObject.name}: Skipping playable update - not face-up or no GameManager");
				return;
			}

			// Get top discard card for rule validation
			CardData topCard = gameManager.GetTopDiscardCard ();
			if (topCard == null) {
				Debug.LogWarning ($"HandManager {gameObject.name}: Cannot update playable states - no top discard card");
				return;
			}

			Debug.Log ($"=== UPDATING PLAYABLE STATES for {gameObject.name} ===");
			Debug.Log ($"Top discard card: {topCard.GetDisplayText ()}");
			Debug.Log ($"Active color: {gameManager.gameState?.activeColor}");
			Debug.Log ($"Checking {cardControllers.Count} cards in hand");

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

					Debug.Log ($"  [{i}] {cardData.GetDisplayText ()} -> {(isPlayable ? "PLAYABLE" : "NOT PLAYABLE")}");
				} else {
					Debug.LogWarning ($"  [{i}] Missing controller or card data");
				}
			}

			Debug.Log ($"Playable state update complete: {playableCount}/{cardControllers.Count} cards playable");
		}

		/// <summary>
		/// Delayed playable state update (for timing issues)
		/// </summary>
		public void DelayedPlayableUpdate () {
			Debug.Log ($"HandManager {gameObject.name}: Performing delayed playable state update");
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
			Debug.Log ($"HandManager {gameObject.name}: Refreshing playable states (forced)");

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
```


```
namespace TakiGame {
	/// <summary>
	/// Card colors in TAKI game 
	/// </summary>
	public enum CardColor {
		Red,
		Blue,
		Green,
		Yellow,
		Wild        // For special cards that can be any color
	}

	/// <summary>
	/// Types of cards in TAKI game
	/// </summary>
	public enum CardType {
		Number,             // Regular numbered cards (1-9)
		Plus,               // +1 card to opponent
		Stop,               // Skip opponent's turn  
		ChangeDirection,    // Reverse turn direction
		ChangeColor,        // Choose new color
		PlusTwo,            // +2 cards to opponent
		Taki,               // Play multiple cards of same color
		SuperTaki           // Play multiple cards of any color
	}

	/// <summary>
	/// Player types in the game
	/// </summary>
	public enum PlayerType {
		Human,
		Computer
	}

	/// <summary>
	/// Whose turn is it currently?
	/// </summary>
	public enum TurnState {
		PlayerTurn,     // Human player's turn
		ComputerTurn,   // Computer player's turn
		Neutral         // During transitions, setup, or game over
	}

	/// <summary>
	/// What special interaction is currently happening?
	/// </summary>
	public enum InteractionState {
		Normal,         // Regular gameplay - play card or draw
		ColorSelection, // Player must choose color (ChangeColor card played)
		TakiSequence,   // TAKI sequence active - can play multiple cards
		PlusTwoChain    // PlusTwo chain active - can stack +2 cards
	}

	/// <summary>
	/// Overall status of the game
	/// </summary>
	public enum GameStatus {
		Active,     // Game is running normally
		Paused,     // Game paused by player
		GameOver    // Game ended - winner determined
	}

	/// <summary>
	/// Turn direction for the game
	/// </summary>
	public enum TurnDirection {
		Clockwise,
		CounterClockwise
	}
}
```


Go over each file, and make the changes needed, don't ignore checks they are important, the logs probably less so.
I will copy your scripts and compare them to the originals', so try to keep the structure as simular as you can.


---


Reorginized logs: BasicComputerAI, GameManager, DeckManager, DeckUIManager, GameStateManager, TurnManager, GameplayUIManager, TakiGameDiagnostics, TakiLogger.

Reorginized logs: CardData, Enums, TakiDeckGenerator, CardDataLoader, Deck, DontDestroyOnLoad, GameSetupManager, CardController, DifficultySlider, HandManager, MenuNavigation, PileManager, ButtonSFX, MusicSlider, SfxSlider.


---

- Update `TAKI Game - Complete Script Documentation & Reference Guide` with `TakiLogger.cs`.

- Update `TAKI Game Development Plan - Unity Engine` with:
	
	- Phase 5 done for now, `TakiLogger.cs` implemented, main files updated, no need for the rest at this time.

	- Next stop is `Phase 6`.

	- I updated the Scene Hierarchy:

```
Scene_Menu
├── Main Camera
├── Canvas
│   ├── Img_Background
│   ├── Screen_MainMenu
│   ├── Screen_StudentInfo
│   ├── Screen_Settings
│   ├── Screen_SinglePlayer
│   ├── Screen_MultiPlayer
│   ├── Screen_ExitValidator
│   ├── Screen_Loading
│   ├── Screen_Exiting
│   ├── Screen_SinglePlayerGame
│   │   ├── Player1Panel (Human Player)
│   │   │   ├── Player1HandPanel - (Components: HandManager)
│   │   │   └── Player1ActionPanel
│   │   │       ├── Btn_Player1PlayCard - Play selected card
│   │   │       ├── Btn_Player1DrawCard - Draw from deck
│   │   │       ├── Btn_Player1EndTurn - End current turn
│   │   │       └── Player1HandSizePanel
│   │   │           └── Player1HandSizeText - Hand size display
│   │   ├── Player2Panel (Computer Player)
│   │   │   ├── Player2HandPanel - (Components: HandManager)
│   │   │   └── Player2ActionPanel
│   │   │       ├── Player2MessageText - Computer actions and thinking
│   │   │       └── Player2HandSizePanel 
│   │   │           └── Player2HandSizeText - Computer hand size
│   │   ├── GameBoardPanel
│   │   │   ├── DrawPilePanel
│   │   │   │   └── DrawPileCountText - Draw pile count
│   │   │   └── DiscardPilePanel
│   │   │       └── DiscardPileCountText - Discard pile count
│   │   ├── GameInfoPanel
│   │   │   ├── TurnIndicatorText - Current turn display
│   │   │   ├── DeckMessageText - Deck event messages
│   │   │   └── GameMessageText - General game feedback
│   │   ├── ColorSelectionPanel - Color choice UI
│   │   │   ├── Btn_SelectRed
│   │   │   ├── Btn_SelectBlue
│   │   │   ├── Btn_SelectGreen
│   │   │   └── Btn_SelectYellow
│   │   ├── CurrentColorIndicator - Active color display
│   │   ├── Btn_Exit - Exit completely (not return to Main Menu)
│   │   ├── Btn_Pause - Pause functionality
│   │   └── Screen_GameEnd - Game over popup
│   │       ├── GameEndMessage - Winner announcement
│   │       ├── Btn_PlayAgain - Start new game
│   │       └── Btn_ReturnToMenu - Back to main menu
│   └── Screen_MultiPlayerGame
├── EventSystem
├── GameObject
├── MenuManager
├── BackgroundMusic
├── SFXController
└── GameManager
```



