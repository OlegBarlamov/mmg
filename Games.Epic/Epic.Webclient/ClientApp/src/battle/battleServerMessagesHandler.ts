import { BattleCommandFromServer, PlayerCommandFromServer } from "../server/battleCommandFromServer";
import {IBattleConnectionMessagesHandler} from "../server/battleServerConnection";
import {getUnitById} from "./battleLogic";
import {IBattleMapController} from "../battleMap/battleMapController";
import {BattleTurnInfo} from "../battleMap/battleMap";
import { BattlePlayerNumber } from "../player/playerNumber";
import { Signal } from "typed-signals";

export interface ITurnAwaiter {
    onCurrentPlayerActionReceived: Signal<() => void>
    /**
     * Fired when this handler starts processing a server message
     * (i.e. after previously queued animations complete).
     */
    onServerMessageHandlingStarted: Signal<(message: BattleCommandFromServer) => void>
    readonly currentTurnIndex: number
    readonly currentRoundNumber: number
    getCurrentTurnInfo(): BattleTurnInfo 
    waitForTurn(turnIndex: number): Promise<BattleTurnInfo>
    dispose(): void
}

export class BattleServerMessagesHandler implements IBattleConnectionMessagesHandler, ITurnAwaiter {
    onCurrentPlayerActionReceived: Signal<() => void> = new Signal()
    onServerMessageHandlingStarted: Signal<(message: BattleCommandFromServer) => void> = new Signal()
    currentTurnIndex: number
    currentRoundNumber: number
    
    private disposed = false

    private previousAnimationPromise: Promise<void> = Promise.resolve()

    private readonly previousTurnInfos: Map<number, BattleTurnInfo> = new Map()
    private readonly awaitingPromises: Map<number, Promise<BattleTurnInfo>> = new Map()
    private readonly triggerAwaitingPromises: Map<number, (info: BattleTurnInfo) => void> = new Map()
    private readonly rejectAwaitingPromises: Map<number, (error: Error) => void> = new Map()
    
    constructor(private mapController: IBattleMapController) {
        this.currentTurnIndex = -1
        this.currentRoundNumber = 0
    }
    
    getCurrentTurnInfo(): BattleTurnInfo {
        return this.previousTurnInfos.get(this.currentTurnIndex)!
    }

    waitForTurn(turnIndex: number): Promise<BattleTurnInfo> {
        if (turnIndex <= this.currentTurnIndex) {
            return Promise.resolve(this.previousTurnInfos.get(turnIndex)!)
        }
        if (this.awaitingPromises.has(turnIndex)) {
            return this.awaitingPromises.get(turnIndex)!
        }
        return this.addNewAwaitingPromiseForTurn(turnIndex)
    }
    
    private addNewAwaitingPromiseForTurn(turnIndex: number): Promise<BattleTurnInfo> {
        const newAwaitingPromise = new Promise<BattleTurnInfo>((resolve, reject) => {
            this.triggerAwaitingPromises.set(turnIndex, resolve)
            this.rejectAwaitingPromises.set(turnIndex, reject)
        })
        this.awaitingPromises.set(turnIndex, newAwaitingPromise)
        return newAwaitingPromise
    }
    
    private triggerAwaitingPromiseForTurn(turnIndex: number, turnInfo: BattleTurnInfo): void {
        if (this.triggerAwaitingPromises.has(turnIndex)) {
            const trigger = this.triggerAwaitingPromises.get(turnIndex)!
            this.triggerAwaitingPromises.delete(turnIndex)
            this.rejectAwaitingPromises.delete(turnIndex)
            this.awaitingPromises.delete(turnIndex)
            trigger(turnInfo)
        }
    }
    
    private rejectAwaitingPromiseForTurn(turnIndex: number, error: Error): void {
        if (this.rejectAwaitingPromises.has(turnIndex)) {
            const reject = this.rejectAwaitingPromises.get(turnIndex)!
            this.triggerAwaitingPromises.delete(turnIndex)
            this.rejectAwaitingPromises.delete(turnIndex)
            this.awaitingPromises.delete(turnIndex)
            reject(error)
        }
    }

    dispose(): void {
        this.disposed = true
        
        this.onCurrentPlayerActionReceived.disconnectAll()
        this.onServerMessageHandlingStarted.disconnectAll()

        this.rejectAwaitingPromises.forEach((reject: (error: Error) => void, key: number) =>
            this.rejectAwaitingPromiseForTurn(key, new Error("The battle is disposed"))
        )
        
        this.triggerAwaitingPromises.clear()
        this.rejectAwaitingPromises.clear()
        this.awaitingPromises.clear()
        this.previousTurnInfos.clear()
    }
    
    async onMessage(message: BattleCommandFromServer): Promise<void> {
        if (this.disposed) return

        // Queue work after any previously running animation completes.
        // Emit log notification right when handling starts (in sync with animations).
        const enqueue = (action: () => Promise<void> | void): Promise<void> => {
            this.previousAnimationPromise = this.previousAnimationPromise.then(async () => {
                if (this.disposed) return
                this.onServerMessageHandlingStarted.emit(message)
                await action()
            })
            return this.previousAnimationPromise
        }

        if (message.command === 'UNIT_MOVE') {
            this.cancelCurrentUserActionPending(message)
            const unit = getUnitById(this.mapController.map, message.actorId)
            if (unit) {
                return enqueue(() => this.mapController.moveUnit(unit, message.moveToCell.r, message.moveToCell.c))
            } else {
                throw Error("Target unit from server not found: " + message.actorId)
            }
        } else if (message.command === 'NEXT_TURN') {
            const turnInfo: BattleTurnInfo = {
                index: message.turnNumber,
                player: message.player,
                result: undefined,
                nextTurnUnitId: message.nextTurnUnitId,
                roundNumber: message.roundNumber,
            }
            return enqueue(() => {
                // Decrement buff durations for the active unit (server already processed this)
                // Buffs are removed via LOSE_BUFF message when durationRemaining goes below 0
                const activeUnit = getUnitById(this.mapController.map, message.nextTurnUnitId)
                if (activeUnit?.currentProps.buffs) {
                    for (const buff of activeUnit.currentProps.buffs) {
                        if (!buff.permanent) {
                            buff.durationRemaining--
                        }
                    }
                }
                this.onNextTurnInfo(turnInfo)
            })
        } else if (message.command === 'UNIT_ATTACK') {
            if (!message.isCounterattack) {
                this.cancelCurrentUserActionPending(message)
            }
            const unit = getUnitById(this.mapController.map, message.actorId)
            const target = getUnitById(this.mapController.map, message.targetId)
            if (unit && target) {
                return enqueue(() => this.mapController.unitAttacks(unit, target, message.attackIndex))
            } else {
                throw Error("Target unit from server not found: " + message.targetId + " or actor unit not found: " + message.actorId)
            }
        } else if (message.command === 'TAKE_DAMAGE') {
            const unit = getUnitById(this.mapController.map, message.actorId)
            if (unit) {
                return enqueue(() => this.mapController.unitTakeDamage(unit, message.damageTaken, message.killedCount, message.remainingCount, message.remainingHealth))
            } else {
                throw Error("Target unit from server not found: " + message.actorId)
            }
        } else if (message.command === 'BATTLE_FINISHED') {
            const turnInfo: BattleTurnInfo = {
                index: message.turnNumber,
                player: BattlePlayerNumber.Player1,
                result: {
                    finished: true,
                    winner: message.winner,
                    reportId: message.reportId
                },
                nextTurnUnitId: "",
                roundNumber: this.currentRoundNumber,
            }
            return enqueue(() => this.onNextTurnInfo(turnInfo))
        } else if (message.command === 'UNIT_PASS') {
            this.cancelCurrentUserActionPending(message)
            // No specific action needed for pass - just acknowledge receipt
            return enqueue(() => undefined)
        } else if (message.command === 'UNIT_WAIT') {
            this.cancelCurrentUserActionPending(message)
            const unit = getUnitById(this.mapController.map, message.actorId)
            if (unit) {
                return enqueue(() => this.mapController.unitWaits(unit))
            } else {
                throw Error("Target unit from server not found: " + message.actorId)
            }
        } else if (message.command === 'PLAYER_RANSOM') {
            this.cancelCurrentUserActionPending(message)
            this.mapController.map.players.find(player => player.playerNumber === message.player)!.ransomClaimed = true
            return enqueue(() => undefined)
        } else if (message.command === 'PLAYER_RUN') {
            this.cancelCurrentUserActionPending(message)
            this.mapController.map.players.find(player => player.playerNumber === message.player)!.runClaimed = true
            return enqueue(() => undefined)
        } else if (message.command === 'RECEIVE_BUFF') {
            const unit = getUnitById(this.mapController.map, message.actorId)
            if (unit) {
                this.cancelCurrentUserActionPending(message)
                return enqueue(async () => {
                    // Add the buff to the unit's buffs list
                    if (!unit.currentProps.buffs) {
                        unit.currentProps.buffs = []
                    }
                    unit.currentProps.buffs.push({
                        id: message.buffId,
                        name: message.buffName,
                        thumbnailUrl: message.thumbnailUrl,
                        permanent: message.permanent,
                        durationRemaining: message.durationRemaining,
                        stunned: message.stunned
                    })
                    // Update buff icons on the battlefield
                    await this.mapController.updateUnitBuffIcons(unit)
                })
            } else {
                throw Error("Target unit from server not found: " + message.actorId)
            }
        } else if (message.command === 'LOSE_BUFF') {
            const unit = getUnitById(this.mapController.map, message.actorId)
            if (unit) {
                return enqueue(async () => {
                    // Remove the buff from the unit's buffs list
                    if (unit.currentProps.buffs && message.buffId) {
                        const buffIdLower = message.buffId.toLowerCase()
                        const buffIndex = unit.currentProps.buffs.findIndex(b => b.id?.toLowerCase() === buffIdLower)
                        if (buffIndex >= 0) {
                            unit.currentProps.buffs.splice(buffIndex, 1)
                        }
                    }
                    // Update buff icons on the battlefield
                    await this.mapController.updateUnitBuffIcons(unit)
                })
            } else {
                throw Error("Target unit from server not found: " + message.actorId)
            }
        } else if (message.command === 'UNIT_HEALS') {
            const unit = getUnitById(this.mapController.map, message.actorId)
            if (unit) {
                return enqueue(() => this.mapController.unitHeals(unit, message.healedAmount, message.resurrectedCount, message.newCount, message.newHealth))
            } else {
                throw Error("Target unit from server not found: " + message.actorId)
            }
        }

        throw Error("Unknown or invalid command from server")
    }

    private cancelCurrentUserActionPending(command: BattleCommandFromServer): void {
        if ((command as PlayerCommandFromServer).player === this.getCurrentTurnInfo().player) {
            this.onCurrentPlayerActionReceived.emit()
        }
    }

    private onNextTurnInfo(turnInfo: BattleTurnInfo) {
        this.currentTurnIndex = turnInfo.index
        this.currentRoundNumber = turnInfo.roundNumber
        this.mapController.map.turnInfo = turnInfo
        this.previousTurnInfos.set(turnInfo.index, turnInfo)
        this.triggerAwaitingPromiseForTurn(turnInfo.index, turnInfo)
    }
    
}