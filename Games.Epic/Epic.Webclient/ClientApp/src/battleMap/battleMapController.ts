import {IHexagon} from "../canvas/hexagon";
import {BattleMap, BattleMapCell} from "./battleMap";
import {ICanvasService} from "../services/canvasService";
import {IUnitTile} from "../canvas/unitTile";
import {getPlayerColor} from "../player/getPlayerColor";
import {BattleMapUnit} from "./battleMapUnit";
import {Point} from "../common/Point";

export interface IBattleMapController {
    readonly map: BattleMap
    
    getUnit(row: number, col: number): BattleMapUnit | null
    moveUnit(unit: BattleMapUnit, row: number, col: number): Promise<void>
    
    setCellCustomColor(row: number, col: number, color?: number): void
    setCellDefaultColor(row: number, col: number, color: number): void
    restoreCellDefaultColor(row: number, col: number): void
    
    activateUnit(unit: BattleMapUnit): Promise<void>
    deactivateUnit(): Promise<void>
    
    onCellMouseEnter: ((cell: BattleMapCell) => void) | null
    onCellMouseLeave: ((cell: BattleMapCell) => void) | null
    onCellMouseUp: ((cell: BattleMapCell) => void) | null
    
    destroy(): void 
}

export class BattleMapController implements IBattleMapController{
    map: BattleMap
    hexagons: IHexagon[][] = []
    units: IUnitTile[] = []

    activeUnit: IUnitTile | null = null

    onCellMouseEnter: ((cell: BattleMapCell) => void) | null = null
    onCellMouseLeave: ((cell: BattleMapCell) => void) | null = null
    onCellMouseUp: ((cell: BattleMapCell) => void) | null = null
    
    private readonly cellRadius: number = 60
    
    private readonly defaultCellsStrokeColor = 0xFFFFFF
    private readonly defaultCellsFillColor = 0x66CCFF
    private readonly unitStrokeColor = 0x111111
    private readonly unitsNumberBackgroundImgSrc = "https://static.vecteezy.com/system/resources/thumbnails/012/981/790/small/old-parchment-paper-scroll-sheet-vintage-aged-or-texture-background-png.png"
    private readonly highlightIntervalMs = 500
    private readonly unitHighlightColor = 0xffea5e
    
    private readonly canvasService: ICanvasService
    private readonly visualOffset: Point 
    
    private isHighlighted: boolean = false
    private readonly unitHighlightTimer: NodeJS.Timeout

    constructor(model: BattleMap, canvasService: ICanvasService) {
        this.map = model
        this.canvasService = canvasService;
        
        this.highlightActiveUnit = this.highlightActiveUnit.bind(this)
        this.unitHighlightTimer = setInterval(this.highlightActiveUnit, this.highlightIntervalMs)
        
        this.visualOffset = this.getCanvasOffset()
    }
    
    private getCanvasOffset(): Point {
        const offset = {x: 0, y: 0}
        const containerSize = this.canvasService.size()
        const gridDesiredSize = this.map.grid.getSize(this.cellRadius)
        
        debugger
        if (gridDesiredSize.width < containerSize.width) {
            offset.x = (containerSize.width - gridDesiredSize.width) / 2
        }
        if (gridDesiredSize.height < containerSize.height) {
            offset.y = (containerSize.height - gridDesiredSize.height) / 2
        }
        
        return offset
    }

    async moveUnit(unit: BattleMapUnit, r: number, c: number): Promise<void> {
        const unitTile = this.getUnitTile(unit)

        const newPoint = this.map.grid.getCellCenterPoint(r, c, this.cellRadius)
        await this.canvasService.changeUnit(unitTile, {
            ...unitTile,
            hexagon: {
                ...unitTile.hexagon,
                x: newPoint.x + this.visualOffset.x,
                y: newPoint.y + this.visualOffset.y,
            }
        })
        unit.position = {r, c}
    }

    destroy(): void {
        clearInterval(this.unitHighlightTimer)
        this.activeUnit = null
        
        this.units.forEach(x => this.canvasService.destroyUnit(x))
        this.units = []
        this.hexagons.forEach(rows => rows.forEach(x => this.canvasService.destroyHexagon(x)))
        this.hexagons = []
    }

    async activateUnit(unit: BattleMapUnit): Promise<void> {
        if (this.activeUnit != null) {
            await this.deactivateUnit()
        }

        this.activeUnit = this.getUnitTile(unit)
    }
    
    async deactivateUnit(): Promise<void> {
        if (this.activeUnit == null) throw new Error("There is no unit to deactivate")
        if (this.isHighlighted) {
            await this.restoreHighlightUnitTile(this.activeUnit)
        }
        this.activeUnit = null
    }

    getUnit(row: number, col: number): BattleMapUnit | null {
        return this.map.units.find(x => x.position.r === row && x.position.c === col) ?? null
    }
    
    private getHexagon(row: number, col: number): IHexagon {
        return this.hexagons[row][col]
    }
    
    private getUnitTile(unit: BattleMapUnit): IUnitTile {
        const tileUnit = this.units.find(x => x.model === unit)
        if (!tileUnit) throw new Error("Unit not found on the map")
        return tileUnit
    }
    
    private async highlightActiveUnit(): Promise<void> {
        if (this.activeUnit === null) {
            return
        }
        
        try {
            if (this.isHighlighted) {
                await this.restoreHighlightUnitTile(this.activeUnit)
            } else {
                await this.highlightUnitTile(this.activeUnit)
            }
        } catch (e) {
            console.error("Error while highlighting the active unit: " + e)
        }
    }
    
    private highlightUnitTile(unit: IUnitTile): Promise<IUnitTile> {
        this.isHighlighted = true
        
        return this.canvasService.changeUnit(unit, {
            ...unit,
            hexagon: {
                ...unit.hexagon,
                fillColor: this.unitHighlightColor
            }
        })
    }

    private restoreHighlightUnitTile(unit: IUnitTile): Promise<IUnitTile> {
        this.isHighlighted = false

        return this.canvasService.changeUnit(unit, {
            ...unit,
            hexagon: {
                ...unit.hexagon,
                fillColor: getPlayerColor(unit.model.player)
            }
        })
    }

    setCellCustomColor(row: number, col: number, color?: number): void {
        const cell = this.getHexagon(row, col)
        this.setFillColor(cell, color ?? cell.customFillColor ?? this.defaultCellsFillColor)
    }

    setCellDefaultColor(row: number, col: number, color: number): void {
        const cell = this.getHexagon(row, col)
        cell.customFillColor = color
        this.setFillColor(cell, color)
    }

    restoreCellDefaultColor(row: number, col: number) {
        const cell = this.getHexagon(row, col)
        cell.customFillColor = undefined
        this.setFillColor(cell, this.defaultCellsFillColor)
    }

    loadMap(): Promise<void> {
        if (this.hexagons.length !== 0) throw new Error("Cannot regenerate the visuals of already loaded map")
        
        for (let r = 0; r < this.map.grid.height; r++) {
            const row: IHexagon[] = [];
            for (let c = 0; c < this.map.grid.width; c++) {
                const cell = this.map.grid.getCell(r, c)
                const center = this.map.grid.getCellCenterPoint(r, c, this.cellRadius)
                const hexagonView = this.canvasService.createHexagon({
                    x: center.x + this.visualOffset.x,
                    y: center.y + this.visualOffset.y,
                    radius: this.cellRadius,
                    strokeColor: this.defaultCellsStrokeColor,
                    fillColor: this.defaultCellsFillColor,
                    strokeLine: 2,
                    fillAlpha: 1.0,
                })
                
                hexagonView.onMouseEnters = () => this.onCellMouseEnter?.(cell)
                hexagonView.onMouseLeaves = () => this.onCellMouseLeave?.(cell)
                hexagonView.onMouseUp = () => this.onCellMouseUp?.(cell)
                
                row.push(hexagonView);
            }
            this.hexagons.push(row);
        }
        
        return Promise.resolve() 
    }
    
    async loadUnits() : Promise<void> {
        if (this.units.length !== 0) throw new Error("Cannot regenerate the units tiles of already loaded map")
        
        for (const unit of this.map.units) {
            const center = this.map.grid.getCellCenterPoint(unit.position.r, unit.position.c, this.cellRadius)
            const unitTile = await this.canvasService.createUnit({
                model: unit,
                hexagon: {
                    x: center.x + this.visualOffset.x,
                    y: center.y + this.visualOffset.y,
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
    
    private setFillColor(cell: IHexagon, color: number) {
        this.canvasService.changeHexagon(cell, {...cell, fillColor: color})
    }
}