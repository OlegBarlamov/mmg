import {PlayerNumber} from "./playerNumber";

export function getPlayerColor(player: PlayerNumber) : number {
    switch (player) {
        case PlayerNumber.Player1:
            return 0x8FD14F
        case PlayerNumber.Player2:
            return 0xc96449

    }
}