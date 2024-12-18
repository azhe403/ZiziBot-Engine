using System.Diagnostics.CodeAnalysis;

namespace ZiziBot.Contracts.Constants;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class ValueConst
{
    public const ExecutionStrategy DEFAULT_EXECUTION_STRATEGY = ExecutionStrategy.Instant;

    public const int NEW_MEMBER_RAID_WINDOW_LIMIT = 7;
    public readonly static TimeSpan NEW_MEMBER_RAID_SLIDING_WINDOW = TimeSpan.FromMinutes(5);
    public readonly static TimeSpan NEW_MEMBER_RAID_MODE_MUTE_DURATION = TimeSpan.FromHours(1);

    public readonly static TimeSpan NEW_MEMBER_ENTRY_SAFE_LIMIT = TimeSpan.FromDays(3);

    public readonly static char[] WordSeparator = [' ', '\n', ':', ';', ','];

    public static string UNIQUE_KEY = "";

    public const string NEW_MEMBER_DEFAULT_WELCOME_MESSAGE = "Hai {users}" +
                                                             "\nSelamat datang di Kontrakan <b>{ChatTitle}</b>";

    public readonly static long[] SAFE_IDS = [
        777000
    ];

    public readonly static IEnumerable<int> PaginationSize = [
        10,
        20,
        50,
        100,
        200,
        500,
        1000
    ];
}