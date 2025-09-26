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

## Current Status
✅ **Singleplayer**: Complete & Working
🔄 **Multiplayer**: Core Systems Complete - Special Cards In Progress
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
  - **Hand count synchronization: FIXED** ✅ Fixed double-counting bugs in network processing
- **Card types**:
  - **Basic number cards**: ✅ Working perfectly
  - **PlusTwo cards**: 🔄 Mostly working (chain logic functional, minor sync improvements pending)
  - **Special effect cards**: ⏳ Pending investigation

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
- [x] ✅ **Multiplayer testing** - Game functionality verified with strict turn flow

## Special Cards Multiplayer Investigation Todo

### 🎯 Next Priority: ChangeDirection
- [ ] **ChangeDirection cards**: Investigate multiplayer network synchronization
  - Check if direction changes are properly synced between clients
  - Verify turn order updates correctly after direction change
  - Test in combination with other special cards

### 📋 Special Cards Status & Todo List
- [x] ✅ **Basic Number Cards (1-9)**: Working perfectly in multiplayer
- [x] 🔄 **PlusTwo Cards**: Mostly functional (chain logic works, minor sync improvements completed)
- [ ] ⏳ **ChangeDirection Cards**: Pending investigation - next priority
- [ ] ⏳ **ChangeColor Cards**: Pending investigation
  - Color selection synchronization
  - UI color picker in multiplayer context
- [ ] ⏳ **Plus Cards**: Pending investigation
  - Additional turn logic in multiplayer
  - Turn flow synchronization
- [ ] ⏳ **Stop Cards**: Pending investigation
  - Skip turn logic in multiplayer
  - Turn advance synchronization
- [ ] ⏳ **TAKI Cards**: Pending investigation
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
