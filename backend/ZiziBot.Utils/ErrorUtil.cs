namespace ZiziBot.Utils;

public static class ErrorUtil
{
    public static bool IsIgnorable(this string message)
    {
        var ignorableMessages = new[]
        {
            "bot was blocked",
            "chat not found",
            "connection could not be established",
            "message can't be deleted",
            "message to delete not found",
            "name or service not known",
            "no such host is known",
            "not match",
            "unexpected token"
        };

        return ignorableMessages.Any(message.Contains);
    }
}