using System.Globalization;

namespace ZiziBot.Contracts.Attributes;

public class BuildDateAttribute : Attribute
{
    public DateTime BuildDate { get; set; }

    public BuildDateAttribute(string value)
    {
        BuildDate = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None);
    }
}