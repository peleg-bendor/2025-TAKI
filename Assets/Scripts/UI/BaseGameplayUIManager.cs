using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TakiGame {
    /// <summary>
    /// Base class for screen-specific UI managers - contains shared functionality
    /// Solves architecture problem: Single GameplayUIManager couldn't handle both SinglePlayer and MultiPlayer screen hierarchies
    /// </summary>
    public abstract class BaseGameplayUIManager : MonoBehaviour {

        [Header("Color Settings")]
        [Tooltip("Colors to use for current color indicator")]
        public Color redColor = Color.red;
        public Color blueColor = Color.blue;
        public Color greenColor = Color.green;
        public Color yellowColor = Color.yellow;
        public Color wildColor = Color.white;

        // Events for external systems
        public System.Action OnPlayCardClicked;
        public System.Action OnDrawCardClicked;
        public System.Action OnEndTurnClicked;
        public System.Action<CardColor> OnColorSelected;
        public System.Action OnEndTakiSequenceClicked;

        // Button state tracking
        protected bool playButtonEnabled = false;
        protected bool drawButtonEnabled = false;
        protected bool endTurnButtonEnabled = false;

        // Abstract properties - implemented by derived classes
        public abstract TextMeshProUGUI TurnIndicatorText { get; }
        public abstract Image CurrentColorIndicator { get; }
        public abstract Button PlayCardButton { get; }
        public abstract Button DrawCardButton { get; }
        public abstract Button EndTurnButton { get; }
        public abstract Button PauseButton { get; }
        public abstract TextMeshProUGUI Player1HandSizeText { get; }
        public abstract TextMeshProUGUI Player2HandSizeText { get; }
        public abstract TextMeshProUGUI PlayerMessageText { get; }
        public abstract TextMeshProUGUI ComputerMessageText { get; }
        public abstract GameObject ColorSelectionPanel { get; }
        public abstract Button SelectRedButton { get; }
        public abstract Button SelectBlueButton { get; }
        public abstract Button SelectGreenButton { get; }
        public abstract Button SelectYellowButton { get; }
        public abstract TextMeshProUGUI ChainStatusText { get; }
        public abstract Button EndTakiSequenceButton { get; }
        public abstract TextMeshProUGUI TakiSequenceStatusText { get; }

        public abstract GameObject DrawPilePanel { get; }
        public abstract TextMeshProUGUI DrawPileCountText { get; }
        public abstract GameObject DiscardPilePanel { get; }
        public abstract TextMeshProUGUI DiscardPileCountText { get; }

        protected virtual void Start() {
            // Check if this UI manager should be active based on game mode
            if (!ShouldBeActive()) {
                TakiLogger.LogSystem($"New UI Architecture - {GetType().Name} DISABLED (not active for current game mode)");
                gameObject.SetActive(false);
                return;
            }

            TakiLogger.LogInfo($"New UI Architecture - {GetType().Name} starting", TakiLogger.LogCategory.UI);
            ConnectButtonEvents();
            SetupInitialState();
        }

        #region Button Event Management

        protected virtual void ConnectButtonEvents() {
            TakiLogger.LogUI("Connecting button events with STRICT FLOW validation...");

            if (PlayCardButton != null) {
                PlayCardButton.onClick.AddListener(() => {
                    TakiLogger.LogTurnFlow("=== PLAY CARD BUTTON CLICKED ===");
                    if (!playButtonEnabled) {
                        TakiLogger.LogWarning("PLAY CARD clicked but button should be disabled!", TakiLogger.LogCategory.TurnFlow);
                        ShowComputerMessage("Cannot play card right now!");
                        return;
                    }
                    OnPlayCardClicked?.Invoke();
                });
            }

            if (DrawCardButton != null) {
                DrawCardButton.onClick.AddListener(() => {
                    TakiLogger.LogTurnFlow("=== DRAW CARD BUTTON CLICKED ===");
                    if (!drawButtonEnabled) {
                        TakiLogger.LogWarning("DRAW CARD clicked but button should be disabled!", TakiLogger.LogCategory.TurnFlow);
                        ShowComputerMessage("Cannot draw card right now!");
                        return;
                    }
                    OnDrawCardClicked?.Invoke();
                });
            }

            if (EndTurnButton != null) {
                EndTurnButton.onClick.AddListener(() => {
                    TakiLogger.LogTurnFlow("=== END TURN BUTTON CLICKED ===");
                    if (!endTurnButtonEnabled) {
                        TakiLogger.LogWarning("END TURN clicked but button should be disabled!", TakiLogger.LogCategory.TurnFlow);
                        ShowComputerMessage("You must take an action first!");
                        return;
                    }
                    TakiLogger.LogTurnFlow("IMMEDIATELY disabling all buttons after END TURN click");
                    UpdateStrictButtonStates(false, false, false);
                    OnEndTurnClicked?.Invoke();
                });
            }

            if (EndTakiSequenceButton != null) {
                EndTakiSequenceButton.onClick.AddListener(() => {
                    TakiLogger.LogTurnFlow("=== END TAKI SEQUENCE BUTTON CLICKED ===");
                    if (!EndTakiSequenceButton.interactable) {
                        TakiLogger.LogWarning("END TAKI SEQUENCE clicked but button should be disabled!", TakiLogger.LogCategory.TurnFlow);
                        ShowComputerMessage("Cannot end sequence right now!");
                        return;
                    }
                    OnEndTakiSequenceClicked?.Invoke();
                });
            }

            // Color selection buttons
            if (SelectRedButton != null) {
                SelectRedButton.onClick.AddListener(() => SelectColor(CardColor.Red));
            }
            if (SelectBlueButton != null) {
                SelectBlueButton.onClick.AddListener(() => SelectColor(CardColor.Blue));
            }
            if (SelectGreenButton != null) {
                SelectGreenButton.onClick.AddListener(() => SelectColor(CardColor.Green));
            }
            if (SelectYellowButton != null) {
                SelectYellowButton.onClick.AddListener(() => SelectColor(CardColor.Yellow));
            }

            TakiLogger.LogSystem("All button events connected with strict flow validation");
        }

        protected virtual void SetupInitialState() {
            if (ColorSelectionPanel != null) {
                ColorSelectionPanel.SetActive(false);
            }

            if (CurrentColorIndicator != null) {
                CurrentColorIndicator.color = wildColor;
            }

            UpdateStrictButtonStates(false, false, false);
        }

        #endregion

        #region Button State Management

        public virtual void UpdateStrictButtonStates(bool enablePlay, bool enableDraw, bool enableEndTurn) {
            TakiLogger.LogTurnFlow("=== UPDATING STRICT BUTTON STATES ===");
            TakiLogger.LogTurnFlow($"PLAY: {(enablePlay ? "ENABLED" : "DISABLED")}");
            TakiLogger.LogTurnFlow($"DRAW: {(enableDraw ? "ENABLED" : "DISABLED")}");
            TakiLogger.LogTurnFlow($"END TURN: {(enableEndTurn ? "ENABLED" : "DISABLED")}");

            playButtonEnabled = enablePlay;
            drawButtonEnabled = enableDraw;
            endTurnButtonEnabled = enableEndTurn;

            if (PlayCardButton != null) {
                PlayCardButton.interactable = enablePlay;
            }

            if (DrawCardButton != null) {
                DrawCardButton.interactable = enableDraw;
            }

            if (EndTurnButton != null) {
                EndTurnButton.interactable = enableEndTurn;
            }

            if (PauseButton != null) {
                PauseButton.interactable = true;
            }
        }

        public virtual void ForceEnableEndTurn() {
            TakiLogger.LogTurnFlow("=== FORCE ENABLING END TURN BUTTON ===");
            UpdateStrictButtonStates(false, false, true);
        }

        #endregion

        #region Display Updates

        public virtual void UpdateTurnDisplay(TurnState turnState) {
            TakiLogger.LogUI($"=== UPDATING TURN DISPLAY for {turnState} ===", TakiLogger.LogLevel.Debug);

            if (TurnIndicatorText != null) {
                string turnMessage = GetTurnMessage(turnState);
                TurnIndicatorText.text = turnMessage;
            }
        }

        public virtual void UpdateActiveColorDisplay(CardColor activeColor) {
            if (CurrentColorIndicator != null) {
                CurrentColorIndicator.color = GetColorForCardColor(activeColor);
            }
        }

        public abstract void UpdateHandSizeDisplay(int player1HandSize, int player2HandSize);

        protected virtual string GetTurnMessage(TurnState turnState) {
            if (Application.isPlaying) {
                GameStateManager gameState = FindObjectOfType<GameStateManager>();
                if (gameState != null) {
                    if (gameState.gameStatus == GameStatus.Paused) {
                        return "Game Paused";
                    } else if (gameState.gameStatus == GameStatus.GameOver) {
                        return "Game Over";
                    }
                }
            }

            switch (turnState) {
                case TurnState.PlayerTurn:
                    return "Your Turn";
                case TurnState.ComputerTurn:
                    return "Computer's Turn";
                case TurnState.Neutral:
                    return "Game Setup";
                default:
                    return "Unknown State";
            }
        }

        protected virtual Color GetColorForCardColor(CardColor cardColor) {
            switch (cardColor) {
                case CardColor.Red:
                    return redColor;
                case CardColor.Blue:
                    return blueColor;
                case CardColor.Green:
                    return greenColor;
                case CardColor.Yellow:
                    return yellowColor;
                default:
                    return wildColor;
            }
        }

        #endregion

        #region Message System

        public virtual void ShowPlayerMessage(string message) {
            if (PlayerMessageText != null) {
                PlayerMessageText.text = message;
            }
        }

        public virtual void ShowComputerMessage(string message) {
            if (ComputerMessageText != null) {
                ComputerMessageText.text = message;
            }
        }

        public virtual void ShowPlayerMessageTimed(string message, float duration = 3.0f) {
            if (PlayerMessageText != null) {
                PlayerMessageText.text = message;
                TakiLogger.LogUI($"Player message: '{message}' (duration: {duration}s)");

                if (duration > 0) {
                    CancelInvoke(nameof(ClearPlayerMessage));
                    Invoke(nameof(ClearPlayerMessage), duration);
                }
            }
        }

        public virtual void ShowComputerMessageTimed(string message, float duration = 3.0f) {
            if (ComputerMessageText != null) {
                ComputerMessageText.text = message;
                TakiLogger.LogUI($"Computer message: '{message}' (duration: {duration}s)");

                if (duration > 0) {
                    CancelInvoke(nameof(ClearComputerMessage));
                    Invoke(nameof(ClearComputerMessage), duration);
                }
            }
        }

        protected virtual void ClearPlayerMessage() {
            if (PlayerMessageText != null) {
                PlayerMessageText.text = "";
            }
        }

        protected virtual void ClearComputerMessage() {
            if (ComputerMessageText != null) {
                ComputerMessageText.text = "";
            }
        }

        #endregion

        #region Color Selection

        public virtual void ShowColorSelection(bool show) {
            if (ColorSelectionPanel != null) {
                ColorSelectionPanel.SetActive(show);
            }

            if (show) {
                TakiLogger.LogUI("Color selection active - disabling all action buttons");
                UpdateStrictButtonStates(false, false, true);
            }
        }

        protected virtual void SelectColor(CardColor selectedColor) {
            UpdateActiveColorDisplay(selectedColor);
            OnColorSelected?.Invoke(selectedColor);
            TakiLogger.LogUI($"Player selected color: {selectedColor} (panel stays visible)");
            ShowPlayerMessage($"Color set to {selectedColor} - click END TURN when ready!");
        }

        #endregion

        #region Special Card Effects

        public virtual void ShowPlusTwoChainStatus(int chainCount, int accumulatedDraw, bool isPlayerTurn) {
            if (ChainStatusText != null) {
                string statusMessage = isPlayerTurn 
                    ? $"PlusTwo Chain: {chainCount} cards -> Draw {accumulatedDraw} or play PlusTwo"
                    : $"PlusTwo Chain: {chainCount} cards -> AI must respond";
                
                ChainStatusText.text = statusMessage;
            }
        }

        public virtual void HidePlusTwoChainStatus() {
            if (ChainStatusText != null) {
                ChainStatusText.text = "";
            }
        }

        public virtual void EnableEndTakiSequenceButton(bool enable) {
            if (EndTakiSequenceButton != null) {
                bool shouldEnable = enable;

                if (enable) {
                    GameStateManager gameState = FindObjectOfType<GameStateManager>();
                    if (gameState != null && gameState.IsInTakiSequence) {
                        shouldEnable = (gameState.TakiSequenceInitiator == PlayerType.Human);
                    }
                }

                EndTakiSequenceButton.interactable = shouldEnable;
                EndTakiSequenceButton.gameObject.SetActive(shouldEnable);
            }
        }

        public virtual void ShowTakiSequenceStatus(CardColor sequenceColor, int cardCount, bool isPlayerTurn) {
            if (TakiSequenceStatusText != null) {
                GameStateManager gameState = FindObjectOfType<GameStateManager>();
                string statusMessage;
                
                if (gameState != null && gameState.IsInTakiSequence) {
                    PlayerType initiator = gameState.TakiSequenceInitiator;
                    statusMessage = initiator == PlayerType.Human
                        ? $"Your TAKI Sequence: {cardCount} cards -> Play {sequenceColor} cards or End Sequence"
                        : $"AI TAKI Sequence: {cardCount} cards -> AI playing {sequenceColor} cards";
                } else {
                    statusMessage = isPlayerTurn
                        ? $"TAKI Sequence: {cardCount} cards -> Play {sequenceColor} cards or End Sequence"
                        : $"TAKI Sequence: {cardCount} cards -> AI playing {sequenceColor} cards";
                }

                TakiSequenceStatusText.text = statusMessage;
                TakiSequenceStatusText.gameObject.SetActive(true);
            }
        }

        public virtual void HideTakiSequenceStatus() {
            if (TakiSequenceStatusText != null) {
                TakiSequenceStatusText.text = "";
                TakiSequenceStatusText.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Game State Management

        public virtual void UpdateAllDisplays(TurnState turnState, GameStatus gameStatus, InteractionState interactionState, CardColor activeColor) {
            TakiLogger.LogUI("=== UPDATING ALL DISPLAYS (BASE) ===", TakiLogger.LogLevel.Debug);

            UpdateTurnDisplay(turnState);
            UpdateActiveColorDisplay(activeColor);

            if (interactionState == InteractionState.ColorSelection) {
                ShowColorSelection(true);
            } else {
                ShowColorSelection(false);
            }

            if (interactionState == InteractionState.TakiSequence) {
                GameStateManager gameState = FindObjectOfType<GameStateManager>();
                bool shouldEnableButton = false;

                if (gameState != null && gameState.IsInTakiSequence) {
                    shouldEnableButton = (gameState.TakiSequenceInitiator == PlayerType.Human) &&
                                        (gameState.IsPlayerTurn);
                }

                EnableEndTakiSequenceButton(shouldEnableButton);

                if (gameState != null && TakiSequenceStatusText != null) {
                    bool isPlayerTurn = gameState.IsPlayerTurn;
                    int cardCount = gameState.NumberOfSequenceCards;
                    CardColor sequenceColor = gameState.TakiSequenceColor;
                    ShowTakiSequenceStatus(sequenceColor, cardCount, isPlayerTurn);
                }
            } else {
                EnableEndTakiSequenceButton(false);
                HideTakiSequenceStatus();
            }

            switch (gameStatus) {
                case GameStatus.Paused:
                    HandlePausedState();
                    break;
                case GameStatus.GameOver:
                    HandleGameOverState();
                    break;
                case GameStatus.Active:
                    HandleActiveState(turnState);
                    break;
            }
        }

        protected virtual void HandlePausedState() {
            UpdateStrictButtonStates(false, false, false);
            EnableEndTakiSequenceButton(false);
            ShowPlayerMessage("Game Paused");
            ShowComputerMessage("Game is paused");

            if (PauseButton != null) {
                PauseButton.interactable = true;
            }
        }

        protected virtual void HandleGameOverState() {
            UpdateStrictButtonStates(false, false, false);
            EnableEndTakiSequenceButton(false);

            if (PauseButton != null) {
                PauseButton.interactable = false;
            }
        }

        protected virtual void HandleActiveState(TurnState turnState) {
            if (PauseButton != null) {
                PauseButton.interactable = true;
            }
        }

        #endregion

        #region Properties

        public bool PlayButtonEnabled => playButtonEnabled;
        public bool DrawButtonEnabled => drawButtonEnabled;
        public bool EndTurnButtonEnabled => endTurnButtonEnabled;
        public bool EndTakiSequenceButtonEnabled => EndTakiSequenceButton != null && EndTakiSequenceButton.interactable;
        public bool IsColorSelectionActive => ColorSelectionPanel != null && ColorSelectionPanel.activeSelf;

        public virtual string GetButtonStateSummary() {
            return $"Play:{(playButtonEnabled ? "ON" : "OFF")}, Draw:{(drawButtonEnabled ? "ON" : "OFF")}, EndTurn:{(endTurnButtonEnabled ? "ON" : "OFF")}, EndSequence:{(EndTakiSequenceButtonEnabled ? "ON" : "OFF")}";
        }

        #endregion

        #region Deck Pile Management

        public virtual void UpdateDrawPileCount(int count) {
            if (DrawPileCountText != null) {
                DrawPileCountText.text = count.ToString();
            }
        }

        public virtual void UpdateDiscardPileCount(int count) {
            if (DiscardPileCountText != null) {
                DiscardPileCountText.text = count.ToString();
            }
        }

        public virtual void UpdateAllPileCounts(int drawCount, int discardCount) {
            UpdateDrawPileCount(drawCount);
            UpdateDiscardPileCount(discardCount);
            TakiLogger.LogUI($"Pile counts updated - Draw: {drawCount}, Discard: {discardCount}");
        }

        #endregion

        #region Game Mode Detection

        /// <summary>
        /// Determines if this UI manager should be active based on the current game mode
        /// Prevents duplicate event handlers by ensuring only the appropriate UI manager is active
        /// </summary>
        /// <returns>True if this UI manager should be active for the current game mode</returns>
        protected virtual bool ShouldBeActive() {
            // Find the GameManager to check current mode
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null) {
                TakiLogger.LogWarning($"Cannot determine game mode - GameManager not found. Defaulting {GetType().Name} to ACTIVE", TakiLogger.LogCategory.System);
                return true;
            }

            bool isMultiplayerMode = gameManager.IsMultiplayerMode;
            bool isThisSinglePlayerManager = this is SinglePlayerUIManager;
            bool isThisMultiPlayerManager = this is MultiPlayerUIManager;

            // SinglePlayerUIManager should only be active in single-player mode
            if (isThisSinglePlayerManager) {
                bool shouldBeActive = !isMultiplayerMode;
                TakiLogger.LogSystem($"SinglePlayerUIManager activity check: isMultiplayerMode={isMultiplayerMode}, shouldBeActive={shouldBeActive}");
                return shouldBeActive;
            }

            // MultiPlayerUIManager should only be active in multiplayer mode
            if (isThisMultiPlayerManager) {
                bool shouldBeActive = isMultiplayerMode;
                TakiLogger.LogSystem($"MultiPlayerUIManager activity check: isMultiplayerMode={isMultiplayerMode}, shouldBeActive={shouldBeActive}");
                return shouldBeActive;
            }

            // Unknown UI manager type - default to active with warning
            TakiLogger.LogWarning($"Unknown UI manager type: {GetType().Name}. Defaulting to ACTIVE", TakiLogger.LogCategory.System);
            return true;
        }

        #endregion

        #region Debug Methods

        [ContextMenu("Log UI State")]
        public virtual void LogCompleteUIState() {
            TakiLogger.LogDiagnostics("=== BASE UI STATE DEBUG ===");
            TakiLogger.LogDiagnostics($"Turn indicator: '{(TurnIndicatorText != null ? TurnIndicatorText.text : "NULL")}'");
            TakiLogger.LogDiagnostics($"Player message: '{(PlayerMessageText != null ? PlayerMessageText.text : "NULL")}'");
            TakiLogger.LogDiagnostics($"Computer message: '{(ComputerMessageText != null ? ComputerMessageText.text : "NULL")}'");
            TakiLogger.LogDiagnostics($"Color selection active: {IsColorSelectionActive}");
            TakiLogger.LogDiagnostics($"Button States: {GetButtonStateSummary()}");
        }

        #endregion
    }
}