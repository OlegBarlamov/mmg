import { BattleMapUnit } from "../../battleMap/battleMapUnit";
import {IHexagonProps} from "../hexagon";
import {IUnitTile, IUnitTileProps} from "../unitTile";
import {PixiHexagon} from "./pixiHexagone";
import * as PIXI from "pixi.js";

export class PixiUnitTile implements IUnitTile {
    hexagon: IHexagonProps = undefined!
    
    imgSrc: string = undefined!
    text: string = undefined!
    textBackgroundImgSrc: string = undefined!

    hexagonPixi: PixiHexagon = undefined!
    textGraphics: PIXI.Text = undefined!
    sprite: PIXI.Sprite;
    container: PIXI.Container;
    mask: PIXI.Graphics;
    textSprite: PIXI.Sprite;
    
    model: BattleMapUnit = undefined!

    constructor(
        props: IUnitTileProps,
        hexagon: PixiHexagon,
        sprite: PIXI.Sprite,
        container: PIXI.Container,
        mask: PIXI.Graphics,
        text: PIXI.Text,
        textSprite: PIXI.Sprite) {
        this.sprite = sprite
        this.container = container
        this.mask = mask
        this.textSprite = textSprite
        this.textGraphics = text
        this.hexagonPixi = hexagon
        
        this.update(props)
    }

    dispose(): void {
        this.container.parent.removeChild(this.container)
        
        this.container.removeChild(this.textGraphics)
        this.container.removeChild(this.textSprite)
        this.container.removeChild(this.sprite)
        this.container.removeChild(this.mask)

        this.textGraphics.destroy()
        this.textSprite.destroy()
        this.sprite.destroy()
        this.mask.destroy()
        
        this.hexagonPixi.dispose()

        this.container.destroy()
    }
    
    update(props: IUnitTileProps) {
        this.model = props.model
        this.imgSrc = props.imgSrc
        this.text = props.text
        this.textBackgroundImgSrc = props.textBackgroundImgSrc
        this.hexagon = {
            ...props.hexagon,
        }
    }
}