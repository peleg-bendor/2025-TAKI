using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TakiGame {
	/// <summary>
	/// Handles UI updates for gameplay - MATCHES Scene Hierarchy exactly
	/// NO timer UI (not needed), proper separation of UI ownership
	/// ENHANCED GameplayUIManager with STRICT BUTTON CONTROL SYSTEM
	/// - Supports the strict turn flow: Action -> END TURN
	/// - Precise button state control for each phase of turn
	/// - Enhanced logging for turn flow debugging
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

		// ENHANCED: Strict button state tracking
		private bool playButtonEnabled = false;
		private bool drawButtonEnabled = false;
		private bool endTurnButtonEnabled = false;

		//// UI state tracking
		//private bool buttonsEnabled = false;

		void Start () {
			ConnectButtonEvents ();
			SetupInitialState ();
		}

		/// <summary>
		/// Connect button events to internal handlers - with validation
		/// </summary>
		void ConnectButtonEvents () {
			Debug.Log ("Connecting button events with STRICT FLOW validation...");

			if (playCardButton != null) {
				playCardButton.onClick.AddListener (() => {
					Debug.Log ("=== PLAY CARD BUTTON CLICKED ===");
					Debug.Log ($"Button enabled state: {playButtonEnabled}");
					Debug.Log ($"Button interactable: {playCardButton.interactable}");

					if (!playButtonEnabled) {
						Debug.LogWarning ("PLAY CARD clicked but button should be disabled!");
						ShowComputerMessage ("Cannot play card right now!");
						return;
					}

					OnPlayCardClicked?.Invoke ();
				});
				Debug.Log ("Play Card button event connected");
			} else {
				Debug.LogError ("Play Card button is NULL!");
			}

			if (drawCardButton != null) {
				drawCardButton.onClick.AddListener (() => {
					Debug.Log ("=== DRAW CARD BUTTON CLICKED ===");
					Debug.Log ($"Button enabled state: {drawButtonEnabled}");
					Debug.Log ($"Button interactable: {drawCardButton.interactable}");

					if (!drawButtonEnabled) {
						Debug.LogWarning ("DRAW CARD clicked but button should be disabled!");
						ShowComputerMessage ("Cannot draw card right now!");
						return;
					}

					OnDrawCardClicked?.Invoke ();
				});
				Debug.Log ("Draw Card button event connected");
			} else {
				Debug.LogError ("Draw Card button is NULL!");
			}

			if (endTurnButton != null) {
				endTurnButton.onClick.AddListener (() => {
					Debug.Log ("=== END TURN BUTTON CLICKED ===");
					Debug.Log ($"Button enabled state: {endTurnButtonEnabled}");
					Debug.Log ($"Button interactable: {endTurnButton.interactable}");

					if (!endTurnButtonEnabled) {
						Debug.LogWarning ("END TURN clicked but button should be disabled!");
						ShowComputerMessage ("You must take an action first!");
						return;
					}

					// Immediately disable all buttons to prevent multiple clicks
					Debug.Log ("IMMEDIATELY disabling all buttons after END TURN click");
					UpdateStrictButtonStates (false, false, false);

					OnEndTurnClicked?.Invoke ();
				});
				Debug.Log ("End Turn button event connected");
			} else {
				Debug.LogError ("End Turn button is NULL!");
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

			Debug.Log ("All button events connected with strict flow validation");
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

			// Start with all buttons disabled (safe state)
			UpdateStrictButtonStates (false, false, false);
		}

		/// <summary>
		/// ENHANCED: Strict button state control with comprehensive logging
		/// </summary>
		/// <param name="enablePlay">Enable/disable PLAY button</param>
		/// <param name="enableDraw">Enable/disable DRAW button</param>
		/// <param name="enableEndTurn">Enable/disable END TURN button</param>
		public void UpdateStrictButtonStates (bool enablePlay, bool enableDraw, bool enableEndTurn) {
			Debug.Log ("=== UPDATING STRICT BUTTON STATES ===");
			Debug.Log ($"PLAY: {(enablePlay ? "ENABLED" : "DISABLED")}");
			Debug.Log ($"DRAW: {(enableDraw ? "ENABLED" : "DISABLED")}");
			Debug.Log ($"END TURN: {(enableEndTurn ? "ENABLED" : "DISABLED")}");

			// Update internal state tracking
			playButtonEnabled = enablePlay;
			drawButtonEnabled = enableDraw;
			endTurnButtonEnabled = enableEndTurn;

			// Update actual button states
			if (playCardButton != null) {
				playCardButton.interactable = enablePlay;
				Debug.Log ($"Play Card button updated: {(enablePlay ? "ENABLED" : "DISABLED")}");
			}

			if (drawCardButton != null) {
				drawCardButton.interactable = enableDraw;
				Debug.Log ($"Draw Card button updated: {(enableDraw ? "ENABLED" : "DISABLED")}");
			}

			if (endTurnButton != null) {
				endTurnButton.interactable = enableEndTurn;
				Debug.Log ($"End Turn button updated: {(enableEndTurn ? "ENABLED" : "DISABLED")}");
			}

			// Pause button should always be available (when implemented)
			if (pauseButton != null) {
				pauseButton.interactable = true;
			}

			Debug.Log ("Strict button state update complete");
		}

		/// <summary>
		/// Update turn display
		/// </summary>
		public void UpdateTurnDisplay (TurnState turnState) {
			Debug.Log ($"=== UPDATING TURN DISPLAY for {turnState} ===");

			if (turnIndicatorText != null) {
				string turnMessage = GetTurnMessage (turnState);
				turnIndicatorText.text = turnMessage;
				Debug.Log ($"Turn indicator text: '{turnMessage}'");
			}

			// Do NOT automatically update button states here
			// Button states are controlled by GameManager's strict flow system
			Debug.Log ("Turn display updated - button states controlled by strict flow system");
		}

		/// <summary>
		/// Validate current button states for debugging
		/// </summary>
		public void ValidateButtonStates () {
			Debug.Log ("=== VALIDATING STRICT BUTTON STATES ===");
			Debug.Log ($"Internal state - Play: {playButtonEnabled}, Draw: {drawButtonEnabled}, EndTurn: {endTurnButtonEnabled}");

			if (playCardButton != null) {
				bool actualPlay = playCardButton.interactable;
				Debug.Log ($"Play Card - Expected: {playButtonEnabled}, Actual: {actualPlay}, Match: {playButtonEnabled == actualPlay}");
				if (playButtonEnabled != actualPlay) {
					Debug.LogWarning ("MISMATCH in Play Card button state!");
				}
			}

			if (drawCardButton != null) {
				bool actualDraw = drawCardButton.interactable;
				Debug.Log ($"Draw Card - Expected: {drawButtonEnabled}, Actual: {actualDraw}, Match: {drawButtonEnabled == actualDraw}");
				if (drawButtonEnabled != actualDraw) {
					Debug.LogWarning ("MISMATCH in Draw Card button state!");
				}
			}

			if (endTurnButton != null) {
				bool actualEndTurn = endTurnButton.interactable;
				Debug.Log ($"End Turn - Expected: {endTurnButtonEnabled}, Actual: {actualEndTurn}, Match: {endTurnButtonEnabled == actualEndTurn}");
				if (endTurnButtonEnabled != actualEndTurn) {
					Debug.LogWarning ("MISMATCH in End Turn button state!");
				}
			}

			Debug.Log ("Button state validation complete");
		}

		/// <summary>
		/// Force enable END TURN button after successful action
		/// Used when action was successful and player must end turn
		/// </summary>
		public void ForceEnableEndTurn () {
			Debug.Log ("=== FORCE ENABLING END TURN BUTTON ===");
			Debug.Log ("Action was successful - player must now END TURN");

			// Enable only END TURN button
			UpdateStrictButtonStates (false, false, true);
		}

		/// <summary>
		/// ENHANCED: Emergency button state fix (for debugging)
		/// </summary>
		public void EmergencyButtonFix (bool enablePlay, bool enableDraw, bool enableEndTurn) {
			Debug.Log ("=== EMERGENCY BUTTON STATE FIX ===");
			Debug.Log ("WARNING: Emergency fix called - check turn flow logic");

			UpdateStrictButtonStates (enablePlay, enableDraw, enableEndTurn);
			ValidateButtonStates ();
		}

		/// <summary>
		/// Debug method to log complete UI state
		/// </summary>
		[ContextMenu ("Log Complete UI State")]
		public void LogCompleteUIState () {
			Debug.Log ("=== COMPLETE UI STATE DEBUG ===");
			Debug.Log ($"Turn indicator: '{(turnIndicatorText != null ? turnIndicatorText.text : "NULL")}'");
			Debug.Log ($"Computer message: '{(player2MessageText != null ? player2MessageText.text : "NULL")}'");
			Debug.Log ($"Color selection active: {(colorSelectionPanel != null ? colorSelectionPanel.activeSelf : false)}");

			if (currentColorIndicator != null) {
				Debug.Log ($"Color indicator: {currentColorIndicator.color}");
			}

			ValidateButtonStates ();
			Debug.Log ("=== UI STATE DEBUG COMPLETE ===");
		}

		/// <summary>
		/// Debug method to test button states manually
		/// </summary>
		[ContextMenu ("Test Button States")]
		public void TestButtonStates () {
			Debug.Log ("=== TESTING BUTTON STATES MANUALLY ===");

			Debug.Log ("Testing: All disabled");
			UpdateStrictButtonStates (false, false, false);
			ValidateButtonStates ();

			Debug.Log ("Testing: Can play and draw");
			UpdateStrictButtonStates (true, true, false);
			ValidateButtonStates ();

			Debug.Log ("Testing: Must end turn");
			UpdateStrictButtonStates (false, false, true);
			ValidateButtonStates ();

			Debug.Log ("Button state testing complete");
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

			// Disable all action buttons during color selection
			if (show) {
				Debug.Log ("Color selection active - disabling all action buttons");
				UpdateStrictButtonStates (false, false, false);
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
		/// LEGACY: Basic button state update (kept for compatibility)
		/// </summary>
		public void UpdateButtonStates (bool enabled) {
			Debug.Log ($"=== LEGACY BUTTON UPDATE to {(enabled ? "ENABLED" : "DISABLED")} ===");
			Debug.LogWarning ("WARNING: Using legacy UpdateButtonStates - prefer UpdateStrictButtonStates");

			// Convert to strict button control
			if (enabled) {
				// Default enabled state: can play and draw, but not end turn yet
				UpdateStrictButtonStates (true, true, false);
			} else {
				// Disabled state: nothing enabled
				UpdateStrictButtonStates (false, false, false);
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

			// Disable all action buttons on game over
			UpdateStrictButtonStates (false, false, false);
			Debug.Log ("Game over - all buttons disabled");
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

			// Start with all buttons disabled for safety
			UpdateStrictButtonStates (false, false, false);
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
			Debug.Log ("=== UPDATING ALL DISPLAYS ===");
			Debug.Log ($"Turn: {turnState}, Status: {gameStatus}, Interaction: {interactionState}, Color: {activeColor}");

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
				// Disable all buttons for non-active game states
				UpdateStrictButtonStates (false, false, false);
			}

			Debug.Log ("All displays updated");
		}

		///// <summary>
		///// Debug method to log complete UI state
		///// </summary>
		//[ContextMenu ("Log UI State")]
		//public void LogUIState () {
		//	Debug.Log ("=== COMPLETE UI STATE DEBUG ===");
		//	Debug.Log ($"buttonsEnabled: {buttonsEnabled}");
		//	Debug.Log ($"Turn indicator text: '{(turnIndicatorText != null ? turnIndicatorText.text : "NULL")}'");
		//	Debug.Log ($"Computer message text: '{(player2MessageText != null ? player2MessageText.text : "NULL")}'");
		//	Debug.Log ($"Color selection active: {(colorSelectionPanel != null ? colorSelectionPanel.activeSelf : false)}");

		//	ValidateButtonStates ();

		//	if (currentColorIndicator != null) {
		//		Debug.Log ($"Current color indicator: {currentColorIndicator.color}");
		//	}

		//	Debug.Log ("=== UI STATE DEBUG COMPLETE ===");
		//}

		/// <summary>
		/// Force button state refresh (for emergency fixing)
		/// </summary>
		public void ForceButtonRefresh (bool enableState) {
			Debug.Log ($"=== FORCE BUTTON REFRESH to {(enableState ? "ENABLED" : "DISABLED")} ===");
			UpdateButtonStates (enableState);
			ValidateButtonStates ();
		}

		// Properties
		public bool PlayButtonEnabled => playButtonEnabled;
		public bool DrawButtonEnabled => drawButtonEnabled;
		public bool EndTurnButtonEnabled => endTurnButtonEnabled;
		public bool IsColorSelectionActive => colorSelectionPanel != null && colorSelectionPanel.activeSelf;

		// ENHANCED: Get button state summary for debugging
		public string GetButtonStateSummary () {
			return $"Play:{(playButtonEnabled ? "ON" : "OFF")}, Draw:{(drawButtonEnabled ? "ON" : "OFF")}, EndTurn:{(endTurnButtonEnabled ? "ON" : "OFF")}";
		}
	}
}