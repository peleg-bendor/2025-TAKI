# TAKI Game Development Plan - Unity Engine

Comprehensive Phase & Milestone Breakdown

## Project Status Overview

FOUNDATION COMPLETE - Excellent work on Phase 1! Your data architecture, UI hierarchy, and menu system provide a rock-solid foundation for the entire game.

CURRENT TARGET: Phase 2, Milestone 4 - Deck Management System

---

## Phase 1: Foundation Setup [COMPLETE]

### Milestone 1: Menu System [COMPLETE]

- Main Menu scene with navigation

- Scene transition system  

- UI responsiveness tested

### Milestone 2: UI Framework Creation [COMPLETE]

- All UI panels and buttons created

- Proper hierarchy established

- Screen responsiveness verified

### Milestone 3: Data Architecture Implementation [COMPLETE] 

- Enums.cs with all game states defined

- CardData ScriptableObject with helper methods

- 14 test CardData assets created

- Namespace TakiGame applied throughout

- Integration test working with UI

---

## Phase 2: Core Card System

### Milestone 3: Data Architecture Implementation [COMPLETE]

Achievements:

-  Complete enum system (CardColor, CardType, PlayerType, GameState)

-  CardData ScriptableObject with helper methods

-  Namespace organization (`TakiGame`)

-  14 test cards created and validated

-  UI integration tested (CardDataTester)

### Milestone 4: Complete Deck System [CURRENT TARGET]

Objective: Refactor DeckManager architecture and implement complete deck system

## Current DeckManager Problems (Too Many Responsibilities!)

1. Deck Data Loading - Loading CardData from Resources

2. Deck State Management - Managing draw/discard piles

3. Card Operations - Drawing, discarding, shuffling

4. UI Management - Updating text components and panels

5. Message Display - Showing game messages

6. Game Setup Logic - Initial dealing and game state setup  

7. Statistics/Debugging - Providing deck stats

#### Substep 4.1: Create Specialized Managers

Objective: Split DeckManager responsibilities into focused components following Single Responsibility Principle

1. Pure Deck Operations:

```csharp

public class Deck : MonoBehaviour 

{

    [Header("Deck State")]

    public List<CardData> drawPile = new List<CardData>();

    public List<CardData> discardPile = new List<CardData>();

    

    [Header("Settings")]

    public int reshuffleThreshold = 5;

    

    // Events for other systems

    public System.Action<CardData> OnCardDrawn;

    public System.Action<CardData> OnCardDiscarded;

    public System.Action OnDeckShuffled;

    public System.Action OnDeckEmpty;

    

    public CardData DrawCard()

    public void DiscardCard(CardData card)

    public void Shuffle()

    public void ReshuffleDiscardIntoDraw()

    public CardData GetTopDiscardCard()

    

    // Properties

    public int DrawPileCount => drawPile.Count;

    public int DiscardPileCount => discardPile.Count;

    public bool HasCardsInDrawPile => drawPile.Count > 0;

    public bool HasCardsInDiscardPile => discardPile.Count > 0;

}

```

2. Resource Management:

```csharp

public class CardDataLoader : MonoBehaviour 

{

    [Header("Card Data")]

    public List<CardData> allCardData = new List<CardData>();

    

    public List<CardData> LoadAllCardData()

    public string GetDeckStats()

    public bool ValidateDeckComposition()

    

    // Properties

    public int TotalCardCount => allCardData.Count;

}

```

3. UI Updates Only:

```csharp

public class DeckUIManager : MonoBehaviour 

{

    [Header("UI References")]

    public TextMeshProUGUI drawPileCountText;

    public TextMeshProUGUI discardPileCountText;

    public TextMeshProUGUI gameMessageText;

    public Transform drawPilePanel;

    public Transform discardPilePanel;

    

    public void UpdateDeckUI(int drawCount, int discardCount)

    public void ShowMessage(string message)

    public void UpdateDiscardPileDisplay(CardData topCard)

}

```

4. Game Setup Logic:

```csharp

public class GameSetupManager : MonoBehaviour 

{

    [Header("Setup Settings")]

    public int initialHandSize = 8;

    

    [Header("Dependencies")]

    public CardDataLoader cardLoader;

    public Deck deck;

    

    public void InitializeNewGame()

    public (List<CardData> player1Hand, List<CardData> player2Hand, CardData startingCard) SetupInitialGame()

    public List<CardData> DrawInitialHand(int handSize)

    public CardData SelectStartingCard()

}

```

#### Substep 4.2: Refactor DeckManager (Coordinator Pattern)

5. Simplified DeckManager (Coordinator):

```csharp

public class DeckManager : MonoBehaviour 

{

    [Header("Component References")]

    public Deck deck;

    public CardDataLoader cardLoader;

    public DeckUIManager deckUI;

    public GameSetupManager gameSetup;

    

    // Events for external systems

    public System.Action<CardData> OnCardDrawn;

    public System.Action<CardData> OnCardDiscarded;

    public System.Action OnDeckShuffled;

    public System.Action OnGameInitialized;

    

    void Start()

    void ConnectEvents()

    

    // Public API - delegates to appropriate managers

    public CardData DrawCard() => deck.DrawCard();

    public void DiscardCard(CardData card) => deck.DiscardCard(card);

    public List<CardData> DrawCards(int count) => gameSetup.DrawInitialHand(count);

    public CardData GetTopDiscardCard() => deck.GetTopDiscardCard();

    public void InitializeDeck() => gameSetup.InitializeNewGame();

    public (List<CardData>, List<CardData>, CardData) SetupInitialGame() => gameSetup.SetupInitialGame();

    

    // Properties - delegates to components

    public int DrawPileCount => deck.DrawPileCount;

    public int DiscardPileCount => deck.DiscardPileCount;

    public bool HasCardsInDrawPile => deck.HasCardsInDrawPile;

    public bool HasCardsInDiscardPile => deck.HasCardsInDiscardPile;

}

```

#### Substep 4.3: Integration & Testing

- Connect all components together with proper event system

- Verify all existing functionality still works

- Test that separation doesn't break anything

- Update any dependent scripts

- Ensure UI updates work properly between components

Acceptance Criteria:

- All components properly separated with single responsibilities  

- Event system working correctly between components  

- Existing functionality preserved after refactoring  

- No breaking changes to public API  

- 110 cards load and distribute correctly  

- UI updates reflect deck state accurately  

- Improved code maintainability and testability  

- Architecture prepared for future multiplayer features

Benefits of This Refactoring:

- Single Responsibility - Each class has one clear job  

- Easier Testing - Test deck operations separate from UI  

- Better Maintainability - Changes to UI don't affect deck logic  

- Preparation for Multiplayer - Deck logic separate from UI makes networking easier  

- Code Reusability - Other systems can use Deck without UI dependencies

---

## Phase 3: Basic Gameplay Loop

### Milestone 6: Turn Management System

Objective: Implement core turn-based mechanics

#### Substep 6A: GameManager Implementation

Script: Scripts/Managers/GameManager.cs 

Deliverables:

- Game state management (GameState enum integration)

- Player turn tracking

- Game initialization sequence

- Win condition detection

#### Substep 6B: TurnManager Implementation  

Script: Scripts/Core/TurnManager.cs

Deliverables:

- Turn switching logic

- UI updates for TurnIndicatorText

- Enable/disable action buttons based on turn

- Turn action validation

#### Substep 6C: Basic Computer AI

Script: Scripts/AI/ComputerPlayer.cs

Deliverables:

- Random valid move selection

- Card play logic for computer

- Draw card when no valid plays

- End turn automatically

### Milestone 7: Card Play Validation System

Objective: Implement TAKI game rules for card plays

#### Substep 7A: Basic Card Rules

Script: Scripts/Core/GameRules.cs

Deliverables:

- CanPlayCard() method for basic number/color matching

- Color matching validation

- Number matching validation

- Wild card play validation

#### Substep 7B: Player Action Implementation

Deliverables:

- Connect Btn_PlayerPlayCard to card selection

- Connect Btn_PlayerDrawCard to deck system

- Connect Btn_PlayerEndTurn to turn manager

- Add GameMessageText feedback for all actions

#### Substep 7C: Card Selection System

Deliverables:

- Click to select cards from hand

- Visual feedback for selected cards

- Deselect cards when invalid

- Play selected card functionality

---

## Phase 4: Core Rules Implementation  

### Milestone 8: Basic Card Rules Integration

Objective: Full implementation of number and color matching

#### Substep 8A: Turn Action Restrictions

Deliverables:

- If player plays card: disable draw, enable end turn

- If player draws card: disable play, enable end turn only

- Enforce "one action per turn" rule

- Update UI feedback accordingly

#### Substep 8B: Game Flow Testing

Deliverables:

- Complete turn cycle testing

- Valid/invalid move feedback

- Computer AI integration testing

- Turn indicator accuracy

### Milestone 9: Color Selection System

Objective: Implement ChangeColor card functionality

#### Substep 9A: ColorSelectionPanel Integration

Deliverables:

- Show/hide ColorSelectionPanel when ChangeColor played

- Connect color selection buttons to game logic

- Update CurrentColorIndicator visual feedback

- GameState.ColorSelection handling

#### Substep 9B: Wild Card Logic

Deliverables:

- ChangeColor card play validation

- Color selection for both human and computer

- Update game rules to respect selected color

- Visual feedback for active color

---

## Phase 5: Win Conditions & Game Flow

### Milestone 10: End Game System

Objective: Detect and handle game completion

#### Substep 10A: Win Condition Detection

Deliverables:

- Detect when player hand is empty after turn end

- Declare winner (human vs computer)

- Update GameMessageText with winner announcement

- Disable all game actions when game ends

#### Substep 10B: Game End UI

Deliverables:

- Show Play Again button

- Show Return to Menu button  

- Game statistics display (optional)

- Proper game state cleanup

### Milestone 11: Special Cards Implementation - Part 1

Objective: Add Plus and Stop cards

#### Substep 11A: Plus Card Logic

Deliverables:

- Plus card allows additional card play in same turn

- Update turn restrictions for Plus cards

- Chain Plus card functionality

- Computer AI handles Plus cards

#### Substep 11B: Stop Card Logic  

Deliverables:

- Stop card skips opponent's turn

- Turn manager handles skip logic

- Visual feedback for skipped turns

- Computer AI plays Stop cards strategically

---

## Phase 6: Advanced Special Cards

### Milestone 12: Special Cards Implementation - Part 2

Objective: Add PlusTwo and Taki cards

#### Substep 12A: PlusTwo Card System

Deliverables:

- Force opponent to draw 2 cards

- Skip opponent's turn after drawing

- Computer AI handles PlusTwo strategically

- Visual feedback for forced draws

#### Substep 12B: Taki Card System

Deliverables:

- Taki allows multiple cards of same color

- Continue until non-matching card or choice to stop

- Computer AI Taki chain logic

- Special Taki turn state management

#### Substep 12C: TakiAllColors Implementation

Deliverables:

- TakiAllColors allows any color sequence  

- Most powerful card implementation

- Computer AI strategic usage

- Complex turn state handling

---

## Phase 7: Polish & Quality Assurance

### Milestone 13: Game Polish

Objective: Improve user experience and fix edge cases

#### Substep 13A: Error Handling

Deliverables:

- Handle empty deck scenarios

- Invalid move prevention

- Edge case testing and fixes

- Graceful error messages

#### Substep 13B: UI Improvements

Deliverables:

- Better visual feedback for all actions

- Improved card layout and spacing

- Consistent button states and messaging

- Accessibility improvements

### Milestone 14: Testing & Optimization

Objective: Comprehensive testing and bug fixes

#### Substep 14A: Gameplay Testing

Deliverables:

- Complete game playthrough testing

- All card combinations tested

- Computer AI behavior verification

- Performance optimization

#### Substep 14B: Code Review & Documentation

Deliverables:

- Code cleanup and commenting

- Remove temporary testing scripts

- Documentation for future multiplayer expansion

- Final project organization

---

## Phase 8: Future-Proofing (Optional)

### Milestone 15: Multiplayer Preparation

Objective: Prepare architecture for multiplayer expansion

#### Substep 15A: Network Architecture Planning

Deliverables:

- Identify components that need networking

- Plan for player synchronization

- Design network message system

- Create multiplayer roadmap

---

## Development Notes

### Key Design Principles

- Single Responsibility: Each script handles one specific task

- Future-Proof: Architecture supports multiplayer expansion

- Beginner-Friendly: Clear, documented code suitable for learning

- Modular Design: Easy to test and debug individual components

### Recommended Development Order

1. Complete one milestone fully before moving to next

2. Test thoroughly at each substep

3. Keep temporary testing scripts until feature is stable

4. Document any deviations from plan

5. Regular code reviews and refactoring

### Critical Success Factors

- UI Integration: Every system must properly update your existing UI

- State Management: Consistent game state handling throughout

- Error Prevention: Validate all user actions before execution

- Computer AI: Simple but functional AI that follows all rules

- Code Organization: Maintain clean separation of concerns