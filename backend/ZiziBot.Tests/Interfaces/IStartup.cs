using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.DependencyInjection;

namespace ZiziBot.Tests.Interfaces;

public interface IStartup
{
    void ConfigureHost(IHostBuilder hostBuilder);
    void ConfigureServices(IServiceCollection services);
    void Configure(IServiceProvider provider, ITestOutputHelperAccessor accessor);
}