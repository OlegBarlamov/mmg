using System;
using Epic.Core.Objects;
using Epic.Core.Services.BuffTypes;
using Epic.Data.Buff;

namespace Epic.Core.Services.Buffs
{
    public interface IBuffObject : IGameObject<IBuffEntity>, IBuffEntityFields
    {
        Guid Id { get; }

        IBuffTypeObject BuffType { get; }
    }
}

