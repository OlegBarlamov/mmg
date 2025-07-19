using System;

namespace Epic.Core.Objects
{
    public interface IPlayerUnitObject
    {
        Guid Id { get; }
        IUnitTypeObject UnitType { get; }
        int Count { get; }
        Guid PlayerId { get; }
        bool IsAlive { get; }
    }
}