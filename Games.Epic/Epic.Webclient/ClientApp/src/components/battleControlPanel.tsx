import React, { Component } from 'react';
import './battleControlPanel.css';
import { IBattlePanelActionsController } from '../battle/IBattlePanelActionsController';
import { BattleMapUnit } from '../battleMap/battleMapUnit';
import { IServiceLocator } from '../services/serviceLocator';
import { IRansomPrice, IResourceInfo } from '../services/serverAPI';
import { RansomConfirmationModal } from './ransomConfirmationModal';

export interface IBattleControlPanelProps {
    isVisible: boolean;
    isPlayerTurn: boolean;
    activeUnit: BattleMapUnit | null;
    serviceLocator: IServiceLocator;
    battleId: string;
    onRansomAction?: () => void;
}

interface IBattleControlPanelState {
    showRansomModal: boolean;
    ransomPrice: IRansomPrice | null;
    playerGold: number | null;
    isLoadingRansomPrice: boolean;
}

export class BattleControlPanel extends Component<IBattleControlPanelProps, IBattleControlPanelState> implements IBattlePanelActionsController {
    onWaitPressed: (() => void) | null = null;
    onPassPressed: (() => void) | null = null;
    onRansomPressed: (() => void) | null = null;

    constructor(props: IBattleControlPanelProps) {
        super(props);
        this.state = {
            showRansomModal: false,
            ransomPrice: null,
            playerGold: null,
            isLoadingRansomPrice: false
        };
    }

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

    private handleRansomClick = async () => {
        this.setState({ 
            showRansomModal: true, 
            isLoadingRansomPrice: true, 
            ransomPrice: null,
            playerGold: null
        });

        try {
            const serverAPI = this.props.serviceLocator.serverAPI();
            
            // Fetch both ransom price and player's current resources
            const [ransomPrice, resources] = await Promise.all([
                serverAPI.getRansomPrice(this.props.battleId),
                serverAPI.getResources()
            ]);
            
            // Find gold resource
            const goldResource = resources.find(resource => 
                resource.name.toLowerCase() === 'gold' || resource.id.toLowerCase().includes('gold')
            );
            
            this.setState({ 
                ransomPrice: ransomPrice, 
                playerGold: goldResource ? goldResource.amount : 0,
                isLoadingRansomPrice: false 
            });
        } catch (error) {
            console.error('Failed to get ransom price or resources:', error);
            this.setState({ 
                isLoadingRansomPrice: false 
            });
        }
    };

    private handleRansomConfirm = () => {
        this.setState({ showRansomModal: false });
        
        // Use the direct callback if provided, otherwise fall back to the interface callback
        if (this.props.onRansomAction) {
            this.props.onRansomAction();
        } else if (this.onRansomPressed) {
            this.onRansomPressed();
        }
    };

    private handleRansomCancel = () => {
        this.setState({ showRansomModal: false });
    };

    render() {
        if (!this.props.isVisible) {
            return null;
        }

        return (
            <>
                <div className="battle-control-panel-container">
                    <div className="ransom-button-container">
                        <button 
                            className="control-button ransom-button"
                            onClick={this.handleRansomClick}
                            disabled={!this.props.isPlayerTurn}
                        >
                            Ransom
                        </button>
                    </div>
                    <div className="battle-control-panel">
                        <button 
                            className="control-button pass-button"
                            onClick={this.handlePassClick}
                            disabled={!this.props.isPlayerTurn || !this.props.activeUnit}
                        >
                            Pass
                        </button>
                        <button 
                            className="control-button wait-button"
                            onClick={this.handleWaitClick}
                            disabled={!this.props.isPlayerTurn || this.props.activeUnit?.currentProps.waited === true}
                        >
                            Wait
                        </button>
                    </div>
                </div>
                <RansomConfirmationModal
                    isVisible={this.state.showRansomModal}
                    ransomPrice={this.state.ransomPrice}
                    playerGold={this.state.playerGold}
                    isLoading={this.state.isLoadingRansomPrice}
                    onConfirm={this.handleRansomConfirm}
                    onCancel={this.handleRansomCancel}
                />
            </>
        );
    }
} 