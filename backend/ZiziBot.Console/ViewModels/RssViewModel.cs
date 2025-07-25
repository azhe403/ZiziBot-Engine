using Microsoft.EntityFrameworkCore;
using ZiziBot.Database;
using ZiziBot.Database.MongoDb.Entities;

namespace ZiziBot.Console.ViewModels;

public class RssViewModel(
    ProtectedLocalStorage protectedLocalStorage,
    DataFacade dataFacade
)
    : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public async Task<IEnumerable<string>> GetChat()
    {
        var bearerToken = await protectedLocalStorage.GetAsync<string>("bearer_token");
        if (bearerToken.Value == null)
            return default;

        var listChat = await dataFacade.ChatSetting.GetChatByBearerToken(bearerToken.Value);
        return listChat.Select(dto => dto.ChatTitle);
    }

    public async Task<List<RssSettingEntity>> GetRss(long chatId)
    {
        var data = await dataFacade.MongoDb.RssSetting.AsNoTracking()
            .Where(entity => entity.ChatId == chatId)
            .ToListAsync();

        return data;
    }

    public async Task<List<RssHistoryEntity>> GetRssHistory(long chatId)
    {
        var data = await dataFacade.MongoDb.RssHistory.AsNoTracking()
            .Where(entity => entity.ChatId == chatId)
            .ToListAsync();

        return data;
    }
}