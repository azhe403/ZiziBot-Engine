using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class SubmitPaymentUrlRequest : ApiRequestBase<bool>
{
    [FromBody]
    public SubmitPaymentUrlRequestBody Body { get; set; }
}

public class SubmitPaymentUrlRequestBody
{
    public string Payload { get; set; }
}

public class SubmitPaymentUrlValidation : AbstractValidator<SubmitPaymentUrlRequest>
{
    public SubmitPaymentUrlValidation()
    {
        RuleFor(x => x.Body.Payload).NotEmpty().WithMessage("Payload is required");
    }
}

public class SubmitPaymentUrlRequestHandler : IRequestHandler<SubmitPaymentUrlRequest, ApiResponseBase<bool>>
{
    private readonly MirrorDbContext _mirrorDbContext;

    public SubmitPaymentUrlRequestHandler(MirrorDbContext mirrorDbContext)
    {
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<ApiResponseBase<bool>> Handle(SubmitPaymentUrlRequest request, CancellationToken cancellationToken)
    {
        var response = new ApiResponseBase<bool>();

        var mirrorApproval = await _mirrorDbContext.MirrorApproval
            .FirstOrDefaultAsync(x =>
                    x.PaymentUrl == request.Body.Payload &&
                    x.Status == (int)EventStatus.Complete,
                cancellationToken);

        if (mirrorApproval != null)
        {
            return response.BadRequest("Pembayaran sudah terverifikasi sebelumnya.");
        }

        var trakteerParsedDto = await request.Body.Payload.ParseTrakteerWeb();

        if (!trakteerParsedDto.IsValid)
        {
            return response.BadRequest("Tautan Pembayaran tidak valid. Contoh: https://trakteer.id/payment-status/123456");
        }

        _mirrorDbContext.MirrorApproval.Add(new MirrorApprovalEntity()
        {
            UserId = request.SessionUserId,
            PaymentUrl = trakteerParsedDto.PaymentUrl,
            RawText = trakteerParsedDto.RawText,
            CendolCount = trakteerParsedDto.CendolCount,
            Cendols = trakteerParsedDto.Cendols,
            AdminFees = trakteerParsedDto.AdminFees,
            Subtotal = trakteerParsedDto.Subtotal,
            OrderDate = trakteerParsedDto.OrderDate,
            PaymentMethod = trakteerParsedDto.PaymentMethod,
            OrderId = trakteerParsedDto.OrderId,
            Status = (int)EventStatus.Complete,
            TransactionId = request.TransactionId
        });

        var cendolCount = trakteerParsedDto.CendolCount;


        var mirrorUser = await _mirrorDbContext.MirrorUsers
            .FirstOrDefaultAsync(x =>
                    x.UserId == request.SessionUserId &&
                    x.Status == (int)EventStatus.Complete,
                cancellationToken: cancellationToken
            );

        var expireDate = DateTime.Now.AddMonths(cendolCount);

        if (mirrorUser == null)
        {
            _mirrorDbContext.MirrorUsers.Add(new MirrorUserEntity()
            {
                UserId = request.SessionUserId,
                ExpireDate = expireDate,
                Status = (int)EventStatus.Complete,
                TransactionId = request.TransactionId
            });
        }

        else
        {
            expireDate = mirrorUser.ExpireDate < DateTime.Now
                ? expireDate // If expired, will be started from now
                : mirrorUser.ExpireDate.AddMonths(cendolCount); // If not expired, will be extended from expire date

            mirrorUser.ExpireDate = expireDate;
            mirrorUser.Status = (int)EventStatus.Complete;
            mirrorUser.TransactionId = request.TransactionId;
        }

        await _mirrorDbContext.SaveChangesAsync(cancellationToken);

        return response.Success("Pembayaran berhasil diverifikasi.", true);
    }

}