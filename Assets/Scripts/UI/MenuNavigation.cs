using System.Collections;
using UnityEngine;

namespace TakiGame {

	/// <summary>
	/// Manages navigation between different menu screens using a stack-based approach.
	/// Handles transitions, loading screens, and back navigation.
	/// Integrates with GameManager to start games at proper time
	/// Now calls proper GameManager initialization methods
	/// Separates single player vs multiplayer system initialization
	/// </summary>
	public class MenuNavigation : MonoBehaviour {
		[Header ("Menu Screens")]
		[SerializeField] private GameObject MainMenuScreen;
		[SerializeField] private GameObject StudentInfoScreen;
		[SerializeField] private GameObject SettingsScreen;
		[SerializeField] private GameObject SinglePlayerScreen;
		[SerializeField] private GameObject MultiPlayerScreen;
		[SerializeField] private GameObject ExitValidationScreen;

		[Header ("Transition Screens")]
		[SerializeField] private GameObject LoadingScreen;
		[SerializeField] private GameObject ExitingScreen;

		[Header ("Game Screens")]
		[SerializeField] private GameObject SinglePlayerGameScreen;
		[SerializeField] private GameObject MultiPlayerGameScreen;

		[Header ("Game Integration")]
		[Tooltip ("Reference to GameManager for starting single player games")]
		[SerializeField] private GameManager gameManager;

		[Header ("Pause and Game End Screens")]
		[SerializeField] private GameObject Screen_Paused;
		[SerializeField] private GameObject Screen_GameEnd;

		[Header ("Variables")]
		[SerializeField] private float loadingScreenDuration = 2f;

		// Stack to manage screen navigation history
		private System.Collections.Generic.Stack<GameObject> screenStack = new System.Collections.Generic.Stack<GameObject> ();

		void Awake () {
			// Initialize with main menu as the base screen
			screenStack.Push (MainMenuScreen);
			MainMenuScreen.SetActive (true);

			// Find GameManager if not assigned
			if (gameManager == null) {
				gameManager = FindObjectOfType<GameManager> ();
				if (gameManager == null) {
					Debug.LogWarning ("MenuNavigation: GameManager not found!");
				}
			}

			// Find screen references if not assigned
			FindScreenReferencesIfMissing ();
		}

		/// <summary>
		/// Find screen references if not assigned in inspector
		/// </summary>
		void FindScreenReferencesIfMissing () {
			GameObject canvas = GameObject.Find ("Canvas");
			if (canvas == null) {
				Debug.LogError ("MenuNavigation: Canvas not found!");
				return;
			}

			// Find Screen_Paused if not assigned
			if (Screen_Paused == null) {
				Transform pausedTransform = canvas.transform.Find ("Screen_Paused");
				if (pausedTransform != null) {
					Screen_Paused = pausedTransform.gameObject;
					Debug.Log ("MenuNavigation: Screen_Paused found automatically");
				} else {
					Debug.LogWarning ("MenuNavigation: Screen_Paused not found!");
				}
			}

			// Find Screen_GameEnd if not assigned
			if (Screen_GameEnd == null) {
				Transform gameEndTransform = canvas.transform.Find ("Screen_GameEnd");
				if (gameEndTransform != null) {
					Screen_GameEnd = gameEndTransform.gameObject;
					Debug.Log ("MenuNavigation: Screen_GameEnd found automatically");
				} else {
					Debug.LogWarning ("MenuNavigation: Screen_GameEnd not found!");
				}
			}

			Debug.Log ("MenuNavigation: Screen references resolved");
		}

		/// <summary>
		/// Navigates to a new screen and adds it to the navigation stack
		/// </summary>
		/// <param name="newScreen">The screen to navigate to</param>
		private void SetScreen (GameObject newScreen) {
			screenStack.Peek ().SetActive (false);
			newScreen.SetActive (true);
			screenStack.Push (newScreen);
		}

		/// <summary>
		/// Navigates to a new screen and clears the navigation history (for game screens)
		/// </summary>
		/// <param name="newScreen">The screen to navigate to</param>
		private void SetScreenAndClearStack (GameObject newScreen) {
			// Deactivate all screens in stack
			while (screenStack.Count > 0) {
				screenStack.Pop ().SetActive (false);
			}

			// Set new screen as the only active screen
			newScreen.SetActive (true);
			screenStack.Push (newScreen);
		}

		/// <summary>
		/// Clears the navigation stack
		/// </summary>
		private void ClearStack () {
			while (screenStack.Count > 0) {
				screenStack.Pop ().SetActive (false);
			}
		}

		/// <summary>
		/// Shows a screen temporarily for a specified duration, then transitions to target screen
		/// </summary>
		/// <param name="temporaryScreen">The screen to show temporarily</param>
		/// <param name="targetScreen">The final screen to show after the delay</param>
		/// <param name="clearStack">Whether to clear the navigation stack when reaching target</param>
		private IEnumerator ShowScreenTemporarily (GameObject temporaryScreen, GameObject targetScreen, bool clearStack = true) {
			SetScreen (temporaryScreen);
			yield return new WaitForSeconds (loadingScreenDuration);

			if (clearStack) {
				SetScreenAndClearStack (targetScreen);
			} else {
				SetScreen (targetScreen);
			}

			// Start game AFTER screen is active
			StartGameIfNeeded (targetScreen);
		}

		/// <summary>
		/// Start appropriate game when game screen becomes active
		/// Now uses proper separation between single player and multiplayer
		/// </summary>
		/// <param name="gameScreen">The game screen that just became active</param>
		private void StartGameIfNeeded (GameObject gameScreen) {
			if (gameScreen == SinglePlayerGameScreen) {
				StartSinglePlayerGame ();
			} else if (gameScreen == MultiPlayerGameScreen) {
				StartMultiPlayerGame ();
			}
		}

		/// <summary>
		/// Start single player game through GameManager
		/// Now initializes systems only when needed
		/// </summary>
		private void StartSinglePlayerGame () {
			if (gameManager != null) {
				Debug.Log ("MenuNavigation: Starting single player game...");
				// CHANGED: Call the new single player specific method
				gameManager.StartNewSinglePlayerGame ();
			} else {
				Debug.LogError ("MenuNavigation: Cannot start game - GameManager not assigned!");
			}
		}

		/// <summary>
		/// Start multiplayer game (placeholder for future implementation)
		/// </summary>
		private void StartMultiPlayerGame () {
			if (gameManager != null) {
				Debug.Log ("MenuNavigation: Initializing multiplayer systems...");
				// FUTURE: This will call gameManager.InitializeMultiPlayerSystems()
				gameManager.InitializeMultiPlayerSystems ();
			} else {
				Debug.LogError ("MenuNavigation: Cannot start multiplayer - GameManager not assigned!");
			}
		}

		/// <summary>
		/// Goes back to the previous screen in the navigation stack
		/// </summary>
		private void GoBack () {
			if (screenStack.Count <= 1) {
				Debug.LogWarning ("MenuNavigation: Cannot go back - already at base screen!");
				return;
			}

			screenStack.Pop ().SetActive (false);

			if (screenStack.Count > 0) {
				GameObject prevScreen = screenStack.Peek ();
				prevScreen.SetActive (true);

				// If previous screen is main menu, clear the stack
				if (prevScreen == MainMenuScreen) {
					ClearStack ();
					MainMenuScreen.SetActive (true);
					screenStack.Push (MainMenuScreen);
				}
			}
		}

		#region Main Menu Button Logic

		public void Btn_StudentInfoLogic () {
			SetScreen (StudentInfoScreen);
		}

		public void Btn_SettingsLogic () {
			SetScreen (SettingsScreen);
		}

		public void Btn_SinglePlayerLogic () {
			SetScreen (SinglePlayerScreen);
		}

		public void Btn_MultiPlayerLogic () {
			SetScreen (MultiPlayerScreen);
		}

		#endregion

		#region Game Start Logic

		/// <summary>
		/// Single player game start - only initializes single player systems
		/// </summary>
		public void Btn_PlaySinglePlayerLogic () {
			StartCoroutine (ShowScreenTemporarily (LoadingScreen, SinglePlayerGameScreen, clearStack: true));
		}

		/// <summary>
		/// Multiplayer game start - will initialize multiplayer systems (future)
		/// </summary>
		public void Btn_PlayMultiPlayerLogic () {
			StartCoroutine (ShowScreenTemporarily (LoadingScreen, MultiPlayerGameScreen, clearStack: true));
		}

		#endregion

		#region Pause and Game End Button Logic

		/// <summary>
		/// Handle Btn_Pause click - pause current game
		/// </summary>
		public void Btn_PauseLogic () {
			Debug.Log ("MenuNavigation: Pause button clicked");

			if (gameManager != null) {
				gameManager.RequestPauseGame ();
			} else {
				Debug.LogError ("MenuNavigation: Cannot pause - GameManager not assigned!");
			}

			// Show pause screen as OVERLAY (don't hide the game screen)
			ShowPauseScreenOverlay ();
		}

		/// <summary>
		/// Show pause screen as overlay without hiding game screen
		/// </summary>
		private void ShowPauseScreenOverlay () {
			if (Screen_Paused == null) {
				Debug.LogError ("MenuNavigation: Screen_Paused not assigned!");
				return;
			}

			// Simply activate the pause screen - don't touch the navigation stack
			// This keeps Screen_SinglePlayerGame visible underneath
			Screen_Paused.SetActive (true);

			Debug.Log ("MenuNavigation: Pause screen shown as overlay");
		}

		/// <summary>
		/// Handle Btn_Continue click - resume paused game and hide overlay
		/// </summary>
		public void Btn_ContinueLogic () {
			Debug.Log ("MenuNavigation: Continue button clicked");

			if (gameManager != null) {
				gameManager.RequestResumeGame ();
			} else {
				Debug.LogError ("MenuNavigation: Cannot resume - GameManager not assigned!");
			}

			// Hide pause screen overlay
			HidePauseScreenOverlay ();
		}

		/// <summary>
		/// Hide pause screen overlay
		/// </summary>
		private void HidePauseScreenOverlay () {
			if (Screen_Paused != null) {
				Screen_Paused.SetActive (false);
				Debug.Log ("MenuNavigation: Pause screen overlay hidden");
			}
		}

		/// <summary>
		/// Handle Btn_Restart click - restart current game - ENHANCED with AI state verification
		/// </summary>
		public void Btn_RestartLogic () {
			Debug.Log ("MenuNavigation: Restart button clicked FROM PAUSE");

			// Hide pause screen first
			HidePauseScreenOverlay ();

			if (gameManager != null) {
				// ENHANCED: Pre-emptively fix AI state before restart
				Debug.Log ("MenuNavigation: Pre-emptive AI state fix before restart...");

				if (gameManager.computerAI != null && gameManager.computerAI.IsAIPaused) {
					Debug.Log ("MenuNavigation: AI is paused - applying pre-emptive fix");
					gameManager.computerAI.ForceCompleteReset ();
				}

				// Now restart 
				RestartGameFromPause ();

				// Verify after restart
				StartCoroutine (VerifyAIStateAfterRestart ());
			} else {
				Debug.LogError ("MenuNavigation: Cannot restart - GameManager not assigned!");
			}
		}

		/// <summary>
		/// Restart game specifically from pause state (not from game end)
		/// </summary> 
		private void RestartGameFromPause () {
			Debug.Log ("MenuNavigation: Restarting game from pause state");

			// Directly start a new single player game
			// This bypasses the GameEndManager which expects a game-end state
			if (gameManager != null) {
				gameManager.StartNewSinglePlayerGame ();
				Debug.Log ("MenuNavigation: New game started from pause restart");
			}
		}

		/// <summary>
		/// NEW: Verify AI state after restart to catch any issues
		/// </summary>
		private System.Collections.IEnumerator VerifyAIStateAfterRestart () {
			// Wait a moment for restart to complete
			yield return new WaitForSeconds (0.5f);

			Debug.Log ("MenuNavigation: Verifying AI state after restart...");

			if (gameManager != null && gameManager.computerAI != null) {
				bool aiIsPaused = gameManager.computerAI.IsAIPaused;
				bool gameIsActive = gameManager.IsGameActive;

				Debug.Log ($"MenuNavigation: Post-restart verification - AI Paused: {aiIsPaused}, Game Active: {gameIsActive}");

				if (aiIsPaused && gameIsActive) {
					Debug.LogError ("MenuNavigation: PROBLEM DETECTED - AI still paused after restart!");

					// IMPROVED Emergency fix - try multiple approaches
					Debug.Log ("MenuNavigation: Applying comprehensive emergency AI fix...");

					// Approach 1: Force complete reset
					gameManager.computerAI.ForceCompleteReset ();

					// Wait a moment
					yield return new WaitForSeconds (0.1f);

					// Check if that worked
					if (gameManager.computerAI.IsAIPaused) {
						Debug.LogError ("MenuNavigation: Force reset failed - trying additional fixes...");

						// Approach 2: Try resume then clear
						gameManager.computerAI.ResumeAI ();
						yield return new WaitForSeconds (0.1f);
						gameManager.computerAI.ClearHand ();

						// Approach 3: Final check and manual field reset if needed
						if (gameManager.computerAI.IsAIPaused) {
							Debug.LogError ("MenuNavigation: All fixes failed - AI may be permanently stuck");
							Debug.LogError ("MenuNavigation: Please restart the game completely");
						} else {
							Debug.Log ("MenuNavigation: AI unstuck with approach 2");
						}
					} else {
						Debug.Log ("MenuNavigation: AI unstuck with force reset");
					}

				} else {
					Debug.Log ("MenuNavigation: AI state verification passed - restart successful");
				}
			}
		}

		/// <summary>
		/// Handle Btn_GoHome click from pause screen - return to main menu
		/// </summary>
		public void Btn_GoHomeFromPauseLogic () {
			Debug.Log ("MenuNavigation: Go Home button clicked FROM PAUSE");

			// Hide pause screen first
			HidePauseScreenOverlay ();

			// Use loading screen transition to main menu
			StartCoroutine (GoToMainMenuWithLoadingCoroutine ());
		}

		/// <summary>
		/// Enhanced Btn_GoHome logic that works from both pause and game end
		/// </summary>
		public void Btn_GoHomeLogic () {
			Debug.Log ("MenuNavigation: Go Home button clicked");

			// Check if we're coming from pause screen
			if (Screen_Paused != null && Screen_Paused.activeSelf) {
				Btn_GoHomeFromPauseLogic ();
			} else {
				// This is from game end screen - use existing logic
				StartCoroutine (GoToMainMenuWithLoadingCoroutine ());
			}
		}

		/// <summary>
		/// Go to main menu with loading screen transition - COROUTINE VERSION
		/// </summary>
		private IEnumerator GoToMainMenuWithLoadingCoroutine () {
			Debug.Log ("MenuNavigation: Transitioning to main menu with loading screen");
			yield return StartCoroutine (ShowScreenTemporarily (LoadingScreen, MainMenuScreen, clearStack: true));
		}

		/// <summary>
		/// Go to main menu with loading screen transition - PUBLIC METHOD for external use
		/// </summary>
		public void GoToMainMenuWithLoading () {
			Debug.Log ("MenuNavigation: Go to main menu with loading screen requested");
			StartCoroutine (GoToMainMenuWithLoadingCoroutine ());
		}

		#endregion

		#region Enhanced Exit Logic

		/// <summary>
		/// Handle Btn_Exit click - show exit validation instead of direct exit
		/// </summary>
		public void Btn_ExitLogic () {
			Debug.Log ("MenuNavigation: Exit button clicked - showing validation");

			// Request exit confirmation through GameManager
			if (gameManager != null) {
				gameManager.RequestExitConfirmation ();
			} else {
				Debug.LogError ("MenuNavigation: Cannot show exit confirmation - GameManager not assigned!");
				// Fallback to direct validation screen
				SetScreen (ExitValidationScreen);
			}
		}

		/// <summary>
		/// Show exit validation screen as overlay (similar to pause)
		/// </summary>
		private void ShowExitValidationOverlay () {
			if (ExitValidationScreen == null) {
				Debug.LogError ("MenuNavigation: ExitValidationScreen not assigned!");
				return;
			}

			// Show as overlay, keeping the game screen visible underneath
			ExitValidationScreen.SetActive (true);
			Debug.Log ("MenuNavigation: Exit validation screen shown as overlay");
		}

		/// <summary>
		/// Handle Btn_ExitCancel click - treat as continue/resume
		/// </summary>
		public void Btn_ExitCancelLogic () {
			Debug.Log ("MenuNavigation: Exit cancelled - treating as continue");

			// Let ExitValidationManager handle the cancellation logic
			if (gameManager != null && gameManager.exitValidationManager != null) {
				gameManager.exitValidationManager.CancelExit ();
			} else {
				Debug.LogWarning ("MenuNavigation: ExitValidationManager not available - using fallback");

				// Fallback: hide exit validation screen
				if (ExitValidationScreen != null) {
					ExitValidationScreen.SetActive (false);
				}

				// Try to resume game 
				if (gameManager != null) {
					gameManager.RequestResumeGame ();
				}
			}
		}

		/// <summary>
		/// Handle Btn_ExitConfirm click - confirm application exit
		/// </summary>
		public void Btn_ExitConfirmLogic () {
			Debug.Log ("MenuNavigation: Exit confirmed - application will close");

			// Let ExitValidationManager handle the confirmation
			if (gameManager != null && gameManager.exitValidationManager != null) {
				gameManager.exitValidationManager.ConfirmExit ();
			} else {
				Debug.LogWarning ("MenuNavigation: ExitValidationManager not available - using direct exit");
				// Existing exit logic will run
				StartCoroutine (ShowExitingScreenAndQuit ());
			}
		}

		public void StartExitSequence () {
			Debug.Log ("MenuNavigation: Starting exit sequence");
			StartCoroutine (ShowExitingScreenAndQuit ());
		} 

		#endregion

		#region Exit Application Logic

		/// <summary>
		/// Shows the exiting screen temporarily, then quits the application
		/// </summary>
		private IEnumerator ShowExitingScreenAndQuit () {
			SetScreen (ExitingScreen);
			yield return new WaitForSeconds (loadingScreenDuration); // Brief pause for user feedback

			// Close the application
			QuitApplication ();
		}

		/// <summary>
		/// Closes the application, handling both editor and build environments
		/// </summary>
		private void QuitApplication () {
#if UNITY_EDITOR
			// Stop play mode in editor since we can't actually quit Unity
			UnityEditor.EditorApplication.isPlaying = false;
			Debug.Log ("MenuNavigation: Exiting application (Editor mode)");
#else
            // Close the application in builds
            Application.Quit();
            Debug.Log("MenuNavigation: Exiting application");
#endif
		}

		#endregion

		#region Universal Navigation

		public void Btn_BackLogic () {
			GoBack ();
		}

		#endregion

		#region Debug/Utility Methods

		/// <summary>
		/// Debug method to log current stack state
		/// </summary>
		[System.Diagnostics.Conditional ("UNITY_EDITOR")]
		public void LogStackState () {
			Debug.Log ($"MenuNavigation Stack Count: {screenStack.Count}");
			if (screenStack.Count > 0) {
				Debug.Log ($"Current Screen: {screenStack.Peek ().name}");
			}
		}

		#endregion
	}
}