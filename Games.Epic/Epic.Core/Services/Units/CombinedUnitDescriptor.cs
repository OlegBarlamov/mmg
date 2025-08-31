using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Services.UnitTypes;
using Epic.Data.PlayerUnits;
using NetExtensions.Collections;

namespace Epic.Core.Services.Units
{
    public class CombinedUnitDescriptor : MutableGlobalUnitObject, IGlobalUnitObject
    {
        protected CombinedUnitDescriptor(IGlobalUnitObject baseUnit)
            : base(baseUnit)
        {
        }

        public static CombinedUnitDescriptor[] Create(IReadOnlyCollection<IGlobalUnitObject> units, bool combineUpgradedTypes)
        {
            if (units.Count == 0)
                throw new ArgumentException(nameof(units));
            
            var descriptors = units.GroupBy(u => u.UnitType.Id)
                .Select(group => CreateFromSameTyped(group.ToArray()))
                .ToList();

            if (combineUpgradedTypes)
            {
                var descriptorsCopy = descriptors.ToArray();
                foreach (var descriptor in descriptorsCopy)
                {
                    var upgradeForUnit = descriptors.FirstOrDefault(x => descriptor.UnitType.IsUpgradeFor(x.UnitType));
                    if (upgradeForUnit != null)
                    {
                        descriptors.Remove(descriptor);
                        upgradeForUnit.AddCount(descriptor.Count);
                    }
                }
            }

            return descriptors.ToArray();
        } 

        public static CombinedUnitDescriptor CreateFromSameTyped(IReadOnlyCollection<IGlobalUnitObject> units)
        {
            if (units.Count == 0)
                throw new ArgumentException(nameof(units));

            var first = units.First();
            var result = new CombinedUnitDescriptor(first);
            units.Skip(1).ForEach(result.Add);
            
            return result;
        }

        public void Add(IGlobalUnitObject unit)
        {
            if (!this.IsSameType(unit))
                throw new ArgumentException($"{unit} can not be combined with {this}. Types do not match.");

            AddCount(unit.Count);
        }

        public void AddCount(int count)
        {
            Count += count;
        }

        public override IGlobalUnitEntity ToEntity()
        {
            throw new InvalidOperationException(
                $"{nameof(CombinedUnitDescriptor)} can not be represented as an entity");
        }
    }
}