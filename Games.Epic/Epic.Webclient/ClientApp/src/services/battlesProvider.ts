import {IBattleDefinition} from "../battle/IBattleDefinition";
import {IServerAPI} from "./serverAPI";

export interface IBattlesProvider {
    fetchBattles() : Promise<IBattleDefinition[]>
}

export class ServerBattlesProvider implements IBattlesProvider {
    constructor(private readonly serverAPI: IServerAPI) {
    }

    fetchBattles(): Promise<IBattleDefinition[]> {
        return this.serverAPI.getBattles()
    }
}