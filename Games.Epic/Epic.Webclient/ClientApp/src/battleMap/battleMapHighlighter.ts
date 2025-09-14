import {BattleMapCell} from "./battleMap";
import {IAttackTarget} from "../battle/attackTarget";
import {BattleMapUnit} from "./battleMapUnit";
import {IHexagon} from "../canvas/hexagon";
import {IUnitTile} from "../canvas/unitTile";
import {getPlayerColor} from "../player/getPlayerColor";
import {ICanvasService} from "../services/canvasService";
import {BattleMapController} from "./battleMapController";

export interface IBattleMapHighlighter {
    readonly activeUnit: BattleMapUnit | null

    highlightCellsForMove(cellsForMove: BattleMapCell[]): void
    restoreHighlightingForCells(cellsForMove: BattleMapCell[]): void
    highlightAttackTargets(attackTargets: IAttackTarget[]): void
    restoreHighlightingForAttackTargets(attackTargets: IAttackTarget[]): void

    setActiveUnit(unit: BattleMapUnit | null): Promise<void>
} 

export class BattleMapHighlighter implements IBattleMapHighlighter {
    activeUnit: BattleMapUnit | null = null

    private isHighlighted: boolean = false

    private readonly unitHighlightTimer: NodeJS.Timeout

    private readonly highlightIntervalMs = 500
    private readonly unitHighlightColor = 0xffea5e
    private readonly movableCellsColor = 0x6dbfac
    private readonly movableCellsHoverColor = 0x32a852
    private readonly cellAttackFromHighlightColor = 0xffa08f 
    private readonly cellsAttackFromHoverColor = 0xfa3b19 
    
    private readonly canvasService: ICanvasService
    private readonly battleMapController: BattleMapController
    private readonly defaultCellsFillColor: number
    
    constructor(canvasService: ICanvasService, battleMapController: BattleMapController, defaultCellsFillColor: number) {
        this.canvasService = canvasService
        this.battleMapController = battleMapController
        this.defaultCellsFillColor = defaultCellsFillColor

        this.highlightActiveUnit = this.highlightActiveUnit.bind(this)
        this.unitHighlightTimer = setInterval(this.highlightActiveUnit, this.highlightIntervalMs)
    }
    
    destroy(): void {
        clearInterval(this.unitHighlightTimer)
        this.activeUnit = null
    }

    highlightCellsForMove(cellsForMove: BattleMapCell[]): void {
        cellsForMove.forEach(cell => this.setCellDefaultColor(cell.r, cell.c, this.movableCellsColor))
        this.battleMapController.onCellMouseEnter = (cell) => {
            if (cellsForMove.indexOf(cell) >= 0) {
                this.setCellCustomColor(cell.r, cell.c, this.movableCellsHoverColor)
                this.setCursorForCell(cell.r, cell.c, 'pointer')
            }
        }
        this.battleMapController.onCellMouseLeave = (cell) => {
            if (cellsForMove.indexOf(cell) >= 0) {
                this.setCellCustomColor(cell.r, cell.c, undefined)
                this.setCursorForCell(cell.r, cell.c, undefined)
            }
        }
    }
    restoreHighlightingForCells(cellsForMove: BattleMapCell[]): void {
        this.battleMapController.onCellMouseEnter = null
        this.battleMapController.onCellMouseLeave = null

        cellsForMove.forEach(cell => {
            this.restoreCellDefaultColor(cell.r, cell.c)
            this.setCellCustomColor(cell.r, cell.c)
        })
    }
    highlightAttackTargets(attackTargets: IAttackTarget[]): void {
        this.battleMapController.onUnitMouseEnter = (unit) => {
            const target = attackTargets.find(x => x.target === unit)
            if (target) {
                target.cells.forEach(x => this.setCellCustomColor(x.r, x.c, this.cellAttackFromHighlightColor))
                this.setCursorForUnit(unit, 'pointer')

                this.battleMapController.onUnitMouseLeave = (unitMouseLeft) => {
                    if (unitMouseLeft === unit) {
                        this.setCursorForUnit(unit, undefined)
                        this.battleMapController.onUnitMouseLeave = null
                        this.battleMapController.onUnitMouseMove = null

                        target.cells.forEach(x => this.setCellCustomColor(x.r, x.c, undefined))
                    }
                }
                let superHighlightedCell: BattleMapCell | null = null
                this.battleMapController.onUnitMouseMove = (unitMouseMove, event) => {
                    if (unitMouseMove === unit) {
                        const mouseCoordinates = {x: event.x, y: event.y}
                        const closestHexagon = this.battleMapController.getClosestCellToPoint(target.cells, mouseCoordinates)
                        if (closestHexagon !== superHighlightedCell) {
                            if (superHighlightedCell != null) {
                                const restoringColor = target.cells.indexOf(superHighlightedCell) >= 0 ? this.cellAttackFromHighlightColor : undefined
                                this.setCellCustomColor(superHighlightedCell.r, superHighlightedCell.c, restoringColor)
                            }
                            if (!closestHexagon.isObstacle) {
                                superHighlightedCell = closestHexagon
                                this.setCellCustomColor(superHighlightedCell.r, superHighlightedCell.c, this.cellsAttackFromHoverColor)
                            }
                        }
                    }
                }
            }
        }
    }
    restoreHighlightingForAttackTargets(attackTargets: IAttackTarget[]): void {
        this.battleMapController.onUnitMouseEnter = null
        this.battleMapController.onUnitMouseLeave = null
        this.battleMapController.onUnitMouseMove = null

        attackTargets.forEach((target) => target.cells.forEach(x => this.setCellCustomColor(x.r, x.c, undefined)))
    }

    async setActiveUnit(unit: BattleMapUnit | null): Promise<void> {
        if (this.activeUnit != null) {
            await this.deactivateUnit()
        }

        this.activeUnit = unit
    }

    private async deactivateUnit(): Promise<void> {
        if (this.activeUnit == null) throw new Error("There is no unit to deactivate")
        if (this.isHighlighted) {
            const unit = this.battleMapController.getUnitTile(this.activeUnit)
            await this.restoreHighlightUnitTile(unit)
        }
        this.activeUnit = null
    }

    setCellCustomColor(row: number, col: number, color?: number): void {
        const cell = this.battleMapController.getHexagon(row, col)
        this.setFillColor(cell, color ?? cell.customFillColor ?? this.defaultCellsFillColor)
    }

    setCellDefaultColor(row: number, col: number, color: number): void {
        const cell = this.battleMapController.getHexagon(row, col)
        cell.customFillColor = color
        this.setFillColor(cell, color)
    }

    restoreCellDefaultColor(row: number, col: number) {
        const cell = this.battleMapController.getHexagon(row, col)
        cell.customFillColor = undefined
        this.setFillColor(cell, this.defaultCellsFillColor)
    }

    private setCursorForCell(row: number, col: number, cursor?: string): void {
        const hexagon = this.battleMapController.getHexagon(row, col)
        this.canvasService.setCursorForHexagon(hexagon, cursor)
    }

    private setCursorForUnit(unit: BattleMapUnit, cursor?: string): void {
        const unitTile = this.battleMapController.getUnitTile(unit)
        this.canvasService.setCursorForUnit(unitTile, cursor)
    }

    private setFillColor(cell: IHexagon, color: number) {
        this.canvasService.changeHexagon(cell, {...cell, fillColor: color})
    }

    private async highlightActiveUnit(): Promise<void> {
        if (this.activeUnit === null) {
            return
        }

        try {
            const unit = this.battleMapController.getUnitTile(this.activeUnit)
            if (this.isHighlighted) {
                await this.restoreHighlightUnitTile(unit)
            } else {
                await this.highlightUnitTile(unit)
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
}