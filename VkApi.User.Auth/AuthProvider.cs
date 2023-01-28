using System.Text.Json;
using JorgeSerrano.Json;
using VkApi.Core.Abstractions;

namespace VkApi.User.Auth;

public class AuthProvider : IAuthProvider
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new("https://oauth.vk.com/")
    };

    private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
    };

    public AuthProvider(IApiClient apiClient)
    {
        foreach (var (key, value) in apiClient.Headers)
        {
            _client.DefaultRequestHeaders.Add(key, value);
        }
    }
    
    public async Task LoginAsync(string username, string password, string? confirmationCode = null)
    {
        var parameters = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        {
            new("grant_type", "password"),
            new("client_id", "2274003"),
            new("client_secret", "hHbZxrka2uZ6jB1inYsH"),
            new("2fa_supported", "1"),
            new("username", username),
            new("password", password),
            new("code", confirmationCode!),
            new("scope", "all"),
            new("device_id", Guid.NewGuid().ToString())
        });

        using var response = await _client.PostAsync("token", parameters);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var error = await JsonSerializer.DeserializeAsync<AuthError>(stream, _options);
        
        if (error is null) return;

        throw new AuthException(error);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _client.Dispose();
    }
}

public class AuthException : Exception
{
    public AuthError Error { get; }

    public AuthException(AuthError error) : base(error.ErrorDescription)
    {
        Error = error;
    }
}