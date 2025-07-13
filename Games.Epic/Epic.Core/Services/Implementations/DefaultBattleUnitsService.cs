using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Core.Objects.BattleUnit;
using Epic.Data.BattleUnits;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NetExtensions.Collections;

namespace Epic.Core
{
    [UsedImplicitly]
    public partial class DefaultBattleUnitsService : IBattleUnitsService
    {
        public IBattleUnitsRepository BattleUnitsRepository { get; }
        public IUserUnitsService UserUnitsService { get; }
        
        private ILogger<DefaultBattleUnitsService> Logger { get; }

        public DefaultBattleUnitsService([NotNull] IBattleUnitsRepository battleUnitsRepository,
            [NotNull] IUserUnitsService userUnitsService, [NotNull] ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            BattleUnitsRepository =
                battleUnitsRepository ?? throw new ArgumentNullException(nameof(battleUnitsRepository));
            UserUnitsService = userUnitsService ?? throw new ArgumentNullException(nameof(userUnitsService));
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
            return CreateUnitsFromUserUnits(battleDefinition.Units, InBattlePlayerNumber.Player2, battleId);
        }

        public async Task<IReadOnlyCollection<IBattleUnitObject>> CreateUnitsFromUserUnits(IReadOnlyCollection<IUserUnitObject> userUnits, InBattlePlayerNumber playerNumber, Guid battleId)
        {
            var userUnitsById = userUnits.ToDictionary(u => u.Id, u => u);
            var entities = userUnits.Select(u => BattleUnitEntity.FromUserUnit(u, battleId, playerNumber)).ToArray<IBattleUnitEntityFields>();

            var createdInstances = await BattleUnitsRepository.CreateBatch(entities);
            var createdBattleUnits = createdInstances.Select(MutableBattleUnitObject.FromEntity).ToArray();
            createdBattleUnits.ForEach(u =>
            {
                u.UserUnit = MutableUserUnitObject.CopyFrom(userUnitsById[u.UserUnitId]);
            });

            return createdBattleUnits;
        }

        public Task UpdateUnits(IReadOnlyCollection<IBattleUnitObject> battleUnits, bool updateCache = false)
        {
            if (updateCache)
                throw new NotImplementedException();
            
            var entities = battleUnits.Select(BattleUnitEntity.FromBattleUnitObject).ToArray<IBattleUnitEntity>();
            return BattleUnitsRepository.Update(entities);
        }

        private async Task<IReadOnlyCollection<MutableBattleUnitObject>> FillBattleUnitObjects(
            IReadOnlyCollection<MutableBattleUnitObject> battleUnitObjects)
        {
            var userUnitIds = battleUnitObjects.Select(x => x.UserUnitId).ToArray();
      
            var userUnits = await UserUnitsService.GetUnitsByIds(userUnitIds);
            var userUnitsById = userUnits.ToDictionary(u => u.Id, u => u);
            var validUnits = new List<MutableBattleUnitObject>();
            foreach (var battleUnitObject in battleUnitObjects)
            {
                if (userUnitsById.TryGetValue(battleUnitObject.UserUnitId, out var userUnit))
                    battleUnitObject.UserUnit = MutableUserUnitObject.CopyFrom(userUnit);
              
                if (IsValid(battleUnitObject))
                    validUnits.Add(battleUnitObject);
                else
                    Logger.LogError($"Invalid User Unit: {battleUnitObject.UserUnitId}");
            }

            return validUnits;
        }

        private bool IsValid(IBattleUnitObject battleUnitObject)
        {
            return battleUnitObject.UserUnit != null;
        }
    }
}