import React, { PureComponent } from "react";
import "./playerBattleModal.css";

export interface IPlayerBattleModalProps {
    isVisible: boolean;
    onAccept: (playerName: string) => Promise<void>;
    onCancel: () => void;
    isLoading?: boolean;
}

interface IPlayerBattleModalState {
    playerName: string;
    errorMessage: string | null;
}

export class PlayerBattleModal extends PureComponent<IPlayerBattleModalProps, IPlayerBattleModalState> {
    constructor(props: IPlayerBattleModalProps) {
        super(props);
        this.state = {
            playerName: "",
            errorMessage: null
        };
    }

    componentDidUpdate(prevProps: IPlayerBattleModalProps) {
        // Reset state when modal becomes visible
        if (this.props.isVisible && !prevProps.isVisible) {
            this.setState({
                playerName: "",
                errorMessage: null
            });
        }
    }

    private handlePlayerNameChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({
            playerName: event.target.value,
            errorMessage: null
        });
    };

    private handleAccept = async () => {
        const { playerName } = this.state;
        
        if (!playerName.trim()) {
            this.setState({
                errorMessage: "Please enter a player name"
            });
            return;
        }

        try {
            await this.props.onAccept(playerName.trim());
        } catch (error) {
            this.setState({
                errorMessage: error instanceof Error ? error.message : "Failed to start battle with player"
            });
        }
    };

    private handleCancel = () => {
        this.props.onCancel();
    };

    private handleKeyPress = (event: React.KeyboardEvent) => {
        if (event.key === 'Enter') {
            this.handleAccept();
        } else if (event.key === 'Escape') {
            this.handleCancel();
        }
    };

    render() {
        if (!this.props.isVisible) {
            return null;
        }

        return (
            <div className="player-battle-modal-overlay">
                <div className="player-battle-modal">
                    <div className="player-battle-modal-header">
                        <h2>Start Battle with Player</h2>
                        <button className="close-button" onClick={this.handleCancel}>×</button>
                    </div>
                    
                    <div className="player-battle-modal-content">
                        <div className="input-section">
                            <label htmlFor="player-name-input" className="input-label">
                                Enter Player Name:
                            </label>
                            <input
                                id="player-name-input"
                                type="text"
                                className="player-name-input"
                                value={this.state.playerName}
                                onChange={this.handlePlayerNameChange}
                                onKeyPress={this.handleKeyPress}
                                placeholder="Enter player name..."
                                disabled={this.props.isLoading}
                                autoFocus
                            />
                        </div>

                        {this.state.errorMessage && (
                            <div className="error-message">
                                {this.state.errorMessage}
                            </div>
                        )}

                        <div className="modal-actions">
                            <button 
                                className="modal-button accept-button"
                                onClick={this.handleAccept}
                                disabled={this.props.isLoading || !this.state.playerName.trim()}
                            >
                                {this.props.isLoading ? 'Starting Battle...' : 'Start Battle'}
                            </button>
                            <button 
                                className="modal-button cancel-button"
                                onClick={this.handleCancel}
                                disabled={this.props.isLoading}
                            >
                                Cancel
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
