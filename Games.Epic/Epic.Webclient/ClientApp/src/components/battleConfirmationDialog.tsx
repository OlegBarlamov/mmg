import React from 'react';
import './battleConfirmationDialog.css';

export interface IBattleConfirmationDialogProps {
    isVisible: boolean;
    onAccept: () => void;
    onCancel: () => void;
}

export class BattleConfirmationDialog extends React.Component<IBattleConfirmationDialogProps> {
    render() {
        if (!this.props.isVisible) {
            return null;
        }

        return (
            <div className="battle-confirmation-overlay">
                <div className="battle-confirmation-dialog">
                    <div className="confirmation-header">
                        <h3>Battle Confirmation</h3>
                    </div>
                    <div className="confirmation-message">
                        Some of the units do not explicitly fit to the battle field and might not be able to participate. Are you sure you want to start the battle?
                    </div>
                    <div className="confirmation-actions">
                        <button 
                            className="confirmation-button cancel-button"
                            onClick={this.props.onCancel}
                        >
                            Cancel
                        </button>
                        <button 
                            className="confirmation-button accept-button"
                            onClick={this.props.onAccept}
                        >
                            Start Battle
                        </button>
                    </div>
                </div>
            </div>
        );
    }
}
