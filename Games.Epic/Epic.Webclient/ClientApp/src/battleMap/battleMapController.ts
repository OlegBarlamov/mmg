import { IHexagon } from "../canvas/hexagon";
import { BattleMap, BattleMapCell } from "./battleMap";
import { ICanvasService } from "../services/canvasService";
import { IUnitTile } from "../canvas/unitTile";
import { getPlayerColor } from "../player/getPlayerColor";
import { BattleMapUnit } from "./battleMapUnit";
import { Point } from "../common/Point";
import { distance } from "../common/math";
import { BattleMapHighlighter, IBattleMapHighlighter } from "./battleMapHighlighter";
import { EffectAnimationCommandFromServer, normalizeEffectAnimationType } from "../server/battleCommandFromServer";
import { getUnitById } from "../battle/battleLogic";
import { BattlePlayerNumber } from "../player/playerNumber";

export interface IBattleMapController {
    readonly cellRadius: number
    readonly map: BattleMap
    readonly battleMapHighlighter: IBattleMapHighlighter

    moveUnit(unit: BattleMapUnit, row: number, col: number): Promise<void>
    unitWaits(unit: BattleMapUnit): Promise<void>
    unitAttacks(unit: BattleMapUnit, target: BattleMapUnit, attackIndex: number): Promise<void>

    removeUnit(unit: BattleMapUnit): Promise<void>

    getClosestCellToPoint(cells: BattleMapCell[], canvasPoint: Point): BattleMapCell

    unitTakeDamage(unit: BattleMapUnit, damageTaken: number, killedCount: number, remainingCount: number, remainingHealth: number): Promise<void>
    unitHeals(unit: BattleMapUnit, healedAmount: number, resurrectedCount: number, newCount: number, newHealth: number): Promise<void>
    
    updateUnitBuffIcons(unit: BattleMapUnit): Promise<void>

    /** Play effect animation (e.g. from magic). Does not add to battle log. */
    playEffectAnimation(message: EffectAnimationCommandFromServer): Promise<void>

    /** Called when a unit is removed from the map (e.g. killed). Use to refresh move highlights. */
    onUnitRemoved: (() => void) | null

    onCellMouseClick: ((cell: BattleMapCell, event: PointerEvent) => void) | null
    onUnitMouseClick: ((unit: BattleMapUnit, event: PointerEvent) => void) | null
    /** If set and returns true, the click is consumed and onCellMouseClick is not called. */
    onCellClickIntercept: ((cell: BattleMapCell, event: PointerEvent) => boolean) | null
    /** If set and returns true, the click is consumed and onUnitMouseClick is not called. */
    onUnitClickIntercept: ((unit: BattleMapUnit, event: PointerEvent) => boolean) | null
    onUnitRightMouseClick: ((unit: BattleMapUnit, event: PointerEvent) => void) | null

    onCellMouseEnter: ((cell: BattleMapCell, event: PointerEvent) => void) | null
    onCellMouseLeave: ((cell: BattleMapCell, event: PointerEvent) => void) | null
    onUnitMouseEnter: ((unit: BattleMapUnit, event: PointerEvent) => void) | null
    onUnitMouseLeave: ((unit: BattleMapUnit, event: PointerEvent) => void) | null

    destroy(): void
}

export class BattleMapController implements IBattleMapController {
    readonly map: BattleMap
    readonly battleMapHighlighter: IBattleMapHighlighter

    onCellMouseClick: ((cell: BattleMapCell, event: PointerEvent) => void) | null = null
    onUnitMouseClick: ((unit: BattleMapUnit, event: PointerEvent) => void) | null = null
    onCellClickIntercept: ((cell: BattleMapCell, event: PointerEvent) => boolean) | null = null
    onUnitClickIntercept: ((unit: BattleMapUnit, event: PointerEvent) => boolean) | null = null
    onUnitRightMouseClick: ((unit: BattleMapUnit, event: PointerEvent) => void) | null = null

    onCellMouseEnter: ((cell: BattleMapCell, event: PointerEvent) => void) | null = null
    onCellMouseLeave: ((cell: BattleMapCell, event: PointerEvent) => void) | null = null

    onUnitMouseMove: ((unit: BattleMapUnit, event: PointerEvent) => void) | null = null
    onUnitMouseEnter: ((unit: BattleMapUnit, event: PointerEvent) => void) | null = null
    onUnitMouseLeave: ((unit: BattleMapUnit, event: PointerEvent) => void) | null = null

    onUnitRemoved: (() => void) | null = null

    private hexagons: IHexagon[][] = []
    private units: IUnitTile[] = []

    readonly cellRadius: number = 60
    private readonly visualOffset: Point
    private readonly defaultCellsStrokeColor = 0xFFFFFF
    private readonly defaultCellsFillColor = 0x66CCFF
    private readonly unitStrokeColor = 0x111111
    private readonly unitsNumberBackgroundImgSrc = "https://static.vecteezy.com/system/resources/thumbnails/012/981/790/small/old-parchment-paper-scroll-sheet-vintage-aged-or-texture-background-png.png"

    private readonly canvasService: ICanvasService
    private readonly battleMapHighlighterImpl: BattleMapHighlighter

    constructor(model: BattleMap, canvasService: ICanvasService) {
        this.map = model
        this.canvasService = canvasService
        this.battleMapHighlighterImpl = new BattleMapHighlighter(canvasService, this, this.defaultCellsFillColor)
        this.battleMapHighlighter = this.battleMapHighlighterImpl

        this.visualOffset = this.getCanvasOffset()
    }

    destroy(): void {
        this.battleMapHighlighterImpl.destroy()

        this.units.forEach(x => this.canvasService.destroyUnit(x))
        this.units = []

        this.hexagons.forEach(rows => rows.forEach(x => this.canvasService.destroyHexagon(x)))
        this.hexagons = []
    }

    getClosestCellToPoint(cells: BattleMapCell[], canvasPoint: Point): BattleMapCell {
        const hexagonCenters = cells.map(cell => {
            return {
                cell,
                centerPoint: this.map.grid.getCellCenterPoint(cell.r, cell.c, this.cellRadius)
            }
        })

        let closestHexagon = hexagonCenters[0]
        let closestHexagonDistanceSqr = Number.MAX_SAFE_INTEGER
        for (const hexagonPair of hexagonCenters) {
            const distanceSqr = distance(canvasPoint, hexagonPair.centerPoint)
            if (distanceSqr < closestHexagonDistanceSqr) {
                closestHexagon = hexagonPair
                closestHexagonDistanceSqr = distanceSqr
            }
        }

        return closestHexagon.cell
    }

    async unitWaits(unit: BattleMapUnit): Promise<void> {
       unit.currentProps.waited = true
       
       // Get the unit tile for animation
       const unitTile = this.getUnitTile(unit)
       
       // Animate the wait effect
       await this.canvasService.animateUnitWait(unitTile, {
           duration: 300 // Slightly longer duration for a more noticeable wait effect
       })
    }

    async unitAttacks(unit: BattleMapUnit, target: BattleMapUnit, attackIndex: number): Promise<void> {
        unit.currentProps.attacksStates[attackIndex].bulletsCount--
        
        // Get the unit tiles for animation
        const attackerTile = this.getUnitTile(unit)
        const targetTile = this.getUnitTile(target)
        
        // Animate the attack
        await this.canvasService.animateUnitAttack(attackerTile, targetTile, {
            duration: 200,
            attackDistance: 0.25, // Move 25% toward target
            returnDuration: 200
        })
    }

    async unitTakeDamage(target: BattleMapUnit, damageTaken: number, killedCount: number, remainingCount: number, remainingHealth: number): Promise<void> {
        target.currentProps.health = remainingHealth
        target.count = remainingCount

        const unitTile = this.getUnitTile(target)
        
        // Animate damage effect
        await this.canvasService.animateUnitDamage(unitTile, {
            duration: 300
        })

        if (target.count < 1) {
            await this.removeUnit(target)
        } else {
            await this.canvasService.changeUnit(unitTile, {
                ...unitTile,
                text: target.count.toString()
            })
        }
    }

    async unitHeals(unit: BattleMapUnit, healedAmount: number, resurrectedCount: number, newCount: number, newHealth: number): Promise<void> {
        unit.currentProps.health = newHealth
        unit.count = newCount

        const unitTile = this.getUnitTile(unit)
        
        // Animate heal effect
        await this.canvasService.animateUnitHeal(unitTile, {
            duration: 300
        })

        await this.canvasService.changeUnit(unitTile, {
            ...unitTile,
            text: unit.count.toString()
        })
    }

    async playEffectAnimation(message: EffectAnimationCommandFromServer): Promise<void> {
        const animationType = normalizeEffectAnimationType(message.animationType)
        if (animationType === 'None' || !message.effectSpriteUrl) return

        const targetPoint = this.map.grid.getCellCenterPoint(message.targetRow, message.targetColumn, this.cellRadius)
        const targetX = targetPoint.x + this.visualOffset.x
        const targetY = targetPoint.y + this.visualOffset.y

        let sourceX: number | undefined
        let sourceY: number | undefined
        if (animationType === 'FromSource') {
            if (message.sourceUnitId) {
                const sourceUnit = getUnitById(this.map, message.sourceUnitId)
                if (sourceUnit) {
                    const pt = this.map.grid.getCellCenterPoint(sourceUnit.position.r, sourceUnit.position.c, this.cellRadius)
                    sourceX = pt.x + this.visualOffset.x
                    sourceY = pt.y + this.visualOffset.y
                }
            }
            if (sourceX == null || sourceY == null) {
                // No source unit: use top corner of canvas (Player1 = top-left, Player2 = top-right)
                const canvasSize = this.canvasService.size()
                const inset = 24
                const playerNum = message.sourcePlayer === 'Player2' ? BattlePlayerNumber.Player2 : BattlePlayerNumber.Player1
                sourceX = playerNum === BattlePlayerNumber.Player1 ? inset : canvasSize.width - inset
                sourceY = inset
            }
        }

        await this.canvasService.playEffectAnimation({
            spriteUrl: message.effectSpriteUrl,
            animationType,
            targetX,
            targetY,
            sourceX,
            sourceY,
            animationTimeMs: message.animationTimeMs,
        })
    }

    removeUnit(unit: BattleMapUnit): Promise<void> {
        const unitTile = this.getUnitTile(unit)
        this.canvasService.destroyUnit(unitTile)
        const unitIndex = this.map.units.indexOf(unit)
        if (unitIndex >= 0) {
            this.map.units.splice(this.map.units.indexOf(unit), 1)
        }
        this.units.splice(this.units.indexOf(unitTile), 1)
        this.onUnitRemoved?.()
        return Promise.resolve()
    }

    private getCanvasOffset(): Point {
        const offset = { x: 0, y: 0 }

        const containerSize = this.canvasService.size()
        const gridDesiredSize = this.map.grid.getSize(this.cellRadius)

        // Use the original container size for positioning calculations
        // The scale affects the rendering, not the positioning logic
        if (gridDesiredSize.width < containerSize.width) {
            offset.x = (containerSize.width - gridDesiredSize.width) / 2
        }
        if (gridDesiredSize.height < containerSize.height) {
            offset.y = (containerSize.height - gridDesiredSize.height) / 2
        }

        return offset
    }

    async moveUnit(unit: BattleMapUnit, r: number, c: number): Promise<void> {
        const unitTile = this.getUnitTile(unit)

        const newPoint = this.map.grid.getCellCenterPoint(r, c, this.cellRadius)
        await this.canvasService.changeUnitAnimated(unitTile, {
            ...unitTile,
            hexagon: {
                ...unitTile.hexagon,
                x: newPoint.x + this.visualOffset.x,
                y: newPoint.y + this.visualOffset.y,
            }
        }, {
            duration: 300, // 300ms animation duration
            easing: 'easeInOut'
        })
        unit.position = { r, c }
    }

    getHexagon(row: number, col: number): IHexagon {
        return this.hexagons[row][col]
    }

    getUnitTile(unit: BattleMapUnit): IUnitTile {
        const tileUnit = this.units.find(x => x.model === unit)
        if (!tileUnit) throw new Error("Unit not found on the map")
        return tileUnit
    }

    async updateUnitBuffIcons(unit: BattleMapUnit): Promise<void> {
        const unitTile = this.getUnitTile(unit)
        
        // Get buffs that have thumbnail URLs
        const buffsWithIcons = (unit.currentProps.buffs ?? [])
            .filter(b => b.thumbnailUrl)
            .map(b => ({ id: b.id, thumbnailUrl: b.thumbnailUrl! }))
        
        await this.canvasService.updateUnitBuffIcons(unitTile, buffsWithIcons)
    }

    loadMap(): Promise<void> {
        if (this.hexagons.length !== 0) throw new Error("Cannot regenerate the visuals of already loaded map")

        for (let r = 0; r < this.map.grid.height; r++) {
            const row: IHexagon[] = [];
            for (let c = 0; c < this.map.grid.width; c++) {
                const cell = this.map.grid.getCell(r, c)

                const alpha = cell.isObstacle ? 0 : 1.0
                const center = this.map.grid.getCellCenterPoint(r, c, this.cellRadius)
                const hexagonView = this.canvasService.createHexagon({
                    x: center.x + this.visualOffset.x,
                    y: center.y + this.visualOffset.y,
                    radius: this.cellRadius,
                    strokeColor: this.defaultCellsStrokeColor,
                    fillColor: this.defaultCellsFillColor,
                    strokeLine: cell.isObstacle ? 0 : 2,
                    fillAlpha: alpha,
                    strokeAlpha: alpha,
                })

                hexagonView.onMouseEnters = (sender, event) => this.onCellMouseEnter?.(cell, this.translatePointerEvent(event, this.canvasService))
                hexagonView.onMouseLeaves = (sender, event) => this.onCellMouseLeave?.(cell, this.translatePointerEvent(event, this.canvasService))
                hexagonView.onMouseUp = (sender, event) => {
                    const e = this.translatePointerEvent(event, this.canvasService)
                    if (this.onCellClickIntercept?.(cell, e)) return
                    this.onCellMouseClick?.(cell, e)
                }

                row.push(hexagonView);
            }
            this.hexagons.push(row);
        }

        return Promise.resolve()
    }

    async loadUnits(): Promise<void> {
        if (this.units.length !== 0) throw new Error("Cannot regenerate the units tiles of already loaded map")

        for (const unit of this.map.units) {
            if (!unit.isAlive) {
                continue
            }

            const center = this.map.grid.getCellCenterPoint(unit.position.r, unit.position.c, this.cellRadius)
            const unitTile = await this.canvasService.createUnit({
                model: unit,
                hexagon: {
                    x: center.x + this.visualOffset.x,
                    y: center.y + this.visualOffset.y,
                    radius: this.cellRadius,
                    strokeColor: this.unitStrokeColor,
                    fillColor: getPlayerColor(unit.player),
                    strokeLine: 2,
                    fillAlpha: 1,
                    strokeAlpha: 1,
                },
                text: unit.count.toString(),
                textBackgroundImgSrc: this.unitsNumberBackgroundImgSrc,
                imgSrc: unit.currentProps.battleImgUrl
            })

            unitTile.onMouseMove = (sender, event) => this.onUnitMouseMove?.(unit, this.translatePointerEvent(event, this.canvasService))
            unitTile.onMouseEnters = (sender, event) => this.onUnitMouseEnter?.(unit, this.translatePointerEvent(event, this.canvasService))
            unitTile.onMouseLeaves = (sender, event) => this.onUnitMouseLeave?.(unit, this.translatePointerEvent(event, this.canvasService))
            unitTile.onMouseUp = (sender, event) => {
                const e = this.translatePointerEvent(event, this.canvasService)
                if (this.onUnitClickIntercept?.(unit, e)) return
                this.onUnitMouseClick?.(unit, e)
            }
            unitTile.onRightClick = (sender, event) => this.onUnitRightMouseClick?.(unit, this.translatePointerEvent(event, this.canvasService))

            this.units.push(unitTile)
            
            // Load initial buff icons (without animation for existing buffs)
            const buffsWithIcons = (unit.currentProps.buffs ?? [])
                .filter(b => b.thumbnailUrl)
                .map(b => ({ id: b.id, thumbnailUrl: b.thumbnailUrl! }))
            if (buffsWithIcons.length > 0) {
                await this.canvasService.updateUnitBuffIcons(unitTile, buffsWithIcons)
            }
        }
    }

    private translatePointerEvent(event: PointerEvent, canvasService: ICanvasService): PointerEvent {
        const pixiEvent = event as any;
        const global = pixiEvent.data?.global ?? { x: event.x, y: event.y }
        const resultPoint = canvasService.toLocal(global)
        
        return {
            ...event,
            x: resultPoint.x - this.visualOffset.x,
            y: resultPoint.y - this.visualOffset.y,
        }
    }
}
