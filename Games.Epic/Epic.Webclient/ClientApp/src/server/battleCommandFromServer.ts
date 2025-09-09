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
}

export interface BattleFinishedCommandFromServer extends BaseCommandFromServer {
    command: 'BATTLE_FINISHED'
    winner?: BattlePlayerNumber
    reportId: string
}

export interface PlayerRansomCommandFromServer extends PlayerCommandFromServer {
    command: 'PLAYER_RANSOM'
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