using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Logic;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.Players;
using Epic.Core.Services.UnitsContainers;
using Epic.Core.Services.Users;
using Epic.Data;
using Epic.Data.BattleDefinitions;
using Epic.Data.GameResources;
using Epic.Data.GlobalUnits;
using Epic.Data.Reward;
using Epic.Data.UnitTypes;
using Epic.Logic;
using Epic.Logic.Generator;
using Epic.Server.Authentication;
using FrameworkSDK;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Server
{
    public class DebugStartupScript : IAppComponent
    {
        [NotNull] public IUsersService UsersService { get; }
        [NotNull] public IGlobalUnitsRepository GlobalUnitsRepository { get; }
        public IRewardsRepository RewardsRepository { get; }
        public IPlayersService PlayersService { get; }
        public IUnitsContainersService UnitsContainersService { get; }
        public IBattleDefinitionsService BattleDefinitionsService { get; }
        public IHeroesService HeroesService { get; }
        public IGameResourcesRepository ResourcesRepository { get; }
        public IBattlesGenerator BattlesGenerator { get; }
        [NotNull] public IUsersRepository UsersRepository { get; }
        [NotNull] public ISessionsRepository SessionsRepository { get; }
        public IBattleDefinitionsRepository BattleDefinitionsRepository { get; }
        [NotNull] public IUnitTypesRepository UnitTypesRepository { get; set; }
        [NotNull] public IGameModeProvider GameModeProvider { get; }
        
        public DebugStartupScript(
            [NotNull] IUsersRepository usersRepository,
            [NotNull] ISessionsRepository sessionsRepository,
            [NotNull] IBattleDefinitionsRepository battleDefinitionsRepository,
            [NotNull] IUnitTypesRepository unitTypesRepository,
            [NotNull] IUsersService usersService,
            [NotNull] IGlobalUnitsRepository globalUnitsRepository,
            [NotNull] IRewardsRepository rewardsRepository,
            [NotNull] IPlayersService playersService,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IHeroesService heroesService,
            [NotNull] IGameResourcesRepository resourcesRepository,
            [NotNull] IBattlesGenerator battlesGenerator,
            [NotNull] IGameModeProvider gameModeProvider)
        {
            UsersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            GlobalUnitsRepository = globalUnitsRepository ?? throw new ArgumentNullException(nameof(globalUnitsRepository));
            RewardsRepository = rewardsRepository ?? throw new ArgumentNullException(nameof(rewardsRepository));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            HeroesService = heroesService ?? throw new ArgumentNullException(nameof(heroesService));
            ResourcesRepository = resourcesRepository ?? throw new ArgumentNullException(nameof(resourcesRepository));
            BattlesGenerator = battlesGenerator ?? throw new ArgumentNullException(nameof(battlesGenerator));
            UsersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            SessionsRepository = sessionsRepository ?? throw new ArgumentNullException(nameof(sessionsRepository));
            BattleDefinitionsRepository = battleDefinitionsRepository ?? throw new ArgumentNullException(nameof(battleDefinitionsRepository));
            UnitTypesRepository = unitTypesRepository ?? throw new ArgumentNullException(nameof(unitTypesRepository));
            GameModeProvider = gameModeProvider ?? throw new ArgumentNullException(nameof(gameModeProvider));
        }
        
        public void Dispose()
        {
        }

        public async void Configure()
        {
            // Get initial army score from active game mode (already loaded in GameModeProvider)
            var gameMode = GameModeProvider.GetGameMode();
            var targetScore = gameMode.InitialArmyScore;
            
            var user = await UsersRepository.CreateUserAsync("admin",
                BasicAuthentication.GetHashFromCredentials("admin", "123"));
            var user1 = await UsersRepository.CreateUserAsync("admin1",
                BasicAuthentication.GetHashFromCredentials("admin1", "123"));
            await SessionsRepository.CreateSessionAsync("test_token", user.Id, new SessionData());
            await SessionsRepository.CreateSessionAsync("test_token", user1.Id, new SessionData());
            
            var userPlayer = await PlayersService.CreatePlayer(user.Id, "admin_player", PlayerObjectType.Human);
            var user1Player = await PlayersService.CreatePlayer(user1.Id, "admin_1_player", PlayerObjectType.Human);

            await ResourcesRepository.GiveResource(ResourcesRepository.GoldResourceId, userPlayer.Id, 500);
            await ResourcesRepository.GiveResource(ResourcesRepository.GoldResourceId, user1Player.Id, 500);
            // var resourcesByKeys = await ResourcesRepository.GetAllResourcesByKeys();
            // await Task.WhenAll(resourcesByKeys.Values.Select(async x =>
            // {
            //     if (x.Id != ResourcesRepository.GoldResourceId)
            //     {
            //         await ResourcesRepository.GiveResource(x.Id, userPlayer.Id, 1);
            //         await ResourcesRepository.GiveResource(x.Id, user1Player.Id, 1);
            //     }
            // }));
            
            var hero = await HeroesService.CreateNew(userPlayer.Name, userPlayer.Id);
            await PlayersService.SetActiveHero(userPlayer.Id, hero.Id);
            
            var hero1 = await HeroesService.CreateNew(user1Player.Name, user1Player.Id);
            await PlayersService.SetActiveHero(user1Player.Id, hero1.Id);
            
            // Generate random presets for both heroes
            var preset1 = await GenerateRandomPreset(targetScore);
            var preset2 = await GenerateRandomPreset(targetScore);
            
            int slot = 0;
            foreach (var (unitType, count) in preset1)
            {
                await GlobalUnitsRepository.Create(unitType.Id, count, hero.ArmyContainerId, true, slot);
                slot++;
            }
            
            slot = 0;
            foreach (var (unitType, count) in preset2)
            {
                await GlobalUnitsRepository.Create(unitType.Id, count, hero1.ArmyContainerId, true, slot);
                slot++;
            }

            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
        }

        private async Task<List<(IUnitTypeEntity UnitType, int Count)>> GenerateRandomPreset(int targetScore)
        {
            var allUnits = await UnitTypesRepository.GetAll();
            
            // Filter to only trainable units (exclude upgrades)
            var trainableUnits = allUnits.Where(u => u.ToTrainAmount > 0).ToList();
            
            // Group units by fraction
            var unitsByFraction = GroupUnitsByFraction(trainableUnits);
            
            // Get available fractions
            var fractions = unitsByFraction.Keys.ToList();
            if (fractions.Count == 0)
            {
                throw new InvalidOperationException("No units found in any fraction");
            }
            
            var random = new Random();
            var preset = new List<(IUnitTypeEntity UnitType, int Count)>();
            
            // Select a single random fraction - all units will be from this fraction
            var selectedFraction = fractions[random.Next(fractions.Count)];
            var fractionUnits = unitsByFraction[selectedFraction];
            
            if (fractionUnits.Count == 0)
            {
                throw new InvalidOperationException($"No units found in fraction: {selectedFraction}");
            }
            
            var currentScore = 0;
            
            // Allow some tolerance in score (within 10% of target)
            const double tolerance = 0.1;
            var minScore = (int)(targetScore * (1 - tolerance));
            var maxScore = (int)(targetScore * (1 + tolerance));
            
            // Sort units by Value (ascending) - start with lower tier units first
            var sortedUnits = fractionUnits.OrderBy(u => u.Value).ToList();
                
            // Fill army starting from lowest tier units, progressing to higher tiers
            // If higher tier units can't fit, go back to lower tiers to fill remaining space
            while (currentScore < minScore)
            {
                var addedAnyUnit = false;
                    
                foreach (var unit in sortedUnits)
                {
                    var remainingScore = maxScore - currentScore;
                    if (remainingScore < unit.Value)
                        continue; // Can't fit even one of this unit, move to next
                    
                    // Calculate how many of this unit we can add
                    var maxCount = Math.Min(5, remainingScore / unit.Value);
                    if (maxCount < 1)
                        continue;
                    
                    // Add 1 to maxCount units of this type
                    var count = random.Next(1, maxCount + 1);
                    var addedScore = unit.Value * count;
                    
                    if (currentScore + addedScore <= maxScore)
                    {
                        // Check if this unit type already exists in the preset
                        var existingIndex = preset.FindIndex(p => p.UnitType.Id == unit.Id);
                        if (existingIndex >= 0)
                        {
                            // Combine with existing entry - update count
                            var existingEntry = preset[existingIndex];
                            preset[existingIndex] = (existingEntry.UnitType, existingEntry.Count + count);
                        }
                        else
                        {
                            // Add new entry
                        preset.Add((unit, count));
                        }
                        
                        currentScore += addedScore;
                        addedAnyUnit = true;
                    }
                }
                
                // If we couldn't add any unit in this iteration, break to avoid infinite loop
                if (!addedAnyUnit)
                    break;
            }
            
            return preset;
        }
        
        private Dictionary<string, List<IUnitTypeEntity>> GroupUnitsByFraction(List<IUnitTypeEntity> units)
        {
            var fractionMapping = GetFractionMapping();
            var result = new Dictionary<string, List<IUnitTypeEntity>>();
            
            foreach (var unit in units)
            {
                var fraction = GetUnitFraction(unit.Key, fractionMapping);
                if (fraction == null)
                    continue; // Skip units that don't belong to any known fraction (only tiers 1-3 are in the mapping)
                
                if (!result.ContainsKey(fraction))
                {
                    result[fraction] = new List<IUnitTypeEntity>();
                }
                
                result[fraction].Add(unit);
            }
            
            return result;
        }
        
        private Dictionary<string, string> GetFractionMapping()
        {
            // Map unit keys to their fractions - only tiers 1-3 are included
            return new Dictionary<string, string>
            {
                // Castle (tiers 1-3)
                { "Pikeman", "Castle" }, { "Archer", "Castle" }, { "Griffin", "Castle" },
                
                // Necropolis (tiers 1-3)
                { "Skeleton", "Necropolis" }, { "WalkingDead", "Necropolis" }, { "Wight", "Necropolis" },
                
                // Rampart (tiers 1-3)
                { "Centaur", "Rampart" }, { "Dwarf", "Rampart" }, { "WoodElf", "Rampart" },
                
                // Tower (tiers 1-3)
                { "Gremlin", "Tower" }, { "StoneGargoyle", "Tower" }, { "StoneGolem", "Tower" },
                
                // Inferno (tiers 1-3)
                { "Imp", "Inferno" }, { "Gog", "Inferno" }, { "HellHound", "Inferno" },
                
                // Dungeon (tiers 1-3)
                { "Troglodyte", "Dungeon" }, { "Harpy", "Dungeon" }, { "Beholder", "Dungeon" },
                
                // Stronghold (tiers 1-3)
                { "Goblin", "Stronghold" }, { "WolfRider", "Stronghold" }, { "Orc", "Stronghold" },
                
                // Fortress (tiers 1-3)
                { "Gnoll", "Fortress" }, { "Lizardman", "Fortress" }, { "SerpentFly", "Fortress" },
                
                // Conflux (tiers 1-3)
                { "Pixie", "Conflux" }, { "AirElemental", "Conflux" }, { "WaterElemental", "Conflux" },
                
                // Cove (tiers 1-3)
                { "Nymph", "Cove" }, { "CrewMate", "Cove" }, { "Pirate", "Cove" },
                
                // Factory (tiers 1-3)
                { "Halfling", "Factory" }, { "Mechanic", "Factory" }, { "Armadillo", "Factory" },
                
                // Bulwark (tiers 1-3)
                { "Kobold", "Bulwark" }, { "Argali", "Bulwark" }, { "SnowElf", "Bulwark" }
            };
        }
        
        private string GetUnitFraction(string unitKey, Dictionary<string, string> fractionMapping)
        {
            return fractionMapping.TryGetValue(unitKey, out var fraction) ? fraction : null;
        }
        

        private class SessionData : ISessionData
        {
            public DateTime Created { get; } = DateTime.Now;
            public DateTime LastAccessed { get; } = DateTime.Now;
            public DateTime? RevokedDate { get; }  = null;
            public bool IsRevoked { get; } = false;
            public string RevokedReason { get; } = null;
            public string DeviceInfo { get; } = null;
            public string IpAddress { get; } = null;
            public string UserAgent { get; } = null;
        }
    }
}