using Epic.Core.Objects;

namespace Epic.Server.Resourses
{
    public class UserResource
    {
        public string UserName { get; }
        public string UserId { get; }
        
        public UserResource(IUserObject userObject)
        {
            UserName = userObject.Name;
            UserId = userObject.Id.ToString();
        }
    }
}