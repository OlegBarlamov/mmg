using System.Collections.Generic;
using System.Linq;
using Epic.Core;
using Epic.Core.Services.Battles;
using Epic.Data.UnitTypes.Subtypes;

namespace Epic.Logic.Battle.Map
{
    public static class MapUtils
    {
        public static bool IsEnemyInRange(IBattleUnitObject actor, int range, IReadOnlyCollection<IBattleUnitObject> units)
        {
            return units.Any(u => u.PlayerIndex != actor.PlayerIndex &&
                                  u.GlobalUnit.IsAlive && OddRHexoGrid.Distance(actor, u) <= range);
        }
        
        public static List<HexoPoint> GetCellsForUnitMove(IBattleObject map, IBattleUnitObject unit, MovementType movementType)
        {
            var start = new HexoPoint(unit.Column, unit.Row);
            int moveRange = unit.GlobalUnit.UnitType.Speed;
            var availableCells = new List<HexoPoint>();

            // Track visited cells
            var visited = new HashSet<string>();

            // BFS queue: each entry is (cell, distance)
            var queue = new Queue<(HexoPoint Cell, int Distance)>();
            queue.Enqueue((start, 0));

            // Convert a cell to a unique string key
            string CellToString(HexoPoint cell) => $"{cell.R},{cell.C}";

            bool IsCellBlocked(HexoPoint cell)
            {
                return map.Units.Any(x => x.GlobalUnit.IsAlive && x.Column == cell.C && x.Row == cell.R);
            }

            // Start BFS
            while (queue.Count > 0)
            {
                var (currentCell, distance) = queue.Dequeue();
                var key = CellToString(currentCell);

                // Skip if already visited or out of range
                if (visited.Contains(key) || distance > moveRange)
                    continue;

                // Mark visited
                visited.Add(key);

                // Add to available cells:
                // - Ground: only if not blocked
                // - Fly: allow traversal but cannot stop on blocked cells
                if (!currentCell.Equals(start))
                {
                    if (movementType == MovementType.Fly)
                    {
                        if (!IsCellBlocked(currentCell)) 
                            availableCells.Add(currentCell);
                    }
                    else // Ground
                    {
                        if (!IsCellBlocked(currentCell))
                            availableCells.Add(currentCell);
                        else
                            continue; // stop BFS at blocked cell
                    }
                }

                // Explore neighbors
                var neighbors = GetNeighborCells(currentCell.R, currentCell.C, map.Height, map.Width);
                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(CellToString(neighbor)))
                    {
                        queue.Enqueue((neighbor, distance + 1));
                    }
                }
            }

            return availableCells;
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
                (-1, 0),   // top
                (-1, -1),  // top-left
                (0, -1),   // left
                (0, 1),    // right
                (1, 0),    // bottom
                (1, -1)    // bottom-left
            };

            var evenRDirections = new (int dr, int dc)[]
            {
                (-1, 0),   // top
                (-1, 1),   // top-right
                (0, -1),   // left
                (0, 1),    // right
                (1, 0),    // bottom
                (1, 1)     // bottom-right
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