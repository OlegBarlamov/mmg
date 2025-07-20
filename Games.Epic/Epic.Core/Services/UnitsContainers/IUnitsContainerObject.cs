using System;
using Epic.Core.Objects;
using Epic.Data.UnitsContainers;

namespace Epic.Core.Services.UnitsContainers
{
    public interface IUnitsContainerObject : IGameObject<IUnitsContainerEntity>
    {
        Guid Id { get; }
        int Capacity { get; }
    }
    
    public class MutableUnitsContainerObject : IUnitsContainerObject
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        
        public IUnitsContainerEntity ToEntity()
        {
            return new MutableUnitsContainerEntity
            {
                Id = Id,
                Capacity = Capacity
            };
        }

        public static MutableUnitsContainerObject FromEntity(IUnitsContainerEntity entity)
        {
            return new MutableUnitsContainerObject
            {
                Id = entity.Id,
                Capacity = entity.Capacity
            };
        }
    }
}