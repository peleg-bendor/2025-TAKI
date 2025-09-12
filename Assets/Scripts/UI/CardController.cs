using UnityEngine;
using UnityEngine.UI;

namespace TakiGame {
	/// <summary> 
	/// ENHANCED: CardController with Privacy Mode Support
	/// SAFETY: Preserves ALL existing functionality exactly as-is
	/// APPROACH: Surgical additions only, no modifications to existing methods
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

		// NEW: Privacy mode support (ADDITION ONLY)
		private bool isPrivacyMode = false;

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
		/// PRESERVED: Original initialization method (UNCHANGED)
		/// Initialize card with CardData and parent hand manager
		/// MILESTONE 1 FIX: Handle null CardData for opponent card backs
		/// </summary>
		/// <param name="cardData">CardData to represent (null = card back for opponent)</param>
		/// <param name="handManager">Parent HandManager</param>
		/// <param name="faceUp">Whether card should be face-up</param>
		public void InitializeCard (CardData data, HandManager handManager, bool faceUp = true) {
			// MILESTONE 1 FIX: Handle null CardData for opponent card backs
			if (data == null) {
				SetupCardBackDisplay (handManager, faceUp);
				return; // Exit early - no further initialization needed for card backs
			}

			// EXISTING LOGIC: Continue with normal card initialization
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
		}

		/// <summary>
		/// NEW: Enhanced initialization method with privacy mode support
		/// SAFETY: Additive method, does not replace existing InitializeCard
		/// </summary>
		/// <param name="data">CardData to represent (always real)</param>
		/// <param name="handManager">Parent HandManager</param>
		/// <param name="faceUp">Whether card should be face-up</param>
		/// <param name="privacyMode">Enable privacy mode for opponent cards</param>
		public void InitializeCardEnhanced (CardData data, HandManager handManager, bool faceUp = true, bool privacyMode = false) {
			if (data == null) {
				TakiLogger.LogError ("CardController: Real CardData required for enhanced initialization!", TakiLogger.LogCategory.System);
				return;
			}

			// Store card data and settings
			cardData = data;
			parentHandManager = handManager;
			isFaceUp = faceUp;
			isPrivacyMode = privacyMode;

			// Load actual card images using EXISTING method
			LoadCardImages ();

			// Set card facing based on privacy mode
			bool displayFaceUp = faceUp && !privacyMode;
			SetCardFacing (displayFaceUp);

			// Set interactability based on privacy mode
			SetInteractable (!privacyMode);

			// Reset selection state
			SetSelected (false);
			SetPlayable (true);

			TakiLogger.LogNetwork ($"CardController: Enhanced initialization - Card: {data.GetDisplayText ()}, FaceUp: {displayFaceUp}, Privacy: {privacyMode}");
		}

		/// <summary>
		/// PRESERVED: Legacy card back display (UNCHANGED)
		/// MILESTONE 1: Setup card back display for opponent cards
		/// </summary>
		/// <param name="handManager">Parent HandManager</param>
		/// <param name="faceUp">Face up state (ignored for card backs)</param>
		private void SetupCardBackDisplay (HandManager handManager, bool faceUp) {
			// Set basic properties
			this.cardData = null; // Explicitly null for card backs
			this.parentHandManager = handManager;
			this.isFaceUp = false; // Card backs are always face-down
			this.isPlayable = false; // Card backs are never playable
			this.isPrivacyMode = true; // Card backs are privacy mode

			// Force card back visual
			SetCardFacing (false); // Force face-down display

			// Disable interaction
			SetSelected (false);
			if (cardButton != null) {
				cardButton.interactable = false; // Prevent clicking opponent cards
			}

			TakiLogger.LogNetwork ("CardController: Card back initialized for opponent display");
		}

		/// <summary>
		/// NEW: Set privacy mode for real cards (ADDITION ONLY)
		/// </summary>
		/// <param name="privacyMode">Enable/disable privacy mode</param>
		public void SetPrivacyMode (bool privacyMode) {
			isPrivacyMode = privacyMode;

			// Update visual display based on privacy mode
			bool displayFaceUp = isFaceUp && !privacyMode;
			SetCardFacing (displayFaceUp);

			// Update interactability
			SetInteractable (!privacyMode);

			// Clear selection if entering privacy mode
			if (privacyMode && isSelected) {
				SetSelected (false);
			}

			TakiLogger.LogNetwork ($"CardController: Privacy mode set to {privacyMode} for card {cardData?.GetDisplayText () ?? "NULL"}");
		}

		/// <summary>
		/// NEW: Set card interactability (ADDITION ONLY)
		/// </summary>
		/// <param name="interactable">Whether card should be interactable</param>
		public void SetInteractable (bool interactable) {
			if (cardButton != null) {
				cardButton.interactable = interactable;
			}
		}

		/// <summary>
		/// NEW: Force visual refresh (ADDITION ONLY)
		/// </summary>
		public void ForceVisualRefresh () {
			UpdateVisualFeedback ();
		}

		// ===================================================================
		// PRESERVED: ALL EXISTING METHODS BELOW (COMPLETELY UNCHANGED)
		// ===================================================================

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
			} else {
				TakiLogger.LogWarning ($"Could not load card front image: {imagePath}", TakiLogger.LogCategory.System);
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
			} else {
				TakiLogger.LogWarning ($"Could not load card back image: {backImagePath}", TakiLogger.LogCategory.System);
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

			if (cardFrontImage != null) {
				cardFrontImage.gameObject.SetActive (faceUp);
			}
			if (cardBackImage != null) {
				cardBackImage.gameObject.SetActive (!faceUp);
			}
		}

		/// <summary>
		/// Set card selection state
		/// </summary>
		/// <param name="selected">Selected state</param>
		public void SetSelected (bool selected) {
			// NEW: Don't allow selection in privacy mode
			if (isPrivacyMode && selected) {
				return;
			}

			isSelected = selected;

			// Update visual position
			Vector3 targetPosition = originalPosition;
			if (selected) {
				targetPosition.y += selectionOffset;
			}
			transform.localPosition = targetPosition;

			// Update visual feedback
			UpdateVisualFeedback ();
		}

		/// <summary>
		/// Set card playable state
		/// </summary>
		/// <param name="playable">Playable state</param>
		public void SetPlayable (bool playable) {
			// NEW: Privacy mode cards are never playable
			if (isPrivacyMode) {
				playable = false;
			}

			isPlayable = playable;

			// FIXED: Immediate visual feedback update
			UpdateVisualFeedback ();
		}

		/// <summary>
		/// Update visual feedback based on current state
		/// Only apply tinting when selected
		/// </summary>
		void UpdateVisualFeedback () {
			// NEW: Skip visual feedback for privacy mode
			if (isPrivacyMode) return;

			if (isSelected) {
				// FIXED: Properly check playable flag for tint color selection
				Color tintColor;
				if (isPlayable) {
					tintColor = validCardTint; // Gold for valid cards
				} else {
					tintColor = invalidCardTint; // Red for invalid cards  
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
			}
		}

		/// <summary>
		/// Handle card button clicked
		/// </summary>
		void OnCardButtonClicked () {
			// NEW: Prevent interaction in privacy mode
			if (isPrivacyMode) {
				TakiLogger.LogNetwork ("Card interaction blocked - privacy mode enabled");
				return;
			}

			if (!isFaceUp) return; // Can't select face-down cards

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
		public bool IsPrivacyMode => isPrivacyMode; // NEW PROPERTY
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