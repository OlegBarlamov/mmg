import {IBattleMapController} from "../battleMap/battleMapController";
import {BattleMapUnit} from "../battleMap/battleMapUnit";
import {BattleMap, BattleMapCell} from "../battleMap/battleMap";
import {BattleUserAction} from "./battleUserAction";
import {wait} from "../common/wait";
import {IAttackTarget} from "./attackTarget";
import {distance} from "../common/math";
import {Point} from "../common/Point";

export interface IBattleController {
    startBattle(): Promise<void>
}

export class BattleController implements IBattleController {
    mapController: IBattleMapController

    private readonly map: BattleMap
    private readonly orderedUnits: BattleMapUnit[]
    
    private battleStarted: boolean = false
    private battleFinished: boolean = false
    private currentStepUnitIndex: number = -1
    
    constructor(mapController: IBattleMapController) {
        this.mapController = mapController

        this.orderedUnits = [...this.mapController.map.units]
            .sort((a, b) => b.props.speed - a.props.speed)
        this.map = mapController.map
    }

    async startBattle(): Promise<void> {
        if (this.battleStarted) throw new Error("The battle already started");
        this.battleStarted = true
        this.currentStepUnitIndex = -1
        
        while (!this.battleFinished) {
            this.currentStepUnitIndex++
            if (this.currentStepUnitIndex >= this.orderedUnits.length) {
                this.currentStepUnitIndex = 0
            }
            
            const currentUnit = this.orderedUnits[this.currentStepUnitIndex]
            await this.processStep(currentUnit)
        }
    }

    private async processStep(unit: BattleMapUnit): Promise<void> {
        await this.mapController.activateUnit(unit)
        
        const cellsForMove = this.getCellsForUnitMove(unit)
        const reachableCells = [this.map.grid.getCell(unit.position.r, unit.position.c), ...cellsForMove]
        const movableCellsColor = 0x6dbfac
        cellsForMove.forEach(cell => this.mapController.setCellDefaultColor(cell.r, cell.c, movableCellsColor))

        const attackTargets = this.getAttackTargets(unit, reachableCells)
        
        this.mapController.onCellMouseEnter = (cell) => {
            if (cellsForMove.indexOf(cell) >= 0) {
                this.mapController.setCellCustomColor(cell.r, cell.c, 0x32a852)
                this.mapController.setCursorForCell(cell.r, cell.c, 'pointer')
            }
        }
        this.mapController.onCellMouseLeave = (cell) => {
            if (cellsForMove.indexOf(cell) >= 0) {
                this.mapController.setCellCustomColor(cell.r, cell.c, undefined)
                this.mapController.setCursorForCell(cell.r, cell.c, undefined)
            }
        }
        this.mapController.onUnitMouseEnter = (unit) => {
            const target = attackTargets.find(x => x.target === unit)
            if (target) {
                target.cells.forEach(x => this.mapController.setCellDefaultColor(x.r, x.c, 0xffa08f))
                this.mapController.setCursorForUnit(unit, 'pointer')
                
                this.mapController.onUnitMouseLeave = (unitMouseLeft) => {
                    if (unitMouseLeft === unit) {
                        this.mapController.setCursorForUnit(unit, undefined)
                        this.mapController.onUnitMouseLeave = null
                        this.mapController.onUnitMouseMove = null
                        target.cells.forEach(x => this.mapController.setCellDefaultColor(x.r, x.c, movableCellsColor))
                    }
                }
                let superHighlightedCell: BattleMapCell | null = null
                this.mapController.onUnitMouseMove = (unitMouseMove, event) => {
                    if (unitMouseMove === unit) {
                        const mouseCoordinates = {x: event.x - this.mapController.visualOffset.x, y: event.y - this.mapController.visualOffset.y}

                        const closestHexagon = this.getClosestCellToPoint(target.cells, mouseCoordinates)
                        if (closestHexagon !== superHighlightedCell) {
                            if (superHighlightedCell != null) {
                                this.mapController.setCellCustomColor(superHighlightedCell.r, superHighlightedCell.c) 
                            }
                            superHighlightedCell = closestHexagon
                            this.mapController.setCellCustomColor(superHighlightedCell.r, superHighlightedCell.c, 0xfa3b19)
                        }
                    }
                }
            }
        }
        
        let action: BattleUserAction
        try {
            action = await this.getUserInputAction(unit, cellsForMove, attackTargets)
        } finally {
            this.mapController.onCellMouseEnter = null
            this.mapController.onCellMouseLeave = null
            this.mapController.onUnitMouseEnter = null
            this.mapController.onUnitMouseMove = null
            this.mapController.onUnitMouseMove = null

            cellsForMove.forEach(cell => {
                this.mapController.restoreCellDefaultColor(cell.r, cell.c)
                this.mapController.setCellCustomColor(cell.r, cell.c)
            })

            await this.mapController.deactivateUnit()
        }
        
        await this.processInputAction(action)
        
        await wait(500)
        
        debugger
    }
    
    private async processInputAction(action: BattleUserAction): Promise<void> {
        if (action.command === 'UNIT_MOVE') {
            const cell = action.moveToCell
            await this.mapController.moveUnit(action.actor, cell.r, cell.c)
        } else if (action.command === 'UNIT_ATTACK') {
            const cell = action.moveToCell
            await this.mapController.moveUnit(action.actor, cell.r, cell.c)
            await this.processAttack(action.actor, action.attackTarget)
        } else {
            throw new Error("Unknown type of user action")
        }
    }
    
    private async processAttack(actor: BattleMapUnit, target: BattleMapUnit): Promise<void> {
        const damage = actor.props.damage * actor.unitsCount
        const eliminated = await this.mapController.unitTakeDamage(target, damage)
        if (eliminated) {
            this.orderedUnits.splice(this.orderedUnits.indexOf(target), 1)
        }
    }
    
    private getUserInputAction(originalUnit: BattleMapUnit, cellsToMove: BattleMapCell[], attackTargets: IAttackTarget[]): Promise<BattleUserAction> {
        return new Promise((resolve) => {
            this.mapController.onCellMouseUp = (cell) => {
                if (cellsToMove.indexOf(cell) >= 0) {
                    this.mapController.onCellMouseUp = null
                    resolve({
                        command: 'UNIT_MOVE',
                        player: originalUnit.player,
                        actor: originalUnit,
                        moveToCell: cell,
                    })
                }
            }
            this.mapController.onUnitMouseUp = (unit, event) => {
                const target = attackTargets.find(x => x.target === unit)
                if (target) {
                    this.mapController.onUnitMouseUp = null
                    const mouseCoordinates = {x: event.x - this.mapController.visualOffset.x, y: event.y - this.mapController.visualOffset.y}
                    const closestCell = this.getClosestCellToPoint(target.cells, mouseCoordinates)
                    resolve({
                        command: 'UNIT_ATTACK',
                        player: originalUnit.player,
                        actor: originalUnit,
                        moveToCell: closestCell,
                        attackTarget: target.target,
                    })
                }
            }
        })
    }
    
    private getClosestCellToPoint(cells: BattleMapCell[], point: Point): BattleMapCell {
        const hexagonCenters = cells.map(cell => {
            return {
                cell,
                centerPoint: this.map.grid.getCellCenterPoint(cell.r, cell.c, this.mapController.cellRadius)
            }
        })

        let closestHexagon = hexagonCenters[0]
        let closestHexagonDistanceSqr = Number.MAX_SAFE_INTEGER
        for (const hexagonPair of hexagonCenters) {
            const distanceSqr = distance(point, hexagonPair.centerPoint)
            if (distanceSqr < closestHexagonDistanceSqr) {
                closestHexagon = hexagonPair
                closestHexagonDistanceSqr = distanceSqr
            }
        }
        
        return closestHexagon.cell
    }
    
    private getAttackTargets(unit: BattleMapUnit, reachableCells: BattleMapCell[]): IAttackTarget[] {
        const possibleTargets = this.orderedUnits.filter(x => x.player !== unit.player)
        const result: IAttackTarget[] = []
        
        for (const target of possibleTargets) {
            const cellsToAttachFrom: BattleMapCell[] = []
            
            for (const cell of reachableCells) {
                const targetPosition = this.map.grid.getCell(target.position.r, target.position.c)
                const distance = this.map.grid.getDistance(targetPosition, cell)
                if (distance <= unit.props.attackMaxRange && distance >= unit.props.attackMinRange) {
                    cellsToAttachFrom.push(cell)
                }
            }
            
            if (cellsToAttachFrom.length > 0) {
                result.push({
                    target,
                    actor: unit,
                    cells: cellsToAttachFrom,
                })
            }
        }
        
        return result
    }
    
    private getCellsForUnitMove(unit: BattleMapUnit): BattleMapCell[] {
        const start = unit.position
        const moveRange = unit.props.speed
        const availableCells: BattleMapCell[] = []

        // Use a set to track visited cells
        const visited: Set<string> = new Set()

        // BFS queue for cells to explore; each entry is [cell, distance]
        const queue: [BattleMapCell, number][] = [[start, 0]]

        // Convert a cell to a unique string representation
        const cellToString = (cell: BattleMapCell) => `${cell.r},${cell.c}`
        
        const isCellBlocked = (cell: BattleMapCell) => {
            return this.mapController.getUnit(cell.r, cell.c)
        }

        // Start BFS
        while (queue.length > 0) {
            const [currentCell, distance] = queue.shift()!
            const key = cellToString(currentCell)
            
            // Skip if we've already visited this cell, or it exceeds movement range
            if (visited.has(key) || distance > moveRange) continue

            // Mark this cell as visited
            visited.add(key)

            // If it's not blocked by another unit and within bounds, add to available cells
            if (isCellBlocked(currentCell) && currentCell !== start) {
                continue
            }

            if (currentCell !== start) {
                availableCells.push(currentCell)
            }

            // Get neighboring cells and continue exploring
            const neighbors = this.mapController.map.grid.getNeighborCells(currentCell.r, currentCell.c)
            for (const neighbor of neighbors) {
                if (!visited.has(cellToString(neighbor))) {
                    queue.push([neighbor, distance + 1]);
                }
            }
        }

        return availableCells;
    }
}