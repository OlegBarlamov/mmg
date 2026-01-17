import './marketRewardComponent.css'
import React from "react";
import { IRewardToAccept } from "../rewards/IRewardToAccept";

export interface IMarketRewardComponentProps {
    reward: IRewardToAccept
    availableResources: { [resourceId: string]: number }
    resourcePrices: { [resourceId: string]: number }
    marketTransactions: { [resourceId: string]: number }
    isSubmitting: boolean
    errorMessage: string | null
    onBuy: (resourceId: string, amount: number) => void
    onSell: (resourceId: string, amount: number) => void
    onReset: (resourceId: string) => void
    onAccept: () => Promise<void>
    onDecline: () => Promise<void>
}

export const MarketRewardComponent: React.FC<IMarketRewardComponentProps> = ({
    reward,
    availableResources,
    resourcePrices,
    marketTransactions,
    isSubmitting,
    errorMessage,
    onBuy,
    onSell,
    onReset,
    onAccept,
    onDecline
}) => {
    const getGoldResourceId = (): string => {
        const goldKey = Object.keys(availableResources).find(key => 
            key.toLowerCase().includes('gold')
        )
        return goldKey || Object.keys(availableResources)[0] || ''
    }

    const calculateMarketTotal = (): { goldCost: number, goldGain: number } => {
        let goldCost = 0
        let goldGain = 0
        
        Object.entries(marketTransactions).forEach(([resourceId, amount]) => {
            const resourcePrice = resourcePrices[resourceId] || 0
            if (amount > 0) {
                // Buying: 5x price
                goldCost += amount * resourcePrice * 5
            } else if (amount < 0) {
                // Selling: full price
                goldGain += Math.abs(amount) * resourcePrice
            }
        })
        
        return { goldCost, goldGain }
    }

    const goldResourceId = getGoldResourceId()
    const currentGold = availableResources[goldResourceId] || 0
    const { goldCost, goldGain } = calculateMarketTotal()
    const netGoldChange = goldGain - goldCost
    const finalGold = currentGold + netGoldChange
    
    // Get all resources except gold
    const resources = reward.resourcesRewards?.filter(r => r.id !== goldResourceId) || []
    
    return (
        <div className="reward-content market-content">
            <div className="reward-message">{reward.message}</div>
            
            <div className="market-info">
                <div className="market-gold-display">
                    <div className="gold-current">
                        <span className="gold-label">Current Gold:</span>
                        <span className="gold-amount">{currentGold.toLocaleString()}</span>
                    </div>
                    {netGoldChange !== 0 && (
                        <div className={`gold-change ${netGoldChange > 0 ? 'positive' : 'negative'}`}>
                            <span>{netGoldChange > 0 ? '+' : ''}{netGoldChange.toLocaleString()}</span>
                        </div>
                    )}
                    <div className="gold-final">
                        <span className="gold-label">Final Gold:</span>
                        <span className="gold-amount">{finalGold.toLocaleString()}</span>
                    </div>
                </div>
            </div>
            
            <div className="market-resources">
                <h4>Resources</h4>
                <div className="market-resources-list">
                    {resources.map((resource) => {
                            const currentAmount = availableResources[resource.id] || 0
                            const transaction = marketTransactions[resource.id] || 0
                            const finalAmount = currentAmount + transaction
                            const resourcePrice = resourcePrices[resource.id] || 0
                            const buyPrice = resourcePrice * 5
                            const sellPrice = resourcePrice
                            // Can buy if we have enough gold after all current transactions, and resource has a valid price
                            const canBuy = resourcePrice > 0 && finalGold >= buyPrice
                            const canSell = finalAmount > 0
                        
                        return (
                            <div key={resource.id} className="market-resource-item">
                                <div className="resource-info">
                                    <img 
                                        src={resource.iconUrl} 
                                        alt={resource.name}
                                        className="resource-icon"
                                    />
                                    <span className="resource-name">{resource.name}</span>
                                    <span className="resource-current">{currentAmount}</span>
                                    {transaction !== 0 && (
                                        <span className={`resource-transaction ${transaction > 0 ? 'buy' : 'sell'}`}>
                                            {transaction > 0 ? '+' : ''}{transaction}
                                        </span>
                                    )}
                                    <span className="resource-final">→ {finalAmount}</span>
                                </div>
                                <div className="resource-actions">
                                    <button
                                        className="market-button buy-button"
                                        onClick={() => onBuy(resource.id, 1)}
                                        disabled={!canBuy || isSubmitting}
                                        title={`Buy 1 ${resource.name} for ${buyPrice} gold`}
                                    >
                                        Buy
                                    </button>
                                    <button
                                        className="market-button buy-button"
                                        onClick={() => onBuy(resource.id, 10)}
                                        disabled={!canBuy || finalGold < buyPrice * 10 || isSubmitting}
                                        title={`Buy 10 ${resource.name} for ${buyPrice * 10} gold`}
                                    >
                                        Buy 10
                                    </button>
                                    <button
                                        className="market-button sell-button"
                                        onClick={() => onSell(resource.id, 1)}
                                        disabled={!canSell || isSubmitting}
                                        title={`Sell 1 ${resource.name} for ${sellPrice} gold`}
                                    >
                                        Sell
                                    </button>
                                    <button
                                        className="market-button sell-button"
                                        onClick={() => onSell(resource.id, 10)}
                                        disabled={!canSell || finalAmount < 10 || isSubmitting}
                                        title={`Sell 10 ${resource.name} for ${sellPrice * 10} gold`}
                                    >
                                        Sell 10
                                    </button>
                                    {transaction !== 0 && (
                                        <button
                                            className="market-button reset-button"
                                            onClick={() => onReset(resource.id)}
                                            disabled={isSubmitting}
                                        >
                                            Reset
                                        </button>
                                    )}
                                </div>
                            </div>
                        )
                    })}
                </div>
            </div>
            
            {errorMessage && (
                <div className="error-message">
                    {errorMessage}
                </div>
            )}
            
            <div className="reward-actions">
                <button 
                    className="reward-button accept-button" 
                    onClick={async () => await onAccept()}
                    disabled={isSubmitting || finalGold < 0}
                >
                    {isSubmitting ? 'Processing...' : 'Complete Trade'}
                </button>
                {reward.canDecline && (
                    <button 
                        className="reward-button decline-button" 
                        onClick={async () => await onDecline()}
                        disabled={isSubmitting}
                    >
                        {isSubmitting ? 'Declining...' : 'Decline'}
                    </button>
                )}
            </div>
        </div>
    )
}
