import * as PIXI from "pixi.js";
import {IHexagon} from "../canvas/hexagon";
import {PixiHexagon} from "../canvas/pixi/pixiHexagone";
import {Size} from "../common/Size";

export interface ICanvasService {
    dimensions(): Size
    init(container: HTMLElement): Promise<void>
    createHexagon(props: IHexagon): IHexagon
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
    
    createHexagon(props: IHexagon): IHexagon {
        const hex = new PIXI.Graphics();
        // Set the line style for the hexagon border
        hex.lineStyle(2, props.strokeColor);
        // Set the fill style for the hexagon
        hex.beginFill(props.fillColor, props.fillAlpha);

        // Initialize an array to hold the vertices for the hitArea
        const vertices: number[] = [];

        // Calculate and store the vertices
        for (let side = 0; side < 6; side++) {
            const vx = props.x + props.radius * Math.cos(side * 2 * Math.PI / 6);
            const vy = props.y + props.radius * Math.sin(side * 2 * Math.PI / 6);

            // Draw the line to the vertex
            if (side === 0) {
                hex.moveTo(vx, vy);
            } else {
                hex.lineTo(vx, vy);
            }

            // Add the vertex to the vertices array
            vertices.push(vx, vy);
        }

        // Close the hexagon path and apply the fill
        hex.lineTo(vertices[0], vertices[1]);
        // End the fill
        hex.endFill()
        
        hex.interactive = true
        hex.hitArea = new PIXI.Polygon(vertices);
        
        // Add the hexagon to the stage
        this.app.stage.addChild(hex)
        return new PixiHexagon(props, hex)
    }
}