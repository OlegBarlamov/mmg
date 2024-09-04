import {BattleMapUnit} from "./battleMapUnit";

export type BattleMapCell = {
    r: number
    c: number
}

export type BattleMap = {
    width: number
    height: number
    
    cells: BattleMapCell[][]
    units: BattleMapUnit[]
}