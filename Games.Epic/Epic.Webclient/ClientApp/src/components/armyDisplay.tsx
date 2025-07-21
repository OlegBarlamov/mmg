import React, {PureComponent} from "react";
import {IServiceLocator} from "../services/serviceLocator";
import {IPlayerInfo} from "../services/serverAPI";
import "./armyDisplay.css";

export interface IArmyDisplayProps {
    serviceLocator: IServiceLocator
    playerInfo: IPlayerInfo | null
    onSupplyClick?: () => void
}

interface IArmyDisplayState {
    armyUnits: any[] | null
    isLoading: boolean
    error: string | null
}

export class ArmyDisplay extends PureComponent<IArmyDisplayProps, IArmyDisplayState> {
    constructor(props: IArmyDisplayProps) {
        super(props)
        
        this.state = {
            armyUnits: null,
            isLoading: true,
            error: null
        }
    }
    
    async componentDidMount() {
        await this.fetchArmyUnits()
    }

    // Expose this method so parent can call it
    public async refreshArmy() {
        await this.fetchArmyUnits()
    }

    private async fetchArmyUnits() {
        this.setState({ isLoading: true, error: null })
        try {
            const serverAPI = this.props.serviceLocator.serverAPI()
            const armyUnits = await serverAPI.getArmyUnits()
            this.setState({ armyUnits, isLoading: false })
        } catch (error) {
            console.error('Failed to fetch army units:', error)
            this.setState({ 
                error: 'Failed to load army units', 
                isLoading: false 
            })
        }
    }

    private renderArmySlot(unit: any, index: number) {
        if (unit) {
            return (
                <div key={index} className="army-slot">
                    <img 
                        src={unit.thumbnailUrl} 
                        alt={`Unit ${index + 1}`}
                        className="unit-image"
                    />
                    <div className="unit-count">{unit.count}</div>
                    <div className="unit-name">{unit.typeId}</div>
                </div>
            )
        } else {
            return (
                <div key={index} className="army-slot empty">
                    <div className="empty-slot">Empty</div>
                </div>
            )
        }
    }

    render() {
        const { armyUnits, isLoading, error } = this.state
        const { playerInfo } = this.props

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

        // Use armyCapacity from playerInfo, default to 7 if not available
        const armyCapacity = playerInfo?.armyCapacity || 7
        const armySlots = new Array(armyCapacity).fill(null)
        
        // Fill the slots with actual units
        if (armyUnits) {
            armyUnits.forEach((unit, index) => {
                if (index < armySlots.length) {
                    armySlots[index] = unit
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