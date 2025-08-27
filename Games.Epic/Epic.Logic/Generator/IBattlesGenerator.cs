using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Players;
using Epic.Core.Services.UnitsContainers;
using Epic.Data.GameResources;
using Epic.Data.GlobalUnits;
using Epic.Data.Reward;
using Epic.Data.UnitTypes;
using Epic.Logic.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NetExtensions.Collections;

namespace Epic.Logic.Generator
{
    public interface IBattlesGenerator
    {
        Task Initialize();
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

        private readonly Random _random = new Random();

        private readonly Dictionary<Guid, IUnitTypeEntity> _unitTypesByIds = new Dictionary<Guid, IUnitTypeEntity>();
        private readonly List<IUnitTypeEntity> _orderedUnitTypes = new List<IUnitTypeEntity>();
        private readonly List<IUnitTypeEntity> _orderedUnitTypesToTrain = new List<IUnitTypeEntity>();
        private readonly List<IGameResourceEntity> _resources = new List<IGameResourceEntity>();
        
        public BattleGenerator(
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IGlobalUnitsRepository globalUnitsRepository,
            [NotNull] IUnitTypesRepository unitTypesRepository,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IRewardsRepository rewardsRepository,
            [NotNull] IGameResourcesRepository gameResourcesRepository,
            [NotNull] IPlayersService playersService,
            [NotNull] ILogger<BattleGenerator> logger)
        {
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            GlobalUnitsRepository = globalUnitsRepository ?? throw new ArgumentNullException(nameof(globalUnitsRepository));
            UnitTypesRepository = unitTypesRepository ?? throw new ArgumentNullException(nameof(unitTypesRepository));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            RewardsRepository = rewardsRepository ?? throw new ArgumentNullException(nameof(rewardsRepository));
            GameResourcesRepository = gameResourcesRepository ?? throw new ArgumentNullException(nameof(gameResourcesRepository));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Initialize()
        {
            _orderedUnitTypes.Clear();

            var allUnits = await UnitTypesRepository.GetAll();
            _orderedUnitTypes.AddRange(allUnits);
            allUnits.ForEach(x => _unitTypesByIds.Add(x.Id, x));
            
            _orderedUnitTypes.Sort((x, y) => x.Value.CompareTo(y.Value));
            _orderedUnitTypesToTrain.AddRange(_orderedUnitTypes.Where(x => x.ToTrainAmount > 0));
            
            _resources.Clear();
            var resourcesByKeys = await GameResourcesRepository.GetAllResourcesByKeys();
            _resources.AddRange(resourcesByKeys.Values);
        }

        private enum SlotsDistributionPattern
        {
            Single,
            Few,
            Partially,
            Full,
        } 

        public async Task GenerateSingle(Guid playerId, int day)
        {
            var rewardFactor = 1;
            
            var player = await PlayersService.GetById(playerId);
            var difficulty = DifficultyMarker.GenerateFromDay(_random, day);
            
            Logger.LogInformation($"Generated Difficulty day {day}: {difficulty.TargetDifficulty}; {difficulty.MinDifficulty}-{difficulty.MaxDifficulty}");

            var maxWidth = Math.Min(BattleConstants.MaxBattleWidth, BattleConstants.StartBattleWidth + difficulty.TargetDifficulty / 300);
            var maxHeight = Math.Min(BattleConstants.MaxBattleHeight, BattleConstants.StartBattleHeight + difficulty.TargetDifficulty / 300);
            
            var width = _random.Next(BattleConstants.MinBattleWidth, maxWidth);
            var height = _random.Next(BattleConstants.MinBattleHeight, maxHeight);
            
            var maxStrongUnitIndex = BinarySearch.FindClosestNotExceedingIndex(_orderedUnitTypes,
                entity => entity.Value, difficulty.TargetDifficulty);
            var normalizedMean = 1.0 / 3.0; // Bias toward lower part
            var stdDev = 0.25; // how chaotic the output
            var sample = RandomDistributions.GetBoundedNormal(_random, normalizedMean, stdDev, 0, 1);
            var targetIndex = (int)(sample * maxStrongUnitIndex);
            var targetUnit = _orderedUnitTypes[targetIndex];

            var unitsCount = Math.Max(1, (int)Math.Round((double)difficulty.TargetDifficulty / targetUnit.Value));

            var container = await UnitsContainersService.Create(height, Guid.Empty);

            var slotsDistribution = (SlotsDistributionPattern)_random.Next(0, 4);

            var maxSlotsCount = int.MaxValue;
            var minSlotsCount = 1;
            switch (slotsDistribution)
            {
                case SlotsDistributionPattern.Single:
                    maxSlotsCount = 1;
                    maxSlotsCount = 1;
                    break;
                case SlotsDistributionPattern.Few:
                    minSlotsCount = 2;
                    maxSlotsCount = 3;
                    break;
                case SlotsDistributionPattern.Partially:
                    minSlotsCount = 4;
                    maxSlotsCount = 6;
                    break;
                case SlotsDistributionPattern.Full:
                    minSlotsCount = int.MaxValue;
                    maxSlotsCount = int.MaxValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var slotsCountLimit = Math.Min(unitsCount, height);
            maxSlotsCount = Math.Min(maxSlotsCount, slotsCountLimit);
            minSlotsCount = Math.Min(minSlotsCount, slotsCountLimit);
            
            var targetSlotsCount = _random.Next(minSlotsCount, maxSlotsCount + 1);

            // 1. Create the unit distribution
            var slotDistributions = new List<int>();
            var baseUnitsPerSlot = unitsCount / targetSlotsCount;
            var extraUnits = unitsCount % targetSlotsCount;
            for (var i = 0; i < targetSlotsCount; i++)
            {
                // Distribute one of the extras to the first few slots
                var unitsInSlot = baseUnitsPerSlot + (i < extraUnits ? 1 : 0);
                slotDistributions.Add(unitsInSlot);
            }

            // 2. Spread filled slots evenly across the container height
            List<int> slotIndices = new List<int>();
            for (int i = 0; i < targetSlotsCount; i++)
            {
                if (targetSlotsCount == 1)
                {
                    // Just place the single slot in the middle
                    slotIndices.Add(height / 2);
                }
                else
                {
                    int slotIndex = (int)Math.Round(i * (height - 1.0) / (targetSlotsCount - 1));
                    slotIndices.Add(slotIndex);
                }
            }

            // Optional: Make sure indices are unique (in case of rounding)
            slotIndices = slotIndices.Distinct().ToList();

            for (int i = 0; i < slotIndices.Count && i < slotDistributions.Count; i++)
            {
                await GlobalUnitsRepository.Create(targetUnit.Id, slotDistributions[i], container.Id, true,
                    slotIndices[i]);
            }

            double t = ((double)difficulty.TargetDifficulty - difficulty.MinDifficulty) /
                       ((double)difficulty.MaxDifficulty - difficulty.MinDifficulty);
            int duration = Math.Max(1, (int)Math.Round(1 + t * 12) + _random.Next(-2, 3));

            var rewardVisibility = 0;
            if (difficulty.IdealDifficulty > 1000)
            {
                var rewardVisibilityChance = _random.Next(101);
                if (rewardVisibilityChance < 10)
                    rewardVisibility = -1;
                if (rewardVisibilityChance < 3)
                    rewardVisibility = -2;
            }

            var guardVisibility = 0;
            if (difficulty.IdealDifficulty > 2000)
            {
                var guardVisibilityChance = _random.Next(101);
                if (guardVisibilityChance < 10)
                    guardVisibility = -1;
                if (guardVisibilityChance < 3)
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


            var rewardTypeIndex = _random.Next(0, Enum.GetValues(typeof(RewardTypes))
                .Cast<int>()
                .Max() + 1);

            var rewardType = (RewardTypes)rewardTypeIndex;

            if (rewardType == RewardTypes.Gold)
            {
                var goldAmount = RoundToFriendlyNumber(difficulty.TargetDifficulty);
                await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                {
                    RewardType = RewardType.ResourcesGain,
                    Amounts = new[] { goldAmount * rewardFactor },
                    CanDecline = true,
                    NextBattleDefinitionId = null,
                    CustomIconUrl = null,
                    CustomTitle = null,
                    Ids = new[] { GameResourcesRepository.GoldResourceId },
                });
            }
            else if (rewardType == RewardTypes.Resource)
            {
                var resourcesValue = difficulty.TargetDifficulty;
                var resourceTypes = new List<IGameResourceEntity>();
                var resourcesAmounts = new List<int>();
                var resourceTypesCount = resourcesValue > _resources.Sum(x => x.Price) / 2
                    ? _random.Next(1, _resources.Count) : 1;
                var valuePerResource = (double)resourcesValue / resourceTypesCount;
                for (var i = 0; i < resourceTypesCount; i++)
                {
                    var availableResources = new List<IGameResourceEntity>(_resources); 
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
                    NextBattleDefinitionId = null,
                    CustomIconUrl = null,
                    CustomTitle = null,
                    Ids = resourceTypes.Select(x => x.Id).ToArray(),
                });
            }
            else if (rewardType == RewardTypes.UnitsGain)
            {
                var maxUnitIndex = BinarySearch.FindClosestNotExceedingIndex(_orderedUnitTypes,
                    entity => entity.Value, difficulty.TargetDifficulty);
                var unitToGain = _orderedUnitTypes[_random.Next(0, maxUnitIndex + 1)];
                var unitsGainAmount = Math.Max(1, (int)Math.Floor(((double)difficulty.TargetDifficulty / 2) / unitToGain.Value));
                
                var supplyUnits = await GlobalUnitsRepository.GetAliveByContainerId(player.SupplyContainerId);
                var armyUnits = await GlobalUnitsRepository.GetAliveByContainerId(player.ActiveHero.ArmyContainerId);
                var desiredUnits = supplyUnits.Concat(armyUnits)
                    .Select(x => x.TypeId)
                    .Distinct()
                    .Select(id => _unitTypesByIds[id]);

                var availableDesiredUnits = desiredUnits.Where(x => x.Value <= difficulty.TargetDifficulty).ToList();
                if (availableDesiredUnits.Any() && _random.Next(100) > 66)
                {
                    unitToGain = availableDesiredUnits[_random.Next(0, availableDesiredUnits.Count)];
                    unitsGainAmount = Math.Max(1, (int)Math.Floor(((double)difficulty.TargetDifficulty / 3) / unitToGain.Value));
                }
                
                await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                {
                    RewardType = RewardType.UnitsGain,
                    Amounts = new[] { unitsGainAmount * rewardFactor },
                    CanDecline = true,
                    NextBattleDefinitionId = null,
                    CustomIconUrl = null,
                    CustomTitle = null,
                    Ids = new[] { unitToGain.Id },
                });
            } else if (rewardType == RewardTypes.UnitsToBuy)
            {
                var maxUnitIndex = BinarySearch.FindClosestNotExceedingIndex(_orderedUnitTypesToTrain,
                    entity => entity.Value, (int)(difficulty.TargetDifficulty * 1.5));
                var unitToBuy = _orderedUnitTypesToTrain[_random.Next(maxUnitIndex + 1)];
                
                var supplyUnits = await GlobalUnitsRepository.GetAliveByContainerId(player.SupplyContainerId);
                var armyUnits = await GlobalUnitsRepository.GetAliveByContainerId(player.ActiveHero.ArmyContainerId);
                var desiredUnits = supplyUnits.Concat(armyUnits)
                    .Select(x => x.TypeId)
                    .Distinct()
                    .Select(id => _unitTypesByIds[id]);

                var availableDesiredUnits = desiredUnits.Where(x => x.Value <= difficulty.TargetDifficulty).ToList();
                if (availableDesiredUnits.Any() && _random.Next(100) > 66)
                    unitToBuy = availableDesiredUnits[_random.Next(availableDesiredUnits.Count)];


                var dwellingIcon = string.IsNullOrWhiteSpace(unitToBuy.DwellingImgUrl) 
                    ? unitToBuy.BattleImgUrl 
                    : unitToBuy.DwellingImgUrl;
                    
                var isGuarded = unitToBuy.Value >= 400;
                var rewardedBattleDefinition = battleDefinition;
                if (isGuarded)
                {
                    var guardBattleWidth = Math.Min(BattleConstants.MaxBattleWidth, BattleConstants.StartBattleWidth + (unitToBuy.Value * unitToBuy.ToTrainAmount / 2) / 300);
                    var guardBattleHeight = Math.Min(BattleConstants.MaxBattleHeight, BattleConstants.StartBattleHeight + (unitToBuy.Value * unitToBuy.ToTrainAmount / 2) / 300);
                    
                    var guardBattleDefinition = await BattleDefinitionsService.CreateBattleDefinition(guardBattleWidth, guardBattleHeight);
                    await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                    {
                        RewardType = RewardType.Battle,
                        Ids = Array.Empty<Guid>(),
                        Amounts = Array.Empty<int>(),
                        Message = $"You need to defeat guards to train {unitToBuy.Name}",
                        CanDecline = true,
                        NextBattleDefinitionId = guardBattleDefinition.Id,
                        CustomIconUrl = dwellingIcon,
                        CustomTitle = $"Dwelling of {unitToBuy.Name}",
                    });
                    
                    await GlobalUnitsRepository.Create(unitToBuy.Id, unitToBuy.ToTrainAmount / 2,
                        guardBattleDefinition.ContainerId, true, guardBattleDefinition.Height / 2);
                    
                    rewardedBattleDefinition = guardBattleDefinition;
                }
                
                await RewardsRepository.CreateRewardAsync(rewardedBattleDefinition.Id, new MutableRewardFields
                {
                    RewardType = RewardType.UnitToBuy,
                    Amounts = new[] { unitToBuy.ToTrainAmount * rewardFactor },
                    Message = "You can train units now",
                    CanDecline = true,
                    NextBattleDefinitionId = null,
                    CustomIconUrl = dwellingIcon,
                    CustomTitle = $"Dwelling of {unitToBuy.Name}",
                    Ids = new[] { unitToBuy.Id },
                });
            }
        }

        public async Task Generate(Guid playerId, int day, int currentBattlesCount)
        {
            int count = currentBattlesCount < 5 ? _random.Next(3, 7) : _random.Next(1, 4);
            for (int i = 0; i < count; i++)
            {
                await GenerateSingle(playerId, day);
            }
        }

        internal enum RewardTypes
        {
            Gold, 
            Resource,
            UnitsGain,
            UnitsToBuy,
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