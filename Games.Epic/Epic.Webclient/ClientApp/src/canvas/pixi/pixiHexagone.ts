import {IHexagon, IHexagonProps} from "../hexagon";
import * as PIXI from "pixi.js";

export class PixiHexagon implements IHexagon {
    readonly graphics: PIXI.Graphics;

    x: number = 0
    y: number = 0
    radius: number = 0
    strokeColor: number = 0
    fillColor: number = 0
    fillAlpha: number = 0
    strokeLine: number = 0

    customFillColor: number | undefined

    onMouseEnters: (sender: IHexagon) => void = () => {}
    onMouseLeaves: (sender: IHexagon) => void = () => {}
    onMouseDown: (sender: IHexagon) => void = () => {}
    
    constructor(props: IHexagonProps, graphics: PIXI.Graphics) {
        this.graphics = graphics
        this.update(props)
        
        graphics.onmouseenter = () => this.onMouseEnters(this)
        graphics.onmouseleave = () => this.onMouseLeaves(this)
        graphics.onmousedown = () => this.onMouseDown(this)
    }
    
    dispose(): void {
        this.onMouseEnters = undefined!
        this.onMouseLeaves = undefined!
        this.onMouseDown = undefined!
    }
    
    update(props: IHexagonProps): void {
        this.x = props.x
        this.y = props.y
        this.radius = props.radius
        this.fillAlpha = props.fillAlpha
        this.strokeColor = props.strokeColor
        this.fillColor = props.fillColor
        this.strokeLine = props.strokeLine
    }
}