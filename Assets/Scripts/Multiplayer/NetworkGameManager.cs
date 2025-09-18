using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

namespace TakiGame {
	/// <summary>
	/// PHASE 2 MILESTONE 1: Enhanced NetworkGameManager with Deck Initialization
	/// Following instructor's proven pattern with master/client coordination
	/// FIXED: All CardData constructor calls removed
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
		/// Start network game - called by GameManager
		/// ENHANCED: Now includes deck initialization
		/// </summary>
		public void StartNetworkGame () {
			TakiLogger.LogNetwork ("=== STARTING NETWORK GAME WITH DECK INITIALIZATION ===");

			_isGameOver = false;
			_isFirstTurn = true;
			_isDeckInitialized = false;

			// MILESTONE 1: Initialize shared deck state
			InitializeSharedDeck ();
		}

		/// <summary>
		/// MILESTONE 1: Initialize shared deck with master/client coordination
		/// Master creates deck, clients receive synchronized state
		/// </summary>
		void InitializeSharedDeck () {
			TakiLogger.LogNetwork ("=== INITIALIZING SHARED DECK ===");

			if (PhotonNetwork.IsMasterClient) {
				TakiLogger.LogNetwork ("I am Master Client - setting up deck and broadcasting state");
				SetupMasterDeck ();
			} else {
				TakiLogger.LogNetwork ("I am Client - waiting for initial game state from master");
				_waitingForDeckState = true;
			}
		}

		/// <summary>
		/// ENHANCED: Master deck setup with simplified state broadcasting
		/// REPLACES: Complex SetupMasterDeck method
		/// APPROACH: Setup -> broadcast -> done
		/// </summary>
		void SetupMasterDeck () {
			if (gameManager?.deckManager == null) {
				TakiLogger.LogError ("Cannot setup master deck: Missing components", TakiLogger.LogCategory.Network);
				return;
			}

			TakiLogger.LogNetwork ("Master client setting up deck - simplified approach");

			// Use existing deck setup logic
			var gameState = gameManager.deckManager.SetupInitialGame ();

			if (gameState.startingCard != null) {
				// Create and send network state
				string startingCardId = CardDataHelper.CreateCardIdentifier (gameState.startingCard);
				string serializedPlayer1Hand = SerializeHand (gameState.player1Hand);
				string serializedPlayer2Hand = SerializeHand (gameState.player2Hand);

				// Log the complete message before sending
				TakiLogger.LogNetwork ("=== SENDING INITIAL GAME STATE RPC ===");
				TakiLogger.LogNetwork ($"Starting Card ID: {startingCardId}");
				TakiLogger.LogNetwork ($"Draw Pile Count: {gameManager.deckManager.DrawPileCount}");
				TakiLogger.LogNetwork ($"Player 1 Hand (serialized): {serializedPlayer1Hand}");
				TakiLogger.LogNetwork ($"Player 2 Hand (serialized): {serializedPlayer2Hand}");
				TakiLogger.LogNetwork ($"Master Client Actor Number: {PhotonNetwork.LocalPlayer.ActorNumber}");
				TakiLogger.LogNetwork ($"Player 1 Hand Size: {gameState.player1Hand.Count} cards");
				TakiLogger.LogNetwork ($"Player 2 Hand Size: {gameState.player2Hand.Count} cards");
				TakiLogger.LogNetwork ("=== RPC MESSAGE DETAILS LOGGED ===");

				// Send to other clients
				photonView.RPC ("ReceiveInitialGameState", RpcTarget.Others,
					startingCardId,
					gameManager.deckManager.DrawPileCount,
					serializedPlayer1Hand,
					serializedPlayer2Hand,
					PhotonNetwork.LocalPlayer.ActorNumber);

				// Setup local state using simplified method
				SetupLocalMultiplayerHands (gameState.player1Hand, gameState.player2Hand);

				// Update deck display
				UpdateMultiplayerDeckDisplay ();

				_isDeckInitialized = true;

				// Start turns
				if (turnMgr != null) {
					turnMgr.BeginTurn ();
				}

				TakiLogger.LogNetwork ("Master deck setup complete - simplified approach successful");
			} else {
				TakiLogger.LogError ("Master deck setup failed - no starting card", TakiLogger.LogCategory.Network);
			}
		}

		/// <summary>
		/// MILESTONE 1: Receive initial game state from master client
		/// FIXED: Receive and deserialize actual card data
		/// </summary>
		[PunRPC]
		void ReceiveInitialGameState (string startingCardId, int drawCount, string serializedPlayer1Hand, string serializedPlayer2Hand, int masterActor) {
			TakiLogger.LogNetwork ("=== RECEIVED INITIAL GAME STATE RPC ===");
			TakiLogger.LogNetwork ($"Starting Card ID: {startingCardId}");
			TakiLogger.LogNetwork ($"Draw Pile Count: {drawCount}");
			TakiLogger.LogNetwork ($"Player 1 Hand (serialized): {serializedPlayer1Hand}");
			TakiLogger.LogNetwork ($"Player 2 Hand (serialized): {serializedPlayer2Hand}");
			TakiLogger.LogNetwork ($"Master Client Actor: {masterActor}");
			TakiLogger.LogNetwork ($"Local Player Actor: {PhotonNetwork.LocalPlayer.ActorNumber}");
			TakiLogger.LogNetwork ("=== RPC MESSAGE RECEIVED DETAILS LOGGED ===");

			if (!_waitingForDeckState) {
				TakiLogger.LogWarning ("Received game state but wasn't waiting for it", TakiLogger.LogCategory.Network);
				return;
			}

			_waitingForDeckState = false;

			// FIXED: Deserialize actual cards instead of creating placeholders
			List<CardData> player1Hand = DeserializeHand (serializedPlayer1Hand);
			List<CardData> player2Hand = DeserializeHand (serializedPlayer2Hand);

			// Apply the received state to local game
			ApplyReceivedGameState (startingCardId, drawCount, player1Hand, player2Hand, masterActor);
			_isDeckInitialized = true;

			TakiLogger.LogNetwork ("Client deck initialization complete with actual cards");
		}

		/// <summary>
		/// ENHANCED: Apply received game state with simplified hand setup
		/// REPLACES: Complex ApplyReceivedGameState method
		/// INTEGRATION: Uses simplified hand setup
		/// </summary>
		void ApplyReceivedGameState (string startingCardId, int drawCount, List<CardData> player1Hand, List<CardData> player2Hand, int masterActor) {
			if (gameManager?.deckManager == null) {
				TakiLogger.LogError ("Cannot apply game state: Missing components", TakiLogger.LogCategory.Network);
				return;
			}

			TakiLogger.LogNetwork ("Applying received game state with simplified approach");

			// Initialize local deck without full setup
			gameManager.deckManager.InitializeDeck ();

			// Find and place starting card
			CardData startingCard = FindCardFromIdentifier (startingCardId);
			if (startingCard != null) {
				gameManager.deckManager.DiscardCard (startingCard);
				TakiLogger.LogNetwork ($"Starting card placed: {startingCard.GetDisplayText ()}");
			} else {
				TakiLogger.LogWarning ($"Could not find starting card: {startingCardId}", TakiLogger.LogCategory.Network);
			}

			// Setup hands using simplified method
			SetupLocalMultiplayerHands (player1Hand, player2Hand);

			// Update deck display
			UpdateMultiplayerDeckDisplay ();

			// Show ready message
			if (gameManager.GetActiveUI() != null) {
				gameManager.GetActiveUI().ShowPlayerMessage ("Game synchronized - Ready to play!");
			}

			TakiLogger.LogNetwork ("Game state applied successfully with simplified approach");
		}

		/// <summary>
		/// FIXED: Simplified multiplayer hands setup - direct approach without validation corruption
		/// REMOVES: All complex fallback logic that was corrupting hand data
		/// APPROACH: Simple assignment -> GameManager setup -> display
		/// </summary>
		void SetupLocalMultiplayerHands (List<CardData> player1Hand, List<CardData> player2Hand) {
			TakiLogger.LogNetwork ("Setting up multiplayer hands - simplified approach");

			// Validate inputs first
			if (player1Hand == null || player2Hand == null) {
				TakiLogger.LogError ("Cannot setup hands: One or both hands are null", TakiLogger.LogCategory.Network);
				return;
			}

			if (player1Hand.Count == 0 && player2Hand.Count == 0) {
				TakiLogger.LogError ("Cannot setup hands: Both hands are empty", TakiLogger.LogCategory.Network);
				return;
			}

			// Simple player assignment logic - use actor number sorting
			List<Player> sortedPlayers = PhotonNetwork.PlayerList.OrderBy (p => p.ActorNumber).ToList ();

			if (sortedPlayers.Count < 2) {
				TakiLogger.LogError ("Not enough players for hand assignment!", TakiLogger.LogCategory.Network);
				return;
			}

			// Direct assignment - no complex logic
			bool isPlayer1 = (PhotonNetwork.LocalPlayer.ActorNumber == sortedPlayers [0].ActorNumber);
			List<CardData> myHand = isPlayer1 ? player1Hand : player2Hand;
			List<CardData> opponentHand = isPlayer1 ? player2Hand : player1Hand;

			// Simple validation - if my hand is empty, something is wrong with network data
			if (myHand.Count == 0) {
				TakiLogger.LogError ($"CRITICAL: My hand is empty after assignment! Network data problem.", TakiLogger.LogCategory.Network);
				return;
			}

			TakiLogger.LogNetwork ($"Hand assignment: Local={myHand.Count} cards, Opponent={opponentHand.Count} cards");

			// Setup GameManager with our hand - direct approach
			if (gameManager != null) {
				// Clear and add our cards
				gameManager.playerHand.Clear ();
				gameManager.playerHand.AddRange (myHand);

				TakiLogger.LogNetwork ($"GameManager playerHand updated: {gameManager.playerHand.Count} cards");

				// Setup local player hand display using per-screen architecture
				HandManager activePlayerHandManager = gameManager.GetActivePlayerHandManager();
				if (activePlayerHandManager != null) {
					activePlayerHandManager.SetNetworkMode (true);
					activePlayerHandManager.UpdateHandDisplay (myHand);
					TakiLogger.LogNetwork ($"Local player hand displayed: {myHand.Count} cards (per-screen architecture)");
				} else {
					TakiLogger.LogError ("Active player HandManager not found - check per-screen architecture setup", TakiLogger.LogCategory.Network);
				}

				// Setup opponent hand display with REAL CARDS and privacy
				HandManager activeOpponentHandManager = gameManager.GetActiveOpponentHandManager();
				if (activeOpponentHandManager != null) {
					activeOpponentHandManager.SetNetworkModeEnhanced (true, true); // Force opponent mode
					activeOpponentHandManager.InitializeNetworkHandsEnhanced (false, opponentHand);
					TakiLogger.LogNetwork ($"Opponent hand setup with REAL CARDS and privacy: {opponentHand.Count} cards (per-screen architecture)");
				} else {
					TakiLogger.LogError ("Active opponent HandManager not found - check per-screen architecture setup", TakiLogger.LogCategory.Network);
				}

				// Update UI
				if (gameManager.GetActiveUI() != null) {
					gameManager.GetActiveUI().UpdateHandSizeDisplay (myHand.Count, opponentHand.Count);
				}
			}

			TakiLogger.LogNetwork ("Multiplayer hands setup complete - simplified approach successful");
		}

		/// <summary>
		/// DEBUG: Test serialization/deserialization process
		/// Add this method to NetworkGameManager and call it before sending hands
		/// </summary>
		[ContextMenu ("Debug Serialization")]
		void DebugSerialization () {
			TakiLogger.LogNetwork ("=== DEBUGGING SERIALIZATION PROCESS ===");

			// Test with a known hand
			List<CardData> testHand = new List<CardData> ();

			if (gameManager?.deckManager?.cardLoader?.allCardData != null) {
				// Get first 3 cards for testing
				for (int i = 0; i < 3 && i < gameManager.deckManager.cardLoader.allCardData.Count; i++) {
					testHand.Add (gameManager.deckManager.cardLoader.allCardData [i]);
				}

				TakiLogger.LogNetwork ($"Created test hand with {testHand.Count} cards:");
				foreach (CardData card in testHand) {
					TakiLogger.LogNetwork ($"  Test card: {card.GetDisplayText ()}");
				}

				// Test serialization
				string serialized = SerializeHand (testHand);
				TakiLogger.LogNetwork ($"Serialized result: {serialized}");

				// Test deserialization
				List<CardData> deserialized = DeserializeHand (serialized);
				TakiLogger.LogNetwork ($"Deserialized hand: {deserialized.Count} cards");

				foreach (CardData card in deserialized) {
					TakiLogger.LogNetwork ($"  Deserialized card: {card?.GetDisplayText () ?? "NULL"}");
				}

				// Compare
				bool matches = testHand.Count == deserialized.Count;
				for (int i = 0; i < testHand.Count && i < deserialized.Count && matches; i++) {
					if (testHand [i].GetDisplayText () != deserialized [i]?.GetDisplayText ()) {
						matches = false;
					}
				}

				TakiLogger.LogNetwork ($"Serialization test result: {(matches ? "SUCCESS" : "FAILED")}");
			} else {
				TakiLogger.LogError ("Cannot test serialization: CardLoader not available", TakiLogger.LogCategory.Network);
			}
		}

		/// <summary>
		/// ENHANCED: Update deck display for multiplayer games
		/// INTEGRATES: With DeckManager to show draw/discard piles
		/// OBJECTIVE: Make both players see the same deck state
		/// </summary>
		void UpdateMultiplayerDeckDisplay () {
			if (gameManager?.deckManager == null) {
				TakiLogger.LogWarning ("Cannot update deck display: Missing components", TakiLogger.LogCategory.Network);
				return;
			}

			// Force update of deck UI elements
			int drawPileCount = gameManager.deckManager.DrawPileCount;
			int discardPileCount = gameManager.deckManager.DiscardPileCount;
			CardData topDiscardCard = gameManager.deckManager.GetTopDiscardCard ();

			TakiLogger.LogNetwork ($"Updating multiplayer deck display: Draw={drawPileCount}, Discard={discardPileCount}, Top={topDiscardCard?.GetDisplayText ()}");

			// Check if deck UI is properly assigned
			if (gameManager.deckManager.deckUI == null) {
				TakiLogger.LogError ("CRITICAL: deckUI is null! Check Inspector assignments.", TakiLogger.LogCategory.Network);
				return;
			}

			// Check if pile manager is assigned
			TakiLogger.LogNetwork ($"DeckUI PileManager status: {(gameManager.deckManager.deckUI.pileManager != null ? "ASSIGNED" : "NULL")}");

			// Update deck UI if available
			gameManager.deckManager.deckUI.UpdateDeckUI (drawPileCount, discardPileCount);

			if (topDiscardCard != null) {
				gameManager.deckManager.deckUI.UpdateDiscardPileDisplay (topDiscardCard);
			}

			// Update gameplay UI with deck status message (using existing methods)
			if (gameManager.GetActiveUI() != null) {
				gameManager.GetActiveUI().ShowDeckSyncStatus ($"Draw: {drawPileCount}, Discard: {discardPileCount}");
			}

			TakiLogger.LogNetwork ("Multiplayer deck display updated successfully");
		}

		/// <summary>
		/// FIXED: Enhanced debugging for serialization process
		/// </summary>
		string SerializeHand (List<CardData> hand) {
			if (hand == null || hand.Count == 0) {
				TakiLogger.LogNetwork ("SerializeHand: Empty hand being serialized");
				return "";
			}

			List<string> cardIds = new List<string> ();
			foreach (CardData card in hand) {
				if (card != null) {
					string cardId = CardDataHelper.CreateCardIdentifier (card);
					cardIds.Add (cardId);
					TakiLogger.LogNetwork ($"Serializing card: {card.GetDisplayText ()} -> {cardId}", TakiLogger.LogLevel.Trace);
				} else {
					TakiLogger.LogWarning ("Null card found during serialization", TakiLogger.LogCategory.Network);
				}
			}

			string serialized = string.Join ("|", cardIds);
			TakiLogger.LogNetwork ($"Hand serialized: {hand.Count} cards -> {serialized.Length} characters");
			return serialized;
		}

		/// <summary>
		/// FIXED: Enhanced debugging for deserialization process
		/// </summary>
		List<CardData> DeserializeHand (string serializedHand) {
			List<CardData> hand = new List<CardData> ();

			if (string.IsNullOrEmpty (serializedHand)) {
				TakiLogger.LogNetwork ("DeserializeHand: Empty serialized string received");
				return hand;
			}

			TakiLogger.LogNetwork ($"Deserializing hand from: {serializedHand}");

			string [] cardIds = serializedHand.Split ('|');
			TakiLogger.LogNetwork ($"Split into {cardIds.Length} card IDs");

			foreach (string cardId in cardIds) {
				if (!string.IsNullOrEmpty (cardId)) {
					CardData card = FindCardFromIdentifier (cardId);
					if (card != null) {
						hand.Add (card);
						TakiLogger.LogNetwork ($"Deserialized card: {cardId} -> {card.GetDisplayText ()}", TakiLogger.LogLevel.Trace);
					} else {
						TakiLogger.LogWarning ($"Could not find card for ID: {cardId}", TakiLogger.LogCategory.Network);
					}
				}
			}

			TakiLogger.LogNetwork ($"Deserialized hand: {hand.Count} cards from {cardIds.Length} IDs");
			return hand;
		}

		/// <summary>
		/// MILESTONE 1: Find card from network identifier using CardDataHelper
		/// FIXED: No CardData constructors - only finding existing cards
		/// </summary>
		CardData FindCardFromIdentifier (string cardId) {
			if (string.IsNullOrEmpty (cardId)) return null;

			// Get CardDataLoader to find matching card
			CardDataLoader cardLoader = gameManager?.deckManager?.cardLoader;
			if (cardLoader == null) {
				TakiLogger.LogError ("Cannot find card: CardDataLoader not available", TakiLogger.LogCategory.Network);
				return null;
			}

			// Use helper to parse identifier and find matching card
			return CardDataHelper.ParseCardIdentifier (cardLoader, cardId);
		}

		// === EXISTING METHODS PRESERVED ===

		// IPunTurnManagerCallbacks implementation (following instructor's pattern)
		public void OnTurnBegins (int turn) {
			TakiLogger.LogNetwork ($"=== TURN {turn} BEGINS ===");

			// Wait for deck initialization before processing turns
			if (!_isDeckInitialized) {
				TakiLogger.LogNetwork ("Turn begins but deck not initialized yet - waiting...");
				return;
			}

			// Determine whose turn (following instructor's exact pattern)
			int expectedActor = GetExpectedActorForTurn (turn);
			_isMyTurn = PhotonNetwork.LocalPlayer.ActorNumber == expectedActor;

			TakiLogger.LogNetwork ($"Is my turn: {_isMyTurn}");

			// Update GameManager turn state
			if (gameManager != null && gameManager.gameState != null) {
				TurnState newTurnState = _isMyTurn ? TurnState.PlayerTurn : TurnState.ComputerTurn;
				gameManager.gameState.ChangeTurnState (newTurnState);

				// Update UI
				if (gameManager.GetActiveUI() != null) {
					gameManager.GetActiveUI().UpdateTurnDisplayMultiplayer (_isMyTurn);
				}
			}

			if (_isFirstTurn) {
				_isFirstTurn = false;
				TakiLogger.LogNetwork ("First turn initialization complete");
			}
		}

		public void OnPlayerFinished (Player player, int turn, object move) {
			TakiLogger.LogNetwork ($"=== PLAYER {player.ActorNumber} FINISHED TURN {turn} ===");

			// Process remote player action
			if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber && move != null) {
				ProcessRemoteAction (player, move);
			}

			// Master client advances turn
			if (PhotonNetwork.IsMasterClient && turnMgr != null) {
				turnMgr.BeginTurn ();
			}
		}

		public void OnTurnCompleted (int turn) { }
		public void OnPlayerMove (Player player, int turn, object move) { }
		public void OnTurnTimeEnds (int turn) { }

		/// <summary>
		/// Get expected actor for turn (instructor's exact pattern)
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
		/// Send card play to network
		/// </summary>
		public void SendCardPlay (CardData card) {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			string cardId = GetCardIdentifier (card);

			var moveData = new NetworkMoveData {
				actionType = "PLAY_CARD",
				cardIdentifier = cardId
			};

			turnMgr.SendMove (moveData, true);
			TakiLogger.LogNetwork ($"Sent card play: {cardId}");
		}

		/// <summary>
		/// Send card draw to network
		/// </summary>
		public void SendCardDraw () {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			var moveData = new NetworkMoveData {
				actionType = "DRAW_CARD",
				cardIdentifier = ""
			};

			turnMgr.SendMove (moveData, true);
			TakiLogger.LogNetwork ("Sent card draw");
		}

		/// <summary>
		/// Process action from remote player
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
		/// Get card identifier for network using helper
		/// </summary>
		string GetCardIdentifier (CardData card) {
			return CardDataHelper.CreateCardIdentifier (card);
		}

		// Properties
		public bool IsMyTurn => _isMyTurn;
		public bool IsNetworkGameActive => !_isGameOver;
		public bool IsDeckInitialized => _isDeckInitialized;
	}

	/// <summary>
	/// MILESTONE 1: Network initial game state data
	/// </summary>
	[System.Serializable]
	public class NetworkInitialGameState {
		public string startingCardIdentifier;
		public int drawPileCount;
		public int player1HandSize;
		public int player2HandSize;
		public int masterClientActor;
	}

	/// <summary>
	/// Simple network move data (preserved from existing)
	/// </summary>
	[System.Serializable]
	public class NetworkMoveData {
		public string actionType;
		public string cardIdentifier;
	}

	/// <summary>
	/// NetworkGameState class to hold complete game state
	/// </summary>
	[System.Serializable]
	public class NetworkGameState {
		public string startingCardIdentifier;
		public int drawPileCount;
		public List<CardData> player1Hand;  // Actual cards
		public List<CardData> player2Hand;  // Actual cards
		public int masterClientActor;
	}
}