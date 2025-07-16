import './App.css';
import React, {PureComponent} from 'react';
import {IServiceLocator} from "./services/serviceLocator";
import {IBattleDefinition} from "./battle/IBattleDefinition";
import {MenuComponent} from "./components/menuComponent";
import {BattleComponent} from "./components/battleComponent";
import {IUserInfo} from "./services/serverAPI";
import { BattleMap } from './battleMap/battleMap';

export interface IAppProps {
    serviceLocator: IServiceLocator;
}

export interface IAppState {
    userInfo: IUserInfo | null
    selectedBattle: BattleMap | null
    isLoading: boolean
}

export class App extends PureComponent<IAppProps, IAppState> {
    private menuComponentRef = React.createRef<MenuComponent>()  // Add this ref
    
    constructor(props: IAppProps) {
        super(props)
        
        this.state = {selectedBattle: null, userInfo: null, isLoading: true}
        
        this.onBattleDefinitionSelected = this.onBattleDefinitionSelected.bind(this)
        this.onBattleFinished = this.onBattleFinished.bind(this)
    }
    async componentDidMount(): Promise<void> {
        const serverAPI = this.props.serviceLocator.serverAPI()
        let userInfo: IUserInfo
        try {
            userInfo = await serverAPI.getUserInfo()
        } catch (error) {
            await serverAPI.login()
            userInfo = await serverAPI.getUserInfo()
        }

        const activeBattle = await serverAPI.getActiveBattle()
        if (activeBattle) {
            this.setState({
                ...this.state,
                selectedBattle: activeBattle,
                userInfo,
                isLoading: false,
            })
        } else {
            this.setState({
                ...this.state,
                userInfo,
                isLoading: false,
            })
        }
    }
    
    private onBattleDefinitionSelected(definition: IBattleDefinition) {
        this.setState({
            ...this.state,
            isLoading: true,
        })

        const battleLoader = this.props.serviceLocator.battleLoader()
        battleLoader.loadBattle(definition).then(map => {
            this.setState({
                ...this.state,
                selectedBattle: map,
                isLoading: false,
            })
        }).catch(() =>
            this.setState({
                ...this.state,
                isLoading: false,
            })
        )


    }
    
    private onBattleFinished() : void {
        this.setState({selectedBattle: null})
        
        // Call the refresh method on MenuComponent
        if (this.menuComponentRef.current) {
            this.menuComponentRef.current.refreshBattles()
        }
    } 

    render() {
        const showMenuComponent = !this.state.isLoading || (!this.state.selectedBattle && this.state.userInfo)
        const showBattleComponent = this.state.selectedBattle
        return (
            <div className="App">
                { showBattleComponent
                    && (
                        <div className="BattleComponent">
                            <BattleComponent serviceLocator={this.props.serviceLocator}
                                             onBattleFinished={this.onBattleFinished}
                                             battleMap={this.state.selectedBattle!} />
                        </div>
                    ) 
                }
                { showMenuComponent
                    && (
                        <div className="MenuComponent">
                            <div>Hello {this.state.userInfo!.userName}</div>
                            <MenuComponent 
                                ref={this.menuComponentRef}  // Add the ref here
                                serviceLocator={this.props.serviceLocator}
                                onBattleSelected={this.onBattleDefinitionSelected}/>
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
