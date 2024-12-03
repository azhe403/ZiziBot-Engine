namespace ZiziBot.Utils;

public static class EnvUtil
{
    public static string GetEnv(string key, string? defaultValue = default, bool throwIsMissing = false)
    {
        var envVal = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        if (!string.IsNullOrEmpty(envVal))
            return envVal;

        if (throwIsMissing)
            throw new EnvMissingException(key);

        return defaultValue ?? string.Empty;
    }

    public static bool IsEnvExist(string key)
    {
        var envVal = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        return !string.IsNullOrEmpty(envVal);
    }

    public static bool IsDevelopment()
    {
        return IsEnv("ASPNETCORE_ENVIRONMENT", "Development");
    }

    public static bool IsEnv(string key, string value)
    {
        var envVal = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        return string.Equals(envVal, value, StringComparison.OrdinalIgnoreCase);
    }
}