import React, {PureComponent} from "react";
import {IServiceLocator} from "../services/serviceLocator";
import {IPlayerInfo, IUserUnit} from "../services/serverAPI";
import "./supplyComponent.css";

export interface ISupplyComponentProps {
    serviceLocator: IServiceLocator
    playerInfo: IPlayerInfo | null
    onClose: () => void
}

interface ISupplyComponentState {
    supplyUnits: IUserUnit[] | null
    armyUnits: IUserUnit[] | null
    isLoading: boolean
    error: string | null
}

export class SupplyComponent extends PureComponent<ISupplyComponentProps, ISupplyComponentState> {
    constructor(props: ISupplyComponentProps) {
        super(props)
        
        this.state = {
            supplyUnits: null,
            armyUnits: null,
            isLoading: true,
            error: null
        }
    }
    
    async componentDidMount() {
        await this.fetchData()
    }

    private async fetchData() {
        this.setState({ isLoading: true, error: null })
        try {
            const serverAPI = this.props.serviceLocator.serverAPI()
            const [supplyUnits, armyUnits] = await Promise.all([
                serverAPI.getSupplyUnits(),
                serverAPI.getArmyUnits()
            ])
            this.setState({ supplyUnits, armyUnits, isLoading: false })
        } catch (error) {
            console.error('Failed to fetch supply data:', error)
            this.setState({ 
                error: 'Failed to load supply data', 
                isLoading: false 
            })
        }
    }

    private renderSupplySlot(unit: IUserUnit | null, index: number) {
        if (unit) {
            return (
                <div key={index} className="supply-slot">
                    <img 
                        src={unit.thumbnailUrl} 
                        alt={`Supply Unit ${index + 1}`}
                        className="unit-image"
                    />
                    <div className="unit-count">{unit.count}</div>
                    <div className="unit-name">{unit.typeId}</div>
                </div>
            )
        } else {
            return (
                <div key={index} className="supply-slot empty">
                    <div className="empty-slot">Empty</div>
                </div>
            )
        }
    }

    private renderArmySlot(unit: IUserUnit | null, index: number) {
        if (unit) {
            return (
                <div key={index} className="army-slot">
                    <img 
                        src={unit.thumbnailUrl} 
                        alt={`Army Unit ${index + 1}`}
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
        const { supplyUnits, armyUnits, isLoading, error } = this.state
        const { playerInfo } = this.props

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

        // Use supplyCapacity from playerInfo, default to 10 if not available
        const supplyCapacity = playerInfo?.supplyCapacity || 10
        const armyCapacity = playerInfo?.armyCapacity || 7
        
        const supplySlots = new Array(supplyCapacity).fill(null)
        const armySlots = new Array(armyCapacity).fill(null)
        
        // Fill the slots with actual units
        if (supplyUnits) {
            supplyUnits.forEach((unit, index) => {
                if (index < supplySlots.length) {
                    supplySlots[index] = unit
                }
            })
        }
        
        if (armyUnits) {
            armyUnits.forEach((unit, index) => {
                if (index < armySlots.length) {
                    armySlots[index] = unit
                }
            })
        }

        return (
            <div className="supply-overlay">
                <div className="supply-modal">
                    <div className="supply-header">
                        <h1 className="supply-title">Supply Depot</h1>
                        <button className="close-button" onClick={this.props.onClose}>×</button>
                    </div>
                    
                    <div className="supply-content">
                        <div className="supply-section">
                            <h2 className="section-title">Supply Units ({supplyCapacity} slots)</h2>
                            <div className="supply-grid">
                                {supplySlots.map((unit, index) => this.renderSupplySlot(unit, index))}
                            </div>
                        </div>
                        
                        <div className="army-section">
                            <h2 className="section-title">Your Army ({armyCapacity} slots)</h2>
                            <div className="army-grid">
                                {armySlots.map((unit, index) => this.renderArmySlot(unit, index))}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        )
    }
} 