using VkApi.DependencyInjection.Abstractions;

namespace VkApi.DependencyInjection.Services;

public class DefaultApiVersionProvider : IApiVersionProvider
{
    public required string Version { get; init; }

    public static IApiVersionProvider CreateWithVersion(string version) => new DefaultApiVersionProvider
    {
        Version = version
    };
}