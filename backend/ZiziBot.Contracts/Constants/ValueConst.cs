using System.Diagnostics.CodeAnalysis;

namespace ZiziBot.Contracts.Constants;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class ValueConst
{
    public const int RAID_WINDOW_LIMIT = 7;
    public static readonly TimeSpan RAID_SLIDING_WINDOW = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan RAID_MODE_MUTE_DURATION = TimeSpan.FromHours(1);

    public const string DEFAULT_WELCOME_MESSAGE = "Hai {users}" +
                                                  "\nSelamat datang di Kontrakan <b>{ChatTitle}</b>";

    public static readonly IEnumerable<int> PaginationSize = new[] {
        10,
        20,
        50,
        100,
        200,
        500,
        1000
    };
}