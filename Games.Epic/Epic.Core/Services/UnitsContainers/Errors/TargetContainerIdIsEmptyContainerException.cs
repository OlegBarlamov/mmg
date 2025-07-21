namespace Epic.Core.Services.UnitsContainers.Errors
{
    public class TargetContainerIdIsEmptyContainerException : InvalidUnitSlotsOperationException
    {
        public TargetContainerIdIsEmptyContainerException()
            : base("You can not target the Default null UnitsContainer for this operation.")
        {
        }
    }
}