import {BattleMap} from "../battleMap/battleMap";
import {IBattleDefinition} from "../battle/battleDefinition";
import {IBattleMapsService} from "./battleMapsService";
import {IServerAPI} from "./serverAPI";
import {OddRGrid} from "../hexogrid/oddRGrid";
import {PlayerNumber} from "../player/playerNumber";

export interface IBattleLoader {
    loadBattle(battleDefinition: IBattleDefinition): Promise<BattleMap>
}

export class ServerBattleLoader implements IBattleLoader {
    constructor(private readonly serverAPI: IServerAPI) {
    }
    
    async loadBattle(battleDefinition: IBattleDefinition): Promise<BattleMap> {
        const battle = await this.serverAPI.beginBattle(battleDefinition.battleId)
        return {
            grid: new OddRGrid(battle.grid.cells),
            units: battle.units,
            turn: battle.turn,
        }
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
