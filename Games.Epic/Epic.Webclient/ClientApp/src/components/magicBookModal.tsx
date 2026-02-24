import './magicBookModal.css';
import React, { PureComponent } from 'react';
import { IServiceLocator } from '../services/serviceLocator';
import { IKnownMagicInfo, IKnownMagicBuffEntry, IKnownMagicEffectEntry } from '../services/serverAPI';

export interface IMagicBookModalProps {
    isVisible: boolean;
    serviceLocator: IServiceLocator;
    onClose: () => void;
    /** When set (e.g. in battle), show a Cast button. Options may include castTargetType (to request target selection) or targetUnitId/targetRow/targetColumn. */
    onCastMagic?: (magicTypeId: string, options?: { castTargetType?: string; effectRadius?: number; targetUnitId?: string; targetRow?: number; targetColumn?: number }) => void;
    /** Whether casting is allowed (e.g. one magic per round). */
    canCast?: boolean;
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
                                            {magics.map(m => (
                                                <li
                                                    key={m.magicTypeId}
                                                    className={`magic-book-item ${selectedMagic?.magicTypeId === m.magicTypeId ? 'selected' : ''}`}
                                                    onMouseEnter={() => this.setState({ selectedMagic: m })}
                                                >
                                                    {m.thumbnailUrl ? (
                                                        <img src={m.thumbnailUrl} alt="" className="magic-book-item-thumb" />
                                                    ) : (
                                                        <div className="magic-book-item-thumb-placeholder">?</div>
                                                    )}
                                                    <span className="magic-book-item-name">{m.name || m.magicTypeId}</span>
                                                </li>
                                            ))}
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
                                    {this.formatBuffEntry(b)}
                                    {b.durationExpression && (
                                        <span className="magic-book-expression"> ← {b.durationExpression}</span>
                                    )}
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
                {this.props.onCastMagic && (
                    <div className="magic-book-detail-section magic-book-cast-row">
                        <button
                            type="button"
                            className="magic-book-cast-button"
                            disabled={!this.props.canCast}
                            onClick={() => {
                                this.props.onCastMagic!(m.magicTypeId, { castTargetType: m.castTargetType, effectRadius: m.effectRadius });
                                this.props.onClose();
                            }}
                        >
                            Cast
                        </button>
                    </div>
                )}
            </div>
        );
    }

    private formatBuffEntry(b: IKnownMagicBuffEntry): string {
        const name = b.name || b.key || 'Buff';
        if (b.duration > 0) return `${name} (${b.duration} turns)`;
        return name;
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
