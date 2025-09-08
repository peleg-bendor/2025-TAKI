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
		private int searchValue = 100;
		private int maxPlayers = 2;
		private string password = "taki2025";

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
			// Try to find references if not assigned in inspector
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

			if (screenMultiPlayerGame != null) {
				screenMultiPlayerGame.SetActive (false);
			}

			if (btnPlayMultiPlayer != null) {
				btnPlayMultiPlayer.interactable = false;
			}

			UpdateStatus ("Connecting to Photon...");

			PhotonNetwork.AutomaticallySyncScene = true;
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
				Debug.Log ("[Network] Creating TAKI room...");
			}

			var roomProperties = new ExitGames.Client.Photon.Hashtable
			{
				{"sv", searchValue},
				{"pwd", password}
			};

			var roomOptions = new RoomOptions {
				MaxPlayers = maxPlayers,
				IsVisible = true,
				IsOpen = true,
				CustomRoomProperties = roomProperties,
				CustomRoomPropertiesForLobby = new [] { "sv", "pwd" }
			};

			PhotonNetwork.CreateRoom (null, roomOptions, TypedLobby.Default);
		}

		/// <summary>
		/// Start multiplayer game ensuring ALL players transition to game screen
		/// </summary>
		private void StartGame () {
			var room = PhotonNetwork.CurrentRoom;
			if (room == null || hasGameStarted) {
				return;
			}

			int players = room.PlayerCount;
			int max = room.MaxPlayers;
			bool reachMax = (max > 0) && (players == max);

			// TESTING MODE: Allow single player for development in editor
#if UNITY_EDITOR
			bool canStart = (players >= 1);
#else
			bool canStart = reachMax;
#endif

			if (!canStart) {
				UpdateStatus ($"Waiting for players... ({players}/{max})");
				return;
			}

			hasGameStarted = true;
			UpdateStatus ("Starting Game...");

			// Essential network log
			if (enableNetworkLogs) {
				Debug.Log ($"[Network] Game starting - Players: {players}/{max}, IsMasterClient: {PhotonNetwork.IsMasterClient}");
			}

			if (PhotonNetwork.IsMasterClient) {
				room.IsVisible = false;
				room.IsOpen = false;
			}

			// Fire event for ALL players
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
				Debug.Log ($"[Network] Room check - {room.PlayerCount} players connected");
			}

			StartGame ();
		}

		#endregion

		#region Server Callbacks

		public override void OnConnectedToMaster () {
			UpdateStatus ("Connected to server!");

			if (btnPlayMultiPlayer != null) {
				btnPlayMultiPlayer.interactable = true;
			}
		}

		public override void OnJoinedLobby () {
			UpdateStatus ("Searching for TAKI games...");

			var expected = new ExitGames.Client.Photon.Hashtable
			{
				{"sv", searchValue},
				{"pwd", password}
			};

			var op = new OpJoinRandomRoomParams {
				ExpectedCustomRoomProperties = expected,
			};

			PhotonNetwork.JoinRandomRoom (op.ExpectedCustomRoomProperties, maxPlayers);
		}

		public override void OnJoinRandomFailed (short returnCode, string message) {
			UpdateStatus ("Creating TAKI Room...");
			CreateRoom ();
		}

		public override void OnJoinedRoom () {
			UpdateStatus ("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
			hasGameStarted = false;

			// Password validation
			if (!string.IsNullOrEmpty (password)) {
				var expectedHash = PhotonNetwork.CurrentRoom.CustomProperties ["pwd"].ToString ();
				var myHash = password;
				if (!string.IsNullOrEmpty (expectedHash) && myHash != expectedHash) {
					PhotonNetwork.LeaveRoom ();
					return;
				}
			}

			CheckAndStartGame ();
		}

		/// <summary>
		/// Handle player joining room
		/// </summary>
		public override void OnPlayerEnteredRoom (Player newPlayer) {
			UpdateStatus ($"Player joined! Starting game...");

			// Essential network log
			if (enableNetworkLogs) {
				Debug.Log ($"[Network] Player joined: {newPlayer.ActorNumber} - Room now has {PhotonNetwork.CurrentRoom.PlayerCount} players");
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

		#endregion

		#region Controllers

		/// <summary>
		/// Button click handler to start matchmaking
		/// </summary>
		public void Btn_PlayMultiPlayer () {
			hasGameStarted = false;

			if (btnPlayMultiPlayer != null) {
				btnPlayMultiPlayer.interactable = false;
			}

			PhotonNetwork.JoinLobby ();
			UpdateStatus ("Searching for available TAKI rooms...");
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
			Debug.Log ($"[Network] Debug logging: {(enableNetworkLogs ? "ENABLED" : "DISABLED")}");
		}

		/// <summary>
		/// Check current network state
		/// </summary>
		[ContextMenu ("Check Network State")]
		public void CheckNetworkState () {
			Debug.Log ("=== Network State ===");
			Debug.Log ($"Connected: {PhotonNetwork.IsConnected}");
			Debug.Log ($"In Room: {PhotonNetwork.InRoom}");
			Debug.Log ($"Game Started: {hasGameStarted}");

			if (PhotonNetwork.CurrentRoom != null) {
				var room = PhotonNetwork.CurrentRoom;
				Debug.Log ($"Room: {room.Name}, Players: {room.PlayerCount}/{room.MaxPlayers}");
			}
			Debug.Log ("===================");
		}

		/// <summary>
		/// Force game start for testing
		/// </summary>
		[ContextMenu ("Force Game Start (Debug)")]
		public void ForceGameStart () {
			Debug.Log ("[Network] FORCE GAME START - DEBUG ONLY");
			hasGameStarted = false;
			StartGame ();
		}

		#endregion
	}
}