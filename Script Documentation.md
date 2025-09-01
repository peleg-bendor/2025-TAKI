# TAKI Game - Complete Script Documentation & Reference Guide

## 📋 **Document Overview**
**Purpose**: Master reference for all scripts in the TAKI game project  
**Total Scripts**: 23 core scripts + utilities  
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