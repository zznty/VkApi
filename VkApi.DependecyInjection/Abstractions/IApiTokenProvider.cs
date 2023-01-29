namespace VkApi.DependencyInjection.Abstractions;

public interface IApiTokenProvider
{
    string Token { get; }
}