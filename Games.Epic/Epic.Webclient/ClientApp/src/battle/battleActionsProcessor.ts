import {BattleUserAction} from "./battleUserAction";
import {IBattleMapController} from "../battleMap/battleMapController";
import {BattleMapUnit} from "../battleMap/battleMapUnit";

export interface IBattleActionsProcessor {
    processAction(action: BattleUserAction): Promise<BattleMapUnit | null>
}

export class BattleActionsProcessor implements IBattleActionsProcessor {
    constructor(private mapController: IBattleMapController) {
    }
    
    async processAction(action: BattleUserAction): Promise<BattleMapUnit | null> {
        if (action.command === 'UNIT_MOVE') {
            const cell = action.moveToCell
            await this.mapController.moveUnit(action.actor, cell.r, cell.c)
        } else if (action.command === 'UNIT_ATTACK') {
            const cell = action.moveToCell
            await this.mapController.moveUnit(action.actor, cell.r, cell.c)
            return await this.processAttack(action.actor, action.attackTarget)
        } else {
            throw new Error("Unknown type of user action")
        }
        
        return null
    }

    private async processAttack(actor: BattleMapUnit, target: BattleMapUnit): Promise<BattleMapUnit | null> {
        const damage = actor.props.damage * actor.unitsCount
        const eliminated = await this.mapController.unitTakeDamage(target, damage)
        if (eliminated) {
            return target
        }
        return null
    }
}