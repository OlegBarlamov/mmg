using System;

namespace Epic.Core.Objects
{
    public class MutableUserUnitObject : IUserUnitObject
    {
        public Guid Id { get; set; }
        public IUnitTypeObject UnitType { get; set; }
        public Guid UnitTypeId { get; set; }
        public int Count { get; set; }
        public Guid UserId { get; set; }
        public bool IsAlive { get; set; }

        public static MutableUserUnitObject CopyFrom(IUserUnitObject x)
        {
            return new MutableUserUnitObject
            {
                Id = x.Id,
                UnitType = x.UnitType,
                UnitTypeId = x.UnitType.Id,
                Count = x.Count,
                UserId = x.UserId,
                IsAlive = x.IsAlive,
            };
        }
    }
}