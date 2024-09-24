import {IBattleDefinition} from "../battle/battleDefinition";
import {BattleMap} from "../battleMap/battleMap";
import {IBattleConnectionMessagesHandler, IBattleServerConnection} from "../server/battleServerConnection";

export interface IUserInfo {
    userId: string
    userName: string
}

export interface IUserUnit {
    id: string
    typeId: string
    count: number
    thumbnailUrl: string
}

export interface IServerAPI {
    signup(userName: string): Promise<IUserInfo>
    getUserInfo(): Promise<IUserInfo>
    
    getUnits(): Promise<IUserUnit[]>
    
    getBattles(): Promise<IBattleDefinition[]>
    beginBattle(battleId: string): Promise<BattleMap>
    getActiveBattle(): Promise<BattleMap | null>
    
    establishBattleConnection(handler: IBattleConnectionMessagesHandler): Promise<IBattleServerConnection> 
}