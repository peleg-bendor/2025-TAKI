using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// Manages turn switching and player turn logic
	/// NO game state management, NO AI logic, NO UI updates
	/// </summary>
	public class TurnManager : MonoBehaviour {

		[Header ("Turn Settings")]
		[Tooltip ("Current player whose turn it is")]
		public PlayerType currentPlayer = PlayerType.Human;

		[Tooltip ("Maximum time for human player turn (0 = no limit)")]
		public float playerTurnTimeLimit = 0f;

		[Tooltip ("Delay before computer takes its turn")]
		public float computerTurnDelay = 1.5f;

		[Header ("Dependencies")]
		[Tooltip ("Reference to game state manager")]
		public GameStateManager gameState;

		// Turn timing
		private float turnTimer = 0f;
		private bool isTurnTimerActive = false;
		private bool isComputerTurnScheduled = false;
		private float computerTurnStartTime = 0f;

		// Events
		public System.Action<PlayerType> OnTurnChanged;
		public System.Action<PlayerType> OnTurnTimeOut;
		public System.Action OnComputerTurnReady;

		void Update () {
			HandleTurnTiming ();
		}

		/// <summary>
		/// UPDATED: Enhanced start turn with pause state checking
		/// STOP skip logic now handled by GameManager flag system
		/// </summary>
		/// <param name="player">Player whose turn it is</param>
		public void StartTurn (PlayerType player) {
			// Check if game is paused - don't start turns during pause
			if (gameState != null && gameState.gameStatus == GameStatus.Paused) {
				TakiLogger.LogTurnManagement ($"Turn start cancelled for {player} - game is paused");
				return;
			}

			currentPlayer = player;

			// Update turn state in GameStateManager
			// This will trigger OnTurnStateChanged in GameManager
			// which now checks for STOP flags
			if (gameState != null) {
				if (player == PlayerType.Human) {
					gameState.ChangeTurnState (TurnState.PlayerTurn);
				} else {
					gameState.ChangeTurnState (TurnState.ComputerTurn);
				}
			}

			// Start turn timer for human players if time limit is set
			if (player == PlayerType.Human && playerTurnTimeLimit > 0) {
				StartTurnTimer ();
			} else {
				StopTurnTimer ();
			}

			// Schedule computer turn with delay 
			if (player == PlayerType.Computer) {
				ScheduleComputerTurn ();
			} else {
				CancelComputerTurn ();
			}

			TakiLogger.LogTurnManagement ($"Turn started for: {player}");
			OnTurnChanged?.Invoke (player);
		}

		/// <summary>
		/// UPDATED: End the current player's turn and switch to next player
		/// No longer handles STOP cards - that's done by flag system
		/// </summary>
		public void EndTurn () {
			TakiLogger.LogTurnManagement ($"Turn ended for: {currentPlayer}");
			SwitchToNextPlayer ();
		}

		/// <summary>
		/// UPDATED: Enhanced turn switching with pause awareness
		/// Clean turn switching - STOP handling moved to GameManager flag system
		/// </summary>
		void SwitchToNextPlayer () {
			// Don't switch if game is paused
			if (gameState != null && gameState.gameStatus == GameStatus.Paused) {
				TakiLogger.LogTurnManagement ("Turn switch cancelled - game is paused");
				return;
			}

			// In 2-player game, just alternate
			PlayerType nextPlayer = currentPlayer == PlayerType.Human ?
									PlayerType.Computer : PlayerType.Human;

			TakiLogger.LogTurnManagement ($"Switching from {currentPlayer} to {nextPlayer}");
			StartTurn (nextPlayer);
		}

		///// <summary>
		///// Skip the current player's turn (for Stop cards)
		///// </summary>
		//public void SkipTurn () {
		//	TakiLogger.LogTurnManagement ($"Turn skipped for: {currentPlayer}");
		//	// Switch to next player, then immediately switch again to skip them
		//	PlayerType skippedPlayer = currentPlayer == PlayerType.Human ?
		//							   PlayerType.Computer : PlayerType.Human;

		//	TakiLogger.LogTurnManagement ($"Player {skippedPlayer} turn is skipped");

		//	// Don't actually start the skipped player's turn, just switch back
		//	SwitchToNextPlayer ();
		//}

		/// <summary>
		/// Force turn to a specific player (for special cards)
		/// </summary>
		/// <param name="player">Player to give turn to</param>
		public void ForceTurnTo (PlayerType player) {
			TakiLogger.LogTurnManagement ($"Turn forced to: {player}");
			StartTurn (player);
		}

		/// <summary>
		/// Pause turns - preserve state for accurate resumption
		/// </summary>
		public void PauseTurns () {
			TakiLogger.LogTurnManagement ("=== PAUSING TURN SYSTEM ===");

			// Stop turn timer if active
			if (isTurnTimerActive) {
				StopTurnTimer ();
				TakiLogger.LogTurnManagement ($"Turn timer paused with {turnTimer:F1}s remaining");
			}

			// Cancel any scheduled computer turns
			if (isComputerTurnScheduled) {
				CancelComputerTurn ();
				TakiLogger.LogTurnManagement ("Scheduled computer turn cancelled");
			}

			// Set game state to neutral during pause but preserve current player info
			if (gameState != null) {
				gameState.ChangeTurnState (TurnState.Neutral);
				TakiLogger.LogTurnManagement ($"Turn state set to Neutral (preserving {currentPlayer})");
			}

			TakiLogger.LogTurnManagement ("Turn system paused successfully");
		}

		/// <summary>
		/// Resume turns - restore to exact previous state
		/// </summary>
		public void ResumeTurns () {
			TakiLogger.LogTurnManagement ("=== RESUMING TURN SYSTEM ===");
			TakiLogger.LogTurnManagement ($"Resuming turn for: {currentPlayer}");

			// Restart the turn for the current player
			StartTurn (currentPlayer);

			TakiLogger.LogTurnManagement ("Turn system resumed successfully");
		}

		/// <summary>
		/// Data structure for preserving turn state during pause
		/// </summary>
		[System.Serializable]
		public class TurnStateData {
			public PlayerType currentPlayer;
			public float turnTimeRemaining;
			public bool isTurnTimerActive;
			public bool isComputerTurnScheduled;
			public float computerTurnStartTime;
		}

		/// <summary>
		/// Restore turn state from pause data
		/// </summary>
		/// <param name="pausedTurnState">Previously saved turn state</param>
		public void RestoreTurnState (TurnStateData pausedTurnState) {
			TakiLogger.LogTurnManagement ("=== RESTORING TURN STATE FROM PAUSE ===");

			this.currentPlayer = pausedTurnState.currentPlayer;
			this.turnTimer = pausedTurnState.turnTimeRemaining;
			this.isTurnTimerActive = pausedTurnState.isTurnTimerActive;
			this.isComputerTurnScheduled = pausedTurnState.isComputerTurnScheduled;
			this.computerTurnStartTime = pausedTurnState.computerTurnStartTime;

			TakiLogger.LogTurnManagement ($"Turn state restored for {currentPlayer}");

			// Restart the appropriate turn
			StartTurn (currentPlayer);
		}

		/// <summary>
		/// Force cancel any ongoing computer operations
		/// </summary>
		public void ForceCancelComputerOperations () {
			TakiLogger.LogTurnManagement ("Force cancelling all computer operations");

			CancelComputerTurn ();

			// Cancel any invoke calls on this component
			CancelInvoke ();

			TakiLogger.LogTurnManagement ("All computer operations cancelled");
		}

		/// <summary>
		/// Check if turn system can be safely paused
		/// </summary>
		/// <returns>True if safe to pause</returns>
		public bool CanBeSafelyPaused () {
			// Safe to pause unless in critical turn transition
			bool isSafe = gameState != null &&
						  gameState.gameStatus == GameStatus.Active &&
						  gameState.interactionState == InteractionState.Normal;

			TakiLogger.LogTurnManagement ($"Turn system pause safety check: {isSafe}");
			return isSafe;
		}

		/// <summary>
		/// Handle turn timing and computer turn delays
		/// </summary>
		void HandleTurnTiming () {
			// Handle human player turn timeout
			if (isTurnTimerActive) {
				turnTimer -= Time.deltaTime;
				if (turnTimer <= 0) {
					TakiLogger.LogTurnManagement ("Human player turn timed out");
					OnTurnTimeOut?.Invoke (PlayerType.Human);
					StopTurnTimer ();
					EndTurn (); // Auto-end turn on timeout
				}
			}

			// Handle computer turn delay
			if (isComputerTurnScheduled) {
				if (Time.time >= computerTurnStartTime) {
					isComputerTurnScheduled = false;
					TakiLogger.LogTurnManagement ("Computer turn ready");
					OnComputerTurnReady?.Invoke ();
				}
			}
		}

		/// <summary>
		/// Start turn timer for human player
		/// </summary>
		void StartTurnTimer () {
			turnTimer = playerTurnTimeLimit;
			isTurnTimerActive = true;
			TakiLogger.LogTurnManagement ($"Turn timer started: {playerTurnTimeLimit} seconds");
		}

		/// <summary>
		/// Stop turn timer
		/// </summary>
		void StopTurnTimer () {
			isTurnTimerActive = false;
			turnTimer = 0f;
		}

		/// <summary>
		/// Schedule computer turn with delay
		/// </summary>
		void ScheduleComputerTurn () {
			isComputerTurnScheduled = true;
			computerTurnStartTime = Time.time + computerTurnDelay;
			TakiLogger.LogTurnManagement ($"Computer turn scheduled with {computerTurnDelay}s delay");
		}

		/// <summary>
		/// Cancel scheduled computer turn
		/// </summary>
		void CancelComputerTurn () {
			isComputerTurnScheduled = false;
		}

		/// <summary>
		/// Initialize turn manager with first player
		/// </summary>
		/// <param name="firstPlayer">Player who goes first</param>
		public void InitializeTurns (PlayerType firstPlayer = PlayerType.Human) {
			TakiLogger.LogTurnManagement ($"Turn system initialized. First player: {firstPlayer}");
			StartTurn (firstPlayer);
		}

		/// <summary>
		/// Check if it's a specific player's turn
		/// </summary>
		/// <param name="player">Player to check</param>
		/// <returns>True if it's that player's turn</returns>
		public bool IsPlayerTurn (PlayerType player) {
			return currentPlayer == player;
		}

		/// <summary>
		/// Get remaining time for current turn (if timer is active)
		/// </summary>
		/// <returns>Remaining seconds, or -1 if no timer</returns>
		public float GetRemainingTurnTime () {
			return isTurnTimerActive ? turnTimer : -1f;
		}

		/// <summary>
		/// Reset turn manager for new game
		/// </summary>
		public void ResetTurns () {
			StopTurnTimer ();
			CancelComputerTurn ();
			currentPlayer = PlayerType.Human;

			// Set game state to neutral during reset
			if (gameState != null) {
				gameState.ChangeTurnState (TurnState.Neutral);
			}

			TakiLogger.LogTurnManagement ("Turn manager reset");
		}

		/// <summary>
		/// Check if turns are currently active (not in neutral state)
		/// </summary>
		/// <returns>True if someone has an active turn</returns>
		public bool AreTurnsActive () {
			return gameState != null && gameState.turnState != TurnState.Neutral;
		}

		// Enhanced turn information 
		public string GetCurrentTurnInfo () {
			return $"Player: {currentPlayer}, Timer: {(isTurnTimerActive ? $"{turnTimer:F1}s" : "None")}, " +
				   $"Computer Pending: {isComputerTurnScheduled}";
		}

		// Properties
		public bool IsHumanTurn => currentPlayer == PlayerType.Human;
		public bool IsComputerTurn => currentPlayer == PlayerType.Computer;
		public bool IsTurnTimerActive => isTurnTimerActive;
		public bool IsComputerTurnPending => isComputerTurnScheduled;
		public PlayerType CurrentPlayer => currentPlayer;
		public float TurnTimeRemaining => GetRemainingTurnTime ();
		public bool IsPaused => gameState != null && gameState.gameStatus == GameStatus.Paused;
		public bool CanBePaused => CanBeSafelyPaused ();
	}
}