
export interface IBattleDefinitionUnit {
    typeId: string
    count: number
    thumbnailUrl: string
    name: string
}

export interface IBattleDefinitionReward {
    name: string
    thumbnailUrl: string
    amount: string
}

export interface IBattleDefinition {
    readonly id: string
    readonly width: number
    readonly height: number
    readonly units: IBattleDefinitionUnit[]
    readonly rewards: IBattleDefinitionReward[]
}