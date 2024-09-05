import {IHexagon} from "../canvas/hexagon";
import {BattleMap} from "./battleMap";
import {ICanvasService} from "../services/canvasService";
import {Point} from "../common/Point";
import {IUnitTile} from "../canvas/unitTile";
import {getPlayerColor} from "../player/getPlayerColor";
import {Size} from "../common/Size";

const sqrt3 = Math.sqrt(3)

export interface IBattleMapController {
    loadMap(): Promise<void>
    
    getCell(row: number, col: number): IHexagon 
    getCellCenterPoint(row: number, col: number): Point
    
    highlightCell(row: number, col: number): void
}

export class BattleMapController implements IBattleMapController{
    private cells: IHexagon[][] = []
    private units: IUnitTile[] = []
    
    private readonly model: BattleMap
    private readonly cellRadius: number = 75
    
    private readonly defaultStrokeColor = 0xFFFFFF
    private readonly defaultFillColor = 0x66CCFF
    private readonly unitStrokeColor = 0x111111
    private readonly unitsNumberBackgroundImgSrc = "https://static.vecteezy.com/system/resources/thumbnails/012/981/790/small/old-parchment-paper-scroll-sheet-vintage-aged-or-texture-background-png.png"
    
    private readonly canvasService: ICanvasService;

    constructor(model: BattleMap, canvasService: ICanvasService) {
        this.model = model
        this.canvasService = canvasService;
        
        // TODO center the field?
        // const leftCell = this.model.cells[0][0]
        // const topCell = model.width > 0 ? this.model.cells[0][1] : leftCell 
        // const rightCell = this.model.cells[this.model.height - 1][this.model.width - 1]
        // const bottomCell = model.width > 0 ? this.model.cells[this.model.height - 1][this.model.width - 2] : rightCell
    }

    getCell(row: number, col: number): IHexagon {
        return this.cells[row][col]
    }

    highlightCell(row: number, col: number): void {
        const cell = this.getCell(row, col)
        const color = 0x6dbfac
        cell.customFillColor = color
        this.setFillColor(cell, color)
    }

    getCellCenterPoint(row: number, col: number): Point {
        const hexWidth = 2 * this.cellRadius
        const hexHeight = sqrt3 * this.cellRadius
        const x = col * hexWidth * 0.75 + this.cellRadius
        const y = row * hexHeight + (col % 2 === 0 ? hexHeight / 2 : 0) + this.cellRadius
        return { x, y }
    }

    loadMap(): Promise<void> {
        if (this.cells.length !== 0) throw new Error("Cannot regenerate the visuals of already loaded map")
        
        for (let r = 0; r < this.model.height; r++) {
            const row: IHexagon[] = [];
            for (let c = 0; c < this.model.width; c++) {
                const center = this.getCellCenterPoint(r, c)
                const hexagonView = this.canvasService.createHexagon({
                    x: center.x,
                    y: center.y,
                    radius: this.cellRadius,
                    strokeColor: this.defaultStrokeColor,
                    fillColor: this.defaultFillColor,
                    strokeLine: 2,
                    fillAlpha: 1.0,
                })
                
                hexagonView.onMouseEnters = this.onCellMouseEnters.bind(this)
                hexagonView.onMouseLeaves = this.onCellMouseLeaves.bind(this)
                
                row.push(hexagonView);
            }
            this.cells.push(row);
        }
        
        return Promise.resolve() 
    }
    
    async loadUnits() : Promise<void> {
        if (this.units.length !== 0) throw new Error("Cannot regenerate the units tiles of already loaded map")
        
        for (const unit of this.model.units) {
            const center = this.getCellCenterPoint(unit.position.r, unit.position.c)
            const unitTile = await this.canvasService.createUnit({
                hexagon: {
                    x: center.x,
                    y: center.y,
                    radius: this.cellRadius,
                    strokeColor: this.unitStrokeColor,
                    fillColor: getPlayerColor(unit.player),
                    strokeLine: 2,
                    fillAlpha: 1,
                },
                text: unit.unitsCount.toString(),
                textBackgroundImgSrc: this.unitsNumberBackgroundImgSrc,
                imgSrc: unit.props.battleMapIcon
            })
            this.units.push(unitTile)
        }
    }
    
    private onCellMouseEnters(cell: IHexagon) {
        this.setFillColor(cell, 0x32a852)
    }

    private onCellMouseLeaves(cell: IHexagon) {
        this.setFillColor(cell, cell.customFillColor || this.defaultFillColor)
    }
    
    private setFillColor(cell: IHexagon, color: number) {
        this.canvasService.changeHexagon(cell, {...cell, fillColor: color})
    }
}