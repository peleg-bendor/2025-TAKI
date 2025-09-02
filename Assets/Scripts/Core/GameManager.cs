using UnityEngine;
using System.Collections.Generic;

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
		/// Reset all game systems for a new game
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
			ResetTurnFlowState ();
			isGameActive = false;
		}

		/// <summary>
		/// ENHANCED: Reset turn flow control state
		/// </summary>
		void ResetTurnFlowState () {
			hasPlayerTakenAction = false;
			canPlayerDraw = true;
			canPlayerPlay = true;
			canPlayerEndTurn = false;

			TakiLogger.LogTurnFlow ("TURN FLOW STATE RESET");
		}

		/// <summary>
		/// ENHANCED: Start player turn with strict flow control
		/// </summary>
		void StartPlayerTurnFlow () {
			TakiLogger.LogTurnFlow ("STARTING PLAYER TURN WITH STRICT FLOW");

			// Reset turn flow state
			hasPlayerTakenAction = false;
			canPlayerDraw = true;
			canPlayerPlay = true;
			canPlayerEndTurn = false;

			// Check if player has valid cards
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
		/// Find first valid card in player's hand
		/// </summary>
		CardData FindFirstValidCard () {
			if (playerHand == null || playerHand.Count == 0) {
				TakiLogger.LogCardPlay ("No cards in player hand");
				return null;
			}

			CardData topCard = GetTopDiscardCard ();
			if (topCard == null) {
				TakiLogger.LogError ("Cannot find valid card - no top discard card", TakiLogger.LogCategory.CardPlay);
				return null;
			}

			TakiLogger.LogRules ($"Searching for valid card to play on: {topCard.GetDisplayText ()} with active color: {gameState.activeColor}");

			foreach (CardData card in playerHand) {
				if (gameState?.IsValidMove (card, topCard) == true) {
					TakiLogger.LogRules ($"Found valid card: {card.GetDisplayText ()}");
					return card;
				}
			}

			TakiLogger.LogCardPlay ("No valid cards found in hand");
			return null;
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
			TakiLogger.LogTurnFlow ("DRAW CARD BUTTON CLICKED - STRICT FLOW");

			if (!isGameActive || !gameState.CanPlayerAct ()) {
				TakiLogger.LogWarning ("Cannot draw card: Game not active or not player turn", TakiLogger.LogCategory.TurnFlow);
				gameplayUI?.ShowPlayerMessage ("Not your turn!");
				return;
			}

			if (!canPlayerDraw) {
				TakiLogger.LogWarning ("Cannot draw card: Player already took action", TakiLogger.LogCategory.TurnFlow);
				gameplayUI?.ShowPlayerMessage ("You already took an action - END TURN!");
				return;
			}

			DrawCardWithStrictFlow ();
		}

		/// <summary>
		/// Handle end turn button clicked
		/// ENHANCED: Handle end turn with strict flow control
		/// </summary>
		void OnEndTurnButtonClicked () {
			TakiLogger.LogTurnFlow ("END TURN BUTTON CLICKED - STRICT FLOW");

			if (!canPlayerEndTurn) {
				TakiLogger.LogWarning ("Cannot end turn: Player has not taken an action yet", TakiLogger.LogCategory.TurnFlow);
				gameplayUI?.ShowPlayerMessage ("You must take an action first (PLAY or DRAW)!");
				return;
			}

			EndPlayerTurnWithStrictFlow ();
		}

		/// <summary>
		/// Play card with comprehensive special card logging
		/// </summary>
		void PlayCardWithStrictFlow (CardData card) {
			TakiLogger.LogCardPlay ($"PLAYING CARD WITH STRICT FLOW: {card.GetDisplayText ()}");

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

			// Log comprehensive card effect information
			LogCardEffectRules (card);

			// Update UI
			UpdateAllUI ();
			RefreshPlayerHandStates ();

			// Mark action taken and update flow
			hasPlayerTakenAction = true;
			canPlayerPlay = false;
			canPlayerDraw = false; // Cannot draw after playing
			canPlayerEndTurn = true;

			// UI already disabled buttons immediately on click, now enable END TURN
			TakiLogger.LogTurnFlow ("ENABLING END TURN button after successful card play");
			gameplayUI?.ForceEnableEndTurn ();
			gameplayUI?.ShowPlayerMessage ("Card played - you must END TURN!");

			// Check for win condition
			if (playerHand.Count == 0) {
				TakiLogger.LogGameState ("Player wins - hand is empty!");
				gameState.DeclareWinner (PlayerType.Human);
				return;
			}

			OnCardPlayed?.Invoke (card);
			TakiLogger.LogTurnFlow ("CARD PLAY COMPLETE - WAITING FOR END TURN");
		}

		/// <summary>
		/// Draw card with strict flow control
		/// </summary>
		void DrawCardWithStrictFlow () {
			TakiLogger.LogCardPlay ("DRAWING CARD WITH STRICT FLOW");

			CardData drawnCard = deckManager?.DrawCard ();
			if (drawnCard != null) {
				playerHand.Add (drawnCard);

				TakiLogger.LogCardPlay ($"Player drew: {drawnCard.GetDisplayText ()}");

				// Update visual hands
				UpdateAllUI ();
				RefreshPlayerHandStates ();

				// Mark action taken and update flow
				hasPlayerTakenAction = true;
				canPlayerPlay = false;
				canPlayerDraw = false;
				canPlayerEndTurn = true;

				// FIXED: UI already disabled buttons immediately on click, now enable END TURN
				TakiLogger.LogTurnFlow ("ENABLING END TURN button after successful draw");
				gameplayUI?.ForceEnableEndTurn ();
				gameplayUI?.ShowPlayerMessage ($"Drew: {drawnCard.GetDisplayText ()} - you must END TURN!");

				TakiLogger.LogTurnFlow ("Player has no more valid moves, must end turn");
			} else {
				TakiLogger.LogError ("Failed to draw card - deck may be empty", TakiLogger.LogCategory.CardPlay);
				gameplayUI?.ShowPlayerMessage ("Cannot draw card!");
			}
		}

		/// <summary>
		/// End player turn with strict flow control
		/// </summary>
		void EndPlayerTurnWithStrictFlow () {
			TakiLogger.LogTurnFlow ("ENDING PLAYER TURN - STRICT FLOW COMPLETE");

			// Clear any selected cards
			playerHandManager?.ClearSelection ();

			// Reset turn flow state for next turn
			ResetTurnFlowState ();

			// No need to disable buttons - they were already disabled immediately on click
			// Turn state change will re-enable appropriate buttons for next player

			if (turnManager != null) {
				TakiLogger.LogTurnFlow ("Calling TurnManager.EndTurn()");
				turnManager.EndTurn ();
			} else {
				TakiLogger.LogError ("Cannot end turn: TurnManager is null!", TakiLogger.LogCategory.TurnFlow);
			}
		}

		/// <summary>
		/// ENHANCED: Comprehensive card effect rule logging
		/// </summary>
		void LogCardEffectRules (CardData card) {
			switch (card.cardType) {
				case CardType.Number:
					TakiLogger.LogRules ($"NUMBER card {card.GetDisplayText ()} played - basic card behavior");
					gameplayUI?.ShowPlayerMessage ($"Played NUMBER {card.GetDisplayText ()}");
					TakiLogger.LogRules ("Player must END turn");
					break;

				case CardType.Plus:
					TakiLogger.LogRules ("PLUS card played - Must either PLAY 1 more card, OR DRAW a card");
					gameplayUI?.ShowPlayerMessage ("PLUS: PLAY or DRAW 1 card");
					TakiLogger.LogRules ("Player must PLAY or DRAW another card (for now: must end turn)");
					break;

				case CardType.Stop:
					TakiLogger.LogRules ("STOP card played - Opponent's next turn is skipped");
					gameplayUI?.ShowPlayerMessage ("STOP: Opponent's turn skipped");
					TakiLogger.LogRules ("Opponent's turn is stopped/skipped, Player starts another NEW turn (for now: must end turn)");
					break;

				case CardType.ChangeDirection:
					TakiLogger.LogRules ("CHANGE DIRECTION card played - Turn direction changes");
					string oldDirection = gameState?.turnDirection.ToString () ?? "Unknown";
					if (gameState != null) gameState.ChangeTurnDirection ();
					string newDirection = gameState?.turnDirection.ToString () ?? "Unknown";
					gameplayUI?.ShowPlayerMessage ($"DIRECTION: {oldDirection} to {newDirection}");
					TakiLogger.LogGameState ($"Direction changed from {oldDirection} to {newDirection}");
					break;

				case CardType.PlusTwo:
					TakiLogger.LogRules ("PLUS TWO card played - Opponent draws 2 cards");
					gameplayUI?.ShowPlayerMessage ("PLUS TWO: Opponent draws 2 cards");
					TakiLogger.LogRules ("Player must DRAW 1x2=2 cards (for now: must end turn)");
					break;

				case CardType.Taki:
					TakiLogger.LogRules ($"TAKI card played - Player may play series of {card.color} cards");
					gameplayUI?.ShowPlayerMessage ($"TAKI: May play series of {card.color} cards");
					TakiLogger.LogRules ($"Player may PLAY a series of cards the color of {card.color} (for now: must end turn)");
					break;

				case CardType.ChangeColor:
					TakiLogger.LogRules ("CHANGE COLOR card played - Player must choose new color");
					gameplayUI?.ShowPlayerMessage ("CHANGE COLOR: Must choose new color");
					TakiLogger.LogRules ("Player must choose a color (for now: ColorSelectionPanel will not appear, must end turn)");
					break;

				case CardType.SuperTaki:
					TakiLogger.LogRules ("SUPER TAKI card played - Player may play series of any color");
					gameplayUI?.ShowPlayerMessage ("SUPER TAKI: May play series of any color");
					TakiLogger.LogRules ("Player may PLAY a series of cards of any color (for now: must end turn)");
					break;

				default:
					TakiLogger.LogWarning ($"Unknown card type: {card.cardType}", TakiLogger.LogCategory.Rules);
					gameplayUI?.ShowPlayerMessage ($"Unknown card: {card.GetDisplayText ()}");
					break;
			}

			TakiLogger.LogRules ("Player has no more valid moves, must end turn");
		}

		/// <summary>
		/// Handle color selection by player
		/// </summary>
		/// <param name="selectedColor">Color selected by player</param>
		void OnColorSelectedByPlayer (CardColor selectedColor) {
			if (gameState != null) {
				gameState.ChangeActiveColor (selectedColor);
				gameState.ChangeInteractionState (InteractionState.Normal);
			}
			TakiLogger.LogGameState ($"Player selected color: {selectedColor}");
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
		/// Handle player playing a card with comprehensive error checking
		/// </summary>
		public void PlayCard (CardData card) {
			TakiLogger.LogCardPlay ($"PLAY CARD: {card?.GetDisplayText ()}");

			if (card == null) {
				TakiLogger.LogError ("PlayCard called with null card", TakiLogger.LogCategory.CardPlay);
				return;
			}

			if (!isGameActive) {
				TakiLogger.LogWarning ("Cannot play card: Game not active", TakiLogger.LogCategory.CardPlay);
				gameplayUI?.ShowPlayerMessage ("Game not active!");
				return;
			}

			if (!gameState.CanPlayerAct ()) {
				TakiLogger.LogWarning ($"Cannot play card: Player cannot act. State: {gameState.turnState}", TakiLogger.LogCategory.CardPlay);
				gameplayUI?.ShowPlayerMessage ("Not your turn!");
				return;
			}

			// Validate the move
			CardData topCard = GetTopDiscardCard ();
			if (topCard == null) {
				TakiLogger.LogError ("Cannot play card: No top discard card available", TakiLogger.LogCategory.CardPlay);
				gameplayUI?.ShowPlayerMessage ("Game error - no discard pile!");
				return;
			}

			bool isValidMove = gameState.IsValidMove (card, topCard);
			TakiLogger.LogRules ($"Rule validation: {card.GetDisplayText ()} on {topCard.GetDisplayText ()} = {isValidMove}");

			if (!isValidMove) {
				TakiLogger.LogWarning ($"Invalid move: Cannot play {card.GetDisplayText ()} on {topCard.GetDisplayText ()}", TakiLogger.LogCategory.Rules);
				gameplayUI?.ShowPlayerMessage ($"Invalid move! Cannot play {card.GetDisplayText ()}");
				return;
			}

			// Remove card from player's hand
			bool removed = playerHand.Remove (card);
			TakiLogger.LogCardPlay ($"Card removed from hand: {removed}");

			// Clear selection in visual hand
			if (playerHandManager != null) {
				playerHandManager.ClearSelection ();
			}

			// Discard the card
			if (deckManager != null) {
				deckManager.DiscardCard (card);
				TakiLogger.LogCardPlay ($"Card discarded to pile: {card.GetDisplayText ()}");
			}

			// Update active color
			gameState.UpdateActiveColorFromCard (card);
			TakiLogger.LogGameState ($"Active color updated to: {gameState.activeColor}");

			// Handle special card effects
			HandleSpecialCardEffects (card);

			// Update UI (this will now update visual hands)
			UpdateAllUI ();

			// FIXED: Force refresh playable states after play
			Invoke (nameof (RefreshPlayerHandStates), 0.05f);

			// Check for win condition
			if (playerHand.Count == 0) {
				TakiLogger.LogGameState ("Player wins - hand is empty!");
				gameState.DeclareWinner (PlayerType.Human);
				return;
			}

			OnCardPlayed?.Invoke (card);

			// Show message instead of auto-ending turn
			gameplayUI?.ShowPlayerMessage ($"Played {card.GetDisplayText ()} - Click 'End Turn' when ready");

			TakiLogger.LogCardPlay ($"CARD PLAY COMPLETE: {card.GetDisplayText ()}");
		}

		/// <summary>
		/// LEGACY: Handle player drawing a card No automatic turn ending
		/// </summary>
		public void DrawCard () {
			TakiLogger.LogCardPlay ("DRAW CARD");

			if (!isGameActive || !gameState.CanPlayerAct ()) {
				TakiLogger.LogWarning ("Cannot draw card: Not player's turn or game not active", TakiLogger.LogCategory.CardPlay);
				gameplayUI?.ShowPlayerMessage ("Not your turn!");
				return;
			}

			CardData drawnCard = deckManager.DrawCard ();
			if (drawnCard != null) {
				playerHand.Add (drawnCard);

				UpdateAllUI ();

				// Force refresh playable states after drawing
				Invoke (nameof (RefreshPlayerHandStates), 0.05f);

				gameplayUI.ShowPlayerMessage ($"Drew: {drawnCard.GetDisplayText ()} - Click 'End Turn' when ready");
				TakiLogger.LogCardPlay ($"Player drew: {drawnCard.GetDisplayText ()} - turn continues");
			} else {
				TakiLogger.LogError ("Failed to draw card - deck may be empty", TakiLogger.LogCategory.CardPlay);
				gameplayUI?.ShowPlayerMessage ("Cannot draw card!");
			}

			TakiLogger.LogCardPlay ("DRAW COMPLETE - TURN CONTINUES");
		}

		/// <summary>
		/// FIXED: End player turn with comprehensive logging
		/// </summary>
		void EndPlayerTurn () {
			TakiLogger.LogTurnFlow ("ENDING PLAYER TURN");

			// Clear any selected cards
			if (playerHandManager != null) {
				playerHandManager.ClearSelection ();
			}

			if (turnManager != null) {
				TakiLogger.LogTurnFlow ("Calling TurnManager.EndTurn()");
				turnManager.EndTurn ();
			} else {
				TakiLogger.LogError ("Cannot end turn: TurnManager is null!", TakiLogger.LogCategory.TurnFlow);
			}
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

		/// <summary>
		/// Handle special card effects
		/// </summary>
		/// <param name="card">Card that was played</param>
		void HandleSpecialCardEffects (CardData card) {
			switch (card.cardType) {
				case CardType.Stop:
					// Skip opponent's turn
					if (turnManager != null) {
						turnManager.SkipTurn ();
					}
					gameplayUI.ShowPlayerMessage ("Computer's turn skipped!");
					break;

				case CardType.ChangeColor:
					// Trigger color selection
					gameState.ChangeInteractionState (InteractionState.ColorSelection);
					gameplayUI.ShowColorSelection (true);
					break;

				case CardType.PlusTwo:
					// Make opponent draw 2 cards
					MakeOpponentDrawCards (2);
					gameplayUI.ShowPlayerMessage ("Computer draws 2 cards!");
					break;

				case CardType.Plus:
					// Make opponent draw 1 card
					MakeOpponentDrawCards (1);
					gameplayUI.ShowPlayerMessage ("Computer draws 1 card!");
					break;

				case CardType.ChangeDirection:
					// Change turn direction
					if (gameState != null) {
						gameState.ChangeTurnDirection ();
					}
					gameplayUI.ShowPlayerMessage ("Turn direction changed!");
					break;

					// TODO: Implement Taki and SuperTaki in later milestones
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
			}

			// Update visual card displays
			UpdateVisualHands ();
		}

		// ===== EVENT HANDLERS =====

		/// <summary>
		/// Turn state change handler with forced UI sync
		/// ENHANCED: Turn state change handler with strict flow control
		/// </summary>
		void OnTurnStateChanged (TurnState newTurnState) {
			TakiLogger.LogGameState ($"Turn state changed to {newTurnState}");

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

		/// <summary>
		/// Delayed button synchronization method
		/// </summary>
		void DelayedButtonSync () {
			if (gameState != null && gameState.CanPlayerAct () && gameplayUI != null) {
				gameplayUI.UpdateButtonStates (true);
				TakiLogger.LogUI ("DELAYED button sync - ensured buttons are ENABLED");
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
		/// AI card selection handler with validation
		/// </summary>
		void OnAICardSelected (CardData card) {
			TakiLogger.LogAI ("AI SELECTED CARD: " + (card != null ? card.GetDisplayText () : "NULL"));

			if (card == null || computerAI == null) {
				TakiLogger.LogError ("AI selected null card or computerAI is null!", TakiLogger.LogCategory.AI);
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

			// Log AI card effects (simplified)
			TakiLogger.LogAI ($"AI played {card.cardType}: {card.GetDisplayText ()}");

			UpdateAllUI ();

			if (computerAI.HandSize == 0) {
				TakiLogger.LogGameState ("Computer wins - hand is empty!");
				if (gameState != null) {
					gameState.DeclareWinner (PlayerType.Computer);
				}
				return;
			}

			TakiLogger.LogAI ("Ending computer turn - switching to human");
			if (turnManager != null) {
				turnManager.EndTurn ();
			}
		}

		/// <summary>
		/// AI draw card handler
		/// </summary>
		void OnAIDrawCard () {
			TakiLogger.LogAI ("AI DRAWING CARD");

			if (deckManager == null || computerAI == null) {
				TakiLogger.LogError ("AI draw card but components are null!", TakiLogger.LogCategory.AI);
				return;
			}

			CardData drawnCard = deckManager.DrawCard ();
			if (drawnCard != null) {
				computerAI.AddCardToHand (drawnCard);
				TakiLogger.LogAI ("AI drew card: " + drawnCard.GetDisplayText ());
				UpdateAllUI ();
			} else {
				TakiLogger.LogError ("AI could not draw card - deck empty?", TakiLogger.LogCategory.AI);
			}

			TakiLogger.LogAI ("Ending computer turn after draw");
			if (turnManager != null) {
				turnManager.EndTurn ();
			}
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
		/// Verify turn switch after AI plays
		/// </summary>
		void VerifyTurnSwitch () {
			if (gameState != null) {
				TakiLogger.LogDiagnostics ($"Turn verification - Current state: {gameState.turnState}");
				if (gameState.turnState != TurnState.PlayerTurn) {
					TakiLogger.LogError ("TURN SWITCH FAILED - Still not player turn!", TakiLogger.LogCategory.TurnManagement);
				} else {
					TakiLogger.LogDiagnostics ("Turn switch successful - Player turn active");
					// Force UI sync if needed
					ForceUISync ();
				}
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

		// Turn flow properties
		public bool HasPlayerTakenAction => hasPlayerTakenAction;
		public bool CanPlayerDrawCard => canPlayerDraw;
		public bool CanPlayerPlayCard => canPlayerPlay;
		public bool CanPlayerEndTurn => canPlayerEndTurn;

		// Pause/Game End Properties 
		public bool IsGamePaused => pauseManager != null && pauseManager.IsGamePaused;
		public bool IsGameEndProcessed => gameEndManager != null && gameEndManager.IsGameEndProcessed;
		public bool IsExitValidationActive => exitValidationManager != null && exitValidationManager.IsExitValidationActive;
	}
}