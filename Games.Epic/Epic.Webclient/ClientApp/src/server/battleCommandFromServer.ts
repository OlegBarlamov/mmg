import {BattlePlayerNumber} from "../player/playerNumber";
import {IHexoPoint} from "../hexogrid/hexoGrid";

interface BaseCommandFromServer {
    commandId: string
    command: string
    turnNumber: number
}

export interface PlayerCommandFromServer extends BaseCommandFromServer {
    player: BattlePlayerNumber
}

interface UnitCommandFromServer extends PlayerCommandFromServer {
    actorId: string
}

export interface UnitMoveCommandFromServer extends UnitCommandFromServer {
    command: 'UNIT_MOVE'
    moveToCell: IHexoPoint
}

export interface UnitAttackCommandFromServer extends UnitCommandFromServer {
    command: 'UNIT_ATTACK'
    targetId: string
    attackIndex: number
    isCounterattack: boolean
}

export interface UnitPassCommandFromServer extends UnitCommandFromServer {
    command: 'UNIT_PASS'
}

export interface UnitWaitCommandFromServer extends UnitCommandFromServer {
    command: 'UNIT_WAIT'
}

export interface UnitTakeDamageCommandFromServer extends UnitCommandFromServer {
    command: 'TAKE_DAMAGE'
    damageTaken: number
    killedCount: number
    remainingCount: number
    remainingHealth: number
}

export interface NextTurnCommandFromServer extends PlayerCommandFromServer {
    command: 'NEXT_TURN'
    nextTurnUnitId: string
    roundNumber: number
    canAct: boolean
}

export interface BattleFinishedCommandFromServer extends BaseCommandFromServer {
    command: 'BATTLE_FINISHED'
    winner?: BattlePlayerNumber
    reportId: string
}

export interface PlayerRansomCommandFromServer extends PlayerCommandFromServer {
    command: 'PLAYER_RANSOM'
}

export interface PlayerRunCommandFromServer extends PlayerCommandFromServer {
    command: 'PLAYER_RUN'
}

export interface PlayerMagicCommandFromServer extends PlayerCommandFromServer {
    command: 'PLAYER_MAGIC'
    magicTypeId: string
    magicName?: string
    currentMana?: number
}

export interface UnitReceivesBuffCommandFromServer extends UnitCommandFromServer {
    command: 'RECEIVE_BUFF'
    buffId: string
    buffTypeId: string
    buffName: string
    thumbnailUrl?: string
    permanent: boolean
    stunned: boolean
    paralyzed: boolean
    durationRemaining: number
}

export interface UnitLosesBuffCommandFromServer extends UnitCommandFromServer {
    command: 'LOSE_BUFF'
    buffId: string
    buffName: string
}

export interface UnitHealsCommandFromServer extends UnitCommandFromServer {
    command: 'UNIT_HEALS'
    healedAmount: number
    resurrectedCount: number
    newCount: number
    newHealth: number
}

export type EffectAnimationType = 'None' | 'Instant' | 'FromSource' | 'TopDown'

const EFFECT_ANIMATION_BY_NUM: EffectAnimationType[] = ['None', 'Instant', 'FromSource', 'TopDown']

export function normalizeEffectAnimationType(v: number | string | undefined): EffectAnimationType {
    if (typeof v === 'string' && (v === 'None' || v === 'Instant' || v === 'FromSource' || v === 'TopDown')) return v
    if (typeof v === 'number' && v >= 0 && v <= 3) return EFFECT_ANIMATION_BY_NUM[v]
    return 'None'
}

export interface EffectAnimationCommandFromServer extends BaseCommandFromServer {
    command: 'EFFECT_ANIMATION'
    effectSpriteUrl: string
    /** Server may send number (enum) or string */
    animationType: EffectAnimationType | number
    targetRow: number
    targetColumn: number
    sourceUnitId?: string | null
    sourcePlayer?: string | null
    animationTimeMs: number
}

export type BattleCommandFromServer = 
    UnitMoveCommandFromServer
    | UnitAttackCommandFromServer
    | UnitPassCommandFromServer
    | UnitWaitCommandFromServer
    | UnitTakeDamageCommandFromServer
    | NextTurnCommandFromServer
    | BattleFinishedCommandFromServer
    | PlayerRansomCommandFromServer
    | PlayerRunCommandFromServer
    | PlayerMagicCommandFromServer
    | UnitReceivesBuffCommandFromServer
    | UnitLosesBuffCommandFromServer
    | UnitHealsCommandFromServer
    | EffectAnimationCommandFromServer