using Epic.Data.BuffType;

namespace Epic.Server.Resources
{
    public class BuffTypeResource
    {
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool Permanent { get; set; }
        public int Duration { get; set; }
        
        public BuffTypeResource(IBuffTypeEntity buffType)
        {
            Name = buffType.Name;
            ThumbnailUrl = buffType.ThumbnailUrl;
            Permanent = buffType.Permanent;
            Duration = buffType.Duration;
        }
    }
}
