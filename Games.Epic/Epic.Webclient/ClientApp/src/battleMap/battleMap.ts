import {BattleMapUnit} from "./battleMapUnit";
import {IHexoGrid} from "../hexogrid/hexoGrid";
import {BattlePlayerSide} from "./battlePlayerSide";
import {PlayerNumber} from "../player/playerNumber";

export type BattleMapCell = {
    r: number
    c: number
}

export type BattleTurnInfo = {
    index: number
    player: PlayerNumber
}

export type BattleMap = {
    grid: IHexoGrid<BattleMapCell>
    
    units: BattleMapUnit[]
    
    turn: BattleTurnInfo
}