import {BattleMap} from "../battleMap/battleMap";
import {BattleController, IBattleController} from "../battle/battleController";
import {IBattleMapsService} from "./battleMapsService";

export interface IBattlesService {
    createBattle(map: BattleMap): Promise<IBattleController>
}

export class BattlesService implements IBattlesService {
    constructor(private readonly battleMapService: IBattleMapsService) {
    }
    async createBattle(map: BattleMap): Promise<IBattleController> {
        const mapController = await this.battleMapService.load(map)
        return new BattleController(mapController)
    }
}