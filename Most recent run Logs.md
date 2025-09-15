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
[DECK] Drew card: Yellow 1
[DECK] Drew card: Yellow 1
[DECK] Drew card: Blue Stop
[DECK] Drew card: Green PlusTwo
[DECK] Drew card: Blue 7
[DECK] Drew card: Wild ChangeColor
[DECK] Drew card: Yellow 3
[DECK] Drew card: Green 3
[DECK] Drew card: Green Stop
[DECK] Drew card: Red ChangeDirection
[DECK] Drew card: Yellow 5
[DECK] Drew card: Wild SuperTaki
[DECK] Drew card: Red 4
[DECK] Drew card: Red 3
[DECK] Drew card: Blue Stop
[DECK] Drew card: Green Taki
[DECK] Drew card: Red Plus
[DECK] Drew card: Blue ChangeDirection
[DECK] Drew card: Green 4
[DECK] Discarded card: Green 4
[STATE] Starting card: Green 4
[STATE] Initial setup complete. Player 1: 8 cards, Player 2: 8 cards
[AI] AI received 8 cards. Hand size: 8
[STATE] Active color changed: Wild -> Green
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
[DECK] Deck Message: Starting card: Green 4
[NET] SetupInitialGame successful: P1=8, P2=8, Start=Green 4
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
[RULES] Move validation: Yellow 1 on Green 4 with active color Green = False
[RULES] Move validation: Yellow 1 on Green 4 with active color Green = False
[RULES] Move validation: Blue Stop on Green 4 with active color Green = False
[RULES] Move validation: Green PlusTwo on Green 4 with active color Green = True
[RULES] Move validation: Blue 7 on Green 4 with active color Green = False
[RULES] Move validation: Wild ChangeColor on Green 4 with active color Green = True
[RULES] Move validation: Yellow 3 on Green 4 with active color Green = False
[RULES] Move validation: Green 3 on Green 4 with active color Green = True
[TURN] Player has 3 valid cards, may PLAY or DRAW a card
[SYS] GetActiveUI() called - useNewUIArchitecture: True, isMultiplayerMode: False
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning singlePlayerUI: SinglePlayerUIManager
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: ENABLED
[TURN] DRAW: ENABLED
[TURN] END TURN: DISABLED
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Yellow 1 on Green 4 with active color Green = False
[RULES] Move validation: Yellow 1 on Green 4 with active color Green = False
[RULES] Move validation: Blue Stop on Green 4 with active color Green = False
[RULES] Move validation: Green PlusTwo on Green 4 with active color Green = True
[RULES] Move validation: Blue 7 on Green 4 with active color Green = False
[RULES] Move validation: Wild ChangeColor on Green 4 with active color Green = True
[RULES] Move validation: Yellow 3 on Green 4 with active color Green = False
[RULES] Move validation: Green 3 on Green 4 with active color Green = True
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Yellow 1 on Green 4 with active color Green = False
[RULES] Move validation: Yellow 1 on Green 4 with active color Green = False
[RULES] Move validation: Blue Stop on Green 4 with active color Green = False
[RULES] Move validation: Green PlusTwo on Green 4 with active color Green = True
[RULES] Move validation: Blue 7 on Green 4 with active color Green = False
[RULES] Move validation: Wild ChangeColor on Green 4 with active color Green = True
[RULES] Move validation: Yellow 3 on Green 4 with active color Green = False
[RULES] Move validation: Green 3 on Green 4 with active color Green = True
[UI] Player selected visual card: Green 3
[RULES] Move validation: Green 3 on Green 4 with active color Green = True
[TURN] === PLAY CARD BUTTON CLICKED ===
[TURN] === PLAY CARD BUTTON CLICKED ===
[CARD] Attempting to play selected card: Green 3
[CARD] PLAYING CARD WITH STRICT FLOW (PHASE 8B): Green 3
[RULES] Move validation: Green 3 on Green 4 with active color Green = True
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
[RULES] === CARD EFFECT ANALYSIS COMPLETE ===
[RULES] === LogCardEffectRules COMPLETED ===
[UI] === UPDATING ALL DISPLAYS (FIXED SEQUENCE CONTROL) ===
[UI] === UPDATING TURN DISPLAY for PlayerTurn ===
[TURN] Turn display updated - button states controlled by strict flow system
[UI] End TAKI Sequence button DISABLED & HIDDEN (Human sequences only)
[UI] TAKI sequence status hidden
[UI] Handling active game state
[UI] All displays updated (with enhanced TAKI sequence support)
[UI] Chain status hidden
[RULES] Move validation: Yellow 1 on Green 3 with active color Green = False
[RULES] Move validation: Yellow 1 on Green 3 with active color Green = False
[RULES] Move validation: Blue Stop on Green 3 with active color Green = False
[RULES] Move validation: Green PlusTwo on Green 3 with active color Green = True
[RULES] Move validation: Blue 7 on Green 3 with active color Green = False
[RULES] Move validation: Wild ChangeColor on Green 3 with active color Green = True
[RULES] Move validation: Yellow 3 on Green 3 with active color Green = True
[NET] Hand display updated: 7 cards, Network=False, Opponent=False
[NET] Hand display updated: 8 cards, Network=False, Opponent=False
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Yellow 1 on Green 3 with active color Green = False
[RULES] Move validation: Yellow 1 on Green 3 with active color Green = False
[RULES] Move validation: Blue Stop on Green 3 with active color Green = False
[RULES] Move validation: Green PlusTwo on Green 3 with active color Green = True
[RULES] Move validation: Blue 7 on Green 3 with active color Green = False
[RULES] Move validation: Wild ChangeColor on Green 3 with active color Green = True
[RULES] Move validation: Yellow 3 on Green 3 with active color Green = True
[TURN] === HANDLING POST-CARD-PLAY TURN FLOW for Green 3 ===
[TURN] NORMAL CARD TURN FLOW - Single action, must end turn
[TURN] === FORCE ENABLING END TURN BUTTON ===
[TURN] Action was successful - player must now END TURN
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: ENABLED
[TURN] Strict button state update complete
[TURN] Normal turn flow: Must END TURN after single action
[TURN] CARD PLAY COMPLETE - Turn flow handled based on card type
[TURN] === END TURN BUTTON CLICKED ===
[TURN] END TURN clicked but button should be disabled!

Assets\Scripts\Core\GameManager.cs(278,20): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ResetUIForNewGame' and no accessible extension method 'ResetUIForNewGame' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(340,21): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ResetUIForNewGame' and no accessible extension method 'ResetUIForNewGame' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(425,20): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowOpponentAction' and no accessible extension method 'ShowOpponentAction' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(459,20): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowOpponentAction' and no accessible extension method 'ShowOpponentAction' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(605,21): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'UpdateTurnDisplayMultiplayer' and no accessible extension method 'UpdateTurnDisplayMultiplayer' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Managers\GameEndManager.cs(100,31): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowWinnerAnnouncement' and no accessible extension method 'ShowWinnerAnnouncement' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(1292,21): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSequenceProgressMessage' and no accessible extension method 'ShowSequenceProgressMessage' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(1423,21): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Multiplayer\MultiplayerGameManager.cs(362,31): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowDeckSyncStatus' and no accessible extension method 'ShowDeckSyncStatus' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Multiplayer\MultiplayerGameManager.cs(468,32): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'UpdateTurnDisplayMultiplayer' and no accessible extension method 'UpdateTurnDisplayMultiplayer' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2423,20): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSequenceProgressMessage' and no accessible extension method 'ShowSequenceProgressMessage' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2773,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2802,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2826,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2851,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowImmediateFeedback' and no accessible extension method 'ShowImmediateFeedback' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2859,24): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2920,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2927,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2951,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(2958,23): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowSpecialCardEffect' and no accessible extension method 'ShowSpecialCardEffect' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(3142,21): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'ShowWinnerAnnouncement' and no accessible extension method 'ShowWinnerAnnouncement' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\GameManager.cs(3686,19): error CS1061: 'BaseGameplayUIManager' does not contain a definition for 'UpdateButtonStates' and no accessible extension method 'UpdateButtonStates' accepting a first argument of type 'BaseGameplayUIManager' could be found (are you missing a using directive or an assembly reference?)

