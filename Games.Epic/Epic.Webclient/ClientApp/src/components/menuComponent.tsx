import React, {PureComponent} from "react";
import {IBattleDefinition} from "../battle/IBattleDefinition";
import {IServiceLocator} from "../services/serviceLocator";
import {IPlayerInfo, IUserUnit, IResourceInfo} from "../services/serverAPI";
import {ArmyDisplay} from "./armyDisplay";
import {SupplyComponent} from "./supplyComponent";
import {ResourcesView} from "./resourcesView";
import "./menuComponent.css";

export interface IMenuComponentProps {
    serviceLocator: IServiceLocator
    onBattleSelected(definition: IBattleDefinition): void
    playerInfo: IPlayerInfo | null
}

interface IMenuComponentState {
    availableBattles: IBattleDefinition[] | null 
    isLoading: boolean
    battlesGenerationInProgress: boolean
    showSupply: boolean
    armyUnits: IUserUnit[] | null
    armyCapacity: number
    resources: IResourceInfo[] | null
}

export class MenuComponent extends PureComponent<IMenuComponentProps, IMenuComponentState> {
    private retryTimeoutId: number | null = null;
    private readonly RETRY_DELAY_MS = 2000; // 2 seconds
    private armyDisplayRef: React.RefObject<ArmyDisplay> = React.createRef();
    
    constructor(props: IMenuComponentProps) {
        super(props)
        
        this.state = {
            availableBattles: null,
            isLoading: true,
            battlesGenerationInProgress: false,
            showSupply: false,
            armyUnits: null,
            armyCapacity: 7,
            resources: null
        }
    }
    
    async componentDidMount() {
        await Promise.all([
            this.fetchBattles(),
            this.fetchArmyUnits(),
            this.fetchResources()
        ])
    }

    public setBattlesGenerationInProgress(battlesGenerationInProgress: boolean) {
        this.setState({
            ...this.state,
            battlesGenerationInProgress
        })
    }
    
    componentWillUnmount() {
        // Clear any pending retry timeout
        if (this.retryTimeoutId) {
            clearTimeout(this.retryTimeoutId)
            this.retryTimeoutId = null
        }
    }
    
    // Expose this method so parent can call it
    public async refreshBattles() {
        await this.fetchBattles()
    }

    // Method to fetch army units
    private async fetchArmyUnits() {
        try {
            const serverAPI = this.props.serviceLocator.serverAPI()
            if (!this.props.playerInfo?.armyContainerId) {
                throw new Error('No army container ID available')
            }
            const armyContainer = await serverAPI.getUnitsContainer(this.props.playerInfo.armyContainerId)
            this.setState({ 
                armyUnits: armyContainer.units, 
                armyCapacity: armyContainer.capacity
            })
        } catch (error) {
            console.error('Failed to fetch army units:', error)
        }
    }

    // Method to refresh army display
    public async refreshArmy() {
        await this.fetchArmyUnits()
    }

    // Method to refresh resources display
    public async refreshResources() {
        await this.fetchResources()
    }

    // Method to check if there are any army units
    private hasArmyUnits(): boolean {
        const { armyUnits } = this.state
        return armyUnits !== null && armyUnits.length > 0
    }

    private handleSupplyClick = () => {
        this.setState({ showSupply: true })
    }

    private handleSupplyClose = () => {
        this.setState({ showSupply: false })
    }

    private handleArmyUnitsUpdate = (units: IUserUnit[]) => {
        this.setState({ armyUnits: units })
    }

    // Method to refresh army units from server
    private refreshArmyFromServer = async () => {
        await this.fetchArmyUnits()
    }
    
    private async fetchBattles() {
        this.setState({ isLoading: true })
        try {
            const battlesProvider = this.props.serviceLocator.battlesProvider()
            const battles = await battlesProvider.fetchBattles()
            this.setState({availableBattles: battles, isLoading: false})
        } catch (error) {
            console.error('Failed to fetch battles:', error)
            
            // Retry after delay
            this.retryTimeoutId = window.setTimeout(() => {
                this.fetchBattles()
            }, this.RETRY_DELAY_MS)
        }
    }

    // Method to fetch resources
    private async fetchResources() {
        try {
            const serverAPI = this.props.serviceLocator.serverAPI()
            const resources = await serverAPI.getResources()
            this.setState({ resources })
        } catch (error) {
            console.error('Failed to fetch resources:', error)
        }
    }
    
    render() {
        return (
            <div className="menu-container">
                <div className="battles-section">
                    <h1 className="battles-title">Select a Battle</h1>
                    {
                        this.state.isLoading ? (
                            <div className="loading">Loading battles...</div>
                        ) : this.state.availableBattles ? (
                            <div className="battles-table-container">
                                <table className="battles-table">
                                    <thead>
                                        <tr>
                                            <th>Size</th>
                                            <th>Enemy Units</th>
                                            <th>Rewards</th>
                                            <th>Expires In</th>
                                            <th>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {this.state.availableBattles.map((battle, index) => (
                                            <tr key={index} className="battle-row">
                                                <td className="battle-size">{battle.width}x{battle.height}</td>
                                                <td className="battle-units">
                                                    <div className="units-horizontal">
                                                        {battle.units.map((unit, unitIndex) => (
                                                            <div key={unitIndex} className="unit-horizontal-item">
                                                                <img 
                                                                    src={unit.thumbnailUrl} 
                                                                    alt={unit.name}
                                                                    className="unit-thumbnail"
                                                                />
                                                                <span className="unit-count">x{unit.count}</span>
                                                            </div>
                                                        ))}
                                                    </div>
                                                </td>
                                                <td className="battle-rewards">
                                                    <div className="rewards-horizontal">
                                                        {battle.rewards.map((reward, rewardIndex) => (
                                                            <div key={rewardIndex} className="reward-horizontal-item">
                                                                <img 
                                                                    src={reward.thumbnailUrl} 
                                                                    alt={reward.name}
                                                                    className="reward-thumbnail"
                                                                />
                                                                <span className="reward-amount">{reward.amount}</span>
                                                            </div>
                                                        ))}
                                                    </div>
                                                </td>
                                                <td className="battle-expires">
                                                    {battle.expiresAfterDays === 0 ? (
                                                        <span className="expires-soon">Expires today</span>
                                                    ) : battle.expiresAfterDays === 1 ? (
                                                        <span className="expires-soon">1</span>
                                                    ) : (
                                                        <span className="expires-days">{battle.expiresAfterDays} days</span>
                                                    )}
                                                </td>
                                                <td className="battle-action">
                                                    <button 
                                                        className="battle-button"
                                                        onClick={() => this.props.onBattleSelected(battle)}
                                                        disabled={!this.hasArmyUnits()}
                                                    >
                                                        Start Battle
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        ) : (
                            <div className="no-battles">No battles available</div>
                        )
                    }
                    {this.state.battlesGenerationInProgress && (
                        <div className="generation-progress">Battles generation in progress...</div>
                    )}
                </div>
                
                <ArmyDisplay 
                    ref={this.armyDisplayRef}
                    serviceLocator={this.props.serviceLocator}
                    playerInfo={this.props.playerInfo}
                    onSupplyClick={this.handleSupplyClick}
                    armyUnits={this.state.armyUnits}
                    armyCapacity={this.state.armyCapacity}
                    onArmyUnitsUpdate={this.handleArmyUnitsUpdate}
                />

                {this.state.showSupply && (
                    <SupplyComponent
                        serviceLocator={this.props.serviceLocator}
                        playerInfo={this.props.playerInfo}
                        onClose={this.handleSupplyClose}
                        armyUnits={this.state.armyUnits}
                        armyCapacity={this.state.armyCapacity}
                        onArmyUnitsUpdate={this.handleArmyUnitsUpdate}
                        onRefreshArmy={this.refreshArmyFromServer}
                    />
                )}

                {/* Resources Display */}
                <ResourcesView resources={this.state.resources} />
            </div>
        )
    }
}