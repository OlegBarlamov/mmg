using System;
using Epic.Data.UnitsContainers;

namespace Epic.Core.Services.Units
{
    public class CreatePlayerUnitData
    {
        public Guid PlayerId { get; set; }
        public Guid UnitTypeId { get; set; }
        public int Amount { get; set; }
        public Guid ContainerId { get; set; } = DefaultContainers.EmptyContainerId;

        public CreatePlayerUnitData(Guid playerId, Guid unitTypeId, int amount)
        {
            PlayerId = playerId;
            UnitTypeId = unitTypeId;
            Amount = amount;
        }
    }
}
