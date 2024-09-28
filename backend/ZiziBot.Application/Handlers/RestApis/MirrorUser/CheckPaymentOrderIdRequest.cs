namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class CheckPaymentOrderIdRequest : ApiRequestBase<TrakteerParsedDto>
{
    public string OrderId { get; set; }
}

public class CheckPaymentOrderIdHandler(MirrorPaymentService mirrorPaymentService) : IApiRequestHandler<CheckPaymentOrderIdRequest, TrakteerParsedDto>
{
    private readonly ApiResponseBase<TrakteerParsedDto> _response = new();

    public async Task<ApiResponseBase<TrakteerParsedDto>> Handle(CheckPaymentOrderIdRequest request, CancellationToken cancellationToken)
    {
        var parsedTrakteer = await mirrorPaymentService.ParseSaweriaWeb(request.OrderId);

        if (parsedTrakteer.IsValid)
        {
            return _response.Success("Get OrderId succeed", parsedTrakteer);
        }

        var parsedSaweria = await mirrorPaymentService.ParseSaweriaWeb(request.OrderId);

        return parsedSaweria.IsValid
            ? _response.Success("Get OrderId succeed", parsedSaweria)
            : _response.BadRequest("OrderId not found");
    }
}