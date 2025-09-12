# Unity Inspector Setup Instructions
## New UI Architecture Implementation

### 🎯 **GOAL**: Fix "GameplayUIManager not found" errors by implementing screen-specific UI managers

---

## 📋 **STEP 1: Add New UI Manager Scripts**

### 1.1 Add SinglePlayerUIManager Component
1. **Select GameManager** GameObject in hierarchy
2. **Add Component** → Search "SinglePlayerUIManager" → Add
3. **Verify** the script appears in Inspector under GameManager

### 1.2 Add MultiPlayerUIManager Component  
1. **Select GameManager** GameObject in hierarchy
2. **Add Component** → Search "MultiPlayerUIManager" → Add
3. **Verify** both new UI managers appear in Inspector

---

## 📋 **STEP 2: Assign SinglePlayer UI Elements**

### Navigate to Screen_SinglePlayerGame UI Elements:
```
Canvas/Screen_SinglePlayerGame/
```

### 2.1 Turn Display
- **Turn Indicator Text**: `Screen_SinglePlayerGame/SideInfoPanel/TurnIndicatorText`
- **Current Color Indicator**: `Screen_SinglePlayerGame/CurrentColorIndicator` (Image component)

### 2.2 Player Action Buttons
- **Play Card Button**: `Screen_SinglePlayerGame/Player1Panel/Player1ActionPanel/Btn_Player1PlayCard`
- **Draw Card Button**: `Screen_SinglePlayerGame/Player1Panel/Player1ActionPanel/Btn_Player1DrawCard`
- **End Turn Button**: `Screen_SinglePlayerGame/Player1Panel/Player1ActionPanel/Btn_Player1EndTurn`
- **Pause Button**: `Screen_SinglePlayerGame/Btn_Pause`

### 2.3 Hand Size Displays
- **Player1 Hand Size Text**: `Screen_SinglePlayerGame/Player1Panel/Player1ActionPanel/Player1HandSizePanel/Player1HandSizeText`
- **Player2 Hand Size Text**: `Screen_SinglePlayerGame/Player2Panel/Player2ActionPanel/Player2HandSizePanel/Player2HandSizeText`

### 2.4 Player Messages
- **Player Message Text**: `Screen_SinglePlayerGame/MainGameInfoPanel/GameMessageText`
- **Computer Message Text**: `Screen_SinglePlayerGame/Player2Panel/Player2ActionPanel/Player2MessageText`

### 2.5 Color Selection
- **Color Selection Panel**: `Screen_SinglePlayerGame/ColorSelectionPanel` (GameObject)
- **Select Red Button**: `Screen_SinglePlayerGame/ColorSelectionPanel/Btn_SelectRed`
- **Select Blue Button**: `Screen_SinglePlayerGame/ColorSelectionPanel/Btn_SelectBlue`
- **Select Green Button**: `Screen_SinglePlayerGame/ColorSelectionPanel/Btn_SelectGreen`
- **Select Yellow Button**: `Screen_SinglePlayerGame/ColorSelectionPanel/Btn_SelectYellow`

### 2.6 Special Features
- **Chain Status Text**: `Screen_SinglePlayerGame/MainGameInfoPanel/chainStatusText`
- **End Taki Sequence Button**: `Screen_SinglePlayerGame/GameBoardPanel/Btn_Player1EndTakiSequence`
- **Taki Sequence Status Text**: `Screen_SinglePlayerGame/MainGameInfoPanel/takiSequenceStatusText`

### 2.7 Deck Pile Elements
- **Draw Pile Panel**: `Screen_SinglePlayerGame/GameBoardPanel/DrawPilePanel` (GameObject)
- **Draw Pile Count Text**: `Screen_SinglePlayerGame/GameBoardPanel/DrawPilePanel/DrawPileCountText`
- **Discard Pile Panel**: `Screen_SinglePlayerGame/GameBoardPanel/DiscardPilePanel` (GameObject)
- **Discard Pile Count Text**: `Screen_SinglePlayerGame/GameBoardPanel/DiscardPilePanel/DiscardPileCountText`

---

## 📋 **STEP 3: Assign MultiPlayer UI Elements**

### Navigate to Screen_MultiPlayerGame UI Elements:
```
Canvas/Screen_MultiPlayerGame/
```

### 3.1 Turn Display
- **Turn Indicator Text**: `Screen_MultiPlayerGame/SideInfoPanel/TurnIndicatorText`
- **Current Color Indicator**: `Screen_MultiPlayerGame/CurrentColorIndicator` (Image component)

### 3.2 Player Action Buttons
- **Play Card Button**: `Screen_MultiPlayerGame/Player1Panel/Player1ActionPanel/Btn_Player1PlayCard`
- **Draw Card Button**: `Screen_MultiPlayerGame/Player1Panel/Player1ActionPanel/Btn_Player1DrawCard`
- **End Turn Button**: `Screen_MultiPlayerGame/Player1Panel/Player1ActionPanel/Btn_Player1EndTurn`
- **Pause Button**: `Screen_MultiPlayerGame/Btn_Pause`

### 3.3 Hand Size Displays
- **Player1 Hand Size Text**: `Screen_MultiPlayerGame/Player1Panel/Player1ActionPanel/Player1HandSizePanel/Player1HandSizeText`
- **Player2 Hand Size Text**: `Screen_MultiPlayerGame/Player2Panel/Player2ActionPanel/Player2HandSizePanel/Player2HandSizeText`

### 3.4 Player Messages
- **Player Message Text**: `Screen_MultiPlayerGame/MainGameInfoPanel/GameMessageText`
- **Computer Message Text**: `Screen_MultiPlayerGame/Player2Panel/Player2ActionPanel/Player2MessagePanel/Player2MessageText`

### 3.5 Color Selection
- **Color Selection Panel**: `Screen_MultiPlayerGame/ColorSelectionPanel` (GameObject)
- **Select Red Button**: `Screen_MultiPlayerGame/ColorSelectionPanel/Btn_SelectRed`
- **Select Blue Button**: `Screen_MultiPlayerGame/ColorSelectionPanel/Btn_SelectBlue`
- **Select Green Button**: `Screen_MultiPlayerGame/ColorSelectionPanel/Btn_SelectGreen`
- **Select Yellow Button**: `Screen_MultiPlayerGame/ColorSelectionPanel/Btn_SelectYellow`

### 3.6 Special Features
- **Chain Status Text**: `Screen_MultiPlayerGame/MainGameInfoPanel/chainStatusText`
- **End Taki Sequence Button**: `Screen_MultiPlayerGame/GameBoardPanel/Btn_Player1EndTakiSequence`
- **Taki Sequence Status Text**: `Screen_MultiPlayerGame/MainGameInfoPanel/takiSequenceStatusText`

### 3.7 Deck Pile Elements
- **Draw Pile Panel**: `Screen_MultiPlayerGame/GameBoardPanel/DrawPilePanel` (GameObject)
- **Draw Pile Count Text**: `Screen_MultiPlayerGame/GameBoardPanel/DrawPilePanel/DrawPileCountText`
- **Discard Pile Panel**: `Screen_MultiPlayerGame/GameBoardPanel/DiscardPilePanel` (GameObject)
- **Discard Pile Count Text**: `Screen_MultiPlayerGame/GameBoardPanel/DiscardPilePanel/DiscardPileCountText`

---

## 📋 **STEP 4: Enable New Architecture**

### 4.1 Configure GameManager
1. **Select GameManager** in hierarchy
2. **Find "Use New UI Architecture"** checkbox in Inspector
3. **CHECK** the box ✅ to enable the new system
4. **Save** the scene (Ctrl+S)

### 4.2 Verify Setup
1. **All SinglePlayerUIManager fields** should be assigned (no "None" entries)
2. **All MultiPlayerUIManager fields** should be assigned (no "None" entries)
3. **Use New UI Architecture** should be checked ✅

---

## 🚨 **CRITICAL REMINDERS**

### ⚠️ **Common Issues to Avoid:**
1. **Wrong Screen Elements**: Don't assign Screen_SinglePlayer elements to MultiPlayer manager (or vice versa)
2. **Missing Components**: Some GameObjects might not have required components (TextMeshProUGUI, Button, etc.)
3. **Inactive GameObjects**: Make sure parent panels are active when assigning

### ✅ **Validation Checklist:**
- [ ] SinglePlayerUIManager has all 20+ fields assigned
- [ ] MultiPlayerUIManager has all 20+ fields assigned  
- [ ] Both managers show in GameManager Inspector
- [ ] "Use New UI Architecture" is checked
- [ ] No console errors about missing UI elements

---

## 🧪 **STEP 5: Test Implementation**

### 5.1 Test SinglePlayer Mode
1. **Run game** in Unity
2. **Start SinglePlayer game** from menu
3. **Verify**: No "GameplayUIManager not found" errors in console
4. **Test**: Buttons work, messages display, card counts update

### 5.2 Test MultiPlayer Mode  
1. **Run game** in Unity
2. **Start MultiPlayer game** from menu
3. **Verify**: No "GameplayUIManager not found" errors in console
4. **Test**: Network UI elements work, opponent display functions

---

## 🔧 **Troubleshooting**

### If you see "No UI manager available" errors:
1. **Check** "Use New UI Architecture" is enabled
2. **Verify** both UI managers are attached to GameManager
3. **Confirm** all UI elements are assigned correctly

### If buttons don't work:
1. **Check** Button components exist on assigned GameObjects
2. **Verify** GameObjects are active in hierarchy
3. **Confirm** UI managers are properly connected

### If text doesn't update:
1. **Check** TextMeshProUGUI components exist
2. **Verify** correct text elements are assigned
3. **Confirm** parent panels are active

---

## 🎉 **Success Indicators**

When setup is complete, you should see:
- ✅ No "GameplayUIManager not found" console errors
- ✅ Both SinglePlayer and MultiPlayer modes work
- ✅ HandManager connects to correct UI elements
- ✅ Turn indicators, button states, and messages all function
- ✅ Deck pile counts display correctly

The new architecture automatically selects the correct UI manager based on game mode, solving the original screen hierarchy problem!