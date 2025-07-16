import {BattlePlayerNumber} from "../player/playerNumber";
import {IHexoPoint} from "../hexogrid/hexoGrid";

interface BaseCommandFromServer {
    commandId: string
    command: string
    turnNumber: number
}

interface PlayerCommandFromServer extends BaseCommandFromServer {
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
}

export interface BattleFinishedCommandFromServer extends BaseCommandFromServer {
    command: 'BATTLE_FINISHED'
    winner?: BattlePlayerNumber
}

export type BattleCommandFromServer = 
    UnitMoveCommandFromServer
    | UnitAttackCommandFromServer
    | UnitTakeDamageCommandFromServer
    | NextTurnCommandFromServer
    | BattleFinishedCommandFromServer