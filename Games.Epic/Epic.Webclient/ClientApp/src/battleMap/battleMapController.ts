import { IHexagon } from "../canvas/hexagon";
import { BattleMap, BattleMapCell } from "./battleMap";
import { ICanvasService } from "../services/canvasService";
import { IUnitTile } from "../canvas/unitTile";
import { getPlayerColor } from "../player/getPlayerColor";
import { BattleMapUnit } from "./battleMapUnit";
import { Point } from "../common/Point";
import { distance } from "../common/math";
import { BattleMapHighlighter, IBattleMapHighlighter } from "./battleMapHighlighter";

export interface IBattleMapController {
    readonly map: BattleMap
    readonly battleMapHighlighter: IBattleMapHighlighter

    moveUnit(unit: BattleMapUnit, row: number, col: number): Promise<void>
    removeUnit(unit: BattleMapUnit): Promise<void>

    getClosestCellToPoint(cells: BattleMapCell[], canvasPoint: Point): BattleMapCell

    unitTakeDamage(unit: BattleMapUnit, damageTaken: number, killedCount: number, remainingCount: number, remainingHealth: number): Promise<void>

    onCellMouseClick: ((cell: BattleMapCell, event: PointerEvent) => void) | null
    onUnitMouseClick: ((unit: BattleMapUnit, event: PointerEvent) => void) | null

    destroy(): void
}

export class BattleMapController implements IBattleMapController {
    readonly map: BattleMap
    readonly battleMapHighlighter: IBattleMapHighlighter

    onCellMouseClick: ((cell: BattleMapCell, event: PointerEvent) => void) | null = null
    onUnitMouseClick: ((unit: BattleMapUnit, event: PointerEvent) => void) | null = null

    onCellMouseEnter: ((cell: BattleMapCell, event: PointerEvent) => void) | null = null
    onCellMouseLeave: ((cell: BattleMapCell, event: PointerEvent) => void) | null = null

    onUnitMouseMove: ((unit: BattleMapUnit, event: PointerEvent) => void) | null = null
    onUnitMouseEnter: ((unit: BattleMapUnit, event: PointerEvent) => void) | null = null
    onUnitMouseLeave: ((unit: BattleMapUnit, event: PointerEvent) => void) | null = null

    private hexagons: IHexagon[][] = []
    private units: IUnitTile[] = []

    private readonly cellRadius: number = 60
    private readonly visualOffset: Point
    private readonly defaultCellsStrokeColor = 0xFFFFFF
    private readonly defaultCellsFillColor = 0x66CCFF
    private readonly unitStrokeColor = 0x111111
    private readonly unitsNumberBackgroundImgSrc = "https://static.vecteezy.com/system/resources/thumbnails/012/981/790/small/old-parchment-paper-scroll-sheet-vintage-aged-or-texture-background-png.png"

    private readonly canvasService: ICanvasService
    private readonly battleMapHighlighterImpl: BattleMapHighlighter

    constructor(model: BattleMap, canvasService: ICanvasService) {
        this.map = model
        this.canvasService = canvasService
        this.battleMapHighlighterImpl = new BattleMapHighlighter(canvasService, this, this.defaultCellsFillColor)
        this.battleMapHighlighter = this.battleMapHighlighterImpl

        this.visualOffset = this.getCanvasOffset()
    }

    destroy(): void {
        this.battleMapHighlighterImpl.destroy()

        this.units.forEach(x => this.canvasService.destroyUnit(x))
        this.units = []

        this.hexagons.forEach(rows => rows.forEach(x => this.canvasService.destroyHexagon(x)))
        this.hexagons = []
    }

    getClosestCellToPoint(cells: BattleMapCell[], canvasPoint: Point): BattleMapCell {
        const hexagonCenters = cells.map(cell => {
            return {
                cell,
                centerPoint: this.map.grid.getCellCenterPoint(cell.r, cell.c, this.cellRadius)
            }
        })

        let closestHexagon = hexagonCenters[0]
        let closestHexagonDistanceSqr = Number.MAX_SAFE_INTEGER
        for (const hexagonPair of hexagonCenters) {
            const distanceSqr = distance(canvasPoint, hexagonPair.centerPoint)
            if (distanceSqr < closestHexagonDistanceSqr) {
                closestHexagon = hexagonPair
                closestHexagonDistanceSqr = distanceSqr
            }
        }

        return closestHexagon.cell
    }

    async unitTakeDamage(target: BattleMapUnit, damageTaken: number, killedCount: number, remainingCount: number, remainingHealth: number): Promise<void> {
        target.currentProps.health = remainingHealth
        target.count = remainingCount

        if (target.count < 1) {
            await this.removeUnit(target)
        } else {
            const unitTile = this.getUnitTile(target)
            await this.canvasService.changeUnit(unitTile, {
                ...unitTile,
                text: target.count.toString()
            })
        }
    }

    removeUnit(unit: BattleMapUnit): Promise<void> {
        const unitTile = this.getUnitTile(unit)
        this.canvasService.destroyUnit(unitTile)
        const unitIndex = this.map.units.indexOf(unit)
        if (unitIndex >= 0) {
            this.map.units.splice(this.map.units.indexOf(unit), 1)
        }
        this.units.splice(this.units.indexOf(unitTile), 1)
        return Promise.resolve()
    }

    private getCanvasOffset(): Point {
        const offset = { x: 0, y: 0 }
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
        unit.position = { r, c }
    }

    getHexagon(row: number, col: number): IHexagon {
        return this.hexagons[row][col]
    }

    getUnitTile(unit: BattleMapUnit): IUnitTile {
        const tileUnit = this.units.find(x => x.model === unit)
        if (!tileUnit) throw new Error("Unit not found on the map")
        return tileUnit
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

                hexagonView.onMouseEnters = (sender, event) => this.onCellMouseEnter?.(cell, this.translatePointerEvent(event))
                hexagonView.onMouseLeaves = (sender, event) => this.onCellMouseLeave?.(cell, this.translatePointerEvent(event))
                hexagonView.onMouseUp = (sender, event) => this.onCellMouseClick?.(cell, this.translatePointerEvent(event))

                row.push(hexagonView);
            }
            this.hexagons.push(row);
        }

        return Promise.resolve()
    }

    async loadUnits(): Promise<void> {
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
                text: unit.count.toString(),
                textBackgroundImgSrc: this.unitsNumberBackgroundImgSrc,
                imgSrc: unit.currentProps.battleImgUrl
            })

            unitTile.onMouseMove = (sender, event) => this.onUnitMouseMove?.(unit, this.translatePointerEvent(event))
            unitTile.onMouseEnters = (sender, event) => this.onUnitMouseEnter?.(unit, this.translatePointerEvent(event))
            unitTile.onMouseLeaves = (sender, event) => this.onUnitMouseLeave?.(unit, this.translatePointerEvent(event))
            unitTile.onMouseUp = (sender, event) => this.onUnitMouseClick?.(unit, this.translatePointerEvent(event))

            this.units.push(unitTile)
        }
    }

    private translatePointerEvent(event: PointerEvent): PointerEvent {
        return {
            ...event,
            x: event.x - this.visualOffset.x,
            y: event.y - this.visualOffset.y,
        }
    }
}
