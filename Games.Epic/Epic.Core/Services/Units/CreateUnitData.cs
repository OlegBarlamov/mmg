using System;
using Epic.Data.UnitsContainers;

namespace Epic.Core.Services.Units
{
    public class CreateUnitData
    {
        public Guid UnitTypeId { get; set; }
        public int Amount { get; set; }
        public Guid ContainerId { get; set; } = DefaultContainers.EmptyContainerId;

        public CreateUnitData(Guid unitTypeId, int amount)
        {
            UnitTypeId = unitTypeId;
            Amount = amount;
        }
    }
}
