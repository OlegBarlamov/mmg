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

    onMouseEnters: (sender: IHexagon, event: PointerEvent) => void = () => {}
    onMouseLeaves: (sender: IHexagon, event: PointerEvent) => void = () => {}
    onMouseDown: (sender: IHexagon, event: PointerEvent) => void = () => {}
    onMouseUp: (sender: IHexagon, event: PointerEvent) => void = () => {}
    onMouseMove: (sender: IHexagon, event: PointerEvent) => void = () => {}
    onRightClick: (sender: IHexagon, event: PointerEvent) => void = () => {}
    
    constructor(props: IHexagonProps, graphics: PIXI.Graphics) {
        this.graphics = graphics
        this.update(props)
        
        graphics.onmouseenter = (event) => this.onMouseEnters(this, event)
        graphics.onmouseleave = (event) => this.onMouseLeaves(this, event)
        graphics.onmousedown = (event) => this.onMouseDown(this, event)
        graphics.onmouseup = (event) => this.onMouseUp(this, event)
        graphics.onmousemove = (event) => this.onMouseMove(this, event)
        graphics.on('rightclick', (event) => this.onRightClick(this, event))
    }
    
    dispose(): void {
        this.graphics.onmouseenter = null
        this.graphics.onmouseleave = null
        this.graphics.onmousedown = null
        this.graphics.onmouseup = null
        this.graphics.onmousemove = null
        this.onMouseEnters = undefined!
        this.onMouseLeaves = undefined!
        this.onMouseDown = undefined!
        this.onMouseUp = undefined!
        this.onMouseMove = undefined!
        this.graphics.parent.removeChild(this.graphics)
        this.graphics.destroy()
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