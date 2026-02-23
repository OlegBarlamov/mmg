import {IBattleDefinition, IBattleDefinitionReward, IBattleDefinitionUnit} from "../battle/IBattleDefinition";
import {BattleMap, BattleMapCell} from "../battleMap/battleMap";
import {IAcceptedReward, IArtifactInfo, IKnownMagicInfo, IPlayerInfo, IRansomPrice, IReportInfo, IResourceInfo, IServerAPI, IUnitTypeInfo, IUnitsContainerInfo, IUserInfo, IUserUnit} from "../services/serverAPI";
import {getSessionCookie, setSessionCookie} from "../units/cookiesHelper";
import {BattleMapUnit} from "../battleMap/battleMapUnit";
import {OddRGrid} from "../hexogrid/oddRGrid";
import {BattlePlayerNumber} from "../player/playerNumber";
import {MovementType, UnitProperties} from "../units/unitProperties";
import {IHexoPoint} from "../hexogrid/hexoGrid";
import {getRandomStringKey} from "../units/getRandomString";
import {
    BattleServerMessageRejectionReason,
    BattleServerMessageResponseStatus,
    IBattleConnectionMessagesHandler,
    IBattleServerConnection
} from "./battleServerConnection";
import {BattleCommandToServer} from "./battleCommandToServer";
import {getUnitById} from "../battle/battleLogic";
import {IBattleCommandToServerResponse} from "./IBattleCommandToServerResponse";
import { AcceptRewardBody } from "../rewards/AcceptRewardBody";
import { IRewardToAccept } from "../rewards/IRewardToAccept";

class ServerSideBattle implements IBattleDefinition {
    readonly id: string
    readonly width: number
    readonly height: number
    readonly units: IBattleDefinitionUnit[] = []
    readonly rewards: IBattleDefinitionReward[] = []
    readonly expiresAfterDays: number = 10
    readonly enemyUnits: IUserUnit[]
    readonly isFinished: boolean = false
    constructor(battleId: string, mapWidth: number, mapHeight: number, enemyUnits: IUserUnit[]) {
        this.id = battleId
        this.width = mapWidth
        this.height = mapHeight
        this.enemyUnits = enemyUnits
    }
    readonly stage = 0

}

const FakeUserToken = 'FakeToken123'

export class FakeServerAPI implements IServerAPI, IBattleServerConnection {
    getHeroKnownMagic(): Promise<IKnownMagicInfo[]> {
        throw new Error("Method not implemented.");
    }
    getRansomPrice(): Promise<IRansomPrice> {
        throw new Error("Method not implemented.");
    }
    beginGuardBattle(rewardId: string): Promise<BattleMap> {
        throw new Error("Method not implemented.");
    }
    getUnitTypesInfos(unitTypeIds: string[]): Promise<IUnitTypeInfo[]> {
        throw new Error("Method not implemented.");
    }
    getBattleUnitInfo(battleId: string, unitId: string): Promise<import("../services/serverAPI").IBattleUnitInfo> {
        throw new Error("Method not implemented.");
    }
    beginBattleWithPlayer(playerName: string): Promise<BattleMap> {
        throw new Error("Method not implemented.");
    }
    getUnitTypeInfo(unitTypeId: string): Promise<IUnitTypeInfo> {
        throw new Error("Method not implemented.");
    }
    getReport(reportId: string): Promise<IReportInfo> {
        throw new Error("Method not implemented.");
    }
    getResources(): Promise<IResourceInfo[]> {
        throw new Error("Method not implemented.");
    }
    getMyArtifacts(): Promise<IArtifactInfo[]> {
        throw new Error("Method not implemented.");
    }
    equipArtifact(artifactId: string, equippedSlotsIndexes: number[]): Promise<IArtifactInfo> {
        throw new Error("Method not implemented.");
    }
    async moveUnits(unitId: string, containerId: string, count: number, slotIndex: number): Promise<IUnitsContainerInfo> {
        const id = await this.getUserId()
        const units = this.units.get(id) ?? []
        
        // Find the source unit
        const sourceUnit = units.find(u => u.id === unitId)
        if (!sourceUnit) {
            throw new Error('Unit not found')
        }
        
        // Check if we have enough units to move
        if (sourceUnit.count < count) {
            throw new Error('Not enough units to move')
        }
        
        // Check if target slot is available
        const targetSlotOccupied = units.some(u => u.slotIndex === slotIndex && u.id !== unitId)
        if (targetSlotOccupied) {
            throw new Error('Target slot is occupied')
        }
        
        // Remove the source unit
        const sourceIndex = units.findIndex(u => u.id === unitId)
        if (sourceIndex !== -1) {
            units.splice(sourceIndex, 1)
        }
        
        // Create updated source unit with reduced count (if count > 0)
        if (sourceUnit.count > count) {
            const updatedSourceUnit: IUserUnit = {
                id: getRandomStringKey(7),
                typeId: sourceUnit.typeId,
                count: sourceUnit.count - count,
                thumbnailUrl: sourceUnit.thumbnailUrl,
                slotIndex: sourceUnit.slotIndex,
                name: sourceUnit.name,
            }
            units.push(updatedSourceUnit)
        }
        
        // Create new unit in target slot
        const newUnit: IUserUnit = {
            id: getRandomStringKey(7),
            typeId: sourceUnit.typeId,
            count: count,
            thumbnailUrl: sourceUnit.thumbnailUrl,
            slotIndex: slotIndex,
            name: sourceUnit.name,
        }
        units.push(newUnit)
        
        // Return updated container info
        if (containerId.includes('army')) {
            return {
                id: containerId,
                name: 'Army',
                units: units,
                capacity: 7
            }
        } else {
            return {
                id: containerId,
                name: 'Supply',
                units: units,
                capacity: 10
            }
        }
    }
    async getUnitsContainer(containerId: string): Promise<IUnitsContainerInfo> {
        const id = await this.getUserId()
        const units = this.units.get(id) ?? []
        
        // For now, assume army container has ID ending with 'army' and supply with 'supply'
        // In a real implementation, you'd have separate containers
        if (containerId.includes('army')) {
            return {
                id: containerId,
                name: 'Army',
                units: units,
                capacity: 7
            }
        } else {
            return {
                id: containerId,
                name: 'Supply',
                units: units,
                capacity: 10
            }
        }
    }
    getPlayer(id: string): Promise<IPlayerInfo> {
        return Promise.resolve({
            id: id,
            day: 1,
            stage: 0,
            name: "FakePlayer",
            isDefeated: false,
            battlesGenerationInProgress: false,
            armyContainerId: `${id}_army`,
            supplyContainerId: `${id}_supply`,
            experience: 0,
            level: 1,
            stats: {
                attack: 0,
                defense: 0,
                power: 0,
                knowledge: 0,
                currentMana: 0,
                maxMana: 0,
            }
        })
    }
    getPlayers(): Promise<IPlayerInfo[]> {
        throw new Error("Method not implemented.");
    }
    setActivePlayer(playerId: string): Promise<void> {
        throw new Error("Method not implemented.");
    }
    private readonly users = new Map<string, IUserInfo>()
    
    private readonly units = new Map<string, IUserUnit[]>()
    private readonly battles = new Map<string, ServerSideBattle[]>()
    private readonly activeBattles = new Map<string, BattleMap>()
    private readonly unitTypes = new Map<string, UnitProperties>([
        ['1', {
            battleImgUrl: "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp",
            speed: 5,
            health: 10,
            attack: 1,
            defense: 1,
            attacks: [
                {
                    name: "Attack",
                    thumbnailUrl: "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp",
                    attackMaxRange: 1,
                    attackMinRange: 1,
                    stayOnly: false,
                    counterattacksCount: 0,
                    counterattackPenaltyPercentage: 0,
                    rangePenalty: false,
                    rangePenaltyZonesCount: 0,
                    minDamage: 1,
                    maxDamage: 1,
                    enemyInRangeDisablesAttack: 0,
                    pierceThrough: 0,
                    splash: 0,
                }
            ],
            attacksStates: [],
            waited: false,
            movementType: MovementType.Walk,
        }]
    ])
    private messagesHandler: IBattleConnectionMessagesHandler | undefined = undefined
    
    login(): Promise<void> {
        const token = FakeUserToken //getRandomStringKey(10)
        const userId = getRandomStringKey(7)
        const userInfo = {
            userId,
            userName: "FakeName",
            playerId: null,
        }
        this.users.set(token, userInfo)
        
        this.units.set(userId,[
            {
                id: getRandomStringKey(7),
                typeId: '1',
                name: "FakeUnit",
                thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                count: 30,
                slotIndex: 0,
            },
        ])
        this.battles.set(userId, [
            new ServerSideBattle('0', 6, 6, [
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 3,
                    slotIndex: 0,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 10,
                    slotIndex: 1,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 3,
                    slotIndex: 2,
                },
            ]),
            new ServerSideBattle('1', 10, 8, [
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 3,
                    slotIndex: 0,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 3,
                    slotIndex: 1,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 10,
                    slotIndex: 2,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 3,   
                    slotIndex: 3,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 3,
                    slotIndex: 4,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 9,
                    slotIndex: 5,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 3,
                    slotIndex: 6,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 3,
                    slotIndex: 7,
                },
            ]),
            new ServerSideBattle('2', 8, 4, [
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 1,
                    slotIndex: 0,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 2,
                    slotIndex: 1,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 3,
                    slotIndex: 2,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 4,
                    slotIndex: 3,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 5,
                    slotIndex: 4,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 6,
                    slotIndex: 5,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    name: "FakeUnit",
                    thumbnailUrl: this.unitTypes.get('1')!.battleImgUrl,
                    count: 7,
                    slotIndex: 6,
                },
            ]),
        ])
        
        setSessionCookie(token)
        return Promise.resolve()
    }
    async getUserInfo(): Promise<IUserInfo> {
        const token = await this.getSessionToken()
        const userInfo = this.users.get(token)
        if (userInfo) {
            return userInfo
        }

        throw {
            status: 403,
            message: "Wrong token."
        }
    }
    
    async getBattles(): Promise<IBattleDefinition[]> {
        const id = await this.getUserId()
        const battles = structuredClone(this.battles.get(id)) ?? []
        return battles
    }
    async beginBattle(battleId: string): Promise<BattleMap> {
        const id = await this.getUserId()
        const battles = this.battles.get(id) ?? []
        const targetBattle = battles.find(battle => battle.id === battleId)
        if (!targetBattle) {
            throw {
                status: 404,
                message: "No battle found."
            }
        }
        
        const userUnits = this.units.get(id) ?? []
        const activeBattle = await this.beginBattleInternal(targetBattle, userUnits)
        this.activeBattles.set(id, activeBattle)
        return structuredClone(activeBattle)
    }
    
    async getActiveBattle(): Promise<BattleMap | null> {
        const id = await this.getUserId()
        return structuredClone(this.activeBattles.get(id)) ?? null
    }

    establishBattleConnection(battleId: string, handler: IBattleConnectionMessagesHandler): Promise<IBattleServerConnection> {
        this.messagesHandler = handler
        return Promise.resolve(this)
    }

    getMyRewards(): Promise<IRewardToAccept[]> {
        throw new Error("Method not implemented.");
    }
    acceptReward(id: string, body: AcceptRewardBody): Promise<IAcceptedReward> {
        throw new Error("Method not implemented.");
    }

    private getSessionToken(): Promise<string> {
        const token = getSessionCookie()
        if (!token) {
            return Promise.reject({
                status: 401,
                message: "Not authenticated."
            })
        }
        return Promise.resolve(token)
    }

    private async getUserId(): Promise<string> {
        const info = await this.getUserInfo();
        return info.userId;
    }

    private beginBattleInternal(battleDefinition: ServerSideBattle, userUnits: IUserUnit[]): Promise<BattleMap> {
        const cells: BattleMapCell[][] = []
        for (let i = 0; i < battleDefinition.height; i++) {
            const row: BattleMapCell[] = []
            for (let j = 0; j < battleDefinition.width; j++) {
                const cell: BattleMapCell = { c: j, r: i, isObstacle: false }
                row.push(cell);
            }
            cells.push(row)
        }
        return Promise.resolve({
            id: getRandomStringKey(7),
            width: battleDefinition.width,
            height: battleDefinition.height,
            grid: new OddRGrid(cells),
            units: [
                ...this.loadUnits(battleDefinition.enemyUnits, BattlePlayerNumber.Player2, battleDefinition.width, battleDefinition.height),
                ...this.loadUnits(userUnits, BattlePlayerNumber.Player1, battleDefinition.width, battleDefinition.height),
            ],
            turnInfo: {
                player: BattlePlayerNumber.Player1,
                index: 0,
                roundNumber: 0,
                nextTurnUnitId: undefined,
                canAct: true,
            },
            players: [
                {
                    playerId: getRandomStringKey(7),
                    playerNumber: BattlePlayerNumber.Player1,
                    index: 0,
                    ransomClaimed: false,
                    runClaimed: false,
                    heroStats: {
                        attack: 5,
                        defense: 3,
                        power: 2,
                        knowledge: 1,
                        currentMana: 10,
                        maxMana: 10,
                    },
                },
                {
                    playerId: getRandomStringKey(7),
                    playerNumber: BattlePlayerNumber.Player2,
                    index: 1,
                    ransomClaimed: false,
                    runClaimed: false,
                    heroStats: {
                        attack: 4,
                        defense: 4,
                        power: 1,
                        knowledge: 2,
                        currentMana: 8,
                        maxMana: 20,
                    },
                },
            ],
            obstacles: [],
        })
    }
    
    private loadUnits(userUnits: IUserUnit[], playerNumber: BattlePlayerNumber, mapWidth: number, mapHeight: number): BattleMapUnit[] {
        return userUnits.map((unit, index) => {
            const unitPosition = this.getUnitPosition(index, playerNumber, mapWidth, mapHeight)  
            const unitType = this.unitTypes.get(unit.typeId)
            return {
                id: unit.id,
                player: playerNumber,
                props: {...unitType!},
                currentProps:    {...unitType!},
                count: unit.count,
                isAlive: true,
                position: unitPosition,
                waited: false,
                name: unit.name,
            }
        })
    }
    
    private getUnitPosition(index: number, playerNumber: BattlePlayerNumber, mapWidth: number, mapHeight: number): IHexoPoint {
        const halfWidth = Math.floor(mapWidth / 2)

        // Determine position based on player
        let column: number
        let row: number

        if (playerNumber === BattlePlayerNumber.Player1) {
            // Player 1 units are placed on the left side (columns 0 to halfWidth - 1)
            column = Math.floor(index / mapHeight);
            row = index % mapHeight;
        } else {
            // Player 2 units are placed on the far right side (columns mapWidth - 1 to halfWidth)
            column = mapWidth - 1 - Math.floor(index / mapHeight);
            row = index % mapHeight;
        }

        return { c: column, r: row }
    }

    async sendMessage(message: BattleCommandToServer): Promise<IBattleCommandToServerResponse> {
        const id = await this.getUserId()
        const activeBattle = this.activeBattles.get(id)
        if (!activeBattle) {
            return {
                rejectedReason: BattleServerMessageRejectionReason.BattleNotFound,
                rejectedReasonDetails: undefined,
                requestedCommand: message,
                status: BattleServerMessageResponseStatus.Rejected
            }
        }
        
        if (message.command === 'UNIT_MOVE') {
            const actorUnit = getUnitById(activeBattle, message.actorId)
            if (!actorUnit) {
                return {
                    rejectedReason: BattleServerMessageRejectionReason.InvalidCommand,
                    rejectedReasonDetails: "Unit not found",
                    requestedCommand: message,
                    status: BattleServerMessageResponseStatus.Rejected
                }
            }
            
            this.messagesHandler?.onMessage({
                turnNumber: activeBattle.turnInfo.index,
                player: message.player,
                command: 'UNIT_MOVE',
                actorId: message.actorId,
                moveToCell: message.moveToCell,
                commandId: getRandomStringKey(10),
            })
            
            this.onNextTurn(activeBattle)
            
            return {
                rejectedReason: undefined,
                rejectedReasonDetails: undefined,
                requestedCommand: message,
                status: BattleServerMessageResponseStatus.Approved,
            }
            
        } else if (message.command === 'UNIT_ATTACK') {
            const actorUnit = getUnitById(activeBattle, message.actorId)
            if (!actorUnit) {
                return {
                    rejectedReason: BattleServerMessageRejectionReason.InvalidCommand,
                    rejectedReasonDetails: "Unit not found",
                    requestedCommand: message,
                    status: BattleServerMessageResponseStatus.Rejected
                }
            }
            const targetUnit = getUnitById(activeBattle, message.targetId)
            if (!targetUnit) {
                return {
                    rejectedReason: BattleServerMessageRejectionReason.InvalidCommand,
                    rejectedReasonDetails: "Target unit not found",
                    requestedCommand: message,
                    status: BattleServerMessageResponseStatus.Rejected
                }
            }

            this.messagesHandler?.onMessage({
                turnNumber: activeBattle.turnInfo.index,
                player: message.player,
                command: 'UNIT_MOVE',
                actorId: message.actorId,
                moveToCell: message.moveToCell,
                commandId: getRandomStringKey(10),
            })

            this.messagesHandler?.onMessage({
                turnNumber: activeBattle.turnInfo.index,
                player: message.player,
                command: 'UNIT_ATTACK',
                actorId: message.actorId,
                targetId: message.targetId,
                commandId: getRandomStringKey(10),
                attackIndex: message.attackIndex,
                isCounterattack: false,
            })
            
            const globalUnits = this.units.get(id)!
            const targetUnitGlobal = globalUnits.find(x => x.id === targetUnit.id)
            const damageParams = this.processAttack(actorUnit, targetUnit)
            if (damageParams.unitSurvived) {
                targetUnit.count = targetUnit.count - damageParams.killedCount
                targetUnit.currentProps.health = damageParams.healthLeft
                if (targetUnitGlobal) {
                    // targetUnitGlobal.count = targetUnit.count
                }
            } else {
                targetUnit.count = 0
                activeBattle.units.splice(activeBattle.units.indexOf(targetUnit), 1)
                if (targetUnitGlobal) {
                    globalUnits.splice(globalUnits.indexOf(targetUnitGlobal), 1)
                }
            }
            this.messagesHandler?.onMessage({
                turnNumber: activeBattle.turnInfo.index,
                player: message.player,
                command: 'TAKE_DAMAGE',
                actorId: message.targetId,
                commandId: getRandomStringKey(10),
                damageTaken: damageParams.damageTaken,
                killedCount: damageParams.killedCount,
                remainingCount: targetUnit.count,
                remainingHealth: damageParams.healthLeft,
            })

            this.onNextTurn(activeBattle)

            return {
                rejectedReason: undefined,
                rejectedReasonDetails: undefined,
                requestedCommand: message,
                status: BattleServerMessageResponseStatus.Approved,
            }
        } else {
            return {
                rejectedReason: BattleServerMessageRejectionReason.UnknownCommand,
                rejectedReasonDetails: undefined,
                requestedCommand: message,
                status: BattleServerMessageResponseStatus.Rejected
            }
        }
    }

    private processAttack(actor: BattleMapUnit, target: BattleMapUnit): {
        damageTaken: number
        killedCount: number
        unitSurvived: boolean
        healthLeft: number
    } {
        const damage = actor.props.attacks[0].maxDamage * actor.count
        return this.unitTakeDamage(target, damage)
    }

    private unitTakeDamage(target: BattleMapUnit, damage: number): {
        damageTaken: number
        killedCount: number
        unitSurvived: boolean
        healthLeft: number
    }  {
        let finalHealth = target.currentProps.health - damage
        let newCount = target.count
        if (finalHealth < 0) {
            const killedUnits = Math.trunc(finalHealth * (-1) / target.props.health) + 1
            finalHealth = finalHealth + killedUnits * target.props.health
            newCount = target.count - killedUnits

            if (newCount < 1) {
                return {
                    damageTaken: target.count * target.props.health + target.currentProps.health,
                    killedCount: target.count,
                    unitSurvived: false,
                    healthLeft: finalHealth,
                }
            }
        }
        return {
            damageTaken: damage,
            killedCount: target.count - newCount,
            unitSurvived: true,
            healthLeft: finalHealth,
        }
    }
    
    private onNextTurn(map: BattleMap) {
        map.turnInfo.index = map.turnInfo.index + 1
        const orderedUnits = [...map.units]
            .sort((a, b) => b.props.speed - a.props.speed)
        const targetIndex = map.turnInfo.index % orderedUnits.length
        const targetUnit = orderedUnits[targetIndex]
        
        this.messagesHandler?.onMessage({
            command: 'NEXT_TURN',
            player: targetUnit.player,
            commandId: getRandomStringKey(10),
            turnNumber: map.turnInfo.index,
            nextTurnUnitId: targetUnit.id,
            roundNumber: map.turnInfo.roundNumber,
            canAct: true,
        })
    }
    
    close(): Promise<void> {
        this.messagesHandler = undefined
        return Promise.resolve()
    }
}