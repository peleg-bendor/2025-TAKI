using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TakiGame {
	/// <summary>
	/// Handles UI updates related to gameplay and turns using multi-enum architecture
	/// NO game logic, NO turn management, NO AI decisions
	/// </summary>
	public class GameplayUIManager : MonoBehaviour {

		[Header ("Turn UI References")]
		[Tooltip ("Text showing whose turn it is")]
		public TextMeshProUGUI currentTurnText;

		[Tooltip ("Text showing current game state")]
		public TextMeshProUGUI gameStateText;

		[Tooltip ("Text showing active color")]
		public TextMeshProUGUI activeColorText;

		[Tooltip ("Panel showing active color visually")]
		public Image activeColorPanel;

		[Header ("Action Buttons")]
		[Tooltip ("Button for player to draw a card")]
		public Button drawCardButton;

		[Tooltip ("Button for player to end turn")]
		public Button endTurnButton;

		[Tooltip ("Button to pause/settings")]
		public Button pauseButton;

		[Header ("Timer UI")]
		[Tooltip ("Text showing remaining turn time")]
		public TextMeshProUGUI turnTimerText;

		[Tooltip ("Slider showing turn time progress")]
		public Slider turnTimerSlider;

		[Header ("Hand Size Display")]
		[Tooltip ("Text showing player hand size")]
		public TextMeshProUGUI playerHandSizeText;

		[Tooltip ("Text showing computer hand size")]
		public TextMeshProUGUI computerHandSizeText;

		[Header ("Color Display Settings")]
		[Tooltip ("Colors to use for active color display")]
		public Color redColor = Color.red;
		public Color blueColor = Color.blue;
		public Color greenColor = Color.green;
		public Color yellowColor = Color.yellow;
		public Color wildColor = Color.white;

		// UI state tracking
		private bool buttonsEnabled = false;

		/// <summary>
		/// Update turn display using new architecture
		/// </summary>
		/// <param name="turnState">Current turn state</param>
		public void UpdateTurnDisplay (TurnState turnState) {
			if (currentTurnText != null) {
				string turnMessage = "";
				switch (turnState) {
					case TurnState.PlayerTurn:
						turnMessage = "Your Turn";
						break;
					case TurnState.ComputerTurn:
						turnMessage = "Computer's Turn";
						break;
					case TurnState.Neutral:
						turnMessage = "Game Setup";
						break;
				}
				currentTurnText.text = turnMessage;
			}

			// Update button states based on turn state
			UpdateButtonStates (turnState == TurnState.PlayerTurn);
		}

		/// <summary>
		/// Update game state display using multi-enum architecture
		/// </summary>
		/// <param name="gameStatus">Current game status</param>
		/// <param name="interactionState">Current interaction state</param>
		/// <param name="description">Optional custom description</param>
		public void UpdateGameStateDisplay (GameStatus gameStatus, InteractionState interactionState, string description = "") {
			if (gameStateText != null) {
				string stateText = "";

				if (!string.IsNullOrEmpty (description)) {
					stateText = description;
				} else {
					// Build description from states
					if (gameStatus != GameStatus.Active) {
						stateText = gameStatus == GameStatus.GameOver ? "Game Over" : "Game Paused";
					} else {
						switch (interactionState) {
							case InteractionState.Normal:
								stateText = "Play a card or draw";
								break;
							case InteractionState.ColorSelection:
								stateText = "Choose a color";
								break;
							case InteractionState.TakiSequence:
								stateText = "TAKI sequence active";
								break;
							case InteractionState.PlusTwoChain:
								stateText = "Plus Two chain active";
								break;
						}
					}
				}

				gameStateText.text = stateText;
			}
		}

		/// <summary>
		/// Update active color display
		/// </summary>
		/// <param name="activeColor">Current active color</param>
		public void UpdateActiveColorDisplay (CardColor activeColor) {
			if (activeColorText != null) {
				activeColorText.text = $"Active Color: {activeColor}";
			}

			if (activeColorPanel != null) {
				activeColorPanel.color = GetColorForCardColor (activeColor);
			}
		}

		/// <summary>
		/// Update hand size displays
		/// </summary>
		/// <param name="playerHandSize">Number of cards in player's hand</param>
		/// <param name="computerHandSize">Number of cards in computer's hand</param>
		public void UpdateHandSizeDisplay (int playerHandSize, int computerHandSize) {
			if (playerHandSizeText != null) {
				playerHandSizeText.text = $"Your Cards: {playerHandSize}";
			}

			if (computerHandSizeText != null) {
				computerHandSizeText.text = $"Computer Cards: {computerHandSize}";
			}
		}

		/// <summary>
		/// Update all displays using new multi-enum architecture
		/// </summary>
		/// <param name="turnState">Current turn state</param>
		/// <param name="gameStatus">Current game status</param>
		/// <param name="interactionState">Current interaction state</param>
		/// <param name="activeColor">Current active color</param>
		public void UpdateAllDisplays (TurnState turnState, GameStatus gameStatus, InteractionState interactionState, CardColor activeColor) {
			UpdateTurnDisplay (turnState);
			UpdateGameStateDisplay (gameStatus, interactionState);
			UpdateActiveColorDisplay (activeColor);
		}

		/// <summary>
		/// Update turn timer display
		/// </summary>
		/// <param name="remainingTime">Seconds remaining in turn</param>
		/// <param name="maxTime">Maximum turn time</param>
		public void UpdateTurnTimer (float remainingTime, float maxTime) {
			if (turnTimerText != null) {
				if (remainingTime > 0) {
					turnTimerText.text = $"Time: {remainingTime:F1}s";
					turnTimerText.gameObject.SetActive (true);
				} else {
					turnTimerText.gameObject.SetActive (false);
				}
			}

			if (turnTimerSlider != null) {
				if (remainingTime > 0 && maxTime > 0) {
					turnTimerSlider.value = remainingTime / maxTime;
					turnTimerSlider.gameObject.SetActive (true);
				} else {
					turnTimerSlider.gameObject.SetActive (false);
				}
			}
		}

		/// <summary>
		/// Enable or disable action buttons
		/// </summary>
		/// <param name="enabled">Whether buttons should be enabled</param>
		public void UpdateButtonStates (bool enabled) {
			buttonsEnabled = enabled;

			if (drawCardButton != null) {
				drawCardButton.interactable = enabled;
			}

			if (endTurnButton != null) {
				endTurnButton.interactable = enabled;
			}

			// Pause button should always be available
			if (pauseButton != null) {
				pauseButton.interactable = true;
			}
		}

		/// <summary>
		/// Show winner announcement using new architecture
		/// </summary>
		/// <param name="winner">Winning player</param>
		public void ShowWinnerAnnouncement (PlayerType winner) {
			string winnerText = winner == PlayerType.Human ?
								"You Win!" : "Computer Wins!";

			if (currentTurnText != null) {
				currentTurnText.text = winnerText;
			}

			if (gameStateText != null) {
				gameStateText.text = "Game Over";
			}

			// Disable all action buttons
			UpdateButtonStates (false);
		}

		/// <summary>
		/// Reset UI for new game
		/// </summary>
		public void ResetUIForNewGame () {
			if (currentTurnText != null) {
				currentTurnText.text = "New Game";
			}

			if (gameStateText != null) {
				gameStateText.text = "Starting...";
			}

			if (activeColorText != null) {
				activeColorText.text = "Active Color: -";
			}

			if (activeColorPanel != null) {
				activeColorPanel.color = wildColor;
			}

			UpdateHandSizeDisplay (0, 0);
			UpdateTurnTimer (0, 0);
			UpdateButtonStates (false);
		}

		/// <summary>
		/// Convert CardColor to Unity Color (fixed based on user feedback)
		/// </summary>
		/// <param name="cardColor">Card color to convert</param>
		/// <returns>Unity color</returns>
		Color GetColorForCardColor (CardColor cardColor) {
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

		/// <summary>
		/// Show temporary status message
		/// </summary>
		/// <param name="message">Message to show</param>
		/// <param name="duration">How long to show it</param>
		public void ShowTemporaryMessage (string message, float duration = 2f) {
			if (gameStateText != null) {
				gameStateText.text = message;

				// Clear message after duration
				if (duration > 0) {
					CancelInvoke (nameof (ClearTemporaryMessage));
					Invoke (nameof (ClearTemporaryMessage), duration);
				}
			}
		}

		/// <summary>
		/// Clear temporary message
		/// </summary>
		void ClearTemporaryMessage () {
			if (gameStateText != null) {
				gameStateText.text = "";
			}
		}

		/// <summary>
		/// Validate that required UI elements are assigned
		/// </summary>
		/// <returns>True if basic UI elements are present</returns>
		public bool ValidateUIElements () {
			bool hasRequired = currentTurnText != null && gameStateText != null;

			if (!hasRequired) {
				Debug.LogWarning ("GameplayUIManager: Missing required UI elements!");
			}

			return hasRequired;
		}

		// Properties
		public bool ButtonsEnabled => buttonsEnabled;
		public bool HasRequiredElements => ValidateUIElements ();
	}
}