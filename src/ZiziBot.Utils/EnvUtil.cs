namespace ZiziBot.Utils;

public static class EnvUtil
{
    public static string GetEnv(string key, string? defaultValue = default)
    {
        return Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process) ?? defaultValue;
    }

    public static bool IsEnvExist(string key)
    {
        var envVal = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        return !string.IsNullOrEmpty(envVal);
    }
}