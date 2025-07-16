using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Core.Objects.UserUnit;
using Epic.Data.UserUnits;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Core
{
    public class DefaultUserUnitsService : IUserUnitsService
    {
        public IUserUnitsRepository UserUnitsRepository { get; }
        public IUnitTypesService UnitTypesService { get; }
        
        private ILogger<DefaultUserUnitsService> Logger { get; }

        public DefaultUserUnitsService([NotNull] IUserUnitsRepository userUnitsRepository, [NotNull] IUnitTypesService unitTypesService, ILoggerFactory loggerFactory)
        {
            UserUnitsRepository = userUnitsRepository ?? throw new ArgumentNullException(nameof(userUnitsRepository));
            UnitTypesService = unitTypesService ?? throw new ArgumentNullException(nameof(unitTypesService));
            Logger = loggerFactory.CreateLogger<DefaultUserUnitsService>();
        }
        
        public async Task<IReadOnlyCollection<IUserUnitObject>> GetAliveUnitsByUserAsync(Guid userId)
        {
            var aliveUsersEntities = await UserUnitsRepository.GetAliveUnitsByUserAsync(userId);
            var aliveUsersObjects = aliveUsersEntities.Select(MutableUserUnitObject.FromEntity).ToArray();
            return await FillUserObjects(aliveUsersObjects);
        }

        public async Task<IReadOnlyCollection<IUserUnitObject>> GetUnitsByIds(IReadOnlyCollection<Guid> ids)
        {
            var units = await UserUnitsRepository.FetchUnitsByIds(ids);
            if (units.Length < ids.Count)
            {
                var missingIds = ids.Except(units.Select(u => u.Id))
                    .Select(x => x.ToString())
                    .ToArray();
                Logger.LogError($"Missing User Units with ids: {string.Join(',', missingIds)}");
            }
            var unitObjects = units.Select(MutableUserUnitObject.FromEntity).ToArray();
            return await FillUserObjects(unitObjects);
        }

        public Task UpdateUnits(IReadOnlyCollection<IUserUnitObject> userUnits, bool updateCache = false)
        {
            if (updateCache)
                throw new NotImplementedException();
            
            var entities = userUnits.Select(UserUnitEntity.FromUserUnitObject).ToArray<IUserUnitEntity>();
            return UserUnitsRepository.Update(entities);
        }

        public async Task<IReadOnlyCollection<IUserUnitObject>> CreateUnits(IReadOnlyCollection<CreateUserUnitData> unitsToCreate)
        {
            var unitEntities = await Task.WhenAll(unitsToCreate.Select(data =>
                UserUnitsRepository.CreateUserUnit(data.UnitTypeId, data.Amount, data.UserId, true)));
            
            var unitObjects = unitEntities.Select(MutableUserUnitObject.FromEntity).ToArray();
            return await FillUserObjects(unitObjects);
        }

        private async Task<IReadOnlyCollection<MutableUserUnitObject>> FillUserObjects(IReadOnlyCollection<MutableUserUnitObject> userUnits)
        {
            var userTypesIds = userUnits.Select(user => user.UnitTypeId).Distinct().ToArray();
            var unitTypes = await UnitTypesService.GetUnitTypesByIdsAsync(userTypesIds);
            
            var unitTypesByIds = unitTypes.ToDictionary(x => x.Id, x => x);
            var validAliveUserObjects = new List<MutableUserUnitObject>();
            foreach (var unit in userUnits)
            {
                if (unitTypesByIds.TryGetValue(unit.UnitTypeId, out var unitType))
                    unit.UnitType = unitType;

                if (IsValidObject(unit))
                    validAliveUserObjects.Add(unit);
                else
                    Logger.LogError($"Invalid Unit Type: {unit.UnitTypeId}");
            }

            return validAliveUserObjects;
        }

        private static bool IsValidObject(MutableUserUnitObject unitObject)
        {
            return unitObject.UnitType != null;
        }

        private class UserUnitEntity : IUserUnitEntity
        {
            public Guid Id { get; set; }
            public Guid TypeId { get; set; }
            public int Count { get; set; }
            public Guid UserId { get; set; }
            public bool IsAlive { get; set; }

            private UserUnitEntity()
            {
                
            }

            public static UserUnitEntity FromUserUnitObject(IUserUnitObject userUnitObject)
            {
                return new UserUnitEntity
                {
                    Id = userUnitObject.Id,
                    TypeId = userUnitObject.UnitType.Id,
                    Count = userUnitObject.Count,
                    UserId = userUnitObject.UserId,
                    IsAlive = userUnitObject.IsAlive,
                };
            }
        }
    }
}