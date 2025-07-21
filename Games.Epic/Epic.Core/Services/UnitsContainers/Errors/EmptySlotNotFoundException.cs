namespace Epic.Core.Services.UnitsContainers.Errors
{
    public class EmptySlotNotFoundException : InvalidUnitSlotsOperationException
    {
        public EmptySlotNotFoundException(IUnitsContainerObject container)
            : base($"Empty slot not found in container {container}.")
        {
        }
    }
}