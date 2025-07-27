using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Data.Reward
{
    public interface IRewardsRepository : IRepository
    {
        Task<IRewardEntity[]> GetRewardsByIdAsync(IReadOnlyList<Guid> ids);
        Task<IRewardEntity[]> GetRewardsByBattleDefinitionId(Guid battleDefinitionId);
        Task<IRewardEntity[]> FindNotAcceptedRewardsByPlayerId(Guid playerId);
        Task<IRewardEntity> RemoveRewardFromPlayer(Guid playerId, Guid rewardId);
        Task<IRewardEntity> CreateRewardAsync(Guid battleDefinitionId, IRewardEntityFields fields);
        Task GiveRewardsToPlayerAsync(IReadOnlyList<Guid> rewardIds, Guid playerId);
        Task SetRewardAsAccepted(Guid rewardId, Guid playerId, bool accepted);
    }
}