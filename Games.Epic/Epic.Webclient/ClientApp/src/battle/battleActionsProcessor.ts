import {BattleUserAction} from "./battleUserAction";
import {IBattleMapController} from "../battleMap/battleMapController";
import {
    BattleServerMessageResponseStatus,
    IBattleServerConnection
} from "../server/battleServerConnection";
import {getRandomStringKey} from "../units/getRandomString";
import {IBattleCommandToServerResponse} from "../server/IBattleCommandToServerResponse";

export interface IBattleActionsProcessor {
    onClientConnected(): Promise<void>
    processAction(action: BattleUserAction): Promise<void>
}

export class BattleActionsProcessor implements IBattleActionsProcessor {
    constructor(private readonly mapController: IBattleMapController,
                private readonly battleServerConnection: IBattleServerConnection) {
    }
    async onClientConnected(): Promise<void> {
        await this.battleServerConnection.sendMessage({
            command: 'CLIENT_CONNECTED',
            commandId: getRandomStringKey(10),
            // TODO: send the correct player number
            player: this.mapController.map.turnInfo.player,
            turnIndex: this.mapController.map.turnInfo.index,
        })
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
                    turnIndex: this.mapController.map.turnInfo.index,
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
                    attackIndex: action.attackTypeIndex,
                    turnIndex: this.mapController.map.turnInfo.index,
                })
        } else if (action.command === 'UNIT_PASS') {
            response = await this.battleServerConnection.sendMessage(
                {
                    command: 'UNIT_PASS',
                    actorId: action.actor.id,
                    commandId: getRandomStringKey(10),
                    player: action.player,
                    turnIndex: this.mapController.map.turnInfo.index,
                })
        } else if (action.command === 'UNIT_WAIT') {
            response = await this.battleServerConnection.sendMessage(
                {
                    command: 'UNIT_WAIT',
                    actorId: action.actor.id,
                    commandId: getRandomStringKey(10),
                    player: action.player,
                    turnIndex: this.mapController.map.turnInfo.index,
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