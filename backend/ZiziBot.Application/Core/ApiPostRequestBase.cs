using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.Application.Core;

public class ApiPostRequestBase<TBody, T> : ApiRequestBase<T>
{
    [FromBody]
    public required TBody Body { get; set; }
}