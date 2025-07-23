using System;
using Epic.Core.Objects;
using Epic.Core.Services.UnitTypes;
using Epic.Data.PlayerUnits;

namespace Epic.Core.Services.Units
{
    public interface IGlobalUnitObject : IGameObject<IGlobalUnitEntity>
    {
        Guid Id { get; }
        IUnitTypeObject UnitType { get; }
        int Count { get; }
        bool IsAlive { get; }
        Guid ContainerId { get; }
        int ContainerSlotIndex { get; }
    }
}