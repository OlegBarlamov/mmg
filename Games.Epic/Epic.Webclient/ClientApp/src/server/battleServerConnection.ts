import {BattleCommandToServer} from "./battleCommandToServer";
import {BattleCommandFromServer} from "./battleCommandFromServer";

export interface IBattleServerConnection {
    sendMessage(message: BattleCommandToServer): Promise<IBattleCommandToServerResponse>
    close(): Promise<void>
}

export interface IBattleConnectionMessagesHandler {
    onMessage(message: BattleCommandFromServer): Promise<void>
}

export interface IBattleCommandToServerResponse {
    requestedCommand: BattleCommandToServer
    status: BattleServerMessageResponseStatus
    rejectedReason: BattleServerMessageRejectionReason | undefined
    rejectedReasonDetails: string | undefined
}

export enum BattleServerMessageResponseStatus {
    Approved = 'Approved',
    Rejected = 'Rejected',
}

export enum BattleServerMessageRejectionReason {
    RulesViolations = 'RulesViolations',
    WrongStepOrder = 'WrongStepOrder',
    UnknownCommand = 'UnknownCommand',
    InvalidCommand = 'InvalidCommand',
    BattleNotFound = 'BattleNotFound',
    Other = 'Other',
}

