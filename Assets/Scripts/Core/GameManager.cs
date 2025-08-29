using UnityEngine;
using System.Collections.Generic;

namespace TakiGame {
	/// <summary>
	/// Main coordinator - Separate initialization from game start
	/// No longer initializes game systems until player chooses game mode
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
		/// Validate components exist and connect basic references - called at scene load
		/// NO game initialization, NO deck loading, NO UI setup
		/// </summary>
		void ValidateAndConnectComponents () {
			Debug.Log ("Validating and connecting components...");

			// Validate components exist
			if (!ValidateComponents ()) {
				Debug.LogError ("GameManager: Missing required components!");
				return;
			}

			// Connect basic component references (lightweight)
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

			isGameActive = false;
		}

		// ===== PLAYER ACTION HANDLERS =====

		/// <summary>
		/// Handle play card button clicked
		/// </summary>
		void OnPlayCardButtonClicked () {
			if (!isGameActive || !gameState.CanPlayerAct ()) {
				Debug.LogWarning ("Cannot play card: Not player's turn or game not active");
				gameplayUI.ShowComputerMessage ("Not your turn!");
				return;
			}

			// Get selected card from visual hand manager
			CardData cardToPlay = null;
			if (playerHandManager != null) {
				cardToPlay = playerHandManager.GetSelectedCard ();
			}

			//// Fallback to first valid card if no selection
			//if (cardToPlay == null) {
			//	cardToPlay = FindFirstValidCard ();
			//}

			if (cardToPlay != null) {
				PlayCard (cardToPlay);
			} else {
				Debug.LogWarning ("No valid cards to play!");
				gameplayUI.ShowComputerMessage ("No valid cards to play!");
			}
		}

		/// <summary>
		/// Handle draw card button clicked
		/// </summary>
		void OnDrawCardButtonClicked () {
			DrawCard ();
		}

		/// <summary>
		/// Handle end turn button clicked
		/// </summary>
		void OnEndTurnButtonClicked () {
			EndPlayerTurn ();
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
		/// Handle player playing a card
		/// </summary>
		/// <param name="card">Card being played</param>
		public void PlayCard (CardData card) {
			if (!isGameActive || !gameState.CanPlayerAct ()) {
				Debug.LogWarning ("Cannot play card: Not player's turn or game not active");
				return;
			}

			// Validate the move
			CardData topCard = deckManager.GetTopDiscardCard ();
			if (!gameState.IsValidMove (card, topCard)) {
				Debug.LogWarning ($"Invalid move: Cannot play {card.GetDisplayText ()}");
				gameplayUI.ShowComputerMessage ("Invalid move!");
				return;
			}

			// Remove card from player's hand
			playerHand.Remove (card);

			// Clear selection in visual hand
			if (playerHandManager != null) {
				playerHandManager.ClearSelection ();
			}

			// Discard the card
			deckManager.DiscardCard (card);

			// Update active color
			gameState.UpdateActiveColorFromCard (card);

			// Handle special card effects
			HandleSpecialCardEffects (card);

			// Update UI (this will now update visual hands)
			UpdateAllUI ();

			// Check for win condition
			if (playerHand.Count == 0) {
				gameState.DeclareWinner (PlayerType.Human);
				return;
			}

			OnCardPlayed?.Invoke (card);

			// End turn (unless card allows continued play)
			if (!card.isActiveCard) {
				EndPlayerTurn ();
			}

			Debug.Log ($"Player played: {card.GetDisplayText ()}");
		}

		/// <summary>
		/// Handle player drawing a card
		/// </summary>
		public void DrawCard () {
			if (!isGameActive || !gameState.CanPlayerAct ()) {
				Debug.LogWarning ("Cannot draw card: Not player's turn or game not active");
				return;
			}

			CardData drawnCard = deckManager.DrawCard ();
			if (drawnCard != null) {
				playerHand.Add (drawnCard);

				// Update visual hands
				UpdateVisualHands ();

				gameplayUI.ShowComputerMessage ($"You drew: {drawnCard.GetDisplayText ()}");
				Debug.Log ($"Player drew: {drawnCard.GetDisplayText ()}");
			}

			// End turn after drawing
			EndPlayerTurn ();
		}

		/// <summary>
		/// End the player's turn
		/// </summary>
		void EndPlayerTurn () {
			if (turnManager != null) {
				turnManager.EndTurn ();
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
		/// Update all UI elements with current game state
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

		void OnTurnStateChanged (TurnState newTurnState) {
			if (gameplayUI != null) {
				gameplayUI.UpdateTurnDisplay (newTurnState);
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

		void OnComputerTurnReady () {
			// Trigger AI decision making
			if (computerAI != null) {
				CardData topCard = deckManager.GetTopDiscardCard ();
				computerAI.MakeDecision (topCard);
			}
		}

		void OnAICardSelected (CardData card) {
			// AI selected a card to play
			if (computerAI != null) {
				deckManager.DiscardCard (card);
				gameState.UpdateActiveColorFromCard (card);
				HandleSpecialCardEffects (card);
				UpdateAllUI ();

				// Check for AI win
				if (computerAI.HandSize == 0) {
					gameState.DeclareWinner (PlayerType.Computer);
					return;
				}

				// End AI turn (unless card allows continued play)
				if (!card.isActiveCard) {
					turnManager.EndTurn ();
				}
			}
		}

		void OnAIDrawCard () {
			// AI decided to draw a card
			CardData drawnCard = deckManager.DrawCard ();
			if (drawnCard != null && computerAI != null) {
				computerAI.AddCardToHand (drawnCard);
				UpdateAllUI ();
			}

			// End AI turn after drawing
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
				// Auto-draw card on timeout
				DrawCard ();
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

			// Validate hand managers
			if (playerHandManager == null) {
				Debug.LogError ("GameManager: PlayerHandManager not assigned!");
				return;
			}

			if (computerHandManager == null) {
				Debug.LogError ("GameManager: ComputerHandManager not assigned!");
				return;
			}

			// Connect hand manager events
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
		/// Update visual hand displays to match current data
		/// </summary>
		void UpdateVisualHands () {
			// Update player hand visual
			if (playerHandManager != null && playerHand != null) {
				playerHandManager.UpdateHandDisplay (playerHand);
			}

			// Update computer hand visual
			if (computerHandManager != null && computerAI != null) {
				List<CardData> computerHand = computerAI.GetHandCopy ();
				computerHandManager.UpdateHandDisplay (computerHand);
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

		// ===== PUBLIC API =====

		public void RequestDrawCard () => DrawCard ();
		public void RequestPlayCard (CardData card) => PlayCard (card);
		public List<CardData> GetPlayerHand () => new List<CardData> (playerHand);
		public CardData GetTopDiscardCard () => deckManager?.GetTopDiscardCard ();
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
		public TurnState CurrentTurnState => gameState?.turnState ?? TurnState.Neutral;
		public InteractionState CurrentInteractionState => gameState?.interactionState ?? InteractionState.Normal;
		public GameStatus CurrentGameStatus => gameState?.gameStatus ?? GameStatus.Active;
	}
}