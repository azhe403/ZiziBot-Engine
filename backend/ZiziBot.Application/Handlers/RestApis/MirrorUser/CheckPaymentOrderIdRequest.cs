namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class CheckPaymentOrderIdRequest : ApiRequestBase<ParsedDonationDto>
{
    public string OrderId { get; set; }
}

public class CheckPaymentOrderIdHandler(
    ServiceFacade serviceFacade
) : IApiRequestHandler<CheckPaymentOrderIdRequest, ParsedDonationDto>
{
    private readonly ApiResponseBase<ParsedDonationDto> _response = new();

    public async Task<ApiResponseBase<ParsedDonationDto>> Handle(CheckPaymentOrderIdRequest request, CancellationToken cancellationToken)
    {
        var parsedDonationDto = await serviceFacade.MirrorPaymentService.ParseDonation(request.OrderId);

        return parsedDonationDto.IsValid ?
            _response.Success("Get OrderId succeed", parsedDonationDto) :
            _response.BadRequest("OrderId not found");
    }
}