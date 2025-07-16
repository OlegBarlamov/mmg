import {BattlePlayerNumber} from "../player/playerNumber";
import {UnitProperties} from "../units/unitProperties";
import {IHexoPoint} from "../hexogrid/hexoGrid";
import {getRandomStringKey} from "../units/getRandomString";

export type BattleMapUnit = {
    id: string
    position: IHexoPoint
    
    player: BattlePlayerNumber
    
    props: UnitProperties
    currentProps: UnitProperties
    
    count: number
}

export function getTestUnit(r: number, c: number, count: number, player: BattlePlayerNumber): BattleMapUnit {
    const props = {
        battleImgUrl: "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp",
        speed: 5,
        attackMinRange: 1,
        attackMaxRange: 1,
        damage: 6,
        health: 10,
    }
    return {
        id: getRandomStringKey(10),
        position: {
            c: c,
            r: r
        },
        player: player,
        props,
        currentProps: props,
        count: count,
    }
}