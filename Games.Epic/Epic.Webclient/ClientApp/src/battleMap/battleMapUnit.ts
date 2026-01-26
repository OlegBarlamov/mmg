import {BattlePlayerNumber} from "../player/playerNumber";
import {MovementType, UnitProperties} from "../units/unitProperties";
import {IHexoPoint} from "../hexogrid/hexoGrid";
import {getRandomStringKey} from "../units/getRandomString";

export type BattleMapUnit = {
    id: string
    position: IHexoPoint
    name: string

    player: BattlePlayerNumber
    
    props: UnitProperties
    currentProps: UnitProperties
    
    count: number
    isAlive: boolean
}

export function getTestUnit(r: number, c: number, count: number, player: BattlePlayerNumber): BattleMapUnit {
    const props = {
        battleImgUrl: "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp",
        speed: 5,
        health: 10,
        attack: 1,
        defense: 1,
        attacks: [
            {
                name: "Attack",
                thumbnailUrl: "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp",
                attackMaxRange: 1,
                attackMinRange: 1,
                stayOnly: false,
                counterattacksCount: 0,
                counterattackPenaltyPercentage: 0,
                rangePenalty: false,
                rangePenaltyZonesCount: 0,
                minDamage: 1,
                maxDamage: 1,
                enemyInRangeDisablesAttack: 0,
                pierceThrough: 0,
                splash: 0,
            }
        ],
        attacksStates: [],
        waited: false,
        movementType: MovementType.Walk,
        buffs: [],
    }
    return {
        id: getRandomStringKey(10),
        isAlive: true,
        position: {
            c: c,
            r: r
        },
        player: player,
        props,
        currentProps: props,
        count: count,
        name: "TestUnit",
    }
}