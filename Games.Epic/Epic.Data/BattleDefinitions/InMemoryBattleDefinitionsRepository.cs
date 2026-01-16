using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.Exceptions;
using Epic.Data.Reward;
using JetBrains.Annotations;

namespace Epic.Data.BattleDefinitions
{
    [UsedImplicitly]
    public class InMemoryBattleDefinitionsRepository : IBattleDefinitionsRepository
    {
        public string Name => nameof(InMemoryBattleDefinitionsRepository);
        public string EntityName => "BattleDefinition";
        
        private readonly List<BattleDefinitionEntity> _battleDefinitions = new List<BattleDefinitionEntity>();
        private readonly List<IPlayerToBattleDefinitionEntity> _playerBattleDefinitions = new List<IPlayerToBattleDefinitionEntity>();
        private readonly IRewardsRepository _rewardsRepository;

        public InMemoryBattleDefinitionsRepository(IRewardsRepository rewardsRepository)
        {
            _rewardsRepository = rewardsRepository ?? throw new ArgumentNullException(nameof(rewardsRepository));
        }

        public Task<IBattleDefinitionEntity[]> GetActiveBattleDefinitionsByPlayer(Guid playerId, int day)
        {
            // Find all user-battle relations for the given user
            var userBattles = _playerBattleDefinitions
                .Where(ub => ub.PlayerId == playerId)
                .Select(ub => ub.BattleDefinitionId)
                .ToList();

            // Find corresponding battle definitions
            var battles = _battleDefinitions
                .Where(bd => !bd.Finished && bd.ExpireAtDay > day && userBattles.Contains(bd.Id))
                .ToArray<IBattleDefinitionEntity>();

            return Task.FromResult(battles);
        }

        public async Task<IBattleDefinitionEntity> Create(Guid playerId, IBattleDefinitionFields fields)
        {
            var battleDefinition = await Create(fields);

            var userBattleDefinition = new PlayerToBattleDefinitionEntity
            {
                Id = Guid.NewGuid(),
                BattleDefinitionId = battleDefinition.Id,
                PlayerId = playerId,
            };
            _playerBattleDefinitions.Add(userBattleDefinition);

            return battleDefinition;
        }

        public Task<IBattleDefinitionEntity> Create(IBattleDefinitionFields fields)
        {
            var battleDefinition = BattleDefinitionEntity.FromFields(fields);
            _battleDefinitions.Add(battleDefinition);
            
            return Task.FromResult((IBattleDefinitionEntity)battleDefinition);
        }

        public Task<IBattleDefinitionEntity> GetByPlayerAndId(Guid playerId, Guid battleDefinitionId)
        {
            var userBattleDefinition = _playerBattleDefinitions.FirstOrDefault(x =>
                x.PlayerId == playerId && x.BattleDefinitionId == battleDefinitionId);
            if (userBattleDefinition == null)
                throw new EntityNotFoundException(this, $"PlayerId: {playerId}; BattleDefinitionId: {battleDefinitionId}");

            return Task.FromResult<IBattleDefinitionEntity>(_battleDefinitions.First(x => x.Id == battleDefinitionId));
        }

        public Task<IBattleDefinitionEntity> GetById(Guid battleDefinitionId)
        {
            return Task.FromResult<IBattleDefinitionEntity>(_battleDefinitions.First(x => x.Id == battleDefinitionId));
        }

        public Task SetFinished(Guid battleDefinitionId)
        {
            _battleDefinitions.First(x => x.Id == battleDefinitionId).Finished = true;
            return Task.CompletedTask;
        }

        public Task<int> CountBattles(Guid playerId, int? day = null)
        {
            var targetDay = day ?? 0;
            return Task.FromResult(_playerBattleDefinitions
                .Count(x => x.PlayerId == playerId &&
                            !IsFinishedOrExpired(_battleDefinitions.First(b => b.Id == x.BattleDefinitionId),
                                targetDay)));
        }

        public async Task<IBattleDefinitionEntity[]> GetActiveBattlesDefinitionsWithRewardType(Guid playerId, int currentDay, RewardType rewardType)
        {
            // Get all active battle definitions for the player
            var activeBattles = await GetActiveBattleDefinitionsByPlayer(playerId, currentDay);
            
            if (activeBattles.Length == 0)
                return Array.Empty<IBattleDefinitionEntity>();
            
            // Get all rewards for all active battles in one bulk request
            var battleDefinitionIds = activeBattles.Select(b => b.Id).ToList();
            var allRewards = await _rewardsRepository.GetRewardsByBattleDefinitionIds(battleDefinitionIds);
            
            // Group rewards by battle definition ID and filter battles with the specified reward type
            var rewardsByBattleId = allRewards
                .Where(r => r.RewardType == rewardType)
                .GroupBy(r => r.BattleDefinitionId)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            var battlesWithRewardType = activeBattles
                .Where(battle => rewardsByBattleId.ContainsKey(battle.Id))
                .ToArray();
            
            return battlesWithRewardType;
        }

        private static bool IsFinishedOrExpired(IBattleDefinitionEntity battleDefinition, int day)
        {
            return battleDefinition.Finished || battleDefinition.ExpireAtDay <= day;
        }
    }
}