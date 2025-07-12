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
    public class DefaultBattleUnitsService : IBattleUnitsService
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
            var unitObjects = unitEntities.Select(ToBattleUnitObject).ToArray();
            return await FillBattleUnitObjects(unitObjects);
        }

        public async Task<IReadOnlyCollection<IBattleUnitObject>> CreateUnitsFromBattleDefinition(IBattleDefinitionObject battleDefinition, Guid battleId)
        {
            var userUnits = battleDefinition.Units;
            var userUnitsById = userUnits.ToDictionary(u => u.Id, u => u);
            var entitiesToCreate = battleDefinition.Units.Select(u => new BattleUnitEntityFields
            {
                BattleId = battleId,
                UserUnitId = u.Id,
                Column = 0,
                Row = 0,
                PlayerIndex = (int)PlayerNumber.Player2,
            }).ToArray<IBattleUnitEntityFields>();

            var battleUnitsEntities = await BattleUnitsRepository.CreateBatch(entitiesToCreate);
            var battleUnitsObjects = battleUnitsEntities.Select(ToBattleUnitObject).ToArray();
            battleUnitsObjects.ForEach(u =>
            {
                u.UserUnit = MutableUserUnitObject.CopyFrom(userUnitsById[u.UserUnitId]);
            });
            return battleUnitsObjects;
        }

        public async Task<IReadOnlyCollection<IBattleUnitObject>> CreateUnitsFromUserUnits(IReadOnlyCollection<IUserUnitObject> userUnits, int playerIndex, Guid battleId)
        {
            var userUnitsById = userUnits.ToDictionary(u => u.Id, u => u);
            var entities = userUnits.Select(x => new BattleUnitEntityFields
            {
                BattleId = battleId,
                UserUnitId = x.Id,
                Column = 0,
                Row = 0,
                PlayerIndex = playerIndex,
            }).ToArray<IBattleUnitEntityFields>();

            var createdInstances = await BattleUnitsRepository.CreateBatch(entities);
            var createdBattleUnits = createdInstances.Select(ToBattleUnitObject).ToArray();
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
            
            var entities = battleUnits.Select(x => new BattleUnitEntity(x.Id)
            {
                BattleId = x.BattleId,
                UserUnitId = x.UserUnit.Id,
                Column = x.Column,
                Row = x.Row,
                PlayerIndex = x.PlayerIndex,
            }).ToArray<IBattleUnitEntity>();
            
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

        private MutableBattleUnitObject ToBattleUnitObject(IBattleUnitEntity entity)
        {
            return new MutableBattleUnitObject
            {
                Id = entity.Id,
                BattleId = entity.BattleId,
                UserUnitId = entity.UserUnitId,
                UserUnit = null,
                Column = entity.Column,
                Row = entity.Row,
                PlayerIndex = entity.PlayerIndex,
            };
        }

        private class BattleUnitEntity : BattleUnitEntityFields, IBattleUnitEntity
        {
            public Guid Id { get; }

            public BattleUnitEntity(Guid id)
            {
                Id = id;
            }
        } 
        
        private class BattleUnitEntityFields : IBattleUnitEntityFields
        {
            public Guid BattleId { get; set; }
            public Guid UserUnitId { get; set; }
            public int Column { get; set; }
            public int Row { get; set; }
            public int PlayerIndex { get; set; }
        } 
    }
}