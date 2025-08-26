using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TakiGame {

	public class MusicSlider : MonoBehaviour {
		[Header ("UI References")]
		public TextMeshProUGUI musicAmountText;

		[Header ("Audio References")]
		public AudioSource backgroundMusicSource;

		[Header ("Settings")]
		[Range (0f, 10f)]
		public float defaultVolume = 10f;

		private Slider musicSlider;

		private void Awake () {
			// Get the slider component attached to this GameObject
			musicSlider = GetComponent<Slider> ();

			// Set the initial slider value
			if (musicSlider != null) {
				musicSlider.value = defaultVolume;
			}

			// Update the display and volume to reflect the initial value
			UpdateMusicVolume ();
		}

		/// <summary>
		/// Called when the slider value changes. Updates both the display text and the background music volume.
		/// This method should be assigned to the slider's OnValueChanged event in the Inspector.
		/// </summary>
		public void UpdateMusicVolume () {
			if (musicSlider == null) return;

			float sliderValue = musicSlider.value;

			// Update the display text with proper formatting
			if (musicAmountText != null) {
				musicAmountText.text = sliderValue.ToString (sliderValue % 1 == 0 ? "0" : "0.##");
			}

			// Update the background music volume (normalize slider value to 0-1 range)
			if (backgroundMusicSource != null) {
				backgroundMusicSource.volume = sliderValue / musicSlider.maxValue;
			}
		}
	}
}