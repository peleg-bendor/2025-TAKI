using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TakiGame {
	/// <summary>
	/// Handles UI updates for gameplay - MATCHES Scene Hierarchy exactly
	/// NO timer UI (not needed), proper separation of UI ownership
	/// </summary>
	public class GameplayUIManager : MonoBehaviour {

		[Header ("Turn Display")]
		[Tooltip ("TurnIndicatorText - shows whose turn it is")]
		public TextMeshProUGUI turnIndicatorText;

		[Tooltip ("CurrentColorIndicator - Image showing active color")]
		public Image currentColorIndicator;

		[Header ("Player Action Buttons")]
		[Tooltip ("Btn_Player1PlayCard - play selected card")]
		public Button playCardButton;

		[Tooltip ("Btn_Player1DrawCard - draw a card")]
		public Button drawCardButton;

		[Tooltip ("Btn_Player1EndTurn - end current turn")]
		public Button endTurnButton;

		[Tooltip ("Btn_Pause - pause game")]
		public Button pauseButton;

		[Header ("Hand Size Displays")]
		[Tooltip ("Player1HandSizeText - human player hand size")]
		public TextMeshProUGUI player1HandSizeText;

		[Tooltip ("Player2HandSizeText - computer player hand size")]
		public TextMeshProUGUI player2HandSizeText;

		[Header ("Computer Feedback")]
		[Tooltip ("Player2MessageText - shows computer actions")]
		public TextMeshProUGUI player2MessageText;

		[Header ("Color Selection")]
		[Tooltip ("ColorSelectionPanel - show/hide for color selection")]
		public GameObject colorSelectionPanel;

		[Tooltip ("Color selection buttons")]
		public Button selectRedButton;
		public Button selectBlueButton;
		public Button selectGreenButton;
		public Button selectYellowButton;

		[Header ("Color Settings")]
		[Tooltip ("Colors to use for current color indicator")]
		public Color redColor = Color.red;
		public Color blueColor = Color.blue;
		public Color greenColor = Color.green;
		public Color yellowColor = Color.yellow;
		public Color wildColor = Color.white;

		// Events for external systems
		public System.Action OnPlayCardClicked;
		public System.Action OnDrawCardClicked;
		public System.Action OnEndTurnClicked;
		public System.Action<CardColor> OnColorSelected;

		// UI state tracking
		private bool buttonsEnabled = false;

		void Start () {
			ConnectButtonEvents ();
			SetupInitialState ();
		}

		/// <summary>
		/// Connect button events to internal handlers
		/// </summary>
		void ConnectButtonEvents () {
			if (playCardButton != null) {
				playCardButton.onClick.AddListener (() => OnPlayCardClicked?.Invoke ());
			}

			if (drawCardButton != null) {
				drawCardButton.onClick.AddListener (() => OnDrawCardClicked?.Invoke ());
			}

			if (endTurnButton != null) {
				endTurnButton.onClick.AddListener (() => OnEndTurnClicked?.Invoke ());
			}

			// Color selection buttons
			if (selectRedButton != null) {
				selectRedButton.onClick.AddListener (() => SelectColor (CardColor.Red));
			}

			if (selectBlueButton != null) {
				selectBlueButton.onClick.AddListener (() => SelectColor (CardColor.Blue));
			}

			if (selectGreenButton != null) {
				selectGreenButton.onClick.AddListener (() => SelectColor (CardColor.Green));
			}

			if (selectYellowButton != null) {
				selectYellowButton.onClick.AddListener (() => SelectColor (CardColor.Yellow));
			}
		}

		/// <summary>
		/// Setup initial UI state
		/// </summary>
		void SetupInitialState () {
			// Hide color selection initially
			if (colorSelectionPanel != null) {
				colorSelectionPanel.SetActive (false);
			}

			// Set initial color indicator
			if (currentColorIndicator != null) {
				currentColorIndicator.color = wildColor;
			}

			// Disable buttons initially
			UpdateButtonStates (false);
		}

		/// <summary>
		/// Update turn display using new architecture
		/// </summary>
		/// <param name="turnState">Current turn state</param>
		public void UpdateTurnDisplay (TurnState turnState) {
			if (turnIndicatorText != null) {
				string turnMessage = GetTurnMessage (turnState);
				turnIndicatorText.text = turnMessage;
			}

			// Update button states based on turn state
			UpdateButtonStates (turnState == TurnState.PlayerTurn);
		}

		/// <summary>
		/// Get turn message from turn state
		/// </summary>
		string GetTurnMessage (TurnState turnState) {
			switch (turnState) {
				case TurnState.PlayerTurn:
					return "Your Turn";
				case TurnState.ComputerTurn:
					return "Computer's Turn";
				case TurnState.Neutral:
					return "Game Setup";
				default:
					return "Unknown State";
			}
		}

		/// <summary>
		/// Update current color indicator
		/// </summary>
		/// <param name="activeColor">Current active color</param>
		public void UpdateActiveColorDisplay (CardColor activeColor) {
			if (currentColorIndicator != null) {
				currentColorIndicator.color = GetColorForCardColor (activeColor);
			}
		}

		/// <summary>
		/// Update hand size displays
		/// </summary>
		/// <param name="player1HandSize">Player 1 (human) hand size</param>
		/// <param name="player2HandSize">Player 2 (computer) hand size</param>
		public void UpdateHandSizeDisplay (int player1HandSize, int player2HandSize) {
			if (player1HandSizeText != null) {
				player1HandSizeText.text = $"Your Cards: {player1HandSize}";
			}

			if (player2HandSizeText != null) {
				player2HandSizeText.text = $"Computer Cards: {player2HandSize}";
			}
		}

		/// <summary>
		/// Show computer action message
		/// </summary>
		/// <param name="message">Message to show</param>
		public void ShowComputerMessage (string message) {
			if (player2MessageText != null) {
				player2MessageText.text = message;
			}
		}

		/// <summary>
		/// Show/hide color selection panel
		/// </summary>
		/// <param name="show">Whether to show the color selection</param>
		public void ShowColorSelection (bool show) {
			if (colorSelectionPanel != null) {
				colorSelectionPanel.SetActive (show);
			}

			// Disable other buttons during color selection
			if (show) {
				UpdateButtonStates (false);
			}
		}

		/// <summary>
		/// Handle color selection
		/// </summary>
		/// <param name="selectedColor">Selected color</param>
		void SelectColor (CardColor selectedColor) {
			// Hide color selection panel
			ShowColorSelection (false);

			// Update color indicator
			UpdateActiveColorDisplay (selectedColor);

			// Notify external systems
			OnColorSelected?.Invoke (selectedColor);

			Debug.Log ($"Player selected color: {selectedColor}");
		}

		/// <summary>
		/// Enable or disable action buttons
		/// </summary>
		/// <param name="enabled">Whether buttons should be enabled</param>
		public void UpdateButtonStates (bool enabled) {
			buttonsEnabled = enabled;

			if (playCardButton != null) {
				playCardButton.interactable = enabled;
			}

			if (drawCardButton != null) {
				drawCardButton.interactable = enabled;
			}

			if (endTurnButton != null) {
				endTurnButton.interactable = enabled;
			}

			// Pause button should always be available
			if (pauseButton != null) {
				pauseButton.interactable = true;
			}
		}

		/// <summary>
		/// Show winner announcement
		/// </summary>
		/// <param name="winner">Winning player type</param>
		public void ShowWinnerAnnouncement (PlayerType winner) {
			string winnerText = winner == PlayerType.Human ?
								"You Win!" : "Computer Wins!";

			if (turnIndicatorText != null) {
				turnIndicatorText.text = winnerText;
			}

			ShowComputerMessage ("Game Over");

			// Disable all action buttons
			UpdateButtonStates (false);
		}

		/// <summary>
		/// Reset UI for new game
		/// </summary>
		public void ResetUIForNewGame () {
			if (turnIndicatorText != null) {
				turnIndicatorText.text = "New Game";
			}

			ShowComputerMessage ("Starting new game...");
			UpdateActiveColorDisplay (CardColor.Wild);
			UpdateHandSizeDisplay (0, 0);
			ShowColorSelection (false);
			UpdateButtonStates (false);
		}

		/// <summary>
		/// Convert CardColor to Unity Color
		/// </summary>
		Color GetColorForCardColor (CardColor cardColor) {
			switch (cardColor) {
				case CardColor.Red:
					return redColor;
				case CardColor.Blue:
					return blueColor;
				case CardColor.Green:
					return greenColor;
				case CardColor.Yellow:
					return yellowColor;
				default:
					return wildColor;
			}
		}

		/// <summary>
		/// Update all displays using new multi-enum architecture
		/// </summary>
		/// <param name="turnState">Current turn state</param>
		/// <param name="gameStatus">Current game status</param>
		/// <param name="interactionState">Current interaction state</param>
		/// <param name="activeColor">Current active color</param>
		public void UpdateAllDisplays (TurnState turnState, GameStatus gameStatus, InteractionState interactionState, CardColor activeColor) {
			UpdateTurnDisplay (turnState);
			UpdateActiveColorDisplay (activeColor);

			// Handle special interaction states
			if (interactionState == InteractionState.ColorSelection) {
				ShowColorSelection (true);
			} else {
				ShowColorSelection (false);
			}

			// Show game status in computer message area
			if (gameStatus != GameStatus.Active) {
				string statusMessage = gameStatus == GameStatus.GameOver ? "Game Over" : "Game Paused";
				ShowComputerMessage (statusMessage);
			}
		}

		// Properties
		public bool ButtonsEnabled => buttonsEnabled;
		public bool IsColorSelectionActive => colorSelectionPanel != null && colorSelectionPanel.activeSelf;
	}
}