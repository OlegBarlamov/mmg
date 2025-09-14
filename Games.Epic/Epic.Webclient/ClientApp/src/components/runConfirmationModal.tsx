import React from 'react';
import './runConfirmationModal.css';

export interface IRunConfirmationModalProps {
    isVisible: boolean;
    penaltyRounds: number;
    onConfirm: () => void;
    onCancel: () => void;
}

export class RunConfirmationModal extends React.Component<IRunConfirmationModalProps> {
    render() {
        if (!this.props.isVisible) {
            return null;
        }

        return (
            <div className="run-confirmation-overlay">
                <div className="run-confirmation-modal">
                    <div className="run-header">
                        <h3>Flee from Battle</h3>
                    </div>
                    <div className="run-content">
                        <p>Are you sure you want to flee from this battle?</p>
                        <div className="run-penalty-info">
                            {this.props.penaltyRounds > 0 ? (
                                <>
                                    <p className="run-warning">You will lose the battle and the enemy will process <strong>{this.props.penaltyRounds}</strong> round{this.props.penaltyRounds !== 1 ? 's' : ''} against you.</p>
                                    <p className="run-calculation">
                                        <small>Penalty = Enemy max speed - Your min speed</small>
                                    </p>
                                </>
                            ) : (
                                <>
                                    <p className="run-warning">You will lose the battle but suffer no additional penalty rounds.</p>
                                    <p className="run-calculation">
                                        <small>Your units are fast enough to escape safely!</small>
                                    </p>
                                </>
                            )}
                        </div>
                    </div>
                    <div className="run-actions">
                        <button 
                            className="run-button run-cancel-button"
                            onClick={this.props.onCancel}
                        >
                            No
                        </button>
                        <button 
                            className="run-button run-confirm-button"
                            onClick={this.props.onConfirm}
                        >
                            Yes, Flee
                        </button>
                    </div>
                </div>
            </div>
        );
    }
}
