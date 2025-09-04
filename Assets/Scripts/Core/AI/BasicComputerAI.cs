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

		[Header ("Pause State Management")]
		[Tooltip ("AI state preservation during pause")]
		private bool isPaused = false;
		private CardData pausedTopDiscardCard = null;
		private bool wasThinkingWhenPaused = false;
		private float pausedThinkingTimeRemaining = 0f;
		private CardData currentChainPlusTwo = null;

		// Events for AI decisions
		public System.Action<CardData> OnAICardSelected;
		public System.Action OnAIDrawCard;
		public System.Action<CardColor> OnAIColorSelected;
		public System.Action<string> OnAIDecisionMade;

		// Events for AI pause system
		public System.Action OnAIPaused;
		public System.Action OnAIResumed;


		/// <summary>
		/// Make AI decision for current turn
		/// </summary>
		/// <param name="topDiscardCard">Current top card of discard pile</param>
		public void MakeDecision (CardData topDiscardCard) {
			// Check if AI is paused

			if (isPaused) {
				TakiLogger.LogAI ("AI decision requested but AI is paused");
				return;
			}

			TakiLogger.LogAI ($"=== AI MAKING DECISION (PLUSTWO CHAIN AWARE) ===");
			TakiLogger.LogAI ($"AI Hand size: {computerHand.Count}");
			TakiLogger.LogAI ($"Top discard card: {topDiscardCard?.GetDisplayText () ?? "NULL"}");
			TakiLogger.LogAI ($"PlusTwo chain active: {gameState?.IsPlusTwoChainActive ?? false}");

			if (topDiscardCard == null) {
				TakiLogger.LogError ("AI cannot make decision: No top discard card provided", TakiLogger.LogCategory.AI);
				OnAIDecisionMade?.Invoke ("Error: No discard card");
				return;
			}

			// CRITICAL: Check for active PlusTwo chain FIRST
			if (gameState != null && gameState.IsPlusTwoChainActive) {
				TakiLogger.LogAI ("=== AI HANDLING PLSTWO CHAIN DECISION ===");
				HandlePlusTwoChainDecision ();
				return; // Skip normal decision making
			}

			TakiLogger.LogAI ($"AI thinking... (Hand size: {computerHand.Count})", TakiLogger.LogLevel.Debug);
			OnAIDecisionMade?.Invoke ("AI is thinking...");

			// Store the top card for ExecuteDecision to use
			currentTopDiscardCard = topDiscardCard;

			// Add thinking delay - but check for pause during delay
			StartThinkingProcess ();
		}

		/// <summary>
		/// Handle AI decision when PlusTwo chain is active
		/// AI Strategy: ALWAYS continue chain if possible, otherwise draw to break
		/// </summary>
		void HandlePlusTwoChainDecision () {
			TakiLogger.LogAI ("=== AI PROCESSING PLSTWO CHAIN ===");

			int chainLength = gameState.NumberOfChainedCards;
			int drawCount = gameState.ChainDrawCount;

			TakiLogger.LogAI ($"Chain status: {chainLength} PlusTwo cards, {drawCount} cards to draw");

			// Check if AI has PlusTwo cards
			var plusTwoCards = computerHand.Where (card => card.cardType == CardType.PlusTwo).ToList ();

			TakiLogger.LogAI ($"AI has {plusTwoCards.Count} PlusTwo cards available");

			if (plusTwoCards.Count > 0) {
				// AI has PlusTwo cards - ALWAYS continue chain (aggressive strategy)
				CardData selectedPlusTwo = plusTwoCards [Random.Range (0, plusTwoCards.Count)];
				TakiLogger.LogAI ($"AI STRATEGY: Continue chain with {selectedPlusTwo.GetDisplayText ()}");
				TakiLogger.LogAI ($"Chain will grow to {chainLength + 1} cards ({drawCount + 2} total draw)");

				OnAIDecisionMade?.Invoke ($"AI continues chain with {selectedPlusTwo.GetDisplayText ()}!");

				// Store for execution after thinking time
				currentChainPlusTwo = selectedPlusTwo;
				Invoke (nameof (ExecuteChainContinue), thinkingTime);
			} else {
				// AI has no PlusTwo cards - must break chain by drawing
				TakiLogger.LogAI ($"AI STRATEGY: Break chain by drawing {drawCount} cards (no PlusTwo available)");

				OnAIDecisionMade?.Invoke ($"AI draws {drawCount} cards to break chain");
				Invoke (nameof (ExecuteChainBreak), thinkingTime);
			}
		}

		/// <summary>
		/// Execute AI decision to continue PlusTwo chain
		/// </summary>
		void ExecuteChainContinue () {
			// Double-check pause state
			if (isPaused) {
				TakiLogger.LogAI ("AI chain continue execution cancelled - AI is paused");
				return;
			}

			TakiLogger.LogAI ("=== AI EXECUTING CHAIN CONTINUE ===");

			if (currentChainPlusTwo == null) {
				TakiLogger.LogError ("AI trying to continue chain but no PlusTwo card stored!", TakiLogger.LogCategory.AI);
				ExecuteChainBreak (); // Fallback to breaking chain
				return;
			}

			// Verify card is still in hand and is still valid
			if (!computerHand.Contains (currentChainPlusTwo)) {
				TakiLogger.LogError ("AI trying to play PlusTwo not in hand!", TakiLogger.LogCategory.AI);
				ExecuteChainBreak (); // Fallback to breaking chain
				return;
			}

			TakiLogger.LogAI ($"AI playing PlusTwo to continue chain: {currentChainPlusTwo.GetDisplayText ()}");
			PlayCard (currentChainPlusTwo);

			// Clear stored card
			currentChainPlusTwo = null;
		}

		/// <summary>
		/// Execute AI decision to break PlusTwo chain by drawing
		/// </summary>
		void ExecuteChainBreak () {
			// Double-check pause state
			if (isPaused) {
				TakiLogger.LogAI ("AI chain break execution cancelled - AI is paused");
				return;
			}

			TakiLogger.LogAI ("=== AI EXECUTING CHAIN BREAK ===");

			int cardsToDraw = gameState?.ChainDrawCount ?? 2;
			TakiLogger.LogAI ($"AI breaking PlusTwo chain by drawing {cardsToDraw} cards");

			// Trigger AI draw event - GameManager will handle the multi-card draw
			OnAIDrawCard?.Invoke ();
		}

		/// <summary>
		/// Pause AI operations and preserve state
		/// </summary>
		public void PauseAI () {
			if (isPaused) {
				TakiLogger.LogAI ("AI is already paused");
				return;
			}

			TakiLogger.LogAI ("=== PAUSING AI ===");

			// Preserve current thinking state
			if (IsInvokePending (nameof (ExecuteDecision))) {
				wasThinkingWhenPaused = true;
				pausedTopDiscardCard = currentTopDiscardCard;
				// Calculate remaining thinking time (approximate)
				pausedThinkingTimeRemaining = thinkingTime * 0.5f; // Estimate half remaining

				TakiLogger.LogAI ("AI was thinking - state preserved");
			} else {
				wasThinkingWhenPaused = false;
				pausedTopDiscardCard = null;
				pausedThinkingTimeRemaining = 0f;
			}

			// Cancel all ongoing operations
			CancelAllAIOperations ();

			isPaused = true;
			OnAIPaused?.Invoke ();

			TakiLogger.LogAI ("AI paused successfully");
		}

		/// <summary>
		/// Resume AI operations and restore state
		/// </summary>
		public void ResumeAI () {
			if (!isPaused) {
				TakiLogger.LogAI ("AI is not currently paused");
				return;
			}

			TakiLogger.LogAI ("=== RESUMING AI ===");

			isPaused = false;

			// If AI was thinking when paused, resume the thinking process
			if (wasThinkingWhenPaused && pausedTopDiscardCard != null) {
				TakiLogger.LogAI ("Resuming AI thinking process");

				// Restore the thinking context
				currentTopDiscardCard = pausedTopDiscardCard;

				// Resume with remaining thinking time (or minimum time)
				float resumeThinkingTime = Mathf.Max (pausedThinkingTimeRemaining, 0.5f);
				Invoke (nameof (ExecuteDecision), resumeThinkingTime);

				OnAIDecisionMade?.Invoke ("AI is thinking...");
				TakiLogger.LogAI ($"AI thinking resumed with {resumeThinkingTime:F1}s delay");
			}

			// Clear pause state
			wasThinkingWhenPaused = false;
			pausedTopDiscardCard = null;
			pausedThinkingTimeRemaining = 0f;

			OnAIResumed?.Invoke ();
			TakiLogger.LogAI ("AI resumed successfully");
		}

		/// <summary>
		/// Cancel all AI operations safely
		/// </summary>
		public void CancelAllAIOperations () {
			TakiLogger.LogAI ("Cancelling all AI operations");

			// Cancel all invoke calls
			CancelInvoke ();

			// Clear current decision context
			currentTopDiscardCard = null;

			TakiLogger.LogAI ("All AI operations cancelled");
		}

		/// <summary>
		/// Start thinking process with pause awareness
		/// </summary>
		void StartThinkingProcess () {
			// Don't start thinking if paused
			if (isPaused) {
				TakiLogger.LogAI ("Thinking cancelled - AI is paused");
				return;
			}

			Invoke (nameof (ExecuteDecision), thinkingTime);
			TakiLogger.LogAI ($"AI thinking started with {thinkingTime:F1}s delay");
		}

		// Store current top card for ExecuteDecision
		private CardData currentTopDiscardCard;

		/// <summary>
		/// Execute the AI decision after thinking time - UPDATED with pause checking
		/// </summary>
		void ExecuteDecision () {
			// Double-check pause state
			if (isPaused) {
				TakiLogger.LogAI ("AI decision execution cancelled - AI is paused");
				return;
			}

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
		/// Check if AI can make decisions (not paused, game active)
		/// </summary>
		/// <returns>True if AI can act</returns>
		public bool CanMakeDecisions () {
			if (isPaused) {
				return false;
			}

			if (gameState != null && !gameState.CanComputerAct ()) {
				return false;
			}

			return true;
		}

		/// <summary>
		/// Get current AI state for debugging
		/// </summary>
		/// <returns>AI state description</returns>
		public string GetAIStateDescription () {
			if (isPaused) {
				return wasThinkingWhenPaused ?
					$"Paused (was thinking, {pausedThinkingTimeRemaining:F1}s remaining)" :
					"Paused (idle)";
			}

			if (IsInvokePending (nameof (ExecuteDecision))) {
				return "Thinking...";
			}

			return "Ready";
		}

		/// <summary>
		/// Check if a specific method is pending invocation - FIXED Unity implementation
		/// </summary>
		/// <param name="methodName">Name of the method to check</param>
		/// <returns>True if method is pending invocation</returns>
		bool IsInvokePending (string methodName) {
			// Unity doesn't have built-in IsInvokePending, so we'll track it manually
			return IsInvoking (methodName);
		}

		/// <summary>
		/// Debug method to log AI pause state
		/// </summary>
		[ContextMenu ("Log AI Pause State")]
		public void LogAIPauseState () {
			TakiLogger.LogDiagnostics ("=== AI PAUSE STATE DEBUG ===");
			TakiLogger.LogDiagnostics ($"AI Paused: {isPaused}");
			TakiLogger.LogDiagnostics ($"Was Thinking When Paused: {wasThinkingWhenPaused}");
			TakiLogger.LogDiagnostics ($"Paused Top Card: {pausedTopDiscardCard?.GetDisplayText () ?? "NULL"}");
			TakiLogger.LogDiagnostics ($"Thinking Time Remaining: {pausedThinkingTimeRemaining:F1}s");
			TakiLogger.LogDiagnostics ($"Current State: {GetAIStateDescription ()}");
			TakiLogger.LogDiagnostics ($"Can Make Decisions: {CanMakeDecisions ()}");
		}

		/// <summary>
		/// Test AI pause functionality
		/// </summary>
		[ContextMenu ("Test AI Pause")]
		public void TestAIPause () {
			if (isPaused) {
				TakiLogger.LogDiagnostics ("Testing AI resume");
				ResumeAI ();
			} else {
				TakiLogger.LogDiagnostics ("Testing AI pause");
				PauseAI ();
			}
		}

		/// <summary>
		/// Force complete reset of AI state - bypasses all pause logic
		/// Use this when AI gets stuck in paused state
		/// </summary>
		public void ForceCompleteReset () {
			TakiLogger.LogAI ("=== FORCE COMPLETE AI RESET ===");

			// Force reset all pause-related fields
			isPaused = false;
			wasThinkingWhenPaused = false;
			pausedTopDiscardCard = null;
			pausedThinkingTimeRemaining = 0f;

			// Cancel all operations
			CancelAllAIOperations ();

			// Clear decision context
			currentTopDiscardCard = null;

			// Clear hand
			computerHand.Clear ();

			// Fire resume events (in case other systems are listening)
			OnAIResumed?.Invoke ();

			TakiLogger.LogAI ("AI force reset complete - all state cleared");
		}

		/// <summary>
		/// Check if AI is stuck and needs force reset
		/// </summary>
		public bool IsAIStuckInPauseState () {
			// AI is stuck if it's paused but game state says it should be active
			if (!isPaused) return false;

			if (gameState == null) return true; // Can't determine, assume stuck

			// If game is active but AI is paused, AI is stuck
			bool gameIsActive = gameState.gameStatus == GameStatus.Active;
			bool shouldBeAbleToAct = gameState.CanComputerAct ();

			return gameIsActive && shouldBeAbleToAct && isPaused;
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
		/// PHASE 7: Simplified AI decision making with 70% special card preference
		/// </summary>
		/// <param name="validCards">List of cards that can be played</param>
		/// <returns>Selected card to play</returns>
		CardData SelectBestCard (List<CardData> validCards) {
			if (validCards.Count == 0) return null;

			TakiLogger.LogAI ($"=== AI SELECTING BEST CARD from {validCards.Count} options (CHAIN AWARE) ===");

			// CHAIN AWARENESS: During PlusTwo chain, only PlusTwo cards should be in validCards
			if (gameState != null && gameState.IsPlusTwoChainActive) {
				var plusTwoCards = validCards.Where (c => c.cardType == CardType.PlusTwo).ToList ();
				if (plusTwoCards.Count > 0) {
					CardData selectedPlusTwo = plusTwoCards [Random.Range (0, plusTwoCards.Count)];
					TakiLogger.LogAI ($"AI selected PlusTwo for chain: {selectedPlusTwo.GetDisplayText ()}");
					return selectedPlusTwo;
				} else {
					TakiLogger.LogWarning ("AI in PlusTwo chain but no PlusTwo cards in valid list!", TakiLogger.LogCategory.AI);
					return null; // This should trigger a draw instead
				}
			}

			// Separate special cards and number cards
			List<CardData> specialCards = validCards.Where (c => c.IsSpecialCard).ToList ();
			List<CardData> numberCards = validCards.Where (c => !c.IsSpecialCard).ToList ();

			TakiLogger.LogAI ($"Options: {specialCards.Count} special cards, {numberCards.Count} number cards");

			// PHASE 7: Simplified special card preference logic
			bool useSpecialCard = ShouldAIUseSpecialCard (specialCards, numberCards);

			CardData selectedCard;

			if (useSpecialCard && specialCards.Count > 0) {
				// SIMPLIFIED: Random selection from ALL special cards
				selectedCard = SelectFromSpecialCards (specialCards);
				TakiLogger.LogAI ($"AI chose special card: {selectedCard.GetDisplayText ()}", TakiLogger.LogLevel.Debug);
			} else if (numberCards.Count > 0) {
				// Select from number cards
				selectedCard = SelectFromNumberCards (numberCards);
				TakiLogger.LogAI ($"AI chose number card: {selectedCard.GetDisplayText ()}", TakiLogger.LogLevel.Debug);
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
		/// PHASE 7: SIMPLIFIED - 70% chance to use special cards if available
		/// </summary>
		/// <param name="specialCards">Available special cards</param>
		/// <param name="numberCards">Available number cards</param>
		/// <returns>True if AI should use special cards</returns>
		bool ShouldAIUseSpecialCard (List<CardData> specialCards, List<CardData> numberCards) {
			// If no special cards available, use number cards
			if (specialCards.Count == 0) {
				TakiLogger.LogAI ("No special cards available - using number cards");
				return false;
			}

			// SIMPLIFIED LOGIC: 70% chance to use special cards when available
			float specialCardChance = 0.7f; // 70% chance
			bool useSpecial = Random.value < specialCardChance;

			TakiLogger.LogAI ($"Special card decision: {specialCards.Count} available, {specialCardChance * 100}% chance, Use={useSpecial}");

			return useSpecial;
		}

		/// <summary>
		/// PHASE 7: SIMPLIFIED - Completely random selection from special cards
		/// </summary>
		/// <param name="specialCards">Available special cards</param>
		/// <returns>Selected special card</returns>
		CardData SelectFromSpecialCards (List<CardData> specialCards) {
			if (specialCards.Count == 0) return null;

			TakiLogger.LogAI ($"=== AI RANDOMLY SELECTING from {specialCards.Count} special cards ===");

			// SIMPLIFIED: Completely random selection from ALL special cards
			CardData selectedCard = specialCards [Random.Range (0, specialCards.Count)];

			TakiLogger.LogAI ($"AI randomly selected: {selectedCard.GetDisplayText ()} ({selectedCard.cardType})");

			return selectedCard;
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
		/// PHASE 7: Enhanced AI card selected handler with special card handling
		/// </summary>
		/// <param name="card">Card selected by AI</param>
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

			// Notify game manager about card selection
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
		/// PHASE 7: Enhanced AI color selection for ChangeColor cards
		/// </summary>
		/// <returns>Selected color</returns>
		public CardColor SelectColor () {
			TakiLogger.LogAI ("=== AI SELECTING COLOR (ENHANCED) ===");

			// Strategy: Select color that appears most in hand (excluding wild cards)
			var colorCounts = new Dictionary<CardColor, int> ();

			// Count cards by color in AI hand
			foreach (CardData card in computerHand) {
				if (card != null && card.color != CardColor.Wild) {
					if (colorCounts.ContainsKey (card.color)) {
						colorCounts [card.color]++;
					} else {
						colorCounts [card.color] = 1;
					}
				}
			}

			TakiLogger.LogAI ($"AI color analysis: {colorCounts.Count} colors found in hand");
			foreach (var kvp in colorCounts) {
				TakiLogger.LogAI ($"  {kvp.Key}: {kvp.Value} cards", TakiLogger.LogLevel.Verbose);
			}

			// Select most common color, or random if tie/no cards
			if (colorCounts.Count > 0) {
				var bestColor = colorCounts.OrderByDescending (kvp => kvp.Value).First ().Key;
				int bestCount = colorCounts [bestColor];

				TakiLogger.LogAI ($"AI selected color: {bestColor} (appears {bestCount} times in hand)", TakiLogger.LogLevel.Info);

				// Notify external systems about color selection
				OnAIColorSelected?.Invoke (bestColor);
				return bestColor;
			} else {
				// Random color selection if no colored cards in hand
				CardColor [] colors = { CardColor.Red, CardColor.Blue, CardColor.Green, CardColor.Yellow };
				CardColor randomColor = colors [Random.Range (0, colors.Length)];

				TakiLogger.LogAI ($"AI selected random color: {randomColor} (no colored cards in hand)", TakiLogger.LogLevel.Info);

				// Notify external systems about color selection
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
		/// Updated Clear Hand method with comprehensive reset
		/// </summary>
		public void ClearHand () {
			computerHand.Clear ();

			// COMPREHENSIVE RESET: Always reset pause state during clear hand
			TakiLogger.LogAI ("Clearing hand and resetting all AI state (including chain state)");

			// Force reset pause state regardless of current state
			isPaused = false;
			wasThinkingWhenPaused = false;
			pausedTopDiscardCard = null;
			pausedThinkingTimeRemaining = 0f;

			// RESET CHAIN STATE
			currentChainPlusTwo = null;

			// Cancel any pending operations
			CancelAllAIOperations ();

			// Clear any stored decision context
			currentTopDiscardCard = null;

			// Fire resume events in case other systems need to know
			if (isPaused) { // This should be false now, but just in case
				OnAIResumed?.Invoke ();
			}

			TakiLogger.LogAI ("AI hand cleared and state reset");
		}

		/// <summary>
		/// Reset AI to fresh state for new game
		/// </summary>
		public void ResetForNewGame () {
			TakiLogger.LogAI ("=== RESETTING AI FOR NEW GAME (INCLUDING CHAIN STATE) ===");

			// Clear hand
			computerHand.Clear ();

			// Reset all pause-related state
			isPaused = false;
			wasThinkingWhenPaused = false;
			pausedTopDiscardCard = null;
			pausedThinkingTimeRemaining = 0f;

			// RESET CHAIN STATE
			currentChainPlusTwo = null;

			// Cancel all operations
			CancelAllAIOperations ();

			// Clear any stored decision context
			currentTopDiscardCard = null;

			TakiLogger.LogAI ("AI completely reset for new game");
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

		// AI pause state properties 
		public bool IsAIPaused => isPaused;
		public bool WasThinkingWhenPaused => wasThinkingWhenPaused;
		public float ThinkingTimeRemaining => pausedThinkingTimeRemaining;
		public CardData PausedTopCard => pausedTopDiscardCard;
		public string AIState => GetAIStateDescription ();
	}
}