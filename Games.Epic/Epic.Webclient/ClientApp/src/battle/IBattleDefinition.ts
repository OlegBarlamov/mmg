
export interface IBattleDefinitionUnit {
    count: string
    thumbnailUrl: string
    name: string
}

export interface IBattleDefinitionReward {
    name: string
    thumbnailUrl: string
    amount: string
    description: string
    isNextStage: boolean
}

export interface IBattleDefinition {
    readonly id: string
    readonly width: number
    readonly height: number
    readonly units: IBattleDefinitionUnit[]
    readonly rewards: IBattleDefinitionReward[]
    readonly expiresAfterDays: number
    readonly isFinished: boolean
    readonly stage: number
}