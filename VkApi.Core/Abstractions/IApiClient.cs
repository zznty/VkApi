using System.Net.Http.Headers;

namespace VkApi.Core.Abstractions;

public interface IApiClient
{
    HttpRequestHeaders Headers { get; }
    
    Task<TResponse> RequestAsync<TRequest, TResponse>(string methodName, TRequest request)
        where TRequest : class where TResponse : class;
    
    Task<TResponse> RequestAsync<TResponse>(string methodName)
        where TResponse : class;
}