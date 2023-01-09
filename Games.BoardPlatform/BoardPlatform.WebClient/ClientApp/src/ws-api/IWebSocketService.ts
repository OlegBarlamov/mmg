import {Signal} from "typed-signals";
import {IWidget} from "../models/IWidget";

export interface IWebSocketService {
    newMessage: Signal<(message: WsServerToClientMessage) => void>
    
    connect(url: string): void
    
    disconnect(): void
    
    sendMessage(message: WsClientToServerMessage): void
    
    getStatus(): WebSocketStatus
}

export enum WebSocketStatus {
    Disconnected,
    Connecting,
    Connected,
}

export class WebSocketServiceImpl implements IWebSocketService{
    newMessage: Signal<(message:WsServerToClientMessage) => void> = new Signal<(message:WsServerToClientMessage) => void>()
    
    private ws: WebSocket | undefined
    private status: WebSocketStatus = WebSocketStatus.Disconnected
    
    connect(url: string) {
        if (this.status != WebSocketStatus.Disconnected) {
            return
        }
        
        this.ws = new WebSocket(url)
        console.log('Connecting!')
        
        this.status = WebSocketStatus.Connecting
        this.ws.onopen = (ev: Event) => {
            console.log('On Open!!!' + ev)
            this.status = WebSocketStatus.Connected
        }
        this.ws.onerror = (ev: Event) => {
            console.log('On Error!!!' + ev)
            this.status = WebSocketStatus.Disconnected
        }
        this.ws.onclose = (ev: CloseEvent) => {
            console.log('Closed!!!')
            this.status = WebSocketStatus.Disconnected
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

    getStatus(): WebSocketStatus {
        return this.status
    }

    disconnect(): void {
        this.ws?.close()
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
    messageId: number
    command: WsClientToServerCommand
    payload: any
}

export enum WsServerToClientCommand {
    None = 0,
    ConnectedHandshake = 1,
    WidgetExist = 2,
}

export enum WsClientToServerCommand {
    None = 0,
    ConnectedHandshake = 1,
}



