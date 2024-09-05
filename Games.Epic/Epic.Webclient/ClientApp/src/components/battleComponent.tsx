import './battleComponent.css'
import React, {PureComponent} from "react";
import {IBattleDefinition} from "../battle/battleDefinition";
import {IServiceLocator} from "../services/serviceLocator";
import {IBattleMapController} from "../battleMap/battleMapController";

const CanvasContainerId = 'CanvasContainer'

export interface IBattleComponentProps {
    serviceLocator: IServiceLocator
    battleDefinition: IBattleDefinition
}

interface IBattleComponentState {
    battleLoaded: boolean
}

export class BattleComponent extends PureComponent<IBattleComponentProps, IBattleComponentState> {
    private mapController: IBattleMapController | null = null
    
    constructor(props: IBattleComponentProps) {
        super(props)
        
        this.state = {battleLoaded: false}
    }
    
    async componentDidMount() {
        const canvasService = this.props.serviceLocator.canvasService()
        await canvasService.init(document.getElementById(CanvasContainerId)!)

        const battleLoader = this.props.serviceLocator.battleLoader()
        const map = await battleLoader.loadBattle(this.props.battleDefinition)
        const battleService = this.props.serviceLocator.battleMapService()
        this.mapController = await battleService.load(map)
        
        this.setState({...this.state, battleLoaded: true})
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