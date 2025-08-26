using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TakiGame {

	public class SfxSlider : MonoBehaviour {
		[Header ("UI References")]
		public TextMeshProUGUI sfxAmountText;

		[Header ("Audio References")]
		public AudioSource sfxAudioSource;

		[Header ("Settings")]
		[Range (0f, 10f)]
		public float defaultVolume = 10f;

		private Slider sfxSlider;

		private void Awake () {
			// Get the slider component attached to this GameObject
			sfxSlider = GetComponent<Slider> ();

			// Set the initial slider value
			if (sfxSlider != null) {
				sfxSlider.value = defaultVolume;
			}

			// Update the display and volume to reflect the initial value
			UpdateSfxVolume ();
		}

		/// <summary>
		/// Called when the slider value changes. Updates both the display text and the SFX volume.
		/// This method should be assigned to the slider's OnValueChanged event in the Inspector.
		/// </summary>
		public void UpdateSfxVolume () {
			if (sfxSlider == null) return;

			float sliderValue = sfxSlider.value;

			// Update the display text with proper formatting
			if (sfxAmountText != null) {
				sfxAmountText.text = sliderValue.ToString (sliderValue % 1 == 0 ? "0" : "0.##");
			}

			// Update the SFX volume (normalize slider value to 0-1 range)
			if (sfxAudioSource != null) {
				sfxAudioSource.volume = sliderValue / sfxSlider.maxValue;
			}
		}
	}
}