import React, {PureComponent} from "react";
import {BattleMapUnit} from "../battleMap/battleMapUnit";
import {IBattleUnitInfo, IServerAPI} from "../services/serverAPI";
import "./battleUnitInfoModal.css";

export interface IBattleUnitInfoModalProps {
    isVisible: boolean
    unit: BattleMapUnit | null
    battleId: string
    serverAPI: IServerAPI
    onClose: () => void
}

interface IBattleUnitInfoModalState {
    loading: boolean
    serverData: IBattleUnitInfo | null
    error: string | null
}

export class BattleUnitInfoModal extends PureComponent<IBattleUnitInfoModalProps, IBattleUnitInfoModalState> {
    state: IBattleUnitInfoModalState = {
        loading: false,
        serverData: null,
        error: null
    }

    componentDidUpdate(prevProps: IBattleUnitInfoModalProps) {
        // Fetch data when modal becomes visible or unit changes
        if (this.props.isVisible && this.props.unit) {
            if (!prevProps.isVisible || prevProps.unit?.id !== this.props.unit.id) {
                this.fetchUnitData()
            }
        }
    }

    async fetchUnitData() {
        if (!this.props.unit) return

        this.setState({ loading: true, error: null })
        
        try {
            const data = await this.props.serverAPI.getBattleUnitInfo(
                this.props.battleId,
                this.props.unit.id
            )
            this.setState({ serverData: data, loading: false })
        } catch (err) {
            this.setState({ 
                error: err instanceof Error ? err.message : 'Failed to load unit data',
                loading: false 
            })
        }
    }

    render() {
        if (!this.props.isVisible || !this.props.unit) {
            return null
        }

        const unit = this.props.unit
        const { loading, serverData, error } = this.state

        return (
            <div className="battle-unit-info-modal-overlay">
                <div className="battle-unit-info-modal">
                    <div className="battle-unit-info-modal-header">
                        <h2>Unit Information</h2>
                        <button className="close-button" onClick={this.props.onClose}>×</button>
                    </div>
                    <div className="battle-unit-info-modal-content">
                        {loading && <div className="loading-indicator">Loading...</div>}
                        {error && <div className="error-message">{error}</div>}
                        
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
                            <h4>Properties</h4>
                            <div className="properties-grid">
                                <div className="property-item">
                                    <span className="property-label">Unit ID:</span>
                                    <span className="property-value">{unit.id}</span>
                                </div>
                                <div className="property-item">
                                    <span className="property-label">Health:</span>
                                    <span className="property-value">
                                        {serverData ? serverData.props.health : unit.props.health}
                                        {serverData && serverData.currentProps.health !== serverData.props.health && (
                                            <span className={serverData.currentProps.health > serverData.props.health ? "buff-value" : "debuff-value"}>
                                                {" "}({serverData.currentProps.health})
                                            </span>
                                        )}
                                    </span>
                                </div>
                                <div className="property-item">
                                    <span className="property-label">Speed:</span>
                                    <span className="property-value">
                                        {serverData ? serverData.props.speed : unit.props.speed}
                                        {serverData && serverData.currentProps.speed !== serverData.props.speed && (
                                            <span className={serverData.currentProps.speed > serverData.props.speed ? "buff-value" : "debuff-value"}>
                                                {" "}({serverData.currentProps.speed})
                                            </span>
                                        )}
                                    </span>
                                </div>
                                <div className="property-item">
                                    <span className="property-label">Attack:</span>
                                    <span className="property-value">
                                        {serverData ? serverData.props.attack : unit.props.attack}
                                        {serverData && serverData.currentProps.attack !== serverData.props.attack && (
                                            <span className={serverData.currentProps.attack > serverData.props.attack ? "buff-value" : "debuff-value"}>
                                                {" "}({serverData.currentProps.attack})
                                            </span>
                                        )}
                                    </span>
                                </div>
                                <div className="property-item">
                                    <span className="property-label">Defense:</span>
                                    <span className="property-value">
                                        {serverData ? serverData.props.defense : unit.props.defense}
                                        {serverData && serverData.currentProps.defense !== serverData.props.defense && (
                                            <span className={serverData.currentProps.defense > serverData.props.defense ? "buff-value" : "debuff-value"}>
                                                {" "}({serverData.currentProps.defense})
                                            </span>
                                        )}
                                    </span>
                                </div>
                                <div className="property-item">
                                    <span className="property-label">Movement:</span>
                                    <span className="property-value">
                                        {serverData ? serverData.props.movementType : unit.props.movementType}
                                    </span>
                                </div>
                            </div>
                        </div>

                        <div className="unit-buffs">
                            <h4>Buffs ({(serverData?.buffs ?? unit.currentProps.buffs ?? []).length})</h4>
                            {(serverData?.buffs ?? unit.currentProps.buffs ?? []).length === 0 ? (
                                <div className="buff-empty">No active buffs</div>
                            ) : (
                                <div className="buffs-list">
                                    {(serverData?.buffs ?? unit.currentProps.buffs ?? []).map((buff, index) => (
                                        <div key={index} className="buff-item">
                                            {buff.thumbnailUrl && (
                                                <img 
                                                    src={buff.thumbnailUrl} 
                                                    alt={buff.name ?? 'Buff'} 
                                                    className="buff-thumbnail"
                                                />
                                            )}
                                            <span className="buff-name">
                                                {buff.name ?? 'Unknown buff'}
                                                {buff.permanent === false && ` (${buff.durationRemaining})`}
                                            </span>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>

                        <div className="unit-attacks">
                            <h4>Attacks ({(serverData?.currentProps.attacks ?? unit.currentProps.attacks).length})</h4>
                            {(serverData?.currentProps.attacks ?? unit.currentProps.attacks).map((attack, index) => {
                                const baseAttack = serverData?.props.attacks[index] ?? unit.props.attacks[index]
                                const attackState = unit.currentProps.attacksStates[index]
                                
                                const hasDamageModifier = attack.minDamage !== baseAttack.minDamage || attack.maxDamage !== baseAttack.maxDamage
                                const isDamageBuff = attack.minDamage > baseAttack.minDamage || attack.maxDamage > baseAttack.maxDamage
                                
                                return (
                                    <div key={index} className="attack-item">
                                        <div className="attack-header">
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
                                                    {hasDamageModifier && (
                                                        <span className={isDamageBuff ? "buff-value" : "debuff-value"}> ({attack.minDamage}-{attack.maxDamage})</span>
                                                    )}
                                                </span>
                                            </div>
                                            <div className="attack-stat">
                                                <span className="stat-label">Range:</span>
                                                <span className="stat-value">
                                                    {baseAttack.attackMinRange}-{baseAttack.attackMaxRange}
                                                </span>
                                            </div>
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
