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

			// DEBUGGING: Log Photon connection state
			TakiLogger.LogNetwork ($"PHOTON DEBUG: IsConnected={PhotonNetwork.IsConnected}");
			TakiLogger.LogNetwork ($"PHOTON DEBUG: IsConnectedAndReady={PhotonNetwork.IsConnectedAndReady}");
			TakiLogger.LogNetwork ($"PHOTON DEBUG: InRoom={PhotonNetwork.InRoom}");
			TakiLogger.LogNetwork ($"PHOTON DEBUG: CurrentRoom={PhotonNetwork.CurrentRoom?.Name ?? "NULL"}");
			TakiLogger.LogNetwork ($"PHOTON DEBUG: PlayerCount={PhotonNetwork.CurrentRoom?.PlayerCount ?? 0}");
			TakiLogger.LogNetwork ($"PHOTON DEBUG: IsMasterClient={PhotonNetwork.IsMasterClient}");
			TakiLogger.LogNetwork ($"PHOTON DEBUG: LocalPlayer ActorNumber={PhotonNetwork.LocalPlayer?.ActorNumber ?? -1}");
			TakiLogger.LogNetwork ($"PHOTON DEBUG: MasterClient ActorNumber={PhotonNetwork.MasterClient?.ActorNumber ?? -1}");

			// Log all players in room
			if (PhotonNetwork.CurrentRoom != null) {
				TakiLogger.LogNetwork ($"PHOTON DEBUG: Players in room:");
				foreach (var player in PhotonNetwork.PlayerList) {
					TakiLogger.LogNetwork ($"  - Player {player.ActorNumber}: {player.NickName} (Master: {player.IsMasterClient})");
				}
			}

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
		void   InitializeSharedDeck () {
			TakiLogger.LogNetwork ("=== INITIALIZING SHARED DECK ===");

			// DEBUGGING: Double-check Master Client status
			TakiLogger.LogNetwork ($"DECK INIT DEBUG: PhotonNetwork.IsMasterClient={PhotonNetwork.IsMasterClient}");
			TakiLogger.LogNetwork ($"DECK INIT DEBUG: LocalPlayer.ActorNumber={PhotonNetwork.LocalPlayer?.ActorNumber ?? -1}");
			TakiLogger.LogNetwork ($"DECK INIT DEBUG: MasterClient.ActorNumber={PhotonNetwork.MasterClient?.ActorNumber ?? -1}");
			TakiLogger.LogNetwork ($"DECK INIT DEBUG: _waitingForDeckState={_waitingForDeckState}");

			if (PhotonNetwork.IsMasterClient) {
				TakiLogger.LogNetwork ("TAKING MASTER PATH: I am Master Client - setting up deck and broadcasting state");
				SetupMasterDeck ();
			} else {
				TakiLogger.LogNetwork ("TAKING CLIENT PATH: I am Client - waiting for initial game state from master");
				_waitingForDeckState = true;
				TakiLogger.LogNetwork ($"DECK INIT DEBUG: _waitingForDeckState set to {_waitingForDeckState}");
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

				// DIAGNOSTIC: Check hands before passing to SetupLocalMultiplayerHands
				TakiLogger.LogNetwork ($"DIAGNOSTIC: Before SetupLocalMultiplayerHands - P1 Count={gameState.player1Hand.Count}, P2 Count={gameState.player2Hand.Count}");
				if (gameState.player1Hand.Count > 0) {
					TakiLogger.LogNetwork ($"DIAGNOSTIC: P1 First card exists: {gameState.player1Hand[0] != null}");
					if (gameState.player1Hand[0] != null) {
						TakiLogger.LogNetwork ($"DIAGNOSTIC: P1 First card: {gameState.player1Hand[0].GetDisplayText()}");
					}
				}
				if (gameState.player2Hand.Count > 0) {
					TakiLogger.LogNetwork ($"DIAGNOSTIC: P2 First card exists: {gameState.player2Hand[0] != null}");
					if (gameState.player2Hand[0] != null) {
						TakiLogger.LogNetwork ($"DIAGNOSTIC: P2 First card: {gameState.player2Hand[0].GetDisplayText()}");
					}
				}

				// MASTER DIAGNOSTIC: Enhanced logging for master client
				TakiLogger.LogNetwork ($"=== MASTER CLIENT HAND ASSIGNMENT DEBUG ===");
				TakiLogger.LogNetwork ($"Master ActorNumber: {PhotonNetwork.LocalPlayer.ActorNumber}");
				TakiLogger.LogNetwork ($"Master should be isPlayer1=True and get player1Hand");
				TakiLogger.LogNetwork ($"About to call SetupLocalMultiplayerHands with P1={gameState.player1Hand.Count} cards, P2={gameState.player2Hand.Count} cards");

				// Setup local state using simplified method
				SetupLocalMultiplayerHands (gameState.player1Hand, gameState.player2Hand);

				// MASTER DIAGNOSTIC: Check GameManager.playerHand after setup
				if (gameManager != null) {
					TakiLogger.LogNetwork ($"=== MASTER POST-SETUP DIAGNOSTIC ===");
					TakiLogger.LogNetwork ($"Master GameManager.playerHand count after setup: {gameManager.playerHand.Count}");
					if (gameManager.playerHand.Count > 0 && gameManager.playerHand[0] != null) {
						TakiLogger.LogNetwork ($"Master GameManager.playerHand first card: {gameManager.playerHand[0].GetDisplayText()}");
					} else {
						TakiLogger.LogError ($"CRITICAL: Master has NO CARDS after SetupLocalMultiplayerHands!", TakiLogger.LogCategory.Network);
					}
				}

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

			// DEBUGGING: Log why we might reject this RPC
			TakiLogger.LogNetwork ($"RPC DEBUG: _waitingForDeckState={_waitingForDeckState}");
			TakiLogger.LogNetwork ($"RPC DEBUG: PhotonNetwork.IsMasterClient={PhotonNetwork.IsMasterClient}");
			TakiLogger.LogNetwork ($"RPC DEBUG: sender masterActor={masterActor}, local ActorNumber={PhotonNetwork.LocalPlayer.ActorNumber}");

			if (!_waitingForDeckState) {
				TakiLogger.LogWarning ($"REJECTING RPC: Received game state but wasn't waiting for it (_waitingForDeckState={_waitingForDeckState})", TakiLogger.LogCategory.Network);
				TakiLogger.LogWarning ($"RPC REJECT REASON: This client thinks it's Master={PhotonNetwork.IsMasterClient}, so it didn't set _waitingForDeckState=true", TakiLogger.LogCategory.Network);
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

			// FIXED: Initialize deck but then sync to master's count
			gameManager.deckManager.InitializeDeck ();
			TakiLogger.LogNetwork ($"Network deck initialized with {gameManager.deckManager.DrawPileCount} cards");

			// Find and place starting card
			CardData startingCard = FindCardFromIdentifier (startingCardId);
			if (startingCard != null) {
				gameManager.deckManager.DiscardCard (startingCard);
				TakiLogger.LogNetwork ($"Starting card placed: {startingCard.GetDisplayText ()}");

				// CRITICAL FIX: Set active color from starting card (same as master does)
				if (gameManager.gameState != null) {
					gameManager.gameState.ChangeActiveColor (startingCard.color);
					TakiLogger.LogNetwork ($"COLOR SYNC: Active color set to {startingCard.color} from starting card");
				}
			} else {
				TakiLogger.LogWarning ($"Could not find starting card: {startingCardId}", TakiLogger.LogCategory.Network);
			}

			// CRITICAL FIX: Sync deck count to master's state
			// The master dealt cards and has the correct count, we need to match it
			int currentDrawCount = gameManager.deckManager.DrawPileCount;
			if (currentDrawCount != drawCount) {
				TakiLogger.LogNetwork ($"DECK SYNC: Adjusting draw pile from {currentDrawCount} to {drawCount} to match master");
				// Adjust the deck count to match master's state
				gameManager.deckManager.SyncDrawPileCount (drawCount);
				TakiLogger.LogNetwork ($"DECK SYNC: Draw pile count synchronized to {gameManager.deckManager.DrawPileCount}");
			}

			// Setup hands using simplified method
			SetupLocalMultiplayerHands (player1Hand, player2Hand);

			// Update deck display with correct count
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

			// DIAGNOSTIC: Log player assignment details
			TakiLogger.LogNetwork ($"DIAGNOSTIC: Player assignment setup");
			TakiLogger.LogNetwork ($"DIAGNOSTIC: Local ActorNumber={PhotonNetwork.LocalPlayer.ActorNumber}");
			TakiLogger.LogNetwork ($"DIAGNOSTIC: Total players={sortedPlayers.Count}");
			for (int i = 0; i < sortedPlayers.Count; i++) {
				TakiLogger.LogNetwork ($"DIAGNOSTIC: Player[{i}] ActorNumber={sortedPlayers[i].ActorNumber}");
			}
			TakiLogger.LogNetwork ($"DIAGNOSTIC: Input hands - Player1: {player1Hand.Count} cards, Player2: {player2Hand.Count} cards");

			// DIAGNOSTIC: Check if the hands actually contain cards or just have count
			if (player1Hand.Count > 0) {
				TakiLogger.LogNetwork ($"DIAGNOSTIC: Player1Hand[0] is null: {player1Hand[0] == null}");
				if (player1Hand[0] != null) {
					TakiLogger.LogNetwork ($"DIAGNOSTIC: Player1Hand[0]: {player1Hand[0].GetDisplayText()}");
				}
			}
			if (player2Hand.Count > 0) {
				TakiLogger.LogNetwork ($"DIAGNOSTIC: Player2Hand[0] is null: {player2Hand[0] == null}");
				if (player2Hand[0] != null) {
					TakiLogger.LogNetwork ($"DIAGNOSTIC: Player2Hand[0]: {player2Hand[0].GetDisplayText()}");
				}
			}

			if (sortedPlayers.Count < 2) {
				TakiLogger.LogError ("Not enough players for hand assignment!", TakiLogger.LogCategory.Network);
				return;
			}

			// Direct assignment - no complex logic
			bool isPlayer1 = (PhotonNetwork.LocalPlayer.ActorNumber == sortedPlayers [0].ActorNumber);
			TakiLogger.LogNetwork ($"DIAGNOSTIC: isPlayer1={isPlayer1} (Local actor {PhotonNetwork.LocalPlayer.ActorNumber} vs First player {sortedPlayers[0].ActorNumber})");

			List<CardData> myHand = isPlayer1 ? player1Hand : player2Hand;
			List<CardData> opponentHand = isPlayer1 ? player2Hand : player1Hand;

			// DIAGNOSTIC: Log assignment results
			TakiLogger.LogNetwork ($"DIAGNOSTIC: After assignment - myHand: {myHand.Count} cards, opponentHand: {opponentHand.Count} cards");

			// Simple validation - if my hand is empty, something is wrong with network data
			if (myHand.Count == 0) {
				TakiLogger.LogError ($"CRITICAL: My hand is empty after assignment! Network data problem.", TakiLogger.LogCategory.Network);
				return;
			}

			TakiLogger.LogNetwork ($"Hand assignment: Local={myHand.Count} cards, Opponent={opponentHand.Count} cards");

			// Setup GameManager with our hand - direct approach
			if (gameManager != null) {
				// DIAGNOSTIC: Log before clearing
				TakiLogger.LogNetwork ($"DIAGNOSTIC: GameManager.playerHand before clear: {gameManager.playerHand.Count} cards");
				TakiLogger.LogNetwork ($"DIAGNOSTIC: About to add {myHand.Count} cards to GameManager.playerHand");

				// CRITICAL: Check if they're the same object reference!
				bool sameReference = ReferenceEquals(gameManager.playerHand, myHand);
				TakiLogger.LogNetwork ($"DIAGNOSTIC: CRITICAL - gameManager.playerHand == myHand reference: {sameReference}", sameReference ? TakiLogger.LogLevel.Info : TakiLogger.LogLevel.Info);

				// FIX: Create a copy of myHand BEFORE clearing to avoid reference equality bug
				// The bug was: myHand and gameManager.playerHand pointed to the same list
				// So Clear() would empty myHand too, causing AddRange to add 0 cards
				List<CardData> myHandCopy = new List<CardData>(myHand);
				TakiLogger.LogNetwork ($"DIAGNOSTIC: Created myHandCopy with {myHandCopy.Count} cards");

				// Clear and add our cards
				gameManager.playerHand.Clear ();
				TakiLogger.LogNetwork ($"DIAGNOSTIC: GameManager.playerHand after clear: {gameManager.playerHand.Count} cards");

				// DIAGNOSTIC: Verify myHand after clear (should be empty if same reference)
				TakiLogger.LogNetwork ($"DIAGNOSTIC: myHand after clear - Count: {myHand.Count}");

				gameManager.playerHand.AddRange (myHandCopy);
				TakiLogger.LogNetwork ($"DIAGNOSTIC: GameManager.playerHand after AddRange: {gameManager.playerHand.Count} cards");

				TakiLogger.LogNetwork ($"GameManager playerHand updated: {gameManager.playerHand.Count} cards");

				// Setup local player hand display using per-screen architecture
				HandManager activePlayerHandManager = gameManager.GetActivePlayerHandManager();
				if (activePlayerHandManager != null) {
					activePlayerHandManager.SetNetworkMode (true);
					TakiLogger.LogNetwork ($"*** REFERENCE FIX VERIFICATION: Using myHandCopy ({myHandCopy.Count} cards) instead of myHand ({myHand.Count} cards) ***");
					activePlayerHandManager.UpdateHandDisplay (myHandCopy);  // FIX: Use myHandCopy instead of myHand
					TakiLogger.LogNetwork ($"Local player hand displayed: {myHandCopy.Count} cards (per-screen architecture) - FIXED VERSION");
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
					gameManager.GetActiveUI().UpdateHandSizeDisplay (myHandCopy.Count, opponentHand.Count);  // FIX: Use myHandCopy instead of myHand
				}
			}

			TakiLogger.LogNetwork ("Multiplayer hands setup complete - simplified approach successful");

			// FIXED: Set game as active after successful multiplayer setup
			if (gameManager != null) {
				gameManager.SetGameActive(true);
				TakiLogger.LogNetwork ("Game activated after multiplayer hands setup");
			}
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
		public void UpdateMultiplayerDeckDisplay () {
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
		public void OnPlayerMove (Player player, int turn, object move) {
		TakiLogger.LogNetwork ($"=== PLAYER {player.ActorNumber} MADE MOVE IN TURN {turn} ===");

		// Process remote player action (non-finishing move)
		if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber && move != null) {
			ProcessRemoteAction (player, move);
		}

		// Note: Turn does NOT advance for non-finishing moves
	}
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

			var moveData = new ExitGames.Client.Photon.Hashtable {
				{"actionType", "PLAY_CARD"},
				{"cardIdentifier", cardId}
			};

			turnMgr.SendMove (moveData, false);
			TakiLogger.LogNetwork ($"Sent card play: {cardId}");
		}

		/// <summary>
		/// Send card draw to network
		/// </summary>
		public void SendCardDraw () {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			var moveData = new ExitGames.Client.Photon.Hashtable {
				{"actionType", "DRAW_CARD"},
				{"cardIdentifier", ""}
			};

			turnMgr.SendMove (moveData, false);
			TakiLogger.LogNetwork ("Sent card draw");
		}

		/// <summary>
		/// Send color selection to network
		/// </summary>
		public void SendColorSelection (CardColor selectedColor) {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			var moveData = new ExitGames.Client.Photon.Hashtable {
				{"actionType", "COLOR_SELECTION"},
				{"cardIdentifier", selectedColor.ToString ()}
			};

			turnMgr.SendMove (moveData, false);
			TakiLogger.LogNetwork ($"Sent color selection: {selectedColor}");
		}

		/// <summary>
		/// Send end turn to network
		/// </summary>
		public void SendEndTurn () {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			var moveData = new ExitGames.Client.Photon.Hashtable {
				{"actionType", "END_TURN"},
				{"cardIdentifier", ""}
			};

			turnMgr.SendMove (moveData, true);
			TakiLogger.LogNetwork ("Sent end turn");
		}

		/// <summary>
		/// Send end TAKI sequence to network
		/// </summary>
		public void SendEndTakiSequence () {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			var moveData = new ExitGames.Client.Photon.Hashtable {
				{"actionType", "END_TAKI_SEQUENCE"},
				{"cardIdentifier", ""}
			};

			turnMgr.SendMove (moveData, false);  // CRITICAL FIX: Don't auto-advance turns
			TakiLogger.LogNetwork ("Sent end TAKI sequence");
		}

		/// <summary>
		/// Send STOP card effect to network
		/// </summary>
		public void SendStopEffect () {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			var moveData = new ExitGames.Client.Photon.Hashtable {
				{"actionType", "STOP_EFFECT"},
				{"cardIdentifier", ""}
			};

			turnMgr.SendMove (moveData, false);  // CRITICAL FIX: Don't auto-advance turns
			TakiLogger.LogNetwork ("Sent STOP effect");
		}

		/// <summary>
		/// Send direction change effect to network
		/// </summary>
		public void SendDirectionChange () {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			var moveData = new ExitGames.Client.Photon.Hashtable {
				{"actionType", "DIRECTION_CHANGE"},
				{"cardIdentifier", ""}
			};

			turnMgr.SendMove (moveData, false);  // CRITICAL FIX: Don't auto-advance turns
			TakiLogger.LogNetwork ("Sent direction change");
		}

		/// <summary>
		/// Send plus-two chain break to network
		/// </summary>
		public void SendChainBreak () {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			// CRITICAL FIX: Include chain draw count in the message
			int chainDrawCount = 2; // Default for single PlusTwo
			if (gameManager != null && gameManager.gameState != null && gameManager.gameState.IsPlusTwoChainActive) {
				chainDrawCount = gameManager.gameState.ChainDrawCount;
				TakiLogger.LogNetwork ($"Sending chain break with draw count: {chainDrawCount}");
			}

			var moveData = new ExitGames.Client.Photon.Hashtable {
				{"actionType", "CHAIN_BREAK"},
				{"cardIdentifier", chainDrawCount.ToString()}
			};

			turnMgr.SendMove (moveData, false);  // CRITICAL FIX: Don't auto-advance turns
			TakiLogger.LogNetwork ($"Sent plus-two chain break with {chainDrawCount} cards");
		}

		/// <summary>
		/// Send PlusTwo effect to network with chain information
		/// </summary>
		public void SendPlusTwoEffect (int chainCount, int drawCount) {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			var moveData = new ExitGames.Client.Photon.Hashtable {
				{"actionType", "PLUS_TWO_EFFECT"},
				{"cardIdentifier", $"{chainCount},{drawCount}"} // Encode chain info in identifier
			};

			turnMgr.SendMove (moveData, false);  // CRITICAL FIX: Don't auto-advance turns
			TakiLogger.LogNetwork ($"Sent PlusTwo effect: {chainCount} cards, {drawCount} total draw");
		}

		/// <summary>
		/// Process action from remote player
		/// </summary>
		void ProcessRemoteAction (Player player, object moveData) {
			if (gameManager == null) {
				TakiLogger.LogError ("NetworkGameManager: ProcessRemoteAction - gameManager is null!", TakiLogger.LogCategory.Network);
				return;
			}

			if (moveData is ExitGames.Client.Photon.Hashtable networkMove) {
				string actionType = (string)networkMove["actionType"];
				string cardIdentifier = (string)networkMove["cardIdentifier"];

				TakiLogger.LogNetwork($"Processing network move: actionType='{actionType}', cardIdentifier='{cardIdentifier}'");

				switch (actionType) {
					case "PLAY_CARD":
						gameManager.ProcessNetworkCardPlay (cardIdentifier, player.ActorNumber);
						break;
					case "DRAW_CARD":
						gameManager.ProcessNetworkCardDraw (player.ActorNumber);
						break;
					case "COLOR_SELECTION":
						// Parse color from string and process
						if (System.Enum.TryParse<CardColor> (cardIdentifier, out CardColor selectedColor)) {
							gameManager.ProcessNetworkColorSelection (selectedColor, player.ActorNumber);
						} else {
							TakiLogger.LogError ($"Invalid color selection received: {cardIdentifier}", TakiLogger.LogCategory.Network);
						}
						break;
					case "END_TURN":
						gameManager.ProcessNetworkEndTurn (player.ActorNumber);
						break;
					case "END_TAKI_SEQUENCE":
						gameManager.ProcessNetworkEndTakiSequence (player.ActorNumber);
						break;
					case "STOP_EFFECT":
						gameManager.ProcessNetworkStopEffect (player.ActorNumber);
						break;
					case "DIRECTION_CHANGE":
						gameManager.ProcessNetworkDirectionChange (player.ActorNumber);
						break;
					case "CHAIN_BREAK":
						TakiLogger.LogNetwork($"CHAIN_BREAK message received from actor {player.ActorNumber}");
						// Parse chain draw count from cardIdentifier
						int chainDrawCount = -1;
						if (!string.IsNullOrEmpty(cardIdentifier) && int.TryParse(cardIdentifier, out int parsedCount)) {
							chainDrawCount = parsedCount;
							TakiLogger.LogNetwork($"Parsed chain draw count from network: {chainDrawCount}");
						} else {
							TakiLogger.LogNetwork($"Failed to parse chain draw count from: '{cardIdentifier}'");
						}
						gameManager.ProcessNetworkChainBreak (player.ActorNumber, chainDrawCount);
						break;
					case "PLUS_TWO_EFFECT":
						// Parse chain info from cardIdentifier
						if (!string.IsNullOrEmpty (cardIdentifier)) {
							string[] parts = cardIdentifier.Split (',');
							if (parts.Length == 2 && int.TryParse (parts[0], out int chainCount) && int.TryParse (parts[1], out int drawCount)) {
								gameManager.ProcessNetworkPlusTwoEffect (chainCount, drawCount, player.ActorNumber);
							} else {
								TakiLogger.LogError ($"Invalid PlusTwo effect data: {cardIdentifier}", TakiLogger.LogCategory.Network);
							}
						} else {
							TakiLogger.LogError ("PlusTwo effect received with empty data", TakiLogger.LogCategory.Network);
						}
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