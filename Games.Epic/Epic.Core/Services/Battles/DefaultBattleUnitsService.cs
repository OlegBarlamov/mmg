using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Units;
using Epic.Data.BattleUnits;
using Epic.Data.Heroes;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NetExtensions.Collections;

namespace Epic.Core.Services.Battles
{
    [UsedImplicitly]
    public partial class DefaultBattleUnitsService : IBattleUnitsService
    {
        public IBattleUnitsRepository BattleUnitsRepository { get; }
        public IGlobalUnitsService PlayerUnitsService { get; }
        public IBattlesCacheService CacheService { get; }

        private ILogger<DefaultBattleUnitsService> Logger { get; }

        public DefaultBattleUnitsService(
            [NotNull] IBattleUnitsRepository battleUnitsRepository,
            [NotNull] IGlobalUnitsService playerUnitsService,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IBattlesCacheService cacheService)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            BattleUnitsRepository =
                battleUnitsRepository ?? throw new ArgumentNullException(nameof(battleUnitsRepository));
            PlayerUnitsService = playerUnitsService ?? throw new ArgumentNullException(nameof(playerUnitsService));
            CacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            Logger = loggerFactory.CreateLogger<DefaultBattleUnitsService>();
        }

        public async Task<IReadOnlyCollection<IBattleUnitObject>> GetBattleUnits(Guid battleId)
        {
            var unitEntities = await BattleUnitsRepository.GetByBattleId(battleId);
            var unitObjects = unitEntities.Select(MutableBattleUnitObject.FromEntity).ToArray();
            return await FillBattleUnitObjects(unitObjects);
        }

        public Task<IReadOnlyCollection<IBattleUnitObject>> CreateUnitsFromBattleDefinition(IBattleDefinitionObject battleDefinition, Guid battleId)
        {
            var unitsFitToBattle = PickUnitsFitToBattleSize(battleDefinition.Units, battleDefinition);
            return CreateBattleUnitsFromGlobalUnits(unitsFitToBattle, InBattlePlayerNumber.Player2, battleId, DefaultHeroStats.Instance);
        }

        public async Task<IReadOnlyCollection<IBattleUnitObject>> CreateBattleUnitsFromGlobalUnits(
            IReadOnlyCollection<IGlobalUnitObject> playerUnits, 
            InBattlePlayerNumber playerNumber, 
            Guid battleId,
            IHeroStats heroStats)
        {
            var userUnitsById = playerUnits.ToDictionary(u => u.Id, u => u);
            var entities = playerUnits.Select(u => BattleUnitEntityFields.FromUserUnit(u, battleId, playerNumber, heroStats))
                .ToArray<IBattleUnitEntityFields>();

            var createdInstances = await BattleUnitsRepository.CreateBatch(entities);
            
            var attacksDataMapPerBattleUnit = createdInstances.ToDictionary(x => x.Id, x => CreateAttackData(x, userUnitsById[x.GlobalUnitId]));
            await BattleUnitsRepository.InsertAttacksData(attacksDataMapPerBattleUnit
                .SelectMany(x => x.Value)
                .ToArray()
            );
            
            var createdBattleUnits = createdInstances.Select(MutableBattleUnitObject.FromEntity).ToArray();
            createdBattleUnits.ForEach(u =>
            {
                u.GlobalUnit = MutableGlobalUnitObject.CopyFrom(userUnitsById[u.PlayerUnitId]);
                u.AttackFunctionsData = attacksDataMapPerBattleUnit[u.Id];
            });

            return createdBattleUnits;
        }

        public async Task UpdateUnits(IReadOnlyCollection<IBattleUnitObject> battleUnits)
        {
            var entities = battleUnits.Select(BattleUnitEntity.FromBattleUnitObject).ToArray<IBattleUnitEntity>();
            await BattleUnitsRepository.Update(entities);
            await BattleUnitsRepository.UpdateAttacksData(battleUnits.SelectMany(x => x.AttackFunctionsData).ToArray());
        }

        public IReadOnlyCollection<IGlobalUnitObject> PickUnitsFitToBattleSize(IReadOnlyCollection<IGlobalUnitObject> units, ISize size)
        {
            return units.OrderBy(x => x.ContainerSlotIndex).Take(size.Height).ToArray();
        }

        private async Task<IReadOnlyCollection<MutableBattleUnitObject>> FillBattleUnitObjects(
            IReadOnlyCollection<MutableBattleUnitObject> battleUnitObjects)
        {
            var attacksDataPerUnitTasks =
                battleUnitObjects.ToDictionary(x => x.Id, x => BattleUnitsRepository.GetAttacksData(x.Id));
            await Task.WhenAll(attacksDataPerUnitTasks.Values);
            
            var userUnitIds = battleUnitObjects.Select(x => x.PlayerUnitId).ToArray();
            
            var userUnits = await PlayerUnitsService.GetUnitsByIds(userUnitIds);
            var userUnitsById = userUnits.ToDictionary(u => u.Id, u => u);
            var validUnits = new List<MutableBattleUnitObject>();
                
            foreach (var battleUnitObject in battleUnitObjects)
            {
                if (userUnitsById.TryGetValue(battleUnitObject.PlayerUnitId, out var playerUnitObject))
                    battleUnitObject.GlobalUnit = MutableGlobalUnitObject.CopyFrom(playerUnitObject);

                battleUnitObject.AttackFunctionsData = attacksDataPerUnitTasks[battleUnitObject.Id].Result;
              
                if (IsValid(battleUnitObject))
                    validUnits.Add(battleUnitObject);
                else
                    Logger.LogError($"Invalid User Unit: {battleUnitObject.PlayerUnitId}");
            }

            return validUnits;
        }
        
        private IReadOnlyList<AttackFunctionStateEntity> CreateAttackData(IBattleUnitEntity battleUnitEntity, IGlobalUnitObject globalUnitObject)
        {
            return globalUnitObject.UnitType.Attacks.Select(
                (x, i) => AttackFunctionStateEntity.FromAttackFunction(battleUnitEntity.Id, i, x))
                .ToArray();
        }

        private bool IsValid(IBattleUnitObject battleUnitObject)
        {
            return battleUnitObject.GlobalUnit != null;
        }
    }
}