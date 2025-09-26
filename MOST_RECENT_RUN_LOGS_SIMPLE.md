[UI] Player selected visual card: Yellow ChangeDirection
[RULES] Move validation: Yellow ChangeDirection on Yellow 8 with active color Yellow = True
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[TURN] === PLAY CARD BUTTON CLICKED ===
[TURN] Button enabled state: True
[TURN] Button interactable: True
[TURN] === PLAY CARD BUTTON CLICKED (MULTIPLAYER) ===
[NET] === MULTIPLAYER PLAY CARD CLICKED ===
[CARD] Attempting to play selected card: Yellow ChangeDirection
[NET] === PLAYER 2 MADE MOVE IN TURN 4 ===
[NET] Sent card play: Yellow_ChangeDirection
[NET] Sent card play to network: Yellow ChangeDirection
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[CARD] PLAYING CARD WITH STRICT FLOW: Yellow ChangeDirection
[RULES] Move validation: Yellow ChangeDirection on Yellow 8 with active color Yellow = True
[DECK] Top discard card updated: Yellow ChangeDirection
[DECK] Discarded card: Yellow ChangeDirection
[RULES] === CALLING HandleSpecialCardEffects WITH SEQUENCE AWARENESS ===
[RULES] === HANDLING SPECIAL CARD EFFECTS for Yellow ChangeDirection ===
[RULES] Card type: ChangeDirection
[RULES] Card name: Yellow ChangeDirection
[RULES] SEQUENCE CONTEXT: In sequence=False, Last card=False
[RULES] SPECIAL EFFECT ACTIVATION: True
[RULES] CHANGE DIRECTION card effect - Turn direction changes
[RULES] === ENTERED HandleChangeDirectionCardEffect METHOD ===
[RULES] === ROUTING TO MULTIPLAYER DIRECTION EFFECT ===
[NET] === MULTIPLAYER CHANGE DIRECTION CARD EFFECT ===
[STATE] Turn direction changed to: CounterClockwise
[NET] Direction changed locally from Clockwise to CounterClockwise
[NET] === PLAYER 2 MADE MOVE IN TURN 4 ===
[NET] Sent direction change
[NET] Sent direction change to remote player: CounterClockwise
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[SYS] GetActiveUI() called - useNewUIArchitecture: isMultiplayerMode: True
[SYS]   - singlePlayerUI: ASSIGNED (SinglePlayerUIManager)
[SYS]   - multiPlayerUI: ASSIGNED (MultiPlayerUIManager)
[SYS] GetActiveUI() returning multiPlayerUI: MultiPlayerUIManager
[NET] Multiplayer direction change processed - opponent will be synchronized
[RULES] === HandleSpecialCardEffects COMPLETED ===
[RULES] === CALLING LogCardEffectRules ===
[RULES] === CARD EFFECT ANALYSIS: Yellow ChangeDirection ===
[RULES] CHANGE DIRECTION CARD: Turn Direction Reversal
[RULES] RULE: Reverses turn direction (Clockwise <-> CounterClockwise)
[RULES] TURN FLOW: Normal turn completion after direction change
[RULES] 2-PLAYER NOTE: Direction change is informational only
[RULES] IMPLEMENTATION STATUS: FULLY IMPLEMENTED in Phase 7
[RULES] - HandleChangeDirectionCardEffect() manages direction change
[RULES] - GameStateManager.ChangeTurnDirection() integration
[RULES] - Clear player feedback with before/after direction display
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
[NET] Player1 UI updated: Your Cards: 6
[NET] Player2 UI updated: Opponent Cards: 0
[NET] Multiplayer hand sizes updated: Local=6, Opponent=0
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
[RULES] Move validation: Blue PlusTwo on Yellow ChangeDirection with active color Yellow = False
[RULES] Move validation: Blue 4 on Yellow ChangeDirection with active color Yellow = False
[RULES] Move validation: Red 7 on Yellow ChangeDirection with active color Yellow = False
[RULES] Move validation: Yellow Plus on Yellow ChangeDirection with active color Yellow = True
[RULES] Move validation: Wild SuperTaki on Yellow ChangeDirection with active color Yellow = True
[RULES] Move validation: Red 5 on Yellow ChangeDirection with active color Yellow = False
[NET] Hand display updated: 6 cards, Network=True, Opponent=False
[UI] Updated player hand display: 6 cards
[NET] Showing 8 opponent cards with privacy mode
[NET] HandManager Player2HandPanel: FORCE CLEARED opponent display for intentional update
[NET] CardController: Enhanced initialization - Card: Blue ChangeDirection, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Wild ChangeColor, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue Stop, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow 1, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 8, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Yellow 9, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Wild ChangeColor, FaceUp: False, Privacy: True
[NET] CardController: Enhanced initialization - Card: Blue 1, FaceUp: False, Privacy: True
[NET] DIAGNOSTIC: player1HandSizeText null check: False
[NET] DIAGNOSTIC: player2HandSizeText null check: False
[NET] Player1 UI updated: Your Cards: 6
[NET] Player2 UI updated: Opponent Cards: 8
[NET] Multiplayer hand sizes updated: Local=6, Opponent=8
[NET] Opponent hand displayed with privacy: 8 real cards as card backs (PROTECTED)
[NET] Hand display updated (enhanced): 8 cards, Privacy=True
[UI] Updated opponent hand display (multiplayer): 8 cards with privacy
[UI] REFRESHING PLAYER HAND STATES
[RULES] Move validation: Blue PlusTwo on Yellow ChangeDirection with active color Yellow = False
[RULES] Move validation: Blue 4 on Yellow ChangeDirection with active color Yellow = False
[RULES] Move validation: Red 7 on Yellow ChangeDirection with active color Yellow = False
[RULES] Move validation: Yellow Plus on Yellow ChangeDirection with active color Yellow = True
[RULES] Move validation: Wild SuperTaki on Yellow ChangeDirection with active color Yellow = True
[RULES] Move validation: Red 5 on Yellow ChangeDirection with active color Yellow = False
[TURN] === HANDLING POST-CARD-PLAY TURN FLOW for Yellow ChangeDirection ===
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
[NET] Multiplayer card play completed: Yellow ChangeDirection
[TURN] === END TURN BUTTON CLICKED ===
[TURN] Button enabled state: True
[TURN] Button interactable: True
[TURN] IMMEDIATELY disabling all buttons after END TURN click
[TURN] === UPDATING STRICT BUTTON STATES ===
[TURN] PLAY: DISABLED
[TURN] DRAW: DISABLED
[TURN] END TURN: DISABLED
[TURN] Play Card button updated: DISABLED
[TURN] Draw Card button updated: DISABLED
[TURN] End Turn button updated: DISABLED
[TURN] Strict button state update complete
[TURN] === END TURN BUTTON CLICKED (MULTIPLAYER) ===
[NET] === MULTIPLAYER END TURN CLICKED ===
[NET] === PLAYER 2 FINISHED TURN 4 ===
[NET] Sent end turn
[NET] Sent end turn to network
[TURN] ENDING PLAYER TURN - STRICT FLOW WITH SPECIAL CARDS
[TURN] === RESETTING SPECIAL CARD STATE ===
[TURN] TURN FLOW STATE RESET (includes special card state)
[TURN] Normal turn end - switching to computer turn
[TURNS] Turn ended for: Computer
[TURNS] Switching from Computer to Human
[TURNS] Turn started for: Human
[TURN] Turn changed to Local Player (PlayerType: Human)
[NET] Multiplayer end turn completed
[NET] === TURN 5 BEGINS ===
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
