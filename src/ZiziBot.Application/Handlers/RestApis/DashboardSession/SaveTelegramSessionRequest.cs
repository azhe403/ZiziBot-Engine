using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ZiziBot.Application.Handlers.RestApis.DashboardSession;

public class SaveTelegramSessionRequestModel : IRequest<bool>
{
    [JsonProperty("id")]
    public long TelegramUserId { get; set; }

    [JsonProperty("first_name")]
    public string FirstName { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("photo_url")]
    public string PhotoUrl { get; set; }

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public long AuthDate { get; set; }

    [JsonProperty("hash")]
    public string Hash { get; set; }

    [JsonProperty("session_id")]
    public string SessionId { get; set; }
}

public class SaveTelegramSessionRequestHandler : IRequestHandler<SaveTelegramSessionRequestModel, bool>
{
    private readonly UserDbContext _userDbContext;

    public SaveTelegramSessionRequestHandler(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public async Task<bool> Handle(SaveTelegramSessionRequestModel request, CancellationToken cancellationToken)
    {
        _userDbContext.DashboardSessions.Add(
            new DataSource.MongoDb.Entities.DashboardSession()
            {
                TelegramUserId = request.TelegramUserId,
                FirstName = request.FirstName,
                PhotoUrl = request.PhotoUrl,
                Username = request.Username,
                AuthDate = request.AuthDate,
                Hash = request.Hash,
                SessionId = request.SessionId,
                Status = (int) EventStatus.Complete
            }
        );

        await _userDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}