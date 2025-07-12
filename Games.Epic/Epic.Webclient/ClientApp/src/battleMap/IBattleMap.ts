import {BattleTurnInfo} from "./battleMap";

export interface IBattleMap {
    readonly width: number 
    readonly height: number
    readonly units: []
    readonly turnInfo: BattleTurnInfo
}
