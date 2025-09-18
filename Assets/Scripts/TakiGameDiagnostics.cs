using UnityEngine;
using System.Collections.Generic;

namespace TakiGame {
	/// <summary>
	/// Diagnostic script to identify exactly where TAKI game flow breaks
	/// Attach to GameManager GameObject and run diagnostics
	/// </summary>
	public class TakiGameDiagnostics : MonoBehaviour {

		[Header ("Components to Test")]
		public GameManager gameManager;
		public DeckManager deckManager;
		public GameStateManager gameState;
		public TurnManager turnManager;
		public BasicComputerAI computerAI;

		[Header ("Test Controls")]
		public KeyCode runDiagnosticsKey = KeyCode.F1;
		public KeyCode testRuleValidationKey = KeyCode.F2;
		public KeyCode testTurnSequenceKey = KeyCode.F3;

		void Update () {
			if (Input.GetKeyDown (runDiagnosticsKey)) {
				RunFullDiagnostics ();
			}

			if (Input.GetKeyDown (testRuleValidationKey)) {
				TestRuleValidation ();
			}

			if (Input.GetKeyDown (testTurnSequenceKey)) {
				TestTurnSequence ();
			}
		}

		/// <summary>
		/// Comprehensive diagnostic check - run this first
		/// </summary>
		public void RunFullDiagnostics () {
			TakiLogger.LogDebug ("=== TAKI GAME DIAGNOSTICS START ===", TakiLogger.LogCategory.Diagnostics);

			// Test 1: Component References
			CheckComponentReferences ();

			// Test 2: Deck State
			CheckDeckState ();

			// Test 3: Game State
			CheckGameState ();

			// Test 4: Turn Management
			CheckTurnManagement ();

			// Test 5: Player Hand
			CheckPlayerHand ();

			// Test 6: AI State
			CheckAIState ();

			TakiLogger.LogDebug ("=== TAKI GAME DIAGNOSTICS END ===", TakiLogger.LogCategory.Diagnostics);
		}

		void CheckComponentReferences () {
			TakiLogger.LogDebug ("--- COMPONENT REFERENCES CHECK ---", TakiLogger.LogCategory.Diagnostics);

			if (gameManager == null) {
				gameManager = FindObjectOfType<GameManager> ();
				TakiLogger.LogError ("GameManager was null - attempting to find", TakiLogger.LogCategory.Diagnostics);
			} else {
				TakiLogger.LogDebug ("GameManager reference OK", TakiLogger.LogCategory.Diagnostics);
			}

			if (gameManager != null) {
				TakiLogger.LogDebug ("GameManager.deckManager: " + (gameManager.deckManager != null ? "OK" : "NULL"), TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("GameManager.gameState: " + (gameManager.gameState != null ? "OK" : "NULL"), TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("GameManager.turnManager: " + (gameManager.turnManager != null ? "OK" : "NULL"), TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("GameManager.computerAI: " + (gameManager.computerAI != null ? "OK" : "NULL"), TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("GameManager.playerHandManager: " + (gameManager.playerHandManager != null ? "OK" : "NULL"), TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("GameManager.opponentHandManager: " + (gameManager.opponentHandManager != null ? "OK" : "NULL"), TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("GameManager.gameplayUI: " + (gameManager.gameplayUI != null ? "OK" : "NULL"), TakiLogger.LogCategory.Diagnostics);
			}
		}

		void CheckDeckState () {
			TakiLogger.LogDebug ("--- DECK STATE CHECK ---", TakiLogger.LogCategory.Diagnostics);

			if (deckManager == null) deckManager = gameManager?.deckManager;

			if (deckManager != null) {
				TakiLogger.LogDebug ("Draw Pile Count: " + deckManager.DrawPileCount, TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("Discard Pile Count: " + deckManager.DiscardPileCount, TakiLogger.LogCategory.Diagnostics);

				CardData topCard = deckManager.GetTopDiscardCard ();
				if (topCard != null) {
					TakiLogger.LogDebug ("Top Discard Card: " + topCard.GetDisplayText (), TakiLogger.LogCategory.Diagnostics);
				} else {
					TakiLogger.LogError ("Top Discard Card is NULL - CRITICAL ISSUE", TakiLogger.LogCategory.Diagnostics);
				}

				TakiLogger.LogDebug ("Can Draw Cards: " + deckManager.CanDrawCards, TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("Has Valid Deck: " + deckManager.HasValidDeck, TakiLogger.LogCategory.Diagnostics);
			} else {
				TakiLogger.LogError ("DeckManager is NULL - CRITICAL ISSUE", TakiLogger.LogCategory.Diagnostics);
			}
		}

		void CheckGameState () {
			TakiLogger.LogDebug ("--- GAME STATE CHECK (WITH TAKI SEQUENCES) ---", TakiLogger.LogCategory.Diagnostics);

			if (gameState == null) gameState = gameManager?.gameState;

			if (gameState != null) {
				TakiLogger.LogDebug ("Turn State: " + gameState.turnState, TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("Interaction State: " + gameState.interactionState, TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("Game Status: " + gameState.gameStatus, TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("Active Color: " + gameState.activeColor, TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("Can Player Act: " + gameState.CanPlayerAct (), TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("Can Computer Act: " + gameState.CanComputerAct (), TakiLogger.LogCategory.Diagnostics);

				// PHASE 8B: TAKI sequence diagnostics 
				TakiLogger.LogDebug ("TAKI Sequence Active: " + gameState.IsInTakiSequence, TakiLogger.LogCategory.Diagnostics);
				if (gameState.IsInTakiSequence) {
					TakiLogger.LogDebug ("Sequence Color: " + gameState.TakiSequenceColor, TakiLogger.LogCategory.Diagnostics);
					TakiLogger.LogDebug ("Sequence Cards: " + gameState.NumberOfSequenceCards, TakiLogger.LogCategory.Diagnostics);
					TakiLogger.LogDebug ("Sequence Initiator: " + gameState.TakiSequenceInitiator, TakiLogger.LogCategory.Diagnostics);
				}
			} else {
				TakiLogger.LogError ("GameState is NULL - CRITICAL ISSUE", TakiLogger.LogCategory.Diagnostics);
			}
		}

		void CheckTurnManagement () {
			TakiLogger.LogDebug ("--- TURN MANAGEMENT CHECK ---", TakiLogger.LogCategory.Diagnostics);

			if (turnManager == null) turnManager = gameManager?.turnManager;

			if (turnManager != null) {
				TakiLogger.LogDebug ("Current Player: " + turnManager.CurrentPlayer, TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("Is Human Turn: " + turnManager.IsHumanTurn, TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("Is Computer Turn: " + turnManager.IsComputerTurn, TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("Turns Active: " + turnManager.AreTurnsActive (), TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("Computer Turn Pending: " + turnManager.IsComputerTurnPending, TakiLogger.LogCategory.Diagnostics);

				if (turnManager.TurnTimeRemaining > 0) {
					TakiLogger.LogDebug ("Turn Time Remaining: " + turnManager.TurnTimeRemaining.ToString ("F1") + "s", TakiLogger.LogCategory.Diagnostics);
				}
			} else {
				TakiLogger.LogError ("TurnManager is NULL - CRITICAL ISSUE", TakiLogger.LogCategory.Diagnostics);
			}
		}

		void CheckPlayerHand () {
			TakiLogger.LogDebug ("--- PLAYER HAND CHECK ---", TakiLogger.LogCategory.Diagnostics);

			if (gameManager != null) {
				TakiLogger.LogDebug ("Player Hand Size: " + gameManager.PlayerHandSize, TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("Computer Hand Size: " + gameManager.ComputerHandSize, TakiLogger.LogCategory.Diagnostics);

				List<CardData> playerHand = gameManager.GetPlayerHand ();
				if (playerHand.Count > 0) {
					TakiLogger.LogDebug ("First card in player hand: " + playerHand [0].GetDisplayText (), TakiLogger.LogCategory.Diagnostics);

					// Test if any cards are playable
					CardData topCard = deckManager?.GetTopDiscardCard ();
					if (topCard != null) {
						int playableCount = 0;
						foreach (CardData card in playerHand) {
							if (gameState?.IsValidMove (card, topCard) == true) {
								playableCount++;
							}
						}
						TakiLogger.LogDebug ("Playable cards in hand: " + playableCount, TakiLogger.LogCategory.Diagnostics);
					}
				} else {
					TakiLogger.LogWarning ("Player hand is empty", TakiLogger.LogCategory.Diagnostics);
				}

				// Check selected card
				CardData selectedCard = gameManager.playerHandManager?.GetSelectedCard ();
				if (selectedCard != null) {
					TakiLogger.LogDebug ("Selected card: " + selectedCard.GetDisplayText (), TakiLogger.LogCategory.Diagnostics);
				} else {
					TakiLogger.LogDebug ("No card currently selected", TakiLogger.LogCategory.Diagnostics);
				}
			}
		}

		void CheckAIState () {
			TakiLogger.LogDebug ("--- AI STATE CHECK ---", TakiLogger.LogCategory.Diagnostics);

			if (computerAI == null) computerAI = gameManager?.computerAI;

			if (computerAI != null) {
				TakiLogger.LogDebug ("AI Hand Size: " + computerAI.HandSize, TakiLogger.LogCategory.Diagnostics);
				TakiLogger.LogDebug ("AI Has Cards: " + computerAI.HasCards, TakiLogger.LogCategory.Diagnostics);

				if (computerAI.Hand.Count > 0) {
					TakiLogger.LogDebug ("First card in AI hand: " + computerAI.Hand [0].GetDisplayText (), TakiLogger.LogCategory.Diagnostics);
				}
			} else {
				TakiLogger.LogError ("ComputerAI is NULL - CRITICAL ISSUE", TakiLogger.LogCategory.Diagnostics);
			}
		}

		/// <summary>
		/// Test rule validation with specific cards
		/// </summary>
		void TestRuleValidation () {
			TakiLogger.LogDebug ("=== RULE VALIDATION TEST ===", TakiLogger.LogCategory.Diagnostics);

			CardData topCard = deckManager?.GetTopDiscardCard ();
			if (topCard == null) {
				TakiLogger.LogError ("Cannot test rules - no top discard card", TakiLogger.LogCategory.Diagnostics);
				return;
			}

			TakiLogger.LogDebug ("Testing against top card: " + topCard.GetDisplayText (), TakiLogger.LogCategory.Diagnostics);

			List<CardData> playerHand = gameManager?.GetPlayerHand ();
			if (playerHand != null) {
				foreach (CardData card in playerHand) {
					bool canPlay = gameState?.IsValidMove (card, topCard) ?? false;
					TakiLogger.LogDebug ("Card: " + card.GetDisplayText () + " - Can Play: " + canPlay, TakiLogger.LogCategory.Diagnostics);

					// Also test the underlying rule
					bool cardRuleResult = card.CanPlayOn (topCard, gameState.activeColor);
					if (canPlay != cardRuleResult) {
						TakiLogger.LogError ("RULE MISMATCH for " + card.GetDisplayText () + ": GameState says " + canPlay + ", CardData says " + cardRuleResult, TakiLogger.LogCategory.Diagnostics);
					}
				}
			}
		}

		/// <summary>
		/// Test turn sequence manually
		/// </summary>
		void TestTurnSequence () {
			TakiLogger.LogDebug ("=== TURN SEQUENCE TEST ===", TakiLogger.LogCategory.Diagnostics);

			if (turnManager != null && gameState != null) {
				TakiLogger.LogDebug ("Current turn: " + turnManager.CurrentPlayer, TakiLogger.LogCategory.Diagnostics);

				if (turnManager.IsHumanTurn) {
					TakiLogger.LogDebug ("Ending human turn manually...", TakiLogger.LogCategory.Diagnostics);
					turnManager.EndTurn ();
				} else if (turnManager.IsComputerTurn) {
					TakiLogger.LogDebug ("Computer turn - triggering AI decision...", TakiLogger.LogCategory.Diagnostics);
					CardData topCard = deckManager?.GetTopDiscardCard ();
					computerAI?.MakeDecision (topCard);
				} else {
					TakiLogger.LogDebug ("Game in neutral state - initializing turns...", TakiLogger.LogCategory.Diagnostics);
					turnManager.InitializeTurns (PlayerType.Human);
				}
			}
		}

		/// <summary>
		/// Manual setup for testing 
		/// </summary>
		void Start () {
			// Auto-find components if not assigned
			if (gameManager == null) gameManager = FindObjectOfType<GameManager> ();
			if (deckManager == null) deckManager = FindObjectOfType<DeckManager> ();
			if (gameState == null) gameState = FindObjectOfType<GameStateManager> ();
			if (turnManager == null) turnManager = FindObjectOfType<TurnManager> ();
			if (computerAI == null) computerAI = FindObjectOfType<BasicComputerAI> ();

			TakiLogger.LogDebug ("TakiGameDiagnostics ready. Press F1 for full diagnostics, F2 for rule validation, F3 for turn sequence test.", TakiLogger.LogCategory.Diagnostics);
		}
	}
}