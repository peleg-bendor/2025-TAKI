using UnityEngine;
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