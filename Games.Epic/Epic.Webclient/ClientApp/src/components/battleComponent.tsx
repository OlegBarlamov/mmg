import './battleComponent.css'
import React, { PureComponent } from "react";
import { IServiceLocator } from "../services/serviceLocator";
import { IBattleController } from "../battle/battleController";
import { BattleMap, BattleMapCell } from "../battleMap/battleMap";
import { HexagonStyle } from "../services/canvasService";
import { EvenQGrid } from "../hexogrid/evenQGrid";
import { OddRGrid } from "../hexogrid/oddRGrid";
import { BattlePlayerNumber } from '../player/playerNumber';

const CanvasContainerId = 'CanvasContainer'

export interface IBattleComponentProps {
    serviceLocator: IServiceLocator
    battleMap: BattleMap

    onBattleFinished(): void
}

interface IBattleComponentState {
    battleLoaded: boolean
}

export class BattleComponent extends PureComponent<IBattleComponentProps, IBattleComponentState> {
    private battleController: IBattleController | null = null

    constructor(props: IBattleComponentProps) {
        super(props)

        this.state = { battleLoaded: false }
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

            await Promise.all(rewards.map(async reward => {
                const message = `${reward.message}\n
                    ${reward.unitRewardResources.map(resource => `${resource.name}: ${resource.amount}`).join('\n')}
                    \n\nAccept?`

                if (confirm(message)) {
                    await serverAPI.acceptReward(reward.id, {
                        accepted: true,
                        amounts: reward.unitRewardResources.map(resource => resource.amount),
                    })
                } else {
                    await serverAPI.acceptReward(reward.id, {
                        accepted: false,
                        amounts: [],
                    })
                }
            }))
        }

        this.props.onBattleFinished()
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
            </div>
        )
    }
}