import { IRewardToAccept } from "../rewards/IRewardToAccept";
import { IServerAPI } from "./serverAPI";
import { IBattleDefinition } from "../battle/IBattleDefinition";

export interface IRewardManagerCallbacks {
    onRewardComplete?: () => void;
    onBattleReward?: (battleMap: any) => void;
    onRewardError?: (error: Error) => void;
}

export interface IRewardManagerState {
    rewards: IRewardToAccept[] | null;
    currentReward: IRewardToAccept | null;
    currentRewardIndex: number;
}

export class RewardManager {
    private serverAPI: IServerAPI;
    private callbacks: IRewardManagerCallbacks;
    private state: IRewardManagerState;

    constructor(serverAPI: IServerAPI, callbacks: IRewardManagerCallbacks = {}) {
        this.serverAPI = serverAPI;
        this.callbacks = callbacks;
        this.state = {
            rewards: null,
            currentReward: null,
            currentRewardIndex: 0
        };
    }

    /**
     * Check for unaccepted rewards and initialize the reward flow
     */
    async checkForRewards(): Promise<IRewardManagerState> {
        try {
            const rewards = await this.serverAPI.getMyRewards();
            
            if (rewards.length > 0) {
                this.state = {
                    rewards: rewards,
                    currentReward: rewards[0],
                    currentRewardIndex: 0
                };
            } else {
                this.state = {
                    rewards: null,
                    currentReward: null,
                    currentRewardIndex: 0
                };
            }
            
            return this.state;
        } catch (error) {
            console.error('Failed to fetch rewards:', error);
            this.callbacks.onRewardError?.(error as Error);
            return this.state;
        }
    }

    /**
     * Get the current reward manager state
     */
    getState(): IRewardManagerState {
        return { ...this.state };
    }

    /**
     * Handle reward acceptance
     */
    async acceptReward(affectedSlots?: number[]): Promise<void> {
        const { currentReward } = this.state;
        if (!currentReward) return;

        try {
            const result = await this.serverAPI.acceptReward(currentReward.id, {
                accepted: true,
                amounts: currentReward.amounts,
                affectedSlots: affectedSlots || [],
            });

            // If this was a Battle reward and we got a new battle map, notify callback
            if (currentReward.rewardType === 'Battle' && result.nextBattle) {
                this.callbacks.onBattleReward?.(result.nextBattle);
            } else {
                this.showNextReward();
            }
        } catch (error) {
            console.error('Failed to accept reward:', error);
            this.callbacks.onRewardError?.(error as Error);
        }
    }

    /**
     * Handle reward decline
     */
    async declineReward(): Promise<void> {
        const { currentReward } = this.state;
        if (!currentReward) return;

        try {
            await this.serverAPI.acceptReward(currentReward.id, {
                accepted: false,
                amounts: [],
                affectedSlots: [],
            });

            this.showNextReward();
        } catch (error) {
            console.error('Failed to decline reward:', error);
            this.callbacks.onRewardError?.(error as Error);
        }
    }

    /**
     * Show the next reward in the queue
     */
    private showNextReward(): void {
        const { rewards, currentRewardIndex } = this.state;
        if (!rewards) return;
        
        const nextIndex = currentRewardIndex + 1;

        if (nextIndex < rewards.length) {
            this.state = {
                ...this.state,
                currentReward: rewards[nextIndex],
                currentRewardIndex: nextIndex
            };
        } else {
            // No more rewards, clear the state
            this.state = {
                rewards: null,
                currentReward: null,
                currentRewardIndex: 0
            };
            this.callbacks.onRewardComplete?.();
        }
    }

    /**
     * Update callbacks (useful when component context changes)
     */
    updateCallbacks(callbacks: IRewardManagerCallbacks): void {
        this.callbacks = { ...this.callbacks, ...callbacks };
    }
}

