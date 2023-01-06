using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ZiziBot.Application.Pipelines;

public class SaveTelegramSessionRequestModel : IRequest<bool>
{
    public long Id { get; set; }

    [FromQuery(Name = "first_name")]
    public string FirstName { get; set; }

    public string Username { get; set; }

    [FromQuery(Name = "photo_url")]
    public string PhotoUrl { get; set; }

    [FromQuery(Name = "auth_date")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public long AuthDate { get; set; }

    public string Hash { get; set; }
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
         _userDbContext.DashboardSessions.Add(new DashboardSession()
        {
            TelegramUserId = request.Id,
            FirstName = request.FirstName,
            PhotoUrl = request.PhotoUrl,
            Username = request.Username,
            AuthDate = request.AuthDate,
            Hash = request.Hash,
            Status = (int)EventStatus.Complete
        });

         await _userDbContext.SaveChangesAsync(cancellationToken);

         return true;
    }
}