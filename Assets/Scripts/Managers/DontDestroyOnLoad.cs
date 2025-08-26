using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {
	[Header ("Singleton Settings")]
	[Tooltip ("Tag to identify this object type for duplicate prevention")]
	public string uniqueTag = "PersistentAudio";

	private void Awake () {
		// Check if another instance of this object already exists
		GameObject [] existingObjects = GameObject.FindGameObjectsWithTag (uniqueTag);

		if (existingObjects.Length > 1) {
			// If duplicate found, destroy this instance
			Debug.Log ($"Duplicate {uniqueTag} object found. Destroying duplicate.");
			Destroy (gameObject);
			return;
		}

		// Set the tag for future duplicate detection
		if (!gameObject.CompareTag (uniqueTag)) {
			gameObject.tag = uniqueTag;
		}

		// Make this object persist across scene loads
		DontDestroyOnLoad (gameObject);

		Debug.Log ($"{gameObject.name} marked as persistent and will not be destroyed on scene load.");
	}
}
