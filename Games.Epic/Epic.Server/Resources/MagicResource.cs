using System;
using System.Linq;
using Epic.Core.Services.Magic;
using Epic.Data.EffectType;

namespace Epic.Server.Resources
{
    public class MagicResource
    {
        public Guid MagicTypeId { get; }
        public string Name { get; }
        public string Description { get; }
        public string ThumbnailUrl { get; }
        public int MannaCost { get; }
        public string CastTargetType { get; }
        public int EffectRadius { get; }
        public MagicBuffResource[] ApplyBuffs { get; }
        public MagicEffectResource[] ApplyEffects { get; }

        public MagicResource(IMagicObject magic)
        {
            if (magic == null) throw new ArgumentNullException(nameof(magic));
            MagicTypeId = magic.MagicTypeId;
            var type = magic.MagicType;
            Name = type?.Name;
            Description = type?.Description;
            ThumbnailUrl = type?.ThumbnailUrl;
            MannaCost = type?.MannaCost ?? 0;
            CastTargetType = type?.CastTargetType.ToString();
            EffectRadius = type?.EffectRadius ?? 0;
            ApplyBuffs = magic.ApplyBuffs?
                .Select(b => new MagicBuffResource(b.BuffType?.Name, b.BuffType?.Key, b.EffectiveValues?.Duration ?? 0, b.BuffType?.DurationExpression))
                .ToArray() ?? Array.Empty<MagicBuffResource>();
            ApplyEffects = magic.ApplyEffects?
                .Select(e => new MagicEffectResource(
                    e.EffectType?.Name, e.EffectType?.Key,
                    e.TakesDamageMin, e.TakesDamageMax, e.Heals, e.HealsPercentage,
                    e.EffectType?.TakesDamageMinExpression, e.EffectType?.TakesDamageMaxExpression,
                    e.EffectType?.HealsExpression, e.EffectType?.HealsPercentageExpression,
                    e.Animation, e.AnimationTimeMs))
                .ToArray() ?? Array.Empty<MagicEffectResource>();
        }
    }

    public class MagicBuffResource
    {
        public string Name { get; }
        public string Key { get; }
        public int Duration { get; }
        public string DurationExpression { get; }

        public MagicBuffResource(string name, string key, int duration, string durationExpression)
        {
            Name = name;
            Key = key;
            Duration = duration;
            DurationExpression = durationExpression;
        }
    }

    public class MagicEffectResource
    {
        public string Name { get; }
        public string Key { get; }
        public int TakesDamageMin { get; }
        public int TakesDamageMax { get; }
        public int Heals { get; }
        public int HealsPercentage { get; }
        public string TakesDamageMinExpression { get; }
        public string TakesDamageMaxExpression { get; }
        public string HealsExpression { get; }
        public string HealsPercentageExpression { get; }
        public EffectAnimation Animation { get; }
        public int AnimationTimeMs { get; }

        public MagicEffectResource(string name, string key, int takesDamageMin, int takesDamageMax, int heals, int healsPercentage,
            string takesDamageMinExpression, string takesDamageMaxExpression, string healsExpression, string healsPercentageExpression,
            EffectAnimation animation = EffectAnimation.None, int animationTimeMs = 0)
        {
            Name = name;
            Key = key;
            TakesDamageMin = takesDamageMin;
            TakesDamageMax = takesDamageMax;
            Heals = heals;
            HealsPercentage = healsPercentage;
            TakesDamageMinExpression = takesDamageMinExpression;
            TakesDamageMaxExpression = takesDamageMaxExpression;
            HealsExpression = healsExpression;
            HealsPercentageExpression = healsPercentageExpression;
            Animation = animation;
            AnimationTimeMs = animationTimeMs;
        }
    }
}
