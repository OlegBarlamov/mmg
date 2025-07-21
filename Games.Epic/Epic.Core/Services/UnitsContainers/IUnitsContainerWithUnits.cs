using System;
using System.Collections.Generic;
using Epic.Core.Services.Units;
using Epic.Data.UnitsContainers;
using JetBrains.Annotations;

namespace Epic.Core.Services.UnitsContainers
{
    public interface IUnitsContainerWithUnits : IUnitsContainerObject
    {
        IReadOnlyList<IPlayerUnitObject> Units { get; }
    }

    internal class UnitsContainerWithUnits : MutableUnitsContainerObject, IUnitsContainerWithUnits
    {
        public List<IPlayerUnitObject> Units { get; }

        IReadOnlyList<IPlayerUnitObject> IUnitsContainerWithUnits.Units => Units; 

        private IUnitsContainerObject Container { get; }

        public UnitsContainerWithUnits([NotNull] IUnitsContainerObject container, params IPlayerUnitObject[] units)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Id = container.Id;
            Capacity = container.Capacity;
            Units = new List<IPlayerUnitObject>(units);
        }
    }
}