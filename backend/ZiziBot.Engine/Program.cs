using Serilog;
using ZiziBot.Application.Extensions;
using ZiziBot.Application.Database.Extensions;
using ZiziBot.Presentation.Bots.Telegram;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.LoadSettings();

builder.WebHost.ConfigureCustomListenPort();

await builder.Services.ConfigureServices();
await builder.Services.ConfigureTelegramBot();

builder.Services.AddSerilog(builder);
builder.Services.ConfigureScheduler();
builder.Services.AddRestApi();
builder.Services.AddAllMiddleware();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.ConfigureFlurl();

await app.PrintAbout();
await app.UseMongoMigration();

app.UseAuthorization();

app.ConfigureApi();
app.UseScheduler();

await app.RunTelegramBot();

await app.RunAsync();
