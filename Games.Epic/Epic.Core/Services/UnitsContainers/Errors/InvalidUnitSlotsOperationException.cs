using System;

namespace Epic.Core.Services.UnitsContainers.Errors
{
    public class InvalidUnitSlotsOperationException : Exception
    {
        public InvalidUnitSlotsOperationException(string message) : base(message)
        {
            
        }
    }
}