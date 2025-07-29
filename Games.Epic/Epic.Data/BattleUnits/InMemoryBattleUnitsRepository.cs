using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Data.BattleUnits
{
    [UsedImplicitly]
    public class InMemoryBattleUnitsRepository : IBattleUnitsRepository
    {
        public string Name => nameof(InMemoryBattleUnitsRepository);
        public string EntityName => "BattleUnit";

        private readonly List<AttackFunctionStateEntity> _attackFunctionStateEntities =
            new List<AttackFunctionStateEntity>();
        private readonly List<BattleUnitEntity> _battleUnits = new List<BattleUnitEntity>();

        public Task<IBattleUnitEntity[]> GetByBattleId(Guid battleId)
        {
            var battleUnits = _battleUnits.Where(bu => bu.BattleId == battleId).ToArray<IBattleUnitEntity>();
            return Task.FromResult(battleUnits);
        }

        public Task<IBattleUnitEntity[]> CreateBatch(IBattleUnitEntityFields[] data)
        {
            var entities = data.Select(x => new BattleUnitEntity(x)
            {
                Id = Guid.NewGuid(),
            }).ToArray();
            
            _battleUnits.AddRange(entities);
            return Task.FromResult(entities.ToArray<IBattleUnitEntity>());
        }

        public Task Update(IBattleUnitEntity[] entities)
        {
            entities.ForEach(entity =>
            {
                var targetUnit = _battleUnits.First(x => x.Id == entity.Id);
                targetUnit.UpdateFrom(entity);
            });
            return Task.CompletedTask;
        }

        public Task<AttackFunctionStateEntity[]> GetAttacksData(Guid battleUnitId)
        {
            return Task.FromResult(_attackFunctionStateEntities
                .Where(x => x.BattleUnitId == battleUnitId)
                .ToArray());
        }

        public Task UpdateAttacksData(IReadOnlyCollection<AttackFunctionStateEntity> attacksData)
        {
            attacksData.ForEach(x =>
            {
                _attackFunctionStateEntities.First(t => t.BattleUnitId == x.BattleUnitId && t.AttackIndex == x.AttackIndex)
                    .UpdateFrom(x);
            });
            return Task.CompletedTask;
        }

        public Task InsertAttacksData(IReadOnlyCollection<AttackFunctionStateEntity> attacksDataPerUnitId)
        {
            _attackFunctionStateEntities.AddRange(attacksDataPerUnitId);
            return Task.CompletedTask;
        }
    }
}