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
    thumbnailUrl?: string
    permanent: boolean
    duration: number
    chance: number
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

// Buff instance on a unit - minimal data stored client-side
// Full details fetched from server when modal opens
export type UnitBuff = {
    id: string
    buffTypeId: string
    name?: string
    thumbnailUrl?: string
    permanent?: boolean
    stunned?: boolean
    paralyzed?: boolean
    durationRemaining: number
}
