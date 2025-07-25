import { IResourceInfo } from "../services/serverAPI";
import { IUnitRewardResource } from "./IUnitRewardResource";
import { RewardType } from "./RewardType";

export interface IPriceResource {
    resources: IResourceInfo[],
}

export interface IRewardToAccept {
    id: string,
    battleDefinitionId: string,
    rewardType: RewardType,
    message: string,
    amounts: number[],
    unitsRewards: IUnitRewardResource[],
    resourcesRewards: IResourceInfo[],
    prices: IPriceResource[],
}
