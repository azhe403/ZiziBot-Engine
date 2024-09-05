using Microsoft.EntityFrameworkCore;
using ZiziBot.Application.Facades;

public class ListPendekinRequest : ApiRequestBase<List<ListPendekinResponse>>
{ }

public class ListPendekinResponse
{
    public string ShortPath { get; set; }
    public string OriginalUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

public class ListPendekinHandler(DataFacade dataFacade) : IApiRequestHandler<ListPendekinRequest, List<ListPendekinResponse>>
{
    public async Task<ApiResponseBase<List<ListPendekinResponse>>> Handle(ListPendekinRequest request, CancellationToken cancellationToken)
    {
        var pendekinList = await dataFacade.MongoEf.PendekinMap.ToListAsync(cancellationToken: cancellationToken);

        var data = pendekinList.Select(x => new ListPendekinResponse() {
            ShortPath = x.ShortPath,
            OriginalUrl = x.OriginalUrl,
            CreatedDate = x.CreatedDate,
            UpdatedDate = x.UpdatedDate
        }).ToList();

        return ApiResponseBase.ReturnSuccess("Get list Pendekin successfully", data);
    }
}