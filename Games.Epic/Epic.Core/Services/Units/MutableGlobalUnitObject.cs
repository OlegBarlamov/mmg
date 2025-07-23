using System;
using Epic.Core.Services.UnitTypes;
using Epic.Data.PlayerUnits;

namespace Epic.Core.Services.Units
{
    public class MutableGlobalUnitObject : IGlobalUnitObject
    {
        public Guid Id { get; set; }
        public IUnitTypeObject UnitType { get; set; }
        public Guid UnitTypeId { get; set; }
        public int Count { get; set; }
        public bool IsAlive { get; set; }
        public Guid ContainerId { get; set; }
        public int ContainerSlotIndex { get; set; }


        private MutableGlobalUnitObject()
        {
            
        }
        
        public static MutableGlobalUnitObject CopyFrom(IGlobalUnitObject x)
        {
            return new MutableGlobalUnitObject
            {
                Id = x.Id,
                UnitType = x.UnitType,
                UnitTypeId = x.UnitType.Id,
                Count = x.Count,
                IsAlive = x.IsAlive,
                ContainerId = x.ContainerId,
                ContainerSlotIndex = x.ContainerSlotIndex,
            };
        }

        public static MutableGlobalUnitObject FromEntity(IGlobalUnitEntity entity)
        {
            return new MutableGlobalUnitObject
            {
                Id = entity.Id,
                Count = entity.Count,
                IsAlive = entity.IsAlive,
                UnitTypeId = entity.TypeId,
                ContainerId = entity.ContainerId,
                ContainerSlotIndex = entity.ContainerSlotIndex,
            };
        }

        public IGlobalUnitEntity ToEntity()
        {
            return new GlobalUnitEntity
            {
                Id = Id,
                TypeId = UnitType.Id,
                Count = Count,
                IsAlive = IsAlive,
                ContainerId = ContainerId,
                ContainerSlotIndex = ContainerSlotIndex,
            };
        }
    }
}