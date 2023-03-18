using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace ZiziBot.Parsers;

public static class SerializationUtil
{
    public static string ToJson<T>(this T source, bool indented = false)
    {
        return JsonConvert.SerializeObject(source, indented ? Formatting.Indented : Formatting.None);
    }

    public static string ToYaml(this object? obj)
    {
        var serializer = new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();

        var yaml = serializer.Serialize(obj)
            .Trim();

        return yaml;
    }
}