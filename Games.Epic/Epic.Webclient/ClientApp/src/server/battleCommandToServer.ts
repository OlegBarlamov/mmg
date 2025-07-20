import {BattlePlayerNumber} from "../player/playerNumber";
import {IHexoPoint} from "../hexogrid/hexoGrid";

interface BaseCommandToServer {
    commandId: string
    command: string
    player: BattlePlayerNumber
    turnIndex: number
}

interface ClientConnectedCommandToServer extends BaseCommandToServer {
    command: 'CLIENT_CONNECTED'
}

interface UnitCommandToServer extends BaseCommandToServer {
    actorId: string
}

interface UnitMoveCommandToServer extends UnitCommandToServer {
    command: 'UNIT_MOVE'
    moveToCell: IHexoPoint
}

interface UnitAttackCommandToServer extends UnitCommandToServer {
    command: 'UNIT_ATTACK'
    targetId: string
    moveToCell: IHexoPoint
}

export type BattleCommandToServer = UnitMoveCommandToServer | UnitAttackCommandToServer | ClientConnectedCommandToServer