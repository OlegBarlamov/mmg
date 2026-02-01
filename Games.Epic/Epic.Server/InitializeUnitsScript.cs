using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core.Services.BuffTypes;
using Epic.Core.Services.UnitTypes;
using Epic.Data.UnitTypes;
using FrameworkSDK;
using JetBrains.Annotations;

namespace Epic.Server
{
    internal class UnitsConfig
    {
        internal class UnitDeclaration : UnitTypeProperties
        {
            [UsedImplicitly]
            public string[] UpgradeOf { get; set; } = Array.Empty<string>();
            
            [UsedImplicitly]
            public string[] BuffTypes { get; set; } = Array.Empty<string>();
        }
        
        [UsedImplicitly]
        public Dictionary<string, UnitDeclaration> Units { get; set; }
    }
    
    [UsedImplicitly]
    public class InitializeUnitsScript : IAppComponent
    {
        public IUnitTypesRepository UnitTypesRepository { get; }
        public DefaultUnitTypesRegistry UnitTypesRegistry { get; }
        public IBuffTypesRegistry BuffTypesRegistry { get; }

        public InitializeUnitsScript(
            [NotNull] IUnitTypesRepository unitTypesRepository,
            [NotNull] DefaultUnitTypesRegistry unitTypesRegistry,
            [NotNull] IBuffTypesRegistry buffTypesRegistry)
        {
            UnitTypesRepository = unitTypesRepository ?? throw new ArgumentNullException(nameof(unitTypesRepository));
            UnitTypesRegistry = unitTypesRegistry ?? throw new ArgumentNullException(nameof(unitTypesRegistry));
            BuffTypesRegistry = buffTypesRegistry ?? throw new ArgumentNullException(nameof(buffTypesRegistry));
        }
        
        public void Configure()
        {
            ProcessAsync().Wait();
        }

        private async Task ProcessAsync()
        {
            var config = YamlConfigParser<UnitsConfig>
                .Parse("Configs/units.yaml");
            
            var createdUnits = await UnitTypesRepository.CreateBatch(config.Units.Select(x =>
            {
                x.Value.Key = x.Key;
                
                // Resolve BuffTypes string keys to GUIDs for unit-level buffs
                if (x.Value.BuffTypes != null && x.Value.BuffTypes.Length > 0)
                {
                    x.Value.BuffTypeIds = x.Value.BuffTypes
                        .Where(key => !string.IsNullOrWhiteSpace(key))
                        .Select(key => BuffTypesRegistry.ByKey(key).Id)
                        .ToList();
                }
                
                // Resolve ApplyBuffTypes string keys to GUIDs for each attack
                foreach (var attack in x.Value.Attacks)
                {
                    if (attack.ApplyBuffTypes != null && attack.ApplyBuffTypes.Count > 0)
                    {
                        attack.ApplyBuffTypeIds = attack.ApplyBuffTypes
                            .Where(key => !string.IsNullOrWhiteSpace(key))
                            .Select(key => BuffTypesRegistry.ByKey(key).Id)
                            .ToList();
                    }
                }
                
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