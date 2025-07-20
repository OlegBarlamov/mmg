using System;
using Epic.Core.Objects;
using Epic.Core.Objects.Rewards;
using Epic.Core.Services.UnitTypes;
using Epic.Data.Reward;

namespace Epic.Core.Services.Rewards
{
    public interface IRewardObject : IGameObject<IRewardEntity>
    {
        public Guid Id { get; }
        public Guid BattleDefinitionId { get; }
        public RewardType RewardType { get; }
        public Guid[] TypeIds { get; }
        public int[] Amounts { get; }
        public string Message { get; }
        
        RewardDescription GetDescription();
        
        IUnitTypeObject[] GetUnitTypes();
    }
}
