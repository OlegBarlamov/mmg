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

    onBattleFinished(newBattleMap?: BattleMap): void
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

        // Calculate and apply the appropriate scale to fit the map in the container
        const targetScale = this.calculateTargetScale(canvasContainer, this.props.battleMap)
        canvasService.setScale(targetScale)

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

        // If this was a Battle reward and we got a new battle map, notify the parent component
        if (currentReward.rewardType === RewardType.Battle && result.nextBattle) {
            // Pass the new battle map to the parent component
            this.props.onBattleFinished(result.nextBattle)
        } else {
            this.showNextReward()
        }
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

                <div className="battle-control-panel-wrapper">
                    <BattleControlPanel
                        ref={this.controlPanelRef}
                        isVisible={this.state.battleLoaded}
                        isPlayerTurn={this.state.isPlayerTurn}
                        activeUnit={this.state.activeUnit}
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
                        serviceLocator={this.props.serviceLocator}
                    />
                )}
            </div>
        )
    }
}