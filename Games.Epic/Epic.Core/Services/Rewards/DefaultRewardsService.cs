using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Objects.Rewards;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Battles;
using Epic.Core.Services.GameResources;
using Epic.Core.Services.GameResources.Errors;
using Epic.Core.Services.Heroes;
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
        public IGameResourcesService GameResourcesService { get; }
        public IHeroesService HeroesService { get; }

        public DefaultRewardsService(
            [NotNull] IRewardsRepository rewardsRepository,
            [NotNull] IUnitTypesService unitTypesService,
            [NotNull] IGlobalUnitsService globalUnitsService,
            [NotNull] IUnitsContainersService containersService,
            [NotNull] IPlayersService playersService,
            [NotNull] IContainersManipulator containersManipulator,
            [NotNull] IGameResourcesRepository gameResourcesRepository,
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IBattlesService battlesService,
            [NotNull] IGameResourcesService gameResourcesService,
            [NotNull] IHeroesService heroesService)
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
            GameResourcesService = gameResourcesService ?? throw new ArgumentNullException(nameof(gameResourcesService));
            HeroesService = heroesService ?? throw new ArgumentNullException(nameof(heroesService));
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
                if (rewardObject.GuardBattleDefinition is { IsFinished: false })
                    throw new RewardGuardBattleIsNotFinishedException();

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
                        var armyUnitsInSlots =
                            await GlobalUnitsService.GetAliveUnitFromContainerPerSlots(
                                player.ActiveHero.ArmyContainerId, 0, maxSlot);
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

                        prices.For((x, i) =>
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

                        var notPlacedUnits = units.Where(x => x.ContainerId != player.ActiveHero.ArmyContainerId)
                            .ToArray();
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
                        var createData = amounts.Select((count, i) => new CreateUnitData(unitTypes[i].Id, count))
                            .ToArray();
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

                // Handle Market rewards separately before standard resource handling
                if (rewardObject.RewardType == RewardType.Market)
                {
                    // Market uses standard resource handling: amounts array contains net changes for each resource
                    // Positive amounts = resources to add, negative amounts = resources to remove
                    // Gold changes are included in the resources list
                    // Client calculates all net changes including gold cost/gain based on 5x buy price and full sell price
                    
                    var resources = rewardObject.Resources;
                    if (resources.Count > 0)
                    {
                        // Validate that player has enough resources for negative amounts (selling)
                        for (int i = 0; i < resources.Count && i < amounts.Length; i++)
                        {
                            if (amounts[i] < 0)
                            {
                                var currentAmount = await GameResourcesRepository.GetResourceByPlayer(resources[i].Id, playerId);
                                if (Math.Abs(amounts[i]) > currentAmount.Amount)
                                    throw new NotEnoughResourcesToPayException();
                            }
                        }
                        
                        // Apply net changes using GiveResource (handles both positive and negative deltas)
                        for (int i = 0; i < resources.Count && i < amounts.Length; i++)
                        {
                            if (amounts[i] != 0)
                            {
                                await GameResourcesRepository.GiveResource(resources[i].Id, playerId, amounts[i]);
                            }
                        }
                        
                        // Set resourcesGiven for positive amounts only (resources gained)
                        resourcesGiven = resources
                            .Select((x, i) => new { Resource = x, Amount = i < amounts.Length ? amounts[i] : 0 })
                            .Where(x => x.Amount > 0)
                            .Select(x => ResourceAmount.Create(x.Resource, x.Amount))
                            .ToArray();
                    }
                }
                else
                {
                    // Standard resource handling for non-Market rewards
                    var resources = rewardObject.Resources;
                    if (resources.Count > 0)
                    {
                        resourcesGiven = resources.Select((x, i) => ResourceAmount.Create(x, amounts[i])).ToArray();
                        await GameResourcesRepository.GiveResources(resourcesGiven, playerId);
                    }
                }

                if (rewardObject.RewardType == RewardType.Attack)
                {
                    var attackAmount = rewardObject.Amounts.Length > 0 ? rewardObject.Amounts[0] : 0;
                    if (attackAmount > 0 && player.ActiveHero != null)
                    {
                        await HeroesService.AddAttack(player.ActiveHero.Id, attackAmount);
                    }
                }

                if (rewardObject.RewardType == RewardType.Defense)
                {
                    var defenseAmount = rewardObject.Amounts.Length > 0 ? rewardObject.Amounts[0] : 0;
                    if (defenseAmount > 0 && player.ActiveHero != null)
                    {
                        await HeroesService.AddDefense(player.ActiveHero.Id, defenseAmount);
                    }
                }

                if (rewardObject.RewardType == RewardType.NextStage)
                {
                    // We intentionally do nothing here, as the stage increment is handled in the BattleResultLogic
                }

                return new AcceptedRewardData
                {
                    RewardId = rewardId,
                    PlayerId = playerId,
                    UnitsGiven = unitsGiven,
                    ResourcesGiven = resourcesGiven,
                    PricePayed = priceToPay,
                };
            }
            catch (RewardGuardBattleIsNotFinishedException)
            {
                await RewardsRepository.GiveRewardsToPlayerAsync(new[] { rewardId }, playerId);
                throw;
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

        public async Task<IBattleObject> BeginRewardGuardBattle(Guid rewardId, Guid playerId)
        {
            var rewardEntity = await RewardsRepository.GetRewardForPlayer(playerId, rewardId);
            if (!rewardEntity.GuardBattleDefinitionId.HasValue)
                throw new InvalidOperationException("The reward guard battle definition is null.");
            
            var guardBattleDefinition = await BattleDefinitionsService.GetBattleDefinitionById(rewardEntity.GuardBattleDefinitionId.Value);
            var battleObject = await BattlesService.CreateBattleFromDefinition(playerId, guardBattleDefinition, false);
            return await BattlesService.BeginBattle(playerId, battleObject);
        }

        public async Task RejectRewardsGuardedByBattleDefinition(Guid playerId, Guid guardBattleDefinitionId)
        {
            var notAcceptedRewards = await RewardsRepository.FindNotAcceptedRewardsByPlayerId(playerId);
            var guardedRewards = notAcceptedRewards
                .Where(x => x.GuardBattleDefinitionId.HasValue && x.GuardBattleDefinitionId.Value == guardBattleDefinitionId)
                .ToArray();

            // Force remove the rewards from the player regardless of CanDecline.
            // Run in parallel since these are independent operations.
            await Task.WhenAll(guardedRewards.Select(reward =>
                RewardsRepository.RemoveRewardFromPlayer(playerId, reward.Id)));
        }

        private async Task<IRewardObject> ToRewardObject(IRewardEntity entity)
        {
            var rewardObject = new CompositeRewardObject(entity);
            
            switch (entity.RewardType)
            {
                case RewardType.None:
                    break;
                case RewardType.UnitsGain:
                    rewardObject.UnitTypes = (await UnitTypesService.GetUnitTypesByIdsAsync(entity.Ids)).ToArray();
                    break;
                case RewardType.UnitsToBuy:
                case RewardType.UnitsToUpgrade:
                    rewardObject.UnitTypes = (await UnitTypesService.GetUnitTypesByIdsAsync(entity.Ids)).ToArray();
                    var prices = await UnitTypesService.GetPrices(rewardObject.UnitTypes);
                    rewardObject.Prices = await GameResourcesService.GetResourcesAmountsFromPrices(prices);
                    break;
                case RewardType.ResourcesGain:
                    rewardObject.Resources = await GameResourcesRepository.GetByIds(entity.Ids);
                    break;
                case RewardType.Attack:
                case RewardType.Defense:
                case RewardType.NextStage:
                    // No additional data needed for Attack/Defense/NextStage rewards
                    break;
                case RewardType.Market:
                    // Load all resources for market trading
                    var allResourcesByKeys = await GameResourcesRepository.GetAllResourcesByKeys();
                    rewardObject.Resources = allResourcesByKeys.Values.ToArray();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (rewardObject.GuardBattleDefinitionId.HasValue)
                rewardObject.GuardBattleDefinition = await BattleDefinitionsService.GetBattleDefinitionById(rewardObject.GuardBattleDefinitionId.Value);

            return rewardObject;
        }
    }
}