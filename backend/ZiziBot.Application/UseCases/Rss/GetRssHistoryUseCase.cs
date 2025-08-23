using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ZiziBot.Database.MongoDb;

namespace ZiziBot.Application.UseCases.Rss;

public class GetRssHistoryRequest
{
    public long ChatId { get; set; }
    public int? ThreadId { get; set; }
    public string? RssUrl { get; set; }
    public HistoryType? HistoryType { get; set; }
}

public class GetRssHistoryValidator : AbstractValidator<GetRssHistoryRequest>
{
    public GetRssHistoryValidator()
    {
        RuleFor(x => x.ChatId).NotEqual(0).WithMessage("ChatId is required");
    }
}

public class GetRssHistoryResponse
{
    public long ChatId { get; set; }
    public int? ThreadId { get; set; }
    public string? RssUrl { get; set; }
    public required string? ErrorMessage { get; set; }
    public DateTime PublishDate { get; set; }
    public required string? ArticleUrl { get; set; }
    public string? ArticleTitle { get; set; }
    public string? ArticleAuthor { get; set; }
    public HistoryType HistoryType { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class GetRssHistoryUseCase(MongoDbContext mongoDbContext)
{
    public async Task<ApiResponseBase<List<GetRssHistoryResponse>>> Handle(GetRssHistoryRequest request)
    {
        var response = ApiResponse.Create<List<GetRssHistoryResponse>>();

        var query = await mongoDbContext.RssHistory.AsNoTracking()
            .Where(x => x.ChatId == request.ChatId)
            .WhereIf(request.ThreadId.HasValue, e => e.ThreadId == request.ThreadId)
            .WhereIf(!string.IsNullOrWhiteSpace(request.RssUrl), e => e.RssUrl == request.RssUrl)
            .WhereIf(request.HistoryType.HasValue, e => e.HistoryType == request.HistoryType)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();

        var data = query.Select(e => new GetRssHistoryResponse
        {
            ChatId = e.ChatId,
            ThreadId = e.ThreadId,
            RssUrl = e.RssUrl,
            ErrorMessage = e.ErrorMessage,
            ArticleUrl = e.Url,
            ArticleTitle = e.Title,
            ArticleAuthor = e.Author,
            PublishDate = e.PublishDate,
            CreatedDate = e.CreatedDate,
            HistoryType = e.HistoryType ?? HistoryType.Unknown
        }).ToList();

        return response.Success("OK", data);
    }
}