using System;
using Epic.Core.Objects;
using Epic.Data.UnitsContainers;

namespace Epic.Core.Services.UnitsContainers
{
    public interface IUnitsContainerObject : IGameObject<IUnitsContainerEntity>
    {
        Guid Id { get; }
        int Capacity { get; }
        Guid OwnerPlayerId { get; }
    }
    
    public class MutableUnitsContainerObject : IUnitsContainerObject
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        public Guid OwnerPlayerId { get; set; }

        protected MutableUnitsContainerObject()
        {
            
        }
        
        public IUnitsContainerEntity ToEntity()
        {
            return new MutableUnitsContainerEntity
            {
                Id = Id,
                Capacity = Capacity,
                OwnerPlayerId = OwnerPlayerId,
            };
        }

        public static MutableUnitsContainerObject FromEntity(IUnitsContainerEntity entity)
        {
            return new MutableUnitsContainerObject
            {
                Id = entity.Id,
                Capacity = entity.Capacity,
                OwnerPlayerId = entity.OwnerPlayerId,
            };
        }
    }
}