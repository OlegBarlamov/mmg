import { ArtifactSlot } from "../services/serverAPI";

export interface IArtifactRewardResource {
    id: string
    key: string
    name: string
    thumbnailUrl: string | null
    slots: ArtifactSlot[]
    attackBonus: number
    defenseBonus: number
    amount: number
}

