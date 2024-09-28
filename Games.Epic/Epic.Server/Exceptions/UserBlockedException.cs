using System;
using Epic.Core.Objects;

namespace Epic.Server.Exceptions
{
    public class UserBlockedException : Exception
    {
        public UserBlockedException(IUserObject user) 
            : base($"User {user.Id} is blocked")
        {
            
        }
    }
}