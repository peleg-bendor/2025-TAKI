using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TakiGame {
    /// <summary>
    /// Base class for screen-specific UI managers - contains shared functionality
    /// Solves architecture problem: Single GameplayUIManager couldn't handle both SinglePlayer and MultiPlayer screen hierarchies
    /// </summary>
    public abstract class BaseGameplayUIManager : MonoBehaviour {

        [Header("Color Settings")]
        [Tooltip("Colors to use for current color indicator")]
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
        public System.Action OnEndTakiSequenceClicked;

        // Button state tracking
        protected bool playButtonEnabled = false;
        protected bool drawButtonEnabled = false;
        protected bool endTurnButtonEnabled = false;

        // Abstract properties - implemented by derived classes
        public abstract TextMeshProUGUI TurnIndicatorText { get; }
        public abstract Image CurrentColorIndicator { get; }
        public abstract Button PlayCardButton { get; }
        public abstract Button DrawCardButton { get; }
        public abstract Button EndTurnButton { get; }
        public abstract Button PauseButton { get; }
        public abstract TextMeshProUGUI Player1HandSizeText { get; }
        public abstract TextMeshProUGUI Player2HandSizeText { get; }
        public abstract TextMeshProUGUI PlayerMessageText { get; }
        public abstract TextMeshProUGUI OpponentMessageText { get; }
        public abstract GameObject ColorSelectionPanel { get; }
        public abstract Button SelectRedButton { get; }
        public abstract Button SelectBlueButton { get; }
        public abstract Button SelectGreenButton { get; }
        public abstract Button SelectYellowButton { get; }
        public abstract TextMeshProUGUI ChainStatusText { get; }
        public abstract Button EndTakiSequenceButton { get; }
        public abstract TextMeshProUGUI TakiSequenceStatusText { get; }

        protected virtual void Start() {
            // Check if this UI manager should be active based on game mode
            if (!ShouldBeActive()) {
                TakiLogger.LogSystem($"New UI Architecture - {GetType().Name} DISABLED (not active for current game mode)");
                gameObject.SetActive(false);
                return;
            }

			TakiLogger.LogInfo($"New UI Architecture starting", TakiLogger.LogCategory.UI);
            ConnectButtonEvents();
            SetupInitialState();
        }

		#region Button Event Management

		/// <summary>
		/// Connect button events to internal handlers - with validation and TAKI sequence support
		/// </summary>
		protected virtual void ConnectButtonEvents () {
			TakiLogger.LogUI ("Connecting button events with STRICT FLOW validation...");

			if (PlayCardButton != null) {
				PlayCardButton.onClick.AddListener (() => {
					TakiLogger.LogTurnFlow ("=== PLAY CARD BUTTON CLICKED ===", TakiLogger.LogLevel.Debug);
					TakiLogger.LogTurnFlow ($"Button enabled state: {playButtonEnabled}", TakiLogger.LogLevel.Trace);
					TakiLogger.LogTurnFlow ($"Button interactable: {PlayCardButton.interactable}", TakiLogger.LogLevel.Trace);

					// Check actual button state instead of internal tracking (fixes duplicate handler issue)
					if (!PlayCardButton.interactable) {
						TakiLogger.LogWarning ("PLAY CARD clicked but button is not interactable!", TakiLogger.LogCategory.TurnFlow);
						ShowPlayerMessage ("Cannot play card right now!");
						return;
					}

					OnPlayCardClicked?.Invoke ();
				});
				TakiLogger.LogSystem ("Play Card button event connected", TakiLogger.LogLevel.Trace);
			} else {
				TakiLogger.LogError ("Play Card button is NULL!", TakiLogger.LogCategory.UI);
			}

			if (DrawCardButton != null) {
				DrawCardButton.onClick.AddListener (() => {
					TakiLogger.LogTurnFlow ("=== DRAW CARD BUTTON CLICKED ===", TakiLogger.LogLevel.Debug);
					TakiLogger.LogTurnFlow ($"Button enabled state: {drawButtonEnabled}", TakiLogger.LogLevel.Trace);
					TakiLogger.LogTurnFlow ($"Button interactable: {DrawCardButton.interactable}", TakiLogger.LogLevel.Trace);

					// Check actual button state instead of internal tracking (fixes duplicate handler issue)
					if (!DrawCardButton.interactable) {
						TakiLogger.LogWarning ("DRAW CARD clicked but button is not interactable!", TakiLogger.LogCategory.TurnFlow);
						ShowPlayerMessage ("Cannot draw card right now!");
						return;
					}

					OnDrawCardClicked?.Invoke ();
				});
				TakiLogger.LogSystem ("Draw Card button event connected", TakiLogger.LogLevel.Trace);
			} else {
				TakiLogger.LogError ("Draw Card button is NULL!", TakiLogger.LogCategory.UI);
			}

			if (EndTurnButton != null) {
				EndTurnButton.onClick.AddListener (() => {
					TakiLogger.LogTurnFlow ("=== END TURN BUTTON CLICKED ===");
					TakiLogger.LogTurnFlow ($"Button enabled state: {endTurnButtonEnabled}", TakiLogger.LogLevel.Trace);
					TakiLogger.LogTurnFlow ($"Button interactable: {EndTurnButton.interactable}", TakiLogger.LogLevel.Trace);

					// Check actual button state instead of internal tracking (fixes duplicate handler issue)
					if (!EndTurnButton.interactable) {
						TakiLogger.LogWarning ("END TURN clicked but button is not interactable!", TakiLogger.LogCategory.TurnFlow);
						ShowPlayerMessage ("You must take an action first!");
						return;
					}

					// Immediately disable all buttons to prevent multiple clicks
					TakiLogger.LogTurnFlow ("IMMEDIATELY disabling all buttons after END TURN click", TakiLogger.LogLevel.Trace);
					UpdateStrictButtonStates (false, false, false);

					OnEndTurnClicked?.Invoke ();
				});
				TakiLogger.LogSystem ("End Turn button event connected", TakiLogger.LogLevel.Trace);
			} else {
				TakiLogger.LogError ("End Turn button is NULL!", TakiLogger.LogCategory.UI);
			}

			// PHASE 8B: Connect End TAKI Sequence button
			if (EndTakiSequenceButton != null) {
				EndTakiSequenceButton.onClick.AddListener (() => {
					TakiLogger.LogTurnFlow ("=== END TAKI SEQUENCE BUTTON CLICKED ===", TakiLogger.LogLevel.Debug);
					TakiLogger.LogTurnFlow ($"Button interactable: {EndTakiSequenceButton.interactable}", TakiLogger.LogLevel.Trace);

					if (!EndTakiSequenceButton.interactable) {
						TakiLogger.LogWarning ("END TAKI SEQUENCE clicked but button should be disabled!", TakiLogger.LogCategory.TurnFlow);
						ShowPlayerMessage ("Cannot end sequence right now!");
						return;
					}

					OnEndTakiSequenceClicked?.Invoke ();
				});
				TakiLogger.LogSystem ("End TAKI Sequence button event connected", TakiLogger.LogLevel.Trace);
			} else {
				TakiLogger.LogWarning ("Btn_Player1EndTakiSequence is NULL - TAKI sequence ending will not work!", TakiLogger.LogCategory.UI);
			}

			// Color selection buttons
			if (SelectRedButton != null) {
				SelectRedButton.onClick.AddListener (() => SelectColor (CardColor.Red));
			}
			if (SelectBlueButton != null) {
				SelectBlueButton.onClick.AddListener (() => SelectColor (CardColor.Blue));
			}
			if (SelectGreenButton != null) {
				SelectGreenButton.onClick.AddListener (() => SelectColor (CardColor.Green));
			}
			if (SelectYellowButton != null) {
				SelectYellowButton.onClick.AddListener (() => SelectColor (CardColor.Yellow));
			}

			TakiLogger.LogSystem ("All button events connected with strict flow validation", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Setup initial UI state
		/// </summary>
		protected virtual void SetupInitialState() {
			// Hide color selection initially
			if (ColorSelectionPanel != null) {
                ColorSelectionPanel.SetActive(false);
            }

			// Set initial color indicator
			if (CurrentColorIndicator != null) {
                CurrentColorIndicator.color = wildColor;
            }

			// Start with all buttons disabled (safe state)
			UpdateStrictButtonStates (false, false, false);
        }

		#endregion

		#region Button State Management

		/// <summary>
		/// ENHANCED: Strict button state control with comprehensive logging
		/// </summary>
		/// <param name="enablePlay">Enable/disable PLAY button</param>
		/// <param name="enableDraw">Enable/disable DRAW button</param>
		/// <param name="enableEndTurn">Enable/disable END TURN button</param>
		public virtual void UpdateStrictButtonStates(bool enablePlay, bool enableDraw, bool enableEndTurn) {
            TakiLogger.LogTurnFlow("=== UPDATING STRICT BUTTON STATES ===", TakiLogger.LogLevel.Debug);
            TakiLogger.LogTurnFlow($"PLAY: {(enablePlay ? "ENABLED" : "DISABLED")}", TakiLogger.LogLevel.Debug);
            TakiLogger.LogTurnFlow($"DRAW: {(enableDraw ? "ENABLED" : "DISABLED")}", TakiLogger.LogLevel.Debug);
            TakiLogger.LogTurnFlow($"END TURN: {(enableEndTurn ? "ENABLED" : "DISABLED")}", TakiLogger.LogLevel.Debug);

			// Update internal state tracking
			playButtonEnabled = enablePlay;
            drawButtonEnabled = enableDraw;
            endTurnButtonEnabled = enableEndTurn;

            if (PlayCardButton != null) {
                PlayCardButton.interactable = enablePlay;
				TakiLogger.LogTurnFlow ($"Play Card button updated: {(enablePlay ? "ENABLED" : "DISABLED")}", TakiLogger.LogLevel.Trace);
			}

			if (DrawCardButton != null) {
                DrawCardButton.interactable = enableDraw;
				TakiLogger.LogTurnFlow ($"Draw Card button updated: {(enableDraw ? "ENABLED" : "DISABLED")}", TakiLogger.LogLevel.Trace);
			}

			if (EndTurnButton != null) {
                EndTurnButton.interactable = enableEndTurn;
				TakiLogger.LogTurnFlow ($"End Turn button updated: {(enableEndTurn ? "ENABLED" : "DISABLED")}", TakiLogger.LogLevel.Trace);
			}

			// Pause button should always be available 
			if (PauseButton != null) {
                PauseButton.interactable = true;
            }

			TakiLogger.LogTurnFlow ("Strict button state update complete", TakiLogger.LogLevel.Debug);
		}

		/// <summary> 
		/// Force enable END TURN button after successful action
		/// Used when action was successful and player must end turn
		/// </summary>
		public virtual void ForceEnableEndTurn() {
			TakiLogger.LogTurnFlow ("=== FORCE ENABLING END TURN BUTTON ===", TakiLogger.LogLevel.Debug);
			TakiLogger.LogTurnFlow ("Action was successful - player must now END TURN", TakiLogger.LogLevel.Info);

			// Enable only END TURN button
			UpdateStrictButtonStates (false, false, true);
        }

		#endregion

		#region Display Updates

		/// <summary>
		/// Update turn display
		/// </summary>
		public virtual void UpdateTurnDisplay(TurnState turnState) {
            TakiLogger.LogUI($"=== UPDATING TURN DISPLAY for {turnState} ===", TakiLogger.LogLevel.Debug);

            if (TurnIndicatorText != null) {
                string turnMessage = GetTurnMessage(turnState);
				TakiLogger.LogUI ($"Turn indicator text: '{turnMessage}'", TakiLogger.LogLevel.Trace);
			}

			// Button states are controlled by GameManager's strict flow system
			// Do not automatically update button states here
			TakiLogger.LogTurnFlow ("Turn display updated - button states controlled by strict flow system", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Update current color indicator
		/// </summary>
		/// <param name="activeColor">Current active color</param>
		public virtual void UpdateActiveColorDisplay(CardColor activeColor) {
            if (CurrentColorIndicator != null) {
                CurrentColorIndicator.color = GetColorForCardColor(activeColor);
            }
        }

		/// <summary>
		/// Update hand size displays
		/// </summary>
		/// <param name="player1HandSize">Player 1 (human) hand size</param>
		/// <param name="player2HandSize">Player 2 (opponent) hand size</param>
		public virtual void UpdateHandSizeDisplay (int player1HandSize, int player2HandSize) {
			if (Player1HandSizeText != null) {
				Player1HandSizeText.text = $"Your Cards: {player1HandSize}";
			}

			if (Player2HandSizeText != null) {
				Player2HandSizeText.text = $"Opponent Cards: {player2HandSize}";
			}
		}

		/// <summary>
		/// Get turn message from turn state
		/// </summary>
		protected virtual string GetTurnMessage(TurnState turnState) {
			// Check game status first through GameStateManager if available
			if (Application.isPlaying) {
                GameStateManager gameState = FindObjectOfType<GameStateManager>();
                if (gameState != null) {
                    if (gameState.gameStatus == GameStatus.Paused) {
                        return "Game Paused";
                    } else if (gameState.gameStatus == GameStatus.GameOver) {
                        return "Game Over";
                    }
                }
            }

            switch (turnState) {
                case TurnState.PlayerTurn:
                    return "Your Turn";
                case TurnState.ComputerTurn:
                    return "Opponent's Turn";
                case TurnState.Neutral:
                    return "Game Setup";
                default:
                    return "Unknown State";
            }
        }

		/// <summary>
		/// Convert CardColor to Unity Color
		/// </summary>
		protected virtual Color GetColorForCardColor(CardColor cardColor) {
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

		#endregion

		#region Message System

		/// <summary>
		/// Show immediate action feedback (short duration, high priority)
		/// </summary>
		/// <param name="message">Urgent message to show</param>
		/// <param name="toPlayer">If true, show to player; if false, show to opponent area</param>
		public virtual void ShowImmediateFeedback (string message, bool toPlayer = true) {
			if (toPlayer) {
				ShowPlayerMessageTimed (message, 2.0f);
			} else {
				ShowOpponentMessageTimed (message, 2.0f);
			}

			TakiLogger.LogUI ($"Immediate feedback: '{message}' -> {(toPlayer ? "Player" : "Opponent")}");
		}

		/// <summary>
		/// Show message to player (instructions, warnings, guidance)
		/// </summary>
		/// <param name="message">Message to show</param>
		public virtual void ShowPlayerMessage(string message) {
            if (PlayerMessageText != null) {
                PlayerMessageText.text = message;
            }
        }

		/// <summary>
		/// Show opponent action message (Opponent thinking, actions)
		/// UPDATED: Now uses opponentMessageText instead of gameMessageText
		/// </summary>
		/// <param name="message">Message to show</param>
		public virtual void ShowOpponentMessage(string message) {
            if (OpponentMessageText != null) {
                OpponentMessageText.text = message;
            }
        }

		/// <summary> 
		/// Show player message with duration to prevent overwrites
		/// </summary>
		/// <param name="message">Message to show</param>
		/// <param name="duration">How long to show before clearing (0 = permanent)</param>
		public virtual void ShowPlayerMessageTimed(string message, float duration = 3.0f) {
            if (PlayerMessageText != null) {
                PlayerMessageText.text = message;
                TakiLogger.LogUI($"Player message: '{message}' (duration: {duration}s)", TakiLogger.LogLevel.Trace);

                if (duration > 0) {
					// Cancel any existing clear operations
					CancelInvoke (nameof(ClearPlayerMessage));
					// Schedule clear
					Invoke (nameof(ClearPlayerMessage), duration);
                }
            }
        }

		/// <summary>
		/// Show opponent message with duration to prevent overwrites
		/// </summary>
		/// <param name="message">Message to show</param>
		/// <param name="duration">How long to show before clearing (0 = permanent)</param>
		public virtual void ShowOpponentMessageTimed(string message, float duration = 3.0f) {
            if (OpponentMessageText != null) {
                OpponentMessageText.text = message;
                TakiLogger.LogUI($"Opponent message: '{message}' (duration: {duration}s)", TakiLogger.LogLevel.Trace);

                if (duration > 0) {
					// Cancel any existing clear operations
					CancelInvoke (nameof(ClearOpponentMessage));
					// Schedule clear
					Invoke (nameof(ClearOpponentMessage), duration);
                }
            }
        }

		/// <summary>
		/// Clear player message
		/// </summary>
		protected virtual void ClearPlayerMessage() {
            if (PlayerMessageText != null) {
                PlayerMessageText.text = "";
				TakiLogger.LogUI ("Player message cleared", TakiLogger.LogLevel.Trace);
			}
		}

		/// <summary>
		/// Clear opponent message
		/// </summary>
		protected virtual void ClearOpponentMessage() {
            if (OpponentMessageText != null) {
                OpponentMessageText.text = "";
				TakiLogger.LogUI ("Opponent message cleared", TakiLogger.LogLevel.Trace);
			}
		}

		/// <summary>
		/// Show opponent action feedback
		/// </summary>
		public virtual void ShowOpponentAction (string action) {
			ShowOpponentMessage ($"Opponent {action}");
			TakiLogger.LogNetwork ($"Opponent action displayed: {action}", TakiLogger.LogLevel.Trace);
		}

		#endregion

		#region Color Selection

		/// <summary>
		/// Show/hide color selection panel
		/// </summary>
		/// <param name="show">Whether to show the color selection</param>
		public virtual void ShowColorSelection(bool show) {
            if (ColorSelectionPanel != null) {
                ColorSelectionPanel.SetActive(show);
            }

			// Disable all action buttons during color selection
			if (show) {
                TakiLogger.LogUI("Color selection active - disabling all action buttons", TakiLogger.LogLevel.Info);
                UpdateStrictButtonStates(false, false, true);
            }
        }

		/// <summary>
		/// Handle color selection, keep panel visible
		/// </summary> 
		/// <param name="selectedColor">Selected color</param>
		protected virtual void SelectColor(CardColor selectedColor) {
			// Update color indicator
			UpdateActiveColorDisplay (selectedColor);

			// Notify external systems
			OnColorSelected?.Invoke(selectedColor);
            
            TakiLogger.LogUI($"Player selected color: {selectedColor}", TakiLogger.LogLevel.Trace);

			// Show feedback that color was selected but player can choose again
			ShowPlayerMessage ($"Color set to {selectedColor} - click END TURN when ready!");
        }

		#endregion

		#region Special Card Effects

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
				ShowOpponentMessageTimed (message, 4.0f);
			}
		}

		/// <summary>
		/// Show sequence ended message
		/// </summary>
		/// <param name="finalCardCount">Number of cards in final sequence</param>
		/// <param name="sequenceColor">Color of the sequence</param>
		/// <param name="who">Who ended the sequence</param>
		public virtual void ShowSequenceEndedMessage (int finalCardCount, CardColor sequenceColor, PlayerType who) {

			string playerMessage = who == PlayerType.Human ?
				$"Sequence ended! You played {finalCardCount} {sequenceColor} cards" :
				$"";

			string computerMessage = who == PlayerType.Computer ?
				$"Opponent ended sequence: {finalCardCount} {sequenceColor} cards" :
				"";

			ShowPlayerMessageTimed (playerMessage, 3.0f);
			ShowOpponentMessageTimed (computerMessage, 3.0f);

			// Clear sequence status
			HideTakiSequenceStatus ();
		}

		/// <summary>
		/// Show sequence progress message during TAKI sequence
		/// </summary>
		/// <param name="cardCount">Current number of cards in sequence</param>
		/// <param name="sequenceColor">Color of the sequence</param>
		/// <param name="who">Who is playing the sequence</param>
		public virtual void ShowSequenceProgressMessage (int cardCount, CardColor sequenceColor, PlayerType
		who) {
			TakiLogger.LogUI ($"Sequence progress: {cardCount} {sequenceColor} cards by {who}",
		TakiLogger.LogLevel.Info);

			// Default implementation - basic message
			ShowPlayerMessageTimed ($"TAKI Sequence: {cardCount} {sequenceColor} cards played", 2.0f);
		}

		/// <summary>
		/// Show special card effect message with appropriate routing
		/// </summary>
		/// <param name="cardType">Type of special card</param>
		/// <param name="playedBy">Who played the card</param>
		/// <param name="effectDescription">Description of the effect</param>
		public virtual void ShowSpecialCardEffect (CardType cardType, PlayerType playedBy, string effectDescription) {
			TakiLogger.LogUI ($"Special card effect: {cardType} by {playedBy} - {effectDescription}", TakiLogger.LogLevel.Info);

			// Default implementation - basic message
			ShowPlayerMessageTimed ($"{cardType} played: {effectDescription}", 4.0f);
		}

		/// <summary>
		/// Show PlusTwo chain progress message
		/// </summary>
		/// <param name="chainCount">Number of PlusTwo cards in chain</param>
		/// <param name="accumulatedDraw">Total cards to draw</param>
		/// <param name="who">Who added to the chain</param>
		public virtual void ShowChainProgressMessage (int chainCount, int accumulatedDraw, PlayerType who) {
			TakiLogger.LogUI ($"Chain progress: {chainCount} PlusTwo cards by {who}, draw {accumulatedDraw}",
		TakiLogger.LogLevel.Info);

			// Default implementation - basic message
			ShowPlayerMessageTimed ($"PlusTwo Chain: {chainCount} cards -> Draw {accumulatedDraw}", 3.0f);
		}

		/// <summary>
		/// Show PlusTwo chain broken message
		/// </summary>
		/// <param name="cardsDrawn">Total cards that were drawn</param>
		/// <param name="who">Who broke the chain by drawing</param>
		public virtual void ShowChainBrokenMessage (int cardsDrawn, PlayerType who) {
			TakiLogger.LogUI ($"Chain broken: {cardsDrawn} cards drawn by {who}", TakiLogger.LogLevel.Info);

			// Default implementation - basic message
			ShowPlayerMessageTimed ($"PlusTwo chain broken: Drew {cardsDrawn} cards", 3.0f);
		}

		/// <summary>
		/// Show PlusTwo chain status with progressive messaging 
		/// </summary>
		/// <param name="chainCount">Number of PlusTwo cards in chain</param>
		/// <param name="accumulatedDraw">Total cards to draw</param>
		/// <param name="isPlayerTurn">True if it's player's turn to respond</param>
		public virtual void ShowPlusTwoChainStatus(int chainCount, int accumulatedDraw, bool isPlayerTurn) {
			if (ChainStatusText != null) {
				string statusMessage;

				if (isPlayerTurn) {
					statusMessage = $"PlusTwo Chain: {chainCount} cards -> Draw {accumulatedDraw} or play PlusTwo";
				} else {
					statusMessage = $"PlusTwo Chain: {chainCount} cards -> Opponent must respond";
				}

				ChainStatusText.text = statusMessage;
			}

			TakiLogger.LogUI ($"Chain status displayed: {chainCount} cards, {accumulatedDraw} draw, PlayerTurn={isPlayerTurn}", TakiLogger.LogLevel.Info);
		}

		/// <summary>
		/// Hide PlusTwo chain status
		/// </summary>
		public virtual void HidePlusTwoChainStatus() {
            if (ChainStatusText != null) {
                ChainStatusText.text = "";
            }

			TakiLogger.LogUI ("Chain status hidden", TakiLogger.LogLevel.Trace);
        }

		/// <summary>
		/// Enable/disable the End TAKI Sequence button
		/// </summary>
		/// <param name="enable">Whether to enable the button</param>
		public virtual void EnableEndTakiSequenceButton (bool enable) {
			if (EndTakiSequenceButton != null) {
				EndTakiSequenceButton.interactable = enable;
				EndTakiSequenceButton.gameObject.SetActive (enable);
				TakiLogger.LogUI ($"End TAKI Sequence button {(enable ? "ENABLED & VISIBLE" : "DISABLED & HIDDEN")}");
			} else {
				TakiLogger.LogError ("CRITICAL: EndTakiSequenceButton is NULL! Button not assigned in Inspector!", TakiLogger.LogCategory.UI);
			}
		}

		/// <summary>
		/// Display TAKI sequence status - no context logic, just shows what the concrete class tells it to show
		/// </summary>
		/// <param name="sequenceColor">Color required for sequence</param>
		/// <param name="cardCount">Number of cards in sequence</param>
		/// <param name="isPlayerTurn">True if it's player's turn</param>
		public virtual void ShowTakiSequenceStatus (CardColor sequenceColor, int cardCount, bool isPlayerTurn) {
			if (TakiSequenceStatusText != null) {
				string message = BuildTakiSequenceMessage (sequenceColor, cardCount, isPlayerTurn);
				TakiSequenceStatusText.text = message;
				TakiSequenceStatusText.gameObject.SetActive (true);
				TakiLogger.LogUI ($"TAKI sequence status: '{message}'");
			} else {
				TakiLogger.LogError ("CRITICAL: TakiSequenceStatusText is NULL! Text element not assigned in Inspector!",
		TakiLogger.LogCategory.UI);
			}
		}

		/// <summary>
		/// Build a basic TAKI sequence message - overridden by concrete classes for context-specific messages
		/// </summary>
		protected virtual string BuildTakiSequenceMessage (CardColor sequenceColor, int cardCount, bool isPlayerTurn) {
			return isPlayerTurn
				? $"TAKI Sequence: {cardCount} cards -> Play {sequenceColor} cards or End Sequence"
				: $"TAKI Sequence: {cardCount} cards -> Opponent playing {sequenceColor} cards";
		}

		/// <summary>
		/// Hides sequence status
		/// </summary>
		public virtual void HideTakiSequenceStatus() {
			if (TakiSequenceStatusText != null) {
				TakiSequenceStatusText.text = "";
				TakiSequenceStatusText.gameObject.SetActive (false);
				TakiLogger.LogUI ("TAKI sequence status hidden", TakiLogger.LogLevel.Trace);
			} else {
				TakiLogger.LogError ("CRITICAL: takiSequenceStatusText is NULL when trying to hide!", TakiLogger.LogCategory.UI);
			}
		}

		#endregion

		#region Game State Management

		public virtual void UpdateAllDisplays(TurnState turnState, GameStatus gameStatus, InteractionState interactionState, CardColor activeColor) {
            TakiLogger.LogUI("=== UPDATING ALL DISPLAYS (BASE) ===", TakiLogger.LogLevel.Debug);

            UpdateTurnDisplay(turnState);
            UpdateActiveColorDisplay(activeColor);

			// Handle special interaction states
			if (interactionState == InteractionState.ColorSelection) {
                ShowColorSelection(true);
            } else {
                ShowColorSelection(false);
            }

            if (interactionState == InteractionState.TakiSequence) {
                GameStateManager gameState = FindObjectOfType<GameStateManager>();

                if (gameState != null && gameState.IsInTakiSequence) {
                    // Let concrete classes decide whether to enable the button based on their context
                    EnableEndTakiSequenceButton(true);

                    // Show sequence status with current game state
                    bool isPlayerTurn = gameState.IsPlayerTurn;
                    int cardCount = gameState.NumberOfSequenceCards;
                    CardColor sequenceColor = gameState.TakiSequenceColor;
                    ShowTakiSequenceStatus(sequenceColor, cardCount, isPlayerTurn);
                } else {
                    EnableEndTakiSequenceButton(false);
                    HideTakiSequenceStatus();
                }
            } else {
                EnableEndTakiSequenceButton(false);
                HideTakiSequenceStatus();
            }

            switch (gameStatus) {
                case GameStatus.Paused:
                    HandlePausedState();
                    break;
                case GameStatus.GameOver:
                    HandleGameOverState();
                    break;
                case GameStatus.Active:
                    HandleActiveState(turnState);
                    break;
            }
        }

		/// <summary>
		/// Handle UI state when game is paused
		/// </summary>
		protected virtual void HandlePausedState() {
			TakiLogger.LogUI ("Handling paused game state", TakiLogger.LogLevel.Debug);

			// Disable all gameplay buttons including TAKI sequence button
			UpdateStrictButtonStates (false, false, false);
            EnableEndTakiSequenceButton(false);

			// Show pause message to user
			ShowPlayerMessage ("Game Paused");
            ShowOpponentMessage("Game Paused");

			// Pause button should still be available (it becomes Continue when paused)
			if (PauseButton != null) {
                PauseButton.interactable = true;
            }
        }

		/// <summary>
		/// Handle UI state when game is over
		/// </summary>
		protected virtual void HandleGameOverState() {
			TakiLogger.LogUI ("Handling game over state", TakiLogger.LogLevel.Debug);

			// Disable all gameplay buttons including TAKI sequence button
			UpdateStrictButtonStates (false, false, false);
            EnableEndTakiSequenceButton(false);

			// Pause button should be disabled
			if (PauseButton != null) {
                PauseButton.interactable = false;
            }

			// Winner message will be handled by GameEndManager
		}

		/// <summary>
		/// Handle UI state when game is active
		/// </summary>
		/// <param name="turnState">Current turn state</param>
		protected virtual void HandleActiveState(TurnState turnState) {
			TakiLogger.LogUI ("Handling active game state",TakiLogger.LogLevel.Debug);

			// Pause button should be available
			if (PauseButton != null) {
                PauseButton.interactable = true;
            }

			// Button states will be handled by strict turn flow system
			// Don't automatically enable buttons here - let GameManager control them
		}

		/// <summary>
		/// Reset UI for new game
		/// </summary>
		public virtual void ResetUIForNewGame () {
			TakiLogger.LogUI ("Resetting UI for new game (base implementation)", TakiLogger.LogLevel.Debug);

			// Reset turn display
			if (TurnIndicatorText != null) {
				TurnIndicatorText.text = "New Game";
			}

			// Reset color indicator
			UpdateActiveColorDisplay (CardColor.Wild);

			// Reset hand size displays
			UpdateHandSizeDisplay (0, 0);

			// Hide color selection
			ShowColorSelection (false);

			// Start with all buttons disabled for safety
			UpdateStrictButtonStates (false, false, false);

			// Clear messages
			ShowPlayerMessage ("Setting up game...");
			ShowOpponentMessage ("");

			TakiLogger.LogUI ("Base UI reset for new game complete");
		}

		#endregion

		#region Properties

		// Properties 
		public bool PlayButtonEnabled => playButtonEnabled;
        public bool DrawButtonEnabled => drawButtonEnabled;
        public bool EndTurnButtonEnabled => endTurnButtonEnabled;
        public bool EndTakiSequenceButtonEnabled => EndTakiSequenceButton != null && EndTakiSequenceButton.interactable;
        public bool IsColorSelectionActive => ColorSelectionPanel != null && ColorSelectionPanel.activeSelf;

        public virtual string GetButtonStateSummary() {
            return $"Play:{(playButtonEnabled ? "ON" : "OFF")}, Draw:{(drawButtonEnabled ? "ON" : "OFF")}, EndTurn:{(endTurnButtonEnabled ? "ON" : "OFF")}, EndSequence:{(EndTakiSequenceButtonEnabled ? "ON" : "OFF")}";
        }

        #endregion

        #region Game Mode Detection

        /// <summary>
        /// Determines if this UI manager should be active based on the current game mode
        /// Prevents duplicate event handlers by ensuring only the appropriate UI manager is active
        /// </summary>
        /// <returns>True if this UI manager should be active for the current game mode</returns>
        protected virtual bool ShouldBeActive() {
            // Find the GameManager to check current mode
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null) {
                TakiLogger.LogWarning($"Cannot determine game mode - GameManager not found. Defaulting {GetType().Name} to ACTIVE", TakiLogger.LogCategory.System);
                return true;
            }

            bool isMultiplayerMode = gameManager.IsMultiplayerMode;
            bool isThisSinglePlayerManager = this is SinglePlayerUIManager;
            bool isThisMultiPlayerManager = this is MultiPlayerUIManager;

            // SinglePlayerUIManager should only be active in single-player mode
            if (isThisSinglePlayerManager) {
                bool shouldBeActive = !isMultiplayerMode;
                TakiLogger.LogSystem($"SinglePlayerUIManager activity check: isMultiplayerMode={isMultiplayerMode}, shouldBeActive={shouldBeActive}");
                return shouldBeActive;
            }

            // MultiPlayerUIManager should only be active in multiplayer mode
            if (isThisMultiPlayerManager) {
                bool shouldBeActive = isMultiplayerMode;
                TakiLogger.LogSystem($"MultiPlayerUIManager activity check: isMultiplayerMode={isMultiplayerMode}, shouldBeActive={shouldBeActive}");
                return shouldBeActive;
            }

            // Unknown UI manager type - default to active with warning
            TakiLogger.LogWarning($"Unknown UI manager type: {GetType().Name}. Defaulting to ACTIVE", TakiLogger.LogCategory.System);
            return true;
        }

        #endregion

        #region Debug Methods

        [ContextMenu("Log UI State")]
        public virtual void LogCompleteUIState() {
            TakiLogger.LogDiagnostics("=== BASE UI STATE DEBUG ===");
            TakiLogger.LogDiagnostics($"Turn indicator: '{(TurnIndicatorText != null ? TurnIndicatorText.text : "NULL")}'");
            TakiLogger.LogDiagnostics($"Player message: '{(PlayerMessageText != null ? PlayerMessageText.text : "NULL")}'");
            TakiLogger.LogDiagnostics($"Opponent message: '{(OpponentMessageText != null ? OpponentMessageText.text : "NULL")}'");
            TakiLogger.LogDiagnostics($"Color selection active: {IsColorSelectionActive}");
            TakiLogger.LogDiagnostics($"Button States: {GetButtonStateSummary()}");
        }

        #endregion
    }
}