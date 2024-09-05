using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZiziBot.Application.Facades;

public class GetPendekinRequest : ApiRequestBase<GetPendekinResponse>
{
    [FromRoute]
    public string ShortPath { get; set; }
}

public class GetPendekinRequestValidator : AbstractValidator<GetPendekinRequest>
{
    public GetPendekinRequestValidator()
    {
        RuleFor(x => x.ShortPath).NotNull().NotEmpty();
    }
}

public class GetPendekinResponse
{
    public string OriginalUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

public class GetPendekinHandler(DataFacade dataFacade) : IApiRequestHandler<GetPendekinRequest, GetPendekinResponse>
{
    public async Task<ApiResponseBase<GetPendekinResponse>> Handle(GetPendekinRequest request, CancellationToken cancellationToken)
    {
        var pendekinMap = await dataFacade.MongoEf.PendekinMap.FirstOrDefaultAsync(x => x.ShortPath == request.ShortPath, cancellationToken);

        if (pendekinMap == null)
            return ApiResponseBase.ReturnBadRequest<GetPendekinResponse>("Pendekin not found");

        return ApiResponseBase.ReturnSuccess("Get Pendekin successfully", new GetPendekinResponse() {
            OriginalUrl = pendekinMap.OriginalUrl,
            CreatedDate = pendekinMap.CreatedDate,
            UpdatedDate = pendekinMap.UpdatedDate
        });
    }
}