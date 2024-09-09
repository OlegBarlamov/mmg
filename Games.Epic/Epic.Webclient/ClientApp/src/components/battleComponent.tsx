import './battleComponent.css'
import React, {PureComponent} from "react";
import {IBattleDefinition} from "../battle/battleDefinition";
import {IServiceLocator} from "../services/serviceLocator";
import {IBattleMapController} from "../battleMap/battleMapController";
import {IBattleController} from "../battle/battleController";

const CanvasContainerId = 'CanvasContainer'

export interface IBattleComponentProps {
    serviceLocator: IServiceLocator
    battleDefinition: IBattleDefinition
}

interface IBattleComponentState {
    battleLoaded: boolean
}

export class BattleComponent extends PureComponent<IBattleComponentProps, IBattleComponentState> {
    private battleController: IBattleController | null = null
    
    constructor(props: IBattleComponentProps) {
        super(props)
        
        this.state = {battleLoaded: false}
    }
    
    async componentDidMount() {
        const canvasService = this.props.serviceLocator.canvasService()
        await canvasService.init(document.getElementById(CanvasContainerId)!)

        const battleLoader = this.props.serviceLocator.battleLoader()
        const map = await battleLoader.loadBattle(this.props.battleDefinition)
        const battlesService = this.props.serviceLocator.battlesService()
        this.battleController = await battlesService.createBattle(map)
        this.onBattleFinished = this.onBattleFinished.bind(this)
        this.battleController.startBattle().then(() => this.onBattleFinished())
        
        this.setState({...this.state, battleLoaded: true})
    }

    private onBattleFinished() {
        
    }

    render() {
        const canvasStyle: React.CSSProperties = {
            visibility: this.state.battleLoaded ? 'visible' : 'hidden'
        };

        return (
            <div className={"Container"}>
                {(!this.state.battleLoaded) && (
                    <div>Loading...</div>
                )}
                
                <div id={CanvasContainerId} style={canvasStyle}></div>
            </div>
        )
    }
}