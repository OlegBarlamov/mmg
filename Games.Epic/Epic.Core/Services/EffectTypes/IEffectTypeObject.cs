using System;
using Epic.Core.Objects;
using Epic.Data.EffectType;

namespace Epic.Core.Services.EffectTypes
{
    public interface IEffectTypeObject : IGameObject<IEffectTypeEntity>, IEffectTypeFields
    {
        Guid Id { get; }
    }
}
