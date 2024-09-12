import {IHexagon} from "../canvas/hexagon";
import {BattleMap, BattleMapCell} from "./battleMap";
import {ICanvasService} from "../services/canvasService";
import {IUnitTile} from "../canvas/unitTile";
import {getPlayerColor} from "../player/getPlayerColor";
import {BattleMapUnit} from "./battleMapUnit";
import {Point} from "../common/Point";

export interface IBattleMapController {
    readonly map: BattleMap
    readonly cellRadius: number
    readonly visualOffset: Point

    getUnit(row: number, col: number): BattleMapUnit | null
    moveUnit(unit: BattleMapUnit, row: number, col: number): Promise<void>
    removeUnit(unit: BattleMapUnit): Promise<void>

    setCellCustomColor(row: number, col: number, color?: number): void
    setCellDefaultColor(row: number, col: number, color: number): void
    restoreCellDefaultColor(row: number, col: number): void

    activateUnit(unit: BattleMapUnit): Promise<void>
    deactivateUnit(): Promise<void>
    
    unitTakeDamage(unit: BattleMapUnit, damage: number): Promise<boolean>

    setCursorForCell(row: number, col: number, cursor?: string): void
    setCursorForUnit(unit: BattleMapUnit, cursor?: string): void

    onCellMouseEnter: ((cell: BattleMapCell, event: PointerEvent) => void) | null
    onCellMouseLeave: ((cell: BattleMapCell, event: PointerEvent) => void) | null
    onCellMouseUp: ((cell: BattleMapCell, event: PointerEvent) => void) | null

    onUnitMouseMove: ((unit: BattleMapUnit, event: PointerEvent) => void) | null
    onUnitMouseEnter: ((unit: BattleMapUnit, event: PointerEvent) => void) | null
    onUnitMouseLeave: ((unit: BattleMapUnit, event: PointerEvent) => void) | null
    onUnitMouseUp: ((unit: BattleMapUnit, event: PointerEvent) => void) | null

    destroy(): void
}

export class BattleMapController implements IBattleMapController {
    map: BattleMap
    hexagons: IHexagon[][] = []
    units: IUnitTile[] = []

    activeUnit: IUnitTile | null = null

    onCellMouseEnter: ((cell: BattleMapCell, event: PointerEvent) => void) | null = null
    onCellMouseLeave: ((cell: BattleMapCell, event: PointerEvent) => void) | null = null
    onCellMouseUp: ((cell: BattleMapCell, event: PointerEvent) => void) | null = null

    onUnitMouseMove: ((unit: BattleMapUnit, event: PointerEvent) => void) | null = null
    onUnitMouseEnter: ((unit: BattleMapUnit, event: PointerEvent) => void) | null = null
    onUnitMouseLeave: ((unit: BattleMapUnit, event: PointerEvent) => void) | null = null
    onUnitMouseUp: ((unit: BattleMapUnit, event: PointerEvent) => void) | null = null

    readonly cellRadius: number = 60
    readonly visualOffset: Point

    private readonly defaultCellsStrokeColor = 0xFFFFFF
    private readonly defaultCellsFillColor = 0x66CCFF
    private readonly unitStrokeColor = 0x111111
    private readonly unitsNumberBackgroundImgSrc = "https://static.vecteezy.com/system/resources/thumbnails/012/981/790/small/old-parchment-paper-scroll-sheet-vintage-aged-or-texture-background-png.png"
    private readonly highlightIntervalMs = 500
    private readonly unitHighlightColor = 0xffea5e

    private readonly canvasService: ICanvasService

    private isHighlighted: boolean = false
    private readonly unitHighlightTimer: NodeJS.Timeout

    constructor(model: BattleMap, canvasService: ICanvasService) {
        this.map = model
        this.canvasService = canvasService;

        this.highlightActiveUnit = this.highlightActiveUnit.bind(this)
        this.unitHighlightTimer = setInterval(this.highlightActiveUnit, this.highlightIntervalMs)

        this.visualOffset = this.getCanvasOffset()
    }

    async unitTakeDamage(target: BattleMapUnit, damage: number): Promise<boolean> {
        target.currentHealth = target.currentHealth - damage
        if (target.currentHealth < 0) {
            const killedUnits = (target.currentHealth * (-1)) / target.props.health + 1
            target.currentHealth = target.currentHealth + killedUnits * target.props.health
            target.unitsCount = target.unitsCount - killedUnits
            
            if (target.unitsCount < 1) {
                await this.removeUnit(target)
                return true
            } else {
                const unitTile = this.getUnitTile(target)
                await this.canvasService.changeUnit(unitTile, {
                    ...unitTile,
                    text: target.unitsCount.toString()
                })
            }
        }
        return false
    }

    removeUnit(unit: BattleMapUnit): Promise<void> {
        const unitTile = this.getUnitTile(unit)
        this.canvasService.destroyUnit(unitTile)
        this.map.units.splice(this.map.units.indexOf(unit), 1)
        return Promise.resolve()
    }

    setCursorForCell(row: number, col: number, cursor?: string): void {
        const hexagon = this.getHexagon(row, col)
        this.canvasService.setCursorForHexagon(hexagon, cursor)
    }
    
    setCursorForUnit(unit: BattleMapUnit, cursor?: string): void {
        const unitTile = this.getUnitTile(unit)
        this.canvasService.setCursorForUnit(unitTile, cursor)
    }

    private getCanvasOffset(): Point {
        const offset = {x: 0, y: 0}
        const containerSize = this.canvasService.size()
        const gridDesiredSize = this.map.grid.getSize(this.cellRadius)

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

                hexagonView.onMouseEnters = (sender, event) => this.onCellMouseEnter?.(cell, event)
                hexagonView.onMouseLeaves = (sender, event) => this.onCellMouseLeave?.(cell, event)
                hexagonView.onMouseUp = (sender, event) => this.onCellMouseUp?.(cell, event)

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

            unitTile.onMouseMove = (sender, event) => this.onUnitMouseMove?.(unit, event)
            unitTile.onMouseEnters = (sender, event) => this.onUnitMouseEnter?.(unit, event)
            unitTile.onMouseLeaves = (sender, event) => this.onUnitMouseLeave?.(unit, event)
            unitTile.onMouseUp = (sender, event) => this.onUnitMouseUp?.(unit, event)

            this.units.push(unitTile)
        }
    }

    private setFillColor(cell: IHexagon, color: number) {
        this.canvasService.changeHexagon(cell, {...cell, fillColor: color})
    }
}
