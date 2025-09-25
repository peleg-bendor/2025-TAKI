# investigating!

## investigating our methods in `GameManager.cs`

I need you to read and look into each comment here, and acknowledge each comment fully when we discuss.

### `OnInitialGameSetupComplete`
- This one looks a bit trickier. I would like to discuss on it with you.
- It looked to me like `OnInitialGameSetupComplete` was made for singleplayer mode.
- But I fixed it appropriately I believe.
- check it out and inform me if it's alright.

### `OnPlayCardButtonClicked`
- This one looks a bit trickier. I would like to discuss on it with you.
- It looks to me like `OnPlayCardButtonClicked` is made for singleplayer mode, but redirects to multiplyer mode `OnPlayCardButtonClickedMultiplayer` as needed, so The method properly redirects to multiplayer.
- But... how come `OnPlayCardButtonClicked` and `OnPlayCardButtonClickedMultiplayer` look so different? In singleplayer we have all kinds of checks and multiplayer looks very different.
- I want to discuss with you - what is really happening in each method? And is it how it should be?

### `OnDrawCardButtonClicked`
- This one looks a bit trickier. I would like to discuss on it with you.
- It looks to me like `OnDrawCardButtonClicked` is made for singleplayer mode, but redirects to multiplyer mode `OnDrawCardButtonClickedMultiplayer` as needed, so The method properly redirects to multiplayer.
- But... how come `OnDrawCardButtonClicked` and `OnDrawCardButtonClickedMultiplayer` look so different? In singleplayer we have all kinds of checks and multiplayer looks very different.
- I want to discuss with you - what is really happening in each method? And is it how it should be?

### `OnEndTurnButtonClicked`
- This one looks a bit trickier. I would like to discuss on it with you.
- I added multiplayer routing, and I want you to check if it makes sense.
- Now we need to properly implement `OnEndTurnButtonClickedMultiplayer` too, I'd like to know how you would do that.

### `OnEndTakiSequenceButtonClicked`
- This one looks a bit trickier. I would like to discuss on it with you.
- I added multiplayer routing, and I want you to check if it makes sense.
- Now we need to properly implement `OnEndTakiSequenceButtonClickedMultiplayer` too, I'd like to know how you would do that.

### `OnColorSelectedByPlayer`
- This one looks a bit trickier. I would like to discuss on it with you.
- For this one I tried to add network sync for multiplayer, and I want you to check if it makes sense.
- Now we need to properly implement `SendColorSelection` too, I'd like to know how you would do that.

### `PlayCardWithStrictFlow`
- Problem: PlayCardWithStrictFlow appears mostly mode-neutral but is called by multiplayer methods. It doesn't have direct AI calls but may trigger special effects that call AI methods.
- My Approach: This method seems mostly safe as-is, but we need to ensure the special effects it calls (HandleSpecialCardEffects) are properly protected with our centralized properties.
- I want you to trace and investigate this.

### `DrawCardWithStrictFlow`
- Problem: DrawCardWithStrictFlow appears mode-neutral and handles human player draws. It's used in both singleplayer and multiplayer contexts.
- My Approach: This method looks safe as-is since it only handles player (human) draws and doesn't call AI methods directly. However, it may trigger downstream effects that call AI methods.
- I want you to trace and investigate this.

## Tasks:
- Fix `OnPlayCardButtonClickedMultiplayer` 
- Fix `OnDrawCardButtonClickedMultiplayer`
- Implement `OnEndTurnButtonClickedMultiplayer`
- Implement `OnEndTakiSequenceButtonClickedMultiplayer`
- Implement `SendColorSelection`
- Methodically trace and read `HandlePostCardDrawTurnFlow`

## Tasks:
- Implement `SendColorSelection`
- Implement `SendEndTurn`
- Implement `SendEndTakiSequence`
- Methodically trace and read `HandlePostCardDrawTurnFlow`


### `HandleSpecialCardEffects`
- This one looks a bit trickier. I would like to discuss on it with you.
- I suspect this is not neutral for both modes, at least not entirely.
- It may also trigger downstream effects that call AI methods.
- I want you to trace and investigate this.

### `HandleStopCardEffect`
- This one looks a bit trickier. I would like to discuss on it with you.
- It looks to me like it's for singleplayer mode.
- Is there someplace else something to take care of multiplayer mode?
- Shouldn't this move be communicated to the network in some kind of way? Or something along those lines? Or you know what - maybe not, I'm unsure...

### `HandleChangeDirectionCardEffect`
- This one looks a bit trickier. I would like to discuss on it with you.
- It looks pretty neutral for both modes, but... is it truly?
- Should this move be communicated to the network?

### `HandleChangeColorCardEffect`
- This one looks a bit trickier. I would like to discuss on it with you.
- It looks pretty neutral for both modes, but... is it truly?
- Should this move be communicated to the network?

### `LogCardEffectRules`
- This one looks a bit trickier. I would like to discuss on it with you.
- It looks pretty neutral for both modes, but... is it truly?
- Should this move be communicated to the network?

### `ResetSpecialCardState`
- This one looks a bit trickier. I would like to discuss on it with you.
- It looks pretty neutral for both modes, but... is it truly?
- Should this move be communicated to the network?

### `HasPendingSpecialCardEffects`
- This method looks ok.

### `GetSpecialCardStateDescription`
- This method looks ok.

### `ProcessStopSkipEffect`- This one looks a bit trickier. I would like to discuss on it with you.
- It looks to me like it's for singleplayer mode.
- Is there someplace else something to take care of multiplayer mode?
- Shouldn't this move be communicated to the network in some kind of way? Or something along those lines?
- Maybe we should redirect it if in multiplayer mode and implement a new multiplayer method?

### `BreakPlusTwoChainByDrawing`
- This one looks a bit trickier. I would like to discuss on it with you.
- It looks to me like it's for singleplayer mode.
- Is there someplace else something to take care of multiplayer mode?
- Shouldn't this move be communicated to the network in some kind of way? Or something along those lines?
- Maybe we should redirect it if in multiplayer mode and implement a new multiplayer method?

### `GetTwoPlayerDirectionNote`
- This method looks ok.


### Tasks:
- Fix `HandleStopCardEffect`
- Fix `HandleChangeDirectionCardEffect`
- Fix `ProcessStopSkipEffect`
- Fix `BreakPlusTwoChainByDrawing`
- Investigate and fix `ResetSpecialCardState`
- `HandleSpecialCardEffects` - I still don't think this method is neutral. How could it be when we have lines like `PlayerType targetPlayer = currentPlayer == PlayerType.Human ? PlayerType.Computer : PlayerType.Human;`? That doesn't look very neutral to me. This is a very large method that needs special care.



### `OnComputerTurnReady`, `OnAICardSelected`, `OnAIDrawCard`, `OnAISequenceComplete`, `HandleAISpecialCardEffects`, `TriggerAIAdditionalAction`, `TriggerAISequenceDecision`
- For each of these methods, I want you to check if multiplayer needs some equivelent of them too, how does this look with singleplayer vs multiplayer



### `ProcessNetworkCardPlay`, `ProcessNetworkCardDraw`, `SendLocalCardPlayToNetwork`, `SendLocalCardDrawToNetwork`, `OnPlayCardButtonClickedMultiplayer`, `OnDrawCardButtonClickedMultiplayer`, `UpdateAllUIWithNetworkSupport`, `SynchronizeNetworkHandCounts`, `IsMultiplayerGameReady`, `GetNetworkGameStatus`
- Let's deep dive into each method - What singleplayer-mode method do they "replace"? Where/when/by who are they being called? are they complete and proper?



### `OnTurnStateChanged`, `OnInteractionStateChanged`, `OnGameStatusChanged`, `OnActiveColorChanged`, `OnTurnChanged`, `OnGameWon`, `OnPlayerTurnTimeOut`, `OnCardDrawnFromDeck`, `OnTakiSequenceStarted`, `OnTakiSequenceCardAdded`, `OnTakiSequenceEnded`
- Let's deep dive into each method - Some here look like they're neutral for both modes but some definitely aren't.

Investigate the questionable methods for multiplayer compatibility - make sure to trace them as needed as well
Need Investigation:
  3. OnTurnStateChanged - May trigger AI-specific logic
  4. OnGameStatusChanged - Should use network-safe UI updates
  5. OnTurnChanged - PlayerType semantics in multiplayer
  6. OnPlayerTurnTimeOut - Timeout handling in multiplayer
  7. OnTakiSequenceCardAdded - IsPlayerTurn meaning in multiplayer

### `UpdateAllUI` and `UpdateAllUIWithNetworkSupport`
- I would like to discuss on it with you.
- It looks to me like `UpdateAllUI` is for singleplayer mode, and `UpdateAllUIWithNetworkSupport` is for multiplayer mode. 
- We need to investigate who calls them and when/where, and if these calls are handled correctly (especially in regard to single/multi mode checking)

### `UpdateVisualHands`
- This one looks a bit trickier. I would like to discuss on it with you.
- It looks to me like it's for singleplayer mode.
- Is there someplace else something to take care of multiplayer mode?

### `RefreshPlayerHandStates`, `OnPlayerCardSelected`, `OnComputerCardSelected`
- I think these look neutral for both modes, please investigate and make sure

### The methods in `region External System Coordination`
- ok

### `RequestRestartGame`, `RequestRestartGameFromPause`, `RequestReturnToMenu`, `RequestExitConfirmation`, `RequestDrawCard`, `RequestPlayCard`, `GetPlayerHand`, `CanPlayerAct`
- I think these look neutral for both modes, please investigate and make sure



