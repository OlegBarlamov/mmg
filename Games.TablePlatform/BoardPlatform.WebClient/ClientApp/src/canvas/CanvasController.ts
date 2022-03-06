import {ICanvasService, IRectangle} from "./ICanvasService";
import {MouseButtons} from "../common/MouseButtons";
import {Point, Size} from "../common/Geometry";

export class CanvasController {
    private _canvasContainer: HTMLElement | undefined;
    private _canvasService: ICanvasService;
    
    private _isRightMouseDown: boolean = false
    
    constructor(canvasService: ICanvasService) {
        this._canvasService = canvasService;
    }
    
    public destroy() {
        
    }
    
    public initialize(canvasContainer: HTMLElement) {
        this._canvasContainer = canvasContainer
        
        this._canvasContainer.addEventListener('mousemove', this.onMouseMoveHandler)
        this._canvasContainer.addEventListener('mousedown', this.onMouseDownHandler)
        this._canvasContainer.addEventListener('mouseup', this.onMouseUpHandler)
        this._canvasContainer.addEventListener('mousewheel', this.onMouseWheelHandler)
    }
    
    private onMouseMoveHandler = (e: MouseEvent) => { this.onMouseMove(e) }
    private onMouseDownHandler = (e: MouseEvent) => { this.onMouseDown(e) }
    private onMouseUpHandler = (e: MouseEvent) => { this.onMouseUp(e) }
    private onMouseWheelHandler = (e: any) => { e.preventDefault(); this.onMouseWheel(e) }
    
    private onMouseMove(e: MouseEvent) : void {
        if (!this._isRightMouseDown) {
            return
        }
        
        this.moveViewport(e.movementX, e.movementY)
    }
    
    private onMouseDown(e: MouseEvent): void {
        if (e.button === MouseButtons.Right) {
            this._isRightMouseDown = true
        } 
    }

    private onMouseUp(e: MouseEvent): void {
        if (e.button === MouseButtons.Right) {
            this._isRightMouseDown = false
        }
    }
    
    private onMouseWheel(e: WheelEvent): void {
        const speed = 0.1
        const viewport = this._canvasService.viewport
        const newViewport: IRectangle = {
            x: viewport.x - e.deltaY * speed,
            y: viewport.y - e.deltaY * speed,
            width: viewport.width + e.deltaY * 2 * speed,
            height: viewport.height + e.deltaY * 2 * speed,

        }

        this._canvasService.setViewport(newViewport)
    }
    
    private moveViewport(xDiff: number, yDiff: number) {
        const diffPoint : Size = {
            width: xDiff,
            height: yDiff,
        } 
        
        const projectedToCanvasDiffPoint = this._canvasService.projectSizeScreenToCanvas(diffPoint)
        const viewport = this._canvasService.viewport
        const newViewport: IRectangle = {
            x: viewport.x - projectedToCanvasDiffPoint.width,
            y: viewport.y - projectedToCanvasDiffPoint.height,
            width: viewport.width,
            height: viewport.height,
        }
        
        this._canvasService.setViewport(newViewport)
    }
}