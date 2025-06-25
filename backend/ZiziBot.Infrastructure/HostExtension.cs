using Microsoft.AspNetCore.Hosting;
using ZiziBot.Common.Utils;

namespace ZiziBot.Infrastructure;

public static class HostExtension
{
    public static IWebHostBuilder ConfigureCustomListenPort(this IWebHostBuilder webHostBuilder)
    {
        var portVar = EnvUtil.GetEnv("PORT");
        if (portVar?.Length > 0 && int.TryParse(portVar, out int port))
        {
            webHostBuilder.ConfigureKestrel(options => {
                options.ListenAnyIP(port);
            });
        }

        return webHostBuilder;
    }
}