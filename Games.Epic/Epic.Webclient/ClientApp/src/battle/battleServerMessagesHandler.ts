import { BattleCommandFromServer } from "../server/battleCommandFromServer";
import {IBattleConnectionMessagesHandler} from "../server/battleServerConnection";
import {getUnitById} from "./battleLogic";
import {IBattleMapController} from "../battleMap/battleMapController";
import {BattleTurnInfo} from "../battleMap/battleMap";
import { BattlePlayerNumber } from "../player/playerNumber";

export interface ITurnAwaiter {
    readonly currentTurnIndex: number
    getCurrentTurnInfo(): BattleTurnInfo 
    waitForTurn(turnIndex: number): Promise<BattleTurnInfo>
    dispose(): void
}

export class BattleServerMessagesHandler implements IBattleConnectionMessagesHandler, ITurnAwaiter {
    currentTurnIndex: number
    
    private disposed = false

    private readonly previousTurnInfos: Map<number, BattleTurnInfo> = new Map()
    private readonly awaitingPromises: Map<number, Promise<BattleTurnInfo>> = new Map()
    private readonly triggerAwaitingPromises: Map<number, (info: BattleTurnInfo) => void> = new Map()
    private readonly rejectAwaitingPromises: Map<number, (error: Error) => void> = new Map()
    
    constructor(private mapController: IBattleMapController, currentTurnInfo: BattleTurnInfo) {
        this.currentTurnIndex = currentTurnInfo.index
        this.previousTurnInfos.set(currentTurnInfo.index, currentTurnInfo)
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
        
        if (message.command === 'UNIT_MOVE') {
            const unit = getUnitById(this.mapController.map, message.actorId)
            if (unit) {
                await this.mapController.moveUnit(unit, message.moveToCell.r, message.moveToCell.c)
                return
            } else {
                throw Error("Target unit from server not found: " + message.actorId)
            }
        } else if (message.command === 'NEXT_TURN') {
            const turnInfo: BattleTurnInfo = {
                index: message.turnNumber,
                player: message.player,
                result: undefined,
            }
            this.onNextTurnInfo(turnInfo)
            return
        } else if (message.command === 'UNIT_ATTACK') {
            // Nothing
            return
        } else if (message.command === 'TAKE_DAMAGE') {
            const unit = getUnitById(this.mapController.map, message.actorId)
            if (unit) {
                await this.mapController.unitTakeDamage(unit, message.damageTaken)
                return
            } else {
                throw Error("Target unit from server not found: " + message.actorId)
            }
        } else if (message.command === 'BATTLE_FINISHED') {
            const turnInfo: BattleTurnInfo = {
                index: message.turnNumber,
                player: BattlePlayerNumber.Player1,
                result: {
                    finished: true,
                    winner: message.winner
                },
            }
            this.onNextTurnInfo(turnInfo)
            return
        }

        throw Error("Unknown or invalid command from server")
    }

    private onNextTurnInfo(turnInfo: BattleTurnInfo) {
        this.currentTurnIndex = turnInfo.index
        this.mapController.map.turnInfo = turnInfo
        this.previousTurnInfos.set(turnInfo.index, turnInfo)
        this.triggerAwaitingPromiseForTurn(turnInfo.index, turnInfo)
    }
    
}