import './App.css';
import React, {PureComponent} from 'react';
import {IServiceLocator} from "./services/serviceLocator";
import {IBattleDefinition} from "./battle/IBattleDefinition";
import {MenuComponent} from "./components/menuComponent";
import {BattleComponent} from "./components/battleComponent";
import {IPlayerInfo, IUserInfo} from "./services/serverAPI";
import { BattleMap } from './battleMap/battleMap';

export interface IAppProps {
    serviceLocator: IServiceLocator;
}

export interface IAppState {
    userInfo: IUserInfo | null
    playerInfo: IPlayerInfo | null
    selectedBattle: BattleMap | null
    isLoading: boolean
}

export class App extends PureComponent<IAppProps, IAppState> {
    private menuComponentRef = React.createRef<MenuComponent>()  // Add this ref
    
    constructor(props: IAppProps) {
        super(props)
        
        this.state = {selectedBattle: null, userInfo: null, playerInfo: null, isLoading: true}
        
        this.onBattleDefinitionSelected = this.onBattleDefinitionSelected.bind(this)
        this.onBattleFinished = this.onBattleFinished.bind(this)
    }
    async componentDidMount(): Promise<void> {
        const serverAPI = this.props.serviceLocator.serverAPI()
        let userInfo: IUserInfo
        let playerInfo: IPlayerInfo
        try {
            userInfo = await serverAPI.getUserInfo()
        } catch (error) {
            await serverAPI.login()
            userInfo = await serverAPI.getUserInfo()
        }

        if (!userInfo.playerId) {
            const players = await serverAPI.getPlayers()
            playerInfo = players[0]
            await serverAPI.setActivePlayer(playerInfo.id)
        } else {
            playerInfo = await serverAPI.getPlayer(userInfo.playerId)
        }

        const activeBattle = await serverAPI.getActiveBattle()
        if (activeBattle) {
            this.setState({
                ...this.state,
                selectedBattle: activeBattle,
                userInfo,
                playerInfo,
                isLoading: false,
            })
        } else {
            this.setState({
                ...this.state,
                userInfo,
                playerInfo,
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

        this.refreshPlayerInfoAndBattles()
    } 

    private async refreshPlayerInfoAndBattles() {
        const serverAPI = this.props.serviceLocator.serverAPI()
        serverAPI.getPlayer(this.state.playerInfo!.id).then(playerInfo => {
            this.setState({
                ...this.state,
                playerInfo,
            })
        })

        // Call the refresh method on MenuComponent
        if (this.menuComponentRef.current) {
            this.menuComponentRef.current.refreshBattles()
            this.menuComponentRef.current.refreshArmy()
            this.menuComponentRef.current.refreshResources()
        }

        if (this.state.playerInfo!.battlesGenerationInProgress) {
            this.menuComponentRef.current?.setBattlesGenerationInProgress(true)
            setTimeout(() => {
                this.refreshPlayerInfoAndBattles()
            }, 2000)
        } else {
            this.menuComponentRef.current?.setBattlesGenerationInProgress(false)
        }
    }

    private renderUserInfoSection() {
        const { userInfo, playerInfo } = this.state
        
        if (!userInfo || !playerInfo) return null

        return (
            <div className="user-info-section">
                <div className="user-info">
                    <div className="user-avatar">
                        {userInfo.userName.charAt(0).toUpperCase()}
                    </div>
                    <div className="user-name">Hello, {userInfo.userName}</div>
                </div>
                
                <div className="player-info">
                    <div className={`player-badge ${playerInfo.isDefeated ? 'defeated' : ''}`}>
                        <div className="player-icon">P</div>
                        {playerInfo.name}
                    </div>
                    
                    <div className="day-badge">
                        <div className="day-icon">D</div>
                        Day {playerInfo.day}
                    </div>
                    
                    {playerInfo.isDefeated && (
                        <div className="defeated-badge">
                            DEFEATED
                        </div>
                    )}
                </div>
            </div>
        )
    }

    private renderLoadingState() {
        return (
            <div className="loading-container">
                <div>
                    <div className="loading-spinner"></div>
                    <div className="loading-text">Loading...</div>
                </div>
            </div>
        )
    }

    render() {
        const showMenuComponent = !this.state.isLoading || (!this.state.selectedBattle && this.state.userInfo)
        const showBattleComponent = this.state.selectedBattle
        
        if (this.state.isLoading) {
            return this.renderLoadingState()
        }
        
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
                        <>
                            {this.renderUserInfoSection()}
                            <div className="MenuComponent">
                                <MenuComponent 
                                    ref={this.menuComponentRef}
                                    serviceLocator={this.props.serviceLocator}
                                    onBattleSelected={this.onBattleDefinitionSelected}
                                    playerInfo={this.state.playerInfo}
                                />
                            </div>
                        </>
                    )
                }
            </div>
        )
    }
}

export default App;
