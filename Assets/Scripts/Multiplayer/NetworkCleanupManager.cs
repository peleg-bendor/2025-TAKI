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
		[SerializeField] private MultiplayerMenuLogic multiplayerMenuLogic;

		[Header("Debug Settings")]
		public bool enableCleanupLogs = true;

		// Cleanup state tracking
		private bool isCleaningUp = false;

		#region MonoBehaviour

		void Awake() {
			// Find MultiplayerMenuLogic if not assigned
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
			if (isCleaningUp) {
				LogCleanup("Network cleanup already in progress");
				return;
			}

			LogCleanup("=== STARTING NETWORK CLEANUP FOR MENU RETURN ===");
			isCleaningUp = true;

			// Check if we're in a multiplayer context that needs cleanup
			if (!PhotonNetwork.IsConnected) {
				LogCleanup("Not connected to Photon - no cleanup needed");
				CompleteCleanup();
				return;
			}

			LogCleanup($"Network State: Connected={PhotonNetwork.IsConnected}, InRoom={PhotonNetwork.InRoom}");

			if (PhotonNetwork.InRoom) {
				LogCleanup($"Leaving room: {PhotonNetwork.CurrentRoom?.Name}");
				PhotonNetwork.LeaveRoom();
				// OnLeftRoom callback will complete cleanup
			} else {
				LogCleanup("Not in room - cleanup complete");
				CompleteCleanup();
			}
		}

		/// <summary>
		/// Reset multiplayer menu state for fresh matchmaking
		/// </summary>
		public void ResetMultiplayerMenuState() {
			if (multiplayerMenuLogic != null) {
				LogCleanup("Resetting multiplayer menu state for fresh matchmaking");
				// The menu logic will handle button states and status updates
				// when it detects we're back in lobby/connected state
			} else {
				LogCleanup("MultiplayerMenuLogic not found - cannot reset menu state");
			}
		}

		/// <summary>
		/// Complete cleanup process
		/// </summary>
		private void CompleteCleanup() {
			LogCleanup("Network cleanup completed");
			isCleaningUp = false;

			// Reset menu state for fresh multiplayer entry
			ResetMultiplayerMenuState();
		}

		/// <summary>
		/// Get current network state for debugging
		/// </summary>
		public string GetNetworkStateInfo() {
			if (!PhotonNetwork.IsConnected) {
				return "Not connected to Photon";
			}

			var room = PhotonNetwork.CurrentRoom;
			if (room != null) {
				return $"In room: {room.Name}, Players: {room.PlayerCount}/{room.MaxPlayers}";
			}

			return $"Connected to Photon, State: {PhotonNetwork.NetworkClientState}";
		}

		#endregion

		#region Photon Callbacks

		/// <summary>
		/// Called when successfully left the room
		/// </summary>
		public override void OnLeftRoom() {
			LogCleanup("Successfully left room - network cleanup complete");
			CompleteCleanup();
		}

		/// <summary>
		/// Called if leaving room fails
		/// </summary>
		public override void OnDisconnected(DisconnectCause cause) {
			LogCleanup($"Disconnected from Photon: {cause}");
			CompleteCleanup();
		}

		#endregion

		#region Utility Methods

		/// <summary>
		/// Log cleanup messages if enabled
		/// </summary>
		private void LogCleanup(string message) {
			if (enableCleanupLogs) {
				TakiLogger.LogNetwork(message);
			}
		}

		/// <summary>
		/// Check if cleanup is currently in progress
		/// </summary>
		public bool IsCleaningUp => isCleaningUp;

		#endregion

		#region Integration Methods

		/// <summary>
		/// Called by GameEndManager during go home sequence
		/// </summary>
		public void OnGoingHome() {
			LogCleanup("GameEndManager requested network cleanup for menu return");
			CleanupNetworkStateForMenu();
		}

		/// <summary>
		/// Called by MultiplayerMenuLogic to check if fresh start is needed
		/// </summary>
		public bool ShouldStartFreshMatchmaking() {
			// If we're still in a room after cleanup, we need fresh matchmaking
			bool inRoom = PhotonNetwork.InRoom;
			if (inRoom) {
				LogCleanup("Still in room - fresh matchmaking required");
			}
			return inRoom;
		}

		#endregion
	}
}