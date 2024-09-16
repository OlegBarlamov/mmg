import {IBattleDefinition} from "../battle/battleDefinition";
import {BattleMap} from "../battleMap/battleMap";

export interface IUserInfo {
    userId: string
    userName: string
}

export interface IUsersUnit {
    id: string
    typeId: string
    count: number
    thumbnailUrl: string
}

export interface IServerAPI {
    signup(userName: string): Promise<IUserInfo>
    getUserInfo(): Promise<IUserInfo>
    
    getUnits(): Promise<IUsersUnit[]>
    
    getBattles(): Promise<IBattleDefinition[]>
    beginBattle(battleId: string): Promise<BattleMap>
    getActiveBattle(): Promise<BattleMap | null>
}