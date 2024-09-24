import {BattleUserAction} from "./battleUserAction";
import {IBattleMapController} from "../battleMap/battleMapController";
import {
    BattleServerMessageResponseStatus,
    IBattleCommandToServerResponse,
    IBattleServerConnection
} from "../server/battleServerConnection";
import {getRandomStringKey} from "../units/getRandomString";

export interface IBattleActionsProcessor {
    processAction(action: BattleUserAction): Promise<void>
}

export class BattleActionsProcessor implements IBattleActionsProcessor {
    constructor(private readonly mapController: IBattleMapController,
                private readonly battleServerConnection: IBattleServerConnection) {
    }
    
    async processAction(action: BattleUserAction): Promise<void> {
        let response: IBattleCommandToServerResponse
        if (action.command === 'UNIT_MOVE') {
            response = await this.battleServerConnection.sendMessage(
                {
                    command: 'UNIT_MOVE',
                    actorId: action.actor.id,
                    moveToCell: action.moveToCell,
                    commandId: getRandomStringKey(10),
                    player: action.player,
                })
        } else if (action.command === 'UNIT_ATTACK') {
            response = await this.battleServerConnection.sendMessage(
                {
                    command: 'UNIT_ATTACK',
                    actorId: action.actor.id,
                    targetId: action.attackTarget.id,
                    commandId: getRandomStringKey(10),
                    player: action.player,
                    moveToCell: action.moveToCell,
                })
        } else {
            throw new Error("Unknown type of user action")
        }
        
        if (response.status === BattleServerMessageResponseStatus.Rejected) {
            throw new Error("Command rejected. Reason: " + response.rejectedReason + response.rejectedReasonDetails 
                ? (". Details: " + response.rejectedReasonDetails) : ""
            )
        }
    }
}