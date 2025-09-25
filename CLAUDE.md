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

## Current Status
✅ **Singleplayer**: Complete & Working
✅ **Multiplayer**: Complete & Working - All systems operational
- Card assignment: 8/8 cards ✅
- UI display: Correct hand counts ✅
- Game logic: Card validation & turns ✅
- Network sync: RPC communication ✅
- **Network serialization: FIXED** ✅
- **Opponent hand display: FIXED** ✅ Consistent count synchronization with proper card back sprites

## Todo List
- [x] ✅ Multiplayer compatibility investigation - All methods analyzed and fixed
- [x] ✅ UI architecture consistency - UpdateAllUIWithNetworkSupport matches UpdateAllUI
- [x] ✅ Game restart system - Mode-aware restart for both singleplayer and multiplayer
- [x] ✅ UpdateVisualHands fix - No longer assumes AI exists in multiplayer
- [x] ✅ **FIXED: Multiplayer card assignment bug** - Reference equality issue resolved
- [x] ✅ **FIXED: Btn_Player1EndTakiSequence state synchronization** - ActorNumber-based network logic
- [x] ✅ **FIXED: Network serialization bug** - NetworkMoveData → Hashtable conversion
- [x] ✅ **FIXED: Opponent hand display synchronization** - Real card data synchronization with consistent display architecture
- [x] ✅ **Multiplayer testing** - Game functionality verified

## Side Notes
- **No Unicode**: Avoid special characters in code/files
