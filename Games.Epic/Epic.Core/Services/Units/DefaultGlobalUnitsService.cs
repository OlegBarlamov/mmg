using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.UnitTypes;
using Epic.Data.GlobalUnits;
using Epic.Data.PlayerUnits;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Core.Services.Units
{
    [UsedImplicitly]
    public class DefaultGlobalUnitsService : IGlobalUnitsService
    {
        public IGlobalUnitsRepository GlobalUnitsRepository { get; }
        public IUnitTypesService UnitTypesService { get; }

        private ILogger<DefaultGlobalUnitsService> Logger { get; }

        public DefaultGlobalUnitsService([NotNull] IGlobalUnitsRepository globalUnitsRepository,
            [NotNull] IUnitTypesService unitTypesService, ILoggerFactory loggerFactory)
        {
            GlobalUnitsRepository =
                globalUnitsRepository ?? throw new ArgumentNullException(nameof(globalUnitsRepository));
            UnitTypesService = unitTypesService ?? throw new ArgumentNullException(nameof(unitTypesService));
            Logger = loggerFactory.CreateLogger<DefaultGlobalUnitsService>();
        }

        public async Task<IGlobalUnitObject> GetById(Guid id)
        {
            var units = await GetUnitsByIds(new[] { id });
            return units.First();
        }

        public async Task<bool> HasAliveUnits(Guid containerId)
        {
            var units = await GlobalUnitsRepository.GetAliveByContainerId(containerId);
            return units.Any();
        }

        public async Task<IReadOnlyCollection<IGlobalUnitObject>> GetAliveUnitsByContainerId(Guid containerId)
        {
            var aliveUsersEntities = await GlobalUnitsRepository.GetAliveByContainerId(containerId);
            var aliveUsersObjects = aliveUsersEntities.Select(MutableGlobalUnitObject.FromEntity).ToArray();
            return await FillUserObjects(aliveUsersObjects);
        }

        public async Task<IReadOnlyCollection<IGlobalUnitObject>> GetUnitsByIds(IReadOnlyCollection<Guid> ids)
        {
            var units = await GlobalUnitsRepository.FetchUnitsByIds(ids);
            if (units.Length < ids.Count)
            {
                var missingIds = ids.Except(units.Select(u => u.Id))
                    .Select(x => x.ToString())
                    .ToArray();
                Logger.LogError($"Missing User Units with ids: {string.Join(',', missingIds)}");
            }

            var unitObjects = units.Select(MutableGlobalUnitObject.FromEntity).ToArray();
            return await FillUserObjects(unitObjects);
        }

        public Task UpdateUnits(IReadOnlyCollection<IGlobalUnitObject> userUnits)
        {
            var entities = userUnits.ToArray().Select(x => x.ToEntity()).ToArray();
            return GlobalUnitsRepository.Update(entities);
        }

        public async Task<IReadOnlyCollection<IGlobalUnitObject>> CreateUnits(
            IReadOnlyCollection<CreateUnitData> unitsToCreate)
        {
            var unitEntities = await Task.WhenAll(unitsToCreate.Select(data =>
                GlobalUnitsRepository.Create(data.UnitTypeId, data.Amount, data.ContainerId, true, 0)));

            var unitObjects = unitEntities.Select(MutableGlobalUnitObject.FromEntity).ToArray();
            return await FillUserObjects(unitObjects);
        }

        public async Task<IGlobalUnitObject> GetAliveUnitFromContainerInSlot(Guid containerId, int slotIndex)
        {
            var unitEntity = await GlobalUnitsRepository.GetAliveUnitFromContainerInSlot(containerId, slotIndex);
            var unit = MutableGlobalUnitObject.FromEntity(unitEntity);
            
            var filledUnits = await FillUserObjects(new [] { unit });
            return filledUnits.First();
        }

        private async Task<IReadOnlyCollection<MutableGlobalUnitObject>> FillUserObjects(
            IReadOnlyCollection<MutableGlobalUnitObject> userUnits)
        {
            var userTypesIds = userUnits.Select(user => user.UnitTypeId).Distinct().ToArray();
            var unitTypes = await UnitTypesService.GetUnitTypesByIdsAsync(userTypesIds);

            var unitTypesByIds = unitTypes.ToDictionary(x => x.Id, x => x);
            var validAliveUserObjects = new List<MutableGlobalUnitObject>();
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

        private static bool IsValidObject(MutableGlobalUnitObject unitObject)
        {
            return unitObject.UnitType != null;
        }
    }
}