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