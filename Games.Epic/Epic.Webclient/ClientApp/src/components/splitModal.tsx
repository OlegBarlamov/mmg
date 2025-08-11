import React, {PureComponent} from "react";
import {IUserUnit} from "../services/serverAPI";
import {IServiceLocator} from "../services/serviceLocator";
import "./splitModal.css";

export interface ISplitModalProps {
    isVisible: boolean
    selectedUnit: IUserUnit | null
    splitAmount: number
    splitTargetSlot: number | null
    splitTargetContainer: 'supply' | 'army' | null
    serviceLocator: IServiceLocator
    playerInfo: any
    onSplitAmountChange: (amount: number) => void
    onClose: () => void
    onSplitComplete: (updatedContainer: any) => void
    onError: (error: string) => void
}

export class SplitModal extends PureComponent<ISplitModalProps> {
    private handleSplitAmountChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const amount = parseInt(event.target.value)
        this.props.onSplitAmountChange(amount)
    }

    private handleSplitConfirm = async () => {
        if (!this.props.selectedUnit || this.props.splitTargetSlot === null || this.props.splitTargetContainer === null) {
            return
        }

        const selectedUnit = this.props.selectedUnit
        const targetSlotIndex = this.props.splitTargetSlot
        const splitAmount = this.props.splitAmount
        const targetContainerType = this.props.splitTargetContainer

        try {
            const serverAPI = this.props.serviceLocator.serverAPI()
            const containerId = targetContainerType === 'supply' 
                ? this.props.playerInfo?.supplyContainerId 
                : this.props.playerInfo?.armyContainerId

            if (!containerId) {
                throw new Error('Container ID not available')
            }

            // Split the units
            const updatedContainer = await serverAPI.moveUnits(
                selectedUnit.id, 
                containerId, 
                splitAmount, 
                targetSlotIndex
            )

            // Notify parent of successful split
            this.props.onSplitComplete(updatedContainer)

        } catch (error) {
            console.error('Failed to split units:', error)
            this.props.onError('Failed to split units')
        }
    }

    render() {
        if (!this.props.isVisible || !this.props.selectedUnit) {
            return null
        }

        const selectedUnit = this.props.selectedUnit
        const maxAmount = selectedUnit.count - 1
        const minAmount = 1

        return (
            <div className="split-modal-overlay">
                <div className="split-modal">
                    <div className="split-modal-header">
                        <h2>Split Army</h2>
                        <button className="close-button" onClick={this.props.onClose}>×</button>
                    </div>
                    <div className="split-modal-content">
                        <div className="split-unit-info">
                            <img src={selectedUnit.thumbnailUrl} alt="Unit" className="unit-thumbnail" />
                            <div>
                                <div className="unit-name">{selectedUnit.typeId}</div>
                                <div className="unit-count">Total: {selectedUnit.count}</div>
                            </div>
                        </div>
                        
                        <div className="split-amount-section">
                            <label htmlFor="split-amount">Amount to split: {this.props.splitAmount}</label>
                            <input
                                id="split-amount"
                                type="range"
                                min={minAmount}
                                max={maxAmount}
                                value={this.props.splitAmount}
                                onChange={this.handleSplitAmountChange}
                                className="split-slider"
                            />
                            <div className="split-amount-display">
                                Split: {this.props.splitAmount} | Keep: {selectedUnit.count - this.props.splitAmount}
                            </div>
                        </div>

                        <div className="split-confirmation">
                            <p>Target: {this.props.splitTargetContainer} slot {this.props.splitTargetSlot! + 1}</p>
                            <button 
                                className="confirm-split-button"
                                onClick={this.handleSplitConfirm}
                            >
                                Confirm Split
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        )
    }
}
