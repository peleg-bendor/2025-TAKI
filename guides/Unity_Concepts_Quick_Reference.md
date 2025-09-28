# Unity Concepts - Quick Reference Guide

## Prefabs & ScriptableObjects

**Prefabs** = Visual templates (GameObjects you can copy)
- **Use for**: Cards, UI elements, anything you need multiple copies of
- **TAKI Example**: CardPrefab.prefab → creates visual cards in hands

**ScriptableObjects** = Data containers (pure information storage)
- **Use for**: Card stats, game rules, settings
- **TAKI Example**: Blue_5_01.asset → stores card properties & game rules

**Pattern**: ScriptableObject (data) + Prefab (visual) = Complete system

---

## Start() vs Awake()

**Awake()** = "Set up yourself" (runs immediately when object created)
- **Use for**: Singletons, component caching, internal setup
- **TAKI Example**: GameManager singleton setup

**Start()** = "Connect with others" (runs after all objects ready)
- **Use for**: Finding other objects, UI connections, cross-references
- **TAKI Example**: HandManager finding GameManager

**Rule**: Awake() first (all objects), then Start() (all objects)

---

## Events

**Events** = "Something happened" notifications (one-to-many communication)
- **Publisher**: Fires event when something happens
- **Subscribers**: Listen and react to events
- **TAKI Example**: GameManager.OnCardPlayed → UI updates, sound plays, network syncs

**Pattern**: Loose coupling - systems don't need direct references

---

## Singleton vs Instance

**Singleton** = Only one exists globally
- **Use for**: Managers, services (GameManager, AudioManager)
- **Access**: `GameManager.Instance.DoSomething()`
- **TAKI Example**: One GameManager for entire game

**Instance** = Multiple copies with independent data
- **Use for**: Visual elements, data objects (cards, UI panels)
- **Access**: Direct reference to specific instance
- **TAKI Example**: Many CardData objects with different properties

---

## Photon Hashtables

**Hashtable** = Network-safe key-value data container
- **Problem**: Can't send custom classes over network
- **Solution**: Use `ExitGames.Client.Photon.Hashtable`
- **TAKI Example**: Send `{"actionType", "PLAY_CARD"}` instead of custom NetworkMoveData class

**Pattern**: Convert game data → Hashtable → Network → Hashtable → Game data

---

## Quick Decision Guide

**Need multiple visual copies?** → Use Prefab
**Need to store game data?** → Use ScriptableObject
**Need to notify multiple systems?** → Use Events
**Need global access to one object?** → Use Singleton
**Need multiple independent objects?** → Use Instances
**Need to send data over network?** → Use Photon Hashtable

---

## TAKI Architecture Summary

```
CardData (ScriptableObject) ←→ CardPrefab (Prefab)
      ↓                              ↓
Game Rules & Stats              Visual Display
      ↓                              ↓
GameManager (Singleton) ←→ UI Managers (Instances)
      ↓                              ↓
Events fired                    Events received
      ↓                              ↓
Hashtable created               Network synced
```

**Result**: Clean, scalable, networked card game architecture!