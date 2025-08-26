// Assets\Scripts\ButtonSFX.cs
using UnityEngine;
using UnityEngine.UI;

namespace TakiGame {

	public class ButtonSFX : MonoBehaviour {
		[Header ("Audio References")]
		public AudioSource sfxAudioSource;  // The main SFX audio source that controls volume

		[Header ("Sound Settings")]
		public AudioClip buttonClickSound;  // The audio clip to play when button is clicked

		private Button button;

		private void Awake () {
			// Cache the button component for efficiency
			button = GetComponent<Button> ();

			// Automatically register our sound method to play when button is clicked
			if (button != null) {
				button.onClick.AddListener (PlayButtonSound);
			}
		}

		/// <summary>
		/// Plays the button click sound effect
		/// </summary>
		public void PlayButtonSound () {
			// Use PlayOneShot to play sound without interrupting other SFX
			if (sfxAudioSource != null && buttonClickSound != null) {
				sfxAudioSource.PlayOneShot (buttonClickSound);
			}
		}

		/// <summary>
		/// Clean up event listeners to prevent memory leaks
		/// </summary>
		private void OnDestroy () {
			if (button != null) {
				button.onClick.RemoveListener (PlayButtonSound);
			}
		}
	}
}
