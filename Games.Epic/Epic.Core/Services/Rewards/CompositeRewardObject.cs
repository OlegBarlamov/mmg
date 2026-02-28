using System;
using System.Collections.Generic;
using Epic.Core.Services.ArtifactTypes;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.MagicTypes;
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
        public IReadOnlyList<IArtifactTypeObject> ArtifactTypes { get; set; } = Array.Empty<IArtifactTypeObject>();
        public IReadOnlyList<ResourceAmount[]> Prices { get; set; } = Array.Empty<ResourceAmount[]>();
        public IReadOnlyList<IMagicTypeObject> Magics { get; set; } = Array.Empty<IMagicTypeObject>();
        
        [CanBeNull] public IBattleDefinitionObject GuardBattleDefinition { get; set; } 
    }
}