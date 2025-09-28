using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// Handles network cleanup for multiplayer games following the established pattern
	/// Responsible for proper room leaving and connection state management
	/// Integrates with GameEndManager for clean game-to-menu transitions
	/// </summary>
	public class NetworkCleanupManager : MonoBehaviourPunCallbacks {

		[Header("Dependencies")]
		[SerializeField] private MultiplayerMenuLogic multiplayerMenuLogic;  // Reference to menu logic

		[Header("Debug Settings")]
		public bool enableCleanupLogs = true;                                 // Toggle cleanup logging

		// State tracking
		private bool isCleaningUp = false;  // Prevents concurrent cleanup operations

		#region MonoBehaviour

		void Awake() {
			// Auto-find MultiplayerMenuLogic component if not assigned in Inspector
			if (multiplayerMenuLogic == null) {
				multiplayerMenuLogic = FindObjectOfType<MultiplayerMenuLogic>();
			}
		}

		#endregion

		#region Network Cleanup Methods

		/// <summary>
		/// Main cleanup method called when returning to menu from multiplayer
		/// Handles room leaving and connection state reset
		/// </summary>
		public void CleanupNetworkStateForMenu() {
			// Prevent concurrent cleanup operations
			if (isCleaningUp) {
				LogCleanup("Network cleanup already in progress");
				return;
			}

			LogCleanup("Starting network cleanup for menu return");
			isCleaningUp = true;

			// Skip cleanup if not connected to Photon
			if (!PhotonNetwork.IsConnected) {
				LogCleanup("Not connected - no cleanup needed");
				CompleteCleanup();
				return;
			}

			// Log current network state
			LogCleanup($"Network State: Connected={PhotonNetwork.IsConnected}, InRoom={PhotonNetwork.InRoom}");

			// Leave room if we're in one
			if (PhotonNetwork.InRoom) {
				LogCleanup($"Leaving room: {PhotonNetwork.CurrentRoom?.Name}");
				PhotonNetwork.LeaveRoom();
				// OnLeftRoom callback will complete the cleanup process
			} else {
				// Already out of room, cleanup complete
				LogCleanup("Not in room - cleanup complete");
				CompleteCleanup();
			}
		}

		/// <summary>
		/// Reset multiplayer menu for fresh matchmaking after cleanup
		/// </summary>
		public void ResetMultiplayerMenuState() {
			if (multiplayerMenuLogic != null) {
				LogCleanup("Resetting menu state for fresh matchmaking");
				// MultiplayerMenuLogic will handle UI updates when connection state changes
			} else {
				LogCleanup("MultiplayerMenuLogic not found - cannot reset menu");
			}
		}

		/// <summary>
		/// Finalize cleanup process and reset state
		/// </summary>
		private void CompleteCleanup() {
			LogCleanup("Network cleanup completed");
			isCleaningUp = false;  // Allow future cleanup operations

			// Prepare menu for fresh multiplayer session
			ResetMultiplayerMenuState();
		}

		/// <summary>
		/// Get current network connection state as string (for debugging)
		/// </summary>
		public string GetNetworkStateInfo() {
			// Check basic connection status
			if (!PhotonNetwork.IsConnected) {
				return "Not connected to Photon";
			}

			// Check room status
			var room = PhotonNetwork.CurrentRoom;
			if (room != null) {
				return $"In room: {room.Name}, Players: {room.PlayerCount}/{room.MaxPlayers}";
			}

			// Connected but not in room
			return $"Connected, State: {PhotonNetwork.NetworkClientState}";
		}

		#endregion

		#region Photon Callbacks

		// Called by Photon when we successfully leave a room
		public override void OnLeftRoom() {
			LogCleanup("Successfully left room - cleanup complete");
			CompleteCleanup();
		}

		// Called by Photon when connection is lost (also completes cleanup)
		public override void OnDisconnected(DisconnectCause cause) {
			LogCleanup($"Disconnected from Photon: {cause}");
			CompleteCleanup();
		}

		#endregion

		#region Utility Methods

		/// <summary>
		/// Log cleanup messages (if logging enabled)
		/// </summary>
		private void LogCleanup(string message) {
			if (enableCleanupLogs) {
				TakiLogger.LogNetwork(message);
			}
		}

		/// <summary>
		/// Returns true if cleanup operation is currently running
		/// </summary>
		public bool IsCleaningUp => isCleaningUp;

		#endregion

		#region Integration Methods

		/// <summary>
		/// Called by GameEndManager when player returns to menu from multiplayer
		/// </summary>
		public void OnGoingHome() {
			LogCleanup("GameEndManager requesting network cleanup");
			CleanupNetworkStateForMenu();
		}

		/// <summary>
		/// Check if we need fresh matchmaking (still in a room after cleanup)
		/// </summary>
		public bool ShouldStartFreshMatchmaking() {
			// Return true if we're still in a room (cleanup incomplete)
			bool inRoom = PhotonNetwork.InRoom;
			if (inRoom) {
				LogCleanup("Still in room - need fresh matchmaking");
			}
			return inRoom;
		}

		#endregion
	}
}