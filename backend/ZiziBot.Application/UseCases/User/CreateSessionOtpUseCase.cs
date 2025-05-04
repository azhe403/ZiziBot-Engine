using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ZiziBot.Contracts.Validators;

namespace ZiziBot.Application.UseCases.User;

public class CreateSessionOtpRequest
{
    public int Otp { get; set; }
}

public class CreateSessionOtpValidator : AbstractValidator<CreateSessionOtpRequest>
{
    public CreateSessionOtpValidator()
    {
        RuleFor(x => x.Otp).Required().WithMessage("Otp is required!");
    }
}

public class CreateSessionOtpResponse
{
    public string? AccessToken { get; set; }
    public double AccessExpireIn { get; set; }
    public string? RefreshToken { get; set; }
    public double RefreshExpireIn { get; set; }
}

public class CreateSessionOtpUseCase(DataFacade dataFacade, CreateSessionOtpValidator validator)
{
    private ApiResponseBase<CreateSessionOtpResponse> response = new();

    public async Task<ApiResponseBase<CreateSessionOtpResponse>> Handle(CreateSessionOtpRequest request)
    {
        await validator.ValidateAsync(request);

        var userOtp = await dataFacade.MongoEf.UserOtp
            .Where(x => x.Otp == request.Otp)
            .Where(x => x.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (userOtp == null)
            return response.Unauthorized("Invalid OTP, please try again!");

        #region Generate Token
        var jwtConfig = await dataFacade.AppSetting.GetConfigSectionAsync<JwtConfig>();

        if (jwtConfig == null)
        {
            return response.BadRequest("Authentication is not yet configured");
        }

        var botUser = await dataFacade.MongoEf.BotUser.Where(x => x.UserId == userOtp.UserId)
            .FirstOrDefaultAsync();

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, botUser?.Username ?? string.Empty),
            new Claim(ClaimTypes.Name, botUser?.FirstName ?? string.Empty),
            new Claim(RequestKey.UserId, userOtp.UserId.ToString()),
        };

        var dateTime = DateTime.UtcNow;
        var tokenExpiration = dateTime.AddMinutes(15);
        var accessExpireIn = (tokenExpiration - dateTime).TotalSeconds;
        var token = new JwtSecurityToken(jwtConfig.Issuer, jwtConfig.Audience, claims, expires: tokenExpiration, signingCredentials: credentials);
        var stringToken = new JwtSecurityTokenHandler().WriteToken(token);
        #endregion

        return response.Success("Success", new CreateSessionOtpResponse() {
            AccessToken = stringToken,
            AccessExpireIn = (int)accessExpireIn,
        });
    }
}