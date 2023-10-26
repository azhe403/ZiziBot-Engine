using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.Rss;

public class SaveRssRequest : ApiRequestBase<bool>
{
    [FromBody]
    public SaveRssRequestBody Body { get; set; }
}

public class SaveRssRequestBody
{
    public string Url { get; set; }
    public long ChatId { get; set; }
}

public class SaveRssValidation : AbstractValidator<SaveRssRequest>
{
    public SaveRssValidation()
    {
        RuleFor(x => x.Body.ChatId).NotEqual(0).WithMessage("ChatId is Required");
        RuleFor(x => x.Body.Url).NotEmpty().WithMessage("URL is required");
    }
}

public class SaveRssHandler : IRequestHandler<SaveRssRequest, ApiResponseBase<bool>>
{
    private readonly MongoDbContextBase _mongoDbContext;

    public SaveRssHandler(MongoDbContextBase mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<ApiResponseBase<bool>> Handle(SaveRssRequest request, CancellationToken cancellationToken)
    {
        ApiResponseBase<bool> response = new();

        if (!request.ListChatId.Contains(request.Body.ChatId))
        {
            return response.BadRequest($"Kamu tidak mempunyai akses ke ChatId: {request.Body.ChatId}");
        }

        var rss = await _mongoDbContext.RssSetting
            .Where(entity => entity.ChatId == request.Body.ChatId)
            .Where(entity => entity.RssUrl == request.Body.Url)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (rss == null)
        {
            _mongoDbContext.RssSetting.Add(new RssSettingEntity()
            {
                RssUrl = request.Body.Url,
                ChatId = request.Body.ChatId,
                Status = (int)EventStatus.Complete
            });
        }
        else
        {

        }

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        return response.Success("RSS Berhasil disimpan", true);
    }
}