namespace ZiziBot.Engine.Extensions;

public static class Infrastructure
{
    public static IWebHostBuilder ConfigureCustomListenPort(this IWebHostBuilder webHostBuilder)
    {
        var portVar = Environment.GetEnvironmentVariable("PORT");
        if (portVar?.Length > 0 && int.TryParse(portVar, out int port))
        {
            webHostBuilder.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(port);
            });
        }

        return webHostBuilder;
    }
}