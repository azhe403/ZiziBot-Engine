using ZiziBot.Common.Interfaces;
using ZiziBot.Database.MongoDb;
using ZiziBot.Database.Repository;

namespace ZiziBot.Database;

public class DataFacade(
    ICacheService cache,
    MongoDbContext mongoDb,
    AdditionalRepository additional,
    AppSettingRepository appSetting,
    FeatureFlagRepository featureFlag,
    ChatSettingRepository chatSetting,
    GroupRepository group,
    RssRepository rss,
    MirrorUserRepository mirrorUser,
    WordFilterRepository wordFilter
) : IDataFacade
{
    public ICacheService Cache => cache;
    public MongoDbContext MongoDb => mongoDb;
    public AdditionalRepository Additional => additional;
    public AppSettingRepository AppSetting => appSetting;
    public FeatureFlagRepository FeatureFlag => featureFlag;
    public ChatSettingRepository ChatSetting => chatSetting;
    public GroupRepository Group => group;
    public RssRepository Rss => rss;
    public MirrorUserRepository MirrorUser => mirrorUser;
    public WordFilterRepository WordFilter => wordFilter;

    public async Task<int> SaveChangesAsync()
    {
        return await MongoDb.SaveChangesAsync();
    }
}