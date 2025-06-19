using System.Net;
using System.Text.Json.Serialization;

namespace ZiziBot.Application.Core;

public class ApiResponseBase<TResult>
{
    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? TransactionId { get; set; }

    public TimeSpan ExecutionTime { get; set; }

    public string Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ApiMetadata? Metadata { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TResult? Result { get; set; }

    public ApiResponseBase<TResult> Success(string message, TResult? result = default)
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

    public ApiResponseBase<TResult> NotFound(string message, TResult? result = default)
    {
        StatusCode = HttpStatusCode.NotFound;
        Message = message;
        Result = result;

        return this;
    }

    public ApiResponseBase<TResult> SetMetadata(int totalItem = 0, int pageNumber = 0, int pageSize = 0)
    {
        Metadata = new ApiMetadata {
            PageNumber = pageNumber,
            TotalItem = totalItem,
            PageSize = totalItem > pageSize ? pageSize : totalItem,
            HasNext = totalItem > pageSize
        };

        return this;
    }
}

public class ApiMetadata
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItem { get; set; }
    public bool HasNext { get; set; }
}

public static class ApiResponse
{
    public static ApiResponseBase<T> Create<T>()
    {
        return new ApiResponseBase<T>();
    }
}