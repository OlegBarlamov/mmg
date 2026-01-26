using System;
using Epic.Core.Objects;
using Epic.Data.BuffType;

namespace Epic.Core.Services.BuffTypes
{
    public interface IBuffTypeObject : IGameObject<IBuffTypeEntity>, IBuffFields
    {
        Guid Id { get; }
    }
}

