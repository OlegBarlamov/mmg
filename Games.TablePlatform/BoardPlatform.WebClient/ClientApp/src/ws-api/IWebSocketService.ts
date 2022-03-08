import {Signal} from "typed-signals";
import {IWidget} from "../models/IWidget";

export interface IWebSocketService {
    newMessage: Signal<(message: WsServerToClientMessage) => void>
    
    connect(url: string): void
    
    sendMessage(message: WsClientToServerMessage): void
}

export class WebSocketServiceImpl implements IWebSocketService{
    newMessage: Signal<(message:WsServerToClientMessage) => void> = new Signal<(message:WsServerToClientMessage) => void>()
    
    private ws: WebSocket | undefined
    
    connect(url: string) {
        this.ws = new WebSocket(url)
        this.ws.onopen = (ev: Event) => {
            
        }
        this.ws.onerror = (ev: Event) => {
            
        }
        this.ws.onclose = (ev: CloseEvent) => {
            
        }
        this.ws.onmessage = (ev: MessageEvent) => {
            const message: WsServerToClientMessage = ev.data
            if (message.command && message.messageId) {
                this.newMessage.emit(message)
            }
        }
    }

    sendMessage(message: WsClientToServerMessage) {
        this.ws!.send(JSON.stringify(message))
    }
}

export interface WsServerToClientMessage {
    messageId: bigint
    command: WsServerToClientCommand
    payload: any
}

export interface WidgetExistMessage extends WsServerToClientMessage{
    payload: IWidget  
}

export interface WsClientToServerMessage {
    messageId: bigint
    command: WsClientToServerCommand
}

export enum WsServerToClientCommand {
    None = 0,
    Connected = 1,
    WidgetExist = 2,
}

export enum WsClientToServerCommand {
    None = 0,
}



