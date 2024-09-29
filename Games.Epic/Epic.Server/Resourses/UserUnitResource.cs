using Epic.Core.Objects;

namespace Epic.Server.Resourses
{
    public class UserUnitResource
    {
        public string Id { get; }
        public string TypeId { get; }
        public int Count { get; }
        
        public UserUnitResource(IUserUnitObject userUnit)
        {
            Id = userUnit.Id.ToString();
            TypeId = userUnit.TypeId.ToString();
            Count = userUnit.Count;
        }
    }
}