[UI] Screen references resolved
[SYS] BackgroundMusic marked as persistent and will not be destroyed on scene load.
[SYS] Switched to DEVELOPMENT mode
[SYS] TakiLogger configured: TakiLogger - Level: Info, Mode: Development
[SYS] Validating and connecting components...
[SYS] New UI architecture validation complete - both UI managers assigned
[SYS] Per-screen HandManager architecture validation complete - all HandManagers assigned
[SYS] Components validated and connected - Ready for game mode selection
[DIAG] TakiGameDiagnostics ready. Press F1 for full diagnostics, F2 for rule validation, F3 for turn sequence test.
[SYS] ExitValidationManager dependencies resolved
[SYS] GameEndManager dependencies resolved
[SYS] PauseManager dependencies resolved
[UI] New UI Architecture - MultiPlayerUIManager starting
[UI] Connecting button events with STRICT FLOW validation...
[SYS] All button events connected with strict flow validation
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: DISABLED
[UI] New UI Architecture - SinglePlayerUIManager starting
[UI] Connecting button events with STRICT FLOW validation...
[SYS] All button events connected with strict flow validation
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: DISABLED
[SYS] Loaded 110 cards from Resources/Data/Cards
[SYS] Deck composition verified: All cards loaded successfully
[SYS] DeckManager components initialized
[DECK] Deck Message: Loading deck...
[SYS] HandManager Player2HandPanel: Awake() called - HandManager initializing...
[SYS] HandManager Player1HandPanel: Awake() called - HandManager initializing...
[STATE] Starting single player game...
[SYS] Initializing single player game systems...
[INVESTIGATION] ConnectEvents called!
[SYS] === CONNECTING ACTIVE UI MANAGER EVENTS ===
[SYS] useNewUIArchitecture: True
[SYS] singlePlayerUI: ASSIGNED
[SYS] multiPlayerUI: ASSIGNED
[SYS] Event handlers connected to SinglePlayerUI
[SYS] Event handlers connected to MultiPlayerUI
[SYS] === UI MANAGER EVENTS CONNECTION COMPLETE ===
[SYS] Initializing visual card system...
[SYS] Visual card system initialized with per-screen architecture - Mode: SinglePlayer
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: DISABLED
[TURN] Strict button state update complete
[SYS] Single player systems initialized - Ready to start game
[SYS] Starting new single player game...
[AI] Clearing hand and resetting all AI state (including sequence state)
[AI] Cancelling all AI operations
[AI] All AI operations cancelled
[AI] AI hand cleared and all state reset (including sequence processing)
[STATE] Game state reset for new game (including PlusTwo chain and TAKI sequence state)
[TURNS] Turn manager reset
[TURN] === RESETTING SPECIAL CARD STATE ===
[TURN] TURN FLOW STATE RESET (includes special card state)
[SYS] Game initialization state reset - ready for new game
[DECK] Deck Message: New deck shuffled!
[DECK] Shuffled
[DECK] Initialized with 110 cards
[STATE] New game initialized successfully
[DECK] Drew card: Yellow 1
[DECK] Drew card: Yellow 1
[DECK] Drew card: Blue Stop
[DECK] Drew card: Green PlusTwo
[DECK] Drew card: Blue 7
[DECK] Drew card: Wild ChangeColor
[DECK] Drew card: Yellow 3
[DECK] Drew card: Green 3
[DECK] Drew card: Green Stop
[DECK] Drew card: Red ChangeDirection
[DECK] Drew card: Yellow 5
[DECK] Drew card: Wild SuperTaki
[DECK] Drew card: Red 4
[DECK] Drew card: Red 3
[DECK] Drew card: Blue Stop
[DECK] Drew card: Green Taki
[DECK] Drew card: Red Plus
[DECK] Drew card: Blue ChangeDirection
[DECK] Drew card: Green 4
[DECK] Discarded card: Green 4
[STATE] Starting card: Green 4
[STATE] Initial setup complete. Player 1: 8 cards, Player 2: 8 cards
[AI] AI received 8 cards. Hand size: 8
[STATE] Active color changed: Wild -> Green
[UI] === UPDATING ALL DISPLAYS (FIXED SEQUENCE CONTROL) ===
[UI] === UPDATING TURN DISPLAY for Neutral ===
[TURN] Turn display updated - button states controlled by strict flow system
[UI] End TAKI Sequence button DISABLED & HIDDEN (Human sequences only)
[UI] TAKI sequence status hidden
[UI] Handling active game state
[UI] All displays updated (with enhanced TAKI sequence support)
[UI] Chain status hidden
[NET] Hand display updated: 8 cards, Network=False, Opponent=False
[NET] Hand display updated: 8 cards, Network=False, Opponent=False
[TURNS] Turn system initialized. First player: Human
[STATE] Turn state changed: Neutral -> PlayerTurn
[STATE] Turn state changed to PlayerTurn
[UI] === UPDATING TURN DISPLAY for PlayerTurn ===
[TURN] Turn display updated - button states controlled by strict flow system
[TURNS] Turn started for: Human
[SYS] Game started! Player: 8 cards, Computer: 8 cards
[DECK] Deck Message: Starting card: Green 4
[NET] SetupInitialGame successful: P1=8, P2=8, Start=Green 4
[SYS] HandManager Player2HandPanel: Start() called - Looking for GameManager...
[SYS] HandManager Player2HandPanel: DIAGNOSTIC - GameManager found, checking UI architecture...
[SYS]   - useNewUIArchitecture: True
[SYS]   - singlePlayerUI: ASSIGNED
[SYS]   - multiPlayerUI: ASSIGNED
[SYS] GetActiveUI() called - useNewUIArchitecture: True, isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] HandManager Player2HandPanel: Connected to active UI manager: SinglePlayerUIManager
[SYS] HandManager Player1HandPanel: Start() called - Looking for GameManager...
[SYS] HandManager Player1HandPanel: DIAGNOSTIC - GameManager found, checking UI architecture...
[SYS]   - useNewUIArchitecture: True
[SYS]   - singlePlayerUI: ASSIGNED
[SYS]   - multiPlayerUI: ASSIGNED
[SYS] GetActiveUI() called - useNewUIArchitecture: True, isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] HandManager Player1HandPanel: Connected to active UI manager: SinglePlayerUIManager
[TURN] STARTING PLAYER TURN WITH PLSTWO CHAIN AWARENESS
[TURN] Normal turn flow - no active chain
[RULES] Move validation: Yellow 1 on Green 4 with active color Green = False
[RULES] Move validation: Yellow 1 on Green 4 with active color Green = False
[RULES] Move validation: Blue Stop on Green 4 with active color Green = False
[RULES] Move validation: Green PlusTwo on Green 4 with active color Green = True
[RULES] Move validation: Blue 7 on Green 4 with active color Green = False
[RULES] Move validation: Wild ChangeColor on Green 4 with active color Green = True
[RULES] Move validation: Yellow 3 on Green 4 with active color Green = False
[RULES] Move validation: Green 3 on Green 4 with active color Green = True
[TURN] Player has 3 valid cards, may PLAY or DRAW a card
[SYS] GetActiveUI() called - useNewUIArchitecture: True, isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: ENABLED
[TURN] DRAW: ENABLED
[TURN] END TURN: DISABLED
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Yellow 1 on Green 4 with active color Green = False
[RULES] Move validation: Yellow 1 on Green 4 with active color Green = False
[RULES] Move validation: Blue Stop on Green 4 with active color Green = False
[RULES] Move validation: Green PlusTwo on Green 4 with active color Green = True
[RULES] Move validation: Blue 7 on Green 4 with active color Green = False
[RULES] Move validation: Wild ChangeColor on Green 4 with active color Green = True
[RULES] Move validation: Yellow 3 on Green 4 with active color Green = False
[RULES] Move validation: Green 3 on Green 4 with active color Green = True
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Yellow 1 on Green 4 with active color Green = False
[RULES] Move validation: Yellow 1 on Green 4 with active color Green = False
[RULES] Move validation: Blue Stop on Green 4 with active color Green = False
[RULES] Move validation: Green PlusTwo on Green 4 with active color Green = True
[RULES] Move validation: Blue 7 on Green 4 with active color Green = False
[RULES] Move validation: Wild ChangeColor on Green 4 with active color Green = True
[RULES] Move validation: Yellow 3 on Green 4 with active color Green = False
[RULES] Move validation: Green 3 on Green 4 with active color Green = True
[UI] Player selected visual card: Green 3
[RULES] Move validation: Green 3 on Green 4 with active color Green = True
[TURN] === PLAY CARD BUTTON CLICKED ===
[TURN] === PLAY CARD BUTTON CLICKED ===
[CARD] Attempting to play selected card: Green 3
[CARD] PLAYING CARD WITH STRICT FLOW (PHASE 8B): Green 3
[RULES] Move validation: Green 3 on Green 4 with active color Green = True
[DECK] Discarded card: Green 3
[RULES] === CALLING HandleSpecialCardEffects WITH SEQUENCE AWARENESS ===
[RULES] === HANDLING SPECIAL CARD EFFECTS for Green 3 ===
[RULES] Card type: Number
[RULES] Card name: Green 3
[RULES] SEQUENCE CONTEXT: In sequence=False, Last card=False
[RULES] SPECIAL EFFECT ACTIVATION: True
[RULES] NUMBER card - no special effects
[RULES] === HandleSpecialCardEffects COMPLETED ===
[RULES] === CALLING LogCardEffectRules ===
[RULES] === CARD EFFECT ANALYSIS: Green 3 ===
[RULES] NUMBER CARD: Green 3
[RULES] RULE: Basic card - no special effects
[RULES] TURN FLOW: Player must END TURN after playing
[RULES] IMPLEMENTATION: Standard single-action turn completion
[RULES] === CARD EFFECT ANALYSIS COMPLETE ===
[RULES] === LogCardEffectRules COMPLETED ===
[UI] === UPDATING ALL DISPLAYS (FIXED SEQUENCE CONTROL) ===
[UI] === UPDATING TURN DISPLAY for PlayerTurn ===
[TURN] Turn display updated - button states controlled by strict flow system
[UI] End TAKI Sequence button DISABLED & HIDDEN (Human sequences only)
[UI] TAKI sequence status hidden
[UI] Handling active game state
[UI] All displays updated (with enhanced TAKI sequence support)
[UI] Chain status hidden
[RULES] Move validation: Yellow 1 on Green 3 with active color Green = False
[RULES] Move validation: Yellow 1 on Green 3 with active color Green = False
[RULES] Move validation: Blue Stop on Green 3 with active color Green = False
[RULES] Move validation: Green PlusTwo on Green 3 with active color Green = True
[RULES] Move validation: Blue 7 on Green 3 with active color Green = False
[RULES] Move validation: Wild ChangeColor on Green 3 with active color Green = True
[RULES] Move validation: Yellow 3 on Green 3 with active color Green = True
[NET] Hand display updated: 7 cards, Network=False, Opponent=False
[NET] Hand display updated: 8 cards, Network=False, Opponent=False
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Yellow 1 on Green 3 with active color Green = False
[RULES] Move validation: Yellow 1 on Green 3 with active color Green = False
[RULES] Move validation: Blue Stop on Green 3 with active color Green = False
[RULES] Move validation: Green PlusTwo on Green 3 with active color Green = True
[RULES] Move validation: Blue 7 on Green 3 with active color Green = False
[RULES] Move validation: Wild ChangeColor on Green 3 with active color Green = True
[RULES] Move validation: Yellow 3 on Green 3 with active color Green = True
[TURN] === HANDLING POST-CARD-PLAY TURN FLOW for Green 3 ===
[TURN] NORMAL CARD TURN FLOW - Single action, must end turn
[TURN] === FORCE ENABLING END TURN BUTTON ===
[TURN] Action was successful - player must now END TURN
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: ENABLED
[TURN] Strict button state update complete
[TURN] Normal turn flow: Must END TURN after single action
[TURN] CARD PLAY COMPLETE - Turn flow handled based on card type
[TURN] === END TURN BUTTON CLICKED ===
[TURN] END TURN clicked but button should be disabled!

Assets\Scripts\Core\GameManager.cs(278,20): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ResetUIForNewGame' and no accessible extension method 'ResetUIForNewGame' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(340,21): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ResetUIForNewGame' and no accessible extension method 'ResetUIForNewGame' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(425,20): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowOpponentAction' and no accessible extension method 'ShowOpponentAction' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(459,20): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowOpponentAction' and no accessible extension method 'ShowOpponentAction' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(605,21): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'UpdateTurnDisplayMultiplayer' and no accessible extension method 'UpdateTurnDisplayMultiplayer' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Managers\GameEndManager.cs(100,31): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowWinnerAnnouncement' and no accessible extension method 'ShowWinnerAnnouncement' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(1292,21): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSequenceProgressMessage' and no accessible extension method 'ShowSequenceProgressMessage' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(1423,21): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Multiplayer\MultiplayerGameManager.cs(362,31): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowDeckSyncStatus' and no accessible extension method 'ShowDeckSyncStatus' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Multiplayer\MultiplayerGameManager.cs(468,32): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'UpdateTurnDisplayMultiplayer' and no accessible extension method 'UpdateTurnDisplayMultiplayer' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2423,20): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSequenceProgressMessage' and no accessible extension method 'ShowSequenceProgressMessage' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2773,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2802,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2826,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2851,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowImmediateFeedback' and no accessible extension method 'ShowImmediateFeedback' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2859,24): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2920,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2927,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2951,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2958,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(3142,21): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowWinnerAnnouncement' and no accessible extension method 'ShowWinnerAnnouncement' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(3686,19): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'UpdateButtonStates' and no accessible extension method 'UpdateButtonStates' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)
















# investigating!

To track, we can create a TODO list.
I will not be giving much attention to purely testing-purposed methods.

## `ResetUIForNewGame()`

- In `GameManager.cs`, read `InitializeSinglePlayerSystems ()` and `InitializeMultiPlayerSystems ()`
   - In both `InitializeSinglePlayerSystems ()` and `InitializeMultiPlayerSystems ()` we see the following lines:
      ```csharp
      // Initialize UI for gameplay
      if (gameplayUI != null) {
         GetActiveUI ()?.ResetUIForNewGame ();
      }
      ```
   - I find this odd - My understanding is that `gameplayUI` is legacy related, so why is our new architecture (`GetActiveUI`) dependant or even related to it?
   - Also, It's a bit problematic or at the very least messy, these 2 functions that have these 2 methods are so differently orginized - of course some things must be different (network, ai, etc), but the base should be fairly simular, don't you think? 
- Wait, what?? I am finding `public void ResetUIForNewGame ()` in 3 scripts:
   - In `GameplayUIManager`, makes sense since this is a legacy script
   - In `SinglePlayerUIManager`, practicaly identical to the one in `GameplayUIManager`, which makes sense. 
      - But `public void ResetUIForNewGame ()` doesn't appear in `BaseGameplayUIManager` or in `MultiPlayerUIManager`. If anything, it should be in `BaseGameplayUIManager`, and not the inheritants, right?
   - In `DeckUIManager` - This is bizzare I think. Maybe there was a mix up in the names? We need to investigate this carefully
- I can see we have `ForceNewGameStart ()`, which is never being used anywhere. I think it's best if we remove this.

## `ShowOpponentAction()`

- In `GameManager.cs`
   - In both `ProcessNetworkCardDraw ()` and `ProcessNetworkCardPlay ()` we see the following lines:
      ```csharp
		// Enhanced feedback for milestone 1
		if (gameplayUI != null) {
			GetActiveUI ()?.ShowOpponentAction ($"played {cardIdentifier}");
			GetActiveUI ()?.ShowComputerMessage ($"Opponent played {cardIdentifier}");
		}
      ```
   - Same issue as before, `gameplayUI` is legacy related.
   - I find it odd that `ProcessNetworkCardDraw` has `UpdateAllUI ();`, while `ProcessNetworkCardPlay` has `UpdateAllUIWithNetworkSupport ();`.
- I am finding `public void ShowOpponentAction (string action)` in 2 scripts:
   - In `GameplayUIManager`, makes sense since this is a legacy script
   - In `MultiPlayerUIManager`, practicaly identical to the one in `GameplayUIManager`, which makes sense. 
      - But `public void ResetUIForNewGame ()` doesn't appear in `BaseGameplayUIManager` or in `SinglePlayerUIManager`. If anything, it should be in `BaseGameplayUIManager`, and not the inheritants, right?

## `UpdateTurnDisplayMultiplayer()`

- In `GameManager.cs`
   - In `UpdateAllUIWithNetworkSupport ()` we see the following lines:
      ```csharp
		// MILESTONE 1: Additional network-specific UI updates
		if (isMultiplayerMode && networkGameManager != null) {
			// Update turn display for multiplayer
			if (gameplayUI != null) {
				GetActiveUI ()?.UpdateTurnDisplayMultiplayer (networkGameManager.IsMyTurn);
			}

			// Sync hand counts for network display (already done above)
			SynchronizeNetworkHandCounts ();
		}
      ```
   - Same issue as before, `gameplayUI` is legacy related.
- In `MultiplayerGameManager.cs`
   - In `OnTurnBegins (int turn)` we see the following lines:
      ```csharp
		// Update GameManager turn state
		if (gameManager != null && gameManager.gameState != null) {
			TurnState newTurnState = _isMyTurn ? TurnState.PlayerTurn : TurnState.ComputerTurn;
			gameManager.gameState.ChangeTurnState (newTurnState);

			// Update UI
			if (gameManager.GetActiveUI() != null) {
				gameManager.GetActiveUI().UpdateTurnDisplayMultiplayer (_isMyTurn);
			}
		}
      ```
   - I suppose it alright, after we'll fix `GameManager.cs` that is.
- I am finding `public void UpdateTurnDisplayMultiplayer (bool isLocalPlayerTurn)` and also `UpdateTurnDisplayMultiplayer(isMyTurn);` in 2 scripts:
   - In `GameplayUIManager`, makes sense since this is a legacy script
   - In `MultiPlayerUIManager`, practicaly identical to the one in `GameplayUIManager`, which makes sense. 

## `ShowSequenceProgressMessage()`

- In `GameManager.cs`
   - In `OnTakiSequenceStarted (CardColor sequenceColor, PlayerType initiator)` we see the following lines:
      ```csharp
		// Update UI to show sequence status
		if (gameplayUI != null) {
			gameplayUI.ShowTakiSequenceStatus (sequenceColor, 1, initiator == PlayerType.Human);
			GetActiveUI ()?.ShowSequenceProgressMessage (sequenceColor, 1, initiator);
		}
      ```
   - Same issue as before, `gameplayUI` is legacy related.
   - In `PlayCardWithStrictFlow (CardData card)` we see the following lines:
      ```csharp
		// Update sequence UI immediately
		if (gameplayUI != null) {
			int cardCount = gameState.NumberOfSequenceCards;
			CardColor sequenceColor = gameState.TakiSequenceColor;
			GetActiveUI ()?.ShowTakiSequenceStatus (sequenceColor, cardCount, true);
			GetActiveUI ()?.ShowSequenceProgressMessage (sequenceColor, cardCount, PlayerType.Human);
		}
      ```
   - Same issue as before, `gameplayUI` is legacy related.
- I am finding `public void ShowSequenceProgressMessage (CardColor sequenceColor, int cardCount, PlayerType initiator)` in 1 script:
   - In `GameplayUIManager`, makes sense since this is a legacy script (we should probably add to `BaseGameplayUIManager`)

## `ShowSpecialCardEffect()`

- In `GameManager.cs`
   - In `HandlePostCardPlayTurnFlow (CardData card)` we see the following lines:
      ```csharp
		if (gameplayUI != null) {
			GetActiveUI ()?.ShowSpecialCardEffect (CardType.Plus, PlayerType.Human, "You get one more action");
		}
      ```
	   - Same issue as before, `gameplayUI` is legacy related.

   - In `HandlePostCardPlayTurnFlow (CardData card)` we see the following lines:
      ```csharp
		if (gameplayUI != null) {
			GetActiveUI ()?.ShowSpecialCardEffect (CardType.Plus, PlayerType.Computer, "AI gets one more action");
		}
      ```
	   - Same issue as before, `gameplayUI` is legacy related.

      ```csharp
		if (gameplayUI != null) {
			GetActiveUI ()?.ShowSpecialCardEffect (CardType.Stop, PlayerType.Computer, "Your next turn will be skipped!");
		}
      ```
	   - Same issue as before, `gameplayUI` is legacy related.

      ```csharp
		if (gameplayUI != null) {
			string directionMessage = $"Direction changed by AI: {gameState?.turnDirection}";
			GetActiveUI ()?.ShowSpecialCardEffect (CardType.ChangeDirection, PlayerType.Computer, directionMessage);
		}
      ```
	   - Same issue as before, `gameplayUI` is legacy related.

      ```csharp
		if (gameplayUI != null) {
			GetActiveUI ()?.ShowSpecialCardEffect (CardType.ChangeColor, PlayerType.Computer,
				$"New active color: {selectedColor}");
		}
      ```
	   - Same issue as before, `gameplayUI` is legacy related.

      ```csharp
		gameplayUI?.ShowSpecialCardEffect (CardType.PlusTwo, PlayerType.Computer,
			$"Chain continues! Draw {drawCount} or play PlusTwo");
      ```
	   - Same issue as before, `gameplayUI` is legacy related.

      ```csharp
		TakiLogger.LogError ("AI played PlusTwo but no chain is active - this indicates a sequence processing issue!", TakiLogger.LogCategory.AI);
		gameplayUI?.ShowSpecialCardEffect (CardType.PlusTwo, PlayerType.Computer, "You draw 2 cards!");
      ```
	   - Same issue as before, `gameplayUI` is legacy related.

      ```csharp
		if (gameplayUI != null) {
			GetActiveUI ()?.ShowSpecialCardEffect (CardType.Taki, PlayerType.Computer,
				$"AI starts TAKI sequence for {card.color}");
		}
      ```
	   - Same issue as before, `gameplayUI` is legacy related.

      ```csharp
		if (gameplayUI != null) {
			GetActiveUI ()?.ShowSpecialCardEffect (CardType.Taki, PlayerType.Computer,
				"AI continues TAKI sequence");
		}
      ```
	   - Same issue as before, `gameplayUI` is legacy related.

      ```csharp
		if (gameplayUI != null) {
			GetActiveUI ()?.ShowSpecialCardEffect (CardType.SuperTaki, PlayerType.Computer,
				$"AI starts SuperTAKI sequence for {sequenceColor}");
		}
      ```
	   - Same issue as before, `gameplayUI` is legacy related.

      ```csharp
		if (gameplayUI != null) {
			GetActiveUI ()?.ShowSpecialCardEffect (CardType.SuperTaki, PlayerType.Computer,
				"AI continues SuperTAKI sequence");
		}
      ```
	   - Same issue as before, `gameplayUI` is legacy related.

   - In `PlayCardWithStrictFlow (CardData card)` we see the following lines:
      ```csharp

      ```
   - Same issue as before, `gameplayUI` is legacy related.
- I am finding `public void ShowSpecialCardEffect (CardType cardType, PlayerType playedBy, string effectDescription)` in 1 script:
   - In `GameplayUIManager`, makes sense since this is a legacy script (we should probably add to `BaseGameplayUIManager`)

## `ShowWinnerAnnouncement()`

- In `GameManager.cs`
   - In `OnGameWon (PlayerType winner)` we see the following lines:
      ```csharp
		// Process through GameEndManager instead of direct UI
		if (gameEndManager != null) {
			gameEndManager.ProcessGameEnd (winner);
		} else {
			// Fallback to existing logic if GameEndManager not available
			if (gameplayUI != null) {
				GetActiveUI ()?.ShowWinnerAnnouncement (winner);
			}
		}
      ```
   - Same issue as before, `gameplayUI` is legacy related, (even if it's fallback).

- In `GameEndManager.cs`
   - In `private IEnumerator ShowGameEndSequence (PlayerType winner)` we see the following lines:
      ```csharp
		// First, show winner in gameplay UI for a moment
		if (gameManager.GetActiveUI() != null) {
			gameManager.GetActiveUI().ShowWinnerAnnouncement (winner);
		}
      ```
   - I suppose it alright, after we'll fix `GameManager.cs` that is.
   
- I am finding `public void ShowWinnerAnnouncement (PlayerType winner)` in 2 scripts:
   - In `GameplayUIManager`, makes sense since this is a legacy script
   - In `SinglePlayerUIManager`, practicaly identical to the one in `GameplayUIManager`, which makes sense. 
- I am also finding `public void ShowWinnerAnnouncementMultiplayer(bool localPlayerWon)` and also in `MultiPlayerUIManager`, very simular to the other 2, but not identical, which makes sense. 
- It might be best to have a function like this in `BaseGameplayUIManager`.

## `ShowDeckSyncStatus()`

- In `GameManager.cs`
   - We don't see it at all!
   
- I am finding `public void ShowDeckSyncStatus (string message)` in 2 scripts:
   - In `GameplayUIManager`, makes sense since this is a legacy script
   - In `MultiPlayerUIManager`, completely identical to the one in `GameplayUIManager`, which makes sense. 
- I believe it would be best to have a function like this in `BaseGameplayUIManager`.

## `ShowDeckSyncStatus()`

- In `GameManager.cs`
   - In `HandleAISpecialCardEffects (CardData card)` we see the following lines:
      ```csharp
		if (gameplayUI != null) {
			GetActiveUI ()?.ShowImmediateFeedback ("AI is selecting new color...", true);
		}
      ```
   - I suppose it alright, after we'll fix `GameManager.cs` that is.
   
- I am finding `public void ShowImmediateFeedback (string message, bool toPlayer = true)` in 1 script:
   - In `GameplayUIManager`, makes sense since this is a legacy script
- I believe we want some kind of version of this in `BaseGameplayUIManager`, `SinglePlayerUIManager`, and `MultiPlayerUIManager`.

## `UpdateButtonStates()`

- In `GameManager.cs`
   - In `ForceUISync ()` we see the following lines:
      ```csharp
		// Force button state update based on current turn state
		bool shouldEnableButtons = gameState.CanPlayerAct ();
		GetActiveUI ()?.UpdateButtonStates (shouldEnableButtons);
      ```
   - I suppose it alright, after we'll fix `GameManager.cs` that is.

- In `GamePlayUIManager.cs`
   - In `ForceButtonRefresh (bool enableState)` we see the following lines:
      ```csharp
		public void ForceButtonRefresh (bool enableState) {
			TakiLogger.LogDiagnostics ($"=== FORCE BUTTON REFRESH to {(enableState ? "ENABLED" : "DISABLED")} ===");
			UpdateButtonStates (enableState);
			ValidateButtonStates ();
		}
      ```
   - I suppose it alright, after we'll fix `GameManager.cs` that is.
   
- I am finding `public void UpdateButtonStates (bool enabled)` in 1 script:
   - In `GameplayUIManager`, makes sense since this is a legacy script
- I believe we want some kind of version of this in `BaseGameplayUIManager`.





