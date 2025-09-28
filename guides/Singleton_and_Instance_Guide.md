# Singleton and Instance Patterns in Unity - TAKI Project Guide

## Part 1: Singleton Pattern

### What is a Singleton in Unity?

**Short**: A Singleton ensures that only one instance of a class exists throughout the entire game, providing global access to that single instance.

**Details**: The Singleton pattern is a design pattern that restricts a class to have only one instance and provides a global point of access to it. In Unity, this is commonly used for managers (GameManager, AudioManager, etc.) that should exist only once and be accessible from anywhere in the code. The pattern prevents multiple instances from being created and typically persists across scene changes.

### Core Singleton Characteristics

1. **Only One Instance**: Can't have multiple copies
2. **Global Access**: Accessible from anywhere via static reference
3. **Lazy Creation**: Often created when first accessed
4. **Persistence**: Usually survives scene changes
5. **Self-Managing**: Controls its own creation/destruction

---

### Basic Singleton Implementation

```csharp
public class GameManager : MonoBehaviour
{
    // Static reference to the single instance
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // Singleton logic - only one can exist
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }
}
```

### TAKI Project Singleton Example

**GameManager.cs - Our Main Singleton**
```csharp
public class GameManager : MonoBehaviour {
    private static GameManager instance;

    public static GameManager Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    void Awake() {
        // Singleton enforcement
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize game systems
        InitializeGame();
    }
}
```

**Usage Throughout TAKI Project:**
```csharp
// HandManager.cs - Accessing the singleton
if (GameManager.Instance != null) {
    GameManager.Instance.PlayCard(selectedCard);
}

// NetworkGameManager.cs - Using singleton for game state
GameManager.Instance.SetGameActive(true);

// Any script can access the single GameManager
GameManager.Instance.IsMultiplayerMode;
```

---

### Singleton Variations in TAKI

### 1. Simple Singleton (GameManager)
```csharp
public static GameManager Instance { get; private set; }

void Awake() {
    if (Instance != null && Instance != this) {
        Destroy(gameObject);
        return;
    }
    Instance = this;
}
```

### 2. Lazy Singleton with FindObjectOfType
```csharp
public static GameManager Instance {
    get {
        if (instance == null) {
            instance = FindObjectOfType<GameManager>();
        }
        return instance;
    }
}
```

### 3. Thread-Safe Singleton (Advanced)
```csharp
private static readonly object lockObject = new object();
public static GameManager Instance {
    get {
        if (instance == null) {
            lock (lockObject) {
                if (instance == null) {
                    instance = FindObjectOfType<GameManager>();
                }
            }
        }
        return instance;
    }
}
```

---

### Benefits of Singleton in TAKI

### ✅ Advantages
- **Global Access**: Any script can access `GameManager.Instance`
- **Single Source of Truth**: One place for game state
- **Persistence**: Survives scene changes (menu ↔ game transitions)
- **Memory Efficient**: Only one instance uses memory
- **Easy Communication**: No need to pass references around

### ❌ Disadvantages
- **Global State**: Can make testing difficult
- **Hidden Dependencies**: Hard to see what depends on singleton
- **Tight Coupling**: Classes become dependent on singleton
- **Difficult to Mock**: Testing becomes more complex

---

### Singleton Best Practices from TAKI

### ✅ DO:
```csharp
// 1. Use null checks before accessing
if (GameManager.Instance != null) {
    GameManager.Instance.PlayCard(card);
}

// 2. Implement proper cleanup
void OnDestroy() {
    if (instance == this) {
        instance = null;
    }
}

// 3. Use for managers that should truly be unique
// GameManager, AudioManager, NetworkManager

// 4. Initialize in Awake()
void Awake() {
    if (Instance != null && Instance != this) {
        Destroy(gameObject);
        return;
    }
    Instance = this;
}
```

### ❌ DON'T:
```csharp
// 1. Don't assume singleton exists without checking
GameManager.Instance.PlayCard(card); // Could be null!

// 2. Don't make everything a singleton
public class ButtonController : MonoBehaviour // This shouldn't be singleton!

// 3. Don't create singletons for data that could have multiple instances
public class PlayerData : MonoBehaviour // Multiple players need multiple instances!

// 4. Don't forget DontDestroyOnLoad for persistent singletons
// Missing DontDestroyOnLoad means singleton dies on scene change
```

---

## Part 2: Instance Pattern

### What is an Instance in Unity?

**Short**: An instance is a specific copy or occurrence of a class/object that exists in memory with its own unique data and state.

**Details**: In programming, an instance is a concrete realization of a class. When you create an object from a class, you're creating an instance. Each instance has its own memory space and can have different values for its variables. In Unity, every GameObject in your scene is an instance, and each component attached to GameObjects are also instances. Multiple instances of the same class can exist simultaneously, each with their own independent state.

### Core Instance Characteristics

1. **Unique Memory**: Each instance has its own memory space
2. **Independent State**: Variables can have different values per instance
3. **Multiple Existence**: Many instances of same class can exist
4. **Individual Lifecycle**: Created/destroyed independently
5. **Polymorphic Behavior**: Can behave differently based on their data

---

### Instance Examples in TAKI

### 1. CardData Instances
```csharp
// Multiple instances of CardData exist
CardData blueCard1 = Resources.Load<CardData>("Cards/Blue_5_01");
CardData blueCard2 = Resources.Load<CardData>("Cards/Blue_5_02");
CardData redCard = Resources.Load<CardData>("Cards/Red_7_01");

// Each instance has different properties
Debug.Log(blueCard1.cardName);  // "Blue 5"
Debug.Log(redCard.cardName);    // "Red 7"
Debug.Log(blueCard1.color);     // CardColor.Blue
Debug.Log(redCard.color);       // CardColor.Red
```

### 2. GameObject Instances
```csharp
// HandManager.cs - Creating multiple card GameObject instances
public void UpdateHandDisplay() {
    foreach (CardData cardData in currentHand) {
        GameObject cardInstance = CreateCardPrefab(cardData, index);
        // Each cardInstance is a separate GameObject with its own:
        // - Transform position
        // - Components
        // - Visual state
    }
}

// Result: Multiple card GameObjects in scene, each displaying different cards
```

### 3. Component Instances
```csharp
// Multiple HandManager instances can exist (Player1Panel, Player2Panel)
HandManager player1Hand = player1Panel.GetComponent<HandManager>();
HandManager player2Hand = player2Panel.GetComponent<HandManager>();

// Each instance manages different hands
player1Hand.currentHand = playerCards;    // Instance 1 has player cards
player2Hand.currentHand = opponentCards;  // Instance 2 has opponent cards

// Same class, different data and behavior per instance
```

---

### Instance Creation Methods

### 1. Constructor Instantiation
```csharp
// Creating instances of regular C# classes
List<CardData> playerHand = new List<CardData>();  // New instance
List<CardData> opponentHand = new List<CardData>(); // Another instance

// Each list is independent
playerHand.Add(card1);
opponentHand.Add(card2);
// They contain different cards
```

### 2. Unity Instantiate()
```csharp
// Creating GameObject instances from prefabs
GameObject cardInstance1 = Instantiate(cardPrefab, handContainer);
GameObject cardInstance2 = Instantiate(cardPrefab, handContainer);
GameObject cardInstance3 = Instantiate(cardPrefab, handContainer);

// Each instance:
// - Has unique Transform
// - Can be moved/rotated independently
// - Has its own component values
// - Can be destroyed separately
```

### 3. ScriptableObject Instances
```csharp
// Creating runtime CardData instances
CardData runtimeCard = ScriptableObject.CreateInstance<CardData>();
runtimeCard.cardName = "Custom Card";
runtimeCard.color = CardColor.Red;

// This instance exists only in memory, not as an asset file
```

---

### Instance Management in TAKI

### 1. Card Instance Tracking
```csharp
// HandManager.cs - Managing multiple card GameObject instances
private List<GameObject> cardInstances = new List<GameObject>();

void CreateCardPrefabs(List<CardData> hand) {
    // Create multiple instances
    for (int i = 0; i < hand.Count; i++) {
        GameObject cardInstance = CreateCardPrefab(hand[i], i);
        cardInstances.Add(cardInstance); // Track each instance
    }
}

void ClearHand() {
    // Destroy all instances
    foreach (GameObject instance in cardInstances) {
        if (instance != null) {
            Destroy(instance);
        }
    }
    cardInstances.Clear();
}
```

### 2. Multiple UI Manager Instances
```csharp
// GameManager.cs - Different UI manager instances for different modes
private SinglePlayerUIManager singlePlayerUI; // Instance 1
private MultiPlayerUIManager multiPlayerUI;   // Instance 2

void SwitchToSinglePlayer() {
    // Activate specific instance
    singlePlayerUI.enabled = true;
    multiPlayerUI.enabled = false;
}

void SwitchToMultiplayer() {
    // Activate different instance
    singlePlayerUI.enabled = false;
    multiPlayerUI.enabled = true;
}
```

### 3. Network Player Instances
```csharp
// NetworkGameManager.cs - Each player has their own game state instance
void SetupLocalMultiplayerHands() {
    // Each client has their own instance of the game data
    List<CardData> myHand = new List<CardData>();      // Local player instance
    List<CardData> opponentHand = new List<CardData>(); // Opponent instance

    // Same data structure, different instances, different contents
    myHand.AddRange(localPlayerCards);
    opponentHand.AddRange(remotePlayerCards);
}
```

---

### Instance vs Class vs Object

| Term | Definition | Example |
|------|------------|---------|
| **Class** | Blueprint/template | `public class CardData` |
| **Object** | Runtime entity created from class | The actual card in memory |
| **Instance** | Specific occurrence of a class | `CardData blueCard = new CardData()` |

```csharp
// Class definition - blueprint
public class CardData : ScriptableObject {
    public string cardName;
    public CardColor color;
}

// Creating instances - specific cards
CardData card1 = CreateInstance<CardData>(); // Instance 1
card1.cardName = "Blue 5";
card1.color = CardColor.Blue;

CardData card2 = CreateInstance<CardData>(); // Instance 2
card2.cardName = "Red 7";
card2.color = CardColor.Red;

// Same class, different instances, different data
```

---

### Instance Lifecycle

### Creation → Usage → Destruction
```csharp
// 1. Creation
GameObject cardInstance = Instantiate(cardPrefab);
CardController controller = cardInstance.GetComponent<CardController>();

// 2. Usage
controller.Initialize(cardData, index);
controller.SetClickable(true);
cardInstance.transform.position = targetPosition;

// 3. Destruction
Destroy(cardInstance); // Instance is removed from memory
```

---

### Common Instance Patterns in TAKI

### 1. Object Pooling (Performance)
```csharp
// Instead of constantly creating/destroying instances
public class CardPool : MonoBehaviour {
    private Queue<GameObject> pool = new Queue<GameObject>();

    public GameObject GetCardInstance() {
        if (pool.Count > 0) {
            return pool.Dequeue(); // Reuse existing instance
        }
        return Instantiate(cardPrefab); // Create new instance if needed
    }

    public void ReturnCardInstance(GameObject instance) {
        instance.SetActive(false);
        pool.Enqueue(instance); // Store for reuse
    }
}
```

### 2. Instance Factory Pattern
```csharp
// CardFactory.cs - Centralized instance creation
public static class CardFactory {
    public static GameObject CreateCardInstance(CardData data, Transform parent) {
        GameObject instance = Instantiate(cardPrefab, parent);
        CardController controller = instance.GetComponent<CardController>();
        controller.Initialize(data);
        return instance;
    }
}
```

### 3. Instance Reference Management
```csharp
// Keeping track of related instances
public class HandManager : MonoBehaviour {
    private Dictionary<CardData, GameObject> cardToInstance = new Dictionary<CardData, GameObject>();

    void CreateCard(CardData cardData) {
        GameObject instance = CreateCardPrefab(cardData);
        cardToInstance[cardData] = instance; // Link data to visual instance
    }

    void RemoveCard(CardData cardData) {
        if (cardToInstance.TryGetValue(cardData, out GameObject instance)) {
            Destroy(instance);
            cardToInstance.Remove(cardData);
        }
    }
}
```

---

## Singleton vs Instance - Key Differences

| Aspect | Singleton | Instance |
|--------|-----------|----------|
| **Count** | Only one exists | Multiple can exist |
| **Access** | Global static access | Reference-based access |
| **Purpose** | Shared global state | Individual objects with own state |
| **Memory** | One copy in memory | Each instance uses memory |
| **Example** | GameManager | Individual cards, UI elements |

### When to Use Each

**Use Singleton for:**
- Game managers (GameManager, AudioManager)
- Settings/configuration objects
- Services that should be unique (NetworkManager)

**Use Instances for:**
- Visual elements (cards, UI panels, buttons)
- Data objects (player stats, card data)
- Temporary objects (effects, animations)

---

## Summary

### Singleton Pattern
- **Purpose**: Ensure only one instance of a class exists globally
- **TAKI Example**: GameManager - one game state manager for entire application
- **Benefits**: Global access, single source of truth, persistence
- **Use Cases**: Managers, services, configurations

### Instance Pattern
- **Purpose**: Create multiple independent copies of a class with their own state
- **TAKI Example**: CardData objects - many cards with different properties
- **Benefits**: Independent state, scalability, object-oriented design
- **Use Cases**: Visual elements, data objects, game entities

**In TAKI Project:**
- **GameManager** = Singleton (one game state)
- **CardData objects** = Instances (many different cards)
- **HandManager components** = Instances (player hand vs opponent hand)
- **UI elements** = Instances (multiple buttons, panels, etc.)

Both patterns are essential for well-structured Unity projects - singletons for global management, instances for individual game objects!