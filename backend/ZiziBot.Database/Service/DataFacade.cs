using ZiziBot.Database.MongoDb;
using ZiziBot.Database.Repository;

namespace ZiziBot.Database.Service;

public class DataFacade(
    CacheService cache,
    MongoDbContext mongoDb,
    AdditionalRepository additional,
    AppSettingRepository appSetting,
    BotRepository botRepository,
    FeatureFlagRepository featureFlag,
    ChatSettingRepository chatSetting,
    GroupRepository group,
    RssRepository rss,
    MirrorUserRepository mirrorUser,
    WordFilterRepository wordFilter
)
{
    public CacheService Cache => cache;
    public MongoDbContext MongoDb => mongoDb;
    public AdditionalRepository Additional => additional;
    public AppSettingRepository AppSetting => appSetting;
    public BotRepository Bot => botRepository;
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