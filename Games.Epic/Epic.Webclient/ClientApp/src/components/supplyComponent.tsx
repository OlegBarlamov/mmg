import React, {PureComponent} from "react";
import {IServiceLocator} from "../services/serviceLocator";
import {IPlayerInfo, IUserUnit} from "../services/serverAPI";
import {SplitModal} from "./splitModal";
import {UnitInfoModal} from "./unitInfoModal";
import "./supplyComponent.css";

export interface ISupplyComponentProps {
    serviceLocator: IServiceLocator
    playerInfo: IPlayerInfo | null
    onClose: () => void
    armyUnits: IUserUnit[] | null
    armyCapacity: number
    onArmyUnitsUpdate?: (units: IUserUnit[]) => void
    onRefreshArmy?: () => Promise<void>
}

interface ISupplyComponentState {
    supplyUnits: IUserUnit[] | null
    supplyCapacity: number
    isLoading: boolean
    error: string | null
    selectedUnit: { unit: IUserUnit, containerType: 'supply' | 'army' } | null
    isDragging: boolean
    showSplitModal: boolean
    splitAmount: number
    splitTargetSlot: number | null
    splitTargetContainer: 'supply' | 'army' | null
    showUnitInfoModal: boolean
    infoUnit: IUserUnit | null
}

export class SupplyComponent extends PureComponent<ISupplyComponentProps, ISupplyComponentState> {
    constructor(props: ISupplyComponentProps) {
        super(props)
        
        this.state = {
            supplyUnits: null,
            supplyCapacity: 10,
            isLoading: true,
            error: null,
            selectedUnit: null,
            isDragging: false,
            showSplitModal: false,
            splitAmount: 1,
            splitTargetSlot: null,
            splitTargetContainer: null,
            showUnitInfoModal: false,
            infoUnit: null
        }
    }
    
    async componentDidMount() {
        await this.fetchData()
    }

    private async fetchData() {
        this.setState({ isLoading: true, error: null })
        try {
            const serverAPI = this.props.serviceLocator.serverAPI()
            if (!this.props.playerInfo?.supplyContainerId) {
                throw new Error('Supply container ID not available')
            }
            const supplyContainer = await serverAPI.getUnitsContainer(this.props.playerInfo.supplyContainerId)
            this.setState({ 
                supplyUnits: supplyContainer.units, 
                supplyCapacity: supplyContainer.capacity,
                isLoading: false 
            })
        } catch (error) {
            console.error('Failed to fetch supply data:', error)
            this.setState({ 
                error: 'Failed to load supply data', 
                isLoading: false 
            })
        }
    }

    private handleUnitClick = (unit: IUserUnit, containerType: 'supply' | 'army') => {
        // Check if we're in split mode (selected unit exists but not dragging)
        if (this.state.selectedUnit && !this.state.isDragging) {
            // We're in split mode, check if this is a valid target
            const canBeSplitTarget = this.state.selectedUnit.unit.typeId === unit.typeId
            if (canBeSplitTarget) {
                this.handleSplitTargetSelect(unit.slotIndex, containerType)
            }
        } else if (this.state.selectedUnit) {
            // If we have a selected unit and are dragging, try to move it to this slot
            this.moveUnitToSlot(unit.slotIndex, containerType)
        } else {
            // Select this unit for moving
            this.setState({ selectedUnit: { unit, containerType }, isDragging: true })
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

    private handleSplitClick = (unit: IUserUnit, containerType: 'supply' | 'army') => {
        this.setState({ 
            selectedUnit: { unit, containerType }, 
            isDragging: false,
            showSplitModal: false,
            splitAmount: Math.max(1, Math.floor(unit.count / 2)),
            splitTargetSlot: null,
            splitTargetContainer: null
        })
    }

    private handleEmptySlotClick = (slotIndex: number, containerType: 'supply' | 'army') => {
        // Check if we're in split mode (selected unit exists but not dragging)
        if (this.state.selectedUnit && !this.state.isDragging) {
            // We're in split mode, empty slots are always valid targets
            this.handleSplitTargetSelect(slotIndex, containerType)
        } else if (this.state.selectedUnit) {
            // If we have a selected unit and are dragging, try to move it to this slot
            this.moveUnitToSlot(slotIndex, containerType)
        }
    }

    private moveUnitToSlot = async (targetSlotIndex: number, targetContainerType: 'supply' | 'army') => {
        if (!this.state.selectedUnit) return

        const { unit: selectedUnit, containerType: sourceContainerType } = this.state.selectedUnit
        
        // Don't allow moving to the same slot
        if (sourceContainerType === targetContainerType && selectedUnit.slotIndex === targetSlotIndex) {
            this.setState({ selectedUnit: null, isDragging: false })
            return
        }

        try {
            const serverAPI = this.props.serviceLocator.serverAPI()
            const containerId = targetContainerType === 'supply' 
                ? this.props.playerInfo?.supplyContainerId 
                : this.props.playerInfo?.armyContainerId

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

            // Update the target container state
            if (targetContainerType === 'supply') {
                this.setState({ 
                    supplyUnits: updatedContainer.units,
                    selectedUnit: null, 
                    isDragging: false 
                })
            } else {
                // Update parent's army units state
                this.props.onArmyUnitsUpdate?.(updatedContainer.units)
                this.setState({ 
                    selectedUnit: null, 
                    isDragging: false 
                })
            }

            // If moving between containers, refresh the source container
            if (sourceContainerType !== targetContainerType) {
                await this.refreshSourceContainer(sourceContainerType)
            }

        } catch (error) {
            console.error('Failed to move unit:', error)
            this.setState({ 
                error: 'Failed to move unit', 
                selectedUnit: null, 
                isDragging: false 
            })
        }
    }

    private refreshSourceContainer = async (containerType: 'supply' | 'army') => {
        try {
            if (containerType === 'supply') {
                const serverAPI = this.props.serviceLocator.serverAPI()
                const containerId = this.props.playerInfo?.supplyContainerId

                if (!containerId) return

                const container = await serverAPI.getUnitsContainer(containerId)
                this.setState({ supplyUnits: container.units })
            } else {
                // Use the callback to refresh army units from parent
                await this.props.onRefreshArmy?.()
            }
        } catch (error) {
            console.error('Failed to refresh container:', error)
        }
    }

    private handleSplitTargetSelect = (slotIndex: number, containerType: 'supply' | 'army') => {
        this.setState({ 
            splitTargetSlot: slotIndex, 
            splitTargetContainer: containerType,
            showSplitModal: true
        })
    }

    private handleSplitModalClose = () => {
        this.setState({ 
            showSplitModal: false, 
            selectedUnit: null,
            splitAmount: 1,
            splitTargetSlot: null,
            splitTargetContainer: null
        })
    }

    private handleSplitAmountChange = (amount: number) => {
        this.setState({ splitAmount: amount })
    }

    private handleSplitComplete = (updatedContainer: any) => {
        const targetContainerType = this.state.splitTargetContainer
        const sourceContainerType = this.state.selectedUnit?.containerType

        // Update the target container state
        if (targetContainerType === 'supply') {
            this.setState({ 
                supplyUnits: updatedContainer.units,
                showSplitModal: false,
                selectedUnit: null,
                splitAmount: 1,
                splitTargetSlot: null,
                splitTargetContainer: null
            })
        } else {
            // Update parent's army units state
            this.props.onArmyUnitsUpdate?.(updatedContainer.units)
            this.setState({ 
                showSplitModal: false,
                selectedUnit: null,
                splitAmount: 1,
                splitTargetSlot: null,
                splitTargetContainer: null
            })
        }

        // If moving between containers, refresh the source container
        if (sourceContainerType && sourceContainerType !== targetContainerType) {
            this.refreshSourceContainer(sourceContainerType)
        }
    }

    private handleSplitError = (error: string) => {
        this.setState({ 
            error: error, 
            showSplitModal: false,
            selectedUnit: null,
            splitAmount: 1,
            splitTargetSlot: null,
            splitTargetContainer: null
        })
    }

    private renderSupplySlot(unit: IUserUnit | null, index: number) {
        const isSelected = this.state.selectedUnit?.unit.id === unit?.id && this.state.selectedUnit?.containerType === 'supply'
        const isTarget = this.state.isDragging && !isSelected
        const isSplitTarget = this.state.showSplitModal && 
            this.state.splitTargetSlot === index && 
            this.state.splitTargetContainer === 'supply'
        const canBeSplitTarget = this.state.showSplitModal && unit && 
            this.state.selectedUnit?.unit.typeId === unit.typeId
        
        if (unit) {
            return (
                <div 
                    key={index} 
                    className={`supply-slot ${isSelected ? 'selected' : ''} ${isTarget ? 'target' : ''} ${isSplitTarget ? 'split-target' : ''} ${canBeSplitTarget ? 'split-valid' : ''}`}
                    onClick={() => this.handleUnitClick(unit, 'supply')}
                >
                    <img 
                        src={unit.thumbnailUrl} 
                        alt={`Supply Unit ${index + 1}`}
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
                    className={`supply-slot empty ${isTarget ? 'target' : ''} ${isSplitTarget ? 'split-target' : ''}`}
                    onClick={() => this.handleEmptySlotClick(index, 'supply')}
                >
                    <div className="empty-slot">Empty</div>
                </div>
            )
        }
    }

    private renderArmySlot(unit: IUserUnit | null, index: number) {
        const isSelected = this.state.selectedUnit?.unit.id === unit?.id && this.state.selectedUnit?.containerType === 'army'
        const isTarget = this.state.isDragging && !isSelected
        const isSplitTarget = this.state.showSplitModal && 
            this.state.splitTargetSlot === index && 
            this.state.splitTargetContainer === 'army'
        const canBeSplitTarget = this.state.showSplitModal && unit && 
            this.state.selectedUnit?.unit.typeId === unit.typeId
        
        if (unit) {
            return (
                <div 
                    key={index} 
                    className={`army-slot ${isSelected ? 'selected' : ''} ${isTarget ? 'target' : ''} ${isSplitTarget ? 'split-target' : ''} ${canBeSplitTarget ? 'split-valid' : ''}`}
                    onClick={() => this.handleUnitClick(unit, 'army')}
                >
                    <img 
                        src={unit.thumbnailUrl} 
                        alt={`Army Unit ${index + 1}`}
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
                    className={`army-slot empty ${isTarget ? 'target' : ''} ${isSplitTarget ? 'split-target' : ''}`}
                    onClick={() => this.handleEmptySlotClick(index, 'army')}
                >
                    <div className="empty-slot">Empty</div>
                </div>
            )
        }
    }

    render() {
        const { supplyUnits, isLoading, error } = this.state
        const { playerInfo, armyUnits, armyCapacity } = this.props

        if (isLoading) {
            return (
                <div className="supply-overlay">
                    <div className="supply-modal">
                        <div className="supply-header">
                            <h1 className="supply-title">Supply Depot</h1>
                            <button className="close-button" onClick={this.props.onClose}>×</button>
                        </div>
                        <div className="supply-loading">Loading supply data...</div>
                    </div>
                </div>
            )
        }

        if (error) {
            return (
                <div className="supply-overlay">
                    <div className="supply-modal">
                        <div className="supply-header">
                            <h1 className="supply-title">Supply Depot</h1>
                            <button className="close-button" onClick={this.props.onClose}>×</button>
                        </div>
                        <div className="supply-error">{error}</div>
                    </div>
                </div>
            )
        }

        // Use capacities from state and props
        const supplyCapacity = this.state.supplyCapacity
        
        const supplySlots = new Array(supplyCapacity).fill(null)
        const armySlots = new Array(armyCapacity).fill(null)
        
        // Fill the slots with actual units using slotIndex from server
        if (supplyUnits) {
            supplyUnits.forEach((unit) => {
                if (unit.slotIndex >= 0 && unit.slotIndex < supplySlots.length) {
                    supplySlots[unit.slotIndex] = unit
                }
            })
        }
        
        if (armyUnits) {
            armyUnits.forEach((unit) => {
                if (unit.slotIndex >= 0 && unit.slotIndex < armySlots.length) {
                    armySlots[unit.slotIndex] = unit
                }
            })
        }

        return (
            <>
                <div className="supply-overlay">
                    <div className="supply-modal">
                        <div className="supply-header">
                            <h1 className="supply-title">Supply Depot</h1>
                            <div className="header-buttons">
                                {this.state.isDragging && (
                                    <button className="cancel-button" onClick={() => this.setState({ selectedUnit: null, isDragging: false })}>
                                        Cancel
                                    </button>
                                )}
                                <button className="close-button" onClick={this.props.onClose}>×</button>
                            </div>
                        </div>
                        
                        <div className="supply-content">
                            <div className="supply-section">
                                <div className="section-header">
                                    <h2 className="section-title">Supply Units ({supplyCapacity} slots)</h2>
                                    {this.renderSplitButton('supply')}
                                </div>
                                <div className="supply-grid">
                                    {supplySlots.map((unit, index) => this.renderSupplySlot(unit, index))}
                                </div>
                            </div>
                            
                            <div className="army-section">
                                <div className="section-header">
                                    <h2 className="section-title">Your Army ({armyCapacity} slots)</h2>
                                    {this.renderSplitButton('army')}
                                </div>
                                <div className="army-grid">
                                    {armySlots.map((unit, index) => this.renderArmySlot(unit, index))}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <SplitModal
                    isVisible={this.state.showSplitModal}
                    selectedUnit={this.state.selectedUnit?.unit || null}
                    splitAmount={this.state.splitAmount}
                    splitTargetSlot={this.state.splitTargetSlot}
                    splitTargetContainer={this.state.splitTargetContainer}
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

    private renderSplitButton(containerType: 'supply' | 'army') {
        const isSelected = this.state.selectedUnit?.containerType === containerType
        const selectedUnit = this.state.selectedUnit?.unit
        const isDisabled = !isSelected || !selectedUnit || selectedUnit.count <= 1

        return (
            <button 
                className={`split-button ${isDisabled ? 'disabled' : ''}`}
                onClick={() => {
                    if (!isDisabled && selectedUnit) {
                        this.handleSplitClick(selectedUnit, containerType)
                    }
                }}
                disabled={isDisabled}
            >
                Split
            </button>
        )
    }
} 