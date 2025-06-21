using System.Diagnostics;

namespace ZiziBot.Utils;

public static class ExceptionUtil
{
    public static StackTrace? ToStackTrace(this Exception exception)
    {
        var stackTrace = new StackTrace(exception, true);
        return stackTrace;
    }
}