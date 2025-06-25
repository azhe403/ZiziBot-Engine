using System.Collections.Immutable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ZiziBot.Common.Dtos;

namespace ZiziBot.Application.Core;

public class ApiRequestBase<T> : IRequest<ApiResponseBase<T>>
{
    internal IHttpContextAccessor? HttpContextAccessor { get; set; }

    internal IHeaderDictionary Headers => HttpContextAccessor?.HttpContext?.Request.Headers!;

    internal IDictionary<object, object?> Items => HttpContextAccessor?.HttpContext?.Items!;

    internal string TransactionId => HttpContextAccessor?.HttpContext?.GetTransactionId() ?? Guid.NewGuid().ToString();

    internal long SessionUserId => BearerToken.DecodeJwt()?.Claims.FirstOrDefault(claim => claim.Type == RequestKey.UserId)?.Value.Convert<long>() ?? 0;

    internal string? Authorization => Headers[RequestKey.Authorization];

    internal ApiRole? SessionUserRole => Items[RequestKey.UserRole]?.ToString()?.ToEnum(ApiRole.Guest);

    internal ImmutableList<RoleLevel> UserRoles => (Items["UserRoles"] as RoleLevel[] ?? []).ToImmutableList();

    internal List<long> AdminChatId => Items[RequestKey.ListChatId]?.ToString().ToObject<List<long>>() ?? [];

    internal List<long> ListChatId => AdminChatId.Append(SessionUserId).Where(x => x != 0).ToList();

    internal string? BearerToken => Authorization?.Replace("Bearer ", string.Empty);

    internal DataRbac DataRbac => Items["DRBAC"] as DataRbac ?? new();

    internal async Task<string> RequestBody() => await HttpContextAccessor?.HttpContext?.GetBodyAsync();
}

public class ApiRequestBase<T, TBody> : ApiRequestBase<T>
{
    [FromBody]
    public required TBody Body { get; set; }
}