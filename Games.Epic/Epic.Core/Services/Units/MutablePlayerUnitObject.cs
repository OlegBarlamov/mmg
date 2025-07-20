using System;
using Epic.Core.Services.UnitTypes;
using Epic.Data.PlayerUnits;

namespace Epic.Core.Services.Units
{
    public class MutablePlayerUnitObject : IPlayerUnitObject
    {
        public Guid Id { get; set; }
        public IUnitTypeObject UnitType { get; set; }
        public Guid UnitTypeId { get; set; }
        public int Count { get; set; }
        public Guid PlayerId { get; set; }
        public bool IsAlive { get; set; }

        private MutablePlayerUnitObject()
        {
            
        }
        
        public static MutablePlayerUnitObject CopyFrom(IPlayerUnitObject x)
        {
            return new MutablePlayerUnitObject
            {
                Id = x.Id,
                UnitType = x.UnitType,
                UnitTypeId = x.UnitType.Id,
                Count = x.Count,
                PlayerId = x.PlayerId,
                IsAlive = x.IsAlive,
            };
        }

        public static MutablePlayerUnitObject FromEntity(IPlayerUnitEntity entity)
        {
            return new MutablePlayerUnitObject
            {
                Id = entity.Id,
                Count = entity.Count,
                IsAlive = entity.IsAlive,
                PlayerId = entity.PlayerId,
                UnitTypeId = entity.TypeId,
            };
        }

        public IPlayerUnitEntity ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}