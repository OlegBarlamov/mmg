using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.GameResources;
using Epic.Core.Services.Players;
using Epic.Core.Services.RewardDefinitions;
using Epic.Core.Services.UnitsContainers;
using Epic.Core.Services.UnitTypes;
using Epic.Data.GameResources;
using Epic.Data.GlobalUnits;
using Epic.Data.Reward;
using Epic.Data.UnitTypes;
using Epic.Logic.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Logic.Generator
{
    public interface IBattlesGenerator
    {
        Task Generate(Guid playerId, int day, int currentBattlesCount);
        Task GenerateSingle(Guid playerId, int day);
    }

    [UsedImplicitly]
    public class BattleGenerator : IBattlesGenerator
    {
        public IBattleDefinitionsService BattleDefinitionsService { get; }
        public IGlobalUnitsRepository GlobalUnitsRepository { get; }
        public IUnitTypesRepository UnitTypesRepository { get; }
        public IUnitsContainersService UnitsContainersService { get; }
        public IRewardsRepository RewardsRepository { get; }
        public IGameResourcesRepository GameResourcesRepository { get; }
        public IPlayersService PlayersService { get; }
        public ILogger<BattleGenerator> Logger { get; }
        public IGameResourcesRegistry ResourcesRegistry { get; }
        public IUnitTypesRegistry UnitTypesRegistry { get; }
        public IRewardDefinitionsService RewardDefinitionsService { get; }
        public IRewardDefinitionsRegistry RewardDefinitionsRegistry { get; }
        public GlobalUnitsForBattleGenerator GlobalUnitsForBattleGenerator { get; }

        private readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
        
        public BattleGenerator(
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IGlobalUnitsRepository globalUnitsRepository,
            [NotNull] IUnitTypesRepository unitTypesRepository,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IRewardsRepository rewardsRepository,
            [NotNull] IGameResourcesRepository gameResourcesRepository,
            [NotNull] IPlayersService playersService,
            [NotNull] ILogger<BattleGenerator> logger,
            [NotNull] IGameResourcesRegistry resourcesRegistry,
            [NotNull] IUnitTypesRegistry unitTypesRegistry,
            [NotNull] IRewardDefinitionsService rewardDefinitionsService,
            [NotNull] IRewardDefinitionsRegistry rewardDefinitionsRegistry,
            [NotNull] GlobalUnitsForBattleGenerator globalUnitsForBattleGenerator)
        {
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            GlobalUnitsRepository = globalUnitsRepository ?? throw new ArgumentNullException(nameof(globalUnitsRepository));
            UnitTypesRepository = unitTypesRepository ?? throw new ArgumentNullException(nameof(unitTypesRepository));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            RewardsRepository = rewardsRepository ?? throw new ArgumentNullException(nameof(rewardsRepository));
            GameResourcesRepository = gameResourcesRepository ?? throw new ArgumentNullException(nameof(gameResourcesRepository));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ResourcesRegistry = resourcesRegistry ?? throw new ArgumentNullException(nameof(resourcesRegistry));
            UnitTypesRegistry = unitTypesRegistry ?? throw new ArgumentNullException(nameof(unitTypesRegistry));
            RewardDefinitionsService = rewardDefinitionsService ?? throw new ArgumentNullException(nameof(rewardDefinitionsService));
            RewardDefinitionsRegistry = rewardDefinitionsRegistry ?? throw new ArgumentNullException(nameof(rewardDefinitionsRegistry));
            GlobalUnitsForBattleGenerator = globalUnitsForBattleGenerator ?? throw new ArgumentNullException(nameof(globalUnitsForBattleGenerator));
        }

        public async Task GenerateSingle(Guid playerId, int day)
        {
            var rewardFactor = 1;
            
            var orderedUnitTypes = UnitTypesRegistry.AllOrderedByValue;
            var toTrainOrderedUnitTypes = UnitTypesRegistry.ToTrainOrderedByValue;
            var resources = ResourcesRegistry.GetAll();
            
            var player = await PlayersService.GetById(playerId);
            var difficulty = DifficultyMarker.GenerateFromDay(_random, day);
            
            Logger.LogInformation($"Generated Difficulty day {day}: {difficulty.TargetDifficulty}; {difficulty.MinDifficulty}-{difficulty.MaxDifficulty}");

            var maxWidth = Math.Min(BattleConstants.MaxBattleWidth, BattleConstants.StartBattleWidth + difficulty.TargetDifficulty / 300);
            var maxHeight = Math.Min(BattleConstants.MaxBattleHeight, BattleConstants.StartBattleHeight + difficulty.TargetDifficulty / 300);
            
            var width = _random.Next(BattleConstants.MinBattleWidth, maxWidth);
            var height = _random.Next(BattleConstants.MinBattleHeight, maxHeight);
            
            var maxStrongUnitIndex = BinarySearch.FindClosestNotExceedingIndex(orderedUnitTypes,
                entity => entity.Value, difficulty.TargetDifficulty);
            var normalizedMean = 1.0 / 3.0; // Bias toward lower part
            var stdDev = 0.25; // how chaotic the output
            var sample = RandomDistributions.GetBoundedNormal(_random, normalizedMean, stdDev, 0, 1);
            var targetIndex = (int)(sample * maxStrongUnitIndex);
            var targetUnit = orderedUnitTypes[targetIndex];

            var unitsCount = Math.Max(1, (int)Math.Round((double)difficulty.TargetDifficulty / targetUnit.Value));
            
            var container = await UnitsContainersService.Create(height, Guid.Empty);

            await GlobalUnitsForBattleGenerator.Generate(container, _random, targetUnit.Id, unitsCount, true);
            
            double t = ((double)difficulty.TargetDifficulty - difficulty.MinDifficulty) /
                       ((double)difficulty.IdealDifficulty - difficulty.MinDifficulty);
            int duration = Math.Max(1, (int)Math.Round(1 + t * 3) + _random.Next(-2, 3));

            var rewardVisibility = 0;
            if (difficulty.IdealDifficulty > 1000)
            {
                var rewardVisibilityChance = _random.NextDouble();
                if (rewardVisibilityChance < 0.06)
                    rewardVisibility = -1;
                if (rewardVisibilityChance < 0.02)
                    rewardVisibility = -2;
            }

            var guardVisibility = 0;
            if (difficulty.IdealDifficulty > 2000)
            {
                var guardVisibilityChance = _random.NextDouble();
                if (guardVisibilityChance < 0.06)
                    guardVisibility = -1;
                if (guardVisibilityChance < 0.02)
                    guardVisibility = -2;
            }
            
            var battleDefinition = await BattleDefinitionsService.CreateBattleDefinition(
                    playerId,
                    width,
                    height,
                    day + duration,
                    rewardVisibility,
                    guardVisibility,
                    container.Id);


            var rewardTypeIndex = _random.Next(0, Enum.GetValues(typeof(GeneratedRewardTypes))
                .Cast<int>()
                .Max() + 1);

            var rewardType = (GeneratedRewardTypes)rewardTypeIndex;
            rewardType = GeneratedRewardTypes.Template;

            if (rewardType == GeneratedRewardTypes.Gold)
            {
                var goldAmount = RoundToFriendlyNumber(difficulty.TargetDifficulty);
                await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                {
                    RewardType = RewardType.ResourcesGain,
                    Amounts = new[] { goldAmount * rewardFactor },
                    CanDecline = true,
                    GuardBattleDefinitionId = null,
                    IconUrl = null,
                    Title = null,
                    Ids = new[] { GameResourcesRepository.GoldResourceId },
                });
            }
            else if (rewardType == GeneratedRewardTypes.Resource)
            {
                var resourcesValue = difficulty.TargetDifficulty;
                var resourceTypes = new List<IGameResourceEntity>();
                var resourcesAmounts = new List<int>();
                var resourceTypesCount = resourcesValue > resources.Sum(x => x.Price) / 2
                    ? _random.Next(1, resources.Count) : 1;
                var valuePerResource = (double)resourcesValue / resourceTypesCount;
                var availableResources = new List<IGameResourceEntity>(resources);
                for (var i = 0; i < resourceTypesCount; i++)
                {
                    var resourceType = availableResources[_random.Next(0, availableResources.Count)];
                    availableResources.Remove(resourceType);
                    
                    var resourceAmount = Math.Max(1,
                        (int)Math.Ceiling(valuePerResource / resourceType.Price));
                    resourceAmount = RoundToFriendlyNumber(resourceAmount);
                    
                    resourceTypes.Add(resourceType);
                    resourcesAmounts.Add(resourceAmount * rewardFactor);
                }
                
                await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                {
                    RewardType = RewardType.ResourcesGain,
                    Amounts = resourcesAmounts.ToArray(),
                    CanDecline = true,
                    GuardBattleDefinitionId = null,
                    IconUrl = null,
                    Title = null,
                    Ids = resourceTypes.Select(x => x.Id).ToArray(),
                });
            }
            else if (rewardType == GeneratedRewardTypes.UnitsGain)
            {
                var maxUnitIndex = BinarySearch.FindClosestNotExceedingIndex(orderedUnitTypes,
                    entity => entity.Value, difficulty.TargetDifficulty);
                var unitToGain = orderedUnitTypes[_random.Next(0, maxUnitIndex + 1)];
                var unitsGainAmount = Math.Max(1, (int)Math.Floor(((double)difficulty.TargetDifficulty / 2) / unitToGain.Value));
                
                var supplyUnits = await GlobalUnitsRepository.GetAliveByContainerId(player.SupplyContainerId);
                var armyUnits = await GlobalUnitsRepository.GetAliveByContainerId(player.ActiveHero.ArmyContainerId);
                var desiredUnits = supplyUnits.Concat(armyUnits)
                    .Select(x => x.TypeId)
                    .Distinct()
                    .Select(UnitTypesRegistry.ById);

                var availableDesiredUnits = desiredUnits.Where(x => x.Value <= difficulty.TargetDifficulty).ToList();
                if (availableDesiredUnits.Any() && _random.Next(100) < 33)
                {
                    unitToGain = availableDesiredUnits[_random.Next(0, availableDesiredUnits.Count)];
                    unitsGainAmount = Math.Max(1, (int)Math.Floor(((double)difficulty.TargetDifficulty / 3) / unitToGain.Value));
                }
                
                await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                {
                    RewardType = RewardType.UnitsGain,
                    Amounts = new[] { unitsGainAmount * rewardFactor },
                    CanDecline = true,
                    GuardBattleDefinitionId = null,
                    IconUrl = null,
                    Title = null,
                    Ids = new[] { unitToGain.Id },
                });
            } else if (rewardType == GeneratedRewardTypes.UnitsToBuy)
            {
                var maxUnitIndex = BinarySearch.FindClosestNotExceedingIndex(toTrainOrderedUnitTypes,
                    entity => entity.Value, (int)(difficulty.TargetDifficulty * 1.5));
                var unitToBuy = toTrainOrderedUnitTypes[_random.Next(maxUnitIndex + 1)];
                
                var supplyUnits = await GlobalUnitsRepository.GetAliveByContainerId(player.SupplyContainerId);
                var armyUnits = await GlobalUnitsRepository.GetAliveByContainerId(player.ActiveHero.ArmyContainerId);
                IEnumerable<Guid> GetWithUpgrades(Guid typeId) =>
                    new[] { typeId }.Concat(UnitTypesRegistry.GetUpgradesFor(typeId).Select(u => u.Id));
                IEnumerable<Guid> GetOriginalTypes(Guid typeId) =>
                    new[] { typeId }.Concat(UnitTypesRegistry.GetSourceTypeFromUpgraded(typeId).Select(u => u.Id));

                var playerUnitTypes = supplyUnits.Concat(armyUnits)
                    .Select(x => x.TypeId)
                    .Distinct()
                    .ToArray();
                var desiredUnits = playerUnitTypes
                    .SelectMany(GetOriginalTypes)
                    .SelectMany(GetWithUpgrades)
                    .Distinct()
                    .Select(UnitTypesRegistry.ById);
                
                var availableDesiredUnits = desiredUnits.Where(x => x.Value <= difficulty.TargetDifficulty).ToList();
                var desiredUnit = false;
                if (availableDesiredUnits.Any() && _random.Next(100) > 66)
                {
                    unitToBuy = availableDesiredUnits[_random.Next(availableDesiredUnits.Count)];
                    desiredUnit = true;
                }

                var upgradeOnly = unitToBuy.ToTrainAmount < 1 && unitToBuy.UpgradeForUnitTypeIds.Any();
                var isUpgrade = upgradeOnly || (desiredUnit && unitToBuy.UpgradeForUnitTypeIds.Any() && _random.Next(100) < 50);
                var dwellingIcon = string.IsNullOrWhiteSpace(unitToBuy.DwellingImgUrl) 
                    ? unitToBuy.BattleImgUrl 
                    : unitToBuy.DwellingImgUrl;
                    
                var isGuarded = !isUpgrade && unitToBuy.Value >= 400;
                var rewardedBattleDefinition = battleDefinition;
                
                var rewardFields = new MutableRewardFields
                {
                    RewardType = isUpgrade ? RewardType.UnitsToUpgrade : RewardType.UnitsToBuy,
                    Amounts = isUpgrade ? new[] { 0 } : new[] { unitToBuy.ToTrainAmount * rewardFactor },
                    Message = isUpgrade ? "You can upgrade units now" : "You can train units now",
                    CanDecline = true,
                    GuardBattleDefinitionId = null,
                    IconUrl = dwellingIcon,
                    Title = $"Dwelling of {unitToBuy.Name}",
                    Ids = new[] { unitToBuy.Id },
                };
                
                if (isGuarded)
                {
                    var guardBattleWidth = Math.Min(BattleConstants.MaxBattleWidth, BattleConstants.StartBattleWidth + (unitToBuy.Value * unitToBuy.ToTrainAmount / 2) / 300);
                    var guardBattleHeight = Math.Min(BattleConstants.MaxBattleHeight, BattleConstants.StartBattleHeight + (unitToBuy.Value * unitToBuy.ToTrainAmount / 2) / 300);
                    
                    var guardBattleDefinition = await BattleDefinitionsService.CreateBattleDefinition(guardBattleWidth, guardBattleHeight);
                    
                    await GlobalUnitsRepository.Create(unitToBuy.Id, unitToBuy.ToTrainAmount / 2,
                        guardBattleDefinition.ContainerId, true, guardBattleDefinition.Height / 2);
                    
                    rewardFields.GuardBattleDefinitionId = guardBattleDefinition.Id;
                    rewardFields.GuardMessage = $"You need to defeat guards to train {unitToBuy.Name}";
                }
                
                await RewardsRepository.CreateRewardAsync(rewardedBattleDefinition.Id, rewardFields);
            } else if (rewardType == GeneratedRewardTypes.Template)
            {
                var maxRewardIndex = BinarySearch.FindClosestNotExceedingIndex(RewardDefinitionsRegistry.AllOrdered,
                    group => group.First().Value, difficulty.TargetDifficulty);
                var rewardGroup = RewardDefinitionsRegistry.AllOrdered[_random.Next(maxRewardIndex + 1)];

                var rewardTemplates = rewardGroup.ToArray();
                var targetRewardTemplate = rewardTemplates[_random.Next(rewardTemplates.Length)];
                await RewardDefinitionsService.CreateRewardsFromDefinition(targetRewardTemplate, battleDefinition.Id, rewardFactor);
                
                var remainingValue = difficulty.TargetDifficulty - rewardGroup.First().Value;
                if (remainingValue > 500)
                {
                    var resourcesValue = remainingValue;
                    var resourceTypes = new List<IGameResourceEntity>();
                    var resourcesAmounts = new List<int>();
                    var resourceTypesCount = resourcesValue > resources.Sum(x => x.Price) / 2
                        ? _random.Next(1, resources.Count) : 1;
                    var valuePerResource = (double)resourcesValue / resourceTypesCount;
                    var availableResources = new List<IGameResourceEntity>(resources);
                    for (var i = 0; i < resourceTypesCount; i++)
                    {
                        var resourceType = availableResources[_random.Next(0, availableResources.Count)];
                        availableResources.Remove(resourceType);
                    
                        var resourceAmount = Math.Max(1,
                            (int)Math.Ceiling(valuePerResource / resourceType.Price));
                        resourceAmount = RoundToFriendlyNumber(resourceAmount);
                    
                        resourceTypes.Add(resourceType);
                        resourcesAmounts.Add(resourceAmount * rewardFactor);
                    }
                
                    await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                    {
                        RewardType = RewardType.ResourcesGain,
                        Amounts = resourcesAmounts.ToArray(),
                        CanDecline = true,
                        GuardBattleDefinitionId = null,
                        IconUrl = null,
                        Title = null,
                        Ids = resourceTypes.Select(x => x.Id).ToArray(),
                    });
                }
            }
        }

        public async Task Generate(Guid playerId, int day, int currentBattlesCount)
        {
            int count = currentBattlesCount <= Math.Max(5, day / 2) ? _random.Next(3, 7) : _random.Next(1, 4);
            for (int i = 0; i < count; i++)
            {
                await GenerateSingle(playerId, day);
            }
        }

        internal enum GeneratedRewardTypes
        {
            Gold, 
            Resource,
            UnitsGain,
            UnitsToBuy,
            Template,
        }
        
        public static int RoundToFriendlyNumber(int value)
        {
            if (value <= 0) return 0;

            // Determine magnitude (1, 10, 100, etc.)
            int magnitude = (int)Math.Pow(10, (int)Math.Floor(Math.Log10(value)));

            // Define some rounding steps (can be adjusted)
            double[] steps = { 1, 2, 2.5, 5, 10 };

            foreach (var step in steps)
            {
                int rounded = (int)(Math.Round(value / (magnitude * step)) * magnitude * step);
                if (Math.Abs(rounded - value) <= magnitude * step / 2)
                    return rounded;
            }

            // Fallback: round to next magnitude
            return (int)(Math.Round((double)value / magnitude) * magnitude);
        }
    }
}