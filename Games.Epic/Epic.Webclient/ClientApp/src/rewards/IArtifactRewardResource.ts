import { ArtifactSlot } from "../services/serverAPI";

export interface IArtifactRewardResource {
    id: string
    key: string
    name: string
    thumbnailUrl: string | null
    slots: ArtifactSlot[]
    attackBonus: number
    defenseBonus: number
    knowledgeBonus: number
    powerBonus: number
    manaRestorationBonus: number
    amount: number
}

