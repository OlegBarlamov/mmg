using System.Collections.Generic;
using System.Linq;
using Epic.Core;
using Epic.Core.Services.Battles;
using Epic.Data.BattleObstacles;
using Epic.Data.UnitTypes.Subtypes;

namespace Epic.Logic.Battle.Map
{
    public static class MapUtils
    {
        public static bool IsEnemyInRange(IBattleUnitObject actor, int range,
            IReadOnlyCollection<IBattleUnitObject> units)
        {
            return units.Any(u => u.PlayerIndex != actor.PlayerIndex &&
                                  u.GlobalUnit.IsAlive && OddRHexoGrid.Distance(actor, u) <= range);
        }

        private static string CellToString(HexoPoint cell) => $"{cell.R},{cell.C}";

        private static bool IsCellBlocked(IBattleObject map, HexoPoint cell, IEnumerable<IBattleObstacleFields> obstacles, bool ignoreUnits)
        {
            // Units
            if (!ignoreUnits && map.Units.Any(x => x.GlobalUnit.IsAlive && x.Column == cell.C && x.Row == cell.R))
                return true;

            // Obstacles
            foreach (var obstacle in obstacles)
            {
                var dx = cell.C - obstacle.Column;
                var dy = cell.R - obstacle.Row;

                if (dx < 0 || dy < 0 || dx >= obstacle.Width || dy >= obstacle.Height)
                    continue;

                if (obstacle.Mask == null || obstacle.Mask[dx, dy])
                    return true;
            }

            return false;
        }

        /// <summary>
        /// BFS that yields reachable cells. 
        /// If target is given → stops when found.
        /// If moveRange is null → no distance limit.
        /// </summary>
        private static IEnumerable<HexoPoint> BfsExplore(
            IBattleObject map,
            IReadOnlyCollection<IBattleObstacleFields> obstacles,
            HexoPoint start,
            MovementType movementType,
            bool ignoreUnits,
            int? moveRange = null,
            HexoPoint? target = null)
        {
            var visited = new HashSet<string>();
            var queue = new Queue<(HexoPoint Cell, int Distance)>();
            queue.Enqueue((start, 0));

            while (queue.Count > 0)
            {
                var (currentCell, distance) = queue.Dequeue();
                var key = CellToString(currentCell);

                if (visited.Contains(key))
                    continue;

                if (moveRange.HasValue && distance > moveRange.Value)
                    continue;

                // Check if cell is blocked
                bool blocked = IsCellBlocked(map, currentCell, obstacles, ignoreUnits);

                // If target is specified, yield only if it can be stopped on
                if (target.HasValue && currentCell.Equals(target.Value))
                {
                    if (!blocked) // target is landable
                        yield return currentCell;
                    yield break; // stop BFS at target
                }

                // Ground cannot traverse blocked cells (except start)
                if (movementType == MovementType.Walk && blocked && !currentCell.Equals(start))
                    continue;

                // Mark visited AFTER deciding whether to skip
                visited.Add(key);

                // Yield reachable cells (skip start, only if not blocked)
                if (!currentCell.Equals(start) && !blocked)
                    yield return currentCell;

                // Enqueue neighbors
                var neighbors = GetNeighborCells(currentCell.R, currentCell.C, map.Height, map.Width);
                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(CellToString(neighbor)))
                        queue.Enqueue((neighbor, distance + 1));
                }
            }
        }

        /// <summary>
        /// All reachable cells for a unit (respects moveRange).
        /// </summary>
        public static List<HexoPoint> GetCellsForUnitMove(
            IBattleObject map,
            IReadOnlyCollection<IBattleObstacleFields> obstacles,
            IBattleUnitObject unit,
            MovementType movementType)
        {
            int moveRange = unit.GlobalUnit.UnitType.Speed;
            var start = new HexoPoint(unit.Column, unit.Row);

            return BfsExplore(map, obstacles, start, movementType, false, moveRange).ToList();
        }

        /// <summary>
        /// Checks if there is any path between two cells (ignores moveRange).
        /// </summary>
        public static bool IsPathAvailable(
            IBattleObject map,
            IReadOnlyCollection<IBattleObstacleFields> obstacles,
            HexoPoint start,
            HexoPoint target,
            MovementType movementType,
            bool ignoreUnits)
        {
            var reachableCells = BfsExplore(map, obstacles, start, movementType, ignoreUnits, null, target); 
            // Check if target is in the reachable cells
            return reachableCells.Any(cell => cell.Equals(target));
        }

        public static List<HexoPoint> GetNeighborCells(int row, int col, int rows, int cols)
        {
            bool IsValidCell(int r, int c)
            {
                return r >= 0 && r < rows && c >= 0 && c < cols;
            }

            // Neighbor directions for odd-r layout
            var oddRDirections = new (int dr, int dc)[]
            {
                (-1, 0), // top
                (-1, -1), // top-left
                (0, -1), // left
                (0, 1), // right
                (1, 0), // bottom
                (1, -1) // bottom-left
            };

            var evenRDirections = new (int dr, int dc)[]
            {
                (-1, 0), // top
                (-1, 1), // top-right
                (0, -1), // left
                (0, 1), // right
                (1, 0), // bottom
                (1, 1) // bottom-right
            };

            // Use different direction sets based on whether the row is even or odd
            var directions = (row % 2 == 0) ? oddRDirections : evenRDirections;

            var neighbors = new List<HexoPoint>();

            foreach (var (dr, dc) in directions)
            {
                int nr = row + dr;
                int nc = col + dc;

                if (IsValidCell(nr, nc))
                {
                    neighbors.Add(new HexoPoint(nc, nr));
                }
            }

            return neighbors;
        }
    }
}