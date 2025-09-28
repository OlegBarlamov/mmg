using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core.Services.UnitTypes;
using Epic.Data.UnitTypes;
using FrameworkSDK;
using JetBrains.Annotations;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Epic.Server
{
    internal class UnitsConfig
    {
        internal class UnitDeclaration : UnitTypeProperties
        {
            [UsedImplicitly]
            public string[] UpgradeOf { get; set; } = Array.Empty<string>();
        }
        
        [UsedImplicitly]
        public Dictionary<string, UnitDeclaration> Units { get; set; }
    }
    
    [UsedImplicitly]
    public class InitializeUnitsScript : IAppComponent
    {
        public IUnitTypesRepository UnitTypesRepository { get; }
        public DefaultUnitTypesRegistry UnitTypesRegistry { get; }

        public InitializeUnitsScript(
            [NotNull] IUnitTypesRepository unitTypesRepository,
            [NotNull] DefaultUnitTypesRegistry unitTypesRegistry)
        {
            UnitTypesRepository = unitTypesRepository ?? throw new ArgumentNullException(nameof(unitTypesRepository));
            UnitTypesRegistry = unitTypesRegistry ?? throw new ArgumentNullException(nameof(unitTypesRegistry));
        }
        
        public void Configure()
        {
            ProcessAsync().Wait();
        }

        private async Task ProcessAsync()
        {
            using var file = File.OpenText("Configs/units.yaml");
            
            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();
                
            var config = deserializer.Deserialize<UnitsConfig>(new MergingParser(new Parser(file)));
            
            var createdUnits = await UnitTypesRepository.CreateBatch(config.Units.Select(x =>
            {
                x.Value.Key = x.Key; 
                return x.Value;
            }));
            var createdUnitsByNames = createdUnits.ToDictionary(x => x.Name, x => x);
            
            FillUpgradedUnitsData(config, createdUnitsByNames, out var updatedUnits);

            await UnitTypesRepository.UpdateBatch(updatedUnits);

            await UnitTypesRegistry.Load(CancellationToken.None);
        }

        private void FillUpgradedUnitsData(
            UnitsConfig config, 
            IReadOnlyDictionary<string, IUnitTypeEntity> createdUnits,
            out List<UnitTypeEntity> updatedEntities)
        {
            updatedEntities = new List<UnitTypeEntity>();
            
            foreach (var configUnit in config.Units.Values)
            {
                var upgradeOfIds = configUnit.UpgradeOf
                    .Select(configName =>
                    {
                        var configUnitUpgradeTo = config.Units[configName];
                        return createdUnits[configUnitUpgradeTo.Name].Id;
                    }).ToArray();
                var targetEntity = createdUnits[configUnit.Name];
                var updatedEntity = UnitTypeEntity.FromProperties(targetEntity.Id, targetEntity);
                var hasChanged = updatedEntity.UpgradeForUnitTypeIds.Count != upgradeOfIds.Length ||
                    updatedEntity.UpgradeForUnitTypeIds.Any(x => !upgradeOfIds.Contains(x));
                
                if (!hasChanged)
                    continue;
                
                var finalUpgradeOfIds = updatedEntity.UpgradeForUnitTypeIds
                    .Concat(upgradeOfIds)
                    .Distinct();
                
                updatedEntity.UpgradeForUnitTypeIds = new List<Guid>(finalUpgradeOfIds);
                updatedEntities.Add(updatedEntity);
            }
        }

        public void Dispose()
        {
        }
    }
}