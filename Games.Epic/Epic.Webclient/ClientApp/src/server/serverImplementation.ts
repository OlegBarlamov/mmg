import {IBattleDefinition} from "../battle/IBattleDefinition";
import {BattleMap, BattleMapCell} from "../battleMap/battleMap";
import {IPlayerInfo, IServerAPI, IUnitsContainerInfo, IUserInfo, IUserUnit} from "../services/serverAPI";
import {
    BattleServerConnection,
    IBattleConnectionMessagesHandler,
    IBattleServerConnection
} from "./battleServerConnection";
import {BaseServer} from "./baseServer";
import {OddRGrid} from "../hexogrid/oddRGrid";
import { IRewardToAccept } from "../rewards/IRewardToAccept";
import { AcceptRewardBody } from "../rewards/AcceptRewardBody";

export const SERVER_BASE_URL = "http://localhost:5000"

export class ServerImplementation extends BaseServer implements IServerAPI {
    constructor(baseUrl: string) {
        super(baseUrl)
    }    
    moveUnits(unitId: string, containerId: string, count: number, slotIndex: number): Promise<IUnitsContainerInfo> {
        return this.fetchResource(`api/units/move/${unitId}`, "POST", "move_units", {
            containerId: containerId,
            amount: count,
            slotIndex: slotIndex,
        })
    }
    getPlayer(id: string): Promise<IPlayerInfo> {
        return this.fetchResource(`api/players/${id}`, "GET", "player")
    }
    login(): Promise<void> {
        return this.fetchResource("login", "GET", "login")
    }
    getUserInfo(): Promise<IUserInfo> {
        return this.fetchResource("api/user", "GET", "userInfo") 
    }
    getPlayers(): Promise<IPlayerInfo[]> {
        return this.fetchResource("api/players", "GET", "players")
    }
    getUnitsContainer(containerId: string): Promise<IUnitsContainerInfo> {
        return this.fetchResource(`api/units/containers/${containerId}`, "GET", "units_container")
    }
    getBattles(): Promise<IBattleDefinition[]> {
        return this.fetchResource("api/battles", "GET", "battles")
    }
    setActivePlayer(playerId: string): Promise<void> {
        return this.fetchResource(`api/players/${playerId}`, "POST", "set_active_player")
    }
    async beginBattle(battleId: string): Promise<BattleMap> {
        const battleMap = await this.fetchResource<BattleMap>("api/battle", "POST", "begin_battle", {
            battleDefinitionId: battleId,
        })
        this.fillMapCells(battleMap)
        return battleMap
    }
    async getActiveBattle(): Promise<BattleMap | null> {
        const response = await this.fetchResource<BattleMap[]>("api/battle", "GET", "active_battle")
        const map = response ? response[0] : null
        if (map) {
            this.fillMapCells(map)
        }
        return map
    }
    async establishBattleConnection(battleId: string, handler: IBattleConnectionMessagesHandler): Promise<IBattleServerConnection> {
        const webSocket = await this.establishWS(`api/battle/${battleId}`)
        return new BattleServerConnection(webSocket, handler)
    }
    async acceptReward(id: string, body: AcceptRewardBody): Promise<void> {
        await this.fetchResource(`api/rewards/${id}`, "POST", 'accept_reward', body)
    }

    getMyRewards(): Promise<IRewardToAccept[]> {
        return this.fetchResource("api/rewards", "GET", "rewards")
    }

    private fillMapCells(battleMap: BattleMap) {
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
    }
    
}