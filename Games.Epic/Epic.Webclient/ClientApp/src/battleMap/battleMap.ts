import {BattleMapUnit} from "./battleMapUnit";
import {IHexoGrid} from "../hexogrid/hexoGrid";
import {BattlePlayerNumber} from "../player/playerNumber";

export type BattleMapCell = {
    r: number
    c: number
    isObstacle: boolean
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
    nextTurnUnitId: string | undefined
    roundNumber: number
}

export type InBattlePlayerInfo = {
    playerId: string
    playerNumber: BattlePlayerNumber
    index: number
    ransomClaimed: boolean
    runClaimed: boolean
}

export type IBattleObstacle = {
    column: number
    row: number
    width: number
    height: number
    mask: boolean[][]
}

export type BattleMap = {
    id: string
    
    width: number
    height: number
    
    grid: IHexoGrid<BattleMapCell>
    
    units: BattleMapUnit[]
    
    turnInfo: BattleTurnInfo

    players: InBattlePlayerInfo[]

    obstacles: IBattleObstacle[]
}