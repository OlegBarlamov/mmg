import {IHexagonProps} from "./hexagon";

export interface IUnitTileProps {
    readonly hexagon: IHexagonProps
    readonly imgSrc: string
    readonly text: string
    readonly textBackgroundImgSrc: string
}

export interface IUnitTile extends IUnitTileProps {
    
}