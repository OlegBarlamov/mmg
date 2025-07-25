import './battleComponent.css'
import React, { PureComponent } from "react";
import { IServiceLocator } from "../services/serviceLocator";
import { IBattleController } from "../battle/battleController";
import { BattleMap, BattleMapCell } from "../battleMap/battleMap";
import { HexagonStyle } from "../services/canvasService";
import { EvenQGrid } from "../hexogrid/evenQGrid";
import { OddRGrid } from "../hexogrid/oddRGrid";
import { BattlePlayerNumber } from '../player/playerNumber';
import { RewardDialog } from './rewardDialog';
import { IRewardToAccept } from '../rewards/IRewardToAccept';

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
}

export class BattleComponent extends PureComponent<IBattleComponentProps, IBattleComponentState> {
    private battleController: IBattleController | null = null

    constructor(props: IBattleComponentProps) {
        super(props)

        this.state = { 
            battleLoaded: false,
            currentReward: null,
            rewards: [],
            currentRewardIndex: 0
        }
    }

    async componentDidMount() {
        const canvasContainer = document.getElementById(CanvasContainerId)!

        const canvasService = this.props.serviceLocator.canvasService()
        await canvasService.init(canvasContainer, this.getMapHexagonStyle(this.props.battleMap))

        const battlesService = this.props.serviceLocator.battlesService()
        this.battleController = await battlesService.createBattle(this.props.battleMap)

        // Do not wait
        this.startBattle()

        this.setState({ ...this.state, battleLoaded: true })
    }

    private async startBattle() {
        const winner = await this.battleController!.startBattle()

        if (winner === BattlePlayerNumber.Player1) {
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
        } else {
            this.props.onBattleFinished()
        }
    }

    private handleRewardAccept = async () => {
        const { currentReward } = this.state
        if (!currentReward) return

        const serverAPI = this.props.serviceLocator.serverAPI()
        await serverAPI.acceptReward(currentReward.id, {
            accepted: true,
            amounts: currentReward.amounts,
        })

        this.showNextReward()
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
        this.battleController?.dispose()
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

                <div id={CanvasContainerId} style={canvasStyle}></div>

                {this.state.currentReward && (
                    <RewardDialog
                        reward={this.state.currentReward}
                        onAccept={this.handleRewardAccept}
                        onDecline={this.handleRewardDecline}
                    />
                )}
            </div>
        )
    }
}