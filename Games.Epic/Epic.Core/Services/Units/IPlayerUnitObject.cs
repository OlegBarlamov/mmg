using System;
using Epic.Core.Objects;
using Epic.Core.Services.UnitTypes;
using Epic.Data.PlayerUnits;

namespace Epic.Core.Services.Units
{
    public interface IPlayerUnitObject : IGameObject<IPlayerUnitEntity>
    {
        Guid Id { get; }
        IUnitTypeObject UnitType { get; }
        int Count { get; }
        Guid PlayerId { get; }
        bool IsAlive { get; }
        Guid ContainerId { get; }
    }
}