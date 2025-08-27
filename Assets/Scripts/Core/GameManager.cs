using UnityEngine;
using System.Collections.Generic;

namespace TakiGame {
	/// <summary>
	/// Main coordinator for all gameplay systems using multi-enum architecture
	/// Manages the overall game flow and connects all gameplay components
	/// </summary>
	public class GameManager : MonoBehaviour {

		[Header ("Component References")]
		[Tooltip ("Manages game state using multi-enum architecture")]
		public GameStateManager gameState;

		[Tooltip ("Manages turns and player switching")]
		public TurnManager turnManager;

		[Tooltip ("Computer AI decision making")]
		public BasicComputerAI computerAI;

		[Tooltip ("Gameplay UI updates")]
		public GameplayUIManager gameplayUI;

		[Tooltip ("Deck management system")]
		public DeckManager deckManager;

		[Header ("Game Setup")]
		[Tooltip ("Starting player for new games")]
		public PlayerType startingPlayer = PlayerType.Human;

		[Tooltip ("Player's hand of cards")]
		public List<CardData> playerHand = new List<CardData> ();

		// Events for external systems
		public System.Action OnGameStarted;
		public System.Action<PlayerType> OnGameEnded;
		public System.Action<PlayerType> OnTurnStarted;
		public System.Action<CardData> OnCardPlayed;

		// Game state
		private bool isGameInitialized = false;

		void Start () {
			InitializeGameSystems ();
		}

		/// <summary>
		/// Initialize all game systems and connections
		/// </summary>
		void InitializeGameSystems () {
			Debug.Log ("Initializing game systems...");

			// Validate components
			if (!ValidateComponents ()) {
				Debug.LogError ("GameManager: Missing required components!");
				return;
			}

			// Connect component dependencies
			ConnectComponentReferences ();

			// Connect events between systems
			ConnectEvents ();

			// Initialize UI
			if (gameplayUI != null) {
				gameplayUI.ResetUIForNewGame ();
			}

			Debug.Log ("Game systems initialized");
		}

		/// <summary>
		/// Connect references between components
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
		/// Connect events between all systems using new architecture
		/// </summary>
		void ConnectEvents () {
			// Deck Manager events
			if (deckManager != null) {
				deckManager.OnInitialGameSetup += OnInitialGameSetupComplete;
				deckManager.OnCardDrawn += OnCardDrawnFromDeck;
			}

			// Game State events - updated for multi-enum architecture
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
		}

		/// <summary>
		/// Start a new game
		/// </summary>
		public void StartNewGame () {
			Debug.Log ("Starting new game...");

			if (deckManager == null) {
				Debug.LogError ("Cannot start game: DeckManager not assigned!");
				return;
			}

			// Reset all systems
			ResetGameSystems ();

			// Setup initial game state through deck manager
			deckManager.SetupInitialGame ();
		}

		/// <summary>
		/// Reset all game systems for a new game
		/// </summary>
		void ResetGameSystems () {
			// Clear hands
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

			// Reset UI
			if (gameplayUI != null) {
				gameplayUI.ResetUIForNewGame ();
			}

			isGameInitialized = false;
		}

		/// <summary>
		/// Handle completion of initial game setup
		/// </summary>
		/// <param name="player1Hand">Player's initial hand</param>
		/// <param name="player2Hand">Computer's initial hand</param>
		/// <param name="startingCard">Starting discard card</param>
		void OnInitialGameSetupComplete (List<CardData> player1Hand, List<CardData> player2Hand, CardData startingCard) {
			// Assign hands
			playerHand = player1Hand;
			if (computerAI != null) {
				computerAI.AddCardsToHand (player2Hand);
			}

			// Set active color from starting card (using user's Wild initial approach)
			if (startingCard != null && gameState != null) {
				gameState.ChangeActiveColor (startingCard.color);
			}

			// Update UI
			UpdateAllUI ();

			// Start the first turn
			if (turnManager != null) {
				turnManager.InitializeTurns (startingPlayer);
			}

			isGameInitialized = true;
			OnGameStarted?.Invoke ();

			Debug.Log ($"Game started! Player: {player1Hand.Count} cards, Computer: {player2Hand.Count} cards");
		}

		/// <summary>
		/// Handle player playing a card
		/// </summary>
		/// <param name="card">Card being played</param>
		public void PlayCard (CardData card) {
			if (!isGameInitialized || !gameState.CanPlayerAct ()) {
				Debug.LogWarning ("Cannot play card: Not player's turn or game not initialized");
				return;
			}

			// Validate the move
			CardData topCard = deckManager.GetTopDiscardCard ();
			if (!gameState.IsValidMove (card, topCard)) {
				Debug.LogWarning ($"Invalid move: Cannot play {card.GetDisplayText ()}");
				if (gameplayUI != null) {
					gameplayUI.ShowTemporaryMessage ("Invalid move!", 2f);
				}
				return;
			}

			// Remove card from player's hand
			playerHand.Remove (card);

			// Discard the card
			deckManager.DiscardCard (card);

			// Update active color
			gameState.UpdateActiveColorFromCard (card);

			// Handle special card effects
			HandleSpecialCardEffects (card);

			// Update UI
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
			if (!isGameInitialized || !gameState.CanPlayerAct ()) {
				Debug.LogWarning ("Cannot draw card: Not player's turn or game not initialized");
				return;
			}

			CardData drawnCard = deckManager.DrawCard ();
			if (drawnCard != null) {
				playerHand.Add (drawnCard);
				UpdateAllUI ();
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
					break;

				case CardType.ChangeColor:
					// Trigger color selection (for now, random)
					SelectRandomColor ();
					break;

				case CardType.PlusTwo:
					// Make opponent draw 2 cards
					MakeOpponentDrawCards (2);
					break;

				case CardType.Plus:
					// Make opponent draw 1 card
					MakeOpponentDrawCards (1);
					break;

				case CardType.ChangeDirection:
					// Change turn direction
					if (gameState != null) {
						gameState.ChangeTurnDirection ();
					}
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
		/// Select random color (temporary implementation)
		/// TODO: Implement proper color selection UI in later milestone
		/// </summary>
		void SelectRandomColor () {
			CardColor [] colors = { CardColor.Red, CardColor.Blue, CardColor.Green, CardColor.Yellow };
			CardColor newColor = colors [Random.Range (0, colors.Length)];
			if (gameState != null) {
				gameState.ChangeActiveColor (newColor);
			}
			Debug.Log ($"Color changed to: {newColor}");
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
		}

		// Event handlers for system events using new architecture
		void OnTurnStateChanged (TurnState newTurnState) {
			if (gameplayUI != null) {
				gameplayUI.UpdateTurnDisplay (newTurnState);
			}
		}

		void OnInteractionStateChanged (InteractionState newInteractionState) {
			if (gameplayUI != null && gameState != null) {
				gameplayUI.UpdateGameStateDisplay (gameState.gameStatus, newInteractionState);
			}
		}

		void OnGameStatusChanged (GameStatus newGameStatus) {
			if (gameplayUI != null && gameState != null) {
				gameplayUI.UpdateGameStateDisplay (newGameStatus, gameState.interactionState);
			}
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
				gameplayUI.ShowTemporaryMessage (decision, 1.5f);
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

			return isValid;
		}

		// Public API for external systems
		public void RequestDrawCard () => DrawCard ();
		public void RequestPlayCard (CardData card) => PlayCard (card);
		public void RequestNewGame () => StartNewGame ();
		public List<CardData> GetPlayerHand () => new List<CardData> (playerHand);
		public CardData GetTopDiscardCard () => deckManager?.GetTopDiscardCard ();
		public bool CanPlayerAct () => gameState?.CanPlayerAct () ?? false;

		// Properties using new architecture
		public bool IsGameActive => gameState?.IsGameActive ?? false;
		public bool IsPlayerTurn => gameState?.IsPlayerTurn ?? false;
		public PlayerType CurrentPlayer => turnManager?.CurrentPlayer ?? PlayerType.Human;
		public int PlayerHandSize => playerHand.Count;
		public int ComputerHandSize => computerAI?.HandSize ?? 0;
		public CardColor ActiveColor => gameState?.activeColor ?? CardColor.Wild;
		public bool IsInitialized => isGameInitialized;
		public TurnState CurrentTurnState => gameState?.turnState ?? TurnState.Neutral;
		public InteractionState CurrentInteractionState => gameState?.interactionState ?? InteractionState.Normal;
		public GameStatus CurrentGameStatus => gameState?.gameStatus ?? GameStatus.Active;
	}
}