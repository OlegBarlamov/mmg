import {IBattleMapController} from "../battleMap/battleMapController";
import {BattleMapUnit} from "../battleMap/battleMapUnit";
import {BattleMap, BattleMapCell} from "../battleMap/battleMap";
import {BattleUserAction} from "./battleUserAction";
import {wait} from "../common/wait";
import {IAttackTarget} from "./attackTarget";
import {PlayerNumber} from "../player/playerNumber";

export interface IBattleController {
    startBattle(): Promise<PlayerNumber>
    dispose(): void
}

export class BattleController implements IBattleController {
    mapController: IBattleMapController

    private readonly map: BattleMap
    private readonly orderedUnits: BattleMapUnit[]
    
    private battleStarted: boolean = false
    private battleFinished: boolean = false
    private currentStepUnitIndex: number = -1
    private winnerPlayer: PlayerNumber | null = null
    
    constructor(mapController: IBattleMapController) {
        this.mapController = mapController

        this.orderedUnits = [...this.mapController.map.units]
            .sort((a, b) => b.props.speed - a.props.speed)
        this.map = mapController.map
    }

    dispose(): void {
        this.orderedUnits.splice(0)
        this.mapController.destroy()
    }

    async startBattle(): Promise<PlayerNumber> {
        if (this.battleStarted) throw new Error("The battle already started");
        this.battleStarted = true
        this.currentStepUnitIndex = 0
        
        while (!this.battleFinished) {
            if (this.currentStepUnitIndex >= this.orderedUnits.length) {
                this.currentStepUnitIndex = 0
            }

            try {
                const currentUnit = this.orderedUnits[this.currentStepUnitIndex]
                await this.processStep(currentUnit)
                this.winnerPlayer = this.getWinner()
                this.battleFinished = this.winnerPlayer != null

                this.currentStepUnitIndex++
            } catch (e) {
                console.log(e)
            }
        }
        
        await wait(1000 * 3)
        
        return this.winnerPlayer!
    }
    
    private getWinner(): PlayerNumber | null {
        const unitsCounts = new Map<PlayerNumber, number>()
        this.orderedUnits.forEach((unit) => {
            const count = unitsCounts.get(unit.player) ?? 0
            unitsCounts.set(unit.player, count + 1)
        })
        const playersWithUnits: PlayerNumber[] = []
        for (const playerKey of unitsCounts.keys()) {
            if (unitsCounts.get(playerKey) ?? 0 > 0) {
                playersWithUnits.push(playerKey)
            }
        }
        if (playersWithUnits.length === 1) {
            return playersWithUnits[0]
        }
        return null
    }

    private async processStep(unit: BattleMapUnit): Promise<void> {
        await this.mapController.battleMapHighlighter.setActiveUnit(unit)
        
        const cellsForMove = this.getCellsForUnitMove(unit)
        const reachableCells = [this.map.grid.getCell(unit.position.r, unit.position.c), ...cellsForMove]
        const attackTargets = this.getAttackTargets(unit, reachableCells)
        
        this.mapController.battleMapHighlighter.highlightCellsForMove(cellsForMove)
        this.mapController.battleMapHighlighter.highlightAttackTargets(attackTargets)
        
        let action: BattleUserAction
        try {
            action = await this.getUserInputAction(unit, cellsForMove, attackTargets)
        } finally {
            this.mapController.battleMapHighlighter.restoreHighlightingForCells(cellsForMove)
            this.mapController.battleMapHighlighter.restoreHighlightingForAttackTargets(attackTargets)
            await this.mapController.battleMapHighlighter.setActiveUnit(null)
        }
        
        await this.processInputAction(action)
        
        await wait(500)
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
            this.mapController.onCellMouseClick = (cell) => {
                if (cellsToMove.indexOf(cell) >= 0) {
                    this.mapController.onCellMouseClick = null
                    resolve({
                        command: 'UNIT_MOVE',
                        player: originalUnit.player,
                        actor: originalUnit,
                        moveToCell: cell,
                    })
                }
            }
            this.mapController.onUnitMouseClick = (unit, event) => {
                const target = attackTargets.find(x => x.target === unit)
                if (target) {
                    this.mapController.onUnitMouseClick = null
                    const mouseCoordinates = {x: event.x, y: event.y}
                    const closestCell = this.mapController.getClosestCellToPoint(target.cells, mouseCoordinates)
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