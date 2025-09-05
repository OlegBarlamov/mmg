using System;
using Epic.Core.Services.UnitTypes;
using Epic.Data.UnitsContainers;

namespace Epic.Core.Services.Units
{
    public class UpgradeUnitData
    {
        public IGlobalUnitObject Unit { get; set; }
        public IUnitTypeObject UpgradeToType { get; set; }
        public int Amount { get; set; }

        public Guid ContainerId { get; set; } = DefaultContainers.EmptyContainerId;

        public UpgradeUnitData(IGlobalUnitObject unit, IUnitTypeObject upgradeTo, int amount)
        {
            Unit = unit;
            UpgradeToType = upgradeTo;
            Amount = amount;
        }
    }
}
