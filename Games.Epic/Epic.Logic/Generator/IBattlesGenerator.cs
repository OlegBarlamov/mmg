using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Logic;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.GameResources;
using Epic.Core.Services.Players;
using Epic.Core.Services.RewardDefinitions;
using Epic.Core.Services.ArtifactTypes;
using Epic.Data.BattleDefinitions;
using Epic.Data.Players;
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
        /// <summary>
        /// Generate battles for a specific stage.
        /// </summary>
        /// <param name="playerId">Player ID</param>
        /// <param name="effectiveDay">The effective day for the stage (globalDay - stageUnlockedDay + 1) - used for difficulty</param>
        /// <param name="globalDay">The global day (used for expiration calculation)</param>
        /// <param name="currentBattlesCount">Current number of battles for the player</param>
        /// <param name="stage">Stage index</param>
        Task Generate(Guid playerId, int effectiveDay, int globalDay, int currentBattlesCount, int stage);
        
        /// <summary>
        /// Generate a single battle for a specific stage.
        /// </summary>
        Task GenerateSingle(Guid playerId, int effectiveDay, int globalDay, int stage);
        
        /// <summary>
        /// Generate a test battle with specified enemies. No obstacles, no rewards.
        /// </summary>
        /// <param name="playerId">Player ID</param>
        /// <param name="width">Battle field width</param>
        /// <param name="height">Battle field height</param>
        /// <param name="enemies">List of (unitKey, count) tuples for enemy units</param>
        Task GenerateTestBattle(Guid playerId, int width, int height, IReadOnlyList<(string unitKey, int count)> enemies);
    }

    [UsedImplicitly]
    public class BattleGenerator : IBattlesGenerator
    {
        public IBattleDefinitionsService BattleDefinitionsService { get; }
        public IBattleDefinitionsRepository BattleDefinitionsRepository { get; }
        public IGlobalUnitsRepository GlobalUnitsRepository { get; }
        public IUnitTypesRepository UnitTypesRepository { get; }
        public IUnitsContainersService UnitsContainersService { get; }
        public IRewardsRepository RewardsRepository { get; }
        public IGameResourcesRepository GameResourcesRepository { get; }
        public IPlayersService PlayersService { get; }
        public IPlayersRepository PlayersRepository { get; }
        public ILogger<BattleGenerator> Logger { get; }
        public IGameResourcesRegistry ResourcesRegistry { get; }
        public IUnitTypesRegistry UnitTypesRegistry { get; }
        public IRewardDefinitionsService RewardDefinitionsService { get; }
        public IRewardDefinitionsRegistry RewardDefinitionsRegistry { get; }
        public GlobalUnitsForBattleGenerator GlobalUnitsForBattleGenerator { get; }
        public IGameModeProvider GameModeProvider { get; }
        public IArtifactTypesRegistry ArtifactTypesRegistry { get; }

        private readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
        
        public BattleGenerator(
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IBattleDefinitionsRepository battleDefinitionsRepository,
            [NotNull] IGlobalUnitsRepository globalUnitsRepository,
            [NotNull] IUnitTypesRepository unitTypesRepository,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IRewardsRepository rewardsRepository,
            [NotNull] IGameResourcesRepository gameResourcesRepository,
            [NotNull] IPlayersService playersService,
            [NotNull] IPlayersRepository playersRepository,
            [NotNull] ILogger<BattleGenerator> logger,
            [NotNull] IGameResourcesRegistry resourcesRegistry,
            [NotNull] IUnitTypesRegistry unitTypesRegistry,
            [NotNull] IRewardDefinitionsService rewardDefinitionsService,
            [NotNull] IRewardDefinitionsRegistry rewardDefinitionsRegistry,
            [NotNull] GlobalUnitsForBattleGenerator globalUnitsForBattleGenerator,
            [NotNull] IGameModeProvider gameModeProvider,
            [NotNull] IArtifactTypesRegistry artifactTypesRegistry)
        {
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            BattleDefinitionsRepository = battleDefinitionsRepository ?? throw new ArgumentNullException(nameof(battleDefinitionsRepository));
            GlobalUnitsRepository = globalUnitsRepository ?? throw new ArgumentNullException(nameof(globalUnitsRepository));
            UnitTypesRepository = unitTypesRepository ?? throw new ArgumentNullException(nameof(unitTypesRepository));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            RewardsRepository = rewardsRepository ?? throw new ArgumentNullException(nameof(rewardsRepository));
            GameResourcesRepository = gameResourcesRepository ?? throw new ArgumentNullException(nameof(gameResourcesRepository));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            PlayersRepository = playersRepository ?? throw new ArgumentNullException(nameof(playersRepository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ResourcesRegistry = resourcesRegistry ?? throw new ArgumentNullException(nameof(resourcesRegistry));
            UnitTypesRegistry = unitTypesRegistry ?? throw new ArgumentNullException(nameof(unitTypesRegistry));
            RewardDefinitionsService = rewardDefinitionsService ?? throw new ArgumentNullException(nameof(rewardDefinitionsService));
            RewardDefinitionsRegistry = rewardDefinitionsRegistry ?? throw new ArgumentNullException(nameof(rewardDefinitionsRegistry));
            GlobalUnitsForBattleGenerator = globalUnitsForBattleGenerator ?? throw new ArgumentNullException(nameof(globalUnitsForBattleGenerator));
            GameModeProvider = gameModeProvider ?? throw new ArgumentNullException(nameof(gameModeProvider));
            ArtifactTypesRegistry = artifactTypesRegistry ?? throw new ArgumentNullException(nameof(artifactTypesRegistry));
        }

        public async Task GenerateSingle(Guid playerId, int effectiveDay, int globalDay, int stage)
        {
            var player = await PlayersService.GetById(playerId);
            var gameMode = GameModeProvider.GetGameMode();
            var gameStage = gameMode.Stages[Math.Min(stage, gameMode.Stages.Length - 1)];
            var rewardFactor = gameStage.RewardsFactor;
            
            // Apply bonus reward multiplier for admin player
            if (player.Name == "admin_1_player")
            {
                rewardFactor *= 1.5;
            }
            
            // Check if we should generate a NextStage battle (only on current/highest unlocked stage)
            bool isNextStageBattle = false;
            int fixedDifficulty = 0;
            bool isCurrentStage = stage == player.Stage;
            bool hasNextStageAvailable = stage < gameMode.Stages.Length - 1;
            if (isCurrentStage && hasNextStageAvailable)
            {
                var nextStage = gameMode.Stages[stage + 1];
                if (nextStage.GuardDifficulty > 0 && _random.NextDouble() < 0.07)
                {
                    // Check if player already has an active NextStage battle
                    var existingNextStageBattles = await BattleDefinitionsRepository.GetActiveBattlesDefinitionsWithRewardType(playerId, player.Day, RewardType.NextStage);
                    
                    // Check if player already has not accepted rewards with NextStage type
                    var notAcceptedNextStageRewards = await RewardsRepository.FindNotAcceptedRewardsByPlayerIdAndRewardType(playerId, RewardType.NextStage);
                    
                    // Generate NextStage battle if none exists and no pending NextStage rewards
                    if (existingNextStageBattles.Length == 0 && notAcceptedNextStageRewards.Length == 0)
                    {
                        isNextStageBattle = true;
                        fixedDifficulty = nextStage.GuardDifficulty;
                        Logger.LogInformation($"Generating NextStage battle with fixed difficulty: {fixedDifficulty}");
                    }
                }
            }
            
            var orderedUnitTypes = UnitTypesRegistry.AllOrderedByValue;
            var toTrainOrderedUnitTypes = UnitTypesRegistry.ToTrainOrderedByValue;
            var resources = ResourcesRegistry.GetAll();
            
            // Use effectiveDay for difficulty calculation (frozen until stage is unlocked)
            var difficulty = DifficultyMarker.GenerateFromDay(_random, gameStage, effectiveDay);
            difficulty.TargetDifficulty =  isNextStageBattle ? fixedDifficulty : difficulty.TargetDifficulty;
            
            Logger.LogInformation($"Generated Difficulty stage {stage} effectiveDay {effectiveDay} globalDay {globalDay}: {difficulty.TargetDifficulty}; {difficulty.MinDifficulty}-{difficulty.MaxDifficulty}");

            var maxWidth = Math.Min(BattleConstants.MaxBattleWidth, BattleConstants.StartBattleWidth + difficulty.TargetDifficulty / 300);
            var maxHeight = Math.Min(BattleConstants.MaxBattleHeight, BattleConstants.StartBattleHeight + difficulty.TargetDifficulty / 300);
            
            var width = _random.Next(BattleConstants.MinBattleWidth, maxWidth);
            var height = _random.Next(BattleConstants.MinBattleHeight, maxHeight);
            
            var maxStrongUnitIndex = BinarySearch.FindClosestNotExceedingIndex(orderedUnitTypes,
                entity => entity.Value, difficulty.TargetDifficulty);
            var normalizedMean = 1.0 / 2.25; // Bias toward lower part
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
            if (!isNextStageBattle && difficulty.IdealDifficulty > 1000)
            {
                var rewardVisibilityChance = _random.NextDouble();
                if (rewardVisibilityChance < 0.06)
                    rewardVisibility = -1;
                if (rewardVisibilityChance < 0.02)
                    rewardVisibility = -2;
            }

            var guardVisibility = 0;
            if (!isNextStageBattle && difficulty.IdealDifficulty > 2000)
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
                    globalDay + duration, // Use globalDay for expiration
                    rewardVisibility,
                    guardVisibility,
                    stage,
                    container.Id);

            // For NextStage battles, use NextStage reward type; otherwise, randomly select
            GeneratedRewardTypes rewardType;
            if (isNextStageBattle)
            {
                rewardType = GeneratedRewardTypes.NextStage;
            }
            else
            {
                var availableRewardTypes = Enum.GetValues(typeof(GeneratedRewardTypes))
                    .Cast<int>()
                    .Where(x => x != (int)GeneratedRewardTypes.NextStage)
                    .ToArray();
                var rewardTypeIndex = _random.Next(0, availableRewardTypes.Length);
                rewardType = (GeneratedRewardTypes)availableRewardTypes[rewardTypeIndex];
            }

            if (rewardType == GeneratedRewardTypes.Gold)
            {
                var goldAmount = RoundToFriendlyNumber(difficulty.TargetDifficulty);
                await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                {
                    RewardType = RewardType.ResourcesGain,
                    Amounts = new[] { (int)(goldAmount * rewardFactor) },
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
                    resourcesAmounts.Add((int)(resourceAmount * rewardFactor));
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
                
                // Calculate if the total value is a multiple of TargetDifficulty / 3
                var totalValue = unitsGainAmount * unitToGain.Value;
                var baseThreshold = (double)difficulty.TargetDifficulty / 3;
                var multiplier = 1.0;
                if (baseThreshold > 0 && totalValue > baseThreshold)
                {
                    var calculatedMultiplier = totalValue / baseThreshold;
                    // Only apply multiplier if it's at least 2x
                    if (calculatedMultiplier >= 2.0)
                    {
                        multiplier = (int)calculatedMultiplier;
                    }
                }
                
                await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                {
                    RewardType = RewardType.UnitsGain,
                    Amounts = new[] { (int)(unitsGainAmount * rewardFactor * multiplier) },
                    CanDecline = true,
                    GuardBattleDefinitionId = null,
                    IconUrl = null,
                    Title = null,
                    Ids = new[] { unitToGain.Id },
                });
            }
            else if (rewardType == GeneratedRewardTypes.ArtifactsGain)
            {
                var orderedArtifacts = ArtifactTypesRegistry.AllOrderedByValue;
                if (orderedArtifacts.Count == 0)
                {
                    // fallback to gold if artifacts are not configured
                    var goldAmount = RoundToFriendlyNumber(difficulty.TargetDifficulty);
                    await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                    {
                        RewardType = RewardType.ResourcesGain,
                        Amounts = new[] { (int)(goldAmount * rewardFactor) },
                        CanDecline = true,
                        GuardBattleDefinitionId = null,
                        IconUrl = null,
                        Title = null,
                        Ids = new[] { GameResourcesRepository.GoldResourceId },
                    });
                }
                else
                {
                    var maxArtifactIndex = BinarySearch.FindClosestNotExceedingIndex(orderedArtifacts,
                        x => x.Value, difficulty.TargetDifficulty);
                    
                    // If difficulty is below the minimum artifact value, select from all artifacts 
                    // with the minimum value (instead of always picking the first one)
                    if (maxArtifactIndex < 0)
                    {
                        var minValue = orderedArtifacts[0].Value;
                        maxArtifactIndex = 0;
                        while (maxArtifactIndex < orderedArtifacts.Count - 1 && 
                               orderedArtifacts[maxArtifactIndex + 1].Value == minValue)
                        {
                            maxArtifactIndex++;
                        }
                    }
                    
                    var artifactToGain = orderedArtifacts[_random.Next(0, maxArtifactIndex + 1)];

                    const int artifactAmount = 1;
                    await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                    {
                        RewardType = RewardType.ArtifactsGain,
                        Amounts = new[] { artifactAmount },
                        CanDecline = true,
                        GuardBattleDefinitionId = null,
                        IconUrl = artifactToGain.ThumbnailUrl,
                        Title = artifactToGain.Name,
                        Message = $"You found {artifactToGain.Name}",
                        Ids = new[] { artifactToGain.Id },
                    });

                    var remainingValue = difficulty.TargetDifficulty - artifactToGain.Value * artifactAmount;
                    if (remainingValue > 500)
                    {
                        // Fill remaining value with either gold or resources, similar to Template rewards
                        if (_random.NextDouble() < 0.5)
                        {
                            var goldAmount = RoundToFriendlyNumber(remainingValue);
                            await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                            {
                                RewardType = RewardType.ResourcesGain,
                                Amounts = new[] { (int)(goldAmount * rewardFactor) },
                                CanDecline = true,
                                GuardBattleDefinitionId = null,
                                IconUrl = null,
                                Title = null,
                                Ids = new[] { GameResourcesRepository.GoldResourceId },
                            });
                        }
                        else
                        {
                            var resourcesValue = remainingValue;
                            var resourceTypes = new List<IGameResourceEntity>();
                            var resourcesAmounts = new List<int>();
                            var nonGoldResources = resources.Where(x => x.Id != GameResourcesRepository.GoldResourceId).ToList();
                            var availablePool = nonGoldResources.Any() ? nonGoldResources : resources;
                            var resourceTypesCount = resourcesValue > availablePool.Sum(x => x.Price) / 2
                                ? _random.Next(1, availablePool.Count)
                                : 1;
                            var valuePerResource = (double)resourcesValue / resourceTypesCount;
                            var availableResources = new List<IGameResourceEntity>(availablePool);
                            for (var i = 0; i < resourceTypesCount; i++)
                            {
                                var resourceType = availableResources[_random.Next(0, availableResources.Count)];
                                availableResources.Remove(resourceType);

                                var resourceAmount = Math.Max(1,
                                    (int)Math.Ceiling(valuePerResource / resourceType.Price));
                                resourceAmount = RoundToFriendlyNumber(resourceAmount);

                                resourceTypes.Add(resourceType);
                                resourcesAmounts.Add((int)(resourceAmount * rewardFactor));
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
            }
            else if (rewardType == GeneratedRewardTypes.UnitsToBuy)
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
                    Amounts = isUpgrade ? new[] { 0 } : new[] { (int)(unitToBuy.ToTrainAmount * rewardFactor) },
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
                    
                    var guardBattleDefinition = await BattleDefinitionsService.CreateBattleDefinition(guardBattleWidth, guardBattleHeight, stage);
                    
                    await GlobalUnitsRepository.Create(unitToBuy.Id, Math.Max(1, unitToBuy.ToTrainAmount / 2),
                        guardBattleDefinition.ContainerId, true, guardBattleDefinition.Height / 2);
                    
                    rewardFields.GuardBattleDefinitionId = guardBattleDefinition.Id;
                    rewardFields.GuardMessage = $"You need to defeat guards to train {unitToBuy.Name}";
                }
                
                await RewardsRepository.CreateRewardAsync(rewardedBattleDefinition.Id, rewardFields);
            } else if (rewardType == GeneratedRewardTypes.Template)
            {
                var maxRewardIndex = BinarySearch.FindClosestNotExceedingIndex(RewardDefinitionsRegistry.AllOrdered,
                    group => group.First().Value, difficulty.TargetDifficulty);

                maxRewardIndex = Math.Max(0, maxRewardIndex);
                var maxRewardGroupValue = RewardDefinitionsRegistry.AllOrdered[maxRewardIndex].First().Value;
                for (var i = maxRewardIndex + 1; i < RewardDefinitionsRegistry.AllOrdered.Count; i++)
                {
                    if (RewardDefinitionsRegistry.AllOrdered[i].First().Value <= maxRewardGroupValue)
                        maxRewardIndex++;
                }

                var rewardGroup = RewardDefinitionsRegistry.AllOrdered[_random.Next(maxRewardIndex + 1)];

                var rewardTemplates = rewardGroup.ToArray();
                var variation = _random.Next(rewardTemplates.Length);
                var targetRewardTemplate = rewardTemplates[variation];

                Logger.LogInformation(
                    $"Generating reward template: {targetRewardTemplate.Name}; variant: {variation + 1}");
                ;

                await RewardDefinitionsService.CreateRewardsFromDefinition(targetRewardTemplate, battleDefinition.Id,
                    rewardFactor);

                var remainingValue = difficulty.TargetDifficulty - rewardGroup.First().Value;
                if (remainingValue > 500)
                {
                    var resourcesValue = remainingValue;
                    var resourceTypes = new List<IGameResourceEntity>();
                    var resourcesAmounts = new List<int>();
                    var resourceTypesCount = resourcesValue > resources.Sum(x => x.Price) / 2
                        ? _random.Next(1, resources.Count)
                        : 1;
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
                        resourcesAmounts.Add((int)(resourceAmount * rewardFactor));
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
            } else if (rewardType == GeneratedRewardTypes.NextStage)
            {
                await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                {
                    RewardType = RewardType.NextStage,
                    Amounts = Array.Empty<int>(),
                    CanDecline = false,
                    GuardBattleDefinitionId = null,
                    IconUrl = null,
                    Title = "Stage Advancement",
                    Message = "Advance to the next stage",
                });
            }
        }

        public async Task Generate(Guid playerId, int effectiveDay, int globalDay, int currentBattlesCount, int stage)
        {
            var gameMode = GameModeProvider.GetGameMode();
            var gameStage = gameMode.Stages[Math.Min(stage, gameMode.Stages.Length - 1)];
            
            // Use effectiveDay for battles count factor
            int count = currentBattlesCount <= Math.Max(7, effectiveDay * gameStage.BattlesCountFactor) ? _random.Next(3, 7) : _random.Next(1, 4);
            for (int i = 0; i < count; i++)
            {
                await GenerateSingle(playerId, effectiveDay, globalDay, stage);
            }
        }

        public async Task GenerateTestBattle(Guid playerId, int width, int height, IReadOnlyList<(string unitKey, int count)> enemies)
        {
            var container = await UnitsContainersService.Create(height, Guid.Empty);
            
            // Add enemy units
            int slot = 0;
            foreach (var (unitKey, count) in enemies)
            {
                var unitType = await UnitTypesRepository.GetByName(unitKey);
                if (unitType != null)
                {
                    await GlobalUnitsRepository.Create(unitType.Id, count, container.Id, true, slot);
                    slot++;
                }
                else
                {
                    Logger.LogWarning($"Unit type not found: {unitKey}");
                }
            }
            
            // Create battle definition with no expiration (max day)
            await BattleDefinitionsService.CreateBattleDefinition(
                playerId,
                width,
                height,
                int.MaxValue, // Never expires
                0, // Full reward visibility
                0, // Full guard visibility
                0, // Stage 0
                container.Id);
            
            Logger.LogInformation($"Generated test battle for player {playerId}: {width}x{height} with {enemies.Count} enemy types");
        }

        internal enum GeneratedRewardTypes
        {
            Gold, 
            Resource,
            UnitsGain,
            ArtifactsGain,
            UnitsToBuy,
            Template,
            NextStage,
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