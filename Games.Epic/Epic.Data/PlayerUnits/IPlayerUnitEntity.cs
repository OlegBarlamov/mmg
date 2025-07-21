using System;

namespace Epic.Data.PlayerUnits
{
    public interface IPlayerUnitEntity
    {
        Guid Id { get; }
        Guid TypeId { get; set; }
        int Count { get; set; }
        Guid PlayerId { get; set; }
        Guid ContainerId { get; set; }
        bool IsAlive { get; set; }
        int ContainerSlotIndex { get; set; }
    }

    public class PlayerUnitEntity : IPlayerUnitEntity
    {
        public Guid Id { get; set; }
        public Guid TypeId { get; set; }
        public int Count { get; set; }
        public Guid PlayerId { get; set; }
        public Guid ContainerId { get; set; }
        public bool IsAlive { get; set; }
        public int ContainerSlotIndex { get; set; }

        public PlayerUnitEntity()
        {
            
        }
    }
}