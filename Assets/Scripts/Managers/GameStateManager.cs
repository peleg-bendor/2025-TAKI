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

		[Header ("TAKI Sequence State - Phase 8B")]
		[Tooltip ("Is a TAKI sequence currently active")]
		private bool isInTakiSequence = false;

		[Tooltip ("Color required for TAKI sequence cards")]
		private CardColor takiSequenceColor = CardColor.Wild;

		[Tooltip ("Last card played in current sequence")]
		private CardData lastCardPlayedInSequence = null;

		[Tooltip ("Number of cards played in current sequence")]
		private int numberOfSequenceCards = 0;

		[Tooltip ("Player who initiated the current sequence")]
		private PlayerType takiSequenceInitiator = PlayerType.Human;

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

		// PHASE 8B: Events for TAKI sequence changes
		public System.Action<CardColor, PlayerType> OnTakiSequenceStarted;
		public System.Action<int> OnTakiSequenceCardAdded;
		public System.Action OnTakiSequenceEnded;

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
		/// SIMPLIFIED: Change interaction state with straightforward sequence protection
		/// NO special ColorSelection handling - just protect TakiSequence from interruption
		/// </summary>
		/// <param name="newInteractionState">New interaction state</param>
		public void ChangeInteractionState (InteractionState newInteractionState) {
			if (interactionState == newInteractionState) return;

			InteractionState previousState = interactionState;

			// SIMPLE RULE: TakiSequence can only transition to Normal
			// All other state changes happen AFTER sequence ends
			if (interactionState == InteractionState.TakiSequence &&
				newInteractionState != InteractionState.Normal) {

				TakiLogger.LogWarning ($"BLOCKED: TakiSequence can only transition to Normal, attempted {newInteractionState}", TakiLogger.LogCategory.GameState);
				return;
			}

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
		/// CRITICAL FIX: Enhanced StartTakiSequence with validation
		/// </summary>
		/// <param name="sequenceColor">Color required for sequence cards</param>
		/// <param name="initiator">Player who started the sequence</param>
		public void StartTakiSequence (CardColor sequenceColor, PlayerType initiator) {
			// CRITICAL VALIDATION: Prevent overlapping sequences
			if (isInTakiSequence) {
				TakiLogger.LogWarning ($"Cannot start new sequence - already in sequence initiated by {takiSequenceInitiator}", TakiLogger.LogCategory.GameState);
				return;
			}

			// CRITICAL VALIDATION: Ensure initiator matches current turn
			if ((initiator == PlayerType.Human && turnState != TurnState.PlayerTurn) ||
				(initiator == PlayerType.Computer && turnState != TurnState.ComputerTurn)) {
				TakiLogger.LogWarning ($"Cannot start sequence - initiator {initiator} doesn't match turn state {turnState}", TakiLogger.LogCategory.GameState);
				return;
			}

			isInTakiSequence = true;
			takiSequenceColor = sequenceColor;
			numberOfSequenceCards = 0;
			takiSequenceInitiator = initiator;
			lastCardPlayedInSequence = null;

			// Change to TakiSequence interaction state
			ChangeInteractionState (InteractionState.TakiSequence);

			TakiLogger.LogGameState ($"TAKI sequence started by {initiator} for color {sequenceColor}");
			TakiLogger.LogRules ($"SEQUENCE START: {initiator} starts TAKI sequence - only {sequenceColor} cards allowed");

			OnTakiSequenceStarted?.Invoke (sequenceColor, initiator);
		}

		/// <summary>
		/// CRITICAL FIX: Enhanced EndTakiSequence with proper cleanup
		/// </summary>
		public void EndTakiSequence () {
			if (!isInTakiSequence) {
				TakiLogger.LogWarning ("Cannot end sequence - no sequence is active", TakiLogger.LogCategory.GameState);
				return;
			}

			int finalCardCount = numberOfSequenceCards;
			CardColor finalColor = takiSequenceColor;
			PlayerType finalInitiator = takiSequenceInitiator;

			// CRITICAL FIX: Clear all sequence state before state transition
			isInTakiSequence = false;
			takiSequenceColor = CardColor.Wild;
			lastCardPlayedInSequence = null;
			numberOfSequenceCards = 0;
			// DON'T clear takiSequenceInitiator until after logging for debugging

			// Return to normal interaction state
			ChangeInteractionState (InteractionState.Normal);

			TakiLogger.LogGameState ($"TAKI sequence ended - was {finalCardCount} cards of {finalColor} by {finalInitiator}");
			TakiLogger.LogRules ($"SEQUENCE END: {finalInitiator} sequence of {finalCardCount} cards completed");

			// NOW clear initiator for clean state
			takiSequenceInitiator = PlayerType.Human; // Reset to default

			OnTakiSequenceEnded?.Invoke ();
		}

		/// <summary>
		/// CRITICAL FIX: Enhanced AddCardToSequence with validation
		/// </summary>
		/// <param name="card">Card being added to sequence</param>
		public void AddCardToSequence (CardData card) {
			if (!isInTakiSequence) {
				TakiLogger.LogWarning ("Attempting to add card to sequence when no sequence active", TakiLogger.LogCategory.GameState);
				return;
			}

			// CRITICAL VALIDATION: Ensure card can be played in sequence
			if (card != null && !CanPlayInSequence (card)) {
				TakiLogger.LogWarning ($"Cannot add {card.GetDisplayText ()} to sequence - invalid for {takiSequenceColor} sequence", TakiLogger.LogCategory.GameState);
				return;
			}

			lastCardPlayedInSequence = card;
			numberOfSequenceCards++;

			string cardDisplay = card?.GetDisplayText () ?? "NULL";
			TakiLogger.LogGameState ($"Card added to TAKI sequence: {cardDisplay} (card #{numberOfSequenceCards})");
			TakiLogger.LogRules ($"SEQUENCE CARD: {cardDisplay} added - sequence now {numberOfSequenceCards} cards by {takiSequenceInitiator}");

			OnTakiSequenceCardAdded?.Invoke (numberOfSequenceCards);
		}

		/// <summary>
		/// CRITICAL FIX: Enhanced sequence validation with proper null checking
		/// </summary>
		/// <param name="card">Card to check</param>
		/// <returns>True if card can be played in sequence</returns>
		public bool CanPlayInSequence (CardData card) {
			if (card == null) {
				TakiLogger.LogWarning ("CanPlayInSequence called with null card", TakiLogger.LogCategory.GameState);
				return false;
			}

			if (!isInTakiSequence) {
				// No sequence active - any valid card can be played
				return true;
			}

			// In sequence - only same color or wild cards allowed
			bool canPlay = card.color == takiSequenceColor || card.IsWildCard;

			TakiLogger.LogRules ($"SEQUENCE VALIDATION: {card.GetDisplayText ()} vs sequence color {takiSequenceColor} (initiated by {takiSequenceInitiator}) = {canPlay}");

			return canPlay;
		}

		/// <summary> 
		/// CRITICAL FIX: Enhanced reset with proper sequence cleanup
		/// </summary>
		public void ResetTakiSequenceState () {
			bool wasActive = isInTakiSequence;
			PlayerType previousInitiator = takiSequenceInitiator;

			isInTakiSequence = false;
			takiSequenceColor = CardColor.Wild;
			lastCardPlayedInSequence = null;
			numberOfSequenceCards = 0;
			takiSequenceInitiator = PlayerType.Human; // Reset to default

			if (wasActive) {
				TakiLogger.LogGameState ($"TAKI sequence state reset for new game (was initiated by {previousInitiator})");
			}
		}

		/// <summary>
		/// ENHANCED: Check if a card can be legally played with better sequence awareness
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

			// PHASE 8B: During TAKI sequence, check sequence color rules 
			if (isInTakiSequence) {
				bool sequenceValid = CanPlayInSequence (cardToPlay);
				if (!sequenceValid) {
					TakiLogger.LogRules ($"SEQUENCE RULE: Only {takiSequenceColor} or wild cards allowed in sequence, blocked {cardToPlay.GetDisplayText ()}");
					return false;
				}
			}

			// Use the normal CanPlayOn method for other validation
			bool isValid = cardToPlay.CanPlayOn (topDiscardCard, activeColor);

			if (isPlusTwoChainActive && isValid) {
				TakiLogger.LogRules ($"CHAIN VALID: {cardToPlay.GetDisplayText ()} can continue chain");
			}

			if (isInTakiSequence && isValid) {
				TakiLogger.LogRules ($"SEQUENCE VALID: {cardToPlay.GetDisplayText ()} can continue sequence");
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
		/// ENHANCED: Start a new PlusTwo chain with sequence awareness
		/// CRITICAL FIX: Don't overwrite TakiSequence state when chain starts during sequence
		/// </summary>
		/// <param name="initiator">Player who started the chain</param>
		public void StartPlusTwoChain (PlayerType initiator) {
			isPlusTwoChainActive = true;
			numberOfChainedPlusTwos = 1;
			chainInitiator = initiator;

			TakiLogger.LogGameState ($"PlusTwo chain started by {initiator}");
			TakiLogger.LogRules ($"CHAIN START: {initiator} plays PlusTwo #1 - opponent must draw 2 or continue");

			// CRITICAL FIX: Only change to PlusTwoChain interaction state if not in TakiSequence
			if (interactionState != InteractionState.TakiSequence) {
				ChangeInteractionState (InteractionState.PlusTwoChain);
				TakiLogger.LogGameState ("Interaction state changed to PlusTwoChain");
			} else {
				TakiLogger.LogGameState ("PlusTwo chain started during TakiSequence - preserving sequence state");
			}
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
		/// ENHANCED: Break PlusTwo chain with proper state restoration
		/// </summary>
		public void BreakPlusTwoChain () {
			int finalDrawCount = ChainDrawCount;
			int finalChainLength = numberOfChainedPlusTwos;

			TakiLogger.LogGameState ($"PlusTwo chain broken - was {finalChainLength} cards ({finalDrawCount} total draw)");
			TakiLogger.LogRules ($"CHAIN BREAK: Chain of {finalChainLength} PlusTwo cards broken by drawing {finalDrawCount} cards");

			isPlusTwoChainActive = false;
			numberOfChainedPlusTwos = 0;

			// ENHANCED: Only return to Normal if we were in PlusTwoChain state
			// If we were in TakiSequence, preserve that state
			if (interactionState == InteractionState.PlusTwoChain) {
				ChangeInteractionState (InteractionState.Normal);
				TakiLogger.LogGameState ("Returned to Normal interaction state after chain break");
			} else {
				TakiLogger.LogGameState ($"Chain broken but preserving {interactionState} interaction state");
			}
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
			// PHASE 8B: End any active sequence before declaring winner
			if (isInTakiSequence) {
				EndTakiSequence ();
			}

			ChangeGameStatus (GameStatus.GameOver);
			ChangeTurnState (TurnState.Neutral);
			ChangeInteractionState (InteractionState.Normal);

			TakiLogger.LogGameState ($"Game Over! Winner: {winner}");
			OnGameWon?.Invoke (winner);
		}

		/// <summary>
		/// Reset game state for a new game - ENHANCED with TAKI sequence reset
		/// </summary>
		public void ResetGameState () {
			turnState = TurnState.Neutral;
			interactionState = InteractionState.Normal;
			gameStatus = GameStatus.Active;
			activeColor = CardColor.Wild; // No color set initially
			turnDirection = TurnDirection.Clockwise;

			ResetPlusTwoChainState ();
			ResetTakiSequenceState ();

			TakiLogger.LogGameState ("Game state reset for new game (including PlusTwo chain and TAKI sequence state)");
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
					return $"TAKI sequence - play {takiSequenceColor} cards or end sequence";
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

		[ContextMenu ("Validate Sequence State Integrity")]
		public void ValidateSequenceStateIntegrity () {
			TakiLogger.LogDiagnostics ("=== SEQUENCE STATE INTEGRITY CHECK ===");

			// Check for state consistency
			bool interactionMatch = (interactionState == InteractionState.TakiSequence) == isInTakiSequence;
			TakiLogger.LogDiagnostics ($"Interaction State Match: {interactionMatch}");

			if (!interactionMatch) {
				TakiLogger.LogError ("STATE MISMATCH: InteractionState and isInTakiSequence don't match!", TakiLogger.LogCategory.GameState);
				TakiLogger.LogDiagnostics ($"  InteractionState: {interactionState}");
				TakiLogger.LogDiagnostics ($"  isInTakiSequence: {isInTakiSequence}");
			}

			if (isInTakiSequence) {
				// Validate sequence properties 
				bool hasValidColor = takiSequenceColor != CardColor.Wild;
				bool hasValidInitiator = takiSequenceInitiator == PlayerType.Human || takiSequenceInitiator == PlayerType.Computer;
				bool hasReasonableCardCount = numberOfSequenceCards >= 1 && numberOfSequenceCards <= 20;

				TakiLogger.LogDiagnostics ($"Valid Color: {hasValidColor} ({takiSequenceColor})");
				TakiLogger.LogDiagnostics ($"Valid Initiator: {hasValidInitiator} ({takiSequenceInitiator})");
				TakiLogger.LogDiagnostics ($"Reasonable Card Count: {hasReasonableCardCount} ({numberOfSequenceCards})");

				if (!hasValidColor || !hasValidInitiator || !hasReasonableCardCount) {
					TakiLogger.LogError ("INVALID SEQUENCE PROPERTIES!", TakiLogger.LogCategory.GameState);
				}
			}

			TakiLogger.LogDiagnostics ("=== END INTEGRITY CHECK ===");
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

		// PHASE 8B: TAKI Sequence Properties
		public bool IsInTakiSequence => isInTakiSequence;
		public CardColor TakiSequenceColor => takiSequenceColor;
		public int NumberOfSequenceCards => numberOfSequenceCards;
		public PlayerType TakiSequenceInitiator => takiSequenceInitiator;
		public CardData LastCardPlayedInSequence => lastCardPlayedInSequence;

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