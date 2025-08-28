using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// Helper script for Milestone 5 integration testing and validation
	/// Updated to work with new GameplayUIManager architecture
	/// </summary>
	public class Milestone5IntegrationHelper : MonoBehaviour {

		[Header ("System References")]
		public GameManager gameManager;
		public DeckManager deckManager;
		public GameStateManager gameState;
		public TurnManager turnManager;
		public BasicComputerAI computerAI;
		public GameplayUIManager gameplayUI;

		[Header ("Testing Options")]
		public bool autoStartGameOnPlay = true;
		public bool verboseLogging = true;

		void Start () {
			FindSystemReferences ();
			if (autoStartGameOnPlay) {
				Invoke (nameof (StartTestGame), 1f); // Small delay to let systems initialize
			}
		}

		/// <summary>
		/// Find all system references automatically
		/// </summary>
		void FindSystemReferences () {
			if (gameManager == null) gameManager = FindObjectOfType<GameManager> ();
			if (deckManager == null) deckManager = FindObjectOfType<DeckManager> ();
			if (gameState == null) gameState = FindObjectOfType<GameStateManager> ();
			if (turnManager == null) turnManager = FindObjectOfType<TurnManager> ();
			if (computerAI == null) computerAI = FindObjectOfType<BasicComputerAI> ();
			if (gameplayUI == null) gameplayUI = FindObjectOfType<GameplayUIManager> ();

			Debug.Log ("=== Milestone 5 Integration Helper ===");
			Debug.Log ($"GameManager found: {gameManager != null}");
			Debug.Log ($"DeckManager found: {deckManager != null}");
			Debug.Log ($"GameState found: {gameState != null}");
			Debug.Log ($"TurnManager found: {turnManager != null}");
			Debug.Log ($"ComputerAI found: {computerAI != null}");
			Debug.Log ($"GameplayUI found: {gameplayUI != null}");
		}

		/// <summary>
		/// Start a test game
		/// </summary>
		[ContextMenu ("Start Test Game")]
		public void StartTestGame () {
			if (gameManager != null) {
				Debug.Log ("=== STARTING TEST GAME ===");
				gameManager.StartNewGame ();
			} else {
				Debug.LogError ("Cannot start test game: GameManager not found!");
			}
		}

		/// <summary>
		/// Force player to draw a card (for testing)
		/// </summary>
		[ContextMenu ("Force Player Draw")]
		public void ForcePlayerDraw () {
			if (gameManager != null) {
				Debug.Log ("=== FORCING PLAYER DRAW ===");
				gameManager.RequestDrawCard ();
			}
		}

		/// <summary>
		/// Skip to computer turn (for testing)
		/// </summary>
		[ContextMenu ("Skip To Computer Turn")]
		public void SkipToComputerTurn () {
			if (turnManager != null) {
				Debug.Log ("=== SKIPPING TO COMPUTER TURN ===");
				turnManager.StartTurn (PlayerType.Computer);
			}
		}

		/// <summary>
		/// Test color selection system
		/// </summary>
		[ContextMenu ("Test Color Selection")]
		public void TestColorSelection () {
			if (gameplayUI != null) {
				Debug.Log ("=== TESTING COLOR SELECTION ===");
				gameplayUI.ShowColorSelection (true);
			}
		}

		/// <summary>
		/// Validate all systems are properly connected
		/// </summary>
		[ContextMenu ("Validate All Systems")]
		public void ValidateAllSystems () {
			Debug.Log ("=== COMPLETE SYSTEM VALIDATION ===");

			// Check GameManager
			ValidateGameManager ();

			// Check DeckManager  
			ValidateDeckManager ();

			// Check Game State
			ValidateGameState ();

			// Check Turn Manager
			ValidateTurnManager ();

			// Check Computer AI
			ValidateComputerAI ();

			// Check Gameplay UI
			ValidateGameplayUI ();

			Debug.Log ("=== VALIDATION COMPLETE ===");
		}

		/// <summary>
		/// Validate GameManager integration
		/// </summary>
		void ValidateGameManager () {
			Debug.Log ("--- GameManager Validation ---");

			if (gameManager == null) {
				Debug.LogError ("GameManager is null!");
				return;
			}

			Debug.Log ($"IsInitialized: {gameManager.IsInitialized}");
			Debug.Log ($"IsGameActive: {gameManager.IsGameActive}");
			Debug.Log ($"CurrentPlayer: {gameManager.CurrentPlayer}");
			Debug.Log ($"PlayerHandSize: {gameManager.PlayerHandSize}");
			Debug.Log ($"ComputerHandSize: {gameManager.ComputerHandSize}");
			Debug.Log ($"ActiveColor: {gameManager.ActiveColor}");
		}

		/// <summary>
		/// Validate DeckManager integration
		/// </summary>
		void ValidateDeckManager () {
			Debug.Log ("--- DeckManager Validation ---");

			if (deckManager == null) {
				Debug.LogError ("DeckManager is null!");
				return;
			}

			Debug.Log ($"DrawPileCount: {deckManager.DrawPileCount}");
			Debug.Log ($"DiscardPileCount: {deckManager.DiscardPileCount}");
			Debug.Log ($"HasValidDeck: {deckManager.HasValidDeck}");
			Debug.Log ($"CanSetupNewGame: {deckManager.CanSetupNewGame}");
		}

		/// <summary>
		/// Validate GameState integration
		/// </summary>
		void ValidateGameState () {
			Debug.Log ("--- GameState Validation ---");

			if (gameState == null) {
				Debug.LogError ("GameState is null!");
				return;
			}

			Debug.Log ($"TurnState: {gameState.turnState}");
			Debug.Log ($"InteractionState: {gameState.interactionState}");
			Debug.Log ($"GameStatus: {gameState.gameStatus}");
			Debug.Log ($"ActiveColor: {gameState.activeColor}");
			Debug.Log ($"CanPlayerAct: {gameState.CanPlayerAct ()}");
		}

		/// <summary>
		/// Validate TurnManager integration
		/// </summary>
		void ValidateTurnManager () {
			Debug.Log ("--- TurnManager Validation ---");

			if (turnManager == null) {
				Debug.LogError ("TurnManager is null!");
				return;
			}

			Debug.Log ($"CurrentPlayer: {turnManager.CurrentPlayer}");
			Debug.Log ($"IsHumanTurn: {turnManager.IsHumanTurn}");
			Debug.Log ($"IsComputerTurn: {turnManager.IsComputerTurn}");
			Debug.Log ($"AreTurnsActive: {turnManager.AreTurnsActive ()}");
		}

		/// <summary>
		/// Validate ComputerAI integration
		/// </summary>
		void ValidateComputerAI () {
			Debug.Log ("--- ComputerAI Validation ---");

			if (computerAI == null) {
				Debug.LogError ("ComputerAI is null!");
				return;
			}

			Debug.Log ($"HandSize: {computerAI.HandSize}");
			Debug.Log ($"HasCards: {computerAI.HasCards}");
		}

		/// <summary>
		/// Validate GameplayUI integration
		/// </summary>
		void ValidateGameplayUI () {
			Debug.Log ("--- GameplayUI Validation ---");

			if (gameplayUI == null) {
				Debug.LogError ("GameplayUI is null!");
				return;
			}

			Debug.Log ($"ButtonsEnabled: {gameplayUI.ButtonsEnabled}");
			Debug.Log ($"IsColorSelectionActive: {gameplayUI.IsColorSelectionActive}");

			// Check if UI elements are assigned (updated property names)
			bool hasRequiredElements =
				gameplayUI.turnIndicatorText != null &&
				gameplayUI.player1HandSizeText != null &&
				gameplayUI.player2HandSizeText != null &&
				gameplayUI.player2MessageText != null;

			Debug.Log ($"HasRequiredUIElements: {hasRequiredElements}");

			if (!hasRequiredElements) {
				Debug.LogWarning ("Some UI elements are not assigned in GameplayUIManager!");

				// Check each element individually
				Debug.Log ($"turnIndicatorText assigned: {gameplayUI.turnIndicatorText != null}");
				Debug.Log ($"player1HandSizeText assigned: {gameplayUI.player1HandSizeText != null}");
				Debug.Log ($"player2HandSizeText assigned: {gameplayUI.player2HandSizeText != null}");
				Debug.Log ($"player2MessageText assigned: {gameplayUI.player2MessageText != null}");
				Debug.Log ($"currentColorIndicator assigned: {gameplayUI.currentColorIndicator != null}");
				Debug.Log ($"playCardButton assigned: {gameplayUI.playCardButton != null}");
				Debug.Log ($"drawCardButton assigned: {gameplayUI.drawCardButton != null}");
			}
		}

		/// <summary>
		/// Show current game state in a readable format
		/// </summary>
		[ContextMenu ("Show Game State")]
		public void ShowCurrentGameState () {
			Debug.Log ("=== CURRENT GAME STATE ===");

			if (gameState != null) {
				Debug.Log ($"State Description: {gameState.GetStateDescription ()}");
				Debug.Log ($"Turn Description: {gameState.GetTurnDescription ()}");
			}

			if (gameManager != null) {
				Debug.Log ($"Player Hand: {gameManager.PlayerHandSize} cards");
				Debug.Log ($"Computer Hand: {gameManager.ComputerHandSize} cards");

				var topCard = gameManager.GetTopDiscardCard ();
				if (topCard != null) {
					Debug.Log ($"Top Discard: {topCard.GetDisplayText ()}");
				}
			}
		}

		/// <summary>
		/// Test AI decision making
		/// </summary>
		[ContextMenu ("Test AI Decision")]
		public void TestAIDecision () {
			if (computerAI != null && deckManager != null) {
				Debug.Log ("=== TESTING AI DECISION ===");
				CardData topCard = deckManager.GetTopDiscardCard ();
				if (topCard != null) {
					computerAI.MakeDecision (topCard);
				} else {
					Debug.LogWarning ("No top card available for AI decision test");
				}
			}
		}

		/// <summary>
		/// Simulate a complete turn cycle
		/// </summary>
		[ContextMenu ("Simulate Turn Cycle")]
		public void SimulateTurnCycle () {
			Debug.Log ("=== SIMULATING TURN CYCLE ===");

			// Player draws a card
			ForcePlayerDraw ();

			// Wait a moment, then trigger computer turn
			Invoke (nameof (DelayedComputerTurn), 2f);
		}

		void DelayedComputerTurn () {
			TestAIDecision ();
		}

		// Properties for easy access in Inspector
		public bool AllSystemsFound => gameManager != null && deckManager != null &&
									   gameState != null && turnManager != null &&
									   computerAI != null && gameplayUI != null;

		public string SystemStatus => $"Systems Found: {(AllSystemsFound ? "ALL" : "MISSING")}";
	}
}