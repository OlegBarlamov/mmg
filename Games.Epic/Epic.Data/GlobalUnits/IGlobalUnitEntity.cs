using System;

namespace Epic.Data.PlayerUnits
{
    public interface IGlobalUnitEntity
    {
        Guid Id { get; }
        Guid TypeId { get; set; }
        int Count { get; set; }
        Guid ContainerId { get; set; }
        bool IsAlive { get; set; }
        int ContainerSlotIndex { get; set; }
    }

    public class GlobalUnitEntity : IGlobalUnitEntity
    {
        public Guid Id { get; set; }
        public Guid TypeId { get; set; }
        public int Count { get; set; }
        public Guid ContainerId { get; set; }
        public bool IsAlive { get; set; }
        public int ContainerSlotIndex { get; set; }

        public GlobalUnitEntity()
        {
            
        }

        public void UpdateFrom(IGlobalUnitEntity globalUnit)
        {
            Id = globalUnit.Id;
            TypeId = globalUnit.TypeId;
            Count = globalUnit.Count;
            ContainerId = globalUnit.ContainerId;
            IsAlive = globalUnit.IsAlive;
            ContainerSlotIndex = globalUnit.ContainerSlotIndex;
        }
    }
}