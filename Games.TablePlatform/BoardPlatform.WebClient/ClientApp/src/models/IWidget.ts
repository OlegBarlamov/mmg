export interface IWidget extends IPositioned, ISizable {
    id: number
}

export interface IPositioned {
    x: number,
    y: number,
}

export interface ISizable {
    width: number,
    height: number,
}