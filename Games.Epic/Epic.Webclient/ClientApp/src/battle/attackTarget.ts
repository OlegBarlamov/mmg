import {BattleMapUnit} from "../battleMap/battleMapUnit";
import {BattleMapCell} from "../battleMap/battleMap";

export interface IAttackTarget {
    actor: BattleMapUnit
    target: BattleMapUnit
    cells: BattleMapCell[]
}