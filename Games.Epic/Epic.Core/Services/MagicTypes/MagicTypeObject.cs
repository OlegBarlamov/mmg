using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Services.BuffTypes;
using Epic.Core.Services.EffectTypes;
using Epic.Data.MagicType;
using JetBrains.Annotations;

namespace Epic.Core.Services.MagicTypes
{
    public class MutableMagicTypeObject : IMagicTypeObject, IMagicTypeFields
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Value { get; set; }
        public int MannaCost { get; set; }
        public CastTargetType CastTargetType { get; set; }
        public int EffectRadius { get; set; }
        public Guid[] ApplyBuffsIds { get; set; }
        public Guid[] ApplyEffectsIds { get; set; }

        public IReadOnlyList<IBuffTypeObject> ApplyBuffsTypes { get; private set; }
        public IReadOnlyList<IEffectTypeObject> EffectTypes { get; private set; }

        private MutableMagicTypeObject() { }

        public static async Task<MutableMagicTypeObject> FromEntityAsync(
            IMagicTypeEntity entity,
            [NotNull] IBuffTypesService buffTypesService,
            [NotNull] IEffectTypesService effectTypesService)
        {
            var buffIds = entity.ApplyBuffsIds ?? Array.Empty<Guid>();
            var applyBuffsTypes = new List<IBuffTypeObject>(buffIds.Length);
            foreach (var id in buffIds)
                applyBuffsTypes.Add(await buffTypesService.GetById(id));

            var effectIds = entity.ApplyEffectsIds ?? Array.Empty<Guid>();
            var effectTypes = new List<IEffectTypeObject>(effectIds.Length);
            foreach (var id in effectIds)
                effectTypes.Add(await effectTypesService.GetById(id));

            return new MutableMagicTypeObject
            {
                Id = entity.Id,
                Key = entity.Key,
                Name = entity.Name,
                ThumbnailUrl = entity.ThumbnailUrl,
                Value = entity.Value,
                MannaCost = entity.MannaCost,
                CastTargetType = entity.CastTargetType,
                EffectRadius = entity.EffectRadius,
                ApplyBuffsIds = entity.ApplyBuffsIds,
                ApplyEffectsIds = entity.ApplyEffectsIds,
                ApplyBuffsTypes = applyBuffsTypes,
                EffectTypes = effectTypes,
            };
        }

        public IMagicTypeEntity ToEntity()
        {
            return MagicTypeEntity.FromFields(Id, this);
        }
    }
}
