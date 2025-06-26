using Microsoft.Extensions.DependencyInjection;
using ZiziBot.Common.Interfaces;
using ZiziBot.Database.MongoDb;
using ZiziBot.Database.Repository;

namespace ZiziBot.Database;

public class DataFacade(IServiceProvider serviceProvider) : IDataFacade
{
    public ICacheService Cache => serviceProvider.GetRequiredService<ICacheService>();
    public MongoDbContext MongoDb => serviceProvider.GetRequiredService<MongoDbContext>();
    public AdditionalRepository Additional => serviceProvider.GetRequiredService<AdditionalRepository>();
    public AppSettingRepository AppSetting => serviceProvider.GetRequiredService<AppSettingRepository>();
    public FeatureFlagRepository FeatureFlag => serviceProvider.GetRequiredService<FeatureFlagRepository>();
    public ChatSettingRepository ChatSetting => serviceProvider.GetRequiredService<ChatSettingRepository>();
    public GroupRepository Group => serviceProvider.GetRequiredService<GroupRepository>();
    public RssRepository Rss => serviceProvider.GetRequiredService<RssRepository>();
    public MirrorUserRepository MirrorUser => serviceProvider.GetRequiredService<MirrorUserRepository>();
    public WordFilterRepository WordFilter => serviceProvider.GetRequiredService<WordFilterRepository>();

    public async Task<int> SaveChangesAsync()
    {
        return await MongoDb.SaveChangesAsync();
    }
}