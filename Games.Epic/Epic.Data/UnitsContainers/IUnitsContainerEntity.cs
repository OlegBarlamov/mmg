using System;

namespace Epic.Data.UnitsContainers
{
    public interface IUnitsContainerEntity
    {
        Guid Id { get; set; }
        int Capacity { get; set; }
        Guid OwnerPlayerId { get; set; }
    }

    public class MutableUnitsContainerEntity : IUnitsContainerEntity
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        public Guid OwnerPlayerId { get; set; }

        public MutableUnitsContainerEntity() {}

        public void CopyFrom(IUnitsContainerEntity entity)
        {
            Capacity = entity.Capacity;
            OwnerPlayerId = entity.OwnerPlayerId;
        }
    }
}