import { IBattleMapController } from "../battleMap/battleMapController";
import { BattleMapUnit } from "../battleMap/battleMapUnit";
import { BattleMap } from "../battleMap/battleMap";
import { BattleUserAction } from "./battleUserAction";
import { wait } from "../common/wait";
import { BattlePlayerNumber } from "../player/playerNumber";
import { BattleUserInputController } from "./battleUserInputController";
import { getAttackTargets, getCellsForUnitMove } from "./battleLogic";
import { IBattleActionsProcessor } from "./battleActionsProcessor";
import { ITurnAwaiter } from "./battleServerMessagesHandler";
import { SignalBasedCancellationToken, TaskCancelledError } from "../common/cancellationToken";

export interface IBattleController {
    startBattle(): Promise<BattlePlayerNumber | null>
    dispose(): void
}

export class BattleController implements IBattleController {
    mapController: IBattleMapController

    private battleStarted: boolean = false
    private battleFinished: boolean = false
    private currentTurnIndex: number = -1
    private winnerPlayer: BattlePlayerNumber | null = null
    private orderedUnits: BattleMapUnit[]

    private readonly map: BattleMap

    private readonly battleUserInputController: BattleUserInputController
    private readonly battleActionProcessor: IBattleActionsProcessor
    private readonly turnAwaiter: ITurnAwaiter

    constructor(
        mapController: IBattleMapController,
        battleActionProcessor: IBattleActionsProcessor,
        turnAwaiter: ITurnAwaiter) {

        this.mapController = mapController
        this.battleActionProcessor = battleActionProcessor
        this.turnAwaiter = turnAwaiter

        this.orderedUnits = [...this.mapController.map.units]
            .sort((a, b) => b.currentProps.speed - a.currentProps.speed)
        this.map = mapController.map

        this.battleUserInputController = new BattleUserInputController(mapController)
    }

    dispose(): void {
        this.orderedUnits.splice(0)
        this.mapController.destroy()
        this.turnAwaiter.dispose()
    }

    async startBattle(): Promise<BattlePlayerNumber | null> {
        if (this.battleStarted) throw new Error("The battle already started");
        this.battleStarted = true
        this.currentTurnIndex = this.turnAwaiter.currentTurnIndex

        while (!this.battleFinished) {
            const currentUnit = this.getActiveUnit(this.currentTurnIndex)
            this.map.turnInfo.player = currentUnit.player

            try {
                await this.processStep(currentUnit)

                const turnInfo = await this.turnAwaiter.waitForTurn(this.currentTurnIndex + 1)

                this.orderedUnits = [...this.mapController.map.units]
                    .sort((a, b) => b.currentProps.speed - a.currentProps.speed)

                this.currentTurnIndex = turnInfo.index

                this.battleFinished = turnInfo.result?.finished ?? false
                this.winnerPlayer = turnInfo.result?.winner ?? null

            } catch (e) {
                console.error('Error while processing the battle action: ' + e)
            }
        }

        await wait(1000 * 3)

        return this.winnerPlayer
    }

    private getActiveUnit(turnIndex: number): BattleMapUnit {
        turnIndex %= this.orderedUnits.length;
        let activeUnit = this.orderedUnits[turnIndex];
        
        while (!activeUnit.isAlive) {
            turnIndex++;
            turnIndex %= this.orderedUnits.length;
            activeUnit = this.orderedUnits[turnIndex];
        }
        
        return activeUnit;
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
            }
            throw e
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