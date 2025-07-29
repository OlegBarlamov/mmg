import * as PIXI from "pixi.js";
import {IHexagon, IHexagonProps} from "../canvas/hexagon";
import {PixiHexagon} from "../canvas/pixi/pixiHexagone";
import {Size} from "../common/Size";
import {IUnitTile, IUnitTileProps} from "../canvas/unitTile";
import {getTexture} from "../canvas/pixi/pixiTextures";
import {PixiUnitTile} from "../canvas/pixi/pixiUnitTile";

export enum HexagonStyle {
    QStyle,
    RStyle,
}

export interface ICanvasService {
    size(): Size
    init(container: HTMLElement, hexagonStyle: HexagonStyle): Promise<void>
    clear(): void
    createHexagon(props: IHexagonProps): IHexagon
    changeHexagon(hex: IHexagon, newProps: IHexagonProps): IHexagon
    destroyHexagon(hex: IHexagon): void

    createUnit(props: IUnitTileProps): Promise<IUnitTile>
    changeUnit(unit: IUnitTile, newProps: IUnitTileProps): Promise<IUnitTile>
    destroyUnit(unit: IUnitTile): void
    
    setCursorForHexagon(hex: IHexagon, cursor?: string): void
    setCursorForUnit(unit: IUnitTile, cursor?: string): void
}

export class CanvasService implements ICanvasService {
    private app: PIXI.Application = new PIXI.Application()
    private hexagonStyle: HexagonStyle = HexagonStyle.QStyle

    setCursorForHexagon(hex: IHexagon, cursor?: string): void {
        const pixiHex = hex as PixiHexagon
        if (!pixiHex) throw new Error("The input hexagon is not PIXI based")
        
        pixiHex.graphics.cursor = cursor ?? 'auto'
    }
    setCursorForUnit(unit: IUnitTile, cursor?: string): void {
        const pixiUnit = unit as PixiUnitTile
        if (!pixiUnit) throw new Error("The input unit is not PIXI based")
        
        pixiUnit.hexagonPixi.graphics.cursor = cursor ?? 'auto'
    }
    
    size(): Size {
        return {width: this.app.canvas.width, height: this.app.canvas.height}
    }
    
    async init(container: HTMLElement, hexagonStyle: HexagonStyle): Promise<void> {
        this.hexagonStyle = hexagonStyle;
        this.app = new PIXI.Application()
        await this.app.init({ background: '#1099bb', resizeTo: container})
        container.appendChild(this.app.canvas)
    }

    clear(): void {
        if (this.app) {
            this.app.destroy(true, { children: true })
            this.app = new PIXI.Application()
        }
    }

    changeUnit(unit: IUnitTile, newProps: IUnitTileProps): Promise<IUnitTile> {
        const pixiUnit = unit as PixiUnitTile
        if (!pixiUnit) throw new Error("The input unit is not PIXI based")
        
        if (pixiUnit.text !== newProps.text) {
            pixiUnit.textGraphics.text = newProps.text
        }
        
        this.changeHexagon(pixiUnit.hexagonPixi, {
            ...newProps.hexagon,
            x: 0,
            y: 0,
        })
        pixiUnit.container.x = newProps.hexagon.x
        pixiUnit.container.y = newProps.hexagon.y
        
        pixiUnit.update(newProps)
        
        return Promise.resolve(pixiUnit)
    }
    destroyUnit(unit: IUnitTile): void {
        const pixiUnit = unit as PixiUnitTile
        if (!pixiUnit) throw new Error("The input unit is not PIXI based")
        pixiUnit.dispose()
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
        hexGraphics.interactive = true
        const hex = new PixiHexagon(hexProps, hexGraphics)
        
        const texture = await getTexture(props.imgSrc)
        const sprite = new PIXI.Sprite(texture)
        sprite.width = props.hexagon.radius * 2
        sprite.height = props.hexagon.radius * 2
        sprite.x = -props.hexagon.radius
        sprite.y = -props.hexagon.radius
        
        const mask = new PIXI.Graphics()
        this.setHexagonGraphic(mask, {
            ...hexProps,
            radius: props.hexagon.radius * 0.9,
        })
        sprite.mask = mask
        
        const textStyle = new PIXI.TextStyle({fontFamily : 'Arial', fontSize: props.hexagon.radius * 0.3, fill : 0x000000, align : 'center'})
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
    }
    
    private setHexagonGraphic(hex: PIXI.Graphics, props: IHexagonProps): void {
        hex.lineStyle(props.strokeLine, props.strokeColor)
        hex.beginFill(props.fillColor, props.fillAlpha)

        const vertices: number[] = []
        if (this.hexagonStyle === HexagonStyle.QStyle) {
            this.setHexagonQStyleGeometry(hex, vertices, props)
        } else {
            this.setHexagonRStyleGeometry(hex, vertices, props)
        }
        
        hex.endFill()
        
        hex.hitArea = new PIXI.Polygon(vertices)
    }
    
    private setHexagonQStyleGeometry(hex: PIXI.Graphics, verticesArray: number[], props: IHexagonProps): void {
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
            verticesArray.push(vx, vy)
        }

        // Close the hexagon path and apply the fill
        hex.lineTo(verticesArray[0], verticesArray[1])
    }
    
    private setHexagonRStyleGeometry(hex: PIXI.Graphics, verticesArray: number[], props: IHexagonProps): void {
        // Calculate and store the vertices of a flat-topped hexagon
        for (let side = 0; side < 6; side++) {
            // Flat-topped hexagon vertices: angle starts at π/6 and increments by π/3 (60 degrees)
            const angle = (Math.PI / 6) + (side * Math.PI / 3);  // Start at 30 degrees (π/6) for flat-topped hexagons
            const vx = props.x + props.radius * Math.cos(angle); // x-coordinate of vertex
            const vy = props.y + props.radius * Math.sin(angle); // y-coordinate of vertex

            // Move to the first vertex, then draw lines to the remaining vertices
            if (side === 0) {
                hex.moveTo(vx, vy);
            } else {
                hex.lineTo(vx, vy);
            }

            // Add the vertex to the vertices array (for hit area or future use)
            verticesArray.push(vx, vy);
        }

        // Close the hexagon path and apply the fill
        hex.lineTo(verticesArray[0], verticesArray[1]);  // Connect last vertex back to the first
    }
}