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
    onMouseMove: (sender: IUnitTile, event: PointerEvent) => void
    onMouseEnters: (sender: IUnitTile, event: PointerEvent) => void
    onMouseLeaves: (sender: IUnitTile, event: PointerEvent) => void
    onMouseDown: (sender: IUnitTile, event: PointerEvent) => void
    onMouseUp: (sender: IUnitTile, event: PointerEvent) => void
    onRightClick: (sender: IUnitTile, event: PointerEvent) => void
}