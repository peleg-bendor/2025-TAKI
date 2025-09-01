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
			Debug.Log ("=== TAKI GAME DIAGNOSTICS START ===");

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

			Debug.Log ("=== TAKI GAME DIAGNOSTICS END ===");
		}

		void CheckComponentReferences () {
			Debug.Log ("--- COMPONENT REFERENCES CHECK ---");

			if (gameManager == null) {
				gameManager = FindObjectOfType<GameManager> ();
				Debug.LogError ("GameManager was null - attempting to find");
			} else {
				Debug.Log ("OK: GameManager reference OK");
			}

			if (gameManager != null) {
				Debug.Log ("OK: GameManager.deckManager: " + (gameManager.deckManager != null ? "OK" : "NULL"));
				Debug.Log ("OK: GameManager.gameState: " + (gameManager.gameState != null ? "OK" : "NULL"));
				Debug.Log ("OK: GameManager.turnManager: " + (gameManager.turnManager != null ? "OK" : "NULL"));
				Debug.Log ("OK: GameManager.computerAI: " + (gameManager.computerAI != null ? "OK" : "NULL"));
				Debug.Log ("OK: GameManager.playerHandManager: " + (gameManager.playerHandManager != null ? "OK" : "NULL"));
				Debug.Log ("OK: GameManager.computerHandManager: " + (gameManager.computerHandManager != null ? "OK" : "NULL"));
				Debug.Log ("OK: GameManager.gameplayUI: " + (gameManager.gameplayUI != null ? "OK" : "NULL"));
			}
		}

		void CheckDeckState () {
			Debug.Log ("--- DECK STATE CHECK ---");

			if (deckManager == null) deckManager = gameManager?.deckManager;

			if (deckManager != null) {
				Debug.Log ("OK: Draw Pile Count: " + deckManager.DrawPileCount);
				Debug.Log ("OK: Discard Pile Count: " + deckManager.DiscardPileCount);

				CardData topCard = deckManager.GetTopDiscardCard ();
				if (topCard != null) {
					Debug.Log ("OK: Top Discard Card: " + topCard.GetDisplayText ());
				} else {
					Debug.LogError ("ERROR: Top Discard Card is NULL - CRITICAL ISSUE");
				}

				Debug.Log ("OK: Can Draw Cards: " + deckManager.CanDrawCards);
				Debug.Log ("OK: Has Valid Deck: " + deckManager.HasValidDeck);
			} else {
				Debug.LogError ("ERROR: DeckManager is NULL - CRITICAL ISSUE");
			}
		}

		void CheckGameState () {
			Debug.Log ("--- GAME STATE CHECK ---");

			if (gameState == null) gameState = gameManager?.gameState;

			if (gameState != null) {
				Debug.Log ("OK: Turn State: " + gameState.turnState);
				Debug.Log ("OK: Interaction State: " + gameState.interactionState);
				Debug.Log ("OK: Game Status: " + gameState.gameStatus);
				Debug.Log ("OK: Active Color: " + gameState.activeColor);
				Debug.Log ("OK: Can Player Act: " + gameState.CanPlayerAct ());
				Debug.Log ("OK: Can Computer Act: " + gameState.CanComputerAct ());
			} else {
				Debug.LogError ("ERROR: GameState is NULL - CRITICAL ISSUE");
			}
		}

		void CheckTurnManagement () {
			Debug.Log ("--- TURN MANAGEMENT CHECK ---");

			if (turnManager == null) turnManager = gameManager?.turnManager;

			if (turnManager != null) {
				Debug.Log ("OK: Current Player: " + turnManager.CurrentPlayer);
				Debug.Log ("OK: Is Human Turn: " + turnManager.IsHumanTurn);
				Debug.Log ("OK: Is Computer Turn: " + turnManager.IsComputerTurn);
				Debug.Log ("OK: Turns Active: " + turnManager.AreTurnsActive ());
				Debug.Log ("OK: Computer Turn Pending: " + turnManager.IsComputerTurnPending);

				if (turnManager.TurnTimeRemaining > 0) {
					Debug.Log ("OK: Turn Time Remaining: " + turnManager.TurnTimeRemaining.ToString ("F1") + "s");
				}
			} else {
				Debug.LogError ("ERROR: TurnManager is NULL - CRITICAL ISSUE");
			}
		}

		void CheckPlayerHand () {
			Debug.Log ("--- PLAYER HAND CHECK ---");

			if (gameManager != null) {
				Debug.Log ("OK: Player Hand Size: " + gameManager.PlayerHandSize);
				Debug.Log ("OK: Computer Hand Size: " + gameManager.ComputerHandSize);

				List<CardData> playerHand = gameManager.GetPlayerHand ();
				if (playerHand.Count > 0) {
					Debug.Log ("OK: First card in player hand: " + playerHand [0].GetDisplayText ());

					// Test if any cards are playable
					CardData topCard = deckManager?.GetTopDiscardCard ();
					if (topCard != null) {
						int playableCount = 0;
						foreach (CardData card in playerHand) {
							if (gameState?.IsValidMove (card, topCard) == true) {
								playableCount++;
							}
						}
						Debug.Log ("OK: Playable cards in hand: " + playableCount);
					}
				} else {
					Debug.LogWarning ("WARNING: Player hand is empty");
				}

				// Check selected card
				CardData selectedCard = gameManager.playerHandManager?.GetSelectedCard ();
				if (selectedCard != null) {
					Debug.Log ("OK: Selected card: " + selectedCard.GetDisplayText ());
				} else {
					Debug.Log ("OK: No card currently selected");
				}
			}
		}

		void CheckAIState () {
			Debug.Log ("--- AI STATE CHECK ---");

			if (computerAI == null) computerAI = gameManager?.computerAI;

			if (computerAI != null) {
				Debug.Log ("OK: AI Hand Size: " + computerAI.HandSize);
				Debug.Log ("OK: AI Has Cards: " + computerAI.HasCards);

				if (computerAI.Hand.Count > 0) {
					Debug.Log ("OK: First card in AI hand: " + computerAI.Hand [0].GetDisplayText ());
				}
			} else {
				Debug.LogError ("ERROR: ComputerAI is NULL - CRITICAL ISSUE");
			}
		}

		/// <summary>
		/// Test rule validation with specific cards
		/// </summary>
		void TestRuleValidation () {
			Debug.Log ("=== RULE VALIDATION TEST ===");

			CardData topCard = deckManager?.GetTopDiscardCard ();
			if (topCard == null) {
				Debug.LogError ("Cannot test rules - no top discard card");
				return;
			}

			Debug.Log ("Testing against top card: " + topCard.GetDisplayText ());

			List<CardData> playerHand = gameManager?.GetPlayerHand ();
			if (playerHand != null) {
				foreach (CardData card in playerHand) {
					bool canPlay = gameState?.IsValidMove (card, topCard) ?? false;
					Debug.Log ("Card: " + card.GetDisplayText () + " - Can Play: " + canPlay);

					// Also test the underlying rule
					bool cardRuleResult = card.CanPlayOn (topCard, gameState.activeColor);
					if (canPlay != cardRuleResult) {
						Debug.LogError ("RULE MISMATCH for " + card.GetDisplayText () + ": GameState says " + canPlay + ", CardData says " + cardRuleResult);
					}
				}
			}
		}

		/// <summary>
		/// Test turn sequence manually
		/// </summary>
		void TestTurnSequence () {
			Debug.Log ("=== TURN SEQUENCE TEST ===");

			if (turnManager != null && gameState != null) {
				Debug.Log ("Current turn: " + turnManager.CurrentPlayer);

				if (turnManager.IsHumanTurn) {
					Debug.Log ("Ending human turn manually...");
					turnManager.EndTurn ();
				} else if (turnManager.IsComputerTurn) {
					Debug.Log ("Computer turn - triggering AI decision...");
					CardData topCard = deckManager?.GetTopDiscardCard ();
					computerAI?.MakeDecision (topCard);
				} else {
					Debug.Log ("Game in neutral state - initializing turns...");
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

			Debug.Log ("TakiGameDiagnostics ready. Press F1 for full diagnostics, F2 for rule validation, F3 for turn sequence test.");
		}
	}
}