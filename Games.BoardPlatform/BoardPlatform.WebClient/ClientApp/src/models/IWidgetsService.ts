import {IWidget} from "./IWidget";
import {WidgetModel} from "./WidgetModel";
import {
    IWebSocketService,
    WidgetExistMessage,
    WsServerToClientCommand,
    WsServerToClientMessage
} from "../ws-api/IWebSocketService";

export interface IWidgetsService {
    readonly widgets: IWidget[]
}

export class FakeWidgetsService implements IWidgetsService {
    widgets: IWidget[] = [
        new WidgetModel(0, {x: 0, y: 0, width: 100, height: 100}),
        new WidgetModel(1, {x: 30, y: 300, width: 200, height: 400}),
    ]
}

export class WsWidgetsService implements IWidgetsService {
    readonly widgets: IWidget[] = []

    private readonly _wsService: IWebSocketService;

    constructor(wsService: IWebSocketService) {
        this._wsService = wsService;
        this._wsService.newMessage.connect(this.onWsMessage)
    }

    private onWsMessage = (message: WsServerToClientMessage) => {
        if (message.command === WsServerToClientCommand.WidgetExist) {
            const widgetExistMessage: WidgetExistMessage = message
            const widgetData = widgetExistMessage.payload
            
            this.widgets.push(new WidgetModel(widgetData.id, {
                x: widgetData.x,
                y: widgetData.y,
                width: widgetData.width,
                height: widgetData.height
            }))
        }
    }
}