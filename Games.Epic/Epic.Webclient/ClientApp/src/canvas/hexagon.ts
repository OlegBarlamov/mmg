export interface IHexagonProps {
    readonly x: number
    readonly y: number
    readonly radius: number
    readonly strokeColor: number
    readonly strokeLine: number
    readonly fillColor: number
    readonly fillAlpha: number
}

export interface IHexagon extends IHexagonProps {
    customFillColor: number | undefined
    
    onMouseEnters: (sender: IHexagon, event: PointerEvent) => void
    onMouseLeaves: (sender: IHexagon, event: PointerEvent) => void
    onMouseDown: (sender: IHexagon, event: PointerEvent) => void
    onMouseUp: (sender: IHexagon, event: PointerEvent) => void
}