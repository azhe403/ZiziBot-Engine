using System.Diagnostics.CodeAnalysis;

namespace ZiziBot.Contracts.Constants;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class ValueConst
{
    public const ExecutionStrategy DEFAULT_EXECUTION_STRATEGY = ExecutionStrategy.Instant;

    public const int NEW_MEMBER_RAID_WINDOW_LIMIT = 7;
    public static readonly TimeSpan NEW_MEMBER_RAID_SLIDING_WINDOW = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan NEW_MEMBER_RAID_MODE_MUTE_DURATION = TimeSpan.FromHours(1);

    public static readonly TimeSpan NEW_MEMBER_ENTRY_SAFE_LIMIT = TimeSpan.FromDays(3);

    public static readonly char[] WordSeparator = [' ', '\n', ':', ';', ','];

    public const string NEW_MEMBER_DEFAULT_WELCOME_MESSAGE = "Hai {users}" +
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