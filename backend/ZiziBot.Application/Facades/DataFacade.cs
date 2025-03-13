using ZiziBot.DataSource.MongoEf;
using ZiziBot.Interfaces;

namespace ZiziBot.Application.Facades;

public class DataFacade(
    ICacheService cacheService,
    MongoEfContext mongoEf,
    AdditionalRepository additionalRepository,
    AppSettingRepository appSettingRepository,
    FeatureFlagRepository featureFlagRepository,
    ChatSettingRepository chatSettingRepository,
    GroupRepository groupRepository,
    RssRepository rssRepository,
    MirrorUserRepository mirrorUserRepository,
    WordFilterRepository wordFilterRepository
) : IDataFacade
{
    public ICacheService Cache => cacheService;
    public MongoEfContext MongoEf => mongoEf;
    public AdditionalRepository Additional => additionalRepository;
    public AppSettingRepository AppSetting => appSettingRepository;
    public FeatureFlagRepository FeatureFlag => featureFlagRepository;
    public ChatSettingRepository ChatSetting => chatSettingRepository;
    public GroupRepository Group => groupRepository;
    public RssRepository Rss => rssRepository;
    public MirrorUserRepository MirrorUser => mirrorUserRepository;
    public WordFilterRepository WordFilter => wordFilterRepository;
}