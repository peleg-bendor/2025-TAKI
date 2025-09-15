using System.Collections;
using UnityEngine;

namespace TakiGame {

	/// <summary>
	/// Manages navigation between different menu screens using a stack-based approach.
	/// Handles transitions, loading screens, and back navigation.
	/// Integrates with GameManager to start games at proper time
	/// Now calls proper GameManager initialization methods
	/// Separates single player vs multiplayer system initialization
	/// ENHANCED: Multiplayer integration with Photon PUN2 connection handling
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

		[Header ("Multiplayer Integration")]
		[Tooltip ("Reference to MultiplayerMenuLogic for Photon connection and matchmaking")]
		[SerializeField] private MultiplayerMenuLogic multiplayerMenuLogic;

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
					TakiLogger.LogWarning ("GameManager not found!", TakiLogger.LogCategory.System);
				}
			}

			// Find MultiplayerMenuLogic if not assigned
			if (multiplayerMenuLogic == null) {
				multiplayerMenuLogic = FindObjectOfType<MultiplayerMenuLogic> ();
				if (multiplayerMenuLogic == null) {
					TakiLogger.LogWarning ("MultiplayerMenuLogic not found! Creating one...", TakiLogger.LogCategory.Multiplayer);
					// Create MultiplayerMenuLogic GameObject if not found
					GameObject multiplayerLogicGO = new GameObject ("MultiplayerMenuLogic");
					multiplayerMenuLogic = multiplayerLogicGO.AddComponent<MultiplayerMenuLogic> ();
				}
			}

			// Find screen references if not assigned
			FindScreenReferencesIfMissing ();

			// Subscribe to multiplayer events
			SubscribeToMultiplayerEvents ();
		}

		void OnDestroy () {
			// Unsubscribe from multiplayer events
			UnsubscribeFromMultiplayerEvents ();
		}

		/// <summary>
		/// Subscribe to multiplayer events
		/// </summary>
		void SubscribeToMultiplayerEvents () {
			MultiplayerMenuLogic.OnMultiplayerGameReady += OnMultiplayerGameReady;
		}

		/// <summary>
		/// Unsubscribe from multiplayer events
		/// </summary>
		void UnsubscribeFromMultiplayerEvents () {
			MultiplayerMenuLogic.OnMultiplayerGameReady -= OnMultiplayerGameReady;
		}

		/// <summary>
		/// Handle multiplayer game ready event (following instructor's pattern)
		/// </summary>
		void OnMultiplayerGameReady () {
			TakiLogger.LogInfo ("Multiplayer game ready - transitioning to game screen", TakiLogger.LogCategory.Multiplayer);

			// Go directly to multiplayer game screen
			SetScreenAndClearStack (MultiPlayerGameScreen);

			// Start multiplayer game
			StartMultiPlayerGame ();
		}

		/// <summary>
		/// Find screen references if not assigned in inspector
		/// </summary>
		void FindScreenReferencesIfMissing () {
			GameObject canvas = GameObject.Find ("Canvas");
			if (canvas == null) {
				TakiLogger.LogError ("Canvas not found!", TakiLogger.LogCategory.UI);
				return;
			}

			// Find Screen_Paused if not assigned
			if (Screen_Paused == null) {
				Transform pausedTransform = canvas.transform.Find ("Screen_Paused");
				if (pausedTransform != null) {
					Screen_Paused = pausedTransform.gameObject;
					TakiLogger.LogInfo ("Screen_Paused found automatically", TakiLogger.LogCategory.UI);
				} else {
					TakiLogger.LogWarning ("Screen_Paused not found!", TakiLogger.LogCategory.UI);
				}
			}

			// Find Screen_GameEnd if not assigned
			if (Screen_GameEnd == null) {
				Transform gameEndTransform = canvas.transform.Find ("Screen_GameEnd");
				if (gameEndTransform != null) {
					Screen_GameEnd = gameEndTransform.gameObject;
					TakiLogger.LogInfo ("Screen_GameEnd found automatically", TakiLogger.LogCategory.UI);
				} else {
					TakiLogger.LogWarning ("Screen_GameEnd not found!", TakiLogger.LogCategory.UI);
				}
			}

			TakiLogger.LogInfo ("Screen references resolved", TakiLogger.LogCategory.UI);
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
				TakiLogger.LogInfo ("Starting single player game...", TakiLogger.LogCategory.GameState);
				// CHANGED: Call the new single player specific method
				gameManager.StartNewSinglePlayerGame ();
			} else {
				TakiLogger.LogError ("Cannot start game - GameManager not assigned!", TakiLogger.LogCategory.System);
			}
		}

		/// <summary>
		/// Start multiplayer game (placeholder for future implementation)
		/// ENHANCED: Will integrate with MultiplayerGameManager when created
		/// </summary>
		private void StartMultiPlayerGame () {
			if (gameManager != null) {
				TakiLogger.LogInfo ("Starting multiplayer game...", TakiLogger.LogCategory.Multiplayer);
				// ENHANCED: Now uses unified game start structure
				gameManager.StartNewMultiPlayerGame ();
			} else {
				TakiLogger.LogError ("Cannot start multiplayer - GameManager not assigned!", TakiLogger.LogCategory.System);
			}
		}

		/// <summary>
		/// Goes back to the previous screen in the navigation stack
		/// </summary>
		private void GoBack () {
			if (screenStack.Count <= 1) {
				TakiLogger.LogWarning ("Cannot go back - already at base screen!", TakiLogger.LogCategory.UI);
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

		/// <summary>
		/// ENHANCED: Multiplayer button now starts Photon connection process
		/// Following instructor's pattern: immediate connection attempt on menu selection
		/// DEBUGGING: Added detailed logging to track button click flow
		/// </summary>
		public void Btn_MultiPlayerLogic () {
			if (MultiPlayerScreen == null) {
				TakiLogger.LogError ("MultiPlayerScreen is NULL! Cannot navigate.", TakiLogger.LogCategory.UI);
				return;
			}

			// ALWAYS navigate to the multiplayer screen first
			SetScreen (MultiPlayerScreen);

			// Connection will start automatically via MultiplayerMenuLogic
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
		/// ENHANCED: Multiplayer game start - now handles Photon matchmaking
		/// Following instructor's pattern: button click starts matchmaking process
		/// </summary>
		public void Btn_PlayMultiPlayerLogic () {
			if (multiplayerMenuLogic != null) {
				if (multiplayerMenuLogic.IsConnectedToPhoton) {
					TakiLogger.LogInfo ("Starting matchmaking...", TakiLogger.LogCategory.Multiplayer);
					// Start matchmaking process
					multiplayerMenuLogic.StartMatchmaking ();

					// Show loading screen while matchmaking happens
					// Game will start automatically when room is ready via OnMultiplayerGameReady event
					StartCoroutine (ShowLoadingScreenForMatchmaking ());
				} else {
					TakiLogger.LogWarning ("Not connected to Photon - cannot start matchmaking", TakiLogger.LogCategory.Multiplayer);
				}
			} else {
				TakiLogger.LogError ("MultiplayerMenuLogic not found - cannot start multiplayer matchmaking", TakiLogger.LogCategory.Multiplayer);

				// Fallback to old behavior for now
				StartCoroutine (ShowScreenTemporarily (LoadingScreen, MultiPlayerGameScreen, clearStack: true));
			}
		}

		/// <summary>
		/// Show loading screen during matchmaking process
		/// Will be interrupted by OnMultiplayerGameReady event when room is found
		/// </summary>
		private IEnumerator ShowLoadingScreenForMatchmaking () {
			TakiLogger.LogInfo ("Showing loading screen for matchmaking...", TakiLogger.LogCategory.Multiplayer);

			SetScreen (LoadingScreen);

			// Wait for either matchmaking success or timeout
			float timeout = 30f; // 30 second timeout
			float elapsed = 0f;

			while (elapsed < timeout) {
				// Check if we've successfully entered a game
				if (MultiPlayerGameScreen.activeSelf) {
					TakiLogger.LogInfo ("Matchmaking successful - game screen active", TakiLogger.LogCategory.Multiplayer);
					yield break;
				}

				elapsed += Time.deltaTime;
				yield return null;
			}

			// Timeout - go back to multiplayer menu
			TakiLogger.LogWarning ("Matchmaking timeout - returning to multiplayer menu", TakiLogger.LogCategory.Multiplayer);
			SetScreen (MultiPlayerScreen);
		}

		#endregion

		#region Pause and Game End Button Logic

		/// <summary>
		/// Handle Btn_Pause click - pause current game
		/// </summary>
		public void Btn_PauseLogic () {
			TakiLogger.LogInfo ("Pause button clicked", TakiLogger.LogCategory.UI);

			if (gameManager != null) {
				gameManager.RequestPauseGame ();
			} else {
				TakiLogger.LogError ("Cannot pause - GameManager not assigned!", TakiLogger.LogCategory.System);
			}

			// Show pause screen as OVERLAY (don't hide the game screen)
			ShowPauseScreenOverlay ();
		}

		/// <summary>
		/// Show pause screen as overlay without hiding game screen
		/// </summary>
		private void ShowPauseScreenOverlay () {
			if (Screen_Paused == null) {
				TakiLogger.LogError ("Screen_Paused not assigned!", TakiLogger.LogCategory.UI);
				return;
			}

			// Simply activate the pause screen - don't touch the navigation stack
			// This keeps Screen_SinglePlayerGame visible underneath
			Screen_Paused.SetActive (true);

			TakiLogger.LogInfo ("Pause screen shown as overlay", TakiLogger.LogCategory.UI);
		}

		/// <summary>
		/// Handle Btn_Continue click - resume paused game and hide overlay
		/// </summary>
		public void Btn_ContinueLogic () {
			TakiLogger.LogInfo ("Continue button clicked", TakiLogger.LogCategory.UI);

			if (gameManager != null) {
				gameManager.RequestResumeGame ();
			} else {
				TakiLogger.LogError ("Cannot resume - GameManager not assigned!", TakiLogger.LogCategory.System);
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
				TakiLogger.LogInfo ("Pause screen overlay hidden", TakiLogger.LogCategory.UI);
			}
		}

		/// <summary>
		/// Handle Btn_Restart click - restart current game - ENHANCED with AI state verification
		/// </summary>
		public void Btn_RestartLogic () {
			TakiLogger.LogInfo ("Restart button clicked FROM PAUSE", TakiLogger.LogCategory.UI);

			// Hide pause screen first
			HidePauseScreenOverlay ();

			if (gameManager != null) {
				// ENHANCED: Pre-emptively fix AI state before restart
				TakiLogger.LogInfo ("Pre-emptive AI state fix before restart...", TakiLogger.LogCategory.GameState);

				if (gameManager.computerAI != null && gameManager.computerAI.IsAIPaused) {
					TakiLogger.LogInfo ("AI is paused - applying pre-emptive fix", TakiLogger.LogCategory.GameState);
					gameManager.computerAI.ForceCompleteReset ();
				}

				// Now restart 
				RestartGameFromPause ();

				// Verify after restart
				StartCoroutine (VerifyAIStateAfterRestart ());
			} else {
				TakiLogger.LogError ("Cannot restart - GameManager not assigned!", TakiLogger.LogCategory.System);
			}
		}

		/// <summary>
		/// Restart game specifically from pause state (not from game end)
		/// </summary> 
		private void RestartGameFromPause () {
			TakiLogger.LogInfo ("Restarting game from pause state", TakiLogger.LogCategory.GameState);

			// Directly start a new single player game
			// This bypasses the GameEndManager which expects a game-end state
			if (gameManager != null) {
				gameManager.StartNewSinglePlayerGame ();
				TakiLogger.LogInfo ("New game started from pause restart", TakiLogger.LogCategory.GameState);
			}
		}

		/// <summary>
		/// NEW: Verify AI state after restart to catch any issues
		/// </summary>
		private System.Collections.IEnumerator VerifyAIStateAfterRestart () {
			// Wait a moment for restart to complete
			yield return new WaitForSeconds (0.5f);

			TakiLogger.LogInfo ("Verifying AI state after restart...", TakiLogger.LogCategory.GameState);

			if (gameManager != null && gameManager.computerAI != null) {
				bool aiIsPaused = gameManager.computerAI.IsAIPaused;
				bool gameIsActive = gameManager.IsGameActive;

				TakiLogger.LogInfo ($"Post-restart verification - AI Paused: {aiIsPaused}, Game Active: {gameIsActive}", TakiLogger.LogCategory.GameState);

				if (aiIsPaused && gameIsActive) {
					TakiLogger.LogError ("PROBLEM DETECTED - AI still paused after restart!", TakiLogger.LogCategory.GameState);

					// IMPROVED Emergency fix - try multiple approaches
					TakiLogger.LogInfo ("Applying comprehensive emergency AI fix...", TakiLogger.LogCategory.GameState);

					// Approach 1: Force complete reset
					gameManager.computerAI.ForceCompleteReset ();

					// Wait a moment
					yield return new WaitForSeconds (0.1f);

					// Check if that worked
					if (gameManager.computerAI.IsAIPaused) {
						TakiLogger.LogError ("Force reset failed - trying additional fixes...", TakiLogger.LogCategory.GameState);

						// Approach 2: Try resume then clear
						gameManager.computerAI.ResumeAI ();
						yield return new WaitForSeconds (0.1f);
						gameManager.computerAI.ClearHand ();

						// Approach 3: Final check and manual field reset if needed
						if (gameManager.computerAI.IsAIPaused) {
							TakiLogger.LogError ("All fixes failed - AI may be permanently stuck", TakiLogger.LogCategory.GameState);
							TakiLogger.LogError ("Please restart the game completely", TakiLogger.LogCategory.GameState);
						} else {
							TakiLogger.LogInfo ("AI unstuck with approach 2", TakiLogger.LogCategory.GameState);
						}
					} else {
						TakiLogger.LogInfo ("AI unstuck with force reset", TakiLogger.LogCategory.GameState);
					}

				} else {
					TakiLogger.LogInfo ("AI state verification passed - restart successful", TakiLogger.LogCategory.GameState);
				}
			}
		}

		/// <summary>
		/// Handle Btn_GoHome click from pause screen - return to main menu
		/// ENHANCED: Disconnect from Photon if in multiplayer
		/// </summary>
		public void Btn_GoHomeFromPauseLogic () {
			TakiLogger.LogInfo ("Go Home button clicked FROM PAUSE", TakiLogger.LogCategory.UI);

			// Hide pause screen first
			HidePauseScreenOverlay ();

			// Disconnect from Photon if connected
			if (multiplayerMenuLogic != null && multiplayerMenuLogic.IsConnectedToPhoton) {
				TakiLogger.LogInfo ("Disconnecting from Photon...", TakiLogger.LogCategory.Multiplayer);
				multiplayerMenuLogic.DisconnectFromPhoton ();
			}

			// Use loading screen transition to main menu
			StartCoroutine (GoToMainMenuWithLoadingCoroutine ());
		}

		/// <summary>
		/// Enhanced Btn_GoHome logic that works from both pause and game end
		/// ENHANCED: Disconnect from Photon if in multiplayer
		/// </summary>
		public void Btn_GoHomeLogic () {
			TakiLogger.LogInfo ("Go Home button clicked", TakiLogger.LogCategory.UI);

			// Check if we're coming from pause screen
			if (Screen_Paused != null && Screen_Paused.activeSelf) {
				Btn_GoHomeFromPauseLogic ();
			} else {
				// This is from game end screen - use existing logic with Photon disconnect

				// Disconnect from Photon if connected
				if (multiplayerMenuLogic != null && multiplayerMenuLogic.IsConnectedToPhoton) {
					TakiLogger.LogInfo ("Disconnecting from Photon...", TakiLogger.LogCategory.Multiplayer);
					multiplayerMenuLogic.DisconnectFromPhoton ();
				}

				StartCoroutine (GoToMainMenuWithLoadingCoroutine ());
			}
		}

		/// <summary>
		/// Go to main menu with loading screen transition - COROUTINE VERSION
		/// </summary>
		private IEnumerator GoToMainMenuWithLoadingCoroutine () {
			TakiLogger.LogInfo ("Transitioning to main menu with loading screen", TakiLogger.LogCategory.UI);
			yield return StartCoroutine (ShowScreenTemporarily (LoadingScreen, MainMenuScreen, clearStack: true));
		}

		/// <summary>
		/// Go to main menu with loading screen transition - PUBLIC METHOD for external use
		/// </summary>
		public void GoToMainMenuWithLoading () {
			TakiLogger.LogInfo ("Go to main menu with loading screen requested", TakiLogger.LogCategory.UI);
			StartCoroutine (GoToMainMenuWithLoadingCoroutine ());
		}

		#endregion

		#region Enhanced Exit Logic

		/// <summary>
		/// Handle Btn_Exit click - show exit validation instead of direct exit
		/// ENHANCED: Disconnect from Photon before exit
		/// </summary>
		public void Btn_ExitLogic () {
			TakiLogger.LogInfo ("Exit button clicked - showing validation", TakiLogger.LogCategory.UI);

			// Disconnect from Photon if connected
			if (multiplayerMenuLogic != null && multiplayerMenuLogic.IsConnectedToPhoton) {
				TakiLogger.LogInfo ("Disconnecting from Photon before exit...", TakiLogger.LogCategory.Multiplayer);
				multiplayerMenuLogic.DisconnectFromPhoton ();
			}

			// Request exit confirmation through GameManager
			if (gameManager != null) {
				gameManager.RequestExitConfirmation ();
			} else {
				TakiLogger.LogError ("Cannot show exit confirmation - GameManager not assigned!", TakiLogger.LogCategory.System);
				// Fallback to direct validation screen
				SetScreen (ExitValidationScreen);
			}
		}

		/// <summary>
		/// Show exit validation screen as overlay (similar to pause)
		/// </summary>
		private void ShowExitValidationOverlay () {
			if (ExitValidationScreen == null) {
				TakiLogger.LogError ("ExitValidationScreen not assigned!", TakiLogger.LogCategory.UI);
				return;
			}

			// Show as overlay, keeping the game screen visible underneath
			ExitValidationScreen.SetActive (true);
			TakiLogger.LogInfo ("Exit validation screen shown as overlay", TakiLogger.LogCategory.UI);
		}

		/// <summary>
		/// Handle Btn_ExitCancel click - treat as continue/resume
		/// </summary>
		public void Btn_ExitCancelLogic () {
			TakiLogger.LogInfo ("Exit cancelled - treating as continue", TakiLogger.LogCategory.UI);

			// Let ExitValidationManager handle the cancellation logic
			if (gameManager != null && gameManager.exitValidationManager != null) {
				gameManager.exitValidationManager.CancelExit ();
			} else {
				TakiLogger.LogWarning ("ExitValidationManager not available - using fallback", TakiLogger.LogCategory.System);

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
			TakiLogger.LogInfo ("Exit confirmed - application will close", TakiLogger.LogCategory.UI);

			// Let ExitValidationManager handle the confirmation
			if (gameManager != null && gameManager.exitValidationManager != null) {
				gameManager.exitValidationManager.ConfirmExit ();
			} else {
				TakiLogger.LogWarning ("ExitValidationManager not available - using direct exit", TakiLogger.LogCategory.System);
				// Existing exit logic will run
				StartCoroutine (ShowExitingScreenAndQuit ());
			}
		}

		public void StartExitSequence () {
			TakiLogger.LogInfo ("Starting exit sequence", TakiLogger.LogCategory.UI);
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
			TakiLogger.LogInfo ("Exiting application (Editor mode)", TakiLogger.LogCategory.System);
#else
            // Close the application in builds
            Application.Quit();
            TakiLogger.LogInfo("Exiting application", TakiLogger.LogCategory.System);
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
			TakiLogger.LogInfo ($"Stack Count: {screenStack.Count}", TakiLogger.LogCategory.UI);
			if (screenStack.Count > 0) {
				TakiLogger.LogInfo ($"Current Screen: {screenStack.Peek ().name}", TakiLogger.LogCategory.UI);
			}
		}

		/// <summary>
		/// Debug method to log multiplayer status
		/// </summary>
		[System.Diagnostics.Conditional ("UNITY_EDITOR")]
		public void LogMultiplayerStatus () {
			if (multiplayerMenuLogic != null) {
				TakiLogger.LogInfo ($"Photon Connected: {multiplayerMenuLogic.IsConnectedToPhoton}", TakiLogger.LogCategory.Multiplayer);
				TakiLogger.LogInfo ($"Room Status: {multiplayerMenuLogic.GetRoomStatus ()}", TakiLogger.LogCategory.Multiplayer);
			} else {
				TakiLogger.LogInfo ("MultiplayerMenuLogic not found", TakiLogger.LogCategory.Multiplayer);
			}
		}

		#endregion

		// Also add this debug method to check screen references
		[ContextMenu ("Debug Screen References")]
		public void DebugScreenReferences () {
			TakiLogger.LogInfo ("=== Screen References ===", TakiLogger.LogCategory.UI);
			TakiLogger.LogInfo ($"MainMenuScreen: {(MainMenuScreen != null ? MainMenuScreen.name : "NULL")}", TakiLogger.LogCategory.UI);
			TakiLogger.LogInfo ($"SinglePlayerScreen: {(SinglePlayerScreen != null ? SinglePlayerScreen.name : "NULL")}", TakiLogger.LogCategory.UI);
			TakiLogger.LogInfo ($"MultiPlayerScreen: {(MultiPlayerScreen != null ? MultiPlayerScreen.name : "NULL")}", TakiLogger.LogCategory.UI);
			TakiLogger.LogInfo ($"SinglePlayerGameScreen: {(SinglePlayerGameScreen != null ? SinglePlayerGameScreen.name : "NULL")}", TakiLogger.LogCategory.UI);
			TakiLogger.LogInfo ($"MultiPlayerGameScreen: {(MultiPlayerGameScreen != null ? MultiPlayerGameScreen.name : "NULL")}", TakiLogger.LogCategory.UI);
			TakiLogger.LogInfo ($"LoadingScreen: {(LoadingScreen != null ? LoadingScreen.name : "NULL")}", TakiLogger.LogCategory.UI);
			TakiLogger.LogInfo ($"multiplayerMenuLogic: {(multiplayerMenuLogic != null ? "FOUND" : "NULL")}", TakiLogger.LogCategory.Multiplayer);
			TakiLogger.LogInfo ("========================================", TakiLogger.LogCategory.UI);
		}
	}
}