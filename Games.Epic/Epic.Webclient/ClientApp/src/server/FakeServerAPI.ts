import { IBattleDefinition } from "../battle/battleDefinition";
import {BattleMap, BattleMapCell} from "../battleMap/battleMap";
import {IUserInfo, IUsersUnit, IServerAPI} from "../services/serverAPI";
import {getSessionCookie, setSessionCookie} from "../units/cookiesHelper";
import {BattleMapUnit} from "../battleMap/battleMapUnit";
import {OddRGrid} from "../hexogrid/oddRGrid";
import {PlayerNumber} from "../player/playerNumber";
import {UnitProperties} from "../units/unitProperties";
import {IHexoPoint} from "../hexogrid/hexoGrid";
import {getRandomStringKey} from "../units/getRandomString";

class ServerSideBattle implements IBattleDefinition {
    readonly battleId: string  
    readonly mapWidth: number
    readonly mapHeight: number
    
    readonly enemyUnits: IUsersUnit[]
    
    constructor(battleId: string, mapWidth: number, mapHeight: number, enemyUnits: IUsersUnit[]) {
        this.battleId = battleId
        this.mapWidth = mapWidth
        this.mapHeight = mapHeight
        this.enemyUnits = enemyUnits
    }
}

const FakeUserToken = 'FakeToken123'

export class FakeServerAPI implements IServerAPI {
    private readonly users = new Map<string, IUserInfo>()
    
    private readonly units = new Map<string, IUsersUnit[]>()
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
    
    async getUnits(): Promise<IUsersUnit[]> {
        const id = await this.getUserId()
        return this.units.get(id) ?? []
    }
    async getBattles(): Promise<IBattleDefinition[]> {
        const id = await this.getUserId()
        return this.battles.get(id) ?? []
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
        return activeBattle
    }
    
    async getActiveBattle(): Promise<BattleMap | null> {
        const id = await this.getUserId()
        return this.activeBattles.get(id) ?? null
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

    private beginBattleInternal(battleDefinition: ServerSideBattle, userUnits: IUsersUnit[]): Promise<BattleMap> {
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
            ]
        })
    }
    
    private loadUnits(userUnits: IUsersUnit[], playerNumber: PlayerNumber, mapWidth: number, mapHeight: number): BattleMapUnit[] {
        return userUnits.map((unit, index) => {
            const unitPosition = this.getUnitPosition(index, playerNumber, mapWidth, mapHeight)  
            const unitType = this.unitTypes.get(unit.typeId)
            return {
                player: playerNumber,
                props: unitType!,
                unitsCount: unit.count,
                currentHealth: unitType!.health,
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
}