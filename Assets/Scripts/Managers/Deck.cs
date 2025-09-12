using UnityEngine;
using System.Collections.Generic;

namespace TakiGame {
	/// <summary>
	/// Handles pure deck operations: draw pile, discard pile, shuffling
	/// NO UI updates, NO resource loading, NO game setup logic
	/// </summary>
	public class Deck : MonoBehaviour {

		[Header ("Deck State")]
		[Tooltip ("Current draw pile (cards to be drawn)")]
		public List<CardData> drawPile = new List<CardData> ();

		[Tooltip ("Current discard pile (played cards)")]
		public List<CardData> discardPile = new List<CardData> ();

		// Events for other systems to listen to
		public System.Action<CardData> OnCardDrawn;
		public System.Action<CardData> OnCardDiscarded;
		public System.Action OnDeckShuffled;
		public System.Action OnDeckEmpty;
		public System.Action OnDeckReshuffled;

		/// <summary>
		/// Initialize deck with provided card data
		/// </summary>
		/// <param name="allCards">Complete set of cards to use</param>
		public void InitializeDeck (List<CardData> allCards) {
			if (allCards == null || allCards.Count == 0) {
				TakiLogger.LogError ("Cannot initialize deck: No card data provided!", TakiLogger.LogCategory.Deck);
				return;
			}

			// Copy all cards to draw pile
			drawPile.Clear ();
			drawPile.AddRange (allCards);

			// Clear discard pile
			discardPile.Clear ();

			// Shuffle the deck
			ShuffleDeck ();

			TakiLogger.LogInfo ($"Initialized with {drawPile.Count} cards", TakiLogger.LogCategory.Deck);
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
			TakiLogger.LogInfo ("Shuffled", TakiLogger.LogCategory.Deck);
		}

		/// <summary>
		/// Draw a single card from the draw pile
		/// Automatically reshuffles if needed
		/// </summary>
		/// <returns>The drawn card, or null if no cards available</returns>
		public CardData DrawCard () {
			// Check if we need to reshuffle IMMEDIATELY when draw pile is empty
			if (drawPile.Count == 0 && discardPile.Count >= 2) {
				ReshuffleDiscardIntoDraw ();
			}

			// Check if draw pile is still empty after potential reshuffle
			if (drawPile.Count == 0) {
				TakiLogger.LogWarning ("Cannot draw card: No cards available anywhere!", TakiLogger.LogCategory.Deck);
				OnDeckEmpty?.Invoke ();
				return null;
			}

			// Draw the top card
			CardData drawnCard = drawPile [0];
			drawPile.RemoveAt (0);

			OnCardDrawn?.Invoke (drawnCard);
			TakiLogger.LogInfo ($"Drew card: {drawnCard.GetDisplayText ()}", TakiLogger.LogCategory.Deck);

			return drawnCard;
		}

		/// <summary>
		/// Draw multiple cards one by one (safer than batch)
		/// </summary>
		/// <param name="count">Number of cards to draw</param>
		/// <returns>List of successfully drawn cards</returns>
		public List<CardData> DrawCards (int count) {
			List<CardData> drawnCards = new List<CardData> ();

			for (int i = 0; i < count; i++) {
				CardData card = DrawCard ();
				if (card != null) {
					drawnCards.Add (card);
				} else {
					// Log if we couldn't draw all requested cards
					if (drawnCards.Count < count) {
						TakiLogger.LogWarning ($"Could only draw {drawnCards.Count} out of {count} requested cards", TakiLogger.LogCategory.Deck);
					}
					break; // Stop if we can't draw more cards
				}
			}

			return drawnCards;
		}

		/// <summary>
		/// Discard a card to the discard pile (when a card is played)
		/// </summary>
		/// <param name="card">Card to discard</param>
		public void DiscardCard (CardData card) {
			if (card == null) {
				TakiLogger.LogWarning ("Cannot discard null card!", TakiLogger.LogCategory.Deck);
				return;
			}

			discardPile.Add (card);
			OnCardDiscarded?.Invoke (card);
			TakiLogger.LogInfo ($"Discarded card: {card.GetDisplayText ()}", TakiLogger.LogCategory.Deck);
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
		/// Reshuffle discard pile into draw pile (keeping top card in discard)
		/// </summary>
		void ReshuffleDiscardIntoDraw () {
			if (discardPile.Count < 2) {
				TakiLogger.LogWarning ("Cannot reshuffle: Need at least 2 cards in discard pile", TakiLogger.LogCategory.Deck);
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

			OnDeckReshuffled?.Invoke ();
			TakiLogger.LogInfo ($"Reshuffled discard pile into draw pile. Draw pile now has {drawPile.Count} cards", TakiLogger.LogCategory.Deck);
		}

		/// <summary>
		/// Clear all cards from both piles
		/// </summary>
		public void ClearDeck () {
			drawPile.Clear ();
			discardPile.Clear ();
			TakiLogger.LogInfo ("Cleared", TakiLogger.LogCategory.Deck);
		}

		// Properties for external access
		public int DrawPileCount => drawPile.Count;
		public int DiscardPileCount => discardPile.Count;
		public bool HasCardsInDrawPile => drawPile.Count > 0;
		public bool HasCardsInDiscardPile => discardPile.Count > 0;
		public bool CanDrawCards => drawPile.Count > 0 || discardPile.Count >= 2;
	}
}