# CLAUDE.md

## Project Summary
Unity C# TAKI card game (similar to UNO) with both singleplayer (vs AI) and multiplayer (networked via Photon PUN2) support. Main scene: `Assets/Scenes/Scene_Menu.unity`. Build through Unity Editor.

## Working Style
- **Let me lead the conversation** - don't take actions without discussing first
- **Pass everything by me** before reading files or implementing changes
- **Be token-efficient** - don't read/search speculatively

## Information Files
- `MOST_RECENT_RUN_LOGS_SIMPLE.md`: Latest Unity console logs (clean format)
- `MOST_RECENT_RUN_LOGS_DITAILED.md`: Latest Unity console logs (full Console output - **ASK BEFORE READING, very token-heavy**)
- `investigating.md`: Detailed investigation notes and communication
- `Temp.md`: Temporary notes (ignore unless mentioned)

## Architecture & Scene Hierarchy

### Current Status
- **Singleplayer**: ✅ **Complete & Working** - Full TAKI game with all special cards, AI opponent, NEW UI ARCHITECTURE
- **Multiplayer**: ✅ **Architecture Complete** - All core systems implemented, multiplayer compatibility investigation complete

### UI Architecture (NEW - Fully Migrated)
- **BaseGameplayUIManager**: Abstract base class with Template Method pattern
- **SinglePlayerUIManager**: Human vs AI context (active for singleplayer)
- **MultiPlayerUIManager**: Local vs Remote network context (ready for multiplayer)
- **Legacy GameplayUIManager**: ❌ Removed - no longer used

### Scene Hierarchy
```
Scene_Menu
├── Canvas
│   ├── Screen_SinglePlayerGame - ✅ Fully functional
│   │   ├── Player1Panel (Human) - HandManager
│   │   ├── Player2Panel (Computer) - HandManager
│   │   ├── GameBoardPanel (Draw/Discard piles)
│   │   └── UI Controls (buttons, messages, etc.)
│   ├── Screen_MultiPlayerGame - 🎯 Development focus
│   │   ├── Player1Panel (Human) - HandManager
│   │   ├── Player2Panel (Human) - HandManager
│   │   └── Similar structure to singleplayer
│   └── [Other menu screens]
├── GameManager - Central coordinator with all components
├── DeckManager - Card deck management
└── MultiplayerMenuLogic - Room/matchmaking
```

## Key Scripts

### Core Systems
- **GameManager.cs**: Central coordinator, mode switching (single/multi)
- **GameStateManager.cs**: Rules engine, game state management
- **TurnManager.cs**: Turn flow orchestration
- **BasicComputerAI.cs**: AI opponent (disabled in multiplayer)

### UI System
- **BaseGameplayUIManager.cs**: Abstract base with Template Method pattern
- **SinglePlayerUIManager.cs**: Human vs AI context overrides
- **MultiPlayerUIManager.cs**: Network-aware context overrides
- **HandManager.cs**: Card visualization with network privacy

### Managers
- **DeckManager.cs**: Deck coordination with network support
- **Deck.cs**: Pure deck operations
- **PauseManager.cs**: Game state preservation
- **NetworkGameManager.cs**: Photon PUN2 integration

### Logging
- **TakiLogger.cs**: Centralized logging system
- Categories: TurnFlow, CardPlay, GameState, Network, etc.
- Use `TakiLogger.LogNetwork()`, `TakiLogger.LogGameState()`, etc.

## Progress Tracker
- ✅ New UI architecture fully implemented and working
- ✅ Singleplayer mode complete and stable
- ✅ Multiplayer core systems implemented
- ✅ Network synchronization architecture ready
- ✅ HandManager initialization timing issue RESOLVED
- ✅ Button event architecture FIXED - mode-aware activation/deactivation
- ✅ **Network card assignment bug ROOT CAUSE IDENTIFIED** - AI calls during multiplayer mode
- ✅ **Multiplayer compatibility investigation COMPLETE** - All methods analyzed for mode-awareness
- ✅ **UpdateVisualHands fixed** - Added mode-aware opponent hand display (no longer assumes AI exists)
- ✅ **UI update consistency** - UpdateAllUIWithNetworkSupport now calls UpdateAllDisplays and chain status
- ✅ **Game restart system** - GameEndManager and RequestRestartGameFromPause now mode-aware
- ✅ **Btn_Player1EndTakiSequence Network Fix** - Replaced flawed PlayerType logic with ActorNumber-based tracking

## Recent Changes Made

### Multiplayer Compatibility Fixes
- **HandManager.cs**: Added `GetRealCardsForGameLogic()` method for accessing opponent cards
- **GameManager.UpdateVisualHands()**: Made mode-aware, no longer assumes `computerAI` exists in multiplayer
- **GameManager.UpdateAllUIWithNetworkSupport()**: Added missing `UpdateAllDisplays()` call and chain status logic
- **GameEndManager.RestartGameSequence()**: Now calls appropriate start method based on `IsMultiplayerMode`
- **GameManager.RequestRestartGameFromPause()**: Blocks multiplayer access (pause will be removed from multiplayer)
- **Btn_Player1EndTakiSequence Network Logic**: Fixed button state synchronization using ActorNumber-based tracking

### Investigation Results
- **Mode-neutral methods**: `RefreshPlayerHandStates`, `OnPlayerCardSelected`, `OnComputerCardSelected`, request methods, etc.
- **Network methods**: Fixed inconsistent UI update calls to use `UpdateAllUIWithNetworkSupport()`

## Outstanding Issues
~~**Problem**: `playerHand` still gets 0 cards instead of 8 in multiplayer testing~~
**STATUS**: ✅ **RESOLVED** - Root cause identified and fixed!

### **Bug Resolution Summary:**
**Root Cause**: Reference equality bug in `NetworkGameManager.SetupLocalMultiplayerHands()`
- `gameManager.playerHand` and `myHand` pointed to the same List reference
- `gameManager.playerHand.Clear()` also emptied `myHand`
- `gameManager.playerHand.AddRange(myHand)` added from empty list → 0 cards

**Fix Applied**: Create defensive copy before clearing
```csharp
List<CardData> myHandCopy = new List<CardData>(myHand);
gameManager.playerHand.Clear();
gameManager.playerHand.AddRange(myHandCopy); // Safe!
```

**Result**: ✅ Players now get full 8-card hands, multiplayer fully functional!

### **TAKI Sequence Button Fix:**
**Problem**: `Btn_Player1EndTakiSequence` enabled on both clients when only sequence initiator should control it
**Root Cause**: Flawed PlayerType logic - both clients think they are "Human" in multiplayer

**Solution Applied**: Network-aware ActorNumber-based tracking
- **GameStateManager**: Added `takiSequenceInitiatorActorNumber` field to store network player ID
- **MultiPlayerUIManager**: Replaced PlayerType comparison with ActorNumber comparison
- **Reset compatibility**: Added ActorNumber reset in all cleanup methods
- **Inspector fix**: Set button default state to disabled

**Implementation**:
```csharp
// OLD (BROKEN): Both clients think they're "Human"
bool iInitiatedSequence = gameState.TakiSequenceInitiator == PlayerType.Human;

// NEW (FIXED): Compare actual network player IDs
int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
int sequenceInitiatorActor = gameState.TakiSequenceInitiatorActorNumber;
bool iInitiatedSequence = (sequenceInitiatorActor == myActorNumber);
```

**Result**: ✅ Button only enabled for the player who actually initiated the TAKI sequence

### **Network Serialization Fix:**
**Problem**: `Exception: Write failed. Custom type not found: TakiGame.NetworkMoveData` when clicking Play/Draw buttons
**Root Cause**: Photon PUN2 couldn't serialize the custom `NetworkMoveData` class over network

**Solution Applied**: Replace `NetworkMoveData` with `ExitGames.Client.Photon.Hashtable`
- **NetworkGameManager.cs**: Updated all Send methods (SendCardPlay, SendCardDraw, etc.) to use Hashtable
- **ProcessRemoteAction**: Updated to receive and parse Hashtable data instead of NetworkMoveData
- **Project pattern**: Already used Hashtable in `MultiplayerMenuLogic.cs` for room properties

**Implementation**:
```csharp
// OLD (BROKEN): Custom type not registered with Photon
var moveData = new NetworkMoveData {
    actionType = "PLAY_CARD",
    cardIdentifier = cardId
};

// NEW (WORKING): Native Photon serialization
var moveData = new ExitGames.Client.Photon.Hashtable {
    {"actionType", "PLAY_CARD"},
    {"cardIdentifier", cardId}
};
```

**Result**: ✅ Network card play/draw actions now work without serialization errors!

### **Opponent Hand Display Synchronization Fix:**
**Problem**: Opponent hand count would desynchronize during gameplay (8→7→8 instead of staying at 7)
**Root Cause**: Mixed system using both `ShowOpponentHandAsCardBacks()` (null cards) and `ShowOpponentHandWithPrivacy()` (real cards), with stale opponent card data not synchronized during play/draw actions

**Solution Applied**: Clean architecture with consistent real card tracking
- **Eliminated deprecated method**: All `ShowOpponentHandAsCardBacks()` calls replaced with `ShowOpponentHandWithPrivacy()`
- **Real card synchronization**: `ProcessNetworkCardPlay()` now removes actual played cards from opponent `currentHand`
- **Placeholder card system**: `ProcessNetworkCardDraw()` adds placeholder cards to maintain correct opponent card count
- **Consistent display**: All opponent displays now use synchronized real card data with privacy mode

**Key Changes**:
```csharp
// OLD (BROKEN): Mixed system with stale data
ShowOpponentHandAsCardBacks(count); // Uses null cards, causes blank display
ShowOpponentHandWithPrivacy(staleCards); // Uses original 8 cards even after opponent plays

// NEW (FIXED): Consistent real card system
ShowOpponentHandWithPrivacy(currentHand); // Always uses current synchronized cards
// Plus: RemoveCardEnhanced() on play, AddCardEnhanced() on draw
```

**Result**:
- ✅ **Opponent count synchronization**: 8→7→8 stays consistent throughout game
- ✅ **Proper card back sprites**: Real cards with privacy mode display correctly
- ✅ **No blank white cards**: Eliminated null-based display system
- ✅ **Clean architecture**: Single method approach with real card data

### **Draw Pile Synchronization Fix:**
**Problem**: Draw pile count would desynchronize when opponent draws cards - client shows "Draw: 93" while master shows "Draw: 92"
**Root Cause**: `ProcessNetworkCardDraw()` updated opponent hand count but never decremented the draw pile count on the client side

**Solution Applied**: Added draw pile count decrement in network card draw processing
- **DeckManager.cs**: Added `DecrementDrawPileCount(int count)` method for network synchronization
- **GameManager.ProcessNetworkCardDraw()**: Added draw pile decrement for both normal draws and PlusTwo chain breaks
- **Proper bounds checking**: Uses `Mathf.Max(0, currentCount - count)` to prevent negative counts
- **Consistent UI updates**: Updates deck display after decrementing count

**Implementation**:
```csharp
// NEW: Draw pile synchronization in ProcessNetworkCardDraw()
if (deckManager != null) {
    deckManager.DecrementDrawPileCount(1); // Normal draw
    // OR
    deckManager.DecrementDrawPileCount(cardsToDraw); // Chain break
    TakiLogger.LogNetwork("Draw pile count decremented for network sync");
}
```

**Result**: ✅ **Draw pile synchronization**: Both clients show consistent draw pile counts (93→92) when opponent draws cards

### **Client Draw/Play Issue Fix:**
**Problem**: Client could not draw or play cards - received "Cannot draw card: Game not active or not player turn" even when it was client's turn
**Root Cause**: `isGameActive` field was never set to `true` in multiplayer mode, causing validation `if (!isGameActive || !gameState.CanPlayerAct())` to fail

**Solution Applied**: Added proper game activation after multiplayer initialization
- **GameManager.cs**: Added `SetGameActive(bool active)` method for network game activation
- **NetworkGameManager.SetupLocalMultiplayerHands()**: Call `gameManager.SetGameActive(true)` after successful hands setup
- **Validation fix**: Client now passes game active validation checks

**Debug Process**: Added temporary logging to isolate the exact failing condition:
```csharp
// Debug revealed: isGameActive=False, but CanPlayerAct=True
TakiLogger.LogNetwork($"DRAW VALIDATION DEBUG: isGameActive={gameActiveCheck}, CanPlayerAct={canPlayerActCheck}");
TakiLogger.LogNetwork($"DRAW VALIDATION DEBUG: gameStatus={gameState.gameStatus}, turnState={gameState.turnState}, interactionState={gameState.interactionState}");
```

**Implementation**:
```csharp
// NEW: Proper multiplayer game activation
// In NetworkGameManager.SetupLocalMultiplayerHands():
if (gameManager != null) {
    gameManager.SetGameActive(true);
    TakiLogger.LogNetwork("Game activated after multiplayer hands setup");
}
```

**Result**: ✅ **Client card actions**: Both master and client can now draw and play basic cards successfully

### **Strict Button Flow Fix:**
**Problem**: Actions like PLAY/DRAW would automatically advance turns without requiring END TURN button press, breaking special card effects flow.
**Root Cause**: All network actions (`SendCardPlay`, `SendCardDraw`, `SendEndTurn`) called `SendMove(data, true)` which marked turns as finished, auto-advancing to opponent.

**Solution Applied**: Multi-part fix to enforce strict button flow
- **NetworkGameManager.cs**: Changed non-ending actions to use `SendMove(data, false)`:
  - `SendCardPlay()` → `SendMove(data, false)`
  - `SendCardDraw()` → `SendMove(data, false)`
  - `SendColorSelection()` → `SendMove(data, false)`
  - `SendEndTurn()` → Still uses `SendMove(data, true)` ✅
- **TurnManager.cs**: Added multiplayer mode awareness to prevent computer AI scheduling:
  - Added `isMultiplayerMode` field and `SetMultiplayerMode(bool)` method
  - Modified `StartTurn()` to only schedule computer turns in singleplayer mode
- **GameManager.cs**: Configure TurnManager mode in both game mode initializations
- **NetworkGameManager.cs**: Implemented `OnPlayerMove()` to handle non-finishing moves (was empty)

**Implementation**:
```csharp
// Network flow after fix:
// 1. Card Play: SendMove(PLAY_CARD, false) → EvMove → OnPlayerMove → Process action, no turn advance
// 2. End Turn: SendMove(END_TURN, true) → EvFinalMove → OnPlayerFinished → Process action + advance turn
```

**Result**: ✅ **Strict Button Flow**: Players MUST press END TURN to advance turns, essential for special effect cards

### **Player/Opponent Message System Fix:**
**Problem**: Messages showed in Inspector but not on screen - multiple issues including UI manager switching, timing conflicts, and transparency
**Root Causes**:
1. **UI Manager Mode Switching Bug** - Wrong UI manager active in multiplayer
2. **Unity Start() Timing Override** - Start() method disabling correctly activated UI managers
3. **Alpha Transparency Issue** - TextMeshPro components had alpha=0
4. **Competing Message Calls** - Multiple systems overwriting messages

**Solution Applied**:
- **UI Manager Activation Tracking** - Added `explicitlyActivatedByGameManager` flag to prevent Start() override
- **Proper Disconnection Chain** - Added `DisconnectUIManagerEvents()` to prevent duplicate handlers
- **Message Duration Standardization** - All special card messages now show for 10 seconds
- **Fixed Message Routing** - Replaced competing message calls with proper multiplayer-aware routing

**Implementation**:
```csharp
// GameManager mode switching (lines 590, 643)
ConnectActiveUIManagerEvents(); // Called on every mode change

// BaseGameplayUIManager activation protection
explicitlyActivatedByGameManager = true; // Prevents Start() override
this.enabled = true; // Ensures component stays enabled

// Proper message routing in HandleChangeDirectionCardEffectMultiplayer()
bool iPlayedTheCard = networkGameManager.IsMyTurn;
if (iPlayedTheCard) {
    GetActiveUI()?.ShowPlayerMessageTimed($"You played ChangeDirection: {oldDirection} → {newDirection}", 10.0f);
} else {
    GetActiveUI()?.ShowOpponentMessageTimed($"Opponent played ChangeDirection: {oldDirection} → {newDirection}", 10.0f);
}
```

**Result**: ✅ **Player/Opponent Messages**: Fully functional - messages display correctly on screen with proper perspective and timing

### **STOP Card Turn Skipping Fix:**
**Problem**: STOP cards showed visual messages but turn continued normally instead of being skipped
**Root Cause**: `ProcessStopSkipEffectMultiplayer()` cleared flags and showed messages but didn't implement actual turn skipping logic - incorrectly assumed PunTurnManager would handle it

**Solution Applied**: Implemented correct turn skipping flow matching singleplayer pattern
- **`StartPlayerTurnFlow()` STOP check**: Added STOP flag detection at start of turn (before PlusTwo chains or other logic)
- **`ProcessNetworkStopEffect()` flag setting**: Set `shouldSkipNextTurn = true` when receiving opponent's STOP effect
- **`EndTurnAfterStopSkip()` immediate skip**: Skip flagged turn immediately and advance to opponent

**Correct Flow Implementation**:
1. Player A plays STOP → END TURN
2. Turn advances normally to Player B via PunTurnManager
3. Player B's turn starts → `StartPlayerTurnFlow()` detects STOP flag first
4. Player B's turn skipped immediately (no buttons enabled)
5. Turn automatically advances back to Player A

**Implementation**:
```csharp
// In StartPlayerTurnFlow() - STOP check FIRST
if (isMultiplayerMode && shouldSkipNextTurn) {
    shouldSkipNextTurn = false;
    GetActiveUI()?.ShowPlayerMessageTimed("STOP effect: Your turn is skipped!", averageWaitingTime);
    Invoke(nameof(EndTurnAfterStopSkip), 0.5f);
    return; // Exit early - no normal turn processing
}

// In ProcessNetworkStopEffect() - Set skip flag
shouldSkipNextTurn = true;
TakiLogger.LogNetwork("STOP skip flag set - local player's next turn will be skipped");
```

**Result**: ✅ **STOP Cards**: Turn skipping works perfectly - opponent's turn is actually skipped, not just visually

### **Multiplayer Re-entry Issue Fix:**
**Problem**: After going home from multiplayer, clicking "Play Multiplayer" again would fail - players remained in previous room causing instant game start without proper matchmaking
**Root Cause**: Players stayed connected to Photon room after game end instead of leaving room and starting fresh matchmaking

**Solution Applied**: NetworkCleanupManager pattern following established multiplayer script architecture
- **NetworkCleanupManager.cs**: New dedicated network cleanup script with proper Photon callbacks
- **GameEndManager.cs**: Integration with NetworkCleanupManager for multiplayer mode cleanup
- **MultiplayerMenuLogic.cs**: Enhanced Btn_PlayMultiPlayer() to handle already-in-room state with OnLeftRoom() callback

**Implementation**:
```csharp
// GameEndManager - CleanupGameState() method
if (gameManager != null && gameManager.IsMultiplayerMode && networkCleanup != null) {
    TakiLogger.LogSystem("Initiating network cleanup for multiplayer mode");
    networkCleanup.OnGoingHome();
}

// MultiplayerMenuLogic - Btn_PlayMultiPlayer() method
if (PhotonNetwork.InRoom) {
    UpdateStatus("Leaving previous room...");
    PhotonNetwork.LeaveRoom(); // OnLeftRoom callback continues to fresh matchmaking
}
```

**Result**: ✅ **Multiplayer Re-entry**: Players properly leave rooms on menu return, fresh matchmaking works perfectly

### **Local Player Win Detection Fix:**
**Problem**: `HandleLocalPlayerWin()` method was never called in multiplayer - win detection always used singleplayer logic
**Root Cause**: `PlayCardWithStrictFlow()` win check always called `gameState.DeclareWinner(PlayerType.Human)` regardless of game mode

**Solution Applied**: Mode-aware win condition handling
- **GameManager.cs**: Updated `PlayCardWithStrictFlow()` win detection to check `isMultiplayerMode`
- **Multiplayer mode**: Call `HandleLocalPlayerWin()` for proper network logging context
- **Singleplayer mode**: Call `gameState.DeclareWinner(PlayerType.Human)` directly

**Implementation**:
```csharp
// MODE AWARE: Handle win condition properly for both game modes
if (isMultiplayerMode) {
    HandleLocalPlayerWin();
} else {
    gameState.DeclareWinner(PlayerType.Human);
}
```

**Result**: ✅ **Local Player Win**: `HandleLocalPlayerWin()` now properly called with correct multiplayer logging context

### **Singleplayer-to-Multiplayer Mode Transition Fix:**
**Problem**: Major bug when playing singleplayer first, then multiplayer - master had no cards, discard pile not visible, wrong UI manager active
**Root Causes**: Multiple complex issues with mode transitions
1. **Photon disconnection bug**: `MenuNavigation.cs` disconnected from Photon even from singleplayer mode
2. **Reference contamination**: `gameManager.playerHand` and `myHand` shared same List reference, causing card assignment failures
3. **System initialization flag sharing**: `areSystemsInitialized` flag shared between modes, causing multiplayer initialization to be skipped
4. **Discard pile recreation**: `PileManager` mode change recreation didn't properly restore discard pile visuals

**Solution Applied**: Comprehensive multi-component fix
- **MenuNavigation.cs**: Fixed Photon disconnection to be mode-aware - only disconnect from multiplayer mode
- **GameManager.cs**: Added defensive copy creation in multiplayer mode and system flag reset
- **NetworkGameManager.cs**: Fixed reference contamination with consistent use of defensive copies
- **PileManager.cs**: Fixed `RecreateVisualsForModeChange()` to properly restore discard pile during mode transitions

**Key Implementations**:
```csharp
// MenuNavigation - Mode-aware Photon disconnection
if (multiplayerMenuLogic != null && multiplayerMenuLogic.IsConnectedToPhoton &&
    gameManager != null && gameManager.IsMultiplayerMode) {
    multiplayerMenuLogic.DisconnectFromPhoton();
} else {
    TakiLogger.LogInfo("Staying connected to Photon (was in singleplayer mode)");
}

// GameManager - Defensive copy and system reset
if (isMultiplayerMode) {
    playerHand = new List<CardData>(player1Hand);  // Defensive copy
    areSystemsInitialized = false;  // Reset for proper multiplayer initialization
}

// PileManager - Proper discard pile recreation
void RecreateVisualsForModeChange() {
    bool hadDiscardPile = discardPileCardController != null;
    CardData previousTopDiscard = discardPileCardController?.CardData;

    ClearPileVisuals();
    CreateDrawPileVisual();

    if (hadDiscardPile || previousTopDiscard != null) {
        CreateDiscardPileVisual();
        if (previousTopDiscard != null) {
            UpdateDiscardPileDisplay(previousTopDiscard);
        }
    }
}
```

**Result**: ✅ **Seamless Mode Transitions**: Play singleplayer → Go Home → Play multiplayer works perfectly with:
- ✅ Photon connection maintained between modes
- ✅ Full 8-card assignment to both players
- ✅ Proper UI manager activation (MultiPlayerUIManager)
- ✅ Discard pile correctly visible with starting card
- ✅ Complete game functionality ready for play

## Current Status
✅ **Singleplayer**: Complete & Working
✅ **Multiplayer**: **FULLY COMPLETE & WORKING** - ALL MAJOR ISSUES RESOLVED
- **Core multiplayer functionality**: ✅ Complete
  - Card assignment: 8/8 cards ✅
  - UI display: Correct hand counts ✅
  - Game logic: Card validation & turns ✅
  - Network sync: RPC communication ✅
  - **Network serialization: FIXED** ✅
  - **Opponent hand display: FIXED** ✅ Consistent count synchronization with proper card back sprites
  - **Draw pile synchronization: FIXED** ✅ Draw pile counts stay synchronized across clients
  - **Client card actions: FIXED** ✅ Both master and client can draw and play basic cards
  - **Strict button flow: FIXED** ✅ Actions don't auto-advance turns, END TURN required
  - **Player/Opponent message system: FIXED** ✅ Messages display correctly with proper routing and timing
  - **Hand count synchronization: FIXED** ✅ Fixed double-counting bugs in network processing
  - **Multiplayer re-entry: FIXED** ✅ Room cleanup on menu return, fresh matchmaking works
  - **Local player win detection: FIXED** ✅ HandleLocalPlayerWin() now properly called in multiplayer
  - **Singleplayer-to-multiplayer mode transitions: FIXED** ✅ Complete seamless transition support
- **Card types**:
  - **Basic number cards**: ✅ Working perfectly
  - **ChangeDirection cards**: ✅ Working perfectly (network sync and messages confirmed)
  - **STOP cards**: ✅ Working perfectly (turn skipping logic implemented correctly)
  - **PLUS cards**: ✅ Working perfectly (additional action flow confirmed in multiplayer)
  - **ChangeColor cards**: ✅ **CONFIRMED WORKING** (color selection synchronization perfect)
  - **PlusTwo cards**: ❌ **BROKEN** - Hand synchronization issues identified
  - **TAKI cards**: ⏳ **Investigation pending**

## Todo List
- [x] ✅ Multiplayer compatibility investigation - All methods analyzed and fixed
- [x] ✅ UI architecture consistency - UpdateAllUIWithNetworkSupport matches UpdateAllUI
- [x] ✅ Game restart system - Mode-aware restart for both singleplayer and multiplayer
- [x] ✅ UpdateVisualHands fix - No longer assumes AI exists in multiplayer
- [x] ✅ **FIXED: Multiplayer card assignment bug** - Reference equality issue resolved
- [x] ✅ **FIXED: Btn_Player1EndTakiSequence state synchronization** - ActorNumber-based network logic
- [x] ✅ **FIXED: Network serialization bug** - NetworkMoveData → Hashtable conversion
- [x] ✅ **FIXED: Opponent hand display synchronization** - Real card data synchronization with consistent display architecture
- [x] ✅ **FIXED: Draw pile synchronization** - Added DecrementDrawPileCount() for network card draw processing
- [x] ✅ **FIXED: Client draw/play issue** - Added SetGameActive() method and proper multiplayer game activation
- [x] ✅ **FIXED: Strict button flow** - Actions no longer auto-advance turns, END TURN required for special cards
- [x] ✅ **FIXED: OnComputerTurnReady multiplayer error** - TurnManager now multiplayer-aware
- [x] ✅ **FIXED: Card play sync missing** - Implemented OnPlayerMove() for non-finishing network actions
- [x] ✅ **FIXED: Hand count synchronization** - Fixed double-counting bugs in ProcessNetworkCardPlay/Draw
- [x] ✅ **FIXED: PlusTwo chain break sync** - Enhanced ProcessNetworkChainBreak with proper opponent count updates
- [x] ✅ **FIXED: STOP card turn skipping** - Implemented correct turn skipping flow matching singleplayer pattern
- [x] ✅ **FIXED: Multiplayer re-entry issue** - NetworkCleanupManager pattern with room cleanup on menu return
- [x] ✅ **FIXED: Local player win detection** - Mode-aware win condition handling in PlayCardWithStrictFlow()
- [x] ✅ **FIXED: Singleplayer-to-multiplayer mode transitions** - Comprehensive fix for Photon connection, reference contamination, system initialization, and discard pile recreation
- [x] ✅ **Multiplayer testing** - Game functionality verified with strict turn flow and clean re-entry

## Special Cards Multiplayer Investigation Todo

### 🎯 Next Priority: PlusTwo Cards - Hand Synchronization Fix
- [ ] **PlusTwo cards**: Fix hand synchronization issues in chain scenarios
  - Deep comparison with singleplayer PlusTwo implementation
  - Investigate why card draw amounts differ between clients
  - Fix chain break hand count synchronization
  - Ensure consistent draw pile decrements across clients

### 📋 Special Cards Status & Todo List
- [x] ✅ **Basic Number Cards (1-9)**: Working perfectly in multiplayer
- [x] ✅ **ChangeDirection Cards**: Working perfectly (network sync and messages confirmed)
- [x] ✅ **STOP Cards**: Working perfectly (turn skipping logic implemented correctly)
- [x] ✅ **PLUS Cards**: Working perfectly (additional action flow confirmed in multiplayer)
- [x] ✅ **ChangeColor Cards**: CONFIRMED WORKING (color selection synchronization perfect)
- [ ] ❌ **PlusTwo Cards**: BROKEN - Hand synchronization issues identified
  - Chain logic functional but hand count synchronization broken
  - Incorrect card draw amounts between clients
  - Need deep comparison with singleplayer mode
- [ ] ⏳ **TAKI Cards**: Investigation pending
  - Sequence initiation in multiplayer
  - Sequence end button synchronization (partially fixed)
  - Color selection during TAKI sequences

### 🔍 Investigation Approach
For each special card type:
1. **Trace network flow** - Check if proper RPCs are sent/received
2. **Verify state synchronization** - Ensure both clients maintain identical game state
3. **Test UI synchronization** - Confirm visual elements update consistently
4. **Validate turn flow** - Check strict button flow compliance
5. **Test edge cases** - Special card combinations and error scenarios

## Side Notes
- **No Unicode**: Avoid special characters in code/files

---

# Next Thread Starting Prompt

Summary for Next Thread: PlusTwo Card Hand Synchronization Fix

Context: STOP, ChangeDirection, PLUS, and ChangeColor cards have been FULLY RESOLVED and confirmed working perfectly in multiplayer. PlusTwo cards have BROKEN hand synchronization - the chain logic works but hand counts desynchronize between clients.

**Key Issue Identified from Logs Analysis:**
- **Chain logic works**: PlusTwo chains initiate and continue correctly
- **Hand synchronization broken**: Card draw amounts differ between clients
- **Suspected problem**: Player who draws cards gets correct amount, but opponent assumes different amount
- **Evidence**: Lines 464-469 show opponent drawing 4 cards but later lines 453-469 show confused synchronization

Key Learnings & Patterns from Previous Fixes:
1. **Always Study Singleplayer Logic First**: Check how PlusTwo chains work in singleplayer mode to understand expected multiplayer behavior
2. **Network Message Routing Pattern**: Use `networkGameManager.IsMyTurn` for "You/Opponent" perspective, NOT PlayerType
3. **UI Manager Activation**: Always verify correct UI manager is active via `GetActiveUI()` - don't assume based on game mode
4. **Turn Flow Integration**: Check where/how singleplayer logic fits into `StartPlayerTurnFlow()` and `HandlePostCardPlayTurnFlow()`
5. **Hand Count Synchronization**: Both clients must maintain identical hand counts through network operations

Technical Implementation Patterns Established:
```csharp
// ✅ CORRECT: Network-aware message routing
bool iPlayedTheCard = networkGameManager.IsMyTurn;
if (iPlayedTheCard) {
    GetActiveUI()?.ShowPlayerMessageTimed($"You played {cardName}", 10.0f);
} else {
    GetActiveUI()?.ShowOpponentMessageTimed($"Opponent played {cardName}", 10.0f);
}

// ✅ CORRECT: Hand count synchronization pattern
// Always update both local hand AND notify network of count changes
ProcessNetworkCardDraw() {
    // Update actual hand data
    // Update hand display count
    // Decrement draw pile consistently
    // Send network notification of count change
}
```

Critical Files for PlusTwo Card Investigation:
- GameManager.cs - Look for `HandlePlusTwoCardEffect()` and PlusTwo chain processing
- NetworkGameManager.cs - Check `ProcessNetworkCardDraw()`, `ProcessNetworkChainBreak()`, and PlusTwo RPCs
- GameStateManager.cs - PlusTwo chain state management (interaction states)
- SinglePlayerUIManager.cs - How PlusTwo chains work in singleplayer (reference implementation)

**Specific Investigation Points:**
1. **Compare singleplayer vs multiplayer PlusTwo logic** - What's different in chain break handling?
2. **Trace hand count updates during chain breaks** - Where do the counts desynchronize?
3. **Verify draw pile decrements** - Are both clients decrementing correctly?
4. **Check placeholder card logic** - Are the right number of placeholder cards added?

**Evidence from Logs to Investigate:**
- Line 464: "Opponent breaking chain: 2 PlusTwo cards, drawing 4 cards"
- Line 469: "Added 2 placeholder cards for chain break" (Should be 4 cards?)
- Lines 452-469: Confusing double processing of chain break

Investigation Approach:
1. **Study singleplayer PlusTwo chain implementation** - How do chain breaks work normally?
2. **Trace multiplayer PlusTwo chain break flow** - Follow the network messages step by step
3. **Identify the hand synchronization bug** - Where do the counts diverge?
4. **Fix the synchronization logic** using established patterns
5. **Test chain scenarios** - Single PlusTwo, multiple PlusTwo chains, chain breaks

Next Task: Fix PlusTwo card hand synchronization issues in multiplayer mode - focus on chain break logic and hand count consistency.

● Ready for PlusTwo card synchronization fix! Use the established patterns and deep comparison approach with singleplayer mode.
