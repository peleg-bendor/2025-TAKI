using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TakiGame {
	/// <summary>
	/// SinglePlayer UI Manager - manages Screen_SinglePlayerGame UI elements
	/// Inherits from BaseGameplayUIManager and implements SinglePlayer-specific functionality
	/// </summary>
	public class SinglePlayerUIManager : BaseGameplayUIManager {

		[Header ("SinglePlayer UI References - Screen_SinglePlayerGame")]
		[Header ("Turn Display")]
		[SerializeField] private TextMeshProUGUI turnIndicatorText;
		[SerializeField] private Image currentColorIndicator;

		[Header ("Player Action Buttons")]
		[SerializeField] private Button playCardButton;
		[SerializeField] private Button drawCardButton;
		[SerializeField] private Button endTurnButton;
		[SerializeField] private Button pauseButton;

		[Header ("Hand Size Displays")]
		[SerializeField] private TextMeshProUGUI player1HandSizeText;
		[SerializeField] private TextMeshProUGUI player2HandSizeText;

		[Header ("Player Messages")]
		[SerializeField] private TextMeshProUGUI playerMessageText;
		[SerializeField] private TextMeshProUGUI computerMessageText;

		[Header ("Color Selection")]
		[SerializeField] private GameObject colorSelectionPanel;
		[SerializeField] private Button selectRedButton;
		[SerializeField] private Button selectBlueButton;
		[SerializeField] private Button selectGreenButton;
		[SerializeField] private Button selectYellowButton;

		[Header ("Special Features")]
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

		#region SinglePlayer-Specific Implementations

		public override void ShowSequenceEndedMessage (int finalCardCount, CardColor sequenceColor, PlayerType who) {
			if (who == PlayerType.Human) {
				ShowPlayerMessageTimed ($"Sequence ended! You played {finalCardCount} {sequenceColor} cards", 3.0f);
				ClearOpponentMessage ();
			} else {
				ShowOpponentMessageTimed ($"AI ended sequence: {finalCardCount} {sequenceColor} cards", 3.0f);
				ClearPlayerMessage ();
			}

			HideTakiSequenceStatus ();
		}

		public override void ShowSequenceProgressMessage (int cardCount, CardColor sequenceColor, PlayerType who) {
			if (who == PlayerType.Human) {
				ShowPlayerMessageTimed ($"Your TAKI: {cardCount} {sequenceColor} cards played", 2.0f);
				ClearOpponentMessage ();
			} else {
				ShowOpponentMessageTimed ($"AI TAKI: {cardCount} {sequenceColor} cards played", 2.0f);
				ClearPlayerMessage ();
			}
		}

		/// <summary>
		/// Show special card effect message with appropriate routing
		/// </summary>
		/// <param name="cardType">Type of special card</param>
		/// <param name="playedBy">Who played the card</param>
		/// <param name="effectDescription">Description of the effect</param>
		public override void ShowSpecialCardEffect (CardType cardType, PlayerType playedBy, string effectDescription) {
			TakiLogger.LogUI ($"Special card effect: {cardType} by {playedBy} - {effectDescription}", TakiLogger.LogLevel.Info);

			if (playedBy == PlayerType.Human) {
				// Human played special card
				ShowPlayerMessageTimed ($"You played {cardType}: {effectDescription}", 4.0f);
				ClearOpponentMessage ();
				if (cardType == CardType.Plus) {
					ShowPlayerMessageTimed ($"You played {cardType}: {effectDescription} - Take 1 more action!", 0f);
				} else if (cardType == CardType.Stop) {
					ShowOpponentMessageTimed ("STOP: Opponent's turn is skipped!", 3.0f);
				}
			} else {
				// AI played special card
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
			if (who == PlayerType.Human) {
				ShowPlayerMessageTimed ($"You added to PlusTwo chain: {chainCount} cards -> Draw { accumulatedDraw}", 3.0f);
	  
		  ClearOpponentMessage ();
			} else {
				ShowOpponentMessageTimed ($"AI added to PlusTwo chain: {chainCount} cards -> Draw { accumulatedDraw}", 3.0f);
	  
		  ClearPlayerMessage ();
			}
		}

		public override void ShowChainBrokenMessage (int cardsDrawn, PlayerType who) {
			if (who == PlayerType.Human) {
				ShowPlayerMessageTimed ($"You broke PlusTwo chain: Drew {cardsDrawn} cards", 3.0f);
				ClearOpponentMessage ();
			} else {
				ShowOpponentMessageTimed ($"AI broke PlusTwo chain: Drew {cardsDrawn} cards", 3.0f);
				ClearPlayerMessage ();
			}
		}

		/// <summary>
		/// SinglePlayer context: Only enable for human-initiated sequences
		/// </summary>
		public override void EnableEndTakiSequenceButton (bool enable) {
			if (enable) {
				// SinglePlayer logic: Only humans can end sequences (not AI)
				bool isHumanSequence = IsCurrentSequenceHumanInitiated ();
				base.EnableEndTakiSequenceButton (enable && isHumanSequence);

				if (!isHumanSequence) {
					TakiLogger.LogUI ("BLOCKED: End Sequence button - AI initiated sequence, player cannot end it");
				}
			} else {
				base.EnableEndTakiSequenceButton (false);
			}
		}

		private bool IsCurrentSequenceHumanInitiated () {
			GameStateManager gameState = FindObjectOfType<GameStateManager> ();
			return gameState != null && gameState.IsInTakiSequence &&
				   gameState.TakiSequenceInitiator == PlayerType.Human;
		}

		/// <summary>
		/// Build SinglePlayer-specific TAKI sequence message (Human vs AI)
		/// </summary>
		protected override string BuildTakiSequenceMessage (CardColor sequenceColor, int cardCount, bool isPlayerTurn) {
			GameStateManager gameState = FindObjectOfType<GameStateManager> ();

			if (gameState != null && gameState.IsInTakiSequence) {
				PlayerType initiator = gameState.TakiSequenceInitiator;

				if (initiator == PlayerType.Human) {
					return $"Your TAKI Sequence: {cardCount} cards -> Play {sequenceColor} cards or End Sequence";
				} else {
					return $"AI TAKI Sequence: {cardCount} cards -> AI playing {sequenceColor} cards";
				}
			}

			// Fallback to base implementation
			return base.BuildTakiSequenceMessage (sequenceColor, cardCount, isPlayerTurn);
		}

		protected override string GetTurnMessage (TurnState turnState) {
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

		public override void UpdateHandSizeDisplay (int player1HandSize, int player2HandSize) {
			if (player1HandSizeText != null) {
				player1HandSizeText.text = $"Your Cards: {player1HandSize}";
			}

			if (player2HandSizeText != null) {
				player2HandSizeText.text = $"Computer Cards: {player2HandSize}";
			}

			TakiLogger.LogUI ($"SinglePlayer hand sizes updated: Human={player1HandSize}, Computer={player2HandSize}");
		}

		#endregion

		#region SinglePlayer-Specific Methods

		/// <summary>
		/// Show AI thinking message
		/// </summary>
		public void ShowAIThinking (string message) {
			ShowOpponentMessageTimed ($"AI thinking: {message}", 2.0f);
			TakiLogger.LogUI ($"AI thinking message: {message}");
		}

		/// <summary>
		/// Show AI action result
		/// </summary>
		public void ShowAIAction (string action, string result = "") {
			string message = string.IsNullOrEmpty (result)
				? $"AI {action}"
				: $"AI {action}: {result}";

			ShowOpponentMessageTimed (message, 3.0f);
			TakiLogger.LogUI ($"AI action message: {message}");
		}

		/// <summary>
		/// Show computer difficulty selection feedback
		/// </summary>
		public void ShowDifficultyFeedback (string difficulty) {
			ShowOpponentMessageTimed ($"AI Difficulty: {difficulty}", 2.0f);
			TakiLogger.LogUI ($"Difficulty feedback: {difficulty}");
		}

		#endregion

		#region Debug Methods

		[ContextMenu ("Debug SinglePlayer UI Components")]
		public void DebugSinglePlayerUIComponents () {
			TakiLogger.LogDiagnostics ("=== SINGLEPLAYER UI COMPONENTS DEBUG ===");

			TakiLogger.LogDiagnostics ($"Turn Indicator: {(turnIndicatorText != null ? "ASSIGNED" : "NULL")}");
			TakiLogger.LogDiagnostics ($"Player Message: {(playerMessageText != null ? "ASSIGNED" : "NULL")}");
			TakiLogger.LogDiagnostics ($"Computer Message: {(computerMessageText != null ? "ASSIGNED" : "NULL")}");
			TakiLogger.LogDiagnostics ($"Play Button: {(playCardButton != null ? "ASSIGNED" : "NULL")}");
			TakiLogger.LogDiagnostics ($"Draw Button: {(drawCardButton != null ? "ASSIGNED" : "NULL")}");
			TakiLogger.LogDiagnostics ($"End Turn Button: {(endTurnButton != null ? "ASSIGNED" : "NULL")}");
			TakiLogger.LogDiagnostics ($"Color Selection Panel: {(colorSelectionPanel != null ? "ASSIGNED" : "NULL")}");
			TakiLogger.LogDiagnostics ($"End TAKI Sequence Button: {(endTakiSequenceButton != null ? "ASSIGNED" : "NULL")}");

			TakiLogger.LogDiagnostics ("=== END SINGLEPLAYER UI DEBUG ===");
		}

		[ContextMenu ("Test SinglePlayer UI Messages")]
		public void TestSinglePlayerUIMessages () {
			TakiLogger.LogDiagnostics ("=== TESTING SINGLEPLAYER UI MESSAGES ===");

			ShowPlayerMessage ("TEST: Player instruction message");
			ShowAIThinking ("calculating best move");
			ShowAIAction ("played Red 5", "good strategic choice");
			ShowDifficultyFeedback ("Normal");

			TakiLogger.LogDiagnostics ("SinglePlayer message test complete - check UI text elements");
		}

		public override void LogCompleteUIState () {
			TakiLogger.LogDiagnostics ("=== SINGLEPLAYER UI STATE DEBUG ===");
			base.LogCompleteUIState ();

			if (player1HandSizeText != null) {
				TakiLogger.LogDiagnostics ($"Human Hand Display: {player1HandSizeText.text}");
			}
			if (player2HandSizeText != null) {
				TakiLogger.LogDiagnostics ($"Computer Hand Display: {player2HandSizeText.text}");
			}

			TakiLogger.LogDiagnostics ("=== END SINGLEPLAYER UI DEBUG ===");
		}

		#endregion
	}
}