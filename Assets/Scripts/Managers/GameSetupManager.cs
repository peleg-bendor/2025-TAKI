using UnityEngine;
using System.Collections.Generic;

namespace TakiGame {
	/// <summary>
	/// Handles game setup and initialization logic
	/// NO UI updates, NO direct deck operations, NO resource loading
	/// </summary>
	public class GameSetupManager : MonoBehaviour {

		[Header ("Setup Settings")]
		[Tooltip ("Number of cards to deal to each player at start")]
		public int initialHandSize = 8;

		[Tooltip ("Maximum attempts to find a good starting card")]
		public int maxStartingCardAttempts = 10;

		[Header ("Dependencies")]
		[Tooltip ("Reference to the CardDataLoader component")]
		public CardDataLoader cardLoader;

		[Tooltip ("Reference to the Deck component")]
		public Deck deck;

		// Events
		public System.Action OnGameInitialized;
		public System.Action<List<CardData>, List<CardData>, CardData> OnInitialGameSetup;

		/// <summary>
		/// Initialize a completely new game
		/// </summary>
		public void InitializeNewGame () {
			if (cardLoader == null) {
				Debug.LogError ("CardDataLoader reference is missing!");
				return;
			}

			if (deck == null) {
				Debug.LogError ("Deck reference is missing!");
				return;
			}

			// Make sure we have valid card data
			if (!cardLoader.HasValidDeck) {
				Debug.LogError ("Cannot initialize game: Invalid or missing card data!");
				return;
			}

			// Get a fresh copy of all cards for the deck
			List<CardData> allCards = cardLoader.GetAllCardsForDeck ();

			// Initialize the deck with all cards
			deck.InitializeDeck (allCards);

			OnGameInitialized?.Invoke ();
			Debug.Log ("New game initialized successfully");
		}

		/// <summary>
		/// Set up initial game state: deal cards to players and place starting card
		/// </summary>
		/// <returns>Player hands and starting discard card</returns>
		public (List<CardData> player1Hand, List<CardData> player2Hand, CardData startingCard) SetupInitialGame () {
			// Initialize fresh deck first
			InitializeNewGame ();

			// Deal cards to players
			List<CardData> player1Hand = DrawInitialHand (initialHandSize);
			List<CardData> player2Hand = DrawInitialHand (initialHandSize);

			// Select and place starting card
			CardData startingCard = SelectStartingCard ();

			// Place starting card in discard pile if we found one
			if (startingCard != null) {
				deck.DiscardCard (startingCard);
				Debug.Log ($"Starting card: {startingCard.GetDisplayText ()}");
			} else {
				Debug.LogError ("Could not find a suitable starting card!");
			}

			Debug.Log ($"Initial setup complete. Player 1: {player1Hand.Count} cards, Player 2: {player2Hand.Count} cards");

			OnInitialGameSetup?.Invoke (player1Hand, player2Hand, startingCard);

			return (player1Hand, player2Hand, startingCard);
		}

		/// <summary>
		/// Draw initial hand for a player
		/// </summary>
		/// <param name="handSize">Number of cards to draw</param>
		/// <returns>List of cards for the hand</returns>
		public List<CardData> DrawInitialHand (int handSize) {
			if (deck == null) {
				Debug.LogError ("Deck reference is missing!");
				return new List<CardData> ();
			}

			List<CardData> hand = deck.DrawCards (handSize);

			if (hand.Count < handSize) {
				Debug.LogWarning ($"Could only draw {hand.Count} out of {handSize} requested cards for initial hand");
			}

			return hand;
		}

		/// <summary>
		/// Select a good starting card (preferably a number card)
		/// </summary>
		/// <returns>Starting card, or null if none found</returns>
		public CardData SelectStartingCard () {
			if (deck == null) {
				Debug.LogError ("Deck reference is missing!");
				return null;
			}

			CardData startingCard = null;
			List<CardData> attemptedCards = new List<CardData> ();
			int attempts = 0;

			// Try to get a simple starting card (avoid special cards if possible)
			while (attempts < maxStartingCardAttempts && startingCard == null) {
				CardData candidate = deck.DrawCard ();
				if (candidate != null) {
					// Prefer number cards as starting cards
					if (candidate.cardType == CardType.Number) {
						startingCard = candidate;
						break;
					} else if (attempts == maxStartingCardAttempts - 1) {
						// If we can't find a number card, use whatever we got last
						startingCard = candidate;
						break;
					} else {
						// Put the card back and try again
						attemptedCards.Add (candidate);
					}
				} else {
					// No more cards available
					break;
				}
				attempts++;
			}

			// Put unused attempted cards back into the deck randomly
			foreach (CardData unusedCard in attemptedCards) {
				if (unusedCard != startingCard && deck.DrawPileCount > 0) {
					deck.drawPile.Insert (Random.Range (0, deck.DrawPileCount), unusedCard);
				}
			}

			return startingCard;
		}

		/// <summary>
		/// Quick setup for testing (minimal validation)
		/// </summary>
		/// <param name="testHandSize">Hand size for testing</param>
		public void QuickSetupForTesting (int testHandSize = 5) {
			initialHandSize = testHandSize;
			maxStartingCardAttempts = 3;
			SetupInitialGame ();
		}

		/// <summary>
		/// Validate that all required components are assigned
		/// </summary>
		/// <returns>True if setup is valid</returns>
		public bool ValidateSetup () {
			bool isValid = true;

			if (cardLoader == null) {
				Debug.LogError ("GameSetupManager: CardDataLoader reference is missing!");
				isValid = false;
			}

			if (deck == null) {
				Debug.LogError ("GameSetupManager: Deck reference is missing!");
				isValid = false;
			}

			if (initialHandSize <= 0) {
				Debug.LogError ("GameSetupManager: Initial hand size must be greater than 0!");
				isValid = false;
			}

			return isValid;
		}

		// Properties
		public bool IsSetupValid => ValidateSetup ();
		public int InitialHandSize => initialHandSize;
		public bool CanSetupGame => cardLoader != null && deck != null && cardLoader.HasValidDeck;
	}
}