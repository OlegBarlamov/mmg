import {IBattleDefinition} from "../battle/battleDefinition";

export interface IBattlesProvider {
    fetchBattles() : Promise<IBattleDefinition[]>
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