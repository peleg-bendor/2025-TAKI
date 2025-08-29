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

		public void Btn_ExitLogic () {
			SetScreen (ExitValidationScreen);
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

		#region Exit Validation Logic

		public void Btn_ExitConfirmLogic () {
			// Start graceful exit sequence with visual feedback
			StartCoroutine (ShowExitingScreenAndQuit ());
		}

		public void Btn_ExitCancelLogic () {
			GoBack ();
		}

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
