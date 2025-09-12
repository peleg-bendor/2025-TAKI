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
Currently implementing **Phase 2 Milestone 1**: Deck initialization for multiplayer to show card piles and initial hands with proper network synchronization.