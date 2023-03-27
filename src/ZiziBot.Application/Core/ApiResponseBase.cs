using System.Net;

namespace ZiziBot.Application.Core;

public class ApiResponseBase<TResult>
{
    public HttpStatusCode StatusCode { get; set; }
    public string transactionId { get; set; }
    public string Message { get; set; }
    public TResult? Result { get; set; }

    public ApiResponseBase<TResult> Success(string message, TResult? result)
    {
        StatusCode = HttpStatusCode.OK;
        Message = message;
        Result = result;

        return this;
    }

    public ApiResponseBase<TResult> Unauthorized(string message, TResult? result = default)
    {
        StatusCode = HttpStatusCode.Unauthorized;
        Message = message;
        Result = result;

        return this;
    }

    public ApiResponseBase<TResult> BadRequest(string message, TResult? result = default)
    {
        StatusCode = HttpStatusCode.BadRequest;
        Message = message;
        Result = result;

        return this;
    }
}