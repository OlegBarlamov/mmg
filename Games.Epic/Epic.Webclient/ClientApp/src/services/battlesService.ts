import {BattleMap} from "../battleMap/battleMap";
import {BattleController, IBattleController} from "../battle/battleController";
import {IBattleMapsService} from "./battleMapsService";
import {IServerAPI} from "./serverAPI";
import {BattleActionsProcessor} from "../battle/battleActionsProcessor";
import {BattleServerMessagesHandler} from "../battle/battleServerMessagesHandler";

export interface IBattlesService {
    createBattle(map: BattleMap): Promise<IBattleController>
}

export class BattlesService implements IBattlesService {
    constructor(private readonly battleMapService: IBattleMapsService,
                private readonly serverAPI: IServerAPI) {
    }
    async createBattle(map: BattleMap): Promise<IBattleController> {
        const mapController = await this.battleMapService.load(map)
        const serverMessagesHandler = new BattleServerMessagesHandler(mapController, map.turn)
        const serverConnection = await this.serverAPI.establishBattleConnection(serverMessagesHandler)
        const battleActionsProcessor = new BattleActionsProcessor(mapController, serverConnection)
        
        return new BattleController(mapController, battleActionsProcessor, serverMessagesHandler)
    }
}