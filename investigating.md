# investigating!

`MOST_RECENT_RUN_LOGS_SIMPLE.md` amd `MOST_RECENT_RUN_LOGS_DETAILED.md` show the logs from unity engine console as the client.

## Dictionary
- `Scr1` -> Screen 1, the master, a build
- `Scr2` -> Screen 2, the client, in unity engine run
- `Player1HandSizeText` = `H1` -> number
- `Player2HandSizeText` = `H2` -> number
- `DrawPileCountText` = `Dra` -> number
- `DiscardPileCountText` = `Dis` -> number
- `TurnIndicatorText` = `TI` -> "Your Turn" = `YOU` or "Opponent's Turn" = `OPP`
- `CurrentColorIndicator` = `CI` -> color
- `Btn_Player1PlayCard` = `PLAY` -> "disabled" = `DIS` or "enabled" = `EN`
- `Btn_Player1DrawCard` = `DRAW` -> "disabled" = `DIS` or "enabled" = `EN`
- `Btn_Player1EndTurn` = `END` -> "disabled" = `DIS` or "enabled" = `EN`
- Current card in discard pile = `Dis_Card` 

## State tables:

|    Scr_1   |  H1  |  H2  | Dra  | Dis  |  TI  |  CI  | PLAY | DRAW | END  |  Dis_Card  |
|------------|------|------|------|------|------|------|------|------|------|------------|
|  Start     |  8   |  8   |  93  |  1   | YOU  | GRE  | EN   | EN   | DIS  | green_six  |
|  State 1   |  7   |  8   |  93  |  2   | YOU  | GRE  | DIS  | DIS  | EN   | green_eight |
|  State 2   |  7   |  8   |  93  |  2   | OPP  | GRE  | DIS  | DIS  | DIS  | green_eight |
|  State 3   |  7   |  7   |  93  |  3   | OPP  | YEL  | DIS  | DIS  | DIS  | yellow_eight |
|  State 4   |  7   |  7   |  93  |  3   | YOU  | YEL  | EN   | EN   | DIS  | yellow_eight |
|  State 5   |  8   |  7   |  92  |  3   | YOU  | YEL  | DIS  | DIS  | EN   | yellow_eight |
|  State 6   |  8   |  7   |  92  |  3   | OPP  | YEL  | DIS  | DIS  | DIS  | yellow_changeDirection |
|  State 7   |  8   |  6   |  92  |  4   | OPP  | YEL  | DIS  | DIS  | DIS  | yellow_changeDirection |

|    Scr_2   |  H1  |  H2  | Dra  | Dis  |  TI  |  CI  | PLAY | DRAW | END  |  Dis_Card  |
|------------|------|------|------|------|------|------|------|------|------|------------|
|  Start     |  8   |  8   |  93  |  1   | OPP  | GRE  | DIS  | DIS  | DIS  | green_six  |
|  State 1   |  8   |  7   |  92  |  1   | OPP  | GRE  | DIS  | DIS  | DIS  | green_eight |
|  State 2   |  8   |  7   |  93  |  2   | YOU  | GRE  | EN   | EN   | DIS  | green_eight |
|  State 3   |  7   |  7   |  93  |  3   | YOU  | YEL  | DIS  | DIS  | EN   | yellow_eight |
|  State 4   |  7   |  7   |  93  |  3   | OPP  | YEL  | DIS  | DIS  | DIS  | yellow_eight |
|  State 5   |  7   |  8   |  92  |  3   | OPP  | YEL  | DIS  | DIS  | DIS  | yellow_eight |
|  State 6   |  7   |  8   |  92  |  3   | YOU  | YEL  | EN   | EN   | DIS  | yellow_changeDirection |
|  State 7   |  6   |  8   |  92  |  4   | YOU  | YEL  | DIS  | DIS  | EN   | yellow_changeDirection |

## State tracking:

|   State    | Scr's Turn | Btn click  |    Notes and Input     |
|------------|------------|------------|------------------------|
|   Start    |   Scr_1    |   PLAY     | Played green_eight |
|  State 1   |   Scr_1    |   END TURN | ... |
|  State 2   |   Scr_2    |   PLAY     | Played yellow_eight |
|  State 3   |   Scr_2    |   END TURN | ... |
|  State 4   |   Scr_1    |   DRAW     | ... |
|  State 5   |   Scr_1    |   END TURN | ... |
|  State 6   |   Scr_2    |   PLAY     | Played yellow_changeDirection |
|  State 7   |   Scr_2    |   END TURN | ... |

## Notes:
Everything seems alright and functional.
But the messages for the players really seem off (I'm talking about `Player1MessageText` and `Player2MessageText`):
- When playing the `yellow_changeDirection` I didn't see any messages at all
- Sometimes we don't see any messages
- Sometimes we see the messages for only a moment, not long enough
- Sometimes we see the messages in reverse - A message that's supposed to be for client's screen appears on master's screen, and message that's supposed to be for master's screen appears on client's screen

In State 6, both screens were supposed to be showing appropriate messages, neither of them did.

We need to investigate our Player/Opponent Messages system in multiplayer


