import React, {PureComponent, RefObject} from "react";
import * as PIXI from "pixi.js";
import {ICanvasService} from "./ICanvasService";
import {WidgetView} from "./WidgetView";
import {IWidget} from "../models/IWidget";
import './CanvasViewComponent.css'
import {CanvasController} from "./CanvasController";

interface CanvasViewComponentProps {
    canvasService: ICanvasService
    devicePixelRatio: number
}

export default class CanvasViewComponent extends PureComponent<CanvasViewComponentProps> {
    private readonly widgetViews: WidgetView[] = []
    private readonly pixi: PIXI.Application
    private readonly rootCanvasDomElement: RefObject<HTMLDivElement>
    private readonly canvasController: CanvasController
    
    constructor(props: CanvasViewComponentProps) {
        super(props);
        this.rootCanvasDomElement = React.createRef()
        this.pixi = CanvasViewComponent.createPixiApp(props.devicePixelRatio)
        this.canvasController = new CanvasController(this.props.canvasService)
    }

    private get container(): HTMLElement {
        return this.rootCanvasDomElement.current!;
    }

    componentDidMount(): void {
        this.canvasController.initialize(this.container)
        this.container.append(this.pixi.view)
        
        this.addWidgets(this.props.canvasService.widgets)
        
        this.refreshCanvasSize()
        this.container.addEventListener('resize', this.onContainerSizeChanged)
        this.container.addEventListener('contextmenu', e => e.preventDefault())

        this.pixi.ticker.add(() => {
            this.widgetViews.forEach(x => x.update())
        })
    }
    
    componentWillUnmount(): void {
        this.canvasController.destroy()
        
        this.container.removeEventListener('resize', this.onContainerSizeChanged)
        this.pixi.destroy(true, true)
    }
    
    private refreshCanvasSize() {
        const screenWidth = this.container.clientWidth / this.props.devicePixelRatio
        const screenHeight = this.container.clientHeight / this.props.devicePixelRatio
        this.pixi.renderer.resize(screenWidth, screenHeight)
        this.pixi.stage.scale.x = screenWidth / this.props.canvasService.viewport.width
        this.pixi.stage.scale.y = screenHeight / this.props.canvasService.viewport.height
        this.props.canvasService.setScreenSize(
            {
                width: screenWidth,
                height: screenHeight,
            }
        )
        
    }

    private addWidgets(widgets: IWidget[]) {
        const views = widgets.map((x) => new WidgetView(x, this.props.canvasService))
        views.forEach(x => {
            this.widgetViews.push(x)
            this.pixi!.stage.addChild(x.drawableContainer)
        })
    }

    render() {
        return (
            <div ref={this.rootCanvasDomElement} className="CanvasRoot">
                {/*// Canvas placed here*/}
            </div>
        )
    }

    private static createPixiApp(resolution: number): PIXI.Application {
        return new PIXI.Application(
            {
                backgroundAlpha: 1,
                backgroundColor: 0x1099bb,
                antialias: true,
                resolution: resolution,
                width: 0,
                height: 0,
            });
    }

    private onContainerSizeChanged = () => {
        this.refreshCanvasSize()
    }
}