import {ICanvasService, IRectangle, ViewportChangeEventArgs} from "./ICanvasService";
import {IWidget} from "../models/IWidget";
import {Signal} from "typed-signals";
import {Point, Size} from "../common/Geometry";

export class FakeCanvasService implements ICanvasService{
    viewport: IRectangle = {height: 1600, width: 1600, x: 0, y: 0}
    screenSize: Size = {width: 1, height: 1}
    
    widgets: IWidget[] = [
        {x: 0, y: 0, width: 100, height: 100, id: 0},
        {x: 30, y: 300, width: 200, height: 400, id: 0},
    ];
    
    viewportChanged: Signal<(args: ViewportChangeEventArgs) => void> = new Signal<(args: ViewportChangeEventArgs) => void>() 

    setViewport(viewport: IRectangle): void {
        const previousViewport = this.viewport
        this.viewport = viewport
        this.viewportChanged.emit({
            previousViewport: previousViewport,
            newViewport: this.viewport
        })
        console.log(`x: ${this.viewport.x} y:${this.viewport.y} width:${this.viewport.width} height:${this.viewport.height}`)
    }

    setScreenSize(newSize: Size): void {
        this.screenSize = newSize
    }
    
    projectPointCanvasToScreen(point: Point): Point {
        return {
            x: (point.x - this.viewport.x) / this.viewport.width * this.screenSize.width,
            y: (point.y - this.viewport.y) / this.viewport.height * this.screenSize.height,
        }
    }

    projectPointScreenToCanvas(point: Point): Point {
        return {
            x: point.x / this.screenSize.width * this.viewport.width + this.viewport.x,
            y: point.y / this.screenSize.height * this.viewport.height + this.viewport.y,
        }
    }

    projectSizeCanvasToScreen(size: Size): Size {
        return {
            width: size.width / this.viewport.width * this.screenSize.width,
            height: size.height / this.viewport.height * this.screenSize.height,
        }
    }

    projectSizeScreenToCanvas(size: Size): Size {
        return {
            width: size.width / this.screenSize.width * this.viewport.width,
            height: size.height / this.screenSize.height * this.viewport.height,
        }
    }

    projectRectangleCanvasToScreen(rectangle: IRectangle): IRectangle {
        const left: Point = this.projectPointCanvasToScreen({x: rectangle.x, y: rectangle.y})
        const size: Size = this.projectSizeCanvasToScreen({width: rectangle.width, height: rectangle.height})
        return {
            x: left.x,
            y: left.y,
            width: size.width,
            height: size.height
        }
    }

    projectRectangleScreenToCanvas(rectangle: IRectangle): IRectangle {
        const left: Point = this.projectPointScreenToCanvas({x: rectangle.x, y: rectangle.y})
        const size: Size = this.projectSizeScreenToCanvas({width: rectangle.width, height: rectangle.height})
        return {
            x: left.x,
            y: left.y,
            width: size.width,
            height: size.height
        }
    }
    
    
}