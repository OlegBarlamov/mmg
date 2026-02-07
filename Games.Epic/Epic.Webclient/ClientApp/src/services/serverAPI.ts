import {IBattleDefinition} from "../battle/IBattleDefinition";
import {BattleMap} from "../battleMap/battleMap";
import { AcceptRewardBody } from "../rewards/AcceptRewardBody";
import { IPriceResource, IRewardToAccept } from "../rewards/IRewardToAccept";
import {IBattleConnectionMessagesHandler, IBattleServerConnection} from "../server/battleServerConnection";
import { UnitProperties } from "../units/unitProperties";

export interface IUserInfo {
    readonly userId: string
    readonly userName: string
    readonly playerId: string | null
}

export interface IUserUnit {
    readonly id: string
    readonly typeId: string
    readonly count: number
    readonly name: string
    readonly thumbnailUrl: string
    readonly slotIndex: number
}

export interface IHeroStats {
    readonly attack: number
    readonly defense: number
}

export interface IPlayerInfo {
    readonly id: string
    readonly day: number
    readonly stage: number
    readonly name: string
    readonly isDefeated: boolean
    readonly battlesGenerationInProgress: boolean
    readonly armyContainerId: string
    readonly supplyContainerId: string
    readonly experience: number
    readonly level: number
    readonly stats: IHeroStats
}

export interface IUnitsContainerInfo {
    readonly id: string
    readonly name: string
    readonly units: IUserUnit[]
    readonly capacity: number
}

export interface IResourceInfo {
    readonly id: string
    readonly name: string
    readonly iconUrl: string
    readonly amount: number
    readonly price: number
}

export enum ArtifactSlot {
    None = 0,
    Bag = 1,
    Hand = 2,
    Body = 3,
    Head = 4,
    Cloak = 5,
    Legs = 6,
    Neck = 7,
    Shield = 8,
    Wrist = 9,
}

export interface IArtifactInfo {
    readonly id: string
    readonly typeId: string
    readonly typeKey: string | null
    readonly typeName: string | null
    readonly thumbnailUrl: string | null
    readonly slots: ArtifactSlot[]
    readonly attackBonus: number
    readonly defenseBonus: number
    readonly equippedSlotsIndexes: number[]
    readonly buffNames: string[]
}

export interface IAcceptedReward {
    readonly rewardId: string
}

export interface IReportInfo {
    readonly id: string
    readonly isWinner: boolean
    playerUnits: IReportUnit[]
    enemyUnits: IReportUnit[]
}

export interface IReportUnit { 
    readonly thumbnailUrl: string
    readonly startCount: number
    readonly finalCount: number
    readonly name: string
}

export interface IUnitTypeInfo extends UnitProperties {
    readonly id: string
    readonly name: string
    readonly dashboardImgUrl: string
    readonly upgradeForUnitTypeIds: string[]
    readonly price: IPriceResource
}

export interface IRansomPrice {
    gold: number
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
    beginBattleWithPlayer(playerName: string): Promise<BattleMap>
    getActiveBattle(): Promise<BattleMap | null>
    establishBattleConnection(battleId: string, handler: IBattleConnectionMessagesHandler): Promise<IBattleServerConnection> 
    getRansomPrice(battleId: string): Promise<IRansomPrice>

    getMyArtifacts(): Promise<IArtifactInfo[]>
    equipArtifact(artifactId: string, equippedSlotsIndexes: number[]): Promise<IArtifactInfo>

    getMyRewards(): Promise<IRewardToAccept[]>
    acceptReward(id: string, body: AcceptRewardBody): Promise<IAcceptedReward>
    beginGuardBattle(rewardId: string): Promise<BattleMap>
    
    moveUnits(unitId: string, containerId: string, count: number, slotIndex: number): Promise<IUnitsContainerInfo>

    getResources(): Promise<IResourceInfo[]>

    getReport(reportId: string): Promise<IReportInfo>

    getUnitTypeInfo(unitTypeId: string): Promise<IUnitTypeInfo>
    getUnitTypesInfos(unitTypeIds: string[]): Promise<IUnitTypeInfo[]>

    getBattleUnitInfo(battleId: string, unitId: string): Promise<IBattleUnitInfo>
}

// Battle unit info fetched from server with computed buff effects
export interface IBattleUnitInfo {
    id: string
    name: string
    count: number
    isAlive: boolean
    position: { r: number, c: number }
    player: string
    props: IBattleUnitProps
    currentProps: IBattleUnitProps
    buffs: IBattleUnitBuff[]
}

export interface IBattleUnitProps {
    battleImgUrl: string
    speed: number
    health: number
    attack: number
    defense: number
    movementType: string
    attacks: IBattleUnitAttack[]
    waited: boolean
    buffs: IBattleUnitBuff[]
}

export interface IBattleUnitAttack {
    name: string
    minDamage: number
    maxDamage: number
    attackMinRange: number
    attackMaxRange: number
}

export interface IBattleUnitBuff {
    id: string
    buffTypeId: string
    thumbnailUrl?: string
    name?: string
    permanent?: boolean
    durationRemaining: number
}