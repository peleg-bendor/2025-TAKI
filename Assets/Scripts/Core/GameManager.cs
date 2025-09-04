using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// Main coordinator - Separate initialization from game start
	/// No longer initializes game systems until player chooses game mode
	/// ENHANCED GameManager with STRICT TURN FLOW SYSTEM
	/// - Player must take ONE action (PLAY or DRAW) then END TURN
	/// - END TURN disabled until action taken
	/// - DRAW disabled after playing card
	/// - All special cards logged but act as basic cards for now
	/// </summary>
	public class GameManager : MonoBehaviour {

		[Header ("Component References")]
		[Tooltip ("Manages game state using multi-enum architecture")]
		public GameStateManager gameState;

		[Tooltip ("Manages turns and player switching")]
		public TurnManager turnManager;

		[Tooltip ("Computer AI decision making")]
		public BasicComputerAI computerAI;

		[Tooltip ("Gameplay UI updates - owns turn, player actions, color selection")]
		public GameplayUIManager gameplayUI;

		[Tooltip ("Deck management system")]
		public DeckManager deckManager;

		[Header ("Game Setup")]
		[Tooltip ("Starting player for new games")]
		public PlayerType startingPlayer = PlayerType.Human;

		[Tooltip ("Player's hand of cards (Player1 = Human)")]
		public List<CardData> playerHand = new List<CardData> ();

		[Header ("Visual Card System")]
		[Tooltip ("Hand manager for player cards (Player1HandPanel)")]
		public HandManager playerHandManager;

		[Tooltip ("Hand manager for computer cards (Player2HandPanel)")]
		public HandManager computerHandManager;

		[Header ("Phase 7: Special Card State")]
		[Tooltip ("Track if player is waiting for additional action after Plus card")]
		private bool isWaitingForAdditionalAction = false;

		[Header ("STOP Card State")]
		[Tooltip ("Flag to track if next turn should be skipped")]
		private bool shouldSkipNextTurn = false;

		[Tooltip ("Player who played the STOP card (for messaging)")]
		private PlayerType stopCardPlayer = PlayerType.Human;

		[Tooltip ("Track which special card effect is active")]
		private CardType activeSpecialCardEffect = CardType.Number; // Number = no special effect

		[Header ("Logging Configuration")]
		[Tooltip ("Log level for console output")]
		public TakiLogger.LogLevel logLevel = TakiLogger.LogLevel.Info;

		[Tooltip ("Enable production mode (minimal logging)")]
		public bool productionMode = false;

		[Header ("Game Flow Managers")]
		[Tooltip ("Manages pause/resume functionality")]
		public PauseManager pauseManager;

		[Tooltip ("Manages game end scenarios")]
		public GameEndManager gameEndManager;

		[Tooltip ("Manages exit confirmation")]
		public ExitValidationManager exitValidationManager;

		// ENHANCED: Turn flow control state
		[Header ("Turn Flow Control")]
		private bool hasPlayerTakenAction = false;
		private bool canPlayerDraw = true;
		private bool canPlayerPlay = true;
		private bool canPlayerEndTurn = false;

		// Events for external systems
		public System.Action OnGameStarted;
		public System.Action<PlayerType> OnGameEnded;
		public System.Action<PlayerType> OnTurnStarted;
		public System.Action<CardData> OnCardPlayed;

		// System state - More granular state tracking
		private bool areComponentsValidated = false;
		private bool areSystemsInitialized = false;
		private bool isGameActive = false;

		void Start () {
			// Configure logging system
			ConfigureLogging ();

			// ONLY validate components exist - DON'T initialize game systems yet
			ValidateAndConnectComponents ();
		}

		/// <summary>
		/// Configure TakiLogger system
		/// </summary>
		void ConfigureLogging () {
			TakiLogger.SetLogLevel (logLevel);
			TakiLogger.SetProductionMode (productionMode);
			TakiLogger.LogSystem ("TakiLogger configured: " + TakiLogger.GetLoggerInfo ());
		}

		/// <summary>
		/// Validate components exist and connect basic references
		/// </summary>
		void ValidateAndConnectComponents () {
			TakiLogger.LogSystem ("Validating and connecting components...");

			// Validate components exist
			if (!ValidateComponents ()) {
				TakiLogger.LogError ("GameManager: Missing required components!", TakiLogger.LogCategory.System);
				return;
			}

			ConnectComponentReferences ();
			areComponentsValidated = true;
			TakiLogger.LogSystem ("Components validated and connected - Ready for game mode selection");
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

			TakiLogger.LogSystem ("Initializing single player game systems...");

			// Connect events between systems
			ConnectEvents ();

			// Initialize visual card system
			InitializeVisualCardSystem ();

			// Initialize UI for gameplay
			if (gameplayUI != null) {
				gameplayUI.ResetUIForNewGame ();
			}

			areSystemsInitialized = true;
			TakiLogger.LogSystem ("Single player systems initialized - Ready to start game");
		}

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
		/// PUBLIC METHOD: Initialize multiplayer systems - Future implementation
		/// </summary>
		public void InitializeMultiPlayerSystems () {
			// TODO: Implement multiplayer system initialization
			TakiLogger.LogSystem ("Multiplayer systems initialization - Not yet implemented");
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
				pauseManager.gameplayUI = gameplayUI;
				// pauseManager.menuNavigation will be found automatically
			}

			if (gameEndManager != null) {
				gameEndManager.gameManager = this;
				gameEndManager.gameState = gameState;
				gameEndManager.gameplayUI = gameplayUI;
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
			}

			// Turn Manager events
			if (turnManager != null) {
				turnManager.OnTurnChanged += OnTurnChanged;
				turnManager.OnComputerTurnReady += OnComputerTurnReady;
				turnManager.OnTurnTimeOut += OnPlayerTurnTimeOut;
			}

			// Computer AI events
			if (computerAI != null) {
				computerAI.OnAICardSelected += OnAICardSelected;
				computerAI.OnAIDrawCard += OnAIDrawCard;
				computerAI.OnAIColorSelected += OnAIColorSelected;
				computerAI.OnAIDecisionMade += OnAIDecisionMade;
			}

			// GameplayUI events
			if (gameplayUI != null) {
				gameplayUI.OnPlayCardClicked += OnPlayCardButtonClicked;
				gameplayUI.OnDrawCardClicked += OnDrawCardButtonClicked;
				gameplayUI.OnEndTurnClicked += OnEndTurnButtonClicked;
				gameplayUI.OnColorSelected += OnColorSelectedByPlayer;
			}

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
		/// ENHANCED: Reset all game systems for a new game - now includes special card state
		/// </summary>
		void ResetGameSystems () {
			// Clear hands and selection
			playerHand.Clear ();

			if (computerAI != null) {
				computerAI.ClearHand ();
			}

			// Reset game state
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
		}

		/// <summary>
		/// ENHANCED: Reset turn flow control state - now includes special card state
		/// </summary>
		void ResetTurnFlowState () {
			hasPlayerTakenAction = false;
			canPlayerDraw = true;
			canPlayerPlay = true;
			canPlayerEndTurn = false;

			// PHASE 7: Reset special card state
			ResetSpecialCardState ();

			TakiLogger.LogTurnFlow ("TURN FLOW STATE RESET (includes special card state)");
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
		/// ENHANCED: Start player turn with strict flow control
		/// </summary>
		void StartPlayerTurnFlow () {
			TakiLogger.LogTurnFlow ("STARTING PLAYER TURN WITH PLSTWO CHAIN AWARENESS");

			// CRITICAL: Check for active PlusTwo chain FIRST
			if (gameState.IsPlusTwoChainActive) {
				TakiLogger.LogTurnFlow ("=== PLSTWO CHAIN ACTIVE - SPECIAL TURN LOGIC ===");

				int drawCount = gameState.ChainDrawCount;
				int chainLength = gameState.NumberOfChainedCards;

				TakiLogger.LogTurnFlow ($"Chain status: {chainLength} cards, player must draw {drawCount} or play PlusTwo");

				// Check if player has PlusTwo cards
				bool hasPlusTwo = playerHand.Any (card => card.cardType == CardType.PlusTwo);

				if (hasPlusTwo) {
					// Player can either play PlusTwo to continue chain or draw to break it
					hasPlayerTakenAction = false;
					canPlayerDraw = true;     // Can draw to break chain
					canPlayerPlay = true;     // Can play PlusTwo to continue chain  
					canPlayerEndTurn = false; // Must take action first

					gameplayUI?.UpdateStrictButtonStates (true, true, false);
					gameplayUI?.ShowPlayerMessage ($"Chain active! Play PlusTwo to continue or draw {drawCount} cards to break");

					TakiLogger.LogTurnFlow ($"Player has PlusTwo options: continue chain or draw {drawCount} cards");
				} else {
					// Player has no PlusTwo cards - must draw to break chain
					hasPlayerTakenAction = false;
					canPlayerDraw = true;     // Must draw to break chain
					canPlayerPlay = false;    // No valid plays during chain without PlusTwo
					canPlayerEndTurn = false; // Must take action first

					gameplayUI?.UpdateStrictButtonStates (false, true, false);
					gameplayUI?.ShowPlayerMessage ($"No PlusTwo cards - you must draw {drawCount} cards to break chain");

					TakiLogger.LogTurnFlow ($"Player must break chain by drawing {drawCount} cards (no PlusTwo available)");
				}

				// Update visual card states - only PlusTwo cards should show as playable
				RefreshPlayerHandStates ();
				return;
			}

			// ... KEEP ALL EXISTING NORMAL TURN FLOW LOGIC AFTER THIS POINT ...
			TakiLogger.LogTurnFlow ("Normal turn flow - no active chain");

			// Reset turn flow state
			hasPlayerTakenAction = false;
			canPlayerDraw = true;
			canPlayerPlay = true;
			canPlayerEndTurn = false;

			// Check if player has valid cards (existing logic)
			int validCardCount = CountPlayableCards ();

			if (validCardCount == 0) {
				// Player has NO valid cards - must draw
				TakiLogger.LogTurnFlow ("Player has no valid cards, must draw a card");
				gameplayUI?.ShowPlayerMessage ("No valid cards - you must DRAW a card!");
				gameplayUI?.UpdateStrictButtonStates (false, true, false); // No Play, Yes Draw, No EndTurn
			} else {
				// Player has valid cards - can play or draw
				TakiLogger.LogTurnFlow ($"Player has {validCardCount} valid cards, may PLAY or DRAW a card");
				gameplayUI?.ShowPlayerMessage ($"You have {validCardCount} valid moves - PLAY a card or DRAW");
				gameplayUI?.UpdateStrictButtonStates (true, true, false); // Yes Play, Yes Draw, No EndTurn
			}

			// Update visual card states
			RefreshPlayerHandStates ();
		}

		// ===== PLAYER ACTION HANDLERS =====

		/// <summary>
		/// Handle play card button clicked (No Auto-Play)
		/// Play Card button only works with explicit selection
		/// Handle play card with strict flow control
		/// </summary>
		void OnPlayCardButtonClicked () {
			TakiLogger.LogTurnFlow ("PLAY CARD BUTTON CLICKED - STRICT FLOW");

			if (!isGameActive || !gameState.CanPlayerAct ()) {
				TakiLogger.LogWarning ("Cannot play card: Game not active or not player turn", TakiLogger.LogCategory.TurnFlow);
				gameplayUI?.ShowPlayerMessage ("Not your turn!");
				return;
			}

			if (!canPlayerPlay) {
				TakiLogger.LogWarning ("Cannot play card: Player already took action", TakiLogger.LogCategory.TurnFlow);
				gameplayUI?.ShowPlayerMessage ("You already took an action - END TURN!");
				return;
			}

			// Get selected card
			CardData cardToPlay = playerHandManager?.GetSelectedCard ();

			if (cardToPlay != null) {
				TakiLogger.LogCardPlay ($"Attempting to play selected card: {cardToPlay.GetDisplayText ()}");

				// ENHANCEMENT: Handle ChangeColor UI logic for HUMAN players before playing card
				if (cardToPlay.cardType == CardType.ChangeColor) {
					TakiLogger.LogUI ("Human playing ChangeColor card - will show color selection after card play");
				}

				PlayCardWithStrictFlow (cardToPlay);
			} else {
				int playableCount = CountPlayableCards ();
				if (playableCount > 0) {
					gameplayUI?.ShowPlayerMessage ($"Please select a card! You have {playableCount} valid moves.");
				} else {
					gameplayUI?.ShowPlayerMessage ("No valid moves - try drawing a card!");
				}
				TakiLogger.LogUI ("No card selected - player must choose explicitly");
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

		/// <summary>
		/// Handle draw card button clicked
		/// ENHANCED: Handle draw card with strict flow control
		/// </summary>
		void OnDrawCardButtonClicked () {
			TakiLogger.LogTurnFlow ("DRAW CARD BUTTON CLICKED - CHECKING FOR PLSTWO CHAIN");

			if (!isGameActive || !gameState.CanPlayerAct ()) {
				TakiLogger.LogWarning ("Cannot draw card: Game not active or not player turn", TakiLogger.LogCategory.TurnFlow);
				gameplayUI?.ShowPlayerMessage ("Not your turn!");
				return;
			}

			// CRITICAL: Check for active PlusTwo chain FIRST
			if (gameState.IsPlusTwoChainActive) {
				TakiLogger.LogTurnFlow ("=== PLAYER BREAKING PLSTWO CHAIN BY DRAWING ===");
				BreakPlusTwoChainByDrawing ();
				return; // Skip normal draw logic - chain breaking is the action
			}

			if (!canPlayerDraw) {
				TakiLogger.LogWarning ("Cannot draw card: Player already took action", TakiLogger.LogCategory.TurnFlow);
				gameplayUI?.ShowPlayerMessage ("You already took an action - END TURN!");
				return;
			}

			DrawCardWithStrictFlow ();
		}

		/// <summary>
		/// ENHANCED: Handle end turn with special card validation
		/// </summary>
		void OnEndTurnButtonClicked () {
			TakiLogger.LogTurnFlow ("END TURN BUTTON CLICKED - STRICT FLOW WITH SPECIAL CARDS");

			// PHASE 7: Check for pending special card effects first
			if (HasPendingSpecialCardEffects ()) {
				TakiLogger.LogWarning ("Cannot end turn: Pending special card effects", TakiLogger.LogCategory.TurnFlow);
				gameplayUI?.ShowPlayerMessage ("You must complete your additional action first!");
				gameplayUI?.ShowPlayerMessage (GetSpecialCardStateDescription ());
				return;
			}

			if (!canPlayerEndTurn) {
				TakiLogger.LogWarning ("Cannot end turn: Player has not taken an action yet", TakiLogger.LogCategory.TurnFlow);
				gameplayUI?.ShowPlayerMessage ("You must take an action first (PLAY or DRAW)!");
				return;
			}

			EndPlayerTurnWithStrictFlow ();
		}

		/// <summary>
		/// PHASE 7: Play card with comprehensive special card handling - ENHANCED DEBUG VERSION
		/// </summary>
		void PlayCardWithStrictFlow (CardData card) {
			TakiLogger.LogCardPlay ($"PLAYING CARD WITH STRICT FLOW (PHASE 7): {card.GetDisplayText ()}");

			// Validate the move
			CardData topCard = GetTopDiscardCard ();
			if (topCard == null) {
				TakiLogger.LogError ("Cannot play card: No top discard card available", TakiLogger.LogCategory.CardPlay);
				gameplayUI?.ShowPlayerMessage ("Game error - no discard pile!");
				return;
			}

			bool isValidMove = gameState.IsValidMove (card, topCard);
			if (!isValidMove) {
				TakiLogger.LogWarning ($"Invalid move: Cannot play {card.GetDisplayText ()} on {topCard.GetDisplayText ()}", TakiLogger.LogCategory.Rules);
				gameplayUI?.ShowPlayerMessage ($"Invalid move! Cannot play {card.GetDisplayText ()}");
				return;
			}

			// Remove card from player's hand
			bool removed = playerHand.Remove (card);
			if (!removed) {
				TakiLogger.LogError ("Could not remove card from player hand!", TakiLogger.LogCategory.CardPlay);
				return;
			}

			// Clear selection
			playerHandManager?.ClearSelection ();

			// Discard the card
			deckManager?.DiscardCard (card);

			// Update active color
			gameState.UpdateActiveColorFromCard (card);

			// DEBUG: Add explicit logging before calling HandleSpecialCardEffects
			TakiLogger.LogRules ("=== ABOUT TO CALL HandleSpecialCardEffects ===");
			TakiLogger.LogRules ($"Card type: {card.cardType}");
			TakiLogger.LogRules ($"Card display: {card.GetDisplayText ()}");
			TakiLogger.LogRules ($"Is special card: {card.IsSpecialCard}");

			try {
				// PHASE 7: Handle special card effects BEFORE determining turn flow
				TakiLogger.LogRules ("=== CALLING HandleSpecialCardEffects NOW ===");
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

			// PHASE 7: Handle turn flow based on card type and special effects
			HandlePostCardPlayTurnFlow (card);

			// Check for win condition
			if (playerHand.Count == 0) {
				TakiLogger.LogGameState ("Player wins - hand is empty!");
				gameState.DeclareWinner (PlayerType.Human);
				return;
			}

			OnCardPlayed?.Invoke (card);
			TakiLogger.LogTurnFlow ("CARD PLAY COMPLETE - Turn flow handled based on card type");
		}

		/// <summary>
		/// ENHANCED: HandlePostCardPlayTurnFlow with ChangeColor UI handling for HUMANS
		/// </summary>
		/// <param name="card">Card that was just played</param>
		void HandlePostCardPlayTurnFlow (CardData card) {
			TakiLogger.LogTurnFlow ($"=== HANDLING POST-CARD-PLAY TURN FLOW for {card.GetDisplayText ()} ===");

			// ENHANCED: Plus card handling with better messaging
			if (card.cardType == CardType.Plus) {
				TakiLogger.LogTurnFlow ("PLUS CARD PLAYED - Player gets additional action");

				// Set special card state
				isWaitingForAdditionalAction = true;
				activeSpecialCardEffect = CardType.Plus;

				// Reset action state to allow additional action
				hasPlayerTakenAction = false;  // Reset to allow additional action
				canPlayerPlay = true;          // Re-enable play
				canPlayerDraw = true;          // Re-enable draw  
				canPlayerEndTurn = false;      // Keep end turn disabled until additional action

				// ENHANCED: Update UI with better messaging
				gameplayUI?.UpdateStrictButtonStates (true, true, false);
				if (gameplayUI != null) {
					gameplayUI.ShowSpecialCardEffect (CardType.Plus, PlayerType.Human, "You get one more action");
				}

				TakiLogger.LogTurnFlow ("Plus card turn flow: Additional action enabled, END TURN disabled");
				return;
			}

			// FIXED: ChangeColor card handling for HUMAN players - ADD UI LOGIC HERE
			if (card.cardType == CardType.ChangeColor) {
				TakiLogger.LogTurnFlow ("CHANGE COLOR CARD PLAYED BY HUMAN - Player must select color");

				if (gameState == null || gameplayUI == null) {
					TakiLogger.LogError ("Cannot handle ChangeColor: Missing components!", TakiLogger.LogCategory.Rules);
					return;
				}

				// Set interaction state to color selection
				gameState.ChangeInteractionState (InteractionState.ColorSelection);
				TakiLogger.LogGameState ("Interaction state changed to ColorSelection");

				// Show color selection panel (this will disable PLAY/DRAW buttons)
				gameplayUI.ShowColorSelection (true);
				TakiLogger.LogUI ("Color selection panel displayed");

				// Mark action as taken but don't allow new actions
				hasPlayerTakenAction = true;
				canPlayerPlay = false;         // Disable play during color selection
				canPlayerDraw = false;         // Disable draw during color selection
				canPlayerEndTurn = true;       // Allow end turn after color selection

				// Note: END TURN button will be enabled, but the color selection must complete first
				gameplayUI?.UpdateStrictButtonStates (false, false, true);

				// ENHANCED: Better feedback about the selection process 
				gameplayUI.ShowPlayerMessage ("CHANGE COLOR: Choose colors freely, then click END TURN!");
				gameplayUI.ShowComputerMessage ("Player is selecting new color...");

				TakiLogger.LogTurnFlow ("ChangeColor turn flow: Color selection required, END TURN enabled after selection");
				return;
			}

			// PHASE 7: Check if this was the additional action after a Plus card
			if (isWaitingForAdditionalAction && activeSpecialCardEffect == CardType.Plus) {
				TakiLogger.LogTurnFlow ("COMPLETING ADDITIONAL ACTION after Plus card");

				// Clear special card state
				isWaitingForAdditionalAction = false;
				activeSpecialCardEffect = CardType.Number;

				// Now follow normal turn completion flow
				hasPlayerTakenAction = true;
				canPlayerPlay = false;
				canPlayerDraw = false;
				canPlayerEndTurn = true;

				gameplayUI?.ForceEnableEndTurn ();
				gameplayUI?.ShowPlayerMessage ("Additional action complete - you must END TURN!");

				TakiLogger.LogTurnFlow ("Additional action completed - normal END TURN flow");
				return;
			}

			// Normal card flow (non-Plus, non-ChangeColor cards or cards not during Plus sequence)
			TakiLogger.LogTurnFlow ("NORMAL CARD TURN FLOW - Single action, must end turn");

			hasPlayerTakenAction = true;
			canPlayerPlay = false;
			canPlayerDraw = false; // Cannot draw after playing
			canPlayerEndTurn = true;

			// UI already disabled buttons immediately on click, now enable END TURN
			gameplayUI?.ForceEnableEndTurn ();
			gameplayUI?.ShowPlayerMessage ("Card played - you must END TURN!");

			TakiLogger.LogTurnFlow ("Normal turn flow: Must END TURN after single action");
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
				HandlePostDrawTurnFlow (drawnCard);

				TakiLogger.LogCardPlay ("DRAW COMPLETE");
			} else {
				TakiLogger.LogError ("Failed to draw card - deck may be empty", TakiLogger.LogCategory.CardPlay);
				gameplayUI?.ShowPlayerMessage ("Cannot draw card!");
			}
		}

		/// <summary>
		/// PHASE 7: Handle turn flow after drawing a card
		/// </summary>
		/// <param name="drawnCard">Card that was drawn</param>
		void HandlePostDrawTurnFlow (CardData drawnCard) {
			TakiLogger.LogTurnFlow ($"=== HANDLING POST-DRAW TURN FLOW ===");

			// PHASE 7: Check if this was the additional action after a Plus card
			if (isWaitingForAdditionalAction && activeSpecialCardEffect == CardType.Plus) {
				TakiLogger.LogTurnFlow ("COMPLETING ADDITIONAL DRAW ACTION after Plus card");

				// Clear special card state
				isWaitingForAdditionalAction = false;
				activeSpecialCardEffect = CardType.Number;

				// Now follow normal turn completion flow
				hasPlayerTakenAction = true;
				canPlayerPlay = false;
				canPlayerDraw = false;
				canPlayerEndTurn = true;

				gameplayUI?.ForceEnableEndTurn ();
				gameplayUI?.ShowPlayerMessage ($"Additional draw complete: {drawnCard.GetDisplayText ()} - you must END TURN!");

				TakiLogger.LogTurnFlow ("Additional draw action completed - normal END TURN flow");
				return;
			}

			// Normal draw flow (first action or not during Plus sequence)
			TakiLogger.LogTurnFlow ("NORMAL DRAW TURN FLOW - Single action, must end turn");

			hasPlayerTakenAction = true;
			canPlayerPlay = false;
			canPlayerDraw = false;
			canPlayerEndTurn = true;

			// Enable END TURN button
			gameplayUI?.ForceEnableEndTurn ();
			gameplayUI?.ShowPlayerMessage ($"Drew: {drawnCard.GetDisplayText ()} - you must END TURN!");

			TakiLogger.LogTurnFlow ("Normal draw flow: Must END TURN after single action");
		}

		/// <summary>
		/// FIXED: Enhanced turn completion to handle Stop card effects at correct timing
		/// </summary>
		void EndPlayerTurnWithStrictFlow () {
			TakiLogger.LogTurnFlow ("ENDING PLAYER TURN - STRICT FLOW WITH SPECIAL CARDS");

			// STOP CARD FIX: Check STOP flag FIRST, before any turn switching logic
			if (shouldSkipNextTurn) {
				TakiLogger.LogTurnFlow ("=== STOP FLAG DETECTED - PROCESSING STOP EFFECT ===");
				ProcessStopSkipEffect ();
				return; // Exit early - don't proceed with normal turn switch
			}

			// Check for pending effects (PLUS cards only)
			if (HasPendingSpecialCardEffects ()) {
				TakiLogger.LogError ("Attempting to end turn with pending special card effects!", TakiLogger.LogCategory.TurnFlow);
				gameplayUI?.ShowPlayerMessage ("Cannot end turn - special card effect pending!");
				return;
			}

			// FIXED: Hide color selection panel if it's still visible
			if (gameState != null && gameState.interactionState == InteractionState.ColorSelection) {
				TakiLogger.LogGameState ("Ending turn during color selection - hiding panel and returning to normal");

				// Hide the color selection panel
				gameplayUI?.ShowColorSelection (false);

				// Return to normal interaction state
				gameState.ChangeInteractionState (InteractionState.Normal);

				// Show completion message
				gameplayUI?.ShowPlayerMessage ($"Color selection complete: {gameState.activeColor}");
			}

			// Clear any selected cards
			playerHandManager?.ClearSelection ();

			// Reset turn flow state for next turn (includes special card state)
			ResetTurnFlowState ();

			// Normal turn end - proceed with turn switch
			TakiLogger.LogTurnFlow ("Normal turn end - switching to computer turn");
			if (turnManager != null) {
				turnManager.EndTurn ();
			}
		}

		/// <summary>
		/// FIXED: Enhanced AI turn completion to handle Stop card effects at correct timing
		/// Mirrors the logic in EndPlayerTurnWithStrictFlow() for symmetrical behavior
		/// </summary>
		void EndAITurnWithStrictFlow () {
			TakiLogger.LogTurnFlow ("ENDING AI TURN - STRICT FLOW WITH SPECIAL CARDS");

			// STOP CARD FIX: Check STOP flag FIRST, before any turn switching logic
			if (shouldSkipNextTurn) {
				TakiLogger.LogTurnFlow ("=== STOP FLAG DETECTED FOR AI TURN END - PROCESSING STOP EFFECT ===");
				ProcessStopSkipEffect ();
				return; // Exit early - don't proceed with normal turn switch
			}

			// Normal turn end - proceed with turn switch
			TakiLogger.LogTurnFlow ("Normal AI turn end - switching to human turn");
			if (turnManager != null) {
				turnManager.EndTurn ();
			}
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

			gameplayUI?.ShowPlayerMessage (benefitMessage);
			gameplayUI?.ShowComputerMessage (skipMessage);

			TakiLogger.LogTurnFlow ($"STOP effect: {skippedPlayer} turn skipped, {benefitPlayer} gets another turn");

			// Clear any selected cards from previous action
			playerHandManager?.ClearSelection ();

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
		/// NEW: Start player turn after STOP effect processing
		/// </summary>
		void StartPlayerTurnAfterStop () {
			TakiLogger.LogTurnFlow ("=== STARTING PLAYER TURN AFTER STOP EFFECT ===");

			// Ensure game state shows player turn
			if (gameState != null) {
				gameState.ChangeTurnState (TurnState.PlayerTurn);
			}

			// Start the player turn flow
			StartPlayerTurnFlow ();

			// Show additional feedback
			gameplayUI?.ShowPlayerMessage ("Your turn continues thanks to STOP card!");
			gameplayUI?.ShowComputerMessage ("Waiting (turn skipped)...");

			TakiLogger.LogTurnFlow ("Player turn restarted successfully after STOP effect");
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
					gameplayUI?.ShowPlayerMessage ($"Played NUMBER {card.GetDisplayText ()}");
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
					gameplayUI?.ShowComputerMessage ("PLUS: Additional action required!");
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
					gameplayUI?.ShowComputerMessage ("STOP: Opponent's turn skipped!");
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
					gameplayUI?.ShowComputerMessage ("DIRECTION: Turn direction changed!");
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
					gameplayUI?.ShowComputerMessage ("CHANGE COLOR: Color selection active!");
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
					gameplayUI?.ShowComputerMessage ("PLUS TWO: Opponent draws 2 cards");
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
					gameplayUI?.ShowComputerMessage ($"TAKI: Multi-card sequence planned (Phase 8)");
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
					gameplayUI?.ShowComputerMessage ("SUPER TAKI: Multi-card any color planned (Phase 8)");
					break;

				default:
					TakiLogger.LogWarning ($"UNKNOWN CARD TYPE: {card.cardType}", TakiLogger.LogCategory.Rules);
					TakiLogger.LogRules ("ERROR: Card type not recognized by rule system");
					gameplayUI?.ShowComputerMessage ($"Unknown card: {card.GetDisplayText ()}");
					break;
			}

			TakiLogger.LogRules ("=== CARD EFFECT ANALYSIS COMPLETE ===");
		}

		/// <summary>
		/// FIXED: Enhanced color selection handler with proper flow control
		/// </summary>
		/// <param name="selectedColor">Color selected by player</param>
		void OnColorSelectedByPlayer (CardColor selectedColor) {
			TakiLogger.LogGameState ($"=== PLAYER SELECTED COLOR: {selectedColor} ===");

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
			TakiLogger.LogGameState ($"Active color changed to: {selectedColor}");

			// Show feedback about color selection
			gameplayUI?.ShowPlayerMessage ($"Color changed to {selectedColor} - you must END TURN!");
			gameplayUI?.ShowComputerMessage ($"Active color: {selectedColor}");

			// Update UI to reflect new color
			UpdateAllUI ();

			TakiLogger.LogRules ("CHANGE COLOR effect complete - color selection processed");
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

		/// <summary>
		/// PHASE 7: ENHANCED DEBUG HandleSpecialCardEffects - Find why it's not working
		/// </summary>
		/// <param name="card">Card that was played</param>
		void HandleSpecialCardEffects (CardData card) {
			TakiLogger.LogRules ($"=== HANDLING SPECIAL CARD EFFECTS for {card.GetDisplayText ()} ===");

			if (card == null) {
				TakiLogger.LogError ("HandleSpecialCardEffects called with NULL card!", TakiLogger.LogCategory.Rules);
				return;
			}

			TakiLogger.LogRules ($"=== HANDLING SPECIAL CARD EFFECTS for {card.GetDisplayText ()} ===");
			TakiLogger.LogRules ($"Card type: {card.cardType}");
			TakiLogger.LogRules ($"Card name: {card.cardName}");

			switch (card.cardType) {
				case CardType.Plus:
					// PHASE 7: Plus card effect - player gets additional action
					TakiLogger.LogRules ("PLUS card effect - Player must take 1 additional action");
					// Note: Turn flow handling is done in HandlePostCardPlayTurnFlow
					// This method just handles game state changes, not turn flow
					break;

				case CardType.Stop:
					TakiLogger.LogRules ("=== STOP CARD DETECTED IN SWITCH STATEMENT ===");
					// FIXED: STOP card effect - set flag instead of immediate skip
					TakiLogger.LogRules ("STOP card effect - Next opponent turn will be skipped");

					try {
						TakiLogger.LogRules ("=== CALLING HandleStopCardEffect ===");
						HandleStopCardEffect ();
						TakiLogger.LogRules ("=== HandleStopCardEffect COMPLETED ===");
					} catch (System.Exception ex) {
						TakiLogger.LogError ($"EXCEPTION in HandleStopCardEffect: {ex.Message}", TakiLogger.LogCategory.Rules);
					}
					break;

				case CardType.ChangeDirection:
					// PHASE 7: ChangeDirection card effect - reverse turn direction
					TakiLogger.LogRules ("CHANGE DIRECTION card effect - Turn direction changes");
					HandleChangeDirectionCardEffect ();
					break;

				case CardType.ChangeColor:
					// PHASE 7: ChangeColor card effect - player must choose new color
					TakiLogger.LogRules ("CHANGE COLOR card effect - Player must choose new color");
					HandleChangeColorCardEffect ();
					break;

				case CardType.PlusTwo:
					TakiLogger.LogRules ("=== PLUS TWO CARD: ENHANCED CHAIN SYSTEM ===");

					PlayerType currentPlayer = turnManager?.CurrentPlayer ?? PlayerType.Human;
					PlayerType targetPlayer = currentPlayer == PlayerType.Human ? PlayerType.Computer : PlayerType.Human;

					if (!gameState.IsPlusTwoChainActive) {
						// Start new chain
						gameState.StartPlusTwoChain (currentPlayer);
						gameState.ChangeInteractionState (InteractionState.PlusTwoChain);

						TakiLogger.LogRules ("CHAIN STARTED: First PlusTwo played - opponent must draw 2 or continue chain");

						// Enhanced UI integration
						gameplayUI?.ShowChainProgressMessage (1, 2, targetPlayer);
						gameplayUI?.ShowPlusTwoChainStatus (1, 2, targetPlayer == PlayerType.Human);

					} else {
						// Continue existing chain
						gameState.ContinuePlusTwoChain ();

						int chainCount = gameState.NumberOfChainedCards;
						int drawCount = gameState.ChainDrawCount;
						TakiLogger.LogRules ($"CHAIN CONTINUED: Now {chainCount} PlusTwo cards, opponent must draw {drawCount} or continue");

						// Enhanced UI integration
						gameplayUI?.ShowChainProgressMessage (chainCount, drawCount, targetPlayer);
						gameplayUI?.ShowPlusTwoChainStatus (chainCount, drawCount, targetPlayer == PlayerType.Human);
					}

					TakiLogger.LogRules ($"PlusTwo chain status: {gameState.NumberOfChainedCards} cards, {gameState.ChainDrawCount} total draw");
					break;

				case CardType.Taki:
					// TODO Phase 8: Implement Taki sequence
					TakiLogger.LogRules ("TAKI card - Multi-card sequence (Phase 8)");
					break;

				case CardType.SuperTaki:
					// TODO Phase 8: Implement SuperTaki sequence
					TakiLogger.LogRules ("SUPER TAKI card - Multi-card sequence any color (Phase 8)");
					break;

				case CardType.Number:
					// No special effects for number cards
					break;

				default:
					TakiLogger.LogWarning ($"Unknown card type for special effects: {card.cardType}", TakiLogger.LogCategory.Rules);
					break;
			}
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
				gameplayUI?.ShowPlayerMessage ($"DIRECTION CHANGED: {oldDirection} -> {newDirection}");
				gameplayUI?.ShowComputerMessage ($"Turn direction: {newDirection}");

				// For 2-player game, this is mainly visual/informational
				string twoPlayerNote = GetTwoPlayerDirectionNote (newDirection);
				if (!string.IsNullOrEmpty (twoPlayerNote)) {
					gameplayUI?.ShowComputerMessage (twoPlayerNote);
				}

				TakiLogger.LogRules ("CHANGE DIRECTION effect complete");
			} else {
				TakiLogger.LogError ("Cannot change direction: GameStateManager is null!", TakiLogger.LogCategory.Rules);
				gameplayUI?.ShowPlayerMessage ("ERROR: Cannot change direction!");
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
				gameplayUI?.ShowPlayerMessage ("ERROR: Cannot change color!");
				return;
			}

			if (gameplayUI == null) {
				TakiLogger.LogError ("Cannot show color selection: GameplayUIManager is null!", TakiLogger.LogCategory.Rules);
				return;
			}

			//// Set interaction state to color selection
			//gameState.ChangeInteractionState (InteractionState.ColorSelection);
			//TakiLogger.LogGameState ("Interaction state changed to ColorSelection");

			//// Show color selection panel (this will disable PLAY/DRAW buttons)
			//gameplayUI.ShowColorSelection (true);
			//TakiLogger.LogUI ("Color selection panel displayed");

			//// ENHANCED: Better feedback about the selection process 
			//gameplayUI.ShowPlayerMessage ("CHANGE COLOR: Choose colors freely, then click END TURN!");
			//gameplayUI.ShowComputerMessage ("Player is selecting new color...");

			//TakiLogger.LogRules ("CHANGE COLOR effect activated - panel stays visible until END TURN");



			// CORE LOGIC: This applies to ALL players (human and AI)
			// Just log that a ChangeColor card was played - actual color selection handled separately
			TakiLogger.LogRules ("CHANGE COLOR card played - color selection required");
			TakiLogger.LogRules ("Color will be selected by appropriate player handler");
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
			gameplayUI?.ShowPlayerMessage (message);
			gameplayUI?.ShowComputerMessage ($"STOP card effect - {targetPlayer} turn scheduled for skip");

			TakiLogger.LogRules ("STOP effect prepared - flag set for next turn check");
			TakiLogger.LogRules ("=== EXITING HandleStopCardEffect METHOD ===");
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
				gameplayUI?.ShowChainBrokenMessage (drawnCards.Count, PlayerType.Human);

				// Show what cards were drawn (first few for feedback)
				if (drawnCards.Count <= 3) {
					string cardList = string.Join (", ", drawnCards.Select (c => c.GetDisplayText ()));
					TakiLogger.LogCardPlay ($"Cards drawn: {cardList}");
				} else {
					TakiLogger.LogCardPlay ($"Cards drawn: {drawnCards [0].GetDisplayText ()}, {drawnCards [1].GetDisplayText ()}, ... and {drawnCards.Count - 2} more");
				}
			} else {
				TakiLogger.LogError ("Failed to draw any cards for chain breaking", TakiLogger.LogCategory.CardPlay);
				gameplayUI?.ShowPlayerMessage ("Error: Cannot draw cards!");
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
			HandlePostDrawTurnFlow (drawnCards.LastOrDefault ());
		}

		/// <summary>
		/// Update all UI with forced button state sync
		/// </summary>
		void UpdateAllUI () {
			if (gameplayUI != null && gameState != null) {
				// Update hand sizes
				int computerHandSize = computerAI?.HandSize ?? 0;
				gameplayUI.UpdateHandSizeDisplay (playerHand.Count, computerHandSize);

				// Update all displays using new architecture
				gameplayUI.UpdateAllDisplays (
					gameState.turnState,
					gameState.gameStatus,
					gameState.interactionState,
					gameState.activeColor
		);

				// CHAIN UI INTEGRATION: Update chain status if active
				if (gameState.IsPlusTwoChainActive) {
					bool isPlayerTurn = gameState.IsPlayerTurn;
					gameplayUI.ShowPlusTwoChainStatus (
						gameState.NumberOfChainedCards,
						gameState.ChainDrawCount,
						isPlayerTurn
					);
				} else {
					gameplayUI.HidePlusTwoChainStatus ();
				}
			}

			// Update visual card displays 
			UpdateVisualHands ();
		}

		// ===== EVENT HANDLERS =====

		/// <summary>
		/// UPDATED: OnTurnStateChanged - Remove STOP processing (now handled earlier)
		/// </summary>
		void OnTurnStateChanged (TurnState newTurnState) {
			TakiLogger.LogGameState ($"Turn state changed to {newTurnState}");

			// Normal turn flow processing
			if (gameplayUI != null) {
				gameplayUI.UpdateTurnDisplay (newTurnState);
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
				gameplayUI.ShowColorSelection (newInteractionState == InteractionState.ColorSelection);
			}
		}

		void OnGameStatusChanged (GameStatus newGameStatus) {
			UpdateAllUI ();
		}

		void OnActiveColorChanged (CardColor newColor) {
			if (gameplayUI != null) {
				gameplayUI.UpdateActiveColorDisplay (newColor);
			}
		}

		void OnTurnChanged (PlayerType player) {
			OnTurnStarted?.Invoke (player);
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

			gameplayUI?.ShowPlayerMessage (skipMessage);
			gameplayUI?.ShowComputerMessage (benefitMessage);

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
		/// NEW: Start AI turn after STOP effect processing
		/// Mirrors StartPlayerTurnAfterStop() for when computer benefits from STOP
		/// </summary>
		void StartAITurnAfterStop () {
			TakiLogger.LogTurnFlow ("=== STARTING AI TURN AFTER STOP EFFECT ===");

			// Ensure game state shows computer turn
			if (gameState != null) {
				gameState.ChangeTurnState (TurnState.ComputerTurn);
			}

			// Show feedback messages
			gameplayUI?.ShowPlayerMessage ("Computer gets another turn thanks to STOP card!");
			gameplayUI?.ShowComputerMessage ("I get another turn!");

			// Trigger AI decision for the new turn
			CardData topCard = GetTopDiscardCard ();
			if (topCard != null && computerAI != null) {
				TakiLogger.LogTurnFlow ("Triggering AI decision for STOP benefit turn");
				// Give AI time to "think" about the new turn
				Invoke (nameof (TriggerAITurnAfterStop), 1.5f);
			} else {
				TakiLogger.LogError ("Cannot start AI turn after STOP - missing components", TakiLogger.LogCategory.AI);
			}

			TakiLogger.LogTurnFlow ("AI turn setup complete after STOP effect");
		}

		/// <summary>
		/// Helper method to trigger AI decision after STOP benefit
		/// </summary>
		void TriggerAITurnAfterStop () {
			TakiLogger.LogAI ("=== AI MAKING DECISION FOR STOP BENEFIT TURN ===");

			CardData topCard = GetTopDiscardCard ();
			if (topCard != null && computerAI != null) {
				gameplayUI?.ShowComputerMessage ("Thinking about my bonus turn...");
				computerAI.MakeDecision (topCard);
			} else {
				TakiLogger.LogError ("Cannot trigger AI turn after STOP - missing components", TakiLogger.LogCategory.AI);
			}
		}

		/// <summary>
		/// AI card selection handler with validation
		/// </summary> 
		void OnAICardSelected (CardData card) {
			TakiLogger.LogAI ("=== AI SELECTED CARD (CHAIN AWARE): " + (card != null ? card.GetDisplayText () : "NULL") + " ===");

			if (card == null || computerAI == null) {
				TakiLogger.LogError ("AI selected null card or computerAI is null!", TakiLogger.LogCategory.AI);
				return;
			}

			// CHAIN VALIDATION: During PlusTwo chain, verify this is a PlusTwo card
			if (gameState != null && gameState.IsPlusTwoChainActive && card.cardType != CardType.PlusTwo) {
				TakiLogger.LogError ($"AI tried to play {card.GetDisplayText ()} during PlusTwo chain - only PlusTwo allowed!", TakiLogger.LogCategory.AI);
				gameplayUI?.ShowComputerMessage ("Error: Invalid card during chain");
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
					gameplayUI?.ShowComputerMessage ("I win if you don't continue the chain!");
					// Don't declare winner yet - chain must be resolved first
					// Game will end after player responds to chain
				} else {
					TakiLogger.LogGameState ("Computer wins - hand is empty!");
					if (gameState != null) {
						gameState.DeclareWinner (PlayerType.Computer);
					}
					return;
				}
			}

			// Handle special card effects for AI - this will process the PlusTwo chain logic
			if (card.IsSpecialCard) {
				TakiLogger.LogAI ($"AI played special card - handling {card.cardType} effect");

				// Call main special card effects method to set flags and handle core logic
				TakiLogger.LogAI ("=== CALLING MAIN HandleSpecialCardEffects for AI card ===");
				HandleSpecialCardEffects (card);

				// Then handle AI-specific turn flow
				TakiLogger.LogAI ("=== CALLING AI-specific turn flow handling ===");
				HandleAISpecialCardEffects (card);
			} else {
				// Normal card - end turn normally
				TakiLogger.LogAI ("AI played normal card - ending turn normally");
				if (turnManager != null) {
					turnManager.EndTurn ();
				}
			}
		}

		/// <summary>
		/// FIXED: Handle AI-specific turn flow for special cards - ChangeColor case properly separated
		/// </summary>
		/// <param name="card">Card played by AI</param>
		void HandleAISpecialCardEffects (CardData card) {
			TakiLogger.LogAI ($"=== AI HANDLING TURN FLOW for {card.GetDisplayText ()} ===");
			TakiLogger.LogAI ("Note: Core special card effects already handled by main HandleSpecialCardEffects method");

			switch (card.cardType) {
				case CardType.Plus:
					// AI gets additional action just like human
					TakiLogger.LogAI ("AI played PLUS card - AI gets additional action");

					// ENHANCED: Use proper message routing
					if (gameplayUI != null) {
						gameplayUI.ShowSpecialCardEffect (CardType.Plus, PlayerType.Computer, "AI gets one more action");
					}

					// AI needs to make another decision
					CardData topCard = GetTopDiscardCard ();
					if (topCard != null && computerAI != null) {
						TakiLogger.LogAI ("Triggering AI additional action for PLUS card");
						// Give AI another decision with slight delay
						Invoke (nameof (TriggerAIAdditionalAction), 1.0f);
					}
					break;

				case CardType.Stop:
					// FIXED: Core STOP logic (flag setting) handled by main method
					TakiLogger.LogAI ("AI played STOP card - flag already set by main method");

					if (gameplayUI != null) {
						gameplayUI.ShowSpecialCardEffect (CardType.Stop, PlayerType.Computer, "Your next turn will be skipped!");
					}

					// FIXED: Use new AI turn end method that checks STOP flag
					TakiLogger.LogAI ("Ending AI turn with STOP flag check");
					EndAITurnWithStrictFlow ();
					break;

				case CardType.ChangeDirection:
					// AI played ChangeDirection - core logic already handled
					TakiLogger.LogAI ("AI played CHANGE DIRECTION card - direction already changed");

					// ENHANCED: Use proper message routing
					if (gameplayUI != null) {
						string directionMessage = $"Direction changed by AI: {gameState?.turnDirection}";
						gameplayUI.ShowSpecialCardEffect (CardType.ChangeDirection, PlayerType.Computer, directionMessage);
					}

					// Normal turn end for AI
					if (turnManager != null) {
						turnManager.EndTurn ();
					}
					break;

				case CardType.ChangeColor:
					// FIXED: AI played ChangeColor - handles color selection completely
					TakiLogger.LogAI ("AI played CHANGE COLOR card - handling AI color selection");

					// ENHANCED: Use proper message routing
					if (gameplayUI != null) {
						gameplayUI.ShowImmediateFeedback ("AI is selecting new color...", true);
					}

					// AI selects color automatically - NO UI STATE CHANGES NEEDED
					if (computerAI != null) {
						CardColor selectedColor = computerAI.SelectColor ();
						gameState?.ChangeActiveColor (selectedColor);

						// Show color selection result with enhanced messaging
						if (gameplayUI != null) {
							gameplayUI.ShowSpecialCardEffect (CardType.ChangeColor, PlayerType.Computer,
								$"New active color: {selectedColor}");
						}

						TakiLogger.LogAI ($"AI selected color: {selectedColor}");
					}

					// Normal turn end for AI
					if (turnManager != null) {
						turnManager.EndTurn ();
					}
					break;

				case CardType.PlusTwo:
					// ENHANCED: PlusTwo chain handling
					TakiLogger.LogAI ("AI played PLUS TWO card - chain logic already handled by main method");

					if (gameState.IsPlusTwoChainActive) {
						// Chain is now active - opponent must respond
						int chainCount = gameState.NumberOfChainedCards;
						int drawCount = gameState.ChainDrawCount;

						gameplayUI?.ShowSpecialCardEffect (CardType.PlusTwo, PlayerType.Computer,
							$"Chain continues! Draw {drawCount} or play PlusTwo");

						TakiLogger.LogAI ($"PlusTwo chain continues: {chainCount} cards, opponent draws {drawCount} or continues");
					} else {
						// This shouldn't happen as PlusTwo should always create/continue a chain
						TakiLogger.LogWarning ("AI played PlusTwo but no chain is active!", TakiLogger.LogCategory.AI);
						gameplayUI?.ShowSpecialCardEffect (CardType.PlusTwo, PlayerType.Computer, "You draw 2 cards!");
					}

					// Normal turn end for AI after playing PlusTwo
					if (turnManager != null) {
						turnManager.EndTurn ();
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
				gameplayUI?.ShowComputerMessage ("Taking additional action...");
				computerAI.MakeDecision (topCard);
			} else {
				TakiLogger.LogError ("Cannot trigger AI additional action - missing components", TakiLogger.LogCategory.AI);
				// FIXED: Fallback uses new AI turn end method
				EndAITurnWithStrictFlow ();
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
					gameplayUI?.ShowPlayerMessage ($"AI broke chain by drawing {drawnCards.Count} cards");
					gameplayUI?.ShowComputerMessage ($"I drew {drawnCards.Count} cards - chain broken");

					// Log what cards were drawn (for debugging)
					if (drawnCards.Count <= 3) {
						string cardList = string.Join (", ", drawnCards.Select (c => c.GetDisplayText ()));
						TakiLogger.LogAI ($"AI drew cards: {cardList}");
					} else {
						TakiLogger.LogAI ($"AI drew: {drawnCards [0].GetDisplayText ()}, {drawnCards [1].GetDisplayText ()}, ... and {drawnCards.Count - 2} more");
					}
				} else {
					TakiLogger.LogError ("AI failed to draw any cards for chain breaking", TakiLogger.LogCategory.AI);
					gameplayUI?.ShowComputerMessage ("Error: Cannot draw cards!");
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
				gameplayUI.ShowComputerMessage (decision);
			}
		}

		void OnGameWon (PlayerType winner) {
			TakiLogger.LogSystem ($"GameManager: Game won by {winner}");

			// Process through GameEndManager instead of direct UI
			if (gameEndManager != null) {
				gameEndManager.ProcessGameEnd (winner);
			} else {
				// Fallback to existing logic if GameEndManager not available
				if (gameplayUI != null) {
					gameplayUI.ShowWinnerAnnouncement (winner);
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
		/// Initialize visual card system
		/// </summary>
		void InitializeVisualCardSystem () {
			TakiLogger.LogSystem ("Initializing visual card system...");

			if (playerHandManager == null || computerHandManager == null) {
				TakiLogger.LogError ("HandManager references missing!", TakiLogger.LogCategory.System);
				return;
			}

			playerHandManager.OnCardSelected += OnPlayerCardSelected;
			computerHandManager.OnCardSelected += OnComputerCardSelected;

			TakiLogger.LogSystem ("Visual card system initialized");
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
					gameplayUI.ShowPlayerMessage (message);
				}
			}
		}

		/// <summary>
		/// Handle computer card selection (should not happen)
		/// </summary>
		void OnComputerCardSelected (CardController selectedCardController) {
			TakiLogger.LogWarning ("Computer cards should not be selectable!", TakiLogger.LogCategory.UI);
		}

		/// <summary>
		/// Update visual hands with error checking
		/// </summary>
		void UpdateVisualHands () {
			try {
				// Update player hand visual
				if (playerHandManager != null && playerHand != null) {
					playerHandManager.UpdateHandDisplay (playerHand);
					TakiLogger.LogUI ($"Updated player hand display: {playerHand.Count} cards", TakiLogger.LogLevel.Verbose);
				}

				// Update computer hand visual
				if (computerHandManager != null && computerAI != null) {
					List<CardData> computerHand = computerAI.GetHandCopy ();
					computerHandManager.UpdateHandDisplay (computerHand);
					TakiLogger.LogUI ($"Updated computer hand display: {computerHand.Count} cards", TakiLogger.LogLevel.Verbose);
				}
			} catch (System.Exception e) {
				TakiLogger.LogError ($"Error updating visual hands: {e.Message}", TakiLogger.LogCategory.UI);
			}
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

			if (gameplayUI == null) {
				TakiLogger.LogError ("GameManager: GameplayUIManager not assigned!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (deckManager == null) {
				TakiLogger.LogError ("GameManager: DeckManager not assigned!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (playerHandManager == null) {
				TakiLogger.LogError ("GameManager: PlayerHandManager not assigned!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (computerHandManager == null) {
				TakiLogger.LogError ("GameManager: ComputerHandManager not assigned!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (pauseManager == null) {
				TakiLogger.LogWarning ("GameManager: PauseManager not assigned - pause functionality will be disabled", TakiLogger.LogCategory.System);
			}

			if (gameEndManager == null) {
				TakiLogger.LogWarning ("GameManager: GameEndManager not assigned - game end functionality will be limited", TakiLogger.LogCategory.System);
			}

			if (exitValidationManager == null) {
				TakiLogger.LogWarning ("GameManager: ExitValidationManager not assigned - exit confirmation will be disabled", TakiLogger.LogCategory.System);
			}

			return isValid;
		}

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

		// ===== TURN FLOW STATE PRESERVATION METHODS =====

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
			if (gameplayUI != null && gameState != null && gameState.IsPlayerTurn) {
				gameplayUI.UpdateStrictButtonStates (
					snapshot.canPlayerPlay,
					snapshot.canPlayerDraw,
					snapshot.canPlayerEndTurn
				);

				TakiLogger.LogTurnFlow ("Button states restored to match captured state");
			}

			TakiLogger.LogTurnFlow ("Turn flow state fully restored");
		}

		// ===== PUBLIC METHODS FOR EXTERNAL COORDINATION =====

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

		// ===== DEBUG METHODS =====

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

			gameplayUI?.ShowPlayerMessage ("TEST: STOP flag set - now click END TURN");
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
			gameplayUI.UpdateButtonStates (shouldEnableButtons);

			TakiLogger.LogDiagnostics ($"UI synced - Turn: {gameState.turnState}, Buttons enabled: {shouldEnableButtons}");
		}

		/// <summary>
		/// Force refresh of player hand playable states (delayed)
		/// </summary>
		void RefreshPlayerHandStates () {
			TakiLogger.LogUI ("REFRESHING PLAYER HAND STATES");

			if (playerHandManager != null) {
				playerHandManager.RefreshPlayableStates ();
			}
		}

		// ===== PUBLIC API =====

		public void RequestDrawCard () => OnDrawCardButtonClicked ();
		public void RequestPlayCard (CardData card) => PlayCardWithStrictFlow (card);
		public List<CardData> GetPlayerHand () => new List<CardData> (playerHand);
		public bool CanPlayerAct () => gameState?.CanPlayerAct () ?? false;

		// Properties 
		public bool IsGameActive => isGameActive;
		public bool IsPlayerTurn => gameState?.IsPlayerTurn ?? false;
		public PlayerType CurrentPlayer => turnManager?.CurrentPlayer ?? PlayerType.Human;
		public int PlayerHandSize => playerHand.Count;
		public int ComputerHandSize => computerAI?.HandSize ?? 0;
		public CardColor ActiveColor => gameState?.activeColor ?? CardColor.Wild;
		public bool AreComponentsValidated => areComponentsValidated;
		public bool AreSystemsInitialized => areSystemsInitialized;

		// Turn flow properties - enhanced with special card state
		public bool HasPlayerTakenAction => hasPlayerTakenAction;
		public bool CanPlayerDrawCard => canPlayerDraw;
		public bool CanPlayerPlayCard => canPlayerPlay;
		public bool CanPlayerEndTurn => canPlayerEndTurn && !HasPendingSpecialCardEffects (); // ENHANCED
		public bool IsWaitingForAdditionalAction => isWaitingForAdditionalAction; // NEW
		public CardType ActiveSpecialCardEffect => activeSpecialCardEffect; // NEW

		// Pause/Game End Properties
		public bool IsGamePaused => pauseManager != null && pauseManager.IsGamePaused;
		public bool IsGameEndProcessed => gameEndManager != null && gameEndManager.IsGameEndProcessed;
		public bool IsExitValidationActive => exitValidationManager != null && exitValidationManager.IsExitValidationActive;
	}
}