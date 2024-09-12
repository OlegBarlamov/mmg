import {Point} from "./Point";

export function distance(point1: Point, point2: Point): number {
    return Math.sqrt(distanceSqr(point1, point2))
}

export function distanceSqr(point1: Point, point2: Point): number {
    const dx = point2.x - point1.x
    const dy = point2.y - point1.y
    return dx * dx + dy * dy
}