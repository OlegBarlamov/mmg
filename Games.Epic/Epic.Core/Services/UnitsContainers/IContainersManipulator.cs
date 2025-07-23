using System;
using System.Threading.Tasks;
using Epic.Core.Services.Units;

namespace Epic.Core.Services.UnitsContainers
{
    public interface IContainersManipulator
    {
        Task<IUnitsContainerWithUnits> GetContainerWithUnits(Guid id);
        Task<IUnitsContainerWithUnits> PlaceUnitsToContainer(Guid containerId, params IGlobalUnitObject[] units);
        Task<IUnitsContainerWithUnits> MoveUnits(IGlobalUnitObject originalUnit, Guid targetContainerId, int? amountToSplit, int? specificSlotIndex = null);
        Task<IUnitsContainerWithUnits> FillEmptySlotsWithUnits(IGlobalUnitObject originalUnit, Guid targetContainerId);
        Task ExchangeContainers(Guid container1Id, Guid container2Id);
        Task<Pair<IGlobalUnitObject>> ExchangeUnitSlots(IGlobalUnitObject unit1, IGlobalUnitObject unit2);
        Task<Pair<IGlobalUnitObject>> ExchangeUnitsSameType(IGlobalUnitObject fromUnit, IGlobalUnitObject toUnit, int amountToGive);
    }
}
