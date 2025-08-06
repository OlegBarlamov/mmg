import { Point } from "../common/Point";
import { Size } from "../common/Size";
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

        const x = col * hexWidth + (row % 2 !== 0 ? hexWidth / 2 : 0) + cellRadius
        const y = row * (3 / 4) * hexHeight + cellRadius

        return { x, y };
    }
    getNeighborCells(row: number, col: number): T[] {
        // Neighbor directions for odd-r layout
        const oddRDirections = [
            {dr: -1, dc: 0},  // top
            {dr: -1, dc: -1}, // top-left
            {dr: 0, dc: -1},  // left
            {dr: 0, dc: 1},   // right
            {dr: 1, dc: 0},   // bottom
            {dr: 1, dc: -1}   // bottom-left
        ]

        const evenRDirections = [
            {dr: -1, dc: 0},  // top
            {dr: -1, dc: 1}, // top-right
            {dr: 0, dc: -1},  // left
            {dr: 0, dc: 1},   // right
            {dr: 1, dc: 0},   // bottom
            {dr: 1, dc: 1}   // bottom-right
        ]

        // Use different direction sets based on whether the row is even or odd
        const directions = row % 2 === 0 ? oddRDirections : evenRDirections

        const neighbors: T[] = []

        for (const dir of directions) {
            const neighbor = {
                r: row + dir.dr,
                c: col + dir.dc
            }

            // Check if the neighbor is within bounds
            if (this.isValidCell(neighbor.r, neighbor.c)) {
                neighbors.push(this.getCell(neighbor.r, neighbor.c))
            }
        }

        return neighbors
    }

    getSize(cellRadius: number): Size {
        const hexWidth = sqrt3 * cellRadius; // Width of a single hexagon (pointy-topped)
        const hexHeight = 2 * cellRadius; // Height of a single hexagon (pointy-topped)

        // Total grid width for odd-r grid:
        const gridWidth = hexWidth * this.width + hexWidth / 2; // Accounts for staggered odd columns

        // Total grid height
        const gridHeight = (this.height - 1) * (3 / 4) * hexHeight + hexHeight;

        return { width: gridWidth, height: gridHeight };
    }
    
    protected toAxial(row: number, col: number): { q: number, r: number } {
        const q = col - Math.floor((row - (row & 1)) / 2);
        const r = row;
        return { q, r };
    }
}