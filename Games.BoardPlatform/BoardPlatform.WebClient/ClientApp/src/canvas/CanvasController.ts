import {ICanvasService, IRectangle} from "./ICanvasService";
import {MouseButtons} from "../common/MouseButtons";
import {Point} from "../common/Geometry";
import {IWidget} from "../models/IWidget";

export class CanvasController {
    private _canvasContainer: HTMLElement | undefined;
    private _canvasService: ICanvasService;
    
    private _isRightMouseDown: boolean = false
    private _isLeftMouseDown: boolean = false
    
    private _druggingWidget: IWidget | undefined
    
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
        if (this._isRightMouseDown) {
            const canvasDiff = this._canvasService.projectSizeScreenToCanvas({
                width: e.movementX / window.devicePixelRatio,
                height: e.movementY / window.devicePixelRatio,
            })
            
            this.moveViewport(canvasDiff.width, canvasDiff.height)
        }
        
        if (this._isLeftMouseDown && this._druggingWidget) {
            const canvasDiff = this._canvasService.projectSizeScreenToCanvas({
                width: e.movementX / window.devicePixelRatio,
                height: e.movementY / window.devicePixelRatio,
            })
            
            this._druggingWidget.moveRelative(canvasDiff.width, canvasDiff.height)
        }
    }
    
    private onMouseDown(e: MouseEvent): void {
        if (e.button === MouseButtons.Right) {
            this._isRightMouseDown = true
        }
        
        if (e.button === MouseButtons.Left) {
            this._isLeftMouseDown = true

            const containerRect = this._canvasContainer?.getBoundingClientRect() ?? new DOMRect(0,0,0,0)
            const containerMousePoint: Point = {
                x: e.clientX - containerRect.left,
                y: e.clientY - containerRect.top,
            }
            
            const canvasPoint = this._canvasService.projectPointScreenToCanvas(containerMousePoint)
            this._druggingWidget = this._canvasService.findWidgetUnderThePoint(canvasPoint)
        }
    }

    private onMouseUp(e: MouseEvent): void {
        if (e.button === MouseButtons.Right) {
            this._isRightMouseDown = false
        }

        if (e.button === MouseButtons.Left) {
            this._isLeftMouseDown = false
        }
    }
    
    private onMouseWheel(e: WheelEvent): void {
        const speed = 3
        const viewport = this._canvasService.viewport
        const aspectRatio = viewport.width / viewport.height
        
        let newHeight = viewport.height + e.deltaY * viewport.height * speed / 10000
        if (newHeight < 1) {
            newHeight = 1
        }
        if (newHeight !== viewport.height) {
            const newWidth = newHeight * aspectRatio
            const newViewport: IRectangle = {
                x: viewport.x - (newWidth - viewport.width) / 2,
                y: viewport.y - (newHeight - viewport.height) / 2,
                width: newWidth,
                height: newHeight,
            }

            this._canvasService.setViewport(newViewport)
        }
    }
    
    private moveViewport(xDiff: number, yDiff: number) {
        const viewport = this._canvasService.viewport
        const newViewport: IRectangle = {
            x: viewport.x - xDiff,
            y: viewport.y - yDiff,
            width: viewport.width,
            height: viewport.height,
        }
        
        this._canvasService.setViewport(newViewport)
    }
}