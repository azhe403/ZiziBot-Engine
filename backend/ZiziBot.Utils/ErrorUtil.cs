namespace ZiziBot.Utils;

public static class ErrorUtil
{
    public static bool IsIgnorable(this string message)
    {
        var ignorableMessages = new[]
        {
            "a connection attempt failed",
            "bot was blocked",
            "chat not found",
            "connection could not be established",
            "error occurred while parsing",
            "message can't be deleted",
            "message to delete not found",
            "name or service not known",
            "no such host is known",
            "not match",
            "reference to undeclared entity",
            "root level is invalid",
            "user is deactivated",
            "unexpected token"
        };

        return ignorableMessages.Any(message.ToLower().Contains);
    }
}