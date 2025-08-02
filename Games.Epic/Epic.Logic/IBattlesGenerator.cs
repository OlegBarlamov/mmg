using System;
using System.Threading.Tasks;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Players;
using Epic.Core.Services.UnitsContainers;
using Epic.Data.GlobalUnits;
using Epic.Data.UnitTypes;
using JetBrains.Annotations;

namespace Epic.Logic
{
    public interface IBattlesGenerator
    {
        Task Generate(IPlayerObject playerObject, int day, int currentBattlesCount);
    }

    [UsedImplicitly]
    public class BattleGenerator : IBattlesGenerator
    {
        public IBattleDefinitionsService BattleDefinitionsService { get; }
        public IGlobalUnitsRepository GlobalUnitsRepository { get; }
        public IUnitTypesRepository UnitTypesRepository { get; }
        public IUnitsContainersService UnitsContainersService { get; }

        private readonly Random _random = new Random();

        public BattleGenerator(
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IGlobalUnitsRepository globalUnitsRepository,
            [NotNull] IUnitTypesRepository unitTypesRepository,
            [NotNull] IUnitsContainersService unitsContainersService)
        {
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            GlobalUnitsRepository = globalUnitsRepository ?? throw new ArgumentNullException(nameof(globalUnitsRepository));
            UnitTypesRepository = unitTypesRepository ?? throw new ArgumentNullException(nameof(unitTypesRepository));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
        }
        
        public async Task Generate(IPlayerObject playerObject, int day, int currentBattlesCount)
        {
            var width = _random.Next(5, 16);
            var height = _random.Next(5, 16);
            
            var unitTypes = await UnitTypesRepository.GetAll();
            var container = await UnitsContainersService.Create(height, Guid.Empty);
            
            var unitsCount = _random.Next(1, 5);
            for (var i = 0; i < unitsCount; i++)
            {
                var unitType = unitTypes[_random.Next(unitTypes.Length)];
                var count = _random.Next(1, day * 20);
                
                await GlobalUnitsRepository.Create(unitType.Id, count, container.Id, true, i);
            }
            
            await BattleDefinitionsService.CreateBattleDefinition(playerObject.Id, width , height, playerObject.Day + 1, container.Id);
        }
    }
}