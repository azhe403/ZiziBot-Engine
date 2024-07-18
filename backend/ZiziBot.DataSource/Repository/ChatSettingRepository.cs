using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.DataSource.Repository;

public class ChatSettingRepository(
    ILogger<ChatSettingRepository> logger,
    ICacheService cacheService,
    MongoDbContextBase mongoDbContext
)
{
    public async Task<ChatRestrictionEntity?> GetChatRestriction(long chatId)
    {
        var cache = await cacheService.GetOrSetAsync($"{CacheKey.CHAT_RESTRICTION}{chatId}", async () => {
            var check = await mongoDbContext.ChatRestriction.AsNoTracking()
                .Where(entity => entity.Status == (int)EventStatus.Complete)
                .Where(entity => entity.ChatId == chatId)
                .FirstOrDefaultAsync();

            return check;
        });

        return cache;
    }

    public async Task<WebhookChatEntity?> GetWebhookRouteById(string routeId)
    {
        var webhookChat = await mongoDbContext.WebhookChat
            .Where(entity => entity.RouteId == routeId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();
        return webhookChat;
    }

    public async Task<List<ChatInfoDto>?> GetChatByBearerToken(string bearerToken)
    {
        #region Check Dashboard Session

        var dashboardSession = await mongoDbContext.DashboardSessions
            .Where(entity => entity.BearerToken == bearerToken)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (dashboardSession == null)
        {
            return null;
        }

        var userId = dashboardSession.TelegramUserId;

        #endregion

        var chatAdmin = await mongoDbContext.ChatAdmin
            .Where(entity =>
                entity.UserId == userId &&
                entity.Status == (int)EventStatus.Complete
            )
            .ToListAsync();

        if (chatAdmin.Count == 0)
        {
            return null;
        }

        var chatIds = chatAdmin.Select(y => y.ChatId);

        var listChatSetting = await mongoDbContext.ChatSetting
            .Where(x => chatIds.Contains(x.ChatId))
            .ToListAsync();

        List<ChatInfoDto>? listPermission = new();
        // listPermission.Add(new ChatInfoDto()
        // {
        //     ChatId = request.SessionUserId,
        //     ChatTitle = "Saya"
        // });

        var listGroup = chatAdmin
            .Join(listChatSetting, adminEntity => adminEntity.ChatId,
                settingEntity => settingEntity.ChatId, (adminEntity, settingEntity) => new ChatInfoDto() {
                    ChatId = adminEntity.ChatId,
                    ChatTitle = settingEntity.ChatTitle
                })
            .DistinctBy(entity => entity.ChatId)
            .OrderBy(res => res.ChatTitle)
            .ToList();

        listPermission.AddRange(listGroup);

        return listPermission;
    }

    public async Task<List<NoteDto>> GetListNote(long chatId)
    {
        var listNoteEntity = await mongoDbContext.Note
            .AsNoTracking()
            .Where(entity => entity.ChatId == chatId)
            .Join(mongoDbContext.ChatSetting, note => note.ChatId, chat => chat.ChatId, (note, chat) => new NoteDto() {
                Id = note.Id.ToString(),
                ChatId = note.ChatId,
                ChatTitle = chat.ChatTitle,
                Query = note.Query,
                Text = note.Content,
                RawButton = note.RawButton,
                FileId = note.FileId,
                DataType = note.DataType,
                Status = note.Status,
                TransactionId = note.TransactionId,
                CreatedDate = chat.CreatedDate,
                UpdatedDate = chat.UpdatedDate
            })
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .OrderBy(entity => entity.Query)
            .ToListAsync();

        return listNoteEntity;
    }

    public async Task<NoteDto> GetNote(string noteId)
    {
        var listNoteEntity = await mongoDbContext.Note
            .AsNoTracking()
            .Where(entity => entity.Id == new ObjectId(noteId))
            .Join(mongoDbContext.ChatSetting, note => note.ChatId, chat => chat.ChatId, (note, chat) => new NoteDto() {
                Id = note.Id.ToString(),
                ChatId = note.ChatId,
                ChatTitle = chat.ChatTitle,
                Query = note.Query,
                Text = note.Content,
                RawButton = note.RawButton,
                FileId = note.FileId,
                DataType = note.DataType,
                Status = note.Status,
                TransactionId = note.TransactionId,
                CreatedDate = chat.CreatedDate,
                UpdatedDate = chat.UpdatedDate
            })
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();

        return listNoteEntity;
    }

    public async Task<List<BangHasan_ShalatCityEntity>> GetShalatCity(long chatId)
    {
        var cityList = await mongoDbContext.BangHasan_ShalatCity
            .Where(entity => entity.ChatId == chatId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .OrderBy(entity => entity.CityName)
            .ToListAsync();

        return cityList;
    }

    public async Task<bool> MeasureActivity(ChatActivityDto dto)
    {
        mongoDbContext.ChatActivity.Add(new ChatActivityEntity {
            ChatId = dto.ChatId,
            UserId = dto.UserId,
            ActivityType = dto.ActivityType,
            ActivityTypeName = dto.ActivityType.ToString(),
            Chat = dto.Chat,
            User = dto.User,
            Status = (int)dto.Status,
            TransactionId = dto.TransactionId,
            MessageId = dto.MessageId
        });

        await mongoDbContext.SaveChangesAsync();

        var chatActivityEntities = await mongoDbContext.ChatActivity.AsNoTracking()
            .Where(x => x.Status == (int)EventStatus.Complete)
            .Where(x => x.ActivityType == ChatActivityType.NewChatMember)
            .Where(x => x.CreatedDate > DateTime.UtcNow.Add(-ValueConst.NEW_MEMBER_RAID_SLIDING_WINDOW))
            .ToListAsync();

        logger.LogInformation("Chat Activity Count: {Count} in a window {Window}",
            chatActivityEntities.Count, ValueConst.NEW_MEMBER_RAID_SLIDING_WINDOW);

        return chatActivityEntities.Count > ValueConst.NEW_MEMBER_RAID_WINDOW_LIMIT;
    }

    public async Task<int> LastWebhookMessageBetterEdit(long chatId, WebhookSource webhookSource, string eventName)
    {
        if (eventName.Like("push"))
            return default;

        var lastWebhookHistory = await mongoDbContext.WebhookHistory.AsNoTracking()
            .Where(x => x.ChatId == chatId)
            .Where(x => x.WebhookSource == webhookSource)
            .Where(x => x.EventName == eventName)
            .Where(x => x.Status == (int)EventStatus.Complete)
            .OrderByDescending(o => o.CreatedDate)
            .FirstOrDefaultAsync();

        var lastChatActivity = await mongoDbContext.ChatActivity.AsNoTracking()
            .Where(x => x.Status == (int)EventStatus.Complete)
            .Where(x => x.ChatId == chatId)
            .OrderByDescending(o => o.CreatedDate)
            .FirstOrDefaultAsync();

        if (lastChatActivity?.MessageId != lastWebhookHistory?.MessageId)
            return default;

        logger.LogDebug("Last Webhook Message for Better Edit: {MessageId}", lastWebhookHistory.MessageId);
        return lastWebhookHistory.MessageId;
    }
}