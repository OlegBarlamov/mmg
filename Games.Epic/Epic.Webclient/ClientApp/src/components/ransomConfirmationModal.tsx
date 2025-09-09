import React from 'react';
import './ransomConfirmationModal.css';
import { IRansomPrice, IResourceInfo } from '../services/serverAPI';

export interface IRansomConfirmationModalProps {
    isVisible: boolean;
    ransomPrice: IRansomPrice | null;
    playerGold: number | null;
    isLoading: boolean;
    onConfirm: () => void;
    onCancel: () => void;
}

export class RansomConfirmationModal extends React.Component<IRansomConfirmationModalProps> {
    render() {
        if (!this.props.isVisible) {
            return null;
        }

        const { ransomPrice, playerGold, isLoading } = this.props;
        const canAffordRansom = ransomPrice && playerGold !== null && playerGold >= ransomPrice.gold;

        return (
            <div className="ransom-confirmation-overlay">
                <div className="ransom-confirmation-modal">
                    <div className="ransom-header">
                        <h3>Pay Ransom</h3>
                    </div>
                    <div className="ransom-content">
                        {isLoading ? (
                            <div className="ransom-loading">
                                Loading ransom price...
                            </div>
                        ) : ransomPrice ? (
                            <div className="ransom-price-display">
                                <p>Do you want to pay the ransom to escape this battle?</p>
                                <div className="ransom-price-info">
                                    <span className="ransom-price-label">Cost:</span>
                                    <span className="ransom-price-amount">{ransomPrice.gold} gold</span>
                                </div>
                                {playerGold !== null && (
                                    <div className="ransom-player-gold">
                                        <span className="ransom-gold-label">Your gold:</span>
                                        <span className="ransom-gold-amount">{playerGold} gold</span>
                                    </div>
                                )}
                                {!canAffordRansom && playerGold !== null && (
                                    <div className="ransom-insufficient-funds">
                                        You don't have enough gold to pay the ransom!
                                    </div>
                                )}
                            </div>
                        ) : (
                            <div className="ransom-error">
                                Failed to load ransom price. Please try again.
                            </div>
                        )}
                    </div>
                    <div className="ransom-actions">
                        <button 
                            className="ransom-button ransom-cancel-button"
                            onClick={this.props.onCancel}
                        >
                            No
                        </button>
                        <button 
                            className="ransom-button ransom-confirm-button"
                            onClick={this.props.onConfirm}
                            disabled={isLoading || !ransomPrice || !canAffordRansom}
                        >
                            Yes
                        </button>
                    </div>
                </div>
            </div>
        );
    }
}
