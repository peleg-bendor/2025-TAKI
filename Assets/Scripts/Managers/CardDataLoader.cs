using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TakiGame {
	/// <summary>
	/// Handles loading and validating CardData from Resources
	/// NO deck operations, NO UI updates, NO game logic
	/// </summary>
	public class CardDataLoader : MonoBehaviour {

		[Header ("Card Data")]
		[Tooltip ("All loaded card data from Resources")]
		public List<CardData> allCardData = new List<CardData> ();

		[Header ("Validation")]
		[Tooltip ("Expected total number of cards in a complete TAKI deck")]
		private const int EXPECTED_DECK_SIZE = 110;

		// Events
		public System.Action<List<CardData>> OnCardsLoaded;
		public System.Action<string> OnLoadError;

		void Start () {
			LoadAllCardData ();
		}

		/// <summary>
		/// Load all CardData assets from Resources/Data/Cards
		/// </summary>
		public List<CardData> LoadAllCardData () {
			// Load all CardData assets from the Cards folder
			CardData [] loadedCards = Resources.LoadAll<CardData> ("Data/Cards");

			if (loadedCards.Length == 0) {
				string errorMsg = "No CardData found in Resources/Data/Cards! Make sure cards are in Resources folder.";
				Debug.LogError (errorMsg);
				OnLoadError?.Invoke (errorMsg);
				return new List<CardData> ();
			}

			allCardData.Clear ();
			allCardData.AddRange (loadedCards);

			Debug.Log ($"Loaded {allCardData.Count} cards from Resources/Data/Cards");

			// Validate deck composition
			bool isValid = ValidateDeckComposition ();

			if (isValid) {
				Debug.Log ("Deck composition verified: All cards loaded successfully");
				OnCardsLoaded?.Invoke (allCardData);
			} else {
				string errorMsg = $"Deck composition invalid! Expected {EXPECTED_DECK_SIZE} cards, loaded {allCardData.Count}";
				Debug.LogWarning (errorMsg);
				OnLoadError?.Invoke (errorMsg);
			}

			return allCardData;
		}

		/// <summary>
		/// Validate that we have the correct deck composition
		/// </summary>
		/// <returns>True if deck composition is correct</returns>
		public bool ValidateDeckComposition () {
			if (allCardData.Count != EXPECTED_DECK_SIZE) {
				Debug.LogWarning ($"Expected {EXPECTED_DECK_SIZE} cards, but loaded {allCardData.Count}");
				return false;
			}

			// Additional validation could be added here:
			// - Check for correct number of each card type
			// - Validate card data integrity
			// - Ensure no duplicate cards

			return true;
		}

		/// <summary>
		/// Get detailed statistics about the loaded deck
		/// </summary>
		/// <returns>Formatted string with deck statistics</returns>
		public string GetDeckStats () {
			if (allCardData.Count == 0) {
				return "No cards loaded";
			}

			var stats = allCardData.GroupBy (card => card.cardType)
								   .Select (group => $"{group.Key}: {group.Count ()}")
								   .ToArray ();

			return $"Total Cards: {allCardData.Count}\n" + string.Join ("\n", stats);
		}

		/// <summary>
		/// Get all cards of a specific type
		/// </summary>
		/// <param name="cardType">Type of cards to retrieve</param>
		/// <returns>List of cards of the specified type</returns>
		public List<CardData> GetCardsByType (CardType cardType) {
			return allCardData.Where (card => card.cardType == cardType).ToList ();
		}

		/// <summary>
		/// Get all cards of a specific color
		/// </summary>
		/// <param name="color">Color of cards to retrieve</param>
		/// <returns>List of cards of the specified color</returns>
		public List<CardData> GetCardsByColor (CardColor color) {
			return allCardData.Where (card => card.color == color).ToList ();
		}

		/// <summary>
		/// Get a copy of all card data (safe for deck initialization)
		/// </summary>
		/// <returns>New list containing all cards</returns>
		public List<CardData> GetAllCardsForDeck () {
			return new List<CardData> (allCardData);
		}

		/// <summary>
		/// Reload cards from Resources (useful for testing)
		/// </summary>
		public void ReloadCards () {
			LoadAllCardData ();
		}

		// Properties
		public int TotalCardCount => allCardData.Count;
		public bool HasValidDeck => allCardData.Count == EXPECTED_DECK_SIZE;
		public bool IsLoaded => allCardData.Count > 0;
	}
}