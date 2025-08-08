using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core;
using Epic.Core.Logic;
using Epic.Core.Services.Battles;

namespace Epic.Logic
{
    public class BattleUnitsPlacer : IBattleUnitsPlacer
    {
        public void PlaceBattleUnitsDefaultPattern(MutableBattleObject battleObject)
        {
            var notFittingUnits = new List<MutableBattleUnitObject>();
            var filledRowsPerPlayerIndex = new Dictionary<int, bool[]>
            {
                { (int)InBattlePlayerNumber.Player1, Enumerable.Repeat(false, battleObject.Height).ToArray() },
                { (int)InBattlePlayerNumber.Player2, Enumerable.Repeat(false, battleObject.Height).ToArray() },
            };
            
            battleObject.Units.ForEach(u =>
            {
                if (u.PlayerIndex != (int)InBattlePlayerNumber.Player1 &&
                    u.PlayerIndex != (int)InBattlePlayerNumber.Player2)
                {
                    u.Column = -1;
                    u.Row = -1;
                }
                    
                if (u.Row >= battleObject.Height || u.Row < 0)
                    notFittingUnits.Add(u);
                else
                    filledRowsPerPlayerIndex[u.PlayerIndex][u.Row] = true;
                
                u.Column = u.PlayerIndex == (int)InBattlePlayerNumber.Player1 ? 0 : battleObject.Width - 1;
            });
            
            var notFittingUnitsInDefaultColumnPerPlayerIndex = new Dictionary<int, int>
            {
                { (int)InBattlePlayerNumber.Player1, 0 },
                { (int)InBattlePlayerNumber.Player2, 0 },
            };
            notFittingUnits.ForEach(u =>
            {
                var freeSlotIndex = Array.FindIndex(filledRowsPerPlayerIndex[u.PlayerIndex], filled => !filled);
                if (freeSlotIndex >= 0)
                {
                    u.Row = freeSlotIndex;
                    filledRowsPerPlayerIndex[u.PlayerIndex][u.Row] = true;
                }
                else
                {
                    var targetRow = notFittingUnitsInDefaultColumnPerPlayerIndex[u.PlayerIndex] % battleObject.Height;
                    var columnShift = notFittingUnitsInDefaultColumnPerPlayerIndex[u.PlayerIndex] / battleObject.Height;
                    notFittingUnitsInDefaultColumnPerPlayerIndex[u.PlayerIndex]++;
                    u.Row = targetRow;
                    u.Column = u.PlayerIndex == (int)InBattlePlayerNumber.Player1
                        ? 0 + columnShift
                        : battleObject.Width - 1 - columnShift;
                }
            });
        }
    }
}