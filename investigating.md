# investigating!

---

## region- `Architecture Management`:
Done!

## region- `Unity Lifecycle`:
Done!

## region- `System Initialization`:
Done!

## region- `Game Flow Control`:
RETURN TO LATER ON

---

## region- `Turn Flow Management`:

### `ResetTurnFlowState`

#### Original code:
```csharp
		/// <summary>
		/// Reset turn flow control state, includes special card state
		/// </summary>
		void ResetTurnFlowState () {
			hasPlayerTakenAction = false;
			canPlayerDraw = true;
			canPlayerPlay = true;
			canPlayerEndTurn = false;

			// Reset special card state
			ResetSpecialCardState ();

			TakiLogger.LogTurnFlow ("TURN FLOW STATE RESET (includes special card state)");
		}
```

#### My motes:
Looks good

### `StartPlayerTurnFlow`

#### Original code:

```csharp
		/// <summary>
		/// ENHANCED: Start player turn with strict flow control
		/// </summary>
		void StartPlayerTurnFlow () {
			TakiLogger.LogTurnFlow ("STARTING PLAYER TURN WITH PLSTWO CHAIN AWARENESS");

			// CRITICAL: Check for active PlusTwo chain FIRST
			if (gameState.IsPlusTwoChainActive) {
				TakiLogger.LogTurnFlow ("=== PLSTWO CHAIN ACTIVE - SPECIAL TURN LOGIC ===");

				int drawCount = gameState.ChainDrawCount;
				int chainLength = gameState.NumberOfChainedCards;

				TakiLogger.LogTurnFlow ($"Chain status: {chainLength} cards, player must draw {drawCount} or play PlusTwo");

				// Check if player has PlusTwo cards
				bool hasPlusTwo = playerHand.Any (card => card.cardType == CardType.PlusTwo);

				if (hasPlusTwo) {
					// Player can either play PlusTwo to continue chain or draw to break it
					hasPlayerTakenAction = false;
					canPlayerDraw = true;     // Can draw to break chain
					canPlayerPlay = true;     // Can play PlusTwo to continue chain  
					canPlayerEndTurn = false; // Must take action first

					GetActiveUI ()?.UpdateStrictButtonStates (true, true, false);
					gameplayUI?.ShowPlayerMessage ($"Chain active! Play PlusTwo to continue or draw {drawCount} cards to break");

					TakiLogger.LogTurnFlow ($"Player has PlusTwo options: continue chain or draw {drawCount} cards");
				} else {
					// Player has no PlusTwo cards - must draw to break chain
					hasPlayerTakenAction = false;
					canPlayerDraw = true;     // Must draw to break chain
					canPlayerPlay = false;    // No valid plays during chain without PlusTwo
					canPlayerEndTurn = false; // Must take action first

					GetActiveUI ()?.UpdateStrictButtonStates (false, true, false);
					gameplayUI?.ShowPlayerMessage ($"No PlusTwo cards - you must draw {drawCount} cards to break chain");

					TakiLogger.LogTurnFlow ($"Player must break chain by drawing {drawCount} cards (no PlusTwo available)");
				}

				// Update visual card states - only PlusTwo cards should show as playable
				RefreshPlayerHandStates ();
				return;
			}

			// ... KEEP ALL EXISTING NORMAL TURN FLOW LOGIC AFTER THIS POINT ...
			TakiLogger.LogTurnFlow ("Normal turn flow - no active chain");

			// Reset turn flow state
			hasPlayerTakenAction = false;
			canPlayerDraw = true;
			canPlayerPlay = true;
			canPlayerEndTurn = false;

			// Check if player has valid cards (existing logic)
			int validCardCount = CountPlayableCards ();

			if (validCardCount == 0) {
				// Player has NO valid cards - must draw
				TakiLogger.LogTurnFlow ("Player has no valid cards, must draw a card");
				GetActiveUI ()?.ShowPlayerMessage ("No valid cards - you must DRAW a card!");
				GetActiveUI ()?.UpdateStrictButtonStates (false, true, false); // No Play, Yes Draw, No EndTurn
			} else {
				// Player has valid cards - can play or draw
				TakiLogger.LogTurnFlow ($"Player has {validCardCount} valid cards, may PLAY or DRAW a card");
				gameplayUI?.ShowPlayerMessage ($"You have {validCardCount} valid moves - PLAY a card or DRAW");
				GetActiveUI ()?.UpdateStrictButtonStates (true, true, false); // Yes Play, Yes Draw, No EndTurn
			}

			// Update visual card states
			RefreshPlayerHandStates ();
		}
```

#### My proposal:

```csharp
		/// <summary>
		/// Start player turn with strict flow control
		/// </summary>
		void StartPlayerTurnFlow () {
			TakiLogger.LogTurnFlow ("Starting Player Turn", TakiLogger.LogLevel.Debug);

			// Check for active PlusTwo chain FIRST
			if (gameState.IsPlusTwoChainActive) {
				TakiLogger.LogTurnFlow ("=== PLSTWO CHAIN ACTIVE - SPECIAL TURN LOGIC ===", TakiLogger.LogLevel.Debug);

				int drawCount = gameState.ChainDrawCount;
				int chainLength = gameState.NumberOfChainedCards;

				TakiLogger.LogTurnFlow ($"Chain status: {chainLength} cards, player must draw {drawCount} or play PlusTwo", TakiLogger.LogLevel.Debug);

				// Check if player has PlusTwo cards
				bool hasPlusTwo = playerHand.Any (card => card.cardType == CardType.PlusTwo);

				if (hasPlusTwo) {
					// Player can either play PlusTwo to continue chain or draw to break it
					hasPlayerTakenAction = false;
					canPlayerDraw = true;     // Can draw to break chain
					canPlayerPlay = true;     // Can play PlusTwo to continue chain
					canPlayerEndTurn = false; // Must take action first

					GetActiveUI ()?.UpdateStrictButtonStates (true, true, false);
					GetActiveUI ()?.ShowPlayerMessage ($"Chain active! Play PlusTwo to continue or draw {drawCount} cards to break");

					TakiLogger.LogTurnFlow ($"Player has PlusTwo options: continue chain or draw {drawCount} cards", TakiLogger.LogLevel.Debug);
				} else {
					// Player has no PlusTwo cards - must draw to break chain
					hasPlayerTakenAction = false;
					canPlayerDraw = true;     // Must draw to break chain
					canPlayerPlay = false;    // No valid plays during chain without PlusTwo
					canPlayerEndTurn = false; // Must take action first

					GetActiveUI ()?.UpdateStrictButtonStates (false, true, false);
					GetActiveUI ()?.ShowPlayerMessage ($"No PlusTwo cards - you must draw {drawCount} cards to break chain");

					TakiLogger.LogTurnFlow ($"Player must break chain by drawing {drawCount} cards (no PlusTwo available)", TakiLogger.LogLevel.Debug);
				}

				// Update visual card states - only PlusTwo cards should show as playable
				RefreshPlayerHandStates ();
				return;
			}

			// ... KEEP ALL EXISTING NORMAL TURN FLOW LOGIC AFTER THIS POINT ...
			TakiLogger.LogTurnFlow ("Normal turn flow - no active chain", TakiLogger.LogLevel.Debug);

			// Reset turn flow state
			hasPlayerTakenAction = false;
			canPlayerDraw = true;
			canPlayerPlay = true;
			canPlayerEndTurn = false;

			// Check if player has valid cards (existing logic)
			int validCardCount = CountPlayableCards ();

			if (validCardCount == 0) {
				// Player has NO valid cards - must draw
				TakiLogger.LogTurnFlow ("Player has no valid cards, must draw a card", TakiLogger.LogLevel.Debug);
				GetActiveUI ()?.ShowPlayerMessage ("No valid cards - you must DRAW a card!");
				GetActiveUI ()?.UpdateStrictButtonStates (false, true, false); // No Play, Yes Draw, No EndTurn
			} else {
				// Player has valid cards - can play or draw
				TakiLogger.LogTurnFlow ($"Player has {validCardCount} valid cards, may PLAY or DRAW a card", TakiLogger.LogLevel.Debug);
				GetActiveUI ()?.ShowPlayerMessage ($"You have {validCardCount} valid moves - PLAY a card or DRAW");
				GetActiveUI ()?.UpdateStrictButtonStates (true, true, false); // Yes Play, Yes Draw, No EndTurn
			}

			// Update visual card states
			RefreshPlayerHandStates ();
		}
```

### `HandlePostCardPlayTurnFlow`

#### Original code:

```csharp
		/// <summary> 
		/// PHASE 8B: FIXED HandlePostCardPlayTurnFlow with proper TAKI sequence handling
		/// CRITICAL FIX: Proper sequence continuation and End Sequence button management
		/// </summary>
		/// <param name="card">Card that was just played</param>
		void HandlePostCardPlayTurnFlow (CardData card) {
			TakiLogger.LogTurnFlow ($"=== HANDLING POST-CARD-PLAY TURN FLOW for {card.GetDisplayText ()} ===");

			// PHASE 8B: Check if TAKI sequence was started and we should continue turn
			if (gameState.IsInTakiSequence && (card.cardType == CardType.Taki || card.cardType == CardType.SuperTaki) && !isCurrentCardLastInSequence) {
				TakiLogger.LogTurnFlow ($"{card.cardType} card started sequence - turn continues for sequence play");

				// For TAKI/SuperTAKI cards that start sequences, keep turn active
				hasPlayerTakenAction = false;  // Reset to allow sequence cards
				canPlayerPlay = true;          // Re-enable play for sequence
				canPlayerDraw = false;         // Cannot draw during sequence
				canPlayerEndTurn = false;      // Cannot end turn during sequence (must use End Sequence button)

				GetActiveUI ()?.UpdateStrictButtonStates (true, false, false);
				gameplayUI?.EnableEndTakiSequenceButton (true);

				if (card.cardType == CardType.Taki) {
					gameplayUI?.ShowPlayerMessage ($"TAKI Sequence active - play {card.color} cards or End Sequence!");
				} else {
					gameplayUI?.ShowPlayerMessage ($"SuperTAKI Sequence active - play {gameState.TakiSequenceColor} cards or End Sequence!");
				}

				TakiLogger.LogTurnFlow ($"{card.cardType} sequence turn flow: PLAY enabled, DRAW/END TURN disabled, END SEQUENCE enabled");
				return;
			}

			// PHASE 8B: Check if card was played during existing sequence (not last card)
			if (gameState.IsInTakiSequence && !isCurrentCardLastInSequence) {
				TakiLogger.LogTurnFlow ("Card played in existing sequence - turn continues");

				// Continue sequence - same button states as sequence start 
				hasPlayerTakenAction = false;  // Allow more sequence cards
				canPlayerPlay = true;          // Keep play enabled
				canPlayerDraw = false;         // Still no draw during sequence
				canPlayerEndTurn = false;      // Still no end turn during sequence

				GetActiveUI ()?.UpdateStrictButtonStates (true, false, false);
				GetActiveUI ()?.ShowPlayerMessage ("Sequence continues - play more cards or End Sequence!");

				TakiLogger.LogTurnFlow ("Sequence continuation: PLAY enabled, DRAW/END TURN disabled");
				return;
			}

			// PHASE 8B: Check if sequence just ended (last card processed)
			if (isCurrentCardLastInSequence) {
				TakiLogger.LogTurnFlow ("Last card in sequence processed - ending turn normally");

				// Reset flag
				isCurrentCardLastInSequence = false;

				// Force end turn after sequence completion
				hasPlayerTakenAction = true;
				canPlayerPlay = false;
				canPlayerDraw = false;
				canPlayerEndTurn = true;

				gameplayUI?.ForceEnableEndTurn ();
				gameplayUI?.ShowPlayerMessage ("Sequence completed - you must END TURN!");

				TakiLogger.LogTurnFlow ("Sequence completion: Must END TURN");
				return;
			}

			// ENHANCED: Plus card handling with better messaging
			if (card.cardType == CardType.Plus && !gameState.IsInTakiSequence) {
				TakiLogger.LogTurnFlow ("PLUS CARD PLAYED - Player gets additional action");

				// Set special card state
				isWaitingForAdditionalAction = true;
				activeSpecialCardEffect = CardType.Plus;

				// Reset action state to allow additional action
				hasPlayerTakenAction = false;  // Reset to allow additional action
				canPlayerPlay = true;          // Re-enable play
				canPlayerDraw = true;          // Re-enable draw  
				canPlayerEndTurn = false;      // Keep end turn disabled until additional action

				// ENHANCED: Update UI with better messaging
				GetActiveUI ()?.UpdateStrictButtonStates (true, true, false);
				if (gameplayUI != null) {
					GetActiveUI ()?.ShowSpecialCardEffect (CardType.Plus, PlayerType.Human, "You get one more action");
				}

				TakiLogger.LogTurnFlow ("Plus card turn flow: Additional action enabled, END TURN disabled");
				return;
			}

			// FIXED: ChangeColor card handling for HUMAN players - ADD UI LOGIC HERE
			if (card.cardType == CardType.ChangeColor && !gameState.IsInTakiSequence) {
				TakiLogger.LogTurnFlow ("CHANGE COLOR CARD PLAYED BY HUMAN - Player must select color");

				if (gameState == null || gameplayUI == null) {
					TakiLogger.LogError ("Cannot handle ChangeColor: Missing components!", TakiLogger.LogCategory.Rules);
					return;
				}

				// Set interaction state to color selection
				gameState.ChangeInteractionState (InteractionState.ColorSelection);
				TakiLogger.LogGameState ("Interaction state changed to ColorSelection");

				// Show color selection panel (this will disable PLAY/DRAW buttons)
				GetActiveUI ()?.ShowColorSelection (true);
				TakiLogger.LogUI ("Color selection panel displayed");

				// Mark action as taken but don't allow new actions
				hasPlayerTakenAction = true;
				canPlayerPlay = false;         // Disable play during color selection
				canPlayerDraw = false;         // Disable draw during color selection
				canPlayerEndTurn = true;       // Allow end turn after color selection

				// Note: END TURN button will be enabled, but the color selection must complete first
				GetActiveUI ()?.UpdateStrictButtonStates (false, false, true);

				// ENHANCED: Better feedback about the selection process 
				GetActiveUI ()?.ShowPlayerMessage ("CHANGE COLOR: Choose colors freely, then click END TURN!");
				GetActiveUI ()?.ShowComputerMessage ("Player is selecting new color...");

				TakiLogger.LogTurnFlow ("ChangeColor turn flow: Color selection required, END TURN enabled after selection");
				return;
			}

			// PHASE 7: Check if this was the additional action after a Plus card
			if (isWaitingForAdditionalAction && activeSpecialCardEffect == CardType.Plus) {
				TakiLogger.LogTurnFlow ("COMPLETING ADDITIONAL ACTION after Plus card");

				// Clear special card state
				isWaitingForAdditionalAction = false;
				activeSpecialCardEffect = CardType.Number;

				// Now follow normal turn completion flow
				hasPlayerTakenAction = true;
				canPlayerPlay = false;
				canPlayerDraw = false;
				canPlayerEndTurn = true;

				GetActiveUI ()?.ForceEnableEndTurn ();
				GetActiveUI ()?.ShowPlayerMessage ("Additional action complete - you must END TURN!");

				TakiLogger.LogTurnFlow ("Additional action completed - normal END TURN flow");
				return;
			}

			// Normal card flow (non-Plus, non-ChangeColor cards or cards not during Plus sequence)
			TakiLogger.LogTurnFlow ("NORMAL CARD TURN FLOW - Single action, must end turn");

			hasPlayerTakenAction = true;
			canPlayerPlay = false;
			canPlayerDraw = false; // Cannot draw after playing
			canPlayerEndTurn = true;

			// UI already disabled buttons immediately on click, now enable END TURN
			GetActiveUI ()?.ForceEnableEndTurn ();
			GetActiveUI ()?.ShowPlayerMessage ("Card played - you must END TURN!");

			TakiLogger.LogTurnFlow ("Normal turn flow: Must END TURN after single action");
		}
```

#### My proposal:

```csharp
		/// <summary> 
		/// Handle post card play turn flow with proper TAKI sequence handling (sequence continuation and End Sequence button management)
		/// </summary>
		/// <param name="card">Card that was just played</param>
		void HandlePostCardPlayTurnFlow (CardData card) {
			TakiLogger.LogTurnFlow ($"=== HANDLING POST-CARD-PLAY TURN FLOW for {card.GetDisplayText ()} ===", TakiLogger.LogLevel.Debug);

			// Check if TAKI sequence was started and we should continue turn
			if (gameState.IsInTakiSequence && (card.cardType == CardType.Taki || card.cardType == CardType.SuperTaki) && !isCurrentCardLastInSequence) {
				TakiLogger.LogTurnFlow ($"{card.cardType} card started sequence - turn continues for sequence play", TakiLogger.LogLevel.Debug);

				// For TAKI/SuperTAKI cards that start sequences, keep turn active
				hasPlayerTakenAction = false;  // Reset to allow sequence cards
				canPlayerPlay = true;          // Re-enable play for sequence
				canPlayerDraw = false;         // Cannot draw during sequence
				canPlayerEndTurn = false;      // Cannot end turn during sequence (must use End Sequence button)

				GetActiveUI ()?.UpdateStrictButtonStates (true, false, false);
				GetActiveUI ()?.EnableEndTakiSequenceButton (true);

				if (card.cardType == CardType.Taki) {
					GetActiveUI ()?.ShowPlayerMessage ($"TAKI Sequence active - play {card.color} cards or End Sequence!");
				} else {
					GetActiveUI ()?.ShowPlayerMessage ($"SuperTAKI Sequence active - play {gameState.TakiSequenceColor} cards or End Sequence!");
				}

				TakiLogger.LogTurnFlow ($"{card.cardType} sequence turn flow: PLAY enabled, DRAW/END TURN disabled, END SEQUENCE enabled", TakiLogger.LogLevel.Debug);
				return;
			}

			// Check if card was played during existing sequence (not last card)
			if (gameState.IsInTakiSequence && !isCurrentCardLastInSequence) {
				TakiLogger.LogTurnFlow ("Card played in existing sequence - turn continues");

				// Continue sequence - same button states as sequence start 
				hasPlayerTakenAction = false;  // Allow more sequence cards
				canPlayerPlay = true;          // Keep play enabled
				canPlayerDraw = false;         // Still no draw during sequence
				canPlayerEndTurn = false;      // Still no end turn during sequence

				GetActiveUI ()?.UpdateStrictButtonStates (true, false, false);
				GetActiveUI ()?.ShowPlayerMessage ("Sequence continues - play more cards or End Sequence!");

				TakiLogger.LogTurnFlow ("Sequence continuation: PLAY enabled, DRAW/END TURN disabled");
				return;
			}

			// Check if sequence just ended (last card processed)
			if (isCurrentCardLastInSequence) {
				TakiLogger.LogTurnFlow ("Last card in sequence processed - ending turn normally");

				// Reset flag
				isCurrentCardLastInSequence = false;

				// Force end turn after sequence completion
				hasPlayerTakenAction = true;
				canPlayerPlay = false;
				canPlayerDraw = false;
				canPlayerEndTurn = true;

				GetActiveUI ()?.ForceEnableEndTurn ();
				GetActiveUI ()?.ShowPlayerMessage ("Sequence completed - you must END TURN!");

				TakiLogger.LogTurnFlow ("Sequence completion: Must END TURN");
				return;
			}

			// Plus card handling with better messaging
			if (card.cardType == CardType.Plus && !gameState.IsInTakiSequence) {
				TakiLogger.LogTurnFlow ("PLUS CARD PLAYED - Player gets additional action", TakiLogger.LogLevel.Debug);

				// Set special card state
				isWaitingForAdditionalAction = true;
				activeSpecialCardEffect = CardType.Plus;

				// Reset action state to allow additional action
				hasPlayerTakenAction = false;  // Reset to allow additional action
				canPlayerPlay = true;          // Re-enable play
				canPlayerDraw = true;          // Re-enable draw  
				canPlayerEndTurn = false;      // Keep end turn disabled until additional action

				// Update UI with better messaging
				GetActiveUI ()?.UpdateStrictButtonStates (true, true, false);
				GetActiveUI ()?.ShowSpecialCardEffect (CardType.Plus, PlayerType.Human, "You get one more action");

				TakiLogger.LogTurnFlow ("Plus card turn flow: Additional action enabled, END TURN disabled", TakiLogger.LogLevel.Debug);
				return;
			}

			// ChangeColor card handling for Player
			if (card.cardType == CardType.ChangeColor && !gameState.IsInTakiSequence) {
				TakiLogger.LogTurnFlow ("CHANGE COLOR CARD PLAYED BY PLAYER - Player must select color", TakiLogger.LogLevel.Debug);

				if (gameState == null) {
					TakiLogger.LogError ("Cannot handle ChangeColor: Missing gameState component!", TakiLogger.LogCategory.Rules);
					return;
				}

				// Set interaction state to color selection
				gameState.ChangeInteractionState (InteractionState.ColorSelection);
				TakiLogger.LogGameState ("Interaction state changed to ColorSelection", TakiLogger.LogLevel.Debug);

				// Show color selection panel (this will disable PLAY/DRAW buttons)
				GetActiveUI ()?.ShowColorSelection (true);
				TakiLogger.LogUI ("Color selection panel displayed", TakiLogger.LogLevel.Debug);

				// Mark action as taken but don't allow new actions
				hasPlayerTakenAction = true;
				canPlayerPlay = false;         // Disable play during color selection
				canPlayerDraw = false;         // Disable draw during color selection
				canPlayerEndTurn = true;       // Allow end turn after color selection

				// Note: END TURN button will be enabled, but the color selection must complete first
				GetActiveUI ()?.UpdateStrictButtonStates (false, false, true);

				// ENHANCED: Better feedback about the selection process 
				GetActiveUI ()?.ShowPlayerMessage ("CHANGE COLOR: Choose colors freely, then click END TURN!");
				GetActiveUI ()?.ShowOpponentMessage ("Waiting for player to finish selecting new color...");

				TakiLogger.LogTurnFlow ("ChangeColor turn flow: Color selection required, END TURN enabled after selection", TakiLogger.LogLevel.Debug);
				return;
			}

			// Check if this was the additional action after a Plus card
			if (isWaitingForAdditionalAction && activeSpecialCardEffect == CardType.Plus) {
				TakiLogger.LogTurnFlow ("COMPLETING ADDITIONAL ACTION after Plus card", TakiLogger.LogLevel.Debug);

				// Clear special card state
				isWaitingForAdditionalAction = false;
				activeSpecialCardEffect = CardType.Number;

				// Now follow normal turn completion flow
				hasPlayerTakenAction = true;
				canPlayerPlay = false;
				canPlayerDraw = false;
				canPlayerEndTurn = true;

				GetActiveUI ()?.ForceEnableEndTurn ();
				GetActiveUI ()?.ShowPlayerMessage ("Additional action complete - you must END TURN!");

				TakiLogger.LogTurnFlow ("Additional action completed - normal END TURN flow", TakiLogger.LogLevel.Debug);
				return;
			}

			// Normal card flow (non-Plus, non-ChangeColor cards or cards not during Plus sequence)
			TakiLogger.LogTurnFlow ("NORMAL CARD TURN FLOW - Single action, must end turn", TakiLogger.LogLevel.Debug);

			hasPlayerTakenAction = true;
			canPlayerPlay = false;
			canPlayerDraw = false; // Cannot draw after playing
			canPlayerEndTurn = true;

			// UI already disabled buttons immediately on click, now enable END TURN
			GetActiveUI ()?.ForceEnableEndTurn ();
			GetActiveUI ()?.ShowPlayerMessage ("Card played - you must END TURN!");

			TakiLogger.LogTurnFlow ("Normal turn flow: Must END TURN after single action", TakiLogger.LogLevel.Debug);
		}
```

### `HandlePostCardDrawTurnFlow`

#### Original code:

```csharp
		/// <summary>
		/// PHASE 7: Handle turn flow after drawing a card
		/// </summary>
		/// <param name="drawnCard">Card that was drawn</param>
		void HandlePostCardDrawTurnFlow (CardData drawnCard) {
			TakiLogger.LogTurnFlow ($"=== HANDLING POST-DRAW TURN FLOW ===");

			// PHASE 7: Check if this was the additional action after a Plus card
			if (isWaitingForAdditionalAction && activeSpecialCardEffect == CardType.Plus) {
				TakiLogger.LogTurnFlow ("COMPLETING ADDITIONAL DRAW ACTION after Plus card");

				// Clear special card state
				isWaitingForAdditionalAction = false;
				activeSpecialCardEffect = CardType.Number;

				// Now follow normal turn completion flow
				hasPlayerTakenAction = true;
				canPlayerPlay = false;
				canPlayerDraw = false;
				canPlayerEndTurn = true;

				GetActiveUI ()?.ForceEnableEndTurn ();
				GetActiveUI ()?.ShowPlayerMessage ($"Additional draw complete: {drawnCard.GetDisplayText ()} - you must END TURN!");

				TakiLogger.LogTurnFlow ("Additional draw action completed - normal END TURN flow");
				return;
			}

			// Normal draw flow (first action or not during Plus sequence)
			TakiLogger.LogTurnFlow ("NORMAL DRAW TURN FLOW - Single action, must end turn");

			hasPlayerTakenAction = true;
			canPlayerPlay = false;
			canPlayerDraw = false;
			canPlayerEndTurn = true;

			// Enable END TURN button
			GetActiveUI ()?.ForceEnableEndTurn ();
			GetActiveUI ()?.ShowPlayerMessage ($"Drew: {drawnCard.GetDisplayText ()} - you must END TURN!");

			TakiLogger.LogTurnFlow ("Normal draw flow: Must END TURN after single action");
		}
```
Looks good.

### `EndPlayerTurnWithStrictFlow`

#### Original code:

```csharp
		/// <summary>
		/// FIXED: Enhanced turn completion to handle Stop card effects at correct timing
		/// </summary>
		void EndPlayerTurnWithStrictFlow () {
			TakiLogger.LogTurnFlow ("ENDING PLAYER TURN - STRICT FLOW WITH SPECIAL CARDS");

			// STOP CARD FIX: Check STOP flag FIRST, before any turn switching logic
			if (shouldSkipNextTurn) {
				TakiLogger.LogTurnFlow ("=== STOP FLAG DETECTED - PROCESSING STOP EFFECT ===");
				ProcessStopSkipEffect ();
				return; // Exit early - don't proceed with normal turn switch
			}

			// Check for pending effects (PLUS cards only)
			if (HasPendingSpecialCardEffects ()) {
				TakiLogger.LogError ("Attempting to end turn with pending special card effects!", TakiLogger.LogCategory.TurnFlow);
				GetActiveUI ()?.ShowPlayerMessage ("Cannot end turn - special card effect pending!");
				return;
			}

			// FIXED: Hide color selection panel if it's still visible
			if (gameState != null && gameState.interactionState == InteractionState.ColorSelection) {
				TakiLogger.LogGameState ("Ending turn during color selection - hiding panel and returning to normal");

				// Hide the color selection panel
				GetActiveUI ()?.ShowColorSelection (false);

				// Return to normal interaction state
				gameState.ChangeInteractionState (InteractionState.Normal);

				// Show completion message
				GetActiveUI ()?.ShowPlayerMessage ($"Color selection complete: {gameState.activeColor}");
			}

			// Clear any selected cards
			GetActivePlayerHandManager ()?.ClearSelection ();

			// Reset turn flow state for next turn (includes special card state)
			ResetTurnFlowState ();

			// Normal turn end - proceed with turn switch
			TakiLogger.LogTurnFlow ("Normal turn end - switching to computer turn");
			if (turnManager != null) {
				turnManager.EndTurn ();
			}
		}
```
Looks good.

### `EndAITurnWithStrictFlow`

#### Original code:

```csharp
		/// <summary>
		/// FIXED: Enhanced AI turn completion to handle Stop card effects at correct timing
		/// Mirrors the logic in EndPlayerTurnWithStrictFlow() for symmetrical behavior
		/// </summary>
		void EndAITurnWithStrictFlow () {
			TakiLogger.LogTurnFlow ("ENDING AI TURN - STRICT FLOW WITH SPECIAL CARDS");

			// STOP CARD FIX: Check STOP flag FIRST, before any turn switching logic
			if (shouldSkipNextTurn) {
				TakiLogger.LogTurnFlow ("=== STOP FLAG DETECTED FOR AI TURN END - PROCESSING STOP EFFECT ===");
				ProcessStopSkipEffect ();
				return; // Exit early - don't proceed with normal turn switch
			}

			// Normal turn end - proceed with turn switch
			TakiLogger.LogTurnFlow ("Normal AI turn end - switching to human turn");
			if (turnManager != null) {
				turnManager.EndTurn ();
			}
		}
```

Looks good.

### `StartPlayerTurnAfterStop`

#### Original code:

```csharp
		/// <summary>
		/// NEW: Start player turn after STOP effect processing
		/// </summary>
		void StartPlayerTurnAfterStop () {
			TakiLogger.LogTurnFlow ("=== STARTING PLAYER TURN AFTER STOP EFFECT ===");

			// Ensure game state shows player turn
			if (gameState != null) {
				gameState.ChangeTurnState (TurnState.PlayerTurn);
			}

			// Start the player turn flow
			StartPlayerTurnFlow ();

			// Show additional feedback
			GetActiveUI ()?.ShowPlayerMessage ("Your turn continues thanks to STOP card!");
			GetActiveUI ()?.ShowComputerMessage ("Waiting (turn skipped)...");

			TakiLogger.LogTurnFlow ("Player turn restarted successfully after STOP effect");
		}
```

#### My proposal:

```csharp
```

### `StartAITurnAfterStop`

#### Original code:

```csharp
		/// <summary>
		/// NEW: Start AI turn after STOP effect processing
		/// Mirrors StartPlayerTurnAfterStop() for when computer benefits from STOP
		/// </summary>
		void StartAITurnAfterStop () {
			TakiLogger.LogTurnFlow ("=== STARTING AI TURN AFTER STOP EFFECT ===");

			// Ensure game state shows computer turn
			if (gameState != null) {
				gameState.ChangeTurnState (TurnState.ComputerTurn);
			}

			// Show feedback messages
			gameplayUI?.ShowPlayerMessage ("Computer gets another turn thanks to STOP card!");
			gameplayUI?.ShowComputerMessage ("I get another turn!");

			// Trigger AI decision for the new turn
			CardData topCard = GetTopDiscardCard ();
			if (topCard != null && computerAI != null) {
				TakiLogger.LogTurnFlow ("Triggering AI decision for STOP benefit turn");
				// Give AI time to "think" about the new turn
				Invoke (nameof (TriggerAITurnAfterStop), 1.5f);
			} else {
				TakiLogger.LogError ("Cannot start AI turn after STOP - missing components", TakiLogger.LogCategory.AI);
			}

			TakiLogger.LogTurnFlow ("AI turn setup complete after STOP effect");
		}

```

#### My proposal:

```csharp
```

### `TriggerAITurnAfterStop`

#### Original code:

```csharp
		/// <summary>
		/// Helper method to trigger AI decision after STOP benefit
		/// </summary>
		void TriggerAITurnAfterStop () {
			TakiLogger.LogAI ("=== AI MAKING DECISION FOR STOP BENEFIT TURN ===");

			CardData topCard = GetTopDiscardCard ();
			if (topCard != null && computerAI != null) {
				gameplayUI?.ShowComputerMessage ("Thinking about my bonus turn...");
				computerAI.MakeDecision (topCard);
			} else {
				TakiLogger.LogError ("Cannot trigger AI turn after STOP - missing components", TakiLogger.LogCategory.AI);
			}
		}
```

#### My proposal:

```csharp
```

---

```

Do you want me to continue with **all regions expanded like this** (one huge file), or do you prefer me to give them to you **piece by piece** (region by region, like this)?
```
