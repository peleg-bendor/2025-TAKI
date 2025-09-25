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
[SYS] In multiplayer, hands are populated from network data and should NOT be cleared
[STATE] Game state reset for new game (including PlusTwo chain and TAKI sequence state)
[TURNS] Turn manager reset
[TURN] === RESETTING SPECIAL CARD STATE ===
[TURN] TURN FLOW STATE RESET (includes special card state)
[SYS] Game initialization state reset - ready for new game
[NET] === STARTING NETWORK GAME WITH DECK INITIALIZATION ===
[NET] PHOTON DEBUG: IsConnected=True
[NET] PHOTON DEBUG: IsConnectedAndReady=True
[NET] PHOTON DEBUG: InRoom=True
[NET] PHOTON DEBUG: CurrentRoom=c2b524d0-169f-4a77-b3d4-f803eea151ce
[NET] PHOTON DEBUG: PlayerCount=2
[NET] PHOTON DEBUG: IsMasterClient=False
[NET] PHOTON DEBUG: LocalPlayer ActorNumber=2
[NET] PHOTON DEBUG: MasterClient ActorNumber=1
[NET] PHOTON DEBUG: Players in room:
[NET]   - Player 1:  (Master: True)
[NET]   - Player 2:  (Master: False)
[NET] === INITIALIZING SHARED DECK ===
[NET] DECK INIT DEBUG: PhotonNetwork.IsMasterClient=False
[NET] DECK INIT DEBUG: LocalPlayer.ActorNumber=2
[NET] DECK INIT DEBUG: MasterClient.ActorNumber=1
[NET] DECK INIT DEBUG: _waitingForDeckState=False
[NET] TAKING CLIENT PATH: I am Client - waiting for initial game state from master
[NET] DECK INIT DEBUG: _waitingForDeckState set to True
[NET] === RECEIVED INITIAL GAME STATE RPC ===
[NET] Starting Card ID: Blue_3
[NET] Draw Pile Count: 93
[NET] Player 1 Hand (serialized): Blue_9|Yellow_1|Red_5|Blue_5|Green_1|Red_PlusTwo|Blue_6|Wild_ChangeColor
[NET] Player 2 Hand (serialized): Red_Stop|Yellow_ChangeDirection|Green_Plus|Yellow_Stop|Red_Taki|Red_9|Green_Plus|Yellow_Taki
[NET] Master Client Actor: 1
[NET] Local Player Actor: 2
[NET] === RPC MESSAGE RECEIVED DETAILS LOGGED ===
[NET] RPC DEBUG: _waitingForDeckState=True
[NET] RPC DEBUG: PhotonNetwork.IsMasterClient=False
[NET] RPC DEBUG: sender masterActor=1, local ActorNumber=2
[NET] Deserializing hand from: Blue_9|Yellow_1|Red_5|Blue_5|Green_1|Red_PlusTwo|Blue_6|Wild_ChangeColor
[NET] Split into 8 card IDs
[NET] Deserialized card: Blue_9 -> Blue 9
[NET] Deserialized card: Yellow_1 -> Yellow 1
[NET] Deserialized card: Red_5 -> Red 5
[NET] Deserialized card: Blue_5 -> Blue 5
[NET] Deserialized card: Green_1 -> Green 1
[NET] Deserialized card: Red_PlusTwo -> Red PlusTwo
[NET] Deserialized card: Blue_6 -> Blue 6
[NET] Deserialized card: Wild_ChangeColor -> Wild ChangeColor
[NET] Deserialized hand: 8 cards from 8 IDs
[NET] Deserializing hand from: Red_Stop|Yellow_ChangeDirection|Green_Plus|Yellow_Stop|Red_Taki|Red_9|Green_Plus|Yellow_Taki
[NET] Split into 8 card IDs
[NET] Deserialized card: Red_Stop -> Red Stop
[NET] Deserialized card: Yellow_ChangeDirection -> Yellow ChangeDirection
[NET] Deserialized card: Green_Plus -> Green Plus
[NET] Deserialized card: Yellow_Stop -> Yellow Stop
[NET] Deserialized card: Red_Taki -> Red Taki
[NET] Deserialized card: Red_9 -> Red 9
[NET] Deserialized card: Green_Plus -> Green Plus
[NET] Deserialized card: Yellow_Taki -> Yellow Taki
[NET] Deserialized hand: 8 cards from 8 IDs
[NET] Applying received game state with simplified approach
[DECK] Deck Message: New deck shuffled!
[DECK] Shuffled
[DECK] Initialized with 110 cards
[NET] Network deck initialized with 110 cards
[NET] Network deck initialized with 110 cards
[DECK] Top discard card updated: Blue 3
[DECK] Discarded card: Blue 3
[NET] Starting card placed: Blue 3
[STATE] Active color changed: Wild -> Blue
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[NET] COLOR SYNC: Active color set to Blue from starting card
[NET] DECK SYNC: Adjusting draw pile from 110 to 93 to match master
[NET] Syncing draw pile count to master's state: 93
[NET] Removed 17 cards from draw pile: 110 -> 93
[NET] DECK SYNC: Draw pile count synchronized to 93
[NET] Setting up multiplayer hands - simplified approach
[NET] DIAGNOSTIC: Player assignment setup
[NET] DIAGNOSTIC: Local ActorNumber=2
[NET] DIAGNOSTIC: Total players=2
[NET] DIAGNOSTIC: Player[0] ActorNumber=1
[NET] DIAGNOSTIC: Player[1] ActorNumber=2
[NET] DIAGNOSTIC: Input hands - Player1: 8 cards, Player2: 8 cards
[NET] DIAGNOSTIC: Player1Hand[0] is null: False
[NET] DIAGNOSTIC: Player1Hand[0]: Blue 9
[NET] DIAGNOSTIC: Player2Hand[0] is null: False
[NET] DIAGNOSTIC: Player2Hand[0]: Red Stop
[NET] DIAGNOSTIC: isPlayer1=False (Local actor 2 vs First player 1)
[NET] DIAGNOSTIC: After assignment - myHand: 8 cards, opponentHand: 8 cards
[NET] Hand assignment: Local=8 cards, Opponent=8 cards
[NET] DIAGNOSTIC: GameManager.playerHand before clear: 0 cards
[NET] DIAGNOSTIC: About to add 8 cards to GameManager.playerHand
[NET] DIAGNOSTIC: CRITICAL - gameManager.playerHand == myHand reference: False
[NET] DIAGNOSTIC: Created myHandCopy with 8 cards
[NET] DIAGNOSTIC: GameManager.playerHand after clear: 0 cards
[NET] DIAGNOSTIC: myHand after clear - Count: 8
[NET] DIAGNOSTIC: GameManager.playerHand after AddRange: 8 cards
[NET] GameManager playerHand updated: 8 cards
[NET] HandManager Player1HandPanel: Network mode = True
[RULES] Move validation: Red Stop on Blue 3 with active color Blue = False
[RULES] Move validation: Yellow ChangeDirection on Blue 3 with active color Blue = False
[RULES] Move validation: Green Plus on Blue 3 with active color Blue = False
[RULES] Move validation: Yellow Stop on Blue 3 with active color Blue = False
[RULES] Move validation: Red Taki on Blue 3 with active color Blue = False
[RULES] Move validation: Red 9 on Blue 3 with active color Blue = False
[RULES] Move validation: Green Plus on Blue 3 with active color Blue = False
[RULES] Move validation: Yellow Taki on Blue 3 with active color Blue = False
[RULES] HandManager Player1HandPanel: No playable cards found (0/8)
[NET] Hand display updated: 8 cards, Network=True, Opponent=False
[NET] Local player hand displayed: 8 cards (per-screen architecture)
[NET] HandManager Player2HandPanel: Enhanced network mode = True
[NET] HandManager Player2HandPanel: Configured for enhanced opponent hand privacy
[NET] Initializing enhanced network hand: Local=False, Cards=8
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 8
[NET] Player2 UI updated: Opponent Cards: 8
[NET] Multiplayer hand sizes updated: Local=8, Opponent=8
[NET] Showing 8 opponent cards with privacy mode
[NET] CardController: Enhanced initialization - Card: Blue 9, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow 1, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red 5, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 5, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Green 1, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red PlusTwo, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 6, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Wild ChangeColor, FaceUp: False, Privacy: True
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 8
[NET] Player2 UI updated: Opponent Cards: 8
[NET] Multiplayer hand sizes updated: Local=8, Opponent=8
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
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 8
[NET] Player2 UI updated: Opponent Cards: 8
[NET] Multiplayer hand sizes updated: Local=8, Opponent=8
[NET] Multiplayer hands setup complete - simplified approach successful
[NET] Updating multiplayer deck display: Draw=93, Discard=1, Top=Blue 3
[NET] DeckUI PileManager status: ASSIGNED
[DECK] Top discard card updated: Blue 3
[NET] Multiplayer deck display updated successfully
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[NET] Game state applied successfully with simplified approach
[NET] Client deck initialization complete with actual cards
[NET] === TURN 1 BEGINS ===
[NET] Is my turn: False
[STATE] Turn state changed: Neutral -> ComputerTurn
[STATE] Turn state changed to ComputerTurn
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] === UPDATING TURN DISPLAY for ComputerTurn ===
[UI] Turn indicator text: 'Opponent's Turn'
[TURN] Turn display updated - button states controlled by strict flow system
[NET] First turn initialization complete
