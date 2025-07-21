using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.UnitTypes;
using Epic.Data.PlayerUnits;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Core.Services.Units
{
    [UsedImplicitly]
    public class DefaultPlayerUnitsService : IPlayerUnitsService
    {
        public IPlayerUnitsRepository PlayerUnitsRepository { get; }
        public IUnitTypesService UnitTypesService { get; }

        private ILogger<DefaultPlayerUnitsService> Logger { get; }

        public DefaultPlayerUnitsService([NotNull] IPlayerUnitsRepository playerUnitsRepository,
            [NotNull] IUnitTypesService unitTypesService, ILoggerFactory loggerFactory)
        {
            PlayerUnitsRepository =
                playerUnitsRepository ?? throw new ArgumentNullException(nameof(playerUnitsRepository));
            UnitTypesService = unitTypesService ?? throw new ArgumentNullException(nameof(unitTypesService));
            Logger = loggerFactory.CreateLogger<DefaultPlayerUnitsService>();
        }

        public async Task<IReadOnlyCollection<IPlayerUnitObject>> GetAliveUnitsByContainerId(Guid containerId)
        {
            var aliveUsersEntities = await PlayerUnitsRepository.GetAliveByContainerId(containerId);
            var aliveUsersObjects = aliveUsersEntities.Select(MutablePlayerUnitObject.FromEntity).ToArray();
            return await FillUserObjects(aliveUsersObjects);
        }

        public async Task<IReadOnlyCollection<IPlayerUnitObject>> GetUnitsByIds(IReadOnlyCollection<Guid> ids)
        {
            var units = await PlayerUnitsRepository.FetchUnitsByIds(ids);
            if (units.Length < ids.Count)
            {
                var missingIds = ids.Except(units.Select(u => u.Id))
                    .Select(x => x.ToString())
                    .ToArray();
                Logger.LogError($"Missing User Units with ids: {string.Join(',', missingIds)}");
            }

            var unitObjects = units.Select(MutablePlayerUnitObject.FromEntity).ToArray();
            return await FillUserObjects(unitObjects);
        }

        public Task UpdateUnits(IReadOnlyCollection<IPlayerUnitObject> userUnits)
        {
            var entities = userUnits.ToArray().Select(x => x.ToEntity()).ToArray();
            return PlayerUnitsRepository.Update(entities);
        }

        public async Task<IReadOnlyCollection<IPlayerUnitObject>> CreateUnits(
            IReadOnlyCollection<CreatePlayerUnitData> unitsToCreate)
        {
            var unitEntities = await Task.WhenAll(unitsToCreate.Select(data =>
                PlayerUnitsRepository.CreatePlayerUnit(data.UnitTypeId, data.Amount, data.PlayerId, data.ContainerId, true, 0)));

            var unitObjects = unitEntities.Select(MutablePlayerUnitObject.FromEntity).ToArray();
            return await FillUserObjects(unitObjects);
        }

        private async Task<IReadOnlyCollection<MutablePlayerUnitObject>> FillUserObjects(
            IReadOnlyCollection<MutablePlayerUnitObject> userUnits)
        {
            var userTypesIds = userUnits.Select(user => user.UnitTypeId).Distinct().ToArray();
            var unitTypes = await UnitTypesService.GetUnitTypesByIdsAsync(userTypesIds);

            var unitTypesByIds = unitTypes.ToDictionary(x => x.Id, x => x);
            var validAliveUserObjects = new List<MutablePlayerUnitObject>();
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

        private static bool IsValidObject(MutablePlayerUnitObject unitObject)
        {
            return unitObject.UnitType != null;
        }
    }
}