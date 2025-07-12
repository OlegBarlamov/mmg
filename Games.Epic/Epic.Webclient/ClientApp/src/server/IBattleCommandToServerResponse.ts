import {BattleCommandToServer} from "./battleCommandToServer";
import {BattleServerMessageRejectionReason, BattleServerMessageResponseStatus} from "./battleServerConnection";

export interface IBattleCommandToServerResponse {
    requestedCommand: BattleCommandToServer
    status: BattleServerMessageResponseStatus
    rejectedReason: BattleServerMessageRejectionReason | undefined
    rejectedReasonDetails: string | undefined
}