import {PlayerNumber} from "../player/playerNumber";
import {IHexoPoint} from "../hexogrid/hexoGrid";

interface BaseCommandToServer {
    commandId: string
    command: string
    player: PlayerNumber
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

export type BattleCommandToServer = UnitMoveCommandToServer | UnitAttackCommandToServer