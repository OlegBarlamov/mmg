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
    
    abstract getCellsInRange(row: number, col: number, range: number): T[]
    
    abstract getNeighborCells(row: number, col: number): T[]

    abstract getCellCenterPoint(row: number, col: number, cellRadius: number): Point
}
