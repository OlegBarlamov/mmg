import {BattleCommandToServer} from "./battleCommandToServer";
import {BattleCommandFromServer} from "./battleCommandFromServer";
import {IBattleCommandToServerResponse} from "./IBattleCommandToServerResponse";

export interface IBattleServerConnection {
    sendMessage(message: BattleCommandToServer): Promise<IBattleCommandToServerResponse>
    close(): Promise<void>
}

export interface IBattleConnectionMessagesHandler {
    onMessage(message: BattleCommandFromServer): Promise<void>
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

export class BattleServerConnection implements IBattleServerConnection {
    private isOpened: boolean = true
    private readonly pendingResponseCommands: {[commandId: string]: (response: IBattleCommandToServerResponse) => void} = {}
    
    constructor(private readonly webSocket: WebSocket, private readonly handler: IBattleConnectionMessagesHandler) {
        this.webSocket.onmessage = (ev) => this.onMessage(ev)
    }

    sendMessage(message: BattleCommandToServer): Promise<IBattleCommandToServerResponse> {
        if (!this.isOpened) {
            throw new Error("The WS connection is closed.")
        }
        
        return new Promise<IBattleCommandToServerResponse>((resolve) => {
            this.pendingResponseCommands[message.commandId] = (response: IBattleCommandToServerResponse) => {
                delete this.pendingResponseCommands[message.commandId]
                console.info('Response: ' + response.status)
                resolve(response)
            }
            this.webSocket.send(JSON.stringify(message))
            console.info('Message sent: ' + message.command)
        })
    }
    close(): Promise<void> {
        if (!this.isOpened) {
            return Promise.resolve();
        }

        this.isOpened = false
        this.webSocket.onmessage = null
        return new Promise<void>(resolve => {
            this.webSocket.onclose = () => {
                this.webSocket.onclose = null
                resolve()
            }
            this.webSocket.close();
        })
    }
    
    private onMessage(ev: MessageEvent) {
        try {
            const data = JSON.parse(ev.data)

            if (data.requestedCommand) {
                const response = data as IBattleCommandToServerResponse
                const requestedCommandId = response.requestedCommand.commandId
                if (!requestedCommandId) {
                    console.warn("Invalid response message received:", data)
                    return
                }
                const pendingRequestHandler = this.pendingResponseCommands[requestedCommandId]
                if (!pendingRequestHandler) {
                    console.warn("Response to unknown command received:", data)
                    return
                }
                
                pendingRequestHandler(response)
                return
            }
            
            if (!data.command) {
                console.warn("Invalid message received:", data)
                return
            }
            
            const message = data as BattleCommandFromServer
            console.info('Message received: ' + message.command)
            
            this.handler.onMessage(message).catch((err) => {
                console.error("Error handling message:", err)
            })
        } catch (err) {
            console.error("Failed to parse WebSocket message", err)
        }
    }
}

