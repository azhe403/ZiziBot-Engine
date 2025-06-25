using System.Diagnostics;

namespace ZiziBot.Common.Utils;

public static class ExceptionUtil
{
    public static StackTrace? ToStackTrace(this Exception exception)
    {
        var stackTrace = new StackTrace(exception, true);
        return stackTrace;
    }
}