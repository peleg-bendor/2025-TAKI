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

		[Header ("Game Rules")]
		[Tooltip ("Maximum number of cards a player can hold")]
		public int maxHandSize = 20;

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
		/// Check if a card can be legally played
		/// </summary>
		/// <param name="cardToPlay">Card player wants to play</param>
		/// <param name="topDiscardCard">Current top card of discard pile</param>
		/// <returns>True if the move is legal</returns>
		public bool IsValidMove (CardData cardToPlay, CardData topDiscardCard) {
			if (cardToPlay == null || topDiscardCard == null) {
				TakiLogger.LogWarning ("Cannot validate move: Null card provided", TakiLogger.LogCategory.Rules);
				return false;
			}

			// Use the CanPlayOn method from CardData
			bool isValid = cardToPlay.CanPlayOn (topDiscardCard, activeColor);

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
		/// Check if current state allows player actions
		/// </summary>
		/// <returns>True if player can take actions</returns>
		public bool CanPlayerAct () {
			return gameStatus == GameStatus.Active &&
				   (turnState == TurnState.PlayerTurn || interactionState == InteractionState.ColorSelection);
		}

		/// <summary>
		/// Check if current state allows computer actions
		/// </summary>
		/// <returns>True if computer can take actions</returns>
		public bool CanComputerAct () {
			return gameStatus == GameStatus.Active && turnState == TurnState.ComputerTurn;
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

			TakiLogger.LogGameState ("Game state reset for new game");
		}

		/// <summary>
		/// Get user-friendly description of current combined state
		/// </summary>
		/// <returns>Description of current game state</returns>
		public string GetStateDescription () {
			// If game is not active, show that first
			if (gameStatus != GameStatus.Active) {
				return gameStatus == GameStatus.GameOver ? "Game Over" : "Game Paused";
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
		
		// Properties for external access using new architecture
		public bool IsGameActive => gameStatus == GameStatus.Active;
		public bool IsPlayerTurn => turnState == TurnState.PlayerTurn;
		public bool IsComputerTurn => turnState == TurnState.ComputerTurn;
		public bool IsGameOver => gameStatus == GameStatus.GameOver;
		public bool IsGamePaused => gameStatus == GameStatus.Paused;
		public bool IsColorSelectionActive => interactionState == InteractionState.ColorSelection;
		public bool IsTakiSequenceActive => interactionState == InteractionState.TakiSequence;
		public bool IsPlusTwoChainActive => interactionState == InteractionState.PlusTwoChain;
		public bool IsNormalGameplay => interactionState == InteractionState.Normal;

		// Combined state checks
		public bool IsPlayerTurnNormal => IsPlayerTurn && IsNormalGameplay;
		public bool IsComputerTurnNormal => IsComputerTurn && IsNormalGameplay;
	}
}