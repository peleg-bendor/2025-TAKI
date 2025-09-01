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
			// ONLY validate components exist - DON'T initialize game systems yet
			ValidateAndConnectComponents ();
		}

		/// <summary>
		/// Validate components exist and connect basic references
		/// </summary>
		void ValidateAndConnectComponents () {
			Debug.Log ("Validating and connecting components...");

			// Validate components exist
			if (!ValidateComponents ()) {
				Debug.LogError ("GameManager: Missing required components!");
				return;
			}

			ConnectComponentReferences ();
			areComponentsValidated = true;
			Debug.Log ("Components validated and connected - Ready for game mode selection");
		}

		/// <summary>
		/// Initialize game systems for single player - called by MenuNavigation
		/// THIS is where the real initialization happens
		/// </summary>
		public void InitializeSinglePlayerSystems () {
			if (!areComponentsValidated) {
				Debug.LogError ("Cannot initialize: Components not validated!");
				return;
			}

			Debug.Log ("Initializing single player game systems...");

			// Connect events between systems
			ConnectEvents ();

			// Initialize visual card system
			InitializeVisualCardSystem ();

			// Initialize UI for gameplay
			if (gameplayUI != null) {
				gameplayUI.ResetUIForNewGame ();
			}

			areSystemsInitialized = true;
			Debug.Log ("Single player systems initialized - Ready to start game");
		}

		/// <summary>
		/// PUBLIC METHOD: Start a new single player game - Called by MenuNavigation
		/// </summary>
		public void StartNewSinglePlayerGame () {
			// Initialize systems if not already done
			if (!areSystemsInitialized) {
				InitializeSinglePlayerSystems ();
			}

			Debug.Log ("Starting new single player game...");

			if (deckManager == null) {
				Debug.LogError ("Cannot start game: DeckManager not assigned!");
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
			Debug.Log ("Multiplayer systems initialization - Not yet implemented");
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

			Debug.Log ("=== TURN FLOW STATE RESET ===");
		}

		/// <summary>
		/// ENHANCED: Start player turn with strict flow control
		/// </summary>
		void StartPlayerTurnFlow () {
			Debug.Log ("=== STARTING PLAYER TURN WITH STRICT FLOW ===");

			// Reset turn flow state
			hasPlayerTakenAction = false;
			canPlayerDraw = true;
			canPlayerPlay = true;
			canPlayerEndTurn = false;

			// Check if player has valid cards
			int validCardCount = CountPlayableCards ();

			if (validCardCount == 0) {
				// Player has NO valid cards - must draw
				Debug.Log ("RULE: Player has no valid cards, must draw a card");
				gameplayUI?.ShowComputerMessage ("No valid cards - you must DRAW a card!");
				gameplayUI?.UpdateStrictButtonStates (false, true, false); // No Play, Yes Draw, No EndTurn
			} else {
				// Player has valid cards - can play or draw
				Debug.Log ($"RULE: Player has {validCardCount} valid cards, may PLAY or DRAW a card");
				gameplayUI?.ShowComputerMessage ($"You have {validCardCount} valid moves - PLAY a card or DRAW");
				gameplayUI?.UpdateStrictButtonStates (true, true, false); // Yes Play, Yes Draw, No EndTurn
			}

			// Update visual card states
			RefreshPlayerHandStates ();
		}

		// ===== PLAYER ACTION HANDLERS =====

		/// <summary>
		/// Handle play card button clicked (No Auto-Play)
		/// Play Card button only works with explicit selection
		/// ENHANCED: Handle play card with strict flow control
		/// </summary>
		void OnPlayCardButtonClicked () {
			Debug.Log ("=== PLAY CARD BUTTON CLICKED - STRICT FLOW ===");

			if (!isGameActive || !gameState.CanPlayerAct ()) {
				Debug.LogWarning ("Cannot play card: Game not active or not player turn");
				gameplayUI?.ShowComputerMessage ("Not your turn!");
				return;
			}

			if (!canPlayerPlay) {
				Debug.LogWarning ("Cannot play card: Player already took action");
				gameplayUI?.ShowComputerMessage ("You already took an action - END TURN!");
				return;
			}

			// Get selected card
			CardData cardToPlay = playerHandManager?.GetSelectedCard ();

			if (cardToPlay != null) {
				Debug.Log ($"Attempting to play selected card: {cardToPlay.GetDisplayText ()}");
				PlayCardWithStrictFlow (cardToPlay);
			} else {
				int playableCount = CountPlayableCards ();
				if (playableCount > 0) {
					gameplayUI?.ShowComputerMessage ($"Please select a card! You have {playableCount} valid moves.");
				} else {
					gameplayUI?.ShowComputerMessage ("No valid moves - try drawing a card!");
				}
				Debug.Log ("No card selected - player must choose explicitly");
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
                Debug.Log("No cards in player hand");
                return null;
            }

            CardData topCard = GetTopDiscardCard();
            if (topCard == null) {
                Debug.LogError("Cannot find valid card - no top discard card");
                return null;
            }

            Debug.Log($"Searching for valid card to play on: {topCard.GetDisplayText()} with active color: {gameState.activeColor}");

            foreach (CardData card in playerHand) {
                if (gameState?.IsValidMove(card, topCard) == true) {
                    Debug.Log($"Found valid card: {card.GetDisplayText()}");
                    return card;
                }
            }

            Debug.Log("No valid cards found in hand");
            return null;
        }

		/// <summary>
		/// Get top discard card
		/// </summary>
		public CardData GetTopDiscardCard () {
			if (deckManager == null) {
				Debug.LogError ("GetTopDiscardCard: DeckManager is null!");
				return null;
			}

			return deckManager.GetTopDiscardCard ();
		}

		/// <summary>
		/// Handle draw card button clicked
		/// ENHANCED: Handle draw card with strict flow control
		/// </summary>
		void OnDrawCardButtonClicked () {
			Debug.Log ("=== DRAW CARD BUTTON CLICKED - STRICT FLOW ===");

			if (!isGameActive || !gameState.CanPlayerAct ()) {
				Debug.LogWarning ("Cannot draw card: Game not active or not player turn");
				gameplayUI?.ShowComputerMessage ("Not your turn!");
				return;
			}

			if (!canPlayerDraw) {
				Debug.LogWarning ("Cannot draw card: Player already took action");
				gameplayUI?.ShowComputerMessage ("You already took an action - END TURN!");
				return;
			}

			DrawCardWithStrictFlow ();
		}

		/// <summary>
		/// Handle end turn button clicked
		/// ENHANCED: Handle end turn with strict flow control
		/// </summary>
		void OnEndTurnButtonClicked () {
			Debug.Log ("=== END TURN BUTTON CLICKED - STRICT FLOW ===");

			if (!canPlayerEndTurn) {
				Debug.LogWarning ("Cannot end turn: Player has not taken an action yet");
				gameplayUI?.ShowComputerMessage ("You must take an action first (PLAY or DRAW)!");
				return;
			}

			EndPlayerTurnWithStrictFlow ();
		}

		/// <summary>
		/// Play card with comprehensive special card logging
		/// </summary>
		void PlayCardWithStrictFlow (CardData card) {
			Debug.Log ($"=== PLAYING CARD WITH STRICT FLOW: {card.GetDisplayText ()} ===");

			// Validate the move
			CardData topCard = GetTopDiscardCard ();
			if (topCard == null) {
				Debug.LogError ("Cannot play card: No top discard card available");
				gameplayUI?.ShowComputerMessage ("Game error - no discard pile!");
				return;
			}

			bool isValidMove = gameState.IsValidMove (card, topCard);
			if (!isValidMove) {
				Debug.LogWarning ($"Invalid move: Cannot play {card.GetDisplayText ()} on {topCard.GetDisplayText ()}");
				gameplayUI?.ShowComputerMessage ($"Invalid move! Cannot play {card.GetDisplayText ()}");
				return;
			}

			// Remove card from player's hand
			bool removed = playerHand.Remove (card);
			if (!removed) {
				Debug.LogError ("Could not remove card from player hand!");
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
			Debug.Log ("ENABLING END TURN button after successful card play");
			gameplayUI?.ForceEnableEndTurn ();
			gameplayUI?.ShowComputerMessage ("Card played - you must END TURN!");

			// Check for win condition
			if (playerHand.Count == 0) {
				Debug.Log ("Player wins - hand is empty!");
				gameState.DeclareWinner (PlayerType.Human);
				return;
			}

			OnCardPlayed?.Invoke (card);
			Debug.Log ($"=== CARD PLAY COMPLETE - WAITING FOR END TURN ===");
		}

		/// <summary>
		/// Draw card with strict flow control
		/// </summary>
		void DrawCardWithStrictFlow () {
			Debug.Log ("=== DRAWING CARD WITH STRICT FLOW ===");

			CardData drawnCard = deckManager?.DrawCard ();
			if (drawnCard != null) {
				playerHand.Add (drawnCard);

				Debug.Log ($"Player drew: {drawnCard.GetDisplayText ()}");

				// Update visual hands
				UpdateAllUI (); 
				RefreshPlayerHandStates ();

				// Mark action taken and update flow
				hasPlayerTakenAction = true;
				canPlayerPlay = false;
				canPlayerDraw = false;
				canPlayerEndTurn = true;

				// FIXED: UI already disabled buttons immediately on click, now enable END TURN
				Debug.Log ("ENABLING END TURN button after successful draw");
				gameplayUI?.ForceEnableEndTurn ();
				gameplayUI?.ShowComputerMessage ($"Drew: {drawnCard.GetDisplayText ()} - you must END TURN!");

				Debug.Log ("RULE: Player has no more valid moves, must end turn");
			} else {
				Debug.LogError ("Failed to draw card - deck may be empty");
				gameplayUI?.ShowComputerMessage ("Cannot draw card!");
			}
		}

		/// <summary>
		/// End player turn with strict flow control
		/// </summary>
		void EndPlayerTurnWithStrictFlow () {
			Debug.Log ("=== ENDING PLAYER TURN - STRICT FLOW COMPLETE ===");

			// Clear any selected cards
			playerHandManager?.ClearSelection ();

			// Reset turn flow state for next turn
			ResetTurnFlowState ();

			// No need to disable buttons - they were already disabled immediately on click
			// Turn state change will re-enable appropriate buttons for next player

			if (turnManager != null) {
				Debug.Log ("Calling TurnManager.EndTurn()");
				turnManager.EndTurn ();
			} else {
				Debug.LogError ("Cannot end turn: TurnManager is null!");
			}
		}

		/// <summary>
		/// ENHANCED: Comprehensive card effect rule logging
		/// </summary>
		void LogCardEffectRules (CardData card) {
			switch (card.cardType) {
				case CardType.Number:
					Debug.Log ($"RULE: NUMBER card {card.GetDisplayText ()} played - basic card behavior");
					gameplayUI?.ShowComputerMessage ($"Played NUMBER {card.GetDisplayText ()}");
					Debug.Log ("RULE: Player must END turn");
					break;

				case CardType.Plus:
					Debug.Log ($"RULE: PLUS card played - Must either PLAY 1 more card, OR DRAW a card");
					gameplayUI?.ShowComputerMessage ("PLUS: PLAY or DRAW 1 card");
					Debug.Log ("RULE: Player must PLAY or DRAW another card (for now: must end turn)");
					break;

				case CardType.Stop:
					Debug.Log ($"RULE: STOP card played - Opponent's next turn is skipped");
					gameplayUI?.ShowComputerMessage ("STOP: Opponent's turn skipped");
					Debug.Log ("RULE: Opponent's turn is stopped/skipped, Player starts another NEW turn (for now: must end turn)");
					break;

				case CardType.ChangeDirection:
					Debug.Log ($"RULE: CHANGE DIRECTION card played - Turn direction changes");
					string oldDirection = gameState?.turnDirection.ToString () ?? "Unknown";
					if (gameState != null) gameState.ChangeTurnDirection ();
					string newDirection = gameState?.turnDirection.ToString () ?? "Unknown";
					gameplayUI?.ShowComputerMessage ($"DIRECTION: {oldDirection} to {newDirection}");
					Debug.Log ($"RULE: Direction changed from {oldDirection} to {newDirection}");
					break;

				case CardType.PlusTwo:
					Debug.Log ($"RULE: PLUS TWO card played - Opponent draws 2 cards");
					gameplayUI?.ShowComputerMessage ("PLUS TWO: Opponent draws 2 cards");
					Debug.Log ("RULE: Player must DRAW 1x2=2 cards (for now: must end turn)");
					break;

				case CardType.Taki:
					Debug.Log ($"RULE: TAKI card played - Player may play series of {card.color} cards");
					gameplayUI?.ShowComputerMessage ($"TAKI: May play series of {card.color} cards");
					Debug.Log ($"RULE: Player may PLAY a series of cards the color of {card.color} (for now: must end turn)");
					break;

				case CardType.ChangeColor:
					Debug.Log ($"RULE: CHANGE COLOR card played - Player must choose new color");
					gameplayUI?.ShowComputerMessage ("CHANGE COLOR: Must choose new color");
					Debug.Log ("RULE: Player must choose a color (for now: ColorSelectionPanel will not appear, must end turn)");
					break;

				case CardType.SuperTaki:
					Debug.Log ($"RULE: SUPER TAKI card played - Player may play series of any color");
					gameplayUI?.ShowComputerMessage ("SUPER TAKI: May play series of any color");
					Debug.Log ("RULE: Player may PLAY a series of cards of any color (for now: must end turn)");
					break;

				default:
					Debug.LogWarning ($"Unknown card type: {card.cardType}");
					gameplayUI?.ShowComputerMessage ($"Unknown card: {card.GetDisplayText ()}");
					break;
			}

			Debug.Log ("RULE: Player has no more valid moves, must end turn");
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
			Debug.Log ($"Player selected color: {selectedColor}");
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

			Debug.Log ($"Game started! Player: {player1Hand.Count} cards, Computer: {player2Hand.Count} cards");
		}

		/// <summary>
		/// Handle player playing a card with comprehensive error checking
		/// </summary>
		public void PlayCard (CardData card) {
			Debug.Log ($"=== PLAY CARD: {card?.GetDisplayText ()} ===");

			if (card == null) {
				Debug.LogError ("PlayCard called with null card");
				return;
			}

			if (!isGameActive) {
				Debug.LogWarning ("Cannot play card: Game not active");
				gameplayUI?.ShowComputerMessage ("Game not active!");
				return;
			}

			if (!gameState.CanPlayerAct ()) {
				Debug.LogWarning ($"Cannot play card: Player cannot act. State: {gameState.turnState}");
				gameplayUI?.ShowComputerMessage ("Not your turn!");
				return;
			}

			// Validate the move
			CardData topCard = GetTopDiscardCard ();
			if (topCard == null) {
				Debug.LogError ("Cannot play card: No top discard card available");
				gameplayUI?.ShowComputerMessage ("Game error - no discard pile!");
				return;
			}

			bool isValidMove = gameState.IsValidMove (card, topCard);
			Debug.Log ($"Rule validation: {card.GetDisplayText ()} on {topCard.GetDisplayText ()} = {isValidMove}");

			if (!isValidMove) {
				Debug.LogWarning ($"Invalid move: Cannot play {card.GetDisplayText ()} on {topCard.GetDisplayText ()}");
				gameplayUI?.ShowComputerMessage ($"Invalid move! Cannot play {card.GetDisplayText ()}");
				return;
			}

			// Remove card from player's hand
			bool removed = playerHand.Remove (card);
			Debug.Log ($"Card removed from hand: {removed}");

			// Clear selection in visual hand
			if (playerHandManager != null) {
				playerHandManager.ClearSelection ();
			}

			// Discard the card
			if (deckManager != null) {
				deckManager.DiscardCard (card);
				Debug.Log ($"Card discarded to pile: {card.GetDisplayText ()}");
			}

			// Update active color
			gameState.UpdateActiveColorFromCard (card);
			Debug.Log ($"Active color updated to: {gameState.activeColor}");

			// Handle special card effects
			HandleSpecialCardEffects (card);

			// Update UI (this will now update visual hands)
			UpdateAllUI ();

			// FIXED: Force refresh playable states after play
			Invoke (nameof (RefreshPlayerHandStates), 0.05f);

			// Check for win condition
			if (playerHand.Count == 0) {
				Debug.Log ("Player wins - hand is empty!");
				gameState.DeclareWinner (PlayerType.Human);
				return;
			}

			OnCardPlayed?.Invoke (card);

			// FIXED: Show message instead of auto-ending turn
			gameplayUI?.ShowComputerMessage ($"Played {card.GetDisplayText ()} - Click 'End Turn' when ready");

			Debug.Log ($"=== CARD PLAY COMPLETE: {card.GetDisplayText ()} ===");
		}

		/// <summary>
		/// LEGACY: Handle player drawing a card No automatic turn ending
		/// </summary>
		public void DrawCard () {
			Debug.Log ("=== DRAW CARD ===");

			if (!isGameActive || !gameState.CanPlayerAct ()) {
				Debug.LogWarning ("Cannot draw card: Not player's turn or game not active");
				gameplayUI?.ShowComputerMessage ("Not your turn!");
				return;
			}

			CardData drawnCard = deckManager.DrawCard ();
			if (drawnCard != null) {
				playerHand.Add (drawnCard);

				UpdateAllUI ();

				// FIXED: Force refresh playable states after drawing
				Invoke (nameof (RefreshPlayerHandStates), 0.05f);

				gameplayUI.ShowComputerMessage ($"Drew: {drawnCard.GetDisplayText ()} - Click 'End Turn' when ready");
				Debug.Log ($"Player drew: {drawnCard.GetDisplayText ()} - turn continues");
			} else {
				Debug.LogError ("Failed to draw card - deck may be empty");
				gameplayUI?.ShowComputerMessage ("Cannot draw card!");
			}

			Debug.Log ("=== DRAW COMPLETE - TURN CONTINUES ===");
		}

		/// <summary>
		/// FIXED: End player turn with comprehensive logging
		/// </summary>
		void EndPlayerTurn () {
			Debug.Log ("=== ENDING PLAYER TURN ===");

			// Clear any selected cards
			if (playerHandManager != null) {
				playerHandManager.ClearSelection ();
			}

			if (turnManager != null) {
				Debug.Log ("Calling TurnManager.EndTurn()");
				turnManager.EndTurn ();
			} else {
				Debug.LogError ("Cannot end turn: TurnManager is null!");
			}
		}

		/// <summary>
		/// Force refresh of player hand playable states (delayed)
		/// </summary>
		void RefreshPlayerHandStates () {
			Debug.Log ("=== REFRESHING PLAYER HAND STATES ===");

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
					gameplayUI.ShowComputerMessage ("Computer's turn skipped!");
					break;

				case CardType.ChangeColor:
					// Trigger color selection
					gameState.ChangeInteractionState (InteractionState.ColorSelection);
					gameplayUI.ShowColorSelection (true);
					break;

				case CardType.PlusTwo:
					// Make opponent draw 2 cards
					MakeOpponentDrawCards (2);
					gameplayUI.ShowComputerMessage ("Computer draws 2 cards!");
					break;

				case CardType.Plus:
					// Make opponent draw 1 card
					MakeOpponentDrawCards (1);
					gameplayUI.ShowComputerMessage ("Computer draws 1 card!");
					break;

				case CardType.ChangeDirection:
					// Change turn direction
					if (gameState != null) {
						gameState.ChangeTurnDirection ();
					}
					gameplayUI.ShowComputerMessage ("Turn direction changed!");
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
				Debug.Log ($"Computer drew {drawnCards.Count} cards");
			} else {
				// Player draws cards
				List<CardData> drawnCards = deckManager.DrawCards (count);
				playerHand.AddRange (drawnCards);
				Debug.Log ($"Player drew {drawnCards.Count} cards");
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
			Debug.Log ($"GameManager: Turn state changed to {newTurnState}");

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
		/// ADDED: Delayed button synchronization method
		/// </summary>
		void DelayedButtonSync () {
			if (gameState != null && gameState.CanPlayerAct () && gameplayUI != null) {
				gameplayUI.UpdateButtonStates (true);
				Debug.Log ("DELAYED button sync - ensured buttons are ENABLED");
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
			Debug.Log ("=== COMPUTER TURN READY ===");

			if (computerAI == null || deckManager == null) {
				Debug.LogError ("Computer turn ready but components are null!");
				return;
			}

			CardData topCard = deckManager.GetTopDiscardCard ();
			if (topCard == null) {
				Debug.LogError ("Computer turn ready but no top discard card!");
				return;
			}

			Debug.Log ("Triggering AI decision for top card: " + topCard.GetDisplayText ());
			computerAI.MakeDecision (topCard);
		}

		/// <summary>
		/// AI card selection handler with validation
		/// </summary>
		void OnAICardSelected (CardData card) {
			Debug.Log ("=== AI SELECTED CARD: " + (card != null ? card.GetDisplayText () : "NULL") + " ===");

			if (card == null || computerAI == null) {
				Debug.LogError ("AI selected null card or computerAI is null!");
				return;
			}

			if (deckManager != null) {
				deckManager.DiscardCard (card);
				Debug.Log ("AI card discarded: " + card.GetDisplayText ());
			}

			if (gameState != null) {
				gameState.UpdateActiveColorFromCard (card);
				Debug.Log ("Active color updated to: " + gameState.activeColor);
			}

			// Log AI card effects (simplified)
			Debug.Log ($"AI played {card.cardType}: {card.GetDisplayText ()}");

			UpdateAllUI ();

			if (computerAI.HandSize == 0) {
				Debug.Log ("Computer wins - hand is empty!");
				if (gameState != null) {
					gameState.DeclareWinner (PlayerType.Computer);
				}
				return;
			}

			Debug.Log ("Ending computer turn - switching to human");
			if (turnManager != null) {
				turnManager.EndTurn ();
			}
		}

		/// <summary>
		/// AI draw card handler
		/// </summary>
		void OnAIDrawCard () {
			Debug.Log ("=== AI DRAWING CARD ===");

			if (deckManager == null || computerAI == null) {
				Debug.LogError ("AI draw card but components are null!");
				return;
			}

			CardData drawnCard = deckManager.DrawCard ();
			if (drawnCard != null) {
				computerAI.AddCardToHand (drawnCard);
				Debug.Log ("AI drew card: " + drawnCard.GetDisplayText ());
				UpdateAllUI ();
			} else {
				Debug.LogError ("AI could not draw card - deck empty?");
			}

			Debug.Log ("Ending computer turn after draw");
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
			if (gameplayUI != null) {
				gameplayUI.ShowWinnerAnnouncement (winner);
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
			Debug.Log ("Initializing visual card system...");

			if (playerHandManager == null || computerHandManager == null) {
				Debug.LogError ("HandManager references missing!");
				return;
			}

			playerHandManager.OnCardSelected += OnPlayerCardSelected;
			computerHandManager.OnCardSelected += OnComputerCardSelected;

			Debug.Log ("Visual card system initialized");
		}

		/// <summary>
		/// Handle player card selection from visual hand
		/// </summary>
		/// <param name="selectedCardController">Selected card controller</param>
		void OnPlayerCardSelected (CardController selectedCardController) {
			if (selectedCardController == null) {
				Debug.Log ("Player deselected card");
				return;
			}

			CardData selectedCard = selectedCardController.CardData;
			if (selectedCard != null) {
				Debug.Log ($"Player selected visual card: {selectedCard.GetDisplayText ()}");

				// Update UI feedback
				if (gameplayUI != null) {
					bool canPlay = gameState?.IsValidMove (selectedCard, GetTopDiscardCard ()) ?? false;
					string message = canPlay ? $"Selected: {selectedCard.GetDisplayText ()}" : "Invalid move!";
					gameplayUI.ShowComputerMessage (message);
				}
			}
		}

		/// <summary>
		/// Handle computer card selection (should not happen)
		/// </summary>
		void OnComputerCardSelected (CardController selectedCardController) {
			Debug.LogWarning ("Computer cards should not be selectable!");
		}

		/// <summary>
		/// Update visual hands with error checking
		/// </summary>
		void UpdateVisualHands () {
			try {
				// Update player hand visual
				if (playerHandManager != null && playerHand != null) {
					playerHandManager.UpdateHandDisplay (playerHand);
					Debug.Log ($"Updated player hand display: {playerHand.Count} cards");
				}

				// Update computer hand visual
				if (computerHandManager != null && computerAI != null) {
					List<CardData> computerHand = computerAI.GetHandCopy ();
					computerHandManager.UpdateHandDisplay (computerHand);
					Debug.Log ($"Updated computer hand display: {computerHand.Count} cards");
				}
			} catch (System.Exception e) {
				Debug.LogError ($"Error updating visual hands: {e.Message}");
			}
		}

		/// <summary>
		/// Validate all required components are assigned
		/// </summary>
		bool ValidateComponents () {
			bool isValid = true;

			if (gameState == null) {
				Debug.LogError ("GameManager: GameStateManager not assigned!");
				isValid = false;
			}

			if (turnManager == null) {
				Debug.LogError ("GameManager: TurnManager not assigned!");
				isValid = false;
			}

			if (computerAI == null) {
				Debug.LogError ("GameManager: BasicComputerAI not assigned!");
				isValid = false;
			}

			if (gameplayUI == null) {
				Debug.LogError ("GameManager: GameplayUIManager not assigned!");
				isValid = false;
			}

			if (deckManager == null) {
				Debug.LogError ("GameManager: DeckManager not assigned!");
				isValid = false;
			}

			if (playerHandManager == null) {
				Debug.LogError ("GameManager: PlayerHandManager not assigned!");
				isValid = false;
			}

			if (computerHandManager == null) {
				Debug.LogError ("GameManager: ComputerHandManager not assigned!");
				isValid = false;
			}

			return isValid;
		}

		// ===== DEBUG METHODS =====

		/// DEBUGGING: Force a new game start for testing
		/// </summary>
		[ContextMenu ("Force New Game Start")]
		public void ForceNewGameStart () {
			Debug.Log ("=== FORCING NEW GAME START ===");

			if (!areSystemsInitialized) {
				Debug.Log ("Initializing systems...");
				InitializeSinglePlayerSystems ();
			}

			Debug.Log ("Starting new game...");
			StartNewSinglePlayerGame ();
		}

		[ContextMenu ("Log Turn Flow State")]
		public void LogTurnFlowState () {
			Debug.Log ("=== TURN FLOW STATE DEBUG ===");
			Debug.Log ($"hasPlayerTakenAction: {hasPlayerTakenAction}");
			Debug.Log ($"canPlayerDraw: {canPlayerDraw}");
			Debug.Log ($"canPlayerPlay: {canPlayerPlay}");
			Debug.Log ($"canPlayerEndTurn: {canPlayerEndTurn}");
			Debug.Log ($"isGameActive: {isGameActive}");
		}

		/// <summary>
		/// DEBUGGING: Manual turn trigger for testing
		/// </summary>
		[ContextMenu ("Trigger Computer Turn")]
		public void TriggerComputerTurnManually () {
			Debug.Log ("=== MANUAL COMPUTER TURN TRIGGER ===");
			OnComputerTurnReady ();
		}

		/// <summary>
		/// Force UI sync - call this to fix button states after manual testing
		/// </summary>
		[ContextMenu ("Force UI Sync")]
		public void ForceUISync () {
			Debug.Log ("=== FORCING UI SYNCHRONIZATION ===");

			if (gameState == null || gameplayUI == null) {
				Debug.LogError ("Cannot sync UI - missing components");
				return;
			}

			// Force button state update based on current turn state
			bool shouldEnableButtons = gameState.CanPlayerAct ();
			gameplayUI.UpdateButtonStates (shouldEnableButtons);

			Debug.Log ($"UI synced - Turn: {gameState.turnState}, Buttons enabled: {shouldEnableButtons}");
		}

		/// <summary>
		/// Verify turn switch after AI plays
		/// </summary>
		void VerifyTurnSwitch () {
			if (gameState != null) {
				Debug.Log ($"Turn verification - Current state: {gameState.turnState}");
				if (gameState.turnState != TurnState.PlayerTurn) {
					Debug.LogError ("TURN SWITCH FAILED - Still not player turn!");
				} else {
					Debug.Log ("Turn switch successful - Player turn active");
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

		// ENHANCED: Turn flow properties
		public bool HasPlayerTakenAction => hasPlayerTakenAction;
		public bool CanPlayerDrawCard => canPlayerDraw;
		public bool CanPlayerPlayCard => canPlayerPlay;
		public bool CanPlayerEndTurn => canPlayerEndTurn;
	}
}