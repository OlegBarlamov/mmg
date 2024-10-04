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

        public Task<IBattleUnitEntity[]> CreateBatch(IBattleUnitEntityFields[] data)
        {
            var entities = data.Select(x => new BattleUnitEntity
            {
                Id = Guid.NewGuid(),
                BattleId = x.BattleId,
                Column = x.Column,
                Row = x.Row,
                PlayerIndex = x.PlayerIndex,
                UserUnitId = x.UserUnitId,
            }).ToArray<IBattleUnitEntity>();
            
            _battleUnits.AddRange(entities);
            return Task.FromResult(entities.ToArray());
        }
    }
}