import {IBattleMapController} from "../battleMap/battleMapController";
import {BattleMapUnit} from "../battleMap/battleMapUnit";

export interface IBattleController {
    startBattle(): Promise<void>
}

export class BattleController implements IBattleController {
    mapController: IBattleMapController

    private readonly orderedUnits: BattleMapUnit[]
    
    private battleStarted: boolean = false
    private battleFinished: boolean = false
    private currentStepUnitIndex: number = -1
    
    constructor(mapController: IBattleMapController) {
        this.mapController = mapController

        this.orderedUnits = [...this.mapController.map.units]
            .sort((a, b) => b.props.speed - a.props.speed)
    }

    async startBattle(): Promise<void> {
        if (this.battleStarted) throw new Error("The battle already started");
        this.battleStarted = true
        this.currentStepUnitIndex = -1
        
        while (!this.battleFinished) {
            this.currentStepUnitIndex++
            if (this.currentStepUnitIndex >= this.orderedUnits.length) {
                this.currentStepUnitIndex = 0
            }
            
            const currentUnit = this.orderedUnits[this.currentStepUnitIndex]
            await this.processStep(currentUnit)
        }
    }
    
    private async processStep(unit: BattleMapUnit): Promise<void> {
        await this.mapController.activateUnit(unit)
        
        const unitSpeed = unit.props.speed
        const cellsInRange = this.mapController.getCellsInRange(unit.position.r, unit.position.c, unitSpeed)
        cellsInRange.forEach(x => this.mapController.highlightCell(x.r, x.c))
        // highlight zone
        
        
        // allow user input
        // wait user input
        // process the action
        
        return new Promise((resolve) => {})
    }
}