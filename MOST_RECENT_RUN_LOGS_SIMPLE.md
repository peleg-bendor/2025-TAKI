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
[NET] PHOTON DEBUG: CurrentRoom=de7bf9fa-47c4-4775-9ce7-22f954b8567c
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
[DECK] Drew card: Yellow Stop
[DECK] Drew card: Blue 6
[DECK] Drew card: Green 9
[DECK] Drew card: Yellow 5
[DECK] Drew card: Green 3
[DECK] Drew card: Blue 5
[DECK] Drew card: Red Taki
[DECK] Drew card: Blue Stop
[DECK] Drew card: Red 3
[DECK] Drew card: Red Taki
[DECK] Drew card: Red Stop
[DECK] Drew card: Blue 7
[DECK] Drew card: Red 9
[DECK] Drew card: Yellow ChangeDirection
[DECK] Drew card: Wild SuperTaki
[DECK] Drew card: Yellow 3
[DECK] Drew card: Green 1
[DECK] Top discard card updated: Green 1
[DECK] Discarded card: Green 1
[STATE] Starting card: Green 1
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
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 8
[NET] Player2 UI updated: Opponent Cards: 0
[NET] Multiplayer hand sizes updated: Local=8, Opponent=0
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
[RULES] Move validation: Yellow Stop on Green 1 with active color Green = False
[RULES] Move validation: Blue 6 on Green 1 with active color Green = False
[RULES] Move validation: Green 9 on Green 1 with active color Green = True
[RULES] Move validation: Yellow 5 on Green 1 with active color Green = False
[RULES] Move validation: Green 3 on Green 1 with active color Green = True
[RULES] Move validation: Blue 5 on Green 1 with active color Green = False
[RULES] Move validation: Red Taki on Green 1 with active color Green = False
[RULES] Move validation: Blue Stop on Green 1 with active color Green = False
[NET] Hand display updated: 8 cards, Network=True, Opponent=False
[UI] Updated player hand display: 8 cards
[NET] Showing 0 opponent cards with privacy mode
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 8
[NET] Player2 UI updated: Opponent Cards: 0
[NET] Multiplayer hand sizes updated: Local=8, Opponent=0
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
[DECK] Deck Message: Starting card: Green 1
[NET] SetupInitialGame successful: P1=8, P2=8, Start=Green 1
[NET] Serializing card: Yellow Stop -> Yellow_Stop
[NET] Serializing card: Blue 6 -> Blue_6
[NET] Serializing card: Green 9 -> Green_9
[NET] Serializing card: Yellow 5 -> Yellow_5
[NET] Serializing card: Green 3 -> Green_3
[NET] Serializing card: Blue 5 -> Blue_5
[NET] Serializing card: Red Taki -> Red_Taki
[NET] Serializing card: Blue Stop -> Blue_Stop
[NET] Hand serialized: 8 cards -> 69 characters
[NET] Serializing card: Red 3 -> Red_3
[NET] Serializing card: Red Taki -> Red_Taki
[NET] Serializing card: Red Stop -> Red_Stop
[NET] Serializing card: Blue 7 -> Blue_7
[NET] Serializing card: Red 9 -> Red_9
[NET] Serializing card: Yellow ChangeDirection -> Yellow_ChangeDirection
[NET] Serializing card: Wild SuperTaki -> Wild_SuperTaki
[NET] Serializing card: Yellow 3 -> Yellow_3
[NET] Hand serialized: 8 cards -> 83 characters
[NET] === SENDING INITIAL GAME STATE RPC ===
[NET] Starting Card ID: Green_1
[NET] Draw Pile Count: 93
[NET] Player 1 Hand (serialized): Yellow_Stop|Blue_6|Green_9|Yellow_5|Green_3|Blue_5|Red_Taki|Blue_Stop
[NET] Player 2 Hand (serialized): Red_3|Red_Taki|Red_Stop|Blue_7|Red_9|Yellow_ChangeDirection|Wild_SuperTaki|Yellow_3
[NET] Master Client Actor Number: 1
[NET] Player 1 Hand Size: 8 cards
[NET] Player 2 Hand Size: 8 cards
[NET] === RPC MESSAGE DETAILS LOGGED ===
[NET] DIAGNOSTIC: Before SetupLocalMultiplayerHands - P1 Count=8, P2 Count=8
[NET] DIAGNOSTIC: P1 First card exists: True
[NET] DIAGNOSTIC: P2 First card exists: True
[NET] Setting up multiplayer hands - simplified approach
[NET] DIAGNOSTIC: Player assignment setup
[NET] DIAGNOSTIC: Local ActorNumber=1
[NET] DIAGNOSTIC: Total players=2
[NET] DIAGNOSTIC: Player[0] ActorNumber=1
[NET] DIAGNOSTIC: Player[1] ActorNumber=2
[NET] DIAGNOSTIC: Input hands - Player1: 8 cards, Player2: 8 cards
[NET] DIAGNOSTIC: Player1Hand[0] is null: False
[NET] DIAGNOSTIC: Player1Hand[0]: Yellow Stop
[NET] DIAGNOSTIC: Player2Hand[0] is null: False
[NET] DIAGNOSTIC: Player2Hand[0]: Red 3
[NET] DIAGNOSTIC: isPlayer1=True (Local actor 1 vs First player 1)
[NET] DIAGNOSTIC: After assignment - myHand: 8 cards, opponentHand: 8 cards
[NET] Hand assignment: Local=8 cards, Opponent=8 cards
[NET] DIAGNOSTIC: GameManager.playerHand before clear: 8 cards
[NET] DIAGNOSTIC: About to add 8 cards to GameManager.playerHand
[NET] DIAGNOSTIC: CRITICAL - gameManager.playerHand == myHand reference: True
[NET] DIAGNOSTIC: Created myHandCopy with 8 cards
[NET] DIAGNOSTIC: GameManager.playerHand after clear: 0 cards
[NET] DIAGNOSTIC: myHand after clear - Count: 0
[NET] DIAGNOSTIC: GameManager.playerHand after AddRange: 8 cards
[NET] GameManager playerHand updated: 8 cards
[NET] HandManager Player1HandPanel: Network mode = True
[RULES] Move validation: Yellow Stop on Green 1 with active color Green = False
[RULES] Move validation: Blue 6 on Green 1 with active color Green = False
[RULES] Move validation: Green 9 on Green 1 with active color Green = True
[RULES] Move validation: Yellow 5 on Green 1 with active color Green = False
[RULES] Move validation: Green 3 on Green 1 with active color Green = True
[RULES] Move validation: Blue 5 on Green 1 with active color Green = False
[RULES] Move validation: Red Taki on Green 1 with active color Green = False
[RULES] Move validation: Blue Stop on Green 1 with active color Green = False
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
[NET] CardController: Enhanced initialization - Card: Red 3, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red Taki, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red Stop, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 7, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red 9, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow ChangeDirection, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Wild SuperTaki, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow 3, FaceUp: False, Privacy: True
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
[NET] Updating multiplayer deck display: Draw=93, Discard=1, Top=Green 1
[NET] DeckUI PileManager status: ASSIGNED
[DECK] Top discard card updated: Green 1
[NET] Multiplayer deck display updated successfully
[NET] Master deck setup complete - simplified approach successful
[TURN] Starting Player Turn
[TURN] Normal turn flow - no active chain
[RULES] Move validation: Yellow Stop on Green 1 with active color Green = False
[RULES] Move validation: Blue 6 on Green 1 with active color Green = False
[RULES] Move validation: Green 9 on Green 1 with active color Green = True
[RULES] Move validation: Yellow 5 on Green 1 with active color Green = False
[RULES] Move validation: Green 3 on Green 1 with active color Green = True
[RULES] Move validation: Blue 5 on Green 1 with active color Green = False
[RULES] Move validation: Red Taki on Green 1 with active color Green = False
[RULES] Move validation: Blue Stop on Green 1 with active color Green = False
[TURN] Player has 2 valid cards, may PLAY or DRAW a card
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
[RULES] Move validation: Yellow Stop on Green 1 with active color Green = False
[RULES] Move validation: Blue 6 on Green 1 with active color Green = False
[RULES] Move validation: Green 9 on Green 1 with active color Green = True
[RULES] Move validation: Yellow 5 on Green 1 with active color Green = False
[RULES] Move validation: Green 3 on Green 1 with active color Green = True
[RULES] Move validation: Blue 5 on Green 1 with active color Green = False
[RULES] Move validation: Red Taki on Green 1 with active color Green = False
[RULES] Move validation: Blue Stop on Green 1 with active color Green = False
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Yellow Stop on Green 1 with active color Green = False
[RULES] Move validation: Blue 6 on Green 1 with active color Green = False
[RULES] Move validation: Green 9 on Green 1 with active color Green = True
[RULES] Move validation: Yellow 5 on Green 1 with active color Green = False
[RULES] Move validation: Green 3 on Green 1 with active color Green = True
[RULES] Move validation: Blue 5 on Green 1 with active color Green = False
[RULES] Move validation: Red Taki on Green 1 with active color Green = False
[RULES] Move validation: Blue Stop on Green 1 with active color Green = False
[NET] === TURN 1 BEGINS ===
[NET] Is my turn: True
[NET] First turn initialization complete
[UI] Player selected visual card: Red Taki
[RULES] Move validation: Red Taki on Green 1 with active color Green = False
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] Player selected visual card: Green 3
[RULES] Move validation: Green 3 on Green 1 with active color Green = True
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[TURN] === PLAY CARD BUTTON CLICKED ===
[TURN] Button enabled state: True
[TURN] Button interactable: True
[TURN] === PLAY CARD BUTTON CLICKED (MULTIPLAYER) ===
[NET] === MULTIPLAYER PLAY CARD CLICKED ===
[CARD] Attempting to play selected card: Green 3
[NET] === PLAYER 1 FINISHED TURN 1 ===
[NET] Sent card play: Green_3
[NET] Sent card play to network: Green 3
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[CARD] PLAYING CARD WITH STRICT FLOW: Green 3
[RULES] Move validation: Green 3 on Green 1 with active color Green = True
[DECK] Top discard card updated: Green 3
[DECK] Discarded card: Green 3
[RULES] === CALLING HandleSpecialCardEffects WITH SEQUENCE AWARENESS ===
[RULES] === HANDLING SPECIAL CARD EFFECTS for Green 3 ===
[RULES] Card type: Number
[RULES] Card name: Green 3
[RULES] SEQUENCE CONTEXT: In sequence=False, Last card=False
[RULES] SPECIAL EFFECT ACTIVATION: True
[RULES] NUMBER card - no special effects
[RULES] === HandleSpecialCardEffects COMPLETED ===
[RULES] === CALLING LogCardEffectRules ===
[RULES] === CARD EFFECT ANALYSIS: Green 3 ===
[RULES] NUMBER CARD: Green 3
[RULES] RULE: Basic card - no special effects
[RULES] TURN FLOW: Player must END TURN after playing
[RULES] IMPLEMENTATION: Standard single-action turn completion
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[RULES] === CARD EFFECT ANALYSIS COMPLETE ===
[RULES] === LogCardEffectRules COMPLETED ===
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 7
[NET] Player2 UI updated: Opponent Cards: 0
[NET] Multiplayer hand sizes updated: Local=7, Opponent=0
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
[RULES] Move validation: Yellow Stop on Green 3 with active color Green = False
[RULES] Move validation: Blue 6 on Green 3 with active color Green = False
[RULES] Move validation: Green 9 on Green 3 with active color Green = True
[RULES] Move validation: Yellow 5 on Green 3 with active color Green = False
[RULES] Move validation: Blue 5 on Green 3 with active color Green = False
[RULES] Move validation: Red Taki on Green 3 with active color Green = False
[RULES] Move validation: Blue Stop on Green 3 with active color Green = False
[NET] Hand display updated: 7 cards, Network=True, Opponent=False
[UI] Updated player hand display: 7 cards
[NET] Showing 8 opponent cards with privacy mode
[NET] CardController: Enhanced initialization - Card: Red 3, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red Taki, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red Stop, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 7, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red 9, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow ChangeDirection, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Wild SuperTaki, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow 3, FaceUp: False, Privacy: True
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 7
[NET] Player2 UI updated: Opponent Cards: 8
[NET] Multiplayer hand sizes updated: Local=7, Opponent=8
[NET] Opponent hand displayed with privacy: 8 real cards as card backs
[NET] Hand display updated (enhanced): 8 cards, Privacy=True
[UI] Updated opponent hand display (multiplayer): 8 cards with privacy
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Yellow Stop on Green 3 with active color Green = False
[RULES] Move validation: Blue 6 on Green 3 with active color Green = False
[RULES] Move validation: Green 9 on Green 3 with active color Green = True
[RULES] Move validation: Yellow 5 on Green 3 with active color Green = False
[RULES] Move validation: Blue 5 on Green 3 with active color Green = False
[RULES] Move validation: Red Taki on Green 3 with active color Green = False
[RULES] Move validation: Blue Stop on Green 3 with active color Green = False
[TURN] === HANDLING POST-CARD-PLAY TURN FLOW for Green 3 ===
[TURN] NORMAL CARD TURN FLOW - Single action, must end turn
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[TURN] === FORCE ENABLING END TURN BUTTON ===
[TURN] Action was successful - player must now END TURN
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: ENABLED
[TURN] Play Card button updated: DISABLED
[TURN] Draw Card button updated: DISABLED
[TURN] End Turn button updated: ENABLED
[TURN] Strict button state update complete
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[TURN] Normal turn flow: Must END TURN after single action
[TURN] CARD PLAY COMPLETE - Turn flow handled based on card type
[NET] Multiplayer card play completed: Green 3
[NET] === TURN 2 BEGINS ===
[NET] Is my turn: False
[STATE] Turn state changed: PlayerTurn -> ComputerTurn
[STATE] Turn state changed to ComputerTurn
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[UI] === UPDATING TURN DISPLAY for ComputerTurn ===
[UI] Turn indicator text: 'Opponent's Turn'
[TURN] Turn display updated - button states controlled by strict flow system
