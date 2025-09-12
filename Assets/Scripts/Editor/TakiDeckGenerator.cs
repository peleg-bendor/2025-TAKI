using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace TakiGame {
	/// <summary>
	/// PHASE 7: Editor script to automatically generate all 110 CardData assets for TAKI deck
	/// UPDATED: isActiveCard property assignments for Phase 7 Special Cards Implementation
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
			GUILayout.Label ("TAKI Deck Generator - PHASE 7", EditorStyles.boldLabel);
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

			GUILayout.Space (10);

			// PHASE 7: Updated turn behavior display
			GUILayout.Label ("PHASE 7: Special Card Turn Behavior:", EditorStyles.boldLabel);
			GUILayout.Label ("• Number Cards: END turn after play (isActiveCard = false)");
			GUILayout.Label ("• Plus Cards: CONTINUE turn for additional action (isActiveCard = true)", EditorStyles.boldLabel);
			GUILayout.Label ("• Stop Cards: END turn after play (isActiveCard = false)");
			GUILayout.Label ("• ChangeDirection: END turn after play (isActiveCard = false)");
			GUILayout.Label ("• ChangeColor: END turn after color selection (isActiveCard = false)");
			GUILayout.Label ("• TAKI & SuperTAKI: CONTINUE turn - Phase 8 (isActiveCard = true)");
			GUILayout.Label ("• PlusTwo: END turn - Phase 8 chaining (isActiveCard = false)");

			GUILayout.Space (20);

			if (GUILayout.Button ("Generate Complete Deck - PHASE 7", GUILayout.Height (30))) {
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

			TakiLogger.LogInfo ($"TAKI Deck Generation Complete! Generated {cardsGenerated} cards.", TakiLogger.LogCategory.System);
			TakiLogger.LogInfo ("Updated Plus cards to isActiveCard = true for additional actions", TakiLogger.LogCategory.System);

			// Verify count
			if (cardsGenerated == 110) {
				TakiLogger.LogInfo ("Deck composition verified: 110 cards total", TakiLogger.LogCategory.System);
			} else {
				TakiLogger.LogWarning ($"Expected 110 cards, but generated {cardsGenerated}", TakiLogger.LogCategory.System);
			}
		}

		int CreateNumberCard (CardColor color, int number, int copyNumber) {
			string fileName = $"{color}_{number}_{copyNumber:D2}.asset";
			string assetPath = Path.Combine (outputPath, fileName);

			// Check if already exists
			if (File.Exists (assetPath)) {
				TakiLogger.LogWarning ($"Card already exists: {fileName}", TakiLogger.LogCategory.System);
				return 0;
			}

			CardData card = ScriptableObject.CreateInstance<CardData> ();
			card.number = number;
			card.color = color;
			card.cardType = CardType.Number;
			card.cardName = $"{color} {number}";

			// Number cards should END turn after being played
			card.isActiveCard = false;

			AssetDatabase.CreateAsset (card, assetPath);
			TakiLogger.LogInfo ($"Generated NUMBER card: {card.cardName} (isActiveCard = {card.isActiveCard})", TakiLogger.LogCategory.System);
			return 1;
		}

		int CreateSpecialCard (CardColor color, CardType cardType, int copyNumber) {
			string fileName = $"{color}_{cardType}_{copyNumber:D2}.asset";
			string assetPath = Path.Combine (outputPath, fileName);

			// Check if already exists
			if (File.Exists (assetPath)) {
				TakiLogger.LogWarning ($"Card already exists: {fileName}", TakiLogger.LogCategory.System);
				return 0;
			}

			CardData card = ScriptableObject.CreateInstance<CardData> ();
			card.number = 0; // Special cards don't have numbers
			card.color = color;
			card.cardType = cardType;
			card.cardName = $"{color} {cardType}";

			// PHASE 7: Updated activeCard status based on special card effects
			switch (cardType) {
				case CardType.Plus:
					// PHASE 7 CHANGE: Plus cards now allow additional action
					card.isActiveCard = true; // NEW: Allow additional action after playing
					TakiLogger.LogInfo ($"Plus card set to isActiveCard = true for additional actions", TakiLogger.LogCategory.System);
					break;
				case CardType.Stop:
					card.isActiveCard = false; // END turn after playing (turn skip handled in game logic)
					break;
				case CardType.ChangeDirection:
					card.isActiveCard = false; // END turn after playing (direction change handled in game logic)
					break;
				case CardType.PlusTwo:
					card.isActiveCard = false; // END turn after playing - Phase 8 will add chaining
					break;
				case CardType.Taki:
					card.isActiveCard = true; // Allows continued play - Phase 8 implementation
					break;
				default:
					// Default to ending turn for safety
					card.isActiveCard = false;
					TakiLogger.LogWarning ($"Unknown special card type: {cardType} - defaulting to isActiveCard = false", TakiLogger.LogCategory.System);
					break;
			}

			AssetDatabase.CreateAsset (card, assetPath);
			TakiLogger.LogInfo ($"Generated SPECIAL card: {card.cardName} (isActiveCard = {card.isActiveCard})", TakiLogger.LogCategory.System);
			return 1;
		}

		int CreateWildCard (CardType cardType, int copyNumber) {
			string fileName = $"Wild_{cardType}_{copyNumber:D2}.asset";
			string assetPath = Path.Combine (outputPath, fileName);

			// Check if already exists
			if (File.Exists (assetPath)) {
				TakiLogger.LogWarning ($"Card already exists: {fileName}", TakiLogger.LogCategory.System);
				return 0;
			}

			CardData card = ScriptableObject.CreateInstance<CardData> ();
			card.number = 0;
			card.color = CardColor.Wild;
			card.cardType = cardType;
			card.cardName = cardType.ToString ();

			// Wild cards behavior according to TAKI rules
			switch (cardType) {
				case CardType.SuperTaki:
					card.isActiveCard = true; // Allows multi-card play - Phase 8 implementation
					break;
				case CardType.ChangeColor:
					// PHASE 7: ChangeColor cards END turn after color selection
					card.isActiveCard = false; // END turn after color selection
					break;
				default:
					// Default to ending turn for safety 
					card.isActiveCard = false;
					TakiLogger.LogWarning ($"Unknown wild card type: {cardType} - defaulting to isActiveCard = false", TakiLogger.LogCategory.System);
					break;
			}

			AssetDatabase.CreateAsset (card, assetPath);
			TakiLogger.LogInfo ($"Generated WILD card: {card.cardName} (isActiveCard = {card.isActiveCard})", TakiLogger.LogCategory.System);
			return 1;
		}

		void ClearExistingCards () {
			if (!Directory.Exists (outputPath)) {
				TakiLogger.LogInfo ("Output directory doesn't exist, nothing to clear.", TakiLogger.LogCategory.System);
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

			TakiLogger.LogInfo ($"Cleared {deletedCount} existing card assets.", TakiLogger.LogCategory.System);
		}
	}
}