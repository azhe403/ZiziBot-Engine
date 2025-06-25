using Serilog;
using ZiziBot.Common.Constants;
using ZiziBot.Common.Dtos;
using ZiziBot.Common.Exceptions;

namespace ZiziBot.Common.Utils;

public static class EnvUtil
{
    public static List<FlagDto>? Current { get; set; }

    public static string GetEnv(string key, string? defaultValue = null, bool throwIsMissing = false)
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

    public static bool IsStaging()
    {
        return IsEnv("ASPNETCORE_ENVIRONMENT", "Staging");
    }

    public static bool IsProduction()
    {
        return IsEnv("ASPNETCORE_ENVIRONMENT", "Production");
    }

    public static bool IsEnv(string key, string value)
    {
        var envVal = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        return string.Equals(envVal, value, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsEnabled(string flagName)
    {
        var flag = Current?.FirstOrDefault(x => x.Name == flagName);

        if (flag == null)
        {
            var defaultFlag = Flag.GetFields().FirstOrDefault(x => x.Name == flagName);
            var flagValue = defaultFlag?.Value ?? false;
            Log.Debug("Flag {FlagName} not found. Using default value: {DefaultFlagValue}", flagName, flagValue);

            return flagValue;
        }

        Log.Debug("Flag {FlagName} is {FlagValue}", flag.Name, flag.Value);
        return flag.Value;
    }
}