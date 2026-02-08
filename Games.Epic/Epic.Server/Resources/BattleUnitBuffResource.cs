using Epic.Core.Services.Buffs;

namespace Epic.Server.Resources
{
    public class BattleUnitBuffResource
    {
        public string Id { get; }
        public string BuffTypeId { get; }
        public string Name { get; }
        public string ThumbnailUrl { get; }
        public bool Permanent { get; }
        public int DurationRemaining { get; }

        public BattleUnitBuffResource(IBuffObject buffObject)
        {
            Id = buffObject.Id.ToString();
            BuffTypeId = buffObject.BuffTypeId.ToString();
            Name = buffObject.BuffType?.Name;
            ThumbnailUrl = buffObject.BuffType?.ThumbnailUrl;
            Permanent = buffObject.Permanent;
            DurationRemaining = buffObject.DurationRemaining;
        }
    }
}

