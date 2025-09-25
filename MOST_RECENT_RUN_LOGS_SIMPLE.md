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
[NET] PHOTON DEBUG: CurrentRoom=a7df479d-7a16-4acb-b357-b4ff627cc07d
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
[DECK] Drew card: Blue Plus
[DECK] Drew card: Green 1
[DECK] Drew card: Wild ChangeColor
[DECK] Drew card: Red Plus
[DECK] Drew card: Yellow Plus
[DECK] Drew card: Green 8
[DECK] Drew card: Green 8
[DECK] Drew card: Blue 8
[DECK] Drew card: Red 5
[DECK] Drew card: Blue Taki
[DECK] Drew card: Blue 9
[DECK] Drew card: Yellow 5
[DECK] Drew card: Green ChangeDirection
[DECK] Drew card: Red PlusTwo
[DECK] Drew card: Red 1
[DECK] Drew card: Yellow 6
[DECK] Drew card: Red 8
[DECK] Top discard card updated: Red 8
[DECK] Discarded card: Red 8
[STATE] Starting card: Red 8
[STATE] Initial setup complete. Player 1: 8 cards, Player 2: 8 cards
[STATE] Active color changed: Wild -> Red
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
[RULES] Move validation: Blue Plus on Red 8 with active color Red = False
[RULES] Move validation: Green 1 on Red 8 with active color Red = False
[RULES] Move validation: Wild ChangeColor on Red 8 with active color Red = True
[RULES] Move validation: Red Plus on Red 8 with active color Red = True
[RULES] Move validation: Yellow Plus on Red 8 with active color Red = False
[RULES] Move validation: Green 8 on Red 8 with active color Red = True
[RULES] Move validation: Green 8 on Red 8 with active color Red = True
[RULES] Move validation: Blue 8 on Red 8 with active color Red = True
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
[DECK] Deck Message: Starting card: Red 8
[NET] SetupInitialGame successful: P1=8, P2=8, Start=Red 8
[NET] Serializing card: Blue Plus -> Blue_Plus
[NET] Serializing card: Green 1 -> Green_1
[NET] Serializing card: Wild ChangeColor -> Wild_ChangeColor
[NET] Serializing card: Red Plus -> Red_Plus
[NET] Serializing card: Yellow Plus -> Yellow_Plus
[NET] Serializing card: Green 8 -> Green_8
[NET] Serializing card: Green 8 -> Green_8
[NET] Serializing card: Blue 8 -> Blue_8
[NET] Hand serialized: 8 cards -> 78 characters
[NET] Serializing card: Red 5 -> Red_5
[NET] Serializing card: Blue Taki -> Blue_Taki
[NET] Serializing card: Blue 9 -> Blue_9
[NET] Serializing card: Yellow 5 -> Yellow_5
[NET] Serializing card: Green ChangeDirection -> Green_ChangeDirection
[NET] Serializing card: Red PlusTwo -> Red_PlusTwo
[NET] Serializing card: Red 1 -> Red_1
[NET] Serializing card: Yellow 6 -> Yellow_6
[NET] Hand serialized: 8 cards -> 80 characters
[NET] === SENDING INITIAL GAME STATE RPC ===
[NET] Starting Card ID: Red_8
[NET] Draw Pile Count: 93
[NET] Player 1 Hand (serialized): Blue_Plus|Green_1|Wild_ChangeColor|Red_Plus|Yellow_Plus|Green_8|Green_8|Blue_8
[NET] Player 2 Hand (serialized): Red_5|Blue_Taki|Blue_9|Yellow_5|Green_ChangeDirection|Red_PlusTwo|Red_1|Yellow_6
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
[NET] DIAGNOSTIC: Player1Hand[0]: Blue Plus
[NET] DIAGNOSTIC: Player2Hand[0] is null: False
[NET] DIAGNOSTIC: Player2Hand[0]: Red 5
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
[RULES] Move validation: Blue Plus on Red 8 with active color Red = False
[RULES] Move validation: Green 1 on Red 8 with active color Red = False
[RULES] Move validation: Wild ChangeColor on Red 8 with active color Red = True
[RULES] Move validation: Red Plus on Red 8 with active color Red = True
[RULES] Move validation: Yellow Plus on Red 8 with active color Red = False
[RULES] Move validation: Green 8 on Red 8 with active color Red = True
[RULES] Move validation: Green 8 on Red 8 with active color Red = True
[RULES] Move validation: Blue 8 on Red 8 with active color Red = True
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
[NET] CardController: Enhanced initialization - Card: Red 5, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue Taki, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 9, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow 5, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Green ChangeDirection, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red PlusTwo, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Red 1, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow 6, FaceUp: False, Privacy: True
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
[NET] Updating multiplayer deck display: Draw=93, Discard=1, Top=Red 8
[NET] DeckUI PileManager status: ASSIGNED
[DECK] Top discard card updated: Red 8
[NET] Multiplayer deck display updated successfully
[NET] Master deck setup complete - simplified approach successful
[TURN] Starting Player Turn
[TURN] Normal turn flow - no active chain
[RULES] Move validation: Blue Plus on Red 8 with active color Red = False
[RULES] Move validation: Green 1 on Red 8 with active color Red = False
[RULES] Move validation: Wild ChangeColor on Red 8 with active color Red = True
[RULES] Move validation: Red Plus on Red 8 with active color Red = True
[RULES] Move validation: Yellow Plus on Red 8 with active color Red = False
[RULES] Move validation: Green 8 on Red 8 with active color Red = True
[RULES] Move validation: Green 8 on Red 8 with active color Red = True
[RULES] Move validation: Blue 8 on Red 8 with active color Red = True
[TURN] Player has 5 valid cards, may PLAY or DRAW a card
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
[RULES] Move validation: Blue Plus on Red 8 with active color Red = False
[RULES] Move validation: Green 1 on Red 8 with active color Red = False
[RULES] Move validation: Wild ChangeColor on Red 8 with active color Red = True
[RULES] Move validation: Red Plus on Red 8 with active color Red = True
[RULES] Move validation: Yellow Plus on Red 8 with active color Red = False
[RULES] Move validation: Green 8 on Red 8 with active color Red = True
[RULES] Move validation: Green 8 on Red 8 with active color Red = True
[RULES] Move validation: Blue 8 on Red 8 with active color Red = True
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Blue Plus on Red 8 with active color Red = False
[RULES] Move validation: Green 1 on Red 8 with active color Red = False
[RULES] Move validation: Wild ChangeColor on Red 8 with active color Red = True
[RULES] Move validation: Red Plus on Red 8 with active color Red = True
[RULES] Move validation: Yellow Plus on Red 8 with active color Red = False
[RULES] Move validation: Green 8 on Red 8 with active color Red = True
[RULES] Move validation: Green 8 on Red 8 with active color Red = True
[RULES] Move validation: Blue 8 on Red 8 with active color Red = True
[NET] === TURN 1 BEGINS ===
[NET] Is my turn: True
[NET] First turn initialization complete
[UI] Player selected visual card: Green 8
[RULES] Move validation: Green 8 on Red 8 with active color Red = True
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[TURN] === PLAY CARD BUTTON CLICKED ===
[TURN] Button enabled state: True
[TURN] Button interactable: True
[TURN] === PLAY CARD BUTTON CLICKED (MULTIPLAYER) ===
[NET] === MULTIPLAYER PLAY CARD CLICKED ===
[CARD] Attempting to play selected card: Green 8
"Exception: Write failed. Custom type not found: TakiGame.NetworkMoveData
ExitGames.Client.Photon.Protocol18.WriteCustomType (ExitGames.Client.Photon.StreamBuffer stream, System.Object value, System.Boolean writeType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:741)
ExitGames.Client.Photon.Protocol18.Write (ExitGames.Client.Photon.StreamBuffer stream, System.Object value, ExitGames.Client.Photon.Protocol18+GpType gpType, System.Boolean writeType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:99)
ExitGames.Client.Photon.Protocol18.Write (ExitGames.Client.Photon.StreamBuffer stream, System.Object value, System.Boolean writeType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:29)
ExitGames.Client.Photon.Protocol18.WriteHashtable (ExitGames.Client.Photon.StreamBuffer stream, System.Object value, System.Boolean writeType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:434)
ExitGames.Client.Photon.Protocol18.Write (ExitGames.Client.Photon.StreamBuffer stream, System.Object value, ExitGames.Client.Photon.Protocol18+GpType gpType, System.Boolean writeType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:132)
ExitGames.Client.Photon.Protocol18.Write (ExitGames.Client.Photon.StreamBuffer stream, System.Object value, System.Boolean writeType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:29)
ExitGames.Client.Photon.Protocol18.WriteParameterTable (ExitGames.Client.Photon.StreamBuffer stream, ExitGames.Client.Photon.ParameterDictionary parameters) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:241)
ExitGames.Client.Photon.Protocol18.SerializeOperationRequest (ExitGames.Client.Photon.StreamBuffer stream, System.Byte operationCode, ExitGames.Client.Photon.ParameterDictionary parameters, System.Boolean setType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:276)
ExitGames.Client.Photon.PeerBase.SerializeOperationToMessage (System.Byte opCode, ExitGames.Client.Photon.ParameterDictionary parameters, ExitGames.Client.Photon.EgMessageType messageType, System.Boolean encrypt) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/PeerBase.cs:688)
ExitGames.Client.Photon.PhotonPeer.SendOperation (System.Byte operationCode, ExitGames.Client.Photon.ParameterDictionary operationParameters, ExitGames.Client.Photon.SendOptions sendOptions) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/PhotonPeer.cs:1871)
Photon.Realtime.LoadBalancingPeer.OpRaiseEvent (System.Byte eventCode, System.Object customEventContent, Photon.Realtime.RaiseEventOptions raiseEventOptions, ExitGames.Client.Photon.SendOptions sendOptions) (at Assets/Photon/PhotonRealtime/Code/LoadBalancingPeer.cs:1024)
Photon.Realtime.LoadBalancingClient.OpRaiseEvent (System.Byte eventCode, System.Object customEventContent, Photon.Realtime.RaiseEventOptions raiseEventOptions, ExitGames.Client.Photon.SendOptions sendOptions) (at Assets/Photon/PhotonRealtime/Code/LoadBalancingClient.cs:2320)
Photon.Pun.PhotonNetwork.RaiseEvent (System.Byte eventCode, System.Object eventContent, Photon.Realtime.RaiseEventOptions raiseEventOptions, ExitGames.Client.Photon.SendOptions sendOptions) (at Assets/Photon/PhotonUnityNetworking/Code/PhotonNetwork.cs:2328)
Photon.Pun.UtilityScripts.PunTurnManager.SendMove (System.Object move, System.Boolean finished) (at Assets/Photon/PhotonUnityNetworking/UtilityScripts/TurnBased/PunTurnManager.cs:181)
TakiGame.NetworkGameManager.SendCardPlay (TakiGame.CardData card) (at Assets/Scripts/Multiplayer/NetworkGameManager.cs:623)
TakiGame.GameManager.SendLocalCardPlayToNetwork (TakiGame.CardData cardToPlay) (at Assets/Scripts/Core/GameManager.cs:3580)
TakiGame.GameManager.OnPlayCardButtonClickedMultiplayer () (at Assets/Scripts/Core/GameManager.cs:3651)
TakiGame.GameManager.OnPlayCardButtonClicked () (at Assets/Scripts/Core/GameManager.cs:1330)
TakiGame.BaseGameplayUIManager.<ConnectButtonEvents>b__50_0 () (at Assets/Scripts/UI/BaseGameplayUIManager.cs:88)
UnityEngine.Events.InvokableCall.Invoke () (at <2d8783c7af0442318483a199a473c55b>:0)
UnityEngine.Events.UnityEvent.Invoke () (at <2d8783c7af0442318483a199a473c55b>:0)
UnityEngine.UI.Button.Press () (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/UI/Core/Button.cs:70)
UnityEngine.UI.Button.OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/UI/Core/Button.cs:114)
UnityEngine.EventSystems.ExecuteEvents.Execute (UnityEngine.EventSystems.IPointerClickHandler handler, UnityEngine.EventSystems.BaseEventData eventData) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/ExecuteEvents.cs:57)
UnityEngine.EventSystems.ExecuteEvents.Execute[T] (UnityEngine.GameObject target, UnityEngine.EventSystems.BaseEventData eventData, UnityEngine.EventSystems.ExecuteEvents+EventFunction`1[T1] functor) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/ExecuteEvents.cs:272)
UnityEngine.EventSystems.EventSystem:Update() (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/EventSystem.cs:530)"
[UI] Player deselected card
[TURN] === DRAW CARD BUTTON CLICKED ===
[TURN] Button enabled state: True
[TURN] Button interactable: True
[TURN] === DRAW CARD BUTTON CLICKED (MULTIPLAYER) ===
[NET] === MULTIPLAYER DRAW CARD CLICKED ===
"Exception: Write failed. Custom type not found: TakiGame.NetworkMoveData
ExitGames.Client.Photon.Protocol18.WriteCustomType (ExitGames.Client.Photon.StreamBuffer stream, System.Object value, System.Boolean writeType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:741)
ExitGames.Client.Photon.Protocol18.Write (ExitGames.Client.Photon.StreamBuffer stream, System.Object value, ExitGames.Client.Photon.Protocol18+GpType gpType, System.Boolean writeType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:99)
ExitGames.Client.Photon.Protocol18.Write (ExitGames.Client.Photon.StreamBuffer stream, System.Object value, System.Boolean writeType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:29)
ExitGames.Client.Photon.Protocol18.WriteHashtable (ExitGames.Client.Photon.StreamBuffer stream, System.Object value, System.Boolean writeType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:434)
ExitGames.Client.Photon.Protocol18.Write (ExitGames.Client.Photon.StreamBuffer stream, System.Object value, ExitGames.Client.Photon.Protocol18+GpType gpType, System.Boolean writeType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:132)
ExitGames.Client.Photon.Protocol18.Write (ExitGames.Client.Photon.StreamBuffer stream, System.Object value, System.Boolean writeType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:29)
ExitGames.Client.Photon.Protocol18.WriteParameterTable (ExitGames.Client.Photon.StreamBuffer stream, ExitGames.Client.Photon.ParameterDictionary parameters) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:241)
ExitGames.Client.Photon.Protocol18.SerializeOperationRequest (ExitGames.Client.Photon.StreamBuffer stream, System.Byte operationCode, ExitGames.Client.Photon.ParameterDictionary parameters, System.Boolean setType) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/Protocol18Write.cs:276)
ExitGames.Client.Photon.PeerBase.SerializeOperationToMessage (System.Byte opCode, ExitGames.Client.Photon.ParameterDictionary parameters, ExitGames.Client.Photon.EgMessageType messageType, System.Boolean encrypt) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/PeerBase.cs:688)
ExitGames.Client.Photon.PhotonPeer.SendOperation (System.Byte operationCode, ExitGames.Client.Photon.ParameterDictionary operationParameters, ExitGames.Client.Photon.SendOptions sendOptions) (at D:/Dev/Work/photon-dotnet-sdk/PhotonDotNet/PhotonPeer.cs:1871)
Photon.Realtime.LoadBalancingPeer.OpRaiseEvent (System.Byte eventCode, System.Object customEventContent, Photon.Realtime.RaiseEventOptions raiseEventOptions, ExitGames.Client.Photon.SendOptions sendOptions) (at Assets/Photon/PhotonRealtime/Code/LoadBalancingPeer.cs:1024)
Photon.Realtime.LoadBalancingClient.OpRaiseEvent (System.Byte eventCode, System.Object customEventContent, Photon.Realtime.RaiseEventOptions raiseEventOptions, ExitGames.Client.Photon.SendOptions sendOptions) (at Assets/Photon/PhotonRealtime/Code/LoadBalancingClient.cs:2320)
Photon.Pun.PhotonNetwork.RaiseEvent (System.Byte eventCode, System.Object eventContent, Photon.Realtime.RaiseEventOptions raiseEventOptions, ExitGames.Client.Photon.SendOptions sendOptions) (at Assets/Photon/PhotonUnityNetworking/Code/PhotonNetwork.cs:2328)
Photon.Pun.UtilityScripts.PunTurnManager.SendMove (System.Object move, System.Boolean finished) (at Assets/Photon/PhotonUnityNetworking/UtilityScripts/TurnBased/PunTurnManager.cs:181)
TakiGame.NetworkGameManager.SendCardDraw () (at Assets/Scripts/Multiplayer/NetworkGameManager.cs:638)
TakiGame.GameManager.SendLocalCardDrawToNetwork () (at Assets/Scripts/Core/GameManager.cs:3600)
TakiGame.GameManager.OnDrawCardButtonClickedMultiplayer () (at Assets/Scripts/Core/GameManager.cs:3715)
TakiGame.GameManager.OnDrawCardButtonClicked () (at Assets/Scripts/Core/GameManager.cs:1381)
TakiGame.BaseGameplayUIManager.<ConnectButtonEvents>b__50_1 () (at Assets/Scripts/UI/BaseGameplayUIManager.cs:108)
UnityEngine.Events.InvokableCall.Invoke () (at <2d8783c7af0442318483a199a473c55b>:0)
UnityEngine.Events.UnityEvent.Invoke () (at <2d8783c7af0442318483a199a473c55b>:0)
UnityEngine.UI.Button.Press () (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/UI/Core/Button.cs:70)
UnityEngine.UI.Button.OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/UI/Core/Button.cs:114)
UnityEngine.EventSystems.ExecuteEvents.Execute (UnityEngine.EventSystems.IPointerClickHandler handler, UnityEngine.EventSystems.BaseEventData eventData) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/ExecuteEvents.cs:57)
UnityEngine.EventSystems.ExecuteEvents.Execute[T] (UnityEngine.GameObject target, UnityEngine.EventSystems.BaseEventData eventData, UnityEngine.EventSystems.ExecuteEvents+EventFunction`1[T1] functor) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/ExecuteEvents.cs:272)
UnityEngine.EventSystems.EventSystem:Update() (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/EventSystem.cs:530)"
