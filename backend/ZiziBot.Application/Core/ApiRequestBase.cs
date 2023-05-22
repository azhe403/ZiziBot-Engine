using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ZiziBot.Application.Core;

public class ApiRequestBase<T> : IRequest<ApiResponseBase<T>>
{
    [FromServices]
    public IHttpContextAccessor? HttpContextAccessor { get; set; }

    [BindNever]
    public IHeaderDictionary Headers => HttpContextAccessor?.HttpContext?.Request.Headers!;

    [BindNever]
    public string? TransactionId => Headers[HeaderKey.TransactionId];

    [BindNever]
    public string? Authorization => Headers[HeaderKey.Authorization];

    [BindNever]
    public long SessionUserId => Headers.GetUserId();

    [BindNever]
    public ApiRole? SessionUserRole => Headers[HeaderKey.UserRole].ToString().ToEnum(ApiRole.Guest);

    [BindNever]
    public List<long> AdminChatId => Headers[HeaderKey.ListChatId].ToString().ToObject<List<long>>() ?? new List<long>();

    [BindNever]
    public List<long> ListChatId => AdminChatId.Append(SessionUserId).Where(x => x != 0).ToList();

    [BindNever]
    public string? BearerToken => Authorization?.Replace("Bearer ", string.Empty);
}