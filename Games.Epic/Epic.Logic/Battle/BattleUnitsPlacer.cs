using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core;
using Epic.Core.Logic;
using Epic.Core.Services.Battles;

namespace Epic.Logic.Battle
{
    public class BattleUnitsPlacer : IBattleUnitsPlacer
    {
        public void PlaceBattleUnitsDefaultPattern(
            MutableBattleObject battleObject,
            int? player1OriginalContainerHeight = null,
            int? player2OriginalContainerHeight = null)
        {
            PlacePlayerUnits(
                battleObject,
                (int)InBattlePlayerNumber.Player1,
                defaultColumn: 0,
                originalContainerHeight: player1OriginalContainerHeight);
            PlacePlayerUnits(
                battleObject,
                (int)InBattlePlayerNumber.Player2,
                defaultColumn: battleObject.Width - 1,
                originalContainerHeight: player2OriginalContainerHeight);

            // Any non-battle units (invalid player index) are placed off-map
            foreach (var unit in battleObject.Units)
            {
                if (unit.PlayerIndex != (int)InBattlePlayerNumber.Player1 &&
                    unit.PlayerIndex != (int)InBattlePlayerNumber.Player2)
                {
                    unit.Column = -1;
                    unit.Row = -1;
                }
            }
        }

        private static void PlacePlayerUnits(
            MutableBattleObject battleObject,
            int playerIndex,
            int defaultColumn,
            int? originalContainerHeight)
        {
            // Only alive units participate in placement / blocking.
            var aliveUnits = battleObject.Units
                .Where(u => u.PlayerIndex == playerIndex && u.GlobalUnit is { IsAlive: true })
                .OrderBy(u => u.GlobalUnit.ContainerSlotIndex) // preserve initial hero slot ordering
                .ThenBy(u => u.Id)
                .ToList();

            // Put dead units off-map (keeps map clean, avoids accidental collisions)
            foreach (var unit in battleObject.Units.Where(u => u.PlayerIndex == playerIndex && !(u.GlobalUnit?.IsAlive ?? false)))
            {
                unit.Column = -1;
                unit.Row = -1;
            }

            if (aliveUnits.Count == 0)
                return;

            // When the map is taller than the amount of units, keep the original vertical pattern (hero slots / original rows)
            // and "stretch" it to the map height, creating gaps where needed.
            if (battleObject.Height > aliveUnits.Count)
            {
                var unitsWithOriginalRow = aliveUnits
                    .Select(u => new
                    {
                        Unit = u,
                        // Preserve original placement. For freshly created battle units Row is ContainerSlotIndex,
                        // but for other sources Row may already be set.
                        OriginalRow = Math.Max(0, u.Row)
                    })
                    .OrderBy(x => x.OriginalRow)
                    .ThenBy(x => x.Unit.GlobalUnit.ContainerSlotIndex)
                    .ThenBy(x => x.Unit.Id)
                    .ToList();

                var maxOriginalRowUsed = unitsWithOriginalRow.Max(x => x.OriginalRow);
                var heightFromUsedRows = maxOriginalRowUsed + 1;
                var heightFromContainer = Math.Max(0, originalContainerHeight ?? 0);
                var originalHeight = Math.Max(heightFromUsedRows, heightFromContainer);

                int[] rows;
                if (originalHeight <= 1)
                {
                    // Nothing to stretch (all original rows are 0). Fall back to even spacing by count.
                    rows = GetEvenlySpacedRows(battleObject.Height, unitsWithOriginalRow.Count);
                }
                else
                {
                    var desiredRows = unitsWithOriginalRow
                        .Select(x =>
                        {
                            var originalRow = Math.Min(x.OriginalRow, originalHeight - 1);
                            var scaled = originalRow * (battleObject.Height - 1) / (double)(originalHeight - 1);
                            return (int)Math.Round(scaled);
                        })
                        .ToArray();

                    rows = MakeStrictlyIncreasingWithinBounds(desiredRows, battleObject.Height);
                }

                for (var i = 0; i < unitsWithOriginalRow.Count; i++)
                {
                    unitsWithOriginalRow[i].Unit.Column = defaultColumn;
                    unitsWithOriginalRow[i].Unit.Row = rows[i];
                }

                return;
            }

            // Fallback to previous behavior: keep row if it fits & is unique, otherwise fill from top.
            var filledRows = Enumerable.Repeat(false, battleObject.Height).ToArray();
            var notFittingUnits = new List<MutableBattleUnitObject>();

            foreach (var unit in aliveUnits)
            {
                unit.Column = defaultColumn;

                if (unit.Row >= 0 && unit.Row < battleObject.Height && !filledRows[unit.Row])
                {
                    filledRows[unit.Row] = true;
                }
                else
                {
                    notFittingUnits.Add(unit);
                }
            }

            for (var i = 0; i < notFittingUnits.Count; i++)
            {
                var unit = notFittingUnits[i];
                var freeSlotIndex = Array.FindIndex(filledRows, filled => !filled);
                if (freeSlotIndex >= 0)
                {
                    unit.Row = freeSlotIndex;
                    filledRows[unit.Row] = true;
                }
                else
                {
                    // This should be rare because battle size usually limits unit count to height,
                    // but keep a deterministic overflow behavior just in case.
                    unit.Row = i % battleObject.Height;
                    var columnShift = (i / battleObject.Height) + 1; // shift away from the default column
                    unit.Column = playerIndex == (int)InBattlePlayerNumber.Player1
                        ? defaultColumn + columnShift
                        : defaultColumn - columnShift;
                }
            }
        }

        private static int[] MakeStrictlyIncreasingWithinBounds(int[] desiredRows, int mapHeight)
        {
            if (desiredRows.Length == 0)
                return Array.Empty<int>();

            if (mapHeight <= 1)
                return Enumerable.Repeat(0, desiredRows.Length).ToArray();

            // Requires desiredRows.Length <= mapHeight to be feasible.
            // In our usage this holds because we only call this when mapHeight > unitCount.
            var result = new int[desiredRows.Length];

            for (var i = 0; i < desiredRows.Length; i++)
            {
                var minAllowed = i == 0 ? 0 : result[i - 1] + 1;
                result[i] = Math.Max(desiredRows[i], minAllowed);
            }

            var maxRow = mapHeight - 1;
            if (result[^1] > maxRow)
            {
                result[^1] = maxRow;
                for (var i = result.Length - 2; i >= 0; i--)
                {
                    result[i] = Math.Min(result[i], result[i + 1] - 1);
                }
            }

            // Final clamp (shouldn't be needed, but keeps it safe)
            for (var i = 0; i < result.Length; i++)
            {
                if (result[i] < 0) result[i] = 0;
                if (result[i] > maxRow) result[i] = maxRow;
            }

            return result;
        }

        private static int[] GetEvenlySpacedRows(int mapHeight, int count)
        {
            if (count <= 0)
                return Array.Empty<int>();

            if (mapHeight <= 1)
                return Enumerable.Repeat(0, count).ToArray();

            if (count == 1)
                return new[] { (int)Math.Round((mapHeight - 1) / 2.0) };

            var result = new int[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = (int)Math.Round(i * (mapHeight - 1) / (double)(count - 1));
                if (result[i] < 0) result[i] = 0;
                if (result[i] >= mapHeight) result[i] = mapHeight - 1;
            }
            return result;
        }
    }
}