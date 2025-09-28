# TAKI Multiplayer Network Architecture Documentation

## Overview

This document provides a comprehensive guide to the TAKI card game's multiplayer network implementation using Photon PUN2 (Photon Unity Networking 2). The architecture supports 1v1 multiplayer TAKI gameplay with real-time synchronization between two players.

---

## Architecture Components

### 1. MultiplayerMenuLogic.cs
**Primary Role**: Photon connection management and matchmaking

**Key Responsibilities**:
- Manages the complete Photon connection lifecycle
- Handles room creation and joining with password protection
- Coordinates game start when exactly 2 players are present
- Manages reconnection and re-entry scenarios

**Location in Project**: `Assets/Scripts/Multiplayer/MultiplayerMenuLogic.cs`

### 2. NetworkGameManager.cs
**Primary Role**: In-game networking and turn management

**Key Responsibilities**:
- Manages turn-based gameplay using PunTurnManager
- Handles deck synchronization between master and clients
- Processes all network game actions (card play, draw, special effects)
- Coordinates hand assignment and visual updates

**Location in Project**: `Assets/Scripts/Multiplayer/NetworkGameManager.cs`

### 3. NetworkCleanupManager.cs
**Primary Role**: Session cleanup and state reset

**Key Responsibilities**:
- Handles proper room leaving when returning to menu
- Resets network state for fresh multiplayer sessions
- Prevents players from being stuck in old rooms

**Location in Project**: `Assets/Scripts/Multiplayer/NetworkCleanupManager.cs`

---

## Room Management System

### Room Configuration
The TAKI multiplayer system uses the following room settings:

```csharp
// Room Properties
var roomProperties = new ExitGames.Client.Photon.Hashtable {
    {"sv", 100},           // Search Value: Used for matchmaking filtering
    {"pwd", "taki2025"}    // Password: Basic room security
};

// Room Options
var roomOptions = new RoomOptions {
    MaxPlayers = 2,                                        // Exactly 2 players for TAKI
    IsVisible = true,                                      // Room appears in lobby
    IsOpen = true,                                         // New players can join
    CustomRoomProperties = roomProperties,                 // Attach custom properties
    CustomRoomPropertiesForLobby = new[] { "sv", "pwd" }  // Make properties visible for matchmaking
};
```

### Room Lifecycle

#### 1. Room Creation Process
```
Player clicks "Play Multiplayer"
    ↓
Connect to Photon servers
    ↓
Join lobby and search for existing TAKI rooms
    ↓ (No matching rooms found)
Create new room with custom properties
    ↓
Wait for second player to join
    ↓
Start game when room reaches 2 players
```

#### 2. Room Joining Process
```
Player clicks "Play Multiplayer"
    ↓
Connect to Photon servers
    ↓
Join lobby and search for existing TAKI rooms
    ↓ (Matching room found)
Validate room password
    ↓
Join existing room
    ↓
Start game when room reaches 2 players
```

#### 3. Room Properties Details

| Property | Value | Purpose |
|----------|--------|---------|
| **Search Value ("sv")** | `100` | Allows players with same search value to find each other during matchmaking |
| **Password ("pwd")** | `"taki2025"` | Prevents random players from joining TAKI games |
| **MaxPlayers** | `2` | Enforces 1v1 gameplay (exactly 2 players required) |
| **IsVisible** | `true` → `false` | Room visible during matchmaking, hidden during gameplay |
| **IsOpen** | `true` → `false` | Players can join during matchmaking, blocked during gameplay |

### Room State Management

**During Matchmaking**:
- `IsVisible = true` - Room appears in lobby listings
- `IsOpen = true` - New players can join the room

**During Gameplay** (Master client locks room):
- `IsVisible = false` - Room hidden from lobby to prevent interruptions
- `IsOpen = false` - No new players can join mid-game

---

## Network Message Flow

### Message Types and Purposes

| Message Type | Purpose | Finishing Move | Data Payload |
|--------------|---------|----------------|--------------|
| `PLAY_CARD` | Player plays a card | No | Card identifier string |
| `DRAW_CARD` | Player draws from deck | No | Empty string |
| `COLOR_SELECTION` | Player selects new color (ChangeColor card) | No | Color enum as string |
| `END_TURN` | Player ends their turn | **Yes** | Empty string |
| `END_TAKI_SEQUENCE` | Player ends TAKI sequence | No | Empty string |
| `STOP_EFFECT` | STOP card effect notification | No | Empty string |
| `DIRECTION_CHANGE` | ChangeDirection card effect | No | Empty string |
| `CHAIN_BREAK` | Player breaks PlusTwo chain | No | Number of cards drawn |
| `PLUS_TWO_EFFECT` | PlusTwo card chain effect | No | Chain count and draw count |

### Turn Flow Control

**Critical Design**: The system uses **strict turn flow** where players must explicitly press "END TURN" to advance turns:

```
Player Action (PLAY_CARD/DRAW_CARD) → SendMove(data, false)
    ↓
OnPlayerMove() called on all clients
    ↓
Process action locally, but turn does NOT advance
    ↓
Player presses "END TURN" → SendMove(END_TURN, true)
    ↓
OnPlayerFinished() called on all clients
    ↓
Master client advances to next turn
```

This design is essential for special cards that require additional actions after playing.

---

## Deck Synchronization System

### Master/Client Coordination

The deck synchronization ensures both players have identical game state:

#### Master Client Process:
1. **Initialize Deck**: Create full deck with shuffle
2. **Deal Cards**: Distribute 8 cards to each player
3. **Set Starting Card**: Place first card on discard pile
4. **Serialize State**: Convert all cards to network-safe identifiers
5. **Broadcast State**: Send complete game state to client via RPC

#### Client Process:
1. **Wait for State**: Set `_waitingForDeckState = true`
2. **Receive RPC**: Get serialized game state from master
3. **Deserialize Cards**: Convert identifiers back to CardData objects
4. **Apply State**: Set up local deck, hands, and discard pile
5. **Sync Counts**: Match draw pile count to master's state

### RPC Message Structure

```csharp
// Master sends this RPC to client
[PunRPC]
void ReceiveInitialGameState(
    string startingCardId,           // Starting discard card
    int drawCount,                   // Remaining deck size after dealing
    string serializedPlayer1Hand,   // Player 1's cards (serialized)
    string serializedPlayer2Hand,   // Player 2's cards (serialized)
    int masterActor                  // Master's actor number for reference
)
```

### Hand Assignment Logic

Players are assigned hands based on their Photon Actor Numbers:

```csharp
// Sort all players by actor number for consistent assignment
List<Player> sortedPlayers = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToList();

// First player (lowest actor number) = Player 1
// Second player (highest actor number) = Player 2
bool isPlayer1 = (PhotonNetwork.LocalPlayer.ActorNumber == sortedPlayers[0].ActorNumber);

List<CardData> myHand = isPlayer1 ? player1Hand : player2Hand;
List<CardData> opponentHand = isPlayer1 ? player2Hand : player1Hand;
```

---

## Connection State Management

### Connection Lifecycle

```
Application Start
    ↓
MultiplayerMenuLogic.InitStart()
    ↓
PhotonNetwork.ConnectUsingSettings()
    ↓
OnConnectedToMaster() - Enable "Play Multiplayer" button
    ↓
User clicks "Play Multiplayer"
    ↓
PhotonNetwork.JoinLobby()
    ↓
OnJoinedLobby() - Search for existing rooms
    ↓
PhotonNetwork.JoinRandomRoom() OR CreateRoom()
    ↓
OnJoinedRoom() - Wait for 2 players
    ↓
OnPlayerEnteredRoom() - Start game when full
    ↓
OnMultiplayerGameReady event → GameManager switches mode
    ↓
NetworkGameManager takes control
```

### Re-entry Scenarios

The system handles several re-entry scenarios:

#### Scenario 1: Fresh Connection
- Player not connected to Photon
- **Flow**: Connect → Join Lobby → Search/Create Room

#### Scenario 2: Already in Room (from previous game)
- Player still in room from previous multiplayer session
- **Flow**: Leave Current Room → Join Lobby → Search/Create Room

#### Scenario 3: Connected but Not in Room
- Player connected to Photon but not in any room
- **Flow**: Join Lobby → Search/Create Room

### Cleanup Process

When players return to menu from multiplayer:

```
GameEndManager detects menu return
    ↓
NetworkCleanupManager.OnGoingHome()
    ↓
PhotonNetwork.LeaveRoom()
    ↓
OnLeftRoom() callback
    ↓
Reset MultiplayerMenuLogic state
    ↓
Ready for fresh multiplayer session
```

---

## Turn Management System

### PunTurnManager Integration

The system uses Photon's PunTurnManager for turn coordination:

#### Turn Calculation
```csharp
// Determine whose turn it is based on turn number
int GetExpectedActorForTurn(int turn) {
    var playerActors = PhotonNetwork.PlayerList.Select(p => p.ActorNumber).OrderBy(x => x).ToList();
    int playerIndex = (turn - 1) % playerActors.Count;
    return playerActors[playerIndex];
}
```

#### Turn State Synchronization
```csharp
public void OnTurnBegins(int turn) {
    // Calculate if it's this client's turn
    int expectedActor = GetExpectedActorForTurn(turn);
    _isMyTurn = PhotonNetwork.LocalPlayer.ActorNumber == expectedActor;

    // Update GameManager's turn state
    TurnState newTurnState = _isMyTurn ? TurnState.PlayerTurn : TurnState.ComputerTurn;
    gameManager.gameState.ChangeTurnState(newTurnState);
}
```

### Turn Advancement Rules

| Action Type | Advances Turn | Notes |
|-------------|---------------|--------|
| Play Card | No | Player must press END TURN |
| Draw Card | No | Player must press END TURN |
| Color Selection | No | Part of ChangeColor card flow |
| Special Effects | No | STOP, ChangeDirection, PlusTwo, etc. |
| **END TURN** | **Yes** | Only action that advances turn |

---

## Error Handling and Edge Cases

### Common Issues and Solutions

#### 1. Reference Equality Bug
**Problem**: `gameManager.playerHand` and network hand data pointing to same list
**Solution**: Create defensive copies before modifying lists

```csharp
// FIXED: Create defensive copy to prevent reference equality bugs
List<CardData> myHandCopy = new List<CardData>(myHand);
gameManager.playerHand.Clear();
gameManager.playerHand.AddRange(myHandCopy);
```

#### 2. Deck Count Desynchronization
**Problem**: Draw pile counts differ between clients
**Solution**: Explicit count synchronization

```csharp
// Sync draw pile count to exactly match master's state
if (currentDrawCount != drawCount) {
    gameManager.deckManager.SyncDrawPileCount(drawCount);
}
```

#### 3. Network Serialization Issues
**Problem**: Custom objects not serializable over Photon network
**Solution**: Use Photon's built-in Hashtable

```csharp
// Use Hashtable instead of custom NetworkMoveData class
var moveData = new ExitGames.Client.Photon.Hashtable {
    {"actionType", "PLAY_CARD"},
    {"cardIdentifier", cardId}
};
```

### Connection Recovery

The system handles various connection issues:

- **Connection Loss**: OnDisconnected() callback triggers cleanup
- **Room Leaving Failure**: OnDisconnected() provides fallback cleanup
- **Duplicate Cleanup**: `isCleaningUp` flag prevents concurrent operations
- **Stale Room State**: Re-entry logic forces room leaving before fresh matchmaking

---

## Security Considerations

### Room Protection
- **Password System**: Basic protection using "pwd" custom property
- **Search Value Filtering**: Only matching search values can find rooms
- **Room Locking**: Rooms hidden and closed during gameplay

### Data Validation
- **Card Identifier Validation**: Cards validated against master card database
- **Turn Validation**: Actions only processed during appropriate turns
- **Actor Number Verification**: Network messages validated by sender's actor number

### Anti-Cheat Measures
- **Master Authority**: Master client controls deck state and card dealing
- **State Synchronization**: All clients maintain identical game state
- **Action Validation**: Invalid actions rejected by game logic

---

## Performance Considerations

### Network Optimization
- **Efficient Serialization**: Cards serialized as compact identifier strings
- **Minimal RPCs**: Only essential state changes sent over network
- **Lazy Updates**: UI updates only when necessary

### Memory Management
- **Object Reuse**: Hand managers reuse card display objects
- **Defensive Copying**: Only when necessary to prevent bugs
- **State Cleanup**: Proper cleanup when returning to menu

---

## Integration Points

### GameManager Integration
- **Mode Switching**: Seamless transition between singleplayer and multiplayer
- **UI Management**: Proper UI manager activation for multiplayer context
- **State Management**: Network-aware game state handling

### Hand Manager Integration
- **Network Mode**: Special network mode for opponent hand privacy
- **Card Display**: Real cards with privacy mode for opponent hands
- **Synchronization**: Hand counts synchronized across clients

### Logging Integration
- **Network Logging**: Comprehensive logging via TakiLogger.LogNetwork()
- **Debug Controls**: Toggle-able logging for development and debugging
- **State Tracking**: Detailed state logging for troubleshooting

---

## Development and Debugging

### Debug Features
- **Network State Inspection**: GetNetworkStateInfo() methods
- **Logging Controls**: enableNetworkLogs and enableCleanupLogs flags
- **State Validation**: Extensive diagnostic logging during operations

### Common Debug Commands
```csharp
// Check current network state
multiplayerMenuLogic.CheckNetworkState();

// Toggle network logging
networkGameManager.ToggleNetworkLogs();

// Test serialization process
networkGameManager.DebugSerialization();
```

### Troubleshooting Guide

**Issue**: Players can't find each other's rooms
- **Check**: Search value and password match
- **Check**: Both players connected to same Photon region

**Issue**: Deck synchronization fails
- **Check**: Master client properly sending RPC
- **Check**: Client properly waiting for deck state

**Issue**: Turns not advancing properly
- **Check**: Players pressing END TURN button
- **Check**: Turn manager not in finished state

**Issue**: Players stuck in old rooms
- **Check**: NetworkCleanupManager properly configured
- **Check**: OnLeftRoom callback being received

---

This documentation covers the complete TAKI multiplayer network architecture. The system provides robust, synchronized multiplayer gameplay with proper error handling, security measures, and performance optimization for real-time card game experiences.