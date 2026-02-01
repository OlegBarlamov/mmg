using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Epic.Core.Services.BuffTypes;
using Epic.Data.UnitTypes.Subtypes;

namespace Epic.Server.Resources
{
    public class AttackTypeResource : AttackFunctionType
    {
        [JsonIgnore]
        public new List<Guid> ApplyBuffTypeIds { get; set; }
        
        [JsonIgnore]
        public new List<string> ApplyBuffTypes { get; set; }
        
        public IReadOnlyList<BuffTypeResource> ApplyBuffs { get; set; }
        
        public AttackTypeResource(IAttackFunctionType attackType, IBuffTypesRegistry buffTypesRegistry)
        {
            CopyFrom(attackType);
            
            var buffResources = new List<BuffTypeResource>();
            if (attackType.ApplyBuffTypeIds != null)
            {
                for (int i = 0; i < attackType.ApplyBuffTypeIds.Count; i++)
                {
                    var buffType = buffTypesRegistry.ById(attackType.ApplyBuffTypeIds[i]);
                    if (buffType == null)
                        continue;
                    
                    // Get the chance for this buff (default to 100% if not specified)
                    var chance = (attackType.ApplyBuffChances != null && i < attackType.ApplyBuffChances.Count)
                        ? attackType.ApplyBuffChances[i]
                        : 100;
                    
                    buffResources.Add(new BuffTypeResource(buffType, chance));
                }
            }
            ApplyBuffs = buffResources;
        }
    }
}
