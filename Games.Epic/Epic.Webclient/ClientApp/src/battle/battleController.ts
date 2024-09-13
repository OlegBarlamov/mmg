import {IBattleMapController} from "../battleMap/battleMapController";
import {BattleMapUnit} from "../battleMap/battleMapUnit";
import {BattleMap} from "../battleMap/battleMap";
import {BattleUserAction} from "./battleUserAction";
import {wait} from "../common/wait";
import {PlayerNumber} from "../player/playerNumber";
import {BattleUserInputController} from "./battleUserInputController";
import {getAttackTargets, getCellsForUnitMove, getWinner} from "./battleLogic";
import {BattleActionsProcessor, IBattleActionsProcessor} from "./battleActionsProcessor";

export interface IBattleController {
    startBattle(): Promise<PlayerNumber>
    dispose(): void
}

export class BattleController implements IBattleController {
    mapController: IBattleMapController

    private battleStarted: boolean = false
    private battleFinished: boolean = false
    private currentStepUnitIndex: number = -1
    private winnerPlayer: PlayerNumber | null = null
    
    private readonly map: BattleMap
    private readonly orderedUnits: BattleMapUnit[]
    
    private readonly battleUserInputController: BattleUserInputController
    private readonly battleActionProcessor: IBattleActionsProcessor
    
    constructor(mapController: IBattleMapController) {
        this.mapController = mapController

        this.orderedUnits = [...this.mapController.map.units]
            .sort((a, b) => b.props.speed - a.props.speed)
        this.map = mapController.map
        
        this.battleUserInputController = new BattleUserInputController(mapController)
        this.battleActionProcessor = new BattleActionsProcessor(mapController)
    }

    dispose(): void {
        this.orderedUnits.splice(0)
        this.mapController.destroy()
    }

    async startBattle(): Promise<PlayerNumber> {
        if (this.battleStarted) throw new Error("The battle already started");
        this.battleStarted = true
        this.currentStepUnitIndex = 0
        
        while (!this.battleFinished) {
            if (this.currentStepUnitIndex >= this.orderedUnits.length) {
                this.currentStepUnitIndex = 0
            }

            try {
                const currentUnit = this.orderedUnits[this.currentStepUnitIndex]
                await this.processStep(currentUnit)
                this.winnerPlayer = getWinner(this.map)
                this.battleFinished = this.winnerPlayer != null

                this.currentStepUnitIndex++
            } catch (e) {
                console.error('Error while processing the battle action: ' + e)
            }
        }
        
        await wait(1000 * 3)
        
        return this.winnerPlayer!
    }

    private async processStep(unit: BattleMapUnit): Promise<void> {
        await this.mapController.battleMapHighlighter.setActiveUnit(unit)
        
        const cellsForMove = getCellsForUnitMove(this.map, unit)
        const reachableCells = [this.map.grid.getCell(unit.position.r, unit.position.c), ...cellsForMove]
        const attackTargets = getAttackTargets(this.map, unit, reachableCells)
        
        this.mapController.battleMapHighlighter.highlightCellsForMove(cellsForMove)
        this.mapController.battleMapHighlighter.highlightAttackTargets(attackTargets)
        
        let action: BattleUserAction
        try {
            action = await this.battleUserInputController.getUserInputAction(unit, cellsForMove, attackTargets)
        } finally {
            this.mapController.battleMapHighlighter.restoreHighlightingForCells(cellsForMove)
            this.mapController.battleMapHighlighter.restoreHighlightingForAttackTargets(attackTargets)
            await this.mapController.battleMapHighlighter.setActiveUnit(null)
        }
        
        const eliminatedUnit = await this.battleActionProcessor.processAction(action)
        if (eliminatedUnit) {
            // TODO temporary
            this.orderedUnits.splice(this.orderedUnits.indexOf(eliminatedUnit), 1)
        }
        
        await wait(500)
    }
}