# Start() vs Awake() in Unity - TAKI Project Guide

## What is Awake()?

**Short**: `Awake()` is called immediately when a GameObject is created, before any other initialization.

**Details**: `Awake()` runs as soon as the GameObject becomes active, even before `Start()`. It's called only once per object lifetime and runs regardless of whether the GameObject is enabled or disabled. Use `Awake()` for internal initialization that doesn't depend on other objects - setting up references, initializing variables, and configuring the object itself.

**When to use**: Internal setup, component references, singleton patterns, initialization that other objects might depend on.

---

## What is Start()?

**Short**: `Start()` is called once before the first frame update, after all objects have been initialized.

**Details**: `Start()` runs after `Awake()` has been called on all objects in the scene. This means you can safely reference other objects and their components. It only runs if the GameObject is active and enabled. Use `Start()` for initialization that depends on other objects being ready.

**When to use**: Cross-object communication, finding other objects, initialization that requires the full scene to be loaded.

---

## Key Differences

| Aspect | Awake() | Start() |
|--------|---------|---------|
| **When Called** | Immediately on creation | Before first frame, after all Awake() |
| **Object State** | Can run on inactive GameObjects | Only runs on active GameObjects |
| **Scene State** | Other objects may not be ready | All objects have been initialized |
| **Purpose** | Internal object setup | Cross-object initialization |
| **Frequency** | Once per object lifetime | Once per activation cycle |
| **Safe Operations** | Self-setup, component caching | Finding other objects, cross-references |

---

## Execution Order

```
GameObject Created
        ↓
    Awake() ← All objects get Awake() first
        ↓
   OnEnable() ← If GameObject is active
        ↓
    Start() ← All objects get Start() second
        ↓
   Update() ← Frame loop begins
```

---

## TAKI Project Examples

### Awake() Examples

**GameManager.cs**
```csharp
void Awake() {
    // Self-initialization - doesn't depend on other objects
    if (instance != null && instance != this) {
        Destroy(gameObject);
        return;
    }
    instance = this;
    DontDestroyOnLoad(gameObject);

    // Initialize collections
    playerHand = new List<CardData>();
    computerHand = new List<CardData>();
}
```
*Why Awake(): Singleton pattern needs to run immediately, before other objects try to access the instance.*

**BaseGameplayUIManager.cs**
```csharp
protected virtual void Awake() {
    // Internal setup - caching own components
    if (gameManager == null) {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Self-configuration
    this.enabled = false; // Will be enabled by GameManager later
}
```
*Why Awake(): Setting up internal references and default state before other systems try to use this UI manager.*

### Start() Examples

**BaseGameplayUIManager.cs**
```csharp
protected virtual void Start() {
    // This was causing issues! Start() was overriding GameManager's explicit activation
    if (!explicitlyActivatedByGameManager) {
        this.enabled = false;
        TakiLogger.LogInfo($"{GetType().Name}: Disabled in Start() - not explicitly activated");
        return;
    }

    // Safe to access other objects now
    InitializeUI();
    ConnectEventHandlers();
}
```
*Why Start(): Checking activation flags and connecting to other objects after the scene is fully loaded.*

**HandManager.cs**
```csharp
void Start() {
    // Finding and connecting to other scene objects
    if (gameManager == null) {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Initialize display after all objects are ready
    UpdateHandDisplay();
}
```
*Why Start(): HandManager needs to find GameManager and other objects that might not be ready during Awake().*

---

## Common Patterns in TAKI Project

### 1. Singleton Pattern (Awake)
```csharp
// GameManager.cs
void Awake() {
    if (instance != null && instance != this) {
        Destroy(gameObject);
        return;
    }
    instance = this;
    // Must happen in Awake() so other objects can access instance immediately
}
```

### 2. Component Caching (Awake)
```csharp
// Various scripts
void Awake() {
    // Cache components on same GameObject
    myRenderer = GetComponent<Renderer>();
    myCollider = GetComponent<Collider>();
    // Safe because these are on the same object
}
```

### 3. Scene Object Finding (Start)
```csharp
// HandManager.cs, PileManager.cs
void Start() {
    // Find other objects in scene
    gameManager = FindObjectOfType<GameManager>();
    deckManager = FindObjectOfType<DeckManager>();
    // Safe because all objects have been created by now
}
```

### 4. UI Initialization (Start)
```csharp
// UI Manager classes
void Start() {
    // Connect to other UI elements
    ConnectButtonEvents();
    UpdateDisplayFromGameState();
    // Safe because all UI objects are ready
}
```

---

## TAKI-Specific Issues We Solved

### Issue 1: UI Manager Activation Timing
**Problem**: `Start()` was disabling UI managers that GameManager had explicitly enabled.

```csharp
// PROBLEMATIC CODE in BaseGameplayUIManager.cs
void Start() {
    this.enabled = false; // This was overriding GameManager's activation!
}
```

**Solution**: Added activation tracking
```csharp
void Start() {
    if (!explicitlyActivatedByGameManager) {
        this.enabled = false; // Only disable if not explicitly activated
        return;
    }
    // Continue with initialization...
}
```

### Issue 2: Reference Dependencies
**Problem**: HandManager trying to access GameManager before it was ready.

**Solution**: Used proper Awake/Start separation
```csharp
// HandManager.cs
void Awake() {
    // Internal setup only
    cardInstances = new List<GameObject>();
}

void Start() {
    // External dependencies
    if (gameManager == null) {
        gameManager = FindObjectOfType<GameManager>();
    }
    UpdateHandDisplay();
}
```

---

## Best Practices from TAKI Project

### Use Awake() for:
- **Singleton setup** (`GameManager.cs`)
- **Component caching** (getting components on same GameObject)
- **Collection initialization** (List, Dictionary creation)
- **Default values** (setting initial state)
- **DontDestroyOnLoad** (persistence setup)

### Use Start() for:
- **Finding other GameObjects** (`FindObjectOfType`)
- **Cross-object communication** (connecting event handlers)
- **UI initialization** (after all UI elements are ready)
- **Scene-dependent setup** (things that need full scene loaded)

### Anti-patterns we avoid:
```csharp
// DON'T DO THIS
void Awake() {
    otherManager = FindObjectOfType<OtherManager>(); // Might not exist yet!
}

// DON'T DO THIS
void Start() {
    this.enabled = true; // Might override GameManager's explicit control
}
```

---

## Network Considerations

In multiplayer TAKI, timing is crucial:

**Awake()** - Safe for local setup
```csharp
void Awake() {
    localPlayerCards = new List<CardData>(); // Local initialization
}
```

**Start()** - Good for network setup
```csharp
void Start() {
    if (PhotonNetwork.IsConnected) {
        SetupNetworkHandlers(); // Network is ready
    }
}
```

**Note**: Network objects might not be fully synchronized until after Start(), so be careful with network-dependent initialization.

---

## Summary

In TAKI project:
- **Awake()** = "Set up yourself" (internal initialization, singletons, component caching)
- **Start()** = "Connect with others" (find objects, UI setup, cross-references)
- **Timing matters** = UI manager activation issues taught us to respect the execution order
- **Network awareness** = Multiplayer adds another layer of timing considerations

The key is understanding that Awake() prepares individual objects, while Start() connects the scene together - and our TAKI project demonstrates both the power and pitfalls of getting this timing right!