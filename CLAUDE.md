# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

This is a Unity C# project using Photon PUN2 for multiplayer networking. Build through Unity Editor:
- Open the project in Unity Editor
- Main scene: `Assets/Scenes/Scene_Menu.unity`
- For multiplayer testing: Build standalone executable while keeping Unity Editor open for dual-client testing

## Project Architecture

### Game Structure
This is a TAKI card game (similar to UNO) with both singleplayer and multiplayer support:

**Current Status:**
- **Singleplayer**: ✅ Complete - Full TAKI game with all special cards, AI opponent, pause system
- **Multiplayer**: 🎯 In Progress - Phase 2 (deck initialization and core mechanics)

### Core Architecture Pattern
Event-driven architecture with single responsibility pattern:

```
Scripts/Core/
├── GameManager.cs - Central coordinator, handles both single/multiplayer modes
├── GameStateManager.cs - Rules engine with multi-enum architecture  
├── TurnManager.cs - Turn orchestration
└── BasicComputerAI.cs - Strategic AI (disabled in multiplayer)

Scripts/UI/
├── GameplayUIManager.cs - Strict button control system
├── HandManager.cs - Visual card system with network privacy
├── MenuNavigation.cs - Menu system with multiplayer integration

Scripts/Managers/
├── DeckManager.cs - Deck coordination with network support
├── Deck.cs - Pure deck operations
├── PauseManager.cs - State preservation
└── [8+ other specialized managers]

Scripts/Multiplayer/
├── NetworkGameManager.cs - Photon PUN2 integration with turn management
└── MultiplayerMenuLogic.cs - Room creation and matchmaking
```

### Network Architecture
Uses Photon PUN2 following instructor's proven pattern:
- **Master/Client Pattern**: Master initializes deck state, broadcasts to clients
- **PunTurnManager**: Handles turn-based gameplay
- **State Synchronization**: Deck state, hand privacy, turn coordination
- **Room Management**: Automatic room creation/joining with TAKI-specific configuration

### Key Components Integration
- `GameManager` switches between singleplayer/multiplayer modes
- `DeckManager` has network-aware methods for multiplayer coordination
- `HandManager` includes opponent privacy system (cards as backs with count)
- `NetworkGameManager` implements `IPunTurnManagerCallbacks` for turn management

### Logging System
Centralized logging through `TakiLogger.cs`:
- Categorized logs: TurnFlow, CardPlay, GameState, Network, etc.
- Configurable log levels: Error, Warning, Info, Debug, Verbose
- Use `TakiLogger.LogNetwork()`, `TakiLogger.LogGameState()`, etc.

### Special Considerations
- **No Unicode**: Avoid special characters in code, file names, or text displays
- **Network Privacy**: Own hand face-up, opponent hand as card backs with count
- **State Preservation**: All singleplayer functionality remains intact
- **Turn Management**: Strict one-action-per-turn flow (PLAY or DRAW then END TURN)

### Development Focus
**Phase 2 Milestone 1**: ✅ **COMPLETED** - Deck initialization issues identified and per-screen architecture implemented

## Recent Progress (2025-01-14)

### ✅ MAJOR BUG FIXES - Hand Assignment & Double Initialization

#### **Critical Bug #1: Hand Clearing After Network Assignment**
**Problem**: `[NET] Hand assignment: Local=8 cards` → `[NET] GameManager playerHand updated: 0 cards`
**Root Cause**: `GameManager.ResetGameSystems()` always cleared `playerHand`, even in multiplayer mode
**Solution**: ✅ **Mode-Aware Reset System**
- Modified `ResetGameSystems()` to only clear hands in single-player mode
- Added unified `StartNewMultiPlayerGame()` method matching single-player structure
- Updated `MenuNavigation` to use proper game start entry points

#### **Critical Bug #2: Double Game Initialization**
**Problem**: `"[STATE] New game initialized successfully"` appearing twice
**Root Cause**: Multiplayer called `SetupInitialGame()` → `InitializeNewGame()` multiple times
**Solution**: ✅ **Safety Flag System**
- Added `_isGameInitialized` flag in `GameSetupManager`
- Prevents duplicate initialization with early exit and warning
- Auto-reset integration in `ResetGameSystems()` for new games

### ✅ Unified Game Start Architecture
**Before**: Inconsistent initialization between single-player and multiplayer
**After**: Clean, unified structure for both modes:

```csharp
// Both modes now follow same pattern:
public void StartNewSinglePlayerGame() {
    Initialize → Reset(mode-aware) → Setup
}

public void StartNewMultiPlayerGame() {
    Initialize → Reset(mode-aware) → StartNetwork
}
```

### ✅ Root Cause Analysis - Original Deck Issues
**Problem Identified**: Empty hands and deck piles in multiplayer mode
**Root Cause**: `gameSetup` component reference null in `DeckManager.SetupInitialGame()`
- When null, returns empty hands → serialized as empty strings → transmitted over network → results in 0 cards
- Enhanced logging added to identify missing component assignments

### ✅ Per-Screen Architecture Implementation
**Challenge**: Single UI references couldn't handle both singleplayer and multiplayer screens
**Solution**: Mode-aware UI element selection with backwards compatibility

**DeckUIManager Enhanced**:
- Added per-screen UI references (singleplayer/multiplayer variants)
- Mode detection using `GameManager.IsMultiplayerMode`
- Automatic UI element selection based on game mode
- Backwards compatible with existing Inspector assignments

**PileManager Enhanced**:
- Added per-screen container references
- Mode-aware pile visual creation
- Same functionality, different target containers per screen

### 📋 Inspector Assignment Requirements
**Critical for Multiplayer Mode** - Assign these in Unity Inspector:

**DeckUIManager Component**:
```
Multiplayer UI References:
├── Multi Player Draw Pile Count Text
├── Multi Player Discard Pile Count Text
├── Multi Player Deck Message Text
├── Multi Player Draw Pile Panel
└── Multi Player Discard Pile Panel
```

**PileManager Component**:
```
Multiplayer Pile Containers:
├── Multi Player Draw Pile Container
└── Multi Player Discard Pile Container
```

### 🎯 Current Status
- **Singleplayer**: ✅ **Complete & Stable** - Full TAKI game with all special cards, AI opponent, pause system
- **Multiplayer**: ⚠️ **Architecture Issues Discovered** - Core initialization bugs fixed, but UI architecture incomplete
  - ✅ Hand assignment no longer cleared after network setup
  - ✅ Double initialization prevented with safety system
  - ✅ Unified game start architecture implemented
  - ✅ Per-screen UI architecture ready
  - ✅ Network synchronization logic functional
  - ⚠️ **UI Architecture Migration Incomplete** - Missing base contracts causing compilation errors

## Recent Investigation (2025-01-14 - UI Architecture Warnings)

### 🔍 **PROBLEM IDENTIFIED**: Duplicate UI Manager Button Event Handlers

**Issue**: Singleplayer shows warnings like `"[TURN] PLAY CARD clicked but button should be disabled!"` even though **all functionality works perfectly**.

**Root Cause Discovered**: **Dual UI Architecture** creates duplicate button event handlers:

#### **UI Architecture Status**
The project has **two UI architectures** running simultaneously:

1. **New Architecture** ✅ **Intended for production**:
   - `BaseGameplayUIManager` (abstract base)
   - `SinglePlayerUIManager : BaseGameplayUIManager` (singleplayer)
   - `MultiPlayerUIManager : BaseGameplayUIManager` (multiplayer)
   - Accessed via `GameManager.GetActiveUI()`

2. **Legacy Architecture** ⚠️ **Fallback system**:
   - `GameplayUIManager : MonoBehaviour` (original implementation)
   - Direct GameObject assignment in scene

#### **Investigation Findings**
- **Both** UI managers connect button event handlers in their `Start()` methods
- **Both** check internal state variables (`playButtonEnabled`, `drawButtonEnabled`, etc.)
- When button clicked → **two handlers execute** → second handler may see different state → false warnings
- **Functionality works** because the actual game logic processes correctly
- **Warnings appear** due to timing differences between duplicate handlers

#### **Current Status**
- `useNewUIArchitecture = true` in GameManager
- New architecture **declared** and assigned in Inspector
- If `singlePlayerUI == null` → `GetActiveUI()` returns `null` → systems fall back to legacy manager
- **Need to verify**: Which UI manager is actually active in singleplayer scene

### 🛠️ **Immediate Fix Applied**
Modified legacy `GameplayUIManager` button handlers to check **actual button state** (`button.interactable`) instead of internal tracking variables (`playButtonEnabled`). This eliminates false warnings while maintaining functionality.

### ✅ **PROBLEM RESOLVED** (2025-01-14)

**Investigation Results**:
- **Legacy `GameplayUIManager`**: ✅ **Active and working perfectly** - handles all singleplayer functionality
- **New `SinglePlayerUIManager`**: ❌ **Was causing duplicate handlers** - enabled but not properly integrated
- **Root Issue**: Both UI managers connected button listeners, creating conflicting state checks

**Solution Applied**:
- **Disabled `SinglePlayerUIManager` GameObject** in Unity Inspector
- **Kept `GameplayUIManager` active** for singleplayer functionality
- **Result**: ✅ Perfect button functionality without false warnings

**Architecture Status**:
- **Singleplayer**: Uses legacy `GameplayUIManager` (stable, complete)
- **Multiplayer**: Will use new `MultiPlayerUIManager` when ready
- **Migration**: New architecture exists but disabled until fully integrated

## Current Investigation (2025-01-16 - UI Architecture Contract Issues)

### 🚨 **CRITICAL ISSUE IDENTIFIED**: Incomplete Base Contract in BaseGameplayUIManager

**Problem**: `GameManager` calls methods via `GetActiveUI()` that don't exist in `BaseGameplayUIManager`, causing compilation errors.

**Root Cause**: The new UI architecture migration was incomplete - methods were copied to concrete classes without establishing proper base contracts.

### **Missing Method Analysis**

**9 Missing Methods in BaseGameplayUIManager**:
1. `ResetUIForNewGame()` - exists in legacy + SinglePlayer, missing from base + Multiplayer
2. `ShowOpponentAction()` - exists in legacy + Multiplayer, missing from base + SinglePlayer
3. `UpdateTurnDisplayMultiplayer()` - exists in legacy + Multiplayer, should be in base
4. `ShowSequenceProgressMessage()` - only exists in legacy, missing from new architecture
5. `ShowSpecialCardEffect()` - only exists in legacy, missing from new architecture
6. `ShowWinnerAnnouncement()` - exists in legacy + SinglePlayer, different signature in Multiplayer
7. `ShowDeckSyncStatus()` - exists in legacy + Multiplayer (identical), should be in base
8. `ShowImmediateFeedback()` - only exists in legacy, missing from new architecture
9. `UpdateButtonStates()` - only exists in legacy, missing from new architecture

### **Architectural Pattern Problems**

**Legacy Dependencies**: All GameManager calls use broken pattern:
```csharp
if (gameplayUI != null) {  // Legacy check
    GetActiveUI()?.MethodName();  // New architecture call
}
```

**Inconsistent Implementation**: Methods scattered across concrete classes without base contracts, breaking polymorphism.

### **Current Fix Strategy**

**Phase 1**: ✅ **Investigation Complete** - All 9 methods analyzed, patterns identified
**Phase 2**: 🎯 **In Progress** - Comprehensive UI Architecture Consolidation (Side Quest):

## **🔄 Current Status (2025-01-17)**:
**BREAKTHROUGH**: Clean UI architecture implementation achieved with proper separation of concerns!

**✅ MAJOR ARCHITECTURAL REFACTORING COMPLETE**:

### **🎯 Core Methods - Clean Architecture Implemented**
1. **EnableEndTakiSequenceButton()** - ✅ **CLEAN ARCHITECTURE**
   - **BaseGameplayUIManager**: Simple UI control (enable/disable button)
   - **SinglePlayerUIManager**: Human vs AI context (blocks AI sequences)
   - **MultiPlayerUIManager**: Network context (local player turn validation)
   - **Clean separation**: UI manipulation vs context decisions

2. **ShowTakiSequenceStatus()** - ✅ **CLEAN ARCHITECTURE**
   - **BaseGameplayUIManager**: UI display + basic message building via `BuildTakiSequenceMessage()`
   - **SinglePlayerUIManager**: "Your TAKI Sequence" vs "AI TAKI Sequence"
   - **MultiPlayerUIManager**: "Your TAKI Sequence" vs "Opponent TAKI Sequence" with network awareness
   - **Template Method Pattern**: Public method handles UI, protected virtual builds context messages

3. **UpdateAllDisplays()** - ✅ **REFACTORED FOR CLEAN ARCHITECTURE**
   - **BaseGameplayUIManager**: Removed context logic, now delegates to concrete classes
   - **Flow**: Base calls `EnableEndTakiSequenceButton(true)` → Concrete classes apply context → Base updates UI
   - **SinglePlayerUIManager**: No override needed (uses base implementation)
   - **MultiPlayerUIManager**: Uses `UpdateAllDisplaysWithNetwork()` composition pattern

### **🏗️ Architectural Pattern Achieved**
**Perfect Separation of Concerns**:
- **BaseGameplayUIManager**: Pure UI manipulation, no business logic
- **Concrete Classes**: Context-specific business logic (Human vs AI, Local vs Network)
- **Template Method Pattern**: Base defines flow, concrete classes customize behavior
- **Composition over Inheritance**: MultiPlayer uses `UpdateAllDisplaysWithNetwork()` extension

### **✅ Methods Marked as FINE FOR NOW (Multiplayer refinement deferred)**:
- **HandlePausedState()** - Basic pause UI handling works for both modes
- **HandleGameOverState()** - Basic game over UI handling works for both modes
- **HandleActiveState()** - Basic active game UI handling works for both modes
- **ResetUIForNewGame()** - Basic UI reset works for both modes
- *These may need multiplayer-specific enhancements later but are functionally adequate*

### **✅ Previously Consolidated Core Methods**:
4. **ResetUIForNewGame()** - Virtual method in base, context overrides in concrete classes
5. **ShowOpponentAction()** - Base implementation with context-aware messaging
6. **UpdateStrictButtonStates()** - Base implementation using abstract properties
7. **ForceEnableEndTurn()** - Base implementation for turn flow control
8. **UpdateTurnDisplay()** - Base implementation with `GetTurnMessage()` context override
9. **UpdateActiveColorDisplay()** - Base implementation using color conversion
10. **UpdateHandSizeDisplay()** - Base implementation with context-aware labels
11. **GetTurnMessage()** - Protected virtual for context-specific turn messages
12. **GetColorForCardColor()** - Base color conversion utility
13. **ShowPlayerMessage()** / **ShowComputerMessage()** - Base messaging system
14. **ShowPlayerMessageTimed()** / **ShowComputerMessageTimed()** - Base timed messaging
15. **ClearPlayerMessage()** / **ClearComputerMessage()** - Base message clearing
16. **ShowColorSelection()** - Base color selection UI control
17. **SelectColor()** - Base color selection with event handling
18. **ShowPlusTwoChainStatus()** / **HidePlusTwoChainStatus()** - Base chain status display
19. **HideTakiSequenceStatus()** - Base sequence status hiding

**⏳ Original 9 Missing Methods Status**:
✅ **ResetUIForNewGame()** - COMPLETE
✅ **ShowOpponentAction()** - COMPLETE
✅ **EnableEndTakiSequenceButton()** - COMPLETE (was not in original 9, but was causing compilation errors)
✅ **ShowTakiSequenceStatus()** - COMPLETE (was not in original 9, but was causing compilation errors)
✅ **UpdateAllDisplays()** - COMPLETE (was not in original 9, but needed refactoring for clean architecture)
⏳ **UpdateTurnDisplayMultiplayer()** - Exists in legacy, needs integration
⏳ **ShowSequenceProgressMessage()** - Needs base class integration
⏳ **ShowSpecialCardEffect()** - Needs base class integration
⏳ **ShowWinnerAnnouncement()** - Needs signature standardization
⏳ **ShowDeckSyncStatus()** - Needs base class integration
⏳ **ShowImmediateFeedback()** - Needs base class integration
⏳ **UpdateButtonStates()** - Legacy method, needs integration decision

## **🎯 BaseGameplayUIManager.cs - COMPLETE REVIEW FINISHED** ✅

**ALL METHODS REVIEWED AND STATUS CONFIRMED**:

### **✅ Methods with Clean Architecture Implementation**:
- **EnableEndTakiSequenceButton()** / **ShowTakiSequenceStatus()** - Clean separation of concerns
- **UpdateAllDisplays()** - Refactored to delegate context decisions to concrete classes
- **All core UI methods** - Properly consolidated with Template Method pattern

### **✅ Methods Confirmed Fine For Now**:
- **HandlePausedState()** / **HandleGameOverState()** / **HandleActiveState()** / **ResetUIForNewGame()** - Basic implementations work for both modes
- **Properties section** - **PlayButtonEnabled**, **DrawButtonEnabled**, **EndTurnButtonEnabled**, **EndTakiSequenceButtonEnabled**, **IsColorSelectionActive**, **GetButtonStateSummary()** - All look great

### **✅ Methods Correctly Removed**:
- **UpdateDrawPileCount()** / **UpdateDiscardPileCount()** / **UpdateAllPileCounts()** - Not BaseGameplayUIManager's responsibility, belongs to DeckUIManager

### **✅ Architecture Methods Working Perfectly**:
- **ShouldBeActive()** - Perfect implementation for preventing duplicate handlers, robust error handling

## **🎯 Legacy GameplayUIManager.cs - USAGE ANALYSIS** ⚠️

**LEGACY SYSTEM STILL IN USE**: Found **14 active references** in GameManager.cs:

### **🔗 Active Legacy Dependencies in GameManager.cs**:
- **Event Connections** (lines 809-815): `gameplayUI.OnPlayCardClicked += ...` (5 event handlers)
- **Event Disconnections** (lines 904-908): `gameplayUI.OnPlayCardClicked -= ...` (5 event handlers)
- **Direct Method Calls** (lines 2014, 2053, 2412, 2441): `gameplayUI.ShowTakiSequenceStatus()`, `gameplayUI.EnableEndTakiSequenceButton()`

### **🚨 MIGRATION REQUIRED**:
**GameManager still uses legacy gameplayUI instead of GetActiveUI()**
- **Event system**: Legacy gameplayUI event connections need migration to new architecture
- **Direct calls**: 4 remaining direct legacy calls need conversion to `GetActiveUI()?.Method()`
- **This explains**: Why legacy GameplayUIManager can't be removed yet

## **🎯 CURRENT STATUS: UI Architecture Migration Phase (2025-01-19)**

### **✅ MAJOR PROGRESS - UI Method Migration with 3-Method Pattern**:

**✅ 3-Method Pattern Successfully Established**:
Following the simple, clean approach without over-engineering helpers:

**Core Pattern**:
- **BaseGameplayUIManager**: Virtual method with basic default implementation
- **SinglePlayerUIManager**: Override with Human vs AI context logic
- **MultiPlayerUIManager**: Override with Local vs Opponent network-aware logic

**✅ Methods Successfully Migrated**:
1. **ShowSpecialCardEffect(CardType, PlayerType, string)** - Context-aware special card messaging
2. **ShowSequenceEndedMessage(int, CardColor, PlayerType)** - TAKI sequence completion feedback
3. **ShowSequenceProgressMessage(int, CardColor, PlayerType)** - TAKI sequence progress updates
4. **ShowChainProgressMessage(int, int, PlayerType)** - PlusTwo chain progress tracking
5. **ShowChainBrokenMessage(int, PlayerType)** - PlusTwo chain break notifications
6. **ShowImmediateFeedback(string, bool)** - High-priority urgent messaging

**✅ Architectural Benefits Achieved**:
- **Clean Separation**: UI display logic vs business context logic
- **No Over-Engineering**: Simple 3-method approach without complex helpers
- **Context Awareness**: Each mode shows appropriate messages (Human/AI vs Local/Opponent)
- **Template Method Pattern**: Base handles UI manipulation, concrete classes provide context

### **🚨 CRITICAL DISCOVERY - Turn System Architecture Issue**:

**Duplicate UI Updates Problem**:
- **SinglePlayer Flow**: `TurnManager → GameStateManager.ChangeTurnState() → GameManager.OnTurnStateChanged() → UpdateTurnDisplay()`
- **Multiplayer Flow**: `NetworkGameManager → GameStateManager.ChangeTurnState() → GameManager.OnTurnStateChanged() → UpdateTurnDisplay()` + **ALSO calls `UpdateTurnDisplayMultiplayer()`**
- **Result**: Multiplayer mode updates UI **TWICE** with potentially conflicting information!

**Architecture Gap Identified**:
- Different turn management systems (TurnManager vs NetworkGameManager)
- Different responsibilities (pure display vs display + button control + messages)
- Potential button state conflicts and message overwrites

### **📋 NEXT CRITICAL PHASE: Turn System Investigation**

**Investigation Required**:
1. **Deep analysis of singleplayer turn flow** (TurnManager + AI integration)
2. **Deep analysis of multiplayer turn flow** (NetworkGameManager + network sync)
3. **Design unified `UpdateTurnDisplay` method** that handles both contexts cleanly
4. **Ensure pure display logic in UI managers** when possible
5. **Resolve button state and message conflicts**

**Goal**: Create clean, unified turn display system that eliminates duplicate updates and conflicts while maintaining mode-specific behavior.

### **⏳ Remaining UI Methods to Migrate**:
- **UpdateButtonStates()** - Legacy method integration decision needed
- **ShowWinnerAnnouncement()** - Signature standardization required
- **ShowDeckSyncStatus()** - Base class integration needed

## **🎯 REVISED SYSTEMATIC CLEANUP PLAN** - Top-Down Approach ✅

### **📋 GameManager.cs Organization Structure (Clean Architecture)**

```csharp
#region Core Properties and Fields
// Essential game state, configuration, and component references
- Header attributes and tooltips for Inspector fields
- gameState, turnManager, computerAI (component references)
- singlePlayerUI, multiPlayerUI (new UI architecture)
- deckManager, startingPlayer, playerHand (game setup)
- singleplayerPlayerHandManager, singleplayerOpponentHandManager (per-screen singleplayer hand managers)
- multiplayerPlayerHandManager, multiplayerOpponentHandManager (per-screen multiplayer hand managers)
- pauseManager, gameEndManager, exitValidationManager (flow managers)
- networkGameManager (multiplayer)
- logLevel, productionMode (logging configuration)
- Events: OnGameStarted, OnGameEnded, OnTurnStarted, OnCardPlayed
#endregion

#region Private State Management
// Internal state tracking and control variables
- areComponentsValidated, areSystemsInitialized, isGameActive, isMultiplayerMode
- hasPlayerTakenAction, canPlayerDraw, canPlayerPlay, canPlayerEndTurn (turn flow control)
- isWaitingForAdditionalAction, activeSpecialCardEffect (special card state)
- shouldSkipNextTurn, stopCardPlayer (STOP card state)
- isCurrentCardLastInSequence (TAKI sequence state)
#endregion

#region Public Properties
// External API for accessing game state
- IsGameActive, IsPlayerTurn, CurrentPlayer, PlayerHandSize, ComputerHandSize, ActiveColor
- AreComponentsValidated, AreSystemsInitialized
- HasPlayerTakenAction, CanPlayerDrawCard, CanPlayerPlayCard, CanPlayerEndTurn
- IsWaitingForAdditionalAction, ActiveSpecialCardEffect
- IsGamePaused, IsGameEndProcessed, IsExitValidationActive
- IsMultiplayerMode, IsNetworkReady, IsMyNetworkTurn (multiplayer properties)
#endregion

#region Architecture Management
// UI and HandManager architecture abstraction methods
- GetActiveUI(), GetUI()
- GetActivePlayerHandManager(), GetActiveOpponentHandManager()
- GetPlayerHandManager(), GetOpponentHandManager()
#endregion

#region Unity Lifecycle
// MonoBehaviour lifecycle methods
- Start()
- OnDestroy()
#endregion

#region System Initialization
// Component validation, system setup, and game mode initialization
- ConfigureLogging()
- ValidateAndConnectComponents()
- ValidateComponents()
- ConnectComponentReferences()
- ConnectEvents()
- ConnectActiveUIManagerEvents()
- ConnectUIManagerEvents()
- DisconnectUIManagerEvents()
- DisconnectAllUIManagerEvents()
- InitializeSinglePlayerSystems()
- InitializeMultiPlayerSystems()
- InitializeVisualCardSystem()
- InitializeNetworkHandManagers()
#endregion

#region Game Flow Control
// Main game startup and reset functionality
- StartNewSinglePlayerGame()
- StartNewMultiPlayerGame()
- ResetGameSystems()
- OnInitialGameSetupComplete()
#endregion

#region Turn Flow Management
// Strict turn flow control system
- ResetTurnFlowState()
- StartPlayerTurnFlow()
- HandlePostCardPlayTurnFlow()
- HandlePostDrawTurnFlow()
- EndPlayerTurnWithStrictFlow()
- EndAITurnWithStrictFlow()
- StartPlayerTurnAfterStop()
- StartAITurnAfterStop()
- TriggerAITurnAfterStop()
#endregion

#region Player Actions
// User input handlers and card play mechanics
- OnPlayCardButtonClicked()
- OnDrawCardButtonClicked()
- OnEndTurnButtonClicked()
- OnEndTakiSequenceButtonClicked()
- OnColorSelectedByPlayer()
- PlayCardWithStrictFlow()
- DrawCardWithStrictFlow()
- CountPlayableCards()
- GetTopDiscardCard()
#endregion

#region Special Card System
// Special card effects and rule processing
- HandleSpecialCardEffects()
- HandleStopCardEffect()
- HandleChangeDirectionCardEffect()
- HandleChangeColorCardEffect()
- LogCardEffectRules()
- ResetSpecialCardState()
- HasPendingSpecialCardEffects()
- GetSpecialCardStateDescription()
- ProcessStopSkipEffect()
- BreakPlusTwoChainByDrawing()
- MakeOpponentDrawCards()
- GetTwoPlayerDirectionNote()
#endregion

#region AI Integration
// Computer AI event handlers and coordination
- OnComputerTurnReady()
- OnAICardSelected()
- OnAIDrawCard()
- OnAIColorSelected()
- OnAIDecisionMade()
- OnAISequenceComplete()
- HandleAISpecialCardEffects()
- TriggerAIAdditionalAction()
- TriggerAISequenceDecision()
#endregion

#region Network Multiplayer
// Network game coordination and synchronization
- ProcessNetworkCardPlay()
- ProcessNetworkCardDraw()
- SendLocalCardPlayToNetwork()
- SendLocalCardDrawToNetwork()
- OnPlayCardButtonClickedMultiplayer()
- OnDrawCardButtonClickedMultiplayer()
- UpdateAllUIWithNetworkSupport()
- SynchronizeNetworkHandCounts()
- IsMultiplayerGameReady()
- GetNetworkGameStatus()
#endregion

#region Game State Events
// Event handlers for game state changes
- OnTurnStateChanged()
- OnInteractionStateChanged()
- OnGameStatusChanged()
- OnActiveColorChanged()
- OnTurnChanged()
- OnGameWon()
- OnPlayerTurnTimeOut()
- OnCardDrawnFromDeck()
- OnTakiSequenceStarted()
- OnTakiSequenceCardAdded()
- OnTakiSequenceEnded()
- ProcessStopSkip()
#endregion

#region UI and Visual Updates
// UI synchronization and visual card management
- UpdateAllUI()
- UpdateVisualHands()
- RefreshPlayerHandStates()
- OnPlayerCardSelected()
- OnComputerCardSelected()
#endregion

#region External System Coordination
// Integration with pause, game end, and menu systems
- OnGamePaused()
- OnGameResumed()
- OnGameEndProcessed()
- OnGameRestarted()
- OnReturnedToMenu()
- OnExitValidationShown()
- OnExitValidationCancelled()
- OnExitConfirmed()
#endregion

#region State Preservation
// Pause/resume state management
- CaptureTurnFlowState()
- RestoreTurnFlowState()
#endregion

#region Public API
// External interface methods for other systems
- RequestPauseGame()
- RequestResumeGame()
- RequestRestartGame()
- RequestRestartGameFromPause()
- RequestReturnToMenu()
- RequestExitConfirmation()
- RequestDrawCard()
- RequestPlayCard()
- GetPlayerHand()
- CanPlayerAct()
#endregion

#region Debug and Development
// Debug methods, context menus, and development tools
- DebugNetworkHandState()
- DebugNetworkDeckState()
- ForceNetworkHandSync()
- DebugTakiSequenceState()
- DebugSelectedCardType()
- ForceNewGameStart()
- LogTurnFlowState()
- LogSpecialCardState()
- ForceResetStopFlag()
- TestStopCardEffect()
- CheckStopFlagState()
- ForceResetSpecialCardState()
- TriggerComputerTurnManually()
- ForceUISync()
#endregion
```

**Region Ordering Rationale:**

1. **Core Properties and Fields** - Essential declarations and dependencies
2. **Private State Management** - Internal state tracking
3. **Public Properties** - External API access points
4. **Architecture Management** - Abstraction layer for UI/HandManager systems
5. **Unity Lifecycle** - Standard Unity methods
6. **System Initialization** - Setup and configuration logic
7. **Game Flow Control** - Main game startup and reset
8. **Turn Flow Management** - Core turn mechanics
9. **Player Actions** - User input processing
10. **Special Card System** - Game rules and card effects
11. **AI Integration** - Computer opponent coordination
12. **Network Multiplayer** - Multiplayer-specific functionality
13. **Game State Events** - Event handling and state changes
14. **UI and Visual Updates** - Display synchronization
15. **External System Coordination** - Integration with other managers
16. **State Preservation** - Pause/resume mechanics
17. **Public API** - External interface methods
18. **Debug and Development** - Development tools and debugging

This structure groups related functionality together while maintaining a logical flow from core setup through gameplay mechanics to external integrations and debugging tools.

### **📋 PHASE 1: Aggressive Legacy Removal** (Current Phase)
**Goal**: Complete elimination of all legacy UI architecture from GameManager.cs

**🚨 AGGRESSIVE APPROACH ADOPTED**:
- **All legacy components removed**: No `gameplayUI`, `useNewUIArchitecture`, `usePerScreenHandManagers` flags
- **Clean hand manager naming**: `singleplayerPlayerHandManager`, `singleplayerOpponentHandManager`, etc.
- **Force complete migration**: Remove legacy first, then fix what breaks
- **No dual architecture confusion**: Only new UI architecture remains

**Migration Strategy**:
1. **✅ Legacy Cleanup COMPLETE**:
   - ❌ Removed `gameplayUI` (GameplayUIManager reference)
   - ❌ Removed `useNewUIArchitecture` flag
   - ❌ Removed `usePerScreenHandManagers` flag
   - ❌ Removed legacy hand manager references (`playerHandManager`, `opponentHandManager`)
   - ✅ Clean hand manager naming adopted

2. **🎯 Current Task**: Fix compilation errors and missing method calls:
   - Convert any remaining legacy calls to `GetActiveUI()?.Method()`
   - Add missing methods to BaseGameplayUIManager as needed
   - Test both singleplayer and multiplayer functionality
   - Ensure all UI interactions use new architecture

3. **Expected Outcomes**:
   - **Clean, single architecture**: No legacy fallbacks or confusion
   - **Forced completeness**: All methods MUST exist in new architecture
   - **Immediate clarity**: What's missing becomes obvious immediately
   - **No regression risk**: Can't accidentally use removed legacy system

### **📋 PHASE 2: Final Legacy Cleanup** (Next Phase)
**Goal**: Complete removal of GameplayUIManager.cs and verify architecture completeness

**Approach**:
- **Remove GameplayUIManager.cs entirely** once GameManager migration is complete
- **Verify all UI methods exist** in BaseGameplayUIManager that GameManager actually needs
- **Test complete functionality** in both singleplayer and multiplayer modes
- **Document remaining missing methods** from original 9-method analysis if any are still needed

### **📋 PHASE 3: Architecture Validation** (Future Phase)
**Goal**: Final testing and documentation update

**This aggressive approach ensures**:
- **Single source of truth**: Only new UI architecture exists
- **Complete migration**: No partial or dual systems
- **Clear error messages**: Missing functionality becomes obvious immediately
- **Clean codebase**: No confusion about which system to use

### **Additional Discovery: Orphaned Method Integration**

**Critical Finding**: `DeckUIManager.ResetUIForNewGame()` exists but is **never called** by any code:
- Method exists and is functional (resets deck UI, pile visuals, shows loading message)
- No integration with game initialization flow
- Should likely be called during new game setup
- Indicates incomplete deck UI integration in initialization sequence

### **Files Updated with Clean Architecture**
✅ **BaseGameplayUIManager.cs** - Clean base implementation with Template Method pattern
✅ **SinglePlayerUIManager.cs** - Context-aware overrides for Human vs AI logic
✅ **MultiPlayerUIManager.cs** - Context-aware overrides for network/local player logic
✅ **GameManager.cs** - Legacy component references removed (🎯 CURRENT: Fix compilation errors)
⏳ **GameplayUIManager.cs** - Complete removal after GameManager migration
⏳ **DeckUIManager.cs** - Investigate orphaned ResetUIForNewGame() integration (deferred)

## **🎯 CURRENT STATUS: Aggressive Legacy Removal Applied**

**✅ COMPLETED**: All legacy UI architecture components removed from GameManager.cs
**🎯 ACTIVE**: Fix compilation errors and ensure all GameManager UI calls use `GetActiveUI()` pattern
**📋 NEXT**: Remove GameplayUIManager.cs completely once compilation errors resolved

