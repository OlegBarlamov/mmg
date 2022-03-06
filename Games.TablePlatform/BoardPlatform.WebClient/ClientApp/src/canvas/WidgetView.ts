import {IWidget} from "../models/IWidget";
import * as PIXI from "pixi.js";
import {StaticTextures} from "../static-resources/StaticTextures";
import {ICanvasService} from "./ICanvasService";

export interface IWidgetView {
    drawableContainer: PIXI.Container
    destroy(): void
}

export class WidgetView implements IWidgetView {
    private readonly _model: IWidget
    private readonly _pixiItem: PIXI.Container
    private readonly _canvasService: ICanvasService
    private readonly _sprite: PIXI.Sprite
    
    constructor(model: IWidget, canvasService: ICanvasService) {
        this._model = model;
        this._canvasService = canvasService;
        
        this._pixiItem = new PIXI.Container()
        this._pixiItem.width = this._model.width
        this._pixiItem.height = this._model.height
        
        this._sprite = new PIXI.Sprite(StaticTextures.kingTexture)
        this._sprite.width = this._model.width
        this._sprite.height = this._model.height
        
        this._pixiItem.addChild(this._sprite)
        
        this.initialize()
    }

    get model(): IWidget {
        return this._model;
    }

    get drawableContainer(): PIXI.Container {
        return this._pixiItem;
    }
    
    update() {
        this._pixiItem.x = this._model.x - this._canvasService.viewport.x
        this._pixiItem.y = this._model.y - this._canvasService.viewport.y
        // sizes
    }

    destroy() {
        this._pixiItem.destroy(true)
    }
    
    private initialize() {
        this._pixiItem.x = this._model.x - this._canvasService.viewport.x
        this._pixiItem.y = this._model.y - this._canvasService.viewport.y
    }
}