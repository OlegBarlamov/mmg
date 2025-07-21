import {IBattleDefinition} from "../battle/IBattleDefinition";
import {BattleMap} from "../battleMap/battleMap";
import { AcceptRewardBody } from "../rewards/AcceptRewardBody";
import { IRewardToAccept } from "../rewards/IRewardToAccept";
import {IBattleConnectionMessagesHandler, IBattleServerConnection} from "../server/battleServerConnection";

export interface IUserInfo {
    readonly userId: string
    readonly userName: string
    readonly playerId: string | null
}

export interface IUserUnit {
    readonly id: string
    readonly typeId: string
    readonly count: number
    readonly thumbnailUrl: string
}

export interface IPlayerInfo {
    readonly id: string
    readonly day: number
    readonly name: string
    readonly isDefeated: boolean
    readonly battlesGenerationInProgress: boolean
    readonly armyCapacity: number
    readonly supplyCapacity: number
}

export interface IServerAPI {
    login(): Promise<void>
    getUserInfo(): Promise<IUserInfo>
    getPlayer(id: string): Promise<IPlayerInfo>
    getPlayers(): Promise<IPlayerInfo[]>
    getArmyUnits(): Promise<IUserUnit[]>
    getSupplyUnits(): Promise<IUserUnit[]>
    setActivePlayer(playerId: string): Promise<void>
    getBattles(): Promise<IBattleDefinition[]>
    beginBattle(battleDefinitionId: string): Promise<BattleMap>
    getActiveBattle(): Promise<BattleMap | null>

    getMyRewards(): Promise<IRewardToAccept[]>
    acceptReward(id: string, body: AcceptRewardBody): Promise<void>
    
    establishBattleConnection(battleId: string, handler: IBattleConnectionMessagesHandler): Promise<IBattleServerConnection> 
}