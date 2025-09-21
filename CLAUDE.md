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
- **Multiplayer**: 🎯 **In Development** - Core systems ready, debugging in progress

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
- 🎯 **Current**: Debugging multiplayer issues via logs

## My Prompt to you:
- Read `investigating.md`.

## Todo List
- [ ] **Investigate multiplayer logs** - analyze issues in MOST_RECENT_RUN_LOGS_SIMPLE.md
- [ ] Debug and resolve identified multiplayer problems
- [ ] Test complete multiplayer functionality
- [ ] Final architecture validation

## Side Notes
- **No Unicode**: Avoid special characters in code/files
