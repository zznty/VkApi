namespace VkApi.Core.Abstractions;

public interface IApiExceptionFactory
{
    ApiException CreateExceptionFromCode(int code);
}