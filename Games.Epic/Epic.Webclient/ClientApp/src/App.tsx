import './App.css';
import React, {PureComponent} from 'react';
import {IServiceLocator} from "./services/serviceLocator";
import {IBattleDefinition} from "./battle/battleDefinition";
import {MenuComponent} from "./components/menuComponent";
import {BattleComponent} from "./components/battleComponent";
import {IUserInfo} from "./services/serverAPI";

export interface IAppProps {
    serviceLocator: IServiceLocator;
}

export interface IAppState {
    userInfo: IUserInfo | null
    selectedBattle: IBattleDefinition | null
}

export class App extends PureComponent<IAppProps, IAppState> {
    constructor(props: IAppProps) {
        super(props)
        
        this.state = {selectedBattle: null, userInfo: null}
        
        this.onBattleSelected = this.onBattleSelected.bind(this)
        this.onBattleFinished = this.onBattleFinished.bind(this)
    }
    async componentDidMount(): Promise<void> {
        const serverAPI = this.props.serviceLocator.serverAPI()
        let userInfo: IUserInfo
        try {
            userInfo = await serverAPI.getUserInfo()
        } catch (error) {
            userInfo = await serverAPI.signup("FakeName")
        }
        this.setState({
            ...this.state,
            userInfo,
        })
    }
    
    private onBattleSelected(definition: IBattleDefinition) {
        this.setState({selectedBattle: definition})
    }
    
    private onBattleFinished() {
        this.setState({selectedBattle: null})
    } 

    render() {
        const showMenuComponent = !this.state.selectedBattle && this.state.userInfo
        const showBattleComponent = this.state.selectedBattle
        return (
            <div className="App">
                { showBattleComponent
                    && (
                        <div className="BattleComponent">
                            <BattleComponent serviceLocator={this.props.serviceLocator}
                                             onBattleFinished={this.onBattleFinished}
                                             battleDefinition={this.state.selectedBattle!} />
                        </div>
                    ) 
                }
                { showMenuComponent
                    && (
                        <div className="MenuComponent">
                            <div>Hello {this.state.userInfo!.userName}</div>
                            <MenuComponent serviceLocator={this.props.serviceLocator}
                                           onBattleSelected={this.onBattleSelected}/>
                        </div>
                    )
                }
                {!showMenuComponent && !showBattleComponent && (
                    <div>Loading</div>
                )}
            </div>
        )
    }
}

export default App;
