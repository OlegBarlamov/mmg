using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.RewardDefinitions;
using Epic.Data.BattleDefinitions;
using Epic.Data.GlobalUnits;
using Epic.Data.Reward;
using Epic.Data.RewardDefinitions;
using Epic.Logic.Generator;
using JetBrains.Annotations;

namespace Epic.Logic.Rewards
{
    [UsedImplicitly]
    public class DefaultRewardDefinitionsService : IRewardDefinitionsService
    {
        public IRewardsRepository Repository { get; }
        public IBattleDefinitionsRepository BattleDefinitionsRepository { get; }
        public IBattleDefinitionsService BattleDefinitionsService { get; }
        public IGlobalUnitsRepository GlobalUnitsRepository { get; }
        [NotNull] public GlobalUnitsForBattleGenerator GlobalUnitsForBattleGenerator { get; }

        private readonly Random _random = new Random(Guid.NewGuid().GetHashCode());

        public DefaultRewardDefinitionsService(
            [NotNull] IRewardsRepository repository,
            [NotNull] IBattleDefinitionsRepository battleDefinitionsRepository,
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IGlobalUnitsRepository globalUnitsRepository,
            [NotNull] GlobalUnitsForBattleGenerator globalUnitsForBattleGenerator)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            BattleDefinitionsRepository = battleDefinitionsRepository ?? throw new ArgumentNullException(nameof(battleDefinitionsRepository));
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            GlobalUnitsRepository = globalUnitsRepository ?? throw new ArgumentNullException(nameof(globalUnitsRepository));
            GlobalUnitsForBattleGenerator = globalUnitsForBattleGenerator ?? throw new ArgumentNullException(nameof(globalUnitsForBattleGenerator));
        }
        
        public async Task<IRewardEntity[]> CreateRewardsFromDefinition(IRewardDefinitionEntity rewardDefinitionEntity, Guid battleDefinitionId, int rewardFactor)
        {
            var amounts = rewardDefinitionEntity.MaxAmounts.Select((x,i) => 
                _random.Next(rewardDefinitionEntity.MinAmounts[i], rewardDefinitionEntity.MaxAmounts[i]))
                .ToArray();

            if (rewardDefinitionEntity.RewardType == RewardType.ResourcesGain)
                amounts = amounts.Select(BattleGenerator.RoundToFriendlyNumber).ToArray();
            
            amounts = amounts.Select(x => x * rewardFactor).ToArray();

            IBattleDefinitionObject guardBattleDefinition = null;
            if (rewardDefinitionEntity.GuardUnitTypeIds.Length > 0)
            {
                var valueBasedBattleWidth = BattleConstants.StartBattleWidth + rewardDefinitionEntity.Value / 300;
                var valueBasedBattleHeight = BattleConstants.StartBattleHeight + rewardDefinitionEntity.Value / 300;
                var minBattleWidth = rewardDefinitionEntity.GuardBattleMinWidth > 0
                    ? rewardDefinitionEntity.GuardBattleMinWidth
                    : valueBasedBattleWidth;
                var maxBattleWidth = rewardDefinitionEntity.GuardBattleMaxWidth > 0
                    ? rewardDefinitionEntity.GuardBattleMaxWidth
                    : valueBasedBattleWidth;
                var minBattleHeight = rewardDefinitionEntity.GuardBattleMinHeight > 0
                    ? rewardDefinitionEntity.GuardBattleMinHeight
                    : valueBasedBattleHeight;
                var maxBattleHeight = rewardDefinitionEntity.GuardBattleMaxHeight > 0
                    ? rewardDefinitionEntity.GuardBattleMaxHeight
                    : valueBasedBattleHeight;

                var guardBattleWidth = Math.Max(BattleConstants.StartBattleWidth,
                    Math.Min(BattleConstants.MaxBattleWidth, _random.Next(minBattleWidth, maxBattleWidth + 1)));
                var guardBattleHeight = Math.Max(BattleConstants.StartBattleHeight,
                    Math.Min(BattleConstants.MaxBattleHeight, _random.Next(minBattleHeight, maxBattleHeight + 1)));
                
                guardBattleDefinition = await BattleDefinitionsService.CreateBattleDefinition(guardBattleWidth, guardBattleHeight);

                if (rewardDefinitionEntity.GuardUnitTypeIds.Length == 1)
                {
                    var minAmount = rewardDefinitionEntity.GuardUnitMinAmounts[0];
                    var maxAmount = rewardDefinitionEntity.GuardUnitMaxAmounts[0];
                    var amount = _random.Next(minAmount, maxAmount + 1);
                    
                    await GlobalUnitsForBattleGenerator.Generate(
                        guardBattleDefinition.UnitsContainerObject,
                        _random,
                        rewardDefinitionEntity.GuardUnitTypeIds[0],
                        amount,
                        false);
                }
                else
                {
                    var slotsDistribution = UnitsSlotsDistribution.FindSlotIndices(
                        rewardDefinitionEntity.GuardUnitTypeIds.Length,
                        guardBattleDefinition.Height);
                    
                    await Task.WhenAll(rewardDefinitionEntity.GuardUnitTypeIds.Select((id, i) =>
                    {
                        var minAmount = rewardDefinitionEntity.GuardUnitMinAmounts[i];
                        var maxAmount = rewardDefinitionEntity.GuardUnitMaxAmounts[i];
                        var amount = _random.Next(minAmount, maxAmount + 1);
                        
                        return GlobalUnitsRepository.Create(id, amount, guardBattleDefinition.ContainerId, true, slotsDistribution[i]);
                    }));
                }
            }

            var reward = await Repository.CreateRewardAsync(battleDefinitionId, new MutableRewardFields
            {
                RewardType = rewardDefinitionEntity.RewardType,
                Ids = rewardDefinitionEntity.Ids,
                Amounts = amounts,
                Message = rewardDefinitionEntity.Message,
                CanDecline = rewardDefinitionEntity.CanDecline,
                GuardMessage = rewardDefinitionEntity.GuardMessage,
                GuardBattleDefinitionId = guardBattleDefinition?.Id,
                IconUrl = rewardDefinitionEntity.IconUrl,
                Title = rewardDefinitionEntity.Title,
                Description = rewardDefinitionEntity.Title,
            });
            
            return new [] {reward};
        }
    }
}