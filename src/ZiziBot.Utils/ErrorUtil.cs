namespace ZiziBot.Utils;

public static class ErrorUtil
{
    public static bool CanBeIgnored(this string message)
    {
        var ignorableMessages = new[]
        {
            "message to delete not found",
            "message can't be deleted"
        };

        return ignorableMessages.Any(message.Contains);
    }
}