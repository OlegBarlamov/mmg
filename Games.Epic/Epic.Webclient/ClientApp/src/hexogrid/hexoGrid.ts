import {Point} from "../common/Point";
import {Size} from "../common/Size";

export interface IHexoPoint {
    readonly c: number
    readonly r: number
}

export interface IHexoGrid<T extends IHexoPoint> {
    readonly cells: T[][]
    readonly width: number
    readonly height: number

    getSize(cellRadius: number): Size
    
    getCellCenterPoint(row: number, col: number, cellRadius: number): Point
    isValidCell(row: number, col: number): boolean
    getCell(row: number, col: number): T
    getCellsInRange(row: number, col: number, range: number): T[]
    getNeighborCells(row: number, col: number): T[]
    
    getDistance(point1: T, point2: T): number
}

export abstract class HexoGrid<T extends IHexoPoint> implements IHexoGrid<T> {
    readonly cells: T[][]

    get width(): number {
        return this.cells[0]?.length ?? 0
    }
    get height(): number {
        return this.cells.length
    }
    
    protected constructor(cells: T[][]) {
        this.cells = cells
    }

    abstract getSize(cellRadius: number): Size
    
    isValidCell(row: number, col: number): boolean {
        return row >= 0 && row < this.height && col >= 0 && col < this.width;
    }
    getCell(row: number, col: number): T {
        return this.cells[row][col]
    }

    protected distanceAxial(q1: number, r1: number, q2: number, r2: number): number {
        return (Math.abs(q1 - q2) + Math.abs(r1 - r2) + Math.abs((q1 + r1) - (q2 + r2))) / 2;
    }
    
    abstract getCellsInRange(row: number, col: number, range: number): T[]
    
    abstract getNeighborCells(row: number, col: number): T[]

    abstract getCellCenterPoint(row: number, col: number, cellRadius: number): Point

    protected abstract toAxial(row: number, col: number): { q: number, r: number }
    
    getDistance(point1: T, point2: T): number {
        const { q: q1, r: r1 } = this.toAxial(point1.r, point1.c);
        const { q: q2, r: r2 } = this.toAxial(point2.r, point2.c);

        return this.distanceAxial(q1, r1, q2, r2);
    }
}
