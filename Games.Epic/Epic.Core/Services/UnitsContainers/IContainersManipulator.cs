using System;
using System.Threading.Tasks;
using Epic.Core.Services.Units;

namespace Epic.Core.Services.UnitsContainers
{
    public interface IContainersManipulator
    {
        Task<IUnitsContainerWithUnits> GetContainerWithUnits(Guid id);
        Task<IUnitsContainerWithUnits> PlaceUnitsToContainer(Guid containerId, params IPlayerUnitObject[] units);
        Task<IUnitsContainerWithUnits> MoveUnits(IPlayerUnitObject originalUnit, Guid targetContainerId, int? amountToSplit, int? specificSlotIndex = null);
        Task<IUnitsContainerWithUnits> FillEmptySlotsWithUnits(IPlayerUnitObject originalUnit, Guid targetContainerId);
        Task ExchangeContainers(Guid container1Id, Guid container2Id);
        Task<Pair<IPlayerUnitObject>> ExchangeUnitSlots(IPlayerUnitObject unit1, IPlayerUnitObject unit2);
        Task<Pair<IPlayerUnitObject>> ExchangeUnitsSameType(IPlayerUnitObject fromUnit, IPlayerUnitObject toUnit, int amountToGive);
    }
}
