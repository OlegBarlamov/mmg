import {IBattleDefinition} from "../battle/battleDefinition";
import {BattleMap, BattleMapCell} from "../battleMap/battleMap";
import {IServerAPI, IUserInfo, IUserUnit} from "../services/serverAPI";
import {getSessionCookie, setSessionCookie} from "../units/cookiesHelper";
import {BattleMapUnit} from "../battleMap/battleMapUnit";
import {OddRGrid} from "../hexogrid/oddRGrid";
import {PlayerNumber} from "../player/playerNumber";
import {UnitProperties} from "../units/unitProperties";
import {IHexoPoint} from "../hexogrid/hexoGrid";
import {getRandomStringKey} from "../units/getRandomString";
import {
    BattleServerMessageRejectionReason,
    BattleServerMessageResponseStatus,
    IBattleCommandToServerResponse,
    IBattleConnectionMessagesHandler,
    IBattleServerConnection
} from "./battleServerConnection";
import {BattleCommandToServer} from "./battleCommandToServer";
import {getUnitById} from "../battle/battleLogic";

class ServerSideBattle implements IBattleDefinition {
    readonly battleId: string
    readonly mapWidth: number
    readonly mapHeight: number

    readonly enemyUnits: IUserUnit[]

    constructor(battleId: string, mapWidth: number, mapHeight: number, enemyUnits: IUserUnit[]) {
        this.battleId = battleId
        this.mapWidth = mapWidth
        this.mapHeight = mapHeight
        this.enemyUnits = enemyUnits
    }
}

const FakeUserToken = 'FakeToken123'

export class FakeServerAPI implements IServerAPI, IBattleServerConnection {
    private readonly users = new Map<string, IUserInfo>()
    
    private readonly units = new Map<string, IUserUnit[]>()
    private readonly battles = new Map<string, ServerSideBattle[]>()
    private readonly activeBattles = new Map<string, BattleMap>()
    private readonly unitTypes = new Map<string, UnitProperties>([
        ['1', {
            battleMapIcon: "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp",
            speed: 5,
            attackMinRange: 1,
            attackMaxRange: 1,
            damage: 6,
            health: 10,
        }]
    ])
    private messagesHandler: IBattleConnectionMessagesHandler | undefined = undefined
    
    signup(userName: string): Promise<IUserInfo> {
        const token = FakeUserToken //getRandomStringKey(10)
        const userId = getRandomStringKey(7)
        const userInfo = {
            userId,
            userName,
        }
        this.users.set(token, userInfo)
        
        this.units.set(userId,[
            {
                id: getRandomStringKey(7),
                typeId: '1',
                thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                count: 30,
            },
        ])
        this.battles.set(userId, [
            new ServerSideBattle('0', 6, 6, [
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 3,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 10,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 3,
                },
            ]),
            new ServerSideBattle('1', 10, 8, [
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 3,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 3,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 10,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 3,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 3,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 9,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 3,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 3,
                },
            ]),
            new ServerSideBattle('2', 8, 4, [
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 1,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 2,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 3,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 4,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 5,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 6,
                },
                {
                    id: getRandomStringKey(7),
                    typeId: '1',
                    thumbnailUrl: this.unitTypes.get('1')!.battleMapIcon,
                    count: 7,
                },
            ]),
        ])
        
        setSessionCookie(token)
        return Promise.resolve(userInfo)
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
    
    async getUnits(): Promise<IUserUnit[]> {
        const id = await this.getUserId()
        return structuredClone(this.units.get(id)) ?? []
    }
    async getBattles(): Promise<IBattleDefinition[]> {
        const id = await this.getUserId()
        return structuredClone(this.battles.get(id)) ?? []
    }
    async beginBattle(battleId: string): Promise<BattleMap> {
        const id = await this.getUserId()
        const battles = this.battles.get(id) ?? []
        const targetBattle = battles.find(battle => battle.battleId === battleId)
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

    establishBattleConnection(handler: IBattleConnectionMessagesHandler): Promise<IBattleServerConnection> {
        this.messagesHandler = handler
        return Promise.resolve(this)
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
        for (let i = 0; i < battleDefinition.mapHeight; i++) {
            const row: BattleMapCell[] = []
            for (let j = 0; j < battleDefinition.mapWidth; j++) {
                const cell: BattleMapCell = { c: j, r: i }
                row.push(cell);
            }
            cells.push(row)
        }
        return Promise.resolve({
            grid: new OddRGrid(cells),
            units: [
                ...this.loadUnits(battleDefinition.enemyUnits, PlayerNumber.Player2, battleDefinition.mapWidth, battleDefinition.mapHeight),
                ...this.loadUnits(userUnits, PlayerNumber.Player1, battleDefinition.mapWidth, battleDefinition.mapHeight),
            ],
            turn: {
                player: PlayerNumber.Player1,
                index: 0,
            }
        })
    }
    
    private loadUnits(userUnits: IUserUnit[], playerNumber: PlayerNumber, mapWidth: number, mapHeight: number): BattleMapUnit[] {
        return userUnits.map((unit, index) => {
            const unitPosition = this.getUnitPosition(index, playerNumber, mapWidth, mapHeight)  
            const unitType = this.unitTypes.get(unit.typeId)
            return {
                id: unit.id,
                player: playerNumber,
                props: {...unitType!},
                currentProps: {...unitType!},
                count: unit.count,
                position: unitPosition,
            }
        })
    }
    
    private getUnitPosition(index: number, playerNumber: PlayerNumber, mapWidth: number, mapHeight: number): IHexoPoint {
        const halfWidth = Math.floor(mapWidth / 2)

        // Determine position based on player
        let column: number
        let row: number

        if (playerNumber === PlayerNumber.Player1) {
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
                player: message.player,
                command: 'UNIT_MOVE',
                actorId: message.actorId,
                moveToCell: message.moveToCell,
                commandId: getRandomStringKey(10),
            })

            this.messagesHandler?.onMessage({
                player: message.player,
                command: 'UNIT_ATTACK',
                actorId: message.actorId,
                targetId: message.targetId,
                commandId: getRandomStringKey(10),
            })
            
            const globalUnits = this.units.get(id)!
            const targetUnitGlobal = globalUnits.find(x => x.id === targetUnit.id)
            const damageParams = this.processAttack(actorUnit, targetUnit)
            if (damageParams.unitSurvived) {
                targetUnit.count = targetUnit.count - damageParams.killedCount
                targetUnit.currentProps.health = damageParams.healthLeft
                if (targetUnitGlobal) {
                    targetUnitGlobal.count = targetUnit.count
                }
            } else {
                activeBattle.units.splice(activeBattle.units.indexOf(targetUnit), 1)
                if (targetUnitGlobal) {
                    globalUnits.splice(globalUnits.indexOf(targetUnitGlobal), 1)
                }
            }
            this.messagesHandler?.onMessage({
                player: message.player,
                command: 'TAKE_DAMAGE',
                actorId: message.targetId,
                commandId: getRandomStringKey(10),
                ...damageParams
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
        const damage = actor.props.damage * actor.count
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
        map.turn.index = map.turn.index + 1
        const orderedUnits = [...map.units]
            .sort((a, b) => b.props.speed - a.props.speed)
        const targetIndex = map.turn.index % orderedUnits.length
        const targetUnit = orderedUnits[targetIndex]
        
        this.messagesHandler?.onMessage({
            command: 'NEXT_TURN',
            player: targetUnit.player,
            commandId: getRandomStringKey(10),
            turnIndex: map.turn.index,
        })
    }
    
    close(): Promise<void> {
        this.messagesHandler = undefined
        return Promise.resolve()
    }
}