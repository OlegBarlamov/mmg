using System;
using Epic.Core.Services.Users;

namespace Epic.Server.Resources
{
    public class UserResource
    {
        public string UserName { get; }
        public string UserId { get; }
        public string PlayerId { get; }
        
        public UserResource(IUserObject userObject, Guid? playerId)
        {
            UserName = userObject.Name;
            UserId = userObject.Id.ToString();
            PlayerId = playerId?.ToString();
        }
    }
}