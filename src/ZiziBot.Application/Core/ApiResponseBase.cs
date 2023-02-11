using System.Net;

namespace ZiziBot.Application.Core;

public class ApiResponseBase<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; }
    public T Result { get; set; }
}