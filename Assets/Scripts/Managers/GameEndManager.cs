using UnityEngine;
using TMPro;
using System.Collections;

namespace TakiGame {
	/// <summary>
	/// Handles ALL game end scenarios following Single Responsibility Principle
	/// - Win condition detection and processing
	/// - Game over screen management
	/// - Post-game actions (restart, return to menu)
	/// - Final cleanup and reset coordination
	/// </summary>
	public class GameEndManager : MonoBehaviour {

		[Header ("Game End UI References")]
		[Tooltip ("Screen_GameEnd GameObject")]
		public GameObject gameEndScreen;

		[Tooltip ("EndDeclarationText - Winner announcement")]
		public TextMeshProUGUI endDeclarationText;

		[Header ("Dependencies")]
		[Tooltip ("Reference to GameManager for restart coordination")]
		public GameManager gameManager;

		[Tooltip ("Reference to GameStateManager for state management")]
		public GameStateManager gameState;

		[Tooltip ("Reference to MenuNavigation for screen transitions")]
		public MenuNavigation menuNavigation;

		[Tooltip ("Reference to GameplayUIManager for UI updates")]
		public GameplayUIManager gameplayUI; // DEPRECATED: Use GetActiveUI() instead

		[Header ("Game End Settings")]
		[Tooltip ("Time to display winner before showing options")]
		public float winnerDisplayTime = 2.0f;

		[Tooltip ("Transition time for screen changes")]
		public float transitionTime = 0.5f;

		// Game end state tracking
		private PlayerType gameWinner = PlayerType.Human;
		private bool gameEndProcessed = false;

		// Events for game end system
		public System.Action<PlayerType> OnGameEnded;
		public System.Action OnGameRestarted;
		public System.Action OnReturnedToMenu;

		void Start () {
			// Find dependencies if not assigned
			FindDependenciesIfMissing ();

			// Ensure game end screen starts hidden
			if (gameEndScreen != null) {
				gameEndScreen.SetActive (false);
			}
		}

		/// <summary>
		/// Process game end with winner - main entry point
		/// </summary>
		/// <param name="winner">The winning player</param>
		public void ProcessGameEnd (PlayerType winner) {
			if (gameEndProcessed) {
				TakiLogger.LogWarning ("Game end already processed", TakiLogger.LogCategory.System);
				return;
			}

			TakiLogger.LogGameState ($"=== PROCESSING GAME END - Winner: {winner} ===");

			gameWinner = winner;
			gameEndProcessed = true;

			// Just log that we're processing the end, don't change game state again
			TakiLogger.LogGameState ("Game state should already be GameOver - not calling DeclareWinner again");

			// Disable all gameplay UI
			// Disable all gameplay UI using active UI manager
			if (gameManager.GetActiveUI() != null) {
				gameManager.GetActiveUI().UpdateStrictButtonStates (false, false, false); // Disable all buttons
			}

			// Show game end sequence
			StartCoroutine (ShowGameEndSequence (winner));

			OnGameEnded?.Invoke (winner);
		}

		/// <summary>
		/// Show game end sequence with winner announcement
		/// </summary>
		/// <param name="winner">The winning player</param>
		private IEnumerator ShowGameEndSequence (PlayerType winner) {
			TakiLogger.LogUI ("Starting game end sequence");

			// First, show winner in gameplay UI for a moment
			if (gameManager.GetActiveUI() != null) {
				gameManager.GetActiveUI().ShowWinnerAnnouncement (winner);
			}

			// Wait for winner display time
			yield return new WaitForSeconds (winnerDisplayTime);

			// Show game end screen
			ShowGameEndScreen (winner);

			TakiLogger.LogUI ("Game end sequence complete");
		}

		/// <summary>
		/// Show the game end screen with winner announcement
		/// </summary>
		/// <param name="winner">The winning player</param>
		public void ShowGameEndScreen (PlayerType winner) {
			TakiLogger.LogUI ($"Showing game end screen for winner: {winner}");

			if (gameEndScreen == null) {
				TakiLogger.LogError ("Cannot show game end screen: gameEndScreen is null", TakiLogger.LogCategory.UI);
				return;
			}

			// Update winner text
			UpdateWinnerText (winner);

			// Show the screen
			gameEndScreen.SetActive (true);

			TakiLogger.LogUI ("Game end screen displayed");
		}

		/// <summary>
		/// Update winner announcement text
		/// </summary>
		/// <param name="winner">The winning player</param>
		void UpdateWinnerText (PlayerType winner) {
			if (endDeclarationText == null) {
				TakiLogger.LogError ("Cannot update winner text: endDeclarationText is null", TakiLogger.LogCategory.UI);
				return;
			}

			string winnerMessage = GetWinnerMessage (winner);
			endDeclarationText.text = winnerMessage;

			TakiLogger.LogUI ($"Winner text updated: '{winnerMessage}'");
		}

		/// <summary>
		/// Get appropriate winner message
		/// </summary>
		/// <param name="winner">The winning player</param>
		/// <returns>Winner message string</returns>
		string GetWinnerMessage (PlayerType winner) {
			switch (winner) {
				case PlayerType.Human:
					return "Congratulations!\nYou Win!";
				case PlayerType.Computer:
					return "Game Over!\nComputer Wins!";
				default:
					return "Game Over!";
			}
		}

		/// <summary>
		/// Handle restart button click - start new game on same screen
		/// </summary>
		public void OnRestartButtonClicked () {
			TakiLogger.LogSystem ("=== RESTART BUTTON CLICKED ===");

			if (!gameEndProcessed) {
				TakiLogger.LogWarning ("Restart clicked but game end not processed", TakiLogger.LogCategory.System);
				return;
			}

			StartCoroutine (RestartGameSequence ());
		}

		/// <summary>
		/// Restart game sequence with smooth transition
		/// </summary>
		private IEnumerator RestartGameSequence () {
			TakiLogger.LogSystem ("Starting restart sequence");

			// Hide game end screen
			if (gameEndScreen != null) {
				gameEndScreen.SetActive (false);
			}

			// Brief transition delay
			yield return new WaitForSeconds (0.2f);

			// Reset game end state
			ResetGameEndState ();

			// Start new game through GameManager
			if (gameManager != null) {
				TakiLogger.LogSystem ("Requesting new game from GameManager");
				gameManager.StartNewSinglePlayerGame ();
			} else {
				TakiLogger.LogError ("Cannot restart: GameManager is null", TakiLogger.LogCategory.System);
			}

			OnGameRestarted?.Invoke ();
			TakiLogger.LogSystem ("Restart sequence complete");
		}

		/// <summary>
		/// Handle go home button click - return to main menu
		/// </summary>
		public void OnGoHomeButtonClicked () {
			TakiLogger.LogSystem ("=== GO HOME BUTTON CLICKED ===");

			if (!gameEndProcessed) {
				TakiLogger.LogWarning ("Go Home clicked but game end not processed", TakiLogger.LogCategory.System);
				return;
			}

			StartCoroutine (GoHomeSequence ());
		}

		/// <summary>
		/// Go home sequence with proper cleanup
		/// </summary>
		private IEnumerator GoHomeSequence () {
			TakiLogger.LogSystem ("Starting go home sequence");

			// Hide game end screen
			if (gameEndScreen != null) {
				gameEndScreen.SetActive (false);
			}

			// Show loading screen through MenuNavigation
			if (menuNavigation != null) {
				// Use MenuNavigation's loading screen transition
				yield return StartCoroutine (GoHomeWithLoadingScreen ());
			} else {
				TakiLogger.LogError ("Cannot go home: MenuNavigation is null", TakiLogger.LogCategory.System);
			}

			OnReturnedToMenu?.Invoke ();
			TakiLogger.LogSystem ("Go home sequence complete");
		}

		/// <summary>
		/// Go home with loading screen transition
		/// </summary>
		private IEnumerator GoHomeWithLoadingScreen () {
			TakiLogger.LogSystem ("Transitioning to main menu with loading screen");

			// Clean up game state
			CleanupGameState ();

			// Use MenuNavigation to show loading then main menu
			// This will use the existing loading screen functionality
			if (menuNavigation != null) {
				// Navigate to loading screen, then main menu
				// MenuNavigation handles the screen stack properly
				menuNavigation.GoToMainMenuWithLoading (); // We'll add this method to MenuNavigation
			}

			yield return null; // Complete coroutine
		}

		/// <summary>
		/// Clean up game state before returning to menu
		/// </summary>
		void CleanupGameState () {
			TakiLogger.LogSystem ("Cleaning up game state for menu return");

			// Reset game end state
			ResetGameEndState ();

			// Reset game state manager
			if (gameState != null) {
				gameState.ResetGameState ();
			}

			// Reset GameManager if needed
			if (gameManager != null) {
				// GameManager should handle its own cleanup
				TakiLogger.LogSystem ("GameManager cleanup requested");
			}

			TakiLogger.LogSystem ("Game state cleanup complete");
		}

		/// <summary>
		/// Reset game end manager state for new game
		/// </summary>
		void ResetGameEndState () {
			gameWinner = PlayerType.Human;
			gameEndProcessed = false;

			// Hide game end screen if visible
			if (gameEndScreen != null) {
				gameEndScreen.SetActive (false);
			}

			TakiLogger.LogSystem ("Game end state reset");
		}

		/// <summary>
		/// Hide game end screen (for external use)
		/// </summary>
		public void HideGameEndScreen () {
			if (gameEndScreen != null) {
				gameEndScreen.SetActive (false);
			}
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

			if (menuNavigation == null) {
				menuNavigation = FindObjectOfType<MenuNavigation> ();
			}

			// DEPRECATED: gameplayUI field no longer used - GameManager.GetActiveUI() provides active UI manager
			if (gameplayUI == null) {
				gameplayUI = FindObjectOfType<GameplayUIManager> (); // Legacy fallback for Inspector assignment
			}

			// Find UI references if not assigned
			if (gameEndScreen == null) {
				// Try to find Screen_GameEnd in scene hierarchy
				GameObject canvas = GameObject.Find ("Canvas");
				if (canvas != null) {
					Transform screenTransform = canvas.transform.Find ("Screen_GameEnd");
					if (screenTransform != null) {
						gameEndScreen = screenTransform.gameObject;
						TakiLogger.LogSystem ("Screen_GameEnd found automatically");
					}
				}
			}

			if (endDeclarationText == null && gameEndScreen != null) {
				// Try to find EndDeclarationText within game end screen
				TextMeshProUGUI textComponent = gameEndScreen.GetComponentInChildren<TextMeshProUGUI> ();
				if (textComponent != null) {
					endDeclarationText = textComponent;
					TakiLogger.LogSystem ("EndDeclarationText found automatically");
				}
			}

			TakiLogger.LogSystem ("GameEndManager dependencies resolved");
		}

		/// <summary>
		/// Debug method to test game end screen
		/// </summary>
		[ContextMenu ("Test Game End Screen")]
		public void TestGameEndScreen () {
			TakiLogger.LogDiagnostics ("Testing game end screen with human winner");
			ProcessGameEnd (PlayerType.Human);
		}

		/// <summary>
		/// Debug method to log game end state
		/// </summary> 
		[ContextMenu ("Log Game End State")]
		public void LogGameEndState () {
			TakiLogger.LogDiagnostics ("=== GAME END STATE DEBUG ===");
			TakiLogger.LogDiagnostics ($"Game End Processed: {gameEndProcessed}");
			TakiLogger.LogDiagnostics ($"Game Winner: {gameWinner}");
			TakiLogger.LogDiagnostics ($"Game End Screen Active: {(gameEndScreen != null ? gameEndScreen.activeSelf : false)}");
			TakiLogger.LogDiagnostics ($"End Text Content: '{(endDeclarationText != null ? endDeclarationText.text : "NULL")}'");
		}

		// Properties 
		public bool IsGameEndProcessed => gameEndProcessed;
		public PlayerType GameWinner => gameWinner;
		public bool IsGameEndScreenVisible => gameEndScreen != null && gameEndScreen.activeSelf;
	}
}