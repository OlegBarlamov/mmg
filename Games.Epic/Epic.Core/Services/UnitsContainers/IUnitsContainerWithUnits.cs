using System;
using System.Collections.Generic;
using Epic.Core.Services.Units;
using Epic.Data.UnitsContainers;
using JetBrains.Annotations;

namespace Epic.Core.Services.UnitsContainers
{
    public interface IUnitsContainerWithUnits : IUnitsContainerObject
    {
        IReadOnlyList<IGlobalUnitObject> Units { get; }
    }

    internal class UnitsContainerWithUnits : MutableUnitsContainerObject, IUnitsContainerWithUnits
    {
        public List<IGlobalUnitObject> Units { get; }

        IReadOnlyList<IGlobalUnitObject> IUnitsContainerWithUnits.Units => Units; 

        public UnitsContainerWithUnits([NotNull] IUnitsContainerObject container, params IGlobalUnitObject[] units)
            : base(container)
        {
            Units = new List<IGlobalUnitObject>(units);
        }
    }
}