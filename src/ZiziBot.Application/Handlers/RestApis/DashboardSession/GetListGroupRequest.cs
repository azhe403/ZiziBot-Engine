using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.DashboardSession;

public class GetListGroupRequest : ApiRequestBase<List<GetListGroupResponse>?>
{
}

public class GetListGroupResponse
{
    public long ChatId { get; set; }
    public string ChatTitle { get; set; }
}

public class GetListGroupHandler : IRequestHandler<GetListGroupRequest, ApiResponseBase<List<GetListGroupResponse>?>>
{
    private readonly ChatDbContext _chatDbContext;
    private readonly UserDbContext _userDbContext;

    public GetListGroupHandler(ChatDbContext chatDbContext, UserDbContext userDbContext)
    {
        _chatDbContext = chatDbContext;
        _userDbContext = userDbContext;
    }

    public async Task<ApiResponseBase<List<GetListGroupResponse>?>> Handle(GetListGroupRequest request, CancellationToken cancellationToken)
    {
        ApiResponseBase<List<GetListGroupResponse>?> response = new();

        #region Check Dashboard Session

        var dashboardSession = await _userDbContext.DashboardSessions
            .Where(entity =>
                entity.BearerToken == request.BearerToken &&
                entity.Status == (int)EventStatus.Complete
            )
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (dashboardSession == null)
        {
            return response.Unauthorized("Session not found");
        }

        var userId = dashboardSession.TelegramUserId;

        #endregion

        var chatAdmin = await _chatDbContext.ChatAdmin
            .Where(entity =>
                entity.UserId == userId &&
                entity.Status == (int)EventStatus.Complete
            )
            .ToListAsync(cancellationToken: cancellationToken);

        var chatIds = chatAdmin.Select(y => y.ChatId);

        var listChatSetting = await _chatDbContext.ChatSetting
            .Where(x => chatIds.Contains(x.ChatId))
            .ToListAsync(cancellationToken: cancellationToken);

        var listPermission = chatAdmin
            .Join(listChatSetting, adminEntity => adminEntity.ChatId, settingEntity => settingEntity.ChatId, (adminEntity, settingEntity) => new GetListGroupResponse()
            {
                ChatId = adminEntity.ChatId,
                ChatTitle = settingEntity.ChatTitle
            })
            .ToList();

        return response.Success("Get user permission successfully", listPermission);
    }
}