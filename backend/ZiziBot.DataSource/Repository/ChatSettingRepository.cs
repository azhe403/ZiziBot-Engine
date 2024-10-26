using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using ZiziBot.Contracts.Dtos.Entity;
using ZiziBot.DataSource.MongoEf;
using ZiziBot.DataSource.MongoEf.Entities;
using ZiziBot.Types.Types;

namespace ZiziBot.DataSource.Repository;

public class ChatSettingRepository(
    ILogger<ChatSettingRepository> logger,
    ICacheService cacheService,
    MongoEfContext mongoDbContext
)
{
    public async Task<ChatRestrictionDto> GetChatRestriction(long chatId)
    {
        var cache = await cacheService.GetOrSetAsync($"{CacheKey.CHAT_RESTRICTION}{chatId}", async () => {
            var chatRestriction = await mongoDbContext.ChatRestriction.AsNoTracking()
                .Where(entity => entity.Status == EventStatus.Complete)
                .Where(entity => entity.ChatId == chatId)
                .Select(chatRestriction => new ChatRestrictionDto() {
                    ChatId = chatRestriction.ChatId,
                    UserId = chatRestriction.UserId,
                    Status = (int)chatRestriction.Status, TransactionId = chatRestriction.TransactionId,
                    CreatedDate = chatRestriction.CreatedDate,
                    UpdatedDate = chatRestriction.UpdatedDate
                })
                .FirstOrDefaultAsync();

            return chatRestriction;
        });

        return cache;
    }

    public async Task RefreshChatInfo(ChatSettingDto request)
    {
        logger.LogInformation("Ensure ChatSetting for ChatId: {ChatId} Started", request.ChatId);

        var chatSetting = await mongoDbContext.ChatSetting
            .FirstOrDefaultAsync(x => x.ChatId == request.ChatId);

        if (chatSetting == null)
        {
            logger.LogDebug("Creating fresh ChatSetting for ChatId: {ChatId}", request.ChatId);

            mongoDbContext.ChatSetting.Add(new() {
                ChatId = request.ChatId,
                ChatTitle = request.ChatTitle,
                ChatType = request.ChatType,
                ChatTypeName = request.ChatType.ToString(),
                Status = EventStatus.Complete
            });
        }
        else
        {
            logger.LogDebug("Updating ChatSetting for ChatId: {ChatId}", request.ChatId);

            chatSetting.ChatTitle = request.ChatTitle;
            chatSetting.ChatType = request.ChatType;
            chatSetting.ChatTypeName = request.ChatType.ToString();
            chatSetting.Status = EventStatus.Complete;
        }

        await mongoDbContext.SaveChangesAsync();
    }

    public async Task<WebhookChatEntity?> GetWebhookRouteById(string routeId)
    {
        var webhookChat = await mongoDbContext.WebhookChat
            .Where(entity => entity.RouteId == routeId)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        return webhookChat;
    }

    public async Task<List<ChatInfoDto>?> GetChatByBearerToken(string bearerToken)
    {
        #region Check Dashboard Session
        var dashboardSession = await mongoDbContext.DashboardSessions
            .Where(entity => entity.BearerToken == bearerToken)
            .Where(entity => entity.Status == EventStatus.Complete)
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
                entity.Status == EventStatus.Complete
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
                Status = (int)note.Status,
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
                Status = (int)note.Status,
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
            .Where(entity => entity.Status == EventStatus.Complete)
            .OrderBy(entity => entity.CityName)
            .ToListAsync();

        return cityList;
    }

    public async Task<bool> MeasureActivity(ChatActivityDto dto)
    {
        mongoDbContext.ChatActivity.Add(new() {
            ChatId = dto.ChatId,
            UserId = dto.UserId,
            ActivityType = dto.ActivityType,
            ActivityTypeName = dto.ActivityType.ToString(),
            Chat = dto.Chat,
            User = dto.User,
            Status = dto.Status,
            TransactionId = dto.TransactionId,
            MessageId = dto.MessageId
        });

        await mongoDbContext.SaveChangesAsync();

        var chatActivityEntities = await mongoDbContext.ChatActivity.AsNoTracking()
            .Where(x => x.Status == EventStatus.Complete)
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
            .Where(x => x.Status == EventStatus.Complete)
            .OrderByDescending(o => o.CreatedDate)
            .Select(x => new {
                x.ChatId,
                x.EventName,
                x.RouteId,
                x.MessageId,
                x.MessageThreadId,
                x.WebhookSource
            })
            .FirstOrDefaultAsync();

        if (lastWebhookHistory == null)
            return default;

        var lastChatActivity = await mongoDbContext.ChatActivity.AsNoTracking()
            .Where(x => x.Status == EventStatus.Complete)
            .Where(x => x.ChatId == chatId)
            .OrderByDescending(o => o.CreatedDate)
            .Select(x => new {
                x.ChatId,
                x.MessageId
            })
            .FirstOrDefaultAsync();

        if (lastChatActivity == null)
            return default;

        if (lastChatActivity.MessageId != lastWebhookHistory.MessageId)
            return default;

        logger.LogDebug("Last Webhook Message for Better Edit: {MessageId}", lastWebhookHistory.MessageId);

        return lastWebhookHistory.MessageId;
    }

    public async Task SaveGlobalBan(GlobalBanDto dto)
    {
        var globalBan = await mongoDbContext.GlobalBan
            .Where(entity => entity.UserId == dto.UserId)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (globalBan == null)
        {
            mongoDbContext.GlobalBan.Add(new() {
                UserId = dto.UserId,
                ChatId = dto.ChatId,
                Reason = dto.Reason,
                Status = EventStatus.Complete,
                TransactionId = Guid.NewGuid().ToString()
            });
        }
        else
        {
            globalBan.UserId = dto.UserId;
            globalBan.Reason = dto.Reason;
        }

        await mongoDbContext.SaveChangesAsync();
    }

    public async Task<string> SaveUserActivity(BotUserDto request)
    {
        var trackingMessage = HtmlMessage.Empty;
        var botUser = await mongoDbContext.BotUser
            .Where(entity => entity.UserId == request.UserId)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (botUser == null)
        {
            logger.LogDebug("Adding User with UserId: {UserId}", request.UserId);

            mongoDbContext.BotUser.Add(new() {
                UserId = request.UserId,
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                LanguageCode = request.LanguageCode,
                Status = EventStatus.Complete,
                TransactionId = request.TransactionId
            });
        }
        else
        {
            logger.LogDebug("Updating User with UserId: {UserId}", request.UserId);

            if (botUser.FirstName != request.FirstName)
                trackingMessage.TextBr("Mengubah nama depannya");

            if (botUser.LastName != request.LastName)
                trackingMessage.TextBr("Mengubah nama belakangnya");

            if (botUser.Username != request.Username)
                trackingMessage.TextBr("Mengubah username-nya");

            botUser.UserId = request.UserId;
            botUser.Username = request.Username;
            botUser.FirstName = request.FirstName;
            botUser.LastName = request.LastName;
            botUser.LanguageCode = request.LanguageCode;
            botUser.Status = EventStatus.Complete;
            botUser.TransactionId = request.TransactionId;
        }

        await mongoDbContext.SaveChangesAsync();

        if (trackingMessage.IsEmpty)
            return string.Empty;

        var message = HtmlMessage.Empty
            .Bold("Pengguna: ").User(request.User).Br()
            .Append(trackingMessage);

        return message.ToString();
    }

    public async Task<bool> IsSudoAsync(long userId)
    {
        var sudoers = await GetSudoers();

        var isSudo = sudoers.Any(x => x.UserId == userId);
        logger.LogDebug("UserId: {UserId} is Sudo: {IsSudo}", userId, isSudo);

        return isSudo;
    }

    public async Task<List<SudoerEntity>> GetSudoers()
    {
        var cache = await cacheService.GetOrSetAsync(
            CacheKey.GLOBAL_SUDO,
            async () => {
                return await mongoDbContext.Sudoers.AsNoTracking()
                    .Where(x => x.Status == EventStatus.Complete)
                    .ToListAsync();
            }
        );

        return cache;
    }

    public async Task<ServiceResult> SaveSudo(SudoerEntity entity)
    {
        ServiceResult serviceResult = new();

        var findSudo = await mongoDbContext.Sudoers
            .FirstOrDefaultAsync(x => x.UserId == entity.UserId);

        if (findSudo != null)
        {
            return serviceResult.Complete("This user is already a sudoer.");
        }

        mongoDbContext.Sudoers.Add(entity);
        await mongoDbContext.SaveChangesAsync();

        return serviceResult.Complete("Sudoer added successfully.");
    }
}