import './battleComponent.css'
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
import { BattleResultsModal } from './battleResultsModal';
import { IReportInfo } from '../services/serverAPI';
import { BattleControlPanel } from './battleControlPanel';
import { BattleMapUnit } from '../battleMap/battleMapUnit';

const CanvasContainerId = 'CanvasContainer'

export interface IBattleComponentProps {
    serviceLocator: IServiceLocator
    battleMap: BattleMap

    onBattleFinished(): void
}

interface IBattleComponentState {
    battleLoaded: boolean
    currentReward: IRewardToAccept | null
    rewards: IRewardToAccept[]
    currentRewardIndex: number
    battleReport: IReportInfo | null
    currentRoundNumber: number
    isPlayerTurn: boolean
    activeUnit: BattleMapUnit | null
}

export class BattleComponent extends PureComponent<IBattleComponentProps, IBattleComponentState> {
    private battleController: IBattleController | null = null
    private controlPanelRef: React.RefObject<BattleControlPanel> = React.createRef()

    constructor(props: IBattleComponentProps) {
        super(props)

        this.state = { 
            battleLoaded: false,
            currentReward: null,
            rewards: [],
            currentRewardIndex: 0,
            battleReport: null,
            currentRoundNumber: 0,
            isPlayerTurn: false,
            activeUnit: null
        }
    }

    async componentDidMount() {
        const canvasContainer = document.getElementById(CanvasContainerId)!

        const canvasService = this.props.serviceLocator.canvasService()
        await canvasService.init(canvasContainer, this.getMapHexagonStyle(this.props.battleMap))

        const battlesService = this.props.serviceLocator.battlesService()
        const panelController = this.controlPanelRef.current!
        this.battleController = await battlesService.createBattle(this.props.battleMap, panelController)

        // Subscribe to turn changes
        this.battleController.onNextTurn.connect(this.handleNextTurn)

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
        const serverAPI = this.props.serviceLocator.serverAPI()
        const rewards = await serverAPI.getMyRewards()

        if (rewards.length > 0) {
            this.setState({
                rewards: rewards,
                currentReward: rewards[0],
                currentRewardIndex: 0
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

    private handleRewardAccept = async () => {
        const { currentReward } = this.state
        if (!currentReward) return

        const serverAPI = this.props.serviceLocator.serverAPI()
        const result = await serverAPI.acceptReward(currentReward.id, {
            accepted: true,
            amounts: currentReward.amounts,
        })

        // If this was a Battle reward and we got a new battle map, start the new battle
        if (currentReward.rewardType === RewardType.Battle && result.nextBattle) {
            // Start the new battle immediately
            await this.startNewBattle(result.nextBattle)
        } else {
            this.showNextReward()
        }
    }

    private startNewBattle = async (battleMap: BattleMap) => {
        // Dispose of current battle controller
        if (this.battleController) {
            this.battleController.onNextTurn.disconnect(this.handleNextTurn)
            this.battleController.dispose()
        }
        
        // Clean up the canvas service before initializing new battle
        const canvasService = this.props.serviceLocator.canvasService()
        canvasService.clear()
        
        // Initialize new battle
        const canvasContainer = document.getElementById(CanvasContainerId)!
        await canvasService.init(canvasContainer, this.getMapHexagonStyle(battleMap))

        const battlesService = this.props.serviceLocator.battlesService()
        const panelController = this.controlPanelRef.current!
        this.battleController = await battlesService.createBattle(battleMap, panelController)

        // Subscribe to turn changes for the new battle
        this.battleController.onNextTurn.connect(this.handleNextTurn)

        // Start the new battle
        this.startBattle()
    }

    private handleRewardDecline = async () => {
        const { currentReward } = this.state
        if (!currentReward) return

        const serverAPI = this.props.serviceLocator.serverAPI()
        await serverAPI.acceptReward(currentReward.id, {
            accepted: false,
            amounts: [],
        })

        this.showNextReward()
    }



    private showNextReward = () => {
        const { rewards, currentRewardIndex } = this.state
        const nextIndex = currentRewardIndex + 1

        if (nextIndex < rewards.length) {
            this.setState({
                currentReward: rewards[nextIndex],
                currentRewardIndex: nextIndex
            })
        } else {
            this.setState({
                currentReward: null,
                rewards: [],
                currentRewardIndex: 0
            })
            this.props.onBattleFinished()
        }
    }

    async componentWillUnmount() {
        if (this.battleController) {
            this.battleController.onNextTurn.disconnect(this.handleNextTurn)
            this.battleController.dispose()
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

    render() {
        const canvasStyle: React.CSSProperties = {
            visibility: this.state.battleLoaded ? 'visible' : 'hidden'
        };

        return (
            <div className={"Container"}>
                {(!this.state.battleLoaded) && (
                    <div>Loading...</div>
                )}

                {this.state.battleLoaded && (
                    <div className="battle-header">
                        <div className="round-number">Round {this.state.currentRoundNumber + 1}</div>
                    </div>
                )}

                <div id={CanvasContainerId} style={canvasStyle}></div>

                <BattleControlPanel
                    ref={this.controlPanelRef}
                    isVisible={this.state.battleLoaded}
                    isPlayerTurn={this.state.isPlayerTurn}
                    activeUnit={this.state.activeUnit}
                />

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
                        serviceLocator={this.props.serviceLocator}
                    />
                )}
            </div>
        )
    }
}