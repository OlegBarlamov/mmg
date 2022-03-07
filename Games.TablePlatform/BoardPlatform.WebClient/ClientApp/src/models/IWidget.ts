export interface IWidget extends IPositioned, ISizable, IMovable {
    id: number
}

export interface IPositioned {
    x: number,
    y: number,
}

export interface IMovable {
    moveRelative(xDiff: number, yDiff: number): void
}

export interface ISizable {
    width: number,
    height: number,
}