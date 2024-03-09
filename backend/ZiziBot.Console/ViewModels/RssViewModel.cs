using MongoFramework.Linq;

namespace ZiziBot.Console.ViewModels;

public class RssViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly MongoDbContextBase _mongoDbContextBase;
    private readonly ChatSettingRepository _chatSettingRepository;

    public RssViewModel(ProtectedLocalStorage protectedLocalStorage, ChatSettingRepository chatSettingRepository, MongoDbContextBase mongoDbContextBase)
    {
        _protectedLocalStorage = protectedLocalStorage;
        _chatSettingRepository = chatSettingRepository;
        _mongoDbContextBase = mongoDbContextBase;
    }

    public async Task<IEnumerable<string>> GetChat()
    {
        var bearerToken = await _protectedLocalStorage.GetAsync<string>("bearer_token");
        if (bearerToken.Value == null)
            return default;

        var listChat = await _chatSettingRepository.GetChatByBearerToken(bearerToken.Value);
        return listChat.Select(dto => dto.ChatTitle);
    }

    public async Task<List<RssSettingEntity>> GetRss(long chatId)
    {
        var data = await _mongoDbContextBase.RssSetting.AsNoTracking()
            .Where(entity => entity.ChatId == chatId)
            .ToListAsync();

        return data;
    }

    public async Task<List<RssHistoryEntity>> GetRssHistory(long chatId)
    {
        var data = await _mongoDbContextBase.RssHistory.AsNoTracking()
            .Where(entity => entity.ChatId == chatId)
            .ToListAsync();

        return data;
    }
}