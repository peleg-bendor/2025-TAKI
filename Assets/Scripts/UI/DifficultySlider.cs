using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TakiGame {

	public class DifficultySlider : MonoBehaviour {
		public TextMeshProUGUI difficultyAmountText;

		private Slider difficultySlider;

		private void Awake () {
			difficultySlider = GetComponent<Slider> ();

			// Set the initial value of the slider to 2 (Normal)
			if (difficultySlider != null) {
				difficultySlider.value = 2;
			}

			// Update the text to reflect the initial value
			UpdateDifficultyDisplay ();
		}

		public void UpdateDifficultyDisplay () {
			if (difficultyAmountText != null && difficultySlider != null) {
				if (difficultySlider.value == 1) {
					difficultyAmountText.text = "Easy";
				} else if (difficultySlider.value == 2) {
					difficultyAmountText.text = "Normal";
				} else if (difficultySlider.value == 3) {
					difficultyAmountText.text = "Hard";
				}
			}
		}
	}
}