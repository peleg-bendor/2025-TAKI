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

		[Header ("Player Messages")]
		[Tooltip ("GameMessageText - shows messages to player (instructions, warnings)")]
		public TextMeshProUGUI playerMessageText;

		[Tooltip ("Player2MessageText - shows computer actions and thinking")]
		public TextMeshProUGUI computerMessageText;

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

		[Header ("PlusTwo Chain Display")]
		[Tooltip ("Text element to show chain status")]
		public TextMeshProUGUI chainStatusText;

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
			TakiLogger.LogUI ("Connecting button events with STRICT FLOW validation...");

			if (playCardButton != null) {
				playCardButton.onClick.AddListener (() => {
					TakiLogger.LogTurnFlow ("=== PLAY CARD BUTTON CLICKED ===");
					TakiLogger.LogTurnFlow ($"Button enabled state: {playButtonEnabled}", TakiLogger.LogLevel.Verbose);
					TakiLogger.LogTurnFlow ($"Button interactable: {playCardButton.interactable}", TakiLogger.LogLevel.Verbose);

					if (!playButtonEnabled) {
						TakiLogger.LogWarning ("PLAY CARD clicked but button should be disabled!", TakiLogger.LogCategory.TurnFlow);
						ShowComputerMessage ("Cannot play card right now!");
						return;
					}

					OnPlayCardClicked?.Invoke ();
				});
				TakiLogger.LogSystem ("Play Card button event connected");
			} else {
				TakiLogger.LogError ("Play Card button is NULL!", TakiLogger.LogCategory.UI);
			}

			if (drawCardButton != null) {
				drawCardButton.onClick.AddListener (() => {
					TakiLogger.LogTurnFlow ("=== DRAW CARD BUTTON CLICKED ===");
					TakiLogger.LogTurnFlow ($"Button enabled state: {drawButtonEnabled}", TakiLogger.LogLevel.Verbose);
					TakiLogger.LogTurnFlow ($"Button interactable: {drawCardButton.interactable}", TakiLogger.LogLevel.Verbose);

					if (!drawButtonEnabled) {
						TakiLogger.LogWarning ("DRAW CARD clicked but button should be disabled!", TakiLogger.LogCategory.TurnFlow);
						ShowComputerMessage ("Cannot draw card right now!");
						return;
					}

					OnDrawCardClicked?.Invoke ();
				});
				TakiLogger.LogSystem ("Draw Card button event connected");
			} else {
				TakiLogger.LogError ("Draw Card button is NULL!", TakiLogger.LogCategory.UI);
			}

			if (endTurnButton != null) {
				endTurnButton.onClick.AddListener (() => {
					TakiLogger.LogTurnFlow ("=== END TURN BUTTON CLICKED ===");
					TakiLogger.LogTurnFlow ($"Button enabled state: {endTurnButtonEnabled}", TakiLogger.LogLevel.Verbose);
					TakiLogger.LogTurnFlow ($"Button interactable: {endTurnButton.interactable}", TakiLogger.LogLevel.Verbose);

					if (!endTurnButtonEnabled) {
						TakiLogger.LogWarning ("END TURN clicked but button should be disabled!", TakiLogger.LogCategory.TurnFlow);
						ShowComputerMessage ("You must take an action first!");
						return;
					}

					// Immediately disable all buttons to prevent multiple clicks
					TakiLogger.LogTurnFlow ("IMMEDIATELY disabling all buttons after END TURN click");
					UpdateStrictButtonStates (false, false, false);

					OnEndTurnClicked?.Invoke ();
				});
				TakiLogger.LogSystem ("End Turn button event connected");
			} else {
				TakiLogger.LogError ("End Turn button is NULL!", TakiLogger.LogCategory.UI);
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

			TakiLogger.LogSystem ("All button events connected with strict flow validation");
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
			TakiLogger.LogTurnFlow ("=== UPDATING STRICT BUTTON STATES ===");
			TakiLogger.LogTurnFlow ($"PLAY: {(enablePlay ? "ENABLED" : "DISABLED")}");
			TakiLogger.LogTurnFlow ($"DRAW: {(enableDraw ? "ENABLED" : "DISABLED")}");
			TakiLogger.LogTurnFlow ($"END TURN: {(enableEndTurn ? "ENABLED" : "DISABLED")}");

			// Update internal state tracking
			playButtonEnabled = enablePlay;
			drawButtonEnabled = enableDraw;
			endTurnButtonEnabled = enableEndTurn;

			// Update actual button states
			if (playCardButton != null) {
				playCardButton.interactable = enablePlay;
				TakiLogger.LogTurnFlow ($"Play Card button updated: {(enablePlay ? "ENABLED" : "DISABLED")}", TakiLogger.LogLevel.Verbose);
			}

			if (drawCardButton != null) {
				drawCardButton.interactable = enableDraw;
				TakiLogger.LogTurnFlow ($"Draw Card button updated: {(enableDraw ? "ENABLED" : "DISABLED")}", TakiLogger.LogLevel.Verbose);
			}

			if (endTurnButton != null) {
				endTurnButton.interactable = enableEndTurn;
				TakiLogger.LogTurnFlow ($"End Turn button updated: {(enableEndTurn ? "ENABLED" : "DISABLED")}", TakiLogger.LogLevel.Verbose);
			}

			// Pause button should always be available (when implemented)
			if (pauseButton != null) {
				pauseButton.interactable = true;
			}

			TakiLogger.LogTurnFlow ("Strict button state update complete", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Update turn display
		/// </summary>
		public void UpdateTurnDisplay (TurnState turnState) {
			TakiLogger.LogUI ($"=== UPDATING TURN DISPLAY for {turnState} ===", TakiLogger.LogLevel.Debug);

			if (turnIndicatorText != null) {
				string turnMessage = GetTurnMessage (turnState);
				turnIndicatorText.text = turnMessage;
				TakiLogger.LogUI ($"Turn indicator text: '{turnMessage}'", TakiLogger.LogLevel.Verbose);
			}

			// Button states are controlled by GameManager's strict flow system
			// Do not automatically update button states here
			TakiLogger.LogTurnFlow ("Turn display updated - button states controlled by strict flow system", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Debug method to test pause UI states
		/// </summary>
		[ContextMenu ("Test Pause UI States")]
		public void TestPauseUIStates () {
			TakiLogger.LogDiagnostics ("=== TESTING PAUSE UI STATES ===");

			TakiLogger.LogDiagnostics ("Testing pause state");
			UpdateForPauseState ();

			// Wait a moment, then test resume
			Invoke (nameof (TestResumeUI), 2f);
		}

		void TestResumeUI () {
			TakiLogger.LogDiagnostics ("Testing resume state");
			UpdateForResumeState (TurnState.PlayerTurn);
		}

		/// <summary>
		/// Debug method to log complete UI state including pause info
		/// </summary>
		[ContextMenu ("Log Complete UI State With Pause")]
		public void LogCompleteUIStateWithPause () {
			TakiLogger.LogDiagnostics ("=== COMPLETE UI STATE DEBUG (WITH PAUSE) ===");

			// Log basic UI state
			LogCompleteUIState ();

			// Log pause-related state
			GameStateManager gameState = FindObjectOfType<GameStateManager> ();
			if (gameState != null) {
				TakiLogger.LogDiagnostics ($"Game Status: {gameState.gameStatus}");
				TakiLogger.LogDiagnostics ($"Is Paused: {gameState.IsGamePaused}");
				TakiLogger.LogDiagnostics ($"Can Player Act: {gameState.CanPlayerAct ()}");
				TakiLogger.LogDiagnostics ($"Can Computer Act: {gameState.CanComputerAct ()}");
			} else {
				TakiLogger.LogDiagnostics ("GameStateManager not found");
			}

			TakiLogger.LogDiagnostics ("Pause button state: " + (pauseButton != null ? pauseButton.interactable.ToString () : "NULL"));
		}


		/// <summary>
		/// Validate current button states for debugging
		/// </summary>
		public void ValidateButtonStates () {
			TakiLogger.LogDiagnostics ("=== VALIDATING STRICT BUTTON STATES ===");
			TakiLogger.LogDiagnostics ($"Internal state - Play: {playButtonEnabled}, Draw: {drawButtonEnabled}, EndTurn: {endTurnButtonEnabled}");

			if (playCardButton != null) {
				bool actualPlay = playCardButton.interactable;
				TakiLogger.LogDiagnostics ($"Play Card - Expected: {playButtonEnabled}, Actual: {actualPlay}, Match: {playButtonEnabled == actualPlay}");
				if (playButtonEnabled != actualPlay) {
					TakiLogger.LogWarning ("MISMATCH in Play Card button state!", TakiLogger.LogCategory.UI);
				}
			}

			if (drawCardButton != null) {
				bool actualDraw = drawCardButton.interactable;
				TakiLogger.LogDiagnostics ($"Draw Card - Expected: {drawButtonEnabled}, Actual: {actualDraw}, Match: {drawButtonEnabled == actualDraw}");
				if (drawButtonEnabled != actualDraw) {
					TakiLogger.LogWarning ("MISMATCH in Draw Card button state!", TakiLogger.LogCategory.UI);
				}
			}

			if (endTurnButton != null) {
				bool actualEndTurn = endTurnButton.interactable;
				TakiLogger.LogDiagnostics ($"End Turn - Expected: {endTurnButtonEnabled}, Actual: {actualEndTurn}, Match: {endTurnButtonEnabled == actualEndTurn}");
				if (endTurnButtonEnabled != actualEndTurn) {
					TakiLogger.LogWarning ("MISMATCH in End Turn button state!", TakiLogger.LogCategory.UI);
				}
			}

			TakiLogger.LogDiagnostics ("Button state validation complete");
		}

		/// <summary>
		/// Force enable END TURN button after successful action
		/// Used when action was successful and player must end turn
		/// </summary>
		public void ForceEnableEndTurn () {
			TakiLogger.LogTurnFlow ("=== FORCE ENABLING END TURN BUTTON ===");
			TakiLogger.LogTurnFlow ("Action was successful - player must now END TURN");

			// Enable only END TURN button
			UpdateStrictButtonStates (false, false, true);
		}

		/// <summary>
		/// ENHANCED: Emergency button state fix (for debugging)
		/// </summary>
		public void EmergencyButtonFix (bool enablePlay, bool enableDraw, bool enableEndTurn) {
			TakiLogger.LogDiagnostics ("=== EMERGENCY BUTTON STATE FIX ===");
			TakiLogger.LogWarning ("WARNING: Emergency fix called - check turn flow logic", TakiLogger.LogCategory.UI);

			UpdateStrictButtonStates (enablePlay, enableDraw, enableEndTurn);
			ValidateButtonStates ();
		}

		/// <summary>
		/// Debug method to log complete UI state
		/// </summary>
		[ContextMenu ("Log Complete UI State")]
		public void LogCompleteUIState () {
			TakiLogger.LogDiagnostics ("=== COMPLETE UI STATE DEBUG ===");
			TakiLogger.LogDiagnostics ($"Turn indicator: '{(turnIndicatorText != null ? turnIndicatorText.text : "NULL")}'");
			TakiLogger.LogDiagnostics ($"Player message: '{(playerMessageText != null ? playerMessageText.text : "NULL")}'");
			TakiLogger.LogDiagnostics ($"Computer message: '{(computerMessageText != null ? computerMessageText.text : "NULL")}'");
			TakiLogger.LogDiagnostics ($"Color selection active: {(colorSelectionPanel != null ? colorSelectionPanel.activeSelf : false)}");

			if (currentColorIndicator != null) {
				TakiLogger.LogDiagnostics ($"Color indicator: {currentColorIndicator.color}");
			}

			ValidateButtonStates ();
			TakiLogger.LogDiagnostics ("=== UI STATE DEBUG COMPLETE ===");
		}

		/// <summary>
		/// Debug method to test button states manually
		/// </summary>
		[ContextMenu ("Test Button States")]
		public void TestButtonStates () {
			TakiLogger.LogDiagnostics ("=== TESTING BUTTON STATES MANUALLY ===");

			TakiLogger.LogDiagnostics ("Testing: All disabled");
			UpdateStrictButtonStates (false, false, false);
			ValidateButtonStates ();

			TakiLogger.LogDiagnostics ("Testing: Can play and draw");
			UpdateStrictButtonStates (true, true, false);
			ValidateButtonStates ();

			TakiLogger.LogDiagnostics ("Testing: Must end turn");
			UpdateStrictButtonStates (false, false, true);
			ValidateButtonStates ();

			TakiLogger.LogDiagnostics ("Button state testing complete");
		}

		[ContextMenu ("Test Message Routing")]
		public void TestMessageRouting () {
			ShowPlayerMessage ("TEST: Player instruction message");
			ShowComputerMessage ("TEST: Computer action message");
			TakiLogger.LogDiagnostics ("Message routing test complete - check UI text elements");
		}

		/// <summary>
		/// Get turn message from turn state
		/// </summary>
		string GetTurnMessage (TurnState turnState) {
			// Check game status first through GameStateManager if available
			if (Application.isPlaying) {
				GameStateManager gameState = FindObjectOfType<GameStateManager> ();
				if (gameState != null) {
					if (gameState.gameStatus == GameStatus.Paused) {
						return "Game Paused";
					} else if (gameState.gameStatus == GameStatus.GameOver) {
						return "Game Over";
					}
				}
			}

			// Normal turn state messages
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
		/// Show message to player (instructions, warnings, guidance)
		/// </summary>
		/// <param name="message">Message to show</param>
		public void ShowPlayerMessage (string message) {
			if (playerMessageText != null) {
				playerMessageText.text = message;
			}
		}

		/// <summary>
		/// Show computer action message (AI thinking, actions)
		/// UPDATED: Now uses computerMessageText instead of gameMessageText
		/// </summary>
		/// <param name="message">Message to show</param>
		public void ShowComputerMessage (string message) {
			if (computerMessageText != null) {
				computerMessageText.text = message;
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
				TakiLogger.LogUI ("Color selection active - disabling all action buttons");
				UpdateStrictButtonStates (false, false, true);
			}
		}

		/// <summary>
		/// Handle color selection - FIXED to keep panel visible
		/// </summary> 
		/// <param name="selectedColor">Selected color</param>
		void SelectColor (CardColor selectedColor) {
			// Update color indicator
			UpdateActiveColorDisplay (selectedColor);

			// Notify external systems
			OnColorSelected?.Invoke (selectedColor);

			TakiLogger.LogUI ($"Player selected color: {selectedColor} (panel stays visible)");

			// Show feedback that color was selected but player can choose again
			ShowPlayerMessage ($"Color set to {selectedColor} - click END TURN when ready!");
		}

		/// <summary>
		/// LEGACY: Basic button state update (kept for compatibility)
		/// </summary>
		public void UpdateButtonStates (bool enabled) {
			TakiLogger.LogUI ($"=== LEGACY BUTTON UPDATE to {(enabled ? "ENABLED" : "DISABLED")} ===", TakiLogger.LogLevel.Debug);
			TakiLogger.LogWarning ("WARNING: Using legacy UpdateButtonStates - prefer UpdateStrictButtonStates", TakiLogger.LogCategory.UI);

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

			ShowPlayerMessage ("");

			ShowComputerMessage ("");

			// Disable all action buttons on game over
			UpdateStrictButtonStates (false, false, false);
			TakiLogger.LogGameState ("Game over - all buttons disabled");
		}

		/// <summary>
		/// Reset UI for new game
		/// </summary>
		public void ResetUIForNewGame () {
			if (turnIndicatorText != null) {
				turnIndicatorText.text = "New Game";
			}

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
			TakiLogger.LogUI ("=== UPDATING ALL DISPLAYS ===", TakiLogger.LogLevel.Debug);
			TakiLogger.LogUI ($"Turn: {turnState}, Status: {gameStatus}, Interaction: {interactionState}, Color: {activeColor}", TakiLogger.LogLevel.Debug);

			UpdateTurnDisplay (turnState);
			UpdateActiveColorDisplay (activeColor);

			// Handle special interaction states
			if (interactionState == InteractionState.ColorSelection) {
				ShowColorSelection (true);
			} else {
				ShowColorSelection (false);
			}

			// Handle game status states
			switch (gameStatus) {
				case GameStatus.Paused:
					HandlePausedState ();
					break;
				case GameStatus.GameOver:
					HandleGameOverState ();
					break;
				case GameStatus.Active:
					HandleActiveState (turnState);
					break;
			}

			TakiLogger.LogUI ("All displays updated", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Handle UI state when game is paused
		/// </summary>
		void HandlePausedState () {
			TakiLogger.LogUI ("Handling paused game state");

			// Disable all gameplay buttons
			UpdateStrictButtonStates (false, false, false);

			// Show pause message to user
			ShowPlayerMessage ("Game Paused");
			ShowComputerMessage ("Game is paused");

			// Pause button should still be available (it becomes Continue when paused)
			if (pauseButton != null) {
				pauseButton.interactable = true;
			}
		}

		/// <summary>
		/// Handle UI state when game is over
		/// </summary>
		void HandleGameOverState () {
			TakiLogger.LogUI ("Handling game over state");

			// Disable all gameplay buttons
			UpdateStrictButtonStates (false, false, false);

			// Pause button should be disabled
			if (pauseButton != null) {
				pauseButton.interactable = false;
			}

			// Winner message will be handled by GameEndManager
		}

		/// <summary>
		/// Handle UI state when game is active
		/// </summary>
		/// <param name="turnState">Current turn state</param>
		void HandleActiveState (TurnState turnState) {
			TakiLogger.LogUI ("Handling active game state");

			// Pause button should be available
			if (pauseButton != null) {
				pauseButton.interactable = true;
			}

			// Button states will be handled by strict turn flow system
			// Don't automatically enable buttons here - let GameManager control them
		}

		/// <summary>
		/// Update UI for pause state
		/// </summary>
		public void UpdateForPauseState () {
			TakiLogger.LogUI ("=== UPDATING UI FOR PAUSE STATE ===");

			// Update turn display to show pause
			if (turnIndicatorText != null) {
				turnIndicatorText.text = "Game Paused";
			}

			// Disable all action buttons
			UpdateStrictButtonStates (false, false, false);

			// Show pause message
			ShowPlayerMessage ("Game is paused - use menu to continue");
			ShowComputerMessage ("Waiting for resume...");

			TakiLogger.LogUI ("UI updated for pause state");
		}

		/// <summary>
		/// Update UI for resume state
		/// </summary>
		/// <param name="turnState">Turn state to resume to</param>
		public void UpdateForResumeState (TurnState turnState) {
			TakiLogger.LogUI ("=== UPDATING UI FOR RESUME STATE ===");

			// Update turn display
			UpdateTurnDisplay (turnState);

			// Clear pause messages
			if (turnState == TurnState.PlayerTurn) {
				ShowPlayerMessage ("Game resumed - your turn!");
				ShowComputerMessage ("");
			} else if (turnState == TurnState.ComputerTurn) {
				ShowPlayerMessage ("Game resumed");
				ShowComputerMessage ("Computer's turn");
			}

			// Button states will be restored by GameManager's turn flow system

			TakiLogger.LogUI ("UI updated for resume state");
		}

		/// <summary>
		/// Show exit validation state in UI
		/// </summary>
		public void UpdateForExitValidation () {
			TakiLogger.LogUI ("=== UPDATING UI FOR EXIT VALIDATION ===");

			// Similar to pause but with different message
			if (turnIndicatorText != null) {
				turnIndicatorText.text = "Confirm Exit";
			}

			// Disable all action buttons
			UpdateStrictButtonStates (false, false, false);

			// Show exit validation message
			ShowPlayerMessage ("Exit confirmation shown");
			ShowComputerMessage ("Game paused for exit");

			TakiLogger.LogUI ("UI updated for exit validation");
		}

		/// <summary>
		/// Show pause-related message to player
		/// </summary>
		/// <param name="message">Message to show</param>
		public void ShowPauseMessage (string message) {
			ShowPlayerMessage ($"PAUSE: {message}");
			TakiLogger.LogUI ($"Pause message shown: {message}");
		}

		/// <summary>
		/// Show game end related message
		/// </summary>
		/// <param name="message">Message to show</param>
		public void ShowGameEndMessage (string message) {
			ShowPlayerMessage ($"GAME END: {message}");
			TakiLogger.LogUI ($"Game end message shown: {message}");
		}

		/// <summary>
		/// Force button state refresh (for emergency fixing)
		/// </summary>
		public void ForceButtonRefresh (bool enableState) {
			TakiLogger.LogDiagnostics ($"=== FORCE BUTTON REFRESH to {(enableState ? "ENABLED" : "DISABLED")} ===");
			UpdateButtonStates (enableState);
			ValidateButtonStates ();
		}

		/// <summary>
		/// Show PlusTwo chain status with progressive messaging
		/// </summary>
		/// <param name="chainCount">Number of PlusTwo cards in chain</param>
		/// <param name="accumulatedDraw">Total cards to draw</param>
		/// <param name="isPlayerTurn">True if it's player's turn to respond</param>
		public void ShowPlusTwoChainStatus (int chainCount, int accumulatedDraw, bool isPlayerTurn) {
			if (chainStatusText != null) {
				string statusMessage;

				if (isPlayerTurn) {
					statusMessage = $"PlusTwo Chain: {chainCount} cards -> Draw {accumulatedDraw} or play PlusTwo";
				} else {
					statusMessage = $"PlusTwo Chain: {chainCount} cards -> AI must respond";
				}

				chainStatusText.text = statusMessage;
				chainStatusText.color = Color.yellow; // Highlight chain status 
			}

			TakiLogger.LogUI ($"Chain status displayed: {chainCount} cards, {accumulatedDraw} draw, PlayerTurn={isPlayerTurn}");
		}

		/// <summary>
		/// Hide PlusTwo chain status
		/// </summary>
		public void HidePlusTwoChainStatus () {
			if (chainStatusText != null) {
				chainStatusText.text = "";
			}

			TakiLogger.LogUI ("Chain status hidden");
		}

		/// <summary>
		/// Enhanced chain progress messaging with better routing
		/// </summary>
		/// <param name="chainCount">Number of cards in chain</param>
		/// <param name="drawCount">Cards to draw if broken</param>
		/// <param name="targetPlayer">Who needs to respond</param>
		public void ShowChainProgressMessage (int chainCount, int drawCount, PlayerType targetPlayer) {
			string message;

			if (chainCount == 1) {
				// First PlusTwo played
				message = targetPlayer == PlayerType.Human ?
					$"PlusTwo played! Draw {drawCount} cards or play PlusTwo" :
					$"PlusTwo played! AI draws {drawCount} or continues chain";
			} else {
				// Chain continues  
				message = targetPlayer == PlayerType.Human ?
					$"Chain grows! Draw {drawCount} cards or play PlusTwo" :
					$"Chain grows! AI draws {drawCount} or continues chain";
			}

			if (targetPlayer == PlayerType.Human) {
				ShowPlayerMessageTimed (message, 0f); // Permanent until action taken
			} else {
				ShowComputerMessageTimed (message, 4.0f);
			}
		}

		/// <summary>
		/// Show chain broken message with enhanced feedback
		/// </summary>
		/// <param name="cardsDrawn">Number of cards drawn</param>
		/// <param name="whoBreaks">Who broke the chain</param>
		public void ShowChainBrokenMessage (int cardsDrawn, PlayerType whoBreaks) {
			string playerMessage = whoBreaks == PlayerType.Human ?
				$"Chain broken! You drew {cardsDrawn} cards" :
				$"Chain broken! AI drew {cardsDrawn} cards";

			string computerMessage = whoBreaks == PlayerType.Computer ?
				$"I drew {cardsDrawn} cards - chain broken" :
				$"Opponent drew {cardsDrawn} cards - chain broken";

			ShowPlayerMessageTimed (playerMessage, 3.0f);
			ShowComputerMessageTimed (computerMessage, 3.0f);

			// Clear chain status
			HidePlusTwoChainStatus ();
		}

		// Properties 
		public bool PlayButtonEnabled => playButtonEnabled;
		public bool DrawButtonEnabled => drawButtonEnabled;
		public bool EndTurnButtonEnabled => endTurnButtonEnabled;
		public bool IsColorSelectionActive => colorSelectionPanel != null && colorSelectionPanel.activeSelf;

		public bool IsUIInPauseState => turnIndicatorText != null && turnIndicatorText.text == "Game Paused";
		public bool AreAllButtonsDisabled => !PlayButtonEnabled && !DrawButtonEnabled && !EndTurnButtonEnabled;

		// ENHANCED: Get button state summary for debugging 
		public string GetButtonStateSummary () {
			return $"Play:{(playButtonEnabled ? "ON" : "OFF")}, Draw:{(drawButtonEnabled ? "ON" : "OFF")}, EndTurn:{(endTurnButtonEnabled ? "ON" : "OFF")}";
		}

		/// <summary>
		/// ENHANCED: Show player message with duration to prevent overwrites
		/// </summary>
		/// <param name="message">Message to show</param>
		/// <param name="duration">How long to show before clearing (0 = permanent)</param>
		public void ShowPlayerMessageTimed (string message, float duration = 3.0f) {
			if (playerMessageText != null) {
				playerMessageText.text = message;
				TakiLogger.LogUI ($"Player message: '{message}' (duration: {duration}s)");

				if (duration > 0) {
					// Cancel any existing clear operations
					CancelInvoke (nameof (ClearPlayerMessage));
					// Schedule clear
					Invoke (nameof (ClearPlayerMessage), duration);
				}
			}
		}

		/// <summary>
		/// ENHANCED: Show computer message with duration to prevent overwrites
		/// </summary>
		/// <param name="message">Message to show</param>
		/// <param name="duration">How long to show before clearing (0 = permanent)</param>
		public void ShowComputerMessageTimed (string message, float duration = 3.0f) {
			if (computerMessageText != null) {
				computerMessageText.text = message;
				TakiLogger.LogUI ($"Computer message: '{message}' (duration: {duration}s)");

				if (duration > 0) {
					// Cancel any existing clear operations
					CancelInvoke (nameof (ClearComputerMessage));
					// Schedule clear
					Invoke (nameof (ClearComputerMessage), duration);
				}
			}
		}

		/// <summary>
		/// Clear player message
		/// </summary>
		void ClearPlayerMessage () {
			if (playerMessageText != null) {
				playerMessageText.text = "";
				TakiLogger.LogUI ("Player message cleared");
			}
		}

		/// <summary>
		/// Clear computer message
		/// </summary>
		void ClearComputerMessage () {
			if (computerMessageText != null) {
				computerMessageText.text = "";
				TakiLogger.LogUI ("Computer message cleared");
			}
		}

		/// <summary>
		/// ENHANCED: Show special card effect message with appropriate routing
		/// </summary>
		/// <param name="cardType">Type of special card</param>
		/// <param name="playedBy">Who played the card</param>
		/// <param name="effectDescription">Description of the effect</param>
		public void ShowSpecialCardEffect (CardType cardType, PlayerType playedBy, string effectDescription) {
			TakiLogger.LogUI ($"=== SPECIAL CARD EFFECT MESSAGE ===");
			TakiLogger.LogUI ($"Card: {cardType}, Player: {playedBy}, Effect: {effectDescription}");

			// Route message to appropriate UI element based on who played
			if (playedBy == PlayerType.Human) {
				// Human played - show in computer message area what happened to computer
				ShowComputerMessageTimed ($"You played {cardType}: {effectDescription}", 4.0f);
				// Also show in player area what human should do next
				if (cardType == CardType.Plus) {
					ShowPlayerMessageTimed ("PLUS effect: Take 1 more action (PLAY or DRAW)!", 0f); // Permanent until action taken
				}
			} else {
				// Computer played - show in player message area what happened to player
				ShowPlayerMessageTimed ($"AI played {cardType}: {effectDescription}", 4.0f);
				// Also show in computer area what AI is doing
				if (cardType == CardType.Plus) {
					ShowComputerMessageTimed ("PLUS: I get one more action!", 2.0f);
				} else if (cardType == CardType.Stop) {
					ShowComputerMessageTimed ("STOP: Your turn is skipped!", 3.0f);
				}
			}
		}

		/// <summary>
		/// Show immediate action feedback (short duration, high priority)
		/// </summary>
		/// <param name="message">Urgent message to show</param>
		/// <param name="toPlayer">If true, show to player; if false, show to computer area</param>
		public void ShowImmediateFeedback (string message, bool toPlayer = true) {
			if (toPlayer) {
				ShowPlayerMessageTimed (message, 2.0f);
			} else {
				ShowComputerMessageTimed (message, 2.0f);
			}

			TakiLogger.LogUI ($"Immediate feedback: '{message}' -> {(toPlayer ? "Player" : "Computer")}");
		}
	}
}