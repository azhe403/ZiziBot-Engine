using Serilog;
using ZiziBot.TelegramBot;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.LoadSettings();

builder.Host.ConfigureSerilog(true);

builder.WebHost.ConfigureCustomListenPort();

builder.Services.ConfigureServices();
builder.Services.ConfigureApi();
builder.Services.ConfigureHangfire();
builder.Services.ConfigureTelegramBot();
builder.Services.AddAllMiddleware();
builder.Services.AddConsole();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.ConfigureFlurl();

app.PrintAbout();

await app.UseMongoMigration();

app.UseAuthorization();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}/{id?}");
app.ConfigureConsole();
app.ConfigureApi();
app.UseHangfire();
await app.RunTelegramBot();

await app.RunAsync();