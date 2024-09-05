import './App.css';
import React, {PureComponent} from 'react';
import {IServiceLocator} from "./services/serviceLocator";
import {IBattleDefinition} from "./battle/battleDefinition";
import {MenuComponent} from "./components/menuComponent";
import {BattleComponent} from "./components/battleComponent";

export interface IAppProps {
    serviceLocator: IServiceLocator;
}

export interface IAppState {
    selectedBattle: IBattleDefinition | null
}

export class App extends PureComponent<IAppProps, IAppState> {
    constructor(props: IAppProps) {
        super(props)
        
        this.state = {selectedBattle: null}
        
        this.onBattleSelected = this.onBattleSelected.bind(this)
    }
    
    private onBattleSelected(definition: IBattleDefinition) {
        this.setState({selectedBattle: definition})
    }

    render() {
        return (
            <div className="App">
                {this.state.selectedBattle
                    ? (
                        <div className="BattleComponent">
                            <BattleComponent serviceLocator={this.props.serviceLocator}
                                             battleDefinition={this.state.selectedBattle!}/>
                        </div>
                    )
                    : (
                        <div className="MenuComponent">
                            <MenuComponent serviceLocator={this.props.serviceLocator}
                                           onBattleSelected={this.onBattleSelected}/>
                        </div>
                    )
                }
            </div>
        )
    }
}

export default App;
