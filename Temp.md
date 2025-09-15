[UI] Screen references resolved
[SYS] BackgroundMusic marked as persistent and will not be destroyed on scene load.
[SYS] Switched to DEVELOPMENT mode
[SYS] TakiLogger configured: TakiLogger - Level: Info, Mode: Development
[SYS] Validating and connecting components...
[SYS] New UI architecture validation complete - both UI managers assigned
[SYS] Per-screen HandManager architecture validation complete - all HandManagers assigned
[SYS] Components validated and connected - Ready for game mode selection
[DIAG] TakiGameDiagnostics ready. Press F1 for full diagnostics, F2 for rule validation, F3 for turn sequence test.
[SYS] ExitValidationManager dependencies resolved
[SYS] GameEndManager dependencies resolved
[SYS] PauseManager dependencies resolved
[UI] New UI Architecture - MultiPlayerUIManager starting
[UI] Connecting button events with STRICT FLOW validation...
[SYS] All button events connected with strict flow validation
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: DISABLED
[UI] New UI Architecture - SinglePlayerUIManager starting
[UI] Connecting button events with STRICT FLOW validation...
[SYS] All button events connected with strict flow validation
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: DISABLED
[SYS] Loaded 110 cards from Resources/Data/Cards
[SYS] Deck composition verified: All cards loaded successfully
[SYS] DeckManager components initialized
[DECK] Deck Message: Loading deck...
[SYS] HandManager Player2HandPanel: Awake() called - HandManager initializing...
[SYS] HandManager Player1HandPanel: Awake() called - HandManager initializing...
[STATE] Starting single player game...
[SYS] Initializing single player game systems...
[INVESTIGATION] ConnectEvents called!
[SYS] === CONNECTING ACTIVE UI MANAGER EVENTS ===
[SYS] useNewUIArchitecture: True
[SYS] singlePlayerUI: ASSIGNED
[SYS] multiPlayerUI: ASSIGNED
[SYS] Event handlers connected to SinglePlayerUI
[SYS] Event handlers connected to MultiPlayerUI
[SYS] === UI MANAGER EVENTS CONNECTION COMPLETE ===
[SYS] Initializing visual card system...
[SYS] Visual card system initialized with per-screen architecture - Mode: SinglePlayer
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: DISABLED
[TURN] Strict button state update complete
[SYS] Single player systems initialized - Ready to start game
[SYS] Starting new single player game...
[AI] Clearing hand and resetting all AI state (including sequence state)
[AI] Cancelling all AI operations
[AI] All AI operations cancelled
[AI] AI hand cleared and all state reset (including sequence processing)
[STATE] Game state reset for new game (including PlusTwo chain and TAKI sequence state)
[TURNS] Turn manager reset
[TURN] === RESETTING SPECIAL CARD STATE ===
[TURN] TURN FLOW STATE RESET (includes special card state)
[SYS] Game initialization state reset - ready for new game
[DECK] Deck Message: New deck shuffled!
[DECK] Shuffled
[DECK] Initialized with 110 cards
[STATE] New game initialized successfully
[DECK] Drew card: Yellow Taki
[DECK] Drew card: Wild ChangeColor
[DECK] Drew card: Green 3
[DECK] Drew card: Red 5
[DECK] Drew card: Red Taki
[DECK] Drew card: Green 8
[DECK] Drew card: Blue 8
[DECK] Drew card: Green Plus
[DECK] Drew card: Yellow 7
[DECK] Drew card: Blue Plus
[DECK] Drew card: Blue 4
[DECK] Drew card: Yellow 5
[DECK] Drew card: Red ChangeDirection
[DECK] Drew card: Green Taki
[DECK] Drew card: Blue Stop
[DECK] Drew card: Blue 6
[DECK] Drew card: Yellow 6
[DECK] Discarded card: Yellow 6
[STATE] Starting card: Yellow 6
[STATE] Initial setup complete. Player 1: 8 cards, Player 2: 8 cards
[AI] AI received 8 cards. Hand size: 8
[STATE] Active color changed: Wild -> Yellow
[UI] === UPDATING ALL DISPLAYS (FIXED SEQUENCE CONTROL) ===
[UI] === UPDATING TURN DISPLAY for Neutral ===
[TURN] Turn display updated - button states controlled by strict flow system
[UI] End TAKI Sequence button DISABLED & HIDDEN (Human sequences only)
[UI] TAKI sequence status hidden
[UI] Handling active game state
[UI] All displays updated (with enhanced TAKI sequence support)
[UI] Chain status hidden
[NET] Hand display updated: 8 cards, Network=False, Opponent=False
[NET] Hand display updated: 8 cards, Network=False, Opponent=False
[TURNS] Turn system initialized. First player: Human
[STATE] Turn state changed: Neutral -> PlayerTurn
[STATE] Turn state changed to PlayerTurn
[UI] === UPDATING TURN DISPLAY for PlayerTurn ===
[TURN] Turn display updated - button states controlled by strict flow system
[TURNS] Turn started for: Human
[SYS] Game started! Player: 8 cards, Computer: 8 cards
[DECK] Deck Message: Starting card: Yellow 6
[NET] SetupInitialGame successful: P1=8, P2=8, Start=Yellow 6
[SYS] HandManager Player2HandPanel: Start() called - Looking for GameManager...
[SYS] HandManager Player2HandPanel: DIAGNOSTIC - GameManager found, checking UI architecture...
[SYS]   - useNewUIArchitecture: True
[SYS]   - singlePlayerUI: ASSIGNED
[SYS]   - multiPlayerUI: ASSIGNED
[SYS] GetActiveUI() called - useNewUIArchitecture: True, isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] HandManager Player2HandPanel: Connected to active UI manager: SinglePlayerUIManager
[SYS] HandManager Player1HandPanel: Start() called - Looking for GameManager...
[SYS] HandManager Player1HandPanel: DIAGNOSTIC - GameManager found, checking UI architecture...
[SYS]   - useNewUIArchitecture: True
[SYS]   - singlePlayerUI: ASSIGNED
[SYS]   - multiPlayerUI: ASSIGNED
[SYS] GetActiveUI() called - useNewUIArchitecture: True, isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] HandManager Player1HandPanel: Connected to active UI manager: SinglePlayerUIManager
[TURN] STARTING PLAYER TURN WITH PLSTWO CHAIN AWARENESS
[TURN] Normal turn flow - no active chain
[RULES] Move validation: Yellow Taki on Yellow 6 with active color Yellow = True
[RULES] Move validation: Wild ChangeColor on Yellow 6 with active color Yellow = True
[RULES] Move validation: Green 3 on Yellow 6 with active color Yellow = False
[RULES] Move validation: Red 5 on Yellow 6 with active color Yellow = False
[RULES] Move validation: Red Taki on Yellow 6 with active color Yellow = False
[RULES] Move validation: Green 8 on Yellow 6 with active color Yellow = False
[RULES] Move validation: Blue 8 on Yellow 6 with active color Yellow = False
[RULES] Move validation: Green Plus on Yellow 6 with active color Yellow = False
[TURN] Player has 2 valid cards, may PLAY or DRAW a card
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: ENABLED
[TURN] DRAW: ENABLED
[TURN] END TURN: DISABLED
[TURN] Strict button state update complete
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Yellow Taki on Yellow 6 with active color Yellow = True
[RULES] Move validation: Wild ChangeColor on Yellow 6 with active color Yellow = True
[RULES] Move validation: Green 3 on Yellow 6 with active color Yellow = False
[RULES] Move validation: Red 5 on Yellow 6 with active color Yellow = False
[RULES] Move validation: Red Taki on Yellow 6 with active color Yellow = False
[RULES] Move validation: Green 8 on Yellow 6 with active color Yellow = False
[RULES] Move validation: Blue 8 on Yellow 6 with active color Yellow = False
[RULES] Move validation: Green Plus on Yellow 6 with active color Yellow = False
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Yellow Taki on Yellow 6 with active color Yellow = True
[RULES] Move validation: Wild ChangeColor on Yellow 6 with active color Yellow = True
[RULES] Move validation: Green 3 on Yellow 6 with active color Yellow = False
[RULES] Move validation: Red 5 on Yellow 6 with active color Yellow = False
[RULES] Move validation: Red Taki on Yellow 6 with active color Yellow = False
[RULES] Move validation: Green 8 on Yellow 6 with active color Yellow = False
[RULES] Move validation: Blue 8 on Yellow 6 with active color Yellow = False
[RULES] Move validation: Green Plus on Yellow 6 with active color Yellow = False
[UI] Player selected visual card: Yellow Taki
[RULES] Move validation: Yellow Taki on Yellow 6 with active color Yellow = True
[TURN] === PLAY CARD BUTTON CLICKED ===
[TURN] PLAY CARD clicked but button should be disabled!