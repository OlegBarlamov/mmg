import {IHexagon} from "../canvas/hexagon";
import {BattleMap, BattleMapCell} from "./battleMap";
import {ICanvasService} from "../services/canvasService";
import {Point} from "../common/Point";
import {IUnitTile} from "../canvas/unitTile";
import {getPlayerColor} from "../player/getPlayerColor";
import {BattleMapUnit} from "./battleMapUnit";

export interface IBattleMapController {
    readonly map: BattleMap
    
    getUnit(row: number, col: number): BattleMapUnit | null
    
    highlightCell(row: number, col: number): void
    activateUnit(unit: BattleMapUnit): Promise<void>
    deactivateUnit(): Promise<void>
    
    destroy(): void 
}

export class BattleMapController implements IBattleMapController{
    map: BattleMap
    hexagons: IHexagon[][] = []
    units: IUnitTile[] = []

    activeUnit: IUnitTile | null = null
    
    private readonly cellRadius: number = 75
    
    private readonly defaultStrokeColor = 0xFFFFFF
    private readonly defaultFillColor = 0x66CCFF
    private readonly unitStrokeColor = 0x111111
    private readonly unitsNumberBackgroundImgSrc = "https://static.vecteezy.com/system/resources/thumbnails/012/981/790/small/old-parchment-paper-scroll-sheet-vintage-aged-or-texture-background-png.png"
    private readonly highlightIntervalMs = 500
    private readonly highlightFillColor = 0xffea5e
    
    private readonly canvasService: ICanvasService;
    
    private isHighlighted: boolean = false
    private readonly unitHighlightTimer: NodeJS.Timeout

    constructor(model: BattleMap, canvasService: ICanvasService) {
        this.map = model
        this.canvasService = canvasService;
        
        this.highlightActiveUnit = this.highlightActiveUnit.bind(this)
        this.unitHighlightTimer = setInterval(this.highlightActiveUnit, this.highlightIntervalMs)
        
        // TODO center the field?
        // const leftCell = this.model.cells[0][0]
        // const topCell = model.width > 0 ? this.model.cells[0][1] : leftCell 
        // const rightCell = this.model.cells[this.model.height - 1][this.model.width - 1]
        // const bottomCell = model.width > 0 ? this.model.cells[this.model.height - 1][this.model.width - 2] : rightCell
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
                fillColor: this.highlightFillColor
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

    highlightCell(row: number, col: number): void {
        const cell = this.getHexagon(row, col)
        const color = 0x6dbfac
        cell.customFillColor = color
        this.setFillColor(cell, color)
    }

    loadMap(): Promise<void> {
        if (this.hexagons.length !== 0) throw new Error("Cannot regenerate the visuals of already loaded map")
        
        for (let r = 0; r < this.map.grid.height; r++) {
            const row: IHexagon[] = [];
            for (let c = 0; c < this.map.grid.width; c++) {
                const center = this.map.grid.getCellCenterPoint(r, c, this.cellRadius)
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