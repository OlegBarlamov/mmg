import './artifactsInventoryModal.css'
import React, { PureComponent } from "react";
import { IServiceLocator } from "../services/serviceLocator";
import { ArtifactSlot, IArtifactInfo, IHeroStats } from "../services/serverAPI";

export interface IArtifactsInventoryModalProps {
    isVisible: boolean
    serviceLocator: IServiceLocator
    heroStats?: IHeroStats | null
    onClose: () => void
    onArtifactsChanged?: () => void
}

interface IArtifactsInventoryModalState {
    isLoading: boolean
    isSubmitting: boolean
    errorMessage: string | null
    artifacts: IArtifactInfo[]
    selectedArtifactId: string | null
    pendingEquipArtifactId: string | null
    pendingEquipSlots: number[]
}

type EquipmentSlotDescriptor = {
    index: number
    slot: ArtifactSlot
    label: string
}

const EQUIPMENT_SLOTS: EquipmentSlotDescriptor[] = [
    // Bag (5)
    { index: 0, slot: ArtifactSlot.Bag, label: "Bag 1" },
    { index: 1, slot: ArtifactSlot.Bag, label: "Bag 2" },
    { index: 2, slot: ArtifactSlot.Bag, label: "Bag 3" },
    { index: 3, slot: ArtifactSlot.Bag, label: "Bag 4" },
    { index: 4, slot: ArtifactSlot.Bag, label: "Bag 5" },
    // Wrist (2)
    { index: 5, slot: ArtifactSlot.Wrist, label: "Wrist 1" },
    { index: 6, slot: ArtifactSlot.Wrist, label: "Wrist 2" },
    // Single slots
    { index: 7, slot: ArtifactSlot.Head, label: "Head" },
    { index: 8, slot: ArtifactSlot.Body, label: "Body" },
    { index: 9, slot: ArtifactSlot.Hand, label: "Hand" },
    { index: 10, slot: ArtifactSlot.Shield, label: "Shield" },
    { index: 11, slot: ArtifactSlot.Neck, label: "Neck" },
    { index: 12, slot: ArtifactSlot.Cloak, label: "Cloak" },
    { index: 13, slot: ArtifactSlot.Legs, label: "Legs" },
]

const SLOT_TYPE_BY_INDEX = new Map<number, ArtifactSlot>(EQUIPMENT_SLOTS.map(s => [s.index, s.slot]))

function artifactDisplayName(a: IArtifactInfo): string {
    return a.typeName ?? a.typeKey ?? a.id
}

function formatSigned(n: number): string {
    return n >= 0 ? `+${n}` : `${n}`
}

function getSlotName(slot: ArtifactSlot): string {
    switch (slot) {
        case ArtifactSlot.Bag: return "Bag"
        case ArtifactSlot.Hand: return "Hand"
        case ArtifactSlot.Body: return "Body"
        case ArtifactSlot.Head: return "Head"
        case ArtifactSlot.Cloak: return "Cloak"
        case ArtifactSlot.Legs: return "Legs"
        case ArtifactSlot.Neck: return "Neck"
        case ArtifactSlot.Shield: return "Shield"
        case ArtifactSlot.Wrist: return "Wrist"
        default: return "Unknown"
    }
}

function artifactHoverText(a: IArtifactInfo): string {
    const lines: string[] = [artifactDisplayName(a)]
    
    const statParts: string[] = []
    if ((a.attackBonus ?? 0) !== 0) statParts.push(`ATK ${formatSigned(a.attackBonus)}`)
    if ((a.defenseBonus ?? 0) !== 0) statParts.push(`DEF ${formatSigned(a.defenseBonus)}`)
    if (statParts.length > 0) lines.push(statParts.join(" | "))
    
    // Show slots if artifact takes more than one slot
    const requiredSlots = (a.slots ?? []).filter(s => s !== ArtifactSlot.None)
    if (requiredSlots.length > 1) {
        const slotNames = requiredSlots.map(getSlotName).join(", ")
        lines.push("Slots: " + slotNames)
    }
    
    const buffNames = a.buffNames ?? []
    if (buffNames.length > 0) {
        lines.push("Buffs: " + buffNames.join(", "))
    }

    return lines.join("\n")
}

function getRequiredSlots(artifact: IArtifactInfo | null): ArtifactSlot[] {
    return (artifact?.slots ?? []).filter(s => s !== ArtifactSlot.None)
}

function addToCountMap(map: Map<ArtifactSlot, number>, slot: ArtifactSlot, amount: number) {
    map.set(slot, (map.get(slot) ?? 0) + amount)
}

function getCounts(slots: ArtifactSlot[]): Map<ArtifactSlot, number> {
    const map = new Map<ArtifactSlot, number>()
    for (const s of slots) addToCountMap(map, s, 1)
    return map
}

function getSlotTypesForIndexes(indexes: number[]): ArtifactSlot[] {
    return indexes
        .map(i => SLOT_TYPE_BY_INDEX.get(i) ?? ArtifactSlot.None)
        .filter(s => s !== ArtifactSlot.None)
}

function countsEqual(a: Map<ArtifactSlot, number>, b: Map<ArtifactSlot, number>): boolean {
    if (a.size !== b.size) return false
    for (const [k, v] of a) {
        if ((b.get(k) ?? 0) !== v) return false
    }
    return true
}

export class ArtifactsInventoryModal extends PureComponent<IArtifactsInventoryModalProps, IArtifactsInventoryModalState> {
    constructor(props: IArtifactsInventoryModalProps) {
        super(props)
        this.state = {
            isLoading: false,
            isSubmitting: false,
            errorMessage: null,
            artifacts: [],
            selectedArtifactId: null,
            pendingEquipArtifactId: null,
            pendingEquipSlots: [],
        }
    }

    async componentDidMount() {
        if (this.props.isVisible) {
            await this.fetchArtifacts()
        }
    }

    async componentDidUpdate(prevProps: IArtifactsInventoryModalProps) {
        if (this.props.isVisible && !prevProps.isVisible) {
            await this.fetchArtifacts()
        }
    }

    private fetchArtifacts = async () => {
        this.setState({ isLoading: true, errorMessage: null })
        try {
            const serverAPI = this.props.serviceLocator.serverAPI()
            const artifacts = await serverAPI.getMyArtifacts()
            const prevSelectedId = this.state.selectedArtifactId
            const selectedArtifactId =
                prevSelectedId && artifacts.some(a => a.id === prevSelectedId)
                    ? prevSelectedId
                    : (artifacts[0]?.id ?? null)
            this.setState({
                artifacts,
                isLoading: false,
                selectedArtifactId,
                pendingEquipArtifactId: null,
                pendingEquipSlots: [],
            })
        } catch (e) {
            this.setState({
                isLoading: false,
                errorMessage: e instanceof Error ? e.message : "Failed to load artifacts",
            })
        }
    }

    private getSelectedArtifact(): IArtifactInfo | null {
        const id = this.state.selectedArtifactId
        if (!id) return null
        return this.state.artifacts.find(a => a.id === id) ?? null
    }

    private getOccupiedMap(): Map<number, IArtifactInfo> {
        const map = new Map<number, IArtifactInfo>()
        for (const a of this.state.artifacts) {
            for (const idx of (a.equippedSlotsIndexes ?? [])) {
                map.set(idx, a)
            }
        }
        return map
    }

    private handleSelectArtifact = (artifactId: string) => {
        this.setState({
            selectedArtifactId: artifactId,
            pendingEquipArtifactId: artifactId,
            pendingEquipSlots: [],
            errorMessage: null,
        })
    }

    private handleClickEquipmentSlotIndex = async (equipmentSlotIndex: number) => {
        if (this.state.isSubmitting) return

        const pendingId = this.state.pendingEquipArtifactId
        if (!pendingId) return

        const pendingArtifact = this.state.artifacts.find(a => a.id === pendingId) ?? null
        const requiredSlots = getRequiredSlots(pendingArtifact)
        if (!pendingArtifact || requiredSlots.length === 0) return

        const occupied = this.getOccupiedMap()

        const pendingSet = new Set(this.state.pendingEquipSlots)

        // Toggle off if already selected.
        if (pendingSet.has(equipmentSlotIndex)) {
            const next = this.state.pendingEquipSlots.filter(i => i !== equipmentSlotIndex)
            this.setState({ pendingEquipSlots: next })
            return
        }

        // Validate target slot is free (or occupied by this artifact) and matches remaining requirements.
        const slotType = SLOT_TYPE_BY_INDEX.get(equipmentSlotIndex) ?? ArtifactSlot.None
        if (slotType === ArtifactSlot.None) return

        const requiredCounts = getCounts(requiredSlots)
        const selectedTypes = getSlotTypesForIndexes(this.state.pendingEquipSlots)
        const selectedCounts = getCounts(selectedTypes)
        const alreadySelectedCount = selectedCounts.get(slotType) ?? 0
        const requiredCount = requiredCounts.get(slotType) ?? 0
        if (alreadySelectedCount >= requiredCount) return

        const nextSlots = [...this.state.pendingEquipSlots, equipmentSlotIndex]
        const nextTypes = getSlotTypesForIndexes(nextSlots)
        const nextCounts = getCounts(nextTypes)

        // Auto-equip when selection matches required slots multiset.
        if (nextSlots.length === requiredSlots.length && countsEqual(requiredCounts, nextCounts)) {
            this.setState({ isSubmitting: true, errorMessage: null })
            try {
                const serverAPI = this.props.serviceLocator.serverAPI()
                // Client-side swap logic:
                // If any chosen indexes are occupied by other artifacts, unequip those first.
                const occupiedNow = this.getOccupiedMap()
                const conflictingArtifactIds = nextSlots
                    .map(idx => occupiedNow.get(idx))
                    .filter(a => !!a && a.id !== pendingArtifact.id)
                    .map(a => a!.id)
                    .filter((id, i, arr) => arr.indexOf(id) === i)

                for (const conflictId of conflictingArtifactIds) {
                    await serverAPI.equipArtifact(conflictId, [])
                }

                const updated = await serverAPI.equipArtifact(pendingArtifact.id, nextSlots)

                // Refresh list because multiple artifacts may have changed (swap).
                const artifacts = await serverAPI.getMyArtifacts()
                this.setState({
                    artifacts,
                    selectedArtifactId: null,
                    pendingEquipArtifactId: null,
                    pendingEquipSlots: [],
                })
                this.props.onArtifactsChanged?.()
            } catch (e) {
                this.setState({
                    errorMessage: e instanceof Error ? e.message : "Failed to equip artifact",
                })
            } finally {
                this.setState({ isSubmitting: false })
            }
            return
        }

        this.setState({ pendingEquipSlots: nextSlots })
    }

    private handleUnequip = async () => {
        const artifact = this.getSelectedArtifact()
        if (!artifact) return

        this.setState({ isSubmitting: true, errorMessage: null })
        try {
            const serverAPI = this.props.serviceLocator.serverAPI()
            const updated = await serverAPI.equipArtifact(artifact.id, [])
            this.setState(prev => ({
                artifacts: prev.artifacts.map(a => a.id === updated.id ? updated : a),
                selectedArtifactId: null,
                pendingEquipArtifactId: null,
                pendingEquipSlots: [],
            }))
            this.props.onArtifactsChanged?.()
        } catch (e) {
            this.setState({
                errorMessage: e instanceof Error ? e.message : "Failed to unequip artifact",
            })
        } finally {
            this.setState({ isSubmitting: false })
        }
    }

    private handleBackpackClicked = async () => {
        if (this.state.isSubmitting) return
        const selected = this.getSelectedArtifact()
        if (!selected) return

        const isEquipped = (selected.equippedSlotsIndexes?.length ?? 0) > 0
        if (!isEquipped) return

        await this.handleUnequip()
    }

    render() {
        if (!this.props.isVisible) return null

        const selected = this.getSelectedArtifact()
        const occupied = this.getOccupiedMap()
        const unequipped = this.state.artifacts.filter(a => (a.equippedSlotsIndexes?.length ?? 0) === 0)
        const selectedRequiredSlots = getRequiredSlots(selected)
        const pendingArtifact = this.state.pendingEquipArtifactId
            ? (this.state.artifacts.find(a => a.id === this.state.pendingEquipArtifactId) ?? null)
            : null
        const pendingRequiredSlots = getRequiredSlots(pendingArtifact)
        const pendingSlotsSet = new Set(this.state.pendingEquipSlots)
        const canUnequipToBackpack = !!selected && (selected.equippedSlotsIndexes?.length ?? 0) > 0 && !this.state.isSubmitting

        // Compute which slot indexes are valid targets for the pending artifact (based on remaining required slots).
        const targetIndexes = new Set<number>()
        if (pendingArtifact && pendingRequiredSlots.length > 0) {
            const requiredCounts = getCounts(pendingRequiredSlots)
            const selectedCounts = getCounts(getSlotTypesForIndexes(this.state.pendingEquipSlots))
            const occupiedByOther = new Map<number, IArtifactInfo>()
            for (const [idx, a] of occupied) {
                if (a.id !== pendingArtifact.id) occupiedByOther.set(idx, a)
            }

            for (const s of EQUIPMENT_SLOTS) {
                const already = selectedCounts.get(s.slot) ?? 0
                const need = requiredCounts.get(s.slot) ?? 0
                if (already < need) {
                    targetIndexes.add(s.index)
                }
            }
        }

        const renderEquipmentSlot = (index: number, label: string) => {
            const occ = occupied.get(index)
            const isSelected = !!selected && (selected.equippedSlotsIndexes?.includes(index) ?? false)
            const occName = occ ? artifactDisplayName(occ) : null
            const occTitle = occ ? artifactHoverText(occ) : undefined
            const isTarget = targetIndexes.has(index)
            const isPicked = pendingSlotsSet.has(index)
            const clickable = !!occ || isTarget || isPicked
            const willSwap = !!pendingArtifact && isTarget && !!occ && occ.id !== pendingArtifact.id
            return (
                <div
                    className={`equipment-slot ${isSelected ? 'active' : ''} ${clickable ? 'clickable' : ''} ${isTarget ? 'target' : ''} ${isPicked ? 'picked' : ''} ${willSwap ? 'swap' : ''}`}
                    onClick={() => {
                        // If we're in equip mode and this is a valid target, clicking equips (even if occupied).
                        if (isTarget || isPicked || (occ && occ.id === this.state.pendingEquipArtifactId)) {
                            this.handleClickEquipmentSlotIndex(index)
                            return
                        }

                        // Otherwise allow selecting the currently equipped artifact.
                        if (occ && occ.id !== this.state.pendingEquipArtifactId) {
                            this.handleSelectArtifact(occ.id)
                            return
                        }
                    }}
                    role={clickable ? "button" : undefined}
                    tabIndex={clickable ? 0 : undefined}
                    onKeyDown={(e) => {
                        if (!clickable) return
                        if (e.key === 'Enter' || e.key === ' ') {
                            if (isTarget || isPicked || (occ && occ.id === this.state.pendingEquipArtifactId)) {
                                this.handleClickEquipmentSlotIndex(index)
                                return
                            }
                            if (occ && occ.id !== this.state.pendingEquipArtifactId) {
                                this.handleSelectArtifact(occ.id)
                            }
                        }
                    }}
                >
                    <div className="equipment-slot-label">{label}</div>
                    <div className="equipment-slot-value">
                        {occ ? (
                            <div className="artifact-thumb-wrapper has-tooltip" data-tooltip={occTitle}>
                                <img
                                    className="artifact-thumb"
                                    src={occ.thumbnailUrl ?? "/resources/question.png"}
                                    alt={occName ?? "Artifact"}
                                />
                            </div>
                        ) : (
                            <div className="artifact-thumb artifact-thumb-empty" />
                        )}
                    </div>
                </div>
            )
        }

        const renderEquipmentCell = (index: number, label: string, cellClassName?: string) => (
            <div key={`slot-${index}`} className={`equipment-cell ${cellClassName ?? ''}`.trim()}>
                {renderEquipmentSlot(index, label)}
            </div>
        )

        return (
            <div className="artifacts-modal-overlay">
                <div className="artifacts-modal">
                    <div className="artifacts-modal-header">
                        <h2>Inventory</h2>
                        <button className="close-button" onClick={this.props.onClose}>×</button>
                    </div>

                    <div className="artifacts-modal-content">
                        {this.state.isLoading ? (
                            <div className="artifacts-loading">Loading artifacts...</div>
                        ) : (
                            <div className="artifacts-layout">
                                <div className="equipment-panel">
                                    <div className="panel-title">Equipment</div>
                                    {this.props.heroStats && (
                                        <div className="hero-stats">
                                            Hero stats: <b>ATK</b> {this.props.heroStats.attack} / <b>DEF</b> {this.props.heroStats.defense}
                                        </div>
                                    )}
                                    <div className="equipment-layout">
                                        {/* 3 columns */}
                                        <div className="equipment-row-3">
                                            {renderEquipmentCell(7, "Head", "col-2")}
                                        </div>
                                        <div className="equipment-row-3">
                                            {renderEquipmentCell(11, "Neck", "col-2")}
                                        </div>
                                        <div className="equipment-row-3">
                                            {renderEquipmentCell(9, "Hand")}
                                            {renderEquipmentCell(8, "Body")}
                                            {renderEquipmentCell(10, "Shield")}
                                        </div>
                                        <div className="equipment-row-3">
                                            {renderEquipmentCell(5, "Wrist", "col-1")}
                                            {renderEquipmentCell(6, "Wrist", "col-3")}
                                        </div>

                                        {/* 2 columns */}
                                        <div className="equipment-row-2">
                                            {renderEquipmentCell(12, "Cloak")}
                                            {renderEquipmentCell(13, "Legs")}
                                        </div>

                                        {/* 5 columns */}
                                        <div className="equipment-row-5">
                                            {renderEquipmentCell(0, "Bag 1")}
                                            {renderEquipmentCell(1, "Bag 2")}
                                            {renderEquipmentCell(2, "Bag 3")}
                                            {renderEquipmentCell(3, "Bag 4")}
                                            {renderEquipmentCell(4, "Bag 5")}
                                        </div>
                                    </div>
                                </div>

                                <div className="inventory-panel">
                                    <div className="panel-title">Artifacts</div>

                                    {this.state.errorMessage && (
                                        <div className="artifacts-error">{this.state.errorMessage}</div>
                                    )}

                                    <div className="artifacts-section">
                                        <div className="section-title">Backpack</div>
                                        <div
                                            className={`artifacts-thumbs-grid ${canUnequipToBackpack ? 'backpack-unequip-target' : ''}`}
                                            onClick={this.handleBackpackClicked}
                                            role={canUnequipToBackpack ? "button" : undefined}
                                            tabIndex={canUnequipToBackpack ? 0 : undefined}
                                            onKeyDown={(e) => {
                                                if (e.key === 'Enter' || e.key === ' ') this.handleBackpackClicked()
                                            }}
                                        >
                                            {unequipped.length === 0 ? (
                                                <div className="empty-hint">No unequipped artifacts</div>
                                            ) : unequipped.map(a => (
                                                <button
                                                    key={a.id}
                                                    className={`artifact-thumb-button ${a.id === this.state.selectedArtifactId ? 'selected' : ''}`}
                                                    onClick={(e) => {
                                                        e.stopPropagation()
                                                        this.handleSelectArtifact(a.id)
                                                    }}
                                                    title={artifactDisplayName(a)}
                                                >
                                                    <div className="artifact-thumb-wrapper has-tooltip" data-tooltip={artifactHoverText(a)}>
                                                        <img
                                                            className="artifact-thumb artifact-thumb-grid"
                                                            src={a.thumbnailUrl ?? "/resources/question.png"}
                                                            alt={artifactDisplayName(a)}
                                                        />
                                                    </div>
                                                </button>
                                            ))}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        )
    }
}

