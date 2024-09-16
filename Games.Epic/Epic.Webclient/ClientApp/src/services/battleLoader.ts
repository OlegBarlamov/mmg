import {BattleMap} from "../battleMap/battleMap";
import {IBattleDefinition} from "../battle/battleDefinition";
import {IBattleMapsService} from "./battleMapsService";
import {IServerAPI} from "./serverAPI";

export interface IBattleLoader {
    loadBattle(battleDefinition: IBattleDefinition): Promise<BattleMap>
}

export class ServerBattleLoader implements IBattleLoader {
    constructor(private readonly serverAPI: IServerAPI) {
    }
    
    loadBattle(battleDefinition: IBattleDefinition): Promise<BattleMap> {
        return this.serverAPI.beginBattle(battleDefinition.battleId)
    }
    
}

export class FakeBattleLoader implements IBattleLoader {
    private battleMapsService: IBattleMapsService;
    constructor(battleMapsService: IBattleMapsService) {
        this.battleMapsService = battleMapsService;
    }
    loadBattle(battleDefinition: IBattleDefinition): Promise<BattleMap> {
        return Promise.resolve(this.battleMapsService.generateMap(battleDefinition.mapWidth, battleDefinition.mapHeight))
    }
}
