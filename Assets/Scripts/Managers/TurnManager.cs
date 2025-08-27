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
		/// Start a new turn for the specified player
		/// </summary>
		/// <param name="player">Player whose turn it is</param>
		public void StartTurn (PlayerType player) {
			currentPlayer = player;

			// Update turn state in GameStateManager
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

			Debug.Log ($"Turn started for: {player}");
			OnTurnChanged?.Invoke (player);
		}

		/// <summary>
		/// End the current player's turn and switch to next player
		/// </summary>
		public void EndTurn () {
			Debug.Log ($"Turn ended for: {currentPlayer}");
			SwitchToNextPlayer ();
		}

		/// <summary>
		/// Switch to the next player based on turn direction
		/// </summary>
		void SwitchToNextPlayer () {
			// In 2-player game, just alternate
			PlayerType nextPlayer = currentPlayer == PlayerType.Human ?
									PlayerType.Computer : PlayerType.Human;

			StartTurn (nextPlayer);
		}

		/// <summary>
		/// Skip the current player's turn (for Stop cards)
		/// </summary>
		public void SkipTurn () {
			Debug.Log ($"Turn skipped for: {currentPlayer}");
			// Switch to next player, then immediately switch again to skip them
			PlayerType skippedPlayer = currentPlayer == PlayerType.Human ?
									   PlayerType.Computer : PlayerType.Human;

			Debug.Log ($"Player {skippedPlayer} turn is skipped");

			// Don't actually start the skipped player's turn, just switch back
			SwitchToNextPlayer ();
		}

		/// <summary>
		/// Force turn to a specific player (for special cards)
		/// </summary>
		/// <param name="player">Player to give turn to</param>
		public void ForceTurnTo (PlayerType player) {
			Debug.Log ($"Turn forced to: {player}");
			StartTurn (player);
		}

		/// <summary>
		/// Pause turns (set to neutral state)
		/// </summary>
		public void PauseTurns () {
			StopTurnTimer ();
			CancelComputerTurn ();

			if (gameState != null) {
				gameState.ChangeTurnState (TurnState.Neutral);
			}

			Debug.Log ("Turns paused");
		}

		/// <summary>
		/// Resume turns after pause
		/// </summary>
		public void ResumeTurns () {
			Debug.Log ("Turns resumed");
			StartTurn (currentPlayer);
		}

		/// <summary>
		/// Handle turn timing and computer turn delays
		/// </summary>
		void HandleTurnTiming () {
			// Handle human player turn timeout
			if (isTurnTimerActive) {
				turnTimer -= Time.deltaTime;
				if (turnTimer <= 0) {
					Debug.Log ("Human player turn timed out");
					OnTurnTimeOut?.Invoke (PlayerType.Human);
					StopTurnTimer ();
					EndTurn (); // Auto-end turn on timeout
				}
			}

			// Handle computer turn delay
			if (isComputerTurnScheduled) {
				if (Time.time >= computerTurnStartTime) {
					isComputerTurnScheduled = false;
					Debug.Log ("Computer turn ready");
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
			Debug.Log ($"Turn timer started: {playerTurnTimeLimit} seconds");
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
			Debug.Log ($"Computer turn scheduled with {computerTurnDelay}s delay");
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
			Debug.Log ($"Turn system initialized. First player: {firstPlayer}");
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

			Debug.Log ("Turn manager reset");
		}

		/// <summary>
		/// Check if turns are currently active (not in neutral state)
		/// </summary>
		/// <returns>True if someone has an active turn</returns>
		public bool AreTurnsActive () {
			return gameState != null && gameState.turnState != TurnState.Neutral;
		}

		// Properties
		public bool IsHumanTurn => currentPlayer == PlayerType.Human;
		public bool IsComputerTurn => currentPlayer == PlayerType.Computer;
		public bool IsTurnTimerActive => isTurnTimerActive;
		public bool IsComputerTurnPending => isComputerTurnScheduled;
		public PlayerType CurrentPlayer => currentPlayer;
		public float TurnTimeRemaining => GetRemainingTurnTime ();
	}
}