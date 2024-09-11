import {BattleMapUnit} from "../battleMap/battleMapUnit";
import {BattleMapCell} from "../battleMap/battleMap";

export type BattleUserAction = {
    unit: BattleMapUnit,
    targetCell?: BattleMapCell
    targetUnit?: BattleMapUnit
}