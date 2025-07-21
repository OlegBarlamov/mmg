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
    readonly slotIndex: number
}

export interface IPlayerInfo {
    readonly id: string
    readonly day: number
    readonly name: string
    readonly isDefeated: boolean
    readonly battlesGenerationInProgress: boolean
    readonly armyContainerId: string
    readonly supplyContainerId: string
}

export interface IUnitsContainerInfo {
    readonly id: string
    readonly name: string
    readonly units: IUserUnit[]
    readonly capacity: number
}

export interface IServerAPI {
    login(): Promise<void>
    getUserInfo(): Promise<IUserInfo>
    getPlayer(id: string): Promise<IPlayerInfo>
    getPlayers(): Promise<IPlayerInfo[]>
    setActivePlayer(playerId: string): Promise<void>

    getUnitsContainer(containerId: string): Promise<IUnitsContainerInfo>

    getBattles(): Promise<IBattleDefinition[]>
    beginBattle(battleDefinitionId: string): Promise<BattleMap>
    getActiveBattle(): Promise<BattleMap | null>
    establishBattleConnection(battleId: string, handler: IBattleConnectionMessagesHandler): Promise<IBattleServerConnection> 

    getMyRewards(): Promise<IRewardToAccept[]>
    acceptReward(id: string, body: AcceptRewardBody): Promise<void>
    
    moveUnits(unitId: string, containerId: string, count: number, slotIndex: number): Promise<IUnitsContainerInfo>
}