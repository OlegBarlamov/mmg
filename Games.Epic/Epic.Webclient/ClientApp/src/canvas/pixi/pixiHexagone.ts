import {IHexagon} from "../hexagon";
import * as PIXI from "pixi.js";

export class PixiHexagon implements IHexagon {
    readonly graphics: PIXI.Graphics;

    x: number
    y: number
    radius: number
    strokeColor: number
    fillColor: number
    fillAlpha: number

    constructor(props: IHexagon, graphics: PIXI.Graphics) {
        this.graphics = graphics
        this.x = props.x
        this.y = props.y
        this.radius = props.radius
        this.fillAlpha = props.fillAlpha
        this.strokeColor = props.strokeColor
        this.fillColor = props.fillColor
    }
}