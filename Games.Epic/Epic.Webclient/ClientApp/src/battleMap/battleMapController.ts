import {IHexagon} from "../canvas/hexagon";
import {BattleMap} from "./battleMap";

export interface IBattleMapController {

}

export class BattleMapController implements IBattleMapController{
    model: BattleMap
    views: IHexagon[][]

    constructor(model: BattleMap, cellViews: IHexagon[][]) {
        this.model = model
        this.views = cellViews
    }
}