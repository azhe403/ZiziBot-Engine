using Microsoft.EntityFrameworkCore;

namespace ZiziBot.Application.Handlers.RestApis.DashboardSession;

public class GetListGroupRequest : ApiRequestBase<List<ChatInfoDto>?>
{ }

public class GetListGroupHandler(
    DataFacade dataFacade
) : IApiRequestHandler<GetListGroupRequest, List<ChatInfoDto>?>
{
    public async Task<ApiResponseBase<List<ChatInfoDto>?>> Handle(GetListGroupRequest request, CancellationToken cancellationToken)
    {
        ApiResponseBase<List<ChatInfoDto>?> response = new();

        #region Check Dashboard Session
        var dashboardSession = await dataFacade.MongoEf.DashboardSessions
            .Where(entity => entity.BearerToken == request.BearerToken)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (dashboardSession == null)
        {
            return response.Unauthorized("Session not found");
        }

        var userId = dashboardSession.TelegramUserId;
        #endregion

        var chatAdmin = await dataFacade.MongoEf.ChatAdmin
            .Where(entity => entity.UserId == userId)
            .Where(entity => entity.Status == EventStatus.Complete)
            .ToListAsync(cancellationToken: cancellationToken);

        if (chatAdmin.Count == 0)
        {
            return response.Success("Get user permission successfully", null);
        }

        var chatIds = chatAdmin.Select(y => y.ChatId);

        var listChatSetting = await dataFacade.MongoEf.ChatSetting
            .Where(x => chatIds.Contains(x.ChatId))
            .ToListAsync(cancellationToken: cancellationToken);

        List<ChatInfoDto> listPermission = new();
        listPermission.Add(new ChatInfoDto() {
            ChatId = request.SessionUserId,
            ChatTitle = "Saya"
        });

        var listGroup = chatAdmin
            .Join(listChatSetting, adminEntity => adminEntity.ChatId, settingEntity => settingEntity.ChatId, (adminEntity, settingEntity) => new ChatInfoDto() {
                ChatId = adminEntity.ChatId,
                ChatTitle = settingEntity.ChatTitle
            })
            .DistinctBy(entity => entity.ChatId)
            .OrderBy(res => res.ChatTitle)
            .ToList();

        listPermission.AddRange(listGroup);

        return response.Success("Get user permission successfully", listPermission);
    }
}