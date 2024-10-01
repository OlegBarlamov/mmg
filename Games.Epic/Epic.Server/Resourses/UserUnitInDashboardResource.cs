using Epic.Core.Objects;

namespace Epic.Server.Resourses
{
    public class UserUnitInDashboardResource
    {
        public string Id { get; }
        public string ImageUrl { get; }
        public int Count { get; }
        
        public UserUnitInDashboardResource(IUserUnitObject userUnit)
        {
            Id = userUnit.Id.ToString();
            ImageUrl = userUnit.UnitType.DashboardImgUrl;
            Count = userUnit.Count;
        }
    }
}