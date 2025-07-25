import './rewardDialog.css'
import React, { PureComponent } from "react";
import { IRewardToAccept } from "../rewards/IRewardToAccept";
import { RewardType } from "../rewards/RewardType";

export interface IRewardDialogProps {
    reward: IRewardToAccept
    onAccept: () => void
    onDecline: () => void
}

interface IRewardDialogState {
    isVisible: boolean
}

export class RewardDialog extends PureComponent<IRewardDialogProps, IRewardDialogState> {
    constructor(props: IRewardDialogProps) {
        super(props)
        this.state = { isVisible: true }
    }

    private handleAccept = () => {
        this.setState({ isVisible: false })
        this.props.onAccept()
    }

    private handleDecline = () => {
        this.setState({ isVisible: false })
        this.props.onDecline()
    }



    private renderRewardContent() {
        const { reward } = this.props

        switch (reward.rewardType) {
            case RewardType.None:
                return (
                    <div className="reward-content">
                        <div className="reward-message">{reward.message}</div>
                        <div className="reward-actions">
                            <button className="reward-button ok-button" onClick={this.handleAccept}>
                                OK
                            </button>
                        </div>
                    </div>
                )

            case RewardType.ResourcesGain:
                return (
                    <div className="reward-content">
                        <div className="reward-message">{reward.message}</div>
                        {reward.resourcesRewards && reward.resourcesRewards.length > 0 && (
                            <div className="reward-resources">
                                <h4>Resources to gain:</h4>
                                <div className="resources-list">
                                    {reward.resourcesRewards.map((resource, index) => (
                                        <div key={index} className="resource-item">
                                            <img 
                                                src={resource.iconUrl} 
                                                alt={resource.name}
                                                className="resource-icon"
                                            />
                                            <span className="resource-name">{resource.name}</span>
                                            <span className="resource-amount">+{resource.amount}</span>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                        <div className="reward-actions">
                            <button className="reward-button accept-button" onClick={this.handleAccept}>
                                Accept
                            </button>
                            <button className="reward-button decline-button" onClick={this.handleDecline}>
                                Decline
                            </button>
                        </div>
                    </div>
                )

            case RewardType.UnitsGain:
                return (
                    <div className="reward-content">
                        <div className="reward-message">{reward.message}</div>
                        {reward.unitsRewards && reward.unitsRewards.length > 0 && (
                            <div className="reward-units">
                                <h4>Units to gain:</h4>
                                <div className="units-list">
                                    {reward.unitsRewards.map((unit, index) => (
                                        <div key={index} className="unit-item">
                                            <img 
                                                src={unit.dashboardImgUrl} 
                                                alt={unit.name}
                                                className="unit-thumbnail"
                                            />
                                            <span className="unit-name">{unit.name}</span>
                                            <span className="unit-amount">+{unit.amount}</span>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                        <div className="reward-actions">
                            <button className="reward-button accept-button" onClick={this.handleAccept}>
                                Accept
                            </button>
                            <button className="reward-button decline-button" onClick={this.handleDecline}>
                                Decline
                            </button>
                        </div>
                    </div>
                )

            case RewardType.UnitToBuy:
                return (
                    <div className="reward-content">
                        <div className="reward-message">{reward.message}</div>
                        {reward.unitsRewards && reward.unitsRewards.length > 0 && (
                            <div className="reward-units">
                                <h4>Units available to buy:</h4>
                                <div className="units-list">
                                    {reward.unitsRewards.map((unit, index) => (
                                        <div key={index} className="unit-item">
                                            <img 
                                                src={unit.dashboardImgUrl} 
                                                alt={unit.name}
                                                className="unit-thumbnail"
                                            />
                                            <span className="unit-name">{unit.name}</span>
                                            <span className="unit-amount">x{unit.amount}</span>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                        <div className="reward-actions">
                            <button className="reward-button accept-button" onClick={this.handleAccept}>
                                Buy
                            </button>
                            <button className="reward-button decline-button" onClick={this.handleDecline}>
                                Cancel
                            </button>
                        </div>
                    </div>
                )

            default:
                return (
                    <div className="reward-content">
                        <div className="reward-message">{reward.message}</div>
                        <div className="reward-actions">
                            <button className="reward-button ok-button" onClick={this.handleAccept}>
                                OK
                            </button>
                        </div>
                    </div>
                )
        }
    }

    render() {
        if (!this.state.isVisible) {
            return null
        }

        return (
            <div className="reward-dialog-overlay">
                <div className="reward-dialog">
                    <div className="reward-dialog-header">
                        <h3>Reward</h3>
                    </div>
                    {this.renderRewardContent()}
                </div>
            </div>
        )
    }
} 