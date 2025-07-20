using System;
using Epic.Core.Objects;
using Epic.Core.Services.Users;

namespace Epic.Server.Exceptions
{
    public class InvalidUserTypeException : Exception
    {
        public InvalidUserTypeException(IUserObject user) 
            : base($"User {user.Id} is invalid type")
        {
            
        }
    }
}