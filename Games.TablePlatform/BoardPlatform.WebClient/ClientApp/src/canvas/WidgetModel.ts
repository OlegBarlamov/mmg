import {IWidget} from "../models/IWidget";
import {IRectangle} from "./ICanvasService";

export class WidgetModel implements IWidget {
    readonly id: number;
    
    get height(): number {
        return this._height;
    }
    get width(): number {
        return this._width;
    }
    get x(): number {
        return this._x;
    }
    get y(): number {
        return this._y;
    }

    private _height: number;
    private _width: number;
    private _x: number;
    private _y: number;
    
    constructor(id: number, rec: IRectangle) {
        this.id = id
        this._x = rec.x
        this._y = rec.y
        this._width = rec.width
        this._height = rec.height
    }

    moveRelative(xDiff: number, yDiff: number): void {
        this._x += xDiff
        this._y += yDiff
    }
}