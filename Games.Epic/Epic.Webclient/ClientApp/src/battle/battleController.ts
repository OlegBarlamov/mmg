import { IBattleMapController } from "../battleMap/battleMapController";
import { BattleMapUnit } from "../battleMap/battleMapUnit";
import { BattleMap, BattleTurnInfo } from "../battleMap/battleMap";
import { BattleUserAction } from "./battleUserAction";
import { wait } from "../common/wait";
import { BattlePlayerNumber } from "../player/playerNumber";
import { BattleUserInputController } from "./battleUserInputController";
import { getAttackTargets, getCellsForUnitMove } from "./battleLogic";
import { IBattleActionsProcessor } from "./battleActionsProcessor";
import { ITurnAwaiter } from "./battleServerMessagesHandler";
import { SignalBasedCancellationToken, TaskCancelledError } from "../common/cancellationToken";
import { Signal } from "typed-signals";
import { IBattlePanelActionsController } from "./IBattlePanelActionsController";

export interface IBattleController {
    readonly mapController: IBattleMapController
    readonly onNextTurn: Signal<(turnInfo: BattleTurnInfo) => void>
    isPlayerControlled(player: BattlePlayerNumber): boolean
    startBattle(): Promise<{ winner: BattlePlayerNumber | null, reportId: string | null }>
    dispose(): void
}

export class BattleController implements IBattleController {
    onNextTurn: Signal<(turnInfo: BattleTurnInfo) => void> = new Signal()
    
    mapController: IBattleMapController

    private battleStarted: boolean = false
    private battleFinished: boolean = false
    private currentTurnIndex: number = -1
    private currentRoundNumber: number = 0
    private winnerPlayer: BattlePlayerNumber | null = null

    private readonly map: BattleMap

    private readonly battleUserInputController: BattleUserInputController
    private readonly battleActionProcessor: IBattleActionsProcessor
    private readonly turnAwaiter: ITurnAwaiter

    constructor(
        mapController: IBattleMapController,
        battleActionProcessor: IBattleActionsProcessor,
        turnAwaiter: ITurnAwaiter,
        panelController: IBattlePanelActionsController) {

        this.mapController = mapController
        this.battleActionProcessor = battleActionProcessor
        this.turnAwaiter = turnAwaiter

        this.map = mapController.map

        this.battleUserInputController = new BattleUserInputController(mapController, panelController)
    }

    dispose(): void {
        this.onNextTurn.disconnectAll()
        this.mapController.destroy()
        this.battleUserInputController.dispose()
        this.turnAwaiter.dispose()
    }

    async startBattle(): Promise<{ winner: BattlePlayerNumber | null, reportId: string | null }> {
        if (this.battleStarted) throw new Error("The battle already started");
        this.battleStarted = true

        await this.battleActionProcessor.onClientConnected()

        this.currentRoundNumber = this.map.turnInfo.roundNumber
        this.currentTurnIndex = this.map.turnInfo.index

        let reportId: string | null = null;

        while (!this.battleFinished) {
            try {
                this.onNextTurn.emit(this.map.turnInfo)

                if (this.isPlayerControlled(this.map.turnInfo.player)) {
                    const currentUnit = this.getActiveUnit(this.map.turnInfo.nextTurnUnitId!)
                    if (currentUnit) {
                        await this.processStep(currentUnit)
                    } else {
                        console.error("No active unit found for turn " + this.currentTurnIndex)
                    }
                }

                const turnInfo = await this.turnAwaiter.waitForTurn(this.currentTurnIndex + 1)
                if (turnInfo.roundNumber > this.currentRoundNumber) {
                    this.onNextRound()
                }

                this.currentTurnIndex = turnInfo.index
                this.currentRoundNumber = turnInfo.roundNumber
                this.battleFinished = turnInfo.result?.finished ?? false
                this.winnerPlayer = turnInfo.result?.winner ?? null
                
                // Capture the report ID when battle finishes
                if (this.battleFinished && turnInfo.result?.reportId) {
                    reportId = turnInfo.result.reportId;
                }

            } catch (e) {
                console.error('Error while processing the battle action: ' + e)
            }
        }

        await wait(1000)

        return { winner: this.winnerPlayer, reportId: reportId }
    }

    isPlayerControlled(player: BattlePlayerNumber): boolean {
        return player == BattlePlayerNumber.Player1 || player == BattlePlayerNumber.Player2
    }

    private onNextRound(): void {
        this.map.units.forEach(unit => {
            unit.waited = false
        })
    }

    private getActiveUnit(unitId: string): BattleMapUnit | null {
        return this.map.units.find(unit => unit.id === unitId) ?? null
    }

    private async processStep(unit: BattleMapUnit): Promise<void> {
        await this.mapController.battleMapHighlighter.setActiveUnit(unit)

        const cellsForMove = getCellsForUnitMove(this.map, unit)
        const reachableCells = [this.map.grid.getCell(unit.position.r, unit.position.c), ...cellsForMove]
        const attackTargets = getAttackTargets(this.map, unit, reachableCells)

        this.mapController.battleMapHighlighter.highlightCellsForMove(cellsForMove)
        this.mapController.battleMapHighlighter.highlightAttackTargets(attackTargets)

        const cancellationTokenSource = new SignalBasedCancellationToken(this.turnAwaiter.onCurrentPlayerActionReceived)

        let action: BattleUserAction | null = null
        try {
            action = await this.battleUserInputController.getUserInputAction(unit, cellsForMove, attackTargets, cancellationTokenSource.token)
        } catch (e) {
            if (e instanceof TaskCancelledError) {
                // Do nothing
            } else {
            throw e
            }
        } finally {
            cancellationTokenSource.dispose()
            this.mapController.battleMapHighlighter.restoreHighlightingForCells(cellsForMove)
            this.mapController.battleMapHighlighter.restoreHighlightingForAttackTargets(attackTargets)
            await this.mapController.battleMapHighlighter.setActiveUnit(null)
        }

        if (action) {
            await this.battleActionProcessor.processAction(action)
        }

        await wait(500)
    }
}