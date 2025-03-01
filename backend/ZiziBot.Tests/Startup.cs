using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.DependencyInjection;
using ZiziBot.Tests.Interfaces;

namespace ZiziBot.Tests;

public class Startup : IStartup
{
    public void ConfigureHost(IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureAppConfiguration(builder => builder.LoadSettings());
    }

    public void ConfigureServices(IServiceCollection services)
    {
        _ = services.ConfigureServices().Result;
        services.AddSkippableFactSupport();
    }

    public void Configure(IServiceProvider provider, ITestOutputHelperAccessor accessor)
    {
    }
}