using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ZiziBot.Contracts.Types;

public partial class TgBotApiDoc
{
    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("release_date")]
    public string ReleaseDate { get; set; }

    [JsonProperty("changelog")]
    public Uri Changelog { get; set; }

    [JsonProperty("methods")]
    public Dictionary<string, Method> Methods { get; set; }

    [JsonProperty("types")]
    public Dictionary<string, Method> Types { get; set; }

    public Dictionary<string, Method> MethodsAndTypes => Methods.Concat(Types).ToDictionary(x => x.Key, x => x.Value);
}

public partial class Method
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("href")]
    public Uri Href { get; set; }

    [JsonProperty("description")]
    public List<string>? Description { get; set; }

    [JsonProperty("returns", NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? Returns { get; set; }

    [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
    public List<Field>? Fields { get; set; }

    [JsonProperty("subtypes", NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? Subtypes { get; set; }

    [JsonProperty("subtype_of", NullValueHandling = NullValueHandling.Ignore)]
    public List<SubtypeOf>? SubtypeOf { get; set; }
}

public partial class Field
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("types")]
    public List<string> Types { get; set; }

    [JsonProperty("required")]
    public bool FieldRequired { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }
}

public enum SubtypeOf
{
    BotCommandScope,
    ChatMember,
    InlineQueryResult,
    InputMedia,
    InputMessageContent,
    MenuButton,
    PassportElementError
};

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
        {
            SubtypeOfConverter.Singleton,
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        }
    };
}

internal class SubtypeOfConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(SubtypeOf) || t == typeof(SubtypeOf?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        var value = serializer.Deserialize<string>(reader);
        switch (value)
        {
            case "BotCommandScope":
                return SubtypeOf.BotCommandScope;
            case "ChatMember":
                return SubtypeOf.ChatMember;
            case "InlineQueryResult":
                return SubtypeOf.InlineQueryResult;
            case "InputMedia":
                return SubtypeOf.InputMedia;
            case "InputMessageContent":
                return SubtypeOf.InputMessageContent;
            case "MenuButton":
                return SubtypeOf.MenuButton;
            case "PassportElementError":
                return SubtypeOf.PassportElementError;
        }
        throw new Exception("Cannot unmarshal type SubtypeOf");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (SubtypeOf) untypedValue;
        switch (value)
        {
            case SubtypeOf.BotCommandScope:
                serializer.Serialize(writer, "BotCommandScope");
                return;
            case SubtypeOf.ChatMember:
                serializer.Serialize(writer, "ChatMember");
                return;
            case SubtypeOf.InlineQueryResult:
                serializer.Serialize(writer, "InlineQueryResult");
                return;
            case SubtypeOf.InputMedia:
                serializer.Serialize(writer, "InputMedia");
                return;
            case SubtypeOf.InputMessageContent:
                serializer.Serialize(writer, "InputMessageContent");
                return;
            case SubtypeOf.MenuButton:
                serializer.Serialize(writer, "MenuButton");
                return;
            case SubtypeOf.PassportElementError:
                serializer.Serialize(writer, "PassportElementError");
                return;
        }
        throw new Exception("Cannot marshal type SubtypeOf");
    }

    public static readonly SubtypeOfConverter Singleton = new SubtypeOfConverter();
}