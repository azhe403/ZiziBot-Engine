using Serilog;
using ZiziBot.TelegramBot;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.LoadSettings();

builder.WebHost.ConfigureCustomListenPort();

await builder.Services.ConfigureServices();
await builder.Services.ConfigureTelegramBot();

builder.Services.AddSerilog(builder);
builder.Services.ConfigureHangfire();
builder.Services.AddRestApi();
builder.Services.AddAllMiddleware();
builder.Services.AddConsole();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.ConfigureFlurl();

app.PrintAbout();
await app.LoadFeatureFlag();
await app.UseMongoMigration();

app.UseAuthorization();

app.ConfigureConsole();
app.ConfigureApi();
app.UseHangfire();

await app.RunTelegramBot();

await app.RunAsync();