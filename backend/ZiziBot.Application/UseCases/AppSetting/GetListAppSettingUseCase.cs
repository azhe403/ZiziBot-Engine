using Microsoft.EntityFrameworkCore;

namespace ZiziBot.Application.UseCases.AppSetting;

public class GetListAppSettingRequest
{
    public AvailabilityStatus AvailabilityStatus { get; set; }
    public ConfigRoot Root { get; set; }
    public string? Search { get; set; }
}

public class GetListAppSettingResponse
{
    public required string Root { get; set; }
    public required string Name { get; set; }
    public string? InitialValue { get; set; }
    public string? DefaultValue { get; set; }
    public string? Value { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

public class GetListAppSettingUseCase(AppSettingRepository appSettingRepository, DataFacade dataFacade)
{
    private readonly ApiResponseBase<List<GetListAppSettingResponse>> _response = new();

    public async Task<ApiResponseBase<List<GetListAppSettingResponse>>> Handle(GetListAppSettingRequest request)
    {
        var appSettingsEntities = await dataFacade.MongoEf.AppSettings.AsNoTracking()
            .Select(x => new GetListAppSettingResponse {
                Root = x.Root ?? string.Empty,
                Name = x.Name,
                InitialValue = x.InitialValue,
                DefaultValue = x.DefaultValue,
                Value = x.Value,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
            }).ToListAsync();

        return _response.Success("Get List App Setting Success", appSettingsEntities)
            .SetMetadata(appSettingsEntities.Count);
    }
}