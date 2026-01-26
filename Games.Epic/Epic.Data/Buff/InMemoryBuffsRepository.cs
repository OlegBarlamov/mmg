using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.Exceptions;
using JetBrains.Annotations;

namespace Epic.Data.Buff
{
    [UsedImplicitly]
    public class InMemoryBuffsRepository : IBuffsRepository
    {
        public string Name => nameof(InMemoryBuffsRepository);
        public string EntityName => "Buff";

        private readonly List<BuffEntity> _buffs = new List<BuffEntity>();

        private static void EnsureValidBuffTypeId(Guid buffTypeId)
        {
            if (buffTypeId == Guid.Empty)
                throw new ArgumentException("BuffTypeId must not be empty.", nameof(buffTypeId));
        }

        private static void EnsureValidTargetBattleUnitId(Guid targetBattleUnitId)
        {
            if (targetBattleUnitId == Guid.Empty)
                throw new ArgumentException("TargetBattleUnitId must not be empty.", nameof(targetBattleUnitId));
        }

        public Task<IBuffEntity> GetById(Guid id)
        {
            var entity = _buffs.FirstOrDefault(x => x.Id == id);
            if (entity == null)
                throw new EntityNotFoundException(this, id.ToString());
            return Task.FromResult<IBuffEntity>(entity);
        }

        public Task<IBuffEntity[]> GetByTargetBattleUnitId(Guid targetBattleUnitId)
        {
            EnsureValidTargetBattleUnitId(targetBattleUnitId);
            var entities = _buffs
                .Where(x => x.TargetBattleUnitId == targetBattleUnitId)
                .ToArray<IBuffEntity>();
            return Task.FromResult(entities);
        }

        public Task<IBuffEntity[]> GetAll()
        {
            return Task.FromResult(_buffs.ToArray<IBuffEntity>());
        }

        public Task<IBuffEntity> Create(Guid id, IBuffEntityFields fields)
        {
            EnsureValidBuffTypeId(fields.BuffTypeId);
            EnsureValidTargetBattleUnitId(fields.TargetBattleUnitId);
            var entity = BuffEntity.FromFields(id, fields);
            _buffs.Add(entity);
            return Task.FromResult<IBuffEntity>(entity);
        }

        public Task<IBuffEntity[]> CreateBatch(IBuffEntityFields[] fields)
        {
            foreach (var f in fields)
            {
                EnsureValidBuffTypeId(f.BuffTypeId);
                EnsureValidTargetBattleUnitId(f.TargetBattleUnitId);
            }
            var entities = fields.Select(x => BuffEntity.FromFields(Guid.NewGuid(), x)).ToArray();
            _buffs.AddRange(entities);
            return Task.FromResult(entities.ToArray<IBuffEntity>());
        }

        public Task UpdateBatch(IBuffEntity[] entities)
        {
            foreach (var entity in entities)
            {
                EnsureValidBuffTypeId(entity.BuffTypeId);
                EnsureValidTargetBattleUnitId(entity.TargetBattleUnitId);
                var existing = _buffs.FirstOrDefault(x => x.Id == entity.Id);
                if (existing == null)
                    throw new EntityNotFoundException(this, entity.Id.ToString());
                if (existing.BuffTypeId != entity.BuffTypeId)
                    throw new InvalidOperationException(
                        $"Cannot change BuffTypeId for buff {entity.Id}: {existing.BuffTypeId} -> {entity.BuffTypeId}");
                if (existing.TargetBattleUnitId != entity.TargetBattleUnitId)
                    throw new InvalidOperationException(
                        $"Cannot change TargetBattleUnitId for buff {entity.Id}: {existing.TargetBattleUnitId} -> {entity.TargetBattleUnitId}");
                existing.FillFrom(entity);
            }
            return Task.CompletedTask;
        }

        public Task DeleteById(Guid id)
        {
            _buffs.RemoveAll(x => x.Id == id);
            return Task.CompletedTask;
        }

        public Task DeleteByTargetBattleUnitId(Guid targetBattleUnitId)
        {
            EnsureValidTargetBattleUnitId(targetBattleUnitId);
            _buffs.RemoveAll(x => x.TargetBattleUnitId == targetBattleUnitId);
            return Task.CompletedTask;
        }
    }
}

