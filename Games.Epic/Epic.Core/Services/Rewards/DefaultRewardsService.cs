using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Objects.Rewards;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Battles;
using Epic.Core.Services.GameResources.Errors;
using Epic.Core.Services.Players;
using Epic.Core.Services.Rewards.Errors;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitsContainers;
using Epic.Core.Services.UnitsContainers.Errors;
using Epic.Core.Services.UnitTypes;
using Epic.Data.GameResources;
using Epic.Data.Reward;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Core.Services.Rewards
{
    [UsedImplicitly]
    public class DefaultRewardsService : IRewardsService
    {
        public IRewardsRepository RewardsRepository { get; }
        public IUnitTypesService UnitTypesService { get; }
        public IGlobalUnitsService GlobalUnitsService { get; }
        public IUnitsContainersService ContainersService { get; }
        public IPlayersService PlayersService { get; }
        public IContainersManipulator ContainersManipulator { get; }
        public IGameResourcesRepository GameResourcesRepository { get; }
        public IBattleDefinitionsService BattleDefinitionsService { get; }
        public IBattlesService BattlesService { get; }

        public DefaultRewardsService(
            [NotNull] IRewardsRepository rewardsRepository,
            [NotNull] IUnitTypesService unitTypesService,
            [NotNull] IGlobalUnitsService globalUnitsService,
            [NotNull] IUnitsContainersService containersService,
            [NotNull] IPlayersService playersService,
            [NotNull] IContainersManipulator containersManipulator,
            [NotNull] IGameResourcesRepository gameResourcesRepository,
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IBattlesService battlesService)
        {
            RewardsRepository = rewardsRepository ?? throw new ArgumentNullException(nameof(rewardsRepository));
            UnitTypesService = unitTypesService ?? throw new ArgumentNullException(nameof(unitTypesService));
            GlobalUnitsService = globalUnitsService ?? throw new ArgumentNullException(nameof(globalUnitsService));
            ContainersService = containersService ?? throw new ArgumentNullException(nameof(containersService));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            ContainersManipulator = containersManipulator ?? throw new ArgumentNullException(nameof(containersManipulator));
            GameResourcesRepository = gameResourcesRepository ?? throw new ArgumentNullException(nameof(gameResourcesRepository));
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
        }
        public async Task<IRewardObject[]> GetNotAcceptedPlayerRewards(Guid playerId)
        {
            var entities = await RewardsRepository.FindNotAcceptedRewardsByPlayerId(playerId);
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

        public Task GiveRewardsToPlayerAsync(Guid[] rewardIds, Guid playerId)
        {
            return RewardsRepository.GiveRewardsToPlayerAsync(rewardIds, playerId);
        }

        public async Task<AcceptedRewardData> AcceptRewardAsync(Guid rewardId, Guid playerId, int[] amounts, int[] affectedSlots)
        {
            var player = await PlayersService.GetById(playerId);
            var rewardEntity = await RewardsRepository.RemoveRewardFromPlayer(playerId, rewardId);
            var rewardObject = await ToRewardObject(rewardEntity);

            var priceToPay = Price.Empty();
            var unitsGiven = Array.Empty<IGlobalUnitObject>();
            var resourcesGiven = Array.Empty<ResourceAmount>();

            try
            {
                var unitTypes = rewardObject.UnitTypes;
                if (unitTypes.Count > 0)
                {
                    if (rewardObject.RewardType == RewardType.UnitsToBuy)
                    {
                        var prices = await UnitTypesService.GetPrices(unitTypes);
                        prices.For((x, i) =>
                        {
                            x.MultiplyBy(amounts[i]);
                            return false;
                        });
                        priceToPay = Price.Combine(prices);

                        var enoughResources = await GameResourcesRepository.IsEnoughToPay(priceToPay, playerId);
                        if (!enoughResources)
                            throw new NotEnoughResourcesToPayException();
                    }

                    if (rewardObject.RewardType == RewardType.UnitsToUpgrade)
                    {
                        int maxSlot = affectedSlots.Any() ? affectedSlots.Max() : 0;
                        var armyUnitsInSlots = await GlobalUnitsService.GetAliveUnitFromContainerPerSlots(player.ActiveHero.ArmyContainerId, 0, maxSlot);
                        var upgradeUnitsData = new List<UpgradeUnitData>();
                        for (int i = 0; i < affectedSlots.Length; i++)
                        {
                            var targetSlotIndex = affectedSlots[i];
                            var targetUnit = armyUnitsInSlots[targetSlotIndex];
                            if (targetUnit == null)
                                throw new InvalidOperationException($"Target slot {targetSlotIndex} is empty");
                            
                            var upgradeTo = unitTypes.FirstOrDefault(x => x.IsUpgradeFor(targetUnit.UnitType));
                            if (upgradeTo == null)
                                throw new UnitCantBeUpgradedInTheReward(targetUnit, rewardObject);
                            
                            upgradeUnitsData.Add(new UpgradeUnitData(targetUnit, upgradeTo, amounts[i]));
                        }

                        var prices = await Task.WhenAll(upgradeUnitsData.Select(x =>
                            UnitTypesService.GetPriceForUpgrade(x.Unit.UnitType, x.UpgradeToType)));
                        
                        prices.For((x,i) =>
                        {
                            x.MultiplyBy(amounts[i]);
                            return false;
                        });
                        priceToPay = Price.Combine(prices);
                        
                        var enoughResources = await GameResourcesRepository.IsEnoughToPay(priceToPay, playerId);
                        if (!enoughResources)
                            throw new NotEnoughResourcesToPayException();

                        var units = await GlobalUnitsService.UpgradeUnits(upgradeUnitsData);
                        
                        var payed = await GameResourcesRepository.PayIfEnough(priceToPay, playerId);
                        if (!payed)
                            throw new NotEnoughResourcesToPayException();

                        var notPlacedUnits = units.Where(x => x.ContainerId != player.ActiveHero.ArmyContainerId).ToArray();
                        try
                        {
                            await ContainersManipulator.PlaceUnitsToContainer(player.ActiveHero.ArmyContainerId,
                                notPlacedUnits);
                        }
                        catch (InvalidUnitSlotsOperationException)
                        {
                            await ContainersManipulator.PlaceUnitsToContainer(player.SupplyContainerId, notPlacedUnits);
                        }
                    }
                    else
                    {
                        var createData = amounts.Select((count, i) => new CreateUnitData(unitTypes[i].Id, count)).ToArray();
                        var units = await GlobalUnitsService.CreateUnits(createData);
                        unitsGiven = units.ToArray();

                        if (!priceToPay.IsEmpty())
                        {
                            var payed = await GameResourcesRepository.PayIfEnough(priceToPay, playerId);
                            if (!payed)
                            {
                                await GlobalUnitsService.RemoveUnits(units);
                                throw new NotEnoughResourcesToPayException();
                            }
                        }

                        try
                        {
                            await ContainersManipulator.PlaceUnitsToContainer(player.ActiveHero.ArmyContainerId,
                                unitsGiven);
                        }
                        catch (InvalidUnitSlotsOperationException)
                        {
                            await ContainersManipulator.PlaceUnitsToContainer(player.SupplyContainerId, unitsGiven);
                        }
                    }
                }

                var resources = rewardObject.Resources;
                if (resources.Count > 0)
                {
                    resourcesGiven = resources.Select((x, i) => ResourceAmount.Create(x, amounts[i])).ToArray();
                    await GameResourcesRepository.GiveResources(resourcesGiven, playerId);
                }

                IBattleObject battleObject = null;
                if (rewardObject.NextBattleDefinition != null)
                {
                    var battleDefinition = await BattleDefinitionsService.GetBattleDefinitionById(rewardObject.NextBattleDefinition.Id);
                    battleObject = await BattlesService.CreateBattleFromDefinition(playerId, battleDefinition, false);
                    battleObject = await BattlesService.BeginBattle(playerId, battleObject);
                }

                return new AcceptedRewardData
                {
                    RewardId = rewardId,
                    PlayerId = playerId,
                    UnitsGiven = unitsGiven,
                    ResourcesGiven = resourcesGiven,
                    PricePayed = priceToPay,
                    NextBattle = battleObject,
                };
            }
            catch (NotEnoughResourcesToPayException)
            {
                await RewardsRepository.GiveRewardsToPlayerAsync(new[] { rewardId }, playerId);
                throw;
            }
        }

        public async Task<AcceptedRewardData> RejectRewardAsync(Guid rewardId, Guid playerId)
        {
            var reward = (await RewardsRepository.GetRewardsByIdAsync(new[] { rewardId }))[0];
            if (!reward.CanDecline)
                throw new RewardCanNotBeDeclineException();
            
            await RewardsRepository.RemoveRewardFromPlayer(playerId, rewardId);
            return AcceptedRewardData.Empty(rewardId, playerId);
        }

        private async Task<IRewardObject> ToRewardObject(IRewardEntity entity)
        {
            var rewardObject = new CompositeRewardObject(entity);
            switch (entity.RewardType)
            {
                case RewardType.None:
                    break;
                case RewardType.UnitsGain:
                case RewardType.UnitsToBuy:
                case RewardType.UnitsToUpgrade:
                    rewardObject.UnitTypes = (await UnitTypesService.GetUnitTypesByIdsAsync(entity.Ids)).ToArray();
                    break;
                case RewardType.ResourcesGain:
                    rewardObject.Resources = await GameResourcesRepository.GetByIds(entity.Ids);
                    break;
                case RewardType.Battle:
                    rewardObject.NextBattleDefinition = await BattleDefinitionsService.GetBattleDefinitionById(rewardObject.NextBattleDefinitionId.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return rewardObject;
        }
    }
}