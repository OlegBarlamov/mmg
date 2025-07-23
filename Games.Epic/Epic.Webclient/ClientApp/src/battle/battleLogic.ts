import {BattleMapUnit} from "../battleMap/battleMapUnit";
import {BattleMap, BattleMapCell} from "../battleMap/battleMap";
import {IAttackTarget} from "./attackTarget";

export function getUnit(map: BattleMap, row: number, col: number): BattleMapUnit | null {
    return map.units.find(x => x.position.r === row && x.position.c === col) ?? null
}

export function getUnitById(map: BattleMap, id: string): BattleMapUnit | null {
    return map.units.find(x => x.id === id) ?? null
}

export function getAttackTargets(map: BattleMap, unit: BattleMapUnit, reachableCells: BattleMapCell[]): IAttackTarget[] {
    const possibleTargets = map.units.filter(x => x.player !== unit.player)
    const result: IAttackTarget[] = []

    for (let i = 0; i < unit.currentProps.attacks.length; i++) {
        const attackType = unit.currentProps.attacks[i]
        if (attackType.enemyInRangeDisablesAttack > 0) {
            const enemyInRange = map.units.some(x => 
                x.player !== unit.player &&
                x.isAlive &&
                map.grid.getDistance(unit.position, x.position) <= attackType.enemyInRangeDisablesAttack)
            if (enemyInRange) {
                continue
            }
        }

        for (const target of possibleTargets) {
            const cellsToAttackFrom: BattleMapCell[] = []

            const potentialCellsToAttackFrom = attackType.stayOnly ? [unit.position] : reachableCells
            for (const cell of potentialCellsToAttackFrom) {
                const targetPosition = map.grid.getCell(target.position.r, target.position.c)
                const distance = map.grid.getDistance(targetPosition, cell)
                if (distance <= attackType.attackMaxRange && distance >= attackType.attackMinRange) {
                    cellsToAttackFrom.push(cell)
                }
            }

            if (cellsToAttackFrom.length > 0) {
                result.push({
                    target,
                    actor: unit,
                    cells: cellsToAttackFrom,
                    attackType: { ...attackType, index: i },
                })
            }
        }
    }

    return result
}

export function getCellsForUnitMove(map: BattleMap, unit: BattleMapUnit): BattleMapCell[] {
    const start = unit.position
    const moveRange = unit.currentProps.speed
    const availableCells: BattleMapCell[] = []

    // Use a set to track visited cells
    const visited: Set<string> = new Set()

    // BFS queue for cells to explore; each entry is [cell, distance]
    const queue: [BattleMapCell, number][] = [[start, 0]]

    // Convert a cell to a unique string representation
    const cellToString = (cell: BattleMapCell) => `${cell.r},${cell.c}`

    const isCellBlocked = (cell: BattleMapCell) => {
        return getUnit(map, cell.r, cell.c)
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
        const neighbors = map.grid.getNeighborCells(currentCell.r, currentCell.c)
        for (const neighbor of neighbors) {
            if (!visited.has(cellToString(neighbor))) {
                queue.push([neighbor, distance + 1]);
            }
        }
    }

    return availableCells;
}