var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureCustomListenPort();

builder.Configuration.LoadSettings();

builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureServices();

builder.Services.ConfigureHangfire();
builder.Services.ConfigureTelegramBot();

var app = builder.Build();

app.PrintAbout();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseRouting();

if (EnvUtil.IsEnvExist(Env.AZURE_APP_CONFIG_CONNECTION_STRING))
    app.UseAzureAppConfiguration();

app.UseHangfire();

app.MapFallbackToFile("index.html");

await app.RunAsync();