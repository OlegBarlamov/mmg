import {ICanvasService} from "./canvasService";
import {BattleMap, BattleMapCell} from "../battleMap/battleMap";
import {BattleMapController, IBattleMapController} from "../battleMap/battleMapController";
import {BattlePlayerNumber} from "../player/playerNumber";
import {OddRGrid} from "../hexogrid/oddRGrid";
import {getTestUnit} from "../battleMap/battleMapUnit";

export interface IBattleMapsService {
    load(map: BattleMap): Promise<IBattleMapController>
    
    generateMap(width: number, height: number) : BattleMap
}

export class BattleMapsService implements IBattleMapsService {
    private readonly canvasService: ICanvasService
    constructor(canvasService: ICanvasService) {
        this.canvasService = canvasService
    }
    
    async load(map: BattleMap): Promise<IBattleMapController> {
        const controller = new BattleMapController(map, this.canvasService)
        await controller.loadMap()
        await controller.loadUnits()
        return controller
    }

    generateMap(width: number, height: number) : BattleMap {
        const cells: BattleMapCell[][] = []
        for (let i = 0; i < height; i++) {
            const row: BattleMapCell[] = []
            for (let j = 0; j < width; j++) {
                const cell: BattleMapCell = { c: j, r: i }
                row.push(cell);
            }
            cells.push(row)
        }
        return {
            id: '0',
            width: width,
            height: height,
            grid: new OddRGrid(cells),
            units: [
                getTestUnit(0, 0, 15, BattlePlayerNumber.Player1),
                getTestUnit(0, 1, 199, BattlePlayerNumber.Player1),
                getTestUnit(1, 1, 80, BattlePlayerNumber.Player2),
            ],
            turnInfo: {
                player: BattlePlayerNumber.Player1,
                index: 0,
                roundNumber: 0,
                nextTurnUnitId: undefined,
            },
            players: [
                {
                    playerId: '0',
                    playerNumber: BattlePlayerNumber.Player1,
                    index: 0,
                    ransomClaimed: false,
                },
            ],
        }
    }
}