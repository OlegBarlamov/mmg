using System;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace X4World.Generation
{
    public static class GalaxyConfig
    {
        private static GalaxyGenerationConfig _instance;

        public static GalaxyGenerationConfig Instance =>
            _instance ?? (_instance = Load());

        private static GalaxyGenerationConfig Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("galaxy_generation.yaml", StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
                throw new FileNotFoundException(
                    "Embedded resource galaxy_generation.yaml not found. Available: " +
                    string.Join(", ", assembly.GetManifestResourceNames()));

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                return deserializer.Deserialize<GalaxyGenerationConfig>(reader);
            }
        }
    }
}
