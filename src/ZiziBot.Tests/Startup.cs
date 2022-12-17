using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.DependencyInjection;

namespace ZiziBot.Tests;

public class Startup : IStartup
{
	public void ConfigureHost(IHostBuilder hostBuilder)
	{

	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.ConfigureServices();
	}

	public void Configure(IServiceProvider provider, ITestOutputHelperAccessor accessor)
	{
	}
}