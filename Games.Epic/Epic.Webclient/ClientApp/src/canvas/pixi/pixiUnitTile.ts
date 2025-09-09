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

    onMouseMove: (sender: IUnitTile, event: PointerEvent) => void = () => {}
    onMouseEnters: (sender: IUnitTile, event: PointerEvent) => void = () => {}
    onMouseLeaves: (sender: IUnitTile, event: PointerEvent) => void = () => {}
    onMouseDown: (sender: IUnitTile, event: PointerEvent) => void = () => {}
    onMouseUp: (sender: IUnitTile, event: PointerEvent) => void = () => {}
    onRightClick: (sender: IUnitTile, event: PointerEvent) => void = () => {}

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

        hexagon.onMouseMove = (sender, event) => this.onMouseMove(this, event)
        hexagon.onMouseEnters = (sender, event) => this.onMouseEnters(this, event)
        hexagon.onMouseLeaves = (sender, event) => this.onMouseLeaves(this, event)
        hexagon.onMouseDown = (sender, event) => this.onMouseDown(this, event)
        hexagon.onMouseUp = (sender, event) => this.onMouseUp(this, event)
        hexagon.onRightClick = (sender, event) => this.onRightClick(this, event)
        
        this.update(props)
    }

    dispose(): void {
        this.onMouseMove = undefined!
        this.onMouseEnters = undefined!
        this.onMouseLeaves = undefined!
        this.onMouseDown = undefined!
        this.onMouseUp = undefined!
        
        this.container.parent?.removeChild(this.container)
        
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