# Unity Events - Complete Guide for TAKI Project

## What is an Event in Unity?

**Short**: An event is a communication mechanism that allows one object to notify multiple other objects when something happens, without knowing who's listening.

**Details**: Events in Unity are based on C#'s event system and follow the Observer pattern. When something significant happens (like a button click, card played, or game state change), an event is "fired" or "invoked". Any objects that have "subscribed" to this event will automatically have their methods called. This creates loose coupling - the event sender doesn't need to know who's listening, and listeners don't need direct references to the sender.

---

## Types of Events in Unity

### 1. C# Events (Code-Based)
**Definition**: Events declared in scripts using C# event keyword
```csharp
public event Action<CardData> OnCardPlayed;
public event System.Action OnGameEnded;
```

### 2. UnityEvents (Inspector-Assignable)
**Definition**: Unity's serializable event system visible in Inspector
```csharp
public UnityEvent OnButtonClick;
public UnityEvent<string> OnPlayerNameChanged;
```

### 3. UI Events (Built-in)
**Definition**: Unity's built-in UI system events (buttons, toggles, etc.)
- Button.onClick
- Toggle.onValueChanged
- InputField.onValueChanged

---

## How Events Work - The Observer Pattern

```
Event Publisher          Event System          Event Subscribers
     ↓                        ↓                       ↓
[Card Played] -----> [OnCardPlayed Event] -----> [Update UI]
                              ↓                  [Play Sound]
                              ↓                  [Check Win]
                              ↓                  [Network Sync]
```

**Key Concept**: One event can notify multiple listeners simultaneously, and the publisher doesn't need to know who's listening.

---

## Event Lifecycle

### 1. Declaration
```csharp
public class GameManager : MonoBehaviour {
    public event Action<CardData> OnCardPlayed; // Declare the event
}
```

### 2. Subscription
```csharp
public class UIManager : MonoBehaviour {
    void Start() {
        gameManager.OnCardPlayed += HandleCardPlayed; // Subscribe
    }

    void HandleCardPlayed(CardData card) {
        // React to the event
    }
}
```

### 3. Invocation
```csharp
public class GameManager : MonoBehaviour {
    void PlayCard(CardData card) {
        // Do the action
        // ...

        // Notify everyone
        OnCardPlayed?.Invoke(card); // Fire the event
    }
}
```

### 4. Unsubscription
```csharp
void OnDestroy() {
    gameManager.OnCardPlayed -= HandleCardPlayed; // Unsubscribe to prevent memory leaks
}
```

---

## TAKI Project Event Examples

### Example 1: Card Play Events
**GameManager.cs**
```csharp
// Event declaration
public event System.Action<CardData, PlayerType> OnCardPlayed;

// Event invocation
public void PlayCard(CardData card, PlayerType player) {
    // Game logic...
    currentTopCard = card;

    // Notify all subscribers
    OnCardPlayed?.Invoke(card, player);
}
```

**Multiple subscribers can react:**
```csharp
// UIManager.cs
gameManager.OnCardPlayed += UpdateCardDisplay;

// SoundManager.cs
gameManager.OnCardPlayed += PlayCardSound;

// NetworkManager.cs
gameManager.OnCardPlayed += SyncCardToNetwork;
```

### Example 2: UI Button Events
**BaseGameplayUIManager.cs**
```csharp
// UnityEvent in Inspector
public UnityEvent OnEndTurnClicked;

// Code subscription
void Start() {
    endTurnButton.onClick.AddListener(HandleEndTurnClick);
}

void HandleEndTurnClick() {
    // Handle the button click
    OnEndTurnClicked?.Invoke(); // Chain the event
}
```

### Example 3: Game State Events
**GameStateManager.cs**
```csharp
public event System.Action<GameStatus> OnGameStatusChanged;

public void SetGameStatus(GameStatus newStatus) {
    if (gameStatus != newStatus) {
        gameStatus = newStatus;
        OnGameStatusChanged?.Invoke(newStatus); // State change notification
    }
}
```

---

## Event Patterns in TAKI

### 1. Manager-to-UI Communication
**Pattern**: Game managers fire events, UI managers listen
```csharp
// GameManager fires
OnCardPlayed?.Invoke(card, player);

// UIManager listens
gameManager.OnCardPlayed += (card, player) => {
    UpdateHandDisplay();
    ShowCardPlayAnimation(card);
};
```

### 2. Chain Events
**Pattern**: One event triggers another event
```csharp
// Button click triggers custom event
button.onClick.AddListener(() => OnCustomAction?.Invoke());

// Custom event triggers game logic
OnCustomAction += gameManager.HandleCustomAction;
```

### 3. Mode-Aware Events
**Pattern**: Events that behave differently based on game mode
```csharp
// GameManager.cs
OnCardPlayed += (card, player) => {
    if (isMultiplayerMode) {
        networkManager.SyncCardPlay(card);
    } else {
        aiManager.ReactToPlayerCard(card);
    }
};
```

---

## Benefits of Events in TAKI

### 1. Loose Coupling
**Without Events** (tight coupling):
```csharp
void PlayCard(CardData card) {
    // Directly calling each system - tightly coupled
    uiManager.UpdateDisplay(card);
    soundManager.PlaySound(card);
    networkManager.SyncCard(card);
    // Adding new system requires modifying this method
}
```

**With Events** (loose coupling):
```csharp
void PlayCard(CardData card) {
    // Just fire the event - loosely coupled
    OnCardPlayed?.Invoke(card);
    // New systems can subscribe without modifying this code
}
```

### 2. Easy Feature Addition
Adding a new feature (like achievements) is simple:
```csharp
// New AchievementManager.cs
void Start() {
    gameManager.OnCardPlayed += CheckCardPlayAchievements; // Just subscribe!
}
```

### 3. Clean Architecture
Events create clear boundaries between systems:
- **GameManager**: Manages game logic, fires events
- **UIManager**: Listens to events, updates display
- **NetworkManager**: Listens to events, syncs network
- **SoundManager**: Listens to events, plays audio

---

## Common Event Patterns

### 1. Action Events (No Parameters)
```csharp
public event System.Action OnGameStarted;
OnGameStarted?.Invoke();
```

### 2. Action Events (With Parameters)
```csharp
public event System.Action<int> OnScoreChanged;
OnScoreChanged?.Invoke(newScore);
```

### 3. Custom Event Args
```csharp
public class CardPlayedEventArgs {
    public CardData Card { get; set; }
    public PlayerType Player { get; set; }
    public bool IsValidPlay { get; set; }
}

public event System.Action<CardPlayedEventArgs> OnCardPlayedDetailed;
```

### 4. UnityEvents for Inspector
```csharp
[System.Serializable]
public class CardEvent : UnityEvent<CardData> { }

public CardEvent OnCardSelected; // Shows in Inspector
```

---

## Event Best Practices from TAKI

### ✅ DO:
```csharp
// 1. Always check for null before invoking
OnCardPlayed?.Invoke(card);

// 2. Unsubscribe in OnDestroy to prevent memory leaks
void OnDestroy() {
    if (gameManager != null) {
        gameManager.OnCardPlayed -= HandleCardPlayed;
    }
}

// 3. Use descriptive event names
public event Action<CardData> OnCardPlayedSuccessfully;
public event Action<string> OnPlayerJoinedGame;

// 4. Keep event handlers simple
void HandleCardPlayed(CardData card) {
    UpdateUI(card); // Simple, focused responsibility
}
```

### ❌ DON'T:
```csharp
// 1. Don't invoke without null check (will crash if no subscribers)
OnCardPlayed(card); // DANGEROUS!

// 2. Don't forget to unsubscribe
// Missing OnDestroy() causes memory leaks

// 3. Don't put complex logic in event handlers
void HandleCardPlayed(CardData card) {
    // 50 lines of complex game logic... // BAD!
}

// 4. Don't create circular event dependencies
// A fires event -> B listens -> B fires event -> A listens (INFINITE LOOP!)
```

---

## Events vs Other Communication Methods

| Method | Use Case | Pros | Cons |
|--------|----------|------|------|
| **Events** | One-to-many notifications | Loose coupling, scalable | Setup complexity |
| **Direct References** | Simple one-to-one | Simple, fast | Tight coupling |
| **FindObjectOfType** | Finding objects | Easy to use | Slow, rigid |
| **Static Methods** | Global access | No references needed | Hard to test, global state |
| **Interfaces** | Polymorphic behavior | Clean contracts | Setup complexity |

---

## Network Events in TAKI

Events work with networking by translating local events to network messages:

```csharp
// Local event fired
OnCardPlayed?.Invoke(card, PlayerType.Human);

// Network manager listens and translates
gameManager.OnCardPlayed += (card, player) => {
    if (isMultiplayerMode && player == PlayerType.Human) {
        SendNetworkCardPlay(card.cardName); // Convert to network message
    }
};

// Other clients receive network message and fire local event
[PunRPC]
void ReceiveCardPlay(string cardId) {
    CardData card = LoadCard(cardId);
    OnOpponentCardPlayed?.Invoke(card); // Fire local event for opponent play
}
```

---

## Debugging Events

### Event Subscription Tracking
```csharp
public event System.Action<CardData> OnCardPlayed {
    add {
        _onCardPlayed += value;
        Debug.Log($"Subscribed to OnCardPlayed: {value.Method.Name}");
    }
    remove {
        _onCardPlayed -= value;
        Debug.Log($"Unsubscribed from OnCardPlayed: {value.Method.Name}");
    }
}
private System.Action<CardData> _onCardPlayed;
```

### Event Invocation Logging
```csharp
void FireCardPlayedEvent(CardData card) {
    Debug.Log($"Firing OnCardPlayed event for {card.cardName}");
    OnCardPlayed?.Invoke(card);
}
```

---

## Summary

**Events in Unity are the communication backbone of well-architected games.**

In TAKI project:
- **Game logic** fires events when important things happen
- **UI systems** listen to events and update displays
- **Network systems** listen to events and sync data
- **Audio systems** listen to events and play sounds

**Key Benefits:**
- **Loose Coupling**: Systems don't need direct references
- **Scalability**: Easy to add new features
- **Clean Architecture**: Clear separation of concerns
- **Maintainability**: Changes in one system don't break others

**Remember**: Events are about communication, not control. Use them to say "something happened" and let other systems decide how to react!