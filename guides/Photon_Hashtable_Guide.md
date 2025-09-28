# Photon Hashtable Guide - TAKI Project

## What is a Photon Hashtable?

**Short**: A Photon Hashtable is a key-value data structure used for sending custom data over the network in Photon multiplayer games.

**Details**: `ExitGames.Client.Photon.Hashtable` is Photon's implementation of a dictionary/map that can be serialized and sent across the network. It's designed specifically for multiplayer communication, supporting automatic serialization of common data types (strings, integers, floats, booleans, arrays). Unlike regular C# dictionaries, Photon Hashtables can be transmitted between clients seamlessly and are the standard way to send custom structured data in Photon games.

---

## Why Photon Hashtables Exist

### The Network Serialization Problem

**The Challenge**: In multiplayer games, you need to send complex data between clients, but the network can only send bytes.

**Traditional Approach** (doesn't work with Photon):
```csharp
// This WON'T work - custom classes can't be automatically serialized
public class NetworkMoveData {
    public string actionType;
    public string cardIdentifier;
    public int playerNumber;
}

// Photon can't automatically serialize this over network
SendMove(new NetworkMoveData { actionType = "PLAY_CARD", cardIdentifier = "Blue_5_01" });
// Result: Exception: Write failed. Custom type not found
```

**Photon Solution** (works perfectly):
```csharp
// This WORKS - Hashtable is built for network serialization
var moveData = new ExitGames.Client.Photon.Hashtable {
    {"actionType", "PLAY_CARD"},
    {"cardIdentifier", "Blue_5_01"},
    {"playerNumber", 1}
};

// Photon automatically handles serialization
SendMove(moveData);
```

---

## Hashtable Basics

### Declaration and Usage
```csharp
using ExitGames.Client.Photon; // Required import

// Creating a Hashtable
Hashtable data = new Hashtable();

// Adding key-value pairs
data["playerName"] = "Player1";
data["score"] = 100;
data["isReady"] = true;
data["cards"] = new string[] {"Blue_5", "Red_7"};

// Alternative creation syntax
var data = new Hashtable {
    {"playerName", "Player1"},
    {"score", 100},
    {"isReady", true}
};
```

### Reading from Hashtables
```csharp
// Reading values (with casting)
string playerName = (string)data["playerName"];
int score = (int)data["score"];
bool isReady = (bool)data["isReady"];

// Safe reading with default values
string name = data.ContainsKey("playerName") ? (string)data["playerName"] : "Unknown";

// Using TryGetValue pattern (safer)
if (data.ContainsKey("score")) {
    int playerScore = (int)data["score"];
    Debug.Log($"Player score: {playerScore}");
}
```

---

## TAKI Project Examples

### Before: Custom Class (Broken)
**The Problem**: Our initial attempt used a custom class that Photon couldn't serialize.

```csharp
// NetworkGameManager.cs - ORIGINAL BROKEN CODE
public class NetworkMoveData {
    public string actionType;
    public string cardIdentifier;
    public int cardsToDraw;
    public CardColor selectedColor;
}

public void SendCardPlay(string cardId) {
    var moveData = new NetworkMoveData {
        actionType = "PLAY_CARD",
        cardIdentifier = cardId
    };

    // This caused: Exception: Write failed. Custom type not found
    photonView.RPC("ReceivePlayerMove", RpcTarget.Others, moveData);
}
```

### After: Hashtable (Working)
**The Solution**: We converted to Hashtables for proper network serialization.

```csharp
// NetworkGameManager.cs - FIXED WITH HASHTABLES
public void SendCardPlay(string cardId) {
    var moveData = new ExitGames.Client.Photon.Hashtable {
        {"actionType", "PLAY_CARD"},
        {"cardIdentifier", cardId}
    };

    // This works perfectly - automatic serialization
    photonView.RPC("ReceivePlayerMove", RpcTarget.Others, moveData);
}

[PunRPC]
void ReceivePlayerMove(ExitGames.Client.Photon.Hashtable moveData) {
    string actionType = (string)moveData["actionType"];
    string cardId = (string)moveData["cardIdentifier"];

    ProcessRemoteAction(actionType, cardId);
}
```

---

## TAKI Network Messages with Hashtables

### 1. Card Play Messages
```csharp
// SendCardPlay()
var moveData = new Hashtable {
    {"actionType", "PLAY_CARD"},
    {"cardIdentifier", cardId}
};

// SendCardDraw()
var moveData = new Hashtable {
    {"actionType", "DRAW_CARD"},
    {"drawCount", 1}
};

// SendEndTurn()
var moveData = new Hashtable {
    {"actionType", "END_TURN"},
    {"playerId", PhotonNetwork.LocalPlayer.ActorNumber}
};
```

### 2. Special Card Messages
```csharp
// SendColorSelection() - ChangeColor cards
var moveData = new Hashtable {
    {"actionType", "COLOR_SELECTION"},
    {"selectedColor", selectedColor.ToString()},
    {"playerId", PhotonNetwork.LocalPlayer.ActorNumber}
};

// Chain break messages
var moveData = new Hashtable {
    {"actionType", "CHAIN_BREAK"},
    {"cardsToDraw", totalCardsToDraw},
    {"breakingPlayer", PhotonNetwork.LocalPlayer.ActorNumber}
};
```

### 3. Processing Received Data
```csharp
[PunRPC]
void ReceivePlayerMove(ExitGames.Client.Photon.Hashtable moveData) {
    string actionType = (string)moveData["actionType"];

    switch (actionType) {
        case "PLAY_CARD":
            string cardId = (string)moveData["cardIdentifier"];
            ProcessNetworkCardPlay(cardId);
            break;

        case "DRAW_CARD":
            int drawCount = (int)moveData["drawCount"];
            ProcessNetworkCardDraw(drawCount);
            break;

        case "COLOR_SELECTION":
            string colorStr = (string)moveData["selectedColor"];
            CardColor selectedColor = (CardColor)System.Enum.Parse(typeof(CardColor), colorStr);
            ProcessNetworkColorSelection(selectedColor);
            break;
    }
}
```

---

## Supported Data Types

### ✅ Natively Supported (No conversion needed)
```csharp
var data = new Hashtable {
    {"stringValue", "Hello World"},           // string
    {"intValue", 42},                        // int
    {"floatValue", 3.14f},                   // float
    {"boolValue", true},                     // bool
    {"byteValue", (byte)255},               // byte
    {"arrayValue", new int[] {1, 2, 3}}     // arrays of supported types
};
```

### ⚠️ Requires Conversion
```csharp
// Enums - convert to string
CardColor color = CardColor.Red;
data["color"] = color.ToString();  // Send as string
CardColor receivedColor = (CardColor)System.Enum.Parse(typeof(CardColor), (string)data["color"]); // Convert back

// Complex objects - break into simple parts
Vector3 position = new Vector3(1, 2, 3);
data["posX"] = position.x;
data["posY"] = position.y;
data["posZ"] = position.z;

// Lists - convert to arrays
List<string> cardIds = new List<string> {"card1", "card2"};
data["cards"] = cardIds.ToArray();  // Send as array
List<string> receivedCards = ((string[])data["cards"]).ToList(); // Convert back
```

### ❌ Not Supported (Don't use)
```csharp
// Custom classes/structs
public class PlayerData { } // Can't serialize directly
data["player"] = new PlayerData(); // Will fail

// Unity objects
data["gameObject"] = someGameObject; // Will fail
data["transform"] = transform; // Will fail
data["cardData"] = someCardData; // Will fail - use card ID string instead
```

---

## Best Practices from TAKI Project

### ✅ Good Practices

#### 1. Use Consistent Key Names
```csharp
// Consistent across all network messages
public static class NetworkKeys {
    public const string ACTION_TYPE = "actionType";
    public const string CARD_IDENTIFIER = "cardIdentifier";
    public const string PLAYER_ID = "playerId";
    public const string SELECTED_COLOR = "selectedColor";
}

var moveData = new Hashtable {
    {NetworkKeys.ACTION_TYPE, "PLAY_CARD"},
    {NetworkKeys.CARD_IDENTIFIER, cardId}
};
```

#### 2. Safe Data Retrieval
```csharp
// Always check if key exists before reading
string GetStringValue(Hashtable data, string key, string defaultValue = "") {
    return data.ContainsKey(key) ? (string)data[key] : defaultValue;
}

int GetIntValue(Hashtable data, string key, int defaultValue = 0) {
    return data.ContainsKey(key) ? (int)data[key] : defaultValue;
}

// Usage
string actionType = GetStringValue(moveData, "actionType", "UNKNOWN");
```

#### 3. Validate Received Data
```csharp
[PunRPC]
void ReceivePlayerMove(Hashtable moveData) {
    // Validate essential data exists
    if (!moveData.ContainsKey("actionType")) {
        TakiLogger.LogError("Received network move without actionType");
        return;
    }

    string actionType = (string)moveData["actionType"];

    // Validate action type
    if (string.IsNullOrEmpty(actionType)) {
        TakiLogger.LogError("Received empty actionType");
        return;
    }

    // Process the valid data
    ProcessAction(actionType, moveData);
}
```

### ❌ Bad Practices

#### 1. Direct Casting Without Checks
```csharp
// DANGEROUS - will crash if key doesn't exist or wrong type
string actionType = (string)moveData["actionType"]; // Could crash!

// BETTER - check first
string actionType = moveData.ContainsKey("actionType") ? (string)moveData["actionType"] : "";
```

#### 2. Magic Strings Everywhere
```csharp
// BAD - hard to maintain, typo-prone
var data1 = new Hashtable { {"action_type", "PLAY_CARD"} };
var data2 = new Hashtable { {"actionType", "DRAW_CARD"} };  // Inconsistent!
var data3 = new Hashtable { {"ActionType", "END_TURN"} };   // Different casing!

// GOOD - use constants
const string ACTION_TYPE = "actionType";
var data = new Hashtable { {ACTION_TYPE, "PLAY_CARD"} };
```

#### 3. Sending Too Much Data
```csharp
// BAD - sending entire game state every time
var moveData = new Hashtable {
    {"actionType", "PLAY_CARD"},
    {"cardId", cardId},
    {"playerHand", playerHand.ToArray()}, // Unnecessary!
    {"gameState", currentGameState},      // Too much!
    {"allCards", allCardsInGame}          // Overkill!
};

// GOOD - only send what's needed
var moveData = new Hashtable {
    {"actionType", "PLAY_CARD"},
    {"cardIdentifier", cardId}  // Just the essential data
};
```

---

## Alternative Uses in Photon

### 1. Room Properties
```csharp
// MultiplayerMenuLogic.cs - Setting room properties
ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable {
    {"gameMode", "TAKI"},
    {"maxPlayers", 2},
    {"isPrivate", false}
};

PhotonNetwork.CreateRoom(roomName, new RoomOptions {
    CustomRoomProperties = roomProperties,
    MaxPlayers = 2
});
```

### 2. Player Properties
```csharp
// Setting player-specific properties
ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable {
    {"playerName", playerName},
    {"isReady", true},
    {"wins", 0}
};

PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

// Reading other player's properties
string opponentName = (string)PhotonNetwork.PlayerListOthers[0].CustomProperties["playerName"];
```

---

## Debugging Hashtables

### Logging Hashtable Contents
```csharp
void LogHashtable(Hashtable data, string prefix = "") {
    Debug.Log($"{prefix} Hashtable contents:");
    foreach (var key in data.Keys) {
        Debug.Log($"  {key}: {data[key]} ({data[key]?.GetType()})");
    }
}

// Usage
LogHashtable(moveData, "Sending move:");
// Output:
// Sending move: Hashtable contents:
//   actionType: PLAY_CARD (System.String)
//   cardIdentifier: Blue_5_01 (System.String)
```

### Network Traffic Monitoring
```csharp
// NetworkGameManager.cs - Log all network messages
public void SendMove(Hashtable moveData, bool isFinishing) {
    TakiLogger.LogNetwork($"Sending network move: {moveData["actionType"]}");
    LogHashtable(moveData, "SEND");

    photonView.RPC("ReceivePlayerMove", RpcTarget.Others, moveData);
}

[PunRPC]
void ReceivePlayerMove(Hashtable moveData) {
    TakiLogger.LogNetwork($"Received network move: {moveData["actionType"]}");
    LogHashtable(moveData, "RECEIVE");

    ProcessRemoteAction(moveData);
}
```

---

## Performance Considerations

### ✅ Efficient
```csharp
// Small, focused messages
var moveData = new Hashtable {
    {"actionType", "PLAY_CARD"},
    {"cardId", "Blue_5_01"}
}; // ~50 bytes over network
```

### ❌ Inefficient
```csharp
// Large, wasteful messages
var moveData = new Hashtable {
    {"actionType", "PLAY_CARD"},
    {"cardId", "Blue_5_01"},
    {"entirePlayerHand", playerHand.ToArray()},    // 100s of bytes
    {"gameHistory", allPreviousMoves.ToArray()},   // 1000s of bytes
    {"debugInfo", "lots of unnecessary text"}       // Waste
}; // Could be 5000+ bytes over network
```

---

## Summary

**Photon Hashtables are the bridge between your game data and network communication.**

### Key Points:
- **Purpose**: Send structured data over Photon network
- **Automatic Serialization**: No manual conversion needed
- **Type Safety**: Cast values when reading
- **TAKI Usage**: All network game actions (play, draw, color selection)

### TAKI Project Impact:
- **Fixed Network Serialization**: Replaced broken custom classes
- **Clean Message Protocol**: Consistent key names and structure
- **Reliable Communication**: Card plays, draws, special effects all work
- **Easy Debugging**: Can log and inspect all network messages

**Remember**: Hashtables are your tool for sending "what happened" between clients - keep them small, focused, and well-structured!