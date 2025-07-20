using System;

namespace Epic.Core.Services.Units
{
    public class CreatePlayerUnitData
    {
        public Guid PlayerId { get; set; }
        public Guid ContainerId { get; set; }
        public Guid UnitTypeId { get; set; }
        public int Amount { get; set; }
    }
}