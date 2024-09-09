import {IHexagon} from "../canvas/hexagon";
import {BattleMap, BattleMapCell} from "./battleMap";
import {ICanvasService} from "../services/canvasService";
import {Point} from "../common/Point";
import {IUnitTile} from "../canvas/unitTile";
import {getPlayerColor} from "../player/getPlayerColor";
import {BattleMapUnit} from "./battleMapUnit";

const sqrt3 = Math.sqrt(3)

export interface IBattleMapController {
    readonly map: BattleMap
    
    isValidCell(row: number, col: number): boolean
    getCell(row: number, col: number): BattleMapCell 
    getCellCenterPoint(row: number, col: number): Point
    getCellsInRange(row: number, col: number, range: number): BattleMapCell[]
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

    isValidCell(row: number, col: number): boolean {
        return row >= 0 && row < this.map.height && col >= 0 && col < this.map.width;
    }

    getCellsInRange(row: number, col: number, range: number): BattleMapCell[] {
        const centerCube = this.offsetToCube(row, col);
        const cellsInRadius: BattleMapCell[] = [];

        // Loop over the cube coordinates in the radius range
        for (let dq = -range; dq <= range; dq++) {
            for (let dr = Math.max(-range, -dq - range); dr <= Math.min(range, -dq + range); dr++) {
                const ds = -dq - dr;

                const cube = {
                    q: centerCube.q + dq,
                    r: centerCube.r + dr,
                    s: centerCube.s + ds,
                };

                // Convert the cube coordinate back to offset coordinates
                const offset = this.cubeToOffset(cube.q, cube.r);

                // Check if the cell exists in the grid (bounds checking)
                if (this.isValidCell(offset.row, offset.col)) {
                    cellsInRadius.push(this.getCell(offset.row, offset.col));
                }
            }
        }

        return cellsInRadius;
    }

    getCell(row: number, col: number): BattleMapCell {
        return this.map.cells[row][col]
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
        
        debugger
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

    getCellCenterPoint(row: number, col: number): Point {
        const hexWidth = 2 * this.cellRadius
        const hexHeight = sqrt3 * this.cellRadius
        const x = col * hexWidth * 0.75 + this.cellRadius
        const y = row * hexHeight + (col % 2 === 0 ? hexHeight / 2 : 0) + this.cellRadius
        return { x, y }
    }

    loadMap(): Promise<void> {
        if (this.hexagons.length !== 0) throw new Error("Cannot regenerate the visuals of already loaded map")
        
        for (let r = 0; r < this.map.height; r++) {
            const row: IHexagon[] = [];
            for (let c = 0; c < this.map.width; c++) {
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
            this.hexagons.push(row);
        }
        
        return Promise.resolve() 
    }
    
    async loadUnits() : Promise<void> {
        if (this.units.length !== 0) throw new Error("Cannot regenerate the units tiles of already loaded map")
        
        for (const unit of this.map.units) {
            const center = this.getCellCenterPoint(unit.position.r, unit.position.c)
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
    
    private offsetToCube(row: number, col: number): { q: number, r: number, s: number } {
        const q = col;
        const r = row - (col - (col & 1)) / 2;
        const s = -q - r;
        return { q, r, s };
    }
    
    private cubeToOffset(q: number, r: number): { row: number, col: number } {
        const col = q;
        const row = r + (q - (q & 1)) / 2;
        return { row, col };
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