using System;

namespace Epic.Core.Services.GameResources.Errors
{
    public class NotEnoughResourcesToPayException : Exception
    {
        public NotEnoughResourcesToPayException() : base("Not enough resources to pay")
        {
        }
    }
}