import React, {PureComponent} from "react";
import {IBattleDefinition} from "../battle/IBattleDefinition";
import {IServiceLocator} from "../services/serviceLocator";
import {IPlayerInfo, IUserUnit, IResourceInfo} from "../services/serverAPI";
import {IRewardToAccept} from "../rewards/IRewardToAccept";
import {RewardManager} from "../services/rewardManager";
import {ArmyDisplay} from "./armyDisplay";
import {SupplyComponent} from "./supplyComponent";
import {ResourcesView} from "./resourcesView";
import {BattleConfirmationDialog} from "./battleConfirmationDialog";
import {PlayerBattleModal} from "./playerBattleModal";
import {RewardDialog} from "./rewardDialog";
import "./menuComponent.css";

export interface IMenuComponentProps {
    serviceLocator: IServiceLocator
    onBattleSelected(definition: IBattleDefinition): void
    onPlayerBattleSelected(battleMap: any): void
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
    hoveredBattleHeight: number | null
    showBattleConfirmation: boolean
    pendingBattle: IBattleDefinition | null
    showPlayerBattleModal: boolean
    isPlayerBattleLoading: boolean
    rewards: IRewardToAccept[] | null
    currentReward: IRewardToAccept | null
    currentRewardIndex: number
    tooltip: {
        visible: boolean
        text: string
        x: number
        y: number
    }
}

export class MenuComponent extends PureComponent<IMenuComponentProps, IMenuComponentState> {
    private retryTimeoutId: number | null = null;
    private readonly RETRY_DELAY_MS = 2000; // 2 seconds
    private armyDisplayRef: React.RefObject<ArmyDisplay | null> = React.createRef();
    private rewardManager: RewardManager;
    
    constructor(props: IMenuComponentProps) {
        super(props)
        
        this.state = {
            availableBattles: null,
            isLoading: true,
            battlesGenerationInProgress: false,
            showSupply: false,
            armyUnits: null,
            armyCapacity: 3,
            resources: null,
            hoveredBattleHeight: null,
            showBattleConfirmation: false,
            pendingBattle: null,
            showPlayerBattleModal: false,
            isPlayerBattleLoading: false,
            rewards: null,
            currentReward: null,
            currentRewardIndex: 0,
            tooltip: {
                visible: false,
                text: '',
                x: 0,
                y: 0
            }
        }

        // Initialize reward manager with callbacks
        this.rewardManager = new RewardManager(
            this.props.serviceLocator.serverAPI(),
            {
                onRewardComplete: this.handleRewardComplete,
                onGuardBattleBegins: this.handleGuardBattleBegins,
                onRewardError: this.handleRewardError
            }
        );
    }
    
    async componentDidMount() {
        await Promise.all([
            this.fetchBattles(),
            this.fetchArmyUnits(),
            this.fetchResources()
        ])
        
        // Check for unaccepted rewards after initial load
        const rewardState = await this.rewardManager.checkForRewards()
        this.setState({
            rewards: rewardState.rewards,
            currentReward: rewardState.currentReward,
            currentRewardIndex: rewardState.currentRewardIndex
        })
    }

    private handleRewardComplete = () => {
        // Clear reward state when all rewards are complete
        this.setState({
            rewards: null,
            currentReward: null,
            currentRewardIndex: 0
        })
    }

    private handleGuardBattleBegins = (battleMap: any) => {
        this.props.onPlayerBattleSelected(battleMap)
    }

    private handleRewardError = (error: Error) => {
        console.error('Reward error:', error)
    }

    private handleRewardAccept = async (affectedSlots: number[]) => {
        await this.rewardManager.acceptReward(affectedSlots)
        const state = this.rewardManager.getState()
        this.setState({
            rewards: state.rewards,
            currentReward: state.currentReward,
            currentRewardIndex: state.currentRewardIndex
        })
    }

    private handleRewardDecline = async () => {
        await this.rewardManager.declineReward()
        const state = this.rewardManager.getState()
        this.setState({
            rewards: state.rewards,
            currentReward: state.currentReward,
            currentRewardIndex: state.currentRewardIndex
        })
    }

    private handleBeginGuardBattle = async (rewardId: string) => {
        await this.rewardManager.beginGuardBattle(rewardId)
    }

    private handleRewardMouseEnter = (event: React.MouseEvent, description: string) => {
        const rect = event.currentTarget.getBoundingClientRect()
        this.setState({
            tooltip: {
                visible: true,
                text: description,
                x: rect.left + rect.width / 2,
                y: rect.top - 60
            }
        })
    }

    private handleRewardMouseLeave = () => {
        this.setState({
            tooltip: {
                visible: false,
                text: '',
                x: 0,
                y: 0
            }
        })
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

    private handleBattleRowHover = (battleHeight: number | null) => {
        this.setState({ hoveredBattleHeight: battleHeight })
    }

    private handleBattleSelected = (battle: IBattleDefinition) => {
        // Check if there are units outside the battle height
        const unitsOutsideBattleHeight = this.hasUnitsOutsideBattleHeight(battle.height)
        
        if (unitsOutsideBattleHeight) {
            // Show confirmation dialog
            this.setState({
                showBattleConfirmation: true,
                pendingBattle: battle
            })
        } else {
            // Start battle immediately
            this.props.onBattleSelected(battle)
        }
    }

    private hasUnitsOutsideBattleHeight = (battleHeight: number): boolean => {
        const { armyUnits } = this.state
        if (!armyUnits) return false
        
        return armyUnits.some(unit => unit.slotIndex >= battleHeight)
    }

    private handleBattleConfirmationAccept = () => {
        if (this.state.pendingBattle) {
            this.props.onBattleSelected(this.state.pendingBattle)
        }
        this.setState({
            showBattleConfirmation: false,
            pendingBattle: null
        })
    }

    private handleBattleConfirmationCancel = () => {
        this.setState({
            showBattleConfirmation: false,
            pendingBattle: null
        })
    }

    private handlePlayerBattleClick = () => {
        this.setState({ showPlayerBattleModal: true })
    }

    private handlePlayerBattleClose = () => {
        this.setState({ 
            showPlayerBattleModal: false,
            isPlayerBattleLoading: false
        })
    }

    private handlePlayerBattleAccept = async (playerName: string) => {
        this.setState({ isPlayerBattleLoading: true })
        
        try {
            const serverAPI = this.props.serviceLocator.serverAPI()
            const battleMap = await serverAPI.beginBattleWithPlayer(playerName)
            
            // Close the modal
            this.setState({ 
                showPlayerBattleModal: false,
                isPlayerBattleLoading: false
            })
            
            // Start the battle
            this.props.onPlayerBattleSelected(battleMap)
        } catch (error) {
            console.error('Failed to start battle with player:', error)
            // Error will be handled by the modal component
            this.setState({ isPlayerBattleLoading: false })
            throw error
        }
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
                                            <tr 
                                                key={index} 
                                                className="battle-row"
                                                onMouseEnter={() => this.handleBattleRowHover(battle.height)}
                                                onMouseLeave={() => this.handleBattleRowHover(null)}
                                            >
                                                <td className="battle-size">{battle.width}x{battle.height}</td>
                                                <td className="menu-battle-units">
                                                    <div className="menu-units-horizontal">
                                                        {battle.units.map((unit, unitIndex) => (
                                                            <div key={unitIndex} className="menu-unit-horizontal-item">
                                                                <img 
                                                                    src={unit.thumbnailUrl} 
                                                                    alt={unit.name}
                                                                    className="unit-thumbnail"
                                                                />
                                                                {unit.count ? (
                                                                    <span className="menu-unit-count">{unit.count}</span>
                                                                ) : (
                                                                    <span className="menu-unit-count">?</span>
                                                                )}
                                                            </div>
                                                        ))}
                                                    </div>
                                                </td>
                                                <td className="menu-battle-rewards">
                                                    <div className="rewards-horizontal">
                                                        {battle.rewards.map((reward, rewardIndex) => (
                                                            <div 
                                                                key={rewardIndex} 
                                                                className="reward-horizontal-item"
                                                                onMouseEnter={(e) => this.handleRewardMouseEnter(e, reward.description)}
                                                                onMouseLeave={this.handleRewardMouseLeave}
                                                            >
                                                                <img 
                                                                    src={reward.thumbnailUrl} 
                                                                    alt={reward.name}
                                                                    className="reward-thumbnail"
                                                                />
                                                                {reward.amount && (
                                                                    <span className="reward-amount">{reward.amount}</span>
                                                                )}
                                                            </div>
                                                        ))}
                                                    </div>
                                                </td>
                                                <td className="battle-expires">
                                                    {battle.expiresAfterDays === 0 ? (
                                                        <span className="expires-soon">0</span>
                                                    ) : battle.expiresAfterDays === 1 ? (
                                                        <span className="expires-soon">1 day</span>
                                                    ) : battle.expiresAfterDays < 9 ? (
                                                        <span className="expires-days">{battle.expiresAfterDays} days</span>
                                                    ) : (
                                                        <span className="expires-not-soon-days">{battle.expiresAfterDays} days</span>
                                                    )}
                                                </td>
                                                <td className="battle-action">
                                                    <button 
                                                        className="battle-button"
                                                        onClick={() => this.handleBattleSelected(battle)}
                                                        disabled={!this.hasArmyUnits()}
                                                    >
                                                        Start Battle
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                                
                                <div className="battle-actions">
                                    <button 
                                        className="player-battle-button"
                                        onClick={this.handlePlayerBattleClick}
                                        disabled={!this.hasArmyUnits()}
                                    >
                                        Battle with Player
                                    </button>
                                </div>
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
                    highlightedSlots={this.state.hoveredBattleHeight}
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

                {/* Battle Confirmation Dialog */}
                <BattleConfirmationDialog
                    isVisible={this.state.showBattleConfirmation}
                    onAccept={this.handleBattleConfirmationAccept}
                    onCancel={this.handleBattleConfirmationCancel}
                />

                {/* Player Battle Modal */}
                <PlayerBattleModal
                    isVisible={this.state.showPlayerBattleModal}
                    onCancel={this.handlePlayerBattleClose}
                    onAccept={this.handlePlayerBattleAccept}
                    isLoading={this.state.isPlayerBattleLoading}
                />

                {/* Reward Dialog */}
                {this.state.currentReward && (
                    <RewardDialog
                        reward={this.state.currentReward}
                        onAccept={this.handleRewardAccept}
                        onDecline={this.handleRewardDecline}
                        onBeginGuardBattle={this.handleBeginGuardBattle}
                        serviceLocator={this.props.serviceLocator}
                    />
                )}

                {/* Custom Tooltip */}
                {this.state.tooltip.visible && (
                    <div 
                        className="custom-tooltip"
                        style={{
                            left: this.state.tooltip.x,
                            top: this.state.tooltip.y,
                            transform: 'translateX(-50%)'
                        }}
                    >
                        {this.state.tooltip.text}
                    </div>
                )}
            </div>
        )
    }
}