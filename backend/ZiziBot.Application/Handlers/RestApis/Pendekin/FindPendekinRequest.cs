using Microsoft.EntityFrameworkCore;

namespace ZiziBot.Application.Handlers.RestApis.Pendekin;

public class ListPendekinRequest : ApiRequestBase<List<ListPendekinResponse>>
{ }

public class ListPendekinResponse
{
    public string PendekinId { get; set; }
    public string ShortPath { get; set; }
    public string ShortUrl { get; set; }
    public string OriginalUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

public class ListPendekinHandler(
    DataFacade dataFacade
) : IApiRequestHandler<ListPendekinRequest, List<ListPendekinResponse>>
{
    public async Task<ApiResponseBase<List<ListPendekinResponse>>> Handle(ListPendekinRequest request, CancellationToken cancellationToken)
    {
        var pendekinConfig = await dataFacade.AppSetting.GetConfigSectionAsync<PendekinConfig>();
        if (pendekinConfig == null)
            return ApiResponse.ReturnBadRequest<List<ListPendekinResponse>>("Pendekin not yet prepared");

        var listPendekin = await dataFacade.MongoEf.PendekinMap.AsNoTracking()
            .Select(x => new ListPendekinResponse() {
                PendekinId = x.Id.ToString(),
                ShortPath = x.ShortPath,
                OriginalUrl = x.OriginalUrl,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate
            })
            .OrderByDescending(x => x.UpdatedDate).ThenByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken: cancellationToken);

        listPendekin.ForEach(x => {
            x.ShortUrl = !string.IsNullOrWhiteSpace(pendekinConfig.RouterBaseUrl) ? $"{pendekinConfig.RouterBaseUrl}/{x.ShortPath}" : "";
        });

        return ApiResponse.ReturnSuccess("Get list Pendekin successfully", listPendekin);
    }
}