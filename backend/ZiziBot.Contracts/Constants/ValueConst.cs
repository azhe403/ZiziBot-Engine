using System.Diagnostics.CodeAnalysis;

namespace ZiziBot.Contracts.Constants;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class ValueConst
{
    public const string DEFAULT_WELCOME_MESSAGE = "Hai {users}\nSelamat datang di Kontrakan {request.ChatTitle}";
}