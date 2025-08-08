using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.UnitsContainers;
using Epic.Data.GameResources;
using Epic.Data.GlobalUnits;
using Epic.Data.Reward;
using Epic.Data.UnitTypes;
using JetBrains.Annotations;

namespace Epic.Logic
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

        private readonly Random _random = new Random();

        private readonly List<IUnitTypeEntity> _orderedUnitTypes = new List<IUnitTypeEntity>();
        private readonly List<IGameResourceEntity> _resources = new List<IGameResourceEntity>();
        
        public BattleGenerator(
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IGlobalUnitsRepository globalUnitsRepository,
            [NotNull] IUnitTypesRepository unitTypesRepository,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IRewardsRepository rewardsRepository,
            [NotNull] IGameResourcesRepository gameResourcesRepository)
        {
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            GlobalUnitsRepository = globalUnitsRepository ?? throw new ArgumentNullException(nameof(globalUnitsRepository));
            UnitTypesRepository = unitTypesRepository ?? throw new ArgumentNullException(nameof(unitTypesRepository));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            RewardsRepository = rewardsRepository ?? throw new ArgumentNullException(nameof(rewardsRepository));
            GameResourcesRepository = gameResourcesRepository ?? throw new ArgumentNullException(nameof(gameResourcesRepository));
        }

        public async Task Initialize()
        {
            _orderedUnitTypes.Clear();

            var allUnits = await UnitTypesRepository.GetAll();
            _orderedUnitTypes.AddRange(allUnits);
            
            _orderedUnitTypes.Sort((x, y) => x.Value.CompareTo(y.Value));
            
            _resources.Clear();
            var resourcesByKeys = await GameResourcesRepository.GetAllResourcesByKeys();
            _resources.AddRange(resourcesByKeys.Values);
        }

        public async Task GenerateSingle(Guid playerId, int day)
        {
            var difficulty = DifficultyMarker.GenerateFromDay(_random, day);

            var maxWidth = Math.Min(Constants.MaxBattleWidth, Constants.StartBattleWidth + difficulty.TargetDifficulty / 500);
            var maxHeight = Math.Min(Constants.MaxBattleHeight, Constants.StartBattleHeight + difficulty.TargetDifficulty / 500);
            
            var width = _random.Next(Constants.MinBattleWidth, maxWidth);
            var height = _random.Next(Constants.MinBattleHeight, maxHeight);
            
            var maxStrongUnitIndex = BinarySearch.FindClosestNotExceedingIndex(_orderedUnitTypes,
                entity => entity.Value, difficulty.TargetDifficulty);
            var targetUnit = _orderedUnitTypes[_random.Next(0, maxStrongUnitIndex + 1)];

            var unitsCount = Math.Max(1, (int)Math.Round((double)difficulty.TargetDifficulty / targetUnit.Value));

            var container = await UnitsContainersService.Create(height, Guid.Empty);

            var maxSlotsCount = Math.Min(unitsCount, height);
            var targetSlotsCount = _random.Next(1, maxSlotsCount + 1);

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
            int duration = Math.Max(1, (int)Math.Round(1 + t * 9) + _random.Next(-2, 3));

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
                    Amounts = new[] { goldAmount },
                    CanDecline = true,
                    NextBattleDefinitionId = null,
                    CustomIconUrl = null,
                    CustomTitle = null,
                    Ids = new[] { GameResourcesRepository.GoldResourceId },
                });
            }
            else if (rewardType == RewardTypes.Resource)
            {
                var resourceType = _resources[_random.Next(0, _resources.Count)];
                var resourceAmount = Math.Max(1,
                    (int)Math.Ceiling((double)difficulty.TargetDifficulty / resourceType.Price));
                resourceAmount = RoundToFriendlyNumber(resourceAmount);
                await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                {
                    RewardType = RewardType.ResourcesGain,
                    Amounts = new[] { resourceAmount },
                    CanDecline = true,
                    NextBattleDefinitionId = null,
                    CustomIconUrl = null,
                    CustomTitle = null,
                    Ids = new[] { resourceType.Id },
                });
            }
            else if (rewardType == RewardTypes.UnitsGain)
            {
                var maxUnitIndex = BinarySearch.FindClosestNotExceedingIndex(_orderedUnitTypes,
                    entity => entity.Value, difficulty.TargetDifficulty);
                var unitToGain = _orderedUnitTypes[_random.Next(0, maxUnitIndex + 1)];
                var unitsGainAmount = Math.Max(1, (int)Math.Floor((double)difficulty.TargetDifficulty / unitToGain.Value));
                
                await RewardsRepository.CreateRewardAsync(battleDefinition.Id, new MutableRewardFields
                {
                    RewardType = RewardType.UnitsGain,
                    Amounts = new[] { unitsGainAmount },
                    CanDecline = true,
                    NextBattleDefinitionId = null,
                    CustomIconUrl = null,
                    CustomTitle = null,
                    Ids = new[] { unitToGain.Id },
                });
            } else if (rewardType == RewardTypes.UnitsToBuy)
            {
                var minUnitIndex = BinarySearch.FindClosestNotExceedingIndex(_orderedUnitTypes,
                    entity => entity.Value, difficulty.TargetDifficulty / 2);
                var maxUnitIndex = BinarySearch.FindClosestNotExceedingIndex(_orderedUnitTypes,
                    entity => entity.Value, difficulty.TargetDifficulty * 3);
                var unitToBuy = _orderedUnitTypes[_random.Next(minUnitIndex, maxUnitIndex + 1)];
                var dwellingIcon = string.IsNullOrWhiteSpace(unitToBuy.DwellingImgUrl) 
                    ? unitToBuy.BattleImgUrl 
                    : unitToBuy.DwellingImgUrl;
                    
                var isGuarded = unitToBuy.Value >= 300;
                var rewardedBattleDefinition = battleDefinition;
                if (isGuarded)
                {
                    var guardBattleWidth = Math.Min(Constants.MaxBattleWidth, Constants.StartBattleWidth + unitToBuy.Value * 3 / 500);
                    var guardBattleHeight = Math.Min(Constants.MaxBattleHeight, Constants.StartBattleHeight + unitToBuy.Value * 3 / 500);
                    
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
                    
                    await GlobalUnitsRepository.Create(unitToBuy.Id, unitToBuy.ToTrainAmount * 3,
                        guardBattleDefinition.ContainerId, true, guardBattleDefinition.Height / 2);
                    
                    rewardedBattleDefinition = guardBattleDefinition;
                }
                
                await RewardsRepository.CreateRewardAsync(rewardedBattleDefinition.Id, new MutableRewardFields
                {
                    RewardType = RewardType.UnitToBuy,
                    Amounts = new[] { unitToBuy.ToTrainAmount },
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
            int count = _random.Next(1, 4);
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