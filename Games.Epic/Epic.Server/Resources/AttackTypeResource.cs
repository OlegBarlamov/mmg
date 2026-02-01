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
            
            ApplyBuffs = attackType.ApplyBuffTypeIds?
                .Select(id => buffTypesRegistry.ById(id))
                .Where(bt => bt != null)
                .Select(bt => new BuffTypeResource(bt))
                .ToList() ?? new List<BuffTypeResource>();
        }
    }
}
