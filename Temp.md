[UI] Screen references resolved
[SYS] BackgroundMusic marked as persistent and will not be destroyed on scene load.
[SYS] Switched to DEVELOPMENT mode
[SYS] TakiLogger configured: TakiLogger - Level: Trace, Mode: Development
[SYS] Validating and connecting components...
[SYS] New UI architecture validation complete - both UI managers assigned
[SYS] Per-screen HandManager architecture validation complete - all HandManagers assigned
[SYS] GameManager: ValidateComponents - All required components are assigned
[SYS] Components validated and connected - Ready for game mode selection
[DIAG] TakiGameDiagnostics ready. Press F1 for full diagnostics, F2 for rule validation, F3 for turn sequence test.
[SYS] ExitValidationManager dependencies resolved
[SYS] GameEndManager dependencies resolved
[SYS] PauseManager dependencies resolved
[SYS] DEBUG: ShouldBeActive() called for MultiPlayerUIManager
[SYS] DEBUG: GameManager found for MultiPlayerUIManager
[SYS] DEBUG: MultiPlayerUIManager - isMultiplayerMode=False, isThisSinglePlayerManager=False, isThisMultiPlayerManager=True
[SYS] MultiPlayerUIManager activity check: isMultiplayerMode=False, shouldBeActive=False
[SYS] DEBUG: MultiPlayerUIManager returning shouldBeActive=False
[SYS] New UI Architecture - MultiPlayerUIManager DISABLED (not active for current game mode)
[SYS] DEBUG: ShouldBeActive() called for SinglePlayerUIManager
[SYS] DEBUG: GameManager found for SinglePlayerUIManager
[SYS] DEBUG: SinglePlayerUIManager - isMultiplayerMode=False, isThisSinglePlayerManager=True, isThisMultiPlayerManager=False
[SYS] SinglePlayerUIManager activity check: isMultiplayerMode=False, shouldBeActive=True
[SYS] DEBUG: SinglePlayerUIManager returning shouldBeActive=True
[UI] New UI Architecture starting
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: DISABLED
[TURN] Play Card button updated: DISABLED
[TURN] Draw Card button updated: DISABLED
[TURN] End Turn button updated: DISABLED
[TURN] Strict button state update complete
[SYS] Loaded 110 cards from Resources/Data/Cards
[SYS] Deck composition verified: All cards loaded successfully
[SYS] DeckManager components initialized
[DECK] Deck Message: Loading deck...
[MP] Multiplayer game ready - transitioning to game screen
[SYS] HandManager Player1HandPanel: Awake() called - HandManager initializing...
[SYS] HandManager Player1InfoPanel: Awake() called - HandManager initializing...
[SYS] HandManager Player2InfoPanel: Awake() called - HandManager initializing...
[SYS] HandManager Player2HandPanel: Awake() called - HandManager initializing...
[MP] Starting multiplayer game...
[SYS] Initializing multiplayer game systems...
[NET] Multiplayer mode enabled
[NET] Computer AI disabled for multiplayer
[SYS] GameManager: ConnectEvents called!
[SYS] === CONNECTING ACTIVE UI MANAGER EVENTS ===
[SYS] singlePlayerUI: ASSIGNED
[SYS] multiPlayerUI: ASSIGNED
[UI] Deactivating SinglePlayerUIManager from current game mode
[UI] Disconnecting button events to prevent mode conflicts...
[SYS] Play Card button events disconnected
[SYS] Draw Card button events disconnected
[SYS] End Turn button events disconnected
[SYS] End TAKI Sequence button events disconnected
[SYS] All button events disconnected - mode switch safe
[UI] SinglePlayerUIManager deactivated successfully
[UI] Activating MultiPlayerUIManager for current game mode
[SYS] DEBUG: ShouldBeActive() called for MultiPlayerUIManager
[SYS] DEBUG: GameManager found for MultiPlayerUIManager
[SYS] DEBUG: MultiPlayerUIManager - isMultiplayerMode=True, isThisSinglePlayerManager=False, isThisMultiPlayerManager=True
[SYS] MultiPlayerUIManager activity check: isMultiplayerMode=True, shouldBeActive=True
[SYS] DEBUG: MultiPlayerUIManager returning shouldBeActive=True
[UI] Connecting button events with STRICT FLOW validation...
[SYS] Play Card button event connected
[SYS] Draw Card button event connected
[SYS] End Turn button event connected
[SYS] End TAKI Sequence button event connected
[SYS] All button events connected with strict flow validation
[UI] MultiPlayerUIManager activated successfully
[SYS] === UI MANAGER EVENTS CONNECTION COMPLETE ===
[SYS] Initializing visual card system...
[SYS] Visual card system initialized
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] Resetting UI for new game (base implementation)
[NET] Multiplayer hand sizes updated: Local=0, Opponent=0
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: DISABLED
[TURN] Play Card button updated: DISABLED
[TURN] Draw Card button updated: DISABLED
[TURN] End Turn button updated: DISABLED
[TURN] Strict button state update complete
[UI] Base UI reset for new game complete
[SYS] Multi player systems initialized - Ready to start game
[NET] DeckManager network mode set to: True
[NET] Network mode enabled - deck will be coordinated across clients
[NET] DeckManager configured for network mode
[NET] Initializing hand managers for network privacy
[NET] HandManager Player1HandPanel: Network mode = True
[NET] HandManager Player1HandPanel: Configured for opponent hand privacy
[NET] Initializing network hand: Local=True, FaceUp=False
[NET] Network hand initialized: FaceUp=True, OpponentDisplay=False
[NET] Player hand manager configured for local player
[NET] HandManager Player2HandPanel: Network mode = True
[NET] HandManager Player2HandPanel: Configured for opponent hand privacy
[NET] Initializing network hand: Local=False, FaceUp=False
[NET] Network hand initialized: FaceUp=False, OpponentDisplay=True
[NET] Opponent hand manager configured for opponent display
[NET] Network hand managers initialized with per-screen architecture - Mode: Multiplayer
[NET] === STARTING NETWORK GAME WITH DECK INITIALIZATION ===
[NET] === INITIALIZING SHARED DECK ===
[NET] I am Master Client - setting up deck and broadcasting state
[NET] Master client setting up deck - simplified approach
[DECK] Deck Message: New deck shuffled!
[DECK] Shuffled
[DECK] Initialized with 110 cards
[STATE] New game initialized successfully
[DECK] Drew card: Green Plus
[DECK] Drew card: Blue Plus
[DECK] Drew card: Yellow Taki
[DECK] Drew card: Green Taki
[DECK] Drew card: Green 7
[DECK] Drew card: Green PlusTwo
[DECK] Drew card: Yellow ChangeDirection
[DECK] Drew card: Yellow 7
[DECK] Drew card: Green PlusTwo
[DECK] Drew card: Red 6
[DECK] Drew card: Yellow 9
[DECK] Drew card: Green Taki
[DECK] Drew card: Red Taki
[DECK] Drew card: Blue 3
[DECK] Drew card: Blue 3
[DECK] Drew card: Yellow PlusTwo
[DECK] Drew card: Red ChangeDirection
[DECK] Drew card: Green 6
[DECK] Top discard card updated: Green 6
[DECK] Discarded card: Green 6
[STATE] Starting card: Green 6
[STATE] Initial setup complete. Player 1: 8 cards, Player 2: 8 cards
[STATE] Active color changed: Wild -> Green
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[NET] Multiplayer hand sizes updated: Local=8, Opponent=0
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] === UPDATING ALL DISPLAYS (BASE) ===
[UI] === UPDATING TURN DISPLAY for Neutral ===
[UI] Turn indicator text: 'Game Setup'
[TURN] Turn display updated - button states controlled by strict flow system
[UI] End TAKI Sequence button DISABLED & HIDDEN
[UI] TAKI sequence status hidden
[UI] Handling active game state
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] Chain status hidden
[NET] Hand display updated: 8 cards, Network=True, Opponent=False
[UI] Updated player hand display: 8 cards
[NET] Showing 0 opponent cards with privacy mode
[NET] Opponent hand displayed with privacy: 0 real cards as card backs
[NET] Hand display updated (enhanced): 0 cards, Privacy=True
[UI] Updated opponent hand display (multiplayer): 0 cards with privacy
[TURNS] Turn system initialized. First player: Human
[STATE] Turn state changed: Neutral -> PlayerTurn
[STATE] Turn state changed to PlayerTurn
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] === UPDATING TURN DISPLAY for PlayerTurn ===
[UI] Turn indicator text: 'Your Turn'
[TURN] Turn display updated - button states controlled by strict flow system
[TURNS] Turn started for: Human
[TURN] Turn changed to Local Player (PlayerType: Human)
[SYS] Game started! Player: 8 cards, Opponent: 8 cards
[DECK] Deck Message: Starting card: Green 6
[NET] SetupInitialGame successful: P1=8, P2=8, Start=Green 6
[NET] Serializing card: Green Plus -> Green_Plus
[NET] Serializing card: Blue Plus -> Blue_Plus
[NET] Serializing card: Yellow Taki -> Yellow_Taki
[NET] Serializing card: Green Taki -> Green_Taki
[NET] Serializing card: Green 7 -> Green_7
[NET] Serializing card: Green PlusTwo -> Green_PlusTwo
[NET] Serializing card: Yellow ChangeDirection -> Yellow_ChangeDirection
[NET] Serializing card: Yellow 7 -> Yellow_7
[NET] Hand serialized: 8 cards -> 97 characters
[NET] Serializing card: Green PlusTwo -> Green_PlusTwo
[NET] Serializing card: Red 6 -> Red_6
[NET] Serializing card: Yellow 9 -> Yellow_9
[NET] Serializing card: Green Taki -> Green_Taki
[NET] Serializing card: Red Taki -> Red_Taki
[NET] Serializing card: Blue 3 -> Blue_3
[NET] Serializing card: Blue 3 -> Blue_3
[NET] Serializing card: Yellow PlusTwo -> Yellow_PlusTwo
[NET] Hand serialized: 8 cards -> 77 characters
[NET] === SENDING INITIAL GAME STATE RPC ===
[NET] Starting Card ID: Green_6
[NET] Draw Pile Count: 93
[NET] Player 1 Hand (serialized): Green_Plus|Blue_Plus|Yellow_Taki|Green_Taki|Green_7|Green_PlusTwo|Yellow_ChangeDirection|Yellow_7
[NET] Player 2 Hand (serialized): Green_PlusTwo|Red_6|Yellow_9|Green_Taki|Red_Taki|Blue_3|Blue_3|Yellow_PlusTwo
[NET] Master Client Actor Number: 1
[NET] Player 1 Hand Size: 8 cards
[NET] Player 2 Hand Size: 8 cards
[NET] === RPC MESSAGE DETAILS LOGGED ===
[NET] Setting up multiplayer hands - simplified approach
[NET] Hand assignment: Local=8 cards, Opponent=8 cards
[NET] GameManager playerHand updated: 0 cards
[NET] HandManager Player1HandPanel: Network mode = True
[NET] Hand display updated: 0 cards, Network=True, Opponent=False
[NET] Local player hand displayed: 0 cards (per-screen architecture)
[NET] HandManager Player2HandPanel: Enhanced network mode = True
[NET] HandManager Player2HandPanel: Configured for enhanced opponent hand privacy
[NET] Initializing enhanced network hand: Local=False, Cards=8
[NET] Showing 8 opponent cards with privacy mode
[NET] CardController: Enhanced initialization - Card: Green PlusTwo, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red 6, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow 9, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Green Taki, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red Taki, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 3, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 3, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow PlusTwo, FaceUp: False, Privacy: True
[NET] Opponent hand displayed with privacy: 8 real cards as card backs
[NET] Hand display updated (enhanced): 8 cards, Privacy=True
[NET] Opponent hand displayed with privacy: 8 cards as card backs
[NET] Enhanced network hand initialized: FaceUp=False, OpponentDisplay=True
[NET] Opponent hand setup with REAL CARDS and privacy: 8 cards (per-screen architecture)
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[NET] Multiplayer hand sizes updated: Local=0, Opponent=8
[NET] Multiplayer hands setup complete - simplified approach successful
[NET] Updating multiplayer deck display: Draw=93, Discard=1, Top=Green 6
[NET] DeckUI PileManager status: ASSIGNED
[DECK] Top discard card updated: Green 6
[NET] Multiplayer deck display updated successfully
[NET] Master deck setup complete - simplified approach successful
[NET] Multiplayer systems initialized successfully
[SYS] Starting new multiplayer game...
[SYS] Waiting for HandManager initialization...
[SYS] HandManager initialization check - Player: False, Opponent: False
[SYS] HandManager Player1HandPanel: Start() called - Looking for GameManager...
[SYS] HandManager Player1HandPanel: DIAGNOSTIC - GameManager found, checking UI architecture...
[SYS]   - singlePlayerUI: ASSIGNED
[SYS]   - multiPlayerUI: ASSIGNED
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] HandManager Player1HandPanel: Connected to active UI manager: MultiPlayerUIManager
[SYS] HandManager Player1HandPanel: Initialization COMPLETE
[SYS] HandManager Player1InfoPanel: Start() called - Looking for GameManager...
[SYS] HandManager Player1InfoPanel: DIAGNOSTIC - GameManager found, checking UI architecture...
[SYS]   - singlePlayerUI: ASSIGNED
[SYS]   - multiPlayerUI: ASSIGNED
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] HandManager Player1InfoPanel: Connected to active UI manager: MultiPlayerUIManager
[SYS] HandManager Player1InfoPanel: Initialization COMPLETE
[SYS] HandManager Player2InfoPanel: Start() called - Looking for GameManager...
[SYS] HandManager Player2InfoPanel: DIAGNOSTIC - GameManager found, checking UI architecture...
[SYS]   - singlePlayerUI: ASSIGNED
[SYS]   - multiPlayerUI: ASSIGNED
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] HandManager Player2InfoPanel: Connected to active UI manager: MultiPlayerUIManager
[SYS] HandManager Player2InfoPanel: Initialization COMPLETE
[SYS] HandManager Player2HandPanel: Start() called - Looking for GameManager...
[SYS] HandManager Player2HandPanel: DIAGNOSTIC - GameManager found, checking UI architecture...
[SYS]   - singlePlayerUI: ASSIGNED
[SYS]   - multiPlayerUI: ASSIGNED
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] HandManager Player2HandPanel: Connected to active UI manager: MultiPlayerUIManager
[SYS] HandManager Player2HandPanel: Initialization COMPLETE
[SYS] HandManager initialization check - Player: True, Opponent: True
[SYS] HandManager initialization check - Player: True, Opponent: True
[SYS] All HandManagers initialized successfully!
[SYS] HandManager initialization check - Player: True, Opponent: True
[SYS] Reseting game systems in multiplayer mode
[SYS] In multiplayer, hands are populated from network data and should NOT be cleared
[STATE] Game state reset for new game (including PlusTwo chain and TAKI sequence state)
[TURNS] Turn manager reset
[TURN] === RESETTING SPECIAL CARD STATE ===
[TURN] TURN FLOW STATE RESET (includes special card state)
[SYS] Game initialization state reset - ready for new game
[NET] === STARTING NETWORK GAME WITH DECK INITIALIZATION ===
[NET] === INITIALIZING SHARED DECK ===
[NET] I am Master Client - setting up deck and broadcasting state
[NET] Master client setting up deck - simplified approach
[DECK] Deck Message: New deck shuffled!
[DECK] Shuffled
[DECK] Initialized with 110 cards
[STATE] New game initialized successfully
[DECK] Drew card: Green 8
[DECK] Drew card: Blue PlusTwo
[DECK] Drew card: Red 4
[DECK] Drew card: Green Plus
[DECK] Drew card: Blue Stop
[DECK] Drew card: Blue 8
[DECK] Drew card: Yellow 9
[DECK] Drew card: Red 4
[DECK] Drew card: Wild ChangeColor
[DECK] Drew card: Red ChangeDirection
[DECK] Drew card: Blue Stop
[DECK] Drew card: Red 6
[DECK] Drew card: Blue 5
[DECK] Drew card: Green 1
[DECK] Drew card: Red Taki
[DECK] Drew card: Blue 1
[DECK] Drew card: Yellow 8
[DECK] Top discard card updated: Yellow 8
[DECK] Discarded card: Yellow 8
[STATE] Starting card: Yellow 8
[STATE] Initial setup complete. Player 1: 8 cards, Player 2: 8 cards
[STATE] Active color changed: Wild -> Yellow
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[NET] Multiplayer hand sizes updated: Local=8, Opponent=0
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] === UPDATING ALL DISPLAYS (BASE) ===
[UI] === UPDATING TURN DISPLAY for Neutral ===
[UI] Turn indicator text: 'Game Setup'
[TURN] Turn display updated - button states controlled by strict flow system
[UI] End TAKI Sequence button DISABLED & HIDDEN
[UI] TAKI sequence status hidden
[UI] Handling active game state
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] Chain status hidden
[RULES] Move validation: Green 8 on Yellow 8 with active color Yellow = True
[RULES] Move validation: Blue PlusTwo on Yellow 8 with active color Yellow = False
[RULES] Move validation: Red 4 on Yellow 8 with active color Yellow = False
[RULES] Move validation: Green Plus on Yellow 8 with active color Yellow = False
[RULES] Move validation: Blue Stop on Yellow 8 with active color Yellow = False
[RULES] Move validation: Blue 8 on Yellow 8 with active color Yellow = True
[RULES] Move validation: Yellow 9 on Yellow 8 with active color Yellow = True
[RULES] Move validation: Red 4 on Yellow 8 with active color Yellow = False
[NET] Hand display updated: 8 cards, Network=True, Opponent=False
[UI] Updated player hand display: 8 cards
[NET] Showing 8 opponent cards with privacy mode
[NET] CardController: Enhanced initialization - Card: Green PlusTwo, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red 6, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow 9, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Green Taki, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red Taki, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 3, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 3, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow PlusTwo, FaceUp: False, Privacy: True
[NET] Multiplayer hand sizes updated: Local=8, Opponent=8
[NET] Opponent hand displayed with privacy: 8 real cards as card backs
[NET] Hand display updated (enhanced): 8 cards, Privacy=True
[UI] Updated opponent hand display (multiplayer): 8 cards with privacy
[TURNS] Turn system initialized. First player: Human
[STATE] Turn state changed: Neutral -> PlayerTurn
[STATE] Turn state changed to PlayerTurn
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] === UPDATING TURN DISPLAY for PlayerTurn ===
[UI] Turn indicator text: 'Your Turn'
[TURN] Turn display updated - button states controlled by strict flow system
[TURNS] Turn started for: Human
[TURN] Turn changed to Local Player (PlayerType: Human)
[SYS] Game started! Player: 8 cards, Opponent: 8 cards
[DECK] Deck Message: Starting card: Yellow 8
[NET] SetupInitialGame successful: P1=8, P2=8, Start=Yellow 8
[NET] Serializing card: Green 8 -> Green_8
[NET] Serializing card: Blue PlusTwo -> Blue_PlusTwo
[NET] Serializing card: Red 4 -> Red_4
[NET] Serializing card: Green Plus -> Green_Plus
[NET] Serializing card: Blue Stop -> Blue_Stop
[NET] Serializing card: Blue 8 -> Blue_8
[NET] Serializing card: Yellow 9 -> Yellow_9
[NET] Serializing card: Red 4 -> Red_4
[NET] Hand serialized: 8 cards -> 69 characters
[NET] Serializing card: Wild ChangeColor -> Wild_ChangeColor
[NET] Serializing card: Red ChangeDirection -> Red_ChangeDirection
[NET] Serializing card: Blue Stop -> Blue_Stop
[NET] Serializing card: Red 6 -> Red_6
[NET] Serializing card: Blue 5 -> Blue_5
[NET] Serializing card: Green 1 -> Green_1
[NET] Serializing card: Red Taki -> Red_Taki
[NET] Serializing card: Blue 1 -> Blue_1
[NET] Hand serialized: 8 cards -> 83 characters
[NET] === SENDING INITIAL GAME STATE RPC ===
[NET] Starting Card ID: Yellow_8
[NET] Draw Pile Count: 93
[NET] Player 1 Hand (serialized): Green_8|Blue_PlusTwo|Red_4|Green_Plus|Blue_Stop|Blue_8|Yellow_9|Red_4
[NET] Player 2 Hand (serialized): Wild_ChangeColor|Red_ChangeDirection|Blue_Stop|Red_6|Blue_5|Green_1|Red_Taki|Blue_1
[NET] Master Client Actor Number: 1
[NET] Player 1 Hand Size: 8 cards
[NET] Player 2 Hand Size: 8 cards
[NET] === RPC MESSAGE DETAILS LOGGED ===
[NET] Setting up multiplayer hands - simplified approach
[NET] Hand assignment: Local=8 cards, Opponent=8 cards
[NET] GameManager playerHand updated: 0 cards
[NET] HandManager Player1HandPanel: Network mode = True
[RULES] HandManager Player1HandPanel: No playable cards found (0/0)
[NET] Hand display updated: 0 cards, Network=True, Opponent=False
[NET] Local player hand displayed: 0 cards (per-screen architecture)
[NET] HandManager Player2HandPanel: Enhanced network mode = True
[NET] HandManager Player2HandPanel: Configured for enhanced opponent hand privacy
[NET] Initializing enhanced network hand: Local=False, Cards=8
[NET] Multiplayer hand sizes updated: Local=0, Opponent=8
[NET] Showing 8 opponent cards with privacy mode
[NET] CardController: Enhanced initialization - Card: Wild ChangeColor, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red ChangeDirection, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue Stop, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red 6, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 5, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Green 1, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red Taki, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 1, FaceUp: False, Privacy: True
[NET] Multiplayer hand sizes updated: Local=0, Opponent=8
[NET] Opponent hand displayed with privacy: 8 real cards as card backs
[NET] Hand display updated (enhanced): 8 cards, Privacy=True
[NET] Opponent hand displayed with privacy: 8 cards as card backs
[NET] Enhanced network hand initialized: FaceUp=False, OpponentDisplay=True
[NET] Opponent hand setup with REAL CARDS and privacy: 8 cards (per-screen architecture)
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[NET] Multiplayer hand sizes updated: Local=0, Opponent=8
[NET] Multiplayer hands setup complete - simplified approach successful
[NET] Updating multiplayer deck display: Draw=93, Discard=1, Top=Yellow 8
[NET] DeckUI PileManager status: ASSIGNED
[DECK] Top discard card updated: Yellow 8
[NET] Multiplayer deck display updated successfully
[NET] Master deck setup complete - simplified approach successful
[NET] === TURN 1 BEGINS ===
[NET] Is my turn: True
[NET] First turn initialization complete
[TURN] Starting Player Turn
[TURN] Normal turn flow - no active chain
[TURN] Player has no valid cards, must draw a card
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: ENABLED
[TURN] END TURN: DISABLED
[TURN] Play Card button updated: DISABLED
[TURN] Draw Card button updated: ENABLED
[TURN] End Turn button updated: DISABLED
[TURN] Strict button state update complete
[UI] REFRESHING PLAYER HAND STATES
[RULES] HandManager Player1HandPanel: No playable cards found (0/0)
[UI] REFRESHING PLAYER HAND STATES
[RULES] HandManager Player1HandPanel: No playable cards found (0/0)
[TURN] Starting Player Turn
[TURN] Normal turn flow - no active chain
[TURN] Player has no valid cards, must draw a card
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: ENABLED
[TURN] END TURN: DISABLED
[TURN] Play Card button updated: DISABLED
[TURN] Draw Card button updated: ENABLED
[TURN] End Turn button updated: DISABLED
[TURN] Strict button state update complete
[UI] REFRESHING PLAYER HAND STATES
[RULES] HandManager Player1HandPanel: No playable cards found (0/0)
[UI] REFRESHING PLAYER HAND STATES
[RULES] HandManager Player1HandPanel: No playable cards found (0/0)
[NET] === TURN 1 BEGINS ===
[NET] Is my turn: True
