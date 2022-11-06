import {ICanvasService, IRectangle, ViewportChangeEventArgs} from "./ICanvasService";
import {IWidget} from "../models/IWidget";
import {Signal} from "typed-signals";
import {Point, Size} from "../common/Geometry";
import {IWidgetsService} from "../models/IWidgetsService";

export class DefaultCanvasService implements ICanvasService {
    viewport: IRectangle = {width: 1920, height: 1080, x: 0, y: 0}
    screenSize: Size = {width: 1920, height: 1080}
    
    readonly viewportChanged: Signal<(args: ViewportChangeEventArgs) => void> = new Signal<(args: ViewportChangeEventArgs) => void>()
    private readonly  _widgetsService: IWidgetsService; 

    constructor(widgetsService: IWidgetsService) {
        this._widgetsService = widgetsService;
    }
    
    setViewport(viewport: IRectangle): void {
        const previousViewport = this.viewport
        this.viewport = viewport
        this.viewportChanged.emit({
            previousViewport: previousViewport,
            newViewport: this.viewport
        })
        console.log(`Viewport: x: ${this.viewport.x} y:${this.viewport.y} width:${this.viewport.width} height:${this.viewport.height}`)
    }

    onScreenSizeChanged(newSize: Size): void {
        const sizeDiff: Size = { width: newSize.width - this.screenSize.width, height: newSize.height - this.screenSize.height }
        this.screenSize = newSize
        this.setViewport({x: this.viewport.x, y: this.viewport.y, width: this.viewport.width + sizeDiff.width, height: this.viewport.height + sizeDiff.height })
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

    findWidgetUnderThePoint(point: Point): IWidget | undefined {
        console.log(`click: x: ${point.x} y:${point.y}`)
        for (const widget of this._widgetsService.widgets) {
            const widgetRec: IRectangle = widget
            if (DefaultCanvasService.pointInRectangle(point, widgetRec)) {
                return widget
            }
        }
        return undefined
    }
    
    private static pointInRectangle(point: Point, rectangle: IRectangle): boolean {
        return point.x > rectangle.x && point.x < rectangle.x + rectangle.width
            && point.y > rectangle.y && point.y < rectangle.y + rectangle.height 
    }
}