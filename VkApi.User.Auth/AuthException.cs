namespace VkApi.User.Auth;

public class AuthException : Exception
{
    public AuthError Error { get; }

    public AuthException(AuthError error) : base(error.ErrorDescription)
    {
        Error = error;
    }
}