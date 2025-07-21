namespace Epic.Core.Services.UnitsContainers.Errors
{
    public class SlotIsOccupiedException : InvalidUnitSlotsOperationException
    {
        public SlotIsOccupiedException(IUnitsContainerObject container, int slotNumber) 
            : base($"Slot {slotNumber} has occupied in {container}")
        {
        }
    }
}