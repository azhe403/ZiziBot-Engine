using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ZiziBot.DataMigration.MongoDb.Interfaces;
using ZiziBot.DataSource.MongoEf;

namespace ZiziBot.DataMigration.MongoDb.Migrations;

public class RefreshAppSettingMigration(MongoEfContext mongoDbContext) : IPreMigration
{
    public string Id { get; set; }

    public async Task UpAsync(IMongoDatabase database)
    {
        var appSettings = await mongoDbContext.AppSettings.ToListAsync();
        foreach (var appSettingsEntity in appSettings)
        {
            if (string.IsNullOrEmpty(appSettingsEntity.Root))
                appSettingsEntity.Root = appSettingsEntity.Field.Split(":").FirstOrDefault();
        }

        await mongoDbContext.SaveChangesAsync();
    }

    public async Task DownAsync(IMongoDatabase database)
    { }
}