using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TakiGame {
    /// <summary>
    /// MultiPlayer UI Manager - manages Screen_MultiPlayerGame UI elements
    /// Inherits from BaseGameplayUIManager and implements MultiPlayer-specific functionality
    /// </summary>
    public class MultiPlayerUIManager : BaseGameplayUIManager {

        [Header("MultiPlayer UI References - Screen_MultiPlayerGame")]
        [Header("Turn Display")]
        [SerializeField] private TextMeshProUGUI turnIndicatorText;
        [SerializeField] private Image currentColorIndicator;

        [Header("Player Action Buttons")]
        [SerializeField] private Button playCardButton;
        [SerializeField] private Button drawCardButton;
        [SerializeField] private Button endTurnButton;
        [SerializeField] private Button pauseButton;

        [Header("Hand Size Displays")]
        [SerializeField] private TextMeshProUGUI player1HandSizeText;
        [SerializeField] private TextMeshProUGUI player2HandSizeText;

        [Header("Player Messages")]
        [SerializeField] private TextMeshProUGUI playerMessageText;
        [SerializeField] private TextMeshProUGUI computerMessageText;

        [Header("Color Selection")]
        [SerializeField] private GameObject colorSelectionPanel;
        [SerializeField] private Button selectRedButton;
        [SerializeField] private Button selectBlueButton;
        [SerializeField] private Button selectGreenButton;
        [SerializeField] private Button selectYellowButton;

        [Header("Special Features")]
        [SerializeField] private TextMeshProUGUI chainStatusText;
        [SerializeField] private Button endTakiSequenceButton;
        [SerializeField] private TextMeshProUGUI takiSequenceStatusText;

        #region Abstract Property Implementations

        public override TextMeshProUGUI TurnIndicatorText => turnIndicatorText;
        public override Image CurrentColorIndicator => currentColorIndicator;
        public override Button PlayCardButton => playCardButton;
        public override Button DrawCardButton => drawCardButton;
        public override Button EndTurnButton => endTurnButton;
        public override Button PauseButton => pauseButton;
        public override TextMeshProUGUI Player1HandSizeText => player1HandSizeText;
        public override TextMeshProUGUI Player2HandSizeText => player2HandSizeText;
        public override TextMeshProUGUI PlayerMessageText => playerMessageText;
        public override TextMeshProUGUI OpponentMessageText => computerMessageText;
        public override GameObject ColorSelectionPanel => colorSelectionPanel;
        public override Button SelectRedButton => selectRedButton;
        public override Button SelectBlueButton => selectBlueButton;
        public override Button SelectGreenButton => selectGreenButton;
        public override Button SelectYellowButton => selectYellowButton;
        public override TextMeshProUGUI ChainStatusText => chainStatusText;
        public override Button EndTakiSequenceButton => endTakiSequenceButton;
        public override TextMeshProUGUI TakiSequenceStatusText => takiSequenceStatusText;

		#endregion

		#region MultiPlayer-Specific Implementations

		public override void ShowSequenceEndedMessage (int finalCardCount, CardColor sequenceColor, PlayerType who) {
			if (IsLocalPlayer (who)) {
				ShowPlayerMessageTimed ($"Sequence ended! You played {finalCardCount} {sequenceColor} cards", 3.0f);
				ClearOpponentMessage ();
			} else {
				ShowOpponentMessageTimed ($"Opponent ended sequence: {finalCardCount} {sequenceColor} cards", 3.0f);
				ClearPlayerMessage ();
			}

			HideTakiSequenceStatus ();
		}

		public override void ShowSequenceProgressMessage (int cardCount, CardColor sequenceColor, PlayerType who) {
			if (IsLocalPlayer (who)) {
				ShowPlayerMessageTimed ($"Your TAKI: {cardCount} {sequenceColor} cards played", 2.0f);
				ClearOpponentMessage ();
			} else {
				ShowOpponentMessageTimed ($"Opponent TAKI: {cardCount} {sequenceColor} cards played", 2.0f);
				ClearPlayerMessage ();
			}
		}

		/// <summary>
		/// Show special card effect message with appropriate routing
		/// </summary>
		/// <param name="cardType">Type of special card</param>
		/// <param name="playedBy">Who played the card</param>
		/// <param name="effectDescription">Description of the effect</param>
		public override void ShowSpecialCardEffect (CardType cardType, PlayerType playedBy, string
		effectDescription) {
			if (IsLocalPlayer (playedBy)) {
				// Local player played special card
				ShowPlayerMessageTimed ($"You played {cardType}: {effectDescription}", 4.0f);
				ClearOpponentMessage ();
				if (cardType == CardType.Plus) {
					ShowPlayerMessageTimed ($"You played {cardType}: {effectDescription} - Take 1 more action!", 0f);
				} else if (cardType == CardType.Stop) {
					ShowOpponentMessageTimed ("STOP: Opponent's turn is skipped!", 3.0f);
				}
			} else {
				// Opponent played special card
				ShowOpponentMessageTimed ($"Opponent played {cardType}: {effectDescription}", 4.0f);
				ClearPlayerMessage ();
				if (cardType == CardType.Plus) {
					ShowOpponentMessageTimed ($"Opponent played {cardType}: {effectDescription} - Takes 1 more action!", 0f);
				} else if (cardType == CardType.Stop) {
					ShowPlayerMessageTimed ("STOP: Your turn is skipped!", 3.0f);
				}
			}
		}

		public override void ShowChainProgressMessage (int chainCount, int accumulatedDraw, PlayerType who) {
			if (IsLocalPlayer (who)) {
				ShowPlayerMessageTimed ($"You added to PlusTwo chain: {chainCount} cards -> Draw { accumulatedDraw}", 3.0f);
	  
		  ClearOpponentMessage ();
			} else {
				ShowOpponentMessageTimed ($"Opponent added to PlusTwo chain: {chainCount} cards -> Draw { accumulatedDraw}", 3.0f);
	  
		  ClearPlayerMessage ();
			}
		}

		public override void ShowChainBrokenMessage (int cardsDrawn, PlayerType who) {
			if (IsLocalPlayer (who)) {
				ShowPlayerMessageTimed ($"You broke PlusTwo chain: Drew {cardsDrawn} cards", 3.0f);
				ClearOpponentMessage ();
			} else {
				ShowOpponentMessageTimed ($"Opponent broke PlusTwo chain: Drew {cardsDrawn} cards", 3.0f);
				ClearPlayerMessage ();
			}
		}

		/// <summary>
		/// Determine if the given PlayerType represents the local player in multiplayer
		/// </summary>
		private bool IsLocalPlayer (PlayerType playerType) {
			GameManager gameManager = FindObjectOfType<GameManager> ();

			if (gameManager?.networkGameManager == null) {
				TakiLogger.LogError ("MultiPlayerUIManager: IsLocalPlayer - networkGameManager is NULL!", TakiLogger.LogCategory.UI);
				return playerType == PlayerType.Human;
			}

			// In multiplayer: PlayerType.Human represents the local player when it's their turn
			bool isMyTurn = gameManager.networkGameManager.IsMyTurn;
			return (playerType == PlayerType.Human) && isMyTurn;
		}

		/// <summary>
		/// Multiplayer context: Only enable for local player's sequences when it's their turn
		/// </summary>
		public override void EnableEndTakiSequenceButton (bool enable) {
			if (enable) {
				// Multiplayer logic: Only local player can end their own sequences
				bool canEndSequence = CanLocalPlayerEndSequence ();
				base.EnableEndTakiSequenceButton (enable && canEndSequence);

				if (!canEndSequence) {
					TakiLogger.LogUI ("BLOCKED: End Sequence button - not local player's sequence or not their turn", TakiLogger.LogLevel.Debug);
				}
			} else {
				base.EnableEndTakiSequenceButton (false);
			}
		}

		private bool CanLocalPlayerEndSequence () {
			GameManager gameManager = FindObjectOfType<GameManager> ();
			GameStateManager gameState = FindObjectOfType<GameStateManager> ();

            if (gameManager?.networkGameManager == null || gameState == null) {
                TakiLogger.LogError ("MultiPlayerUIManager: CanLocalPlayerEndSequence - networkGameManager or gameState are NULL!", TakiLogger.LogCategory.UI);
                return false;
            }

            if (!gameState.IsInTakiSequence) {
				return false;
			}

			bool isMyTurn = gameManager.networkGameManager.IsMyTurn;
			bool isHumanSequence = gameState.TakiSequenceInitiator == PlayerType.Human;

			// Can only end sequence if it's my turn AND I initiated it
			return isMyTurn && isHumanSequence;
		}

		/// <summary>
		/// Build Multiplayer-specific TAKI sequence message (Local vs Opponent with network awareness)
		/// </summary>
		protected override string BuildTakiSequenceMessage (CardColor sequenceColor, int cardCount, bool isPlayerTurn) {
			GameManager gameManager = FindObjectOfType<GameManager> ();
			GameStateManager gameState = FindObjectOfType<GameStateManager> ();

			if (gameManager?.networkGameManager != null && gameState != null && gameState.IsInTakiSequence) {
				PlayerType initiator = gameState.TakiSequenceInitiator;
				bool isMyTurn = gameManager.networkGameManager.IsMyTurn;

				// Determine if this is the local player's sequence
				bool isLocalPlayerSequence = (initiator == PlayerType.Human) && isMyTurn;

				if (isLocalPlayerSequence) {
					return $"Your TAKI Sequence: {cardCount} cards -> Play {sequenceColor} cards or End Sequence";
				} else {
					return $"Opponent TAKI Sequence: {cardCount} cards -> Opponent playing {sequenceColor} cards";
				}
			}

			// Fallback to base implementation
			return base.BuildTakiSequenceMessage (sequenceColor, cardCount, isPlayerTurn);
		}

		protected override string GetTurnMessage(TurnState turnState) {
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

        public override void UpdateHandSizeDisplay(int player1HandSize, int player2HandSize) {
            if (player1HandSizeText != null) {
                player1HandSizeText.text = $"Your Cards: {player1HandSize}";
            }

            if (player2HandSizeText != null) {
                player2HandSizeText.text = $"Opponent Cards: {player2HandSize}";
            }

            TakiLogger.LogNetwork($"Multiplayer hand sizes updated: Local={player1HandSize}, Opponent={player2HandSize}");
        }

        #endregion

        #region Multiplayer-Specific Methods

        /// <summary>
        /// Update hand size display for multiplayer
        /// </summary>
        public void UpdateHandSizeDisplayMultiplayer(int localHandSize, int opponentHandSize) {
            UpdateHandSizeDisplay(localHandSize, opponentHandSize);
        }

        /// <summary>
        /// Show network connection status
        /// </summary>
        public void ShowNetworkStatus(string status) {
            ShowPlayerMessageTimed($"Network: {status}", 3.0f);
            TakiLogger.LogNetwork($"Network status displayed: {status}");
        }

        /// <summary>
        /// Show multiplayer game ready feedback
        /// </summary>
        public void ShowMultiplayerGameReady() {
            ShowPlayerMessage("Game ready - waiting for your turn...");
            ShowOpponentMessage("Both players connected");
            TakiLogger.LogNetwork("Multiplayer game ready UI displayed");
        }

        /// <summary>
        /// Show opponent hand privacy feedback
        /// </summary>
        public void ShowOpponentHandPrivacy(int cardCount) {
            ShowOpponentMessageTimed($"Opponent has {cardCount} cards (hidden)", 2.0f);
            TakiLogger.LogNetwork($"Opponent hand privacy feedback: {cardCount} cards");
        }

        /// <summary>
        /// Show turn transition feedback for multiplayer
        /// </summary>
        public void ShowTurnTransitionMultiplayer(bool nowMyTurn, string previousAction = "") {
            if (nowMyTurn) {
                if (!string.IsNullOrEmpty(previousAction)) {
                    ShowPlayerMessage($"Opponent {previousAction} - now your turn!");
                } else {
                    ShowPlayerMessage("Your turn!");
                }
                ShowOpponentMessage("Waiting for your action...");
            } else {
                ShowPlayerMessage("Turn sent - waiting for opponent...");
                ShowOpponentMessage("Opponent is thinking...");
            }

            TakiLogger.LogNetwork($"Turn transition displayed: MyTurn={nowMyTurn}, PreviousAction={previousAction}");
        }

        /// <summary>
        /// Show network error with recovery options
        /// </summary>
        public void ShowNetworkError(string error, bool canRecover = false) {
            ShowPlayerMessage($"Network Error: {error}");

            if (canRecover) {
                ShowOpponentMessage("Attempting to reconnect...");
            } else {
                ShowOpponentMessage("Please check connection and restart");
            }

            TakiLogger.LogNetwork($"Network error displayed: {error}, CanRecover={canRecover}");
        }

        /// <summary>
        /// Show game initialization progress
        /// </summary>
        public void ShowGameInitProgress(string step) {
            ShowPlayerMessage($"Setting up: {step}");
            TakiLogger.LogNetwork($"Game init progress: {step}");
        }

        /// <summary>
        /// Show deck initialization complete
        /// </summary>
        public void ShowDeckInitialized(int drawCount, int discardCount, string startingCard) {
            ShowPlayerMessage("Game ready - deck initialized!");
            ShowOpponentMessage($"Draw: {drawCount}, Discard: {discardCount}, Start: {startingCard}");
            TakiLogger.LogNetwork($"Deck initialized display: Draw={drawCount}, Discard={discardCount}, Start={startingCard}");
        }

        /// <summary>
        /// Show hands initialized for multiplayer
        /// </summary>
        public void ShowHandsInitialized(int myHandSize, int opponentHandSize) {
            ShowPlayerMessage($"Hands dealt - you have {myHandSize} cards");
            ShowOpponentMessage($"Opponent has {opponentHandSize} cards (hidden)");
            TakiLogger.LogNetwork($"Hands initialized display: My={myHandSize}, Opponent={opponentHandSize}");
        }

        /// <summary>
        /// Show waiting for opponent feedback
        /// </summary>
        public void ShowWaitingForOpponent(string action = "to play") {
            ShowPlayerMessage($"Waiting for opponent {action}...");
            ShowOpponentMessage("Opponent's turn");
            TakiLogger.LogNetwork($"Waiting for opponent: {action}");
        }

        #endregion

        #region Debug Methods

        [ContextMenu("Debug MultiPlayer UI Components")]
        public void DebugMultiPlayerUIComponents() {
            TakiLogger.LogDiagnostics("=== MULTIPLAYER UI COMPONENTS DEBUG ===");

            TakiLogger.LogDiagnostics($"Turn Indicator: {(turnIndicatorText != null ? "ASSIGNED" : "NULL")}");
            TakiLogger.LogDiagnostics($"Player Message: {(playerMessageText != null ? "ASSIGNED" : "NULL")}");
            TakiLogger.LogDiagnostics($"Computer Message: {(computerMessageText != null ? "ASSIGNED" : "NULL")}");
            TakiLogger.LogDiagnostics($"Play Button: {(playCardButton != null ? "ASSIGNED" : "NULL")}");
            TakiLogger.LogDiagnostics($"Draw Button: {(drawCardButton != null ? "ASSIGNED" : "NULL")}");
            TakiLogger.LogDiagnostics($"End Turn Button: {(endTurnButton != null ? "ASSIGNED" : "NULL")}");
            TakiLogger.LogDiagnostics($"Color Selection Panel: {(colorSelectionPanel != null ? "ASSIGNED" : "NULL")}");
            TakiLogger.LogDiagnostics($"End TAKI Sequence Button: {(endTakiSequenceButton != null ? "ASSIGNED" : "NULL")}");

            TakiLogger.LogDiagnostics("=== END MULTIPLAYER UI DEBUG ===");
        }

        [ContextMenu("Test MultiPlayer UI Messages")]
        public void TestMultiPlayerUIMessages() {
            TakiLogger.LogDiagnostics("=== TESTING MULTIPLAYER UI MESSAGES ===");

            ShowNetworkStatus("Connected to opponent");
            ShowOpponentAction("played Red 5");
            ShowTurnTransitionMultiplayer(true, "drew a card");
            ShowOpponentHandPrivacy(6);

            TakiLogger.LogDiagnostics("MultiPlayer message test complete - check UI text elements");
        }

        public override void LogCompleteUIState() {
            TakiLogger.LogDiagnostics("=== MULTIPLAYER UI STATE DEBUG ===");
            base.LogCompleteUIState();
            
            if (player1HandSizeText != null) {
                TakiLogger.LogDiagnostics($"Local Hand Display: {player1HandSizeText.text}");
            }
            if (player2HandSizeText != null) {
                TakiLogger.LogDiagnostics($"Opponent Hand Display: {player2HandSizeText.text}");
            }

            TakiLogger.LogDiagnostics("=== END MULTIPLAYER UI DEBUG ===");
        }

        #endregion
    }
}