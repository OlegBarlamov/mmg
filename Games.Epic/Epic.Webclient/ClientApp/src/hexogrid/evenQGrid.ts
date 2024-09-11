import {Point} from "../common/Point";
import {HexoGrid, IHexoGrid, IHexoPoint} from "./hexoGrid";

const sqrt3 = Math.sqrt(3)
export class EvenQGrid<T extends IHexoPoint> extends HexoGrid<T> implements IHexoGrid<T> {
    constructor(cells: T[][]) {
        super(cells)
    }
    getNeighborCells(row: number, col: number): T[] {
        // Define neighbor directions for even-q layout
        const evenDirections = [
            { dr: -1, dc: 0 },  // top
            { dr: 0, dc: -1 },  // left
            { dr: 0, dc: 1 },   // right
            { dr: 1, dc: -1 },  // bottom-left
            { dr: 1, dc: 0 },   // bottom
            { dr: 1, dc: 1 }    // bottom-right
        ]

        const oddDirections = [
            { dr: -1, dc: -1 }, // top-left
            { dr: -1, dc: 0 },  // top
            { dr: -1, dc: 1 },  // top-right
            { dr: 0, dc: 1 },   // right
            { dr: 1, dc: 0 },   // bottom
            { dr: 0, dc: -1 }   // left
        ]

        // Choose directions based on whether the row is even or odd
        const directions = col % 2 === 0 ? evenDirections : oddDirections;

        const neighbors: T[] = []

        for (const dir of directions) {
            const neighbor = {
                r: row + dir.dr,
                c: col + dir.dc
            }
            if (this.isValidCell(neighbor.r, neighbor.c)) {
                neighbors.push(this.getCell(neighbor.r, neighbor.c))
            }
        }
        return neighbors
    }

    getCellCenterPoint(row: number, col: number, cellRadius: number): Point {
        const hexWidth = 2 * cellRadius
        const hexHeight = sqrt3 * cellRadius
        const x = col * hexWidth * 0.75 + cellRadius
        const y = row * hexHeight + (col % 2 === 0 ? hexHeight / 2 : 0) + cellRadius
        return {x, y}
    }

    getCellsInRange(row: number, col: number, range: number): T[] {
        const centerCube = this.offsetToCube(row, col)
        const cellsInRadius: T[] = []

        // Loop over the cube coordinates in the radius range
        for (let dq = -range; dq <= range; dq++) {
            for (let dr = Math.max(-range, -dq - range); dr <= Math.min(range, -dq + range); dr++) {
                const ds = -dq - dr

                const cube = {
                    q: centerCube.q + dq,
                    r: centerCube.r + dr,
                    s: centerCube.s + ds,
                };

                // Convert the cube coordinate back to offset coordinates
                const offset = this.cubeToOffset(cube.q, cube.r)

                // Check if the cell exists in the grid (bounds checking)
                if (this.isValidCell(offset.row, offset.col)) {
                    cellsInRadius.push(this.getCell(offset.row, offset.col))
                }
            }
        }

        return cellsInRadius
    }

    private offsetToCube(row: number, col: number): { q: number, r: number, s: number } {
        const q = col
        const r = row - (col - (col & 1)) / 2
        const s = -q - r
        return {q, r, s}
    }

    private cubeToOffset(q: number, r: number): { row: number, col: number } {
        const col = q
        const row = r + (q - (q & 1)) / 2
        return {row, col}
    }
}