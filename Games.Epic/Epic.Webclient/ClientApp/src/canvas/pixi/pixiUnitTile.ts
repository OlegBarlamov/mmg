import { IHexagonProps } from "../hexagon";
import {IUnitTile, IUnitTileProps} from "../unitTile";
import {PixiHexagon} from "./pixiHexagone";
import * as PIXI from "pixi.js";

export class PixiUnitTile implements IUnitTile {
    hexagon: IHexagonProps = undefined!
    imgSrc: string = undefined!
    text: string = undefined!
    textBackgroundImgSrc: string = undefined!
    
    constructor(
        props: IUnitTileProps,
        hexagon: PixiHexagon,
        sprite: PIXI.Sprite,
        container: PIXI.Container,
        mask: PIXI.Graphics,
        text: PIXI.Text,
        textSprite: PIXI.Sprite) {
        this.update(props)
    }
    
    update(props: IUnitTileProps) {
        this.imgSrc = props.imgSrc
        this.text = props.text
        this.textBackgroundImgSrc = props.textBackgroundImgSrc
        this.hexagon = {
            ...this.hexagon,
        }
    }
}