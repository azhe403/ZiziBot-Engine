using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Application.Handlers.RestApis.Pendekin;

public class CreatePendekinRequest : ApiPostRequestBase<CreatePendekinRequestBody, CreatePendekinResponse>
{ }

public class CreatePendekinRequestBody
{
    public string OriginalUrl { get; set; }
    public string? ShortPath { get; set; }
}

public class CreatePendekinValidation : AbstractValidator<CreatePendekinRequest>
{
    public CreatePendekinValidation()
    {
        RuleFor(x => x.Body).NotNull();

        When(x => x.Body != null, () => {
            RuleFor(x => x.Body.OriginalUrl).NotEmpty();
        });
    }
}

public class CreatePendekinResponse
{
    public string OriginalUrl { get; set; }
    public string ShortUrl { get; set; }
    public string ShortPath { get; set; }
}

public class CreatePendekinHandler(
    DataFacade dataFacade
) : IApiRequestHandler<CreatePendekinRequest, CreatePendekinResponse>
{
    public async Task<ApiResponseBase<CreatePendekinResponse>> Handle(CreatePendekinRequest request, CancellationToken cancellationToken)
    {
        var response = ApiResponse.Create<CreatePendekinResponse>();
        var shortPath = request.Body.ShortPath.IsNullOrWhiteSpace() ? StringUtil.GetNanoId() : request.Body.ShortPath;

        var pendekinMap = await dataFacade.MongoEf.PendekinMap.FirstOrDefaultAsync(x => x.ShortPath == shortPath);

        if (pendekinMap != null)
            return response.BadRequest("Pendekin Path is already exist");

        var pendekinConfig = await dataFacade.AppSetting.GetConfigSectionAsync<PendekinConfig>();

        if (pendekinConfig == null)
            return response.BadRequest("Pendekin not yet prepared");

        dataFacade.MongoEf.PendekinMap.Add(new PendekinMapEntity() {
            OriginalUrl = request.Body.OriginalUrl,
            ShortPath = shortPath,
            Status = EventStatus.Complete,
            CreatedBy = request.SessionUserId,
            UpdatedBy = request.SessionUserId,
            TransactionId = request.TransactionId
        });

        await dataFacade.MongoEf.SaveChangesAsync(cancellationToken);

        return response.Success("Create Pendekin successfully", new CreatePendekinResponse() {
            OriginalUrl = request.Body.OriginalUrl,
            ShortUrl = pendekinConfig.RouterBaseUrl.IsNotNullOrWhiteSpace() ? $"{pendekinConfig.RouterBaseUrl}/{shortPath}" : string.Empty,
            ShortPath = shortPath
        });
    }
}