using System.Diagnostics.CodeAnalysis;

namespace ZiziBot.Contracts.Constants;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class ValueConst
{
    public const string DEFAULT_WELCOME_MESSAGE = "Hai {users}\nSelamat datang di Kontrakan {request.ChatTitle}";

    public static readonly IEnumerable<int> PaginationSize = new[]
    {
        10,
        20,
        50,
        100,
        200,
        500,
        1000
    };
}