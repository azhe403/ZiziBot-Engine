using System.Text.Json.Serialization;
using FluentValidation;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class WebHookTrakteerDonationRequest : ApiPostRequestBase<WebHookTrakteerDonationRequestBody, WebHookTrakteerDonationResponse>
{ }

public class WebHookTrakteerDonationRequestBody
{
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("transaction_id")]
    public string TransactionId { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("supporter_name")]
    public string SupporterName { get; set; }

    [JsonPropertyName("supporter_avatar")]
    public string SupporterAvatar { get; set; }

    [JsonPropertyName("supporter_message")]
    public string SupporterMessage { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; }

    [JsonPropertyName("unit_icon")]
    public string UnitIcon { get; set; }

    [JsonPropertyName("quantity")]
    public long Quantity { get; set; }

    [JsonPropertyName("price")]
    public long Price { get; set; }

    [JsonPropertyName("net_amount")]
    public long NetAmount { get; set; }
}

public class WebHookTrakteerDonationRequestValidator : AbstractValidator<WebHookTrakteerDonationRequestBody>
{ }

public class WebHookTrakteerDonationResponse
{ }

public class WebHookTrakteerDonationHandler : IApiRequestHandler<WebHookTrakteerDonationRequest, WebHookTrakteerDonationResponse>
{
    ApiResponseBase<WebHookTrakteerDonationResponse> response = new();

    public async Task<ApiResponseBase<WebHookTrakteerDonationResponse>> Handle(WebHookTrakteerDonationRequest request, CancellationToken cancellationToken)
    {
        return response.Success("Trakteer webhook berhasil diproses");
    }
}