namespace ZiziBot.Utils;

public static class ErrorUtil
{
    public static bool IsIgnorable(this string message)
    {
        var ignorableMessages = new List<string>() {
            "a connection attempt failed",
            "bot is not a member",
            "bot was blocked",
            "cannot begin with",
            "chat not found",
            "connection could not be established",
            "error occurred while parsing",
            "message can't be deleted",
            "message to delete not found",
            "multiple root elements",
            "name or service not known",
            "no such host is known",
            "not match",
            "reference to undeclared entity",
            "request was canceled",
            "root element is missing",
            "root level is invalid",
            "unexpected token",
            "user is deactivated",
        };

        return ignorableMessages.Exists(message.ToLower().Contains);
    }
}