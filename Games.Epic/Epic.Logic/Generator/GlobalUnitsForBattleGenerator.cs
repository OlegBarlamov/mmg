using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.UnitsContainers;
using Epic.Core.Services.UnitTypes;
using Epic.Data.GlobalUnits;
using Epic.Data.PlayerUnits;
using Epic.Data.UnitTypes;
using JetBrains.Annotations;

namespace Epic.Logic.Generator
{
    public class GlobalUnitsForBattleGenerator
    {
        public IGlobalUnitsRepository GlobalUnitsRepository { get; }
        public IUnitTypesRegistry UnitTypesRegistry { get; }

        private enum SlotsDistributionPattern
        {
            Single,
            Few,
            Partially,
            Full,
        }

        public GlobalUnitsForBattleGenerator([NotNull] IGlobalUnitsRepository globalUnitsRepository,
            [NotNull] IUnitTypesRegistry unitTypesRegistry)
        {
            GlobalUnitsRepository = globalUnitsRepository ?? throw new ArgumentNullException(nameof(globalUnitsRepository));
            UnitTypesRegistry = unitTypesRegistry ?? throw new ArgumentNullException(nameof(unitTypesRegistry));
        }
        
        public async Task<IGlobalUnitEntity[]> Generate(IUnitsContainerObject container, Random seed, Guid unitTypeid, int amount, bool mayHaveUpgraded)
        {
            var height = container.Capacity;
            var slotsDistribution = (SlotsDistributionPattern)seed.Next(0, 4);

            var maxSlotsCount = int.MaxValue;
            var minSlotsCount = 1;
            switch (slotsDistribution)
            {
                case SlotsDistributionPattern.Single:
                    maxSlotsCount = 1;
                    maxSlotsCount = 1;
                    break;
                case SlotsDistributionPattern.Few:
                    minSlotsCount = 2;
                    maxSlotsCount = 3;
                    break;
                case SlotsDistributionPattern.Partially:
                    minSlotsCount = 4;
                    maxSlotsCount = 6;
                    break;
                case SlotsDistributionPattern.Full:
                    minSlotsCount = int.MaxValue;
                    maxSlotsCount = int.MaxValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var slotsCountLimit = Math.Min(amount, height);
            maxSlotsCount = Math.Min(maxSlotsCount, slotsCountLimit);
            minSlotsCount = Math.Min(minSlotsCount, slotsCountLimit);
            
            var targetSlotsCount = seed.Next(minSlotsCount, maxSlotsCount + 1);

            // 1. Create the unit distribution
            var slotDistributions = new List<int>();
            var baseUnitsPerSlot = amount / targetSlotsCount;
            var extraUnits = amount % targetSlotsCount;
            for (var i = 0; i < targetSlotsCount; i++)
            {
                // Distribute one of the extras to the first few slots
                var unitsInSlot = baseUnitsPerSlot + (i < extraUnits ? 1 : 0);
                slotDistributions.Add(unitsInSlot);
            }

            // 2. Spread filled slots evenly across the container height
            var slotIndices = UnitsSlotsDistribution.FindSlotIndices(targetSlotsCount, height);
            
            IUnitTypeEntity upgradedType = null;
            if (mayHaveUpgraded && targetSlotsCount > 2 && seed.Next(100) < 15)
            {
                var upgradeTypes = UnitTypesRegistry.GetUpgradesFor(unitTypeid);
                var orderedUpgrades = upgradeTypes.OrderBy(x => x.Value).ToList();
                upgradedType = orderedUpgrades.FirstOrDefault();
            }
            var upgradedSlot = upgradedType != null ? targetSlotsCount / 2 : -1;

            var resultUnits = new List<IGlobalUnitEntity>();
            for (int i = 0; i < slotIndices.Length && i < slotDistributions.Count; i++)
            {
                var typeId = i == upgradedSlot && upgradedType != null ? upgradedType.Id : unitTypeid;
                var newUnit = await GlobalUnitsRepository.Create(typeId, slotDistributions[i], container.Id, true,
                    slotIndices[i]);
                
                resultUnits.Add(newUnit);
            }

            return resultUnits.ToArray();
        } 
    }
}