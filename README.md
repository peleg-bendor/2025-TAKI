# TAKI Game Development Plan - Unity Engine
## Comprehensive Implementation Guide

### ⚠️ CRITICAL NOTES
- **AVOID UNICODE**: No special characters in code, file names, text displays, or comments
- **Current Status**: Phase 1 Complete ✅, Phase 2 Complete ✅, Milestone 4 Complete ✅, Currently at Milestone 5 🎯
- **Target Platform**: PC/Desktop Unity Build
- **Scope**: Singleplayer (Human vs Computer) with multiplayer-ready architecture

---

## Project Structure

### Scripts Organization:
```
Scripts/
├── Controllers/
│   └── CardDataTester.cs
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
│   ├── GameSetupManager.cs
│   ├── GameStateManager.cs
│   └── TurnManager.cs
├── UI/
│   ├── DifficultySlider.cs
│   ├── GameplayUIManager.cs
│   └── MenuNavigation.cs
├── ButtonSFX.cs
├── MusicSlider.cs
└── SfxSlider.cs
```

### Scene Hierarchy:
```
Scene_Menu
├── Main Camera
├── Canvas
│   ├── Img_Background
│   ├── Screen_MainMenu
│   ├── Screen_StudentInfo
│   ├── Screen_Settings
│   ├── Screen_SinglePlayer
│   ├── Screen_MultiPlayer
│   ├── Screen_ExitValidator
│   ├── Screen_Loading
│   ├── Screen_Exiting
│   ├── Screen_SinglePlayerGame
│   │   ├── Player1Panel (Human Player)
│   │   │   ├── Player1HandPanel - for card prefab instantiation
│   │   │   └── Player1ActionPanel
│   │   │       ├── Btn_Player1PlayCard - Play selected card
│   │   │       ├── Btn_Player1DrawCard - Draw from deck
│   │   │       ├── Btn_Player1EndTurn - End current turn
│   │   │       └── Player1HandSizePanel
│   │   │           └── Player1HandSizeText - Hand size display
│   │   ├── Player2Panel (Computer Player)
│   │   │   ├── Player2HandPanel - for computer card prefabs
│   │   │   └── Player2MessagePanel
│   │   │       ├── Player2MessageText - AI action feedback
│   │   │       └── Player2HandSizePanel 
│   │   │           └── Player2HandSizeText - Computer hand size
│   │   ├── GameBoardPanel
│   │   │   ├── DrawPilePanel - draw pile card display
│   │   │   └── DiscardPilePanel - discard pile card display
│   │   ├── GameInfoPanel
│   │   │   ├── TurnIndicatorText - Current turn display
│   │   │   ├── DrawPileCountText - Draw pile count
│   │   │   ├── DiscardPileCountText - Discard pile count
│   │   │   └── GameMessageText - Deck event messages only
│   │   ├── ColorSelectionPanel - Color choice UI
│   │   │   ├── Btn_SelectRed
│   │   │   ├── Btn_SelectBlue
│   │   │   ├── Btn_SelectGreen
│   │   │   └── Btn_SelectYellow
│   │   ├── CurrentColorIndicator - Active color display
│   │   ├── Btn_Exit - Return to main menu
│   │   └── Btn_Pause - Future implementation
│   └── Screen_MultiPlayerGame
├── EventSystem
├── GameObject
├── MenuManager
├── BackgroundMusic
├── SFXController
├── CardDataTester
└── DeckManager
```


#### Scene Hierarchy notes:
- Screen_SinglePlayerGame - We have player1 and plyer1 instead of human and computer so we can assign the different roles as we please, in this case we want player1 to be a human and player2 to be a computer
    - Player1Panel
        - Player1HandPanel - for future card prefabs
        - Player1ActionPanel
            - Btn_Player1PlayCard - Player first clicks on one of the cards as to select it, and then, if card is valid to play, can click on this Play Card button
            - Btn_Player1DrawCard - Player can click on this Draw Card button if there are cards in the discard pile to be drawn
            - Btn_Player1EndTurn - Player can click on this End Card button on certain logic conditions, like for example if the played a card or drew a card, tho this may be a little more complicated
            - Player1HandSizePanel
                - Player1HandSizeText - A TextMeshProUGUI so we can easily see can Player1's Hand Size
    - Player2Panel
        - Player2HandPanel - for future card prefabs
        - Player2MessagePanel
            - Player2MessageText - In this case, Screen_SinglePlayerGame, Player2 is the computer, so we don't need buttons, instead we can have a TextMeshProUGUI, Player2's Message Texts so we can see what actions the computer is taking
            - Player2HandSizePanel 
                - Player2HandSizeText - A TextMeshProUGUI so we can easily see can Player2's Hand Size
    - GameBoardPanel - At this point, for this game at least, we don;t need any kind of timer for our player
        - DrawPilePanel - for future card prefabs, I think
        - DiscardPilePanel - for future card prefabs, I think
    - GameInfoPanel
        - TurnIndicatorText - A TextMeshProUGUI, so we know which player's turn it is
        - DrawPileCountText - A TextMeshProUGUI, so we how many cards we have in our Draw Pile
        - DiscardPileCountText - A TextMeshProUGUI, so we how many cards we have in our Discard Pile
        - GameMessageText - A TextMeshProUGUI, where we show Deck events only (loading, shuffling)
    - ColorSelectionPanel - initially inactive, needs to be active only during Changing Color state
        - Btn_SelectRed - When this is clicked on -> CurrentColorIndicator will turn to the same color as this button, AKA Red
        - Btn_SelectBlue - When this is clicked on -> CurrentColorIndicator will turn to the same color as this button, AKA Blue
        - Btn_SelectGreen - When this is clicked on -> CurrentColorIndicator will turn to the same color as this button, AKA Green
        - Btn_SelectYellow - When this is clicked on -> CurrentColorIndicator will turn to the same color as this button, AKA Yellow
    - CurrentColorIndicator - An Image indicating the Current Color
    - Btn_Exit - This button already has its implemented logic in MenuNavigation script
    - Btn_Pause - We should not implement this button's logic in the scripts yet, this can wait untill future milestones

---

## Phase 1: Foundation Setup ✅ COMPLETE

### Milestone 1: Menu System ✅ COMPLETE
**Status**: All scenes and navigation working

### Milestone 2: UI Framework Creation ✅ COMPLETE  
**Status**: Full UI hierarchy established, all panels created

---

## Phase 2: Core Card System

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

---

### Milestone 5: Turn Management System ✅ COMPLETE
**Objective**: Implement turn-based mechanics with basic computer AI

**Architecture Completed**:
- ✅ **Multi-Enum Game State Architecture**:
  - `GameStateManager`: Manages TurnState, InteractionState, GameStatus, active color, rules
  - `TurnManager`: Handles turn switching, timing, player transitions
  - `BasicComputerAI`: Simple AI with strategic card selection
  - `GameplayUIManager`: Turn-related UI updates, player actions, color selection
  - `GameManager`: Main coordinator for all gameplay systems

**Integration Completed**:
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

## Phase 3: Basic Gameplay Loop

## 🎯 **Current Milestone 6 Objectives**

### **Primary Goal**: Create Visual Card System
Transform the working data-driven card system into interactive visual cards.

### **Key Requirements**:
1. **Card Prefab Creation**: Visual representation of CardData
2. **Hand Management**: Dynamic card display in Player1HandPanel/Player2HandPanel
3. **Card Selection**: Interactive card choosing system
4. **Integration**: Connect visual cards to existing GameManager methods
5. **Testing**: Comprehensive testing of visual gameplay

---

## 🔧 **Milestone 6 Implementation Plan**

### **Substep 6.1: Card Prefab Creation**

**Create CardPrefab with simplified structure**:
```
CardPrefab
├── CardBack (Image) - Scanned back image for face-down display
├── CardFront (Image) - Scanned front image for face-up display  
├── CardButton (Button) - Click interaction only
└── CardController (Script) - Interaction logic
```

**CardController Script Responsibilities**:
- Load correct scanned image based on CardData
- Handle face-up/face-down state with instant image swapping
- Click selection: Move card up by 10px (configurable constant, no magic numbers)
- Visual feedback: Gold shade for valid cards, red shade for invalid cards (selection only)
- Integration with GameManager for card actions
- **NO animations, NO hover effects, NO text overlays**

**Visual Specifications**:
- Card dimensions: Height = 100px, Width = (2/3) * Height (calculated)
- Selection feedback: Y-position +10px offset when selected
- Color feedback: Gold tint for playable, red tint for unplayable (selection only)

### **Substep 6.2: Hand Display System**

**Create HandManager Script Features**:
- Convert `List<CardData> playerHand` to visual card prefabs with scanned images
- Manual positioning system (NO Layout Group components)
- Adaptive spacing: Minimal to maximal based on hand size
- Cards always added to end of hand (no filtering or sorting)
- Position recalculation after every add/remove operation

**Layout Requirements**:
- Cards maintain consistent 100px height, calculated width  
- Spacing algorithm: More cards = tighter spacing, fewer cards = more spacing
- Manual position calculation for precise control
- **NO animations** for card movement or transitions

**Integration Points**:
- GameManager.playerHand → Visual card prefabs in Player1HandPanel
- Selected card → GameManager.PlayCard(selectedCard)
- Hand updates → Instant prefab add/remove with position recalculation

### **Substep 6.3: Card Interaction Integration**

**Player Interaction Flow**:
1. Player clicks card prefab → CardController moves card up 10px and applies color shade
2. Player clicks "Play Card" button → GameManager validates and plays selected card
3. Visual hand updates → Remove played card prefab instantly
4. **Debug logs only** for feedback (no player message UI element)

**Computer Hand Display**:
- Face-down card prefabs in Player2HandPanel (CardBack image)
- Card facing controlled by CardController enum/boolean
- Flip function for instant CardBack ↔ CardFront image swap (**no animations**)
- Debug logging to show computer's actual CardData for development

**Face-Up/Face-Down System**:
- CardController manages facing state
- Instant image swap between CardBack and CardFront
- Computer cards remain face-down during gameplay
- Debug logs reveal computer cards for testing

### **Substep 6.4: Comprehensive Testing Phase**

**Heavy Testing Areas** (postponed from earlier milestones):
- Visual accuracy of card information
- Card selection responsiveness
- Hand layout and organization
- Performance with 8+ cards
- Integration with turn system
- Computer hand visual updates
- Color selection integration
- All existing functionality preservation

---

## Phase 4: Core Rules Implementation

### Milestone 7: Basic Card Rules System
**Objective**: Implement complete card rule validation and enhanced gameplay

#### Substep 7.1: Enhanced Card Validation
- Visual highlighting of playable cards
- Rule explanation tooltips
- Invalid move prevention at UI level

#### Substep 7.2: Game Rules Engine Enhancement
- Complete rule implementation for all card types
- Turn ending logic refinement
- Win condition detection improvements

#### Substep 7.3: UI Feedback System
- Enhanced visual feedback for game actions
- Rule violation explanations
- Game state communication improvements

### Milestone 8: Color Selection System Enhancement
**Objective**: Polish ChangeColor card functionality with visual improvements

#### Substep 8.1: Enhanced Color Selection UI
- Visual integration with card prefabs
- Improved color selection feedback
- ColorSelectionPanel polish

#### Substep 8.2: Color System Integration
- Visual color indicators on cards
- Color matching visual feedback
- Enhanced active color display

---

## Phase 5: Special Cards Implementation

### Milestone 9: Action Cards - Basic Set
**Objective**: Implement Plus, Stop, PlusTwo cards

#### Substep 9.1: Plus Card
- Force player to play second card
- Handle inability to play second card
- UI feedback for plus card state

#### Substep 9.2: Stop Card
- Skip next player's turn
- Update turn indicator
- Handle consecutive stop cards

#### Substep 9.3: PlusTwo Card
- Force next player to draw 2 cards
- Implement stacking mechanism
- Handle PlusTwo chains
- Special game state for chains

#### Substep 9.4: Integration Testing
- Test all action cards together
- Computer AI updates for action cards
- Rule interaction validation
- Bug fixes and polish

### Milestone 10: Advanced Cards - TAKI System
**Objective**: Implement TAKI and SuperTaki functionality

#### Substep 10.1: TAKI Card Logic
- Allow multiple card plays of same color
- "Closed TAKI" announcement system
- Open TAKI continuation by next player
- TAKI sequence UI state

#### Substep 10.2: SuperTaki Implementation
- Wild TAKI functionality
- Automatic color adoption
- Integration with TAKI system

#### Substep 10.3: TAKI UI System
- Special UI state during TAKI sequence
- Card selection for TAKI continuation  
- Visual feedback for TAKI rules
- Computer AI TAKI strategy

---

## Phase 6: Game Polish & Features

### Milestone 11: Win/Lose System
- Detect game end conditions
- Winner announcement
- Score calculation (if desired)
- Play again functionality

### Milestone 12: Menu Integration  
- Pause menu during gameplay
- Return to main menu
- Restart game functionality  
- Settings integration

### Milestone 13: Final Polish
- Bug fixes and optimization
- UI polish and consistency  
- Audio integration (if time permits)
- Build preparation and testing

---

## Current Architecture Highlights

### **Multi-Enum State Management**:
```csharp
// Clean separation of state concerns
public enum TurnState { PlayerTurn, ComputerTurn, Neutral }
public enum InteractionState { Normal, ColorSelection, TakiSequence, PlusTwoChain }
public enum GameStatus { Active, Paused, GameOver }
```

### **UI Ownership Architecture** (Established in Milestone 5):
```csharp
// Clear separation of UI responsibilities
GameplayUIManager: Turn display, player actions, computer feedback, color selection
DeckUIManager: Draw/discard counts, deck event messages only
```

### **Component Integration Success**:
- **GameManager**: 5 components working together seamlessly
- **DeckManager**: 4 components coordinating deck operations
- **Event-Driven**: All systems communicate via events
- **Coordinator Pattern**: Clean delegation to specialized components

---

## Development Guidelines

### Architecture Principles
- **Separation of Concerns**: Each component has single responsibility
- **Event-Driven Communication**: Components communicate via events
- **Coordinator Pattern**: Managers delegate to specialized components  
- **Multi-Enum State**: Separate enums for different state aspects
- **Visual-Data Separation**: CardData separate from visual representation

### Testing Strategy - Updated
1. **Component Testing**: Each component tested independently ✅ COMPLETE
2. **Integration Testing**: Test component interactions ✅ COMPLETE
3. **Visual Gameplay Testing**: Full testing with card prefabs 🎯 MILESTONE 6
4. **Rule Validation Testing**: All card rules working (Milestone 7+)

---

## Success Metrics

### Milestone 6 Success Criteria 🎯 CURRENT TARGET
- ✅ Card prefabs display CardData information correctly
- ✅ Hand management system functional for both players
- ✅ Card selection and highlighting working
- ✅ Integration with existing GameManager methods
- ✅ Visual feedback for valid/invalid moves
- ✅ Computer hand display appropriate (face-down cards)
- ✅ Performance optimized for full card hands
- ✅ All existing Milestone 5 functionality preserved
- ✅ Layout and spacing professional appearance
- ✅ Smooth card interactions and visual feedback

### Overall Project Success  
- Complete playable TAKI game (Human vs Computer)  
- All major card types implemented correctly  
- Intuitive UI with clear visual feedback  
- Stable gameplay without crashes  
- Clean, maintainable code architecture  
- Code ready for multiplayer extension  
- Professional visual presentation

---

## Current Status Summary

**✅ COMPLETED**:
- Phase 1: Complete foundation (Menu + UI Framework)
- Phase 2: Complete card system (Data + Deck + Turn Management)
- All 110 cards loading and functioning
- Multi-enum state management working
- Turn-based gameplay functional
- Computer AI making strategic decisions
- Clean UI ownership architecture established

**🎯 CURRENT FOCUS - Milestone 6**:
- Create visual card prefab system
- Implement hand management and display
- Add card selection and interaction
- Comprehensive testing of visual gameplay

**🚀 NEXT PHASES**:
- Enhanced rule validation with visual feedback
- Special card implementations
- Game polish and feature completion

---

# My Notes:

Scripts/
├── Controllers/
│   └── CardDataTester.cs
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
│   ├── GameSetupManager.cs
│   ├── GameStateManager.cs
│   └── TurnManager.cs
├── UI/
│   ├── DifficultySlider.cs
│   ├── GameplayUIManager.cs
│   └── MenuNavigation.cs
├── ButtonSFX.cs
├── MusicSlider.cs
└── SfxSlider.cs

I'm taking the following notes/making the code changes right after "Step 1: Create All Script Files" and before "Step 2: Set Up GameManager GameObject".

- In `GameStateManager.cs`:

    - I changed `public CardColor activeColor = CardColor.Red;` to:
        ```
    	[Tooltip ("Active color that must be matched (White = no color set yet)")]
		public CardColor activeColor = CardColor.Wild; // Wild = neutral/white initially
        ```
        Reasoning: Wild represents "no specific color" which is perfect for initial state.


    - I removed `public bool strictRules = true;` - This is redundant.

- In `BasicComputerAI`:

    - Looking into `SelectBestCard`, "higher numbers" doesn't make sense. For TAKI, number value doesn't matter for strategy, so I fixed `SelectFromNumberCards`:
        ```
    	/// <summary>
		/// Select random number card
		/// </summary>
		/// <param name="numberCards">Available number cards</param>
		/// <returns>Selected number card</returns>
		CardData SelectFromNumberCards (List<CardData> numberCards) {
			return numberCards [Random.Range (0, numberCards.Count)];
		}
        ```

    - `AddCardsToHand` should just call `AddCardToHand` in a loop, I fixed the code:
        ```
    	public void AddCardsToHand (List<CardData> cards) {
			foreach (CardData card in cards) {
				AddCardToHand (card);
			}
			Debug.Log ($"AI received {cards.Count} cards. Hand size: {computerHand.Count}");
		}
        ```

- In `GameplayUIManager.cs`:

    - I can see that `public Color wildColor = Color.white;`, and I am debating myself if this direction is good - I think it is, but we need to tread carefully, as I metioned in my note on `GameStateManager`

    - `GetColorForCardColor`, Wild cards shouldn't change color indicator, wild cards should maintain previous color, I fixed the code:
        ```
		/// <summary>
		/// Convert CardColor to Unity Color
		/// </summary>
		/// <param name="cardColor">Card color to convert</param>
		/// <returns>Unity color</returns>
		Color GetColorForCardColor (CardColor cardColor) {
			switch (cardColor) {
				case CardColor.Red:
					return redColor;
				case CardColor.Blue:
					return blueColor;
				case CardColor.Green:
					return greenColor;
				case CardColor.Yellow:
					return yellowColor;
				default:
					return wildColor;
			}
		}
        ```

    - I removed `PulseElement` and `HighlightActiveColor` methods entirely. No animations = faster development, fewer bugs.

---

- I've gotten to the conclusion that we have a in regards to the GameState enum: I think we should either seperate it into different enum groups like: 
        - PlayerTurn, ComputerTurn, + another third neutral option
        - ColorSelection, TakiSequence, PlusTwoChain,, + another neutral option
        - GameOver, Paused, + another third neutral option
    - Or do you maybe have another better idea?
    - Anyhow, this will require changing many many different files, so we need to go and look over EACH ONE of our files and check.
New Multi-Enum Architecture Proposal:
```
// Who's turn is it?
public enum TurnState {
    PlayerTurn,
    ComputerTurn,
    Neutral    // During transitions, game over, etc.
}

// What special interaction is happening?  
public enum InteractionState {
    Normal,           // Regular gameplay
    ColorSelection,   // ChangeColor card played
    TakiSequence,     // TAKI multi-play active
    PlusTwoChain      // +2 cards being chained
}

// What's the overall game status?
public enum GameStatus {
    Active,    // Game is running normally
    Paused,    // Game paused
    GameOver   // Game ended
}
```
Now we need to update all relevant files accordingly


---

I have updated all the scripts, I haven't yet gotten to Step 3: Configure GameManager GameObject. Before that I want to start a ne prompt since this one is getting long and heavy - in order to do that I need to update my project's "Set project instructions" and attached "Files".

1.I need your suggestion on what files I should attach and which are not necessary, I know that it's best to give claude the needed and relevant information, so aside from the "Set project instructions", which files should be attached?

2.I want you to give me back in canvas "Set project instructions", but with the following changes:

A. Make sure all our architectural changes, and any important changes are updated in the doucument.

B. We completed "Milestone 4" and are currently in the middle of "Milestone 5" - Update what's completed and what's left to do. Remember that the heavier testing is planned for after creating the card prefabs (in "Milestone 6" if I remember correctly).

C. Add this:
Project:
```
Scripts/
├── Controllers/
│   └── CardDataTester.cs
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
│   ├── GameSetupManager.cs
│   ├── GameStateManager.cs
│   └── TurnManager.cs
├── UI/
│   ├── DifficultySlider.cs
│   ├── GameplayUIManager.cs
│   └── MenuNavigation.cs
├── ButtonSFX.cs
├── MusicSlider.cs
└── SfxSlider.cs
```

D. Add this:
Hierarchy:
```
Scene_Menu
├── Main Camera
├── Canvas
│   ├── Img_Background
│   ├── Screen_MainMenu
│   ├── Screen_StudentInfo
│   ├── Screen_Settings
│   ├── Screen_SinglePlayer
│   ├── Screen_MultiPlayer
│   ├── Screen_ExitValidator
│   ├── Screen_Loading
│   ├── Screen_Exiting
│   ├── Screen_SinglePlayerGame
│   │   ├── Player1Panel
│   │   │   ├── Player1HandPanel
│   │   │   └── Player1ActionPanel
│   │   │       ├── Btn_Player1PlayCard
│   │   │       ├── Btn_Player1DrawCard
│   │   │       └── Btn_Player1EndTurn
│   │   ├── Player2Panel
│   │   │   ├── Player2HandPanel
│   │   │   └── Player2ActionPanel
│   │   │       ├── Btn_Player2PlayCard
│   │   │       ├── Btn_Player2DrawCard
│   │   │       └── Btn_Player2EndTurn
│   │   ├── GameBoardPanel
│   │   │   ├── DrawPilePanel
│   │   │   └── DiscardPilePanel
│   │   ├── GameInfoPanel
│   │   │   ├── TurnIndicatorText
│   │   │   ├── DrawPileCountText
│   │   │   ├── DiscardPileCountText
│   │   │   └── GameMessageText
│   │   ├── ColorSelectionPanel
│   │   │   ├── Btn_SelectRed
│   │   │   ├── Btn_SelectBlue
│   │   │   ├── Btn_SelectGreen
│   │   │   ├── Btn_SelectYellow
│   │   │   └── CurrentColorIndicator
│   │   └── Btn_Exit
│   └── Screen_MultiPlayerGame
├── EventSystem
├── GameObject
├── MenuManager
├── BackgroundMusic
├── SFXController
├── CardDataTester
└── DeckManager
```










# TAKI Game Development Plan - Unity Engine
## Comprehensive Implementation Guide

### ⚠️ CRITICAL NOTES
- **AVOID UNICODE**: No special characters in code, file names, text displays, or comments
- **Current Status**: Phase 1 Complete ✅, Phase 2 Complete ✅, **Currently at Phase 3 - Milestone 6** 🎯
- **Target Platform**: PC/Desktop Unity Build
- **Scope**: Singleplayer (Human vs Computer) with multiplayer-ready architecture

---

## Project Structure

### Scripts Organization:
```
Scripts/
├── Controllers/
│   └── CardDataTester.cs
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
│   ├── GameSetupManager.cs
│   ├── GameStateManager.cs
│   └── TurnManager.cs
├── UI/
│   ├── DifficultySlider.cs
│   ├── GameplayUIManager.cs
│   └── MenuNavigation.cs
├── ButtonSFX.cs
├── MusicSlider.cs
└── SfxSlider.cs
```

### Scene Hierarchy:
```
Scene_Menu
├── Main Camera
├── Canvas
│   ├── Img_Background
│   ├── Screen_MainMenu
│   ├── Screen_StudentInfo
│   ├── Screen_Settings
│   ├── Screen_SinglePlayer
│   ├── Screen_MultiPlayer
│   ├── Screen_ExitValidator
│   ├── Screen_Loading
│   ├── Screen_Exiting
│   ├── Screen_SinglePlayerGame
│   │   ├── Player1Panel (Human Player)
│   │   │   ├── Player1HandPanel - for card prefab instantiation
│   │   │   └── Player1ActionPanel
│   │   │       ├── Btn_Player1PlayCard - Play selected card
│   │   │       ├── Btn_Player1DrawCard - Draw from deck
│   │   │       ├── Btn_Player1EndTurn - End current turn
│   │   │       └── Player1HandSizePanel
│   │   │           └── Player1HandSizeText - Hand size display
│   │   ├── Player2Panel (Computer Player)
│   │   │   ├── Player2HandPanel - for computer card prefabs
│   │   │   └── Player2MessagePanel
│   │   │       ├── Player2MessageText - AI action feedback
│   │   │       └── Player2HandSizePanel 
│   │   │           └── Player2HandSizeText - Computer hand size
│   │   ├── GameBoardPanel
│   │   │   ├── DrawPilePanel - draw pile card display
│   │   │   └── DiscardPilePanel - discard pile card display
│   │   ├── GameInfoPanel
│   │   │   ├── TurnIndicatorText - Current turn display
│   │   │   ├── DrawPileCountText - Draw pile count
│   │   │   ├── DiscardPileCountText - Discard pile count
│   │   │   └── GameMessageText - Deck event messages only
│   │   ├── ColorSelectionPanel - Color choice UI
│   │   │   ├── Btn_SelectRed
│   │   │   ├── Btn_SelectBlue
│   │   │   ├── Btn_SelectGreen
│   │   │   └── Btn_SelectYellow
│   │   ├── CurrentColorIndicator - Active color display
│   │   ├── Btn_Exit - Return to main menu
│   │   └── Btn_Pause - Future implementation
│   └── Screen_MultiPlayerGame
├── EventSystem
├── GameObject
├── MenuManager
├── BackgroundMusic
├── SFXController
├── CardDataTester
└── DeckManager
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
  - `DeckUIManager`: UI updates for deck events only
  - `GameSetupManager`: Game initialization logic (deal hands, place starting card)
  - `DeckManager`: Coordinator pattern (delegates to specialized components)
- ✅ All 110 cards load and distribute correctly (8+8+1 setup working)
- ✅ Automatic deck initialization and UI updates
- ✅ **Wild as initial color** (represents "no color set yet")
- ✅ Event-driven architecture connecting all components
- ✅ Clean separation of concerns for future multiplayer readiness

### Milestone 5: Turn Management System ✅ COMPLETE
**Objective**: Implement turn-based mechanics with basic computer AI

**Architecture Completed**:
- ✅ **Multi-Enum Game State Architecture**:
  - `GameStateManager`: Manages TurnState, InteractionState, GameStatus, active color, rules
  - `TurnManager`: Handles turn switching, timing, player transitions
  - `BasicComputerAI`: Simple AI with strategic card selection
  - `GameplayUIManager`: Turn-related UI updates, player actions, color selection
  - `GameManager`: Main coordinator for all gameplay systems

**Integration Completed**:
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

## Phase 3: Basic Gameplay Loop 🎯 CURRENT PHASE



I have feedback on Milestone 6:

### Milestone 6: Hand Management System 🎯 **CURRENTLY IN PROGRESS**
**Objective**: Create visual card representation and interactive hand management

#### Substep 6.1: Card Prefab Creation
**Card Prefab Structure**:
```
CardPrefab
├── CardBackground (Image) - Background with card frame
├── CardButton (Button) - Click interaction component
├── CardNumberText (TextMeshProUGUI) - Display card number
├── CardTypeText (TextMeshProUGUI) - Display card type/name
├── CardColorIndicator (Image) - Color visual indicator
└── CardController (Script) - Card interaction logic
```
I was thinking of something a little different - I have already scanned all the cards, so I think we only need:
* CardBack (Image) - Back of all cards scanned image
* CardFront (Image) - Front of this card scanned image
Some of the other things you described sound like they might be good for logging, but aren't actually really necessary for the display.

**CardController Features**:
- Display CardData information visually
- Handle card selection/deselection
- Visual feedback for valid/invalid plays
- Integration with GameManager for card actions
- Hover effects and animations

My notes:
- Only the card front and back visuals should be displayed.
- When player clicks/selects (NOT HOVER) a card, the card's Y location increaces by 10px (no magic numbers) 
    - If card is valid to play at this moment - the card be be given a slight shade of gold (only when clicked/selected)
    - If card is invalid to play at this moment - the card be be given a slight shade of red (only when clicked/selected)
- NO ANIMATIONS AT ALL!!!

#### Substep 6.2: Hand Display System
**HandManager Script Features**:
- Dynamic card prefab instantiation in Player1HandPanel
- Proper card spacing and layout (Horizontal Layout Group)
- Hand size management and visual organization
- Card selection highlighting system
- Integration with GameplayUIManager

My notes:
- All cards should be height 100px and the width is 2/3 of the height. No magic numbers. These are actually the exact sizes of the pile panels.
- We need a minimal spacing and a maximal spacing, depends on how many cards are in the hand
    - the cards should not be filtered, always add to the end of the list
    - NO ANIMATIONS!
- After every card change we must make sure to update/readjust the cards positioning
- golden and red highlighting 
- IGNORE HOVERING!!!

**Key Responsibilities**:
- Convert List<CardData> to visual card prefabs
- Manage card positions and layout
- Handle card selection state
- Remove played cards from display
- Add drawn cards to display

#### Substep 6.3: Card Interaction Integration
**Player Interaction Flow**:
1. **Card Selection**: Click card prefab → highlight selection
2. **Play Validation**: Check if selected card is playable
3. **Play Card**: Use existing GameManager.PlayCard() method
4. **Visual Feedback**: Update hand display and game state

My notes:
- IMPORTANT: We currently don't have any playerMessage element, and I don't want one (not at this pint at least) - so any text feedback can just be logs.

**Computer Hand Display**:
- Show computer cards as face-down prefabs
- Display count but not card details
- Update when computer plays/draws cards

My notes:
- We should have a kind facingUp or facingDown (maybe an enum, or a variable or something)
    - when we want to switch the card's facing direction we can use a flip kind of function (but no animations!!!)
- We want to know the computer's cards in the logs

#### Substep 6.4: Comprehensive Testing
**Testing Areas** (Heavy testing phase):
- Visual card display accuracy
- Card selection and highlighting
- Play card button integration
- Hand layout and organization
- Card removal/addition animations
- Computer hand visual updates
- Integration with existing turn system
- Performance with full card hands

My notes:
- NO ANIMATIONS!

**Success Criteria**:
- ✅ Cards display correct information from CardData
- ✅ Card selection system working smoothly
- ✅ Integration with existing GameManager methods
- ✅ Proper hand layout and visual organization
- ✅ Computer hand display functional
- ✅ No performance issues with full hands
- ✅ All existing functionality preserved

---

## Phase 4: Core Rules Implementation

### Milestone 7: Basic Card Rules System
**Objective**: Implement complete card rule validation and enhanced gameplay

#### Substep 7.1: Enhanced Card Validation
- Visual highlighting of playable cards
- Rule explanation tooltips
- Invalid move prevention at UI level

#### Substep 7.2: Game Rules Engine Enhancement
- Complete rule implementation for all card types
- Turn ending logic refinement
- Win condition detection improvements

#### Substep 7.3: UI Feedback System
- Enhanced visual feedback for game actions
- Rule violation explanations
- Game state communication improvements

### Milestone 8: Color Selection System Enhancement
**Objective**: Polish ChangeColor card functionality with visual improvements

#### Substep 8.1: Enhanced Color Selection UI
- Visual integration with card prefabs
- Improved color selection feedback
- ColorSelectionPanel polish

#### Substep 8.2: Color System Integration
- Visual color indicators on cards
- Color matching visual feedback
- Enhanced active color display

---

## Phase 5: Special Cards Implementation

### Milestone 9: Action Cards - Basic Set
**Objective**: Implement Plus, Stop, PlusTwo cards with full visual feedback

### Milestone 10: Advanced Cards - TAKI System
**Objective**: Implement TAKI and SuperTaki functionality with special UI states

---

## Phase 6: Game Polish & Features

### Milestone 11: Win/Lose System
### Milestone 12: Menu Integration  
### Milestone 13: Final Polish

---

## Current Architecture Highlights

### **Multi-Enum State Management**:
```csharp
// Clean separation of state concerns
public enum TurnState { PlayerTurn, ComputerTurn, Neutral }
public enum InteractionState { Normal, ColorSelection, TakiSequence, PlusTwoChain }
public enum GameStatus { Active, Paused, GameOver }
```

### **UI Ownership Architecture** (Established in Milestone 5):
```csharp
// Clear separation of UI responsibilities
GameplayUIManager: Turn display, player actions, computer feedback, color selection
DeckUIManager: Draw/discard counts, deck event messages only
```

### **Component Integration Success**:
- **GameManager**: 5 components working together seamlessly
- **DeckManager**: 4 components coordinating deck operations
- **Event-Driven**: All systems communicate via events
- **Coordinator Pattern**: Clean delegation to specialized components

---

## Development Guidelines

### Architecture Principles
- **Separation of Concerns**: Each component has single responsibility
- **Event-Driven Communication**: Components communicate via events
- **Coordinator Pattern**: Managers delegate to specialized components  
- **Multi-Enum State**: Separate enums for different state aspects
- **Visual-Data Separation**: CardData separate from visual representation

### Testing Strategy - Updated
1. **Component Testing**: Each component tested independently ✅ COMPLETE
2. **Integration Testing**: Test component interactions ✅ COMPLETE
3. **Visual Gameplay Testing**: Full testing with card prefabs 🎯 MILESTONE 6
4. **Rule Validation Testing**: All card rules working (Milestone 7+)

---

## Success Metrics

### Milestone 6 Success Criteria 🎯 CURRENT TARGET
- ✅ Card prefabs display CardData information correctly
- ✅ Hand management system functional for both players
- ✅ Card selection and highlighting working
- ✅ Integration with existing GameManager methods
- ✅ Visual feedback for valid/invalid moves
- ✅ Computer hand display appropriate (face-down cards)
- ✅ Performance optimized for full card hands
- ✅ All existing Milestone 5 functionality preserved
- ✅ Layout and spacing professional appearance
- ✅ Smooth card interactions and visual feedback

### Overall Project Success  
- Complete playable TAKI game (Human vs Computer)  
- All major card types implemented correctly  
- Intuitive UI with clear visual feedback  
- Stable gameplay without crashes  
- Clean, maintainable code architecture  
- Code ready for multiplayer extension  
- Professional visual presentation

---

## Current Status Summary

**✅ COMPLETED**:
- Phase 1: Complete foundation (Menu + UI Framework)
- Phase 2: Complete card system (Data + Deck + Turn Management)
- All 110 cards loading and functioning
- Multi-enum state management working
- Turn-based gameplay functional
- Computer AI making strategic decisions
- Clean UI ownership architecture established

**🎯 CURRENT FOCUS - Milestone 6**:
- Create visual card prefab system
- Implement hand management and display
- Add card selection and interaction
- Comprehensive testing of visual gameplay

**🚀 NEXT PHASES**:
- Enhanced rule validation with visual feedback
- Special card implementations
- Game polish and feature completion




# TAKI Game Development - Set Project Instructions

## 📋 **Current Project Status**
- **Phase 1**: Foundation Setup ✅ COMPLETE
- **Phase 2**: Core Card System ✅ COMPLETE  
- **Current Focus**: **Phase 3 - Milestone 6: Hand Management System** 🎯
- **Architecture**: Multi-enum state management with clean UI ownership
- **Platform**: Unity PC/Desktop build
- **Scope**: Singleplayer Human vs Computer

---

## 🏗️ **Completed Architecture (Milestone 5)**

### **Multi-Enum State Management**
```csharp
namespace TakiGame {
    public enum TurnState { PlayerTurn, ComputerTurn, Neutral }
    public enum InteractionState { Normal, ColorSelection, TakiSequence, PlusTwoChain }
    public enum GameStatus { Active, Paused, GameOver }
}
```

### **Component Integration Success**
**GameManager GameObject Components**:
- GameManager (coordinator)
- GameStateManager (multi-enum state)
- TurnManager (turn switching)
- BasicComputerAI (strategic decisions)
- GameplayUIManager (player UI)

**DeckManager GameObject Components**:
- DeckManager (coordinator)
- Deck (card operations)
- CardDataLoader (110 cards from Resources)
- DeckUIManager (deck event UI)
- GameSetupManager (game initialization)

### **UI Ownership Architecture**
**Clear separation established**:
- **GameplayUIManager**: Turn display, player actions, computer feedback, color selection
- **DeckUIManager**: Deck counts and deck event messages ONLY

### **Event-Driven Communication**
All systems communicate via events, no direct references between unrelated components.

---

## 🎯 **Current Milestone 6 Objectives**

### **Primary Goal**: Create Visual Card System
Transform the working data-driven card system into interactive visual cards.

### **Key Requirements**:
1. **Card Prefab Creation**: Visual representation of CardData
2. **Hand Management**: Dynamic card display in Player1HandPanel/Player2HandPanel
3. **Card Selection**: Interactive card choosing system
4. **Integration**: Connect visual cards to existing GameManager methods
5. **Testing**: Comprehensive testing of visual gameplay

---

## 🔧 **Milestone 6 Implementation Plan**

### **Substep 6.1: Card Prefab Creation**

**Create CardPrefab with simplified structure**:
```
CardPrefab
├── CardBack (Image) - Scanned back image for face-down display
├── CardFront (Image) - Scanned front image for face-up display  
├── CardButton (Button) - Click interaction only
└── CardController (Script) - Interaction logic
```

**CardController Script Responsibilities**:
- Load correct scanned image based on CardData
- Handle face-up/face-down state with instant image swapping
- Click selection: Move card up by 10px (configurable constant, no magic numbers)
- Visual feedback: Gold shade for valid cards, red shade for invalid cards (selection only)
- Integration with GameManager for card actions
- **NO animations, NO hover effects, NO text overlays**

**Visual Specifications**:
- Card dimensions: Height = 100px, Width = (2/3) * Height (calculated)
- Selection feedback: Y-position +10px offset when selected
- Color feedback: Gold tint for playable, red tint for unplayable (selection only)

### **Substep 6.2: Hand Display System**

**Create HandManager Script Features**:
- Convert `List<CardData> playerHand` to visual card prefabs with scanned images
- Manual positioning system (NO Layout Group components)
- Adaptive spacing: Minimal to maximal based on hand size
- Cards always added to end of hand (no filtering or sorting)
- Position recalculation after every add/remove operation

**Layout Requirements**:
- Cards maintain consistent 100px height, calculated width  
- Spacing algorithm: More cards = tighter spacing, fewer cards = more spacing
- Manual position calculation for precise control
- **NO animations** for card movement or transitions

**Integration Points**:
- GameManager.playerHand → Visual card prefabs in Player1HandPanel
- Selected card → GameManager.PlayCard(selectedCard)
- Hand updates → Instant prefab add/remove with position recalculation

### **Substep 6.3: Card Interaction Integration**

**Player Interaction Flow**:
1. Player clicks card prefab → CardController moves card up 10px and applies color shade
2. Player clicks "Play Card" button → GameManager validates and plays selected card
3. Visual hand updates → Remove played card prefab instantly
4. **Debug logs only** for feedback (no player message UI element)

**Computer Hand Display**:
- Face-down card prefabs in Player2HandPanel (CardBack image)
- Card facing controlled by CardController enum/boolean
- Flip function for instant CardBack ↔ CardFront image swap (**no animations**)
- Debug logging to show computer's actual CardData for development

**Face-Up/Face-Down System**:
- CardController manages facing state
- Instant image swap between CardBack and CardFront
- Computer cards remain face-down during gameplay
- Debug logs reveal computer cards for testing

### **Substep 6.4: Comprehensive Testing Phase**

**Heavy Testing Areas** (postponed from earlier milestones):
- Visual accuracy of card information
- Card selection responsiveness
- Hand layout and organization
- Performance with 8+ cards
- Integration with turn system
- Computer hand visual updates
- Color selection integration
- All existing functionality preservation

---

## 🔄 **Integration with Existing Systems**

### **GameManager Integration**
**Current methods to connect with**:
- `GameManager.PlayCard(CardData card)` - Already working
- `GameManager.DrawCard()` - Already working
- `GameManager.playerHand` - List to convert to visuals
- Event system - Already functional for UI updates

### **GameplayUIManager Integration**
**Existing button events to maintain**:
- OnPlayCardClicked - Connect to selected card system
- OnDrawCardClicked - Already working
- OnColorSelected - Already working
- Button state management - Already working

### **No Changes Needed**:
- DeckManager system (working perfectly)
- Turn management system (working perfectly)
- Computer AI system (working perfectly)
- State management (working perfectly)

---

## 📝 **Code Quality Standards**

### **Naming Conventions**
```csharp
namespace TakiGame {
    public class CardController : MonoBehaviour {
        [Header("Card Display")]
        public Image cardBackground;
        public Button cardButton;
        public TextMeshProUGUI cardNumberText;
        
        private CardData cardData;
        private bool isSelected = false;
    }
}
```

### **Architecture Principles**
- **Single Responsibility**: Each script has one clear purpose
- **Event-Driven**: Use events for component communication
- **No Unicode**: ASCII characters only in all code/text
- **Visual-Data Separation**: CardData remains separate from visual prefab
- **Existing System Preservation**: Don't break Milestone 5 functionality

---

## 🚫 **Critical Constraints**

### **Do Not Modify**:
- GameManager core methods (working correctly)
- DeckManager system (stable and functional)
- Multi-enum state system (working perfectly)
- UI ownership architecture (cleanly separated)
- Turn management flow (tested and stable)

### **Unicode Restrictions**:
- No special characters in any new scripts
- ASCII-only text in UI displays
- Simple characters in all variable names and comments

### **Performance Requirements**:
- Handle 8+ card hands smoothly
- Efficient prefab instantiation/destruction
- No memory leaks from card prefabs
- Responsive card selection (<100ms feedback)

---

## 🎯 **Success Criteria for Milestone 6**

### **Visual System**:
- ✅ Card prefabs accurately display CardData information
- ✅ Professional visual appearance with proper layout
- ✅ Responsive card selection with clear feedback
- ✅ Smooth hand organization and spacing

### **Interaction System**:
- ✅ Card selection system functional
- ✅ Integration with existing Play Card button
- ✅ Visual feedback for valid/invalid moves
- ✅ Computer hand display appropriate

### **Integration**:
- ✅ No breaking changes to existing systems
- ✅ Performance optimized for full card hands
- ✅ Event system integration working
- ✅ All Milestone 5 functionality preserved

### **Testing Completion**:
- ✅ All card interactions tested thoroughly
- ✅ Layout system working across different hand sizes
- ✅ Visual feedback system polished
- ✅ Ready for enhanced rule validation (Milestone 7)

---

## 📁 **File Structure for Milestone 6**

### **New Files to Create**:
```
Scripts/
├── UI/
│   ├── CardController.cs - Individual card prefab logic
│   └── HandManager.cs - Hand display management
└── Prefabs/
    └── CardPrefab.prefab - Visual card prefab
```

### **Files to Modify**:
- Potentially minor GameManager.cs integration points
- Potentially minor GameplayUIManager.cs integration points

### **Files to Keep Unchanged**:
- All DeckManager components
- All state management components
- All turn management components
- All Computer AI components

---

## 🔄 **Development Workflow**

### **Step 1**: Create CardPrefab
- Design visual layout in Unity
- Add all required UI components
- Create CardController script

### **Step 2**: Create HandManager
- Script to manage card prefab instantiation
- Integration with Player1HandPanel/Player2HandPanel
- Handle card selection state

### **Step 3**: Integration Testing
- Connect visual cards to existing GameManager methods
- Test card selection → play card flow
- Verify no existing functionality breaks

### **Step 4**: Polish and Testing
- Visual feedback improvements
- Performance optimization
- Comprehensive functionality testing

---

## 🎮 **Scene Integration Points**

### **UI Elements to Connect**:
- **Player1HandPanel**: Container for player card prefabs
- **Player2HandPanel**: Container for computer card prefabs  
- **Btn_Player1PlayCard**: Connect to selected card system
- **ColorSelectionPanel**: Already working, maintain integration

### **Existing Systems to Preserve**:
- Turn indicator updates (working)
- Hand size displays (working)
- Computer message system (working)
- Deck count displays (working)
- Color selection system (working)

---

## 🏁 **Next Phase Preparation**

After Milestone 6 completion, the project will be ready for:
- **Milestone 7**: Enhanced rule validation with visual feedback
- **Milestone 8**: Improved color selection integration
- **Phase 5**: Special card implementations with visual states

The visual card system will provide the foundation for all advanced card interactions and special card behaviors in subsequent milestones.






---



My Notes:

---

"1. Create Card Prefab
CardPrefab (GameObject + CardController)
├── CardBackground (Image)
├── CardFrontImage (Image) 
├── CardBackImage (Image)
└── CardButton (Button)
"

We don't HAVE a Background (and we don't need one)

---

```
		/// <summary>
		/// Load card images based on CardData
		/// For now, we'll use placeholder colored backgrounds
		/// TODO: Replace with actual scanned card images
		/// </summary>
		void LoadCardImages () {
			if (cardData == null) return;

			// For now, set background color based on card color
			// TODO: Replace with actual card front/back images
			if (cardBackground != null) {
				cardBackground.color = GetColorForCardColor (cardData.color);
			}

			// Set front image color (placeholder)
			if (cardFrontImage != null) {
				cardFrontImage.color = GetColorForCardColor (cardData.color);
			}

			// Set back image to neutral color (placeholder)
			if (cardBackImage != null) {
				cardBackImage.color = Color.gray;
			}
		}
```
I think we should use the scanned imaged already now, why wait and create more work.

---

In `CardController.cd`:
I see the line `public Color normalTint = Color.white;` - I don't think there IS a `normalTint`, if a card is not selected then it doesn't need to be modified.

---


Assets
├── Audio
│   ├── Music
│   └── Sfx
├── Data
│   ├── Cards
├── Plugins
├── Prefabs
│   ├── Cards
│   └── UI
├── Resources
│   ├── Data
│   │   └── Cards
│   └── Sprites
│       ├── Cards
│       │   └── Backs
│       ├── Fonts
│       │   ├── Blue
│       │   ├── Green
│       │   ├── Red
│       │   ├── Special
│       │   └── Yellow
│       └── UI
│           └── Backgrounds
├── Scenes
├── Scripts
└── TextMesh Pro
Packages

---

Also please give me much more clearer Unity Setup Instructions.


