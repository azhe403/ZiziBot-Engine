using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.DependencyInjection;

namespace ZiziBot.Tests;

public class Startup : IStartup
{
    public void ConfigureHost(IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureAppConfiguration(builder => builder.LoadSettings());
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMediatR(typeof(PingRequestHandler).GetTypeInfo().Assembly);

        services.ConfigureServices();
    }

    public void Configure(IServiceProvider provider, ITestOutputHelperAccessor accessor)
    {
    }
}