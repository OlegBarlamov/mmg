import {ICanvasService} from "./canvasService";
import {BattleMap, BattleMapCell} from "../battleMap/battleMap";
import {BattleMapController, IBattleMapController} from "../battleMap/battleMapController";
import {PlayerNumber} from "../player/playerNumber";
import {OddRGrid} from "../hexogrid/oddRGrid";
import {EvenQGrid} from "../hexogrid/evenQGrid";

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
            grid: new OddRGrid(cells),
            units: [{
                position: {
                    c: 0,
                    r: 0
                },
                player: PlayerNumber.Player1,
                props: {
                    battleMapIcon: "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp",
                    speed: 5
                },
                unitsCount: 15
            },
                {
                    position: {
                        c: 0,
                        r: 1
                    },
                    player: PlayerNumber.Player1,
                    props: {
                        battleMapIcon: "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp",
                        speed: 5
                    },
                    unitsCount: 199
                },
                {
                    position: {
                        c: 1,
                        r: 1
                    },
                    player: PlayerNumber.Player2,
                    props: {
                        battleMapIcon: "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp",
                        speed: 5
                    },
                    unitsCount: 1
                },
            ]
        }
    }
}