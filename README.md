# TAKI Game Development Plan - Unity Engine
## Comprehensive Implementation Guide (Updated)

### ⚠️ CRITICAL NOTES
- **AVOID UNICODE**: No special characters in code, file names, text displays, or comments
- **Current Status**: Phase 1 Complete ✅, Milestone 3 Complete ✅, Currently at Milestone 4 🎯
- **Target Platform**: PC/Desktop Unity Build
- **Scope**: Singleplayer (Human vs Computer) with multiplayer-ready architecture

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
- ✅ Complete enum system (CardColor, CardType, PlayerType, GameState)
- ✅ CardData ScriptableObject with helper methods
- ✅ Namespace organization (`TakiGame`)
- ✅ 14 test cards created and validated
- ✅ UI integration tested (CardDataTester)

### Milestone 4: Complete Deck System 🎯 CURRENT TARGET
**Objective**: Refactor DeckManager architecture and implement complete deck system

## Current DeckManager Problems (Too Many Responsibilities!)
1. **Deck Data Loading** - Loading CardData from Resources
2. **Deck State Management** - Managing draw/discard piles
3. **Card Operations** - Drawing, discarding, shuffling
4. **UI Management** - Updating text components and panels
5. **Message Display** - Showing game messages
6. **Game Setup Logic** - Initial dealing and game state setup  
7. **Statistics/Debugging** - Providing deck stats

#### Substep 4.1: Create Specialized Managers
**Objective**: Split DeckManager responsibilities into focused components following Single Responsibility Principle

**1. Pure Deck Operations**:
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

**2. Resource Management**:
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

**3. UI Updates Only**:
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

**4. Game Setup Logic**:
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

**5. Simplified DeckManager (Coordinator)**:
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

**Acceptance Criteria**:
✅ All components properly separated with single responsibilities  
✅ Event system working correctly between components  
✅ Existing functionality preserved after refactoring  
✅ No breaking changes to public API  
✅ 110 cards load and distribute correctly  
✅ UI updates reflect deck state accurately  
✅ Improved code maintainability and testability  
✅ Architecture prepared for future multiplayer features

**Benefits of This Refactoring**:
✅ **Single Responsibility** - Each class has one clear job  
✅ **Easier Testing** - Test deck operations separate from UI  
✅ **Better Maintainability** - Changes to UI don't affect deck logic  
✅ **Preparation for Multiplayer** - Deck logic separate from UI makes networking easier  
✅ **Code Reusability** - Other systems can use Deck without UI dependencies

---

## Phase 3: Basic Gameplay Loop

### Milestone 5: Turn Management System
**Objective**: Implement turn-based mechanics with basic computer AI

#### Substep 5.1: GameManager Core
```csharp
public class GameManager : MonoBehaviour 
{
    public enum GameState { 
        PlayerTurn, ComputerTurn, ColorSelection, 
        TakiSequence, PlusTwoChain, GameOver, Paused 
    }
    
    [Header("Game State")]
    public GameState currentState;
    public PlayerType currentPlayer;
    public CardColor activeColor;
    
    [Header("Managers")]
    public DeckManager deckManager;
    public HandManager handManager;
    public TurnManager turnManager;
}
```

#### Substep 5.2: TurnManager Implementation
**Features**:
- Turn switching logic
- UI updates (enable/disable buttons)  
- Turn timeout handling
- Action validation

#### Substep 5.3: Basic Computer AI
**Simple AI Strategy**:
1. Find all valid cards to play
2. Prioritize special cards over numbers
3. Random selection from valid moves
4. Draw if no valid moves

#### Substep 5.4: UI Integration
- Connect action buttons to turn system
- Update turn indicator text
- Show whose turn it is
- Enable/disable appropriate buttons

### Milestone 6: Hand Management System  
**Objective**: Visual card representation and hand interaction

#### Substep 6.1: Card Prefab Creation
**Card Prefab Structure**:
```
CardPrefab
├── CardImage (Image)
├── CardButton (Button)  
├── CardNumberText (TextMeshProUGUI)
├── CardTypeText (TextMeshProUGUI)
└── CardController (Script)
```

#### Substep 6.2: Hand Display System
**HandManager Features**:
- Dynamic card instantiation
- Proper card spacing and layout
- Hand size management
- Card selection highlighting

#### Substep 6.3: Card Interaction
- Click to select cards
- Visual feedback for valid/invalid moves
- Hover effects
- Hand reorganization

---

## Phase 4: Core Rules Implementation

### Milestone 7: Basic Card Rules System
**Objective**: Implement number card matching and basic play validation

#### Substep 7.1: Card Validation System
```csharp
public class CardPlayValidator : MonoBehaviour 
{
    public bool CanPlayCard(CardData cardToPlay, CardData topCard, CardColor activeColor)
    {
        // Rule 1: Wild cards can always be played
        if (cardToPlay.IsWildCard) return true;
        
        // Rule 2: Color matching
        if (cardToPlay.color == activeColor || cardToPlay.color == topCard.color) 
            return true;
            
        // Rule 3: Number matching (for number cards)
        if (cardToPlay.cardType == CardType.Number && 
            topCard.cardType == CardType.Number && 
            cardToPlay.number == topCard.number) return true;
            
        // Rule 4: Special card type matching  
        if (cardToPlay.cardType == topCard.cardType && 
            cardToPlay.cardType != CardType.Number) return true;
            
        return false;
    }
}
```

#### Substep 7.2: Game Rules Engine
- Implement all number card rules
- Turn ending conditions
- Win condition detection
- Rule violation handling

#### Substep 7.3: UI Feedback System
- Show valid/invalid move messages
- Highlight playable cards
- Display rule explanations
- Error message system

### Milestone 8: Color Selection System
**Objective**: Implement ChangeColor card functionality

#### Substep 8.1: Color Selection UI
**Features**:
- Show/hide ColorSelectionPanel
- Color button interactions
- Visual active color indicator
- Cancel selection option

#### Substep 8.2: Color Change Logic
- Detect ChangeColor card plays
- Pause game for color selection
- Apply new color to game state
- Resume normal play

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

## Development Guidelines

### Code Quality Standards
```csharp
// Namespace for all scripts
namespace TakiGame 
{
    // Clear, descriptive class names
    public class CardPlayValidator : MonoBehaviour
    {
        // Public fields with tooltips
        [Tooltip("Card to validate for play")]
        public CardData targetCard;
        
        // NO UNICODE in comments or strings
        // Use simple ASCII characters only
        
        /// <summary>
        /// Validates if card can be played according to TAKI rules
        /// </summary>
        public bool ValidateCardPlay()
        {
            // Implementation here
        }
    }
}
```

### File Organization Rules
- **Scripts**: Organized by responsibility (Managers/, Controllers/, UI/, Data/)
- **Assets**: Clear folder structure with no Unicode characters
- **Naming**: PascalCase for files, camelCase for variables
- **Comments**: English only, no special characters

### Refactored Architecture Benefits
**New Component Structure**:
```
DeckManager (Coordinator)
├── Deck (Pure card operations)
├── CardDataLoader (Resource management) 
├── DeckUIManager (UI updates only)
└── GameSetupManager (Game initialization)
```

**Advantages**:
- **Separation of Concerns**: Each component has a single, well-defined responsibility
- **Testability**: Components can be tested in isolation
- **Maintainability**: Changes to one aspect don't affect others
- **Reusability**: Components can be used by other systems
- **Multiplayer Ready**: Network synchronization easier with separated logic
- **Performance**: UI updates only when necessary

### Testing Strategy
1. **Unit Testing**: Each component tested independently
2. **Integration Testing**: Test component interactions  
3. **Gameplay Testing**: Verify rules work as expected
4. **Performance Testing**: Ensure smooth gameplay with full deck

### Error Handling
- Null reference checks for all card operations
- Graceful handling of empty piles
- User-friendly error messages
- Debug logging for development

---

## Risk Mitigation

### Potential Challenges
1. **Component Communication**: Mitigated by clear event system and coordinator pattern
2. **Computer AI Difficulty**: Start simple, iterate based on gameplay
3. **Special Card Interactions**: Implement one at a time, test thoroughly
4. **UI Responsiveness**: Regular testing on target hardware

### Fallback Plans
- **Complex Cards**: If SuperTaki is too complex, implement basic TAKI first
- **AI Intelligence**: Random valid moves acceptable for first version  
- **Visual Polish**: Prioritize functionality over aesthetics
- **Audio**: Optional feature, can be added post-core completion

---

## Success Metrics

### Milestone 4 Success Criteria
✅ All components properly separated with single responsibilities  
✅ Event system working correctly between components  
✅ Existing functionality preserved after refactoring  
✅ No breaking changes to public API  
✅ Exactly 110 cards load and distribute correctly  
✅ Proper file naming convention followed  
✅ Initial game setup works (8+8+1 cards distributed)  
✅ UI updates reflect deck state accurately  
✅ No performance issues with full deck operations
✅ Improved code maintainability and testability  
✅ Architecture prepared for future multiplayer features

### Overall Project Success  
✅ Complete playable TAKI game (Human vs Computer)  
✅ All major card types implemented correctly  
✅ Intuitive UI with clear feedback  
✅ Stable gameplay without crashes  
✅ Clean, maintainable code architecture  
✅ Code ready for multiplayer extension  
✅ No Unicode issues in any files or displays

---

# My Notes:

- In `DeckManager.cs`:

    - What does `reshuffleThreshold` mean? I believe we should always reshuffle (meaning take all cards in discard pile minus the card that's at the top, shuffle them, and place in empty draw pile) the moment we see that: there are no cards in draw pile AND there is at least 2 cards in discard pile (if we only take 1 card to move over to draw pile, then no worries, the shuffle logic will simply not take place).
    So what we should do, is that EVERY TIME a (1) card is being drawn from the draw pile we check immidietly after: is the pile empty.
    So cards' logics have to be mindfull of this (the answer is making simple functions with uncomplicated responsebilities that can use other functions), for example, PlusTwo - you take 1 card, and then another, not a few at once.

    - `if (allCardData.Count == 110)` instead of `110` we should create a variable for expected initial card amount

    - In `DrawCard` we should reconsider it's implementation, since we might change/reconsider `reshuffleThreshold`

    - `DrawCards` looks well implemented and safe from what I can see since it calls `DrawCard` in a loop with checking. But I think we should add a logging message if the number of the count of requested cards hadn't been reached (due to no cards being available).

    - So, `DiscardCard` function is when a card from the draw pile is removed, and then add to the discard pile, But not when a card is being played by one of the players, right?

    - We have the following comment and function: 
        ```
        /// <summary>
		/// Set up initial game state (deal cards and place first discard)
		/// </summary>
		/// <returns>Lists of cards for each player and the starting discard card</returns>
		public (List<CardData> player1Hand, List<CardData> player2Hand, CardData startingCard) SetupInitialGame () {
        ```
    Tell me, what kind of function "title" is that? Does it have no name? How is it being called?

    - `UpdateUI` looks fine, but prompt me and I will tell you how I want to change the messages' text

    - 
    


