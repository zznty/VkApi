namespace VkApi.User.Auth;

public record AuthResult(
    string AccessToken,
    int ExpiresIn,
    int UserId,
    string WebviewRefreshToken,
    string WebviewRefreshTokenExpiresIn,
    string WebviewAccessToken,
    string WebviewAccessTokenExpiresIn
);