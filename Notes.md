

Go over each file, and make the changes needed, don't ignore checks they are important, the logs probably less so.
I will copy your scripts and compare them to the originals', so try to keep the structure as simular as you can.


---


Reorginized logs: BasicComputerAI, GameManager, DeckManager, DeckUIManager, GameStateManager, TurnManager, GameplayUIManager, TakiGameDiagnostics, TakiLogger.

Unrginized logs: CardData, Enums, TakiDeckGenerator, CardDataLoader, Deck, DontDestroyOnLoad, GameSetupManager, CardController, DifficultySlider, HandManager, MenuNavigation, PileManager, ButtonSFX, MusicSlider, SfxSlider.


---
---
---

```
# TAKI Game - Complete Script Documentation & Reference Guide

## 📋 **Document Overview**
**Purpose**: Master reference for all scripts in the TAKI game project  
**Total Scripts**: 24 core scripts + utilities  
**Last Updated**: Based on Milestone 7 (Strict Turn Flow System) completion  
**Architecture**: Single Responsibility Pattern with Event-Driven Communication

---

## 🎯 **Quick Reference Guide**

### **For Common Development Tasks**:
- **Adding New Card Effects**: `GameManager.cs`, `CardData.cs`, `GameStateManager.cs`
- **UI Modifications**: `GameplayUIManager.cs`, `DeckUIManager.cs`, `MenuNavigation.cs`
- **Game Flow Changes**: `GameManager.cs`, `TurnManager.cs`, `GameStateManager.cs`
- **AI Improvements**: `BasicComputerAI.cs`
- **Visual Card System**: `CardController.cs`, `HandManager.cs`, `PileManager.cs`
- **Debugging Issues**: `TakiGameDiagnostics.cs`

### **Script Interaction Patterns**:
```
GameManager (Coordinator)
├── DeckManager → CardDataLoader, Deck, DeckUIManager, GameSetupManager
├── GameStateManager → TurnManager, BasicComputerAI
├── GameplayUIManager → ColorSelection, PlayerActions
├── HandManager (Player) ←→ CardController (Individual Cards)
├── HandManager (Computer) ←→ CardController (Individual Cards)
└── PileManager → DeckUIManager
```

---

# 🏗️ **Core Architecture Scripts**

## **GameManager.cs** 🎯 **CENTRAL HUB**
**Purpose**: Main game coordinator with strict turn flow system  
**Responsibility**: Orchestrates all gameplay systems and enforces turn rules  
**Status**: ✅ Enhanced with strict turn flow control

### **Key Features**:
- **Strict Turn Flow**: Player must take ONE action (PLAY/DRAW) then END TURN
- **Bulletproof Button Control**: Immediate disable on action, prevent multiple clicks
- **System Integration**: Connects all major components via events
- **Game State Management**: Handles setup, play, and end conditions

### **Critical Methods**:
```csharp
// Game Lifecycle
StartNewSinglePlayerGame() // Entry point for new games
InitializeSinglePlayerSystems() // System setup
ValidateAndConnectComponents() // Safety validation

// Strict Turn Flow
StartPlayerTurnFlow() // Initialize player turn with proper constraints
OnPlayCardButtonClicked() // Handle card play with flow control
OnDrawCardButtonClicked() // Handle card draw with flow control
OnEndTurnButtonClicked() // Complete turn and switch players
PlayCardWithStrictFlow(card) // Safe card playing with validation
DrawCardWithStrictFlow() // Safe card drawing with rules
EndPlayerTurnWithStrictFlow() // Safe turn completion

// Game Integration
UpdateAllUI() // Comprehensive UI updates
RefreshPlayerHandStates() // Visual card state updates
LogCardEffectRules(card) // Special card logging system
```

### **Dependencies**:
- **Direct**: `GameStateManager`, `TurnManager`, `BasicComputerAI`, `GameplayUIManager`, `DeckManager`
- **Visual**: `HandManager` (Player), `HandManager` (Computer)
- **Events**: All major components via event system

### **Integration Points**:
```csharp
// Events FROM GameManager
OnGameStarted, OnGameEnded, OnTurnStarted, OnCardPlayed

// Events TO GameManager  
OnInitialGameSetup, OnCardDrawn, OnTurnStateChanged, OnAICardSelected
OnPlayCardClicked, OnDrawCardClicked, OnEndTurnClicked
```

### **Turn Flow State Variables**:
```csharp
private bool hasPlayerTakenAction = false;
private bool canPlayerDraw = true;
private bool canPlayerPlay = true; 
private bool canPlayerEndTurn = false;
```

---

## **GameStateManager.cs** 🎮 **GAME RULES ENGINE**
**Purpose**: Manages game state using multi-enum architecture  
**Responsibility**: Rule validation, state transitions, color management  

### **Multi-Enum Architecture**:
```csharp
public TurnState turnState;        // WHO is acting?
public InteractionState interactionState;  // WHAT special interaction?
public GameStatus gameStatus;      // WHAT is overall status?
public CardColor activeColor;      // Current active color
public TurnDirection turnDirection; // Play direction
```

### **Key Methods**:
```csharp
// State Management
ChangeTurnState(newState) // Switch whose turn it is
ChangeInteractionState(newState) // Handle special interactions
ChangeActiveColor(newColor) // Update active color
UpdateActiveColorFromCard(playedCard) // Auto-update from played card

// Rule Validation
IsValidMove(cardToPlay, topDiscardCard) // Core game rule checking
CanPlayerAct() // Check if player can take actions
CanComputerAct() // Check if computer can take actions

// Game Control
DeclareWinner(winner) // End game with winner
ResetGameState() // Clean slate for new game
```

### **Dependencies**: Standalone (other systems depend on this)
### **Used By**: `GameManager`, `TurnManager`, `BasicComputerAI`, `GameplayUIManager`

---

## **TurnManager.cs** 🔄 **TURN ORCHESTRATOR**  
**Purpose**: Manages turn switching and timing  
**Responsibility**: Player transitions, turn timing, computer turn scheduling  

### **Key Features**:
- **Turn Switching**: Clean player-to-player transitions
- **Computer Turn Delay**: Natural AI thinking time
- **Turn Timer**: Optional player time limits
- **Turn Skip Logic**: For Stop cards

### **Key Methods**:
```csharp
// Turn Control
StartTurn(player) // Begin specific player's turn
EndTurn() // End current turn and switch
SwitchToNextPlayer() // Alternate between players
SkipTurn() // Skip current player (Stop card effect)
ForceTurnTo(player) // Force turn to specific player

// Turn Management
InitializeTurns(firstPlayer) // Start turn system
PauseTurns() // Pause turn system
ResumeTurns() // Resume after pause
ResetTurns() // Clean slate

// Turn Timing
HandleTurnTiming() // Process timers and delays
StartTurnTimer() // Begin player time limit
ScheduleComputerTurn() // Delay computer action
```

### **Dependencies**: `GameStateManager`
### **Events**: `OnTurnChanged`, `OnTurnTimeOut`, `OnComputerTurnReady`

---

# 📦 **Deck Management System**

## **DeckManager.cs** 🃏 **DECK COORDINATOR**
**Purpose**: Coordinates all deck-related operations using delegation pattern  
**Responsibility**: Unified interface for deck functionality  

### **Coordinator Pattern**:
```csharp
// Delegates to specialized components:
deck → pure deck operations (draw, discard, shuffle)
cardLoader → resource loading (110 cards from Resources)  
deckUI → UI updates (counts, messages)
gameSetup → initialization logic (deal hands, starting card)
```

### **Public API**:
```csharp
// Card Operations
DrawCard() → delegates to deck.DrawCard()
DrawCards(count) → delegates to deck.DrawCards() 
DiscardCard(card) → delegates to deck.DiscardCard()
GetTopDiscardCard() → delegates to deck.GetTopDiscardCard()

// Game Setup
InitializeDeck() → delegates to gameSetup.InitializeNewGame()
SetupInitialGame() → delegates to gameSetup.SetupInitialGame()

// Information
ShowMessage(message) → delegates to deckUI.ShowDeckMessage()
GetDeckStats() → delegates to cardLoader.GetDeckStats()
```

### **Component References**:
```csharp
public Deck deck;                    // Pure deck operations
public CardDataLoader cardLoader;    // Resource loading
public DeckUIManager deckUI;         // UI updates
public GameSetupManager gameSetup;   // Game initialization
```

### **Events**: Forwards all component events to external systems

---

## **Deck.cs** 🎴 **PURE DECK OPERATIONS**
**Purpose**: Core deck functionality - draw pile, discard pile, shuffling  
**Responsibility**: Card storage and manipulation only (NO UI, NO resources)

### **Core Operations**:
```csharp
// Deck Management
InitializeDeck(allCards) // Set up new deck
ShuffleDeck() // Fisher-Yates shuffle algorithm
ClearDeck() // Empty both piles

// Card Operations  
DrawCard() // Draw single card with auto-reshuffle
DrawCards(count) // Draw multiple cards safely
DiscardCard(card) // Add card to discard pile
GetTopDiscardCard() // View top without removal

// Internal
ReshuffleDiscardIntoDraw() // Auto-reshuffle when needed
```

### **Data Storage**:
```csharp
public List<CardData> drawPile;    // Cards to be drawn
public List<CardData> discardPile; // Played cards
```

### **Auto-Reshuffle Logic**: When draw pile empty + discard pile ≥ 2 cards
### **Events**: `OnCardDrawn`, `OnCardDiscarded`, `OnDeckShuffled`, `OnDeckEmpty`

---

## **CardDataLoader.cs** 📁 **RESOURCE MANAGER**
**Purpose**: Loads and validates CardData from Resources folder  
**Responsibility**: Resource management only (NO deck operations, NO UI)

### **Loading System**:
```csharp
// Resource Loading
LoadAllCardData() // Load 110 cards from Resources/Data/Cards
ValidateDeckComposition() // Verify 110 cards loaded correctly
ReloadCards() // Refresh from resources (testing)

// Data Access
GetAllCardsForDeck() // Safe copy for deck initialization
GetCardsByType(cardType) // Filter by card type
GetCardsByColor(color) // Filter by color
GetDeckStats() // Statistical analysis
```

### **Validation**:
- **Expected Deck Size**: 110 cards total
- **Composition Check**: Validates card counts match TAKI rules
- **Error Handling**: Reports missing or invalid cards

### **Events**: `OnCardsLoaded`, `OnLoadError`

---

## **GameSetupManager.cs** 🎮 **GAME INITIALIZATION**
**Purpose**: Handles game setup and initialization logic  
**Responsibility**: Game state preparation (NO UI, NO deck operations, NO resources)

### **Setup Operations**:
```csharp
// Game Initialization
InitializeNewGame() // Fresh deck from cardLoader
SetupInitialGame() // Deal hands + place starting card

// Initial Dealing
DrawInitialHand(handSize) // Deal cards to player
SelectStartingCard() // Choose good starting card (prefer numbers)

// Testing Support
QuickSetupForTesting(handSize) // Fast setup for development
ValidateSetup() // Check all components assigned
```

### **Starting Card Logic**: Prefers number cards, avoids complex special cards
### **Dependencies**: `CardDataLoader`, `Deck`
### **Events**: `OnGameInitialized`, `OnInitialGameSetup`

---

# 🎨 **Visual Card System**

## **CardController.cs** 🃏 **INDIVIDUAL CARD BEHAVIOR**
**Purpose**: Handles single card prefab behavior and visual representation  
**Responsibility**: Card display, interaction, image loading

### **Visual Features**:
- **Real Scanned Images**: Loads from Resources/Sprites/Cards/
- **Face Up/Down**: Instant image swapping (no animations)
- **Selection System**: 10px Y-offset movement when selected
- **Tint Feedback**: Gold for valid moves, red for invalid
- **Professional Layout**: 100px height, 67px width

### **Image Loading System**:
```csharp
// Image Paths (FIXED for current folder structure)
LoadCardFrontImage() // Sprites/Cards/Fronts/{Color}/{name}_{color}
LoadCardBackImage()  // Sprites/Cards/Backs/card_back
GetCardFrontImagePath(card) // Generate correct resource path
GetNumberName(number) // Convert 1-9 to "one"-"nine"
GetSpecialCardName(cardType) // Convert type to filename
```

### **Interaction System**:
```csharp
// Card States
SetSelected(selected) // Visual selection with position offset
SetPlayable(playable) // Tint feedback for rule validation  
SetCardFacing(faceUp) // Face up or face down display
UpdateVisualFeedback() // Apply tints based on state

// User Interaction
OnCardButtonClicked() // Handle user click
HandleCardSelection() // Notify parent HandManager
```

### **Dependencies**: `HandManager` (parent), `CardData` (data source)
### **Integration**: Event-driven communication with hand management

---

## **HandManager.cs** 👥 **HAND DISPLAY SYSTEM**
**Purpose**: Manages hand display and card prefab instantiation  
**Responsibility**: Dynamic hand layout, card positioning, user interaction

### **Display Features**:
- **Adaptive Spacing**: Smart spacing algorithm based on hand size
- **Manual Positioning**: Precise card placement without Unity UI constraints
- **Instant Updates**: Add/remove cards with position recalculation
- **Dual Mode**: Face-up (player) or face-down (computer) hands

### **Layout Algorithm**:
```csharp
// Spacing Calculation
CalculateSpacing(cardCount) // Dynamic spacing based on hand size
ArrangeCards() // Position all cards with calculated spacing
maxSpacing = 120px // For few cards
minSpacing = 40px  // For many cards
tightSpacingThreshold = 8 // Switch point for tight spacing
```

### **Hand Management**:
```csharp
// Core Operations
UpdateHandDisplay(newHand) // Rebuild entire hand display
AddCard(cardData) // Add single card to end
RemoveCard(cardData) // Remove specific card
ClearAllCards() // Destroy all card prefabs

// User Interaction
HandleCardSelection(cardController) // Process user card clicks
GetSelectedCard() // Get currently selected CardData
ClearSelection() // Deselect all cards

// Integration
RefreshPlayableStates() // Update valid/invalid card feedback
UpdatePlayableStates() // Rule validation integration
DelayedPlayableUpdate() // Timing workaround for UI updates
```

### **Visual Feedback Integration**:
```csharp
// Rule Integration
UpdatePlayableStates() // Check each card against top discard
// Uses GameManager.GetTopDiscardCard() and GameStateManager.IsValidMove()
```

### **Dependencies**: `CardController` (children), `GameManager` (rule validation)
### **Events**: `OnCardSelected`, `OnHandUpdated`

---

## **PileManager.cs** 🏗️ **PILE VISUAL SYSTEM**
**Purpose**: Manages visual display of draw and discard pile cards  
**Responsibility**: Pile card representation (NO animations, instant updates)

### **Visual System**:
```csharp
// Pile Management
UpdateDrawPileDisplay(cardCount) // Show/hide draw pile card
UpdateDiscardPileDisplay(topCard) // Update discard pile card
CreateDrawPileVisual() // Face-down card back
CreateDiscardPileVisual() // Face-up current card

// Lifecycle
ClearPileVisuals() // Destroy pile cards
ResetPiles() // Clean slate for new game
```

### **Integration**: Works with `DeckUIManager` for complete pile system
### **Card Creation**: Uses same `CardController` system as hands for consistency

---

# 🖥️ **User Interface System**

## **GameplayUIManager.cs** 🎮 **GAMEPLAY UI CONTROLLER**
**Purpose**: Handles UI updates for gameplay - turn system, player actions  
**Responsibility**: Turn display, button control, color selection, game feedback

### **Enhanced Button Control System** (Milestone 7):
```csharp
// Strict Button State Management
UpdateStrictButtonStates(enablePlay, enableDraw, enableEndTurn)
ForceEnableEndTurn() // After successful action
EmergencyButtonFix() // Debug recovery
ValidateButtonStates() // Debug verification

// Button State Tracking
private bool playButtonEnabled = false;
private bool drawButtonEnabled = false;  
private bool endTurnButtonEnabled = false;
```

### **UI Management**:
```csharp
// Display Updates
UpdateTurnDisplay(turnState) // Whose turn display
UpdateActiveColorDisplay(activeColor) // Color indicator
UpdateHandSizeDisplay(player1Size, player2Size) // Hand counts
UpdateAllDisplays() // Complete UI refresh using multi-enum

// User Interaction  
ShowColorSelection(show) // Color picker panel
ShowComputerMessage(message) // Feedback display
ShowWinnerAnnouncement(winner) // Game over screen

// Button Event Handling
OnPlayCardClicked() // Validate and forward to GameManager
OnDrawCardClicked() // Validate and forward to GameManager  
OnEndTurnClicked() // Validate and forward to GameManager
OnColorSelected(color) // Handle color choice
```

### **Multi-Enum Integration**:
```csharp
// Uses GameStateManager's multi-enum architecture
UpdateAllDisplays(turnState, gameStatus, interactionState, activeColor)
```

### **Dependencies**: `GameManager` (via events), `GameStateManager` (state display)
### **Events**: `OnPlayCardClicked`, `OnDrawCardClicked`, `OnEndTurnClicked`, `OnColorSelected`

---

## **DeckUIManager.cs** 📊 **DECK UI CONTROLLER** 
**Purpose**: Handles ONLY deck-related UI updates  
**Responsibility**: Deck counts, deck events, pile visuals (NO gameplay UI conflicts)

### **Clean UI Separation** (Fixed in Milestone 7):
```csharp
// ONLY handles:
DrawPileCountText // "Draw: 45"
DiscardPileCountText // "Discard: 3" 
DeckMessageText // ONLY deck events (loading, shuffling, errors)
PileManager // Visual pile cards
```

### **Deck Event Messages**:
```csharp
// Specialized deck messages
ShowLoadingMessage() // "Loading deck..."
ShowDeckLoadedMessage(count) // "Deck loaded: 110 cards"
ShowDeckInitializedMessage() // "New deck shuffled!"
ShowReshuffleMessage() // "Reshuffled discard pile!"
ShowDeckErrorMessage(error) // "ERROR: No more cards!"
ShowGameStartMessage(startCard) // "Starting card: Red 5"
```

### **Pile Visual Integration**:
```csharp
// Works with PileManager for complete system
UpdateDeckUI(drawCount, discardCount) // Updates both text AND visuals
UpdateDiscardPileDisplay(topCard) // Delegates to PileManager
```

### **Dependencies**: `PileManager` (visual piles)  
### **Integration**: Clean separation from `GameplayUIManager`

---

## **MenuNavigation.cs** 🧭 **MENU SYSTEM**
**Purpose**: Navigation between menu screens using stack-based approach  
**Responsibility**: Screen transitions, loading screens, game startup integration

### **Navigation System**:
```csharp
// Stack-Based Navigation
SetScreen(newScreen) // Navigate forward with history
SetScreenAndClearStack(newScreen) // Clear history for games
GoBack() // Return to previous screen
ClearStack() // Reset navigation

// Transition System  
ShowScreenTemporarily(tempScreen, targetScreen) // Loading screens
StartGameIfNeeded(gameScreen) // Automatic game startup
```

### **Game Integration** (Updated for proper initialization):
```csharp
// Game Startup
StartSinglePlayerGame() // Calls GameManager.StartNewSinglePlayerGame()
StartMultiPlayerGame() // Future: GameManager.InitializeMultiPlayerSystems()

// Button Handlers
Btn_PlaySinglePlayerLogic() // Single player game start
Btn_PlayMultiPlayerLogic() // Multiplayer (future implementation)
```

### **Screen Management**:
- **Menu Screens**: MainMenu, StudentInfo, Settings, etc.
- **Game Screens**: SinglePlayerGame, MultiPlayerGame  
- **Transition Screens**: Loading, Exiting
- **Stack History**: Automatic back navigation

### **Dependencies**: `GameManager` (game startup)

---

# 🤖 **AI System**

## **BasicComputerAI.cs** 🧠 **COMPUTER PLAYER**
**Purpose**: Simple AI for computer player decisions  
**Responsibility**: Card selection strategy, color choices (NO turn management, NO UI)

### **AI Strategy System**:
```csharp
// Decision Making
MakeDecision(topDiscardCard) // Main AI entry point
ExecuteDecision() // After thinking delay
GetValidCards(topDiscardCard) // Rule-based filtering
SelectBestCard(validCards) // Strategic selection

// Card Selection Strategy
SelectFromSpecialCards() // Priority: Taki > PlusTwo > Stop > Plus > ChangeDirection > ChangeColor
SelectFromNumberCards() // Prefer higher numbers with randomness
SelectColor() // Choose most common color in hand
```

### **AI Behavior**:
- **Thinking Time**: Configurable delay for natural feel
- **Special Card Preference**: 70% chance to prefer special over number cards
- **Strategic Priority**: Logical card type preferences
- **Fallback Logic**: Random selection if strategy fails

### **Hand Management**:
```csharp
// Hand Operations
AddCardsToHand(cards) // Receive dealt cards
AddCardToHand(card) // Single card (drawn)
ClearHand() // New game cleanup
GetHandCopy() // Safe copy for visual display
```

### **Integration**:
```csharp
// Events TO AI
MakeDecision() // Called by TurnManager via GameManager

// Events FROM AI  
OnAICardSelected // AI chose a card to play
OnAIDrawCard // AI needs to draw card
OnAIColorSelected // AI chose color for ChangeColor  
OnAIDecisionMade // AI made decision (for UI feedback)
```

### **Dependencies**: `GameStateManager` (rule validation)
### **Used By**: `GameManager` (game integration), `HandManager` (visual display)

---

# 📊 **Data & Configuration**

## **CardData.cs** 🃏 **CARD DEFINITION**
**Purpose**: ScriptableObject defining TAKI card properties  
**Responsibility**: Card data, rule validation, display formatting

### **Card Properties**:
```csharp
// Core Properties
public int number;           // 1-9 for numbered cards, 0 for special
public CardColor color;      // Red, Blue, Green, Yellow, Wild  
public CardType cardType;    // Number, Plus, Stop, ChangeDirection, etc.
public Sprite cardSprite;    // Visual representation
public string cardName;      // Display name
public bool isActiveCard;    // Can continue turn after playing
```

### **Helper Properties**:
```csharp
// Game Logic Helpers
bool IsSpecialCard // Not a numbered card
bool IsWildCard    // Can be played on any color  
bool CanChainPlusTwo // Can stack with other +2 cards
```

### **Rule Validation**:
```csharp
// Core Game Rule
CanPlayOn(topCard, currentColor) // Main rule validation logic

// Rule Implementation:
// 1. Wild cards can be played on anything
// 2. Color matching (current color or top card color)  
// 3. Number matching (both numbered cards, same number)
// 4. Special card type matching (same special type)
```

### **Display Formatting**:
```csharp
GetDisplayText() // "Red 5" or "Blue Stop"
```

**ScriptableObject**: 110 assets generated by `TakiDeckGenerator.cs`

---

## **Enums.cs** 📋 **GAME CONSTANTS**
**Purpose**: All enums used throughout the game  
**Responsibility**: Type safety and clear game state definitions

### **Multi-Enum State Architecture**:
```csharp
// WHO is acting?
enum TurnState { PlayerTurn, ComputerTurn, Neutral }

// WHAT special interaction is happening?
enum InteractionState { Normal, ColorSelection, TakiSequence, PlusTwoChain }

// WHAT is overall game status?
enum GameStatus { Active, Paused, GameOver }

// Game direction
enum TurnDirection { Clockwise, CounterClockwise }
```

### **Card System Enums**:
```csharp
// Card properties
enum CardColor { Red, Blue, Green, Yellow, Wild }
enum CardType { Number, Plus, Stop, ChangeDirection, ChangeColor, PlusTwo, Taki, SuperTaki }

// Player identification
enum PlayerType { Human, Computer }
```

**Architecture Benefit**: Clean separation of different state concerns

---

# 🛠️ **Utility & Development Scripts**

## **TakiDeckGenerator.cs** 🏭 **DECK GENERATION TOOL**
**Purpose**: Editor script to automatically generate all 110 CardData assets  
**Responsibility**: Asset creation, deck composition validation

### **Generation System**:
```csharp
// Asset Generation
CreateNumberCard(color, number, copyNumber) // 64 number cards
CreateSpecialCard(color, cardType, copyNumber) // 40 special cards  
CreateWildCard(cardType, copyNumber) // 6 wild cards

// Deck Composition (110 total):
// Numbers 1,3-9: 8 numbers × 4 colors × 2 copies = 64 cards
// Special cards: 5 types × 4 colors × 2 copies = 40 cards  
// Wild cards: SuperTaki ×2, ChangeColor ×4 = 6 cards
```

### **Turn Behavior Configuration** (FIXED):
```csharp
// isActiveCard assignments (CORRECTED):
Number cards: isActiveCard = false     // END turn after playing
Most special cards: isActiveCard = false // END turn after playing  
TAKI cards: isActiveCard = true        // CONTINUE turn (multi-card play)
SuperTaki cards: isActiveCard = true   // CONTINUE turn (multi-card play)
```

**Editor Integration**: Creates assets in `Resources/Data/Cards/`

---

## **TakiGameDiagnostics.cs** 🔍 **DEBUG TOOL**
**Purpose**: Comprehensive diagnostic system for debugging game flow  
**Responsibility**: System validation, rule testing, turn flow debugging

### **Diagnostic Categories**:
```csharp
// System Checks
CheckComponentReferences() // Verify all components connected
CheckDeckState() // Validate deck composition and state
CheckGameState() // Examine current game state
CheckTurnManagement() // Turn system status
CheckPlayerHand() // Hand contents and validation  
CheckAIState() // AI system status

// Interactive Testing
TestRuleValidation() // Test card rules against current state
TestTurnSequence() // Manual turn triggering
RunFullDiagnostics() // Complete system check
```

### **Usage**:
- **F1**: Full diagnostic check
- **F2**: Rule validation testing  
- **F3**: Turn sequence testing
- **Context Menu**: Manual diagnostic triggers

**Integration**: Finds and analyzes all game components automatically

---

## **DontDestroyOnLoad.cs** 🔒 **PERSISTENCE SYSTEM**
**Purpose**: Singleton pattern for objects that persist across scene loads  
**Responsibility**: Audio managers, settings, persistent data

### **Singleton Implementation**:
```csharp
// Duplicate Prevention
string uniqueTag // Identifier for this object type
// Automatically destroys duplicate instances
// Preserves original across scene transitions
```

**Usage**: Audio systems, persistent settings, global managers

---

# 🎵 **Audio & UI Utilities**

## **ButtonSFX.cs** 🔊 **BUTTON SOUND SYSTEM**
**Purpose**: Handles button sound effects consistently  
**Responsibility**: Audio feedback for button interactions

### **Audio Integration**:
```csharp
// Sound System
AudioSource sfxAudioSource // Main SFX controller
AudioClip buttonClickSound // Button click audio
PlayButtonSound() // Play sound effect

// Auto-Integration
// Automatically registers with button onClick event
// Uses PlayOneShot to avoid interrupting other SFX
```

**Usage**: Attach to any button GameObject for automatic sound feedback

---

## **MusicSlider.cs** & **SfxSlider.cs** 🎚️ **VOLUME CONTROLS**
**Purpose**: UI sliders for audio volume control  
**Responsibility**: Audio level management with visual feedback

### **Volume System**:
```csharp
// Audio Control
UpdateMusicVolume() / UpdateSfxVolume() // Called on slider change
AudioSource backgroundMusicSource / sfxAudioSource // Audio targets
TextMeshProUGUI musicAmountText / sfxAmountText // Visual feedback

// Settings
[Range(0f, 10f)] float defaultVolume = 10f // Initial volume
// Normalizes slider value to AudioSource volume (0-1)
```

**Integration**: Connected to slider OnValueChanged events in Inspector

---

## **DifficultySlider.cs** ⚙️ **DIFFICULTY SETTING**
**Purpose**: UI slider for game difficulty selection  
**Responsibility**: Difficulty setting with text feedback

### **Difficulty Levels**:
```csharp
// Difficulty Options
Slider value 1 → "Easy"
Slider value 2 → "Normal" (default)
Slider value 3 → "Hard"
```

**Usage**: Ready for AI difficulty integration when AI enhancements implemented

---

# 🛠️ **Utility & Development Scripts**

## **TakiLogger.cs** 🔍 **CENTRALIZED LOGGING SYSTEM**
**Purpose**: Centralized logging system with categorized, level-controlled output  
**Responsibility**: Replaces scattered Debug.Log calls with organized, configurable logging

### **Log Level System**:
```csharp
public enum LogLevel {
    None = 0,       // No logging
    Error = 1,      // Only errors
    Warning = 2,    // Errors + warnings  
    Info = 3,       // Errors + warnings + info
    Debug = 4,      // Errors + warnings + info + debug
    Verbose = 5     // Everything including verbose details
}
```

### **Log Categories**:
```csharp
public enum LogCategory {
    TurnFlow,       // Strict turn flow system
    CardPlay,       // Card playing and drawing
    GameState,      // Game state changes
    TurnManagement, // Turn switching and timing
    UI,             // UI updates and user interaction
    AI,             // Computer AI decisions
    Deck,           // Deck operations
    Rules,          // Rule validation
    System,         // System integration and events
    Diagnostics     // Debug and diagnostic information
}
```

### **Category-Specific Methods**:
```csharp
// Main logging methods
TakiLogger.LogTurnFlow(message)     // Turn system logging
TakiLogger.LogCardPlay(message)     // Card operations
TakiLogger.LogGameState(message)    // State changes
TakiLogger.LogAI(message)           // AI decisions
TakiLogger.LogUI(message)           // UI updates
TakiLogger.LogDeck(message)         // Deck operations
TakiLogger.LogSystem(message)       // System integration

// Direct level methods
TakiLogger.LogError(message)        // Always shown (unless None)
TakiLogger.LogWarning(message)      // Warning level
TakiLogger.LogInfo(message)         // Info level
```

### **Configuration**:
```csharp
// Runtime configuration
TakiLogger.SetLogLevel(LogLevel.Info)     // Set verbosity level
TakiLogger.SetProductionMode(true)        // Minimal logging for release
TakiLogger.GetLoggerInfo()                // Current configuration

// Production mode: Only shows Error and Warning levels
// Development mode: Shows all levels based on currentLogLevel setting
```

### **Integration Status**:
```csharp
// ✅ ORGANIZED LOGGING (Updated with TakiLogger):
BasicComputerAI.cs         // AI decision logging
GameManager.cs             // Turn flow and card play logging
DeckManager.cs             // Deck operation logging  
DeckUIManager.cs           // UI update logging
GameStateManager.cs        // State change logging
TurnManager.cs             // Turn management logging
GameplayUIManager.cs       // UI interaction logging
TakiGameDiagnostics.cs     // Diagnostic logging

// ⚠️ UNORGANIZED LOGGING (Still uses Debug.Log):
CardData.cs, Enums.cs, TakiDeckGenerator.cs, CardDataLoader.cs
Deck.cs, DontDestroyOnLoad.cs, GameSetupManager.cs
CardController.cs, DifficultySlider.cs, HandManager.cs
MenuNavigation.cs, PileManager.cs, ButtonSFX.cs
MusicSlider.cs, SfxSlider.cs
```

**Dependencies**: None (standalone utility)  
**Used By**: All major game systems for organized debug output

---

# 🔄 **Component Integration Patterns**

## **Event-Driven Communication**
```csharp
// Common Event Pattern:
// Component A: public System.Action<DataType> OnEventName;
// Component A: OnEventName?.Invoke(data);
// Component B: componentA.OnEventName += HandleEvent;
// Component B: void HandleEvent(DataType data) { ... }
```

### **Major Event Flows**:

#### **Game Startup Flow**:
```
MenuNavigation.StartSinglePlayerGame()
└→ GameManager.StartNewSinglePlayerGame()
   └→ GameManager.InitializeSinglePlayerSystems() 
      └→ DeckManager.SetupInitialGame()
         └→ OnInitialGameSetup Event
            └→ GameManager.OnInitialGameSetupComplete()
               └→ TurnManager.InitializeTurns()
```

#### **Turn Flow**:
```
TurnManager.StartTurn() 
└→ GameStateManager.ChangeTurnState()
   └→ OnTurnStateChanged Event
      └→ GameManager.OnTurnStateChanged()
         └→ GameplayUIManager.UpdateTurnDisplay()
         └→ Start strict turn flow control
```

#### **Card Play Flow**:
```  
CardController.OnCardButtonClicked()
└→ HandManager.HandleCardSelection()
   └→ OnCardSelected Event
      └→ GameManager.OnPlayerCardSelected()
         └→ Player clicks "Play Card" button
            └→ GameplayUIManager.OnPlayCardClicked Event
               └→ GameManager.OnPlayCardButtonClicked()
                  └→ GameManager.PlayCardWithStrictFlow()
```

#### **AI Turn Flow**:
```
TurnManager.OnComputerTurnReady Event
└→ GameManager.OnComputerTurnReady()
   └→ BasicComputerAI.MakeDecision()
      └→ OnAICardSelected Event
         └→ GameManager.OnAICardSelected()
            └→ TurnManager.EndTurn()
```

## **Coordinator Pattern Usage**

### **DeckManager Coordination**:
```csharp
// DeckManager delegates to:
DrawCard() → Deck.DrawCard()           // Pure deck operation
ShowMessage() → DeckUIManager.ShowMessage() // UI update
SetupGame() → GameSetupManager.SetupInitialGame() // Game logic
GetStats() → CardDataLoader.GetDeckStats() // Resource info
```

### **GameManager Coordination**:
```csharp  
// GameManager orchestrates:
Game State → GameStateManager         // Rules and state
Turn Flow → TurnManager              // Turn transitions
AI Logic → BasicComputerAI           // Computer decisions  
UI Updates → GameplayUIManager       // Player interface
Visual Cards → HandManager × 2       // Player + Computer hands
Deck Operations → DeckManager        // All deck functionality
```

## **Data Flow Patterns**

### **Visual Card Data Flow**:
```
CardData (ScriptableObject)
└→ CardController.InitializeCard() 
   └→ CardController.LoadCardImages()
      └→ HandManager.CreateCardPrefabs()
         └→ HandManager.UpdateHandDisplay()
            └→ GameManager.UpdateAllUI()
```

### **Rule Validation Flow**:
```
CardData.CanPlayOn() ← Core rule implementation
└→ GameStateManager.IsValidMove() ← Game context validation
   └→ HandManager.UpdatePlayableStates() ← Visual feedback
      └→ CardController.SetPlayable() ← Individual card tinting
```

---

# 🚨 **Known Issues & Fixes**

## **Current Issues Identified**:

### **1. Excessive Logging** 📝  
**Issue**: Console filled with debug messages  
**Solution**: Implement log level system, reduce verbose debugging

---

# 📋 **Development Workflow Guide**

## **Script Selection for Common Tasks**

### **🎮 Gameplay Changes**:
- **New Card Effects**: Modify `GameManager.LogCardEffectRules()` and `CardData.CanPlayOn()`
- **AI Improvements**: Focus on `BasicComputerAI.SelectBestCard()`  
- **Rule Changes**: Update `GameStateManager.IsValidMove()`
- **Turn Flow**: Modify `GameManager` strict flow methods

### **🎨 Visual Changes**:
- **Card Appearance**: `CardController.LoadCardImages()` and image paths
- **Hand Layout**: `HandManager.ArrangeCards()` and spacing calculations
- **UI Updates**: `GameplayUIManager` for gameplay, `DeckUIManager` for deck
- **Pile Display**: `PileManager` visual representation

### **🔧 System Changes**:
- **Game Setup**: `GameSetupManager` and `DeckManager.SetupInitialGame()`
- **State Management**: `GameStateManager` for rules and state
- **Component Integration**: `GameManager` event connections
- **Menu System**: `MenuNavigation` for screen transitions

### **🐛 Debugging**:
- **Start Here**: `TakiGameDiagnostics.RunFullDiagnostics()` (F1 key)
- **Rule Testing**: `TakiGameDiagnostics.TestRuleValidation()` (F2 key)
- **Turn Issues**: `TakiGameDiagnostics.TestTurnSequence()` (F3 key)
- **Manual Checks**: Context menu options on diagnostic script

## **Modification Safety Guidelines**

### **⚠️ High Risk Changes** (Test thoroughly):
- `GameManager` event connections  
- `GameStateManager` rule validation
- `TurnManager` turn switching logic
- `DeckManager` component coordination

### **✅ Low Risk Changes** (Safe to modify):
- UI text and styling
- AI strategy parameters
- Visual card spacing and sizing
- Audio settings and volume controls
- Menu navigation and transitions

### **🔒 Don't Modify** (Unless necessary):
- Core event system patterns
- Multi-enum state architecture  
- Card resource loading paths
- Strict turn flow control logic

---

# 📊 **Performance Characteristics**

## **System Performance**:

### **Visual Card System**:
- **Hand Display**: Efficient with 8+ cards, instant updates
- **Image Loading**: Cached Resources loading, one-time cost per card
- **Layout Calculation**: O(n) complexity for card positioning
- **Memory Usage**: Low - reuses prefabs, destroys unused cards

### **Rule Validation**:
- **Card Matching**: O(1) rule checking per card
- **Hand Analysis**: O(n) where n = hand size
- **AI Decision**: O(n*m) where n = hand size, m = rule complexity

### **Event System**:
- **Event Overhead**: Minimal - lightweight Action delegates
- **Update Frequency**: On-demand updates, no continuous polling
- **Memory Management**: Automatic cleanup, no memory leaks detected

## **Scalability Notes**:
- **Current**: Optimized for 2-player gameplay
- **Expandable**: Architecture supports multiplayer extension
- **Resource**: Resource loading scales with total unique cards (110)
- **UI**: UI system handles dynamic hand sizes efficiently

---

# 🏁 **Next Steps & Recommendations**

## **Immediate Actions** (Milestone 8):
1. **✅ DONE**: Complete script documentation (this document)
2. **🔍 INVESTIGATE**: Fix "Card data is null!" issue in DeckMessageText
3. **🧹 CLEANUP**: Implement log level system to reduce console spam
4. **📝 OPTIMIZE**: Create quick reference cards for common modifications

## **Upcoming Milestones**:

### **Milestone 9**: Pause System
- **Focus Scripts**: `GameStateManager`, `TurnManager`, `GameplayUIManager`
- **New State**: Add pause handling to GameStatus enum
- **UI Integration**: Connect pause button to game state

### **Milestone 10**: Game End Screen  
- **Focus Scripts**: `GameManager`, `GameplayUIManager`, `MenuNavigation`
- **Win Detection**: Enhance win condition handling
- **Screen Transitions**: Smooth game-to-menu flow

### **Milestone 11**: UI Restructuring
- **Focus Scripts**: `GameplayUIManager`, `DeckUIManager`, Scene hierarchy
- **Naming Cleanup**: Fix UI element names for clarity
- **Component Validation**: Ensure all references maintained

## **Long-term Architecture**:
- **Multiplayer Ready**: Current architecture supports extension
- **Special Cards**: Framework ready for complex card implementations  
- **AI Enhancement**: Modular AI system ready for improvements
- **Performance**: Solid foundation for additional features

---

# 📖 **Quick Reference Cards**

## **🚀 Emergency Fixes**
```csharp
// Game won't start? Check GameManager component references
GameManager.ValidateAndConnectComponents()

// UI buttons not working? Force button state sync
GameplayUIManager.ForceUISync() 

// Cards not playable? Refresh hand states  
HandManager.RefreshPlayableStates()

// AI not responding? Check turn manager state
TurnManager.CurrentPlayer and TurnManager.IsComputerTurnPending
```

## **🔧 Common Code Patterns**
```csharp
// Add new card effect in GameManager:
case CardType.NewEffect:
    Debug.Log("RULE: NewEffect card - [describe rule]");
    gameplayUI?.ShowComputerMessage("NewEffect message");
    // Implement effect logic
    break;

// Add new UI element in GameplayUIManager:
public void UpdateNewDisplay(data) {
    if (newDisplayElement != null) {
        newDisplayElement.text = data.ToString();
    }
}

// Add new AI strategy in BasicComputerAI:
// Modify SelectFromSpecialCards() or SelectFromNumberCards()
```

---

**📄 Document Status**: ✅ Complete - Covers all 24+ scripts  
**🎯 Current Milestone**: 8 (Documentation & Cleanup)  
**📅 Last Updated**: Based on Milestone 7 completion  
**🔄 Next Review**: After Milestone 9 (Pause System) completion
```

```
# TAKI Game Development Plan - Unity Engine
## Comprehensive Implementation Guide

### ⚠️ CRITICAL NOTES
- **AVOID UNICODE**: No special characters in code, file names, text displays, or comments
- **Current Status**: Phase 1-4 Complete ✅, Currently at **Code Cleanup & Logging Improvements** 🎯
- **Target Platform**: PC/Desktop Unity Build
- **Scope**: Singleplayer (Human vs Computer) with multiplayer-ready architecture

---

## Project Structure

### Scripts Organization:
```
Scripts/
├── Controllers/
├── Core/
│   ├── AI/
│   │   └── BasicComputerAI.cs
│   └── GameManager.cs
├── Data/
│   ├── CardData.cs
│   └── Enums.cs
├── Editor/
│   └── TakiDeckGenerator.cs
├── Managers/
│   ├── CardDataLoader.cs
│   ├── Deck.cs
│   ├── DeckManager.cs
│   ├── DeckUIManager.cs
│   ├── DontDestroyOnLoad.cs
│   ├── ExitValidationManager.cs
│   ├── GameEndManager.cs
│   ├── GameSetupManager.cs
│   ├── GameStateManager.cs
│   ├── PauseManager.cs
│   └── TurnManager.cs
├── UI/
│   ├── CardController.cs
│   ├── DifficultySlider.cs
│   ├── GameplayUIManager.cs
│   ├── HandManager.cs
│   ├── MenuNavigation.cs
│   └── PileManager.cs
├── ButtonSFX.cs
├── MusicSlider.cs
├── SfxSlider.cs
├── TakiGameDiagnostics.cs
└── TakiLogger.cs
```

### Assets Structure:
```
Assets
├── Audio
│   ├── Music
│   └── Sfx
├── Data
│   ├── Cards
├── Plugins
└── Prefabs/
│   └── Cards/
│   │   └── CardPrefab.prefab      ← Visual card prefab
│   └── UI
Resources/
├── Data/
│   └── Cards/                     ← 110 CardData assets
├── Sprites/
│   └── Cards/
│       ├── Backs/
│       │   └── card_back.png      ← Single back image
│       └── Fronts/
│           ├── Red/               ← Red cards
│           ├── Blue/              ← Blue cards  
│           ├── Green/             ← Green cards
│           ├── Yellow/            ← Yellow cards
│           └── Wild/              ← Wild cards
├── Scenes
├── Scripts
└── TextMesh Pro
```

### Scene Hierarchy:
```
Scene_Menu
├── Main Camera
├── Canvas
│   ├── Img_Background
│   ├── Screen_MainMenu
│   ├── Screen_StudentInfo
│   ├── Screen_SinglePlayer
│   ├── Screen_MultiPlayer
│   ├── Screen_SinglePlayerGame
│   │   ├── Player1Panel (Human Player)
│   │   │   ├── Player1HandPanel - (Components: HandManager)
│   │   │   └── Player1ActionPanel
│   │   │       ├── Btn_Player1PlayCard - Play selected card
│   │   │       ├── Btn_Player1DrawCard - Draw from deck
│   │   │       ├── Btn_Player1EndTurn - End current turn
│   │   │       └── Player1HandSizePanel
│   │   │           └── Player1HandSizeText - Hand size display
│   │   ├── Player2Panel (Computer Player)
│   │   │   ├── Player2HandPanel - (Components: HandManager)
│   │   │   └── Player2ActionPanel
│   │   │       ├── Player2MessageText - Computer actions and thinking
│   │   │       └── Player2HandSizePanel 
│   │   │           └── Player2HandSizeText - Computer hand size
│   │   ├── GameBoardPanel
│   │   │   ├── DrawPilePanel
│   │   │   │   └── DrawPileCountText - Draw pile count
│   │   │   ├── DiscardPilePanel
│   │   │   │   └── DiscardPileCountText - Discard pile count
│   │   │   └── Btn_Player1EndTakiSequence
│   │   ├── GameInfoPanel
│   │   │   ├── TurnIndicatorText - Current turn display
│   │   │   ├── DeckMessageText - Deck event messages
│   │   │   └── GameMessageText - General game feedback
│   │   ├── ColorSelectionPanel - Color choice UI
│   │   │   ├── Btn_SelectRed
│   │   │   ├── Btn_SelectBlue
│   │   │   ├── Btn_SelectGreen
│   │   │   └── Btn_SelectYellow
│   │   ├── CurrentColorIndicator - Active color display
│   │   ├── Btn_Exit - Exit completely (not return to Main Menu)
│   │   ├── Btn_Pause - Pause functionality
│   │   └── Screen_GameEnd - Game over popup
│   │       ├── GameEndMessage - Winner announcement
│   │       ├── Btn_PlayAgain - Start new game
│   │       └── Btn_ReturnToMenu - Back to main menu
│   ├── Screen_MultiPlayerGame
│   ├── Screen_Settings
│   ├── Screen_ExitValidation
│   │   └── Image
│   │       ├── Text (TMP)
│   │       ├── Btn_ExitConfirm
│   │       └── Btn_ExitCancel
│   ├── Screen_Paused
│   │   └── Image
│   │       ├── Text (TMP)
│   │       ├── Btn_Continue
│   │       ├── Btn_Restart
│   │       └── Btn_GoHome
│   ├── Screen_GameEnd
│   │   └── Image
│   │       ├── EndDeclarationText
│   │       ├── Btn_Restart
│   │       └── Btn_GoHome
│   ├── Screen_Loading
│   └── Screen_Exiting
├── EventSystem
├── GameObject
├── MenuManager
├── BackgroundMusic
├── SFXController
└── GameManager
```

---

## Phase 1: Foundation Setup ✅ COMPLETE

### Milestone 1: Menu System ✅ COMPLETE
**Status**: All scenes and navigation working

### Milestone 2: UI Framework Creation ✅ COMPLETE  
**Status**: Full UI hierarchy established, all panels created

---

## Phase 2: Core Card System ✅ COMPLETE

### Milestone 3: Data Architecture Implementation ✅ COMPLETE
**Achievements**:
- ✅ Complete enum system with **Multi-Enum Architecture**:
  - `TurnState`: WHO is acting? (PlayerTurn, ComputerTurn, Neutral)
  - `InteractionState`: WHAT special interaction? (Normal, ColorSelection, TakiSequence, PlusTwoChain) 
  - `GameStatus`: WHAT is overall status? (Active, Paused, GameOver)
- ✅ CardData ScriptableObject with helper methods and rule validation
- ✅ Namespace organization (`TakiGame`)
- ✅ 110-card complete deck system with automatic generation
- ✅ UI integration tested and working

### Milestone 4: Complete Deck System ✅ COMPLETE
**Achievements**:
- ✅ **Refactored Architecture** using **Single Responsibility Principle**:
  - `Deck`: Pure card operations (draw, discard, shuffle)
  - `CardDataLoader`: Resource management (load 110 cards from Resources)
  - `DeckUIManager`: UI updates only (deck counts, messages) 
  - `GameSetupManager`: Game initialization logic (deal hands, place starting card)
  - `DeckManager`: Coordinator pattern (delegates to specialized components)
- ✅ All 110 cards load and distribute correctly (8+8+1 setup working)
- ✅ Automatic deck initialization and UI updates
- ✅ **Wild as initial color** (represents "no color set yet")
- ✅ Event-driven architecture connecting all components
- ✅ Clean separation of concerns for future multiplayer readiness

### Milestone 5: Turn Management System ✅ COMPLETE
**Achievements**:
- ✅ **Multi-Enum Game State Architecture**:
  - `GameStateManager`: Manages TurnState, InteractionState, GameStatus, active color, rules
  - `TurnManager`: Handles turn switching, timing, player transitions
  - `BasicComputerAI`: Simple AI with strategic card selection
  - `GameplayUIManager`: Turn-related UI updates, player actions, color selection
  - `GameManager`: Main coordinator for all gameplay systems
- ✅ All gameplay components properly integrated on GameManager GameObject
- ✅ Multi-enum state transitions working correctly
- ✅ Turn switching between Human ↔ Computer functioning
- ✅ UI updates reflecting current game state accurately
- ✅ Computer AI making decisions and playing cards
- ✅ Basic card play validation working
- ✅ Draw card functionality working for both players
- ✅ Hand size tracking and display working
- ✅ Event system connecting all components properly
- ✅ Color selection system functional
- ✅ **Clean UI Ownership Architecture**:
  - GameplayUIManager: Turn system, player actions, computer feedback
  - DeckUIManager: Deck counts and deck event messages only

---

## Phase 3: Visual Card System ✅ COMPLETE

### Milestone 6: Interactive Visual Cards ✅ COMPLETE
**Achievements**:
- ✅ **Complete Visual Card System**:
  - `CardController`: Individual card behavior with real scanned images
  - `HandManager`: Dynamic hand display with adaptive spacing  
  - `PileManager`: Draw/discard pile visual cards
- ✅ **CardPrefab Architecture**:
  - Face-up/face-down instant image swapping (no animations)
  - Click selection with 10px Y-offset movement
  - Gold/red tint feedback for valid/invalid cards
  - Professional 100px height, calculated 67px width
- ✅ **Hand Display System**:
  - Manual positioning with adaptive spacing algorithm
  - Player hand: Face-up cards with selection
  - Computer hand: Face-down cards for privacy
  - Instant prefab add/remove with position recalculation
- ✅ **Pile Visual System**:
  - Draw pile: Face-down card when not empty
  - Discard pile: Face-up current top card
  - Integrated with DeckUIManager through PileManager
- ✅ **Image Architecture Consistency**:
  - Fixed folder structure: `Wild/` instead of `Special/`
  - Consistent naming: Wild cards no color suffix
  - All cards use real scanned images from Resources
- ✅ **Performance & Integration**:
  - Smooth gameplay with 8+ cards in hand
  - All existing Milestone 5 functionality preserved
  - Event-driven integration with GameManager
  - No memory leaks or performance issues

---

## Phase 4: Strict Turn Flow System ✅ COMPLETE

### Milestone 7: Enhanced Card Rules with Strict Turn Flow ✅ COMPLETE
**Achievements**:
- ✅ **Strict Turn Flow Implementation**:
  - Player must take ONE action (PLAY or DRAW) then END TURN
  - END TURN button disabled until action taken
  - DRAW button disabled after playing card
  - All special cards logged but act as basic cards
  - Immediate button disable on click to prevent multiple actions
- ✅ **Comprehensive Card Effect Logging**:
  - All special card rules documented in console
  - Clear feedback for player actions and constraints
  - Safe testing environment for all card types
- ✅ **Enhanced Button Control System**:
  - Smart button state management based on game flow
  - Clear visual feedback for valid/invalid actions
  - Bulletproof turn completion enforcement
- ✅ **Rule Validation Working**:
  - Color matching validation functional
  - Number matching validation functional
  - Wild card acceptance working
  - Basic special card type matching working

---

## Phase 5: Code Quality & Polish ✅ COMPLETE

### **Milestone 8: Code Cleanup & Logging Improvements** ✅ COMPLETE
**Status**: **✅ COMPLETED** - TakiLogger system implemented successfully

### **Achievements**:
- ✅ **Centralized Logging System**: `TakiLogger.cs` utility class created
- ✅ **Log Level Control**: Configurable verbosity (None, Error, Warning, Info, Debug, Verbose)
- ✅ **Categorized Logging**: System-specific logging categories (TurnFlow, CardPlay, AI, UI, etc.)
- ✅ **Production Mode**: Clean output toggle for release builds
- ✅ **Core Systems Updated**: 8 major scripts updated with organized logging:
  - `BasicComputerAI.cs`, `GameManager.cs`, `DeckManager.cs`, `DeckUIManager.cs`
  - `GameStateManager.cs`, `TurnManager.cs`, `GameplayUIManager.cs`, `TakiGameDiagnostics.cs`
- ✅ **Performance Optimized**: Conditional logging prevents unnecessary string operations
- ✅ **Clean Console Output**: Organized debug messages with category prefixes

### **Logging Architecture**:
```csharp
// Category-based logging system
TakiLogger.LogTurnFlow("Strict turn flow messages")
TakiLogger.LogCardPlay("Card play and draw operations") 
TakiLogger.LogAI("Computer decision making")
TakiLogger.LogUI("User interface updates")
TakiLogger.LogGameState("State transitions")
TakiLogger.SetLogLevel(LogLevel.Info) // Runtime configuration
```

---

## Phase 6: Game Flow Enhancement

### Next Milestone: Pause System Implementation
**Objective**: Implement functional pause button with proper game state management

**Tasks**:
- Create pause screen UI overlay
- Implement pause/resume functionality in GameStateManager  
- Handle pause state in turn system and AI
- Add pause button integration
- Test pause during different game states

### Future Milestone: Game End Screen System
**Objective**: Professional game over experience with proper flow control

**Tasks**:
- **Create Game End Screen UI**:
  - Winner announcement display
  - Play Again button functionality
  - Return to Main Menu button functionality
- **Implement End Game Logic**:
  - Proper win condition detection
  - Game state cleanup on game end
  - Smooth transitions between screens
- **Integration Testing**:
  - Test all game end scenarios
  - Verify button functionality
  - Ensure proper state resets

---

## Phase 7: Special Cards Implementation

### Future Milestone: Basic Special Cards - Real Implementation
**Objective**: Implement actual special card effects (after basic gameplay confirmed working)

**Implementation Order**:
1. **Plus Card**: 
- A player plays PLUS
- If this card is played during a TAKI sequence and this is not the last card:
	- No special effect.
- If this card is NOT played during a TAKI sequence OR if this card is played during a TAKI sequence and this is the last card (player clicked on ENDTAKISEQUENCE after putting this card):
	- Now this player must PLAY 1 additional card, the player may also choose to DRAW a card if they don't wish to play one. 
	- In short: When played, player gets an additional action, this is not optional. 
	- can't end turn before the action is taken, even if the PLUS card is the last card in the hand.

2. **Stop Card**: 
- A player plays STOP
- If this card is played during a TAKI sequence and this is not the last card:
	- No special effect.
- If this card is NOT played during a TAKI sequence OR if this card is played during a TAKI sequence and this is the last card (player clicked on ENDTAKISEQUENCE after putting this card):
	- The opponent's turn is SKIPPED and so the player basically get's another entirely new turn. 
	- In short: Skip opponent's turn, AKA recive another free and new turn - but make sure to properly show the fitting messages.

3. **ChangeDirection Card**: 
- A player plays CHANGEDIRECTION
- If this card is played during a TAKI sequence and this is not the last card:
	- No special effect.
- If this card is NOT played during a TAKI sequence OR if this card is played during a TAKI sequence and this is the last card (player clicked on ENDTAKISEQUENCE after putting this card):
	- Reverse the direction. 
	- We will implement this with the flags and messages, 
	- But take note that this has zero affect on the real logic of the game (We only deal with 2 players, not more, so changing the direction doesn't matter).

4. **PlusTwo Card**: 
- A player plays PLUSTWO
- If this card is played during a TAKI sequence and this is not the last card:
	- No special effect.
- If this card is NOT played during a TAKI sequence OR if this card is played during a TAKI sequence and this is the last card (player clicked on ENDTAKISEQUENCE after putting this card):
	- A chain begins, last player who plays PLUSTWO is tracked (player), NumberOfChainedPlusTwos = 1.
	- Now the opponent has 2 options:
		- Option 1: DRAW NumberOfChainedPlusTwos*2 cards 
			- No need to click a few times, once is enugh -> we must make sure that in this scenario the DRAW button will approprietly call DrawCards.
			- Then the opponent click END TURN, and the chain breaks.
		- Option 2: PLAY a PLUSTWO card 
			- In this scenario the PLAY button will ONLY accept a PLUSTWO card.
			- Then the opponent click END TURN, and the chain updates: last player who plays PLUSTWO is tracked (opponent), NumberOfChainedPlusTwos++.
			- And the cycle goes on until someone DRAWs and breaks the chain.
			- For AI logic: If AI has a PLUSTWO in hand, and a chain starts- the AI wil always RESPOND with a PLUSTWO instead of DRAW if the possibilty exists for them.

5. **ChangeColor Card**: Full color selection implementation
- A player plays CHANGECOLOR
- If this card is played during a TAKI sequence and this is not the last card:
	- No special effect.
- If this card is NOT played during a TAKI sequence OR if this card is played during a TAKI sequence and this is the last card (player clicked on ENDTAKISEQUENCE after putting this card):
	- Now PLAY and DRAW buttons are disabled
	- ColorSelectionPanel and its buttons are enabled
	- Player can press any of the 4 buttons and CurrentColorIndicator will update accordingly
	- When player clicks on ENDTURN ColorSelectionPanel and its buttons are disabled

6. **Taki Card**: Multi-card play sequence of same color
- A player plays TAKI
- If this card is played during a TAKI sequence and this is not the last card:
	- No special effect.
	- This is a little funny to say, but still holds true.
- If this card is NOT played during a TAKI sequence OR if this card is played during a TAKI sequence and this is the last card (player clicked on ENDTAKISEQUENCE after putting this card):
	- A chain begins, last card played is tracked, NumberOfChainedCards = 0.
	- Now the player has 2 possible actions:
		- action 1: PLAY another legal card
			- What does it mean to play a legal card in a TAKISEQUENCE?
				- In a TAKISEQUENCE, a legal card to play means that the card is either the same color 	as the CurrentColorIndicator or is a wild color type. 
				- If player Played Blue Taki, and then Blue Four -> The player can NOT play a Red four (Even tho outside of a TAKISEQUENCE it WOULD be possible) 
				- You can't change the color in a middle of a TAKISEQUENCE, UNLESS you play CHANGECOLOR as the last card, click on ENDTAKISEQUENCE - Althogh you can technically say that the ColorSelectionPanel is only enabled after the TAKISEQUENCE ends, so we are still following the rules
			- Player plays a legal card, last card played is tracked, NumberOfChainedCards++.
		- action 2: Click on ENDTAKISEQUENCE
			- Player clicks on ENDTAKISEQUENCE
			- The last card played acts as tho it was just played now, And if it's special then it will be activated (this is more talked on in the other Special cards above as well)
				- If a player puts TAKI card as a last card of a previus TAKISEQUENCE -> no problem
				- Because FIRST TAKISEQUENCE ends, and only after that we approach the last card's personality.
			- And eventually the player will click on ENDTURN.
			- Of course player can even click on ENDTAKISEQUENCE immidietly.

7. **SuperTaki Card**: Multi-card play sequence of any color
	- This is the EXACT same funtionality as a colored taki.
	- In both cases the cards in the TAKISEQUENCE must fit the color of CurrentColorIndicator.
	- For example:
		- I can only PLAY Blue Taki if CurrentColorIndicator = Blue
		- I can PLAY Super Taki on any CurrentColorIndicator = color
		- In both cases the cards will have to obey CurrentColorIndicator's color
		- Super Taki can't change CurrentColorIndicator's color

### Future Milestone: Advanced Special Card Mechanics
**Objective**: Complex card interactions and chaining

**Tasks**:
- PlusTwo stacking system
- Taki sequence validation
- Special card combination rules
- Edge case handling for special cards
- AI strategy for special cards

---

## Phase 8: Final Polish & Release Preparation

### Future Milestone: Final Polish & Testing
**Objective**: Complete game polish for release

**Tasks**:
- Performance optimization
- Final UI polish and animations
- Audio integration testing
- Complete gameplay testing
- Build preparation and testing
- Final bug fixes and stability improvements

---

## Current Architecture Highlights

### **Strict Turn Flow System** (Enhanced):
```csharp
// Bulletproof turn control with comprehensive logging
- Player takes ONE action (PLAY or DRAW)
- Action buttons immediately disabled on click
- END TURN button enabled only after action
- Clear feedback for all game states
- Special card effects logged but simplified for testing
- Enhanced button state tracking and validation
```

### **Multi-Enum State Management**:
```csharp
// Clean separation of state concerns
public enum TurnState { PlayerTurn, ComputerTurn, Neutral }
public enum InteractionState { Normal, ColorSelection, TakiSequence, PlusTwoChain }
public enum GameStatus { Active, Paused, GameOver }
```

### **Visual Card Architecture**:
```csharp
// Complete visual card system
CardController: Individual card behavior, image loading, selection
HandManager: Dynamic hand layout, card positioning, user interaction
PileManager: Draw/discard pile visual representation
```

### **Clean UI Messaging System**:
```csharp
// Proper message routing with updated hierarchy
GameplayUIManager.ShowPlayerMessage(): Instructions and warnings to player
GameplayUIManager.ShowComputerMessage(): AI actions and thinking messages
DeckUIManager: Deck-specific events and counts only
```

---

## Development Guidelines

### Architecture Principles
- **Separation of Concerns**: Each component has single responsibility
- **Event-Driven Communication**: Components communicate via events
- **Coordinator Pattern**: Managers delegate to specialized components  
- **Multi-Enum State**: Separate enums for different state aspects
- **Visual-Data Separation**: CardData separate from visual representation
- **Strict Turn Flow**: One action per turn with enforced completion
- **Clean Logging**: Categorized, level-controlled debugging information

### Current Development Workflow
1. **Start with Code Cleanup**: Implement log level system before new features
2. **Test in Controlled Environment**: Use strict turn flow for safe testing
3. **Minimal Console Spam**: Keep debugging focused and relevant
4. **Document Changes**: Update development plan when making modifications
5. **Preserve Architecture**: Maintain clean separation of concerns

---

## Success Metrics

### Immediate Success Criteria (Code Cleanup) 🎯 CURRENT TARGET
- ✅ **Clean Console Output**: Only essential information during gameplay
- ✅ **Categorized Logging**: Different log types for different systems
- ✅ **Configurable Verbosity**: Adjustable log levels for debugging
- ✅ **Performance Improvement**: No unnecessary string operations
- ✅ **Production Ready**: Clean output suitable for release

### Phase 6 Success Criteria
- ✅ **Functional Pause System**: Working pause/resume with proper state management
- ✅ **Professional Game End**: Smooth winner announcement and menu flow
- ✅ **Stable Gameplay**: No crashes or UI inconsistencies

### Phase 7 Success Criteria
- ✅ **Real Special Cards**: All special card effects implemented correctly
- ✅ **Complex Interactions**: Card chaining and combination rules working
- ✅ **AI Integration**: Computer AI handles all special cards intelligently

### Overall Project Success  
- Complete playable TAKI game (Human vs Computer)  
- All special card types implemented correctly  
- Intuitive UI with clear visual feedback  
- Stable gameplay without crashes  
- Clean, maintainable, well-documented code architecture  
- Code ready for multiplayer extension  
- Professional visual presentation with real card images
- Efficient development workflow with clean debugging

---

## Current Status Summary

**✅ COMPLETED**:
- **Phase 1**: Complete foundation (Menu + UI Framework)
- **Phase 2**: Complete card system (Data + Deck + Turn Management)  
- **Phase 3**: Complete visual system (Interactive cards + Hand management + Pile visuals)
- **Phase 4**: Complete strict turn flow system with enhanced button control
- All 110 cards loading with real scanned images
- Multi-enum state management working perfectly
- Bulletproof turn-based gameplay with visual cards
- Computer AI making strategic decisions
- Professional visual card system with adaptive layouts
- Safe testing environment with comprehensive logging
- Enhanced UI message routing system

**🎯 CURRENT FOCUS**:
- **IMMEDIATE**: Code cleanup and logging improvements
- Implement proper log level system for clean console output
- Categorize debug messages by system type
- Optimize performance by reducing unnecessary logging
- Create production-ready logging configuration

**🚀 UPCOMING PHASES**:
- Pause system and game end screens
- Real special card implementation (after logging cleanup)
- Final polish and release preparation

**📋 PRIORITY ORDER**:
1. Code cleanup & logging improvements (Current Focus)
2. Pause button implementation
3. Game end screen system
4. Special cards implementation  
5. Final polish & release preparation

I've updated the TAKI Game Development Plan to accurately reflect your current progress. Here are the key changes:

## ✅ **Completed and Removed**:
- **Milestone 11** (UI Hierarchy Restructuring) - Removed entirely ✅
- **Script Documentation milestone** - Removed entirely ✅  
- Updated scene hierarchy to reflect your current structure

## 🎯 **Current Focus Updated**:
- **Primary objective**: Code Cleanup & Logging Improvements
- **Immediate tasks**: Implement log level system, clean console output, categorize debug messages
- **Goal**: Transform excessive logging into organized, configurable system

## 📋 **Key Changes Made**:

### **Updated Status Summary**:
- All Phase 1-4 work marked as complete
- Current focus clearly identified as logging cleanup
- Future phases properly sequenced

### **Enhanced Architecture Documentation**:
- Added your strict turn flow system details
- Updated UI messaging system with proper routing
- Reflected your current component structure

### **Clean Development Path**:
```
Current: Code Cleanup & Logging → 
Next: Pause System → 
Then: Game End Screens → 
Future: Special Cards Implementation
```

### **Specific Implementation Plan**:
- Create `TakiLogger` utility class with log levels
- Categorize logs by system (Turn Flow, Card Play, AI, UI)
- Add production mode toggle
- Optimize performance by removing debug string operations

```

---

# Update `TAKI Game - Complete Script Documentation & Reference Guide`

- Add and document the 3 new files.
- Update all other relevant files.

# Update `TAKI Game Development Plan - Unity Engine`
	
- `Phase 6` complete.
- Next stop is `Phase 7`.
- I updated the `Scene Hierarchy` and the `Scripts Organization`.
- I also added the new button `Btn_Player1EndTakiSequence`.

Right now, all the cards in the game act the same way as the basic card.
We need to appropriately take care of the special cards' personality - As the first prompt we will ONLY modify PLUS, STOP, CHANGEDIRECTION, CHANGECOLOR, and NOT touch PLUSTWO, TAKI and SUPERTAKI.















I  want to play it safely, so here are my rules - user story:

1. Player turn starts
2. END TURN button is disabled
3. If player has no valid cards to play at all:
  1. Log a clear message - Player has no valid cards, must draw a card
  2. Player clicks on DRAW
  3. Log a clear message - Player has no more valid moves, must end turn
  4. Player clicks on END TURN
  5. Turn ends
4. If player has valid cards:
  1. Log a clear message - Player has valid cards, Play or DRAW a card draw a card
  2. If Player clicks on DRAW
    1. Log a clear message - Player has no more valid moves, must end turn
    2. Player clicks on END TURN
    3. Turn ends
  3. If Player selects a valid card and clicks on PLAY
    1. If the card is NUMBER:
      1. DRAW button is disabled
      2. Log a clear message - Player has no more valid moves, must end turn
      3. Player clicks on END TURN
      4. Turn ends
    2. If the card is PLUS (for now):
      1. Log a clear message - RULES: Player must PLAY another card
      2. DRAW button is disabled
      3. Log a clear message - Player has no more valid moves, must end turn
      4. Player clicks on END TURN
      5. Turn ends
    3. If the card is STOP (for now):
      1. Log a clear message - RULES: Opponent's turn is stopped/skipped, Player starts another NEW turn
      2. DRAW button is disabled
      3. Log a clear message - Player has no more valid moves, must end turn
      4. Player clicks on END TURN
      5. Turn ends
    4. If the card is CHANGE DIRECTION (for now):
      1. Log a clear message - RULES: The directions must change from ... to ...
      2. DRAW button is disabled
      3. Log a clear message - Player has no more valid moves, must end turn
      4. Player clicks on END TURN
      5. Turn ends
    5. If the card is PLUS TWO (for now):
      1. Log a clear message - RULES: Player must DRAW Xx2=2X (e.g. 3x2=6, will usually be 1x2=2) cards
      2. DRAW button is disabled
      3. Log a clear message - Player has no more valid moves, must end turn
      4. Player clicks on END TURN
      5. Turn ends
    6. If the card is TAKI (for now):
      1. Log a clear message - RULES: Player may PLAY a series of cards the color of COLOR
      2. DRAW button is disabled
      3. Log a clear message - Player has no more valid moves, must end turn
      4. Player clicks on END TURN
      5. Turn ends
    7. If the card is CHANGE COLOR (for now): 
      1. Log a clear message - RULES: Player must choose a color
        1. ColorSelectionPanel will not even appear
      2. DRAW button is disabled
      3. Log a clear message - Player has no more valid moves, must end turn
      4. Player clicks on END TURN
      5. Turn ends
    8. If the card is SUPER TAKI (for now):
      1. Log a clear message - RULES: Player may PLAY a series of cards the color of COLOR
      2. DRAW button is disabled
      3. Log a clear message - Player has no more valid moves, must end turn
      4. Player clicks on END TURN
      5. Turn ends
    1. DRAW button is disabled
    2. Log a clear message - Player has no more valid moves, must end turn
    3. Player clicks on END TURN
    4. Turn ends

I want us to start basic, and safe - so as of now, ALL cards must act like BASIC cards and COMPLETELY obey the legal flow.
