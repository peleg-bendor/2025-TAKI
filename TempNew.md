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
			TakiLogger.LogAI ($"=== AI MAKING DECISION ===");
			TakiLogger.LogAI ($"AI Hand size: {computerHand.Count}");
			TakiLogger.LogAI ($"Top discard card: {topDiscardCard?.GetDisplayText () ?? "NULL"}");

			if (topDiscardCard == null) {
				TakiLogger.LogError ("AI cannot make decision: No top discard card provided", TakiLogger.LogCategory.AI);
				OnAIDecisionMade?.Invoke ("Error: No discard card");
				return;
			}

			TakiLogger.LogAI ($"AI thinking... (Hand size: {computerHand.Count})", TakiLogger.LogLevel.Debug);
			OnAIDecisionMade?.Invoke ("AI is thinking...");

			// Store the top card for ExecuteDecision to use
			currentTopDiscardCard = topDiscardCard;

			// Add thinking delay
			Invoke (nameof (ExecuteDecision), thinkingTime);
		}

		// ADDED: Store current top card for ExecuteDecision
		private CardData currentTopDiscardCard;

		/// <summary>
		/// Execute the AI decision after thinking time
		/// </summary>
		void ExecuteDecision () {
			TakiLogger.LogAI ($"=== AI EXECUTING DECISION ===");

			if (currentTopDiscardCard == null) {
				TakiLogger.LogError ("ExecuteDecision: No current top discard card stored", TakiLogger.LogCategory.AI);
				DrawCard ();
				return;
			}

			// Find all valid cards to play
			List<CardData> validCards = GetValidCards (currentTopDiscardCard);
			TakiLogger.LogAI ($"AI found {validCards.Count} valid cards from {computerHand.Count} total");

			if (validCards.Count > 0) {
				// Select best card to play
				CardData selectedCard = SelectBestCard (validCards);
				TakiLogger.LogAI ($"AI selected card: {selectedCard?.GetDisplayText ()}", TakiLogger.LogLevel.Info);
				PlayCard (selectedCard);
			} else {
				// No valid cards, must draw
				TakiLogger.LogAI ("AI has no valid moves - drawing card");
				DrawCard ();
			}

			// Clear the stored card
			currentTopDiscardCard = null;
		}

		/// <summary>
		/// Get all cards that can be legally played
		/// </summary>
		/// <param name="topDiscardCard">Current top discard card</param>
		/// <returns>List of playable cards</returns>
		List<CardData> GetValidCards (CardData topDiscardCard) {
			List<CardData> validCards = new List<CardData> ();

			if (gameState == null) {
				TakiLogger.LogError ("Cannot get valid cards: GameState is null", TakiLogger.LogCategory.AI);
				return validCards;
			}

			if (topDiscardCard == null) {
				TakiLogger.LogError ("Cannot get valid cards: topDiscardCard is null", TakiLogger.LogCategory.AI);
				return validCards;
			}

			TakiLogger.LogAI ($"Checking {computerHand.Count} cards against {topDiscardCard.GetDisplayText ()}", TakiLogger.LogLevel.Debug);

			foreach (CardData card in computerHand) {
				if (card == null) {
					TakiLogger.LogWarning ("Found null card in AI hand - skipping", TakiLogger.LogCategory.AI);
					continue;
				}

				bool isValid = gameState.IsValidMove (card, topDiscardCard);
				TakiLogger.LogRules ($"  {card.GetDisplayText ()} -> {isValid}", TakiLogger.LogLevel.Verbose);

				if (isValid) {
					validCards.Add (card);
				}
			}

			TakiLogger.LogAI ($"AI found {validCards.Count} valid cards to play", TakiLogger.LogLevel.Debug);
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
				TakiLogger.LogAI ($"AI chose special card strategy: {selectedCard.GetDisplayText ()}", TakiLogger.LogLevel.Debug);
			} else if (numberCards.Count > 0) {
				// Select from number cards
				selectedCard = SelectFromNumberCards (numberCards);
				TakiLogger.LogAI ($"AI chose number card strategy: {selectedCard.GetDisplayText ()}", TakiLogger.LogLevel.Debug);
			} else if (specialCards.Count > 0) {
				// Fall back to special cards if no number cards available
				selectedCard = SelectFromSpecialCards (specialCards);
				TakiLogger.LogAI ($"AI fell back to special cards: {selectedCard.GetDisplayText ()}", TakiLogger.LogLevel.Debug);
			} else {
				// Random selection as last resort
				selectedCard = validCards [Random.Range (0, validCards.Count)];
				TakiLogger.LogAI ($"AI made random selection: {selectedCard.GetDisplayText ()}", TakiLogger.LogLevel.Debug);
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
			if (card == null) {
				TakiLogger.LogError ("AI PlayCard called with null card", TakiLogger.LogCategory.AI);
				return;
			}

			// Check if card is actually in hand
			if (!computerHand.Contains (card)) {
				TakiLogger.LogError ($"AI trying to play card not in hand: {card.GetDisplayText ()}", TakiLogger.LogCategory.AI);
				return;
			}

			// Remove card from hand
			bool removed = computerHand.Remove (card);
			TakiLogger.LogCardPlay ($"AI plays: {card.GetDisplayText ()} (Removed: {removed}, Hand size now: {computerHand.Count})");

			OnAIDecisionMade?.Invoke ($"AI played {card.GetDisplayText ()}");
			OnAICardSelected?.Invoke (card);
		}

		/// <summary>
		/// AI draws a card when no valid moves
		/// </summary>
		void DrawCard () {
			TakiLogger.LogCardPlay ($"AI draws a card (no valid moves) - Current hand: {computerHand.Count}");
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
				TakiLogger.LogAI ($"AI selected color: {bestColor} (appears {colorCounts [bestColor]} times in hand)", TakiLogger.LogLevel.Info);
				OnAIColorSelected?.Invoke (bestColor);
				return bestColor;
			} else {
				// Random color selection
				CardColor [] colors = { CardColor.Red, CardColor.Blue, CardColor.Green, CardColor.Yellow };
				CardColor randomColor = colors [Random.Range (0, colors.Length)];
				TakiLogger.LogAI ($"AI selected random color: {randomColor}", TakiLogger.LogLevel.Info);
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
			TakiLogger.LogAI ($"AI received {cards.Count} cards. Hand size: {computerHand.Count}", TakiLogger.LogLevel.Debug);
		}

		/// <summary>
		/// Add single card to AI hand
		/// </summary>
		/// <param name="card">Card to add</param>
		public void AddCardToHand (CardData card) {
			if (card != null) {
				computerHand.Add (card);
				TakiLogger.LogAI ($"AI received card: {card.GetDisplayText ()}. Hand size: {computerHand.Count}", TakiLogger.LogLevel.Debug);
			}
		}

		/// <summary>
		/// Clear AI hand for new game
		/// </summary>
		public void ClearHand () {
			computerHand.Clear ();
			TakiLogger.LogAI ("AI hand cleared", TakiLogger.LogLevel.Debug);
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
			TakiLogger.LogDiagnostics ($"Computer AI Hand ({computerHand.Count} cards):");
			for (int i = 0; i < computerHand.Count; i++) {
				TakiLogger.LogDiagnostics ($"  [{i}] {computerHand [i].GetDisplayText ()}");
			}
		}

		/// <summary>
		/// DEBUGGING: Log current AI state
		/// </summary>
		[ContextMenu ("Log AI State")]
		public void LogAIDebugState () {
			TakiLogger.LogDiagnostics ("=== AI DEBUG STATE ===");
			TakiLogger.LogDiagnostics ($"Hand size: {computerHand.Count}");
			TakiLogger.LogDiagnostics ($"Has GameState: {gameState != null}");
			TakiLogger.LogDiagnostics ($"Current top card: {currentTopDiscardCard?.GetDisplayText () ?? "NULL"}");

			TakiLogger.LogDiagnostics ("Cards in hand:");
			for (int i = 0; i < computerHand.Count; i++) {
				TakiLogger.LogDiagnostics ($"  [{i}] {computerHand [i]?.GetDisplayText () ?? "NULL"}");
			}
		}

		// Properties
		public int HandSize => computerHand.Count;
		public bool HasCards => computerHand.Count > 0;
		public List<CardData> Hand => new List<CardData> (computerHand); // Safe copy
	}
}