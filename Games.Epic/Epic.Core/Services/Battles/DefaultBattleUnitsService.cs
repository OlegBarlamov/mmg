using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Core.Services.Buffs;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.Units;
using Epic.Data.BattleUnits;
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
        public IBuffsService BuffsService { get; }

        private ILogger<DefaultBattleUnitsService> Logger { get; }

        public DefaultBattleUnitsService(
            [NotNull] IBattleUnitsRepository battleUnitsRepository,
            [NotNull] IGlobalUnitsService playerUnitsService,
            [NotNull] IBuffsService buffsService,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IBattlesCacheService cacheService)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            BattleUnitsRepository =
                battleUnitsRepository ?? throw new ArgumentNullException(nameof(battleUnitsRepository));
            PlayerUnitsService = playerUnitsService ?? throw new ArgumentNullException(nameof(playerUnitsService));
            BuffsService = buffsService ?? throw new ArgumentNullException(nameof(buffsService));
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
            return CreateBattleUnitsFromGlobalUnits(unitsFitToBattle, InBattlePlayerNumber.Player2, battleId, null);
        }

        public async Task<IReadOnlyCollection<IBattleUnitObject>> CreateBattleUnitsFromGlobalUnits(
            IReadOnlyCollection<IGlobalUnitObject> playerUnits, 
            InBattlePlayerNumber playerNumber, 
            Guid battleId,
            IHeroObject hero)
        {
            var heroStats = hero?.GetCumulativeHeroStats() ?? DefaultHeroStats.Instance;
            
            var userUnitsById = playerUnits.ToDictionary(u => u.Id, u => u);
            var entities = playerUnits.Select(u => BattleUnitEntityFields.FromUserUnit(u, battleId, playerNumber))
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
                u.HeroStats = heroStats;
            });

            await PrecreatePermanentBuffsFromUnitTypes(createdBattleUnits);
            await PrecreatePermanentBuffsFromHeroArtifacts(hero, createdBattleUnits);
            
            // Set initial health to max health (including buff bonuses)
            createdBattleUnits.ForEach(u => u.CurrentHealth = u.MaxHealth);
            
            return createdBattleUnits;
        }

        private async Task PrecreatePermanentBuffsFromUnitTypes(IReadOnlyCollection<MutableBattleUnitObject> createdBattleUnits)
        {
            if (createdBattleUnits == null || createdBattleUnits.Count == 0)
                return;

            const int permanentDurationRemaining = 0;

            var createTasks = new List<Task<IBuffObject>>();
            foreach (var unit in createdBattleUnits)
            {
                var buffTypeIds = unit.GlobalUnit?.UnitType?.BuffTypeIds ?? Array.Empty<Guid>();
                foreach (var buffTypeId in buffTypeIds.Where(x => x != Guid.Empty).Distinct())
                {
                    createTasks.Add(BuffsService.Create(unit.Id, buffTypeId, permanentDurationRemaining));
                }
            }

            if (createTasks.Count == 0)
            {
                createdBattleUnits.ForEach(u => u.Buffs = Array.Empty<IBuffObject>());
                return;
            }

            var createdBuffs = await Task.WhenAll(createTasks);
            var buffsByUnitId = createdBuffs
                .GroupBy(x => x.TargetBattleUnitId)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<IBuffObject>)g.ToArray());

            foreach (var unit in createdBattleUnits)
                unit.Buffs = buffsByUnitId.TryGetValue(unit.Id, out var buffs) ? buffs : Array.Empty<IBuffObject>();
        }

        private async Task PrecreatePermanentBuffsFromHeroArtifacts(
            IHeroObject hero,
            IReadOnlyCollection<MutableBattleUnitObject> createdBattleUnits)
        {
            if (hero == null || createdBattleUnits == null || createdBattleUnits.Count == 0)
                return;

            // Get buff type IDs from all equipped artifacts
            var artifactBuffTypeIds = hero.GetEquippedArtefacts()
                .SelectMany(a => a?.ArtifactType?.BuffTypeIds ?? Array.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray();

            if (artifactBuffTypeIds.Length == 0)
                return;

            const int permanentDurationRemaining = 0;

            // Create buffs for each unit from each artifact buff type
            var createTasks = new List<Task<IBuffObject>>();
            foreach (var unit in createdBattleUnits)
            {
                foreach (var buffTypeId in artifactBuffTypeIds)
                {
                    createTasks.Add(BuffsService.Create(unit.Id, buffTypeId, permanentDurationRemaining));
                }
            }

            if (createTasks.Count == 0)
                return;

            var createdBuffs = await Task.WhenAll(createTasks);
            var buffsByUnitId = createdBuffs
                .GroupBy(x => x.TargetBattleUnitId)
                .ToDictionary(g => g.Key, g => g.ToArray());

            // Append artifact buffs to existing unit buffs
            foreach (var unit in createdBattleUnits)
            {
                if (buffsByUnitId.TryGetValue(unit.Id, out var artifactBuffs))
                {
                    var existingBuffs = unit.Buffs ?? Array.Empty<IBuffObject>();
                    unit.Buffs = existingBuffs.Concat(artifactBuffs).ToArray();
                }
            }
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
            var buffsPerUnitTasks =
                battleUnitObjects.ToDictionary(x => x.Id, x => BuffsService.GetByTargetBattleUnitId(x.Id));

            await Task.WhenAll(attacksDataPerUnitTasks.Values.Concat<Task>(buffsPerUnitTasks.Values));
            
            var userUnitIds = battleUnitObjects.Select(x => x.PlayerUnitId).ToArray();
            
            var userUnits = await PlayerUnitsService.GetUnitsByIds(userUnitIds);
            var userUnitsById = userUnits.ToDictionary(u => u.Id, u => u);
            var validUnits = new List<MutableBattleUnitObject>();
                
            foreach (var battleUnitObject in battleUnitObjects)
            {
                if (userUnitsById.TryGetValue(battleUnitObject.PlayerUnitId, out var playerUnitObject))
                    battleUnitObject.GlobalUnit = MutableGlobalUnitObject.CopyFrom(playerUnitObject);

                battleUnitObject.AttackFunctionsData = attacksDataPerUnitTasks[battleUnitObject.Id].Result;
                battleUnitObject.Buffs = buffsPerUnitTasks[battleUnitObject.Id].Result;
              
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