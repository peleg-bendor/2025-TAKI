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

		[Header ("Deck Pile Elements")]
		[SerializeField] private GameObject drawPilePanel;
		[SerializeField] private TextMeshProUGUI drawPileCountText;
		[SerializeField] private GameObject discardPilePanel;
		[SerializeField] private TextMeshProUGUI discardPileCountText;

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
		public override TextMeshProUGUI ComputerMessageText => computerMessageText;
		public override GameObject ColorSelectionPanel => colorSelectionPanel;
		public override Button SelectRedButton => selectRedButton;
		public override Button SelectBlueButton => selectBlueButton;
		public override Button SelectGreenButton => selectGreenButton;
		public override Button SelectYellowButton => selectYellowButton;
		public override TextMeshProUGUI ChainStatusText => chainStatusText;
		public override Button EndTakiSequenceButton => endTakiSequenceButton;
		public override TextMeshProUGUI TakiSequenceStatusText => takiSequenceStatusText;
		public override GameObject DrawPilePanel => drawPilePanel;
		public override TextMeshProUGUI DrawPileCountText => drawPileCountText;
		public override GameObject DiscardPilePanel => discardPilePanel;
		public override TextMeshProUGUI DiscardPileCountText => discardPileCountText;

		#endregion

		#region SinglePlayer-Specific Implementations

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
			ShowComputerMessageTimed ($"AI thinking: {message}", 2.0f);
			TakiLogger.LogUI ($"AI thinking message: {message}");
		}

		/// <summary>
		/// Show AI action result
		/// </summary>
		public void ShowAIAction (string action, string result = "") {
			string message = string.IsNullOrEmpty (result)
				? $"AI {action}"
				: $"AI {action}: {result}";

			ShowComputerMessageTimed (message, 3.0f);
			TakiLogger.LogUI ($"AI action message: {message}");
		}

		/// <summary>
		/// Show winner announcement for singleplayer
		/// </summary>
		public void ShowWinnerAnnouncement (PlayerType winner) {
			string winnerText = winner == PlayerType.Human ?
								"You Win!" : "Computer Wins!";

			if (turnIndicatorText != null) {
				turnIndicatorText.text = winnerText;
			}

			ShowPlayerMessage ("");
			ShowComputerMessage ("");

			UpdateStrictButtonStates (false, false, false);
			TakiLogger.LogGameState ("SinglePlayer game over - all buttons disabled");
		}

		/// <summary>
		/// Reset UI for new singleplayer game
		/// </summary>
		public void ResetUIForNewGame () {
			if (turnIndicatorText != null) {
				turnIndicatorText.text = "New Game";
			}

			UpdateActiveColorDisplay (CardColor.Wild);
			UpdateHandSizeDisplay (0, 0);
			ShowColorSelection (false);
			UpdateStrictButtonStates (false, false, false);

			TakiLogger.LogUI ("SinglePlayer UI reset for new game");
		}

		/// <summary>
		/// Show computer difficulty selection feedback
		/// </summary>
		public void ShowDifficultyFeedback (string difficulty) {
			ShowComputerMessageTimed ($"AI Difficulty: {difficulty}", 2.0f);
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