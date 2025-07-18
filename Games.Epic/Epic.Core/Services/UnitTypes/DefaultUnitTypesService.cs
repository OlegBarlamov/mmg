using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.UnitTypes;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Core
{
    [UsedImplicitly]
    public class DefaultUnitTypesService : IUnitTypesService
    {
        public IUnitTypesRepository Repository { get; }
        
        private ILogger<DefaultUnitTypesService> Logger { get; }

        public DefaultUnitTypesService([NotNull] IUnitTypesRepository repository, [NotNull] ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
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
    }
}