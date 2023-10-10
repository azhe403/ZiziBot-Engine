namespace ZiziBot.Utils;

public static class ErrorUtil
{
    public static bool IsIgnorable(this string message)
    {
        var ignorableMessages = new List<string>()
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
            "request was canceled",
            "root element is missing",
            "root level is invalid",
            "user is deactivated",
            "unexpected token"
        };

        return ignorableMessages.Exists(message.ToLower().Contains);
    }
}