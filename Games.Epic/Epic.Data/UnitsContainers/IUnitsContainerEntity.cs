using System;

namespace Epic.Data.UnitsContainers
{
    public interface IUnitsContainerEntity
    {
        Guid Id { get; }
        int Capacity { get; }
    }

    public class MutableUnitsContainerEntity : IUnitsContainerEntity
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        
        public MutableUnitsContainerEntity() {}
    }
}