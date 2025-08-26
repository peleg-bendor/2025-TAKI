using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace TakiGame {
	/// <summary>
	/// Editor script to automatically generate all 110 CardData assets for TAKI deck
	/// </summary>
	public class TakiDeckGenerator : EditorWindow {

		[Header ("Generation Settings")]
		private string outputPath = "Assets/Resources/Data/Cards/";
		private bool clearExistingCards = true;
		private bool generateSprites = false; // For now, we'll leave sprites empty

		[MenuItem ("Taki/Generate Complete Deck")]
		public static void ShowWindow () {
			GetWindow<TakiDeckGenerator> ("TAKI Deck Generator");
		}

		void OnGUI () {
			GUILayout.Label ("TAKI Deck Generator", EditorStyles.boldLabel);
			GUILayout.Space (10);

			outputPath = EditorGUILayout.TextField ("Output Path:", outputPath);
			clearExistingCards = EditorGUILayout.Toggle ("Clear Existing Cards", clearExistingCards);
			generateSprites = EditorGUILayout.Toggle ("Generate Placeholder Sprites", generateSprites);

			GUILayout.Space (10);

			// Deck composition display
			GUILayout.Label ("Deck Composition (110 Cards Total):", EditorStyles.boldLabel);
			GUILayout.Label ("• Number Cards: 64 (Numbers 1, 3-9, 2 copies each per color)");
			GUILayout.Label ("• Special Color Cards: 40 (PlusTwo, Plus, Stop, ChangeDirection, Taki)");
			GUILayout.Label ("• Wild Cards: 6 (SuperTaki x2, ChangeColor x4)");

			GUILayout.Space (20);

			if (GUILayout.Button ("Generate Complete Deck", GUILayout.Height (30))) {
				GenerateCompleteDeck ();
			}

			if (GUILayout.Button ("Clear All Cards", GUILayout.Height (20))) {
				ClearExistingCards ();
			}
		}

		void GenerateCompleteDeck () {
			// Ensure output directory exists
			if (!Directory.Exists (outputPath)) {
				Directory.CreateDirectory (outputPath);
			}

			// Clear existing if requested
			if (clearExistingCards) {
				ClearExistingCards ();
			}

			int cardsGenerated = 0;

			// Generate Number Cards (64 total)
			// Numbers: 1, 3, 4, 5, 6, 7, 8, 9 (excluding 2)
			int [] numbers = { 1, 3, 4, 5, 6, 7, 8, 9 };
			CardColor [] colors = { CardColor.Red, CardColor.Blue, CardColor.Green, CardColor.Yellow };

			foreach (CardColor color in colors) {
				foreach (int number in numbers) {
					// Create 2 copies of each number per color
					for (int copy = 1; copy <= 2; copy++) {
						cardsGenerated += CreateNumberCard (color, number, copy);
					}
				}
			}

			// Generate Special Color Cards (40 total)
			CardType [] specialTypes = {
				CardType.PlusTwo,
				CardType.Plus,
				CardType.Stop,
				CardType.ChangeDirection,
				CardType.Taki
			};

			foreach (CardColor color in colors) {
				foreach (CardType specialType in specialTypes) {
					// Create 2 copies of each special card per color
					for (int copy = 1; copy <= 2; copy++) {
						cardsGenerated += CreateSpecialCard (color, specialType, copy);
					}
				}
			}

			// Generate Wild Cards (6 total)
			// SuperTaki: 2 cards
			for (int copy = 1; copy <= 2; copy++) {
				cardsGenerated += CreateWildCard (CardType.SuperTaki, copy);
			}

			// ChangeColor: 4 cards  
			for (int copy = 1; copy <= 4; copy++) {
				cardsGenerated += CreateWildCard (CardType.ChangeColor, copy);
			}

			// Refresh the AssetDatabase
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();

			Debug.Log ($"TAKI Deck Generation Complete! Generated {cardsGenerated} cards.");

			// Verify count
			if (cardsGenerated == 110) {
				Debug.Log ("✅ Deck composition verified: 110 cards total");
			} else {
				Debug.LogWarning ($"⚠️ Expected 110 cards, but generated {cardsGenerated}");
			}
		}

		int CreateNumberCard (CardColor color, int number, int copyNumber) {
			string fileName = $"{color}_{number}_{copyNumber:D2}.asset";
			string assetPath = Path.Combine (outputPath, fileName);

			// Check if already exists
			if (File.Exists (assetPath)) {
				Debug.LogWarning ($"Card already exists: {fileName}");
				return 0;
			}

			CardData card = ScriptableObject.CreateInstance<CardData> ();
			card.number = number;
			card.color = color;
			card.cardType = CardType.Number;
			card.cardName = $"{color} {number}";
			card.isActiveCard = true;

			AssetDatabase.CreateAsset (card, assetPath);
			return 1;
		}

		int CreateSpecialCard (CardColor color, CardType cardType, int copyNumber) {
			string fileName = $"{color}_{cardType}_{copyNumber:D2}.asset";
			string assetPath = Path.Combine (outputPath, fileName);

			// Check if already exists
			if (File.Exists (assetPath)) {
				Debug.LogWarning ($"Card already exists: {fileName}");
				return 0;
			}

			CardData card = ScriptableObject.CreateInstance<CardData> ();
			card.number = 0; // Special cards don't have numbers
			card.color = color;
			card.cardType = cardType;
			card.cardName = $"{color} {cardType}";

			// Set activeCard status based on card type
			switch (cardType) {
				case CardType.Plus:
				case CardType.Stop:
				case CardType.ChangeDirection:
				case CardType.PlusTwo:
					card.isActiveCard = false; // End turn after playing
					break;
				case CardType.Taki:
					card.isActiveCard = true; // Allows continued play
					break;
			}

			AssetDatabase.CreateAsset (card, assetPath);
			return 1;
		}

		int CreateWildCard (CardType cardType, int copyNumber) {
			string fileName = $"Wild_{cardType}_{copyNumber:D2}.asset";
			string assetPath = Path.Combine (outputPath, fileName);

			// Check if already exists
			if (File.Exists (assetPath)) {
				Debug.LogWarning ($"Card already exists: {fileName}");
				return 0;
			}

			CardData card = ScriptableObject.CreateInstance<CardData> ();
			card.number = 0;
			card.color = CardColor.Wild;
			card.cardType = cardType;
			card.cardName = cardType.ToString ();

			// Wild cards behavior
			switch (cardType) {
				case CardType.SuperTaki:
					card.isActiveCard = true; // Allows multi-card play
					break;
				case CardType.ChangeColor:
					card.isActiveCard = false; // Requires color selection
					break;
			}

			AssetDatabase.CreateAsset (card, assetPath);
			return 1;
		}

		void ClearExistingCards () {
			if (!Directory.Exists (outputPath)) {
				Debug.Log ("Output directory doesn't exist, nothing to clear.");
				return;
			}

			string [] existingAssets = Directory.GetFiles (outputPath, "*.asset");
			int deletedCount = 0;

			foreach (string assetPath in existingAssets) {
				AssetDatabase.DeleteAsset (assetPath);
				deletedCount++;
			}

			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();

			Debug.Log ($"Cleared {deletedCount} existing card assets.");
		}
	}
}