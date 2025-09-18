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








