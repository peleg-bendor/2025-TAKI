using UnityEngine;
using TMPro;

namespace TakiGame {
	/// <summary>
	/// Handles all UI updates related to deck display
	/// NO game logic, NO deck operations, NO resource management
	/// </summary>
	public class DeckUIManager : MonoBehaviour {

		[Header ("UI References")]
		[Tooltip ("Text showing number of cards in draw pile")]
		public TextMeshProUGUI drawPileCountText;

		[Tooltip ("Text showing number of cards in discard pile")]
		public TextMeshProUGUI discardPileCountText;

		[Tooltip ("Text for game messages and feedback")]
		public TextMeshProUGUI gameMessageText;

		[Tooltip ("Panel that shows the draw pile")]
		public Transform drawPilePanel;

		[Tooltip ("Panel that shows the discard pile")]
		public Transform discardPilePanel;

		[Header ("UI Settings")]
		[Tooltip ("How long to display temporary messages")]
		public float messageDisplayTime = 3.0f;

		// Message management
		private float messageTimer = 0f;
		private bool hasTemporaryMessage = false;

		/// <summary>
		/// Update the deck count displays
		/// </summary>
		/// <param name="drawCount">Number of cards in draw pile</param>
		/// <param name="discardCount">Number of cards in discard pile</param>
		public void UpdateDeckUI (int drawCount, int discardCount) {
			if (drawPileCountText != null) {
				drawPileCountText.text = $"Draw: {drawCount}";
			}

			if (discardPileCountText != null) {
				discardPileCountText.text = $"Discard: {discardCount}";
			}
		}

		/// <summary>
		/// Show a temporary message to the player
		/// </summary>
		/// <param name="message">Message to display</param>
		/// <param name="isTemporary">If true, message will auto-clear after time</param>
		public void ShowMessage (string message, bool isTemporary = true) {
			if (gameMessageText != null) {
				gameMessageText.text = message;
			}

			Debug.Log ($"Game Message: {message}");

			if (isTemporary) {
				hasTemporaryMessage = true;
				messageTimer = messageDisplayTime;
			} else {
				hasTemporaryMessage = false;
			}
		}

		/// <summary>
		/// Clear the current message
		/// </summary>
		public void ClearMessage () {
			if (gameMessageText != null) {
				gameMessageText.text = "";
			}
			hasTemporaryMessage = false;
			messageTimer = 0f;
		}

		/// <summary>
		/// Show permanent status message (doesn't auto-clear)
		/// </summary>
		/// <param name="message">Status message to display</param>
		public void ShowStatusMessage (string message) {
			ShowMessage (message, false);
		}

		/// <summary>
		/// Update the visual representation of the top discard card
		/// TODO: Implement in Milestone 6 with card prefabs
		/// </summary>
		/// <param name="topCard">The card currently on top of discard pile</param>
		public void UpdateDiscardPileDisplay (CardData topCard) {
			// For now, just show the card info in the message
			// Later this will update a card prefab visual
			if (topCard != null) {
				Debug.Log ($"Top discard card: {topCard.GetDisplayText ()}");
			}
		}

		/// <summary>
		/// Handle UI updates for deck events
		/// </summary>
		void Update () {
			// Handle temporary message timer
			if (hasTemporaryMessage && messageTimer > 0f) {
				messageTimer -= Time.deltaTime;
				if (messageTimer <= 0f) {
					ClearMessage ();
				}
			}
		}

		/// <summary>
		/// Show loading message
		/// </summary>
		public void ShowLoadingMessage () {
			ShowStatusMessage ("Loading deck...");
		}

		/// <summary>
		/// Show deck loaded message
		/// </summary>
		/// <param name="cardCount">Number of cards loaded</param>
		public void ShowDeckLoadedMessage (int cardCount) {
			ShowMessage ($"Deck loaded: {cardCount} cards ready!", true);
		}

		/// <summary>
		/// Show deck initialization message
		/// </summary>
		public void ShowDeckInitializedMessage () {
			ShowMessage ("New deck shuffled and ready!", true);
		}

		/// <summary>
		/// Show reshuffle message
		/// </summary>
		public void ShowReshuffleMessage () {
			ShowMessage ("Reshuffled discard pile into draw pile!", true);
		}

		/// <summary>
		/// Show error message
		/// </summary>
		/// <param name="errorMessage">Error to display</param>
		public void ShowErrorMessage (string errorMessage) {
			ShowMessage ($"ERROR: {errorMessage}", true);
		}

		/// <summary>
		/// Show game start message
		/// </summary>
		/// <param name="startingCard">The first card placed</param>
		public void ShowGameStartMessage (CardData startingCard) {
			if (startingCard != null) {
				ShowMessage ($"Game started! First card: {startingCard.GetDisplayText ()}", true);
			} else {
				ShowMessage ("Game started!", true);
			}
		}

		// Validation methods to check if UI elements are assigned
		public bool HasRequiredUIElements () {
			bool hasElements = drawPileCountText != null && discardPileCountText != null;
			if (!hasElements) {
				Debug.LogWarning ("Missing required UI elements in DeckUIManager");
			}
			return hasElements;
		}
	}
}