import {PlayerNumber} from "../player/playerNumber";
import {IHexoPoint} from "../hexogrid/hexoGrid";

interface BaseCommandFromServer {
    commandId: string
    command: string
    turnNumber: number
}

interface PlayerCommandFromServer extends BaseCommandFromServer {
    player: PlayerNumber
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

export interface PlayerWonCommandFromServer extends PlayerCommandFromServer {
    command: 'PLAYER_WON'
}

export type BattleCommandFromServer = 
    UnitMoveCommandFromServer
    | UnitAttackCommandFromServer
    | UnitTakeDamageCommandFromServer
    | NextTurnCommandFromServer
    | PlayerWonCommandFromServer