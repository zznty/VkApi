using VkApi.DependencyInjection.Abstractions;

namespace VkApi.DependencyInjection.Services;

public class DefaultApiTokenProvider : IApiTokenProvider
{
    public required string Token { get; init; }

    public static IApiTokenProvider CreateWithToken(string token) => new DefaultApiTokenProvider
    {
        Token = token
    };
}