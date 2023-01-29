using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using JorgeSerrano.Json;
using Microsoft.Extensions.Logging;
using VkApi.Core.Abstractions;
using VkApi.Core.Responses;
using VkApi.DependencyInjection.Abstractions;

namespace VkApi.DependencyInjection.Services;

public class ApiClient : IApiClient
{
    private readonly IApiExceptionFactory _exceptionFactory;
    private readonly HttpClient _httpClient;
    private readonly IApiVersionProvider _versionProvider;
    private readonly ILogger<ApiClient> _logger;

    private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
    };

    public ApiClient(IApiExceptionFactory exceptionFactory, HttpClient httpClient, IApiVersionProvider versionProvider,
                     ILogger<ApiClient> logger)
    {
        _exceptionFactory = exceptionFactory;
        _httpClient = httpClient;
        _versionProvider = versionProvider;
        _logger = logger;
    }

    public HttpRequestHeaders Headers => _httpClient.DefaultRequestHeaders;

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(string methodName, TRequest request)
        where TRequest : class where TResponse : class
    {
        var node = JsonSerializer.SerializeToNode(request, _options)!;
        node["v"] = _versionProvider.Version;
        
        if (_logger.IsEnabled(LogLevel.Trace))
            _logger.LogTrace("Invoking {MethodName} with parameters {Parameters}", methodName, node.ToJsonString());

        return await RequestAsync<TResponse>(methodName, WriteRequest(node));
    }

    private HttpContent WriteRequest(JsonNode node)
    {
        return new FormUrlEncodedContent(node.AsObject().Where(b => b.Value is { })
                                             .Select(b => new KeyValuePair<string, string>(
                                                         b.Key, b.Value!.ToString())));
    }

    public Task<TResponse> RequestAsync<TResponse>(string methodName) where TResponse : class
    {
        return RequestAsync<TResponse>(methodName, null);
    }

    private async Task<TResponse> RequestAsync<TResponse>(string methodName, HttpContent? content)
        where TResponse : class
    {
        using var response = await _httpClient.PostAsync(methodName, content);
        response.EnsureSuccessStatusCode();

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var node = JsonNode.Parse(responseStream);
        
        if (_logger.IsEnabled(LogLevel.Trace))
            _logger.LogTrace("Response of {MethodName}: {Response}", methodName, node!.ToJsonString());

        if (node?["error"] is { } error)
            throw _exceptionFactory.CreateExceptionFromCode(error["error_code"]!.GetValue<int>());

        return typeof(TResponse) == typeof(EmptyResponse) ? null! : node!["response"].Deserialize<TResponse>(_options)!;
    }
}