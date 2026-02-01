using Epic.Data.BuffType;

namespace Epic.Server.Resources
{
    public class BuffTypeResource
    {
        public string Name { get; set; }
        public bool Permanent { get; set; }
        public int Duration { get; set; }
        
        public BuffTypeResource(IBuffTypeEntity buffType)
        {
            Name = buffType.Name;
            Permanent = buffType.Permanent;
            Duration = buffType.Duration;
        }
    }
}
