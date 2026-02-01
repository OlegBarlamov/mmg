export type UnitProperties = {
    battleImgUrl: string
    speed: number
    health: number
    attack: number
    defense: number
    attacks: AttackType[]
    attacksStates: AttackState[]
    waited: boolean
    movementType: MovementType

    buffs?: UnitBuff[]
}

export enum MovementType {
    Walk = "Walk",
    Fly = "Fly",
}

export type AttackState = {
    attackIndex: number
    bulletsCount: number
    counterattacksUsed: number
}

export type BuffTypeInfo = {
    name: string
    permanent: boolean
    duration: number
}

export type AttackType = {
    name: string
    thumbnailUrl: string
    attackMaxRange: number
    attackMinRange: number
    stayOnly: boolean
    counterattacksCount: number
    counterattackPenaltyPercentage: number
    rangePenalty: boolean
    rangePenaltyZonesCount: number
    minDamage: number
    maxDamage: number
    enemyInRangeDisablesAttack: number
    pierceThrough: number
    splash: number
    applyBuffs?: BuffTypeInfo[]
}

export type UnitBuff = {
    id: string
    name: string
    permanent: boolean
    durationRemaining: number
}
