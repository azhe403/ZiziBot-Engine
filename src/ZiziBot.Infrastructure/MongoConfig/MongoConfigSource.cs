using Microsoft.Extensions.Configuration;

namespace ZiziBot.Infrastructure.MongoConfig;

public class MongoConfigSource : IConfigurationSource
{
	readonly AppSettingsDbContext _dbContext;

	public MongoConfigSource(string connectionString)
	{
		_dbContext = new AppSettingsDbContext(connectionString);
	}

	public IConfigurationProvider Build(IConfigurationBuilder builder)
	{
		return new MongoConfigProvider(this);
	}

	public Dictionary<string, string?> GetAppSettings()
	{
		SeedEventLogConfig();
		SeedHangfireConfig();

		var appSettingsList = _dbContext.AppSettings.ToList();

		return appSettingsList
			.Select(x => new KeyValuePair<string, string?>(x.Name, x.Value))
			.DistinctBy(pair => pair.Key)
			.ToDictionary(x => x.Key, x => x.Value);
	}

	private void SeedEventLogConfig()
	{
		var appSettingsList = _dbContext.AppSettings.Where(settings => settings.Name.Contains("EventLog")).ToList();

		if (appSettingsList.Any())
		{
			return;
		}

		var configs = new Dictionary<string, object>()
		{
			{ "ChatId", 12345 },
			{ "ThreadId", 34567 }
		};

		foreach (var config in configs)
		{
			_dbContext.AppSettings.Add(
				new AppSettings()
				{
					Name = $"EventLog:{config.Key}",
					Value = $"{config.Value}",
					Field = $"EventLog:{config.Key}",
					DefaultValue = $"{config.Value}",
					Status = (int) EventStatus.Complete,
					TransactionId = Guid.NewGuid().ToString()
				}
			);
		}

		_dbContext.SaveChanges();
	}

	private void SeedHangfireConfig()
	{
		var appSettingsList = _dbContext.AppSettings.Where(settings => settings.Name.Contains("Hangfire")).ToList();

		if (appSettingsList.Any())
		{
			return;
		}

		var configs = new Dictionary<string, object>()
		{
			{ "CurrentStorage", 2 },
			{ "MongoDbConnection", "mongo://localhost:21750" },
			{ "WorkerMultiplier", 2 },
			{ "Queues", "default" }
		};

		foreach (var config in configs)
		{
			_dbContext.AppSettings.Add(
				new AppSettings()
				{
					Name = $"Hangfire:{config.Key}",
					Value = $"{config.Value}",
					Field = $"Hangfire:{config.Key}",
					DefaultValue = $"{config.Value}",
					Status = (int) EventStatus.Complete,
					TransactionId = Guid.NewGuid().ToString()
				}
			);
		}

		_dbContext.SaveChanges();
	}
}