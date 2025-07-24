import { IResourceInfo } from "../services/serverAPI";
import { IUnitRewardResource } from "./IUnitRewardResource";
import { RewardType } from "./RewardType";

export interface IRewardToAccept {
    id: string,
    battleDefinitionId: string,
    type: RewardType,
    message: string,
    amounts: number[],
    unitsRewards: IUnitRewardResource[],
    resourcesRewards: IResourceInfo[],
}
