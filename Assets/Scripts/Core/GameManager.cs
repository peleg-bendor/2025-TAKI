using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// Central Game Coordinator - Handles both SinglePlayer and Multiplayer modes
	///
	/// ARCHITECTURE:
	/// - Event-driven architecture with single responsibility pattern
	/// - Dual UI architecture: BaseGameplayUIManager with concrete implementations
	/// - Per-screen HandManager system for singleplayer/multiplayer separation
	/// - Clean separation between game logic and UI presentation
	///
	/// GAME MODES:
	/// - SinglePlayer: Complete implementation with AI opponent and pause system
	/// - Multiplayer: Photon PUN2 integration with turn-based gameplay
	///
	/// TURN FLOW SYSTEM:
	/// - Strict one-action-per-turn flow (PLAY or DRAW then END TURN)
	/// - Comprehensive special card support (TAKI sequences, PlusTwo chains, etc.)
	/// - State preservation for pause/resume functionality
	///
	/// NETWORKING:
	/// - Master/Client pattern with state synchronization
	/// - Network-aware UI updates and turn management
	/// - Hand privacy system for multiplayer
	/// </summary>

	public class GameManager : MonoBehaviour {

		#region Core Properties and Fields
		// Essential game state, configuration, and component references

		[Header ("Component References")]
		[Tooltip ("Manages game state using multi-enum architecture")]
		public GameStateManager gameState;

		[Tooltip ("Manages turns and player switching")]
		public TurnManager turnManager;

		[Tooltip ("Computer AI decision making")]
		public BasicComputerAI computerAI;

		[Tooltip ("Deck management system")]
		public DeckManager deckManager;

		[Header ("UI Architecture")]
		[Tooltip ("UI manager for Screen_SinglePlayerGame")]
		public SinglePlayerUIManager singlePlayerUI;

		[Tooltip ("UI manager for Screen_MultiPlayerGame")]
		public MultiPlayerUIManager multiPlayerUI;

		[Header ("Hand Manager Architecture")]
		[Tooltip ("Player 1 (Human) hand manager for Screen_SinglePlayerGame")]
		public HandManager singleplayerPlayerHandManager;

		[Tooltip ("Player 2 (Computer) hand manager for Screen_SinglePlayerGame")]
		public HandManager singleplayerOpponentHandManager;

		[Tooltip ("Player 1 hand manager for Screen_MultiPlayerGame")]
		public HandManager multiplayerPlayerHandManager;

		[Tooltip ("Player 2 hand manager for Screen_MultiPlayerGame")]
		public HandManager multiplayerOpponentHandManager;

		[Header ("Game Setup")]
		[Tooltip ("Starting player for new games")]
		public PlayerType startingPlayer = PlayerType.Human;

		[Tooltip ("Player's hand of cards (Player1 = Human)")]
		public List<CardData> playerHand = new List<CardData> ();

		[Header ("System Managers")]
		[Tooltip ("Manages pause/resume functionality")]
		public PauseManager pauseManager;

		[Tooltip ("Manages game end scenarios")]
		public GameEndManager gameEndManager;

		[Tooltip ("Manages exit confirmation")]
		public ExitValidationManager exitValidationManager;

		[Tooltip ("Network game manager for multiplayer coordination")]
		public NetworkGameManager networkGameManager;

		[Header ("Logging Configuration")]
		[Tooltip ("Log level for console output")]
		public TakiLogger.LogLevel logLevel = TakiLogger.LogLevel.Debug;

		[Tooltip ("Enable production mode (minimal logging)")]
		public bool productionMode = false;

		// Events for external systems
		public System.Action OnGameStarted;
		public System.Action<PlayerType> OnGameEnded;
		public System.Action<PlayerType> OnTurnStarted;
		public System.Action<CardData> OnCardPlayed;

		#endregion

		#region Private State Management
		// Internal state tracking and control variables

		// System initialization state
		private bool areComponentsValidated = false;
		private bool areSystemsInitialized = false;
		private bool isGameActive = false;
		private bool isMultiplayerMode = false;

		// Turn flow control state
		private bool hasPlayerTakenAction = false;
		private bool canPlayerDraw = true;
		private bool canPlayerPlay = true;
		private bool canPlayerEndTurn = false;

		// Special card state tracking
		private bool isWaitingForAdditionalAction = false;
		private CardType activeSpecialCardEffect = CardType.Number; // Number = no special effect

		// STOP card state
		private bool shouldSkipNextTurn = false;
		private PlayerType stopCardPlayer = PlayerType.Human;

		// TAKI sequence state
		private bool isCurrentCardLastInSequence = false;

		#endregion

		#region Public Properties
		// External API for accessing game state

		// Core game state properties
		public bool IsGameActive => isGameActive;
		public bool IsPlayerTurn => gameState?.IsPlayerTurn ?? false;
		public PlayerType CurrentPlayer => turnManager?.CurrentPlayer ?? PlayerType.Human;
		public int PlayerHandSize => playerHand.Count;
		public int ComputerHandSize => computerAI?.HandSize ?? 0;
		public CardColor ActiveColor => gameState?.activeColor ?? CardColor.Wild;

		// System state properties
		public bool AreComponentsValidated => areComponentsValidated;
		public bool AreSystemsInitialized => areSystemsInitialized;

		// Turn flow properties
		public bool HasPlayerTakenAction => hasPlayerTakenAction;
		public bool CanPlayerDrawCard => canPlayerDraw;
		public bool CanPlayerPlayCard => canPlayerPlay;
		public bool CanPlayerEndTurn => canPlayerEndTurn && !HasPendingSpecialCardEffects (); // ENHANCED

		// Special card state properties
		public bool IsWaitingForAdditionalAction => isWaitingForAdditionalAction; // NEW
		public CardType ActiveSpecialCardEffect => activeSpecialCardEffect; // NEW

		// Manager state properties
		public bool IsGamePaused => pauseManager != null && pauseManager.IsGamePaused;
		public bool IsGameEndProcessed => gameEndManager != null && gameEndManager.IsGameEndProcessed;
		public bool IsExitValidationActive => exitValidationManager != null && exitValidationManager.IsExitValidationActive;

		// Multiplayer properties
		public bool IsMultiplayerMode => isMultiplayerMode;
		public bool IsNetworkReady => isMultiplayerMode && networkGameManager != null && networkGameManager.IsDeckInitialized;
		public bool IsMyNetworkTurn => isMultiplayerMode && networkGameManager != null && networkGameManager.IsMyTurn;

		#endregion

		#region Architecture Management
		// UI and HandManager architecture abstraction methods

		/// <summary>
		/// Get the active UI manager based on current game mode
		/// </summary>
		public BaseGameplayUIManager GetActiveUI () {
			TakiLogger.LogSystem ($"GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: {isMultiplayerMode}");
			TakiLogger.LogSystem ($"  - singlePlayerUI: {(singlePlayerUI != null ? $"ASSIGNED ({singlePlayerUI.GetType ().Name})" : "NULL")}");
			TakiLogger.LogSystem ($"  - multiPlayerUI: {(multiPlayerUI != null ? $"ASSIGNED ({multiPlayerUI.GetType ().Name})" : "NULL")}");

			if (isMultiplayerMode && multiPlayerUI != null) {
				TakiLogger.LogSystem ($"GetActiveUI() returning multiPlayerUI: {multiPlayerUI.GetType ().Name}");
				return multiPlayerUI;
			} else if (!isMultiplayerMode && singlePlayerUI != null) {
				TakiLogger.LogSystem ($"GetActiveUI() returning singlePlayerUI: {singlePlayerUI.GetType ().Name}");
				return singlePlayerUI;
			}

			TakiLogger.LogError ($"GetActiveUI() returning NULL - No UI manager available for mode: {(isMultiplayerMode ? "Multiplayer" : "SinglePlayer")}", TakiLogger.LogCategory.UI);
			return null;
		}

		/// <summary>
		/// Get the active player hand manager based on current game mode
		/// </summary>
		public HandManager GetActivePlayerHandManager () {
			if (isMultiplayerMode && multiplayerPlayerHandManager != null) {
				return multiplayerPlayerHandManager;
			} else if (!isMultiplayerMode && singleplayerPlayerHandManager != null) {
				return singleplayerPlayerHandManager;
			}

			TakiLogger.LogError ($"No player hand manager available for mode: {(isMultiplayerMode ? "Multiplayer" : "SinglePlayer")}, using legacy", TakiLogger.LogCategory.UI);
			return null;
		}

		/// <summary>
		/// Get the active opponent/computer hand manager based on current game mode
		/// </summary>
		public HandManager GetActiveOpponentHandManager () {
			if (isMultiplayerMode && multiplayerOpponentHandManager != null) {
				return multiplayerOpponentHandManager;
			} else if (!isMultiplayerMode && singleplayerOpponentHandManager != null) {
				return singleplayerOpponentHandManager;
			}

			// Final fallback to legacy
			TakiLogger.LogError ($"No opponent hand manager available for mode: {(isMultiplayerMode ? "Multiplayer" : "SinglePlayer")}, using legacy", TakiLogger.LogCategory.UI);
			return null;
		}

		#endregion

		#region Unity Lifecycle
		// MonoBehaviour lifecycle methods

		void Start () {
			// Configure logging system
			ConfigureLogging ();

			// ONLY validate components exist - DON'T initialize game systems yet
			ValidateAndConnectComponents ();
		}

		/// <summary>
		/// Cleanup event connections on destroy to prevent memory leaks
		/// </summary>
		private void OnDestroy () {
			TakiLogger.LogSystem ("GameManager: OnDestroy - Cleaning up event connections");

			// Disconnect from both UI managers to ensure complete cleanup
			if (isMultiplayerMode && multiPlayerUI != null) {
				DisconnectUIManagerEvents (multiPlayerUI, "MultiPlayerUI");
			} else if (!isMultiplayerMode && singlePlayerUI != null) {
				DisconnectUIManagerEvents (singlePlayerUI, "SinglePlayerUI");
			}
		}

		#endregion

		#region System Initialization
		// Component validation, system setup, and game mode initialization

		/// <summary>
		/// Configure TakiLogger system
		/// </summary>
		void ConfigureLogging () {
			// TakiLogger.SetLogLevel (logLevel);
			TakiLogger.SetProductionMode (productionMode);
			TakiLogger.LogSystem ("TakiLogger configured: " + TakiLogger.GetLoggerInfo ());
		}

		/// <summary>
		/// Validate components exist and connect basic references
		/// </summary>
		void ValidateAndConnectComponents () {
			TakiLogger.LogSystem ("Validating and connecting components...", TakiLogger.LogLevel.Debug);

			// Validate components exist
			if (!ValidateComponents ()) {
				TakiLogger.LogError ("GameManager: Missing required components!", TakiLogger.LogCategory.System);
				return;
			}

			ConnectComponentReferences ();
			areComponentsValidated = true;
			TakiLogger.LogSystem ("Components validated and connected - Ready for game mode selection", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Validate all required components are assigned
		/// </summary>
		bool ValidateComponents () {
			bool isValid = true;

			if (gameState == null) {
				TakiLogger.LogError ("GameManager: GameStateManager not assigned!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (turnManager == null) {
				TakiLogger.LogError ("GameManager: TurnManager not assigned!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (computerAI == null) {
				TakiLogger.LogError ("GameManager: BasicComputerAI not assigned!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (deckManager == null) {
				TakiLogger.LogError ("GameManager: DeckManager not assigned!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (pauseManager == null) {
				TakiLogger.LogError ("GameManager: PauseManager not assigned - pause functionality will be disabled", TakiLogger.LogCategory.System);
			}

			if (singlePlayerUI == null) {
				TakiLogger.LogError ("GameManager: SinglePlayerUIManager not assigned but new UI architecture is enabled!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (multiPlayerUI == null) {
				TakiLogger.LogError ("GameManager: MultiPlayerUIManager not assigned but new UI architecture is enabled!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			TakiLogger.LogSystem ("New UI architecture validation complete - both UI managers assigned");

			// Validate per-screen HandManager architecture
			if (singleplayerPlayerHandManager == null) {
				TakiLogger.LogError ("GameManager: SinglePlayerPlayerHandManager not assigned but per-screen HandManager architecture is enabled!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (singleplayerOpponentHandManager == null) {
				TakiLogger.LogError ("GameManager: SingleplayerOpponentHandManager not assigned but per-screen HandManager architecture is enabled!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (multiplayerPlayerHandManager == null) {
				TakiLogger.LogError ("GameManager: MultiplayerPlayerHandManager not assigned but per-screen HandManager architecture is enabled!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (multiplayerOpponentHandManager == null) {
				TakiLogger.LogError ("GameManager: MultiplayerOpponentHandManager not assigned but per-screen HandManager architecture is enabled!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			TakiLogger.LogSystem ("Per-screen HandManager architecture validation complete - all HandManagers assigned");

			if (gameEndManager == null) {
				TakiLogger.LogError ("GameManager: GameEndManager not assigned - game end functionality will be limited", TakiLogger.LogCategory.System);
			}

			if (exitValidationManager == null) {
				TakiLogger.LogError ("GameManager: ExitValidationManager not assigned - exit confirmation will be disabled", TakiLogger.LogCategory.System);
			}

			if (isValid) {
				TakiLogger.LogSystem ("GameManager: ValidateComponents - All required components are assigned", TakiLogger.LogLevel.Debug);
			}

			return isValid;
		}

		/// <summary>
		/// Connect references between components (lightweight)
		/// </summary>
		void ConnectComponentReferences () {
			// Connect GameState reference to other components
			if (turnManager != null) {
				turnManager.gameState = gameState;
			}

			if (computerAI != null) {
				computerAI.gameState = gameState;
			}

			// Connect pause/game end manager references
			if (pauseManager != null) {
				pauseManager.gameManager = this;
				pauseManager.gameState = gameState;
				pauseManager.turnManager = turnManager;
				pauseManager.computerAI = computerAI;
				if (isMultiplayerMode && multiPlayerUI != null) {
					pauseManager.gameplayUI = multiPlayerUI;
				} else if (!isMultiplayerMode && singlePlayerUI != null) {
					pauseManager.gameplayUI = singlePlayerUI;
				}
				// pauseManager.menuNavigation will be found automatically
			}

			if (gameEndManager != null) {
				gameEndManager.gameManager = this;
				gameEndManager.gameState = gameState;
				if (isMultiplayerMode && multiPlayerUI != null) {
					gameEndManager.gameplayUI = multiPlayerUI;
				} else if (!isMultiplayerMode && singlePlayerUI != null) {
					gameEndManager.gameplayUI = singlePlayerUI;
				}
				// gameEndManager.menuNavigation will be found automatically
			}

			if (exitValidationManager != null) {
				exitValidationManager.pauseManager = pauseManager;
				exitValidationManager.gameState = gameState;
				exitValidationManager.gameManager = this;  // <-- CRITICAL: This line ensures proper cleanup
														   // exitValidationManager.menuNavigation will be found automatically
			}
		}

		/// <summary> 
		/// Connect events between all systems (happens during game mode initialization)
		/// </summary>
		void ConnectEvents () {
			TakiLogger.LogInfo ("GameManager: ConnectEvents called!", TakiLogger.LogCategory.System);

			// Deck Manager events
			if (deckManager != null) {
				deckManager.OnInitialGameSetup += OnInitialGameSetupComplete;
				deckManager.OnCardDrawn += OnCardDrawnFromDeck;
			}

			// Game State events
			if (gameState != null) {
				gameState.OnTurnStateChanged += OnTurnStateChanged;
				gameState.OnInteractionStateChanged += OnInteractionStateChanged;
				gameState.OnGameStatusChanged += OnGameStatusChanged;
				gameState.OnActiveColorChanged += OnActiveColorChanged;
				gameState.OnGameWon += OnGameWon;

				// TAKI sequence events
				gameState.OnTakiSequenceStarted += OnTakiSequenceStarted;
				gameState.OnTakiSequenceCardAdded += OnTakiSequenceCardAdded;
				gameState.OnTakiSequenceEnded += OnTakiSequenceEnded;
			}

			// Turn Manager events
			if (turnManager != null) {
				turnManager.OnTurnChanged += OnTurnChanged;
				turnManager.OnComputerTurnReady += OnComputerTurnReady;
				turnManager.OnTurnTimeOut += OnPlayerTurnTimeOut;
			}

			if (!isMultiplayerMode && computerAI != null) {
				// Computer AI events 
				computerAI.OnAICardSelected += OnAICardSelected;
				computerAI.OnAIDrawCard += OnAIDrawCard;
				computerAI.OnAIColorSelected += OnAIColorSelected;
				computerAI.OnAIDecisionMade += OnAIDecisionMade;
				computerAI.OnAISequenceComplete += OnAISequenceComplete;
			}

			// Connect to active UI manager events (new architecture)
			ConnectActiveUIManagerEvents ();

			// Pause Manager events
			if (pauseManager != null) {
				pauseManager.OnGamePaused += OnGamePaused;
				pauseManager.OnGameResumed += OnGameResumed;
			}

			// Game End Manager events
			if (gameEndManager != null) {
				gameEndManager.OnGameEnded += OnGameEndProcessed;
				gameEndManager.OnGameRestarted += OnGameRestarted;
				gameEndManager.OnReturnedToMenu += OnReturnedToMenu;
			}

			// Exit Validation Manager events
			if (exitValidationManager != null) {
				exitValidationManager.OnExitValidationShown += OnExitValidationShown;
				exitValidationManager.OnExitValidationCancelled += OnExitValidationCancelled;
				exitValidationManager.OnExitConfirmed += OnExitConfirmed;
			}
		}

		/// <summary>
		/// Connect to the active UI manager events based on current game mode
		/// </summary>
		private void ConnectActiveUIManagerEvents () {
			TakiLogger.LogSystem ("=== CONNECTING ACTIVE UI MANAGER EVENTS ===", TakiLogger.LogLevel.Debug);
			TakiLogger.LogSystem ($"singlePlayerUI: {(singlePlayerUI != null ? "ASSIGNED" : "NULL")}", TakiLogger.LogLevel.Debug);
			TakiLogger.LogSystem ($"multiPlayerUI: {(multiPlayerUI != null ? "ASSIGNED" : "NULL")}", TakiLogger.LogLevel.Debug);

			if (isMultiplayerMode && multiPlayerUI != null) {
				multiPlayerUI.OnPlayCardClicked += OnPlayCardButtonClicked;
				multiPlayerUI.OnDrawCardClicked += OnDrawCardButtonClicked;
				multiPlayerUI.OnEndTurnClicked += OnEndTurnButtonClicked;
				multiPlayerUI.OnColorSelected += OnColorSelectedByPlayer;
				multiPlayerUI.OnEndTakiSequenceClicked += OnEndTakiSequenceButtonClicked;
			} else if (!isMultiplayerMode && singlePlayerUI != null) {
				singlePlayerUI.OnPlayCardClicked += OnPlayCardButtonClicked;
				singlePlayerUI.OnDrawCardClicked += OnDrawCardButtonClicked;
				singlePlayerUI.OnEndTurnClicked += OnEndTurnButtonClicked;
				singlePlayerUI.OnColorSelected += OnColorSelectedByPlayer;
				singlePlayerUI.OnEndTakiSequenceClicked += OnEndTakiSequenceButtonClicked;
			}

			TakiLogger.LogSystem ("=== UI MANAGER EVENTS CONNECTION COMPLETE ===", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Helper method to disconnect events from a specific UI manager
		/// </summary>
		private void DisconnectUIManagerEvents (BaseGameplayUIManager uiManager, string managerName) {
			if (uiManager == null) return;

			uiManager.OnPlayCardClicked -= OnPlayCardButtonClicked;
			uiManager.OnDrawCardClicked -= OnDrawCardButtonClicked;
			uiManager.OnEndTurnClicked -= OnEndTurnButtonClicked;
			uiManager.OnColorSelected -= OnColorSelectedByPlayer;
			uiManager.OnEndTakiSequenceClicked -= OnEndTakiSequenceButtonClicked;

			TakiLogger.LogSystem ($"Event handlers disconnected from {managerName}");
		}

		/// <summary>
		/// Initialize game systems for single player - called by MenuNavigation
		/// THIS is where the real initialization happens
		/// </summary>
		public void InitializeSinglePlayerSystems () {
			if (!areComponentsValidated) {
				TakiLogger.LogError ("Cannot initialize: Components not validated!", TakiLogger.LogCategory.System);
				return;
			}

			TakiLogger.LogSystem ("Initializing singleplayer game systems...", TakiLogger.LogLevel.Debug);

			// Set multiplayer mode
			isMultiplayerMode = false;
			TakiLogger.LogNetwork ("Multiplayer mode disabled", TakiLogger.LogLevel.Debug);

			// Enable AI (AI replaces remote human)
			if (computerAI != null) {
				computerAI.enabled = true;
				TakiLogger.LogNetwork ("Computer AI enabled for singleplayer", TakiLogger.LogLevel.Debug);
			}

			// Initialize systems for singleplayer if not already done
			if (!areSystemsInitialized) {
				// Connect events between systems
				ConnectEvents ();

				// Initialize visual card system
				InitializeVisualCardSystem ();

				// Initialize UI for gameplay
				GetActiveUI ()?.ResetUIForNewGame ();

				areSystemsInitialized = true;
				TakiLogger.LogSystem ("Single player systems initialized - Ready to start game", TakiLogger.LogLevel.Debug);
			}
			TakiLogger.LogNetwork ("Singleplayer systems initialized successfully", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Initialize game systems for multi player - called by MenuNavigation
		/// THIS is where the real initialization happens
		/// Enhanced multiplayer system initialization with deck coordination
		/// </summary>
		public void InitializeMultiPlayerSystems () {
			if (!areComponentsValidated) {
				TakiLogger.LogError ("Cannot initialize: Components not validated!", TakiLogger.LogCategory.System);
				return;
			}

			TakiLogger.LogSystem ("Initializing multiplayer game systems...", TakiLogger.LogLevel.Debug);

			if (networkGameManager == null) {
				TakiLogger.LogError ("NetworkGameManager not assigned!", TakiLogger.LogCategory.System);
				return;
			}

			// Set multiplayer mode
			isMultiplayerMode = true;
			TakiLogger.LogNetwork ("Multiplayer mode enabled", TakiLogger.LogLevel.Debug);

			// Disable AI (remote human replaces AI)
			if (computerAI != null) {
				computerAI.enabled = false;
				TakiLogger.LogNetwork ("Computer AI disabled for multiplayer", TakiLogger.LogLevel.Debug);
			}

			// Initialize systems for multiplayer if not already done
			if (!areSystemsInitialized) {
				// Connect events between systems
				ConnectEvents ();

				// Initialize visual card system
				InitializeVisualCardSystem ();

				// Initialize UI for gameplay
				GetActiveUI ()?.ResetUIForNewGame ();

				areSystemsInitialized = true;
				TakiLogger.LogSystem ("Multi player systems initialized - Ready to start game", TakiLogger.LogLevel.Debug);
			}

			// Configure deck manager for network mode
			if (deckManager != null) {
				deckManager.SetNetworkMode (true);
				TakiLogger.LogNetwork ("DeckManager configured for network mode", TakiLogger.LogLevel.Debug);
			}

			// Configure hand managers for network privacy
			InitializeNetworkHandManagers ();

			// Start network game coordination
			networkGameManager.StartNetworkGame ();

			TakiLogger.LogNetwork ("Multiplayer systems initialized successfully", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Initialize visual card system
		/// </summary>
		void InitializeVisualCardSystem () {
			TakiLogger.LogSystem ("Initializing visual card system...", TakiLogger.LogLevel.Debug);

			HandManager activePlayerHandManager = GetActivePlayerHandManager ();
			HandManager activeOpponentHandManager = GetActiveOpponentHandManager ();

			if (activePlayerHandManager == null || activeOpponentHandManager == null) {
				TakiLogger.LogError ($"HandManager references missing! Player: {activePlayerHandManager != null}, Opponent: {activeOpponentHandManager != null}", TakiLogger.LogCategory.System);
				return;
			}

			// Connect events using active managers
			activePlayerHandManager.OnCardSelected += OnPlayerCardSelected;
			activeOpponentHandManager.OnCardSelected += OnComputerCardSelected;

			TakiLogger.LogSystem ("Visual card system initialized", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Initialize hand managers for network privacy system
		/// </summary>
		void InitializeNetworkHandManagers () {
			TakiLogger.LogNetwork ("Initializing hand managers for network privacy", TakiLogger.LogLevel.Debug);

			HandManager activePlayerHandManager = GetActivePlayerHandManager ();
			HandManager activeOpponentHandManager = GetActiveOpponentHandManager ();

			if (activePlayerHandManager == null || activeOpponentHandManager == null) {
				TakiLogger.LogError ("activePlayerHandManager or activeOpponentHandManager is NULL", TakiLogger.LogCategory.Network);
				return;
			}

			// Configure player hand manager (local player - face up)
			activePlayerHandManager.SetNetworkMode (true);
			activePlayerHandManager.InitializeNetworkHands (true); // Local player hand
			TakiLogger.LogNetwork ("Player hand manager configured for local player", TakiLogger.LogLevel.Debug);

			// Configure opponent hand manager (opponent - face down with count)
			activeOpponentHandManager.SetNetworkMode (true);
			activeOpponentHandManager.InitializeNetworkHands (false); // Opponent hand
			TakiLogger.LogNetwork ("Opponent hand manager configured for opponent display", TakiLogger.LogLevel.Debug);

			TakiLogger.LogNetwork ($"Network hand managers initialized with per-screen architecture - Mode: {(isMultiplayerMode ? "Multiplayer" : "SinglePlayer")}", TakiLogger.LogLevel.Debug);
		}

		#endregion

		#region Game Flow Control
		// Main game startup and reset functionality

		/// <summary>
		/// PUBLIC METHOD: Start a new single player game - Called by MenuNavigation
		/// </summary>
		public void StartNewSinglePlayerGame () {
			// Initialize systems if not already done
			if (!areSystemsInitialized) {
				InitializeSinglePlayerSystems ();
			}

			TakiLogger.LogSystem ("Starting new single player game...");

			if (deckManager == null) {
				TakiLogger.LogError ("Cannot start game: DeckManager not assigned!", TakiLogger.LogCategory.System);
				return;
			}

			// Reset all systems for new game
			ResetGameSystems ();

			// Setup initial game state through deck manager
			deckManager.SetupInitialGame ();
		}

		/// <summary>
		/// PUBLIC METHOD: Start a new multiplayer game - Called by MenuNavigation
		/// ENHANCED: Unified structure with single-player (initialize → reset → start)
		/// </summary>
		public void StartNewMultiPlayerGame () {
			// Initialize systems if not already done
			if (!areSystemsInitialized) {
				InitializeMultiPlayerSystems ();
			}

			TakiLogger.LogSystem ("Starting new multiplayer game...");

			if (networkGameManager == null) {
				TakiLogger.LogError ("Cannot start multiplayer game: NetworkGameManager not assigned!", TakiLogger.LogCategory.System);
				return;
			}

			// Reset all systems for new game (mode-aware - won't clear hands in multiplayer)
			ResetGameSystems ();

			// Start network game coordination (hands will be populated via network)
			networkGameManager.StartNetworkGame ();
		}

		/// <summary>
		/// ENHANCED: Mode-aware reset for both single-player and multiplayer
		/// FIXED: Only clears hands in single-player mode (multiplayer hands come from network)
		/// </summary>
		void ResetGameSystems () {
			// Only clear hands in single-player mode
			// In multiplayer, hands are populated from network data and should NOT be cleared
			if (!isMultiplayerMode) {
				playerHand.Clear ();

				if (computerAI != null) {
					computerAI.ClearHand ();
				}
			}

			// Always reset core game systems (both modes need this)
			if (gameState != null) {
				gameState.ResetGameState ();
			}

			// Reset turn manager
			if (turnManager != null) {
				turnManager.ResetTurns ();
			}

			// Reset turn flow control
			ResetTurnFlowState (); // This now includes special card state reset
			isGameActive = false;

			// Reset game initialization flag for fresh setup
			if (deckManager?.gameSetup != null) {
				deckManager.gameSetup.ResetInitializationState ();
			}
		}

		/// <summary>
		/// Handle completion of initial game setup
		/// </summary>
		/// <param name="player1Hand">Player's initial hand (Human)</param>
		/// <param name="player2Hand">Computer's initial hand</param>
		/// <param name="startingCard">Starting discard card</param>
		void OnInitialGameSetupComplete (List<CardData> player1Hand, List<CardData> player2Hand, CardData startingCard) {
			// Assign hands (Player1 = Human, Player2 = Computer)
			playerHand = player1Hand;
			if (computerAI != null) {
				computerAI.AddCardsToHand (player2Hand);
			}

			// Set active color from starting card
			if (startingCard != null && gameState != null) {
				gameState.ChangeActiveColor (startingCard.color);
			}

			// Update UI (this will now include visual cards)
			UpdateAllUI ();

			// Start the first turn
			if (turnManager != null) {
				turnManager.InitializeTurns (startingPlayer);
			}

			isGameActive = true;
			OnGameStarted?.Invoke ();

			TakiLogger.LogSystem ($"Game started! Player: {player1Hand.Count} cards, Computer: {player2Hand.Count} cards");
		}

		#endregion

		#region Turn Flow Management
		// Strict turn flow control system

		/// <summary>
		/// Reset turn flow control state, includes special card state
		/// </summary>
		void ResetTurnFlowState () {
			hasPlayerTakenAction = false;
			canPlayerDraw = true;
			canPlayerPlay = true;
			canPlayerEndTurn = false;

			// Reset special card state
			ResetSpecialCardState ();

			TakiLogger.LogTurnFlow ("TURN FLOW STATE RESET (includes special card state)", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Start player turn with strict flow control
		/// </summary>
		void StartPlayerTurnFlow () {
			TakiLogger.LogTurnFlow ("Starting Player Turn", TakiLogger.LogLevel.Debug);

			// Check for active PlusTwo chain FIRST
			if (gameState.IsPlusTwoChainActive) {
				TakiLogger.LogTurnFlow ("=== PLSTWO CHAIN ACTIVE - SPECIAL TURN LOGIC ===", TakiLogger.LogLevel.Debug);

				int drawCount = gameState.ChainDrawCount;
				int chainLength = gameState.NumberOfChainedCards;

				TakiLogger.LogTurnFlow ($"Chain status: {chainLength} cards, player must draw {drawCount} or play PlusTwo", TakiLogger.LogLevel.Debug);

				// Check if player has PlusTwo cards
				bool hasPlusTwo = playerHand.Any (card => card.cardType == CardType.PlusTwo);

				if (hasPlusTwo) {
					// Player can either play PlusTwo to continue chain or draw to break it
					hasPlayerTakenAction = false;
					canPlayerDraw = true;     // Can draw to break chain
					canPlayerPlay = true;     // Can play PlusTwo to continue chain
					canPlayerEndTurn = false; // Must take action first

					GetActiveUI ()?.UpdateStrictButtonStates (true, true, false);
					GetActiveUI ()?.ShowPlayerMessage ($"Chain active! Play PlusTwo to continue or draw {drawCount} cards to break");

					TakiLogger.LogTurnFlow ($"Player has PlusTwo options: continue chain or draw {drawCount} cards", TakiLogger.LogLevel.Debug);
				} else {
					// Player has no PlusTwo cards - must draw to break chain
					hasPlayerTakenAction = false;
					canPlayerDraw = true;     // Must draw to break chain
					canPlayerPlay = false;    // No valid plays during chain without PlusTwo
					canPlayerEndTurn = false; // Must take action first

					GetActiveUI ()?.UpdateStrictButtonStates (false, true, false);
					GetActiveUI ()?.ShowPlayerMessage ($"No PlusTwo cards - you must draw {drawCount} cards to break chain");

					TakiLogger.LogTurnFlow ($"Player must break chain by drawing {drawCount} cards (no PlusTwo available)", TakiLogger.LogLevel.Debug);
				}

				// Update visual card states - only PlusTwo cards should show as playable
				RefreshPlayerHandStates ();
				return;
			}

			// ... KEEP ALL EXISTING NORMAL TURN FLOW LOGIC AFTER THIS POINT ...
			TakiLogger.LogTurnFlow ("Normal turn flow - no active chain", TakiLogger.LogLevel.Debug);

			// Reset turn flow state
			hasPlayerTakenAction = false;
			canPlayerDraw = true;
			canPlayerPlay = true;
			canPlayerEndTurn = false;

			// Check if player has valid cards (existing logic)
			int validCardCount = CountPlayableCards ();

			if (validCardCount == 0) {
				// Player has NO valid cards - must draw
				TakiLogger.LogTurnFlow ("Player has no valid cards, must draw a card", TakiLogger.LogLevel.Debug);
				GetActiveUI ()?.ShowPlayerMessage ("No valid cards - you must DRAW a card!");
				GetActiveUI ()?.UpdateStrictButtonStates (false, true, false); // No Play, Yes Draw, No EndTurn
			} else {
				// Player has valid cards - can play or draw
				TakiLogger.LogTurnFlow ($"Player has {validCardCount} valid cards, may PLAY or DRAW a card", TakiLogger.LogLevel.Debug);
				GetActiveUI ()?.ShowPlayerMessage ($"You have {validCardCount} valid moves - PLAY a card or DRAW");
				GetActiveUI ()?.UpdateStrictButtonStates (true, true, false); // Yes Play, Yes Draw, No EndTurn
			}

			// Update visual card states
			RefreshPlayerHandStates ();
		}

		/// <summary> 
		/// Handle post card play turn flow with proper TAKI sequence handling (sequence continuation and End Sequence button management)
		/// </summary>
		/// <param name="card">Card that was just played</param>
		void HandlePostCardPlayTurnFlow (CardData card) {
			TakiLogger.LogTurnFlow ($"=== HANDLING POST-CARD-PLAY TURN FLOW for {card.GetDisplayText ()} ===", TakiLogger.LogLevel.Debug);

			// Check if TAKI sequence was started and we should continue turn
			if (gameState.IsInTakiSequence && (card.cardType == CardType.Taki || card.cardType == CardType.SuperTaki) && !isCurrentCardLastInSequence) {
				TakiLogger.LogTurnFlow ($"{card.cardType} card started sequence - turn continues for sequence play", TakiLogger.LogLevel.Debug);

				// For TAKI/SuperTAKI cards that start sequences, keep turn active
				hasPlayerTakenAction = false;  // Reset to allow sequence cards
				canPlayerPlay = true;          // Re-enable play for sequence
				canPlayerDraw = false;         // Cannot draw during sequence
				canPlayerEndTurn = false;      // Cannot end turn during sequence (must use End Sequence button)

				GetActiveUI ()?.UpdateStrictButtonStates (true, false, false);
				GetActiveUI ()?.EnableEndTakiSequenceButton (true);

				if (card.cardType == CardType.Taki) {
					GetActiveUI ()?.ShowPlayerMessage ($"TAKI Sequence active - play {card.color} cards or End Sequence!");
				} else {
					GetActiveUI ()?.ShowPlayerMessage ($"SuperTAKI Sequence active - play {gameState.TakiSequenceColor} cards or End Sequence!");
				}

				TakiLogger.LogTurnFlow ($"{card.cardType} sequence turn flow: PLAY enabled, DRAW/END TURN disabled, END SEQUENCE enabled", TakiLogger.LogLevel.Debug);
				return;
			}

			// Check if card was played during existing sequence (not last card)
			if (gameState.IsInTakiSequence && !isCurrentCardLastInSequence) {
				TakiLogger.LogTurnFlow ("Card played in existing sequence - turn continues");

				// Continue sequence - same button states as sequence start 
				hasPlayerTakenAction = false;  // Allow more sequence cards
				canPlayerPlay = true;          // Keep play enabled
				canPlayerDraw = false;         // Still no draw during sequence
				canPlayerEndTurn = false;      // Still no end turn during sequence

				GetActiveUI ()?.UpdateStrictButtonStates (true, false, false);
				GetActiveUI ()?.ShowPlayerMessage ("Sequence continues - play more cards or End Sequence!");

				TakiLogger.LogTurnFlow ("Sequence continuation: PLAY enabled, DRAW/END TURN disabled");
				return;
			}

			// Check if sequence just ended (last card processed)
			if (isCurrentCardLastInSequence) {
				TakiLogger.LogTurnFlow ("Last card in sequence processed - ending turn normally");

				// Reset flag
				isCurrentCardLastInSequence = false;

				// Force end turn after sequence completion
				hasPlayerTakenAction = true;
				canPlayerPlay = false;
				canPlayerDraw = false;
				canPlayerEndTurn = true;

				GetActiveUI ()?.ForceEnableEndTurn ();
				GetActiveUI ()?.ShowPlayerMessage ("Sequence completed - you must END TURN!");

				TakiLogger.LogTurnFlow ("Sequence completion: Must END TURN");
				return;
			}

			// Plus card handling with better messaging
			if (card.cardType == CardType.Plus && !gameState.IsInTakiSequence) {
				TakiLogger.LogTurnFlow ("PLUS CARD PLAYED - Player gets additional action", TakiLogger.LogLevel.Debug);

				// Set special card state
				isWaitingForAdditionalAction = true;
				activeSpecialCardEffect = CardType.Plus;

				// Reset action state to allow additional action
				hasPlayerTakenAction = false;  // Reset to allow additional action
				canPlayerPlay = true;          // Re-enable play
				canPlayerDraw = true;          // Re-enable draw  
				canPlayerEndTurn = false;      // Keep end turn disabled until additional action

				// Update UI with better messaging
				GetActiveUI ()?.UpdateStrictButtonStates (true, true, false);
				GetActiveUI ()?.ShowSpecialCardEffect (CardType.Plus, PlayerType.Human, "You get one more action");

				TakiLogger.LogTurnFlow ("Plus card turn flow: Additional action enabled, END TURN disabled", TakiLogger.LogLevel.Debug);
				return;
			}

			// ChangeColor card handling for Player
			if (card.cardType == CardType.ChangeColor && !gameState.IsInTakiSequence) {
				TakiLogger.LogTurnFlow ("CHANGE COLOR CARD PLAYED BY PLAYER - Player must select color", TakiLogger.LogLevel.Debug);

				if (gameState == null) {
					TakiLogger.LogError ("Cannot handle ChangeColor: Missing gameState component!", TakiLogger.LogCategory.Rules);
					return;
				}

				// Set interaction state to color selection
				gameState.ChangeInteractionState (InteractionState.ColorSelection);
				TakiLogger.LogGameState ("Interaction state changed to ColorSelection", TakiLogger.LogLevel.Debug);

				// Show color selection panel (this will disable PLAY/DRAW buttons)
				GetActiveUI ()?.ShowColorSelection (true);
				TakiLogger.LogUI ("Color selection panel displayed", TakiLogger.LogLevel.Debug);

				// Mark action as taken but don't allow new actions
				hasPlayerTakenAction = true;
				canPlayerPlay = false;         // Disable play during color selection
				canPlayerDraw = false;         // Disable draw during color selection
				canPlayerEndTurn = true;       // Allow end turn after color selection

				// Note: END TURN button will be enabled, but the color selection must complete first
				GetActiveUI ()?.UpdateStrictButtonStates (false, false, true);

				// ENHANCED: Better feedback about the selection process 
				GetActiveUI ()?.ShowPlayerMessage ("CHANGE COLOR: Choose colors freely, then click END TURN!");
				GetActiveUI ()?.ShowOpponentMessage ("Waiting for player to finish selecting new color...");

				TakiLogger.LogTurnFlow ("ChangeColor turn flow: Color selection required, END TURN enabled after selection", TakiLogger.LogLevel.Debug);
				return;
			}

			// Check if this was the additional action after a Plus card
			if (isWaitingForAdditionalAction && activeSpecialCardEffect == CardType.Plus) {
				TakiLogger.LogTurnFlow ("COMPLETING ADDITIONAL ACTION after Plus card", TakiLogger.LogLevel.Debug);

				// Clear special card state
				isWaitingForAdditionalAction = false;
				activeSpecialCardEffect = CardType.Number;

				// Now follow normal turn completion flow
				hasPlayerTakenAction = true;
				canPlayerPlay = false;
				canPlayerDraw = false;
				canPlayerEndTurn = true;

				GetActiveUI ()?.ForceEnableEndTurn ();
				GetActiveUI ()?.ShowPlayerMessage ("Additional action complete - you must END TURN!");

				TakiLogger.LogTurnFlow ("Additional action completed - normal END TURN flow", TakiLogger.LogLevel.Debug);
				return;
			}

			// Normal card flow (non-Plus, non-ChangeColor cards or cards not during Plus sequence)
			TakiLogger.LogTurnFlow ("NORMAL CARD TURN FLOW - Single action, must end turn", TakiLogger.LogLevel.Debug);

			hasPlayerTakenAction = true;
			canPlayerPlay = false;
			canPlayerDraw = false; // Cannot draw after playing
			canPlayerEndTurn = true;

			// UI already disabled buttons immediately on click, now enable END TURN
			GetActiveUI ()?.ForceEnableEndTurn ();
			GetActiveUI ()?.ShowPlayerMessage ("Card played - you must END TURN!");

			TakiLogger.LogTurnFlow ("Normal turn flow: Must END TURN after single action", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Handle turn flow after drawing a card
		/// </summary>
		/// <param name="drawnCard">Card that was drawn</param>
		void HandlePostCardDrawTurnFlow (CardData drawnCard) {
			TakiLogger.LogTurnFlow ($"=== HANDLING POST-DRAW TURN FLOW ===", TakiLogger.LogLevel.Debug);

			// Check if this was the additional action after a Plus card
			if (isWaitingForAdditionalAction && activeSpecialCardEffect == CardType.Plus) {
				TakiLogger.LogTurnFlow ("COMPLETING ADDITIONAL DRAW ACTION after Plus card", TakiLogger.LogLevel.Debug);

				// Clear special card state
				isWaitingForAdditionalAction = false;
				activeSpecialCardEffect = CardType.Number;

				// Now follow normal turn completion flow
				hasPlayerTakenAction = true;
				canPlayerPlay = false;
				canPlayerDraw = false;
				canPlayerEndTurn = true;

				GetActiveUI ()?.ForceEnableEndTurn ();
				GetActiveUI ()?.ShowPlayerMessage ($"Additional draw complete: {drawnCard.GetDisplayText ()} - you must END TURN!");

				TakiLogger.LogTurnFlow ("Additional draw action completed - normal END TURN flow", TakiLogger.LogLevel.Debug);
				return;
			}

			// Normal draw flow (first action or not during Plus sequence)
			TakiLogger.LogTurnFlow ("NORMAL DRAW TURN FLOW - Single action, must end turn", TakiLogger.LogLevel.Debug);

			hasPlayerTakenAction = true;
			canPlayerPlay = false;
			canPlayerDraw = false;
			canPlayerEndTurn = true;

			// Enable END TURN button
			GetActiveUI ()?.ForceEnableEndTurn ();
			GetActiveUI ()?.ShowPlayerMessage ($"Drew: {drawnCard.GetDisplayText ()} - you must END TURN!");

			TakiLogger.LogTurnFlow ("Normal draw flow: Must END TURN after single action", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Enhanced turn completion to handle Stop card effects at correct timing
		/// </summary>
		void EndPlayerTurnWithStrictFlow () {
			TakiLogger.LogTurnFlow ("ENDING PLAYER TURN - STRICT FLOW WITH SPECIAL CARDS", TakiLogger.LogLevel.Debug);

			// Check STOP flag FIRST, before any turn switching logic
			if (shouldSkipNextTurn) {
				TakiLogger.LogTurnFlow ("=== STOP FLAG DETECTED - PROCESSING STOP EFFECT ===", TakiLogger.LogLevel.Debug);
				ProcessStopSkipEffect ();
				return; // Exit early - don't proceed with normal turn switch
			}

			// Check for pending effects (PLUS cards only)
			if (HasPendingSpecialCardEffects ()) {
				TakiLogger.LogError ("Attempting to end turn with pending special card effects!", TakiLogger.LogCategory.TurnFlow);
				GetActiveUI ()?.ShowPlayerMessage ("Cannot end turn - special card effect pending!");
				return;
			}

			// FIXED: Hide color selection panel if it's still visible
			if (gameState != null && gameState.interactionState == InteractionState.ColorSelection) {
				TakiLogger.LogGameState ("Ending turn during color selection - hiding panel and returning to normal", TakiLogger.LogLevel.Debug);

				// Hide the color selection panel
				GetActiveUI ()?.ShowColorSelection (false);

				// Return to normal interaction state
				gameState.ChangeInteractionState (InteractionState.Normal);

				// Show completion message
				GetActiveUI ()?.ShowPlayerMessage ($"Color selection complete: {gameState.activeColor}");
			}

			// Clear any selected cards
			GetActivePlayerHandManager ()?.ClearSelection ();

			// Reset turn flow state for next turn (includes special card state)
			ResetTurnFlowState ();

			// Normal turn end - proceed with turn switch
			TakiLogger.LogTurnFlow ("Normal turn end - switching to computer turn");
			if (turnManager != null) {
				turnManager.EndTurn ();
			}
		}

		/// <summary>
		/// Enhanced AI turn completion to handle Stop card effects at correct timing
		/// Mirrors the logic in EndPlayerTurnWithStrictFlow() for symmetrical behavior
		/// </summary>
		void EndAITurnWithStrictFlow () {
			TakiLogger.LogTurnFlow ("ENDING AI TURN - STRICT FLOW WITH SPECIAL CARDS", TakiLogger.LogLevel.Debug);

			// Check STOP flag FIRST, before any turn switching logic
			if (shouldSkipNextTurn) {
				TakiLogger.LogTurnFlow ("=== STOP FLAG DETECTED FOR AI TURN END - PROCESSING STOP EFFECT ===", TakiLogger.LogLevel.Debug);
				ProcessStopSkipEffect ();
				return; // Exit early - don't proceed with normal turn switch
			}

			// Normal turn end - proceed with turn switch
			TakiLogger.LogTurnFlow ("Normal AI turn end - switching to human turn", TakiLogger.LogLevel.Debug);
			if (turnManager != null) {
				turnManager.EndTurn ();
			}
		}

		/// <summary>
		/// Start player turn after STOP effect processing
		/// </summary>
		void StartPlayerTurnAfterStop () {
			TakiLogger.LogTurnFlow ("=== STARTING PLAYER TURN AFTER STOP EFFECT ===", TakiLogger.LogLevel.Debug);

			// Ensure game state shows player turn
			if (gameState == null) {
				TakiLogger.LogError ("GameManager: StartPlayerTurnAfterStop - gameState is NULL!", TakiLogger.LogCategory.TurnFlow);
				return;
			}

			gameState.ChangeTurnState (TurnState.PlayerTurn);

			// Start the player turn flow
			StartPlayerTurnFlow ();

			// Show additional feedback
			GetActiveUI ()?.ShowPlayerMessage ("Your turn continues thanks to STOP card!");
			GetActiveUI ()?.ShowOpponentMessage ("Waiting (turn skipped)...");

			TakiLogger.LogTurnFlow ("Player turn restarted successfully after STOP effect", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Start AI turn after STOP effect processing
		/// Mirrors StartPlayerTurnAfterStop() for when computer benefits from STOP
		/// </summary>
		void StartAITurnAfterStop () {
			TakiLogger.LogTurnFlow ("=== STARTING AI TURN AFTER STOP EFFECT ===", TakiLogger.LogLevel.Debug);

			// Ensure game state shows computer turn
			if (gameState == null) {
				TakiLogger.LogError ("GameManager: StartAITurnAfterStop - gameState is NULL!", TakiLogger.LogCategory.TurnFlow);
				return;
			}

			gameState.ChangeTurnState (TurnState.ComputerTurn);

			// Show feedback messages
			GetActiveUI ()?.ShowPlayerMessage ("Your turn got skipped");
			GetActiveUI ()?.ShowOpponentMessage ("Computer gets another turn thanks to STOP card");

			// Trigger AI decision for the new turn
			CardData topCard = GetTopDiscardCard ();
			if (topCard != null && computerAI != null) {
				TakiLogger.LogTurnFlow ("Triggering AI decision for STOP benefit turn", TakiLogger.LogLevel.Debug);
				// Give AI time to "think" about the new turn
				Invoke (nameof (TriggerAITurnAfterStop), 1.5f);
			} else {
				TakiLogger.LogError ("Cannot start AI turn after STOP - missing components", TakiLogger.LogCategory.AI);
			}

			TakiLogger.LogTurnFlow ("AI turn setup complete after STOP effect", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Helper method to trigger AI decision after STOP benefit
		/// </summary>
		void TriggerAITurnAfterStop () {
			TakiLogger.LogAI ("=== AI MAKING DECISION FOR STOP BENEFIT TURN ===", TakiLogger.LogLevel.Debug);

			CardData topCard = GetTopDiscardCard ();
			if (topCard != null && computerAI != null) {
				GetActiveUI ()?.ShowOpponentMessage ("Opponent thinking about bonus turn...");
				computerAI.MakeDecision (topCard);
			} else {
				TakiLogger.LogError ("Cannot trigger AI turn after STOP - missing components", TakiLogger.LogCategory.AI);
			}
		}

		#endregion

		#region Player Actions
		// User input handlers and card play mechanics

		/// <summary>
		/// Handle play card button clicked (No Auto-Play)
		/// Play Card button only works with explicit selection
		/// Handle play card with strict flow control
		/// Play card button handler with multiplayer support
		/// </summary>
		void OnPlayCardButtonClicked () {
			TakiLogger.LogTurnFlow ("=== PLAY CARD BUTTON CLICKED ===", TakiLogger.LogLevel.Debug);

			// Route to multiplayer handler if in multiplayer mode
			if (isMultiplayerMode) {
				OnPlayCardButtonClickedMultiplayer ();
				return;
			}

			if (!isGameActive || !gameState.CanPlayerAct ()) {
				TakiLogger.LogWarning ("Cannot play card: Game not active or not player turn", TakiLogger.LogCategory.TurnFlow);
				GetActiveUI ()?.ShowPlayerMessage ("Not your turn!");
				return;
			}

			if (!canPlayerPlay) {
				TakiLogger.LogWarning ("Cannot play card: Player already took action", TakiLogger.LogCategory.TurnFlow);
				GetActiveUI ()?.ShowPlayerMessage ("You already took an action - END TURN!");
				return;
			}

			// Get selected card using active hand manager
			CardData cardToPlay = GetActivePlayerHandManager ()?.GetSelectedCard ();

			if (cardToPlay != null) {
				TakiLogger.LogCardPlay ($"Attempting to play selected card: {cardToPlay.GetDisplayText ()}");

				// ENHANCEMENT: Handle ChangeColor UI logic for HUMAN players before playing card
				if (cardToPlay.cardType == CardType.ChangeColor) {
					TakiLogger.LogUI ("Human playing ChangeColor card - will show color selection after card play", TakiLogger.LogLevel.Debug);
				}

				PlayCardWithStrictFlow (cardToPlay);
			} else {
				int playableCount = CountPlayableCards ();
				if (playableCount > 0) {
					GetActiveUI ()?.ShowPlayerMessage ($"Please select a card! You have {playableCount} valid moves.");
				} else {
					GetActiveUI ()?.ShowPlayerMessage ("No valid moves - try drawing a card!");
				}
				TakiLogger.LogUI ("No card selected - player must choose explicitly", TakiLogger.LogLevel.Debug);
			}
		}

		/// <summary>
		/// Handle draw card button clicked
		/// ENHANCED: Handle draw card with strict flow control0
		/// ENHANCED: Draw card button handler with multiplayer support
		/// </summary>
		void OnDrawCardButtonClicked () {
			TakiLogger.LogTurnFlow ("=== DRAW CARD BUTTON CLICKED ===");

			// PHASE 2: Route to multiplayer handler if in multiplayer mode
			if (isMultiplayerMode) {
				OnDrawCardButtonClickedMultiplayer ();
				return;
			}

			if (!isGameActive || !gameState.CanPlayerAct ()) {
				TakiLogger.LogWarning ("Cannot draw card: Game not active or not player turn", TakiLogger.LogCategory.TurnFlow);
				GetActiveUI ()?.ShowPlayerMessage ("Not your turn!");
				return;
			}

			// CRITICAL: Check for active PlusTwo chain FIRST
			if (gameState.IsPlusTwoChainActive) {
				TakiLogger.LogTurnFlow ("=== PLAYER BREAKING PLSTWO CHAIN BY DRAWING ===", TakiLogger.LogLevel.Debug);
				BreakPlusTwoChainByDrawing ();
				return; // Skip normal draw logic - chain breaking is the action
			}

			if (!canPlayerDraw) {
				TakiLogger.LogWarning ("Cannot draw card: Player already took action", TakiLogger.LogCategory.TurnFlow);
				GetActiveUI ()?.ShowPlayerMessage ("You already took an action - END TURN!");
				return;
			}

			DrawCardWithStrictFlow ();
		}

		/// <summary>
		/// ENHANCED: Handle end turn with special card validation
		/// </summary>
		void OnEndTurnButtonClicked () {
			TakiLogger.LogTurnFlow ("END TURN BUTTON CLICKED - STRICT FLOW WITH SPECIAL CARDS", TakiLogger.LogLevel.Debug);

			// PHASE 7: Check for pending special card effects first
			if (HasPendingSpecialCardEffects ()) {
				TakiLogger.LogWarning ("Cannot end turn: Pending special card effects", TakiLogger.LogCategory.TurnFlow);
				GetActiveUI ()?.ShowPlayerMessage ("You must complete your additional action first!");
				GetActiveUI ()?.ShowPlayerMessage (GetSpecialCardStateDescription ());
				return;
			}

			if (!canPlayerEndTurn) {
				TakiLogger.LogWarning ("Cannot end turn: Player has not taken an action yet", TakiLogger.LogCategory.TurnFlow);
				GetActiveUI ()?.ShowPlayerMessage ("You must take an action first (PLAY or DRAW)!");
				return;
			}

			EndPlayerTurnWithStrictFlow ();
		}

		/// <summary>
		/// CRITICAL FIX: Handle End TAKI Sequence with validation and no restart loops
		/// </summary>
		void OnEndTakiSequenceButtonClicked () {
			TakiLogger.LogTurnFlow ("=== END TAKI SEQUENCE BUTTON CLICKED ===");

			if (!gameState.IsInTakiSequence) {
				GetActiveUI ()?.ShowPlayerMessage ("No TAKI sequence is active!");
				TakiLogger.LogWarning ("End sequence button clicked but no sequence active", TakiLogger.LogCategory.TurnFlow);
				return;
			}

			// CRITICAL VALIDATION: Only human can end human-initiated sequences
			if (gameState.TakiSequenceInitiator != PlayerType.Human) {
				GetActiveUI ()?.ShowPlayerMessage ("Cannot end AI sequence!");
				TakiLogger.LogWarning ("Human tried to end AI sequence - BLOCKED", TakiLogger.LogCategory.TurnFlow);
				return;
			}

			// CRITICAL VALIDATION: Only end sequence on human's turn
			if (!gameState.IsPlayerTurn) {
				GetActiveUI ()?.ShowPlayerMessage ("Cannot end sequence - not your turn!");
				TakiLogger.LogWarning ("Tried to end sequence on wrong turn - BLOCKED", TakiLogger.LogCategory.TurnFlow);
				return;
			}

			// Get sequence info before ending
			int finalCardCount = gameState.NumberOfSequenceCards;
			CardColor sequenceColor = gameState.TakiSequenceColor;
			CardData lastCard = gameState.LastCardPlayedInSequence;

			TakiLogger.LogGameState ($"Human ending TAKI sequence: {finalCardCount} cards of {sequenceColor}", TakiLogger.LogLevel.Debug);

			// End the sequence FIRST (TakiSequence -> Normal)
			gameState.EndTakiSequence ();
			GetActiveUI ()?.EnableEndTakiSequenceButton (false);
			GetActiveUI ()?.HideTakiSequenceStatus ();

			TakiLogger.LogGameState ("Sequence ended - now in Normal state", TakiLogger.LogLevel.Debug);

			// STEP 2: CRITICAL FIX - Process final card effects WITHOUT restarting sequences
			if (lastCard != null && lastCard.IsSpecialCard) {
				TakiLogger.LogRules ($"Processing final card effects: {lastCard.GetDisplayText ()}", TakiLogger.LogLevel.Debug);

				// CRITICAL FIX: Mark as last card to prevent sequence restart
				isCurrentCardLastInSequence = true;

				// CRITICAL FIX: Only process non-TAKI special effects
				if (lastCard.cardType != CardType.Taki && lastCard.cardType != CardType.SuperTaki) {
					TakiLogger.LogRules ($"Processing {lastCard.cardType} effect from sequence end", TakiLogger.LogLevel.Debug);
					HandleSpecialCardEffects (lastCard);
				} else {
					TakiLogger.LogRules ($"Skipping {lastCard.cardType} effect - sequence already ended", TakiLogger.LogLevel.Debug);
				}

				isCurrentCardLastInSequence = false;
			}

			// STEP 3: Handle turn flow based on current state
			if (gameState.interactionState == InteractionState.ColorSelection) {
				// ChangeColor effect activated - allow color selection
				TakiLogger.LogTurnFlow ("Final card was ChangeColor - color selection required", TakiLogger.LogLevel.Debug);
				hasPlayerTakenAction = true;
				canPlayerEndTurn = true;
				GetActiveUI ()?.UpdateStrictButtonStates (false, false, true);

			} else if (gameState.IsPlusTwoChainActive) {
				// PlusTwo effect activated - end turn for chain response
				TakiLogger.LogTurnFlow ("Final card started PlusTwo chain - ending turn", TakiLogger.LogLevel.Debug);
				hasPlayerTakenAction = true;
				canPlayerEndTurn = true;
				GetActiveUI ()?.ForceEnableEndTurn ();

			} else {
				// Normal sequence end
				TakiLogger.LogTurnFlow ("Normal sequence end - forcing END TURN", TakiLogger.LogLevel.Debug);
				hasPlayerTakenAction = true;
				canPlayerEndTurn = true;
				GetActiveUI ()?.ForceEnableEndTurn ();
			}

			// Show completion message
			GetActiveUI ()?.ShowSequenceEndedMessage (finalCardCount, sequenceColor, PlayerType.Human);

			TakiLogger.LogTurnFlow ("Sequence ending complete - no restart loops", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// FIXED: Enhanced color selection handler with proper flow control
		/// </summary>
		/// <param name="selectedColor">Color selected by player</param>
		void OnColorSelectedByPlayer (CardColor selectedColor) {
			TakiLogger.LogGameState ($"=== PLAYER SELECTED COLOR: {selectedColor} ===", TakiLogger.LogLevel.Debug);

			if (gameState == null) {
				TakiLogger.LogError ("Cannot process color selection: GameStateManager is null!", TakiLogger.LogCategory.GameState);
				return;
			}

			// Verify we're in color selection state
			if (gameState.interactionState != InteractionState.ColorSelection) {
				TakiLogger.LogWarning ($"Color selected but not in ColorSelection state! Current: {gameState.interactionState}", TakiLogger.LogCategory.GameState);
			}

			// Update active color immediately (player can change mind multiple times)
			gameState.ChangeActiveColor (selectedColor);
			TakiLogger.LogGameState ($"Active color changed to: {selectedColor}", TakiLogger.LogLevel.Debug);

			// Show feedback about color selection
			GetActiveUI ()?.ShowPlayerMessage ($"Color changed to {selectedColor} - you must END TURN!");
			GetActiveUI ()?.ShowOpponentMessage ("");

			// Update UI to reflect new color
			UpdateAllUI ();

			TakiLogger.LogRules ("CHANGE COLOR effect complete - color selection processed", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// PHASE 8B: FIXED Play card with sequence-first architecture and comprehensive special card handling
		/// CRITICAL FIX: Cards are added to sequence BEFORE special effects are processed
		/// </summary>
		void PlayCardWithStrictFlow (CardData card) {
			TakiLogger.LogCardPlay ($"PLAYING CARD WITH STRICT FLOW (PHASE 8B): {card.GetDisplayText ()}", TakiLogger.LogLevel.Debug);

			// Validate the move
			CardData topCard = GetTopDiscardCard ();
			if (topCard == null) {
				TakiLogger.LogError ("Cannot play card: No top discard card available", TakiLogger.LogCategory.CardPlay);
				GetActiveUI ()?.ShowPlayerMessage ("Game error - no discard pile!");
				return;
			}

			// PHASE 8B: Add sequence validation before standard validation
			if (gameState.IsInTakiSequence) {
				if (!gameState.CanPlayInSequence (card)) {
					TakiLogger.LogWarning ($"Invalid sequence move: Cannot play {card.GetDisplayText ()} in {gameState.TakiSequenceColor} sequence", TakiLogger.LogCategory.Rules);
					GetActiveUI ()?.ShowPlayerMessage ($"Can only play {gameState.TakiSequenceColor} or wild cards in TAKI sequence!");
					return;
				}
				TakiLogger.LogRules ($"SEQUENCE VALIDATION PASSED: {card.GetDisplayText ()} can be played in {gameState.TakiSequenceColor} sequence");
			}

			bool isValidMove = gameState.IsValidMove (card, topCard);
			if (!isValidMove) {
				TakiLogger.LogWarning ($"Invalid move: Cannot play {card.GetDisplayText ()} on {topCard.GetDisplayText ()}", TakiLogger.LogCategory.Rules);
				GetActiveUI ()?.ShowPlayerMessage ($"Invalid move! Cannot play {card.GetDisplayText ()}");
				return;
			}

			// Remove card from player's hand
			bool removed = playerHand.Remove (card);
			if (!removed) {
				TakiLogger.LogError ("Could not remove card from player hand!", TakiLogger.LogCategory.CardPlay);
				return;
			}

			// Clear selection
			GetActivePlayerHandManager ()?.ClearSelection ();

			// Discard the card
			deckManager?.DiscardCard (card);

			// Update active color
			gameState.UpdateActiveColorFromCard (card);

			// CRITICAL FIX: SEQUENCE-FIRST ARCHITECTURE
			// Add card to sequence BEFORE processing special effects
			if (gameState.IsInTakiSequence) {
				TakiLogger.LogRules ("=== SEQUENCE-FIRST: Adding card to sequence before special effects ===");
				gameState.AddCardToSequence (card);

				// Update sequence UI immediately
				int cardCount = gameState.NumberOfSequenceCards;
				CardColor sequenceColor = gameState.TakiSequenceColor;
				GetActiveUI ()?.ShowTakiSequenceStatus (sequenceColor, cardCount, true);
				GetActiveUI ()?.ShowSequenceProgressMessage (sequenceColor, cardCount, PlayerType.Human);

				TakiLogger.LogRules ($"Card added to sequence: {card.GetDisplayText ()} (sequence now has {gameState.NumberOfSequenceCards} cards)");
			}

			// Process special card effects with sequence awareness
			TakiLogger.LogRules ("=== CALLING HandleSpecialCardEffects WITH SEQUENCE AWARENESS ===");
			try {
				HandleSpecialCardEffects (card);
				TakiLogger.LogRules ("=== HandleSpecialCardEffects COMPLETED ===");
			} catch (System.Exception ex) {
				TakiLogger.LogError ($"EXCEPTION in HandleSpecialCardEffects: {ex.Message}", TakiLogger.LogCategory.Rules);
				TakiLogger.LogError ($"Stack trace: {ex.StackTrace}", TakiLogger.LogCategory.Rules);
			}

			// Log comprehensive card effect information
			TakiLogger.LogRules ("=== CALLING LogCardEffectRules ===");
			LogCardEffectRules (card);
			TakiLogger.LogRules ("=== LogCardEffectRules COMPLETED ===");

			// Update UI
			UpdateAllUI ();
			RefreshPlayerHandStates ();

			// PHASE 8B: Handle turn flow based on card type and special effects
			HandlePostCardPlayTurnFlow (card);

			// PHASE 8B: Enhanced win condition checking with sequence awareness
			if (playerHand.Count == 0) {
				if (gameState.IsInTakiSequence) {
					// Player wins but sequence needs to be ended first
					TakiLogger.LogGameState ("Player wins - hand is empty during TAKI sequence!");
					gameState.EndTakiSequence ();
					GetActiveUI ()?.EnableEndTakiSequenceButton (false);
				} else {
					TakiLogger.LogGameState ("Player wins - hand is empty!");
				}
				gameState.DeclareWinner (PlayerType.Human);
				return;
			}

			OnCardPlayed?.Invoke (card);
			TakiLogger.LogTurnFlow ("CARD PLAY COMPLETE - Turn flow handled based on card type");
		}

		/// <summary>
		/// PHASE 7: Handle draw card with special card awareness
		/// </summary>
		void DrawCardWithStrictFlow () {
			TakiLogger.LogCardPlay ("DRAWING CARD WITH STRICT FLOW (PHASE 7)");

			CardData drawnCard = deckManager?.DrawCard ();
			if (drawnCard != null) {
				playerHand.Add (drawnCard);

				TakiLogger.LogCardPlay ($"Player drew: {drawnCard.GetDisplayText ()}");

				// Update visual hands
				UpdateAllUI ();
				RefreshPlayerHandStates ();

				// PHASE 7: Handle turn flow based on special card state
				HandlePostCardDrawTurnFlow (drawnCard);

				TakiLogger.LogCardPlay ("DRAW COMPLETE");
			} else {
				TakiLogger.LogError ("Failed to draw card - deck may be empty", TakiLogger.LogCategory.CardPlay);
				GetActiveUI ()?.ShowPlayerMessage ("Cannot draw card!");
			}
		}

		/// <summary>
		/// Helper: Count playable cards for better feedback
		/// </summary>
		int CountPlayableCards () {
			if (playerHand == null || playerHand.Count == 0) return 0;

			CardData topCard = GetTopDiscardCard ();
			if (topCard == null) return 0;

			int count = 0;
			foreach (CardData card in playerHand) {
				if (gameState != null && gameState.IsValidMove (card, topCard)) {
					count++;
				}
			}
			return count;
		}

		/// <summary>
		/// Get top discard card
		/// </summary>
		public CardData GetTopDiscardCard () {
			if (deckManager == null) {
				TakiLogger.LogError ("GetTopDiscardCard: DeckManager is null!", TakiLogger.LogCategory.System);
				return null;
			}

			return deckManager.GetTopDiscardCard ();
		}

		#endregion

		#region Special Card System
		// Special card effects and rule processing

		/// <summary>
		/// PHASE 8B: FIXED HandleSpecialCardEffects with proper sequence context awareness
		/// CRITICAL RULE: Special effects only activate when NOT in sequence OR when last card in sequence
		/// </summary>
		/// <param name="card">Card that was played</param>
		void HandleSpecialCardEffects (CardData card) {
			TakiLogger.LogRules ($"=== HANDLING SPECIAL CARD EFFECTS for {card.GetDisplayText ()} ===");

			if (card == null) {
				TakiLogger.LogError ("HandleSpecialCardEffects called with NULL card!", TakiLogger.LogCategory.Rules);
				return;
			}

			TakiLogger.LogRules ($"Card type: {card.cardType}");
			TakiLogger.LogRules ($"Card name: {card.cardName}");

			// PHASE 8B: CRITICAL RULE - Special effects only activate when:
			// 1. NOT in TAKI sequence, OR
			// 2. In TAKI sequence AND this is the last card (player clicked End Sequence)
			bool shouldActivateSpecialEffect = !gameState.IsInTakiSequence || isCurrentCardLastInSequence;

			TakiLogger.LogRules ($"SEQUENCE CONTEXT: In sequence={gameState.IsInTakiSequence}, Last card={isCurrentCardLastInSequence}");
			TakiLogger.LogRules ($"SPECIAL EFFECT ACTIVATION: {shouldActivateSpecialEffect}");

			switch (card.cardType) {
				case CardType.Plus:
					if (shouldActivateSpecialEffect) {
						TakiLogger.LogRules ("PLUS card effect - Player must take 1 additional action");
						// Note: Turn flow handling is done in HandlePostCardPlayTurnFlow
					} else {
						TakiLogger.LogRules ("PLUS card effect DEFERRED - card played during TAKI sequence");
					}
					break;

				case CardType.Stop:
					if (shouldActivateSpecialEffect) {
						TakiLogger.LogRules ("=== STOP CARD DETECTED IN SWITCH STATEMENT ===");
						TakiLogger.LogRules ("STOP card effect - Next opponent turn will be skipped");
						try {
							TakiLogger.LogRules ("=== CALLING HandleStopCardEffect ===");
							HandleStopCardEffect ();
							TakiLogger.LogRules ("=== HandleStopCardEffect COMPLETED ===");
						} catch (System.Exception ex) {
							TakiLogger.LogError ($"EXCEPTION in HandleStopCardEffect: {ex.Message}", TakiLogger.LogCategory.Rules);
						}
					} else {
						TakiLogger.LogRules ("STOP card effect DEFERRED - card played during TAKI sequence");
					}
					break;

				case CardType.ChangeDirection:
					if (shouldActivateSpecialEffect) {
						TakiLogger.LogRules ("CHANGE DIRECTION card effect - Turn direction changes");
						HandleChangeDirectionCardEffect ();
					} else {
						TakiLogger.LogRules ("CHANGE DIRECTION card effect DEFERRED - card played during TAKI sequence");
					}
					break;

				case CardType.ChangeColor:
					if (shouldActivateSpecialEffect) {
						TakiLogger.LogRules ("CHANGE COLOR card effect - Player must choose new color");
						HandleChangeColorCardEffect ();
					} else {
						TakiLogger.LogRules ("CHANGE COLOR card effect DEFERRED - card played during TAKI sequence");
					}
					break;

				case CardType.PlusTwo:
					TakiLogger.LogRules ("=== PLUS TWO CARD: ENHANCED CHAIN SYSTEM WITH SEQUENCE AWARENESS ===");

					if (shouldActivateSpecialEffect) {
						PlayerType currentPlayer = turnManager?.CurrentPlayer ?? PlayerType.Human;
						PlayerType targetPlayer = currentPlayer == PlayerType.Human ? PlayerType.Computer : PlayerType.Human;

						if (!gameState.IsPlusTwoChainActive) {
							// Start new chain
							gameState.StartPlusTwoChain (currentPlayer);

							// CRITICAL FIX: Only change interaction state if NOT in TAKI sequence
							if (!gameState.IsInTakiSequence) {
								gameState.ChangeInteractionState (InteractionState.PlusTwoChain);
							}

							TakiLogger.LogRules ("CHAIN STARTED: First PlusTwo played - opponent must draw 2 or continue chain");

							// Enhanced UI integration
							GetActiveUI ()?.ShowChainProgressMessage (1, 2, targetPlayer);
							GetActiveUI ()?.ShowPlusTwoChainStatus (1, 2, targetPlayer == PlayerType.Human);

						} else {
							// Continue existing chain
							gameState.ContinuePlusTwoChain ();

							int chainCount = gameState.NumberOfChainedCards;
							int drawCount = gameState.ChainDrawCount;
							TakiLogger.LogRules ($"CHAIN CONTINUED: Now {chainCount} PlusTwo cards, opponent must draw {drawCount} or continue");

							// Enhanced UI integration
							GetActiveUI ()?.ShowChainProgressMessage (chainCount, drawCount, targetPlayer);
							GetActiveUI ()?.ShowPlusTwoChainStatus (chainCount, drawCount, targetPlayer == PlayerType.Human);
						}

						TakiLogger.LogRules ($"PlusTwo chain status: {gameState.NumberOfChainedCards} cards, {gameState.ChainDrawCount} total draw");
					} else {
						TakiLogger.LogRules ("PLUS TWO card effect DEFERRED - card played during TAKI sequence");
					}
					break;

				case CardType.Taki:
					// PHASE 8B: TAKI card implementation - same-color sequence
					TakiLogger.LogRules ("=== TAKI CARD: SAME-COLOR SEQUENCE ===");

					if (!gameState.IsInTakiSequence) {
						// Start new TAKI sequence using the TAKI card's color
						TakiLogger.LogRules ($"TAKI SEQUENCE START: Starting sequence for color {card.color}");
						gameState.StartTakiSequence (card.color, turnManager?.CurrentPlayer ?? PlayerType.Human);

						// Add the TAKI card itself to the sequence
						gameState.AddCardToSequence (card);

						GetActiveUI ()?.EnableEndTakiSequenceButton (true);
						GetActiveUI ()?.ShowTakiSequenceStatus (card.color, 1, true);
						GetActiveUI ()?.ShowPlayerMessage ($"TAKI Sequence Started! Play {card.color} cards or click End Sequence");

						TakiLogger.LogRules ($"TAKI sequence initiated - only {card.color} or wild cards allowed");
						// Do NOT end turn - continue sequence
						return; // Important: skip normal turn end flow
					} else {
						// TAKI card played within existing sequence
						if (isCurrentCardLastInSequence) {
							// TAKI as last card - end current sequence but could start new one
							TakiLogger.LogRules ("TAKI card played as last in sequence - sequence will end");
							gameState.AddCardToSequence (card);
							// Note: Sequence ending will be handled by End Sequence button logic
						} else {
							// TAKI in middle of sequence - no special effect, just add to sequence
							TakiLogger.LogRules ("TAKI card played in middle of sequence - added to sequence");
							gameState.AddCardToSequence (card);
						}
					}
					break;

				case CardType.SuperTaki:
					// PHASE 8B: SuperTAKI card implementation
					TakiLogger.LogRules ("=== SUPER TAKI CARD: WILD SEQUENCE INITIATOR ===");

					if (!gameState.IsInTakiSequence) {
						// SuperTAKI uses current active color
						CardColor sequenceColor = gameState.activeColor;

						TakiLogger.LogRules ($"SUPER TAKI SEQUENCE START: Starting sequence for current active color {sequenceColor}");
						gameState.StartTakiSequence (sequenceColor, turnManager?.CurrentPlayer ?? PlayerType.Human);

						// Add the SuperTAKI card itself to the sequence
						gameState.AddCardToSequence (card);

						GetActiveUI ()?.EnableEndTakiSequenceButton (true);
						GetActiveUI ()?.ShowTakiSequenceStatus (sequenceColor, 1, true);
						GetActiveUI ()?.ShowPlayerMessage ($"SuperTAKI Sequence Started! Play {sequenceColor} cards or click End Sequence");

						TakiLogger.LogRules ($"SUPER TAKI sequence initiated - only {sequenceColor} or wild cards allowed");
						// Do NOT end turn - continue sequence
						return; // Important: skip normal turn end flow
					} else {
						// SuperTAKI card played within existing sequence
						if (isCurrentCardLastInSequence) {
							// SuperTAKI as last card - end current sequence
							TakiLogger.LogRules ("SUPER TAKI card played as last in sequence - sequence will end");
							gameState.AddCardToSequence (card);
						} else {
							// SuperTAKI in middle of sequence - no special effect, just add to sequence
							TakiLogger.LogRules ("SUPER TAKI card played in middle of sequence - added to sequence");
							gameState.AddCardToSequence (card);
						}
					}
					break;

				case CardType.Number:
					// PHASE 8B: Number cards can be part of sequences but have no special effects
					if (gameState.IsInTakiSequence) {
						TakiLogger.LogRules ("NUMBER card played in sequence - added to sequence");
						gameState.AddCardToSequence (card);
					} else {
						TakiLogger.LogRules ("NUMBER card - no special effects");
					}
					break;

				default:
					TakiLogger.LogWarning ($"Unknown card type for special effects: {card.cardType}", TakiLogger.LogCategory.Rules);
					break;
			}
		}

		/// <summary>
		/// ENHANCED DEBUG: Handle STOP card effect using flag-based system
		/// </summary>
		void HandleStopCardEffect () {
			TakiLogger.LogRules ("=== ENTERED HandleStopCardEffect METHOD ===");
			TakiLogger.LogRules ("=== EXECUTING STOP CARD EFFECT (FLAG-BASED) ===");

			// Check if turnManager exists
			if (turnManager == null) {
				TakiLogger.LogError ("HandleStopCardEffect: turnManager is NULL!", TakiLogger.LogCategory.Rules);
				return;
			}

			// Determine who played the STOP card
			PlayerType currentPlayer = turnManager.CurrentPlayer;
			PlayerType targetPlayer = currentPlayer == PlayerType.Human ? PlayerType.Computer : PlayerType.Human;

			TakiLogger.LogRules ($"Current player (who played STOP): {currentPlayer}");
			TakiLogger.LogRules ($"Target player (who will be skipped): {targetPlayer}");

			// Set the flag - next turn will be skipped
			shouldSkipNextTurn = true;
			stopCardPlayer = currentPlayer;

			TakiLogger.LogRules ($"STOP flag set: {targetPlayer} turn will be skipped when it starts");
			TakiLogger.LogRules ($"STOP played by: {currentPlayer}, Target: {targetPlayer}");
			TakiLogger.LogRules ($"shouldSkipNextTurn is now: {shouldSkipNextTurn}");

			// Show immediate feedback
			string message = targetPlayer == PlayerType.Computer ?
				"STOP: Computer's next turn will be skipped!" :
				"STOP: Human's next turn will be skipped!";

			TakiLogger.LogRules ($"Showing message: {message}");
			GetActiveUI ()?.ShowPlayerMessage (message);
			GetActiveUI ()?.ShowOpponentMessage ($"STOP card effect - {targetPlayer} turn scheduled for skip");

			TakiLogger.LogRules ("STOP effect prepared - flag set for next turn check");
			TakiLogger.LogRules ("=== EXITING HandleStopCardEffect METHOD ===");
		}

		/// <summary>
		/// PHASE 7: Handle ChangeDirection card effect - reverse turn direction
		/// </summary>
		void HandleChangeDirectionCardEffect () {
			TakiLogger.LogRules ("=== EXECUTING CHANGE DIRECTION CARD EFFECT ===");

			// Get current direction before change
			string oldDirection = gameState?.turnDirection.ToString () ?? "Unknown";

			// Change direction using existing GameStateManager functionality
			if (gameState != null) {
				gameState.ChangeTurnDirection ();

				// Get new direction after change
				string newDirection = gameState.turnDirection.ToString ();

				TakiLogger.LogGameState ($"Direction changed from {oldDirection} to {newDirection}");
				TakiLogger.LogRules ($"CHANGE DIRECTION: {oldDirection} -> {newDirection}");

				// Show immediate feedback to players
				GetActiveUI ()?.ShowPlayerMessage ($"DIRECTION CHANGED: {oldDirection} -> {newDirection}");
				GetActiveUI ()?.ShowOpponentMessage ($"Turn direction: {newDirection}");

				// For 2-player game, this is mainly visual/informational
				string twoPlayerNote = GetTwoPlayerDirectionNote (newDirection);
				if (!string.IsNullOrEmpty (twoPlayerNote)) {
					GetActiveUI ()?.ShowOpponentMessage (twoPlayerNote);
				}

				TakiLogger.LogRules ("CHANGE DIRECTION effect complete");
			} else {
				TakiLogger.LogError ("Cannot change direction: GameStateManager is null!", TakiLogger.LogCategory.Rules);
				GetActiveUI ()?.ShowPlayerMessage ("ERROR: Cannot change direction!");
			}
		}

		/// <summary>
		/// FIXED: Handle ChangeColor card effect - CORE LOGIC ONLY (no UI)
		/// UI logic moved to appropriate player-specific locations
		/// </summary>
		void HandleChangeColorCardEffect () {
			TakiLogger.LogRules ("=== EXECUTING CHANGE COLOR CARD EFFECT ===");

			if (gameState == null) {
				TakiLogger.LogError ("Cannot change color: GameStateManager is null!", TakiLogger.LogCategory.Rules);
				GetActiveUI ()?.ShowPlayerMessage ("ERROR: Cannot change color!");
				return;
			}

			if (gameplayUI == null) {
				TakiLogger.LogError ("Cannot show color selection: GameplayUIManager is null!", TakiLogger.LogCategory.Rules);
				return;
			}

			// CORE LOGIC: This applies to ALL players (human and AI)
			// Just log that a ChangeColor card was played - actual color selection handled separately
			TakiLogger.LogRules ("CHANGE COLOR card played - color selection required");
			TakiLogger.LogRules ("Color will be selected by appropriate player handler");
		}

		/// <summary>
		/// PHASE 7: Enhanced LogCardEffectRules with comprehensive special card documentation
		/// </summary>
		void LogCardEffectRules (CardData card) {
			TakiLogger.LogRules ($"=== CARD EFFECT ANALYSIS: {card.GetDisplayText ()} ===");

			switch (card.cardType) {
				case CardType.Number:
					TakiLogger.LogRules ($"NUMBER CARD: {card.GetDisplayText ()}");
					TakiLogger.LogRules ("RULE: Basic card - no special effects");
					TakiLogger.LogRules ("TURN FLOW: Player must END TURN after playing");
					TakiLogger.LogRules ("IMPLEMENTATION: Standard single-action turn completion");
					GetActiveUI ()?.ShowPlayerMessage ($"Played NUMBER {card.GetDisplayText ()}");
					break;

				case CardType.Plus:
					// PHASE 7: Comprehensive Plus card documentation
					TakiLogger.LogRules ("PLUS CARD: Additional Action Required");
					TakiLogger.LogRules ("RULE: Player must take exactly 1 additional action (PLAY or DRAW)");
					TakiLogger.LogRules ("TURN FLOW: Action buttons re-enabled, END TURN disabled until additional action");
					TakiLogger.LogRules ("IMPLEMENTATION STATUS: FULLY IMPLEMENTED in Phase 7");
					TakiLogger.LogRules ("- HandlePostCardPlayTurnFlow() manages additional action requirement");
					TakiLogger.LogRules ("- isWaitingForAdditionalAction state tracking");
					TakiLogger.LogRules ("- Enhanced button state management");
					TakiLogger.LogRules ("- Complete turn flow validation");
					GetActiveUI ()?.ShowOpponentMessage ("PLUS: Additional action required!");
					break;

				case CardType.Stop:
					// PHASE 7: Comprehensive Stop card documentation
					TakiLogger.LogRules ("STOP CARD: Turn Skipping");
					TakiLogger.LogRules ("RULE: Opponent's next turn is completely skipped");
					TakiLogger.LogRules ("TURN FLOW: Current player gets another full turn immediately");
					TakiLogger.LogRules ("IMPLEMENTATION STATUS: FULLY IMPLEMENTED in Phase 7");
					TakiLogger.LogRules ("- HandleStopCardEffect() prepares skip logic");
					TakiLogger.LogRules ("- TurnManager.SkipTurn() integration");
					TakiLogger.LogRules ("- Enhanced EndPlayerTurnWithStrictFlow() handles skipping");
					GetActiveUI ()?.ShowOpponentMessage ("STOP: Opponent's turn skipped!");
					break;

				case CardType.ChangeDirection:
					// PHASE 7: Comprehensive ChangeDirection card documentation
					TakiLogger.LogRules ("CHANGE DIRECTION CARD: Turn Direction Reversal");
					TakiLogger.LogRules ("RULE: Reverses turn direction (Clockwise <-> CounterClockwise)");
					TakiLogger.LogRules ("TURN FLOW: Normal turn completion after direction change");
					TakiLogger.LogRules ("2-PLAYER NOTE: Direction change is informational only");
					TakiLogger.LogRules ("IMPLEMENTATION STATUS: FULLY IMPLEMENTED in Phase 7");
					TakiLogger.LogRules ("- HandleChangeDirectionCardEffect() manages direction change");
					TakiLogger.LogRules ("- GameStateManager.ChangeTurnDirection() integration");
					TakiLogger.LogRules ("- Clear player feedback with before/after direction display");
					GetActiveUI ()?.ShowOpponentMessage ("DIRECTION: Turn direction changed!");
					break;

				case CardType.ChangeColor:
					// PHASE 7: Comprehensive ChangeColor card documentation
					TakiLogger.LogRules ("CHANGE COLOR CARD: Color Selection Required");
					TakiLogger.LogRules ("RULE: Player must choose new active color for game");
					TakiLogger.LogRules ("TURN FLOW: Color selection required before turn can end");
					TakiLogger.LogRules ("UI FLOW: ColorSelectionPanel shown, action buttons disabled during selection");
					TakiLogger.LogRules ("IMPLEMENTATION STATUS: FULLY IMPLEMENTED in Phase 7");
					TakiLogger.LogRules ("- HandleChangeColorCardEffect() triggers color selection");
					TakiLogger.LogRules ("- InteractionState.ColorSelection state management");
					TakiLogger.LogRules ("- Enhanced OnColorSelectedByPlayer() handles selection");
					TakiLogger.LogRules ("- Complete integration with existing color selection system");
					GetActiveUI ()?.ShowOpponentMessage ("CHANGE COLOR: Color selection active!");
					break;

				case CardType.PlusTwo:
					// PHASE 8: Enhanced PlusTwo documentation with Phase 8 roadmap
					TakiLogger.LogRules ("PLUS TWO CARD: Opponent Draws 2 Cards");
					TakiLogger.LogRules ("CURRENT RULE: Simple - opponent draws 2 cards");
					TakiLogger.LogRules ("IMPLEMENTATION STATUS: BASIC implementation (Phase 7)");
					TakiLogger.LogRules ("PHASE 8 ENHANCEMENT: Advanced chaining system planned");
					TakiLogger.LogRules ("- PlusTwo chain stacking (+2, +4, +6, +8...)");
					TakiLogger.LogRules ("- Chain breaking strategy for AI");
					TakiLogger.LogRules ("- InteractionState.PlusTwoChain management");
					TakiLogger.LogRules ("- Enhanced UI for chain display");
					GetActiveUI ()?.ShowOpponentMessage ("PLUS TWO: Opponent draws 2 cards");
					break;

				case CardType.Taki:
					// PHASE 8: Enhanced Taki documentation
					TakiLogger.LogRules ($"TAKI CARD: Multi-Card Sequence of {card.color} Color");
					TakiLogger.LogRules ("FUTURE RULE: Player may play multiple cards of same color");
					TakiLogger.LogRules ("IMPLEMENTATION STATUS: PLANNED for Phase 8");
					TakiLogger.LogRules ("PHASE 8 FEATURES:");
					TakiLogger.LogRules ("- InteractionState.TakiSequence management");
					TakiLogger.LogRules ("- Btn_Player1EndTakiSequence integration");
					TakiLogger.LogRules ("- Same-color card validation during sequence");
					TakiLogger.LogRules ("- AI strategy for sequence length optimization");
					TakiLogger.LogRules ("- Sequence status UI display");
					GetActiveUI ()?.ShowOpponentMessage ($"TAKI: Multi-card sequence planned (Phase 8)");
					break;

				case CardType.SuperTaki:
					// PHASE 8: Enhanced SuperTaki documentation
					TakiLogger.LogRules ("SUPER TAKI CARD: Multi-Card Sequence of Any Color");
					TakiLogger.LogRules ("FUTURE RULE: Player may play multiple cards of any color");
					TakiLogger.LogRules ("IMPLEMENTATION STATUS: PLANNED for Phase 8");
					TakiLogger.LogRules ("PHASE 8 FEATURES:");
					TakiLogger.LogRules ("- Similar to TAKI but any color allowed");
					TakiLogger.LogRules ("- Enhanced strategic value for AI");
					TakiLogger.LogRules ("- Shared UI system with TAKI sequences");
					GetActiveUI ()?.ShowOpponentMessage ("SUPER TAKI: Multi-card any color planned (Phase 8)");
					break;

				default:
					TakiLogger.LogWarning ($"UNKNOWN CARD TYPE: {card.cardType}", TakiLogger.LogCategory.Rules);
					TakiLogger.LogRules ("ERROR: Card type not recognized by rule system");
					GetActiveUI ()?.ShowOpponentMessage ($"Unknown card: {card.GetDisplayText ()}");
					break;
			}

			TakiLogger.LogRules ("=== CARD EFFECT ANALYSIS COMPLETE ===");
		}

		/// <summary>
		/// ENHANCED: Reset special card state including STOP flag
		/// </summary>
		void ResetSpecialCardState () {
			TakiLogger.LogTurnFlow ("=== RESETTING SPECIAL CARD STATE ===");

			bool hadActiveEffect = isWaitingForAdditionalAction;
			bool hadStopFlag = shouldSkipNextTurn;

			isWaitingForAdditionalAction = false;
			activeSpecialCardEffect = CardType.Number; // No special effect
			shouldSkipNextTurn = false;
			// No need to reset stopCardPlayer as it's always set when flag is set

			if (hadActiveEffect) {
				TakiLogger.LogTurnFlow ("Special card state reset - was waiting for additional action");
			}

			if (hadStopFlag) {
				TakiLogger.LogTurnFlow ("STOP flag reset - was set for skip");
			}
		}

		/// <summary>
		/// PHASE 7: Check if player has pending special card effects
		/// </summary>
		/// <returns>True if special card effects are pending completion</returns>
		bool HasPendingSpecialCardEffects () {
			return isWaitingForAdditionalAction;
		}

		/// <summary>
		/// PHASE 7: Get description of current special card state for debugging
		/// </summary>
		/// <returns>Description of special card state</returns>
		string GetSpecialCardStateDescription () {
			if (!isWaitingForAdditionalAction) {
				return "No pending special card effects";
			}

			return $"Waiting for additional action (Effect: {activeSpecialCardEffect})";
		}

		/// <summary>
		/// FIXED: Process STOP card skip effect - called when STOP flag is detected
		/// Corrected logic to properly determine who benefits based on who played STOP
		/// </summary>
		void ProcessStopSkipEffect () {
			TakiLogger.LogTurnFlow ("=== PROCESSING STOP SKIP EFFECT ===");

			// Clear the STOP flag first
			bool wasStopFlagSet = shouldSkipNextTurn;
			shouldSkipNextTurn = false;
			PlayerType whoPlayedStop = stopCardPlayer;

			TakiLogger.LogTurnFlow ($"STOP flag cleared: was {wasStopFlagSet}, now {shouldSkipNextTurn}");
			TakiLogger.LogTurnFlow ($"STOP played by: {whoPlayedStop}");

			// FIXED LOGIC: Determine who gets skipped and who benefits based on who played STOP
			PlayerType skippedPlayer;
			PlayerType benefitPlayer;

			if (whoPlayedStop == PlayerType.Human) {
				// Human played STOP -> Computer gets skipped -> Human benefits
				skippedPlayer = PlayerType.Computer;
				benefitPlayer = PlayerType.Human;
			} else {
				// Computer played STOP -> Human gets skipped -> Computer benefits  
				skippedPlayer = PlayerType.Human;
				benefitPlayer = PlayerType.Computer;
			}

			TakiLogger.LogTurnFlow ($"CORRECTED LOGIC: {whoPlayedStop} played STOP -> {skippedPlayer} gets skipped -> {benefitPlayer} benefits");

			// Show STOP effect feedback based on who benefits
			string skipMessage = skippedPlayer == PlayerType.Human ?
				"STOP effect: Your turn is skipped!" :
				"STOP effect: Computer's turn is skipped!";

			string benefitMessage = benefitPlayer == PlayerType.Human ?
				"STOP effect: You get another full turn!" :
				"STOP effect: Computer gets another full turn!";

			GetActiveUI ()?.ShowPlayerMessage (benefitMessage);
			GetActiveUI ()?.ShowOpponentMessage (skipMessage);

			TakiLogger.LogTurnFlow ($"STOP effect: {skippedPlayer} turn skipped, {benefitPlayer} gets another turn");

			// Clear any selected cards from previous action
			GetActivePlayerHandManager ()?.ClearSelection ();

			// Reset turn flow state for the new turn
			ResetTurnFlowState ();

			// Start the benefit player's turn
			if (benefitPlayer == PlayerType.Human) {
				TakiLogger.LogTurnFlow ("Starting fresh player turn due to STOP effect");
				// Use a small delay to ensure UI updates are processed
				Invoke (nameof (StartPlayerTurnAfterStop), 0.2f);
			} else {
				TakiLogger.LogTurnFlow ("Starting fresh computer turn due to STOP effect");
				// Use a small delay to ensure UI updates are processed  
				Invoke (nameof (StartAITurnAfterStop), 0.2f);
			}
		}

		/// <summary>
		/// Handle player breaking PlusTwo chain by drawing accumulated cards
		/// </summary>
		void BreakPlusTwoChainByDrawing () {
			int cardsToDraw = gameState.ChainDrawCount;
			int chainLength = gameState.NumberOfChainedCards;

			TakiLogger.LogCardPlay ($"=== BREAKING PLSTWO CHAIN ===");
			TakiLogger.LogCardPlay ($"Chain: {chainLength} PlusTwo cards, drawing {cardsToDraw} cards");

			// Draw the accumulated cards
			List<CardData> drawnCards = new List<CardData> ();

			if (deckManager != null) {
				for (int i = 0; i < cardsToDraw; i++) {
					CardData drawnCard = deckManager.DrawCard ();
					if (drawnCard != null) {
						drawnCards.Add (drawnCard);
						playerHand.Add (drawnCard);
					} else {
						TakiLogger.LogWarning ($"Deck exhausted during chain break: got {drawnCards.Count}/{cardsToDraw} cards", TakiLogger.LogCategory.CardPlay);
						break;
					}
				}
			}

			if (drawnCards.Count > 0) {
				TakiLogger.LogCardPlay ($"Player drew {drawnCards.Count} cards to break PlusTwo chain");

				// Enhanced UI feedback
				GetActiveUI ()?.ShowChainBrokenMessage (drawnCards.Count, PlayerType.Human);

				// Show what cards were drawn (first few for feedback)
				if (drawnCards.Count <= 3) {
					string cardList = string.Join (", ", drawnCards.Select (c => c.GetDisplayText ()));
					TakiLogger.LogCardPlay ($"Cards drawn: {cardList}");
				} else {
					TakiLogger.LogCardPlay ($"Cards drawn: {drawnCards [0].GetDisplayText ()}, {drawnCards [1].GetDisplayText ()}, ... and {drawnCards.Count - 2} more");
				}
			} else {
				TakiLogger.LogError ("Failed to draw any cards for chain breaking", TakiLogger.LogCategory.CardPlay);
				GetActiveUI ()?.ShowPlayerMessage ("Error: Cannot draw cards!");
				return;
			}

			// Break the chain and return to normal state
			gameState.BreakPlusTwoChain ();
			gameState.ChangeInteractionState (InteractionState.Normal);

			// Update UI to reflect new hand and normal state
			UpdateAllUI ();
			RefreshPlayerHandStates ();

			// Handle turn flow - player has taken action by drawing
			TakiLogger.LogTurnFlow ("Chain break completed - handling post-draw turn flow");
			HandlePostCardDrawTurnFlow (drawnCards.LastOrDefault ());
		}

		/// <summary>
		/// Make opponent draw cards
		/// </summary>
		/// <param name="count">Number of cards to draw</param>
		void MakeOpponentDrawCards (int count) {
			if (gameState.IsPlayerTurn) {
				// Computer draws cards
				List<CardData> drawnCards = deckManager.DrawCards (count);
				if (computerAI != null) {
					computerAI.AddCardsToHand (drawnCards);
				}
				TakiLogger.LogCardPlay ($"Computer drew {drawnCards.Count} cards");
			} else {
				// Player draws cards
				List<CardData> drawnCards = deckManager.DrawCards (count);
				playerHand.AddRange (drawnCards);
				TakiLogger.LogCardPlay ($"Player drew {drawnCards.Count} cards");
			}

			UpdateAllUI ();
		}

		/// <summary>
		/// PHASE 7: Get direction change note for 2-player game
		/// </summary>
		/// <param name="newDirection">New turn direction</param>
		/// <returns>Explanatory note for 2-player context</returns>
		string GetTwoPlayerDirectionNote (string newDirection) {
			// In 2-player games, direction change is mostly cosmetic
			// but it's still part of the official TAKI rules

			switch (newDirection) {
				case "Clockwise":
					return "(2-player: Direction change noted)";
				case "CounterClockwise":
					return "(2-player: Direction change noted)";
				default:
					return "";
			}
		}

		#endregion

		#region AI Integration
		// Computer AI event handlers and coordination

		/// <summary>
		/// Handle computer turn ready handler with error checking
		void OnComputerTurnReady () {
			TakiLogger.LogAI ("COMPUTER TURN READY");

			if (computerAI == null || deckManager == null) {
				TakiLogger.LogError ("Computer turn ready but components are null!", TakiLogger.LogCategory.AI);
				return;
			}

			CardData topCard = deckManager.GetTopDiscardCard ();
			if (topCard == null) {
				TakiLogger.LogError ("Computer turn ready but no top discard card!", TakiLogger.LogCategory.AI);
				return;
			}

			TakiLogger.LogAI ("Triggering AI decision for top card: " + topCard.GetDisplayText ());
			computerAI.MakeDecision (topCard);
		}

		/// <summary>
		/// FIXED: AI card selection handler with TAKI sequence awareness
		/// </summary> 
		void OnAICardSelected (CardData card) {
			TakiLogger.LogAI ("=== AI SELECTED CARD (CHAIN AWARE + SEQUENCE AWARE): " + (card != null ? card.GetDisplayText () : "NULL") + " ===");

			if (card == null || computerAI == null) {
				TakiLogger.LogError ("AI selected null card or computerAI is null!", TakiLogger.LogCategory.AI);
				return;
			}

			// CHAIN VALIDATION: During PlusTwo chain, verify this is a PlusTwo card
			if (gameState != null && gameState.IsPlusTwoChainActive && card.cardType != CardType.PlusTwo) {
				TakiLogger.LogError ($"AI tried to play {card.GetDisplayText ()} during PlusTwo chain - only PlusTwo allowed!", TakiLogger.LogCategory.AI);
				GetActiveUI ()?.ShowOpponentMessage ("Error: Invalid card during chain");
				return;
			}

			if (deckManager != null) {
				deckManager.DiscardCard (card);
				TakiLogger.LogAI ("AI card discarded: " + card.GetDisplayText ());
			}

			if (gameState != null) {
				gameState.UpdateActiveColorFromCard (card);
				TakiLogger.LogGameState ("Active color updated to: " + gameState.activeColor);
			}

			// Enhanced logging for chain scenarios
			if (gameState != null && gameState.IsPlusTwoChainActive && card.cardType == CardType.PlusTwo) {
				int newDrawCount = gameState.ChainDrawCount;
				TakiLogger.LogAI ($"AI continued PlusTwo chain - now opponent must draw {newDrawCount} or continue");
			} else {
				TakiLogger.LogAI ($"AI played {card.cardType}: {card.GetDisplayText ()}");
			}

			UpdateAllUI ();

			// Check for AI win condition
			if (computerAI.HandSize == 0) {
				// CHAIN RULE: If game ends during active chain, validate chain resolution
				if (gameState != null && gameState.IsPlusTwoChainActive && card.cardType == CardType.PlusTwo) {
					TakiLogger.LogGameState ("AI played last card (PlusTwo) but chain is active - must wait for resolution");
					GetActiveUI ()?.ShowOpponentMessage ("I win if you don't continue the chain!");
					// Don't declare winner yet - chain must be resolved first
					// Game will end after player responds to chain
				} else if (gameState != null && gameState.IsInTakiSequence) {
					TakiLogger.LogGameState ("AI wins - hand is empty during TAKI sequence!");
					gameState.EndTakiSequence ();
					gameplayUI?.EnableEndTakiSequenceButton (false);
					gameplayUI?.ShowSequenceEndedMessage (gameState.NumberOfSequenceCards, gameState.TakiSequenceColor, PlayerType.Computer);
				} else {
					TakiLogger.LogGameState ("Computer wins - hand is empty!");
				}

				if (gameState != null) {
					gameState.DeclareWinner (PlayerType.Computer);
				}
				return;
			}

			// CRITICAL FIX: Handle turn flow based on sequence state
			if (gameState != null && gameState.IsInTakiSequence) {
				TakiLogger.LogAI ("AI played card during TAKI sequence - handling sequence turn flow");

				// During TAKI sequence, handle special card effects but don't end turn normally
				if (card.IsSpecialCard) {
					TakiLogger.LogAI ($"AI played special card in sequence - handling {card.cardType} effect");

					// Call main special card effects method
					HandleSpecialCardEffects (card);

					// Handle AI-specific turn flow for special cards in sequence
					HandleAISpecialCardEffects (card);
				} else {
					// FIXED: Number card during sequence - continue sequence, don't end turn
					TakiLogger.LogAI ("AI played number card in sequence - continuing sequence");

					// The sequence continuation is handled by the existing AI sequence logic
					// Don't call EndTurn() here - let the sequence logic handle it
				}
			} else {
				// NORMAL FLOW: Not in TAKI sequence, handle normally
				if (card.IsSpecialCard) {
					TakiLogger.LogAI ($"AI played special card - handling {card.cardType} effect");

					// Call main special card effects method to set flags and handle core logic
					TakiLogger.LogAI ("=== CALLING MAIN HandleSpecialCardEffects for AI card ===");
					HandleSpecialCardEffects (card);

					// Then handle AI-specific turn flow
					TakiLogger.LogAI ("=== CALLING AI-specific turn flow handling ===");
					HandleAISpecialCardEffects (card);
				} else {
					// Normal card outside sequence - end turn normally
					TakiLogger.LogAI ("AI played normal card (not in sequence) - ending turn normally");
					if (turnManager != null) {
						turnManager.EndTurn ();
					}
				}
			}
		}

		/// <summary>
		/// AI draw card handler
		/// </summary>
		void OnAIDrawCard () {
			TakiLogger.LogAI ("=== AI DRAWING CARD (CHAIN AWARE) ===");

			if (deckManager == null || computerAI == null) {
				TakiLogger.LogError ("AI draw card but components are null!", TakiLogger.LogCategory.AI);
				return;
			}

			// CRITICAL: Check for PlusTwo chain breaking FIRST
			if (gameState != null && gameState.IsPlusTwoChainActive) {
				TakiLogger.LogAI ("=== AI BREAKING PLSTWO CHAIN BY DRAWING ===");

				int cardsToDraw = gameState.ChainDrawCount;
				int chainLength = gameState.NumberOfChainedCards;

				TakiLogger.LogAI ($"AI breaking chain: {chainLength} PlusTwo cards, drawing {cardsToDraw} cards");

				// Draw the accumulated cards 
				List<CardData> drawnCards = new List<CardData> ();

				for (int i = 0; i < cardsToDraw; i++) {
					CardData singleDrawnCard = deckManager.DrawCard ();
					if (singleDrawnCard != null) {
						drawnCards.Add (singleDrawnCard);
						computerAI.AddCardToHand (singleDrawnCard);
					} else {
						TakiLogger.LogWarning ($"Deck exhausted during AI chain break: got {drawnCards.Count}/{cardsToDraw} cards", TakiLogger.LogCategory.AI);
						break;
					}
				}

				if (drawnCards.Count > 0) {
					TakiLogger.LogAI ($"AI drew {drawnCards.Count} cards to break PlusTwo chain");

					// Enhanced feedback messages
					GetActiveUI ()?.ShowPlayerMessage ($"AI broke chain by drawing {drawnCards.Count} cards");
					GetActiveUI ()?.ShowOpponentMessage ($"I drew {drawnCards.Count} cards - chain broken");

					// Log what cards were drawn (for debugging)
					if (drawnCards.Count <= 3) {
						string cardList = string.Join (", ", drawnCards.Select (c => c.GetDisplayText ()));
						TakiLogger.LogAI ($"AI drew cards: {cardList}");
					} else {
						TakiLogger.LogAI ($"AI drew: {drawnCards [0].GetDisplayText ()}, {drawnCards [1].GetDisplayText ()}, ... and {drawnCards.Count - 2} more");
					}
				} else {
					TakiLogger.LogError ("AI failed to draw any cards for chain breaking", TakiLogger.LogCategory.AI);
					GetActiveUI ()?.ShowOpponentMessage ("Error: Cannot draw cards!");
				}

				// Break the chain and return to normal state
				gameState.BreakPlusTwoChain ();
				gameState.ChangeInteractionState (InteractionState.Normal);

				// Update UI to reflect new AI hand and normal state
				UpdateAllUI ();

				// End AI turn after breaking chain
				TakiLogger.LogAI ("AI chain break completed - ending turn");
				EndAITurnWithStrictFlow ();
				return;
			}

			// ... KEEP ALL EXISTING NORMAL AI DRAW LOGIC AFTER THIS POINT ...
			TakiLogger.LogAI ("Normal AI draw (no active chain)");

			CardData drawnCard = deckManager.DrawCard ();
			if (drawnCard != null) {
				computerAI.AddCardToHand (drawnCard);
				TakiLogger.LogAI ("AI drew card: " + drawnCard.GetDisplayText ());
				UpdateAllUI ();
			} else {
				TakiLogger.LogError ("AI could not draw card - deck empty?", TakiLogger.LogCategory.AI);
			}

			TakiLogger.LogAI ("Ending computer turn after draw");
			// FIXED: Use new AI turn end method that checks STOP flag
			EndAITurnWithStrictFlow ();
		}

		void OnAIColorSelected (CardColor color) {
			// AI selected a color
			if (gameState != null) {
				gameState.ChangeActiveColor (color);
			}
		}

		void OnAIDecisionMade (string decision) {
			// AI made a decision - show in UI 
			if (gameplayUI != null) {
				GetActiveUI ()?.ShowOpponentMessage (decision);
			}
		}

		/// <summary> 
		/// UPDATED: GameManager handler for AI sequence complete - prevent double turn switching
		/// </summary>
		void OnAISequenceComplete () {
			TakiLogger.LogAI ("AI sequence complete - ending AI turn");

			// CRITICAL FIX: Check if we're already in the middle of a turn switch
			// Don't call EndAITurnWithStrictFlow if turn is already switching
			if (gameState != null && gameState.turnState == TurnState.ComputerTurn) {
				TakiLogger.LogAI ("Ending AI turn after sequence completion");
				EndAITurnWithStrictFlow (); // This method exists in GameManager.cs
			} else {
				TakiLogger.LogAI ("Turn already switched - not calling EndAITurnWithStrictFlow again");
			}
		}

		/// <summary>
		/// FIXED: Handle AI-specific turn flow for special cards - Enhanced TAKI sequence & PlusTwo integration
		/// CRITICAL FIX: Proper sequence state awareness and last card effect processing
		/// </summary>
		/// <param name="card">Card played by AI</param>
		void HandleAISpecialCardEffects (CardData card) {
			TakiLogger.LogAI ($"=== AI HANDLING TURN FLOW for {card.GetDisplayText ()} ===");
			TakiLogger.LogAI ("Note: Core special card effects already handled by main HandleSpecialCardEffects method");

			// CRITICAL FIX: Check if we're in a TAKI sequence
			bool inTakiSequence = gameState.IsInTakiSequence;
			bool isLastCardInSequence = isCurrentCardLastInSequence;

			TakiLogger.LogAI ($"Sequence context: InSequence={inTakiSequence}, IsLastCard={isLastCardInSequence}");

			switch (card.cardType) {
				case CardType.Plus:
					if (inTakiSequence && !isLastCardInSequence) {
						// Plus card played during sequence - effect deferred
						TakiLogger.LogAI ("AI played PLUS during TAKI sequence - effect DEFERRED, continuing sequence");

						// Continue sequence - don't end turn
						CardData topCard = GetTopDiscardCard ();
						if (topCard != null && computerAI != null) {
							Invoke (nameof (TriggerAISequenceDecision), 1.0f);
						} else {
							TakiLogger.LogError ("Cannot continue AI sequence after Plus - missing components", TakiLogger.LogCategory.AI);
							EndAITurnWithStrictFlow ();
						}
					} else {
						// Normal Plus card handling (not in sequence or last card)
						TakiLogger.LogAI ("AI played PLUS card - AI gets additional action");

						if (gameplayUI != null) {
							GetActiveUI ()?.ShowSpecialCardEffect (CardType.Plus, PlayerType.Computer, "AI gets one more action");
						}

						// AI needs to make another decision
						CardData topCard = GetTopDiscardCard ();
						if (topCard != null && computerAI != null) {
							TakiLogger.LogAI ("Triggering AI additional action for PLUS card");
							Invoke (nameof (TriggerAIAdditionalAction), 1.0f);
						}
					}
					break;

				case CardType.Stop:
					if (inTakiSequence && !isLastCardInSequence) {
						// Stop card played during sequence - effect deferred
						TakiLogger.LogAI ("AI played STOP during TAKI sequence - effect DEFERRED, continuing sequence");

						// Continue sequence
						CardData topCard = GetTopDiscardCard ();
						if (topCard != null && computerAI != null) {
							Invoke (nameof (TriggerAISequenceDecision), 1.0f);
						} else {
							EndAITurnWithStrictFlow ();
						}
					} else {
						// Normal Stop card handling
						TakiLogger.LogAI ("AI played STOP card - flag already set by main method");

						if (gameplayUI != null) {
							GetActiveUI ()?.ShowSpecialCardEffect (CardType.Stop, PlayerType.Computer, "Your next turn will be skipped!");
						}

						EndAITurnWithStrictFlow ();
					}
					break;

				case CardType.ChangeDirection:
					if (inTakiSequence && !isLastCardInSequence) {
						// ChangeDirection during sequence - effect deferred
						TakiLogger.LogAI ("AI played CHANGE DIRECTION during TAKI sequence - effect DEFERRED, continuing sequence");

						CardData topCard = GetTopDiscardCard ();
						if (topCard != null && computerAI != null) {
							Invoke (nameof (TriggerAISequenceDecision), 1.0f);
						} else {
							EndAITurnWithStrictFlow ();
						}
					} else {
						// Normal ChangeDirection handling
						TakiLogger.LogAI ("AI played CHANGE DIRECTION card - direction already changed");

						if (gameplayUI != null) {
							string directionMessage = $"Direction changed by AI: {gameState?.turnDirection}";
							GetActiveUI ()?.ShowSpecialCardEffect (CardType.ChangeDirection, PlayerType.Computer, directionMessage);
						}

						if (turnManager != null) {
							turnManager.EndTurn ();
						}
					}
					break;

				case CardType.ChangeColor:
					if (inTakiSequence && !isLastCardInSequence) {
						// ChangeColor during sequence - effect deferred
						TakiLogger.LogAI ("AI played CHANGE COLOR during TAKI sequence - effect DEFERRED, continuing sequence");

						CardData topCard = GetTopDiscardCard ();
						if (topCard != null && computerAI != null) {
							Invoke (nameof (TriggerAISequenceDecision), 1.0f);
						} else {
							EndAITurnWithStrictFlow ();
						}
					} else {
						// Normal ChangeColor handling
						TakiLogger.LogAI ("AI played CHANGE COLOR card - handling AI color selection");

						if (gameplayUI != null) {
							GetActiveUI ()?.ShowImmediateFeedback ("AI is selecting new color...", true);
						}

						if (computerAI != null) {
							CardColor selectedColor = computerAI.SelectColor ();
							gameState?.ChangeActiveColor (selectedColor);

							if (gameplayUI != null) {
								GetActiveUI ()?.ShowSpecialCardEffect (CardType.ChangeColor, PlayerType.Computer,
									$"New active color: {selectedColor}");
							}

							TakiLogger.LogAI ($"AI selected color: {selectedColor}");
						}

						if (turnManager != null) {
							turnManager.EndTurn ();
						}
					}
					break;

				case CardType.PlusTwo:
					// CRITICAL FIX: PlusTwo handling with proper sequence awareness
					if (inTakiSequence && !isLastCardInSequence) {
						// PlusTwo played during sequence - effect DEFERRED
						TakiLogger.LogAI ("AI played PLUS TWO during TAKI sequence - effect DEFERRED, continuing sequence");

						// Continue sequence - the PlusTwo effect will be processed when sequence ends
						CardData topCard = GetTopDiscardCard ();
						if (topCard != null && computerAI != null) {
							Invoke (nameof (TriggerAISequenceDecision), 1.0f);
						} else {
							TakiLogger.LogError ("Cannot continue AI sequence after PlusTwo - ending sequence", TakiLogger.LogCategory.AI);
							EndAITurnWithStrictFlow ();
						}
					} else {
						// Normal PlusTwo handling (not in sequence or last card in sequence)
						TakiLogger.LogAI ("AI played PLUS TWO card - chain logic already handled by main method");

						if (gameState.IsPlusTwoChainActive) {
							// Chain is now active - opponent must respond
							int chainCount = gameState.NumberOfChainedCards;
							int drawCount = gameState.ChainDrawCount;

							gameplayUI?.ShowSpecialCardEffect (CardType.PlusTwo, PlayerType.Computer,
								$"Chain continues! Draw {drawCount} or play PlusTwo");

							TakiLogger.LogAI ($"PlusTwo chain continues: {chainCount} cards, opponent draws {drawCount} or continues");
						} else {
							// This shouldn't happen if chain logic worked correctly
							TakiLogger.LogError ("AI played PlusTwo but no chain is active - this indicates a sequence processing issue!", TakiLogger.LogCategory.AI);
							gameplayUI?.ShowSpecialCardEffect (CardType.PlusTwo, PlayerType.Computer, "You draw 2 cards!");
						}

						// Normal turn end for AI after playing PlusTwo
						if (turnManager != null) {
							turnManager.EndTurn ();
						}
					}
					break;

				case CardType.Taki:
					// TAKI card during sequence - continue sequence
					TakiLogger.LogAI ("AI played TAKI card - handling sequence continuation");

					if (!gameState.IsInTakiSequence) {
						TakiLogger.LogWarning ("AI played TAKI but no sequence started - this indicates a problem", TakiLogger.LogCategory.AI);

						if (gameplayUI != null) {
							GetActiveUI ()?.ShowSpecialCardEffect (CardType.Taki, PlayerType.Computer,
								$"AI starts TAKI sequence for {card.color}");
						}
					} else {
						TakiLogger.LogAI ($"AI played TAKI card within {gameState.TakiSequenceColor} sequence");

						if (gameplayUI != null) {
							GetActiveUI ()?.ShowSpecialCardEffect (CardType.Taki, PlayerType.Computer,
								"AI continues TAKI sequence");
						}
					}

					// Continue sequence
					CardData currentTopCard = GetTopDiscardCard ();
					if (currentTopCard != null && computerAI != null) {
						Invoke (nameof (TriggerAISequenceDecision), 1.0f);
					} else {
						TakiLogger.LogError ("Cannot continue AI sequence - missing components", TakiLogger.LogCategory.AI);
						EndAITurnWithStrictFlow ();
					}
					break;

				case CardType.SuperTaki:
					// SuperTAKI card during sequence - continue sequence
					TakiLogger.LogAI ("AI played SUPER TAKI card - handling sequence continuation");

					if (!gameState.IsInTakiSequence) {
						TakiLogger.LogWarning ("AI played SuperTAKI but no sequence started - this indicates a problem", TakiLogger.LogCategory.AI);

						CardColor sequenceColor = gameState.activeColor;
						if (gameplayUI != null) {
							GetActiveUI ()?.ShowSpecialCardEffect (CardType.SuperTaki, PlayerType.Computer,
								$"AI starts SuperTAKI sequence for {sequenceColor}");
						}
					} else {
						TakiLogger.LogAI ($"AI played SuperTAKI card within {gameState.TakiSequenceColor} sequence");

						if (gameplayUI != null) {
							GetActiveUI ()?.ShowSpecialCardEffect (CardType.SuperTaki, PlayerType.Computer,
								"AI continues SuperTAKI sequence");
						}
					}

					// Continue sequence
					CardData currentTopCard2 = GetTopDiscardCard ();
					if (currentTopCard2 != null && computerAI != null) {
						Invoke (nameof (TriggerAISequenceDecision), 1.0f);
					} else {
						TakiLogger.LogError ("Cannot continue AI sequence - missing components", TakiLogger.LogCategory.AI);
						EndAITurnWithStrictFlow ();
					}
					break;

				default:
					// No special effects - normal turn end 
					TakiLogger.LogAI ("AI played normal card - ending turn normally");
					if (turnManager != null) {
						turnManager.EndTurn ();
					}
					break;
			}

			TakiLogger.LogAI ("=== AI TURN FLOW HANDLING COMPLETE ===");
		}

		/// <summary> 
		/// Trigger AI additional action for PLUS card effect
		/// </summary>
		void TriggerAIAdditionalAction () {
			TakiLogger.LogAI ("=== AI TAKING ADDITIONAL ACTION (PLUS effect) ===");

			CardData topCard = GetTopDiscardCard ();
			if (topCard != null && computerAI != null) {
				GetActiveUI ()?.ShowOpponentMessage ("Taking additional action...");
				computerAI.MakeDecision (topCard);
			} else {
				TakiLogger.LogError ("Cannot trigger AI additional action - missing components", TakiLogger.LogCategory.AI);
				// FIXED: Fallback uses new AI turn end method
				EndAITurnWithStrictFlow ();
			}
		}

		/// <summary>
		/// Helper method to trigger AI sequence decision - uses public AI interface
		/// </summary> 
		void TriggerAISequenceDecision () {
			TakiLogger.LogAI ("=== TRIGGERING AI SEQUENCE DECISION AFTER TAKI/SUPERTAKI ===");

			CardData sequenceTopCard = GetTopDiscardCard ();

			if (computerAI != null && computerAI.CanMakeDecisions () && sequenceTopCard != null) {
				// Use public MakeDecision() - it handles sequences internally
				computerAI.MakeDecision (sequenceTopCard);
				TakiLogger.LogAI ("AI sequence decision triggered via MakeDecision()");
			} else {
				TakiLogger.LogError ("Cannot trigger AI sequence decision", TakiLogger.LogCategory.AI);
				EndAITurnWithStrictFlow ();
			}
		}

		#endregion

		#region Network Multiplayer
		// Network game coordination and synchronization

		/// <summary>
		/// MILESTONE 1: Enhanced network card play processing
		/// Replaces the basic ProcessNetworkCardPlay() method
		/// </summary>
		public void ProcessNetworkCardPlay (string cardIdentifier, int remotePlayerActor) {
			TakiLogger.LogNetwork ($"Processing remote card play: {cardIdentifier} from actor {remotePlayerActor}");

			// Enhanced feedback for milestone 1
			GetActiveUI ()?.ShowOpponentAction ($"played {cardIdentifier}");
			GetActiveUI ()?.ShowOpponentMessage ($"Opponent played {cardIdentifier}");

			// MILESTONE 1: Update opponent hand count (simulate card removal)
			if (opponentHandManager != null && opponentHandManager.IsOpponentHand) {
				int currentCount = opponentHandManager.NetworkOpponentHandCount;
				if (currentCount > 0) {
					opponentHandManager.UpdateNetworkOpponentHandCount (currentCount - 1);
					TakiLogger.LogNetwork ($"Updated opponent hand count: {currentCount - 1}");
				}
			}

			// MILESTONE 1: Simulate discard pile update
			// TODO: In next milestone, parse cardIdentifier and update actual discard pile
			if (deckManager != null && deckManager.deckUI != null) {
				deckManager.ShowMessage ($"Opponent played {cardIdentifier}", true);
			}

			// Update UI for both players
			UpdateAllUIWithNetworkSupport ();

			TakiLogger.LogNetwork ("Remote card play processed");
		}

		/// <summary>
		/// MILESTONE 1: Enhanced network card draw processing  
		/// Replaces the basic ProcessNetworkCardDraw() method
		/// </summary>
		public void ProcessNetworkCardDraw (int remotePlayerActor) {
			TakiLogger.LogNetwork ($"Processing remote card draw from actor {remotePlayerActor}");

			// Enhanced feedback for milestone 1
			GetActiveUI ()?.ShowOpponentAction ("drew a card");
			GetActiveUI ()?.ShowOpponentMessage ("Opponent drew a card");

			// MILESTONE 1: Update opponent hand count (simulate card addition)
			if (opponentHandManager != null && opponentHandManager.IsOpponentHand) {
				int currentCount = opponentHandManager.NetworkOpponentHandCount;
				opponentHandManager.UpdateNetworkOpponentHandCount (currentCount + 1);
				TakiLogger.LogNetwork ($"Updated opponent hand count: {currentCount + 1}");
			}

			// MILESTONE 1: Simulate deck count update
			// TODO: In next milestone, update actual deck counts
			if (deckManager != null && deckManager.deckUI != null) {
				deckManager.ShowMessage ("Opponent drew a card", true);
			}

			// Update UI for both players
			UpdateAllUI ();

			TakiLogger.LogNetwork ("Remote card draw processed");
		}

		/// <summary>
		/// MILESTONE 1: Enhanced send local card play to network
		/// Includes hand count synchronization
		/// </summary>
		public void SendLocalCardPlayToNetwork (CardData cardToPlay) {
			if (networkGameManager != null && cardToPlay != null) {
				networkGameManager.SendCardPlay (cardToPlay);
				TakiLogger.LogNetwork ($"Sent card play to network: {cardToPlay.GetDisplayText ()}");

				// MILESTONE 1: Show immediate local feedback
				if (gameplayUI != null) {
					GetActiveUI ()?.ShowPlayerMessage ($"You played {cardToPlay.GetDisplayText ()}");
				}
			}
		}

		/// <summary>
		/// MILESTONE 1: Enhanced send local card draw to network
		/// Includes hand count synchronization
		/// </summary>
		public void SendLocalCardDrawToNetwork () {
			if (networkGameManager != null) {
				networkGameManager.SendCardDraw ();
				TakiLogger.LogNetwork ("Sent card draw to network");

				// MILESTONE 1: Show immediate local feedback
				if (gameplayUI != null) {
					GetActiveUI ()?.ShowPlayerMessage ("You drew a card");
				}
			}
		}

		/// <summary>
		/// MILESTONE 1: Enhanced multiplayer play card handler
		/// Includes network hand synchronization
		/// </summary>
		void OnPlayCardButtonClickedMultiplayer () {
			TakiLogger.LogNetwork ("=== MULTIPLAYER PLAY CARD CLICKED ===");

			// Check if it's our turn
			if (networkGameManager == null || !networkGameManager.IsMyTurn) {
				GetActiveUI ()?.ShowPlayerMessage ("Not your turn!");
				return;
			}

			// Get selected card using active hand manager
			CardData cardToPlay = GetActivePlayerHandManager ()?.GetSelectedCard ();
			if (cardToPlay != null) {
				// Send to network first
				SendLocalCardPlayToNetwork (cardToPlay);

				// Process locally using existing singleplayer logic
				// MILESTONE 1: This preserves all game rules and special card logic
				PlayCardWithStrictFlow (cardToPlay);

				TakiLogger.LogNetwork ($"Multiplayer card play completed: {cardToPlay.GetDisplayText ()}");
			} else {
				GetActiveUI ()?.ShowPlayerMessage ("Please select a card!");
			}
		}

		/// <summary>
		/// MILESTONE 1: Enhanced multiplayer draw card handler
		/// Includes network hand synchronization
		/// </summary>
		void OnDrawCardButtonClickedMultiplayer () {
			TakiLogger.LogNetwork ("=== MULTIPLAYER DRAW CARD CLICKED ===");

			// Check if it's our turn
			if (networkGameManager == null || !networkGameManager.IsMyTurn) {
				GetActiveUI ()?.ShowPlayerMessage ("Not your turn!");
				return;
			}

			// Send to network first
			SendLocalCardDrawToNetwork ();

			// Process locally using existing singleplayer logic
			// MILESTONE 1: This preserves all game rules and flow
			DrawCardWithStrictFlow ();

			TakiLogger.LogNetwork ("Multiplayer card draw completed");
		}

		/// <summary>
		/// MILESTONE 1: Enhanced UpdateAllUI with network hand synchronization
		/// FIXED: Respects network privacy when updating UI
		/// </summary>
		public void UpdateAllUIWithNetworkSupport () {
			TakiLogger.LogNetwork ("Updating all UI with network support");

			// MILESTONE 1 FIX: Respect network privacy when updating UI
			if (isMultiplayerMode && opponentHandManager != null && opponentHandManager.IsOpponentHand) {
				// For opponent hands in multiplayer, don't override the privacy display
				// Only update UI elements that don't conflict with card back display
				TakiLogger.LogNetwork ("Network mode: Updating UI while preserving opponent hand privacy");

				if (gameplayUI != null) {
					// Safe UI updates that don't interfere with hand displays
					// FIXED: Get TurnState from gameState, not from turnManager.CurrentPlayer
					TurnState currentTurnState = gameState?.turnState ?? TurnState.Neutral;
					GetActiveUI ()?.UpdateTurnDisplay (currentTurnState);
					GetActiveUI ()?.UpdateActiveColorDisplay (gameState?.activeColor ?? CardColor.Wild);

					// Update hand size displays without touching hand cards
					int localHandSize = playerHand?.Count ?? 0;
					int opponentHandSize = opponentHandManager?.NetworkOpponentHandCount ?? 0;
					GetActiveUI ()?.UpdateHandSizeDisplay (localHandSize, opponentHandSize);

					TakiLogger.LogNetwork ($"UI updated safely: Local={localHandSize}, Opponent={opponentHandSize}");
				}

				// Don't call UpdateAllUI() - it would override the privacy system
			} else {
				// Normal UI updates for singleplayer or own hand
				TakiLogger.LogNetwork ("Standard mode: Full UI update");
				UpdateAllUI ();
			}

			// MILESTONE 1: Additional network-specific UI updates
			if (isMultiplayerMode && networkGameManager != null) {
				// Update turn display for multiplayer
				GetActiveUI ()?.UpdateTurnDisplayMultiplayer (networkGameManager.IsMyTurn);

				// Sync hand counts for network display (already done above)
				SynchronizeNetworkHandCounts ();
			}
		}

		/// <summary>
		/// MILESTONE 1: Synchronize hand counts for network display
		/// </summary>
		void SynchronizeNetworkHandCounts () {
			if (!isMultiplayerMode) return;

			// Update local player hand count display
			if (playerHandManager != null && gameplayUI != null) {
				int localHandSize = playerHand.Count;
				GetActiveUI ()?.UpdateHandSizeDisplay (localHandSize, opponentHandManager?.NetworkOpponentHandCount ?? 0);
			}

			// Ensure opponent hand count is properly displayed
			if (opponentHandManager != null && opponentHandManager.IsOpponentHand) {
				// Hand count should already be updated by NetworkGameManager
				// This ensures UI consistency
				TakiLogger.LogNetwork ($"Network hand count sync: Local={playerHand.Count}, Opponent={opponentHandManager.NetworkOpponentHandCount}");
			}
		}

		/// <summary>
		/// MILESTONE 1: Check if multiplayer game is ready for actions
		/// </summary>
		public bool IsMultiplayerGameReady () {
			if (!isMultiplayerMode || networkGameManager == null) {
				return false;
			}

			return networkGameManager.IsDeckInitialized && networkGameManager.IsNetworkGameActive;
		}

		/// <summary>
		/// MILESTONE 1: Get network game status for debugging
		/// </summary>
		public string GetNetworkGameStatus () {
			if (!isMultiplayerMode || networkGameManager == null) {
				return "Not in multiplayer mode";
			}

			return $"Network Game - MyTurn: {networkGameManager.IsMyTurn}, DeckReady: {networkGameManager.IsDeckInitialized}, Active: {networkGameManager.IsNetworkGameActive}";
		}

		#endregion

		#region Game State Events
		// Event handlers for game state changes

		/// <summary>
		/// UPDATED: OnTurnStateChanged - Remove STOP processing (now handled earlier)
		/// </summary>
		void OnTurnStateChanged (TurnState newTurnState) {
			TakiLogger.LogGameState ($"Turn state changed to {newTurnState}");

			// Normal turn flow processing
			if (gameplayUI != null) {
				GetActiveUI ()?.UpdateTurnDisplay (newTurnState);
			}

			// Start strict flow control for player turns
			if (newTurnState == TurnState.PlayerTurn) {
				Invoke (nameof (StartPlayerTurnFlow), 0.1f); // Small delay to ensure UI is ready
			}

			// Refresh playable cards for player hand
			if (newTurnState == TurnState.PlayerTurn && playerHandManager != null) {
				Invoke (nameof (RefreshPlayerHandStates), 0.1f);
			}
		}

		void OnInteractionStateChanged (InteractionState newInteractionState) {
			if (gameplayUI != null) {
				GetActiveUI ()?.ShowColorSelection (newInteractionState == InteractionState.ColorSelection);
			}
		}

		void OnGameStatusChanged (GameStatus newGameStatus) {
			UpdateAllUI ();
		}

		void OnActiveColorChanged (CardColor newColor) {
			if (gameplayUI != null) {
				GetActiveUI ()?.UpdateActiveColorDisplay (newColor);
			}
		}

		void OnTurnChanged (PlayerType player) {
			OnTurnStarted?.Invoke (player);
		}

		void OnGameWon (PlayerType winner) {
			TakiLogger.LogSystem ($"GameManager: Game won by {winner}");

			// Process through GameEndManager instead of direct UI
			if (gameEndManager != null) {
				gameEndManager.ProcessGameEnd (winner);
			} else {
				// Fallback to existing logic if GameEndManager not available
				if (gameplayUI != null) {
					GetActiveUI ()?.ShowWinnerAnnouncement (winner);
				}
			}

			OnGameEnded?.Invoke (winner);
		}

		void OnPlayerTurnTimeOut (PlayerType player) {
			if (player == PlayerType.Human) {
				DrawCardWithStrictFlow ();
			}
		}

		void OnCardDrawnFromDeck (CardData card) {
			// Update UI when cards are drawn
			// (Already handled in specific draw methods)
		}

		/// <summary>
		/// PHASE 8B: Handle TAKI sequence started event
		/// </summary>
		/// <param name="sequenceColor">Color of the sequence</param>
		/// <param name="initiator">Who started the sequence</param>
		void OnTakiSequenceStarted (CardColor sequenceColor, PlayerType initiator) {
			TakiLogger.LogGameState ($"TAKI sequence started: {sequenceColor} by {initiator}");

			// Update UI to show sequence status
			if (gameplayUI != null) {
				gameplayUI.ShowTakiSequenceStatus (sequenceColor, 1, initiator == PlayerType.Human);
				GetActiveUI ()?.ShowSequenceProgressMessage (sequenceColor, 1, initiator);
			}
		}

		/// <summary>
		/// PHASE 8B: Handle card added to TAKI sequence event
		/// </summary>
		/// <param name="cardCount">New card count in sequence</param>
		void OnTakiSequenceCardAdded (int cardCount) {
			TakiLogger.LogGameState ($"Card added to TAKI sequence: now {cardCount} cards");

			// Update UI to show updated sequence status
			if (gameplayUI != null && gameState != null) {
				bool isPlayerTurn = gameState.IsPlayerTurn;
				CardColor sequenceColor = gameState.TakiSequenceColor;
				GetActiveUI ()?.ShowTakiSequenceStatus (sequenceColor, cardCount, isPlayerTurn);
			}
		}

		/// <summary> 
		/// PHASE 8B: Handle TAKI sequence ended event
		/// </summary> 
		void OnTakiSequenceEnded () {
			TakiLogger.LogGameState ("TAKI sequence ended");

			// Update UI to hide sequence status
			if (gameplayUI != null) {
				GetActiveUI ()?.HideTakiSequenceStatus ();
				gameplayUI.EnableEndTakiSequenceButton (false);
			}
		}

		/// <summary>
		/// FIXED: Process STOP card skip when turn is about to start
		/// </summary>
		/// <param name="turnStateToSkip">Turn state that should be skipped</param>
		void ProcessStopSkip (TurnState turnStateToSkip) {
			TakiLogger.LogTurnFlow ("=== PROCESSING STOP SKIP ===");

			PlayerType skippedPlayer = turnStateToSkip == TurnState.PlayerTurn ? PlayerType.Human : PlayerType.Computer;
			PlayerType benefitPlayer = stopCardPlayer; // Player who played STOP gets the benefit

			TakiLogger.LogTurnFlow ($"Skipping {skippedPlayer} turn due to STOP played by {stopCardPlayer}");

			// Show skip messages
			string skipMessage = skippedPlayer == PlayerType.Human ?
				"Your turn was skipped by STOP card!" :
				"Computer's turn was skipped by STOP card!";

			string benefitMessage = benefitPlayer == PlayerType.Human ?
				"STOP effect: You get another turn!" :
				"STOP effect: Computer gets another turn!";

			GetActiveUI ()?.ShowPlayerMessage (skipMessage);
			GetActiveUI ()?.ShowOpponentMessage (benefitMessage);

			// Reset the flag - skip has been processed
			shouldSkipNextTurn = false;
			TakiLogger.LogTurnFlow ("STOP flag reset - skip processed");

			// Start the turn for the player who benefits (played the STOP card)
			TurnState benefitTurnState = benefitPlayer == PlayerType.Human ? TurnState.PlayerTurn : TurnState.ComputerTurn;

			TakiLogger.LogTurnFlow ($"Starting turn for STOP benefit player: {benefitPlayer}");

			// Use TurnManager to properly start the benefit player's turn
			if (turnManager != null) {
				turnManager.StartTurn (benefitPlayer);
			}
		}

		#endregion

		#region UI and Visual Updates
		// UI synchronization and visual card management

		/// <summary>
		/// Update all UI with forced button state sync
		/// </summary>
		void UpdateAllUI () {
			if (gameplayUI != null && gameState != null) {
				// Update hand sizes
				int computerHandSize = computerAI?.HandSize ?? 0;
				GetActiveUI ()?.UpdateHandSizeDisplay (playerHand.Count, computerHandSize);

				// Update all displays using new architecture
				GetActiveUI ()?.UpdateAllDisplays (
					gameState.turnState,
					gameState.gameStatus,
					gameState.interactionState,
					gameState.activeColor
		);

				// CHAIN UI INTEGRATION: Update chain status if active
				if (gameState.IsPlusTwoChainActive) {
					bool isPlayerTurn = gameState.IsPlayerTurn;
					GetActiveUI ()?.ShowPlusTwoChainStatus (
						gameState.NumberOfChainedCards,
						gameState.ChainDrawCount,
						isPlayerTurn
					);
				} else {
					GetActiveUI ()?.HidePlusTwoChainStatus ();
				}
			}

			// Update visual card displays 
			UpdateVisualHands ();
		}

		/// <summary>
		/// Update visual hands with error checking
		/// ENHANCED: Uses per-screen HandManager architecture when enabled
		/// </summary>
		void UpdateVisualHands () {
			try {
				// Get active hand managers (supports both legacy and per-screen architecture)
				HandManager activePlayerHandManager = GetActivePlayerHandManager ();
				HandManager activeOpponentHandManager = GetActiveOpponentHandManager ();

				// Update player hand visual
				if (activePlayerHandManager != null && playerHand != null) {
					activePlayerHandManager.UpdateHandDisplay (playerHand);
					TakiLogger.LogUI ($"Updated player hand display: {playerHand.Count} cards", TakiLogger.LogLevel.Trace);
				}

				// Update opponent hand visual
				if (activeOpponentHandManager != null && computerAI != null) {
					List<CardData> computerHand = computerAI.GetHandCopy ();
					activeOpponentHandManager.UpdateHandDisplay (computerHand);
					TakiLogger.LogUI ($"Updated opponent hand display: {computerHand.Count} cards", TakiLogger.LogLevel.Trace);
				}
			} catch (System.Exception e) {
				TakiLogger.LogError ($"Error updating visual hands: {e.Message}", TakiLogger.LogCategory.UI);
			}
		}

		/// <summary>
		/// Force refresh of player hand playable states (delayed)
		/// </summary>
		void RefreshPlayerHandStates () {
			TakiLogger.LogUI ("REFRESHING PLAYER HAND STATES");

			HandManager activePlayerHandManager = GetActivePlayerHandManager ();
			if (activePlayerHandManager != null) {
				activePlayerHandManager.RefreshPlayableStates ();
			}
		}

		/// <summary>
		/// Handle player card selection from visual hand
		/// </summary>
		/// <param name="selectedCardController">Selected card controller</param>
		void OnPlayerCardSelected (CardController selectedCardController) {
			if (selectedCardController == null) {
				TakiLogger.LogUI ("Player deselected card");
				return;
			}

			CardData selectedCard = selectedCardController.CardData;
			if (selectedCard != null) {
				TakiLogger.LogUI ($"Player selected visual card: {selectedCard.GetDisplayText ()}");

				// Update UI feedback
				if (gameplayUI != null) {
					bool canPlay = gameState?.IsValidMove (selectedCard, GetTopDiscardCard ()) ?? false;
					string message = canPlay ? $"Selected: {selectedCard.GetDisplayText ()}" : "Invalid move!";
					GetActiveUI ()?.ShowPlayerMessage (message);
				}
			}
		}

		/// <summary>
		/// Handle computer card selection (should not happen)
		/// </summary>
		void OnComputerCardSelected (CardController selectedCardController) {
			TakiLogger.LogWarning ("Computer cards should not be selectable!", TakiLogger.LogCategory.UI);
		}

		#endregion

		#region External System Coordination
		// Integration with pause, game end, and menu systems

		/// <summary>
		/// Handle game paused event
		/// </summary>
		void OnGamePaused () {
			TakiLogger.LogSystem ("GameManager: Game paused");
			// Additional pause handling if needed
		}

		/// <summary>
		/// Handle game resumed event
		/// </summary>
		void OnGameResumed () {
			TakiLogger.LogSystem ("GameManager: Game resumed");

			// Refresh UI state after resume
			UpdateAllUI ();

			// Turn flow state is restored by PauseManager - don't interfere!
			// The exact button states and turn progress should be preserved
			TakiLogger.LogTurnFlow ("Turn flow state restored by PauseManager - no reset needed");
		}

		/// <summary>
		/// Handle game end processed event
		/// </summary>
		/// <param name="winner">The winning player</param>
		void OnGameEndProcessed (PlayerType winner) {
			TakiLogger.LogSystem ($"GameManager: Game end processed - Winner: {winner}");
			// Additional game end handling if needed
		}

		/// <summary>
		/// Handle game restarted event
		/// </summary>
		void OnGameRestarted () {
			TakiLogger.LogSystem ("GameManager: Game restarted");
			// Game restart already handled by StartNewSinglePlayerGame()
		}

		/// <summary>
		/// Handle returned to menu event
		/// </summary>
		void OnReturnedToMenu () {
			TakiLogger.LogSystem ("GameManager: Returned to main menu");
			// Cleanup handled by GameEndManager
		}

		/// <summary>
		/// Handle exit validation shown event
		/// </summary>
		void OnExitValidationShown () {
			TakiLogger.LogSystem ("GameManager: Exit validation shown");
			// Game automatically paused by ExitValidationManager
		}

		/// <summary>
		/// Handle exit validation cancelled event
		/// </summary>
		void OnExitValidationCancelled () {
			TakiLogger.LogSystem ("GameManager: Exit validation cancelled");
			// Game automatically resumed by ExitValidationManager
		}

		/// <summary>
		/// Handle exit confirmed event
		/// </summary>
		void OnExitConfirmed () {
			TakiLogger.LogSystem ("GameManager: Exit confirmed");
			// Application will exit through MenuNavigation
		}

		#endregion

		#region State Preservation
		// Pause/resume state management

		/// <summary>
		/// Capture current turn flow state for pause preservation
		/// </summary>
		/// <returns>Turn flow state snapshot</returns>
		public PauseManager.GameManagerTurnFlowSnapshot CaptureTurnFlowState () {
			var snapshot = new PauseManager.GameManagerTurnFlowSnapshot {
				hasPlayerTakenAction = this.hasPlayerTakenAction,
				canPlayerDraw = this.canPlayerDraw,
				canPlayerPlay = this.canPlayerPlay,
				canPlayerEndTurn = this.canPlayerEndTurn
			};

			TakiLogger.LogTurnFlow ($"Turn flow state captured: Action={snapshot.hasPlayerTakenAction}, Play={snapshot.canPlayerPlay}, Draw={snapshot.canPlayerDraw}, EndTurn={snapshot.canPlayerEndTurn}");
			return snapshot;
		}

		/// <summary> 
		/// Restore turn flow state from pause snapshot
		/// </summary>
		/// <param name="snapshot">Previously captured turn flow state</param>
		public void RestoreTurnFlowState (PauseManager.GameManagerTurnFlowSnapshot snapshot) {
			if (snapshot == null) {
				TakiLogger.LogError ("Cannot restore turn flow state: snapshot is null", TakiLogger.LogCategory.TurnFlow);
				return;
			}

			TakiLogger.LogTurnFlow ("=== RESTORING TURN FLOW STATE FROM PAUSE ===");
			TakiLogger.LogTurnFlow ($"Restoring: Action={snapshot.hasPlayerTakenAction}, Play={snapshot.canPlayerPlay}, Draw={snapshot.canPlayerDraw}, EndTurn={snapshot.canPlayerEndTurn}");

			// Restore exact state
			this.hasPlayerTakenAction = snapshot.hasPlayerTakenAction;
			this.canPlayerDraw = snapshot.canPlayerDraw;
			this.canPlayerPlay = snapshot.canPlayerPlay;
			this.canPlayerEndTurn = snapshot.canPlayerEndTurn;

			// Update UI to reflect restored button states
			var activeUI = GetActiveUI ();
			if (activeUI != null && gameState != null && gameState.IsPlayerTurn) {
				activeUI.UpdateStrictButtonStates (
					snapshot.canPlayerPlay,
					snapshot.canPlayerDraw,
					snapshot.canPlayerEndTurn
				);

				TakiLogger.LogTurnFlow ("Button states restored to match captured state");
			}

			TakiLogger.LogTurnFlow ("Turn flow state fully restored");
		}

		#endregion

		#region Public API
		// External interface methods for other systems

		/// <summary>
		/// Request pause game - delegates to PauseManager
		/// </summary>
		public void RequestPauseGame () {
			TakiLogger.LogSystem ("GameManager: Pause game requested");
			if (pauseManager != null) {
				pauseManager.PauseGame ();
			} else {
				TakiLogger.LogError ("Cannot pause: PauseManager not assigned", TakiLogger.LogCategory.System);
			}
		}

		/// <summary>
		/// Request resume game - delegates to PauseManager
		/// </summary>
		public void RequestResumeGame () {
			TakiLogger.LogSystem ("GameManager: Resume game requested");
			if (pauseManager != null) {
				pauseManager.ResumeGame ();
			} else {
				TakiLogger.LogError ("Cannot resume: PauseManager not assigned", TakiLogger.LogCategory.System);
			}
		}

		/// <summary>
		/// Request restart game - delegates to GameEndManager (for game-end scenarios)
		/// </summary>
		public void RequestRestartGame () {
			TakiLogger.LogSystem ("GameManager: Restart game requested FROM GAME END");
			if (gameEndManager != null) {
				gameEndManager.OnRestartButtonClicked ();
			} else {
				TakiLogger.LogError ("Cannot restart: GameEndManager not assigned", TakiLogger.LogCategory.System);
			}
		}

		/// <summary> 
		/// Request restart game from pause - bypasses GameEndManager
		/// This is different from RequestRestartGame() which expects a game-end state
		/// </summary>
		public void RequestRestartGameFromPause () {
			TakiLogger.LogSystem ("GameManager: Restart game requested FROM PAUSE");

			// For pause restarts, we don't need GameEndManager
			// Just start a fresh game directly
			StartNewSinglePlayerGame ();

			TakiLogger.LogSystem ("GameManager: New game started from pause restart");
		}

		/// <summary>
		/// Request return to menu - delegates to GameEndManager
		/// </summary>
		public void RequestReturnToMenu () {
			TakiLogger.LogSystem ("GameManager: Return to menu requested");
			if (gameEndManager != null) {
				gameEndManager.OnGoHomeButtonClicked ();
			} else {
				TakiLogger.LogError ("Cannot return to menu: GameEndManager not assigned", TakiLogger.LogCategory.System);
			}
		}

		/// <summary>
		/// Request exit confirmation - delegates to ExitValidationManager
		/// </summary>
		public void RequestExitConfirmation () {
			TakiLogger.LogSystem ("GameManager: Exit confirmation requested");
			if (exitValidationManager != null) {
				exitValidationManager.ShowExitConfirmation ();
			} else {
				TakiLogger.LogError ("Cannot show exit confirmation: ExitValidationManager not assigned", TakiLogger.LogCategory.System);
			}
		}

		public void RequestDrawCard () => OnDrawCardButtonClicked ();
		public void RequestPlayCard (CardData card) => PlayCardWithStrictFlow (card);
		public List<CardData> GetPlayerHand () => new List<CardData> (playerHand);
		public bool CanPlayerAct () => gameState?.CanPlayerAct () ?? false;

		#endregion

		#region Debug and Development
		// Debug methods, context menus, and development tools

		/// <summary>
		/// MILESTONE 1: Debug network hand state
		/// </summary>
		[ContextMenu ("Debug Network Hand State")]
		public void DebugNetworkHandState () {
			TakiLogger.LogDiagnostics ("=== NETWORK HAND STATE DEBUG ===");
			TakiLogger.LogDiagnostics ($"Multiplayer Mode: {isMultiplayerMode}");
			TakiLogger.LogDiagnostics ($"Local Hand Size: {playerHand.Count}");

			if (playerHandManager != null) {
				TakiLogger.LogDiagnostics ($"Player Hand Manager - Network: {playerHandManager.IsNetworkGame}, Size: {playerHandManager.HandSize}");
			}

			if (opponentHandManager != null) {
				TakiLogger.LogDiagnostics ($"Computer Hand Manager - Network: {opponentHandManager.IsNetworkGame}, Opponent: {opponentHandManager.IsOpponentHand}");
				TakiLogger.LogDiagnostics ($"Opponent Hand Count: {opponentHandManager.NetworkOpponentHandCount}");
			}

			if (networkGameManager != null) {
				TakiLogger.LogDiagnostics ($"Network Game - MyTurn: {networkGameManager.IsMyTurn}, Ready: {networkGameManager.IsDeckInitialized}");
			}
		}

		/// <summary>
		/// MILESTONE 1: Debug deck state in network mode
		/// </summary>
		[ContextMenu ("Debug Network Deck State")]
		public void DebugNetworkDeckState () {
			TakiLogger.LogDiagnostics ("=== NETWORK DECK STATE DEBUG ===");

			if (deckManager != null) {
				TakiLogger.LogDiagnostics ($"Deck Manager - Network Mode: {deckManager.IsNetworkMode}");
				TakiLogger.LogDiagnostics ($"Draw Pile: {deckManager.DrawPileCount}, Discard Pile: {deckManager.DiscardPileCount}");

				CardData topCard = deckManager.GetTopDiscardCard ();
				TakiLogger.LogDiagnostics ($"Top Discard Card: {topCard?.GetDisplayText () ?? "None"}");
			}

			if (networkGameManager != null) {
				TakiLogger.LogDiagnostics (GetNetworkGameStatus ());
			}
		}

		/// <summary>
		/// MILESTONE 1: Force network hand synchronization for debugging
		/// </summary>
		[ContextMenu ("Force Network Hand Sync")]
		public void ForceNetworkHandSync () {
			TakiLogger.LogDiagnostics ("=== FORCING NETWORK HAND SYNC ===");
			SynchronizeNetworkHandCounts ();
			UpdateAllUIWithNetworkSupport ();
		}

		[ContextMenu ("Debug TAKI Sequence State")]
		public void DebugTakiSequenceState () {
			TakiLogger.LogDiagnostics ("=== TAKI SEQUENCE STATE DEBUG ===");

			if (gameState != null) {
				TakiLogger.LogDiagnostics ($"In Sequence: {gameState.IsInTakiSequence}");
				TakiLogger.LogDiagnostics ($"Sequence Color: {gameState.TakiSequenceColor}");
				TakiLogger.LogDiagnostics ($"Sequence Cards: {gameState.NumberOfSequenceCards}");
				TakiLogger.LogDiagnostics ($"Sequence Initiator: {gameState.TakiSequenceInitiator}");
				TakiLogger.LogDiagnostics ($"Current Turn: {gameState.turnState}");
				TakiLogger.LogDiagnostics ($"Interaction State: {gameState.interactionState}");

				if (gameState.LastCardPlayedInSequence != null) {
					TakiLogger.LogDiagnostics ($"Last Sequence Card: {gameState.LastCardPlayedInSequence.GetDisplayText ()}");
				} else {
					TakiLogger.LogDiagnostics ("Last Sequence Card: NULL");
				}
			} else {
				TakiLogger.LogDiagnostics ("GameState is NULL!");
			}

			// Check button state 
			if (gameplayUI != null) {
				bool buttonEnabled = GetActiveUI ()?.EndTakiSequenceButtonEnabled ?? false;
				TakiLogger.LogDiagnostics ($"End Sequence Button Enabled: {buttonEnabled}");
			}

			TakiLogger.LogDiagnostics ("=== END SEQUENCE DEBUG ===");
		}

		/// <summary>
		/// Debug method to check what type of card is selected
		/// </summary>
		[ContextMenu ("Debug Selected Card Type")]
		public void DebugSelectedCardType () {
			if (playerHandManager != null) {
				CardData selectedCard = playerHandManager.GetSelectedCard ();
				if (selectedCard != null) {
					TakiLogger.LogDiagnostics ($"=== SELECTED CARD DEBUG ===");
					TakiLogger.LogDiagnostics ($"Card display: {selectedCard.GetDisplayText ()}");
					TakiLogger.LogDiagnostics ($"Card type: {selectedCard.cardType}");
					TakiLogger.LogDiagnostics ($"Card name: {selectedCard.cardName}");
					TakiLogger.LogDiagnostics ($"Card color: {selectedCard.color}");
					TakiLogger.LogDiagnostics ($"Is special card: {selectedCard.IsSpecialCard}");
					TakiLogger.LogDiagnostics ($"Card type == CardType.Stop: {selectedCard.cardType == CardType.Stop}");
				} else {
					TakiLogger.LogDiagnostics ("No card selected");
				}
			} else {
				TakiLogger.LogDiagnostics ("PlayerHandManager is null");
			}
		}

		/// <summary>
		/// DEBUGGING: Force a new game start for testing
		/// </summary>
		[ContextMenu ("Force New Game Start")]
		public void ForceNewGameStart () {
			TakiLogger.LogDiagnostics ("FORCING NEW GAME START");

			if (!areSystemsInitialized) {
				TakiLogger.LogDiagnostics ("Initializing systems...");
				InitializeSinglePlayerSystems ();
			}

			TakiLogger.LogDiagnostics ("Starting new game...");
			StartNewSinglePlayerGame ();
		}

		[ContextMenu ("Log Turn Flow State")]
		public void LogTurnFlowState () {
			TakiLogger.LogDiagnostics ("TURN FLOW STATE DEBUG");
			TakiLogger.LogDiagnostics ($"hasPlayerTakenAction: {hasPlayerTakenAction}");
			TakiLogger.LogDiagnostics ($"canPlayerDraw: {canPlayerDraw}");
			TakiLogger.LogDiagnostics ($"canPlayerPlay: {canPlayerPlay}");
			TakiLogger.LogDiagnostics ($"canPlayerEndTurn: {canPlayerEndTurn}");
			TakiLogger.LogDiagnostics ($"isGameActive: {isGameActive}");
		}

		/// <summary>
		/// PHASE 7: Debug method to log special card state
		/// </summary>
		[ContextMenu ("Log Special Card State")]
		public void LogSpecialCardState () {
			TakiLogger.LogDiagnostics ("=== PHASE 7: SPECIAL CARD STATE DEBUG ===");
			TakiLogger.LogDiagnostics ($"Waiting for additional action: {isWaitingForAdditionalAction}");
			TakiLogger.LogDiagnostics ($"Active special card effect: {activeSpecialCardEffect}");
			TakiLogger.LogDiagnostics ($"Has pending effects: {HasPendingSpecialCardEffects ()}");

			// ADDED: STOP flag debugging
			TakiLogger.LogDiagnostics ($"STOP flag set: {shouldSkipNextTurn}");
			TakiLogger.LogDiagnostics ($"STOP played by: {stopCardPlayer}");

			TakiLogger.LogDiagnostics ($"State description: {GetSpecialCardStateDescription ()}");
		}

		/// <summary>
		/// Debug method to force reset STOP flag
		/// </summary> 
		[ContextMenu ("Force Reset STOP Flag")]
		public void ForceResetStopFlag () {
			TakiLogger.LogDiagnostics ("FORCE RESETTING STOP FLAG");
			shouldSkipNextTurn = false;
			TakiLogger.LogDiagnostics ($"STOP flag now: {shouldSkipNextTurn}");
		}

		/// <summary>
		/// DEBUGGING: Method to test STOP card functionality
		/// </summary>
		[ContextMenu ("Test STOP Card Effect")]
		public void TestStopCardEffect () {
			TakiLogger.LogDiagnostics ("=== TESTING STOP CARD EFFECT ===");

			// Simulate STOP card being played
			shouldSkipNextTurn = true;
			stopCardPlayer = PlayerType.Human;

			TakiLogger.LogDiagnostics ($"STOP flag set: {shouldSkipNextTurn}");
			TakiLogger.LogDiagnostics ($"STOP player: {stopCardPlayer}");

			GetActiveUI ()?.ShowPlayerMessage ("TEST: STOP flag set - now click END TURN");
		}

		/// <summary> 
		/// DEBUGGING: Method to check STOP flag state
		/// </summary>
		[ContextMenu ("Check STOP Flag State")]
		public void CheckStopFlagState () {
			TakiLogger.LogDiagnostics ("=== STOP FLAG STATE CHECK ===");
			TakiLogger.LogDiagnostics ($"shouldSkipNextTurn: {shouldSkipNextTurn}");
			TakiLogger.LogDiagnostics ($"stopCardPlayer: {stopCardPlayer}");
			TakiLogger.LogDiagnostics ($"Current turn state: {gameState?.turnState}");
			TakiLogger.LogDiagnostics ($"Can player end turn: {canPlayerEndTurn}");
		}

		/// <summary>
		/// PHASE 7: Debug method to force reset special card state
		/// </summary>
		[ContextMenu ("Force Reset Special Card State")]
		public void ForceResetSpecialCardState () {
			TakiLogger.LogDiagnostics ("FORCE RESETTING SPECIAL CARD STATE");
			ResetSpecialCardState ();
			LogSpecialCardState ();
		}

		/// <summary>
		/// DEBUGGING: Manual turn trigger for testing
		/// </summary>
		[ContextMenu ("Trigger Computer Turn")]
		public void TriggerComputerTurnManually () {
			TakiLogger.LogDiagnostics ("MANUAL COMPUTER TURN TRIGGER");
			OnComputerTurnReady ();
		}

		/// <summary>
		/// Force UI sync - call this to fix button states after manual testing
		/// </summary>
		[ContextMenu ("Force UI Sync")]
		public void ForceUISync () {
			TakiLogger.LogDiagnostics ("FORCING UI SYNCHRONIZATION");

			if (gameState == null || gameplayUI == null) {
				TakiLogger.LogError ("Cannot sync UI - missing components", TakiLogger.LogCategory.System);
				return;
			}

			// Force button state update based on current turn state
			bool shouldEnableButtons = gameState.CanPlayerAct ();
			GetActiveUI ()?.UpdateButtonStates (shouldEnableButtons);

			TakiLogger.LogDiagnostics ($"UI synced - Turn: {gameState.turnState}, Buttons enabled: {shouldEnableButtons}");
		}

		#endregion
	}
}