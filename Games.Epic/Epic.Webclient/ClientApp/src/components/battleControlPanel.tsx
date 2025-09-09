import React, { Component } from 'react';
import './battleControlPanel.css';
import { IBattlePanelActionsController } from '../battle/IBattlePanelActionsController';
import { BattleMapUnit } from '../battleMap/battleMapUnit';
import { IServiceLocator } from '../services/serviceLocator';
import { IRansomPrice, IResourceInfo } from '../services/serverAPI';
import { RansomConfirmationModal } from './ransomConfirmationModal';
import { RunConfirmationModal } from './runConfirmationModal';

export interface IBattleControlPanelProps {
    isVisible: boolean;
    isPlayerTurn: boolean;
    activeUnit: BattleMapUnit | null;
    serviceLocator: IServiceLocator;
    battleId: string;
    battleMap: any; // BattleMap but avoiding circular import
    currentPlayerId: string;
    onRansomAction?: () => void;
    onRunAction?: () => void;
}

interface IBattleControlPanelState {
    showRansomModal: boolean;
    ransomPrice: IRansomPrice | null;
    playerGold: number | null;
    isLoadingRansomPrice: boolean;
    showRunModal: boolean;
    runPenaltyRounds: number;
}

export class BattleControlPanel extends Component<IBattleControlPanelProps, IBattleControlPanelState> implements IBattlePanelActionsController {
    onWaitPressed: (() => void) | null = null;
    onPassPressed: (() => void) | null = null;
    onRansomPressed: (() => void) | null = null;
    onRunPressed: (() => void) | null = null;

    constructor(props: IBattleControlPanelProps) {
        super(props);
        this.state = {
            showRansomModal: false,
            ransomPrice: null,
            playerGold: null,
            isLoadingRansomPrice: false,
            showRunModal: false,
            runPenaltyRounds: 0
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

    private handleRunClick = () => {
        // Calculate penalty rounds
        const penaltyRounds = this.calculateRunPenalty();
        this.setState({ 
            showRunModal: true,
            runPenaltyRounds: penaltyRounds 
        });
    };

    private calculateRunPenalty = (): number => {
        const { battleMap, currentPlayerId } = this.props;
        
        if (!battleMap || !battleMap.units) {
            return 0;
        }

        // Find current player's player number
        const currentPlayer = battleMap.players.find((p: any) => p.playerId === currentPlayerId);
        if (!currentPlayer) {
            return 0;
        }

        // Get alive player units and their min speed
        const playerUnits = battleMap.units.filter((unit: any) => 
            unit.isAlive && unit.player === currentPlayer.playerNumber
        );
        const playerMinSpeed = playerUnits.length > 0 
            ? Math.min(...playerUnits.map((unit: any) => unit.currentProps.speed))
            : 0;

        // Get alive enemy units and their max speed
        const enemyUnits = battleMap.units.filter((unit: any) => 
            unit.isAlive && unit.player !== currentPlayer.playerNumber
        );
        const enemyMaxSpeed = enemyUnits.length > 0 
            ? Math.max(...enemyUnits.map((unit: any) => unit.currentProps.speed))
            : 0;

        // Calculate penalty: enemy max speed - player min speed
        const penalty = enemyMaxSpeed - playerMinSpeed + 1;
        return Math.max(0, penalty); // Never negative
    };

    private handleRunConfirm = () => {
        this.setState({ showRunModal: false });
        
        // Use the direct callback if provided, otherwise fall back to the interface callback
        if (this.props.onRunAction) {
            this.props.onRunAction();
        } else if (this.onRunPressed) {
            this.onRunPressed();
        }
    };

    private handleRunCancel = () => {
        this.setState({ showRunModal: false });
    };

    render() {
        if (!this.props.isVisible) {
            return null;
        }

        return (
            <>
                <div className="battle-control-panel-container">
                    <div className="left-buttons-container">
                        <button 
                            className="control-button ransom-button"
                            onClick={this.handleRansomClick}
                            disabled={!this.props.isPlayerTurn}
                        >
                            Ransom
                        </button>
                        <button 
                            className="control-button run-button"
                            onClick={this.handleRunClick}
                            disabled={!this.props.isPlayerTurn}
                        >
                            Run
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
                <RunConfirmationModal
                    isVisible={this.state.showRunModal}
                    penaltyRounds={this.state.runPenaltyRounds}
                    onConfirm={this.handleRunConfirm}
                    onCancel={this.handleRunCancel}
                />
            </>
        );
    }
} 