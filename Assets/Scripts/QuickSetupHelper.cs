using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// Helper script for quick Milestone 5 setup and testing
	/// </summary>
	public class QuickSetupHelper : MonoBehaviour {

		[Header ("Quick Actions")]
		[SerializeField] private GameManager gameManager;

		void Start () {
			gameManager = FindObjectOfType<GameManager> ();
			if (gameManager == null) {
				Debug.LogError ("QuickSetupHelper: No GameManager found in scene!");
			}
		}

		[ContextMenu ("Quick Test Game")]
		public void QuickTestGame () {
			if (gameManager != null) {
				Debug.Log ("=== QUICK TEST STARTING ===");
				gameManager.StartNewGame ();
			}
		}

		[ContextMenu ("Test Turn Switch")]
		public void TestTurnSwitch () {
			if (gameManager != null) {
				Debug.Log ("=== TESTING TURN SWITCH ===");
				gameManager.RequestDrawCard ();
			}
		}

		[ContextMenu ("Validate Everything")]
		public void ValidateEverything () {
			Debug.Log ("=== COMPLETE VALIDATION ===");

			// Find all managers
			GameManager gm = FindObjectOfType<GameManager> ();
			DeckManager dm = FindObjectOfType<DeckManager> ();

			Debug.Log ($"GameManager found: {gm != null}");
			Debug.Log ($"DeckManager found: {dm != null}");

			if (gm != null) {
				// Access validation through context menu
				Debug.Log ("Use GameManager context menu: Validate All Systems");
			}

			if (dm != null) {
				Debug.Log ($"Deck Ready: {dm.IsDeckReady}");
				Debug.Log ($"Can Setup Game: {dm.CanSetupNewGame}");
			}
		}
	}
}