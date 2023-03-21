using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.Application.Core;

public class ApiRequestBase<T> : IRequest<ApiResponseBase<T>>
{
    [FromServices]
    public IHttpContextAccessor? HttpContextAccessor { get; set; }

    [FromHeader(Name = "transactionId")]
    public string? TransactionId { get; set; }

    [FromHeader(Name = "Authorization")]
    public string? Authorization { get; set; }

    public string? BearerToken => Authorization?.Replace("Bearer ", string.Empty);
}