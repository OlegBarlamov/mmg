import {BattleMapUnit} from "./battleMapUnit";
import {IHexoGrid} from "../hexogrid/hexoGrid";

export type BattleMapCell = {
    r: number
    c: number
}

export type BattleMap = {
    grid: IHexoGrid<BattleMapCell>
    
    units: BattleMapUnit[]
}