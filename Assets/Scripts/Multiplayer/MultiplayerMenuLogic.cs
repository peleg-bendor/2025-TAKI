using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TakiGame {
	/// <summary>
	/// CLEANED VERSION: Handles Photon PUN2 multiplayer connection with minimal logging
	/// Only essential networking logs remain for development
	/// </summary>
	public class MultiplayerMenuLogic : MonoBehaviourPunCallbacks {
		[Header ("Events")]
		public static Action OnMultiplayerGameReady;

		[Header ("UI References - Assign in Inspector")]
		[SerializeField] private TextMeshProUGUI txtStatus;
		[SerializeField] private Button btnPlayMultiPlayer;
		[SerializeField] private GameObject screenMultiPlayerGame;

		[Header ("Room Configuration")]
		private int searchValue = 100;      // Used for matchmaking - rooms with same value can find each other
		private int maxPlayers = 2;         // TAKI is 1v1, so exactly 2 players required
		private string password = "taki2025"; // Simple room protection

		[Header ("Debug Settings")]
		public bool enableNetworkLogs = false;     // Toggle for network debug logs

		// Track game start state to prevent duplicate starts
		private bool hasGameStarted = false;

		#region MonoBehaviour

		void Awake () {
			InitAwake ();
		}

		void Start () {
			InitStart ();
		}

		#endregion

		#region Logic

		/// <summary>
		/// Initialize with direct references (minimal logging)
		/// </summary>
		private void InitAwake () {
			// Auto-find UI components if not assigned in Inspector
			if (txtStatus == null || btnPlayMultiPlayer == null) {
				TryFindReferences ();
			}
		}

		/// <summary>
		/// Try to find UI references automatically if not assigned
		/// </summary>
		private void TryFindReferences () {
			if (txtStatus == null) {
				GameObject found = GameObject.Find ("Txt_Status");
				if (found != null) {
					txtStatus = found.GetComponent<TextMeshProUGUI> ();
					if (txtStatus == null) {
						Text regularText = found.GetComponent<Text> ();
					}
				}
			}

			if (btnPlayMultiPlayer == null) {
				GameObject found = GameObject.Find ("Btn_PlayMultiPlayer");
				if (found != null) {
					btnPlayMultiPlayer = found.GetComponent<Button> ();
				}
			}

			if (screenMultiPlayerGame == null) {
				screenMultiPlayerGame = GameObject.Find ("Screen_MultiPlayerGame");
			}
		}

		/// <summary>
		/// Initialize multiplayer systems
		/// </summary>
		private void InitStart () {
			hasGameStarted = false;

			// Hide multiplayer game screen initially
			if (screenMultiPlayerGame != null) {
				screenMultiPlayerGame.SetActive (false);
			}

			// Disable play button until connected
			if (btnPlayMultiPlayer != null) {
				btnPlayMultiPlayer.interactable = false;
			}

			UpdateStatus ("Connecting to Photon...");

			// Enable automatic scene synchronization between clients
			PhotonNetwork.AutomaticallySyncScene = true;
			// Start connection to Photon servers using project settings
			PhotonNetwork.ConnectUsingSettings ();
		}

		/// <summary>
		/// Update status display
		/// </summary>
		private void UpdateStatus (string txt) {
			if (txtStatus != null) {
				txtStatus.text = txt;
			} else {
				// Emergency fallback
				GameObject found = GameObject.Find ("Txt_Status");
				if (found != null) {
					TextMeshProUGUI foundText = found.GetComponent<TextMeshProUGUI> ();
					if (foundText != null) {
						foundText.text = txt;
					} else {
						Text regularText = found.GetComponent<Text> ();
						if (regularText != null) {
							regularText.text = txt;
						}
					}
				}
			}
		}

		/// <summary>
		/// Create TAKI room
		/// </summary>
		private void CreateRoom () {
			if (enableNetworkLogs) {
				TakiLogger.LogInfo ("Creating TAKI room...", TakiLogger.LogCategory.Multiplayer);
			}

			// Room properties that other clients can filter by during matchmaking
			var roomProperties = new ExitGames.Client.Photon.Hashtable
			{
				{"sv", searchValue},  // Search value for room filtering
				{"pwd", password}     // Password for basic room security
			};

			// Configure room settings
			var roomOptions = new RoomOptions {
				MaxPlayers = maxPlayers,                          // Limit to 2 players for TAKI
				IsVisible = true,                                 // Room appears in lobby
				IsOpen = true,                                    // New players can join
				CustomRoomProperties = roomProperties,            // Attach custom properties
				CustomRoomPropertiesForLobby = new [] { "sv", "pwd" } // Make properties visible for matchmaking
			};

			// Create room with auto-generated name
			PhotonNetwork.CreateRoom (null, roomOptions, TypedLobby.Default);
		}

		/// <summary>
		/// Start multiplayer game ensuring ALL players transition to game screen
		/// FIXED: Always require 2 players for proper multiplayer testing
		/// </summary>
		private void StartGame () {
			var room = PhotonNetwork.CurrentRoom;
			// Safety checks before starting game
			if (room == null || hasGameStarted) {
				return;
			}

			// Check if we have enough players to start
			int players = room.PlayerCount;
			int max = room.MaxPlayers;
			bool reachMax = (max > 0) && (players == max);

			// Always require exactly 2 players for proper TAKI gameplay
			bool canStart = reachMax;

			if (!canStart) {
				UpdateStatus ($"Waiting for players... ({players}/{max})");
				return;
			}

			hasGameStarted = true;
			UpdateStatus ("Starting Game...");

			// Log game start details
			if (enableNetworkLogs) {
				TakiLogger.LogInfo ($"Game starting - Players: {players}/{max}, IsMasterClient: {PhotonNetwork.IsMasterClient}", TakiLogger.LogCategory.Multiplayer);
			}

			// Master client locks the room to prevent new players joining mid-game
			if (PhotonNetwork.IsMasterClient) {
				room.IsVisible = false;  // Hide from lobby
				room.IsOpen = false;     // Block new joins
			}

			// Notify GameManager to start multiplayer mode on ALL clients
			OnMultiplayerGameReady?.Invoke ();
		}

		/// <summary>
		/// Check if game can start
		/// </summary>
		private void CheckAndStartGame () {
			var room = PhotonNetwork.CurrentRoom;
			if (room == null) return;

			// Essential network log only
			if (enableNetworkLogs) {
				TakiLogger.LogInfo ($"Room check - {room.PlayerCount} players connected", TakiLogger.LogCategory.Multiplayer);
			}

			StartGame ();
		}

		#endregion

		#region Server Callbacks

		// Called when successfully connected to Photon master server
		public override void OnConnectedToMaster () {
			UpdateStatus ("Connected to server!");

			// Enable play button now that we can start matchmaking
			if (btnPlayMultiPlayer != null) {
				btnPlayMultiPlayer.interactable = true;
			}
		}

		// Called when successfully joined the lobby - start looking for existing rooms
		public override void OnJoinedLobby () {
			UpdateStatus ("Searching for TAKI games...");

			// Define what room properties we're looking for
			var expected = new ExitGames.Client.Photon.Hashtable
			{
				{"sv", searchValue},  // Match our search value
				{"pwd", password}     // Match our password
			};

			// Configure random room join parameters
			var op = new OpJoinRandomRoomParams {
				ExpectedCustomRoomProperties = expected,
			};

			// Try to join any existing room with matching properties and space
			PhotonNetwork.JoinRandomRoom (op.ExpectedCustomRoomProperties, maxPlayers);
		}

		// Called when no suitable existing room found - create our own
		public override void OnJoinRandomFailed (short returnCode, string message) {
			UpdateStatus ("Creating TAKI Room...");
			CreateRoom ();
		}

		// Called when successfully joined a room (either existing or newly created)
		public override void OnJoinedRoom () {
			UpdateStatus ("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
			hasGameStarted = false;  // Reset game state flag

			// Verify room password matches what we expect
			if (!string.IsNullOrEmpty (password)) {
				var expectedHash = PhotonNetwork.CurrentRoom.CustomProperties ["pwd"].ToString ();
				var myHash = password;
				// Leave room if passwords don't match
				if (!string.IsNullOrEmpty (expectedHash) && myHash != expectedHash) {
					PhotonNetwork.LeaveRoom ();
					return;
				}
			}

			// Check if we can start the game with current players
			CheckAndStartGame ();
		}

		// Called when another player joins our room
		public override void OnPlayerEnteredRoom (Player newPlayer) {
			UpdateStatus ($"Player joined! Starting game...");

			// Essential network log
			if (enableNetworkLogs) {
				TakiLogger.LogInfo ($"Player joined: {newPlayer.ActorNumber} - Room now has {PhotonNetwork.CurrentRoom.PlayerCount} players", TakiLogger.LogCategory.Multiplayer);
			}

			CheckAndStartGame ();
		}

		public override void OnDisconnected (DisconnectCause cause) {
			UpdateStatus ($"Disconnected: {cause}");
			hasGameStarted = false;

			if (btnPlayMultiPlayer != null) {
				btnPlayMultiPlayer.interactable = false;
			}
		}

		public override void OnPlayerLeftRoom (Player otherPlayer) {
			UpdateStatus ("Player left. Waiting for new player...");
			hasGameStarted = false;
		}

		/// <summary>
		/// Called when we leave a room - continue matchmaking flow
		/// </summary>
		public override void OnLeftRoom () {
			if (enableNetworkLogs) {
				TakiLogger.LogInfo ("Left room successfully - continuing to lobby for fresh matchmaking", TakiLogger.LogCategory.Multiplayer);
			}

			UpdateStatus ("Left previous room. Searching for games...");

			// Continue to lobby for fresh matchmaking
			if (PhotonNetwork.IsConnected) {
				PhotonNetwork.JoinLobby ();
			} else {
				UpdateStatus ("Connection lost. Reconnecting...");
				PhotonNetwork.ConnectUsingSettings ();
			}
		}

		#endregion

		#region Controllers

		/// <summary>
		/// Button click handler to start matchmaking
		/// ENHANCED: Now handles already-in-room state for proper re-entry
		/// </summary>
		public void Btn_PlayMultiPlayer () {
			hasGameStarted = false;

			if (btnPlayMultiPlayer != null) {
				btnPlayMultiPlayer.interactable = false;
			}

			// Handle different connection scenarios:

			// SCENARIO 1: Still in room from previous game - leave first for clean matchmaking
			if (PhotonNetwork.InRoom) {
				if (enableNetworkLogs) {
					TakiLogger.LogInfo ($"Already in room {PhotonNetwork.CurrentRoom.Name} - leaving first for fresh matchmaking", TakiLogger.LogCategory.Multiplayer);
				}
				UpdateStatus ("Leaving previous room...");
				PhotonNetwork.LeaveRoom ();
				// OnLeftRoom callback will continue the matchmaking flow
			}
			// SCENARIO 2: Connected but not in room - go straight to lobby
			else if (PhotonNetwork.IsConnected) {
				PhotonNetwork.JoinLobby ();
				UpdateStatus ("Searching for available TAKI rooms...");
			}
			// SCENARIO 3: Not connected at all - start from beginning
			else {
				UpdateStatus ("Connecting to Photon...");
				PhotonNetwork.ConnectUsingSettings ();
			}
		}

		#endregion

		#region Integration Support

		/// <summary>
		/// Get current connection status
		/// </summary>
		public bool IsConnectedToPhoton => PhotonNetwork.IsConnected;

		/// <summary>
		/// Get current room status
		/// </summary>
		public string GetRoomStatus () {
			if (PhotonNetwork.CurrentRoom != null) {
				var room = PhotonNetwork.CurrentRoom;
				return $"Room: {room.Name}, Players: {room.PlayerCount}/{room.MaxPlayers}, GameStarted: {hasGameStarted}";
			}
			return "Not in room";
		}

		/// <summary>
		/// Disconnect from Photon
		/// </summary>
		public void DisconnectFromPhoton () {
			if (PhotonNetwork.IsConnected) {
				hasGameStarted = false;
				PhotonNetwork.Disconnect ();
			}
		}

		/// <summary>
		/// Start matchmaking programmatically
		/// </summary>
		public void StartMatchmaking () {
			hasGameStarted = false;

			if (PhotonNetwork.IsConnected) {
				PhotonNetwork.JoinLobby ();
				UpdateStatus ("Searching for available TAKI rooms...");
			} else {
				UpdateStatus ("Not connected to server!");
			}
		}

		#endregion

		#region Debug Methods (Only when needed)

		/// <summary>
		/// Toggle network logging on/off
		/// </summary>
		[ContextMenu ("Toggle Network Logs")]
		public void ToggleNetworkLogs () {
			enableNetworkLogs = !enableNetworkLogs;
			TakiLogger.LogInfo ($"Debug logging: {(enableNetworkLogs ? "ENABLED" : "DISABLED")}", TakiLogger.LogCategory.Multiplayer);
		}

		/// <summary>
		/// Check current network state
		/// </summary>
		[ContextMenu ("Check Network State")]
		public void CheckNetworkState () {
			TakiLogger.LogInfo ("=== Network State ===", TakiLogger.LogCategory.Multiplayer);
			TakiLogger.LogInfo ($"Connected: {PhotonNetwork.IsConnected}", TakiLogger.LogCategory.Multiplayer);
			TakiLogger.LogInfo ($"In Room: {PhotonNetwork.InRoom}", TakiLogger.LogCategory.Multiplayer);
			TakiLogger.LogInfo ($"Game Started: {hasGameStarted}", TakiLogger.LogCategory.Multiplayer);

			if (PhotonNetwork.CurrentRoom != null) {
				var room = PhotonNetwork.CurrentRoom;
				TakiLogger.LogInfo ($"Room: {room.Name}, Players: {room.PlayerCount}/{room.MaxPlayers}", TakiLogger.LogCategory.Multiplayer);
			}
			TakiLogger.LogInfo ("===================", TakiLogger.LogCategory.Multiplayer);
		}

		/// <summary>
		/// Force game start for testing
		/// </summary>
		[ContextMenu ("Force Game Start (Debug)")]
		public void ForceGameStart () {
			TakiLogger.LogInfo ("FORCE GAME START - DEBUG ONLY", TakiLogger.LogCategory.Multiplayer);
			hasGameStarted = false;
			StartGame ();
		}

		#endregion
	}
}