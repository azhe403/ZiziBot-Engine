using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.InitSerilogBootstrapper();

builder.WebHost.ConfigureCustomListenPort();

builder.Configuration.LoadSettings();

builder.Services.ConfigureApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        // options.DocumentFilter<PathPrefixSwaggerDocumentFilter>("/api");
    }
);

builder.Services.ConfigureServices();

builder.Services.ConfigureHangfire();
builder.Services.ConfigureTelegramBot();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.ConfigureFlurlLogging();

app.PrintAbout();

await app.RunMigrationsAsync();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}"
);

app.UseAllMiddleware();

if (EnvUtil.IsEnvExist(Env.AZURE_APP_CONFIG_CONNECTION_STRING))
    app.UseAzureAppConfiguration();

app.UseHangfire();
app.RunTelegramBot();

app.MapFallbackToFile("index.html");

await app.RunAsync();