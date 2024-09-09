import {IHexagonProps} from "./hexagon";
import {BattleMapUnit} from "../battleMap/battleMapUnit";

export interface IUnitTileProps {
    readonly model: BattleMapUnit
    readonly hexagon: IHexagonProps
    readonly imgSrc: string
    readonly text: string
    readonly textBackgroundImgSrc: string
}

export interface IUnitTile extends IUnitTileProps {
    
}