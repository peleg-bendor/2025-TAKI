using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace TakiGame {
	/// <summary>
	/// Manages the deck, draw pile, and discard pile for TAKI game
	/// Handles card distribution, shuffling, and UI updates
	/// </summary>
	public class DeckManager : MonoBehaviour {

		[Header ("UI References")]
		[Tooltip ("Panel that shows the draw pile")]
		public Transform drawPilePanel;

		[Tooltip ("Panel that shows the discard pile")]
		public Transform discardPilePanel;

		[Tooltip ("Text showing number of cards in draw pile")]
		public TextMeshProUGUI drawPileCountText;

		[Tooltip ("Text showing number of cards in discard pile")]
		public TextMeshProUGUI discardPileCountText;

		[Tooltip ("Text for game messages and feedback")]
		public TextMeshProUGUI gameMessageText;

		[Header ("Deck State")]
		[Tooltip ("All available card data (loaded at start)")]
		public List<CardData> allCardData = new List<CardData> ();

		[Tooltip ("Current draw pile (cards to be drawn)")]
		public List<CardData> drawPile = new List<CardData> ();

		[Tooltip ("Current discard pile (played cards)")]
		public List<CardData> discardPile = new List<CardData> ();

		[Header ("Game Settings")]
		[Tooltip ("Number of cards to deal to each player at start")]
		public int initialHandSize = 8;

		[Tooltip ("Minimum cards in draw pile before reshuffling discard")]
		public int reshuffleThreshold = 5;

		// Events for other systems to listen to
		public System.Action<CardData> OnCardDrawn;
		public System.Action<CardData> OnCardDiscarded;
		public System.Action OnDeckShuffled;
		public System.Action OnDeckEmpty;

		void Start () {
			LoadAllCardData ();
			InitializeDeck ();
			UpdateUI ();
		}

		/// <summary>
		/// Load all CardData assets from Resources/Data/Cards
		/// </summary>
		void LoadAllCardData () {
			// Load all CardData assets from the Cards folder
			CardData [] loadedCards = Resources.LoadAll<CardData> ("Data/Cards");

			if (loadedCards.Length == 0) {
				Debug.LogError ("No CardData found in Resources/Data/Cards! Make sure cards are in Resources folder.");
				ShowMessage ("ERROR: No cards found! Check Resources/Data/Cards folder.");
				return;
			}

			allCardData.Clear ();
			allCardData.AddRange (loadedCards);

			Debug.Log ($"Loaded {allCardData.Count} cards from Resources/Data/Cards");

			// Verify we have the expected 110 cards
			if (allCardData.Count == 110) {
				Debug.Log ("Deck verified: 110 cards loaded successfully");
				ShowMessage ($"Deck loaded: {allCardData.Count} cards ready!");
			} else {
				Debug.LogWarning ($"Expected 110 cards, but loaded {allCardData.Count}");
				ShowMessage ($"Warning: Expected 110 cards, loaded {allCardData.Count}");
			}
		}

		/// <summary>
		/// Initialize the deck for a new game
		/// </summary>
		public void InitializeDeck () {
			if (allCardData.Count == 0) {
				Debug.LogError ("Cannot initialize deck: No card data loaded!");
				return;
			}

			// Copy all cards to draw pile
			drawPile.Clear ();
			drawPile.AddRange (allCardData);

			// Clear discard pile
			discardPile.Clear ();

			// Shuffle the deck
			ShuffleDeck ();

			Debug.Log ($"Deck initialized with {drawPile.Count} cards");
			ShowMessage ("New deck shuffled and ready!");

			UpdateUI ();
		}

		/// <summary>
		/// Shuffle the draw pile using Fisher-Yates algorithm
		/// </summary>
		public void ShuffleDeck () {
			for (int i = drawPile.Count - 1; i > 0; i--) {
				int randomIndex = Random.Range (0, i + 1);
				CardData temp = drawPile [i];
				drawPile [i] = drawPile [randomIndex];
				drawPile [randomIndex] = temp;
			}

			OnDeckShuffled?.Invoke ();
			Debug.Log ("Deck shuffled");
		}

		/// <summary>
		/// Draw a card from the draw pile
		/// </summary>
		/// <returns>The drawn card, or null if deck is empty</returns>
		public CardData DrawCard () {
			// Check if we need to reshuffle
			if (drawPile.Count <= reshuffleThreshold && discardPile.Count > 1) {
				ReshuffleDiscardIntoDraw ();
			}

			// Check if draw pile is empty
			if (drawPile.Count == 0) {
				Debug.LogWarning ("Cannot draw card: Draw pile is empty!");
				ShowMessage ("Draw pile empty! Cannot draw more cards.");
				OnDeckEmpty?.Invoke ();
				return null;
			}

			// Draw the top card
			CardData drawnCard = drawPile [0];
			drawPile.RemoveAt (0);

			OnCardDrawn?.Invoke (drawnCard);
			UpdateUI ();

			Debug.Log ($"Drew card: {drawnCard.GetDisplayText ()}");
			return drawnCard;
		}

		/// <summary>
		/// Draw multiple cards at once
		/// </summary>
		/// <param name="count">Number of cards to draw</param>
		/// <returns>List of drawn cards</returns>
		public List<CardData> DrawCards (int count) {
			List<CardData> drawnCards = new List<CardData> ();

			for (int i = 0; i < count; i++) {
				CardData card = DrawCard ();
				if (card != null) {
					drawnCards.Add (card);
				} else {
					break; // Stop if we can't draw more cards
				}
			}

			return drawnCards;
		}

		/// <summary>
		/// Discard a card to the discard pile
		/// </summary>
		/// <param name="card">Card to discard</param>
		public void DiscardCard (CardData card) {
			if (card == null) {
				Debug.LogWarning ("Cannot discard null card!");
				return;
			}

			discardPile.Add (card);
			OnCardDiscarded?.Invoke (card);
			UpdateUI ();

			Debug.Log ($"Discarded card: {card.GetDisplayText ()}");
		}

		/// <summary>
		/// Get the top card of the discard pile without removing it
		/// </summary>
		/// <returns>Top discard card, or null if pile is empty</returns>
		public CardData GetTopDiscardCard () {
			if (discardPile.Count == 0) return null;
			return discardPile [discardPile.Count - 1];
		}

		/// <summary>
		/// Reshuffle discard pile into draw pile (keeping top card)
		/// </summary>
		void ReshuffleDiscardIntoDraw () {
			if (discardPile.Count <= 1) {
				Debug.LogWarning ("Cannot reshuffle: Not enough cards in discard pile");
				return;
			}

			// Keep the top card in discard pile
			CardData topCard = discardPile [discardPile.Count - 1];
			discardPile.RemoveAt (discardPile.Count - 1);

			// Move all other discard cards to draw pile
			drawPile.AddRange (discardPile);
			discardPile.Clear ();
			discardPile.Add (topCard);

			// Shuffle the new draw pile
			ShuffleDeck ();

			Debug.Log ($"Reshuffled discard pile into draw pile. Draw pile now has {drawPile.Count} cards");
			ShowMessage ("Reshuffled discard pile into draw pile!");
		}

		/// <summary>
		/// Set up initial game state (deal cards and place first discard)
		/// </summary>
		/// <returns>Lists of cards for each player and the starting discard card</returns>
		public (List<CardData> player1Hand, List<CardData> player2Hand, CardData startingCard) SetupInitialGame () {
			// Initialize fresh deck
			InitializeDeck ();

			// Deal cards to players
			List<CardData> player1Hand = DrawCards (initialHandSize);
			List<CardData> player2Hand = DrawCards (initialHandSize);

			// Draw starting card for discard pile
			CardData startingCard = null;
			int attempts = 0;
			const int maxAttempts = 10;

			// Try to get a simple starting card (avoid special cards if possible)
			while (attempts < maxAttempts) {
				CardData candidate = DrawCard ();
				if (candidate != null) {
					// Prefer number cards as starting cards
					if (candidate.cardType == CardType.Number) {
						startingCard = candidate;
						break;
					} else if (attempts == maxAttempts - 1) {
						// If we can't find a number card, use whatever we got
						startingCard = candidate;
						break;
					} else {
						// Put the card back and try again
						drawPile.Insert (Random.Range (0, drawPile.Count), candidate);
					}
				}
				attempts++;
			}

			// Place starting card in discard pile
			if (startingCard != null) {
				DiscardCard (startingCard);
				Debug.Log ($"Starting card: {startingCard.GetDisplayText ()}");
				ShowMessage ($"Game started! First card: {startingCard.GetDisplayText ()}");
			}

			Debug.Log ($"Initial setup complete. Player 1: {player1Hand.Count} cards, Player 2: {player2Hand.Count} cards");

			return (player1Hand, player2Hand, startingCard);
		}

		/// <summary>
		/// Update UI elements with current deck state
		/// </summary>
		void UpdateUI () {
			if (drawPileCountText != null) {
				drawPileCountText.text = $"Draw: {drawPile.Count}";
			}

			if (discardPileCountText != null) {
				discardPileCountText.text = $"Discard: {discardPile.Count}";
			}

			// TODO: Update visual representation of piles
			// This will be implemented in Milestone 6 with card prefabs
		}

		/// <summary>
		/// Show a message to the player
		/// </summary>
		/// <param name="message">Message to display</param>
		void ShowMessage (string message) {
			if (gameMessageText != null) {
				gameMessageText.text = message;
			}
			Debug.Log ($"Game Message: {message}");
		}

		/// <summary>
		/// Get deck statistics for debugging
		/// </summary>
		public string GetDeckStats () {
			var stats = allCardData.GroupBy (card => card.cardType)
								   .Select (group => $"{group.Key}: {group.Count ()}")
								   .ToArray ();

			return $"Total Cards: {allCardData.Count}\n" + string.Join ("\n", stats);
		}

		// Public accessors for other systems
		public int DrawPileCount => drawPile.Count;
		public int DiscardPileCount => discardPile.Count;
		public bool HasCardsInDrawPile => drawPile.Count > 0;
		public bool HasCardsInDiscardPile => discardPile.Count > 0;
	}
}