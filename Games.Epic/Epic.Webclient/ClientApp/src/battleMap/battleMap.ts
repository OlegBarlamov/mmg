import {BattleMapUnit} from "./battleMapUnit";
import {IHexoGrid} from "../hexogrid/hexoGrid";
import {BattlePlayerNumber} from "../player/playerNumber";

export type BattleMapCell = {
    r: number
    c: number
}

export type BattleResult = {
    finished: boolean
    winner?: BattlePlayerNumber
    reportId?: string
}

export type BattleTurnInfo = {
    index: number
    player: BattlePlayerNumber
    result?: BattleResult
    nextTurnUnitId?: string
    roundNumber: number
}

export type BattleMap = {
    id: string
    
    width: number
    height: number
    
    grid: IHexoGrid<BattleMapCell>
    
    units: BattleMapUnit[]
    
    turnInfo: BattleTurnInfo
}