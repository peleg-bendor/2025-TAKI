# investigating!

Here is what I'm seeing visually:
Screen 1, the master, in unity engine run:
- `Player1HandPanel` has 8 cards, as card fronts, as is supposed to be (e.g. in inspector I can see `Card_Yellow 7_0` - `Card_Yellow 7_7`) [<- good]
- `Player1HandSizeText` is "Your Cards: 8" [<- good]
- `Player2HandPanel` has 8 cards, as card backs, as is supposed to be (in inspector I can see `OpponentCard_0` - `OpponentCard_7`) [<- good]
- `Player2HandSizeText` is "Opponent Cards: 8" [<- good]
- `DrawPilePanel` has `DrawPileCard` , visually shows a card back (matching to not-master!) [<- good, I think, but I find it a little odd that we don't see in the inspector its identifier string]
- `DrawPileCountText` is "Draw: 93" [<- good]
- `DiscardPilePanel` has `DiscardPileCard`, visually shows a card front (matching to not-master!) [<- good, I think, but I find it a little odd that we don't see in the inspector its identifier string]
- `DiscardPileCountText` is "Discard: 1" [<- good]
- `TurnIndicatorText` is "Your Turn" [<- good]
- `CurrentColorIndicator` is the color [<- good]
- `Btn_Player1EndTakiSequence` is disabled [<- good]
- `Btn_Player1PlayCard` is enabled [<- good]
- `Btn_Player1DrawCard` is enabled [<- good]
- `ColorSelectionPanel` is disabled [<- good]
- The cards in `Player1HandPanel` are clickable and tint red/gold appropriately (good, since this is this player's turn) [<- good]
- When I click on `Btn_Player1DrawCard` or `Btn_Player1PlayCard`, I get this log: `Exception: Write failed. Custom type not found: TakiGame.NetworkMoveData` [<- a problem!]
Screen 2, the client, a build:
- `Player1HandPanel` visually has 8 cards, as card fronts, as is supposed to be (e.g. I can see a yellow 1 to a green 8) [<- good]
- `Player1HandSizeText` visually is "Your Cards: 8" [<- good]
- `Player2HandPanel` visually has 8 cards, as card backs, as is supposed to be [<- good]
- `Player1HandSizeText` visually is "Opponent Cards: 8" [<- good]
- `DrawPilePanel` visually shows a card back (matching to master!) [<- good]
- `DrawPileCountText` visually is "Draw: 93" [<- good]
- `DiscardPilePanel` visually shows a card front (matching to master!) [<- good]
- `DiscardPileCountText` visually is "Discard: 1" [<- good]
- `TurnIndicatorText` visually is "Opponent's Turn" [<- good]
- `CurrentColorIndicator` visually is the color (matching to master!) [<- good]
- `Btn_Player1EndTakiSequence` is disabled [<- good]
- `Btn_Player1PlayCard` is disabled [<- good]
- `Btn_Player1DrawCard` is disabled [<- good]
- `ColorSelectionPanel` is disabled [<- good]
Read `CLAUDE.md`.
I want you to carefully read `MOST_RECENT_RUN_LOGS_SIMPLE.md` (you can also read relevant parts in `MOST_RECENT_RUN_LOGS_SIMPLE.md` if you see you need to). I want to discuss with you on what we see there and what we see visually.
