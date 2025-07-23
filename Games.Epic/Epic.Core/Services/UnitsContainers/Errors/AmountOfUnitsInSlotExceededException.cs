using Epic.Core.Services.Units;

namespace Epic.Core.Services.UnitsContainers.Errors
{
    public class AmountOfUnitsInSlotExceededException : InvalidUnitSlotsOperationException
    {
        public AmountOfUnitsInSlotExceededException(IGlobalUnitObject unit, int desiredAmount) : base(
            $"Unit {unit} amount is exceeded for {desiredAmount}")
        {
        }
    }
}