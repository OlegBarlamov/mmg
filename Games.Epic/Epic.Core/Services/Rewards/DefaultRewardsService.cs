using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Objects.Rewards;
using Epic.Core.Objects.UserUnit;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitTypes;
using Epic.Data.Reward;
using JetBrains.Annotations;

namespace Epic.Core.Services.Rewards
{
    public class DefaultRewardsService : IRewardsService
    {
        public IRewardsRepository RewardsRepository { get; }
        public IUnitTypesService UnitTypesService { get; }
        public IUserUnitsService UserUnitsService { get; }

        public DefaultRewardsService(
            [NotNull] IRewardsRepository rewardsRepository,
            [NotNull] IUnitTypesService unitTypesService,
            [NotNull] IUserUnitsService userUnitsService)
        {
            RewardsRepository = rewardsRepository ?? throw new ArgumentNullException(nameof(rewardsRepository));
            UnitTypesService = unitTypesService ?? throw new ArgumentNullException(nameof(unitTypesService));
            UserUnitsService = userUnitsService ?? throw new ArgumentNullException(nameof(userUnitsService));
        }
        public async Task<IRewardObject[]> GetNotAcceptedUserRewards(Guid userId)
        {
            var entities = await RewardsRepository.FindNotAcceptedRewardsByUserIdAsync(userId);
            return await Task.WhenAll(entities.Select(ToRewardObject));
        }

        public async Task<IReadOnlyDictionary<Guid, IRewardObject[]>> GetRewardsFromBattleDefinitions(Guid[] battleDefinitionIds)
        {
            var resultDictionary = battleDefinitionIds.ToDictionary(x => x, x => Array.Empty<IRewardObject>());
            await Task.WhenAll(battleDefinitionIds.Select(async id =>
            {
                var rewardEntities = await RewardsRepository.GetRewardsByBattleDefinitionId(id);
                var rewardObjects = await Task.WhenAll(rewardEntities.Select(ToRewardObject));
                resultDictionary[id] = rewardObjects;
            }));
            return resultDictionary;
        }

        public async Task<IRewardObject[]> GetRewardsFromBattleDefinition(Guid battleDefinitionId)
        {
            var rewardsDictionary = await GetRewardsFromBattleDefinitions(new [] { battleDefinitionId } );
            return rewardsDictionary[battleDefinitionId];
        }

        public Task GiveRewardsToUserAsync(Guid[] rewardIds, Guid userId)
        {
            return RewardsRepository.GiveRewardsToUserAsync(rewardIds, userId);
        }

        public async Task<AcceptedRewardData> AcceptRewardAsync(Guid rewardId, Guid userId, int[] amounts)
        {
            var rewardEntity = await RewardsRepository.RemoveRewardFromUser(userId, rewardId);
            var rewardObject = await ToRewardObject(rewardEntity);

            var unitTypes = rewardObject.GetUnitTypes();
            var unitTypesAndAmounts = amounts.Select((count, i) => new CreateUserUnitData
            {
                UserId = userId,
                UnitTypeId = unitTypes[i].Id,
                Amount = count,
            }).ToArray();

            var units = await UserUnitsService.CreateUnits(unitTypesAndAmounts);
            
            return new AcceptedRewardData
            {
                RewardId = rewardId,
                UserId = userId,
                UnitsGiven = units.ToArray(),
            };
        }

        public async Task<AcceptedRewardData> RejectRewardAsync(Guid rewardId, Guid userId)
        {
            await RewardsRepository.RemoveRewardFromUser(userId, rewardId);
            return AcceptedRewardData.Empty(rewardId, userId);
        }

        private async Task<IRewardObject> ToRewardObject(IRewardEntity entity)
        {
            switch (entity.RewardType)
            {
                case RewardType.None:
                    return EmptyRewardObject.FromEntity(entity);
                case RewardType.UnitsGain:
                    var units = await UnitTypesService.GetUnitTypesByIdsAsync(entity.TypeIds);
                    return UnitsGainRewardObject.FromEntity(entity, units);
                case RewardType.ResourcesGain:
                case RewardType.UnitToBuy:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}