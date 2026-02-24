import {BattleMapUnit} from "../battleMap/battleMapUnit";
import {BattleMapCell} from "../battleMap/battleMap";
import {IAttackTarget} from "./attackTarget";
import {BattleUserAction} from "./battleUserAction";
import {IBattleMapController} from "../battleMap/battleMapController";
import { CancellationToken, TaskCancelledError } from "../common/cancellationToken";
import { IBattlePanelActionsController } from "./IBattlePanelActionsController";

export class BattleUserInputController {
    constructor(
        private readonly mapController: IBattleMapController,
        private readonly panelController: IBattlePanelActionsController
    ) {}
    
    getUserInputAction(
        originalUnit: BattleMapUnit,
        getCellsAndTargets: () => { cellsForMove: BattleMapCell[], attackTargets: IAttackTarget[] },
        cancellationToken: CancellationToken,
    ): Promise<BattleUserAction> {
        return new Promise((resolve, reject) => {
            cancellationToken.onCancel(() => {
                this.dispose()
                reject(new TaskCancelledError())
            })

            this.mapController.onCellMouseClick = (cell) => {
                const { cellsForMove } = getCellsAndTargets()
                if (cellsForMove.indexOf(cell) >= 0) {
                    this.dispose()
                    resolve({
                        command: 'UNIT_MOVE',
                        player: originalUnit.player,
                        actor: originalUnit,
                        moveToCell: cell,
                    })
                }
            }
            this.mapController.onUnitMouseClick = (unit, event) => {
                const { attackTargets } = getCellsAndTargets()
                const target = attackTargets.find(x => x.target === unit)
                if (target) {
                    this.dispose()
                    const mouseCoordinates = {x: event.x, y: event.y}
                    const closestCell = this.mapController.getClosestCellToPoint(target.cells, mouseCoordinates)

                    resolve({
                        command: 'UNIT_ATTACK',
                        player: originalUnit.player,
                        actor: originalUnit,
                        moveToCell: closestCell,
                        attackTarget: target.target,
                        attackTypeIndex: target.attackType.index,
                    })
                }
            }

            // Set up panel controller callbacks
            this.panelController.onPassPressed = () => {
                    this.dispose()
                    resolve({
                        command: 'UNIT_PASS',
                        player: originalUnit.player,
                        actor: originalUnit,
                    })
            }

            this.panelController.onWaitPressed = () => {
                    this.dispose()
                    resolve({
                        command: 'UNIT_WAIT',
                        player: originalUnit.player,
                        actor: originalUnit,
                    })
            }

            this.panelController.onRansomPressed = () => {
                    this.dispose()
                    resolve({
                        command: 'PLAYER_RANSOM',
                        player: originalUnit.player,
                    })
            }

            this.panelController.onRunPressed = () => {
                    this.dispose()
                    resolve({
                        command: 'PLAYER_RUN',
                        player: originalUnit.player,
                    })
            }
        })
    }

    dispose() {
        this.mapController.onCellMouseClick = null
        this.mapController.onUnitMouseClick = null
        this.panelController.onWaitPressed = null
        this.panelController.onPassPressed = null
        this.panelController.onRansomPressed = null
        this.panelController.onRunPressed = null
    }
}