using System.Diagnostics;

namespace ZiziBot.Utils;

public static class ExceptionUtil
{
    public static StackTrace ToStackTrace(this Exception exception)
    {
        StackTrace stackTrace = new(exception, true);
        return stackTrace;
    }
}