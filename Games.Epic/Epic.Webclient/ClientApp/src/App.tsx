import './App.css';
import React, {PureComponent} from 'react';
import {IServiceLocator} from "./services/serviceLocator";
import {BattleMap, BattleMapCell} from "./battleMap/battleMap";

const CanvasContainerId = 'CanvasContainer'

export interface IAppProps {
    serviceLocator: IServiceLocator;
}

export class App extends PureComponent<IAppProps> {
    async componentDidMount() {
        const container = document.getElementById(CanvasContainerId)!
        const canvasService = this.props.serviceLocator.canvasService()
        await canvasService.init(container)
        
        debugger
        const battleService = this.props.serviceLocator.battleMapService()
        const map = battleService.generateMap(11, 7)
        await battleService.load(map)
    }

    render() {
        return (
            <div className="App">
                <div id={CanvasContainerId}></div>
            </div>
        )
    }
}
export default App;
