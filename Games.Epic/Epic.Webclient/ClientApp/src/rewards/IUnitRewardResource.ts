import { UnitProperties } from "../units/unitProperties";

export interface IUnitRewardResource {
    id: string,
    name: string,
    dashboardImgUrl: string,
    amount: number,
    props: UnitProperties,
}
