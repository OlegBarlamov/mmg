import React, {PureComponent} from "react";
import {IServiceLocator} from "../services/serviceLocator";
import {IPlayerInfo, IUserUnit} from "../services/serverAPI";
import {SplitModal} from "./splitModal";
import {UnitInfoModal} from "./unitInfoModal";
import "./armyDisplay.css";

export interface IArmyDisplayProps {
    serviceLocator: IServiceLocator
    playerInfo: IPlayerInfo | null
    onSupplyClick?: () => void
    armyUnits: IUserUnit[] | null
    armyCapacity: number
    onArmyUnitsUpdate?: (units: IUserUnit[]) => void
    highlightedSlots?: number | null
}

interface IArmyDisplayState {
    isLoading: boolean
    error: string | null
    selectedUnit: IUserUnit | null
    isDragging: boolean
    showSplitModal: boolean
    splitAmount: number
    splitTargetSlot: number | null
    showUnitInfoModal: boolean
    infoUnit: IUserUnit | null
}

export class ArmyDisplay extends PureComponent<IArmyDisplayProps, IArmyDisplayState> {
    constructor(props: IArmyDisplayProps) {
        super(props)
        
        this.state = {
            isLoading: true,
            error: null,
            selectedUnit: null,
            isDragging: false,
            showSplitModal: false,
            splitAmount: 1,
            splitTargetSlot: null,
            showUnitInfoModal: false,
            infoUnit: null
        }
    }
    
    async componentDidMount() {
        this.setState({ isLoading: false })
    }

    // Expose this method so parent can call it
    public async refreshArmy() {
        // This is now handled by the parent component
    }

    // Expose army units for parent component
    public getArmyUnits(): IUserUnit[] | null {
        return this.props.armyUnits
    }

    private handleUnitClick = (unit: IUserUnit) => {
        // Check if we're in split mode (selected unit exists but not dragging)
        if (this.state.selectedUnit && !this.state.isDragging) {
            // We're in split mode, check if this is a valid target
            const canBeSplitTarget = this.state.selectedUnit.typeId === unit.typeId
            if (canBeSplitTarget) {
                this.handleSplitTargetSelect(unit.slotIndex)
            }
        } else if (this.state.selectedUnit) {
            // If we have a selected unit and are dragging, try to move it to this slot
            this.moveUnitToSlot(unit.slotIndex)
        } else {
            // Select this unit for moving
            this.setState({ selectedUnit: unit, isDragging: true })
        }
    }

    private handleInfoClick = (unit: IUserUnit, event: React.MouseEvent) => {
        event.stopPropagation() // Prevent triggering unit click
        this.setState({ 
            showUnitInfoModal: true, 
            infoUnit: unit 
        })
    }

    private handleUnitInfoModalClose = () => {
        this.setState({ 
            showUnitInfoModal: false, 
            infoUnit: null 
        })
    }

    private handleSplitClick = (unit: IUserUnit) => {
        this.setState({ 
            selectedUnit: unit, 
            isDragging: false,
            showSplitModal: false,
            splitAmount: Math.max(1, Math.floor(unit.count / 2)),
            splitTargetSlot: null
        })
    }

    private handleEmptySlotClick = (slotIndex: number) => {
        // Check if we're in split mode (selected unit exists but not dragging)
        if (this.state.selectedUnit && !this.state.isDragging) {
            // We're in split mode, empty slots are always valid targets
            this.handleSplitTargetSelect(slotIndex)
        } else if (this.state.selectedUnit) {
            // If we have a selected unit and are dragging, try to move it to this slot
            this.moveUnitToSlot(slotIndex)
        }
    }

    private handleSplitTargetSelect = (slotIndex: number) => {
        this.setState({ 
            splitTargetSlot: slotIndex,
            showSplitModal: true
        })
    }

    private handleSplitModalClose = () => {
        this.setState({ 
            showSplitModal: false, 
            selectedUnit: null,
            splitAmount: 1,
            splitTargetSlot: null
        })
    }

    private handleSplitAmountChange = (amount: number) => {
        this.setState({ splitAmount: amount })
    }

    private handleSplitComplete = (updatedContainer: any) => {
        // Update the parent's army units state
        this.props.onArmyUnitsUpdate?.(updatedContainer.units)
        this.setState({ 
            showSplitModal: false,
            selectedUnit: null,
            splitAmount: 1,
            splitTargetSlot: null
        })
    }

    private handleSplitError = (error: string) => {
        this.setState({ 
            error: error, 
            showSplitModal: false,
            selectedUnit: null,
            splitAmount: 1,
            splitTargetSlot: null
        })
    }

    private moveUnitToSlot = async (targetSlotIndex: number) => {
        if (!this.state.selectedUnit) return

        const selectedUnit = this.state.selectedUnit
        
        // Don't allow moving to the same slot
        if (selectedUnit.slotIndex === targetSlotIndex) {
            this.setState({ selectedUnit: null, isDragging: false })
            return
        }

        try {
            const serverAPI = this.props.serviceLocator.serverAPI()
            const containerId = this.props.playerInfo?.armyContainerId

            if (!containerId) {
                throw new Error('Container ID not available')
            }

            // Move the entire unit count
            const updatedContainer = await serverAPI.moveUnits(
                selectedUnit.id, 
                containerId, 
                selectedUnit.count, 
                targetSlotIndex
            )

            // Update the parent's army units state
            this.props.onArmyUnitsUpdate?.(updatedContainer.units)
            this.setState({ 
                selectedUnit: null, 
                isDragging: false 
            })

        } catch (error) {
            console.error('Failed to move unit:', error)
            this.setState({ 
                error: 'Failed to move unit', 
                selectedUnit: null, 
                isDragging: false 
            })
        }
    }

    // Method to refresh army units from server (for cross-container moves)
    public async refreshArmyFromServer() {
        try {
            const serverAPI = this.props.serviceLocator.serverAPI()
            if (!this.props.playerInfo?.armyContainerId) {
                throw new Error('No army container ID available')
            }
            const armyContainer = await serverAPI.getUnitsContainer(this.props.playerInfo.armyContainerId)
            this.props.onArmyUnitsUpdate?.(armyContainer.units)
        } catch (error) {
            console.error('Failed to refresh army from server:', error)
        }
    }

    private renderArmySlot(unit: IUserUnit | null, index: number) {
        const isSelected = this.state.selectedUnit?.id === unit?.id
        const isTarget = this.state.isDragging && !isSelected
        const isHighlighted = this.props.highlightedSlots !== null && index < this.props.highlightedSlots!
        const isSplitTarget = this.state.showSplitModal && this.state.splitTargetSlot === index
        const canBeSplitTarget = this.state.showSplitModal && unit && 
            this.state.selectedUnit?.typeId === unit.typeId
        
        if (unit) {
            return (
                <div 
                    key={index} 
                    className={`army-slot ${isSelected ? 'selected' : ''} ${isTarget ? 'target' : ''} ${isHighlighted ? 'highlighted' : ''} ${isSplitTarget ? 'split-target' : ''} ${canBeSplitTarget ? 'split-valid' : ''}`}
                    onClick={() => this.handleUnitClick(unit)}
                >
                    <img 
                        src={unit.thumbnailUrl} 
                        alt={`Unit ${index + 1}`}
                        className="unit-image"
                    />
                    <div className="unit-count">{unit.count}</div>
                    <button 
                        className="unit-info-button"
                        onClick={(e) => this.handleInfoClick(unit, e)}
                        title="Unit Information"
                    >
                        i
                    </button>
                </div>
            )
        } else {
            return (
                <div 
                    key={index} 
                    className={`army-slot empty ${isTarget ? 'target' : ''} ${isHighlighted ? 'highlighted' : ''} ${isSplitTarget ? 'split-target' : ''}`}
                    onClick={() => this.handleEmptySlotClick(index)}
                >
                    <div className="empty-slot">Empty</div>
                </div>
            )
        }
    }

    private renderSplitButton() {
        const isSelected = this.state.selectedUnit !== null
        const selectedUnit = this.state.selectedUnit
        const isDisabled = !isSelected || !selectedUnit || selectedUnit.count <= 1

        return (
            <button 
                className={`split-button ${isDisabled ? 'disabled' : ''}`}
                onClick={() => {
                    if (!isDisabled && selectedUnit) {
                        this.handleSplitClick(selectedUnit)
                    }
                }}
                disabled={isDisabled}
            >
                Split
            </button>
        )
    }

    render() {
        const { isLoading, error } = this.state
        const { playerInfo, armyUnits, armyCapacity } = this.props

        if (isLoading) {
            return (
                <div className="army-section">
                    <div className="army-header">
                        <h2 className="army-title">Your Army</h2>
                        <div className="header-buttons">
                            {this.renderSplitButton()}
                            <button 
                                className="supply-button"
                                onClick={this.props.onSupplyClick}
                                disabled={!this.props.onSupplyClick}
                            >
                                Supply
                            </button>
                        </div>
                    </div>
                    <div className="army-loading">Loading army...</div>
                </div>
            )
        }

        if (error) {
            return (
                <div className="army-section">
                    <div className="army-header">
                        <h2 className="army-title">Your Army</h2>
                        <div className="header-buttons">
                            {this.renderSplitButton()}
                            <button 
                                className="supply-button"
                                onClick={this.props.onSupplyClick}
                                disabled={!this.props.onSupplyClick}
                            >
                                Supply
                            </button>
                        </div>
                    </div>
                    <div className="army-error">{error}</div>
                </div>
            )
        }

        // Use armyCapacity from props
        const armySlots = new Array(armyCapacity).fill(null)
        
        // Fill the slots with actual units using slotIndex from server
        if (armyUnits) {
            armyUnits.forEach((unit) => {
                if (unit.slotIndex >= 0 && unit.slotIndex < armySlots.length) {
                    armySlots[unit.slotIndex] = unit
                }
            })
        }

        return (
            <>
                <div className="army-section">
                    <div className="army-header">
                        <h2 className="army-title">Your Army ({armyCapacity} slots)</h2>
                        <div className="header-buttons">
                            {this.state.isDragging && (
                                <button className="cancel-button" onClick={() => this.setState({ selectedUnit: null, isDragging: false })}>
                                    Cancel
                                </button>
                            )}
                            {this.renderSplitButton()}
                            <button 
                                className="supply-button"
                                onClick={this.props.onSupplyClick}
                                disabled={!this.props.onSupplyClick}
                            >
                                Supply
                            </button>
                        </div>
                    </div>
                    <div className="army-grid">
                        {armySlots.map((unit, index) => this.renderArmySlot(unit, index))}
                    </div>
                </div>
                <SplitModal
                    isVisible={this.state.showSplitModal}
                    selectedUnit={this.state.selectedUnit}
                    splitAmount={this.state.splitAmount}
                    splitTargetSlot={this.state.splitTargetSlot}
                    splitTargetContainer="army"
                    serviceLocator={this.props.serviceLocator}
                    playerInfo={this.props.playerInfo}
                    onSplitAmountChange={this.handleSplitAmountChange}
                    onClose={this.handleSplitModalClose}
                    onSplitComplete={this.handleSplitComplete}
                    onError={this.handleSplitError}
                />
                <UnitInfoModal
                    isVisible={this.state.showUnitInfoModal}
                    unit={this.state.infoUnit}
                    serviceLocator={this.props.serviceLocator}
                    onClose={this.handleUnitInfoModalClose}
                />
            </>
        )
    }
} 