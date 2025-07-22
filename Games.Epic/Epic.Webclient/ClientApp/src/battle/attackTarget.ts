import {BattleMapUnit} from "../battleMap/battleMapUnit";
import {BattleMapCell} from "../battleMap/battleMap";
import { AttackType } from "../units/unitProperties";

export interface IAttackTarget {
    actor: BattleMapUnit
    target: BattleMapUnit
    cells: BattleMapCell[]
    attackType: AttackType & {index: number}
}