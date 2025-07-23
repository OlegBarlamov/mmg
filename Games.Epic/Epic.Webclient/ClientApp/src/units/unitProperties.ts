export type UnitProperties = {
    battleImgUrl: string
    speed: number
    health: number
    attacks: AttackType[]
}

export type AttackType = {
    name: string
    thumbnailUrl: string
    attackMaxRange: number
    attackMinRange: number
    stayOnly: boolean
    counterattackAllowed: boolean
    counterattackPenaltyPercentage: number
    rangePenalty: boolean
    rangePenaltyZonesCount: number
    minDamage: number
    maxDamage: number
    enemyInRangeDisablesAttack: number
}
