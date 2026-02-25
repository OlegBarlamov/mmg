import './magicBookModal.css';
import React, { PureComponent } from 'react';
import { IServiceLocator } from '../services/serviceLocator';
import { IKnownMagicInfo, IKnownMagicBuffEntry, IKnownMagicEffectEntry } from '../services/serverAPI';

export interface IMagicBookModalProps {
    isVisible: boolean;
    serviceLocator: IServiceLocator;
    onClose: () => void;
    /** When set (e.g. in battle), clicking a spell in the list casts it. */
    onCastMagic?: (magicTypeId: string, options?: { castTargetType?: string; effectRadius?: number; targetUnitId?: string; targetRow?: number; targetColumn?: number }) => void;
    /** Whether casting is allowed (e.g. one magic per round). */
    canCast?: boolean;
    /** Current hero mana (when in battle). Spells with cost > currentMana are greyed out. */
    currentMana?: number;
}

interface IMagicBookModalState {
    isLoading: boolean;
    errorMessage: string | null;
    magics: IKnownMagicInfo[];
    selectedMagic: IKnownMagicInfo | null;
}

export class MagicBookModal extends PureComponent<IMagicBookModalProps, IMagicBookModalState> {
    constructor(props: IMagicBookModalProps) {
        super(props);
        this.state = {
            isLoading: false,
            errorMessage: null,
            magics: [],
            selectedMagic: null,
        };
    }

    componentDidUpdate(prevProps: IMagicBookModalProps) {
        if (this.props.isVisible && !prevProps.isVisible) {
            this.loadMagics();
        }
    }

    private async loadMagics() {
        this.setState({ isLoading: true, errorMessage: null, selectedMagic: null });
        try {
            const serverAPI = this.props.serviceLocator.serverAPI();
            const magics = await serverAPI.getHeroKnownMagic();
            const selectedMagic = magics.length > 0 ? magics[0] : null;
            this.setState({ magics, selectedMagic, isLoading: false });
        } catch (e) {
            this.setState({
                isLoading: false,
                errorMessage: e instanceof Error ? e.message : 'Failed to load magic',
            });
        }
    }

    render() {
        const { isVisible, onClose } = this.props;
        const { isLoading, errorMessage, magics, selectedMagic } = this.state;

        if (!isVisible) return null;

        return (
            <div className="magic-book-overlay" onClick={onClose}>
                <div className="magic-book-modal" onClick={e => e.stopPropagation()}>
                    <div className="magic-book-header">
                        <h2>Magic Book</h2>
                        <button type="button" className="magic-book-close" onClick={onClose} aria-label="Close">×</button>
                    </div>
                    <div className="magic-book-content">
                        {isLoading && (
                            <div className="magic-book-loading">Loading...</div>
                        )}
                        {errorMessage && (
                            <div className="magic-book-error">{errorMessage}</div>
                        )}
                        {!isLoading && !errorMessage && (
                            <div className="magic-book-layout">
                                <div className="magic-book-list">
                                    <div className="magic-book-list-title">Known magic</div>
                                    {magics.length === 0 ? (
                                        <div className="magic-book-empty">No magic learned yet.</div>
                                    ) : (
                                        <ul className="magic-book-items">
                                            {magics.map(m => {
                                                const canAfford = this.props.currentMana == null || m.mannaCost <= this.props.currentMana;
                                                const canCastThis = this.props.onCastMagic && this.props.canCast && canAfford;
                                                const insufficientMana = this.props.onCastMagic != null && this.props.currentMana != null && m.mannaCost > this.props.currentMana;
                                                return (
                                                    <li
                                                        key={m.magicTypeId}
                                                        className={`magic-book-item ${selectedMagic?.magicTypeId === m.magicTypeId ? 'selected' : ''} ${insufficientMana ? 'magic-book-item-insufficient-mana' : ''}`}
                                                        onMouseEnter={() => this.setState({ selectedMagic: m })}
                                                        onClick={() => {
                                                            if (canCastThis) {
                                                                this.props.onCastMagic!(m.magicTypeId, { castTargetType: m.castTargetType, effectRadius: m.effectRadius });
                                                                this.props.onClose();
                                                            } else {
                                                                this.setState({ selectedMagic: m });
                                                            }
                                                        }}
                                                    >
                                                        {m.thumbnailUrl ? (
                                                            <img src={m.thumbnailUrl} alt="" className="magic-book-item-thumb" />
                                                        ) : (
                                                            <div className="magic-book-item-thumb-placeholder">?</div>
                                                        )}
                                                        <span className="magic-book-item-name">{m.name || m.magicTypeId}</span>
                                                        {this.props.currentMana != null && (
                                                            <span className="magic-book-item-mana">{m.mannaCost}</span>
                                                        )}
                                                    </li>
                                                );
                                            })}
                                        </ul>
                                    )}
                                </div>
                                <div className="magic-book-detail">
                                    {selectedMagic ? this.renderDetail(selectedMagic) : (
                                        <div className="magic-book-detail-placeholder">Select a spell</div>
                                    )}
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        );
    }

    private renderDetail(m: IKnownMagicInfo) {
        return (
            <div className="magic-book-detail-content">
                <div className="magic-book-detail-header">
                    {m.thumbnailUrl ? (
                        <img src={m.thumbnailUrl} alt="" className="magic-book-detail-thumb" />
                    ) : (
                        <div className="magic-book-detail-thumb-placeholder">?</div>
                    )}
                    <div>
                        <h3 className="magic-book-detail-name">{m.name}</h3>
                        <div className="magic-book-detail-meta">
                            Mana: {m.mannaCost} · Target: {m.castTargetType}
                            {m.effectRadius > 0 && ` · Radius: ${m.effectRadius}`}
                        </div>
                    </div>
                </div>
                {m.description && (
                    <p className="magic-book-detail-description">{m.description}</p>
                )}
                {m.applyBuffs.length > 0 && (
                    <div className="magic-book-detail-section">
                        <h4>Buffs applied</h4>
                        <ul className="magic-book-detail-list magic-book-detail-entries">
                            {m.applyBuffs.map((b, i) => (
                                <li key={i}>
                                    <strong>{b.name || b.key || 'Buff'}</strong>
                                    <ul className="magic-book-detail-sublist">
                                        {this.getBuffDetailLines(b).map((line, j) => (
                                            <li key={j}>{line}</li>
                                        ))}
                                    </ul>
                                </li>
                            ))}
                        </ul>
                    </div>
                )}
                {m.applyEffects.length > 0 && (
                    <div className="magic-book-detail-section">
                        <h4>Effects applied</h4>
                        <ul className="magic-book-detail-list magic-book-detail-entries">
                            {m.applyEffects.map((e, i) => (
                                <li key={i}>
                                    {this.formatEffectEntry(e)}
                                    {this.renderEffectExpressions(e)}
                                </li>
                            ))}
                        </ul>
                    </div>
                )}
            </div>
        );
    }

    private getBuffDetailLines(b: IKnownMagicBuffEntry): string[] {
        const lines: string[] = [];
        const perm = b.permanent ?? false;
        const dur = b.duration ?? 0;
        if (perm) {
            lines.push('Permanent');
        } else if (dur > 0) {
            lines.push(`Duration: ${dur} turns${b.durationExpression ? ` (${b.durationExpression})` : ''}`);
        } else if (b.durationExpression) {
            lines.push(`Duration: ${b.durationExpression}`);
        }
        const n = (v: number | undefined) => v ?? 0;
        if (n(b.healthBonus) !== 0) lines.push(`Health: ${(b.healthBonus ?? 0) > 0 ? '+' : ''}${b.healthBonus}`);
        if (n(b.attackBonus) !== 0) lines.push(`Attack: ${(b.attackBonus ?? 0) > 0 ? '+' : ''}${b.attackBonus}`);
        if (n(b.defenseBonus) !== 0) lines.push(`Defense: ${(b.defenseBonus ?? 0) > 0 ? '+' : ''}${b.defenseBonus}`);
        if (n(b.speedBonus) !== 0) lines.push(`Speed: ${(b.speedBonus ?? 0) > 0 ? '+' : ''}${b.speedBonus}`);
        if (n(b.minDamageBonus) !== 0 || n(b.maxDamageBonus) !== 0) {
            lines.push(`Damage: ${(b.minDamageBonus ?? 0) > 0 ? '+' : ''}${b.minDamageBonus ?? 0} / ${(b.maxDamageBonus ?? 0) > 0 ? '+' : ''}${b.maxDamageBonus ?? 0}`);
        }
        if (n(b.healthBonusPercentage) !== 0) lines.push(`Health: ${(b.healthBonusPercentage ?? 0) > 0 ? '+' : ''}${b.healthBonusPercentage}%`);
        if (n(b.attackBonusPercentage) !== 0) lines.push(`Attack: ${(b.attackBonusPercentage ?? 0) > 0 ? '+' : ''}${b.attackBonusPercentage}%`);
        if (n(b.defenseBonusPercentage) !== 0) lines.push(`Defense: ${(b.defenseBonusPercentage ?? 0) > 0 ? '+' : ''}${b.defenseBonusPercentage}%`);
        if (n(b.speedBonusPercentage) !== 0) lines.push(`Speed: ${(b.speedBonusPercentage ?? 0) > 0 ? '+' : ''}${b.speedBonusPercentage}%`);
        if (n(b.minDamageBonusPercentage) !== 0 || n(b.maxDamageBonusPercentage) !== 0) {
            lines.push(`Damage: ${(b.minDamageBonusPercentage ?? 0) > 0 ? '+' : ''}${b.minDamageBonusPercentage ?? 0}% / ${(b.maxDamageBonusPercentage ?? 0) > 0 ? '+' : ''}${b.maxDamageBonusPercentage ?? 0}%`);
        }
        if (b.paralyzed) lines.push('Paralyzed');
        if (b.stunned) lines.push('Stunned');
        if (n(b.vampirePercentage) > 0) {
            lines.push(`Vampire: ${b.vampirePercentage}%${b.vampireCanResurrect ? ' (can resurrect)' : ''}`);
        }
        if (b.declinesWhenTakesDamage) lines.push('Declines when takes damage');
        if (n(b.heals) > 0) lines.push(`Heals: ${b.heals}/turn${b.healCanResurrect ? ' (can resurrect)' : ''}`);
        if (n(b.healsPercentage) > 0) lines.push(`Heals: ${b.healsPercentage}%/turn${b.healCanResurrect ? ' (can resurrect)' : ''}`);
        if (n(b.takesDamageMin) !== 0 || n(b.takesDamageMax) !== 0) {
            lines.push(`Takes damage: ${b.takesDamageMin ?? 0}-${b.takesDamageMax ?? 0}`);
        }
        if (n(b.damageReturnPercentage) > 0) {
            lines.push(`Damage return: ${b.damageReturnPercentage}%${(b.damageReturnMaxRange ?? 0) > 0 ? ` (range ${b.damageReturnMaxRange})` : ''}`);
        }
        return lines.length > 0 ? lines : ['(no values)'];
    }

    private formatEffectEntry(e: IKnownMagicEffectEntry): string {
        const name = e.name || e.key || 'Effect';
        const parts: string[] = [];
        if (e.takesDamageMin > 0 || e.takesDamageMax > 0) {
            parts.push(`Damage: ${e.takesDamageMin}-${e.takesDamageMax}`);
        }
        if (e.heals > 0) parts.push(`Heals: ${e.heals}`);
        if (e.healsPercentage > 0) parts.push(`Heals: ${e.healsPercentage}%`);
        return parts.length > 0 ? `${name} (${parts.join(', ')})` : name;
    }

    private renderEffectExpressions(e: IKnownMagicEffectEntry): React.ReactNode {
        const exprs: string[] = [];
        if (e.takesDamageMinExpression || e.takesDamageMaxExpression) {
            const min = e.takesDamageMinExpression || '';
            const max = e.takesDamageMaxExpression || min;
            if (min === max) {
                exprs.push(`Damage: ${min}`);
            } else {
                exprs.push(`Damage min: ${min}`, `Damage max: ${max}`);
            }
        }
        if (e.healsExpression) exprs.push(`Heals: ${e.healsExpression}`);
        if (e.healsPercentageExpression) exprs.push(`Heals%: ${e.healsPercentageExpression}`);
        if (exprs.length === 0) return null;
        return <span className="magic-book-expression"> ← {exprs.join('; ')}</span>;
    }
}
