import {PlayerNumber} from "../player/playerNumber";
import {UnitProperties} from "../units/unitProperties";
import {IHexoPoint} from "../hexogrid/hexoGrid";

export type BattleMapUnit = {
    position: IHexoPoint
    
    player: PlayerNumber
    
    props: UnitProperties
    
    unitsCount: number
}