import {BattlePlayerNumber} from "./playerNumber";

export function getPlayerColor(player: BattlePlayerNumber) : number {
    switch (player) {
        case BattlePlayerNumber.Player1:
            return 0x8FD14F
        case BattlePlayerNumber.Player2:
            return 0xc96449

    }
}