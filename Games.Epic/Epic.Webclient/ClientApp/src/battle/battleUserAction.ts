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

interface UnitPassAction extends UnitAction {
    command: 'UNIT_PASS'
}

interface UnitWaitAction extends UnitAction {
    command: 'UNIT_WAIT'
}

interface PlayerRansomAction extends BaseUserAction {
    command: 'PLAYER_RANSOM'
}
interface PlayerRunAction extends BaseUserAction {
    command: 'PLAYER_RUN'
}

interface PlayerMagicAction extends BaseUserAction {
    command: 'PLAYER_MAGIC'
    magicTypeId: string
}

export type BattleUserAction =
 UnitMoveAction
  | UnitAttackAction
  | UnitPassAction
  | UnitWaitAction
  | PlayerRansomAction
  | PlayerRunAction
  | PlayerMagicAction