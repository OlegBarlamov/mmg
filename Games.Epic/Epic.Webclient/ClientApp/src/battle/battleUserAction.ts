import {BattlePlayerNumber} from "../player/playerNumber";
import {BattleMapCell} from "../battleMap/battleMap";
import {BattleMapUnit} from "../battleMap/battleMapUnit";

interface BaseUserAction {
    command: string
    player: BattlePlayerNumber
}

interface UnitAction extends BaseUserAction {
    actor: BattleMapUnit
}

interface UnitMoveAction extends UnitAction {
    command: 'UNIT_MOVE'
    moveToCell: BattleMapCell
}

interface UnitAttackAction extends UnitAction {
    command: 'UNIT_ATTACK'
    moveToCell: BattleMapCell
    attackTarget: BattleMapUnit
    attackTypeIndex: number
}

export type BattleUserAction = UnitMoveAction | UnitAttackAction