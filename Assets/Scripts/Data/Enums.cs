namespace TakiGame {
	/// <summary>
	/// Card colors in TAKI game 
	/// </summary>
	public enum CardColor {
		Red,
		Blue,
		Green,
		Yellow,
		Wild        // For special cards that can be any color
	}

	/// <summary>
	/// Types of cards in TAKI game
	/// </summary>
	public enum CardType {
		Number,             // Regular numbered cards (1-9)
		Plus,               // +1 card to opponent
		Stop,               // Skip opponent's turn  
		ChangeDirection,    // Reverse turn direction
		ChangeColor,        // Choose new color
		PlusTwo,            // +2 cards to opponent
		Taki,               // Play multiple cards of same color
		SuperTaki           // Play multiple cards of any color
	}

	/// <summary>
	/// Player types in the game
	/// </summary>
	public enum PlayerType {
		Human,
		Computer
	}

	/// <summary>
	/// Whose turn is it currently?
	/// </summary>
	public enum TurnState {
		PlayerTurn,     // Human player's turn
		ComputerTurn,   // Computer player's turn
		Neutral         // During transitions, setup, or game over
	}

	/// <summary>
	/// What special interaction is currently happening?
	/// </summary>
	public enum InteractionState {
		Normal,         // Regular gameplay - play card or draw
		ColorSelection, // Player must choose color (ChangeColor card played)
		TakiSequence,   // TAKI sequence active - can play multiple cards
		PlusTwoChain    // PlusTwo chain active - can stack +2 cards
	}

	/// <summary>
	/// Overall status of the game
	/// </summary>
	public enum GameStatus {
		Active,     // Game is running normally
		Paused,     // Game paused by player
		GameOver    // Game ended - winner determined
	}

	/// <summary>
	/// Turn direction for the game
	/// </summary>
	public enum TurnDirection {
		Clockwise,
		CounterClockwise
	}
}