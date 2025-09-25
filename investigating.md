# investigating!

`MOST_RECENT_RUN_LOGS_SIMPLE.md` amd `MOST_RECENT_RUN_LOGS_SIMPLE.md` show the logs from unity engine console as the client.

## Here is what I'm seeing visually in the screens BEFORE master draws a card (as an action for the first turn):

### Screen 1, the client, in unity engine run:
- `Player1HandPanel` has 8 cards, as card fronts, as is supposed to be [<- good]
- `Player1HandSizeText` is "Your Cards: 8" [<- good]
- `Player2HandPanel` has 8 cards, as card backs, as is supposed to be [<- good]
- `Player2HandSizeText` is "Opponent Cards: 8" [<- good]
- `DrawPilePanel` has `DrawPileCard` , visually shows a card back (matching to master!) [<- good]
- `DrawPileCountText` is "Draw: 93" [<- good]
- `DiscardPilePanel` has `DiscardPileCard`, visually shows a card front (matching to master!) [<- good]
- `DiscardPileCountText` is "Discard: 1" [<- good]
- `TurnIndicatorText` is "Opponent's Turn" [<- good]
- `CurrentColorIndicator` is the color [<- good]
- `Btn_Player1EndTakiSequence` is disabled [<- good]
- `Btn_Player1PlayCard` is disabled [<- good]
- `Btn_Player1DrawCard` is disabled [<- good]
- `ColorSelectionPanel` is disabled [<- good]

### Screen 2, the master, a build:
- `Player1HandPanel` visually has 8 cards, as card fronts, as is supposed to be (e.g. I can see a yellow 1 to a green 8) [<- good]
- `Player1HandSizeText` visually is "Your Cards: 8" [<- good]
- `Player2HandPanel` visually has 8 cards, as card backs, as is supposed to be [<- good]
- `Player1HandSizeText` visually is "Opponent Cards: 8" [<- good]
- `DrawPilePanel` visually shows a card back (matching to client!) [<- good]
- `DrawPileCountText` visually is "Draw: 93" [<- good]
- `DiscardPilePanel` visually shows a card front (matching to client!) [<- good]
- `DiscardPileCountText` visually is "Discard: 1" [<- good]
- `TurnIndicatorText` visually is "Your Turn" [<- good]
- `CurrentColorIndicator` visually is the color (matching to client!) [<- good]
- `Btn_Player1EndTakiSequence` is disabled [<- good]
- `Btn_Player1PlayCard` is enabled [<- good]
- `Btn_Player1DrawCard` is enabled [<- good]
- `ColorSelectionPanel` is disabled [<- good]

## Here is what I'm seeing visually in the screens AFTER master draws a card (as an action for the first turn), and my turn (client) begins:

### Screen 1, the client, in unity engine run:
- `Player1HandPanel` has 8 cards, as card fronts, as is supposed to be [<- good]
- `Player1HandSizeText` is "Your Cards: 8" [<- good]
- `Player2HandPanel` has 9 cards, as card backs, as is supposed to be [<- good]
- `Player2HandSizeText` is "Opponent Cards: 9" [<- good]
- `DrawPilePanel` has `DrawPileCard` , visually shows a card back (matching to master!) [<- good]
- `DrawPileCountText` is "Draw: 93" [<- a problem]
- `DiscardPilePanel` has `DiscardPileCard`, visually shows a card front (matching to master!) [<- good]
- `DiscardPileCountText` is "Discard: 1" [<- good]
- `TurnIndicatorText` is "Your Turn" [<- good]
- `CurrentColorIndicator` is the color [<- good]
- `Btn_Player1EndTakiSequence` is disabled [<- good]
- `Btn_Player1PlayCard` is enabled [<- good]
- `Btn_Player1DrawCard` is enabled [<- good]
- `ColorSelectionPanel` is disabled [<- good]

### Screen 2, the master, a build:
- `Player1HandPanel` visually has 9 cards, as card fronts, as is supposed to be (e.g. I can see a yellow 1 to a green 8) [<- good]
- `Player1HandSizeText` visually is "Your Cards: 9" [<- good]
- `Player2HandPanel` visually has 8 cards, as card backs, as is supposed to be [<- good]
- `Player1HandSizeText` visually is "Opponent Cards: 8" [<- a problem!]
- `DrawPilePanel` visually shows a card back (matching to client!) [<- good]
- `DrawPileCountText` visually is "Draw: 92" [<- good]
- `DiscardPilePanel` visually shows a card front (matching to client!) [<- good]
- `DiscardPileCountText` visually is "Discard: 1" [<- good]
- `TurnIndicatorText` visually is "Opponent's Turn" [<- good]
- `CurrentColorIndicator` visually is the color (matching to client!) [<- good]
- `Btn_Player1EndTakiSequence` is disabled [<- good]
- `Btn_Player1PlayCard` is disabled [<- good]
- `Btn_Player1DrawCard` is disabled [<- good]
- `ColorSelectionPanel` is disabled [<- good]

