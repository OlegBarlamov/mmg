using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Epic.Server;

public static class YamlConfigParser<T>
{
    public static T Parse(string yamlPath)
    {
        using var file = File.OpenText(yamlPath);
            
        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();
                
        return deserializer.Deserialize<T>(new MergingParser(new Parser(file)));
    }
}
