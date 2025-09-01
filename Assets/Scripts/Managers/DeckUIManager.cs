using UnityEngine;
using TMPro;

namespace TakiGame {
	/// <summary>
	/// Handles ONLY deck-related UI updates - NO conflicts with GameplayUIManager
	/// NOW INCLUDES pile visual management through PileManager integration
	/// Focuses on DrawPileCountText, DiscardPileCountText, DeckMessageText for deck events only
	/// </summary>
	public class DeckUIManager : MonoBehaviour {

		[Header ("Deck Count UI")]
		[Tooltip ("DrawPileCountText - shows number of cards in draw pile")]
		public TextMeshProUGUI drawPileCountText;

		[Tooltip ("DiscardPileCountText - shows number of cards in discard pile")]
		public TextMeshProUGUI discardPileCountText;

		[Header ("Deck Event Messages")]
		[Tooltip ("DeckMessageText - ONLY for deck-specific events (loading, shuffling, etc.)")]
		public TextMeshProUGUI deckMessageText;

		[Header ("Visual Pile Management")]
		[Tooltip ("PileManager component for visual pile cards")]
		public PileManager pileManager;

		[Header ("Deck Visual Panels")]
		[Tooltip ("DrawPilePanel - visual container for draw pile")]
		public Transform drawPilePanel;

		[Tooltip ("DiscardPilePanel - visual container for discard pile")]
		public Transform discardPilePanel;

		[Header ("Message Settings")]
		[Tooltip ("How long to display temporary deck messages")]
		public float messageDisplayTime = 2.0f;

		// Message management
		private float messageTimer = 0f;
		private bool hasTemporaryMessage = false;
		private string originalMessage = "";

		void Start () {
			// Ensure PileManager is connected
			if (pileManager == null) {
				pileManager = GetComponent<PileManager> ();
				if (pileManager == null) {
					TakiLogger.LogWarning ("DeckUIManager: PileManager not found! Pile visuals will not work.", TakiLogger.LogCategory.Deck);
				}
			}
		}

		/// <summary>
		/// Update the deck count displays AND pile visuals
		/// </summary>
		/// <param name="drawCount">Number of cards in draw pile</param>
		/// <param name="discardCount">Number of cards in discard pile</param>
		public void UpdateDeckUI (int drawCount, int discardCount) {
			// Update text counters
			if (drawPileCountText != null) {
				drawPileCountText.text = $"Draw: {drawCount}";
			}

			if (discardPileCountText != null) {
				discardPileCountText.text = $"Discard: {discardCount}";
			}

			// Update visual piles
			if (pileManager != null) {
				pileManager.UpdateDrawPileDisplay (drawCount);
			}
		}
		/// <summary>
		/// Show deck-specific message (loading, shuffling, errors)
		/// NOTE: Does NOT interfere with gameplay messages - only deck events
		/// </summary>
		/// <param name="message">Deck message to display</param>
		/// <param name="isTemporary">If true, message will auto-clear</param>
		public void ShowDeckMessage (string message, bool isTemporary = true) {
			if (deckMessageText != null) {
				// Store original message if we're showing a temporary one
				if (isTemporary && !hasTemporaryMessage) {
					originalMessage = deckMessageText.text;
				}

				deckMessageText.text = message;
			}

			TakiLogger.LogDeck ($"Deck Message: {message}");

			if (isTemporary) {
				hasTemporaryMessage = true;
				messageTimer = messageDisplayTime;
			} else {
				hasTemporaryMessage = false;
				originalMessage = message; // Update the "permanent" message
			}
		}

		/// <summary>
		/// Clear temporary deck message and restore previous
		/// </summary>
		void ClearTemporaryMessage () {
			if (deckMessageText != null) {
				deckMessageText.text = originalMessage;
			}
			hasTemporaryMessage = false;
			messageTimer = 0f;
		}

		/// <summary>
		/// Handle message timing
		/// </summary>
		void Update () {
			// Handle temporary message timer
			if (hasTemporaryMessage && messageTimer > 0f) {
				messageTimer -= Time.deltaTime;
				if (messageTimer <= 0f) {
					ClearTemporaryMessage ();
				}
			}
		}

		// ===== DECK-SPECIFIC EVENT MESSAGES =====
		// These methods are called by DeckManager for specific deck events

		/// <summary>
		/// Show loading message
		/// </summary>
		public void ShowLoadingMessage () {
			ShowDeckMessage ("Loading deck...", false);
		}

		/// <summary>
		/// Show deck loaded message
		/// </summary>
		/// <param name="cardCount">Number of cards loaded</param>
		public void ShowDeckLoadedMessage (int cardCount) {
			ShowDeckMessage ($"Deck loaded: {cardCount} cards", true);
		}

		/// <summary>
		/// Show deck initialization message
		/// </summary>
		public void ShowDeckInitializedMessage () {
			ShowDeckMessage ("New deck shuffled!", true);
		}

		/// <summary>
		/// Show reshuffle message
		/// </summary>
		public void ShowReshuffleMessage () {
			ShowDeckMessage ("Reshuffled discard pile!", true);
		}

		/// <summary>
		/// Show deck error message
		/// </summary>
		/// <param name="errorMessage">Error to display</param>
		public void ShowDeckErrorMessage (string errorMessage) {
			ShowDeckMessage ($"ERROR: {errorMessage}", true);
		}

		/// <summary>
		/// Show game start message (from deck perspective)
		/// </summary>
		/// <param name="startingCard">The first card placed</param>
		public void ShowGameStartMessage (CardData startingCard) {
			if (startingCard != null) {
				ShowDeckMessage ($"Starting card: {startingCard.GetDisplayText ()}", true);
			} else {
				ShowDeckMessage ("Game started!", true);
			}
		}

		/// <summary>
		/// Update discard pile visual with current top card
		/// NOW FUNCTIONAL - Updates both text and visual pile
		/// </summary>
		/// <param name="topCard">Current top card</param>
		public void UpdateDiscardPileDisplay (CardData topCard) {
			// Update visual pile through PileManager
			if (pileManager != null) {
				pileManager.UpdateDiscardPileDisplay (topCard);
			} else {
				TakiLogger.LogWarning ("DeckUIManager: Cannot update discard pile visual - PileManager not assigned!", TakiLogger.LogCategory.Deck);
			}

			if (topCard != null) {
				TakiLogger.LogDeck ($"Top discard card updated: {topCard.GetDisplayText ()}", TakiLogger.LogLevel.Verbose);
			}
		}

		/// <summary>
		/// Set permanent message (won't be cleared by temporary messages)
		/// </summary>
		/// <param name="message">Permanent message</param>
		public void SetPermanentMessage (string message) {
			originalMessage = message;
			if (!hasTemporaryMessage && deckMessageText != null) {
				deckMessageText.text = message;
			}
		}

		/// <summary>
		/// Clear all messages
		/// </summary>
		public void ClearAllMessages () {
			if (deckMessageText != null) {
				deckMessageText.text = "";
			}
			originalMessage = "";
			hasTemporaryMessage = false;
			messageTimer = 0f;
		}

		/// <summary>
		/// Reset UI for new game - includes pile visuals
		/// </summary>
		public void ResetUIForNewGame () {
			// Reset text displays
			UpdateDeckUI (0, 0);
			ShowLoadingMessage ();

			// Reset pile visuals
			if (pileManager != null) {
				pileManager.ResetPiles ();
			}
		}

		/// <summary>
		/// Validate that required UI elements are assigned
		/// </summary>
		/// <returns>True if basic UI elements are present</returns>
		public bool HasRequiredUIElements () {
			bool hasElements = drawPileCountText != null && discardPileCountText != null;

			if (pileManager == null) {
				TakiLogger.LogWarning ("DeckUIManager: PileManager not assigned - pile visuals will not work!", TakiLogger.LogCategory.Deck);
			}

			if (!hasElements) {
				TakiLogger.LogWarning ("DeckUIManager: Missing required UI elements!", TakiLogger.LogCategory.UI);
			}
			return hasElements;
		}

		// Properties 
		public bool HasActiveMessage => hasTemporaryMessage;
		public string CurrentMessage => deckMessageText?.text ?? "";
		public bool HasPileManager => pileManager != null;
		public bool HasVisualPiles => pileManager?.HasDrawPileVisual == true || pileManager?.HasDiscardPileVisual == true;
	}
}