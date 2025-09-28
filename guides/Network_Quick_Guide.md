# TAKI Multiplayer - Quick Reference Guide

## 🚀 How It Works
1. **Connect** → **Find Room** → **Play** → **Leave**
2. Uses Photon PUN2 for networking
3. Supports exactly 2 players (1v1 TAKI)

## 📁 Key Files
- **MultiplayerMenuLogic.cs** - Connection & matchmaking
- **NetworkGameManager.cs** - In-game networking & turns
- **NetworkCleanupManager.cs** - Session cleanup

## 🏠 Room Settings
```
Max Players: 2
Password: "taki2025"
Search Value: 100
```

## 🎮 Game Flow
```
Click "Play Multiplayer"
→ Connect to Photon
→ Search/Create room
→ Wait for 2nd player
→ Start game
```

## 🔄 Turn System
- Players take turns using **PunTurnManager**
- Must press **END TURN** to advance (strict flow)
- Master client controls turn advancement

## 🃏 Card Sync
- **Master**: Creates deck, deals cards, sends to client
- **Client**: Receives deck state, syncs locally
- Cards serialized as identifier strings

## 📤 Network Messages
| Action | Purpose |
|--------|---------|
| `PLAY_CARD` | Play a card |
| `DRAW_CARD` | Draw from deck |
| `END_TURN` | Finish turn (advances) |
| `COLOR_SELECTION` | Choose new color |
| Special effects (STOP, ChangeDirection, PlusTwo, etc.) |

## 🧹 Cleanup Process
When leaving multiplayer:
1. **NetworkCleanupManager** leaves room
2. Resets menu state
3. Ready for new session

## ⚡ Quick Debug
- Toggle network logs in Inspector
- Check connection status with context menu commands
- Use `GetNetworkStateInfo()` for current state

## 🔧 Common Issues
- **Can't find rooms**: Check password/search value
- **Stuck in old room**: Cleanup manager will handle it
- **Turn not advancing**: Must press END TURN button
- **Cards not syncing**: Check master/client deck initialization

## 🎯 Key Points
- Password protection prevents random joins
- Rooms locked during gameplay (invisible/closed)
- Master client has authority over game state
- Automatic cleanup when returning to menu
- Re-entry support for seamless experience