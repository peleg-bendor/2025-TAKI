using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// MILESTONE 1: Helper class for creating CardData at runtime
	/// Since CardData is a ScriptableObject, we need helper methods instead of constructors
	/// </summary>
	public static class CardDataHelper {

		/// <summary>
		/// Create a runtime CardData instance (for network synchronization)
		/// NOTE: This creates a temporary CardData for network purposes only
		/// For actual gameplay, use the loaded ScriptableObject cards from CardDataLoader
		/// </summary>
		/// <param name="color">Card color</param>
		/// <param name="cardType">Card type</param>
		/// <param name="number">Card number (for number cards only)</param>
		/// <returns>Runtime CardData instance</returns>
		public static CardData CreateRuntimeCard (CardColor color, CardType cardType, int number = 0) {
			CardData runtimeCard = ScriptableObject.CreateInstance<CardData> ();

			runtimeCard.color = color;
			runtimeCard.cardType = cardType;
			runtimeCard.number = number;

			// Set appropriate card name
			if (cardType == CardType.Number) {
				runtimeCard.cardName = $"{color} {number}";
			} else {
				runtimeCard.cardName = $"{color} {cardType}";
			}

			// Set isActiveCard based on card type
			runtimeCard.isActiveCard = cardType != CardType.Number;

			return runtimeCard;
		}

		/// <summary>
		/// Find matching card from CardDataLoader instead of creating new one
		/// This is the preferred method for network synchronization
		/// </summary>
		/// <param name="cardLoader">CardDataLoader reference</param>
		/// <param name="color">Card color to find</param>
		/// <param name="cardType">Card type to find</param>
		/// <param name="number">Card number (for number cards)</param>
		/// <returns>Matching CardData from loaded assets, or null if not found</returns>
		public static CardData FindMatchingCard (CardDataLoader cardLoader, CardColor color, CardType cardType, int number = 0) {
			if (cardLoader == null || cardLoader.allCardData == null) {
				return null;
			}

			if (cardType == CardType.Number) {
				// Find number card with matching color and number
				return cardLoader.allCardData.Find (card =>
					card.color == color &&
					card.cardType == CardType.Number &&
					card.number == number);
			} else {
				// Find special card with matching color and type
				return cardLoader.allCardData.Find (card =>
					card.color == color &&
					card.cardType == cardType);
			}
		}

		/// <summary>
		/// Parse card identifier and find matching card from loader
		/// Format: "Color_Number" or "Color_CardType"
		/// </summary>
		/// <param name="cardLoader">CardDataLoader reference</param>
		/// <param name="cardIdentifier">Network card identifier</param>
		/// <returns>Matching CardData or null if not found</returns>
		public static CardData ParseCardIdentifier (CardDataLoader cardLoader, string cardIdentifier) {
			if (string.IsNullOrEmpty (cardIdentifier) || cardLoader == null) {
				return null;
			}

			// Parse format: "Color_Type" or "Color_Number"
			string [] parts = cardIdentifier.Split ('_');
			if (parts.Length != 2) return null;

			// Get color
			if (!System.Enum.TryParse<CardColor> (parts [0], out CardColor color)) {
				return null;
			}

			// Try to parse as number first
			if (int.TryParse (parts [1], out int number)) {
				return FindMatchingCard (cardLoader, color, CardType.Number, number);
			}

			// Try to parse as card type
			if (System.Enum.TryParse<CardType> (parts [1], out CardType cardType)) {
				return FindMatchingCard (cardLoader, color, cardType);
			}

			return null;
		}

		/// <summary>
		/// Create card identifier for network transmission
		/// </summary>
		/// <param name="card">CardData to create identifier for</param>
		/// <returns>Network identifier string</returns>
		public static string CreateCardIdentifier (CardData card) {
			if (card == null) return "";

			if (card.cardType == CardType.Number) {
				return $"{card.color}_{card.number}";
			} else {
				return $"{card.color}_{card.cardType}";
			}
		}
	}
}