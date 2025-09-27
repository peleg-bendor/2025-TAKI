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
[SYS] HandManager Player2HandPanel: Awake() called - HandManager initializing...
[SYS] HandManager Player1HandPanel: Awake() called - HandManager initializing...
[STATE] Starting single player game...
[SYS] Initializing singleplayer game systems...
[NET] Multiplayer mode disabled
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
[SYS] Event handlers disconnected and UI manager deactivated for SinglePlayerUIManager
[UI] Deactivating MultiPlayerUIManager from current game mode
[UI] Disconnecting button events to prevent mode conflicts...
[SYS] Play Card button events disconnected
[SYS] Draw Card button events disconnected
[SYS] End Turn button events disconnected
[SYS] End TAKI Sequence button events disconnected
[SYS] All button events disconnected - mode switch safe
[UI] MultiPlayerUIManager deactivated successfully
[SYS] Event handlers disconnected and UI manager deactivated for MultiPlayerUIManager
[UI] Activating SinglePlayerUIManager for current game mode
[SYS] DEBUG: ShouldBeActive() called for SinglePlayerUIManager
[SYS] DEBUG: GameManager found for SinglePlayerUIManager
[SYS] DEBUG: SinglePlayerUIManager - isMultiplayerMode=False, isThisSinglePlayerManager=True, isThisMultiPlayerManager=False
[SYS] SinglePlayerUIManager activity check: isMultiplayerMode=False, shouldBeActive=True
[SYS] DEBUG: SinglePlayerUIManager returning shouldBeActive=True
[UI] Connecting button events with STRICT FLOW validation...
[SYS] Play Card button event connected
[SYS] Draw Card button event connected
[SYS] End Turn button event connected
[SYS] End TAKI Sequence button event connected
[SYS] All button events connected with strict flow validation
[UI] SinglePlayerUIManager activated successfully
[SYS] === UI MANAGER EVENTS CONNECTION COMPLETE ===
[TURNS] Turn manager multiplayer mode set to: False
[NET] Computer AI enabled for singleplayer
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
[SYS] Event handlers disconnected and UI manager deactivated for SinglePlayerUIManager
[UI] Deactivating MultiPlayerUIManager from current game mode
[UI] Disconnecting button events to prevent mode conflicts...
[SYS] Play Card button events disconnected
[SYS] Draw Card button events disconnected
[SYS] End Turn button events disconnected
[SYS] End TAKI Sequence button events disconnected
[SYS] All button events disconnected - mode switch safe
[UI] MultiPlayerUIManager deactivated successfully
[SYS] Event handlers disconnected and UI manager deactivated for MultiPlayerUIManager
[UI] Activating SinglePlayerUIManager for current game mode
[SYS] DEBUG: ShouldBeActive() called for SinglePlayerUIManager
[SYS] DEBUG: GameManager found for SinglePlayerUIManager
[SYS] DEBUG: SinglePlayerUIManager - isMultiplayerMode=False, isThisSinglePlayerManager=True, isThisMultiPlayerManager=False
[SYS] SinglePlayerUIManager activity check: isMultiplayerMode=False, shouldBeActive=True
[SYS] DEBUG: SinglePlayerUIManager returning shouldBeActive=True
[UI] Connecting button events with STRICT FLOW validation...
[SYS] Play Card button event connected
[SYS] Draw Card button event connected
[SYS] End Turn button event connected
[SYS] End TAKI Sequence button event connected
[SYS] All button events connected with strict flow validation
[UI] SinglePlayerUIManager activated successfully
[SYS] === UI MANAGER EVENTS CONNECTION COMPLETE ===
[SYS] Initializing visual card system...
[SYS] Visual card system initialized
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] Resetting UI for new game (base implementation)
[UI] SinglePlayer hand sizes updated: Human=0, Computer=0
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: DISABLED
[TURN] Play Card button updated: DISABLED
[TURN] Draw Card button updated: DISABLED
[TURN] End Turn button updated: DISABLED
[TURN] Strict button state update complete
[UI] Base UI reset for new game complete
[SYS] Single player systems initialized - Ready to start game
[NET] Singleplayer systems initialized successfully
[SYS] Starting new single player game...
[SYS] Reseting game systems in singleplayer mode
[SYS] Clearing plyer hand...
[SYS] Clearing computer hand...
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
[NET] SETUP DEBUG: About to draw Player 1 hand
[DECK] Drew card: Green ChangeDirection
[DECK] Drew card: Blue PlusTwo
[DECK] Drew card: Yellow 9
[DECK] Drew card: Blue 7
[DECK] Drew card: Blue 6
[DECK] Drew card: Green Stop
[DECK] Drew card: Wild ChangeColor
[DECK] Drew card: Yellow Stop
[NET] DrawInitialHand DEBUG: Drew 8 cards, First card: Green ChangeDirection, Hand reference: 255492154
[NET] SETUP DEBUG: About to draw Player 2 hand
[DECK] Drew card: Green Stop
[DECK] Drew card: Green 9
[DECK] Drew card: Red Stop
[DECK] Drew card: Blue 4
[DECK] Drew card: Green 5
[DECK] Drew card: Red Taki
[DECK] Drew card: Blue 5
[DECK] Drew card: Blue Stop
[NET] DrawInitialHand DEBUG: Drew 8 cards, First card: Green Stop, Hand reference: -1780134110
[NET] SETUP DEBUG: player1Hand == player2Hand reference: False
[NET] SETUP DEBUG: player1Hand reference: 255492154, player2Hand reference: -1780134110
[NET] SETUP DEBUG: player1Hand first card: Green ChangeDirection
[NET] SETUP DEBUG: player2Hand first card: Green Stop
[DECK] Drew card: Yellow ChangeDirection
[DECK] Drew card: Red 8
[DECK] Top discard card updated: Red 8
[DECK] Discarded card: Red 8
[STATE] Starting card: Red 8
[STATE] Initial setup complete. Player 1: 8 cards, Player 2: 8 cards
[AI] AI received 8 cards. Hand size: 8
[STATE] Active color changed: Wild -> Red
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] SinglePlayer hand sizes updated: Human=8, Computer=8
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] === UPDATING ALL DISPLAYS (BASE) ===
[UI] === UPDATING TURN DISPLAY for Neutral ===
[UI] Turn indicator text: 'Game Setup'
[TURN] Turn display updated - button states controlled by strict flow system
[UI] End TAKI Sequence button DISABLED & HIDDEN
[UI] TAKI sequence status hidden
[UI] Handling active game state
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] Chain status hidden
[NET] Hand display updated: 8 cards, Network=False, Opponent=False
[UI] Updated player hand display: 8 cards
[NET] Hand display updated: 8 cards, Network=False, Opponent=False
[UI] Updated opponent hand display (singleplayer): 8 cards
[TURNS] Turn system initialized. First player: Human
[STATE] Turn state changed: Neutral -> PlayerTurn
[STATE] Turn state changed to PlayerTurn
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] === UPDATING TURN DISPLAY for PlayerTurn ===
[UI] Turn indicator text: 'Your Turn'
[TURN] Turn display updated - button states controlled by strict flow system
[TURNS] Turn started for: Human
[SYS] Game started! Player: 8 cards, Opponent: 8 cards
[DECK] Deck Message: Starting card: Red 8
[NET] SetupInitialGame successful: P1=8, P2=8, Start=Red 8
[SYS] HandManager Player2HandPanel: Start() called - Looking for GameManager...
[SYS] HandManager Player2HandPanel: DIAGNOSTIC - GameManager found, checking UI architecture...
[SYS]   - singlePlayerUI: ASSIGNED
[SYS]   - multiPlayerUI: ASSIGNED
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] HandManager Player2HandPanel: Connected to active UI manager: SinglePlayerUIManager
[SYS] HandManager Player2HandPanel: Initialization COMPLETE
[SYS] HandManager Player1HandPanel: Start() called - Looking for GameManager...
[SYS] HandManager Player1HandPanel: DIAGNOSTIC - GameManager found, checking UI architecture...
[SYS]   - singlePlayerUI: ASSIGNED
[SYS]   - multiPlayerUI: ASSIGNED
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] HandManager Player1HandPanel: Connected to active UI manager: SinglePlayerUIManager
[SYS] HandManager Player1HandPanel: Initialization COMPLETE
[TURN] Starting Player Turn
[TURN] Normal turn flow - no active chain
[RULES] Move validation: Green ChangeDirection on Red 8 with active color Red = False
[RULES] Move validation: Blue PlusTwo on Red 8 with active color Red = False
[RULES] Move validation: Yellow 9 on Red 8 with active color Red = False
[RULES] Move validation: Blue 7 on Red 8 with active color Red = False
[RULES] Move validation: Blue 6 on Red 8 with active color Red = False
[RULES] Move validation: Green Stop on Red 8 with active color Red = False
[RULES] Move validation: Wild ChangeColor on Red 8 with active color Red = True
[RULES] Move validation: Yellow Stop on Red 8 with active color Red = False
[TURN] Player has 1 valid cards, may PLAY or DRAW a card
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: ENABLED
[TURN] DRAW: ENABLED
[TURN] END TURN: DISABLED
[TURN] Play Card button updated: ENABLED
[TURN] Draw Card button updated: ENABLED
[TURN] End Turn button updated: DISABLED
[TURN] Strict button state update complete
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Green ChangeDirection on Red 8 with active color Red = False
[RULES] Move validation: Blue PlusTwo on Red 8 with active color Red = False
[RULES] Move validation: Yellow 9 on Red 8 with active color Red = False
[RULES] Move validation: Blue 7 on Red 8 with active color Red = False
[RULES] Move validation: Blue 6 on Red 8 with active color Red = False
[RULES] Move validation: Green Stop on Red 8 with active color Red = False
[RULES] Move validation: Wild ChangeColor on Red 8 with active color Red = True
[RULES] Move validation: Yellow Stop on Red 8 with active color Red = False
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Green ChangeDirection on Red 8 with active color Red = False
[RULES] Move validation: Blue PlusTwo on Red 8 with active color Red = False
[RULES] Move validation: Yellow 9 on Red 8 with active color Red = False
[RULES] Move validation: Blue 7 on Red 8 with active color Red = False
[RULES] Move validation: Blue 6 on Red 8 with active color Red = False
[RULES] Move validation: Green Stop on Red 8 with active color Red = False
[RULES] Move validation: Wild ChangeColor on Red 8 with active color Red = True
[RULES] Move validation: Yellow Stop on Red 8 with active color Red = False
[UI] Pause button clicked
[SYS] GameManager: Pause game requested
[SYS] === PAUSING GAME ===
[TURN] Turn flow state captured: Action=False, Play=True, Draw=True, EndTurn=False
[SYS] GameManager turn flow state captured
[SYS] Game state captured: Turn=PlayerTurn, Player=Human
[STATE] Game status changed: Active -> Paused
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] SinglePlayer hand sizes updated: Human=8, Computer=8
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] === UPDATING ALL DISPLAYS (BASE) ===
[UI] === UPDATING TURN DISPLAY for PlayerTurn ===
[UI] Turn indicator text: 'Game Paused'
[TURN] Turn display updated - button states controlled by strict flow system
[UI] End TAKI Sequence button DISABLED & HIDDEN
[UI] TAKI sequence status hidden
[UI] Handling paused game state
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: DISABLED
[TURN] Play Card button updated: DISABLED
[TURN] Draw Card button updated: DISABLED
[TURN] End Turn button updated: DISABLED
[TURN] Strict button state update complete
[UI] End TAKI Sequence button DISABLED & HIDDEN
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] Chain status hidden
[RULES] Move validation: Green ChangeDirection on Red 8 with active color Red = False
[RULES] Move validation: Blue PlusTwo on Red 8 with active color Red = False
[RULES] Move validation: Yellow 9 on Red 8 with active color Red = False
[RULES] Move validation: Blue 7 on Red 8 with active color Red = False
[RULES] Move validation: Blue 6 on Red 8 with active color Red = False
[RULES] Move validation: Green Stop on Red 8 with active color Red = False
[RULES] Move validation: Wild ChangeColor on Red 8 with active color Red = True
[RULES] Move validation: Yellow Stop on Red 8 with active color Red = False
[NET] Hand display updated: 8 cards, Network=False, Opponent=False
[UI] Updated player hand display: 8 cards
[NET] Hand display updated: 8 cards, Network=False, Opponent=False
[UI] Updated opponent hand display (singleplayer): 8 cards
[SYS] GameStateManager paused
[TURNS] === PAUSING TURN SYSTEM ===
[STATE] Turn state changed: PlayerTurn -> Neutral
[STATE] Turn state changed to Neutral
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[UI] === UPDATING TURN DISPLAY for Neutral ===
[UI] Turn indicator text: 'Game Paused'
[TURN] Turn display updated - button states controlled by strict flow system
[TURNS] Turn state set to Neutral (preserving Human)
[TURNS] Turn system paused successfully
[SYS] TurnManager paused
[AI] === PAUSING AI ===
[AI] Cancelling all AI operations
[AI] All AI operations cancelled
[AI] AI paused successfully
[SYS] Computer AI paused
[SYS] All systems paused successfully
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: DISABLED
[TURN] Play Card button updated: DISABLED
[TURN] Draw Card button updated: DISABLED
[TURN] End Turn button updated: DISABLED
[TURN] Strict button state update complete
[SYS] Game successfully paused
[SYS] GameManager: Game paused
[UI] Pause screen shown as overlay
[UI] Go Home button clicked
[UI] Go Home button clicked FROM PAUSE
[UI] Pause screen overlay hidden
[MP] Staying connected to Photon (was in singleplayer mode)
[UI] Transitioning to main menu with loading screen
[MP] Multiplayer game ready - transitioning to game screen
[SYS] HandManager Player1HandPanel: Awake() called - HandManager initializing...
[SYS] HandManager Player1InfoPanel: Awake() called - HandManager initializing...
[SYS] HandManager Player2InfoPanel: Awake() called - HandManager initializing...
[SYS] HandManager Player2HandPanel: Awake() called - HandManager initializing...
[MP] Starting multiplayer game...
[NET] SYSTEMS RESET: areSystemsInitialized reset for multiplayer mode
[NET] === InitializeMultiPlayerSystems() CALLED ===
[NET] DIAGNOSTIC: areComponentsValidated=True
[NET] DIAGNOSTIC: networkGameManager null check: False
[SYS] Initializing multiplayer game systems...
[NET] Multiplayer mode enabled
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
[SYS] Event handlers disconnected and UI manager deactivated for SinglePlayerUIManager
[UI] Deactivating MultiPlayerUIManager from current game mode
[UI] Disconnecting button events to prevent mode conflicts...
[SYS] Play Card button events disconnected
[SYS] Draw Card button events disconnected
[SYS] End Turn button events disconnected
[SYS] End TAKI Sequence button events disconnected
[SYS] All button events disconnected - mode switch safe
[UI] MultiPlayerUIManager deactivated successfully
[SYS] Event handlers disconnected and UI manager deactivated for MultiPlayerUIManager
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
[TURNS] Turn manager multiplayer mode set to: True
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
[SYS] Event handlers disconnected and UI manager deactivated for SinglePlayerUIManager
[UI] Deactivating MultiPlayerUIManager from current game mode
[UI] Disconnecting button events to prevent mode conflicts...
[SYS] Play Card button events disconnected
[SYS] Draw Card button events disconnected
[SYS] End Turn button events disconnected
[SYS] End TAKI Sequence button events disconnected
[SYS] All button events disconnected - mode switch safe
[UI] MultiPlayerUIManager deactivated successfully
[SYS] Event handlers disconnected and UI manager deactivated for MultiPlayerUIManager
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
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 0
[NET] Player2 UI updated: Opponent Cards: 0
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
[SYS] Clearing player hand to avoid singleplayer contamination
[STATE] Game state reset for new game (including PlusTwo chain and TAKI sequence state)
[TURNS] Turn manager reset
[TURN] === RESETTING SPECIAL CARD STATE ===
[TURN] TURN FLOW STATE RESET (includes special card state)
[SYS] Game initialization state reset - ready for new game
[NET] === STARTING NETWORK GAME WITH DECK INITIALIZATION ===
[NET] PHOTON DEBUG: IsConnected=True
[NET] PHOTON DEBUG: IsConnectedAndReady=True
[NET] PHOTON DEBUG: InRoom=True
[NET] PHOTON DEBUG: CurrentRoom=f000fc9a-e920-405a-abaa-99ed82c856a5
[NET] PHOTON DEBUG: PlayerCount=2
[NET] PHOTON DEBUG: IsMasterClient=True
[NET] PHOTON DEBUG: LocalPlayer ActorNumber=1
[NET] PHOTON DEBUG: MasterClient ActorNumber=1
[NET] PHOTON DEBUG: Players in room:
[NET]   - Player 1:  (Master: True)
[NET]   - Player 2:  (Master: False)
[NET] === INITIALIZING SHARED DECK ===
[NET] DECK INIT DEBUG: PhotonNetwork.IsMasterClient=True
[NET] DECK INIT DEBUG: LocalPlayer.ActorNumber=1
[NET] DECK INIT DEBUG: MasterClient.ActorNumber=1
[NET] DECK INIT DEBUG: _waitingForDeckState=False
[NET] TAKING MASTER PATH: I am Master Client - setting up deck and broadcasting state
[NET] Master client setting up deck - simplified approach
[DECK] Deck Message: New deck shuffled!
[DECK] Shuffled
[DECK] Initialized with 110 cards
[STATE] New game initialized successfully
[NET] SETUP DEBUG: About to draw Player 1 hand
[DECK] Drew card: Wild ChangeColor
[DECK] Drew card: Red 5
[DECK] Drew card: Red Stop
[DECK] Drew card: Red 1
[DECK] Drew card: Red 4
[DECK] Drew card: Green 6
[DECK] Drew card: Green 9
[DECK] Drew card: Blue Stop
[NET] DrawInitialHand DEBUG: Drew 8 cards, First card: Wild ChangeColor, Hand reference: 1873112786
[NET] SETUP DEBUG: About to draw Player 2 hand
[DECK] Drew card: Green 5
[DECK] Drew card: Yellow Plus
[DECK] Drew card: Blue 4
[DECK] Drew card: Yellow 8
[DECK] Drew card: Yellow 1
[DECK] Drew card: Blue PlusTwo
[DECK] Drew card: Yellow 7
[DECK] Drew card: Wild ChangeColor
[NET] DrawInitialHand DEBUG: Drew 8 cards, First card: Green 5, Hand reference: 1981443908
[NET] SETUP DEBUG: player1Hand == player2Hand reference: False
[NET] SETUP DEBUG: player1Hand reference: 1873112786, player2Hand reference: 1981443908
[NET] SETUP DEBUG: player1Hand first card: Wild ChangeColor
[NET] SETUP DEBUG: player2Hand first card: Green 5
[DECK] Drew card: Red ChangeDirection
[DECK] Drew card: Blue 1
[DECK] Top discard card updated: Blue 1
[DECK] Discarded card: Blue 1
[STATE] Starting card: Blue 1
[STATE] Initial setup complete. Player 1: 8 cards, Player 2: 8 cards
[NET] DEFENSIVE COPY: Created playerHand copy in multiplayer mode to prevent reference contamination
[STATE] Active color changed: Wild -> Blue
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 8
[NET] Player2 UI updated: Opponent Cards: 8
[NET] Multiplayer hand sizes updated: Local=8, Opponent=8
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] === UPDATING ALL DISPLAYS (BASE) ===
[UI] === UPDATING TURN DISPLAY for Neutral ===
[UI] Turn indicator text: 'Game Setup'
[TURN] Turn display updated - button states controlled by strict flow system
[UI] ENABLE TAKI DEBUG: EnableEndTakiSequenceButton called with enable=False
[UI] ENABLE TAKI DEBUG: Disabling button (enable=false)
[UI] End TAKI Sequence button DISABLED & HIDDEN
[UI] TAKI sequence status hidden
[UI] Handling active game state
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] Chain status hidden
[RULES] Move validation: Wild ChangeColor on Blue 1 with active color Blue = True
[RULES] Move validation: Red 5 on Blue 1 with active color Blue = False
[RULES] Move validation: Red Stop on Blue 1 with active color Blue = False
[RULES] Move validation: Red 1 on Blue 1 with active color Blue = True
[RULES] Move validation: Red 4 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 6 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 9 on Blue 1 with active color Blue = False
[RULES] Move validation: Blue Stop on Blue 1 with active color Blue = True
[NET] Hand display updated: 8 cards, Network=True, Opponent=False
[UI] Updated player hand display: 8 cards
[NET] Showing 0 opponent cards with privacy mode
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 8
[NET] Player2 UI updated: Opponent Cards: 0
[NET] Multiplayer hand sizes updated: Local=8, Opponent=0
[NET] Opponent hand displayed with privacy: 0 real cards as card backs (PROTECTED)
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
[TURN] Turn changed to Local Player (PlayerType: Human)
[SYS] Game started! Player: 8 cards, Opponent: 8 cards
[NET] DEFENSIVE COPY: Created playerHand copy in multiplayer mode to prevent reference contamination
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 8
[NET] Player2 UI updated: Opponent Cards: 8
[NET] Multiplayer hand sizes updated: Local=8, Opponent=8
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] === UPDATING ALL DISPLAYS (BASE) ===
[UI] === UPDATING TURN DISPLAY for PlayerTurn ===
[UI] Turn indicator text: 'Your Turn'
[TURN] Turn display updated - button states controlled by strict flow system
[UI] ENABLE TAKI DEBUG: EnableEndTakiSequenceButton called with enable=False
[UI] ENABLE TAKI DEBUG: Disabling button (enable=false)
[UI] End TAKI Sequence button DISABLED & HIDDEN
[UI] TAKI sequence status hidden
[UI] Handling active game state
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] Chain status hidden
[RULES] Move validation: Wild ChangeColor on Blue 1 with active color Blue = True
[RULES] Move validation: Red 5 on Blue 1 with active color Blue = False
[RULES] Move validation: Red Stop on Blue 1 with active color Blue = False
[RULES] Move validation: Red 1 on Blue 1 with active color Blue = True
[RULES] Move validation: Red 4 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 6 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 9 on Blue 1 with active color Blue = False
[RULES] Move validation: Blue Stop on Blue 1 with active color Blue = True
[NET] Hand display updated: 8 cards, Network=True, Opponent=False
[UI] Updated player hand display: 8 cards
[NET] Showing 0 opponent cards with privacy mode
[NET] HandManager Player2HandPanel: FORCE CLEARED opponent display for intentional update
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 8
[NET] Player2 UI updated: Opponent Cards: 0
[NET] Multiplayer hand sizes updated: Local=8, Opponent=0
[NET] Opponent hand displayed with privacy: 0 real cards as card backs (PROTECTED)
[NET] Hand display updated (enhanced): 0 cards, Privacy=True
[UI] Updated opponent hand display (multiplayer): 0 cards with privacy
[TURNS] Turn system initialized. First player: Human
[TURNS] Turn started for: Human
[TURN] Turn changed to Local Player (PlayerType: Human)
[TURN] Turn changed to Local Player (PlayerType: Human)
[SYS] Game started! Player: 8 cards, Opponent: 8 cards
[DECK] Deck Message: Starting card: Blue 1
[NET] SetupInitialGame successful: P1=8, P2=8, Start=Blue 1
[NET] Serializing card: Wild ChangeColor -> Wild_ChangeColor
[NET] Serializing card: Red 5 -> Red_5
[NET] Serializing card: Red Stop -> Red_Stop
[NET] Serializing card: Red 1 -> Red_1
[NET] Serializing card: Red 4 -> Red_4
[NET] Serializing card: Green 6 -> Green_6
[NET] Serializing card: Green 9 -> Green_9
[NET] Serializing card: Blue Stop -> Blue_Stop
[NET] Hand serialized: 8 cards -> 69 characters
[NET] Serializing card: Green 5 -> Green_5
[NET] Serializing card: Yellow Plus -> Yellow_Plus
[NET] Serializing card: Blue 4 -> Blue_4
[NET] Serializing card: Yellow 8 -> Yellow_8
[NET] Serializing card: Yellow 1 -> Yellow_1
[NET] Serializing card: Blue PlusTwo -> Blue_PlusTwo
[NET] Serializing card: Yellow 7 -> Yellow_7
[NET] Serializing card: Wild ChangeColor -> Wild_ChangeColor
[NET] Hand serialized: 8 cards -> 83 characters
[NET] === SENDING INITIAL GAME STATE RPC ===
[NET] Starting Card ID: Blue_1
[NET] Draw Pile Count: 93
[NET] Player 1 Hand (serialized): Wild_ChangeColor|Red_5|Red_Stop|Red_1|Red_4|Green_6|Green_9|Blue_Stop
[NET] Player 2 Hand (serialized): Green_5|Yellow_Plus|Blue_4|Yellow_8|Yellow_1|Blue_PlusTwo|Yellow_7|Wild_ChangeColor
[NET] Master Client Actor Number: 1
[NET] Player 1 Hand Size: 8 cards
[NET] Player 2 Hand Size: 8 cards
[NET] === RPC MESSAGE DETAILS LOGGED ===
[NET] DIAGNOSTIC: Before SetupLocalMultiplayerHands - P1 Count=8, P2 Count=8
[NET] DIAGNOSTIC: P1 First card exists: True
[NET] DIAGNOSTIC: P1 First card: Wild ChangeColor
[NET] DIAGNOSTIC: P2 First card exists: True
[NET] DIAGNOSTIC: P2 First card: Green 5
[NET] === MASTER CLIENT HAND ASSIGNMENT DEBUG ===
[NET] Master ActorNumber: 1
[NET] Master should be isPlayer1=True and get player1Hand
[NET] About to call SetupLocalMultiplayerHands with P1=8 cards, P2=8 cards
[NET] Setting up multiplayer hands - simplified approach
[NET] DIAGNOSTIC: Player assignment setup
[NET] DIAGNOSTIC: Local ActorNumber=1
[NET] DIAGNOSTIC: Total players=2
[NET] DIAGNOSTIC: Player[0] ActorNumber=1
[NET] DIAGNOSTIC: Player[1] ActorNumber=2
[NET] DIAGNOSTIC: Input hands - Player1: 8 cards, Player2: 8 cards
[NET] DIAGNOSTIC: Player1Hand[0] is null: False
[NET] DIAGNOSTIC: Player1Hand[0]: Wild ChangeColor
[NET] DIAGNOSTIC: Player2Hand[0] is null: False
[NET] DIAGNOSTIC: Player2Hand[0]: Green 5
[NET] DIAGNOSTIC: isPlayer1=True (Local actor 1 vs First player 1)
[NET] DIAGNOSTIC: After assignment - myHand: 8 cards, opponentHand: 8 cards
[NET] Hand assignment: Local=8 cards, Opponent=8 cards
[NET] DIAGNOSTIC: GameManager.playerHand before clear: 8 cards
[NET] DIAGNOSTIC: About to add 8 cards to GameManager.playerHand
[NET] DIAGNOSTIC: CRITICAL - gameManager.playerHand == myHand reference: False
[NET] DIAGNOSTIC: Created myHandCopy with 8 cards
[NET] DIAGNOSTIC: GameManager.playerHand after clear: 0 cards
[NET] DIAGNOSTIC: myHand after clear - Count: 8
[NET] DIAGNOSTIC: GameManager.playerHand after AddRange: 8 cards
[NET] GameManager playerHand updated: 8 cards
[NET] HandManager Player1HandPanel: Network mode = True
[NET] *** REFERENCE FIX VERIFICATION: Using myHandCopy (8 cards) instead of myHand (8 cards) ***
[RULES] Move validation: Wild ChangeColor on Blue 1 with active color Blue = True
[RULES] Move validation: Red 5 on Blue 1 with active color Blue = False
[RULES] Move validation: Red Stop on Blue 1 with active color Blue = False
[RULES] Move validation: Red 1 on Blue 1 with active color Blue = True
[RULES] Move validation: Red 4 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 6 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 9 on Blue 1 with active color Blue = False
[RULES] Move validation: Blue Stop on Blue 1 with active color Blue = True
[NET] Hand display updated: 8 cards, Network=True, Opponent=False
[NET] Local player hand displayed: 8 cards (per-screen architecture) - FIXED VERSION
[NET] HandManager Player2HandPanel: Enhanced network mode = True
[NET] HandManager Player2HandPanel: Configured for enhanced opponent hand privacy
[NET] Initializing enhanced network hand: Local=False, Cards=8
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 8
[NET] Player2 UI updated: Opponent Cards: 8
[NET] Multiplayer hand sizes updated: Local=8, Opponent=8
[NET] Showing 8 opponent cards with privacy mode
[NET] CardController: Enhanced initialization - Card: Green 5, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow Plus, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 4, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow 8, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow 1, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue PlusTwo, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow 7, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Wild ChangeColor, FaceUp: False, Privacy: True
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 8
[NET] Player2 UI updated: Opponent Cards: 8
[NET] Multiplayer hand sizes updated: Local=8, Opponent=8
[NET] Opponent hand displayed with privacy: 8 real cards as card backs (PROTECTED)
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
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 8
[NET] Player2 UI updated: Opponent Cards: 8
[NET] Multiplayer hand sizes updated: Local=8, Opponent=8
[NET] Multiplayer hands setup complete - simplified approach successful
[NET] Game active state changed to: True
[NET] Game activated after multiplayer hands setup
[NET] === MASTER POST-SETUP DIAGNOSTIC ===
[NET] Master GameManager.playerHand count after setup: 8
[NET] Master GameManager.playerHand first card: Wild ChangeColor
[NET] Updating multiplayer deck display: Draw=93, Discard=1, Top=Blue 1
[NET] DeckUI PileManager status: ASSIGNED
[DECK] Top discard card updated: Blue 1
[NET] Multiplayer deck display updated successfully
[NET] Master deck setup complete - simplified approach successful
[TURN] Starting Player Turn
[TURN] Normal turn flow - no active chain
[RULES] Move validation: Wild ChangeColor on Blue 1 with active color Blue = True
[RULES] Move validation: Red 5 on Blue 1 with active color Blue = False
[RULES] Move validation: Red Stop on Blue 1 with active color Blue = False
[RULES] Move validation: Red 1 on Blue 1 with active color Blue = True
[RULES] Move validation: Red 4 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 6 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 9 on Blue 1 with active color Blue = False
[RULES] Move validation: Blue Stop on Blue 1 with active color Blue = True
[TURN] Player has 3 valid cards, may PLAY or DRAW a card
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: ENABLED
[TURN] DRAW: ENABLED
[TURN] END TURN: DISABLED
[TURN] Play Card button updated: ENABLED
[TURN] Draw Card button updated: ENABLED
[TURN] End Turn button updated: DISABLED
[TURN] Strict button state update complete
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Wild ChangeColor on Blue 1 with active color Blue = True
[RULES] Move validation: Red 5 on Blue 1 with active color Blue = False
[RULES] Move validation: Red Stop on Blue 1 with active color Blue = False
[RULES] Move validation: Red 1 on Blue 1 with active color Blue = True
[RULES] Move validation: Red 4 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 6 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 9 on Blue 1 with active color Blue = False
[RULES] Move validation: Blue Stop on Blue 1 with active color Blue = True
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Wild ChangeColor on Blue 1 with active color Blue = True
[RULES] Move validation: Red 5 on Blue 1 with active color Blue = False
[RULES] Move validation: Red Stop on Blue 1 with active color Blue = False
[RULES] Move validation: Red 1 on Blue 1 with active color Blue = True
[RULES] Move validation: Red 4 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 6 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 9 on Blue 1 with active color Blue = False
[RULES] Move validation: Blue Stop on Blue 1 with active color Blue = True
[TURN] Starting Player Turn
[TURN] Normal turn flow - no active chain
[RULES] Move validation: Wild ChangeColor on Blue 1 with active color Blue = True
[RULES] Move validation: Red 5 on Blue 1 with active color Blue = False
[RULES] Move validation: Red Stop on Blue 1 with active color Blue = False
[RULES] Move validation: Red 1 on Blue 1 with active color Blue = True
[RULES] Move validation: Red 4 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 6 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 9 on Blue 1 with active color Blue = False
[RULES] Move validation: Blue Stop on Blue 1 with active color Blue = True
[TURN] Player has 3 valid cards, may PLAY or DRAW a card
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: ENABLED
[TURN] DRAW: ENABLED
[TURN] END TURN: DISABLED
[TURN] Play Card button updated: ENABLED
[TURN] Draw Card button updated: ENABLED
[TURN] End Turn button updated: DISABLED
[TURN] Strict button state update complete
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Wild ChangeColor on Blue 1 with active color Blue = True
[RULES] Move validation: Red 5 on Blue 1 with active color Blue = False
[RULES] Move validation: Red Stop on Blue 1 with active color Blue = False
[RULES] Move validation: Red 1 on Blue 1 with active color Blue = True
[RULES] Move validation: Red 4 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 6 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 9 on Blue 1 with active color Blue = False
[RULES] Move validation: Blue Stop on Blue 1 with active color Blue = True
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Wild ChangeColor on Blue 1 with active color Blue = True
[RULES] Move validation: Red 5 on Blue 1 with active color Blue = False
[RULES] Move validation: Red Stop on Blue 1 with active color Blue = False
[RULES] Move validation: Red 1 on Blue 1 with active color Blue = True
[RULES] Move validation: Red 4 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 6 on Blue 1 with active color Blue = False
[RULES] Move validation: Green 9 on Blue 1 with active color Blue = False
[RULES] Move validation: Blue Stop on Blue 1 with active color Blue = True
[DECK] PileManager: Mode changed from False to True - recreating visuals
[NET] === TURN 1 BEGINS ===
[NET] Is my turn: True
[NET] First turn initialization complete
