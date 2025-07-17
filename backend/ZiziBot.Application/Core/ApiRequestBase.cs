using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ZiziBot.Common.Dtos;

namespace ZiziBot.Application.Core;

public class ApiRequestBase<T> : IRequest<ApiResponseBase<T>>
{
    [FromServices]
    [SwaggerIgnore]
    public IHttpContextAccessor? HttpContextAccessor { get; set; }

    internal IHeaderDictionary Headers => HttpContextAccessor?.HttpContext?.Request.Headers!;

    internal UserInfo UserInfo => HttpContextAccessor?.HttpContext?.Items.TryGetValue(ValueConst.USER_INFO, out var userInfo) == true ? (UserInfo)userInfo : new UserInfo();

    internal async Task<string> RequestBody() => await HttpContextAccessor?.HttpContext?.GetBodyAsync()!;
}

public class ApiRequestBase<T, TBody> : ApiRequestBase<T>
{
    [FromBody]
    public required TBody Body { get; set; }
}