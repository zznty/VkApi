namespace VkApi.Core.Responses;

public record WrapResponse<T>(T Response) where T : class;
public abstract record EmptyResponse;