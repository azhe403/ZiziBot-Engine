using System.Reflection;
using Hangfire;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.LoadSettings();

builder.Services.AddScoped<ActionFilter>();
builder.Services.AddControllers(
		options => {
			options.Filters.AddService<ActionFilter>();
			options.Filters.AddService<AccessFilter>();
		}
	)
	.AddNewtonsoftJson();

builder.Services.AddMediatR(typeof(PingRequestHandler).GetTypeInfo().Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices();

builder.Services.ConfigureHangfire();
builder.Services.ConfigureTelegramBot();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard(options:new DashboardOptions()
{
	DashboardTitle = "Zizi Dev - Hangfire Dashboard"
});

await app.RunAsync();