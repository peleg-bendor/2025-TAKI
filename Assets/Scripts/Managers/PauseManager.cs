using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// Handles ALL pause-related functionality following Single Responsibility Principle
	/// - Pause/resume game state coordination
	/// - System state preservation and restoration
	/// - Integration with all game systems for safe pause/resume
	/// - Exit validation pause handling
	/// </summary>
	public class PauseManager : MonoBehaviour {

		[Header ("Dependencies")]
		[Tooltip ("Reference to GameManager for coordination")]
		public GameManager gameManager;

		[Tooltip ("Reference to GameStateManager for state coordination")]
		public GameStateManager gameState;

		[Tooltip ("Reference to TurnManager for turn pause/resume")]
		public TurnManager turnManager;

		[Tooltip ("Reference to BasicComputerAI for AI pause/resume")]
		public BasicComputerAI computerAI;

		[Tooltip ("Reference to MenuNavigation for screen transitions")]
		public MenuNavigation menuNavigation;

		// State preservation data
		private GameStateSnapshot pausedState;
		private GameManagerTurnFlowSnapshot pausedTurnFlowState;
		private bool isPaused = false;
		private bool wasComputerTurnActive = false;
		private float pauseStartTime = 0f;

		// Events for pause system
		public System.Action OnGamePaused;
		public System.Action OnGameResumed;
		public System.Action OnPauseStateChanged;

		void Start () {
			// Find dependencies if not assigned
			FindDependenciesIfMissing ();
		}

		/// <summary>
		/// Main pause game method - preserves all system states
		/// </summary>
		public void PauseGame () {
			if (isPaused) {
				TakiLogger.LogSystem ("Game is already paused");
				return;
			}

			TakiLogger.LogSystem ("=== PAUSING GAME ===");

			// Capture current game state
			CaptureGameState ();

			// Pause all game systems
			PauseAllSystems ();

			// Update pause status
			isPaused = true;
			pauseStartTime = Time.time;

			// Update UI state
			// Update UI state using active UI manager
			if (gameManager.GetActiveUI() != null) {
				gameManager.GetActiveUI().UpdateStrictButtonStates (false, false, false); // Disable all game buttons
			}

			TakiLogger.LogSystem ("Game successfully paused");
			OnGamePaused?.Invoke ();
			OnPauseStateChanged?.Invoke ();
		}

		/// <summary>
		/// Main resume game method - restores all system states
		/// </summary>
		public void ResumeGame () {
			if (!isPaused) {
				TakiLogger.LogSystem ("Game is not currently paused");
				return;
			}

			TakiLogger.LogSystem ("=== RESUMING GAME ===");

			// Restore all game systems
			RestoreAllSystems ();

			// Restore game state
			RestoreGameState ();

			// Update pause status
			isPaused = false;
			float pauseDuration = Time.time - pauseStartTime;

			TakiLogger.LogSystem ($"Game successfully resumed after {pauseDuration:F1} seconds");
			OnGameResumed?.Invoke ();
			OnPauseStateChanged?.Invoke ();
		}

		/// <summary>
		/// Pause triggered by exit validation - same as regular pause
		/// </summary>
		public void PauseForExitValidation () {
			TakiLogger.LogSystem ("Pausing game for exit validation");
			PauseGame (); // Use same pause logic
		}

		/// <summary>
		/// Cancel exit validation - same as resume
		/// </summary>
		public void CancelExitValidation () {
			TakiLogger.LogSystem ("Exit validation cancelled - resuming game");
			ResumeGame (); // Use same resume logic
		}

		/// <summary>
		/// Capture current game state for restoration
		/// </summary>
		void CaptureGameState () {
			if (gameState == null) {
				TakiLogger.LogError ("Cannot capture game state: GameStateManager is null", TakiLogger.LogCategory.System);
				return;
			}

			// Capture GameStateManager state
			pausedState = new GameStateSnapshot {
				turnState = gameState.turnState,
				interactionState = gameState.interactionState,
				gameStatus = gameState.gameStatus,
				activeColor = gameState.activeColor,
				turnDirection = gameState.turnDirection,
				isComputerTurnActive = (turnManager != null && turnManager.IsComputerTurn),
				isComputerTurnPending = (turnManager != null && turnManager.IsComputerTurnPending),
				currentPlayer = (turnManager != null ? turnManager.CurrentPlayer : PlayerType.Human),

				// PHASE 8B: Capture TAKI sequence state
				isInTakiSequence = gameState.IsInTakiSequence,
				takiSequenceColor = gameState.TakiSequenceColor,
				numberOfSequenceCards = gameState.NumberOfSequenceCards,
				takiSequenceInitiator = gameState.TakiSequenceInitiator
			};

			// Capture GameManager turn flow state
			if (gameManager != null) {
				pausedTurnFlowState = gameManager.CaptureTurnFlowState ();
				TakiLogger.LogSystem ("GameManager turn flow state captured");
			}

			TakiLogger.LogSystem ($"Game state captured: Turn={pausedState.turnState}, Player={pausedState.currentPlayer}");
		}

		/// <summary>
		/// Pause all game systems safely
		/// </summary>
		void PauseAllSystems () {
			// Pause game state
			if (gameState != null) {
				gameState.ChangeGameStatus (GameStatus.Paused);
				TakiLogger.LogSystem ("GameStateManager paused");
			}

			// Pause turn system
			if (turnManager != null) {
				wasComputerTurnActive = turnManager.IsComputerTurn;
				turnManager.PauseTurns ();
				TakiLogger.LogSystem ("TurnManager paused");
			}

			// Pause AI operations
			if (computerAI != null) {
				computerAI.PauseAI ();
				TakiLogger.LogSystem ("Computer AI paused");
			}

			TakiLogger.LogSystem ("All systems paused successfully");
		}

		/// <summary>
		/// Restore all game systems to their pre-pause state
		/// </summary>
		void RestoreAllSystems () {
			// Restore game state
			if (gameState != null) {
				gameState.ChangeGameStatus (GameStatus.Active);
				TakiLogger.LogSystem ("GameStateManager restored to active");
			}

			// Restore turn system
			if (turnManager != null) {
				turnManager.ResumeTurns ();
				TakiLogger.LogSystem ("TurnManager resumed");
			}

			// Resume AI operations
			if (computerAI != null) {
				computerAI.ResumeAI ();
				TakiLogger.LogSystem ("Computer AI resumed");
			}

			TakiLogger.LogSystem ("All systems restored successfully");
		}

		/// <summary>
		/// Restore specific game state details
		/// </summary>
		void RestoreGameState () {
			if (pausedState == null) {
				TakiLogger.LogError ("Cannot restore game state: No paused state captured", TakiLogger.LogCategory.System);
				return;
			}

			// Restore state manager properties
			if (gameState != null) {
				gameState.ChangeTurnState (pausedState.turnState);
				gameState.ChangeInteractionState (pausedState.interactionState);
				gameState.ChangeActiveColor (pausedState.activeColor);

				// PHASE 8B: Restore TAKI sequence state
				if (pausedState.isInTakiSequence) {
					// Manually restore sequence state (bypass events during restoration)
					gameState.StartTakiSequence (pausedState.takiSequenceColor, pausedState.takiSequenceInitiator);
					// Set the card count directly
					for (int i = 1; i < pausedState.numberOfSequenceCards; i++) {
						gameState.AddCardToSequence (null); // Add null cards to maintain count
					}
					TakiLogger.LogSystem ($"TAKI sequence state restored: {pausedState.numberOfSequenceCards} cards of {pausedState.takiSequenceColor}");
				}

				// Note: gameStatus already set to Active in RestoreAllSystems()
			}

			// Restore GameManager turn flow state
			if (gameManager != null && pausedTurnFlowState != null) {
				gameManager.RestoreTurnFlowState (pausedTurnFlowState);
				TakiLogger.LogSystem ("GameManager turn flow state restored");
			}

			// Update UI to reflect restored state 
			if (gameManager.GetActiveUI() != null) {
				gameManager.GetActiveUI().UpdateAllDisplays (
					gameState.turnState,
					gameState.gameStatus,
					gameState.interactionState,
					gameState.activeColor
				);

				// Restore appropriate button states based on turn
				if (gameState.IsPlayerTurn) {
					// Let GameManager handle proper button states through its turn flow system
					TakiLogger.LogSystem ("Player turn resumed - GameManager will restore button states");
				}
			}

			TakiLogger.LogSystem ("Game state fully restored");
		}

		/// <summary>
		/// Find dependencies if not assigned in inspector
		/// </summary>
		void FindDependenciesIfMissing () {
			if (gameManager == null) {
				gameManager = FindObjectOfType<GameManager> ();
			}

			if (gameState == null) {
				gameState = FindObjectOfType<GameStateManager> ();
			}

			if (turnManager == null) {
				turnManager = FindObjectOfType<TurnManager> ();
			}

			if (computerAI == null) {
				computerAI = FindObjectOfType<BasicComputerAI> ();
			}

			if (menuNavigation == null) {
				menuNavigation = FindObjectOfType<MenuNavigation> ();
			}

			TakiLogger.LogSystem ("PauseManager dependencies resolved");
		}

		/// <summary>
		/// Check if game is currently paused
		/// </summary>
		public bool IsGamePaused => isPaused;

		/// <summary>
		/// Get pause duration if paused
		/// </summary>
		public float GetPauseDuration () {
			return isPaused ? (Time.time - pauseStartTime) : 0f;
		}

		/// <summary>
		/// Debug method to log current pause state
		/// </summary> 
		[ContextMenu ("Log Pause State")]
		public void LogPauseState () {
			TakiLogger.LogDiagnostics ("=== PAUSE STATE DEBUG ===");
			TakiLogger.LogDiagnostics ($"Is Paused: {isPaused}");
			TakiLogger.LogDiagnostics ($"Pause Duration: {GetPauseDuration ():F1}s");

			if (pausedState != null) {
				TakiLogger.LogDiagnostics ($"Captured State - Turn: {pausedState.turnState}, Player: {pausedState.currentPlayer}");
				TakiLogger.LogDiagnostics ($"Computer Turn Active: {pausedState.isComputerTurnActive}");
				TakiLogger.LogDiagnostics ($"Active Color: {pausedState.activeColor}");
			} else {
				TakiLogger.LogDiagnostics ("No captured state available");
			}
		}

		/// <summary>
		/// Data structure for capturing game state
		/// </summary>
		[System.Serializable]
		private class GameStateSnapshot {
			public TurnState turnState;
			public InteractionState interactionState;
			public GameStatus gameStatus;
			public CardColor activeColor;
			public TurnDirection turnDirection;
			public bool isComputerTurnActive;
			public bool isComputerTurnPending;
			public PlayerType currentPlayer;

			// PHASE 8B: TAKI sequence state preservation
			public bool isInTakiSequence;
			public CardColor takiSequenceColor;
			public int numberOfSequenceCards;
			public PlayerType takiSequenceInitiator;
		}

		/// <summary>
		/// Data structure for GameManager turn flow state - FIXED
		/// </summary>
		[System.Serializable]
		public class GameManagerTurnFlowSnapshot {
			public bool hasPlayerTakenAction;
			public bool canPlayerDraw;
			public bool canPlayerPlay;
			public bool canPlayerEndTurn;
		}
	}
}