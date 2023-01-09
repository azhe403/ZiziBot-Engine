using Microsoft.AspNetCore.Builder;
using TIPC.Web.AutoWrapper;

namespace ZiziBot.Infrastructure;

public static class RestApiExtension
{
	public static IApplicationBuilder ConfigureAutoWrapper(this IApplicationBuilder app)
	{
		app.UseAutoWrapper();
		return app;
	}
}