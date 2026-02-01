import React, {PureComponent} from "react";
import {IUserUnit, IUnitTypeInfo} from "../services/serverAPI";
import {IServiceLocator} from "../services/serviceLocator";
import "./unitInfoModal.css";

export interface IUnitInfoModalProps {
    isVisible: boolean
    unit: IUserUnit | null
    serviceLocator: IServiceLocator
    onClose: () => void
}

interface IUnitInfoModalState {
    unitInfo: IUnitTypeInfo | null
    isLoading: boolean
    error: string | null
}

export class UnitInfoModal extends PureComponent<IUnitInfoModalProps, IUnitInfoModalState> {
    constructor(props: IUnitInfoModalProps) {
        super(props)
        
        this.state = {
            unitInfo: null,
            isLoading: false,
            error: null
        }
    }

    componentDidUpdate(prevProps: IUnitInfoModalProps) {
        // When modal becomes visible and we have a unit, fetch the unit info
        if (this.props.isVisible && !prevProps.isVisible && this.props.unit) {
            this.fetchUnitInfo()
        }
        
        // Reset state when modal closes
        if (!this.props.isVisible && prevProps.isVisible) {
            this.setState({
                unitInfo: null,
                isLoading: false,
                error: null
            })
        }
    }

    private async fetchUnitInfo() {
        if (!this.props.unit) return

        this.setState({ isLoading: true, error: null })
        
        try {
            const serverAPI = this.props.serviceLocator.serverAPI()
            const unitInfo = await serverAPI.getUnitTypeInfo(this.props.unit.typeId)
            this.setState({ unitInfo, isLoading: false })
        } catch (error) {
            console.error('Failed to fetch unit info:', error)
            this.setState({ 
                error: 'Failed to load unit information', 
                isLoading: false 
            })
        }
    }

    private renderUnitStats() {
        const { unitInfo, isLoading, error } = this.state

        if (isLoading) {
            return (
                <div className="unit-stats-loading">
                    Loading unit statistics...
                </div>
            )
        }

        if (error) {
            return (
                <div className="unit-stats-error">
                    {error}
                </div>
            )
        }

        if (!unitInfo) {
            return (
                <div className="unit-stats-error">
                    No unit information available
                </div>
            )
        }

        return (
            <div className="unit-stats">
                <h4>Battle Statistics</h4>
                <div className="stats-grid">
                    <div className="stat-item">
                        <span className="stat-label">Health:</span>
                        <span className="stat-value">{unitInfo.health}</span>
                    </div>
                    <div className="stat-item">
                        <span className="stat-label">Speed:</span>
                        <span className="stat-value">{unitInfo.speed}</span>
                    </div>
                    <div className="stat-item">
                        <span className="stat-label">Attack:</span>
                        <span className="stat-value">{unitInfo.attack}</span>
                    </div>
                    <div className="stat-item">
                        <span className="stat-label">Defense:</span>
                        <span className="stat-value">{unitInfo.defense}</span>
                    </div>
                    <div className="stat-item">
                        <span className="stat-label">Movement:</span>
                        <span className="stat-value">{unitInfo.movementType}</span>
                    </div>
                    {unitInfo.attacks && unitInfo.attacks.length > 0 && (
                        <div className="attacks-section">
                            <h5>Attacks ({unitInfo.attacks.length})</h5>
                            {unitInfo.attacks.map((attack, index) => (
                                <div key={index} className="attack-item">
                                    <div className="attack-header">
                                        <img 
                                            src={attack.thumbnailUrl} 
                                            alt={attack.name} 
                                            className="attack-thumbnail"
                                        />
                                        <span className="attack-name">{attack.name}</span>
                                    </div>
                                    <div className="attack-stats">
                                        <div className="attack-stat">
                                            <span className="stat-label">Damage:</span>
                                            <span className="stat-value">{attack.minDamage}-{attack.maxDamage}</span>
                                        </div>
                                        <div className="attack-stat">
                                            <span className="stat-label">Range:</span>
                                            <span className="stat-value">{attack.attackMinRange}-{attack.attackMaxRange}</span>
                                        </div>
                                        <div className="attack-stat">
                                            <span className="stat-label">Stay Only:</span>
                                            <span className="stat-value">{attack.stayOnly ? 'Yes' : 'No'}</span>
                                        </div>
                                        <div className="attack-stat">
                                            <span className="stat-label">Counterattack:</span>
                                            <span className="stat-value">{attack.counterattacksCount ? 'Allowed' : 'Not Allowed'}</span>
                                        </div>
                                        {attack.counterattacksCount && (
                                            <div className="attack-stat">
                                                <span className="stat-label">Counterattack Penalty:</span>
                                                <span className="stat-value">{attack.counterattackPenaltyPercentage}%</span>
                                            </div>
                                        )}
                                        <div className="attack-stat">
                                            <span className="stat-label">Range Penalty:</span>
                                            <span className="stat-value">{attack.rangePenalty ? 'Yes' : 'No'}</span>
                                        </div>
                                        {attack.rangePenalty && (
                                            <div className="attack-stat">
                                                <span className="stat-label">Range Penalty Zones:</span>
                                                <span className="stat-value">{attack.rangePenaltyZonesCount}</span>
                                            </div>
                                        )}
                                        <div className="attack-stat">
                                            <span className="stat-label">Enemy in Range Disables:</span>
                                            <span className="stat-value">{attack.enemyInRangeDisablesAttack ? 'Yes' : 'No'}</span>
                                        </div>
                                        {attack.applyBuffs && attack.applyBuffs.length > 0 && (
                                            <div className="attack-stat">
                                                <span className="stat-label">Applies:</span>
                                                <span className="stat-value attack-buffs-list">
                                                    {attack.applyBuffs.map((buff: any, i: number) => (
                                                        <span key={i} className="attack-buff-item">
                                                            {buff.thumbnailUrl && (
                                                                <img 
                                                                    src={buff.thumbnailUrl} 
                                                                    alt={buff.name} 
                                                                    className="buff-thumbnail-small"
                                                                />
                                                            )}
                                                            {buff.name}{!buff.permanent && ` (${buff.duration})`}
                                                            {i < attack.applyBuffs!.length - 1 && ', '}
                                                        </span>
                                                    ))}
                                                </span>
                                            </div>
                                        )}
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        )
    }

    render() {
        if (!this.props.isVisible || !this.props.unit) {
            return null
        }

        const unit = this.props.unit

        return (
            <div className="unit-info-modal-overlay">
                <div className="unit-info-modal">
                    <div className="unit-info-modal-header">
                        <h2>Unit Information</h2>
                        <button className="close-button" onClick={this.props.onClose}>×</button>
                    </div>
                    <div className="unit-info-modal-content">
                        <div className="unit-info-main">
                            <div className="unit-image-container">
                                <img 
                                    src={unit.thumbnailUrl} 
                                    alt={unit.typeId} 
                                    className="unit-info-image"
                                />
                            </div>
                            <div className="unit-basic-info">
                                <h3 className="unit-name-1">{unit.name}</h3>
                                <div className="unit-count">Count: {unit.count}</div>
                                <div className="unit-slot">Slot: {unit.slotIndex + 1}</div>
                            </div>
                        </div>
                        
                        <div className="unit-properties">
                            <h4>Properties</h4>
                            <div className="properties-grid">
                                <div className="property-item">
                                    <span className="property-label">Type ID:</span>
                                    <span className="property-value">{unit.typeId}</span>
                                </div>
                                <div className="property-item">
                                    <span className="property-label">Unit ID:</span>
                                    <span className="property-value">{unit.id}</span>
                                </div>
                                <div className="property-item">
                                    <span className="property-label">Count:</span>
                                    <span className="property-value">{unit.count}</span>
                                </div>
                                <div className="property-item">
                                    <span className="property-label">Slot Index:</span>
                                    <span className="property-value">{unit.slotIndex}</span>
                                </div>
                            </div>
                        </div>

                        {this.renderUnitStats()}

                        <div className="unit-actions">
                            <button 
                                className="close-info-button"
                                onClick={this.props.onClose}
                            >
                                Close
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        )
    }
}
