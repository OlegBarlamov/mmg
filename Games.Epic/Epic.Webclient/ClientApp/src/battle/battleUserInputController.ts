import {BattleMapUnit} from "../battleMap/battleMapUnit";
import {BattleMapCell} from "../battleMap/battleMap";
import {IAttackTarget} from "./attackTarget";
import {BattleUserAction} from "./battleUserAction";
import {IBattleMapController} from "../battleMap/battleMapController";

export class BattleUserInputController {
    private mapController: IBattleMapController

    constructor(mapController: IBattleMapController) {
        this.mapController = mapController
    }
    
    getUserInputAction(originalUnit: BattleMapUnit, cellsToMove: BattleMapCell[], attackTargets: IAttackTarget[]): Promise<BattleUserAction> {
        return new Promise((resolve) => {
            this.mapController.onCellMouseClick = (cell) => {
                if (cellsToMove.indexOf(cell) >= 0) {
                    this.mapController.onCellMouseClick = null
                    resolve({
                        command: 'UNIT_MOVE',
                        player: originalUnit.player,
                        actor: originalUnit,
                        moveToCell: cell,
                    })
                }
            }
            this.mapController.onUnitMouseClick = (unit, event) => {
                const target = attackTargets.find(x => x.target === unit)
                if (target) {
                    this.mapController.onUnitMouseClick = null
                    const mouseCoordinates = {x: event.x, y: event.y}
                    const closestCell = this.mapController.getClosestCellToPoint(target.cells, mouseCoordinates)
                    resolve({
                        command: 'UNIT_ATTACK',
                        player: originalUnit.player,
                        actor: originalUnit,
                        moveToCell: closestCell,
                        attackTarget: target.target,
                    })
                }
            }
        })
    }
}