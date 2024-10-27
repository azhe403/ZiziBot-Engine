namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class CheckPaymentOrderIdRequest : ApiRequestBase<DonationParsedDto>
{
    public string OrderId { get; set; }
}

public class CheckPaymentOrderIdHandler(
    ServiceFacade serviceFacade
) : IApiRequestHandler<CheckPaymentOrderIdRequest, DonationParsedDto>
{
    private readonly ApiResponseBase<DonationParsedDto> _response = new();

    public async Task<ApiResponseBase<DonationParsedDto>> Handle(CheckPaymentOrderIdRequest request, CancellationToken cancellationToken)
    {
        var parsedTrakteer = await serviceFacade.MirrorPaymentService.ParseTrakteerWeb(request.OrderId);

        if (parsedTrakteer.IsValid)
        {
            return _response.Success("Get OrderId succeed", parsedTrakteer);
        }

        var parsedSaweria = await serviceFacade.MirrorPaymentService.ParseSaweriaWeb(request.OrderId);

        return parsedSaweria.IsValid ?
            _response.Success("Get OrderId succeed", parsedSaweria) :
            _response.BadRequest("OrderId not found");
    }
}