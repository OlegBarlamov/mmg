using Epic.Core.Services.Buffs;

namespace Epic.Server.Resources
{
    public class BattleUnitBuffResource
    {
        public string Name { get; }
        public string ThumbnailUrl { get; }
        public bool Permanent { get; }
        public int DurationRemaining { get; }

        public BattleUnitBuffResource(IBuffObject buffObject)
        {
            // BuffType should be populated by service layer; keep null-safe just in case.
            Name = buffObject.BuffType?.Name;
            ThumbnailUrl = buffObject.BuffType?.ThumbnailUrl;
            Permanent = buffObject.BuffType?.Permanent ?? false;
            DurationRemaining = buffObject.DurationRemaining;
        }
    }
}

