using Epic.Core.Objects;

namespace Epic.Server.Resources
{
    public class UserUnitInDashboardResource
    {
        public string Id { get; }
        public string ImageUrl { get; }
        public int Count { get; }
        
        public UserUnitInDashboardResource(IPlayerUnitObject playerUnit)
        {
            Id = playerUnit.Id.ToString();
            ImageUrl = playerUnit.UnitType.DashboardImgUrl;
            Count = playerUnit.Count;
        }
    }
}