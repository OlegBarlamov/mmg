using Epic.Core.Objects;
using Epic.Core.Services.Units;

namespace Epic.Server.Resources
{
    public class UserUnitInDashboardResource
    {
        public string Id { get; }
        public string TypeId { get; }
        public string ThumbnailUrl { get; }
        public int Count { get; }
        public int SlotIndex { get; }
        
        public UserUnitInDashboardResource(IPlayerUnitObject playerUnit)
        {
            Id = playerUnit.Id.ToString();
            ThumbnailUrl = playerUnit.UnitType.DashboardImgUrl;
            Count = playerUnit.Count;
            TypeId = playerUnit.UnitType.Id.ToString();
            SlotIndex = playerUnit.ContainerSlotIndex;
        }
    }
}