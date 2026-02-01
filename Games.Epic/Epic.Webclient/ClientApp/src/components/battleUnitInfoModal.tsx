import React, {PureComponent} from "react";
import {BattleMapUnit} from "../battleMap/battleMapUnit";
import "./battleUnitInfoModal.css";

export interface IBattleUnitInfoModalProps {
    isVisible: boolean
    unit: BattleMapUnit | null
    onClose: () => void
}

export class BattleUnitInfoModal extends PureComponent<IBattleUnitInfoModalProps> {
    render() {
        if (!this.props.isVisible || !this.props.unit) {
            return null
        }

        const unit = this.props.unit

        return (
            <div className="battle-unit-info-modal-overlay">
                <div className="battle-unit-info-modal">
                    <div className="battle-unit-info-modal-header">
                        <h2>Unit Information</h2>
                        <button className="close-button" onClick={this.props.onClose}>×</button>
                    </div>
                    <div className="battle-unit-info-modal-content">
                        <div className="unit-info-main">
                            <div className="unit-image-container">
                                <img 
                                    src={unit.currentProps.battleImgUrl} 
                                    alt="Unit" 
                                    className="unit-info-image"
                                />
                            </div>
                            <div className="unit-basic-info">
                                <h3 className="unit-name-1">{unit.name}</h3>
                                <div className="unit-count">Count: {unit.count}</div>
                                <div className="unit-position">Position: ({unit.position.r}, {unit.position.c})</div>
                                <div className="unit-player">Player: {unit.player}</div>
                                {unit.currentProps.waited && (
                                    <div className="unit-waited">Status: Waited</div>
                                )}
                            </div>
                        </div>
                        
                        <div className="unit-properties">
                            <h4>Base Properties</h4>
                            <div className="properties-grid">
                                <div className="property-item">
                                    <span className="property-label">Unit ID:</span>
                                    <span className="property-value">{unit.id}</span>
                                </div>
                                <div className="property-item">
                                    <span className="property-label">Health:</span>
                                    <span className="property-value">
                                        {unit.props.health}
                                        {unit.currentProps.health !== unit.props.health && (
                                            <span className="current-value"> ({unit.currentProps.health})</span>
                                        )}
                                    </span>
                                </div>
                                <div className="property-item">
                                    <span className="property-label">Speed:</span>
                                    <span className="property-value">
                                        {unit.props.speed}
                                        {unit.currentProps.speed !== unit.props.speed && (
                                            <span className="current-value"> ({unit.currentProps.speed})</span>
                                        )}
                                    </span>
                                </div>
                                <div className="property-item">
                                    <span className="property-label">Attack:</span>
                                    <span className="property-value">
                                        {unit.props.attack}
                                        {unit.currentProps.attack !== unit.props.attack && (
                                            <span className="current-value"> ({unit.currentProps.attack})</span>
                                        )}
                                    </span>
                                </div>
                                <div className="property-item">
                                    <span className="property-label">Defense:</span>
                                    <span className="property-value">
                                        {unit.props.defense}
                                        {unit.currentProps.defense !== unit.props.defense && (
                                            <span className="current-value"> ({unit.currentProps.defense})</span>
                                        )}
                                    </span>
                                </div>
                                <div className="property-item">
                                    <span className="property-label">Movement:</span>
                                    <span className="property-value">
                                        {unit.props.movementType}
                                        {unit.currentProps.movementType !== unit.props.movementType && (
                                            <span className="current-value"> ({unit.currentProps.movementType})</span>
                                        )}
                                    </span>
                                </div>
                            </div>
                        </div>

                        <div className="unit-buffs">
                            <h4>Buffs ({(unit.currentProps.buffs ?? []).length})</h4>
                            {(unit.currentProps.buffs ?? []).length === 0 ? (
                                <div className="buff-empty">No active buffs</div>
                            ) : (
                                <div className="buffs-list">
                                    {(unit.currentProps.buffs ?? []).map((buff, index) => (
                                        <div key={index} className="buff-item">
                                            {buff.thumbnailUrl && (
                                                <img 
                                                    src={buff.thumbnailUrl} 
                                                    alt={buff.name} 
                                                    className="buff-thumbnail"
                                                />
                                            )}
                                            <span className="buff-name">
                                                {buff.name}{!buff.permanent && ` (${buff.durationRemaining})`}
                                            </span>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>

                        <div className="unit-attacks">
                            <h4>Attacks ({unit.currentProps.attacks.length})</h4>
                            {unit.currentProps.attacks.map((attack, index) => {
                                const baseAttack = unit.props.attacks[index]
                                const attackState = unit.currentProps.attacksStates[index]
                                
                                return (
                                    <div key={index} className="attack-item">
                                        <div className="attack-header">
                                            <img 
                                                src={attack.thumbnailUrl} 
                                                alt={attack.name} 
                                                className="attack-thumbnail"
                                            />
                                            <span className="attack-name">{attack.name}</span>
                                            {attackState && (
                                                <span className="attack-state">
                                                    Bullets: {attackState.bulletsCount} | 
                                                    Counterattacks: {attackState.counterattacksUsed}
                                                </span>
                                            )}
                                        </div>
                                        <div className="attack-stats">
                                            <div className="attack-stat">
                                                <span className="stat-label">Damage:</span>
                                                <span className="stat-value">
                                                    {baseAttack.minDamage}-{baseAttack.maxDamage}
                                                    {(attack.minDamage !== baseAttack.minDamage || attack.maxDamage !== baseAttack.maxDamage) && (
                                                        <span className="current-value"> ({attack.minDamage}-{attack.maxDamage})</span>
                                                    )}
                                                </span>
                                            </div>
                                            <div className="attack-stat">
                                                <span className="stat-label">Range:</span>
                                                <span className="stat-value">
                                                    {baseAttack.attackMinRange}-{baseAttack.attackMaxRange}
                                                    {(attack.attackMinRange !== baseAttack.attackMinRange || attack.attackMaxRange !== baseAttack.attackMaxRange) && (
                                                        <span className="current-value"> ({attack.attackMinRange}-{attack.attackMaxRange})</span>
                                                    )}
                                                </span>
                                            </div>
                                            <div className="attack-stat">
                                                <span className="stat-label">Stay Only:</span>
                                                <span className="stat-value">
                                                    {baseAttack.stayOnly ? 'Yes' : 'No'}
                                                    {attack.stayOnly !== baseAttack.stayOnly && (
                                                        <span className="current-value"> ({attack.stayOnly ? 'Yes' : 'No'})</span>
                                                    )}
                                                </span>
                                            </div>
                                            <div className="attack-stat">
                                                <span className="stat-label">Counterattacks Count:</span>
                                                <span className="stat-value">
                                                    {baseAttack.counterattacksCount}
                                                    {attack.counterattacksCount !== baseAttack.counterattacksCount && (
                                                        <span className="current-value"> ({attack.counterattacksCount})</span>
                                                    )}
                                                </span>
                                            </div>
                                            <div className="attack-stat">
                                                <span className="stat-label">Counterattack Penalty:</span>
                                                <span className="stat-value">
                                                    {baseAttack.counterattackPenaltyPercentage}%
                                                    {attack.counterattackPenaltyPercentage !== baseAttack.counterattackPenaltyPercentage && (
                                                        <span className="current-value"> ({attack.counterattackPenaltyPercentage}%)</span>
                                                    )}
                                                </span>
                                            </div>
                                            <div className="attack-stat">
                                                <span className="stat-label">Range Penalty:</span>
                                                <span className="stat-value">
                                                    {baseAttack.rangePenalty ? 'Yes' : 'No'}
                                                    {attack.rangePenalty !== baseAttack.rangePenalty && (
                                                        <span className="current-value"> ({attack.rangePenalty ? 'Yes' : 'No'})</span>
                                                    )}
                                                </span>
                                            </div>
                                            {baseAttack.rangePenalty && (
                                                <div className="attack-stat">
                                                    <span className="stat-label">Range Penalty Zones:</span>
                                                    <span className="stat-value">
                                                        {baseAttack.rangePenaltyZonesCount}
                                                        {attack.rangePenaltyZonesCount !== baseAttack.rangePenaltyZonesCount && (
                                                            <span className="current-value"> ({attack.rangePenaltyZonesCount})</span>
                                                        )}
                                                    </span>
                                                </div>
                                            )}
                                            <div className="attack-stat">
                                                <span className="stat-label">Enemy in Range Disables:</span>
                                                <span className="stat-value">
                                                    {baseAttack.enemyInRangeDisablesAttack ? 'Yes' : 'No'}
                                                    {attack.enemyInRangeDisablesAttack !== baseAttack.enemyInRangeDisablesAttack && (
                                                        <span className="current-value"> ({attack.enemyInRangeDisablesAttack ? 'Yes' : 'No'})</span>
                                                    )}
                                                </span>
                                            </div>
                                            {attack.applyBuffs && attack.applyBuffs.length > 0 && (
                                                <div className="attack-stat">
                                                    <span className="stat-label">Applies:</span>
                                                    <span className="stat-value attack-buffs-list">
                                                        {attack.applyBuffs.map((buff, i) => (
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
                                )
                            })}
                        </div>

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
