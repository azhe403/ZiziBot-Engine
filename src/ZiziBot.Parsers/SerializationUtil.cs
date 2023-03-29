using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace ZiziBot.Parsers;

public static class SerializationUtil
{
    public static string ToJson<T>(this T source, bool indented = false)
    {
        return JsonConvert.SerializeObject(source, indented ? Formatting.Indented : Formatting.None);
    }

    public static T? ToObject<T>(this string source)
    {
        return JsonConvert.DeserializeObject<T>(source);
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


    public static string GetTransactionId(this IHeaderDictionary headerDictionary)
    {
        var transactionId = headerDictionary[HeaderKey.TransactionId].ToString();
        return transactionId;
    }

    public static long GetUserId(this IHeaderDictionary headerDictionary)
    {
        var userId = headerDictionary[HeaderKey.UserId].Convert<long>();
        return userId;
    }

    public static List<long>? GetListChatId(this IHeaderDictionary headerDictionary)
    {
        var listChatId = headerDictionary[HeaderKey.ListChatId].ToString().ToObject<List<long>>();
        return listChatId;
    }
}