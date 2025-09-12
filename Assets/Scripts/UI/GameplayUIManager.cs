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

		[Header ("TAKI Sequence UI - Phase 8B")]
		[Tooltip ("Btn_Player1EndTakiSequence - end TAKI sequence manually")]
		public Button Btn_Player1EndTakiSequence;

		[Tooltip ("Text element to show TAKI sequence status")]
		public TextMeshProUGUI takiSequenceStatusText;

		// Events for external systems
		public System.Action OnPlayCardClicked;
		public System.Action OnDrawCardClicked;
		public System.Action OnEndTurnClicked;
		public System.Action<CardColor> OnColorSelected;
		public System.Action OnEndTakiSequenceClicked; // PHASE 8B: New event

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
		/// Connect button events to internal handlers - with validation and TAKI sequence support
		/// </summary>
		void ConnectButtonEvents () {
			TakiLogger.LogUI ("Connecting button events with STRICT FLOW validation...");

			if (playCardButton != null) {
				playCardButton.onClick.AddListener (() => {
					TakiLogger.LogTurnFlow ("=== PLAY CARD BUTTON CLICKED ===");
					TakiLogger.LogTurnFlow ($"Button enabled state: {playButtonEnabled}", TakiLogger.LogLevel.Trace);
					TakiLogger.LogTurnFlow ($"Button interactable: {playCardButton.interactable}", TakiLogger.LogLevel.Trace);

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
					TakiLogger.LogTurnFlow ($"Button enabled state: {drawButtonEnabled}", TakiLogger.LogLevel.Trace);
					TakiLogger.LogTurnFlow ($"Button interactable: {drawCardButton.interactable}", TakiLogger.LogLevel.Trace);

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
					TakiLogger.LogTurnFlow ($"Button enabled state: {endTurnButtonEnabled}", TakiLogger.LogLevel.Trace);
					TakiLogger.LogTurnFlow ($"Button interactable: {endTurnButton.interactable}", TakiLogger.LogLevel.Trace);

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

			// PHASE 8B: Connect End TAKI Sequence button
			if (Btn_Player1EndTakiSequence != null) {
				Btn_Player1EndTakiSequence.onClick.AddListener (() => {
					TakiLogger.LogTurnFlow ("=== END TAKI SEQUENCE BUTTON CLICKED ===");
					TakiLogger.LogTurnFlow ($"Button interactable: {Btn_Player1EndTakiSequence.interactable}", TakiLogger.LogLevel.Trace);

					if (!Btn_Player1EndTakiSequence.interactable) {
						TakiLogger.LogWarning ("END TAKI SEQUENCE clicked but button should be disabled!", TakiLogger.LogCategory.TurnFlow);
						ShowComputerMessage ("Cannot end sequence right now!");
						return;
					}

					OnEndTakiSequenceClicked?.Invoke ();
				});
				TakiLogger.LogSystem ("End TAKI Sequence button event connected");
			} else {
				TakiLogger.LogWarning ("Btn_Player1EndTakiSequence is NULL - TAKI sequence ending will not work!", TakiLogger.LogCategory.UI);
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
				TakiLogger.LogTurnFlow ($"Play Card button updated: {(enablePlay ? "ENABLED" : "DISABLED")}", TakiLogger.LogLevel.Trace);
			}

			if (drawCardButton != null) {
				drawCardButton.interactable = enableDraw;
				TakiLogger.LogTurnFlow ($"Draw Card button updated: {(enableDraw ? "ENABLED" : "DISABLED")}", TakiLogger.LogLevel.Trace);
			}

			if (endTurnButton != null) {
				endTurnButton.interactable = enableEndTurn;
				TakiLogger.LogTurnFlow ($"End Turn button updated: {(enableEndTurn ? "ENABLED" : "DISABLED")}", TakiLogger.LogLevel.Trace);
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
				TakiLogger.LogUI ($"Turn indicator text: '{turnMessage}'", TakiLogger.LogLevel.Trace);
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

		[ContextMenu ("Validate End Taki Sequence Button")]
		public void ValidateEndSequenceButton () {
			TakiLogger.LogDiagnostics ("=== END TAKI SEQUENCE BUTTON VALIDATION ===");

			GameStateManager gameState = FindObjectOfType<GameStateManager> ();
			if (gameState == null) {
				TakiLogger.LogDiagnostics ("GameState is NULL - cannot validate");
				return;
			}

			bool inTakiSequence = gameState.IsInTakiSequence;
			PlayerType initiator = gameState.TakiSequenceInitiator;
			bool isPlayerTurn = gameState.IsPlayerTurn;
			bool buttonEnabled = EndTakiSequenceButtonEnabled;

			TakiLogger.LogDiagnostics ($"In Taki Sequence: {inTakiSequence}");
			TakiLogger.LogDiagnostics ($"Initiator: {initiator}");
			TakiLogger.LogDiagnostics ($"Player Turn: {isPlayerTurn}");
			TakiLogger.LogDiagnostics ($"Button Enabled: {buttonEnabled}");

			// Validation logic
			bool shouldBeEnabled = inTakiSequence &&
								  (initiator == PlayerType.Human) &&
								  isPlayerTurn;

			TakiLogger.LogDiagnostics ($"Should Be Enabled: {shouldBeEnabled}");
			TakiLogger.LogDiagnostics ($"State Matches: {buttonEnabled == shouldBeEnabled}");

			if (buttonEnabled != shouldBeEnabled) {
				TakiLogger.LogWarning ("BUTTON STATE MISMATCH!", TakiLogger.LogCategory.UI);
			}
		}

		/// <summary> 
		/// Validate current button states for debugging - ENHANCED with TAKI sequence button
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

			// PHASE 8B: Validate End TAKI Sequence button
			if (Btn_Player1EndTakiSequence != null) {
				bool actualEndSequence = Btn_Player1EndTakiSequence.interactable;
				bool expectedEndSequence = EndTakiSequenceButtonEnabled;
				TakiLogger.LogDiagnostics ($"End TAKI Sequence - Expected: {expectedEndSequence}, Actual: {actualEndSequence}, Match: {expectedEndSequence == actualEndSequence}");
				if (expectedEndSequence != actualEndSequence) {
					TakiLogger.LogWarning ("MISMATCH in End TAKI Sequence button state!", TakiLogger.LogCategory.UI);
				}
			} else {
				TakiLogger.LogWarning ("End TAKI Sequence button is NULL!", TakiLogger.LogCategory.UI);
			}

			TakiLogger.LogDiagnostics ("Button state validation complete (with TAKI sequence support)");
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
		/// CRITICAL FIX: Enhanced UpdateAllDisplays with proper sequence button control
		/// </summary>
		/// <param name="turnState">Current turn state</param>
		/// <param name="gameStatus">Current game status</param>
		/// <param name="interactionState">Current interaction state</param>
		/// <param name="activeColor">Current active color</param>
		public void UpdateAllDisplays (TurnState turnState, GameStatus gameStatus, InteractionState interactionState, CardColor activeColor) {
			TakiLogger.LogUI ("=== UPDATING ALL DISPLAYS (FIXED SEQUENCE CONTROL) ===", TakiLogger.LogLevel.Debug);

			UpdateTurnDisplay (turnState);
			UpdateActiveColorDisplay (activeColor);

			// Handle special interaction states
			if (interactionState == InteractionState.ColorSelection) {
				ShowColorSelection (true);
			} else {
				ShowColorSelection (false);
			}

			// CRITICAL FIX: Enhanced TAKI sequence state handling with proper validation
			if (interactionState == InteractionState.TakiSequence) {
				// CRITICAL FIX: Only enable End Sequence button for HUMAN sequences
				GameStateManager gameState = FindObjectOfType<GameStateManager> ();
				bool shouldEnableButton = false;

				if (gameState != null && gameState.IsInTakiSequence) {
					// Only enable for human-initiated sequences on human's turn
					shouldEnableButton = (gameState.TakiSequenceInitiator == PlayerType.Human) &&
										(gameState.IsPlayerTurn);
				}

				EnableEndTakiSequenceButton (shouldEnableButton);
				TakiLogger.LogUI ($"TAKI sequence active - End Sequence button control: {shouldEnableButton}");

				// Show sequence status (handles who initiated the sequence)
				if (gameState != null && takiSequenceStatusText != null) {
					bool isPlayerTurn = gameState.IsPlayerTurn;
					int cardCount = gameState.NumberOfSequenceCards;
					CardColor sequenceColor = gameState.TakiSequenceColor;
					ShowTakiSequenceStatus (sequenceColor, cardCount, isPlayerTurn);
				}
			} else {
				// Disable End TAKI Sequence button when not in sequence 
				EnableEndTakiSequenceButton (false);
				HideTakiSequenceStatus ();
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

			TakiLogger.LogUI ("All displays updated (with enhanced TAKI sequence support)", TakiLogger.LogLevel.Debug);
		}

		/// <summary> 
		/// DEBUGGING: Method to verify UI component assignments
		/// </summary>
		[ContextMenu ("Debug TAKI UI Components")]
		public void DebugTakiUIComponents () {
			TakiLogger.LogDiagnostics ("=== TAKI UI COMPONENTS DEBUG ===");

			if (Btn_Player1EndTakiSequence != null) {
				TakiLogger.LogDiagnostics ($"Btn_Player1EndTakiSequence: ASSIGNED");
				TakiLogger.LogDiagnostics ($"  - Interactable: {Btn_Player1EndTakiSequence.interactable}");
				TakiLogger.LogDiagnostics ($"  - Active: {Btn_Player1EndTakiSequence.gameObject.activeSelf}");
				TakiLogger.LogDiagnostics ($"  - Name: {Btn_Player1EndTakiSequence.name}");
			} else {
				TakiLogger.LogDiagnostics ("Btn_Player1EndTakiSequence: NULL - NOT ASSIGNED!");
			}

			if (takiSequenceStatusText != null) {
				TakiLogger.LogDiagnostics ($"takiSequenceStatusText: ASSIGNED");
				TakiLogger.LogDiagnostics ($"  - Text: '{takiSequenceStatusText.text}'");
				TakiLogger.LogDiagnostics ($"  - Active: {takiSequenceStatusText.gameObject.activeSelf}");
				TakiLogger.LogDiagnostics ($"  - Name: {takiSequenceStatusText.name}");
			} else {
				TakiLogger.LogDiagnostics ("takiSequenceStatusText: NULL - NOT ASSIGNED!");
			}

			TakiLogger.LogDiagnostics ("=== END TAKI UI DEBUG ===");
		}

		/// <summary>
		/// Handle UI state when game is paused - ENHANCED with TAKI sequence button handling
		/// </summary>
		void HandlePausedState () {
			TakiLogger.LogUI ("Handling paused game state");

			// Disable all gameplay buttons including TAKI sequence button
			UpdateStrictButtonStates (false, false, false);
			EnableEndTakiSequenceButton (false);

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

			// Disable all gameplay buttons including TAKI sequence button
			UpdateStrictButtonStates (false, false, false);
			EnableEndTakiSequenceButton (false);

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

		/// <summary>
		/// CRITICAL FIX: Ensure End TAKI Sequence button works properly
		/// </summary>
		/// <param name="enable">Whether to enable the button</param>
		public void EnableEndTakiSequenceButton (bool enable) {
			if (Btn_Player1EndTakiSequence != null) {
				// CRITICAL FIX: Only enable for human sequences
				bool shouldEnable = enable;

				// Additional validation: only enable if human initiated the sequence
				if (enable) {
					GameStateManager gameState = FindObjectOfType<GameStateManager> ();
					if (gameState != null && gameState.IsInTakiSequence) {
						// Only enable if human is the sequence initiator
						shouldEnable = (gameState.TakiSequenceInitiator == PlayerType.Human);

						if (!shouldEnable) {
							TakiLogger.LogUI ("BLOCKED: End Sequence button - computer initiated sequence, human cannot end it");
						}
					}
				}

				Btn_Player1EndTakiSequence.interactable = shouldEnable;
				Btn_Player1EndTakiSequence.gameObject.SetActive (shouldEnable);

				TakiLogger.LogUI ($"End TAKI Sequence button {(shouldEnable ? "ENABLED & VISIBLE" : "DISABLED & HIDDEN")} (Human sequences only)");
			} else {
				TakiLogger.LogError ("CRITICAL: Btn_Player1EndTakiSequence is NULL! Button not assigned in Inspector!", TakiLogger.LogCategory.UI);
			}
		}

		/// <summary>
		/// CRITICAL FIX: Enhanced TAKI sequence status - show who controls the sequence
		/// </summary>
		/// <param name="sequenceColor">Color required for sequence</param>
		/// <param name="cardCount">Number of cards in sequence</param>
		/// <param name="isPlayerTurn">True if it's player's turn</param>
		public void ShowTakiSequenceStatus (CardColor sequenceColor, int cardCount, bool isPlayerTurn) {
			if (takiSequenceStatusText != null) {
				string statusMessage;

				// CRITICAL FIX: Show who initiated the sequence
				GameStateManager gameState = FindObjectOfType<GameStateManager> ();
				if (gameState != null && gameState.IsInTakiSequence) {
					PlayerType initiator = gameState.TakiSequenceInitiator;

					if (initiator == PlayerType.Human) {
						statusMessage = $"Your TAKI Sequence: {cardCount} cards -> Play {sequenceColor} cards or End Sequence";
					} else {
						statusMessage = $"AI TAKI Sequence: {cardCount} cards -> AI playing {sequenceColor} cards";
					}
				} else {
					// Fallback to original logic 
					if (isPlayerTurn) {
						statusMessage = $"TAKI Sequence: {cardCount} cards -> Play {sequenceColor} cards or End Sequence";
					} else {
						statusMessage = $"TAKI Sequence: {cardCount} cards -> AI playing {sequenceColor} cards";
					}
				}

				takiSequenceStatusText.text = statusMessage;
				takiSequenceStatusText.gameObject.SetActive (true);

				TakiLogger.LogUI ($"TAKI sequence status displayed: '{statusMessage}'");
			} else {
				TakiLogger.LogError ("CRITICAL: takiSequenceStatusText is NULL! Text element not assigned in Inspector!", TakiLogger.LogCategory.UI);
			}
		}
		/// <summary>
		/// CRITICAL FIX: Ensure sequence status is properly hidden
		/// </summary>
		public void HideTakiSequenceStatus () {
			if (takiSequenceStatusText != null) {
				takiSequenceStatusText.text = "";
				takiSequenceStatusText.gameObject.SetActive (false);
				TakiLogger.LogUI ("TAKI sequence status hidden");
			} else {
				TakiLogger.LogError ("CRITICAL: takiSequenceStatusText is NULL when trying to hide!", TakiLogger.LogCategory.UI);
			}
		}

		/// <summary>
		/// PHASE 8B: Enhanced sequence progress messaging
		/// </summary>
		/// <param name="sequenceColor">Color of the sequence</param>
		/// <param name="cardCount">Number of cards played</param>
		/// <param name="initiator">Who started the sequence</param>
		public void ShowSequenceProgressMessage (CardColor sequenceColor, int cardCount, PlayerType initiator) {
			string message;

			if (cardCount == 1) {
				// First card in sequence
				message = initiator == PlayerType.Human ?
					$"TAKI Sequence started! Play {sequenceColor} cards or End Sequence" :
					$"AI started TAKI sequence for {sequenceColor} cards";
			} else {
				// Sequence continues
				message = initiator == PlayerType.Human ?
					$"Sequence continues! {cardCount} cards played, keep playing {sequenceColor} or End Sequence" :
					$"AI sequence continues: {cardCount} cards of {sequenceColor}";
			}

			if (initiator == PlayerType.Human) {
				ShowPlayerMessageTimed (message, 0f); // Permanent until sequence ends
			} else {
				ShowComputerMessageTimed (message, 4.0f);
			}
		}

		/// <summary>
		/// PHASE 8B: Show sequence ended message
		/// </summary>
		/// <param name="finalCardCount">Number of cards in final sequence</param>
		/// <param name="sequenceColor">Color of the sequence</param>
		/// <param name="who">Who ended the sequence</param>
		public void ShowSequenceEndedMessage (int finalCardCount, CardColor sequenceColor, PlayerType who) {
			string playerMessage = who == PlayerType.Human ?
				$"Sequence ended! You played {finalCardCount} {sequenceColor} cards" :
				$"AI sequence ended! AI played {finalCardCount} {sequenceColor} cards";

			string computerMessage = who == PlayerType.Computer ?
				$"I ended sequence: {finalCardCount} {sequenceColor} cards" :
				$"Opponent ended sequence: {finalCardCount} {sequenceColor} cards";

			ShowPlayerMessageTimed (playerMessage, 3.0f);
			ShowComputerMessageTimed (computerMessage, 3.0f);

			// Clear sequence status
			HideTakiSequenceStatus ();
		}

		// Properties 
		// PHASE 8B: Enhanced properties including TAKI sequence button
		public bool PlayButtonEnabled => playButtonEnabled;
		public bool DrawButtonEnabled => drawButtonEnabled;
		public bool EndTurnButtonEnabled => endTurnButtonEnabled;
		public bool EndTakiSequenceButtonEnabled => Btn_Player1EndTakiSequence != null && Btn_Player1EndTakiSequence.interactable;
		public bool IsColorSelectionActive => colorSelectionPanel != null && colorSelectionPanel.activeSelf;

		public bool IsUIInPauseState => turnIndicatorText != null && turnIndicatorText.text == "Game Paused";
		public bool AreAllButtonsDisabled => !PlayButtonEnabled && !DrawButtonEnabled && !EndTurnButtonEnabled && !EndTakiSequenceButtonEnabled;

		// ENHANCED: Get button state summary for debugging including TAKI sequence button
		public string GetButtonStateSummary () {
			return $"Play:{(playButtonEnabled ? "ON" : "OFF")}, Draw:{(drawButtonEnabled ? "ON" : "OFF")}, EndTurn:{(endTurnButtonEnabled ? "ON" : "OFF")}, EndSequence:{(EndTakiSequenceButtonEnabled ? "ON" : "OFF")}";
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

		#region MILESTONE 1: Enhanced Multiplayer UI Support

		/// <summary>
		/// PHASE 2: Update turn display for multiplayer
		/// MILESTONE 1: Enhanced UpdateTurnDisplayMultiplayer with deck status
		/// Preserves existing method while adding deck readiness feedback
		/// </summary>
		public void UpdateTurnDisplayMultiplayer (bool isLocalPlayerTurn) {
			if (turnIndicatorText != null) {
				if (isLocalPlayerTurn) {
					turnIndicatorText.text = "Your Turn";
				} else {
					turnIndicatorText.text = "Opponent's Turn";
				}
			}

			// MILESTONE 1: Enhanced button state management for multiplayer
			if (isLocalPlayerTurn) {
				// Check if game is ready for actions
				GameManager gameManager = FindObjectOfType<GameManager> ();
				bool gameReady = gameManager != null && gameManager.IsNetworkReady;

				if (gameReady) {
					UpdateStrictButtonStates (true, true, false); // Can play/draw
					ShowPlayerMessage ("Your turn - play a card or draw");
				} else {
					UpdateStrictButtonStates (false, false, false); // Wait for game ready
					ShowPlayerMessage ("Setting up game...");
				}
			} else {
				UpdateStrictButtonStates (false, false, false); // Wait for opponent
				ShowPlayerMessage ("Waiting for opponent...");
			}

			TakiLogger.LogNetwork ($"Multiplayer turn display updated: LocalTurn={isLocalPlayerTurn}");
		}

		/// <summary>
		/// PHASE 2: Show opponent action feedback
		/// MILESTONE 1: Enhanced opponent action feedback
		/// </summary>
		public void ShowOpponentAction (string action) {
			ShowComputerMessage ($"Opponent {action}");
			TakiLogger.LogNetwork ($"Opponent action displayed: {action}");
		}

		/// <summary>
		/// MILESTONE 1: Enhanced hand size display for multiplayer
		/// Shows proper labels for local vs opponent hands
		/// </summary>
		public void UpdateHandSizeDisplayMultiplayer (int localHandSize, int opponentHandSize) {
			if (player1HandSizeText != null) {
				player1HandSizeText.text = $"Your Cards: {localHandSize}";
			}

			if (player2HandSizeText != null) {
				player2HandSizeText.text = $"Opponent Cards: {opponentHandSize}";
			}

			TakiLogger.LogNetwork ($"Multiplayer hand sizes updated: Local={localHandSize}, Opponent={opponentHandSize}");
		}

		/// <summary>
		/// MILESTONE 1: Show network connection status
		/// </summary>
		public void ShowNetworkStatus (string status) {
			ShowPlayerMessageTimed ($"Network: {status}", 3.0f);
			TakiLogger.LogNetwork ($"Network status displayed: {status}");
		}

		/// <summary>
		/// MILESTONE 1: Show deck synchronization status
		/// </summary>
		public void ShowDeckSyncStatus (string message) {
			ShowComputerMessageTimed ($"Deck: {message}", 2.0f);
			TakiLogger.LogNetwork ($"Deck sync status: {message}");
		}

		/// <summary>
		/// MILESTONE 1: Enhanced multiplayer game ready feedback
		/// </summary>
		public void ShowMultiplayerGameReady () {
			ShowPlayerMessage ("Game ready - waiting for your turn...");
			ShowComputerMessage ("Both players connected");
			TakiLogger.LogNetwork ("Multiplayer game ready UI displayed");
		}

		/// <summary>
		/// MILESTONE 1: Show opponent hand privacy feedback
		/// </summary>
		public void ShowOpponentHandPrivacy (int cardCount) {
			ShowComputerMessageTimed ($"Opponent has {cardCount} cards (hidden)", 2.0f);
			TakiLogger.LogNetwork ($"Opponent hand privacy feedback: {cardCount} cards");
		}

		/// <summary>
		/// MILESTONE 1: Enhanced turn transition feedback for multiplayer
		/// </summary>
		public void ShowTurnTransitionMultiplayer (bool nowMyTurn, string previousAction = "") {
			if (nowMyTurn) {
				if (!string.IsNullOrEmpty (previousAction)) {
					ShowPlayerMessage ($"Opponent {previousAction} - now your turn!");
				} else {
					ShowPlayerMessage ("Your turn!");
				}
				ShowComputerMessage ("Waiting for your action...");
			} else {
				ShowPlayerMessage ("Turn sent - waiting for opponent...");
				ShowComputerMessage ("Opponent is thinking...");
			}

			TakiLogger.LogNetwork ($"Turn transition displayed: MyTurn={nowMyTurn}, PreviousAction={previousAction}");
		}

		/// <summary>
		/// MILESTONE 1: Show network error with recovery options
		/// </summary>
		public void ShowNetworkError (string error, bool canRecover = false) {
			ShowPlayerMessage ($"Network Error: {error}");

			if (canRecover) {
				ShowComputerMessage ("Attempting to reconnect...");
			} else {
				ShowComputerMessage ("Please check connection and restart");
			}

			TakiLogger.LogNetwork ($"Network error displayed: {error}, CanRecover={canRecover}");
		}

		/// <summary>
		/// MILESTONE 1: Enhanced UpdateAllDisplays with network awareness
		/// Preserves existing method while adding multiplayer support
		/// </summary>
		public void UpdateAllDisplaysWithNetwork (TurnState turnState, GameStatus gameStatus, InteractionState interactionState, CardColor activeColor, bool isMultiplayerMode = false) {
			// Call existing UpdateAllDisplays first
			UpdateAllDisplays (turnState, gameStatus, interactionState, activeColor);

			// MILESTONE 1: Additional multiplayer-specific updates
			if (isMultiplayerMode) {
				// Update for multiplayer context
				GameManager gameManager = FindObjectOfType<GameManager> ();
				if (gameManager != null && gameManager.networkGameManager != null) {
					bool isMyTurn = gameManager.networkGameManager.IsMyTurn;
					UpdateTurnDisplayMultiplayer (isMyTurn);
				}
			}

			TakiLogger.LogNetwork ($"All displays updated with network support: Multiplayer={isMultiplayerMode}");
		}

		#endregion

		#region MILESTONE 1: Network Game State Display

		/// <summary>
		/// MILESTONE 1: Show game initialization progress
		/// </summary>
		public void ShowGameInitProgress (string step) {
			ShowPlayerMessage ($"Setting up: {step}");
			TakiLogger.LogNetwork ($"Game init progress: {step}");
		}

		/// <summary>
		/// MILESTONE 1: Show deck initialization complete
		/// </summary>
		public void ShowDeckInitialized (int drawCount, int discardCount, string startingCard) {
			ShowPlayerMessage ("Game ready - deck initialized!");
			ShowComputerMessage ($"Draw: {drawCount}, Discard: {discardCount}, Start: {startingCard}");
			TakiLogger.LogNetwork ($"Deck initialized display: Draw={drawCount}, Discard={discardCount}, Start={startingCard}");
		}

		/// <summary>
		/// MILESTONE 1: Show hands initialized for multiplayer
		/// </summary>
		public void ShowHandsInitialized (int myHandSize, int opponentHandSize) {
			ShowPlayerMessage ($"Hands dealt - you have {myHandSize} cards");
			ShowComputerMessage ($"Opponent has {opponentHandSize} cards (hidden)");
			TakiLogger.LogNetwork ($"Hands initialized display: My={myHandSize}, Opponent={opponentHandSize}");
		}

		/// <summary>
		/// MILESTONE 1: Enhanced waiting for opponent feedback
		/// </summary>
		public void ShowWaitingForOpponent (string action = "to play") {
			ShowPlayerMessage ($"Waiting for opponent {action}...");
			ShowComputerMessage ("Opponent's turn");
			TakiLogger.LogNetwork ($"Waiting for opponent: {action}");
		}

		#endregion

		#region MILESTONE 1: Debug and Diagnostics

		/// <summary>
		/// MILESTONE 1: Debug multiplayer UI state
		/// </summary>
		[ContextMenu ("Debug Multiplayer UI State")]
		public void DebugMultiplayerUIState () {
			TakiLogger.LogDiagnostics ("=== MULTIPLAYER UI STATE DEBUG ===");

			// Check all UI components
			TakiLogger.LogDiagnostics ($"Turn Indicator: {(turnIndicatorText != null ? turnIndicatorText.text : "NULL")}");
			TakiLogger.LogDiagnostics ($"Player Message: {(playerMessageText != null ? playerMessageText.text : "NULL")}");
			TakiLogger.LogDiagnostics ($"Computer Message: {(computerMessageText != null ? computerMessageText.text : "NULL")}");

			// Check button states
			TakiLogger.LogDiagnostics ($"Button States: {GetButtonStateSummary ()}");

			// Check hand size displays
			if (player1HandSizeText != null) {
				TakiLogger.LogDiagnostics ($"Local Hand Display: {player1HandSizeText.text}");
			}
			if (player2HandSizeText != null) {
				TakiLogger.LogDiagnostics ($"Opponent Hand Display: {player2HandSizeText.text}");
			}

			TakiLogger.LogDiagnostics ("=== END MULTIPLAYER UI DEBUG ===");
		}

		/// <summary>
		/// MILESTONE 1: Test multiplayer UI feedback
		/// </summary>
		[ContextMenu ("Test Multiplayer UI Feedback")]
		public void TestMultiplayerUIFeedback () {
			TakiLogger.LogDiagnostics ("=== TESTING MULTIPLAYER UI FEEDBACK ===");

			// Test various multiplayer messages
			ShowNetworkStatus ("Connected to opponent");
			Invoke (nameof (TestOpponentAction), 1f);
			Invoke (nameof (TestTurnTransition), 2f);
			Invoke (nameof (TestHandPrivacy), 3f);
		}

		void TestOpponentAction () {
			ShowOpponentAction ("played Red 5");
		}

		void TestTurnTransition () {
			ShowTurnTransitionMultiplayer (true, "drew a card");
		}

		void TestHandPrivacy () {
			ShowOpponentHandPrivacy (6);
		}

		#endregion
	}
}