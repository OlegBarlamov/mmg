using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.GameResources;
using Epic.Data.UnitTypes;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Core.Services.UnitTypes
{
    [UsedImplicitly]
    public class DefaultUnitTypesService : IUnitTypesService
    {
        public IUnitTypesRepository Repository { get; }
        public IGameResourcesRepository GameResourcesRepository { get; }

        private ILogger<DefaultUnitTypesService> Logger { get; }

        public DefaultUnitTypesService(
            [NotNull] IUnitTypesRepository repository,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IGameResourcesRepository gameResourcesRepository)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            GameResourcesRepository = gameResourcesRepository ?? throw new ArgumentNullException(nameof(gameResourcesRepository));
            Logger = loggerFactory.CreateLogger<DefaultUnitTypesService>();
        }
        
        public async Task<IUnitTypeObject> GetUnitTypeByIdAsync(Guid id)
        {
            var unitTypeEntity = await Repository.GetById(id);
            return MutualUnitTypeObject.FromEntity(unitTypeEntity);
        }

        public async Task<IReadOnlyCollection<IUnitTypeObject>> GetUnitTypesByIdsAsync(IReadOnlyCollection<Guid> ids)
        {
            var entities = await Repository.FetchByIds(ids);
            var unitTypeObjects = entities.Select(MutualUnitTypeObject.FromEntity).ToArray();
            if (unitTypeObjects.Length != ids.Count)
            {
                var missingIds = new List<String>();
                foreach (var id in ids)
                {
                    if (unitTypeObjects.Any(unit => unit.Id == id))
                        continue;
                    
                    missingIds.Add(id.ToString());
                }
                Logger.LogError($"Missing UnitTypes with ids: {string.Join(',', missingIds)}");
            }
            return unitTypeObjects;
        }

        public async Task<Price> GetPrice(IUnitTypeObject unitType)
        {
            var resourcesByKeys = await GameResourcesRepository.GetAllResourcesByKeys();

            var totalUnitValue = unitType.Value;
            var distribution = unitType.GetNormalizedResourcesDistribution();
            var totalParts = distribution.Values.Sum();

            var resourceValues = new Dictionary<Guid, int>();
            decimal totalAssigned = 0;

            foreach (var kvp in distribution)
            {
                var resourceKey = kvp.Key;
                var part = kvp.Value;
                var resourceId = resourcesByKeys[resourceKey].Id;

                var proportion = (decimal)part / totalParts;
                var share = totalUnitValue * proportion;
                var resourceUnitPrice = resourcesByKeys[resourceKey].Price;

                var resourcePrice = Math.Floor(share * resourceUnitPrice);
                resourceValues[resourceId] = (int)(resourcePrice / resourceUnitPrice);
                totalAssigned += resourcePrice;
            }

            // Compute expected total value in resource prices
            int leftover = (int)Math.Round(totalUnitValue - totalAssigned);
            if (leftover > 0)
            {
                // Assign leftover to gold
                if (!resourceValues.TryAdd(GameResourcesRepository.GoldResourceId, leftover))
                    resourceValues[GameResourcesRepository.GoldResourceId] += leftover;
            }

            return Price.Create(resourceValues);
        }

        public Task<Price[]> GetPrices(IReadOnlyList<IUnitTypeObject> unitTypes)
        {
            return Task.WhenAll(unitTypes.Select(GetPrice));
        }
    }
}