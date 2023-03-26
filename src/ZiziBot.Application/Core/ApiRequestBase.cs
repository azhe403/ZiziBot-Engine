using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.Application.Core;

public class ApiRequestBase<T> : IRequest<ApiResponseBase<T>>
{
    [FromServices]
    public IHttpContextAccessor? HttpContextAccessor { get; set; }

    [FromHeader(Name = HeaderKey.TransactionId)]
    public string? TransactionId { get; set; }

    [FromHeader(Name = HeaderKey.Authorization)]
    public string? Authorization { get; set; }

    [FromHeader(Name = HeaderKey.UserId)]
    public long? UserId { get; set; }

    public List<long>? ListChatId => HttpContextAccessor?.HttpContext?.Request.Headers[HeaderKey.ListChatId].ToString().ToObject<List<long>>();

    public string? BearerToken => Authorization?.Replace("Bearer ", string.Empty);
}