# Prefabs and ScriptableObjects in TAKI Card Game

## What are Prefabs?

**Short**: Prefabs are reusable GameObject templates that can be instantiated multiple times in your scene.

**Details**: Think of prefabs as "stamps" or "cookie cutters" - you create one template and can make many copies. Each copy (instance) can have slight variations but shares the same basic structure. In Unity, prefabs contain GameObjects with components, transforms, and references to other assets.

**When to use**: Use prefabs for any GameObject you need multiple copies of - UI elements, characters, projectiles, cards, etc.

---

## What are ScriptableObjects?

**Short**: ScriptableObjects are data containers that store information independently of GameObjects and scenes.

**Details**: ScriptableObjects are pure data holders that exist as asset files. They don't appear in scenes directly but can be referenced by components. They're perfect for storing game data like item stats, card properties, or configuration settings. Multiple objects can reference the same ScriptableObject without duplicating data.

**When to use**: Use ScriptableObjects for data that needs to be shared, configured by designers, or exists independently of scene objects.

---

## Key Differences

| Aspect | Prefabs | ScriptableObjects |
|--------|---------|-------------------|
| **Purpose** | Visual GameObjects | Pure data storage |
| **In Scene** | Can be placed in scenes | Never in scenes directly |
| **Components** | Contains Unity components | No components, just data |
| **Instantiation** | `Instantiate()` creates GameObjects | `CreateInstance()` creates data |
| **Memory** | Each instance uses memory | Shared data, minimal memory |
| **Use Case** | Visual elements, interactive objects | Configuration, stats, rules |

---

## TAKI Project Implementation

### Our Card System Architecture

```
CardData (ScriptableObject) ←→ CardPrefab (Prefab)
      ↓                              ↓
   Game Rules                   Visual Display
   Card Properties              UI Interaction
   Sprites References           Click Handling
```

### Files Involved

**ScriptableObject System:**
- `CardData.cs` - Main ScriptableObject class defining card properties
- `CardDataHelper.cs` - Runtime creation utilities
- `Assets/Resources/Data/Cards/*.asset` - Individual card data files

**Prefab System:**
- `Assets/Prefabs/Cards/CardPrefab.prefab` - Single reusable card visual template
- `HandManager.cs` - Instantiates card prefabs for player hands
- `PileManager.cs` - Instantiates card prefabs for draw/discard piles

---

## How They Work Together

### Data Flow
1. **Design Time**: Artists create card sprites, designers create CardData assets
2. **Runtime**: `HandManager` loads CardData from Resources
3. **Instantiation**: `CreateCardPrefab()` creates GameObject from prefab
4. **Display**: `CardController` component reads CardData and displays sprite
5. **Interaction**: Player clicks prefab instance, game logic uses CardData rules

### Code Example
```csharp
// HandManager.cs - Creating visual cards
GameObject CreateCardPrefab(CardData cardData, int index) {
    GameObject cardObj = Instantiate(cardPrefab, handContainer);  // Prefab → GameObject
    CardController controller = cardObj.GetComponent<CardController>();
    controller.Initialize(cardData, index);  // ScriptableObject → Component data
    return cardObj;
}

// CardData.cs - Game logic in ScriptableObject
public bool CanPlayOn(CardData topCard, CardColor currentColor) {
    if (IsWildCard) return true;  // Pure data logic
    if (color == currentColor) return true;
    // ... more rules
}
```

---

## Network/Photon Integration

### Does NOT affect networking:
- **Prefab instantiation** - Visual only, not networked
- **ScriptableObject references** - Data stays local to each client

### What IS networked:
- **Card identifiers** - String IDs referencing which CardData to use
- **Game actions** - Play/draw commands that reference cards by ID
- **Game state** - Turn flow, colors, counts (not the visual cards themselves)

### Network Flow Example
```csharp
// NetworkGameManager.cs - Only IDs are sent over network
public void SendCardPlay(string cardId) {
    var moveData = new Hashtable {
        {"actionType", "PLAY_CARD"},
        {"cardIdentifier", cardId}  // Only the ID, not the entire CardData
    };
    // Each client uses this ID to find their local CardData ScriptableObject
}
```

**Key Point**: The network sends card IDs, each client looks up their local CardData ScriptableObject to get the actual card properties and creates local prefab instances for display.

---

## Why This Architecture in TAKI?

### Design Benefits
- **Artist Independence**: Artists can modify card sprites without touching code
- **Designer Control**: Game designers can create new cards without programming
- **Memory Efficiency**: 100+ cards share one prefab template
- **Network Optimization**: Only send small IDs instead of full card data
- **Maintainability**: Card rules centralized in one place

### Specific TAKI Advantages

**Multi-instance Cards**: Same card data (Blue 5) appears in multiple hands
```csharp
// Multiple players can have "Blue_5_01" - same data, different visual instances
CardData blueCard = Resources.Load<CardData>("Cards/Blue_5_01");
```

**Privacy Mode**: Same CardData, different visual display
```csharp
// HandManager.cs - Same data, different presentation
GameObject CreateCardPrefabEnhanced(CardData cardData, int index, bool privacyMode) {
    // privacyMode = true shows card back, false shows actual card
    // Same ScriptableObject, different prefab display
}
```

**Game Rules Consistency**: All clients use identical card rules
```csharp
// CardData.cs - Rules are identical across all clients
public bool CanPlayOn(CardData topCard, CardColor currentColor) {
    // Same logic on master and client ensures consistency
}
```

---

## Summary

In our TAKI game:
- **ScriptableObjects** = The "what" (card properties, rules, sprites)
- **Prefabs** = The "how" (visual display, user interaction)
- **Network** = The "sync" (communicates which cards, not their data)

This separation allows each system to focus on its responsibility while maintaining clean interfaces between data, presentation, and networking.