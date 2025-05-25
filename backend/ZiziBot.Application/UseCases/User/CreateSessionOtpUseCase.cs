using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ZiziBot.Interfaces;

namespace ZiziBot.Application.UseCases.User;

public class CreateSessionOtpRequest
{
    public int Otp { get; set; }
}

public class CreateSessionOtpValidator : AbstractValidator<CreateSessionOtpRequest>
{
    public CreateSessionOtpValidator()
    {
        RuleFor(x => x.Otp).NotNull().WithMessage("Otp is required!");
    }
}

public class CreateSessionOtpResponse
{
    public string? AccessToken { get; set; }
    public double AccessExpireIn { get; set; }
    public string? RefreshToken { get; set; }
    public double RefreshExpireIn { get; set; }
}

public class CreateSessionOtpUseCase(
    DataFacade dataFacade,
    CreateSessionOtpValidator validator,
    IUserService userService
)
{
    private readonly ApiResponseBase<CreateSessionOtpResponse> _response = new();

    public async Task<ApiResponseBase<CreateSessionOtpResponse>> Handle(CreateSessionOtpRequest request)
    {
        await validator.ValidateAsync(request);

        var userOtp = await dataFacade.MongoEf.UserOtp
            .Where(x => x.Otp == request.Otp)
            .Where(x => x.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (userOtp == null)
            return _response.Unauthorized("Invalid OTP, please try again!");

        var token = await userService.GenerateAccessToken(userOtp.UserId);

        return _response.Success("Success", new CreateSessionOtpResponse() {
            AccessToken = token.stringToken,
            AccessExpireIn = token.accessExpireIn,
        });
    }
}