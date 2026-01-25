using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Data.Artifacts;

namespace Epic.Core.Services.Artifacts
{
    /// <summary>
    /// Defines mapping between artifact slot types and concrete hero equipment slot indexes.
    /// Indexes are stable and can be persisted in data.
    ///
    /// Hero equipment slots:
    /// - Bag: 5 slots
    /// - Wrist: 2 slots
    /// - All other slots: 1 slot each
    /// </summary>
    public static class ArtifactEquipmentSlots
    {
        // Stable ordering; do not reorder without a migration.
        private static readonly ArtifactSlot[] _slotsByIndex =
        {
            // Bag (5)
            ArtifactSlot.Bag, ArtifactSlot.Bag, ArtifactSlot.Bag, ArtifactSlot.Bag, ArtifactSlot.Bag,
            // Wrist (2)
            ArtifactSlot.Wrist, ArtifactSlot.Wrist,
            // Single slots
            ArtifactSlot.Head,
            ArtifactSlot.Body,
            ArtifactSlot.Hand,
            ArtifactSlot.Shield,
            ArtifactSlot.Neck,
            ArtifactSlot.Cloak,
            ArtifactSlot.Legs,
        };

        public static IReadOnlyList<ArtifactSlot> SlotsByIndex => _slotsByIndex;

        public static int TotalSlotsCount => _slotsByIndex.Length;

        public static bool TryGetSlotType(int equipmentSlotIndex, out ArtifactSlot slot)
        {
            if (equipmentSlotIndex < 0 || equipmentSlotIndex >= _slotsByIndex.Length)
            {
                slot = ArtifactSlot.None;
                return false;
            }

            slot = _slotsByIndex[equipmentSlotIndex];
            return true;
        }

        public static IReadOnlyList<int> GetIndexesForSlot(ArtifactSlot slot)
        {
            return _slotsByIndex
                .Select((s, i) => new { Slot = s, Index = i })
                .Where(x => x.Slot == slot)
                .Select(x => x.Index)
                .ToArray();
        }
    }
}

