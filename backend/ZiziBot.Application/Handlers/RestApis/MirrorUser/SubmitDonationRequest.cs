﻿using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class SubmitDonationRequest : ApiPostRequestBase<SubmitDonationRequestBody, bool>
{ }

public class SubmitDonationRequestBody
{
    public long UserId { get; set; }
    public required string OrderId { get; set; }
}

public class SubmitDonationValidation : AbstractValidator<SubmitDonationRequest>
{
    public SubmitDonationValidation()
    {
        RuleFor(x => x.Body.UserId).GreaterThan(0).WithMessage("UserId is required");
        RuleFor(x => x.Body.OrderId).NotNull().WithMessage("OrderId is required");
    }
}

public class SubmitDonationHandler(
    DataFacade dataFacade,
    ServiceFacade serviceFacade
) : IApiRequestHandler<SubmitDonationRequest, bool>
{
    private readonly ApiResponseBase<bool> _response = new();

    public async Task<ApiResponseBase<bool>> Handle(
        SubmitDonationRequest request,
        CancellationToken cancellationToken
    )
    {
        var mirrorApproval = await dataFacade.MongoDb.MirrorApproval
            .Where(x => x.OrderId == request.Body.OrderId && x.Status == EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken);

        if (mirrorApproval != null)
        {
            return _response.BadRequest("Donasi sudah terverifikasi");
        }

        var parsedDonationDto = await serviceFacade.MirrorPaymentService.ParseDonation(request.Body.OrderId);

        if (!parsedDonationDto.IsValid)
        {
            return _response.BadRequest("OrderId tidak valid");
        }

        dataFacade.MongoDb.MirrorApproval.Add(new() {
            UserId = request.UserInfo.UserId,
            DonationSource = parsedDonationDto.Source,
            DonationSourceName = parsedDonationDto.Source.ToString(),
            PaymentUrl = parsedDonationDto.PaymentUrl,
            RawText = parsedDonationDto.RawText,
            CendolCount = parsedDonationDto.CendolCount,
            Cendols = parsedDonationDto.Cendols,
            AdminFees = parsedDonationDto.AdminFees,
            Subtotal = parsedDonationDto.Subtotal,
            OrderDate = parsedDonationDto.OrderDate,
            PaymentMethod = parsedDonationDto.PaymentMethod,
            OrderId = parsedDonationDto.OrderId,
            Status = EventStatus.Complete,
            TransactionId = request.UserInfo.TransactionId
        });

        var cendolCount = parsedDonationDto.CendolCount;


        var mirrorUser = await dataFacade.MongoDb.MirrorUser.Where(x =>
                x.UserId == request.UserInfo.UserId &&
                x.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();


        var expireDate = DateTime.Now.AddMonths(cendolCount);

        if (mirrorUser == null)
        {
            dataFacade.MongoDb.MirrorUser.Add(new() {
                UserId = request.UserInfo.UserId,
                ExpireDate = expireDate,
                Status = EventStatus.Complete,
                TransactionId = request.UserInfo.TransactionId
            });
        }

        else
        {
            expireDate = mirrorUser.ExpireDate < DateTime.Now ?
                expireDate // If expired, will be started from now
                : mirrorUser.ExpireDate.AddMonths(cendolCount); // If not expired, will be extended from expire date

            mirrorUser.ExpireDate = expireDate;
            mirrorUser.Status = EventStatus.Complete;
            mirrorUser.TransactionId = request.UserInfo.TransactionId;
        }

        await dataFacade.MongoDb.SaveChangesAsync(cancellationToken);

        return _response.Success("Pembayaran berhasil diverifikasi.", true);
    }
}