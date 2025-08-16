using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Logic.Erros;
using Epic.Core.Services.Battles;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Logic.Battle
{
    internal class BattleUnitsCarousel : IDisposable
    {
        public MutableBattleObject BattleObject { get; }
        public IBattleUnitsService BattleUnitsService { get; }
        public IBattlesService BattlesService { get; }
        public int ActiveUnitIndex { get; private set; }
        public IBattleUnitObject ActiveUnit { get; private set; }
        
        private readonly List<IBattleUnitObject> _sortedBattleUnitObjects;
        
        public BattleUnitsCarousel(
            [NotNull] MutableBattleObject battleObject,
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IBattlesService battlesService
            )
        {
            BattleObject = battleObject ?? throw new ArgumentNullException(nameof(battleObject));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));

            ActiveUnitIndex = BattleObject.LastTurnUnitIndex;
            _sortedBattleUnitObjects = new List<IBattleUnitObject>(BattleObject.Units);
            
            SortByInitiative(_sortedBattleUnitObjects);
        }
        
        public void Dispose()
        {
            _sortedBattleUnitObjects.Clear();
        }

        public void ProcessNextTurn()
        {
            ActiveUnit = GetActiveUnit(ActiveUnitIndex, out var nextUnitIndex);
            ActiveUnitIndex = nextUnitIndex;
        }
        
        public bool IsNextRound()
        {
            GetActiveUnit(ActiveUnitIndex, out var nextActiveUnitIndex);
            return nextActiveUnitIndex <= ActiveUnitIndex;
        }

        public Task OnNextTurn()
        {
            BattleObject.TurnPlayerIndex = ActiveUnit.PlayerIndex;
            BattleObject.LastTurnUnitIndex = ActiveUnitIndex;
            BattleObject.TurnNumber++;
            return BattlesService.UpdateBattle(BattleObject);
        }

        public Task OnNextRound()
        {
            BattleObject.RoundNumber++;
            BattleObject.Units.ForEach(unit =>
            {
                unit.Waited = false;
                unit.AttackFunctionsData.ForEach(x => x.CounterattacksUsed = 0);
            });
            Sort();
            return BattleUnitsService.UpdateUnits(BattleObject.Units);
        }

        public void ActiveUnitWaits()
        {
            if (_sortedBattleUnitObjects.Remove(ActiveUnit))
                _sortedBattleUnitObjects.Add(ActiveUnit);
            
            ActiveUnitIndex--;
        }
        
        private IBattleUnitObject GetActiveUnit(int lastTurnUnitIndex, out int activeUnitIndex)
        {
            int count = _sortedBattleUnitObjects.Count;

            for (int i = 1; i <= count; i++)
            {
                var nextIndex = (lastTurnUnitIndex + i) % count;
                var unit = _sortedBattleUnitObjects[nextIndex];

                if (unit.GlobalUnit.IsAlive)
                {
                    activeUnitIndex = nextIndex;
                    return unit;
                }
            }

            throw new BattleLogicException("No alive unit found for active unit");
        }

        public void Sort()
        {
            SortByInitiative(_sortedBattleUnitObjects);
        }
        
        private void SortByInitiative(List<IBattleUnitObject> units)
        {
            units.Sort((x, y) =>
            {
                // 1. Compare Waited status: false comes before true
                int waitedCompare = x.Waited.CompareTo(y.Waited);
                if (waitedCompare != 0)
                    return waitedCompare;

                // 2. Compare Speed: higher speed comes first
                int speedCompare = y.GlobalUnit.UnitType.Speed.CompareTo(x.GlobalUnit.UnitType.Speed);
                if (speedCompare != 0)
                    return speedCompare;

                // 3. Compare Row: lower slot number comes first
                var slotCompare = x.GlobalUnit.ContainerSlotIndex.CompareTo(y.GlobalUnit.ContainerSlotIndex);
                if (slotCompare != 0)
                    return slotCompare;
                
                // 3. Compare Side: lower player index comes first
                return x.PlayerIndex.CompareTo(y.PlayerIndex);
            });
        }
    }
}