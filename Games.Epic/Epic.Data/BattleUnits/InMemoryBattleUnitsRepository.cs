using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Epic.Data.BattleUnits
{
    [UsedImplicitly]
    public class InMemoryBattleUnitsRepository : IBattleUnitsRepository
    {
        public string Name => nameof(InMemoryBattleUnitsRepository);
        public string EntityName => "BattleUnit";
        
        private readonly List<IBattleUnitEntity> _battleUnits = new List<IBattleUnitEntity>();

        public Task<IBattleUnitEntity[]> GetByBattleId(Guid battleId)
        {
            var battleUnits = _battleUnits.Where(bu => bu.BattleId == battleId).ToArray();
            return Task.FromResult(battleUnits);
        }
    }
}