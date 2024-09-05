import * as PIXI from "pixi.js";
import {IHexagon, IHexagonProps} from "../canvas/hexagon";
import {PixiHexagon} from "../canvas/pixi/pixiHexagone";
import {Size} from "../common/Size";
import {IUnitTile, IUnitTileProps} from "../canvas/unitTile";
import {getTexture} from "../canvas/pixi/pixiTextures";
import {PixiUnitTile} from "../canvas/pixi/pixiUnitTile";

export interface ICanvasService {
    dimensions(): Size
    init(container: HTMLElement): Promise<void>
    createHexagon(props: IHexagonProps): IHexagon
    changeHexagon(hex: IHexagon, newProps: IHexagonProps): IHexagon
    destroyHexagon(hex: IHexagon): void

    createUnit(props: IUnitTileProps): Promise<IUnitTile>
}

export class CanvasService implements ICanvasService {
    private app: PIXI.Application = new PIXI.Application();

    dimensions(): Size {
        return {width: this.app.canvas.width, height: this.app.canvas.height}
    }
    
    async init(container: HTMLElement): Promise<void> {
        this.app = new PIXI.Application()
        await this.app.init({ background: '#1099bb', resizeTo: container})
        container.appendChild(this.app.canvas)
    }
    
    async createUnit(props: IUnitTileProps): Promise<IUnitTile> {
        const container = new PIXI.Container()
        container.x = props.hexagon.x
        container.y = props.hexagon.y
        
        const hexProps: IHexagonProps = {
            x: 0,
            y: 0,
            fillAlpha: props.hexagon.fillAlpha,
            fillColor: props.hexagon.fillColor,
            radius: props.hexagon.radius,
            strokeColor: props.hexagon.strokeColor,
            strokeLine: props.hexagon.strokeLine,
        }
        const hexGraphics = new PIXI.Graphics()
        this.setHexagonGraphic(hexGraphics, hexProps)
        const hex = new PixiHexagon(hexProps, hexGraphics)
        
        const texture = await getTexture(props.imgSrc)
        const sprite = new PIXI.Sprite(texture)
        sprite.width = props.hexagon.radius * 2
        sprite.height = props.hexagon.radius * 2
        sprite.x = -props.hexagon.radius
        sprite.y = -props.hexagon.radius
        
        const maskRadius = props.hexagon.radius * 0.9
        const mask = new PIXI.Graphics()
        mask.beginFill(0xffffff); // Use white to fill the mask
        mask.drawPolygon([
            maskRadius * Math.cos(0), maskRadius * Math.sin(0),
            maskRadius * Math.cos(Math.PI / 3), maskRadius * Math.sin(Math.PI / 3),
            maskRadius * Math.cos((2 * Math.PI) / 3), maskRadius * Math.sin((2 * Math.PI) / 3),
            maskRadius * Math.cos(Math.PI), maskRadius * Math.sin(Math.PI),
            maskRadius * Math.cos((4 * Math.PI) / 3), maskRadius * Math.sin((4 * Math.PI) / 3),
            maskRadius * Math.cos((5 * Math.PI) / 3), maskRadius * Math.sin((5 * Math.PI) / 3)
        ]);
        mask.endFill();
        sprite.mask = mask
        
        const textStyle = new PIXI.TextStyle({fontFamily : 'Arial', fontSize: 22, fill : 0x000000, align : 'center'})
        const text = new PIXI.Text({
            text: props.text,
            style: textStyle
        })
        text.y = props.hexagon.radius - text.height * 1.4
        text.x = -text.width / 2
        
        const textTexture = await getTexture(props.textBackgroundImgSrc)
        const textBackgroundSprite = new PIXI.Sprite(textTexture)
        textBackgroundSprite.width = props.hexagon.radius * 0.8
        textBackgroundSprite.height = props.hexagon.radius * 0.5
        textBackgroundSprite.x = -textBackgroundSprite.width / 2
        textBackgroundSprite.y = props.hexagon.radius - textBackgroundSprite.height * 1.1
        
        container.addChild(hexGraphics)
        container.addChild(sprite)
        container.addChild(mask)
        container.addChild(textBackgroundSprite)
        container.addChild(text)
        
        this.app.stage.addChild(container)
        return new PixiUnitTile(props, hex, sprite, container, mask, text, textBackgroundSprite)
    }

    createHexagon(props: IHexagonProps): IHexagon {
        const hex = new PIXI.Graphics()
        this.setHexagonGraphic(hex, props)
        hex.interactive = true
        this.app.stage.addChild(hex)
        return new PixiHexagon(props, hex)
    }
    
    changeHexagon(hex: IHexagon, newProps: IHexagonProps): IHexagon {
        const pixiHex = hex as PixiHexagon
        if (!pixiHex) throw new Error("The input hexagon is not PIXI based")

        pixiHex.graphics.clear()
        this.setHexagonGraphic(pixiHex.graphics, newProps)
        pixiHex.update(newProps)
        
        return pixiHex
    }

    destroyHexagon(hex: IHexagon): void {
        const pixiHex = hex as PixiHexagon
        if (!pixiHex) throw new Error("The input hexagon is not PIXI based")

        pixiHex.dispose()
        pixiHex.graphics.parent.removeChild(pixiHex.graphics)
        pixiHex.graphics.destroy()
    }
    
    private setHexagonGraphic(hex: PIXI.Graphics, props: IHexagonProps) {
        // Set the line style for the hexagon border
        hex.lineStyle(props.strokeLine, props.strokeColor)
        // Set the fill style for the hexagon
        hex.beginFill(props.fillColor, props.fillAlpha)

        // Initialize an array to hold the vertices for the hitArea
        const vertices: number[] = []

        // Calculate and store the vertices
        for (let side = 0; side < 6; side++) {
            const vx = props.x + props.radius * Math.cos(side * 2 * Math.PI / 6)
            const vy = props.y + props.radius * Math.sin(side * 2 * Math.PI / 6)

            // Draw the line to the vertex
            if (side === 0) {
                hex.moveTo(vx, vy)
            } else {
                hex.lineTo(vx, vy)
            }

            // Add the vertex to the vertices array
            vertices.push(vx, vy)
        }

        // Close the hexagon path and apply the fill
        hex.lineTo(vertices[0], vertices[1])
        // End the fill
        hex.endFill()
        
        hex.hitArea = new PIXI.Polygon(vertices)
    }
}