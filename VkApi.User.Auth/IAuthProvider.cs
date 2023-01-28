namespace VkApi.User.Auth;

public interface IAuthProvider : IDisposable
{
    public Task LoginAsync(string username, string password, string? confirmationCode = null);
}