using System.Globalization;

namespace ZiziBot.Contracts.Attributes;

public class BuildStampAttribute : Attribute
{
    public DateTime BuildDate { get; set; }
    public Version Version { get; set; }

    public BuildStampAttribute(string value)
    {
        BuildDate = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None);
        Version = Version.Parse(BuildDate.GenerateVersion());
    }
}