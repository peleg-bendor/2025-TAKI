		/// <summary>
		/// FIXED: Handle AI-specific turn flow for special cards - Enhanced TAKI sequence & PlusTwo integration
		/// CRITICAL FIX: Proper sequence state awareness and last card effect processing
		/// </summary>
		/// <param name="card">Card played by AI</param>
		void HandleAISpecialCardEffects(CardData card) {
			TakiLogger.LogAI($"=== AI HANDLING TURN FLOW for {card.GetDisplayText()} ===");
			TakiLogger.LogAI("Note: Core special card effects already handled by main HandleSpecialCardEffects method");

			// CRITICAL FIX: Check if we're in a TAKI sequence
			bool inTakiSequence = gameState.IsInTakiSequence;
			bool isLastCardInSequence = isCurrentCardLastInSequence;

			TakiLogger.LogAI($"Sequence context: InSequence={inTakiSequence}, IsLastCard={isLastCardInSequence}");

			switch (card.cardType) {
				case CardType.Plus:
					if (inTakiSequence && !isLastCardInSequence) {
						// Plus card played during sequence - effect deferred
						TakiLogger.LogAI("AI played PLUS during TAKI sequence - effect DEFERRED, continuing sequence");
						
						// Continue sequence - don't end turn
						CardData topCard = GetTopDiscardCard();
						if (topCard != null && computerAI != null) {
							Invoke(nameof(TriggerAISequenceDecision), 1.0f);
						} else {
							TakiLogger.LogError("Cannot continue AI sequence after Plus - missing components", TakiLogger.LogCategory.AI);
							EndAITurnWithStrictFlow();
						}
					} else {
						// Normal Plus card handling (not in sequence or last card)
						TakiLogger.LogAI("AI played PLUS card - AI gets additional action");

						if (gameplayUI != null) {
							gameplayUI.ShowSpecialCardEffect(CardType.Plus, PlayerType.Computer, "AI gets one more action");
						}

						// AI needs to make another decision
						CardData topCard = GetTopDiscardCard();
						if (topCard != null && computerAI != null) {
							TakiLogger.LogAI("Triggering AI additional action for PLUS card");
							Invoke(nameof(TriggerAIAdditionalAction), 1.0f);
						}
					}
					break;

				case CardType.Stop:
					if (inTakiSequence && !isLastCardInSequence) {
						// Stop card played during sequence - effect deferred
						TakiLogger.LogAI("AI played STOP during TAKI sequence - effect DEFERRED, continuing sequence");
						
						// Continue sequence
						CardData topCard = GetTopDiscardCard();
						if (topCard != null && computerAI != null) {
							Invoke(nameof(TriggerAISequenceDecision), 1.0f);
						} else {
							EndAITurnWithStrictFlow();
						}
					} else {
						// Normal Stop card handling
						TakiLogger.LogAI("AI played STOP card - flag already set by main method");

						if (gameplayUI != null) {
							gameplayUI.ShowSpecialCardEffect(CardType.Stop, PlayerType.Computer, "Your next turn will be skipped!");
						}

						EndAITurnWithStrictFlow();
					}
					break;

				case CardType.ChangeDirection:
					if (inTakiSequence && !isLastCardInSequence) {
						// ChangeDirection during sequence - effect deferred
						TakiLogger.LogAI("AI played CHANGE DIRECTION during TAKI sequence - effect DEFERRED, continuing sequence");
						
						CardData topCard = GetTopDiscardCard();
						if (topCard != null && computerAI != null) {
							Invoke(nameof(TriggerAISequenceDecision), 1.0f);
						} else {
							EndAITurnWithStrictFlow();
						}
					} else {
						// Normal ChangeDirection handling
						TakiLogger.LogAI("AI played CHANGE DIRECTION card - direction already changed");

						if (gameplayUI != null) {
							string directionMessage = $"Direction changed by AI: {gameState?.turnDirection}";
							gameplayUI.ShowSpecialCardEffect(CardType.ChangeDirection, PlayerType.Computer, directionMessage);
						}

						if (turnManager != null) {
							turnManager.EndTurn();
						}
					}
					break;

				case CardType.ChangeColor:
					if (inTakiSequence && !isLastCardInSequence) {
						// ChangeColor during sequence - effect deferred
						TakiLogger.LogAI("AI played CHANGE COLOR during TAKI sequence - effect DEFERRED, continuing sequence");
						
						CardData topCard = GetTopDiscardCard();
						if (topCard != null && computerAI != null) {
							Invoke(nameof(TriggerAISequenceDecision), 1.0f);
						} else {
							EndAITurnWithStrictFlow();
						}
					} else {
						// Normal ChangeColor handling
						TakiLogger.LogAI("AI played CHANGE COLOR card - handling AI color selection");

						if (gameplayUI != null) {
							gameplayUI.ShowImmediateFeedback("AI is selecting new color...", true);
						}

						if (computerAI != null) {
							CardColor selectedColor = computerAI.SelectColor();
							gameState?.ChangeActiveColor(selectedColor);

							if (gameplayUI != null) {
								gameplayUI.ShowSpecialCardEffect(CardType.ChangeColor, PlayerType.Computer,
									$"New active color: {selectedColor}");
							}

							TakiLogger.LogAI($"AI selected color: {selectedColor}");
						}

						if (turnManager != null) {
							turnManager.EndTurn();
						}
					}
					break;

				case CardType.PlusTwo:
					// CRITICAL FIX: PlusTwo handling with proper sequence awareness
					if (inTakiSequence && !isLastCardInSequence) {
						// PlusTwo played during sequence - effect DEFERRED
						TakiLogger.LogAI("AI played PLUS TWO during TAKI sequence - effect DEFERRED, continuing sequence");
						
						// Continue sequence - the PlusTwo effect will be processed when sequence ends
						CardData topCard = GetTopDiscardCard();
						if (topCard != null && computerAI != null) {
							Invoke(nameof(TriggerAISequenceDecision), 1.0f);
						} else {
							TakiLogger.LogError("Cannot continue AI sequence after PlusTwo - ending sequence", TakiLogger.LogCategory.AI);
							EndAITurnWithStrictFlow();
						}
					} else {
						// Normal PlusTwo handling (not in sequence or last card in sequence)
						TakiLogger.LogAI("AI played PLUS TWO card - chain logic already handled by main method");

						if (gameState.IsPlusTwoChainActive) {
							// Chain is now active - opponent must respond
							int chainCount = gameState.NumberOfChainedCards;
							int drawCount = gameState.ChainDrawCount;

							gameplayUI?.ShowSpecialCardEffect(CardType.PlusTwo, PlayerType.Computer,
								$"Chain continues! Draw {drawCount} or play PlusTwo");

							TakiLogger.LogAI($"PlusTwo chain continues: {chainCount} cards, opponent draws {drawCount} or continues");
						} else {
							// This shouldn't happen if chain logic worked correctly
							TakiLogger.LogError("AI played PlusTwo but no chain is active - this indicates a sequence processing issue!", TakiLogger.LogCategory.AI);
							gameplayUI?.ShowSpecialCardEffect(CardType.PlusTwo, PlayerType.Computer, "You draw 2 cards!");
						}

						// Normal turn end for AI after playing PlusTwo
						if (turnManager != null) {
							turnManager.EndTurn();
						}
					}
					break;

				case CardType.Taki:
					// TAKI card during sequence - continue sequence
					TakiLogger.LogAI("AI played TAKI card - continuing sequence");
					Invoke(nameof(ContinueSequenceAfterCard), 1.0f);
					break;

				case CardType.SuperTaki:
					// SuperTAKI card during sequence - continue sequence
					TakiLogger.LogAI("AI played SUPER TAKI card - continuing sequence");
					Invoke(nameof(ContinueSequenceAfterCard), 1.0f);
					break;

				default:
					// No special effects - signal turn should end
					TakiLogger.LogAI("AI played normal card - signaling turn end");
					OnAISequenceComplete?.Invoke();
					break;
			}

			TakiLogger.LogAI("=== AI TURN FLOW HANDLING COMPLETE ===");
		}
