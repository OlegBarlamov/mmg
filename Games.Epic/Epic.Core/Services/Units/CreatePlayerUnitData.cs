using System;

namespace Epic.Core.Objects.UserUnit
{
    public class CreatePlayerUnitData
    {
        public Guid PlayerId { get; set; }
        public Guid UnitTypeId { get; set; }
        public int Amount { get; set; }
    }
}