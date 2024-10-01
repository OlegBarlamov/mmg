using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Core.Utils;
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
            var aliveUsersObjects = aliveUsersEntities.Select(ToUserUnitObject).ToArray();
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
            return units.Select(ToUserUnitObject).ToArray();
        }

        private async Task<IReadOnlyCollection<MutableUserUnitObject>> FillUserObjects(IReadOnlyCollection<MutableUserUnitObject> userUnits)
        {
            var userTypesIds = userUnits.Select(user => user.UnitTypeId).ToArray();
            var unitTypes = await UnitTypesService.GetUnitTypesByIdsAsync(userTypesIds);
            
            var unitTypesByIds = unitTypes.ToDictionary(x => x.Id, x => x);
            var validAliveUserObjects = new List<MutableUserUnitObject>();
            foreach (var unit in userUnits)
            {
                if (unitTypesByIds.TryGetValue(unit.UnitTypeId, out var unitType))
                    unit.UnitType = unitType;

                if (IsValidObject(unit))
                    validAliveUserObjects.Add(unit);
            }

            return validAliveUserObjects;
        }

        private static bool IsValidObject(MutableUserUnitObject unitObject)
        {
            return unitObject.UnitType != null;
        }
        
        private static MutableUserUnitObject ToUserUnitObject(IUserUnitEntity entity)
        {
            return new MutableUserUnitObject
            {
                Id = entity.Id,
                Count = entity.Count,
                IsAlive = entity.IsAlive,
                UserId = entity.UserId,
                UnitTypeId = entity.TypeId,
            };
        }
    }
}