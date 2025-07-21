import React, {PureComponent} from "react";
import {IServiceLocator} from "../services/serviceLocator";
import {IPlayerInfo, IUserUnit} from "../services/serverAPI";
import "./armyDisplay.css";

export interface IArmyDisplayProps {
    serviceLocator: IServiceLocator
    playerInfo: IPlayerInfo | null
    onSupplyClick?: () => void
    armyUnits: IUserUnit[] | null
    armyCapacity: number
    onArmyUnitsUpdate?: (units: IUserUnit[]) => void
}

interface IArmyDisplayState {
    isLoading: boolean
    error: string | null
    selectedUnit: IUserUnit | null
    isDragging: boolean
}

export class ArmyDisplay extends PureComponent<IArmyDisplayProps, IArmyDisplayState> {
    constructor(props: IArmyDisplayProps) {
        super(props)
        
        this.state = {
            isLoading: true,
            error: null,
            selectedUnit: null,
            isDragging: false
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
        if (this.state.selectedUnit) {
            // If we have a selected unit, try to move it to this slot
            this.moveUnitToSlot(unit.slotIndex)
        } else {
            // Select this unit for moving
            this.setState({ selectedUnit: unit, isDragging: true })
        }
    }

    private handleEmptySlotClick = (slotIndex: number) => {
        if (this.state.selectedUnit) {
            this.moveUnitToSlot(slotIndex)
        }
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
        
        if (unit) {
            return (
                <div 
                    key={index} 
                    className={`army-slot ${isSelected ? 'selected' : ''} ${isTarget ? 'target' : ''}`}
                    onClick={() => this.handleUnitClick(unit)}
                >
                    <img 
                        src={unit.thumbnailUrl} 
                        alt={`Unit ${index + 1}`}
                        className="unit-image"
                    />
                    <div className="unit-count">{unit.count}</div>
                </div>
            )
        } else {
            return (
                <div 
                    key={index} 
                    className={`army-slot empty ${isTarget ? 'target' : ''}`}
                    onClick={() => this.handleEmptySlotClick(index)}
                >
                    <div className="empty-slot">Empty</div>
                </div>
            )
        }
    }

    render() {
        const { isLoading, error } = this.state
        const { playerInfo, armyUnits, armyCapacity } = this.props

        if (isLoading) {
            return (
                <div className="army-section">
                    <div className="army-header">
                        <h2 className="army-title">Your Army</h2>
                        <button 
                            className="supply-button"
                            onClick={this.props.onSupplyClick}
                            disabled={!this.props.onSupplyClick}
                        >
                            Supply
                        </button>
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
                        <button 
                            className="supply-button"
                            onClick={this.props.onSupplyClick}
                            disabled={!this.props.onSupplyClick}
                        >
                            Supply
                        </button>
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
            <div className="army-section">
                <div className="army-header">
                    <h2 className="army-title">Your Army ({armyCapacity} slots)</h2>
                    <button 
                        className="supply-button"
                        onClick={this.props.onSupplyClick}
                        disabled={!this.props.onSupplyClick}
                    >
                        Supply
                    </button>
                </div>
                <div className="army-grid">
                    {armySlots.map((unit, index) => this.renderArmySlot(unit, index))}
                </div>
            </div>
        )
    }
} 