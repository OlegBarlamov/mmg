using System;
using Epic.Core.Objects;
using Epic.Core.Services.Users;

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