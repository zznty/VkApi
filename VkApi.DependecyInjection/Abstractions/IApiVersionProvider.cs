namespace VkApi.DependencyInjection.Abstractions;

public interface IApiVersionProvider
{
    string Version { get; }
}