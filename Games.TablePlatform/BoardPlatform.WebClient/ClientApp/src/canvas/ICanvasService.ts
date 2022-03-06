import {IPositioned, ISizable, IWidget} from "../models/IWidget";
import {Signal} from "typed-signals";
import {Point, Size} from "../common/Geometry";

export interface IRectangle extends IPositioned, ISizable {
}

export interface ICanvasService {
    readonly widgets: IWidget[]
    readonly viewport: IRectangle

    readonly screenSize: Size
    setScreenSize(newSize: Size): void
    
    viewportChanged: Signal<(args: ViewportChangeEventArgs) => void>
    setViewport(viewport: IRectangle): void
    
    projectPointScreenToCanvas(point: Point): Point
    projectPointCanvasToScreen(point: Point): Point
    projectSizeScreenToCanvas(size: Size): Size
    projectSizeCanvasToScreen(size: Size): Size
    projectRectangleScreenToCanvas(rectangle: IRectangle): IRectangle
    projectRectangleCanvasToScreen(rectangle: IRectangle): IRectangle
}

export type ViewportChangeEventArgs = {
    previousViewport: IRectangle,
    newViewport: IRectangle
}