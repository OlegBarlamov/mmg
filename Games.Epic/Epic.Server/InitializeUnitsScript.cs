using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.UnitTypes;
using Epic.Logic.Generator;
using FrameworkSDK;
using JetBrains.Annotations;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Epic.Server
{
    internal class UnitsConfig
    {
        public Dictionary<string, UnitTypeProperties> Units { get; set; }
    }
    
    [UsedImplicitly]
    public class InitializeUnitsScript : IAppComponent
    {
        public IUnitTypesRepository UnitTypesRepository { get; }
        public IBattlesGenerator BattlesGenerator { get; }

        public InitializeUnitsScript(
            [NotNull] IUnitTypesRepository unitTypesRepository,
            [NotNull] IBattlesGenerator battlesGenerator)
        {
            UnitTypesRepository = unitTypesRepository ?? throw new ArgumentNullException(nameof(unitTypesRepository));
            BattlesGenerator = battlesGenerator ?? throw new ArgumentNullException(nameof(battlesGenerator));
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
            await UnitTypesRepository.CreateBatch(config.Units.Select(x => x.Value));

            await BattlesGenerator.Initialize();
        }

        public void Dispose()
        {
        }
    }
}