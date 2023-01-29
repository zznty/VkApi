using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using JorgeSerrano.Json;
using VkApi.Core.Abstractions;
using VkApi.Core.Errors;
using VkApi.Core.Responses;

namespace VkApi;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _version;

    private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
    };

    public ApiClient(HttpClient httpClient, string version)
    {
        _httpClient = httpClient;
        _version = version;
    }

    public HttpRequestHeaders Headers => _httpClient.DefaultRequestHeaders;

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(string methodName, TRequest request)
        where TRequest : class where TResponse : class
    {
        var node = JsonSerializer.SerializeToNode(request, _options)!;
        node["v"] = _version;

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

        if (node?["error"] is { } error)
            throw new ApiException(error["error_code"]!.GetValue<int>(),
                                   error["error_msg"]?.ToString()); // TODO use generated types instead

        return typeof(TResponse) == typeof(EmptyResponse) ? null! : node!["response"].Deserialize<TResponse>(_options)!;
    }
}