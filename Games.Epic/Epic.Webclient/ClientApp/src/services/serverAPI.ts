import {IBattleDefinition} from "../battle/IBattleDefinition";
import {BattleMap} from "../battleMap/battleMap";
import { AcceptRewardBody } from "../rewards/AcceptRewardBody";
import { IRewardToAccept } from "../rewards/IRewardToAccept";
import {IBattleConnectionMessagesHandler, IBattleServerConnection} from "../server/battleServerConnection";

export interface IUserInfo {
    readonly userId: string
    readonly userName: string
}

export interface IUserUnit {
    readonly id: string
    readonly typeId: string
    readonly count: number
    readonly thumbnailUrl: string
}

export interface IServerAPI {
    login(): Promise<void>
    getUserInfo(): Promise<IUserInfo>
    
    getUnits(): Promise<IUserUnit[]>
    
    getBattles(): Promise<IBattleDefinition[]>
    beginBattle(battleDefinitionId: string): Promise<BattleMap>
    getActiveBattle(): Promise<BattleMap | null>

    getMyRewards(): Promise<IRewardToAccept[]>
    acceptReward(id: string, body: AcceptRewardBody): Promise<void>
    
    establishBattleConnection(battleId: string, handler: IBattleConnectionMessagesHandler): Promise<IBattleServerConnection> 
}