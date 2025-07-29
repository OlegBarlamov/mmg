import React, { Component } from 'react';
import './battleControlPanel.css';
import { IBattlePanelActionsController } from '../battle/IBattlePanelActionsController';

export interface IBattleControlPanelProps {
    isVisible: boolean;
    isPlayerTurn: boolean;
    hasActiveUnit: boolean;
}

interface IBattleControlPanelState {
    // No state needed for this component
}

export class BattleControlPanel extends Component<IBattleControlPanelProps, IBattleControlPanelState> implements IBattlePanelActionsController {
    onWaitPressed: (() => void) | null = null;
    onPassPressed: (() => void) | null = null;

    private handleWaitClick = () => {
        if (this.onWaitPressed) {
            this.onWaitPressed();
        }
    };

    private handlePassClick = () => {
        if (this.onPassPressed) {
            this.onPassPressed();
        }
    };

    render() {
        if (!this.props.isVisible) {
            return null;
        }

        return (
            <div className="battle-control-panel">
                <button 
                    className="control-button pass-button"
                    onClick={this.handlePassClick}
                    disabled={!this.props.isPlayerTurn || !this.props.hasActiveUnit}
                >
                    Pass
                </button>
                <button 
                    className="control-button wait-button"
                    onClick={this.handleWaitClick}
                    disabled={!this.props.isPlayerTurn || !this.props.hasActiveUnit}
                >
                    Wait
                </button>
            </div>
        );
    }
} 