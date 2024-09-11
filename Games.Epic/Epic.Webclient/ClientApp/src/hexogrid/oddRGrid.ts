import { Point } from "../common/Point";
import {HexoGrid, IHexoGrid, IHexoPoint} from "./hexoGrid";

const sqrt3 = Math.sqrt(3)
export class OddRGrid<T extends IHexoPoint> extends HexoGrid<T> implements IHexoGrid<T> {
    constructor(cells: T[][]) {
        super(cells)
    }
    
    getCellsInRange(row: number, col: number, range: number): T[] {
        throw new Error("Method not implemented.")
    }
    getCellCenterPoint(row: number, col: number, cellRadius: number): Point {
        const hexWidth = sqrt3 * cellRadius
        const hexHeight = 2 * cellRadius

        const x = col * hexWidth + (row % 2 !== 0 ? hexWidth / 2 : 0)  + cellRadius
        const y = row * (3 / 4) * hexHeight  + cellRadius

        return { x, y };
    }
    getNeighborCells(row: number, col: number): T[] {
        // Define neighbor directions for even-r layout
        const evenRDirections = [
            {dr: -1, dc: 0}, // top
            {dr: -1, dc: 1}, // top-right
            {dr: 0, dc: -1}, // left
            {dr: 0, dc: 1},  // right
            {dr: 1, dc: 0},  // bottom
            {dr: 1, dc: 1}   // bottom-right
        ];

        const oddRDirections = [
            {dr: -1, dc: -1}, // top-left
            {dr: -1, dc: 0},  // top
            {dr: 0, dc: -1},  // left
            {dr: 0, dc: 1},   // right
            {dr: 1, dc: -1},  // bottom-left
            {dr: 1, dc: 0}    // bottom
        ];

        // Use different direction sets based on whether the row is even or odd
        const directions = row % 2 === 0 ? evenRDirections : oddRDirections

        const neighbors: T[] = []

        for (const dir of directions) {
            const neighbor = {
                r: row + dir.dr,
                c: col + dir.dc
            };

            // Check if the neighbor is within bounds
            if (neighbor.r >= 0 && neighbor.r < this.height && neighbor.c >= 0 && neighbor.c < this.width) {
                neighbors.push(this.getCell(neighbor.r, neighbor.c))
            }
        }

        return neighbors
    }
}