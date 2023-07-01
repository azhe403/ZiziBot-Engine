using System.Net;
using Newtonsoft.Json;

namespace ZiziBot.Application.Core;

public class WebResponseBase<TResult>
{
    public HttpStatusCode StatusCode { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? TransactionId { get; set; }

    public string Message { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TResult? Result { get; set; }

    public WebResponseBase<TResult> Success(string message, TResult? result = default)
    {
        StatusCode = HttpStatusCode.OK;
        Message = message;
        Result = result;

        return this;
    }

    public WebResponseBase<TResult> Unauthorized(string message, TResult? result = default)
    {
        StatusCode = HttpStatusCode.Unauthorized;
        Message = message;
        Result = result;

        return this;
    }

    public WebResponseBase<TResult> BadRequest(string message, TResult? result = default)
    {
        StatusCode = HttpStatusCode.BadRequest;
        Message = message;
        Result = result;

        return this;
    }
}