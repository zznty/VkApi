namespace VkApi.User.Auth;

public record AuthError(string Error, string ErrorType, string? ErrorDescription, string? CaptchaSid, Uri? CaptchaImg, Uri? RedirectUri);