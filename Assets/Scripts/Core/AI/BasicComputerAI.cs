using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

namespace TakiGame {
	/// <summary>
	/// Simple AI that makes basic decisions for computer player
	/// NO turn management, NO game state changes, NO UI updates
	/// </summary>
	public class BasicComputerAI : MonoBehaviour {

		[Header ("AI Settings")]
		[Tooltip ("Computer player's hand of cards")]
		public List<CardData> computerHand = new List<CardData> ();

		[Tooltip ("Time to 'think' before making a move")]
		public float thinkingTime = 1.0f;

		[Tooltip ("Chance to play special cards over number cards (0-1)")]
		[Range (0f, 1f)]
		public float specialCardPreference = 0.7f;

		[Header ("Dependencies")]
		[Tooltip ("Reference to game state manager")]
		public GameStateManager gameState;

		// Events for AI decisions
		public System.Action<CardData> OnAICardSelected;
		public System.Action OnAIDrawCard;
		public System.Action<CardColor> OnAIColorSelected;
		public System.Action<string> OnAIDecisionMade;

		/// <summary>
		/// Make AI decision for current turn
		/// </summary>
		/// <param name="topDiscardCard">Current top card of discard pile</param>
		public void MakeDecision (CardData topDiscardCard) {
			if (topDiscardCard == null) {
				Debug.LogWarning ("AI cannot make decision: No top discard card provided");
				return;
			}

			Debug.Log ($"AI thinking... (Hand size: {computerHand.Count})");
			OnAIDecisionMade?.Invoke ("AI is thinking...");

			// Add thinking delay
			Invoke (nameof (ExecuteDecision), thinkingTime);
		}

		/// <summary>
		/// Execute the AI decision after thinking time
		/// </summary>
		void ExecuteDecision () {
			CardData topDiscardCard = FindTopDiscardCard ();
			if (topDiscardCard == null) return;

			// Find all valid cards to play
			List<CardData> validCards = GetValidCards (topDiscardCard);

			if (validCards.Count > 0) {
				// Select best card to play
				CardData selectedCard = SelectBestCard (validCards);
				PlayCard (selectedCard);
			} else {
				// No valid cards, must draw
				DrawCard ();
			}
		}

		/// <summary>
		/// Get all cards that can be legally played
		/// </summary>
		/// <param name="topDiscardCard">Current top discard card</param>
		/// <returns>List of playable cards</returns>
		List<CardData> GetValidCards (CardData topDiscardCard) {
			if (gameState == null) return new List<CardData> ();

			List<CardData> validCards = new List<CardData> ();

			foreach (CardData card in computerHand) {
				if (gameState.IsValidMove (card, topDiscardCard)) {
					validCards.Add (card);
				}
			}

			Debug.Log ($"AI found {validCards.Count} valid cards to play");
			return validCards;
		}

		/// <summary>
		/// Select the best card from valid options using simple AI strategy
		/// </summary>
		/// <param name="validCards">List of cards that can be played</param>
		/// <returns>Selected card to play</returns>
		CardData SelectBestCard (List<CardData> validCards) {
			if (validCards.Count == 0) return null;

			// Simple AI Strategy:
			// 1. Prefer special cards over number cards (based on preference setting)
			// 2. Within same type, prefer higher numbers
			// 3. Add some randomness

			List<CardData> specialCards = validCards.Where (c => c.IsSpecialCard).ToList ();
			List<CardData> numberCards = validCards.Where (c => !c.IsSpecialCard).ToList ();

			// Decide whether to prioritize special cards
			bool useSpecialCard = specialCards.Count > 0 &&
								  Random.value < specialCardPreference;

			CardData selectedCard;

			if (useSpecialCard) {
				// Select from special cards
				selectedCard = SelectFromSpecialCards (specialCards);
				Debug.Log ($"AI chose special card strategy: {selectedCard.GetDisplayText ()}");
			} else if (numberCards.Count > 0) {
				// Select from number cards
				selectedCard = SelectFromNumberCards (numberCards);
				Debug.Log ($"AI chose number card strategy: {selectedCard.GetDisplayText ()}");
			} else if (specialCards.Count > 0) {
				// Fall back to special cards if no number cards available
				selectedCard = SelectFromSpecialCards (specialCards);
				Debug.Log ($"AI fell back to special cards: {selectedCard.GetDisplayText ()}");
			} else {
				// Random selection as last resort
				selectedCard = validCards [Random.Range (0, validCards.Count)];
				Debug.Log ($"AI made random selection: {selectedCard.GetDisplayText ()}");
			}

			return selectedCard;
		}

		/// <summary>
		/// Select best special card with simple priority
		/// </summary>
		/// <param name="specialCards">Available special cards</param>
		/// <returns>Selected special card</returns>
		CardData SelectFromSpecialCards (List<CardData> specialCards) {
			// Simple priority: Taki > PlusTwo > Stop > Plus > ChangeDirection > ChangeColor
			var priorityOrder = new CardType [] {
				CardType.Taki, CardType.SuperTaki, CardType.PlusTwo,
				CardType.Stop, CardType.Plus, CardType.ChangeDirection, CardType.ChangeColor
			};

			foreach (CardType priority in priorityOrder) {
				var cardsOfType = specialCards.Where (c => c.cardType == priority).ToList ();
				if (cardsOfType.Count > 0) {
					return cardsOfType [Random.Range (0, cardsOfType.Count)];
				}
			}

			// Fallback to random selection
			return specialCards [Random.Range (0, specialCards.Count)];
		}

		/// <summary>
		/// Select best number card (prefer higher numbers)
		/// </summary>
		/// <param name="numberCards">Available number cards</param>
		/// <returns>Selected number card</returns>
		CardData SelectFromNumberCards (List<CardData> numberCards) {
			// Prefer higher numbers with some randomness
			var sortedCards = numberCards.OrderByDescending (c => c.number).ToList ();

			// Select from top 50% of cards to add some randomness
			int selectionRange = Mathf.Max (1, sortedCards.Count / 2);
			return sortedCards [Random.Range (0, selectionRange)];
		}

		/// <summary>
		/// Play the selected card
		/// </summary>
		/// <param name="card">Card to play</param>
		void PlayCard (CardData card) {
			if (card == null) return;

			// Remove card from hand
			computerHand.Remove (card);

			Debug.Log ($"AI plays: {card.GetDisplayText ()} (Hand size now: {computerHand.Count})");
			OnAIDecisionMade?.Invoke ($"AI played {card.GetDisplayText ()}");
			OnAICardSelected?.Invoke (card);
		}

		/// <summary>
		/// AI draws a card when no valid moves
		/// </summary>
		void DrawCard () {
			Debug.Log ("AI draws a card (no valid moves)");
			OnAIDecisionMade?.Invoke ("AI drew a card");
			OnAIDrawCard?.Invoke ();
		}

		/// <summary>
		/// AI selects a color (for ChangeColor cards)
		/// </summary>
		/// <returns>Selected color</returns>
		public CardColor SelectColor () {
			// Simple strategy: Select color that appears most in hand
			var colorCounts = new Dictionary<CardColor, int> ();

			foreach (CardData card in computerHand) {
				if (card.color != CardColor.Wild) {
					if (colorCounts.ContainsKey (card.color)) {
						colorCounts [card.color]++;
					} else {
						colorCounts [card.color] = 1;
					}
				}
			}

			// Select most common color, or random if tie/no cards
			if (colorCounts.Count > 0) {
				var bestColor = colorCounts.OrderByDescending (kvp => kvp.Value).First ().Key;
				Debug.Log ($"AI selected color: {bestColor} (appears {colorCounts [bestColor]} times in hand)");
				OnAIColorSelected?.Invoke (bestColor);
				return bestColor;
			} else {
				// Random color selection
				CardColor [] colors = { CardColor.Red, CardColor.Blue, CardColor.Green, CardColor.Yellow };
				CardColor randomColor = colors [Random.Range (0, colors.Length)];
				Debug.Log ($"AI selected random color: {randomColor}");
				OnAIColorSelected?.Invoke (randomColor);
				return randomColor;
			}
		}

		/// <summary>
		/// Add cards to AI hand
		/// </summary>
		/// <param name="cards">Cards to add</param>
		public void AddCardsToHand (List<CardData> cards) {
			computerHand.AddRange (cards);
			Debug.Log ($"AI received {cards.Count} cards. Hand size: {computerHand.Count}");
		}

		/// <summary>
		/// Add single card to AI hand
		/// </summary>
		/// <param name="card">Card to add</param>
		public void AddCardToHand (CardData card) {
			if (card != null) {
				computerHand.Add (card);
				Debug.Log ($"AI received card: {card.GetDisplayText ()}. Hand size: {computerHand.Count}");
			}
		}

		/// <summary>
		/// Clear AI hand for new game
		/// </summary>
		public void ClearHand () {
			computerHand.Clear ();
			Debug.Log ("AI hand cleared");
		}

		/// <summary>
		/// Find current top discard card (helper method)
		/// </summary>
		/// <returns>Top discard card or null</returns>
		CardData FindTopDiscardCard () {
			// This will be connected to DeckManager in the coordinator
			// For now, return null and let the coordinator handle this
			return null;
		}

		/// <summary>
		/// Get a copy of the computer's hand for visual display
		/// </summary>
		/// <returns>Copy of computer's hand</returns>
		public List<CardData> GetHandCopy () {
			// Return a copy to prevent external modification
			return new List<CardData> (computerHand);
		}

		/// <summary>
		/// Debug method to log computer's current hand
		/// Useful for testing visual card system
		/// </summary>
		public void LogCurrentHand () {
			Debug.Log ($"Computer AI Hand ({computerHand.Count} cards):");
			for (int i = 0; i < computerHand.Count; i++) {
				Debug.Log ($"  [{i}] {computerHand [i].GetDisplayText ()}");
			}
		}

		// Properties
		public int HandSize => computerHand.Count;
		public bool HasCards => computerHand.Count > 0;
		public List<CardData> Hand => new List<CardData> (computerHand); // Safe copy
	}
}