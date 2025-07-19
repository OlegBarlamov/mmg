using System;

namespace Epic.Data.PlayerUnits
{
    public interface IPlayerUnitEntity
    {
        Guid Id { get; }
        Guid TypeId { get; }
        int Count { get; }
        Guid PlayerId { get; }
        bool IsAlive { get; }
    }

    internal class PlayerUnitEntity : IPlayerUnitEntity
    {
        public Guid Id { get; set; }
        public Guid TypeId { get; set; }
        public int Count { get; set; }
        public Guid PlayerId { get; set; }
        public bool IsAlive { get; set; }
    }
}