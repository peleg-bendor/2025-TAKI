using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// Manages the current game state using multi-enum architecture
	/// Handles turn state, interaction state, game status, active color, and rules
	/// NO turn management, NO AI logic, NO UI updates
	/// </summary>
	public class GameStateManager : MonoBehaviour {

		[Header ("Game State")]
		[Tooltip ("Whose turn is it currently?")]
		public TurnState turnState = TurnState.Neutral;

		[Tooltip ("What special interaction is happening?")]
		public InteractionState interactionState = InteractionState.Normal;

		[Tooltip ("Overall game status")]
		public GameStatus gameStatus = GameStatus.Active;

		[Header ("Game Properties")]
		[Tooltip ("Active color that must be matched (Wild = no color set yet)")]
		public CardColor activeColor = CardColor.Wild; // Wild = neutral/white initially

		[Tooltip ("Direction of play (for ChangeDirection cards)")]
		public TurnDirection turnDirection = TurnDirection.Clockwise;

		[Header ("PlusTwo Chain State")]
		[Tooltip ("Is a PlusTwo chain currently active")]
		private bool isPlusTwoChainActive = false;

		[Tooltip ("Number of PlusTwo cards played in current chain")]
		private int numberOfChainedPlusTwos = 0;

		[Tooltip ("Player who initiated the current chain")]
		private PlayerType chainInitiator = PlayerType.Human;

		[Header ("Game Rules")]
		[Tooltip ("Maximum number of cards a player can hold")]
		public int maxHandSize = 30;

		// Events for state changes
		public System.Action<TurnState> OnTurnStateChanged;
		public System.Action<InteractionState> OnInteractionStateChanged;
		public System.Action<GameStatus> OnGameStatusChanged;
		public System.Action<CardColor> OnActiveColorChanged;
		public System.Action<TurnDirection> OnTurnDirectionChanged;
		public System.Action<PlayerType> OnGameWon;

		/// <summary>
		/// Change whose turn it is
		/// </summary>
		/// <param name="newTurnState">New turn state</param>
		public void ChangeTurnState (TurnState newTurnState) {
			if (turnState == newTurnState) return;

			TurnState previousState = turnState;
			turnState = newTurnState;

			TakiLogger.LogGameState ($"Turn state changed: {previousState} -> {newTurnState}");
			OnTurnStateChanged?.Invoke (newTurnState);
		}

		/// <summary>
		/// Change what special interaction is happening
		/// </summary>
		/// <param name="newInteractionState">New interaction state</param>
		public void ChangeInteractionState (InteractionState newInteractionState) {
			if (interactionState == newInteractionState) return;

			InteractionState previousState = interactionState;
			interactionState = newInteractionState;

			TakiLogger.LogGameState ($"Interaction state changed: {previousState} -> {newInteractionState}");
			OnInteractionStateChanged?.Invoke (newInteractionState);
		}

		/// <summary>
		/// Change overall game status
		/// </summary>
		/// <param name="newGameStatus">New game status</param>
		public void ChangeGameStatus (GameStatus newGameStatus) {
			if (gameStatus == newGameStatus) return;

			GameStatus previousStatus = gameStatus;
			gameStatus = newGameStatus;

			TakiLogger.LogGameState ($"Game status changed: {previousStatus} -> {newGameStatus}");
			OnGameStatusChanged?.Invoke (newGameStatus);
		}

		/// <summary>
		/// Change the active color (when ChangeColor card is played)
		/// </summary>
		/// <param name="newColor">New active color</param>
		public void ChangeActiveColor (CardColor newColor) {
			if (activeColor == newColor) return;

			CardColor previousColor = activeColor;
			activeColor = newColor;

			TakiLogger.LogGameState ($"Active color changed: {previousColor} -> {newColor}");
			OnActiveColorChanged?.Invoke (newColor);
		}

		/// <summary>
		/// Change turn direction (when ChangeDirection card is played)
		/// </summary>
		public void ChangeTurnDirection () {
			turnDirection = turnDirection == TurnDirection.Clockwise ?
							TurnDirection.CounterClockwise : TurnDirection.Clockwise;

			TakiLogger.LogGameState ($"Turn direction changed to: {turnDirection}");
			OnTurnDirectionChanged?.Invoke (turnDirection);
		}

		/// <summary>
		/// Check if a card can be legally played - ENHANCED with PlusTwo chain awareness
		/// </summary>
		/// <param name="cardToPlay">Card player wants to play</param>
		/// <param name="topDiscardCard">Current top card of discard pile</param>
		/// <returns>True if the move is legal</returns>
		public bool IsValidMove (CardData cardToPlay, CardData topDiscardCard) {
			if (cardToPlay == null || topDiscardCard == null) {
				TakiLogger.LogWarning ("Cannot validate move: Null card provided", TakiLogger.LogCategory.Rules);
				return false;
			}

			// CRITICAL RULE: During PlusTwo chain, only PlusTwo cards are valid
			if (isPlusTwoChainActive && cardToPlay.cardType != CardType.PlusTwo) {
				TakiLogger.LogRules ($"CHAIN RULE: Only PlusTwo cards allowed during chain, blocked {cardToPlay.GetDisplayText ()}");
				return false;
			}

			// Use the normal CanPlayOn method for other validation
			bool isValid = cardToPlay.CanPlayOn (topDiscardCard, activeColor);

			if (isPlusTwoChainActive && isValid) {
				TakiLogger.LogRules ($"CHAIN VALID: {cardToPlay.GetDisplayText ()} can continue chain");
			}

			TakiLogger.LogRules ($"Move validation: {cardToPlay.GetDisplayText ()} on {topDiscardCard.GetDisplayText ()} with active color {activeColor} = {isValid}");
			return isValid;
		}

		/// <summary>
		/// Update active color based on played card
		/// </summary>
		/// <param name="playedCard">Card that was just played</param>
		public void UpdateActiveColorFromCard (CardData playedCard) {
			if (playedCard == null) return;

			// For non-wild cards, update active color to match the card
			if (!playedCard.IsWildCard) {
				ChangeActiveColor (playedCard.color);
			}
			// For wild cards (ChangeColor, SuperTaki), color will be set separately
		}

		/// <summary>
		/// Start a new PlusTwo chain
		/// </summary>
		/// <param name="initiator">Player who started the chain</param>
		public void StartPlusTwoChain (PlayerType initiator) {
			isPlusTwoChainActive = true;
			numberOfChainedPlusTwos = 1;
			chainInitiator = initiator;

			TakiLogger.LogGameState ($"PlusTwo chain started by {initiator}");
			TakiLogger.LogRules ($"CHAIN START: {initiator} plays PlusTwo #1 - opponent must draw 2 or continue");
		}

		/// <summary>
		/// Continue existing PlusTwo chain
		/// </summary>
		public void ContinuePlusTwoChain () {
			numberOfChainedPlusTwos++;
			int drawCount = ChainDrawCount;

			TakiLogger.LogGameState ($"PlusTwo chain continued - now {numberOfChainedPlusTwos} cards, draw count: {drawCount}");
			TakiLogger.LogRules ($"CHAIN CONTINUE: PlusTwo #{numberOfChainedPlusTwos} played - opponent must draw {drawCount} or continue");
		}

		/// <summary>
		/// Break PlusTwo chain
		/// </summary>
		public void BreakPlusTwoChain () {
			int finalDrawCount = ChainDrawCount;
			int finalChainLength = numberOfChainedPlusTwos;

			TakiLogger.LogGameState ($"PlusTwo chain broken - was {finalChainLength} cards ({finalDrawCount} total draw)");
			TakiLogger.LogRules ($"CHAIN BREAK: Chain of {finalChainLength} PlusTwo cards broken by drawing {finalDrawCount} cards");

			isPlusTwoChainActive = false;
			numberOfChainedPlusTwos = 0;
		}

		/// <summary>
		/// Reset chain state for new game
		/// </summary>
		public void ResetPlusTwoChainState () {
			bool wasActive = isPlusTwoChainActive;
			isPlusTwoChainActive = false;
			numberOfChainedPlusTwos = 0;

			if (wasActive) {
				TakiLogger.LogGameState ("PlusTwo chain state reset for new game");
			}
		}

		/// <summary>
		/// Set chain count directly (for pause/resume restoration)
		/// </summary>
		/// <param name="count">Number of chained cards</param>
		public void SetChainCount (int count) {
			numberOfChainedPlusTwos = count;
			TakiLogger.LogGameState ($"Chain count set to {count} (for pause restoration)");
		}

		/// <summary>
		/// Check if current state allows player actions
		/// </summary>
		/// <returns>True if player can take actions</returns>
		public bool CanPlayerAct () {
			// Player can't act if game is paused or game over
			if (gameStatus != GameStatus.Active) {
				return false;
			}

			return turnState == TurnState.PlayerTurn ||
				   interactionState == InteractionState.ColorSelection;
		}

		/// <summary>
		/// Check if current state allows computer actions
		/// </summary>
		/// <returns>True if computer can take actions</returns>
		public bool CanComputerAct () {
			// Computer can't act if game is paused or game over
			if (gameStatus != GameStatus.Active) {
				return false;
			}

			return turnState == TurnState.ComputerTurn;
		}

		/// <summary>
		/// Declare a winner and end the game
		/// </summary>
		/// <param name="winner">The winning player type</param>
		public void DeclareWinner (PlayerType winner) {
			ChangeGameStatus (GameStatus.GameOver);
			ChangeTurnState (TurnState.Neutral);
			ChangeInteractionState (InteractionState.Normal);

			TakiLogger.LogGameState ($"Game Over! Winner: {winner}");
			OnGameWon?.Invoke (winner);
		}

		/// <summary>
		/// Reset game state for a new game
		/// </summary>
		public void ResetGameState () {
			turnState = TurnState.Neutral;
			interactionState = InteractionState.Normal;
			gameStatus = GameStatus.Active;
			activeColor = CardColor.Wild; // No color set initially
			turnDirection = TurnDirection.Clockwise;

			ResetPlusTwoChainState ();

			TakiLogger.LogGameState ("Game state reset for new game (including PlusTwo chain state)");
		}

		/// <summary>
		/// Get user-friendly description of current combined state
		/// </summary>
		/// <returns>Description of current game state</returns>
		public string GetStateDescription () {
			// Check game status first
			switch (gameStatus) {
				case GameStatus.Paused:
					return "Game Paused";
				case GameStatus.GameOver:
					return "Game Over";
				case GameStatus.Active:
					// Continue to check other states
					break;
			}

			// Handle special interactions
			switch (interactionState) {
				case InteractionState.ColorSelection:
					return "Choose a color";
				case InteractionState.TakiSequence:
					return "TAKI sequence - play more cards or close";
				case InteractionState.PlusTwoChain:
					return "Plus Two chain - play +2 or draw cards";
			}

			// Normal gameplay - show whose turn
			switch (turnState) {
				case TurnState.PlayerTurn:
					return "Your turn - play a card or draw";
				case TurnState.ComputerTurn:
					return "Computer's turn";
				case TurnState.Neutral:
					return "Setting up...";
				default:
					return "Unknown state";
			}
		}

		/// <summary>
		/// Get simple turn description for UI
		/// </summary>
		/// <returns>Simple turn description</returns>
		public string GetTurnDescription () {
			switch (turnState) {
				case TurnState.PlayerTurn:
					return "Your Turn";
				case TurnState.ComputerTurn:
					return "Computer's Turn";
				case TurnState.Neutral:
					return "Game Setup";
				default:
					return turnState.ToString ();
			}
		}

		/// <summary>
		/// Pause the game - preserves all state for resumption
		/// </summary>
		public void PauseGame () {
			if (gameStatus == GameStatus.Paused) {
				TakiLogger.LogWarning ("Game is already paused", TakiLogger.LogCategory.GameState);
				return;
			}

			GameStatus previousStatus = gameStatus;
			ChangeGameStatus (GameStatus.Paused);

			TakiLogger.LogGameState ($"Game paused from status: {previousStatus}");
		}

		/// <summary>
		/// Resume the game - restore to active state
		/// </summary>
		public void ResumeGame () {
			if (gameStatus != GameStatus.Paused) {
				TakiLogger.LogWarning ($"Cannot resume game - current status: {gameStatus}", TakiLogger.LogCategory.GameState);
				return;
			}

			ChangeGameStatus (GameStatus.Active);
			TakiLogger.LogGameState ("Game resumed to active state");
		}

		/// <summary>
		/// Check if game can be paused in current state
		/// </summary>
		/// <returns>True if game can be paused</returns>
		public bool CanGameBePaused () {
			// Can pause if game is active and not during critical interactions
			return gameStatus == GameStatus.Active &&
				   interactionState != InteractionState.ColorSelection;
		}

		/// <summary>
		/// Check if game can be resumed from current state
		/// </summary>
		/// <returns>True if game can be resumed</returns>
		public bool CanGameBeResumed () {
			return gameStatus == GameStatus.Paused;
		}

		// Properties for external access using new architecture
		public bool IsGameActive => gameStatus == GameStatus.Active;
		public bool IsPlayerTurn => turnState == TurnState.PlayerTurn;
		public bool IsComputerTurn => turnState == TurnState.ComputerTurn;
		public bool IsGameOver => gameStatus == GameStatus.GameOver;
		public bool IsColorSelectionActive => interactionState == InteractionState.ColorSelection;
		public bool IsTakiSequenceActive => interactionState == InteractionState.TakiSequence;
		public bool IsNormalGameplay => interactionState == InteractionState.Normal;

		// Enhanced state properties
		public bool IsGamePaused => gameStatus == GameStatus.Paused;
		public bool CanPause => CanGameBePaused ();
		public bool CanResume => CanGameBeResumed ();

		// Combined state checks
		public bool IsPlayerTurnNormal => IsPlayerTurn && IsNormalGameplay;
		public bool IsComputerTurnNormal => IsComputerTurn && IsNormalGameplay;

		// Enhanced combined state checks
		public bool IsActivePlayerTurn => gameStatus == GameStatus.Active && turnState == TurnState.PlayerTurn;
		public bool IsActiveComputerTurn => gameStatus == GameStatus.Active && turnState == TurnState.ComputerTurn;
		public bool IsGamePlayable => gameStatus == GameStatus.Active && interactionState == InteractionState.Normal;

		// FIXED: PlusTwo Chain Properties - using private field instead of interaction state
		public bool IsPlusTwoChainActive => isPlusTwoChainActive;
		public int ChainDrawCount => numberOfChainedPlusTwos * 2;
		public int NumberOfChainedCards => numberOfChainedPlusTwos;
		public PlayerType ChainInitiator => chainInitiator;

		// Enhanced state check that includes chain awareness
		public bool IsActivePlayerTurnNormal => gameStatus == GameStatus.Active &&
												turnState == TurnState.PlayerTurn &&
												interactionState == InteractionState.Normal &&
												!isPlusTwoChainActive;

		public bool IsActiveComputerTurnNormal => gameStatus == GameStatus.Active &&
												  turnState == TurnState.ComputerTurn &&
												  interactionState == InteractionState.Normal &&
												  !isPlusTwoChainActive;
	}
}