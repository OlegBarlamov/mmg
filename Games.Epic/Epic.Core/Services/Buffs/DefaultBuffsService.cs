using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.BuffTypes;
using Epic.Data.Buff;
using JetBrains.Annotations;

namespace Epic.Core.Services.Buffs
{
    [UsedImplicitly]
    public class DefaultBuffsService : IBuffsService
    {
        public IBuffsRepository BuffsRepository { get; }
        public IBuffTypesService BuffTypesService { get; }

        public DefaultBuffsService(
            [NotNull] IBuffsRepository buffsRepository,
            [NotNull] IBuffTypesService buffTypesService)
        {
            BuffsRepository = buffsRepository ?? throw new ArgumentNullException(nameof(buffsRepository));
            BuffTypesService = buffTypesService ?? throw new ArgumentNullException(nameof(buffTypesService));
        }

        public async Task<IBuffObject> GetById(Guid id)
        {
            var entity = await BuffsRepository.GetById(id);
            var obj = MutableBuffObject.FromEntity(entity);
            obj.BuffType = await BuffTypesService.GetById(obj.BuffTypeId);
            return obj;
        }

        public async Task<IBuffObject[]> GetByTargetBattleUnitId(Guid targetBattleUnitId)
        {
            var entities = await BuffsRepository.GetByTargetBattleUnitId(targetBattleUnitId);
            var objects = entities.Select(e => MutableBuffObject.FromEntity(e)).ToArray();

            var typeIds = objects.Select(x => x.BuffTypeId).Distinct().ToArray();
            var types = await Task.WhenAll(typeIds.Select(BuffTypesService.GetById));
            var typesById = typeIds.Zip(types, (id, type) => (id, type)).ToDictionary(x => x.id, x => x.type);

            foreach (var o in objects)
                o.BuffType = typesById[o.BuffTypeId];

            return objects.ToArray<IBuffObject>();
        }

        public async Task<IBuffObject> Create(Guid targetBattleUnitId, Guid buffTypeId, BuffExpressionsVariables variables)
        {
            if (targetBattleUnitId == Guid.Empty) throw new ArgumentException("TargetBattleUnitId must not be empty.", nameof(targetBattleUnitId));
            if (buffTypeId == Guid.Empty) throw new ArgumentException("BuffTypeId must not be empty.", nameof(buffTypeId));

            var type = await BuffTypesService.GetById(buffTypeId);
            var effectiveValues = BuffExpressionEvaluator.Evaluate(type, variables);

            var id = Guid.NewGuid();
            var fields = new BuffEntityFields
            {
                BuffTypeId = buffTypeId,
                TargetBattleUnitId = targetBattleUnitId,
                DurationRemaining = effectiveValues.Duration,
            };
            fields.SetEffectiveValues(effectiveValues);
            var entity = await BuffsRepository.Create(id, fields);

            var obj = MutableBuffObject.FromEntity(entity, effectiveValues);
            obj.BuffType = type;
            return obj;
        }

        public Task UpdateBatch(IBuffObject[] buffs)
        {
            if (buffs == null) throw new ArgumentNullException(nameof(buffs));
            return BuffsRepository.UpdateBatch(buffs.Select(x => x.ToEntity()).ToArray());
        }

        public Task DeleteById(Guid id)
        {
            return BuffsRepository.DeleteById(id);
        }

        public Task DeleteByTargetBattleUnitId(Guid targetBattleUnitId)
        {
            return BuffsRepository.DeleteByTargetBattleUnitId(targetBattleUnitId);
        }
    }
}

