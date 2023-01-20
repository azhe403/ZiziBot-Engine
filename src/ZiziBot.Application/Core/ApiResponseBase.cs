using System.Net;

namespace ZiziBot.Application.Core;

public class ApiResponseBase<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public T Data { get; set; }
}