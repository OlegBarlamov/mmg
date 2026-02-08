using System.Collections.Generic;
using Epic.Core.Services.Buffs;
using Epic.Data.BuffType;

namespace Epic.Server.Resources
{
    public class BuffTypeResource
    {
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool Permanent { get; set; }
        public int Duration { get; set; }
        public int Chance { get; set; }
        
        public BuffTypeResource(IBuffTypeEntity buffType, BuffExpressionsVariables variables, int chance = 100)
        {
            Name = buffType.Name;
            ThumbnailUrl = buffType.ThumbnailUrl;
            Permanent = buffType.Permanent;
            var defaultValues = BuffExpressionEvaluator.Evaluate(buffType, variables);
            Duration = defaultValues.Duration;
            Chance = chance;
        }
    }
}
