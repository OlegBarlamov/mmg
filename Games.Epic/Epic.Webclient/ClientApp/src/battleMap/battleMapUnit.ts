import {PlayerNumber} from "../player/playerNumber";
import {UnitProperties} from "../units/unitProperties";
import {HexoPoint} from "../common/Point";

export type BattleMapUnit = {
    position: HexoPoint
    
    player: PlayerNumber
    
    props: UnitProperties
    
    unitsCount: number
}