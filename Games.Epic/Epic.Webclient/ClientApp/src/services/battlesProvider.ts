import {IBattleDefinition} from "../battle/battleDefinition";
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

export class FakeBattlesProvider implements IBattlesProvider {
    fetchBattles(): Promise<IBattleDefinition[]> {
        return Promise.resolve(
            [
                {
                    mapWidth: 11,
                    mapHeight: 9,
                    battleId: "1",
                },
                {
                    mapWidth: 6,
                    mapHeight: 5,
                    battleId: "2",
                },
            ])
    }
    
}