import * as PIXI from "pixi.js";
import {ICanvasService, IRectangle} from "./ICanvasService";
import {IWidget} from "../models/IWidget";

export interface IWidgetView {
    drawableContainer: PIXI.Container
    destroy(): void
    update(): void
}

export class WidgetViewBase implements IWidgetView {
    public readonly drawableContainer: PIXI.Container;
    
    protected model: IWidget;
    protected canvasService: ICanvasService;

    constructor(model: IWidget, canvasService: ICanvasService) {
        this.model = model;
        this.canvasService = canvasService;
        this.drawableContainer = new PIXI.Container()

        this.updateDrawableContainerPosition()
    }

    updatePositionAndSize(): void {
        this.updateDrawableContainerPosition()
    }
    
    destroy(): void {
        this.drawableContainer.destroy(true)
    }
    
    protected getRectangleOnScreen(): IRectangle {
        const worldRectangle = {
            x: this.model.x,
            y: this.model.y,
            width: this.model.width,
            height: this.model.height,
        }
        return this.canvasService.projectRectangleCanvasToScreen(worldRectangle)
    }
    
    private updateDrawableContainerPosition() {
        const screenRec = this.getRectangleOnScreen()
        
        this.drawableContainer.x = screenRec.x
        this.drawableContainer.y = screenRec.y
        this.drawableContainer.width = screenRec.width
        this.drawableContainer.height = screenRec.height
    }

    update(): void {
        throw new Error("Not overwritten")
    }
} 