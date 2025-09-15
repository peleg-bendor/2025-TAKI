using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

namespace TakiGame {
	/// <summary>
	/// MILESTONE 1: Enhanced DeckManager with network coordination support
	/// Preserves all existing functionality while adding network-aware methods
	/// </summary>
	public class DeckManager : MonoBehaviour {

		[Header ("Component References")]
		[Tooltip ("Handles pure deck operations")]
		public Deck deck;

		[Tooltip ("Handles loading card data from Resources")]
		public CardDataLoader cardLoader;

		[Tooltip ("Handles deck-related UI updates")]
		public DeckUIManager deckUI;

		[Tooltip ("Handles game setup and initialization")]
		public GameSetupManager gameSetup;

		[Header ("MILESTONE 1: Network Support")]
		[Tooltip ("Is this a network multiplayer game?")]
		private bool isNetworkMode = false;

		// Events for external systems (unified interface)
		public System.Action<CardData> OnCardDrawn;
		public System.Action<CardData> OnCardDiscarded;
		public System.Action OnDeckShuffled;
		public System.Action OnDeckEmpty;
		public System.Action OnGameInitialized;
		public System.Action<List<CardData>, List<CardData>, CardData> OnInitialGameSetup;

		// MILESTONE 1: Network events
		public System.Action<int, int> OnNetworkDeckStateUpdated; // drawCount, discardCount

		void Start () {
			InitializeComponents ();
			ConnectEvents ();
			deckUI.ShowLoadingMessage ();
			// UI will update automatically when deck is initialized via events
		}

		/// <summary>
		/// Force UI update after initial loading (called with small delay)
		/// </summary>
		void ForceInitialUIUpdate () {
			UpdateUI ();
			TakiLogger.LogDeck ($"Forced UI update - Draw: {DrawPileCount}, Discard: {DiscardPileCount}", TakiLogger.LogLevel.Trace);
		}

		/// <summary>
		/// Auto-initialize deck when cards are loaded (for immediate play readiness)
		/// </summary>
		void AutoInitializeDeck () {
			if (cardLoader != null && cardLoader.HasValidDeck && deck != null) {
				List<CardData> allCards = cardLoader.GetAllCardsForDeck ();
				deck.InitializeDeck (allCards);
				TakiLogger.LogDeck ($"Auto-initialized deck with {allCards.Count} cards");
				UpdateUI (); // Update UI immediately after deck initialization
			}
		}

		/// <summary>
		/// Initialize and validate all components
		/// </summary>
		void InitializeComponents () {
			// Validate component references
			if (!ValidateComponents ()) {
				TakiLogger.LogError ("DeckManager: Missing required component references!", TakiLogger.LogCategory.System);
				return;
			}

			// Connect component dependencies
			gameSetup.cardLoader = cardLoader;
			gameSetup.deck = deck;

			TakiLogger.LogSystem ("DeckManager components initialized", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Connect events between components and external systems
		/// </summary>
		void ConnectEvents () {
			if (deck != null) {
				// Forward deck events to external systems
				deck.OnCardDrawn += (card) => {
					OnCardDrawn?.Invoke (card);
					UpdateUI ();
				};

				deck.OnCardDiscarded += (card) => {
					OnCardDiscarded?.Invoke (card);
					UpdateUI ();
					deckUI.UpdateDiscardPileDisplay (card);
				};

				deck.OnDeckShuffled += () => {
					OnDeckShuffled?.Invoke ();
					deckUI.ShowDeckInitializedMessage ();
				};

				deck.OnDeckReshuffled += () => {
					deckUI.ShowReshuffleMessage ();
				};

				deck.OnDeckEmpty += () => {
					OnDeckEmpty?.Invoke ();
					deckUI.ShowDeckErrorMessage ("No more cards available!");
				};
			}

			if (cardLoader != null) {
				cardLoader.OnCardsLoaded += (cards) => {
					deckUI.ShowDeckLoadedMessage (cards.Count);
					// Auto-initialize deck with loaded cards only if not in network mode
					if (!isNetworkMode) {
						AutoInitializeDeck ();
					}
				};

				cardLoader.OnLoadError += (error) => {
					deckUI.ShowDeckErrorMessage (error);
				};
			}

			if (gameSetup != null) {
				gameSetup.OnGameInitialized += () => {
					OnGameInitialized?.Invoke ();
					UpdateUI ();
				};

				gameSetup.OnInitialGameSetup += (p1Hand, p2Hand, startCard) => {
					OnInitialGameSetup?.Invoke (p1Hand, p2Hand, startCard);
					deckUI.ShowGameStartMessage (startCard);
				};
			}
		}

		/// <summary>
		/// Update UI elements with current deck state
		/// </summary>
		void UpdateUI () {
			if (deck != null && deckUI != null) {
				deckUI.UpdateDeckUI (deck.DrawPileCount, deck.DiscardPileCount);

				// MILESTONE 1: Fire network deck update event
				if (isNetworkMode) {
					OnNetworkDeckStateUpdated?.Invoke (deck.DrawPileCount, deck.DiscardPileCount);
				}
			}
		}

		// ===== MILESTONE 1: NETWORK-AWARE METHODS =====

		/// <summary>
		/// MILESTONE 1: Set network mode for multiplayer coordination
		/// </summary>
		/// <param name="networkMode">True if this is a network game</param>
		public void SetNetworkMode (bool networkMode) {
			isNetworkMode = networkMode;
			TakiLogger.LogNetwork ($"DeckManager network mode set to: {networkMode}");

			if (networkMode) {
				TakiLogger.LogNetwork ("Network mode enabled - deck will be coordinated across clients");
			}
		}

		/// <summary>
		/// MILESTONE 1: Initialize deck for network game (lightweight)
		/// Used by clients to prepare deck without full game setup
		/// </summary>
		public void InitializeDeck () {
			if (cardLoader == null || !cardLoader.HasValidDeck) {
				TakiLogger.LogError ("Cannot initialize network deck: Invalid card data", TakiLogger.LogCategory.Multiplayer);
				return;
			}

			// Initialize basic deck without full game setup
			List<CardData> allCards = cardLoader.GetAllCardsForDeck ();
			if (deck != null) {
				deck.InitializeDeck (allCards);
				TakiLogger.LogNetwork ($"Network deck initialized with {allCards.Count} cards");
			}
		}

		/// <summary>
		/// MILESTONE 1: Setup network game (master client only)
		/// Coordinates initial game state for network synchronization
		/// </summary>
		/// <returns>Initial game state for network broadcasting</returns>
		public (List<CardData> player1Hand, List<CardData> player2Hand, CardData startingCard) SetupNetworkGame () {
			TakiLogger.LogNetwork ("Setting up network game state (master client)");

			if (!PhotonNetwork.IsMasterClient) {
				TakiLogger.LogWarning ("SetupNetworkGame called on non-master client", TakiLogger.LogCategory.Multiplayer);
				return (new List<CardData> (), new List<CardData> (), null);
			}

			// Use existing setup logic
			var gameState = SetupInitialGame ();

			TakiLogger.LogNetwork ($"Network game setup complete: P1={gameState.player1Hand.Count}, P2={gameState.player2Hand.Count}, Start={gameState.startingCard?.GetDisplayText ()}");

			return gameState;
		}

		/// <summary>
		/// MILESTONE 1: Synchronize deck state from network data
		/// Used by clients to match master's deck state
		/// </summary>
		/// <param name="drawCount">Draw pile count from master</param>
		/// <param name="discardCount">Discard pile count from master</param>
		/// <param name="topDiscardCard">Top discard card from master</param>
		public void SynchronizeDeckState (int drawCount, int discardCount, CardData topDiscardCard) {
			TakiLogger.LogNetwork ($"Synchronizing deck state: Draw={drawCount}, Discard={discardCount}, Top={topDiscardCard?.GetDisplayText ()}");

			if (deck == null) {
				TakiLogger.LogError ("Cannot synchronize deck state: Deck component missing", TakiLogger.LogCategory.Multiplayer);
				return;
			}

			// Place top discard card if provided
			if (topDiscardCard != null) {
				deck.DiscardCard (topDiscardCard);
			}

			// Update UI to reflect synchronized state
			UpdateUI ();

			TakiLogger.LogNetwork ("Deck state synchronized successfully");
		}

		/// <summary>
		/// MILESTONE 1: Get current deck state for network broadcasting
		/// Used by master client to share deck state
		/// </summary>
		/// <returns>Current deck state data</returns>
		public (int drawCount, int discardCount, CardData topCard) GetDeckStateForNetwork () {
			if (deck == null) {
				return (0, 0, null);
			}

			return (deck.DrawPileCount, deck.DiscardPileCount, deck.GetTopDiscardCard ());
		}

		/// <summary>
		/// MILESTONE 1: Update network hand counts for display
		/// Used to show opponent hand counts without revealing cards
		/// </summary>
		/// <param name="player1Count">Player 1 hand count</param>
		/// <param name="player2Count">Player 2 hand count</param>
		public void UpdateNetworkHandCounts (int player1Count, int player2Count) {
			TakiLogger.LogNetwork ($"Updating network hand counts: P1={player1Count}, P2={player2Count}");

			if (deckUI != null) {
				// Show hand counts in UI - this will be picked up by GameplayUIManager
				deckUI.ShowDeckMessage ($"Hands: P1({player1Count}) P2({player2Count})", false);
			}
		}

		// ===== PRESERVED EXISTING PUBLIC API =====

		/// <summary>
		/// Draw a single card from the deck
		/// </summary>
		/// <returns>Drawn card or null if none available</returns>
		public CardData DrawCard () {
			return deck?.DrawCard ();
		}

		/// <summary>
		/// Draw multiple cards at once
		/// </summary>
		/// <param name="count">Number of cards to draw</param>
		/// <returns>List of drawn cards</returns>
		public List<CardData> DrawCards (int count) {
			return deck?.DrawCards (count) ?? new List<CardData> ();
		}

		/// <summary>
		/// Discard a card to the discard pile
		/// </summary>
		/// <param name="card">Card to discard</param>
		public void DiscardCard (CardData card) {
			deck?.DiscardCard (card);
		}

		/// <summary>
		/// Get the top card of the discard pile
		/// </summary>
		/// <returns>Top discard card or null</returns>
		public CardData GetTopDiscardCard () {
			return deck?.GetTopDiscardCard ();
		}

		/// <summary>
		/// Set up initial game state (deal hands and place starting card)
		/// ENHANCED: Works for both singleplayer and network master
		/// </summary>
		/// <returns>Player hands and starting card</returns>
		public (List<CardData> player1Hand, List<CardData> player2Hand, CardData startingCard) SetupInitialGame () {
			if (gameSetup != null) {
				// Initialize the game first
				gameSetup.InitializeNewGame ();
				// Then setup the initial game state
				var result = gameSetup.SetupInitialGame ();
				TakiLogger.LogNetwork ($"SetupInitialGame successful: P1={result.player1Hand.Count}, P2={result.player2Hand.Count}, Start={result.startingCard?.GetDisplayText() ?? "null"}");
				return result;
			}

			TakiLogger.LogError ("CRITICAL: SetupInitialGame failed - gameSetup is null! Check Inspector assignments.", TakiLogger.LogCategory.System);
			return (new List<CardData> (), new List<CardData> (), null);
		}

		/// <summary>
		/// Show a message to the player
		/// </summary>
		/// <param name="message">Message to display</param>
		/// <param name="isTemporary">Whether message should auto-clear</param>
		public void ShowMessage (string message, bool isTemporary = true) {
			deckUI?.ShowDeckMessage (message, isTemporary);
		}

		/// <summary>
		/// Get deck statistics for debugging
		/// </summary>
		/// <returns>Formatted statistics string</returns>
		public string GetDeckStats () {
			return cardLoader?.GetDeckStats () ?? "No card data loaded";
		}

		/// <summary>
		/// Validate all component references are assigned
		/// </summary>
		/// <returns>True if all components are valid</returns>
		bool ValidateComponents () {
			bool isValid = true;

			if (deck == null) {
				TakiLogger.LogError ("DeckManager: Deck component reference is missing!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (cardLoader == null) {
				TakiLogger.LogError ("DeckManager: CardDataLoader component reference is missing!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (deckUI == null) {
				TakiLogger.LogError ("DeckManager: DeckUIManager component reference is missing!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			if (gameSetup == null) {
				TakiLogger.LogError ("DeckManager: GameSetupManager component reference is missing!", TakiLogger.LogCategory.System);
				isValid = false;
			}

			return isValid;
		}

		// ===== PROPERTIES - Delegate to components ===== 

		public int DrawPileCount => deck?.DrawPileCount ?? 0;
		public int DiscardPileCount => deck?.DiscardPileCount ?? 0;
		public bool HasCardsInDrawPile => deck?.HasCardsInDrawPile ?? false;
		public bool HasCardsInDiscardPile => deck?.HasCardsInDiscardPile ?? false;
		public bool CanDrawCards => deck?.CanDrawCards ?? false;
		public int TotalCardCount => cardLoader?.TotalCardCount ?? 0;
		public bool HasValidDeck => cardLoader?.HasValidDeck ?? false;
		public bool IsSetupValid => gameSetup?.IsSetupValid ?? false;
		public bool AllComponentsValid => ValidateComponents ();

		// Quick access methods for common operations
		public bool CanSetupNewGame => cardLoader?.HasValidDeck ?? false;
		public bool IsDeckReady => deck != null && HasValidDeck;

		// MILESTONE 1: Network properties
		public bool IsNetworkMode => isNetworkMode;
		public bool CanSetupNetworkGame => isNetworkMode && PhotonNetwork.IsMasterClient && HasValidDeck;
	}
}