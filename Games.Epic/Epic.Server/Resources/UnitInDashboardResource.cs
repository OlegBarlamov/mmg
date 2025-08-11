using Epic.Core.Objects;
using Epic.Core.Services.Units;

namespace Epic.Server.Resources
{
    public class UnitInDashboardResource
    {
        public string Id { get; }
        public string TypeId { get; }
        public string Name { get; }
        public string ThumbnailUrl { get; }
        public int Count { get; }
        public int SlotIndex { get; }
        
        public UnitInDashboardResource(IGlobalUnitObject globalUnit)
        {
            Id = globalUnit.Id.ToString();
            Name = globalUnit.UnitType.Name;
            ThumbnailUrl = globalUnit.UnitType.DashboardImgUrl;
            Count = globalUnit.Count;
            TypeId = globalUnit.UnitType.Id.ToString();
            SlotIndex = globalUnit.ContainerSlotIndex;
        }
    }
}