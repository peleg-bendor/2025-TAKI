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
- **Singleplayer**: ✅ **Complete & Stable** - Full TAKI game with all special cards, AI opponent, pause system, UI warnings resolved
- **Multiplayer**: 🚀 **Ready for Testing** - Core initialization bugs fixed, UI conflicts resolved!
  - ✅ Hand assignment no longer cleared after network setup
  - ✅ Double initialization prevented with safety system
  - ✅ Unified game start architecture implemented
  - ✅ Per-screen UI architecture ready
  - ✅ Network synchronization logic functional
  - ✅ Singleplayer UI conflicts resolved (false warnings eliminated)

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