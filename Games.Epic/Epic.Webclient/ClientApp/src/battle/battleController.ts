import { IBattleMapController } from "../battleMap/battleMapController";
import { BattleMapUnit } from "../battleMap/battleMapUnit";
import { BattleMap, BattleTurnInfo, InBattlePlayerInfo } from "../battleMap/battleMap";
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
import { IPlayerService } from "../services/playerService";
import { BattleCommandFromServer } from "../server/battleCommandFromServer";

export interface IBattleController {
    readonly mapController: IBattleMapController
    readonly onNextTurn: Signal<(turnInfo: BattleTurnInfo) => void>
    readonly onServerMessageHandlingStarted: Signal<(message: BattleCommandFromServer) => void>
    isPlayerControlled(player: BattlePlayerNumber): boolean
    startBattle(): Promise<{ winner: BattlePlayerNumber | null, reportId: string | null }>
    dispose(): void
}

export class BattleController implements IBattleController {
    onNextTurn: Signal<(turnInfo: BattleTurnInfo) => void> = new Signal()
    onServerMessageHandlingStarted: Signal<(message: BattleCommandFromServer) => void>
    
    mapController: IBattleMapController

    private battleStarted: boolean = false
    private battleFinished: boolean = false
    private currentTurnIndex: number = -1
    private currentRoundNumber: number = 0
    private winnerPlayer: BattlePlayerNumber | null = null
    private currentPlayerInfo: InBattlePlayerInfo | null = null

    private readonly map: BattleMap
    private readonly playerService: IPlayerService

    private readonly battleUserInputController: BattleUserInputController
    private readonly battleActionProcessor: IBattleActionsProcessor
    private readonly turnAwaiter: ITurnAwaiter
    private readonly npcControl: boolean

    constructor(
        mapController: IBattleMapController,
        battleActionProcessor: IBattleActionsProcessor,
        turnAwaiter: ITurnAwaiter,
        panelController: IBattlePanelActionsController,
        playerService: IPlayerService) {

        this.mapController = mapController
        this.battleActionProcessor = battleActionProcessor
        this.turnAwaiter = turnAwaiter
        this.playerService = playerService
        this.map = mapController.map
        this.onServerMessageHandlingStarted = turnAwaiter.onServerMessageHandlingStarted

        this.currentPlayerInfo = this.map.players.find(player => player.playerId === this.playerService.currentPlayerInfo.id) ?? null

        this.battleUserInputController = new BattleUserInputController(mapController, panelController)

        this.npcControl = this.isAbleToControlNpc()
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

        await this.turnAwaiter.waitForTurn(this.currentTurnIndex)

        while (!this.battleFinished) {
            try {
                this.onNextTurn.emit(this.map.turnInfo)

                this.currentPlayerInfo = this.map.players.find(player => player.playerId === this.playerService.currentPlayerInfo.id) ?? null

                if (this.isPlayerControlled(this.map.turnInfo.player) && this.canPlayerAct()) {
                    const currentUnit = this.getActiveUnit(this.map.turnInfo.nextTurnUnitId!)
                    if (currentUnit) {
                        await this.processStep(currentUnit)
                    } else {
                        console.error("No active unit found for turn " + this.currentTurnIndex)
                    }
                }

                console.log("Waiting for turn " + (this.currentTurnIndex + 1))
                const turnInfo = await this.turnAwaiter.waitForTurn(this.currentTurnIndex + 1)
                this.map.turnInfo = turnInfo
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
        return player === this.currentPlayerInfo?.playerNumber || this.npcControl
    }

    private canPlayerAct(): boolean {
        return this.currentPlayerInfo?.ransomClaimed !== true && this.currentPlayerInfo?.runClaimed !== true
    }

    private isAbleToControlNpc() : boolean {
        const params = new URLSearchParams(window.location.search);
        const npcControl = params.get("npc");
        return npcControl === "true";
    }

    private onNextRound(): void {
        this.map.units.forEach(unit => {
            unit.currentProps.waited = false
        })
    }

    private getActiveUnit(unitId: string): BattleMapUnit | null {
        return this.map.units.find(unit => unit.id === unitId) ?? null
    }

    private async processStep(unit: BattleMapUnit): Promise<void> {
        await this.mapController.battleMapHighlighter.setActiveUnit(unit)

        // Check if unit is stunned (cannot move but can still attack from current position)
        const isStunned = unit.currentProps.buffs?.some(b => b.stunned) ?? false
        const cellsForMove = isStunned ? [] : getCellsForUnitMove(this.map, unit, unit.currentProps.movementType)
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