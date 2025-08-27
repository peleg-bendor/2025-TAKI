using UnityEngine;
using System.Collections.Generic;

namespace TakiGame {
	/// <summary>
	/// Coordinator that manages all deck-related operations
	/// Delegates responsibilities to specialized components
	/// Main interface for other systems to interact with deck functionality
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

		// Events for external systems (unified interface)
		public System.Action<CardData> OnCardDrawn;
		public System.Action<CardData> OnCardDiscarded;
		public System.Action OnDeckShuffled;
		public System.Action OnDeckEmpty;
		public System.Action OnGameInitialized;
		public System.Action<List<CardData>, List<CardData>, CardData> OnInitialGameSetup;

		void Start () {
			InitializeComponents ();
			ConnectEvents ();
			deckUI.ShowLoadingMessage ();
		}

		/// <summary>
		/// Initialize and validate all components
		/// </summary>
		void InitializeComponents () {
			// Validate component references
			if (!ValidateComponents ()) {
				Debug.LogError ("DeckManager: Missing required component references!");
				return;
			}

			// Connect component dependencies
			gameSetup.cardLoader = cardLoader;
			gameSetup.deck = deck;

			Debug.Log ("DeckManager components initialized");
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
					deckUI.ShowErrorMessage ("No more cards available!");
				};
			}

			if (cardLoader != null) {
				cardLoader.OnCardsLoaded += (cards) => {
					deckUI.ShowDeckLoadedMessage (cards.Count);
				};

				cardLoader.OnLoadError += (error) => {
					deckUI.ShowErrorMessage (error);
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
			}
		}

		// ===== PUBLIC API - Delegates to appropriate components =====

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
		/// Initialize a new deck for a fresh game
		/// </summary>
		public void InitializeDeck () {
			gameSetup?.InitializeNewGame ();
		}

		/// <summary>
		/// Set up initial game state (deal hands and place starting card)
		/// </summary>
		/// <returns>Player hands and starting card</returns>
		public (List<CardData> player1Hand, List<CardData> player2Hand, CardData startingCard) SetupInitialGame () {
			if (gameSetup != null) {
				return gameSetup.SetupInitialGame ();
			}
			return (new List<CardData> (), new List<CardData> (), null);
		}

		/// <summary>
		/// Show a message to the player
		/// </summary>
		/// <param name="message">Message to display</param>
		/// <param name="isTemporary">Whether message should auto-clear</param>
		public void ShowMessage (string message, bool isTemporary = true) {
			deckUI?.ShowMessage (message, isTemporary);
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
				Debug.LogError ("DeckManager: Deck component reference is missing!");
				isValid = false;
			}

			if (cardLoader == null) {
				Debug.LogError ("DeckManager: CardDataLoader component reference is missing!");
				isValid = false;
			}

			if (deckUI == null) {
				Debug.LogError ("DeckManager: DeckUIManager component reference is missing!");
				isValid = false;
			}

			if (gameSetup == null) {
				Debug.LogError ("DeckManager: GameSetupManager component reference is missing!");
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
	}
}