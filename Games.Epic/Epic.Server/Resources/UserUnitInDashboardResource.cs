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
        
        public UserUnitInDashboardResource(IGlobalUnitObject globalUnit)
        {
            Id = globalUnit.Id.ToString();
            ThumbnailUrl = globalUnit.UnitType.DashboardImgUrl;
            Count = globalUnit.Count;
            TypeId = globalUnit.UnitType.Id.ToString();
            SlotIndex = globalUnit.ContainerSlotIndex;
        }
    }
}