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
		Number,				// Regular numbered cards (1-9)
		Plus,				// +1 card to opponent
		Stop,				// Skip opponent's turn  
		ChangeDirection,	// Reverse turn direction
		ChangeColor,		// Choose new color
		PlusTwo,			// +2 cards to opponent
		Taki,				// Play multiple cards of same color
		SuperTaki			// Play multiple cards of any color
	}

	/// <summary>
	/// Player types in the game
	/// </summary>
	public enum PlayerType {
		Human,
		Computer
	}

	/// <summary>
	/// Current state of the game
	/// </summary>
	public enum GameState {
		PlayerTurn,
		ComputerTurn,
		ColorSelection,  // When ChangeColor card is played
		TakiSequence,    // During TAKI card multi-play
		PlusTwoChain,    // When +2 cards can be chained
		GameOver,
		Paused
	}

	/// <summary>
	/// Turn direction for the game
	/// </summary>
	public enum TurnDirection {
		Clockwise,
		CounterClockwise
	}
}