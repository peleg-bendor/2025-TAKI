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

		// Network state tracking
		private bool _isMyTurn = false;          // True when it's this client's turn to play
		private bool _isGameOver = false;        // True when game has ended
		private bool _isFirstTurn = true;        // True until first turn is processed
		private bool _isDeckInitialized = false; // True when deck sync is complete

		// Deck initialization coordination
		private bool _waitingForDeckState = false;        // Client waiting for master's deck state
		private NetworkInitialGameState _pendingGameState; // Temporary storage for received state

		void Awake () {
			// Register this component to receive turn management callbacks
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

			// Log current Photon connection state for debugging
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

			// Reset game state for new network game
			_isGameOver = false;
			_isFirstTurn = true;
			_isDeckInitialized = false;

			// Start deck synchronization process
			InitializeSharedDeck ();
		}

		/// <summary>
		/// Initialize shared deck with master/client coordination
		/// Master creates deck and sends state to clients, clients wait to receive
		/// </summary>
		void InitializeSharedDeck () {
			TakiLogger.LogNetwork ("=== INITIALIZING SHARED DECK ===");

			// DEBUGGING: Double-check Master Client status
			TakiLogger.LogNetwork ($"DECK INIT DEBUG: PhotonNetwork.IsMasterClient={PhotonNetwork.IsMasterClient}");
			TakiLogger.LogNetwork ($"DECK INIT DEBUG: LocalPlayer.ActorNumber={PhotonNetwork.LocalPlayer?.ActorNumber ?? -1}");
			TakiLogger.LogNetwork ($"DECK INIT DEBUG: MasterClient.ActorNumber={PhotonNetwork.MasterClient?.ActorNumber ?? -1}");
			TakiLogger.LogNetwork ($"DECK INIT DEBUG: _waitingForDeckState={_waitingForDeckState}");

			// Master client creates and distributes deck state
			if (PhotonNetwork.IsMasterClient) {
				TakiLogger.LogNetwork ("MASTER: Setting up deck and broadcasting to clients");
				SetupMasterDeck ();
			}
			// Client waits to receive deck state from master
			else {
				TakiLogger.LogNetwork ("CLIENT: Waiting for deck state from master");
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
			
			// Use DeckManager to create initial game state (deck + hands + starting card)
			var gameState = gameManager.deckManager.SetupInitialGame ();

			if (gameState.startingCard != null) {
				// Prepare network message with serialized game state
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

				// Send complete game state to all other clients via RPC
				photonView.RPC ("ReceiveInitialGameState", RpcTarget.Others,
					startingCardId,                               // Starting discard card
					gameManager.deckManager.DrawPileCount,        // Remaining deck size
					serializedPlayer1Hand,                        // Player 1's cards (serialized)
					serializedPlayer2Hand,                        // Player 2's cards (serialized)
					PhotonNetwork.LocalPlayer.ActorNumber);       // Master's actor number

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

				// Setup master's local game state with the generated hands
				SetupLocalMultiplayerHands (gameState.player1Hand, gameState.player2Hand);

				// Verify master setup completed correctly
				if (gameManager != null) {
					TakiLogger.LogNetwork ($"=== MASTER POST-SETUP DIAGNOSTIC ===");
					TakiLogger.LogNetwork ($"Master GameManager.playerHand count after setup: {gameManager.playerHand.Count}");
					if (gameManager.playerHand.Count > 0 && gameManager.playerHand[0] != null) {
						TakiLogger.LogNetwork ($"Master GameManager.playerHand first card: {gameManager.playerHand[0].GetDisplayText()}");
					} else {
						TakiLogger.LogError ($"CRITICAL: Master has NO CARDS after SetupLocalMultiplayerHands!", TakiLogger.LogCategory.Network);
					}
				}

				// Update visual deck display (draw pile, discard pile)
				UpdateMultiplayerDeckDisplay ();

				// Mark deck initialization as complete
				_isDeckInitialized = true;

				// Begin turn-based gameplay
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
		// RPC called on clients to receive initial game state from master
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

			// Only process if we were actually waiting for deck state (safety check)
			if (!_waitingForDeckState) {
				TakiLogger.LogWarning ("Ignoring unexpected deck state RPC", TakiLogger.LogCategory.Network);
				return;
			}

			// Clear waiting flag
			_waitingForDeckState = false;

			// Convert serialized hand data back to actual CardData objects
			List<CardData> player1Hand = DeserializeHand (serializedPlayer1Hand);
			List<CardData> player2Hand = DeserializeHand (serializedPlayer2Hand);

			// Apply the master's game state to this client's local game
			ApplyReceivedGameState (startingCardId, drawCount, player1Hand, player2Hand, masterActor);

			// Mark initialization as complete
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

			// Initialize local deck, then sync to master's state
			gameManager.deckManager.InitializeDeck ();
			TakiLogger.LogNetwork ($"Client deck initialized with {gameManager.deckManager.DrawPileCount} cards");

			// Recreate the starting discard card and set active color
			CardData startingCard = FindCardFromIdentifier (startingCardId);
			if (startingCard != null) {
				// Place starting card on discard pile
				gameManager.deckManager.DiscardCard (startingCard);
				TakiLogger.LogNetwork ($"Starting card placed: {startingCard.GetDisplayText ()}");

				// Sync active color to match master's game state
				if (gameManager.gameState != null) {
					gameManager.gameState.ChangeActiveColor (startingCard.color);
					TakiLogger.LogNetwork ($"Active color synced to {startingCard.color}");
				}
			} else {
				TakiLogger.LogWarning ($"Could not find starting card: {startingCardId}", TakiLogger.LogCategory.Network);
			}

			// Sync draw pile count to exactly match master's state
			int currentDrawCount = gameManager.deckManager.DrawPileCount;
			if (currentDrawCount != drawCount) {
				TakiLogger.LogNetwork ($"Syncing draw pile: {currentDrawCount} → {drawCount}");
				gameManager.deckManager.SyncDrawPileCount (drawCount);
				TakiLogger.LogNetwork ($"Draw pile synchronized to {gameManager.deckManager.DrawPileCount}");
			}

			// Setup client's local hands using received data
			SetupLocalMultiplayerHands (player1Hand, player2Hand);

			// Update visual deck display to match synchronized state
			UpdateMultiplayerDeckDisplay ();

			// Show ready message
			if (gameManager.GetActiveUI() != null) {
				gameManager.GetActiveUI().ShowPlayerMessage ("Game synchronized - Ready to play!");
			}

			TakiLogger.LogNetwork ("Game state applied successfully with simplified approach");
		}

		/// <summary>
		/// Setup local player hands based on actor number and received hand data
		/// Handles both master and client initialization with proper hand assignment
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

			// Assign hands based on actor numbers (lowest actor = player1, highest = player2)
			List<Player> sortedPlayers = PhotonNetwork.PlayerList.OrderBy (p => p.ActorNumber).ToList ();

			// Log player assignment for debugging
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

			// Determine which hand belongs to this client
			bool isPlayer1 = (PhotonNetwork.LocalPlayer.ActorNumber == sortedPlayers [0].ActorNumber);
			TakiLogger.LogNetwork ($"Player assignment: isPlayer1={isPlayer1} (Actor {PhotonNetwork.LocalPlayer.ActorNumber})");

			// Assign my hand and opponent's hand accordingly
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

				// Check for reference equality bug (defensive programming)
				bool sameReference = ReferenceEquals(gameManager.playerHand, myHand);
				if (sameReference) {
					TakiLogger.LogNetwork ("WARNING: Reference equality detected - creating defensive copy");
				}

				// Create defensive copy to prevent reference equality bugs
				List<CardData> myHandCopy = new List<CardData>(myHand);
				TakiLogger.LogNetwork ($"Created hand copy with {myHandCopy.Count} cards");

				// Replace GameManager's hand with our cards
				gameManager.playerHand.Clear ();
				gameManager.playerHand.AddRange (myHandCopy);
				TakiLogger.LogNetwork ($"GameManager hand updated: {gameManager.playerHand.Count} cards");

				TakiLogger.LogNetwork ($"GameManager playerHand updated: {gameManager.playerHand.Count} cards");

				// Setup visual display for local player's hand
				HandManager activePlayerHandManager = gameManager.GetActivePlayerHandManager();
				if (activePlayerHandManager != null) {
					activePlayerHandManager.SetNetworkMode (true);
					activePlayerHandManager.UpdateHandDisplay (myHandCopy);
					TakiLogger.LogNetwork ($"Player hand display updated: {myHandCopy.Count} cards");
				} else {
					TakiLogger.LogError ("Player HandManager not found", TakiLogger.LogCategory.Network);
				}

				// Setup visual display for opponent's hand (card backs for privacy)
				HandManager activeOpponentHandManager = gameManager.GetActiveOpponentHandManager();
				if (activeOpponentHandManager != null) {
					activeOpponentHandManager.SetNetworkModeEnhanced (true, true); // Network + opponent mode
					activeOpponentHandManager.InitializeNetworkHandsEnhanced (false, opponentHand);
					TakiLogger.LogNetwork ($"Opponent hand display setup: {opponentHand.Count} cards (hidden)");
				} else {
					TakiLogger.LogError ("Opponent HandManager not found", TakiLogger.LogCategory.Network);
				}

				// Update UI hand size displays
				if (gameManager.GetActiveUI() != null) {
					gameManager.GetActiveUI().UpdateHandSizeDisplay (myHandCopy.Count, opponentHand.Count);
				}
			}

			TakiLogger.LogNetwork ("Multiplayer hands setup complete - simplified approach successful");

			// Activate game state after successful setup
			if (gameManager != null) {
				gameManager.SetGameActive(true);
				TakiLogger.LogNetwork ("Multiplayer game activated");
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

		#region Turn Management Callbacks
		// Called by PunTurnManager when a new turn begins
		public void OnTurnBegins (int turn) {
			TakiLogger.LogNetwork ($"=== TURN {turn} BEGINS ===");

			// Don't process turns until deck synchronization is complete
			if (!_isDeckInitialized) {
				TakiLogger.LogNetwork ("Waiting for deck initialization...");
				return;
			}

			// Calculate which player should play this turn
			int expectedActor = GetExpectedActorForTurn (turn);
			_isMyTurn = PhotonNetwork.LocalPlayer.ActorNumber == expectedActor;

			TakiLogger.LogNetwork ($"Is my turn: {_isMyTurn}");

			// Update GameManager's turn state to reflect network turn
			if (gameManager != null && gameManager.gameState != null) {
				// In multiplayer, "ComputerTurn" actually means "opponent's turn"
				TurnState newTurnState = _isMyTurn ? TurnState.PlayerTurn : TurnState.ComputerTurn;
				gameManager.gameState.ChangeTurnState (newTurnState);
			}

			if (_isFirstTurn) {
				_isFirstTurn = false;
				TakiLogger.LogNetwork ("First turn initialization complete");
			}
		}

		// Called when a player finishes their turn (sends END_TURN)
		public void OnPlayerFinished (Player player, int turn, object move) {
			TakiLogger.LogNetwork ($"Player {player.ActorNumber} finished turn {turn}");

			// Process the finishing move if it came from another player
			if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber && move != null) {
				ProcessRemoteAction (player, move);
			}

			// Master client is responsible for advancing to next turn
			if (PhotonNetwork.IsMasterClient && turnMgr != null) {
				turnMgr.BeginTurn ();
			}
		}

		// Called when turn is completed (unused)
		public void OnTurnCompleted (int turn) { }

		// Called when a player makes a non-finishing move (PLAY_CARD, DRAW_CARD, etc.)
		public void OnPlayerMove (Player player, int turn, object move) {
			TakiLogger.LogNetwork ($"Player {player.ActorNumber} made move in turn {turn}");

			// Process the move if it came from another player
			if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber && move != null) {
				ProcessRemoteAction (player, move);
			}

			// Note: Turn does NOT advance - player must send END_TURN separately
		}

		// Called when turn time limit expires (unused)
		public void OnTurnTimeEnds (int turn) { }

		#endregion

		/// <summary>
		/// Calculate which player's turn it should be based on turn number
		/// Uses sorted actor numbers to ensure consistent turn order across clients
		/// </summary>
		int GetExpectedActorForTurn (int turn) {
			var room = PhotonNetwork.CurrentRoom;
			if (room == null) return -1;

			// Get all player actor numbers and sort them
			var list = new System.Collections.Generic.List<int> ();
			foreach (var kvp in room.Players) {
				list.Add (kvp.Key);  // kvp.Key is the actor number
			}
			list.Sort ();  // Ensure consistent order across all clients

			if (list.Count == 0) return -1;

			// Use modulo to cycle through players (turn 1 = player 0, turn 2 = player 1, etc.)
			int idx = (turn - 1) % list.Count;
			return list [idx];
		}

		#region Network Message Sending

		/// <summary>
		/// Send card play action to other players
		/// </summary>
		public void SendCardPlay (CardData card) {
			// Don't send if turn is already finished
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			// Convert card to network identifier
			string cardId = GetCardIdentifier (card);

			// Create network message
			var moveData = new ExitGames.Client.Photon.Hashtable {
				{"actionType", "PLAY_CARD"},
				{"cardIdentifier", cardId}
			};

			// Send as non-finishing move (turn continues)
			turnMgr.SendMove (moveData, false);
			TakiLogger.LogNetwork ($"Sent card play: {cardId}");
		}

		/// <summary>
		/// Send card draw action to other players
		/// </summary>
		public void SendCardDraw () {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			var moveData = new ExitGames.Client.Photon.Hashtable {
				{"actionType", "DRAW_CARD"},
				{"cardIdentifier", ""}  // No specific card data needed
			};

			// Send as non-finishing move
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
		/// Send end turn signal - this advances the turn to the next player
		/// </summary>
		public void SendEndTurn () {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			var moveData = new ExitGames.Client.Photon.Hashtable {
				{"actionType", "END_TURN"},
				{"cardIdentifier", ""}
			};

			// Send as finishing move - this will advance the turn
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
		/// Send PlusTwo card effect with chain information to other players
		/// </summary>
		public void SendPlusTwoEffect (int chainCount, int drawCount) {
			if (turnMgr == null || turnMgr.IsFinishedByMe) return;

			var moveData = new ExitGames.Client.Photon.Hashtable {
				{"actionType", "PLUS_TWO_EFFECT"},
				{"cardIdentifier", $"{chainCount},{drawCount}"}  // Encode chain data
			};

			// Non-finishing move - player must still press END TURN
			turnMgr.SendMove (moveData, false);
			TakiLogger.LogNetwork ($"Sent PlusTwo effect: {chainCount} cards, {drawCount} total");
		}

		#endregion

		#region Network Message Processing

		/// <summary>
		/// Process action received from remote player and apply to local game state
		/// </summary>
		void ProcessRemoteAction (Player player, object moveData) {
			// Safety check
			if (gameManager == null) {
				TakiLogger.LogError ("GameManager not available for processing network action", TakiLogger.LogCategory.Network);
				return;
			}

			// Parse network message
			if (moveData is ExitGames.Client.Photon.Hashtable networkMove) {
				string actionType = (string)networkMove["actionType"];
				string cardIdentifier = (string)networkMove["cardIdentifier"];

				TakiLogger.LogNetwork($"Processing {actionType} from player {player.ActorNumber}");

				switch (actionType) {
					// Basic game actions
					case "PLAY_CARD":
						gameManager.ProcessNetworkCardPlay (cardIdentifier, player.ActorNumber);
						break;
					case "DRAW_CARD":
						gameManager.ProcessNetworkCardDraw (player.ActorNumber);
						break;
					// Color selection (ChangeColor card)
					case "COLOR_SELECTION":
						if (System.Enum.TryParse<CardColor> (cardIdentifier, out CardColor selectedColor)) {
							gameManager.ProcessNetworkColorSelection (selectedColor, player.ActorNumber);
						} else {
							TakiLogger.LogError ($"Invalid color selection: {cardIdentifier}", TakiLogger.LogCategory.Network);
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
					// PlusTwo chain break
					case "CHAIN_BREAK":
						// Extract how many cards the player had to draw
						int chainDrawCount = -1;
						if (!string.IsNullOrEmpty(cardIdentifier) && int.TryParse(cardIdentifier, out int parsedCount)) {
							chainDrawCount = parsedCount;
						}
						gameManager.ProcessNetworkChainBreak (player.ActorNumber, chainDrawCount);
						break;
					// PlusTwo card effect
					case "PLUS_TWO_EFFECT":
						// Parse chain count and total draw count
						if (!string.IsNullOrEmpty (cardIdentifier)) {
							string[] parts = cardIdentifier.Split (',');
							if (parts.Length == 2 && int.TryParse (parts[0], out int chainCount) && int.TryParse (parts[1], out int drawCount)) {
								gameManager.ProcessNetworkPlusTwoEffect (chainCount, drawCount, player.ActorNumber);
							} else {
								TakiLogger.LogError ($"Invalid PlusTwo data: {cardIdentifier}", TakiLogger.LogCategory.Network);
							}
						} else {
							TakiLogger.LogError ("PlusTwo effect missing data", TakiLogger.LogCategory.Network);
						}
						break;
				}
			}
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Convert CardData to network-safe identifier string
		/// </summary>
		string GetCardIdentifier (CardData card) {
			return CardDataHelper.CreateCardIdentifier (card);
		}

		#endregion

		#region Public Properties

		/// <summary>True when it's this client's turn to play</summary>
		public bool IsMyTurn => _isMyTurn;

		/// <summary>True when network game is active (not ended)</summary>
		public bool IsNetworkGameActive => !_isGameOver;

		/// <summary>True when deck synchronization is complete</summary>
		public bool IsDeckInitialized => _isDeckInitialized;

		#endregion
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