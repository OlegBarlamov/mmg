import './battleComponent.css'
import './battleUnitInfoModal.css'
import React, { PureComponent } from "react";
import { IServiceLocator } from "../services/serviceLocator";
import { IBattleController } from "../battle/battleController";
import { BattleMap, BattleMapCell, BattleTurnInfo, InBattlePlayerInfo } from "../battleMap/battleMap";
import { HexagonStyle } from "../services/canvasService";
import { EvenQGrid } from "../hexogrid/evenQGrid";
import { OddRGrid } from "../hexogrid/oddRGrid";
import { BattlePlayerNumber } from '../player/playerNumber';
import { RewardDialog } from './rewardDialog';
import { IRewardToAccept } from '../rewards/IRewardToAccept';
import { RewardType } from '../rewards/RewardType';
import { RewardManager, IRewardManagerState } from '../services/rewardManager';
import { BattleResultsModal } from './battleResultsModal';
import { IReportInfo } from '../services/serverAPI';
import { BattleControlPanel } from './battleControlPanel';
import { BattleMapUnit } from '../battleMap/battleMapUnit';
import { BattleUnitInfoModal } from './battleUnitInfoModal';
import { BattleCommandFromServer } from '../server/battleCommandFromServer';

const CanvasContainerId = 'CanvasContainer'

export interface IBattleComponentProps {
    serviceLocator: IServiceLocator
    battleMap: BattleMap

    onBattleFinished(newBattleMap?: BattleMap): void
}

const NO_TARGET_CAST_TYPES = ['AllEnemies', 'AllAllies', 'AllUnits']

interface IBattleComponentState {
    battleLoaded: boolean
    currentReward: IRewardToAccept | null
    rewards: IRewardToAccept[] | null
    currentRewardIndex: number
    battleReport: IReportInfo | null
    currentRoundNumber: number
    isPlayerTurn: boolean
    activeUnit: BattleMapUnit | null
    showUnitInfoModal: boolean
    selectedUnit: BattleMapUnit | null
    battleLog: string[]
    /** When set, the next map click (cell or unit) is used as the magic target, then we send the message. */
    awaitingMagicTarget: { magicTypeId: string; castTargetType: string; effectRadius?: number } | null
}

export class BattleComponent extends PureComponent<IBattleComponentProps, IBattleComponentState> {
    private battleController: IBattleController | null = null
    private controlPanelRef: React.RefObject<BattleControlPanel | null> = React.createRef()
    private contextMenuHandler: ((e: Event) => void) | null = null
    private rewardManager: RewardManager
    private readonly maxBattleLogLines = 200
    /** Used during magic target selection to restore previous hover highlight when moving to another cell/unit. */
    private lastMagicTargetHighlight: { cells?: BattleMapCell[]; unit?: BattleMapUnit } | null = null

    constructor(props: IBattleComponentProps) {
        super(props)

        this.state = {
            battleLoaded: false,
            currentReward: null,
            rewards: null,
            currentRewardIndex: 0,
            battleReport: null,
            currentRoundNumber: 0,
            isPlayerTurn: false,
            activeUnit: null,
            showUnitInfoModal: false,
            selectedUnit: null,
            battleLog: [],
            awaitingMagicTarget: null
        }

        // Initialize reward manager with callbacks
        this.rewardManager = new RewardManager(
            this.props.serviceLocator.serverAPI(),
            {
                onRewardComplete: this.handleRewardComplete,
                onGuardBattleBegins: this.handleGuardBattleBegins,
                onRewardError: this.handleRewardError
            }
        );
    }

    async componentDidMount() {
        const canvasContainer = document.getElementById(CanvasContainerId)!

        // Prevent default context menu on canvas to allow our custom right-click handling
        this.contextMenuHandler = (e: Event) => {
            e.preventDefault()
        }
        canvasContainer.addEventListener('contextmenu', this.contextMenuHandler)

        const canvasService = this.props.serviceLocator.canvasService()
        await canvasService.init(canvasContainer, this.getMapHexagonStyle(this.props.battleMap))

        const battlesService = this.props.serviceLocator.battlesService()
        const panelController = this.controlPanelRef.current!
        this.battleController = await battlesService.createBattle(this.props.battleMap, panelController)

        // Calculate and apply the appropriate scale to fit the map in the container
        const targetScale = this.calculateTargetScale(canvasContainer, this.props.battleMap)
        canvasService.setScale(targetScale)

        // Subscribe to turn changes
        this.battleController.onNextTurn.connect(this.handleNextTurn)
        // Subscribe when message handling starts (in sync with animations)
        this.battleController.onServerMessageHandlingStarted.connect(this.handleServerMessage)

        // Set up right-click handler for unit info modal
        this.battleController.mapController.onUnitRightMouseClick = this.handleUnitRightClick

        // Do not wait
        this.startBattle()

        // Initialize the turn state based on the initial battle state
        const initialTurnInfo = this.props.battleMap.turnInfo
        const isPlayerTurn = this.battleController!.isPlayerControlled(initialTurnInfo.player)
        const activeUnit = initialTurnInfo.nextTurnUnitId ?
            this.props.battleMap.units.find(unit => unit.id === initialTurnInfo.nextTurnUnitId) ?? null
            : null

        this.setState({
            ...this.state,
            battleLoaded: true,
            isPlayerTurn: isPlayerTurn,
            activeUnit: activeUnit
        })
    }

    componentDidUpdate(prevProps: IBattleComponentProps, prevState: IBattleComponentState) {
        const mapController = this.battleController?.mapController;
        if (!mapController) return;
        const wasAwaiting = prevState.awaitingMagicTarget != null;
        const isAwaiting = this.state.awaitingMagicTarget != null;
        if (isAwaiting && !wasAwaiting) {
            this.lastMagicTargetHighlight = null;
            const { magicTypeId, castTargetType, effectRadius } = this.state.awaitingMagicTarget!;
            const highlighter = mapController.battleMapHighlighter;
            const map = this.props.battleMap;
            mapController.battleMapHighlighter.clearTurnHighlights();
            if (castTargetType === 'Location') {
                const radius = effectRadius ?? 0;
                mapController.onCellClickIntercept = (cell) => {
                    this.handleMagicAction(magicTypeId, { targetRow: cell.r, targetColumn: cell.c });
                    return true;
                };
                mapController.onUnitClickIntercept = null;
                mapController.onCellMouseEnter = (cell) => {
                    if (cell.isObstacle) return;
                    if (this.lastMagicTargetHighlight?.cells) {
                        highlighter.restoreMagicTargetCells(this.lastMagicTargetHighlight.cells);
                    }
                    const cellsInRange = map.grid.getCellsInRange(cell.r, cell.c, radius);
                    highlighter.highlightMagicTargetCells(cellsInRange);
                    highlighter.setCursorForMapCell(cell, 'pointer');
                    this.lastMagicTargetHighlight = { cells: cellsInRange };
                };
                mapController.onCellMouseLeave = (cell) => {
                    if (this.lastMagicTargetHighlight?.cells) {
                        highlighter.restoreMagicTargetCells(this.lastMagicTargetHighlight.cells);
                        this.lastMagicTargetHighlight = null;
                    }
                    highlighter.setCursorForMapCell(cell, undefined);
                };
                mapController.onUnitMouseEnter = null;
                mapController.onUnitMouseLeave = null;
            } else {
                // Enemy/Ally: consume cell clicks so they don't trigger move; only valid unit clicks select target
                const currentPlayerNumber = this.props.battleMap.players.find(
                    p => p.playerId === this.props.serviceLocator.playerService().currentPlayerInfo.id
                )?.playerNumber;
                const isValidUnitTarget = (unit: BattleMapUnit) => {
                    if (currentPlayerNumber == null) return false;
                    if (castTargetType === 'Enemy') return unit.player !== currentPlayerNumber;
                    if (castTargetType === 'Ally') return unit.player === currentPlayerNumber;
                    return false;
                };
                mapController.onCellClickIntercept = () => true;
                mapController.onUnitClickIntercept = (unit) => {
                    if (!isValidUnitTarget(unit)) return true; // consume click but don't send
                    this.handleMagicAction(magicTypeId, { targetUnitId: unit.id });
                    return true;
                };
                mapController.onCellMouseEnter = null;
                mapController.onCellMouseLeave = null;
                mapController.onUnitMouseEnter = async (unit) => {
                    if (!isValidUnitTarget(unit)) return;
                    if (this.lastMagicTargetHighlight?.unit) {
                        await highlighter.restoreMagicTargetUnit(this.lastMagicTargetHighlight.unit);
                    }
                    await highlighter.highlightMagicTargetUnit(unit);
                    highlighter.setCursorForMapUnit(unit, 'pointer');
                    this.lastMagicTargetHighlight = { unit };
                };
                mapController.onUnitMouseLeave = async (unit) => {
                    if (this.lastMagicTargetHighlight?.unit) {
                        await highlighter.restoreMagicTargetUnit(this.lastMagicTargetHighlight.unit);
                        this.lastMagicTargetHighlight = null;
                    }
                    highlighter.setCursorForMapUnit(unit, undefined);
                };
            }
        } else if (!isAwaiting && wasAwaiting) {
            this.lastMagicTargetHighlight = null;
            mapController.onCellClickIntercept = null;
            mapController.onUnitClickIntercept = null;
            mapController.onCellMouseEnter = null;
            mapController.onCellMouseLeave = null;
            mapController.onUnitMouseEnter = null;
            mapController.onUnitMouseLeave = null;
            mapController.battleMapHighlighter.restoreTurnHighlights();
        }
    }

    private handleServerMessage = (message: BattleCommandFromServer) => {
        const line = this.formatBattleServerMessage(message)
        this.setState(prev => {
            const next = [...prev.battleLog, line]
            const trimmed = next.length > this.maxBattleLogLines ? next.slice(next.length - this.maxBattleLogLines) : next
            return { battleLog: trimmed }
        })
    }

    private getPlayerLabel(player: BattlePlayerNumber): string {
        const myPlayerId = this.props.serviceLocator.playerService().currentPlayerInfo.id
        const myPlayerNumber = this.props.battleMap.players.find(p => p.playerId === myPlayerId)?.playerNumber
        if (myPlayerNumber && player === myPlayerNumber) return "You"
        return "Enemy"
    }

    private getUnitLabel(unitId: string): string {
        const unit =
            this.battleController?.mapController.map.units.find(u => u.id === unitId) ??
            this.props.battleMap.units.find(u => u.id === unitId)
        if (!unit) return "A unit"
        const suffix = unitId.length >= 4 ? ` #${unitId.slice(-4)}` : ""
        return `${unit.name ?? "Unit"}${suffix}`
    }

    private formatBattleServerMessage(message: BattleCommandFromServer): string {
        switch (message.command) {
            case 'NEXT_TURN':
                return `Round ${message.roundNumber + 1}: ${this.getPlayerLabel(message.player)} turn.`
            case 'UNIT_MOVE':
                return `${this.getUnitLabel(message.actorId)} moved.`
            case 'UNIT_ATTACK':
                return message.isCounterattack
                    ? `${this.getUnitLabel(message.actorId)} counterattacked ${this.getUnitLabel(message.targetId)}.`
                    : `${this.getUnitLabel(message.actorId)} attacked ${this.getUnitLabel(message.targetId)}.`
            case 'TAKE_DAMAGE':
                {
                    const unitLabel = this.getUnitLabel(message.actorId)
                    const lostPart = message.killedCount > 0 ? ` and lost ${message.killedCount}` : ""
                    const tail = message.remainingCount <= 0
                        ? ` It was destroyed.`
                        : ` (${message.remainingHealth} health left)`
                    return `${unitLabel} took ${message.damageTaken} damage${lostPart}.${tail}`
                }
            case 'UNIT_PASS':
                return `${this.getUnitLabel(message.actorId)} passed.`
            case 'UNIT_WAIT':
                return `${this.getUnitLabel(message.actorId)} waited.`
            case 'PLAYER_RANSOM':
                return `${this.getPlayerLabel(message.player)} offered a ransom.`
            case 'PLAYER_RUN':
                return `${this.getPlayerLabel(message.player)} tried to run away.`
            case 'PLAYER_MAGIC':
                {
                    const magicLabel = message.magicName ?? message.magicTypeId.replace(/_/g, ' ')
                    return `${this.getPlayerLabel(message.player)} cast ${magicLabel}.`
                }
            case 'BATTLE_FINISHED':
                if (message.winner) {
                    const winnerLabel = this.getPlayerLabel(message.winner)
                    return winnerLabel === "You"
                        ? `Battle finished. You won!`
                        : `Battle finished. You lost.`
                }
                return `Battle finished.`
            case 'RECEIVE_BUFF':
                {
                    const unitLabel = this.getUnitLabel(message.actorId)
                    const durationText = message.permanent 
                        ? '' 
                        : ` (${message.durationRemaining})`
                    return `${unitLabel} received "${message.buffName}"${durationText}.`
                }
            case 'LOSE_BUFF':
                {
                    const unitLabel = this.getUnitLabel(message.actorId)
                    return `${unitLabel}'s "${message.buffName}" expired.`
                }
            case 'UNIT_HEALS':
                {
                    const unitLabel = this.getUnitLabel(message.actorId)
                    const resurrectedPart = message.resurrectedCount > 0 
                        ? ` and resurrected ${message.resurrectedCount}` 
                        : ""
                    return `${unitLabel} healed ${message.healedAmount} HP${resurrectedPart}.`
                }
            default:
                return `Something happened.`
        }
    }


    private handleNextTurn = (turnInfo: BattleTurnInfo) => {
        const isPlayerTurn = this.battleController!.isPlayerControlled(turnInfo.player)
        const activeUnit = turnInfo.nextTurnUnitId ?
            this.props.battleMap.units.find(unit => unit.id === turnInfo.nextTurnUnitId) ?? null
            : null

        this.setState({
            currentRoundNumber: turnInfo.roundNumber,
            isPlayerTurn: isPlayerTurn,
            activeUnit: activeUnit
        })
    }

    private handleUnitRightClick = (unit: BattleMapUnit, event: PointerEvent) => {
        this.setState({
            showUnitInfoModal: true,
            selectedUnit: unit
        })
    }

    private handleUnitInfoModalClose = () => {
        this.setState({
            showUnitInfoModal: false,
            selectedUnit: null
        })
    }

    private handleRansomAction = async () => {
        if (!this.battleController) return;
        
        try {
            const playerService = this.props.serviceLocator.playerService();
            
            // Find the current player's battle info
            const currentPlayer = this.props.battleMap.players.find(
                player => player.playerId === playerService.currentPlayerInfo.id
            );
            
            if (!currentPlayer) {
                console.error('Current player not found in battle');
                return;
            }
            
            // Create ransom action
            const ransomAction = {
                command: 'PLAYER_RANSOM' as const,
                player: currentPlayer.playerNumber
            };
            
            // Process the action directly through the battle action processor
            const battleActionProcessor = (this.battleController as any).battleActionProcessor;
            if (battleActionProcessor) {
                await battleActionProcessor.processAction(ransomAction);
            }
        } catch (error) {
            console.error('Failed to process ransom action:', error);
        }
    }

    private handleRunAction = async () => {
        if (!this.battleController) return;
        
        try {
            const playerService = this.props.serviceLocator.playerService();
            
            // Find the current player's battle info
            const currentPlayer = this.props.battleMap.players.find(
                player => player.playerId === playerService.currentPlayerInfo.id
            );
            
            if (!currentPlayer) {
                console.error('Current player not found in battle');
                return;
            }
            
            // Create run action
            const runAction = {
                command: 'PLAYER_RUN' as const,
                player: currentPlayer.playerNumber
            };
            
            // Process the action directly through the battle action processor
            const battleActionProcessor = (this.battleController as any).battleActionProcessor;
            if (battleActionProcessor) {
                await battleActionProcessor.processAction(runAction);
            }
        } catch (error) {
            console.error('Failed to process run action:', error);
        }
    }

    private handleMagicAction = async (
        magicTypeId: string,
        options?: { castTargetType?: string; effectRadius?: number; targetUnitId?: string; targetRow?: number; targetColumn?: number }
    ) => {
        if (!this.battleController) return;
        const playerService = this.props.serviceLocator.playerService();
        const currentPlayer = this.props.battleMap.players.find(
            player => player.playerId === playerService.currentPlayerInfo.id
        );
        if (!currentPlayer) {
            console.error('Current player not found in battle');
            return;
        }

        const castTargetType = options?.castTargetType;
        const hasTarget = options?.targetUnitId != null || (options?.targetRow != null && options?.targetColumn != null);

        if (castTargetType != null && NO_TARGET_CAST_TYPES.includes(castTargetType)) {
            // No target selection: send immediately
            await this.sendPlayerMagic(currentPlayer.playerNumber, magicTypeId, undefined);
            return;
        }
        if (castTargetType != null && (castTargetType === 'Enemy' || castTargetType === 'Ally' || castTargetType === 'Location')) {
            // Enter target selection mode
            this.setState({ awaitingMagicTarget: { magicTypeId, castTargetType, effectRadius: options?.effectRadius } });
            return;
        }
        if (hasTarget) {
            // Completing target selection: restore highlight then send and clear
            if (this.battleController) {
                const highlighter = this.battleController.mapController.battleMapHighlighter;
                if (this.lastMagicTargetHighlight?.unit) {
                    await highlighter.restoreMagicTargetUnit(this.lastMagicTargetHighlight.unit);
                }
                if (this.lastMagicTargetHighlight?.cells) {
                    highlighter.restoreMagicTargetCells(this.lastMagicTargetHighlight.cells);
                }
                this.lastMagicTargetHighlight = null;
            }
            this.setState({ awaitingMagicTarget: null });
            await this.sendPlayerMagic(currentPlayer.playerNumber, magicTypeId, options);
            return;
        }
        // Fallback: send with no target (e.g. legacy call with just magicTypeId)
        await this.sendPlayerMagic(currentPlayer.playerNumber, magicTypeId, options);
    }

    private sendPlayerMagic = async (
        playerNumber: BattlePlayerNumber,
        magicTypeId: string,
        options?: { targetUnitId?: string; targetRow?: number; targetColumn?: number }
    ) => {
        if (!this.battleController) return;
        try {
            const battleActionProcessor = (this.battleController as any).battleActionProcessor;
            if (battleActionProcessor) {
                await battleActionProcessor.processAction({
                    command: 'PLAYER_MAGIC' as const,
                    player: playerNumber,
                    magicTypeId,
                    targetUnitId: options?.targetUnitId,
                    targetRow: options?.targetRow,
                    targetColumn: options?.targetColumn,
                });
            }
        } catch (error) {
            console.error('Failed to process magic action:', error);
        }
    }

    private async startBattle() {
        const battleResult = await this.battleController!.startBattle()

        // If we have a report ID, fetch and show the battle results first (for both wins and losses)
        if (battleResult.reportId) {
            try {
                const serverAPI = this.props.serviceLocator.serverAPI()
                const report = await serverAPI.getReport(battleResult.reportId)
                this.setState({ battleReport: report })
                return // Don't proceed yet, wait for modal to close
            } catch (error) {
                console.error('Failed to fetch battle report:', error)
                // Continue even if report fetch fails
            }
        }

        // If no report or report fetch failed, proceed based on winner
        if (battleResult.winner === BattlePlayerNumber.Player1) {
            await this.proceedToRewards()
        } else {
            this.props.onBattleFinished()
        }
    }

    private async proceedToRewards() {
        const rewardState = await this.rewardManager.checkForRewards()
        
        if (rewardState.rewards && rewardState.rewards.length > 0) {
            this.setState({
                rewards: rewardState.rewards,
                currentReward: rewardState.currentReward,
                currentRewardIndex: rewardState.currentRewardIndex
            })
        } else {
            this.props.onBattleFinished()
        }
    }

    private handleBattleResultsOk = () => {
        // Capture the isWinner value before clearing the state
        const isWinner = this.state.battleReport?.isWinner

        this.setState({ battleReport: null })

        // Check if the player won to determine next action
        if (isWinner) {
            this.proceedToRewards()
        } else {
            this.props.onBattleFinished()
        }
    }

    private handleRewardComplete = () => {
        // Clear reward state when all rewards are complete
        this.setState({
            currentReward: null,
            rewards: null,
            currentRewardIndex: 0
        })
        this.props.onBattleFinished()
    }

    private handleGuardBattleBegins = (battleMap: any) => {
        // Pass the new battle map to the parent component
        this.props.onBattleFinished(battleMap)
    }

    private handleRewardError = (error: Error) => {
        console.error('Reward error:', error)
        this.props.onBattleFinished()
    }

    private handleRewardAccept = async (affectedSlots: number[]) => {
        await this.rewardManager.acceptReward(affectedSlots)
        const state = this.rewardManager.getState()
        this.setState({
            rewards: state.rewards,
            currentReward: state.currentReward,
            currentRewardIndex: state.currentRewardIndex
        })
    }

    private handleRewardDecline = async () => {
        await this.rewardManager.declineReward()
        const state = this.rewardManager.getState()
        this.setState({
            rewards: state.rewards,
            currentReward: state.currentReward,
            currentRewardIndex: state.currentRewardIndex
        })
    }

    private handleBeginGuardBattle = async (rewardId: string) => {
        await this.rewardManager.beginGuardBattle(rewardId)
    }

    async componentWillUnmount() {
        if (this.battleController) {
            this.battleController.onNextTurn.disconnect(this.handleNextTurn)
            this.battleController.onServerMessageHandlingStarted.disconnect(this.handleServerMessage)
            this.battleController.mapController.onUnitRightMouseClick = null
            this.battleController.mapController.onCellClickIntercept = null
            this.battleController.mapController.onUnitClickIntercept = null
            this.battleController.dispose()
        }

        // Remove the context menu event listener
        const canvasContainer = document.getElementById(CanvasContainerId)
        if (canvasContainer) {
            canvasContainer.removeEventListener('contextmenu', this.contextMenuHandler!)
        }
    }

    private renderHeroStats(player: InBattlePlayerInfo): React.ReactNode {
        const { heroStats } = player
        if (!heroStats) return null
        const myPlayerId = this.props.serviceLocator.playerService().currentPlayerInfo.id
        const isMyself = player.playerId === myPlayerId
        const parts: string[] = [
            `Attack: ${heroStats.attack}`,
            `Defense: ${heroStats.defense}`,
            `Knowledge: ${heroStats.knowledge}`,
            `Power: ${heroStats.power}`,
        ]
        if (isMyself) {
            parts.push(`Mana: ${heroStats.currentMana}/${heroStats.maxMana}`)
        }
        return parts.join(' | ')
    }

    private getMapHexagonStyle(map: BattleMap): HexagonStyle {
        if (map.grid instanceof EvenQGrid) {
            return HexagonStyle.QStyle
        } else if (map.grid instanceof OddRGrid) {
            return HexagonStyle.RStyle
        }

        throw new Error("Unknown type of the battle map hexo grid")
    }

    private calculateTargetScale(container: HTMLElement, battleMap: BattleMap): number {
        const containerSize = {
            width: container.clientWidth,
            height: container.clientHeight
        }

        // Handle edge case where container has no size
        if (containerSize.width <= 0 || containerSize.height <= 0) {
            return 1.0
        }

        const availableWidth = containerSize.width
        const availableHeight = containerSize.height

        // Handle edge case where available space is too small
        if (availableWidth <= 0 || availableHeight <= 0) {
            return 1.0
        }

        const hexagonRadius = this.battleController!.mapController.cellRadius
        // Calculate the map size using the grid's getSize method
        const mapSize = battleMap.grid.getSize(hexagonRadius)

        // Handle edge case where map has no size
        if (mapSize.width <= 0 || mapSize.height <= 0) {
            return 1.0
        }

        // Calculate scale factors for both width and height using available space
        const scaleX = availableWidth / mapSize.width
        const scaleY = availableHeight / mapSize.height

        // Use the smaller scale to ensure the map fits in both dimensions
        const targetScale = Math.min(scaleX, scaleY, 1.0) // Don't scale up beyond 1.0

        return targetScale
    }

    render() {
        const canvasStyle: React.CSSProperties = {
            visibility: this.state.battleLoaded ? 'visible' : 'hidden'
        };

        const player1 = this.props.battleMap.players.find(p => p.playerNumber === BattlePlayerNumber.Player1)
        const player2 = this.props.battleMap.players.find(p => p.playerNumber === BattlePlayerNumber.Player2)

        return (
            <div className={"Container"}>
                {(!this.state.battleLoaded) && (
                    <div>Loading...</div>
                )}

                {this.state.battleLoaded && (
                    <div className="battle-header">
                        {player1?.heroStats && (
                            <div className="hero-stats-text hero-stats-left">
                                {this.renderHeroStats(player1)}
                            </div>
                        )}
                        <div className="round-number">Round {this.state.currentRoundNumber + 1}</div>
                        {player2?.heroStats && (
                            <div className="hero-stats-text hero-stats-right">
                                {this.renderHeroStats(player2)}
                            </div>
                        )}
                    </div>
                )}

                <div className="battle-canvas-wrapper">
                    <div id={CanvasContainerId} style={canvasStyle}></div>
                    {this.state.awaitingMagicTarget && (
                        <div className="battle-magic-target-overlay">
                            <span className="battle-magic-target-hint">
                                Select target on the map
                                {this.state.awaitingMagicTarget.castTargetType === 'Location' ? ' (click a cell)' : ' (click a unit)'}
                            </span>
                            <button
                                type="button"
                                className="battle-magic-target-cancel"
                                onClick={() => this.setState({ awaitingMagicTarget: null })}
                            >
                                Cancel
                            </button>
                        </div>
                    )}
                </div>

                <div className="battle-control-panel-wrapper">
                    <BattleControlPanel
                        ref={this.controlPanelRef}
                        isVisible={this.state.battleLoaded}
                        isPlayerTurn={this.state.isPlayerTurn}
                        activeUnit={this.state.activeUnit}
                        serviceLocator={this.props.serviceLocator}
                        battleId={this.props.battleMap.id}
                        battleMap={this.props.battleMap}
                        currentPlayerId={this.props.serviceLocator.playerService().currentPlayerInfo.id}
                        battleLogLines={this.state.battleLog}
                        onRansomAction={this.handleRansomAction}
                        onRunAction={this.handleRunAction}
                        onMagicCast={this.handleMagicAction}
                        canUseMagic={this.state.isPlayerTurn && (() => {
                            const currentPlayer = this.props.battleMap.players.find(
                                (p: InBattlePlayerInfo) => p.playerId === this.props.serviceLocator.playerService().currentPlayerInfo.id
                            );
                            return currentPlayer != null && !currentPlayer.magicUsedThisRound && !currentPlayer.ransomClaimed && !currentPlayer.runClaimed;
                        })()}
                    />
                </div>

                {this.state.battleReport && (
                    <BattleResultsModal
                        report={this.state.battleReport}
                        onOk={this.handleBattleResultsOk}
                    />
                )}

                {this.state.currentReward && (
                    <RewardDialog
                        reward={this.state.currentReward}
                        onAccept={this.handleRewardAccept}
                        onDecline={this.handleRewardDecline}
                        onBeginGuardBattle={this.handleBeginGuardBattle}
                        serviceLocator={this.props.serviceLocator}
                    />
                )}

                <BattleUnitInfoModal
                    isVisible={this.state.showUnitInfoModal}
                    unit={this.state.selectedUnit}
                    battleId={this.props.battleMap.id}
                    serverAPI={this.props.serviceLocator.serverAPI()}
                    onClose={this.handleUnitInfoModalClose}
                />
            </div>
        )
    }
}