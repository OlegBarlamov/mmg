using System;
using System.Collections.Generic;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.UnitTypes;
using Epic.Data.GameResources;
using Epic.Data.Reward;
using JetBrains.Annotations;

namespace Epic.Core.Services.Rewards
{
    public class CompositeRewardObject : BaseRewardObject, IRewardObject
    {
        public CompositeRewardObject(IRewardEntity entity) : base(entity)
        {
        }
        
        public IReadOnlyList<IUnitTypeObject> UnitTypes { get; set; } = Array.Empty<IUnitTypeObject>();
        public IReadOnlyList<IGameResourceEntity> Resources { get; set; } = Array.Empty<IGameResourceEntity>();
        
        [CanBeNull] 
        public IBattleDefinitionObject NextBattleDefinition { get; set; } 
    }
}