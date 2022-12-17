using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.DependencyInjection;

namespace ZiziBot.Tests;

public class Startup : IStartup
{
	public void ConfigureHost(IHostBuilder hostBuilder)
	{
		// throw new NotImplementedException();
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddServices();
		// throw new NotImplementedException();
	}

	public void Configure(IServiceProvider provider, ITestOutputHelperAccessor accessor)
	{
		// throw new NotImplementedException();
	}
}