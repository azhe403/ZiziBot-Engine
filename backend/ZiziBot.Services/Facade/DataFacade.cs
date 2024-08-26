using ZiziBot.DataSource.MongoDb;
using ZiziBot.DataSource.MongoEf;

namespace ZiziBot.Services.Facade;

public class DataFacade(
    ICacheService cacheService,
    MongoDbContextBase mongoDb,
    MongoEfContext mongoEf,
    AppSettingRepository appSettingRepository,
    AdditionalRepository additionalRepository,
    ChatSettingRepository chatSettingRepository,
    GroupRepository groupRepository,
    MirrorUserRepository mirrorUserRepository,
    WordFilterRepository wordFilterRepository
)
{
    public ICacheService Cache = cacheService;

    public MongoDbContextBase MongoDb => mongoDb;
    public MongoEfContext MongoEf => mongoEf;

    public AppSettingRepository AppSetting => appSettingRepository;
    public AdditionalRepository Additional => additionalRepository;
    public ChatSettingRepository ChatSetting => chatSettingRepository;
    public GroupRepository Group => groupRepository;
    public MirrorUserRepository MirrorUser => mirrorUserRepository;
    public WordFilterRepository WordFilter => wordFilterRepository;
}