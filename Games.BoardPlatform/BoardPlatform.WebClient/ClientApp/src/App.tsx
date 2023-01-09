import React, {useEffect} from 'react';
import './App.css';
import CanvasViewComponent from "./canvas/CanvasViewComponent";
import {DefaultCanvasService} from "./canvas/DefaultCanvasService";
import {WsWidgetsService} from "./models/IWidgetsService";
import {
    WebSocketServiceImpl,
    WsClientToServerCommand,
    WsClientToServerMessage,
    WsServerToClientCommand,
    WsServerToClientMessage
} from "./ws-api/IWebSocketService";

function App() {
    initializeApplication()
    
    const wsService = new WebSocketServiceImpl() 
    const widgetsService = new WsWidgetsService(wsService)
    const canvasService = new DefaultCanvasService(widgetsService)
    
    useEffect(() => {
        wsService.connect('wss://localhost:7124/board/test_board/ws')

        function handShakeAwaiter(message: WsServerToClientMessage): void {
            if (message.command === WsServerToClientCommand.ConnectedHandshake) {
                wsService.newMessage.disconnect(handShakeAwaiter)
                const message : WsClientToServerMessage = {command: WsClientToServerCommand.ConnectedHandshake, messageId: 1, payload: null}
                wsService.sendMessage(message)
            }
        }

        wsService.newMessage.connect(handShakeAwaiter)
        
        return () => {
            wsService.disconnect()
        }
    })
    
    return (
        <div className="App">
            <CanvasViewComponent canvasService={canvasService} widgetsService={widgetsService} />
        </div>
    );
}

function initializeApplication() {
    
}

export default App;
