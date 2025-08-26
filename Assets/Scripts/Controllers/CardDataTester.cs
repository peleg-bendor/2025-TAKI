using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace TakiGame {
	/// <summary>
	/// Temporary script to test CardData integration with UI
	/// This will be removed once we build the proper systems
	/// </summary>
	public class CardDataTester : MonoBehaviour {
		[Header ("UI References")]
		[Tooltip ("Drag your GameMessageText here - works with both Text and TextMeshPro")]
		public Text gameMessageText;        // For classic UI Text
		public TextMeshProUGUI gameMessageTMP; // For TextMeshPro

		[Header ("Test Cards")]
		[Tooltip ("Drag some of your CardData assets here to test")]
		public List<CardData> testCards = new List<CardData> ();

		[Header ("Test Controls")]
		public KeyCode testKey = KeyCode.T;

		private int currentCardIndex = 0;

		void Start () {
			if (gameMessageText == null && gameMessageTMP == null) {
				Debug.LogError ("Neither GameMessageText nor GameMessageTMP assigned to CardDataTester!");
				return;
			}

			if (testCards.Count == 0) {
				SetMessageText ("No test cards assigned! Drag CardData assets to the Test Cards list.");
				return;
			}

			// Display the first card
			DisplayCurrentCard ();
		}

		void Update () {
			// Press T key to cycle through test cards
			if (Input.GetKeyDown (testKey) && testCards.Count > 0) {
				CycleToNextCard ();
			}
		}

		/// <summary>
		/// Helper method to set text on either Text or TextMeshPro component
		/// </summary>
		void SetMessageText (string message) {
			if (gameMessageText != null) {
				gameMessageText.text = message;
			} else if (gameMessageTMP != null) {
				gameMessageTMP.text = message;
			}
		}

		void CycleToNextCard () {
			currentCardIndex = (currentCardIndex + 1) % testCards.Count;
			DisplayCurrentCard ();
		}

		void DisplayCurrentCard () {
			if (testCards.Count == 0 || currentCardIndex >= testCards.Count)
				return;

			CardData card = testCards [currentCardIndex];

			if (card == null) {
				SetMessageText ("Card data is null!");
				return;
			}

			// Test our CardData properties and helper methods
			string cardInfo = $"CARD TEST {currentCardIndex + 1}/{testCards.Count}\n";
			cardInfo += $"Name: {card.cardName}";
			cardInfo += $", Display: {card.GetDisplayText ()}";
			cardInfo += $", Type: {card.cardType}";
			cardInfo += $", Color: {card.color}";

			if (card.cardType == CardType.Number) {
				cardInfo += $", Number: {card.number}\n";
			}

			cardInfo += $"Special: {(card.IsSpecialCard ? "Yes" : "No")}\n";
			cardInfo += $"Wild: {(card.IsWildCard ? "Yes" : "No")}\n";
			cardInfo += $"Active: {(card.isActiveCard ? "Yes" : "No")}\n";

			// Test card matching logic
			if (testCards.Count > 1) {
				CardData otherCard = testCards [(currentCardIndex + 1) % testCards.Count];
				bool canPlay = card.CanPlayOn (otherCard, otherCard.color);
				cardInfo += $"Can play on {otherCard.cardName}: {(canPlay ? "Yes" : "No")}\n";
			}

			cardInfo += "\nPress T to cycle through cards";

			SetMessageText (cardInfo);
		}

		/// <summary>
		/// Test method that can be called from buttons
		/// </summary>
		public void OnTestButtonClicked () {
			if (testCards.Count > 0) {
				CycleToNextCard ();
			} else {
				SetMessageText ("No test cards assigned!");
			}
		}
	}
}