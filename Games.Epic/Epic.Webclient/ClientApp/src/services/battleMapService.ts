import {ICanvasService} from "./canvasService";
import {IHexagon} from "../canvas/hexagon";
import {Point} from "../common/Point";
import {BattleMap, BattleMapCell} from "../battleMap/battleMap";
import {BattleMapController, IBattleMapController} from "../battleMap/battleMapController";

export interface IBattleMapService {
    readonly cellRadius: number
    load(map: BattleMap): Promise<IBattleMapController>
    cellCenter(row: number, col: number): Point
    getCell(row: number, col: number, map: BattleMap): BattleMapCell
    generateMap(width: number, height: number) : BattleMap
}

export class BattleMapService implements IBattleMapService {
    readonly cellRadius: number = 75
    private readonly sqrt3 = Math.sqrt(3)
    
    private canvasService: ICanvasService
    constructor(canvasService: ICanvasService) {
        this.canvasService = canvasService

    }
    getCell(row: number, col: number, map: BattleMap): BattleMapCell {
        throw new Error("Method not implemented.");
    }
    
    load(map: BattleMap): Promise<IBattleMapController> {
        const cellViews: IHexagon[][] = [];
        for (let r = 0; r < map.height; r++) {
            const row: IHexagon[] = [];
            for (let c = 0; c < map.width; c++) {
                const center = this.cellCenter(r, c)
                const hexagonView = this.canvasService.createHexagon({
                    x: center.x,
                    y: center.y,
                    radius: this.cellRadius,
                    strokeColor: 0x000000, // Default stroke color
                    fillColor: 0x66CCFF, // Default fill color
                    fillAlpha: 1.0, // Default fill alpha
                })
                row.push(hexagonView);
            }
            cellViews.push(row);
        }

        return Promise.resolve(new BattleMapController(map, cellViews))
    }
    
    cellCenter(row: number, col: number): Point {
        const hexWidth = 2 * this.cellRadius
        const hexHeight = this.sqrt3 * this.cellRadius
        const x = col * hexWidth * 0.75 + this.cellRadius
        const y = row * hexHeight + (col % 2 === 0 ? hexHeight / 2 : 0) + this.cellRadius
        return { x, y }
    }

    generateMap(width: number, height: number) : BattleMap {
        const cells: BattleMapCell[][] = []
        for (let i = 0; i < height; i++) {
            const row: BattleMapCell[] = []
            for (let j = 0; j < width; j++) {
                const cell: BattleMapCell = { c: j, r: i }
                row.push(cell);
            }
            cells.push(row)
        }
        return {
            width,
            height,
            cells,
        }
    }
}