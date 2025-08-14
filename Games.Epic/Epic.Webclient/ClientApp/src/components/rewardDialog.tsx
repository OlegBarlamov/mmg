import './rewardDialog.css'
import React, { PureComponent } from "react";
import { IRewardToAccept } from "../rewards/IRewardToAccept";
import { RewardType } from "../rewards/RewardType";
import { IResourceInfo } from "../services/serverAPI";

export interface IRewardDialogProps {
    reward: IRewardToAccept
    onAccept: (result?: any) => Promise<void>
    onDecline: () => Promise<void>
    serviceLocator?: any
}

interface IRewardDialogState {
    unitQuantities: number[]
    availableResources: { [resourceId: string]: number }
    isSubmitting: boolean
    errorMessage: string | null
}

export class RewardDialog extends PureComponent<IRewardDialogProps, IRewardDialogState> {
    constructor(props: IRewardDialogProps) {
        super(props)
        this.state = { 
            unitQuantities: [],
            availableResources: {},
            isSubmitting: false,
            errorMessage: null
        }
    }

    private handleAccept = async () => {
        this.setState({ isSubmitting: true, errorMessage: null })
        
        try {
            // For UnitToBuy, we need to pass the selected quantities
            if (this.props.reward.rewardType === RewardType.UnitToBuy) {
                // The amounts array should contain the quantities for each unit
                const amounts = this.state.unitQuantities
                // Update the reward amounts before calling onAccept
                this.props.reward.amounts = amounts
            }
            
            const result = await this.props.onAccept()
            
            // Dialog will be closed by parent component when currentReward is set to null
        } catch (error) {
            console.error('Error accepting reward:', error)
            this.setState({ 
                errorMessage: error instanceof Error ? error.message : 'Failed to accept reward. Please try again.',
                isSubmitting: false 
            })
        }
    }

    private handleDecline = async () => {
        this.setState({ isSubmitting: true, errorMessage: null })
        
        try {
            await this.props.onDecline()
            // Dialog will be closed by parent component when currentReward is set to null
        } catch (error) {
            console.error('Error declining reward:', error)
            this.setState({ 
                errorMessage: error instanceof Error ? error.message : 'Failed to decline reward. Please try again.',
                isSubmitting: false 
            })
        }
    }

    componentDidMount() {
        this.initializeUnitBuyState()
    }

    componentDidUpdate(prevProps: IRewardDialogProps) {
        if (prevProps.reward.id !== this.props.reward.id) {
            this.initializeUnitBuyState()
        }
    }

    private initializeUnitBuyState = async () => {
        const { reward, serviceLocator } = this.props
        if (reward.rewardType === RewardType.UnitToBuy) {
            // Initialize unit quantities to 0
            const unitQuantities = new Array(reward.unitsRewards?.length || 0).fill(0)
            
            // Get available resources from the server
            let availableResources: { [resourceId: string]: number } = {}
            
            if (serviceLocator) {
                try {
                    const serverAPI = serviceLocator.serverAPI()
                    const resources: IResourceInfo[] = await serverAPI.getResources()
                    resources.forEach((resource: IResourceInfo) => {
                        availableResources[resource.id] = resource.amount
                    })
                } catch (error) {
                    console.error('Failed to fetch resources:', error)
                    // Fallback to empty resources
                }
            }
            
            this.setState({
                unitQuantities,
                availableResources
            })
        }
    }

    private handleUnitQuantityChange = (unitIndex: number, newQuantity: number) => {
        const { reward } = this.props
        const { unitQuantities } = this.state
        
        // Get the maximum affordable quantity for this unit considering all other units
        const maxAffordable = this.getMaxAffordableQuantityForUnit(unitIndex, newQuantity)
        
        // Limit the quantity to what we can afford
        const limitedQuantity = Math.min(newQuantity, maxAffordable)
        
        const newQuantities = [...unitQuantities]
        newQuantities[unitIndex] = limitedQuantity
        
        this.setState({ unitQuantities: newQuantities })
    }

    private canAffordQuantity = (unitIndex: number, quantity: number): boolean => {
        const { reward } = this.props
        const { unitQuantities, availableResources } = this.state
        
        if (!reward.prices || unitIndex >= reward.prices.length) return false
        
        const unitPrice = reward.prices[unitIndex]
        const currentQuantities = [...unitQuantities]
        currentQuantities[unitIndex] = quantity
        
        // Calculate total cost for all units
        const totalCost: { [resourceId: string]: number } = {}
        
        currentQuantities.forEach((qty, index) => {
            if (qty > 0 && reward.prices && reward.prices[index]) {
                reward.prices[index].resources.forEach(resource => {
                    totalCost[resource.id] = (totalCost[resource.id] || 0) + (resource.amount * qty)
                })
            }
        })
        
        // Check if we have enough resources
        return Object.entries(totalCost).every(([resourceId, cost]) => {
            const available = availableResources[resourceId] || 0
            return available >= cost
        })
    }

    private getRemainingResources = (): { [resourceId: string]: number } => {
        const { reward } = this.props
        const { unitQuantities, availableResources } = this.state
        
        const totalCost: { [resourceId: string]: number } = {}
        
        unitQuantities.forEach((qty, index) => {
            if (qty > 0 && reward.prices && reward.prices[index]) {
                reward.prices[index].resources.forEach(resource => {
                    totalCost[resource.id] = (totalCost[resource.id] || 0) + (resource.amount * qty)
                })
            }
        })
        
        const remaining: { [resourceId: string]: number } = {}
        Object.keys(availableResources).forEach(resourceId => {
            remaining[resourceId] = (availableResources[resourceId] || 0) - (totalCost[resourceId] || 0)
        })
        
        return remaining
    }

    private getTotalCost = (): { [resourceId: string]: number } => {
        const { reward } = this.props
        const { unitQuantities } = this.state
        
        const totalCost: { [resourceId: string]: number } = {}
        
        unitQuantities.forEach((qty, index) => {
            if (qty > 0 && reward.prices && reward.prices[index]) {
                reward.prices[index].resources.forEach(resource => {
                    totalCost[resource.id] = (totalCost[resource.id] || 0) + (resource.amount * qty)
                })
            }
        })
        
        return totalCost
    }

    private getMaxAffordableQuantity = (unitIndex: number): number => {
        const { reward } = this.props
        const { availableResources } = this.state
        
        if (!reward.prices || unitIndex >= reward.prices.length) return 0
        
        const unitPrice = reward.prices[unitIndex]
        const unit = reward.unitsRewards[unitIndex]
        const maxAvailable = unit ? unit.amount : 0
        
        // Find the maximum quantity we can afford based on the most limiting resource
        const resourceLimits: number[] = []
        
        unitPrice.resources.forEach(resource => {
            const available = availableResources[resource.id] || 0
            const maxForThisResource = Math.floor(available / resource.amount)
            resourceLimits.push(maxForThisResource)
        })
        
        let maxAffordable = 0
        if (resourceLimits.length > 0) {
            maxAffordable = Math.min(...resourceLimits)
        }
        
        // Return the minimum of what's available and what we can afford
        return Math.min(maxAvailable, Math.max(0, maxAffordable))
    }

    private getMaxAffordableQuantityForUnit = (unitIndex: number, newQuantity: number): number => {
        const { reward } = this.props
        const { availableResources, unitQuantities } = this.state
        
        if (!reward.prices || unitIndex >= reward.prices.length) return 0
        
        const unitPrice = reward.prices[unitIndex]
        const unit = reward.unitsRewards[unitIndex]
        const maxAvailable = unit ? unit.amount : 0
        
        // Calculate current total cost excluding the unit we're changing
        const currentTotalCost: { [resourceId: string]: number } = {}
        
        unitQuantities.forEach((qty, index) => {
            if (index !== unitIndex && qty > 0 && reward.prices && reward.prices[index]) {
                reward.prices[index].resources.forEach(resource => {
                    currentTotalCost[resource.id] = (currentTotalCost[resource.id] || 0) + (resource.amount * qty)
                })
            }
        })
        
        // Find the maximum quantity we can afford for this specific unit
        const resourceLimits: number[] = []
        
        unitPrice.resources.forEach(resource => {
            const available = availableResources[resource.id] || 0
            const alreadySpent = currentTotalCost[resource.id] || 0
            const remaining = available - alreadySpent
            const maxForThisResource = Math.floor(remaining / resource.amount)
            resourceLimits.push(maxForThisResource)
        })
        
        let maxAffordable = 0
        if (resourceLimits.length > 0) {
            maxAffordable = Math.min(...resourceLimits)
        }
        
        // Return the minimum of what's available and what we can afford
        return Math.min(maxAvailable, Math.max(0, maxAffordable))
    }

    private getMaxAvailableQuantity = (unitIndex: number): number => {
        const { reward } = this.props
        const unit = reward.unitsRewards[unitIndex]
        return unit ? unit.amount : 0
    }

    private getTotalSelectedUnits = (): number => {
        const { unitQuantities } = this.state
        return unitQuantities.reduce((total, quantity) => total + quantity, 0)
    }

    private getUnitCost = (unitIndex: number): Array<{resource: any, amount: number}> | null => {
        const { reward } = this.props
        const { unitQuantities } = this.state
        
        if (!reward.prices || unitIndex >= reward.prices.length) return null
        
        const quantity = unitQuantities[unitIndex] || 0
        if (quantity === 0) return null
        
        const unitPrice = reward.prices[unitIndex]
        return unitPrice.resources.map(resource => ({
            resource,
            amount: resource.amount * quantity
        }))
    }

    private getUnitPrice = (unitIndex: number): Array<{resource: any, amount: number}> | null => {
        const { reward } = this.props
        
        if (!reward.prices || unitIndex >= reward.prices.length) return null
        
        const unitPrice = reward.prices[unitIndex]
        return unitPrice.resources.map(resource => ({
            resource,
            amount: resource.amount
        }))
    }



    private renderRewardContent() {
        const { reward } = this.props

        switch (reward.rewardType) {
            case RewardType.None:
                return (
                    <div className="reward-content">
                        <div className="reward-message">{reward.message}</div>
                        {this.state.errorMessage && (
                            <div className="error-message">
                                {this.state.errorMessage}
                            </div>
                        )}
                        
                        <div className="reward-actions">
                            <button 
                                className="reward-button ok-button" 
                                onClick={this.handleAccept}
                                disabled={this.state.isSubmitting}
                            >
                                {this.state.isSubmitting ? 'Processing...' : 'OK'}
                            </button>
                            {reward.canDecline && (
                                <button 
                                    className="reward-button decline-button" 
                                    onClick={this.handleDecline}
                                    disabled={this.state.isSubmitting}
                                >
                                    {this.state.isSubmitting ? 'Declining...' : 'Decline'}
                                </button>
                            )}
                        </div>
                    </div>
                )

            case RewardType.ResourcesGain:
                return (
                    <div className="reward-content">
                        <div className="reward-message">{reward.message}</div>
                        {reward.resourcesRewards && reward.resourcesRewards.length > 0 && (
                            <div className="reward-resources">
                                <h4>Resources to gain:</h4>
                                <div className="resources-list">
                                    {reward.resourcesRewards.map((resource, index) => (
                                        <div key={index} className="resource-item">
                                            <img 
                                                src={resource.iconUrl} 
                                                alt={resource.name}
                                                className="resource-icon"
                                            />
                                            <span className="resource-name">{resource.name}</span>
                                            <span className="resource-amount">+{resource.amount}</span>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                        {this.state.errorMessage && (
                            <div className="error-message">
                                {this.state.errorMessage}
                            </div>
                        )}
                        
                        <div className="reward-actions">
                            <button 
                                className="reward-button accept-button" 
                                onClick={this.handleAccept}
                                disabled={this.state.isSubmitting}
                            >
                                {this.state.isSubmitting ? 'Accepting...' : 'Accept'}
                            </button>
                            {reward.canDecline && (
                                <button 
                                    className="reward-button decline-button" 
                                    onClick={this.handleDecline}
                                    disabled={this.state.isSubmitting}
                                >
                                    {this.state.isSubmitting ? 'Declining...' : 'Decline'}
                                </button>
                            )}
                        </div>
                    </div>
                )

            case RewardType.UnitsGain:
                return (
                    <div className="reward-content">
                        <div className="reward-message">{reward.message}</div>
                        {reward.unitsRewards && reward.unitsRewards.length > 0 && (
                            <div className="reward-units">
                                <h4>Units to gain:</h4>
                                <div className="units-horizontal">
                                    {reward.unitsRewards.map((unit, index) => (
                                        <div key={index} className="unit-horizontal-item">
                                            <img 
                                                src={unit.dashboardImgUrl} 
                                                alt={unit.name}
                                                className="unit-thumbnail"
                                            />
                                            <span className="unit-amount">+{unit.amount}</span>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                        {this.state.errorMessage && (
                            <div className="error-message">
                                {this.state.errorMessage}
                            </div>
                        )}
                        
                        <div className="reward-actions">
                            <button 
                                className="reward-button accept-button" 
                                onClick={this.handleAccept}
                                disabled={this.state.isSubmitting}
                            >
                                {this.state.isSubmitting ? 'Accepting...' : 'Accept'}
                            </button>
                            {reward.canDecline && (
                                <button 
                                    className="reward-button decline-button" 
                                    onClick={this.handleDecline}
                                    disabled={this.state.isSubmitting}
                                >
                                    {this.state.isSubmitting ? 'Declining...' : 'Decline'}
                                </button>
                            )}
                        </div>
                    </div>
                )

            case RewardType.UnitToBuy:
                const { unitQuantities } = this.state
                const totalCost = this.getTotalCost()
                const remainingResources = this.getRemainingResources()
                
                return (
                    <div className="reward-content">
                        <div className="reward-message">{reward.message}</div>
                        
                        {/* Available Resources */}
                        <div className="available-resources">
                            <div className="resources-horizontal">
                                {Object.entries(this.state.availableResources).map(([resourceId, amount]) => {
                                    const resource = reward.prices?.flatMap(p => p.resources).find(r => r.id === resourceId)
                                    if (!resource) return null
                                    
                                    const cost = totalCost[resourceId] || 0
                                    const remaining = remainingResources[resourceId] || 0
                                    
                                    return (
                                        <div key={resourceId} className="resource-horizontal-item">
                                            <img 
                                                src={resource.iconUrl} 
                                                alt={resource.name}
                                                className="resource-icon"
                                            />
                                            <span className="resource-amount">{amount}</span>
                                            {cost > 0 && (
                                                <>
                                                    <span className="resource-cost">-{cost}</span>
                                                    <span className="resource-remaining">={remaining}</span>
                                                </>
                                            )}
                                        </div>
                                    )
                                })}
                            </div>
                        </div>

                        {/* Units to Buy with Sliders */}
                        {reward.unitsRewards && reward.unitsRewards.length > 0 && (
                            <div className="reward-units">
                                <h4>Units to buy:</h4>
                                <div className="units-list">
                                    {reward.unitsRewards.map((unit, index) => {
                                        const maxAffordable = this.getMaxAffordableQuantity(index)
                                        const maxAvailable = this.getMaxAvailableQuantity(index)
                                        const currentQuantity = unitQuantities[index] || 0
                                        const unitCost = this.getUnitCost(index)
                                        const unitPrice = this.getUnitPrice(index)
                                        
                                        return (
                                            <div key={index} className="unit-buy-item">
                                                <div className="unit-info">
                                                    <img 
                                                        src={unit.dashboardImgUrl} 
                                                        alt={unit.name}
                                                        className="unit-thumbnail"
                                                    />
                                                    <span className="unit-name">{unit.name}</span>
                                                    <span className="unit-quantity">{currentQuantity}</span>
                                                    <span className="unit-available">/ {maxAvailable}</span>
                                                </div>
                                                
                                                {unitPrice && (
                                                    <div className="unit-price">
                                                        <div className="prices-horizontal">
                                                            {unitPrice.map((price, priceIndex) => (
                                                                <div key={priceIndex} className="price-horizontal-item">
                                                                    <img 
                                                                        src={price.resource.iconUrl} 
                                                                        alt={price.resource.name}
                                                                        className="price-icon"
                                                                    />
                                                                    <span className="price-amount">{price.amount}</span>
                                                                </div>
                                                            ))}
                                                        </div>
                                                        {currentQuantity > 0 && unitCost && (
                                                            <div className="unit-cost">
                                                                <div className="costs-horizontal">
                                                                    {unitCost.map((cost, costIndex) => (
                                                                        <div key={costIndex} className="cost-horizontal-item">
                                                                            <img 
                                                                                src={cost.resource.iconUrl} 
                                                                                alt={cost.resource.name}
                                                                                className="cost-icon"
                                                                            />
                                                                            <span className="cost-amount">{cost.amount}</span>
                                                                        </div>
                                                                    ))}
                                                                </div>
                                                            </div>
                                                        )}
                                                    </div>
                                                )}
                                                
                                                <div className="unit-slider-container">
                                                    <input
                                                        type="range"
                                                        min="0"
                                                        max={maxAvailable}
                                                        value={currentQuantity}
                                                        onChange={(e) => this.handleUnitQuantityChange(index, parseInt(e.target.value))}
                                                        className="unit-slider"
                                                        disabled={maxAffordable === 0}
                                                    />
                                                    <div className="slider-labels">
                                                        <span>0</span>
                                                        <span>{maxAvailable}</span>
                                                    </div>
                                                </div>
                                            </div>
                                        )
                                    })}
                                </div>
                            </div>
                        )}
                        
                        {this.state.errorMessage && (
                            <div className="error-message">
                                {this.state.errorMessage}
                            </div>
                        )}
                        
                        <div className="reward-actions">
                            <button 
                                className="reward-button accept-button" 
                                onClick={this.handleAccept}
                                disabled={this.state.isSubmitting || this.getTotalSelectedUnits() < 1}
                            >
                                {this.state.isSubmitting ? 'Buying...' : 'Buy'}
                            </button>
                            {reward.canDecline && (
                                <button 
                                    className="reward-button decline-button" 
                                    onClick={this.handleDecline}
                                    disabled={this.state.isSubmitting}
                                >
                                    {this.state.isSubmitting ? 'Cancelling...' : 'Cancel'}
                                </button>
                            )}
                        </div>
                    </div>
                )

            case RewardType.Battle:
                return (
                    <div className="reward-content">
                        <div className="reward-message">{reward.message}</div>
                        
                        {reward.nextBattle && reward.nextBattle.units && reward.nextBattle.units.length > 0 && (
                            <div className="battle-units">
                                <h4>Battle Units:</h4>
                                <div className="units-horizontal">
                                    {reward.nextBattle.units.map((unit, index) => (
                                        <div key={index} className="unit-horizontal-item">
                                            <img 
                                                src={unit.thumbnailUrl} 
                                                alt={unit.name}
                                                className="unit-thumbnail"
                                            />
                                            <span className="unit-amount">{unit.count}</span>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                        
                        {this.state.errorMessage && (
                            <div className="error-message">
                                {this.state.errorMessage}
                            </div>
                        )}
                        
                        <div className="reward-actions">
                            <button 
                                className="reward-button accept-button" 
                                onClick={this.handleAccept}
                                disabled={this.state.isSubmitting}
                            >
                                {this.state.isSubmitting ? 'Starting Battle...' : 'Start Battle'}
                            </button>
                            {reward.canDecline && (
                                <button 
                                    className="reward-button decline-button" 
                                    onClick={this.handleDecline}
                                    disabled={this.state.isSubmitting}
                                >
                                    {this.state.isSubmitting ? 'Declining...' : 'Decline'}
                                </button>
                            )}
                        </div>
                    </div>
                )

            default:
                return (
                    <div className="reward-content">
                        <div className="reward-message">{reward.message}</div>
                        {this.state.errorMessage && (
                            <div className="error-message">
                                {this.state.errorMessage}
                            </div>
                        )}
                        
                        <div className="reward-actions">
                            <button 
                                className="reward-button ok-button" 
                                onClick={this.handleAccept}
                                disabled={this.state.isSubmitting}
                            >
                                {this.state.isSubmitting ? 'Processing...' : 'OK'}
                            </button>
                            {reward.canDecline && (
                                <button 
                                    className="reward-button decline-button" 
                                    onClick={this.handleDecline}
                                    disabled={this.state.isSubmitting}
                                >
                                    {this.state.isSubmitting ? 'Declining...' : 'Decline'}
                                </button>
                            )}
                        </div>
                    </div>
                )
        }
    }

    render() {
        return (
            <div className="reward-dialog-overlay">
                <div className="reward-dialog">
                    <div className="reward-dialog-header">
                        {this.props.reward.iconUrl && (
                            <img 
                                src={this.props.reward.iconUrl} 
                                alt="Reward Icon"
                                className="reward-header-icon"
                            />
                        )}
                        <h3>{this.props.reward.title || 'Reward'}</h3>
                    </div>
                    {this.renderRewardContent()}
                </div>
            </div>
        )
    }
} 