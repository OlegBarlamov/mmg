import './battleComponent.css'
import './battleUnitInfoModal.css'
import React, { PureComponent } from "react";
import { IServiceLocator } from "../services/serviceLocator";
import { IBattleController } from "../battle/battleController";
import { BattleMap, BattleMapCell, BattleTurnInfo } from "../battleMap/battleMap";
import { HexagonStyle } from "../services/canvasService";
import { EvenQGrid } from "../hexogrid/evenQGrid";
import { OddRGrid } from "../hexogrid/oddRGrid";
import { BattlePlayerNumber } from '../player/playerNumber';
import { RewardDialog } from './rewardDialog';
import { IRewardToAccept } from '../rewards/IRewardToAccept';
import { RewardType } from '../rewards/RewardType';
import { RewardManager, IRewardManagerState } from '../services/rewardManager';
import { BattleResultsModal } from './battleResultsModal';
import { IReportInfo } from '../services/serverAPI';
import { BattleControlPanel } from './battleControlPanel';
import { BattleMapUnit } from '../battleMap/battleMapUnit';
import { BattleUnitInfoModal } from './battleUnitInfoModal';

const CanvasContainerId = 'CanvasContainer'

export interface IBattleComponentProps {
    serviceLocator: IServiceLocator
    battleMap: BattleMap

    onBattleFinished(newBattleMap?: BattleMap): void
}

interface IBattleComponentState {
    battleLoaded: boolean
    currentReward: IRewardToAccept | null
    rewards: IRewardToAccept[] | null
    currentRewardIndex: number
    battleReport: IReportInfo | null
    currentRoundNumber: number
    isPlayerTurn: boolean
    activeUnit: BattleMapUnit | null
    showUnitInfoModal: boolean
    selectedUnit: BattleMapUnit | null
}

export class BattleComponent extends PureComponent<IBattleComponentProps, IBattleComponentState> {
    private battleController: IBattleController | null = null
    private controlPanelRef: React.RefObject<BattleControlPanel | null> = React.createRef()
    private contextMenuHandler: ((e: Event) => void) | null = null
    private rewardManager: RewardManager

    constructor(props: IBattleComponentProps) {
        super(props)

        this.state = {
            battleLoaded: false,
            currentReward: null,
            rewards: null,
            currentRewardIndex: 0,
            battleReport: null,
            currentRoundNumber: 0,
            isPlayerTurn: false,
            activeUnit: null,
            showUnitInfoModal: false,
            selectedUnit: null
        }

        // Initialize reward manager with callbacks
        this.rewardManager = new RewardManager(
            this.props.serviceLocator.serverAPI(),
            {
                onRewardComplete: this.handleRewardComplete,
                onGuardBattleBegins: this.handleGuardBattleBegins,
                onRewardError: this.handleRewardError
            }
        );
    }

    async componentDidMount() {
        const canvasContainer = document.getElementById(CanvasContainerId)!

        // Prevent default context menu on canvas to allow our custom right-click handling
        this.contextMenuHandler = (e: Event) => {
            e.preventDefault()
        }
        canvasContainer.addEventListener('contextmenu', this.contextMenuHandler)

        const canvasService = this.props.serviceLocator.canvasService()
        await canvasService.init(canvasContainer, this.getMapHexagonStyle(this.props.battleMap))

        const battlesService = this.props.serviceLocator.battlesService()
        const panelController = this.controlPanelRef.current!
        this.battleController = await battlesService.createBattle(this.props.battleMap, panelController)

        // Calculate and apply the appropriate scale to fit the map in the container
        const targetScale = this.calculateTargetScale(canvasContainer, this.props.battleMap)
        canvasService.setScale(targetScale)

        // Subscribe to turn changes
        this.battleController.onNextTurn.connect(this.handleNextTurn)

        // Set up right-click handler for unit info modal
        this.battleController.mapController.onUnitRightMouseClick = this.handleUnitRightClick

        // Do not wait
        this.startBattle()

        // Initialize the turn state based on the initial battle state
        const initialTurnInfo = this.props.battleMap.turnInfo
        const isPlayerTurn = this.battleController!.isPlayerControlled(initialTurnInfo.player)
        const activeUnit = initialTurnInfo.nextTurnUnitId ?
            this.props.battleMap.units.find(unit => unit.id === initialTurnInfo.nextTurnUnitId) ?? null
            : null

        this.setState({
            ...this.state,
            battleLoaded: true,
            isPlayerTurn: isPlayerTurn,
            activeUnit: activeUnit
        })
    }


    private handleNextTurn = (turnInfo: BattleTurnInfo) => {
        const isPlayerTurn = this.battleController!.isPlayerControlled(turnInfo.player)
        const activeUnit = turnInfo.nextTurnUnitId ?
            this.props.battleMap.units.find(unit => unit.id === turnInfo.nextTurnUnitId) ?? null
            : null

        this.setState({
            currentRoundNumber: turnInfo.roundNumber,
            isPlayerTurn: isPlayerTurn,
            activeUnit: activeUnit
        })
    }

    private handleUnitRightClick = (unit: BattleMapUnit, event: PointerEvent) => {
        this.setState({
            showUnitInfoModal: true,
            selectedUnit: unit
        })
    }

    private handleUnitInfoModalClose = () => {
        this.setState({
            showUnitInfoModal: false,
            selectedUnit: null
        })
    }

    private handleRansomAction = async () => {
        if (!this.battleController) return;
        
        try {
            const playerService = this.props.serviceLocator.playerService();
            
            // Find the current player's battle info
            const currentPlayer = this.props.battleMap.players.find(
                player => player.playerId === playerService.currentPlayerInfo.id
            );
            
            if (!currentPlayer) {
                console.error('Current player not found in battle');
                return;
            }
            
            // Create ransom action
            const ransomAction = {
                command: 'PLAYER_RANSOM' as const,
                player: currentPlayer.playerNumber
            };
            
            // Process the action directly through the battle action processor
            const battleActionProcessor = (this.battleController as any).battleActionProcessor;
            if (battleActionProcessor) {
                await battleActionProcessor.processAction(ransomAction);
            }
        } catch (error) {
            console.error('Failed to process ransom action:', error);
        }
    }

    private handleRunAction = async () => {
        if (!this.battleController) return;
        
        try {
            const playerService = this.props.serviceLocator.playerService();
            
            // Find the current player's battle info
            const currentPlayer = this.props.battleMap.players.find(
                player => player.playerId === playerService.currentPlayerInfo.id
            );
            
            if (!currentPlayer) {
                console.error('Current player not found in battle');
                return;
            }
            
            // Create run action
            const runAction = {
                command: 'PLAYER_RUN' as const,
                player: currentPlayer.playerNumber
            };
            
            // Process the action directly through the battle action processor
            const battleActionProcessor = (this.battleController as any).battleActionProcessor;
            if (battleActionProcessor) {
                await battleActionProcessor.processAction(runAction);
            }
        } catch (error) {
            console.error('Failed to process run action:', error);
        }
    }

    private async startBattle() {
        const battleResult = await this.battleController!.startBattle()

        // If we have a report ID, fetch and show the battle results first (for both wins and losses)
        if (battleResult.reportId) {
            try {
                const serverAPI = this.props.serviceLocator.serverAPI()
                const report = await serverAPI.getReport(battleResult.reportId)
                this.setState({ battleReport: report })
                return // Don't proceed yet, wait for modal to close
            } catch (error) {
                console.error('Failed to fetch battle report:', error)
                // Continue even if report fetch fails
            }
        }

        // If no report or report fetch failed, proceed based on winner
        if (battleResult.winner === BattlePlayerNumber.Player1) {
            await this.proceedToRewards()
        } else {
            this.props.onBattleFinished()
        }
    }

    private async proceedToRewards() {
        const rewardState = await this.rewardManager.checkForRewards()
        
        if (rewardState.rewards && rewardState.rewards.length > 0) {
            this.setState({
                rewards: rewardState.rewards,
                currentReward: rewardState.currentReward,
                currentRewardIndex: rewardState.currentRewardIndex
            })
        } else {
            this.props.onBattleFinished()
        }
    }

    private handleBattleResultsOk = () => {
        // Capture the isWinner value before clearing the state
        const isWinner = this.state.battleReport?.isWinner

        this.setState({ battleReport: null })

        // Check if the player won to determine next action
        if (isWinner) {
            this.proceedToRewards()
        } else {
            this.props.onBattleFinished()
        }
    }

    private handleRewardComplete = () => {
        // Clear reward state when all rewards are complete
        this.setState({
            currentReward: null,
            rewards: null,
            currentRewardIndex: 0
        })
        this.props.onBattleFinished()
    }

    private handleGuardBattleBegins = (battleMap: any) => {
        // Pass the new battle map to the parent component
        this.props.onBattleFinished(battleMap)
    }

    private handleRewardError = (error: Error) => {
        console.error('Reward error:', error)
        this.props.onBattleFinished()
    }

    private handleRewardAccept = async (affectedSlots: number[]) => {
        await this.rewardManager.acceptReward(affectedSlots)
        const state = this.rewardManager.getState()
        this.setState({
            rewards: state.rewards,
            currentReward: state.currentReward,
            currentRewardIndex: state.currentRewardIndex
        })
    }

    private handleRewardDecline = async () => {
        await this.rewardManager.declineReward()
        const state = this.rewardManager.getState()
        this.setState({
            rewards: state.rewards,
            currentReward: state.currentReward,
            currentRewardIndex: state.currentRewardIndex
        })
    }

    private handleBeginGuardBattle = async (rewardId: string) => {
        await this.rewardManager.beginGuardBattle(rewardId)
    }

    async componentWillUnmount() {
        if (this.battleController) {
            this.battleController.onNextTurn.disconnect(this.handleNextTurn)
            this.battleController.mapController.onUnitRightMouseClick = null
            this.battleController.dispose()
        }

        // Remove the context menu event listener
        const canvasContainer = document.getElementById(CanvasContainerId)
        if (canvasContainer) {
            canvasContainer.removeEventListener('contextmenu', this.contextMenuHandler!)
        }
    }

    private getMapHexagonStyle(map: BattleMap): HexagonStyle {
        if (map.grid instanceof EvenQGrid) {
            return HexagonStyle.QStyle
        } else if (map.grid instanceof OddRGrid) {
            return HexagonStyle.RStyle
        }

        throw new Error("Unknown type of the battle map hexo grid")
    }

    private calculateTargetScale(container: HTMLElement, battleMap: BattleMap): number {
        const containerSize = {
            width: container.clientWidth,
            height: container.clientHeight
        }

        // Handle edge case where container has no size
        if (containerSize.width <= 0 || containerSize.height <= 0) {
            return 1.0
        }

        const availableWidth = containerSize.width
        const availableHeight = containerSize.height

        // Handle edge case where available space is too small
        if (availableWidth <= 0 || availableHeight <= 0) {
            return 1.0
        }

        const hexagonRadius = this.battleController!.mapController.cellRadius
        // Calculate the map size using the grid's getSize method
        const mapSize = battleMap.grid.getSize(hexagonRadius)

        // Handle edge case where map has no size
        if (mapSize.width <= 0 || mapSize.height <= 0) {
            return 1.0
        }

        // Calculate scale factors for both width and height using available space
        const scaleX = availableWidth / mapSize.width
        const scaleY = availableHeight / mapSize.height

        // Use the smaller scale to ensure the map fits in both dimensions
        const targetScale = Math.min(scaleX, scaleY, 1.0) // Don't scale up beyond 1.0

        return targetScale
    }

    render() {
        const canvasStyle: React.CSSProperties = {
            visibility: this.state.battleLoaded ? 'visible' : 'hidden'
        };

        const player1 = this.props.battleMap.players.find(p => p.playerNumber === BattlePlayerNumber.Player1)
        const player2 = this.props.battleMap.players.find(p => p.playerNumber === BattlePlayerNumber.Player2)

        return (
            <div className={"Container"}>
                {(!this.state.battleLoaded) && (
                    <div>Loading...</div>
                )}

                {this.state.battleLoaded && (
                    <div className="battle-header">
                        {player1?.heroStats && (
                            <div className="hero-stats-text hero-stats-left">
                                Attack: {player1.heroStats.attack} Defense: {player1.heroStats.defense}
                            </div>
                        )}
                        <div className="round-number">Round {this.state.currentRoundNumber + 1}</div>
                        {player2?.heroStats && (
                            <div className="hero-stats-text hero-stats-right">
                                Attack: {player2.heroStats.attack} Defense: {player2.heroStats.defense}
                            </div>
                        )}
                    </div>
                )}

                <div id={CanvasContainerId} style={canvasStyle}></div>

                <div className="battle-control-panel-wrapper">
                    <BattleControlPanel
                        ref={this.controlPanelRef}
                        isVisible={this.state.battleLoaded}
                        isPlayerTurn={this.state.isPlayerTurn}
                        activeUnit={this.state.activeUnit}
                        serviceLocator={this.props.serviceLocator}
                        battleId={this.props.battleMap.id}
                        battleMap={this.props.battleMap}
                        currentPlayerId={this.props.serviceLocator.playerService().currentPlayerInfo.id}
                        onRansomAction={this.handleRansomAction}
                        onRunAction={this.handleRunAction}
                    />
                </div>

                {this.state.battleReport && (
                    <BattleResultsModal
                        report={this.state.battleReport}
                        onOk={this.handleBattleResultsOk}
                    />
                )}

                {this.state.currentReward && (
                    <RewardDialog
                        reward={this.state.currentReward}
                        onAccept={this.handleRewardAccept}
                        onDecline={this.handleRewardDecline}
                        onBeginGuardBattle={this.handleBeginGuardBattle}
                        serviceLocator={this.props.serviceLocator}
                    />
                )}

                <BattleUnitInfoModal
                    isVisible={this.state.showUnitInfoModal}
                    unit={this.state.selectedUnit}
                    onClose={this.handleUnitInfoModalClose}
                />
            </div>
        )
    }
}