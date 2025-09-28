import './rewardDialog.css'
import React, { PureComponent } from "react";
import { IRewardToAccept, IPriceResource } from "../rewards/IRewardToAccept";
import { RewardType } from "../rewards/RewardType";
import { IResourceInfo } from "../services/serverAPI";

export interface IRewardDialogProps {
    reward: IRewardToAccept
    onAccept: (affectedSlots: number[]) => Promise<void>
    onBeginGuardBattle: (rewardId: string) => Promise<void>
    onDecline: () => Promise<void>
    serviceLocator?: any
}

interface IRewardDialogState {
    unitQuantities: number[]
    availableResources: { [resourceId: string]: number }
    isSubmitting: boolean
    errorMessage: string | null
    upgradableUnits: IUpgradableUnit[] | null
    upgradeQuantities: number[]
}

interface IUpgradableUnit {
    id: string
    typeId: string
    name: string
    dashboardImgUrl: string
    count: number
    containerName: string
    slotIndex: number
    originalPrice: IPriceResource
    upgradeOptions: IUnitInfo[]
}

interface IUnitInfo {
    id: string
    name: string
    dashboardImgUrl: string
    price: IPriceResource
}

export class RewardDialog extends PureComponent<IRewardDialogProps, IRewardDialogState> {
    constructor(props: IRewardDialogProps) {
        super(props)
        this.state = { 
            unitQuantities: [],
            availableResources: {},
            isSubmitting: false,
            errorMessage: null,
            upgradableUnits: null,
            upgradeQuantities: []
        }
    }

    private handleGuardBattleBegin = async () => {
        this.setState({ isSubmitting: true, errorMessage: null })
        try {
            await this.props.onBeginGuardBattle(this.props.reward.id)
        } catch (error) {
            console.error('Error beginning guard battle:', error)
            this.setState({ 
                errorMessage: error instanceof Error ? error.message : 'Failed to begin guard battle. Please try again.',
                isSubmitting: false 
            })
        }
    }

    private handleAccept = async () => {
        this.setState({ isSubmitting: true, errorMessage: null })
        
        try {
            // For UnitToBuy, we need to pass the selected quantities
            if (this.props.reward.rewardType === RewardType.UnitsToBuy) {
                // The amounts array should contain the quantities for each unit
                const amounts = this.state.unitQuantities
                // Update the reward amounts before calling onAccept
                this.props.reward.amounts = amounts
            }
            
            let affectedSlots: number[] = []
            // For UnitsToUpgrade, we need to pass the selected upgrade quantities and affected slots
            if (this.props.reward.rewardType === RewardType.UnitsToUpgrade) {
                // The amounts array should contain the quantities for each upgrade
                const amounts = this.state.upgradeQuantities
                // The affectedSlots array should contain the slot indices of units being upgraded
                affectedSlots = this.state.upgradableUnits
                    ?.map((unit, index) => amounts[index] > 0 ? unit.slotIndex : -1)
                    .filter(slotIndex => slotIndex !== -1) || []
                
                // Update the reward amounts before calling onAccept
                this.props.reward.amounts = amounts
            }
            
            const result = await this.props.onAccept(affectedSlots)
            
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
        this.preInitializeRewardState()
    }

    componentDidUpdate(prevProps: IRewardDialogProps) {
        if (prevProps.reward.id !== this.props.reward.id) {
            this.preInitializeRewardState()
        }
    }

    private preInitializeRewardState = async () => {
        // Reset submission state when showing a new reward
        this.setState({ 
            isSubmitting: false, 
            errorMessage: null 
        })
        
        const { reward, serviceLocator } = this.props
        if (reward.rewardType === RewardType.UnitsToBuy) {
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
        } else if (reward.rewardType === RewardType.UnitsToUpgrade) {
            // Initialize upgradable units for UnitsToUpgrade case
            if (serviceLocator) {
                try {
                    const serverAPI = serviceLocator.serverAPI()
                    // Get the player info to access army and supply containers
                    const userInfo = await serverAPI.getUserInfo()
                    if (!userInfo.playerId) {
                        throw new Error('No active player')
                    }
                    const playerInfo = await serverAPI.getPlayer(userInfo.playerId)
                    
                    // Get units from army container only
                    const armyContainer = await serverAPI.getUnitsContainer(playerInfo.armyContainerId)
                    
                    // Get unit infos for the reward units to find what can be upgraded TO them
                    const rewardUnitTypeIds = reward.unitsRewards?.map(unit => unit.id) || []
                    const rewardUnitInfos = await serverAPI.getUnitTypesInfos(rewardUnitTypeIds)
                    
                    // Get unit infos for army units to get their prices
                    const armyUnitTypeIds = armyContainer.units.map((u: any) => u.typeId)
                    const armyUnitInfos = await serverAPI.getUnitTypesInfos(armyUnitTypeIds)
                    
                    // Find upgradable units by checking which army units can be upgraded TO the reward unit types
                    const upgradableUnits: IUpgradableUnit[] = []
                    
                    // Check army units - find ones that can be upgraded TO the reward unit types
                    armyContainer.units.forEach((unit: any) => {
                        // Check if this unit can be upgraded to any of the reward unit types
                        const canUpgradeTo = rewardUnitInfos.filter((rewardUnitInfo: any) => 
                            rewardUnitInfo.upgradeForUnitTypeIds && 
                            rewardUnitInfo.upgradeForUnitTypeIds.includes(unit.typeId)
                        )
                        
                        if (canUpgradeTo.length > 0) {
                            const unitInfo = armyUnitInfos.find((info: any) => info.id === unit.typeId)
                            upgradableUnits.push({
                                id: unit.id,
                                typeId: unit.typeId,
                                name: unit.name,
                                dashboardImgUrl: unit.thumbnailUrl,
                                count: unit.count,
                                containerName: 'Army',
                                slotIndex: unit.slotIndex,
                                originalPrice: unitInfo ? unitInfo.price : { resources: [] },
                                upgradeOptions: canUpgradeTo.map((rewardUnitInfo: any) => ({
                                    id: rewardUnitInfo.id,
                                    name: rewardUnitInfo.name,
                                    dashboardImgUrl: rewardUnitInfo.dashboardImgUrl,
                                    price: rewardUnitInfo.price || { resources: [] }
                                }))
                            })
                        }
                    })
                    
                    // Initialize upgrade quantities to 0
                    const upgradeQuantities = new Array(upgradableUnits.length).fill(0)
                    
                    // Get available resources from the server
                    let availableResources: { [resourceId: string]: number } = {}
                    try {
                        const resources: IResourceInfo[] = await serverAPI.getResources()
                        resources.forEach((resource: IResourceInfo) => {
                            availableResources[resource.id] = resource.amount
                        })
                    } catch (error) {
                        console.error('Failed to fetch resources:', error)
                        // Fallback to empty resources
                    }
                    
                    this.setState({ 
                        upgradableUnits,
                        upgradeQuantities,
                        availableResources
                    })
                } catch (error) {
                    console.error('Failed to fetch upgradable units:', error)
                    this.setState({ 
                        upgradableUnits: [],
                        upgradeQuantities: [],
                        availableResources: {}
                    })
                }
            } else {
                // Reset upgrade quantities for non-upgrade rewards
                this.setState({ 
                    upgradableUnits: null,
                    upgradeQuantities: [],
                    availableResources: {}
                })
            }
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

    // Upgrade-related methods
    private handleUpgradeQuantityChange = (unitIndex: number, newQuantity: number) => {
        const { upgradableUnits } = this.state
        if (!upgradableUnits || unitIndex >= upgradableUnits.length) return
        
        const unit = upgradableUnits[unitIndex]
        const maxAvailable = unit.count
        const limitedQuantity = Math.min(newQuantity, maxAvailable)
        
        const newQuantities = [...this.state.upgradeQuantities]
        newQuantities[unitIndex] = limitedQuantity
        
        this.setState({ upgradeQuantities: newQuantities })
    }

    private getUpgradeCost = (unitIndex: number): { [resourceId: string]: number } => {
        const { upgradableUnits } = this.state
        if (!upgradableUnits || unitIndex >= upgradableUnits.length) return {}
        
        const unit = upgradableUnits[unitIndex]
        const quantity = this.state.upgradeQuantities[unitIndex] || 0
        
        if (quantity === 0) return {}
        
        // Calculate upgrade cost as difference between upgraded unit price and original unit price
        const upgradeCost: { [resourceId: string]: number } = {}
        
        // Get the original unit price (from the unit in army/supply)
        const originalUnitPrice = unit.originalPrice || { resources: [] }
        
        // For each upgrade option, calculate the cost difference
        unit.upgradeOptions.forEach(upgradeOption => {
            const upgradePrice = upgradeOption.price || { resources: [] }
            
            // Calculate the difference for each resource
            upgradePrice.resources.forEach((upgradeResource: any) => {
                const originalResource = originalUnitPrice.resources.find((r: any) => r.id === upgradeResource.id)
                const originalAmount = originalResource ? originalResource.amount : 0
                const upgradeAmount = upgradeResource.amount
                
                // Cost per unit is the difference in price
                const costPerUnit = upgradeAmount - originalAmount
                const totalCost = costPerUnit * quantity
                
                if (totalCost > 0) {
                    upgradeCost[upgradeResource.id] = (upgradeCost[upgradeResource.id] || 0) + totalCost
                }
            })
        })
        
        return upgradeCost
    }

    private getTotalUpgradeCost = (): { [resourceId: string]: number } => {
        const { upgradableUnits } = this.state
        if (!upgradableUnits) return {}
        
        const totalCost: { [resourceId: string]: number } = {}
        
        upgradableUnits.forEach((unit, index) => {
            const unitCost = this.getUpgradeCost(index)
            Object.entries(unitCost).forEach(([resourceId, cost]) => {
                totalCost[resourceId] = (totalCost[resourceId] || 0) + cost
            })
        })
        
        return totalCost
    }

    private canAffordUpgrade = (): boolean => {
        const { availableResources } = this.state
        const totalCost = this.getTotalUpgradeCost()
        
        return Object.entries(totalCost).every(([resourceId, cost]) => {
            const available = availableResources[resourceId] || 0
            return available >= cost
        })
    }

    private getTotalSelectedUpgrades = (): number => {
        return this.state.upgradeQuantities.reduce((total, qty) => total + qty, 0)
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

        if (reward.guardBattle && !reward.guardBattle.isFinished) {
            return (
                <div className="reward-content">
                    <div className="reward-message">{reward.guardMessage}</div>
                    
                    {reward.guardBattle && reward.guardBattle.units && reward.guardBattle.units.length > 0 && (
                        <div className="battle-units">
                            <h4>Battle Units:</h4>
                            <div className="units-horizontal">
                                {reward.guardBattle.units.map((unit, index) => (
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
                            onClick={this.handleGuardBattleBegin}
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
        }

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

            case RewardType.UnitsToBuy:
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

            case RewardType.UnitsToUpgrade:
                const { upgradeQuantities } = this.state
                const totalUpgradeCost = this.getTotalUpgradeCost()
                
                return (
                    <div className="reward-content">
                        <div className="reward-message">{reward.message}</div>
                        
                        {/* Available Resources */}
                        <div className="available-resources">
                            <div className="resources-horizontal">
                                {Object.entries(this.state.availableResources).map(([resourceId, amount]) => {
                                    const cost = totalUpgradeCost[resourceId] || 0
                                    const remaining = amount - cost
                                    
                                    // Find the resource info to get the icon and name
                                    const resourceInfo = this.state.upgradableUnits?.flatMap(unit => 
                                        [...unit.originalPrice.resources, ...unit.upgradeOptions.flatMap(opt => opt.price.resources)]
                                    ).find(resource => resource.id === resourceId)
                                    if (!resourceInfo) return null
                                    
                                    return (
                                        <div key={resourceId} className="resource-horizontal-item">
                                            <img 
                                                src={resourceInfo?.iconUrl || "/placeholder-icon.png"} 
                                                alt={resourceInfo?.name || "Resource"}
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
                        
                        {this.state.upgradableUnits && this.state.upgradableUnits.length > 0 ? (
                            <div className="reward-units">
                                <h4>Units that can be upgraded:</h4>
                                <div className="units-list">
                                    {this.state.upgradableUnits.map((unit: any, index: number) => {
                                        const currentQuantity = upgradeQuantities[index] || 0
                                        const unitCost = this.getUpgradeCost(index)
                                        
                                        return (
                                            <div key={index} className="unit-upgrade-item">
                                                <div className="unit-info">
                                                    <img 
                                                        src={unit.dashboardImgUrl} 
                                                        alt={unit.name}
                                                        className="unit-thumbnail"
                                                    />
                                                    <span className="unit-name">{unit.name}</span>
                                                    <span className="unit-quantity">{currentQuantity}</span>
                                                    <span className="unit-available">/ {unit.count}</span>
                                                    <span className="unit-container">({unit.containerName})</span>
                                                </div>
                                                
                                                <div className="unit-upgrade-info">
                                                    <span className="upgrade-label">Upgrade to:</span>
                                                    <div className="upgrade-options">
                                                        {unit.upgradeOptions.map((upgradeOption: any, upgradeIndex: number) => (
                                                            <div key={upgradeIndex} className="upgrade-option">
                                                                <img 
                                                                    src={upgradeOption.dashboardImgUrl} 
                                                                    alt={upgradeOption.name}
                                                                    className="unit-thumbnail small"
                                                                />
                                                                <span className="upgrade-name">{upgradeOption.name}</span>
                                                            </div>
                                                        ))}

                                                        {Object.keys(unitCost).length > 0 && currentQuantity > 0 && (
                                                            <div className="unit-cost">
                                                                <div className="costs-horizontal">
                                                                    {Object.entries(unitCost).map(([resourceId, cost]) => {
                                                                        // Find the resource info to get the icon and name
                                                                        const resourceInfo = [...unit.originalPrice.resources, ...unit.upgradeOptions.flatMap((opt: any) => opt.price.resources)]
                                                                            .find(resource => resource.id === resourceId)
                                                                        
                                                                        return (
                                                                            <div key={resourceId} className="cost-horizontal-item">
                                                                                <img 
                                                                                    src={resourceInfo?.iconUrl || "/placeholder-icon.png"} 
                                                                                    alt={resourceInfo?.name || "Resource"}
                                                                                    className="cost-icon"
                                                                                />
                                                                                <span className="cost-amount">{cost}</span>
                                                                            </div>
                                                                        )
                                                                    })}
                                                                </div>
                                                            </div>
                                                        )}
                                                    </div>
                                                </div>
                                                
                                                <div className="unit-slider-container">
                                                    <input
                                                        type="range"
                                                        min="0"
                                                        max={unit.count}
                                                        value={currentQuantity}
                                                        onChange={(e) => this.handleUpgradeQuantityChange(index, parseInt(e.target.value))}
                                                        className="unit-slider"
                                                    />
                                                    <div className="slider-labels">
                                                        <span>0</span>
                                                        <span>{unit.count}</span>
                                                    </div>
                                                </div>
                                            </div>
                                        )
                                    })}
                                </div>
                            </div>
                        ) : (
                            <div className="no-upgradable-units">
                                <p>No units found that can be upgraded with this reward.</p>
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
                                disabled={this.state.isSubmitting || this.getTotalSelectedUpgrades() < 1 || !this.canAffordUpgrade()}
                            >
                                {this.state.isSubmitting ? 'Upgrading...' : 'Upgrade'}
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