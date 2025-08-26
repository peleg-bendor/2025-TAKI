using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// ScriptableObject that defines the properties of a TAKI card
	/// </summary>
	[CreateAssetMenu (fileName = "New Card", menuName = "Taki/Card")]
	public class CardData : ScriptableObject {
		[Header ("Card Properties")]
		[Tooltip ("Card number (1-9 for numbered cards, 0 for special cards)")]
		public int number;

		[Tooltip ("Card color")]
		public CardColor color;

		[Tooltip ("Type of card (Number, Plus, Stop, etc.)")]
		public CardType cardType;

		[Header ("Visual")]
		[Tooltip ("Sprite image for this card")]
		public Sprite cardSprite;

		[Header ("Game Logic")]
		[Tooltip ("Display name for this card")]
		public string cardName;

		[Tooltip ("Whether this card can be used in TAKI sequences")]
		public bool isActiveCard;

		// Helper properties for game logic
		/// <summary>
		/// Returns true if this is not a numbered card
		/// </summary>
		public bool IsSpecialCard => cardType != CardType.Number;

		/// <summary>
		/// Returns true if this card can be played on any color
		/// </summary>
		public bool IsWildCard => color == CardColor.Wild;

		/// <summary>
		/// Returns true if this card can chain with other +2 cards
		/// </summary>
		public bool CanChainPlusTwo => cardType == CardType.PlusTwo;

		/// <summary>
		/// Check if this card can be played on top of another card
		/// </summary>
		/// <param name="topCard">The card currently on top of the discard pile</param>
		/// <param name="currentColor">The active color (may differ from topCard.color if ChangeColor was played)</param>
		/// <returns>True if this card can be legally played</returns>
		public bool CanPlayOn (CardData topCard, CardColor currentColor) {
			// Rule 1: Wild cards (like SuperTaki) can be played on anything
			if (IsWildCard) return true;

			// Rule 2: Color matching
			// - Match the active color (important when ChangeColor was played)
			// - Match the top card's original color (backup rule)
			if (color == currentColor || color == topCard.color) return true;

			// Rule 3: Number matching (both cards must be numbered)
			// Example: Red 5 can be played on Blue 5
			if (cardType == CardType.Number && topCard.cardType == CardType.Number && number == topCard.number) return true;

			// Rule 4: Special card type matching
			// Example: Red Stop can be played on Blue Stop
			if (cardType == topCard.cardType && cardType != CardType.Number) return true;

			// No valid rule matched - cannot play this card
			return false;
		}

		/// <summary>
		/// Get display text for this card
		/// </summary>
		public string GetDisplayText () {
			if (cardType == CardType.Number) {
				return $"{color} {number}";
			} else {
				return $"{color} {cardType}";
			}
		}
	}
}