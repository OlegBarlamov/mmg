import {IWidget} from "../models/IWidget";
import * as PIXI from "pixi.js";
import {StaticTextures} from "../static-resources/StaticTextures";
import {ICanvasService} from "./ICanvasService";
import {WidgetViewBase} from "./WidgetViewBase";

export class WidgetView extends WidgetViewBase {
    private readonly _sprite: PIXI.Sprite
    
    constructor(model: IWidget, canvasService: ICanvasService) {
        super(model, canvasService)
        
        this._sprite = new PIXI.Sprite(StaticTextures.kingTexture)
        this.updateSpriteSize()
        
        this.drawableContainer.addChild(this._sprite)
    }
    
    update() {
        super.updatePositionAndSize()
        
        this.updateSpriteSize()
    }

    private updateSpriteSize() {
        //this._sprite.width = this.drawableContainer.width
        //this._sprite.height = this.drawableContainer.height
    }
}