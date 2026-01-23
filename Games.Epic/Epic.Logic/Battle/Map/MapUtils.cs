using System;
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

        /// <summary>
        /// Gets units at the specified positions from the collection of all units.
        /// </summary>
        public static List<TBattleUnit> GetUnitsAtPositions<TBattleUnit>(
            IEnumerable<HexoPoint> positions,
            IReadOnlyCollection<TBattleUnit> allUnits) where TBattleUnit : IBattleUnitObject
        {
            var result = new List<TBattleUnit>();
            foreach (var position in positions)
            {
                var unitAtPosition = allUnits.FirstOrDefault(u =>
                    u.GlobalUnit.IsAlive &&
                    u.Row == position.R &&
                    u.Column == position.C);
                
                if (unitAtPosition != null)
                {
                    result.Add(unitAtPosition);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets neighbor cells around the attacker position (after move), walking in a circle starting from the target's direction.
        /// Splash = 1: 3 cells (attacker + 1 from left + 1 from right)
        /// Splash = 2: 5 cells (attacker + 2 from left + 2 from right)
        /// Splash = 3: all 6 neighbors
        /// </summary>
        public static List<HexoPoint> GetSplashNeighbors(
            HexoPoint attackerPosition,
            HexoPoint targetPosition,
            int splash,
            int mapHeight,
            int mapWidth)
        {
            if (splash <= 0)
                return new List<HexoPoint>();
            
            // Get all neighbors of the attacker position (after move)
            var allNeighbors = GetNeighborCells(attackerPosition.R, attackerPosition.C, mapHeight, mapWidth);
            
            // Exclude both the attacker position itself and the target position from splash
            // (attacker shouldn't hit itself, and target is already hit as primary target)
            allNeighbors = allNeighbors.Where(n => 
                !(n.R == attackerPosition.R && n.C == attackerPosition.C) &&
                !(n.R == targetPosition.R && n.C == targetPosition.C)).ToList();
            
            if (splash >= 3)
            {
                // Return all remaining neighbors (all 6 minus attacker and target if they were neighbors)
                return allNeighbors;
            }
            
            // Convert to axial coordinates for angle calculation
            var attackerAxial = OddRHexoGrid.ToAxial(attackerPosition.R, attackerPosition.C);
            var targetAxial = OddRHexoGrid.ToAxial(targetPosition.R, targetPosition.C);
            
            // Direction vector from attacker to target (this is our reference direction)
            var attackDq = targetAxial.q - attackerAxial.q;
            var attackDr = targetAxial.r - attackerAxial.r;
            
            // Calculate angle for each neighbor relative to the attack direction
            // We'll order them by walking around the circle starting from the target's direction
            var neighborsWithAngle = allNeighbors.Select(neighbor =>
            {
                var neighborAxial = OddRHexoGrid.ToAxial(neighbor.R, neighbor.C);
                // Direction vector from attacker to neighbor
                var neighborDq = neighborAxial.q - attackerAxial.q;
                var neighborDr = neighborAxial.r - attackerAxial.r;
                
                // Calculate cross product to determine side (positive = right/counterclockwise, negative = left/clockwise)
                // In hex grids, we use the cross product in axial coordinates
                var crossProduct = attackDq * neighborDr - attackDr * neighborDq;
                
                // Calculate angle using atan2 for proper ordering around the circle
                // We want to order starting from the attack direction and going around
                var dotProduct = attackDq * neighborDq + attackDr * neighborDr;
                var angle = Math.Atan2(crossProduct, dotProduct);
                
                return new { Neighbor = neighbor, Angle = angle, CrossProduct = crossProduct };
            }).OrderBy(x => x.Angle).ToList();
            
            // Split into left and right sides relative to the attack direction
            // Left side: negative cross product (clockwise from attack direction)
            // Right side: positive cross product (counterclockwise from attack direction)
            var leftSide = neighborsWithAngle.Where(x => x.CrossProduct < -0.01).OrderByDescending(x => x.Angle).ToList();
            var rightSide = neighborsWithAngle.Where(x => x.CrossProduct > 0.01).OrderBy(x => x.Angle).ToList();
            
            var result = new List<HexoPoint>();
            
            // Take splash units from each side around the attacker position
            // Splash = 1: 1 from left + 1 from right = 2 total (plus primary target = 3 total)
            // Splash = 2: 2 from left + 2 from right = 4 total (plus primary target = 5 total)
            if (leftSide.Count > 0)
            {
                result.AddRange(leftSide.Take(splash).Select(x => x.Neighbor));
            }
            if (rightSide.Count > 0)
            {
                result.AddRange(rightSide.Take(splash).Select(x => x.Neighbor));
            }
            
            return result;
        }

        /// <summary>
        /// Finds units in a straight line behind the target, continuing from attacker through target.
        /// Returns units up to pierceThrough distance behind the target.
        /// Uses hex line drawing algorithm in axial coordinates.
        /// </summary>
        public static List<TBattleUnit> GetUnitsInLineBehindTarget<TBattleUnit>(
            IBattleUnitObject attacker,
            IBattleUnitObject target,
            int pierceThrough,
            IReadOnlyCollection<TBattleUnit> allUnits,
            int mapHeight,
            int mapWidth) where TBattleUnit : IBattleUnitObject
        {
            return GetUnitsInLineBehindTarget(new HexoPoint(attacker.Column, attacker.Row), target, pierceThrough, allUnits, mapHeight, mapWidth);
        }

        /// <summary>
        /// Returns units up to pierceThrough distance behind the target.
        /// Uses hex line drawing algorithm in axial coordinates.
        /// Overload that accepts attacker position as HexoPoint.
        /// </summary>
        public static List<TBattleUnit> GetUnitsInLineBehindTarget<TBattleUnit>(
            HexoPoint attackerPosition,
            IBattleUnitObject target,
            int pierceThrough,
            IReadOnlyCollection<TBattleUnit> allUnits,
            int mapHeight,
            int mapWidth) where TBattleUnit : IBattleUnitObject
        {
            var result = new List<TBattleUnit>();
            
            if (pierceThrough <= 0)
                return result;

            // Convert to axial coordinates
            var attackerAxial = OddRHexoGrid.ToAxial(attackerPosition.R, attackerPosition.C);
            var targetAxial = OddRHexoGrid.ToAxial(target.Row, target.Column);
            
            // Calculate direction vector from attacker to target
            var dq = targetAxial.q - attackerAxial.q;
            var dr = targetAxial.r - attackerAxial.r;
            
            // If attacker and target are at the same position, no line
            if (dq == 0 && dr == 0)
                return result;
            
            // Use hex line drawing algorithm to find cells in a straight line
            // Continue from target in the same direction
            for (int step = 1; step <= pierceThrough; step++)
            {
                // Calculate next cell in the line using linear interpolation in axial space
                // For hex grids, we need to round to the nearest hex
                double nextQ = targetAxial.q + (double)dq * step;
                double nextR = targetAxial.r + (double)dr * step;
                
                // Round to nearest hex (cube coordinates rounding)
                var nextQInt = (int)Math.Round(nextQ);
                var nextRInt = (int)Math.Round(nextR);
                
                // Convert back to offset coordinates (odd-r)
                var nextCol = nextQInt + (int)Math.Floor((nextRInt - (nextRInt & 1)) / 2.0);
                var nextRow = nextRInt;
                
                // Check if cell is valid
                if (nextRow < 0 || nextRow >= mapHeight || nextCol < 0 || nextCol >= mapWidth)
                    break;
                
                // Find unit at this position
                var unitAtPosition = allUnits.FirstOrDefault(u => 
                    u.GlobalUnit.IsAlive && 
                    u.Row == nextRow && 
                    u.Column == nextCol);
                
                if (unitAtPosition != null)
                {
                    result.Add(unitAtPosition);
                }
            }
            
            return result;
        }
    }
}