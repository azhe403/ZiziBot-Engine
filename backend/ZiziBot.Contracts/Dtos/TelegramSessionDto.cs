using FluentValidation;
using Newtonsoft.Json;

namespace ZiziBot.Contracts.Dtos;

public class TelegramSessionDto
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("last_name")]
    public string? LastName { get; set; }

    [JsonProperty("username")]
    public string? Username { get; set; }

    [JsonProperty("photo_url")]
    public string? PhotoUrl { get; set; }

    [JsonProperty("auth_date")]
    public long AuthDate { get; set; }

    [JsonProperty("hash")]
    public string Hash { get; set; }

    [JsonProperty("session_id")]
    public string SessionId { get; set; }
}

public class TelegramSessionDtoValidator : AbstractValidator<TelegramSessionDto>
{
    public TelegramSessionDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.AuthDate).NotEmpty();
        RuleFor(x => x.Hash).NotEmpty();
        RuleFor(x => x.SessionId).NotEmpty();
    }
}