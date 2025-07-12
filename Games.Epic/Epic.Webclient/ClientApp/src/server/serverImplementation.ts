import {IBattleDefinition} from "../battle/IBattleDefinition";
import {BattleMap, BattleMapCell} from "../battleMap/battleMap";
import {IServerAPI, IUserInfo, IUserUnit} from "../services/serverAPI";
import {
    BattleServerConnection,
    IBattleConnectionMessagesHandler,
    IBattleServerConnection
} from "./battleServerConnection";
import {BaseServer} from "./baseServer";
import {OddRGrid} from "../hexogrid/oddRGrid";

export const SERVER_BASE_URL = "http://localhost:5000"

export class ServerImplementation extends BaseServer implements IServerAPI {
    constructor(baseUrl: string) {
        super(baseUrl)
    }
    
    login(userName: string): Promise<IUserInfo> {
        return this.fetchResource("login", "GET", "login")
    }
    getUserInfo(): Promise<IUserInfo> {
        return this.fetchResource("api/user", "GET", "userInfo") 
    }
    getUnits(): Promise<IUserUnit[]> {
        return this.fetchResource("api/units", "GET", "units")
    }
    getBattles(): Promise<IBattleDefinition[]> {
        return this.fetchResource("api/battles", "GET", "battles")
    }
    async beginBattle(battleId: string): Promise<BattleMap> {
        const battleMap = await this.fetchResource<BattleMap>("api/battle", "POST", "begin_battle", {
            battleDefinitionId: battleId,
        })
        const cells: BattleMapCell[][] = []
        for (let i = 0; i < battleMap.height; i++) {
            const row: BattleMapCell[] = []
            for (let j = 0; j < battleMap.width; j++) {
                const cell: BattleMapCell = { c: j, r: i }
                row.push(cell);
            }
            cells.push(row)
        }
        
        battleMap.grid = new OddRGrid(cells)
        return battleMap
    }
    getActiveBattle(): Promise<BattleMap | null> {
        return this.fetchResource("api/battle", "GET", "active_battle")
    }
    async establishBattleConnection(battleId: string, handler: IBattleConnectionMessagesHandler): Promise<IBattleServerConnection> {
        const webSocket = await this.establishWS(`api/battle/${battleId}`)
        return new BattleServerConnection(webSocket, handler)
    }
    
}