using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using JorgeSerrano.Json;

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

    public AuthProvider(IReadOnlyDictionary<string, string>? headers = null)
    {
        if (headers is null)
            return;
        
        foreach (var (key, value) in headers)
        {
            _client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
        }
    }

    public static IAuthProvider CreateWithUserAgent(
        string userAgent = "VKAndroidApp/7.37-13617 (Android 12; SDK 32; armeabi-v7a; VkApi; ru; 2960x1440)")
    {
        return new AuthProvider(new Dictionary<string, string>
        {
            { "User-Agent", userAgent },
            { "X-VK-Android-Client", "new" }
        });
    }
    
    public async Task<AuthResult> LoginAsync(string username, string password, string? confirmationCode = null)
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

        await using var stream = await response.Content.ReadAsStreamAsync();
        var result = JsonNode.Parse(stream)!;
        
        if (result["error"] != null)
            throw new AuthException(result.Deserialize<AuthError>(_options)!);

        // in case vk did not send us the error, but returned non success code like 500
        response.EnsureSuccessStatusCode();

        return result.Deserialize<AuthResult>(_options)!;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _client.Dispose();
    }
}