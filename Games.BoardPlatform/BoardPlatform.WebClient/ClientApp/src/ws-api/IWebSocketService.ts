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
            console.log('On Open!!!' + ev)
        }
        this.ws.onerror = (ev: Event) => {
            console.log('On Error!!!' + ev)
        }
        this.ws.onclose = (ev: CloseEvent) => {
            console.log('Closed!!!')
        }
        this.ws.onmessage = (ev: MessageEvent) => {
            console.log('Hi!')
            const message: WsServerToClientMessage = JSON.parse(ev.data)
            if (message.command && message.messageId) {
                this.newMessage.emit(message)
                console.log("MESSEGE!!!! " + JSON.stringify(message))
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



