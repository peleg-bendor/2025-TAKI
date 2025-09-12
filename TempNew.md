using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// MILESTONE 1: NetworkGameManager with MINIMAL LOGGING
	/// Reduced log traffic while preserving essential debugging
	/// </summary>
	public class NetworkGameManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks {

		[Header ("Network Turn Management")]
		public PunTurnManager turnMgr;

		[Header ("Game Integration")]
		public GameManager gameManager;

		// Network state
		private bool _isMyTurn = false;
		private bool _isGameOver = false;
		private bool _isFirstTurn = true;
		private bool _isDeckInitialized = false;

		// MILESTONE 1: Deck initialization state
		private bool _waitingForDeckState = false;
		private NetworkInitialGameState _pendingGameState;

		void Awake () {
			if (turnMgr != null) {
				turnMgr.TurnManagerListener = this;
			}
		}

		/// <summary>
		/// Start network game - ESSENTIAL LOG ONLY
		/// </summary>
		public void StartNetworkGame () {
			TakiLogger.LogNetwork ("Starting network game");

			_isGameOver = false;
			_isFirstTurn = true;
			_isDeckInitialized = false;

			InitializeSharedDeck ();
		}

		/// <summary>
		/// Initialize shared deck - ESSENTIAL LOG ONLY
		/// </summary>
		void InitializeSharedDeck () {
			if (PhotonNetwork.IsMasterClient) {
				TakiLogger.LogNetwork ("Master: Setting up deck");
				SetupMasterDeck ();
			} else {
				// REMOVED: Verbose waiting log
				_waitingForDeckState = true;
			}
		}

		/// <summary>
		/// Master client deck setup - MINIMAL LOGGING
		/// </summary>
		void SetupMasterDeck () {
			if (gameManager?.deckManager == null) {
				TakiLogger.LogError ("Missing components for master deck setup", TakiLogger.LogCategory.Network);
				return;
			}

			// REMOVED: Verbose setup logging
			var gameState = gameManager.deckManager.SetupInitialGame ();

			if (gameState.startingCard != null) {
				NetworkGameState networkState = new NetworkGameState {
					startingCardIdentifier = CardDataHelper.CreateCardIdentifier (gameState.startingCard),
					drawPileCount = gameManager.deckManager.DrawPileCount,
					player1Hand = gameState.player1Hand,
					player2Hand = gameState.player2Hand,
					masterClientActor = PhotonNetwork.LocalPlayer.ActorNumber
				};

				// Send complete game state to all clients
				photonView.RPC ("ReceiveInitialGameState", RpcTarget.Others,
					networkState.startingCardIdentifier,
					networkState.drawPileCount,
					SerializeHand (networkState.player1Hand),
					SerializeHand (networkState.player2Hand),
					networkState.masterClientActor);

				SetupLocalMultiplayerHands (gameState.player1Hand, gameState.player2Hand);
				UpdateMultiplayerDeckDisplay ();

				_isDeckInitialized = true;

				if (turnMgr != null) {
					turnMgr.BeginTurn ();
				}

				TakiLogger.LogNetwork ("Master deck setup complete");
			} else {
				TakiLogger.LogError ("Master deck setup failed - no starting card", TakiLogger.LogCategory.Network);
			}
		}

		/// <summary>
		/// Receive initial game state - ESSENTIAL LOG ONLY
		/// </summary>
		[PunRPC]
		void ReceiveInitialGameState (string startingCardId, int drawCount, string serializedPlayer1Hand, string serializedPlayer2Hand, int masterActor) {
			// REMOVED: Verbose receive logging

			if (!_waitingForDeckState) {
				TakiLogger.LogWarning ("Received unexpected game state", TakiLogger.LogCategory.Network);
				return;
			}

			_waitingForDeckState = false;

			List<CardData> player1Hand = DeserializeHand (serializedPlayer1Hand);
			List<CardData> player2Hand = DeserializeHand (serializedPlayer2Hand);

			ApplyReceivedGameState (startingCardId, drawCount, player1Hand, player2Hand, masterActor);
			_isDeckInitialized = true;

			TakiLogger.LogNetwork ("Client deck initialization complete");
		}

		/// <summary>
		/// Apply received game state - MINIMAL LOGGING
		/// </summary>
		void ApplyReceivedGameState (string startingCardId, int drawCount, List<CardData> player1Hand, List<CardData> player2Hand, int masterActor) {
			if (gameManager?.deckManager == null) {
				TakiLogger.LogError ("Cannot apply game state: Missing components", TakiLogger.LogCategory.Network);
				return;
			}

			gameManager.deckManager.InitializeDeck ();

			CardData startingCard = FindCardFromIdentifier (startingCardId);
			if (startingCard != null) {
				gameManager.deckManager.DiscardCard (startingCard);
				// REMOVED: Verbose card logging
			} else {
				TakiLogger.LogWarning ($"Could not find starting card: {startingCardId}", TakiLogger.LogCategory.Network);
			}

			SetupLocalMultiplayerHands (player1Hand, player2Hand);
			UpdateMultiplayerDeckDisplay ();

			if (gameManager.gameplayUI != null) {
				gameManager.gameplayUI.ShowPlayerMessage ("Game synchronized - Ready to play!");
			}

			// REMOVED: Verbose completion logging
		}

		/// <summary>
		/// Setup local multiplayer hands - CRITICAL LOGS ONLY
		/// </summary>
		void SetupLocalMultiplayerHands (List<CardData> player1Hand, List<CardData> player2Hand) {
			// REMOVED: All verbose hand debugging logs
			
			if (player1Hand == null || player2Hand == null) {
				TakiLogger.LogError ("Null hands received", TakiLogger.LogCategory.Network);
				return;
			}

			List<Player> sortedPlayers = PhotonNetwork.PlayerList.OrderBy (p => p.ActorNumber).ToList ();

			if (sortedPlayers.Count < 2) {
				TakiLogger.LogError ("Not enough players for hand assignment", TakiLogger.LogCategory.Network);
				return;
			}

			Player player1 = sortedPlayers [0];
			Player player2 = sortedPlayers [1];

			bool isPlayer1 = (PhotonNetwork.LocalPlayer.ActorNumber == player1.ActorNumber);

			List<CardData> myHand = isPlayer1 ? player1Hand : player2Hand;
			List<CardData> opponentHand = isPlayer1 ? player2Hand : player1Hand;

			// CRITICAL VALIDATION only
			if (myHand == null || myHand.Count == 0) {
				TakiLogger.LogError ($"Empty hand assigned - using fallback", TakiLogger.LogCategory.Network);

				if (player1Hand != null && player1Hand.Count > 0) {
					myHand = player1Hand;
					opponentHand = player2Hand ?? new List<CardData> ();
				} else if (player2Hand != null && player2Hand.Count > 0) {
					myHand = player2Hand;
					opponentHand = player1Hand ?? new List<CardData> ();
				} else {
					TakiLogger.LogError ("Both hands empty - cannot proceed", TakiLogger.LogCategory.Network);
					return;
				}
			}

			// Setup hands in GameManager
			if (gameManager != null) {
				gameManager.playerHand.Clear ();
				gameManager.playerHand.AddRange (myHand);

				if (gameManager.playerHandManager != null) {
					gameManager.playerHandManager.SetNetworkMode (true);
					gameManager.playerHandManager.UpdateHandDisplay (myHand);
					// REMOVED: Success logging
				}

				if (gameManager.computerHandManager != null) {
					gameManager.computerHandManager.SetNetworkModeEnhanced (true, true);
					gameManager.computerHandManager.InitializeNetworkHandsEnhanced (false, opponentHand);
					// REMOVED: Success logging
				}

				if (gameManager.gameplayUI != null) {
					gameManager.gameplayUI.UpdateHandSizeDisplay (myHand.Count, opponentHand.Count);
				}
			}

			// REMOVED: All completion logs
		}

		/// <summary>
		/// Update multiplayer deck display - MINIMAL LOGGING
		/// </summary>
		void UpdateMultiplayerDeckDisplay () {
			if (gameManager?.deckManager == null) {
				return; // REMOVED: Warning log
			}

			int drawPileCount = gameManager.deckManager.DrawPileCount;
			int discardPileCount = gameManager.deckManager.DiscardPileCount;
			CardData topDiscardCard = gameManager.deckManager.GetTopDiscardCard ();

			// REMOVED: Verbose deck status logging

			if (gameManager.deckManager.deckUI != null) {
				gameManager.deckManager.deckUI.UpdateDeckUI (drawPileCount, discardPileCount);

				if (topDiscardCard != null) {
					gameManager.deckManager.deckUI.UpdateDiscardPileDisplay (topDiscardCard);
				}
			}

			if (gameManager.gameplayUI != null) {
				gameManager.gameplayUI.ShowDeckSyncStatus ($"Draw: {drawPileCount}, Discard: {discardPileCount}");
			}

			// REMOVED: Success logging
		}

		/// <summary>
		/// Serialize hand - MINIMAL LOGGING
		/// </summary>
		string SerializeHand (List<CardData> hand) {
			if (hand == null || hand.Count == 0) {
				return "";
			}

			List<string> cardIds = new List<string> ();
			foreach (CardData card in hand) {
				if (card != null) {
					string cardId = CardDataHelper.CreateCardIdentifier (card);
					cardIds.Add (cardId);
					// REMOVED: Per-card logging
				} else {
					TakiLogger.LogWarning ("Null card in serialization", TakiLogger.LogCategory.Network);
				}
			}

			// REMOVED: Serialization success logging
			return string.Join ("|", cardIds);
		}

		/// <summary>
		/// Deserialize hand - MINIMAL LOGGING
		/// </summary>
		List<CardData> DeserializeHand (string serializedHand) {
			List<CardData> hand = new List<CardData> ();

			if (string.IsNullOrEmpty (serializedHand)) {
				return hand; // REMOVED: Empty string log
			}

			// REMOVED: Deserialization start logging

			string [] cardIds = serializedHand.Split ('|');
			// REMOVED: Split count logging

			foreach (string cardId in cardIds) {
				if (!string.IsNullOrEmpty (cardId)) {
					CardData card = FindCardFromIdentifier (cardId);
					if (card != null) {
						hand.Add (card);
						// REMOVED: Per-card deserialization logging
					} else {
						TakiLogger.LogWarning ($"Card not found: {cardId}", TakiLogger.LogCategory.Network);
					}
				}
			}

			// REMOVED: Deserialization complete logging
			return hand;
		}

		/// <summary>
		/// Find card from identifier - ERROR LOGGING ONLY
		/// </summary>
		CardData FindCardFromIdentifier (string cardId) {
			if (string.IsNullOrEmpty (cardId)) return null;

			CardDataLoader cardLoader = gameManager?.deckManager?.cardLoader;
			if (cardLoader == null) {
				TakiLogger.LogError ("CardDataLoader not available", TakiLogger.LogCategory.Network);
				return null;
			}

			return CardDataHelper.ParseCardIdentifier (cardLoader, cardId);
		}

		// === IPunTurnManagerCallbacks - ESSENTIAL LOGS ONLY ===

		public void OnTurnBegins (int turn) {
			// REMOVED: Verbose turn logging
			
			if (!_isDeckInitialized) {
				// REMOVED: Waiting log
				return;
			}

			int expectedActor = GetExpectedActorForTurn (turn);
			_isMyTurn = PhotonNetwork.LocalPlayer.ActorNumber == expectedActor;

			// REMOVED: Turn determination logging

			if (gameManager != null && gameManager.gameState != null) {
				TurnState newTurnState = _isMyTurn ? TurnState.PlayerTurn : TurnState.ComputerTurn;
				gameManager.gameState.ChangeTurnState (newTurnState);

				if (gameManager.gameplayUI != null) {
					gameManager.gameplayUI.UpdateTurnDisplayMultiplayer (_isMyTurn);
				}
			}

			if (_isFirstTurn) {
				_isFirstTurn = false;
				// REMOVED: First turn log
			}
		}

		public void OnPlayerFinished (Player player, int turn, object move) {
			// REMOVED: Verbose player finished logging

			if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber && move != null) {
				ProcessRemoteAction (player, move);
			}

			if (PhotonNetwork.IsMasterClient && turnMgr != null) {
				turnMgr.BeginTurn ();
			}
		}

		public void OnTurnCompleted (int turn) { }
		public void OnPlayerMove (Player player, int turn, object move) { }
		public void OnTurnTimeEnds (int turn) { }

		/// <summary>
		/// Get expected actor for turn - NO LOGGING
		/// </summary>
		int GetExpectedActorForTurn (int turn) {
			var room = PhotonNetwork.CurrentRoom;
			if (room == null) return -1;

			var list = new System.Collections.Generic.List<int> ();
			foreach (var kvp in room.Players) {
				list.Add (kvp.Key);
			}
			list.Sort ();

			if (list.Count == 0) return -1;

			int idx = (turn - 1) % list.Count;
			return list [idx];
		}

		/// <summary>
		/// Send card play - ESSENTIAL LOG ONLY
		/// </summary>
		public void SendCardPlay (CardData card) {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			string cardId = GetCardIdentifier (card);

			var moveData = new NetworkMoveData {
				actionType = "PLAY_CARD",
				cardIdentifier = cardId
			};

			turnMgr.SendMove (moveData, true);
			// REMOVED: Send confirmation logging
		}

		/// <summary>
		/// Send card draw - NO LOGGING
		/// </summary>
		public void SendCardDraw () {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			var moveData = new NetworkMoveData {
				actionType = "DRAW_CARD",
				cardIdentifier = ""
			};

			turnMgr.SendMove (moveData, true);
			// REMOVED: Send confirmation logging
		}

		/// <summary>
		/// Process remote action - NO LOGGING
		/// </summary>
		void ProcessRemoteAction (Player player, object moveData) {
			if (gameManager == null) return;

			if (moveData is NetworkMoveData networkMove) {
				switch (networkMove.actionType) {
					case "PLAY_CARD":
						gameManager.ProcessNetworkCardPlay (networkMove.cardIdentifier, player.ActorNumber);
						break;
					case "DRAW_CARD":
						gameManager.ProcessNetworkCardDraw (player.ActorNumber);
						break;
				}
			}
		}

		/// <summary>
		/// Get card identifier - NO LOGGING
		/// </summary>
		string GetCardIdentifier (CardData card) {
			return CardDataHelper.CreateCardIdentifier (card);
		}

		// Properties
		public bool IsMyTurn => _isMyTurn;
		public bool IsNetworkGameActive => !_isGameOver;
		public bool IsDeckInitialized => _isDeckInitialized;
	}

	// Support classes unchanged
	[System.Serializable]
	public class NetworkInitialGameState {
		public string startingCardIdentifier;
		public int drawPileCount;
		public int player1HandSize;
		public int player2HandSize;
		public int masterClientActor;
	}

	[System.Serializable]
	public class NetworkMoveData {
		public string actionType;
		public string cardIdentifier;
	}

	[System.Serializable]
	public class NetworkGameState {
		public string startingCardIdentifier;
		public int drawPileCount;
		public List<CardData> player1Hand;
		public List<CardData> player2Hand;
		public int masterClientActor;
	}
}