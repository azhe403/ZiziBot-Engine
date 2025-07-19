using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using ZiziBot.Common.Dtos;

namespace ZiziBot.Common.Interfaces;

public interface IHttpContextHelper
{
    public RequestHeaders Headers { get; }
    public IHeaderDictionary HeaderDict { get; }
    public UserInfo UserInfo { get; }

    public string TransactionId { get; }

    Task<string> GetRawBodyAsync();
    Task<T?> GetBodyAsJsonAsync<T>() where T : class;
    Task<Dictionary<string, string>> GetFormDataAsync();
}