import {BattleMap} from "../battleMap/battleMap";
import {IBattleController} from "../battle/battleController";

export interface IBattlesService {
    createBattle(map: BattleMap): Promise<IBattleController>
}