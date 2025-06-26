using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ZiziBot.Database.MongoDb.Interfaces;

namespace ZiziBot.Database.MongoDb.Migrations;

public class RefreshAppSettingMigration(MongoDbContext mongoDbContext) : IPreMigration
{
    public async Task UpAsync(IMongoDatabase db)
    {
        var appSettings = await mongoDbContext.AppSettings.ToListAsync();
        foreach (var appSettingsEntity in appSettings)
        {
            if (string.IsNullOrEmpty(appSettingsEntity.Root))
                appSettingsEntity.Root = appSettingsEntity.Field.Split(":").FirstOrDefault();
        }

        await mongoDbContext.SaveChangesAsync();
    }

    public async Task DownAsync(IMongoDatabase db)
    { }
}