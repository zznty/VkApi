namespace VkApi.Core.Abstractions;

public interface IApiClient
{
    Task<TResponse> RequestAsync<TRequest, TResponse>(string methodName, TRequest request)
        where TRequest : class where TResponse : Response;
    
    Task<TResponse> RequestAsync<TResponse>(string methodName)
        where TResponse : Response;
}