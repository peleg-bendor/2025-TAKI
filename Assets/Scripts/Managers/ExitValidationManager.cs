using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// Handles exit confirmation and safe cleanup following Single Responsibility Principle
	/// - Exit confirmation dialog management
	/// - Coordinate with PauseManager for game state preservation
	/// - Safe game state cleanup before exiting
	/// - Integration with MenuNavigation for screen flow
	/// Proper system cleanup to prevent memory leaks
	/// </summary>
	public class ExitValidationManager : MonoBehaviour {

		[Header ("Exit Validation UI References")]
		[Tooltip ("Screen_ExitValidation GameObject")]
		public GameObject exitValidationScreen;

		[Header ("Dependencies")]
		[Tooltip ("Reference to PauseManager for pause/resume coordination")]
		public PauseManager pauseManager;

		[Tooltip ("Reference to MenuNavigation for screen transitions")]
		public MenuNavigation menuNavigation;

		[Tooltip ("Reference to GameStateManager for state checking")]
		public GameStateManager gameState;

		[Tooltip ("Reference to GameManager for system cleanup")]
		public GameManager gameManager;

		// Exit validation state
		private bool isExitValidationActive = false;
		private bool wasGamePausedBeforeExit = false;
		private bool isExitInProgress = false;

		// Events for exit validation system
		public System.Action OnExitValidationShown;
		public System.Action OnExitValidationCancelled;
		public System.Action OnExitConfirmed;

		void Start () {
			// Find dependencies if not assigned
			FindDependenciesIfMissing ();

			// Ensure exit validation screen starts hidden
			if (exitValidationScreen != null) {
				exitValidationScreen.SetActive (false);
			}
		}

		/// <summary>
		/// Show exit confirmation dialog - main entry point from Btn_Exit
		/// </summary>
		public void ShowExitConfirmation () {
			if (isExitValidationActive) {
				TakiLogger.LogWarning ("Exit validation already active", TakiLogger.LogCategory.UI);
				return;
			}

			TakiLogger.LogSystem ("=== SHOWING EXIT CONFIRMATION ===");

			// Check if game was already paused
			wasGamePausedBeforeExit = (gameState != null && gameState.gameStatus == GameStatus.Paused);

			// Pause the game if it wasn't already paused
			if (!wasGamePausedBeforeExit) {
				TakiLogger.LogSystem ("Pausing game for exit validation");
				if (pauseManager != null) {
					pauseManager.PauseForExitValidation ();
				} else {
					TakiLogger.LogError ("Cannot pause for exit: PauseManager is null", TakiLogger.LogCategory.System);
				}
			} else {
				TakiLogger.LogSystem ("Game was already paused - no additional pause needed");
			}

			// Show exit validation screen
			ShowExitValidationScreen ();

			// Mark as active
			isExitValidationActive = true;

			OnExitValidationShown?.Invoke ();
		}

		/// <summary>
		/// Show the exit validation screen UI
		/// </summary>
		void ShowExitValidationScreen () {
			if (exitValidationScreen == null) {
				TakiLogger.LogError ("Cannot show exit validation screen: exitValidationScreen is null", TakiLogger.LogCategory.UI);
				return;
			}

			// Show the screen
			exitValidationScreen.SetActive (true);

			TakiLogger.LogUI ("Exit validation screen displayed");
		}

		/// <summary>
		/// Handle exit confirmation - user clicked Btn_ExitConfirm
		/// Proper system cleanup to prevent memory leaks
		/// </summary>
		public void ConfirmExit () {
			// Prevent recursion!
			if (isExitInProgress) {
				TakiLogger.LogWarning ("Exit already in progress - ignoring duplicate call", TakiLogger.LogCategory.System);
				return;
			}

			isExitInProgress = true;

			TakiLogger.LogSystem ("=== EXIT CONFIRMED - PERFORMING COMPREHENSIVE CLEANUP ===");

			if (!isExitValidationActive) {
				TakiLogger.LogWarning ("Exit confirmed but validation not active", TakiLogger.LogCategory.System);
			}

			// Hide exit validation screen
			HideExitValidationScreen ();

			// CRITICAL: Perform comprehensive system cleanup BEFORE quitting
			PerformComprehensiveSystemCleanup ();

			// Mark as inactive
			isExitValidationActive = false;

			// OnExitConfirmed event
			OnExitConfirmed?.Invoke ();

			// Final quit with proper cleanup
			QuitApplicationSafely ();
		}

		/// <summary> 
		/// Handle exit cancellation - user clicked Btn_ExitCancel
		/// </summary>
		public void CancelExit () {
			TakiLogger.LogSystem ("=== EXIT CANCELLED ===");

			if (!isExitValidationActive) {
				TakiLogger.LogWarning ("Exit cancelled but validation not active", TakiLogger.LogCategory.System);
				return;
			}

			// Hide exit validation screen
			HideExitValidationScreen ();

			// Resume game if we paused it (and it wasn't paused before)
			if (!wasGamePausedBeforeExit) {
				TakiLogger.LogSystem ("Resuming game after exit cancellation");
				if (pauseManager != null) {
					pauseManager.CancelExitValidation (); // This calls ResumeGame()
				} else {
					TakiLogger.LogError ("Cannot resume after exit cancel: PauseManager is null", TakiLogger.LogCategory.System);
				}
			} else {
				TakiLogger.LogSystem ("Game was paused before exit - leaving in paused state");
			}

			// Mark as inactive
			isExitValidationActive = false;

			OnExitValidationCancelled?.Invoke ();
		}

		/// <summary>
		/// FIXED: Comprehensive system cleanup to prevent memory leaks
		/// This method now properly stops all game systems before exit
		/// </summary>
		void PerformComprehensiveSystemCleanup () {
			TakiLogger.LogSystem ("=== PERFORMING COMPREHENSIVE SYSTEM CLEANUP ===");

			try {
				// 1. Stop AI operations (critical for memory cleanup)
				if (gameManager != null && gameManager.computerAI != null) {
					TakiLogger.LogSystem ("Stopping AI operations...");
					gameManager.computerAI.CancelAllAIOperations ();
					gameManager.computerAI.ForceCompleteReset ();
				}

				// 2. Stop turn manager operations
				if (gameManager != null && gameManager.turnManager != null) {
					TakiLogger.LogSystem ("Stopping turn manager operations...");
					gameManager.turnManager.ForceCancelComputerOperations ();
					gameManager.turnManager.ResetTurns ();
				}

				// 3. Cancel all Invoke calls and coroutines on GameManager
				if (gameManager != null) {
					TakiLogger.LogSystem ("Cancelling GameManager operations...");
					gameManager.CancelInvoke ();
					gameManager.StopAllCoroutines ();
				}

				// 4. Cancel all Invoke calls on this component
				TakiLogger.LogSystem ("Cancelling ExitValidationManager operations...");
				CancelInvoke ();
				StopAllCoroutines ();

				// 5. Cancel operations on other managers
				if (pauseManager != null) {
					pauseManager.CancelInvoke ();
					pauseManager.StopAllCoroutines ();
				}

				if (menuNavigation != null) {
					menuNavigation.CancelInvoke ();
					menuNavigation.StopAllCoroutines ();
				}

				// 6. Force garbage collection (helps with cleanup)
				TakiLogger.LogSystem ("Forcing garbage collection...");
				System.GC.Collect ();
				System.GC.WaitForPendingFinalizers ();
				System.GC.Collect ();

				// 7. Small delay to ensure cleanup completion
				TakiLogger.LogSystem ("Cleanup operations completed successfully");

			} catch (System.Exception e) {
				TakiLogger.LogError ($"Error during system cleanup: {e.Message}", TakiLogger.LogCategory.System);
			}

			TakiLogger.LogSystem ("Comprehensive system cleanup complete - ready for safe exit");
		}

		/// <summary>
		/// FIXED: Safe application quit with proper timing
		/// </summary>
		void QuitApplicationSafely () {
			TakiLogger.LogSystem ("=== SAFE APPLICATION QUIT ===");

			// Use Invoke to add a small delay, ensuring cleanup operations complete
			Invoke (nameof (DelayedApplicationQuit), 0.1f);
		}

		/// <summary>
		/// Delayed application quit - called after cleanup operations
		/// </summary>
		void DelayedApplicationQuit () {
			TakiLogger.LogSystem ("Executing delayed application quit...");

#if UNITY_EDITOR
			// Stop play mode in editor
			UnityEditor.EditorApplication.isPlaying = false;
			TakiLogger.LogSystem ("Exiting application (Editor mode)");
#else
            // Close the application in builds
            Application.Quit();
            TakiLogger.LogSystem("Exiting application (Build mode)");
#endif
		}

		/// <summary>
		/// Hide exit validation screen
		/// </summary>
		void HideExitValidationScreen () {
			if (exitValidationScreen != null) {
				exitValidationScreen.SetActive (false);
				TakiLogger.LogUI ("Exit validation screen hidden");
			}
		}

		/// <summary>
		/// Check if exit validation is currently active
		/// </summary>
		public bool IsExitValidationActive => isExitValidationActive;

		/// <summary>
		/// Force hide exit validation (for emergency cleanup)
		/// </summary>
		public void ForceHideExitValidation () {
			TakiLogger.LogSystem ("Force hiding exit validation");

			HideExitValidationScreen ();
			isExitValidationActive = false;
			wasGamePausedBeforeExit = false;
		}

		/// <summary>
		/// Find dependencies if not assigned in inspector
		/// </summary>
		void FindDependenciesIfMissing () {
			if (pauseManager == null) {
				pauseManager = FindObjectOfType<PauseManager> ();
			}

			if (menuNavigation == null) {
				menuNavigation = FindObjectOfType<MenuNavigation> ();
			}

			if (gameState == null) {
				gameState = FindObjectOfType<GameStateManager> ();
			}

			if (gameManager == null) {
				gameManager = FindObjectOfType<GameManager> ();
			}

			// Find UI references if not assigned
			if (exitValidationScreen == null) {
				// Try to find Screen_ExitValidation in scene hierarchy
				GameObject canvas = GameObject.Find ("Canvas");
				if (canvas != null) {
					Transform screenTransform = canvas.transform.Find ("Screen_ExitValidation");
					if (screenTransform != null) {
						exitValidationScreen = screenTransform.gameObject;
						TakiLogger.LogSystem ("Screen_ExitValidation found automatically");
					}
				}
			}

			TakiLogger.LogSystem ("ExitValidationManager dependencies resolved");
		}

		/// <summary>
		/// Debug method to test exit validation
		/// </summary>
		[ContextMenu ("Test Exit Validation")]
		public void TestExitValidation () {
			TakiLogger.LogDiagnostics ("Testing exit validation screen");
			ShowExitConfirmation ();
		}

		/// <summary> 
		/// Debug method to log exit validation state
		/// </summary>
		[ContextMenu ("Log Exit State")]
		public void LogExitState () {
			TakiLogger.LogDiagnostics ("=== EXIT VALIDATION STATE DEBUG ===");
			TakiLogger.LogDiagnostics ($"Exit Validation Active: {isExitValidationActive}");
			TakiLogger.LogDiagnostics ($"Was Game Paused Before: {wasGamePausedBeforeExit}");
			TakiLogger.LogDiagnostics ($"Exit Screen Active: {(exitValidationScreen != null ? exitValidationScreen.activeSelf : false)}");
			TakiLogger.LogDiagnostics ($"PauseManager Available: {pauseManager != null}");
			TakiLogger.LogDiagnostics ($"MenuNavigation Available: {menuNavigation != null}");
			TakiLogger.LogDiagnostics ($"GameManager Available: {gameManager != null}");
		}
	}
}