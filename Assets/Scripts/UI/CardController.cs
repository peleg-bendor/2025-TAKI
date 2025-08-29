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
		/// FIXED: Matches actual folder structure from your images
		/// </summary>
		/// <param name="card">CardData to get path for</param>
		/// <returns>Resource path string</returns>
		string GetCardFrontImagePath (CardData card) {
			// Based on your actual folder structure:
			// Resources/Sprites/Cards/Fronts/Blue, Green, Red, Special, Yellow

			if (card.cardType == CardType.Number) {
				// Number cards: Sprites/Cards/Fronts/{Color}/{number}_{color}
				string colorFolder = card.color.ToString ();
				string numberName = GetNumberName (card.number);
				string colorLower = card.color.ToString ().ToLower ();
				return $"Sprites/Cards/Fronts/{colorFolder}/{numberName}_{colorLower}";
			} else {
				// Special cards
				if (card.color == CardColor.Wild) {
					// Wild cards go in Special folder
					string specialName = GetSpecialCardName (card.cardType);
					return $"Sprites/Cards/Fronts/Special/{specialName}";
				} else {
					// Colored special cards go in color folders
					string colorFolder = card.color.ToString ();
					string specialName = GetSpecialCardName (card.cardType);
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
					return "changeDirection"; // Based on "direction_blue" seen in your structure
				case CardType.ChangeColor:
					return "changeColor_Wild";
				case CardType.PlusTwo:
					return "plusTwo";
				case CardType.Taki:
					return "taki";
				case CardType.SuperTaki:
					return "taki_Wild";
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
		/// Set whether this card is currently selected
		/// </summary>
		/// <param name="selected">Selection state</param>
		public void SetSelected (bool selected) {
			if (isSelected == selected) return;

			isSelected = selected;

			// Move card up/down by selection offset
			Vector3 targetPosition = originalPosition;
			if (selected) {
				targetPosition.y += selectionOffset;
			}

			// Instant position change - NO animations
			transform.localPosition = targetPosition;

			// Update visual feedback
			UpdateVisualFeedback ();

			Debug.Log ($"Card {cardData?.GetDisplayText ()} selection: {selected}");
		}

		/// <summary>
		/// Set whether this card is playable (affects visual feedback)
		/// </summary>
		/// <param name="playable">Whether card can be played</param>
		public void SetPlayable (bool playable) {
			isPlayable = playable;
			UpdateVisualFeedback ();
		}

		/// <summary>
		/// Update visual feedback based on current state
		/// Only apply tinting when selected
		/// </summary>
		void UpdateVisualFeedback () {
			if (isSelected) {
				// Apply tint overlay when selected
				Color tintColor = isPlayable ? validCardTint : invalidCardTint;

				if (isFaceUp && cardFrontImage != null) {
					// Blend tint with white (since images should be white by default)
					cardFrontImage.color = Color.Lerp (Color.white, tintColor, tintColor.a);
				}
			} else {
				// Remove tinting when not selected
				if (cardFrontImage != null) {
					cardFrontImage.color = Color.white;
				}
				if (cardBackImage != null) {
					cardBackImage.color = Color.white;
				}
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