using System;
using System.Collections.Generic;
using Epic.Core.Objects;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.UnitTypes;
using Epic.Data.GameResources;
using Epic.Data.Reward;
using JetBrains.Annotations;

namespace Epic.Core.Services.Rewards
{
    public interface IRewardObject : IGameObject<IRewardEntity>
    {
        Guid Id { get; }
        Guid BattleDefinitionId { get; }
        RewardType RewardType { get; }
        Guid[] Ids { get; }
        int[] Amounts { get; }
        string Message { get; }
        bool CanDecline { get; }
        [CanBeNull] string IconUrl { get; }
        [CanBeNull] string Title { get; }
        [CanBeNull] string GuardMessage { get; }
        
        IReadOnlyList<IUnitTypeObject> UnitTypes { get; }
        IReadOnlyList<IGameResourceEntity> Resources { get; }
        
        [CanBeNull] IBattleDefinitionObject GuardBattleDefinition { get; }
    }
}
