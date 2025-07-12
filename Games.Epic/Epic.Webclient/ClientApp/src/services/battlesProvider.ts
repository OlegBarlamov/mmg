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

export class FakeBattlesProvider implements IBattlesProvider {
    fetchBattles(): Promise<IBattleDefinition[]> {
        return Promise.resolve(
            [
                {
                    width: 11,
                    height: 9,
                    id: "1",
                },
                {
                    width: 6,
                    height: 5,
                    id: "2",
                },
            ])
    }
    
}