using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.UnitsContainers;
using Epic.Data.BattleDefinitions;
using Epic.Data.PlayerUnits;
using Epic.Data.UnitTypes;
using JetBrains.Annotations;

namespace Epic.Logic
{
    public interface IBattlesGenerator
    {
        Task Generate(Guid playerId, Guid npcPlayerId, int day, int currentBattlesCount);
    }

    [UsedImplicitly]
    public class BattleGenerator : IBattlesGenerator
    {
        public IBattleDefinitionsRepository BattleDefinitionsRepository { get; }
        public IPlayerUnitsRepository PlayerUnitsRepository { get; }
        public IUnitTypesRepository UnitTypesRepository { get; }
        public IUnitsContainersService UnitsContainersService { get; }

        private readonly Random _random = new Random();

        public BattleGenerator(
            [NotNull] IBattleDefinitionsRepository battleDefinitionsRepository,
            [NotNull] IPlayerUnitsRepository playerUnitsRepository,
            [NotNull] IUnitTypesRepository unitTypesRepository,
            [NotNull] IUnitsContainersService unitsContainersService)
        {
            BattleDefinitionsRepository = battleDefinitionsRepository ?? throw new ArgumentNullException(nameof(battleDefinitionsRepository));
            PlayerUnitsRepository = playerUnitsRepository ?? throw new ArgumentNullException(nameof(playerUnitsRepository));
            UnitTypesRepository = unitTypesRepository ?? throw new ArgumentNullException(nameof(unitTypesRepository));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
        }
        
        public async Task Generate(Guid playerId, Guid npcPlayerId, int day, int currentBattlesCount)
        {
            var unitTypes = await UnitTypesRepository.GetAll();
            var container = await UnitsContainersService.Create(30);
            
            var unitsCount = _random.Next(1, 4);
            for (var i = 0; i < unitsCount; i++)
            {
                var unitType = unitTypes[_random.Next(unitTypes.Length)];
                var count = _random.Next(1, day * 20);
                
                await PlayerUnitsRepository.CreatePlayerUnit(unitType.Id, count, npcPlayerId, container.Id, true);
            }
            
            await BattleDefinitionsRepository.Create(playerId, _random.Next(5, 16), _random.Next(5, 16), container.Id);
        }
    }
}