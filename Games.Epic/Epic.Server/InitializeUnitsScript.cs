using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.UnitTypes;
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

        public InitializeUnitsScript([NotNull] IUnitTypesRepository unitTypesRepository)
        {
            UnitTypesRepository = unitTypesRepository ?? throw new ArgumentNullException(nameof(unitTypesRepository));
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
        }

        public void Dispose()
        {
        }
    }
}