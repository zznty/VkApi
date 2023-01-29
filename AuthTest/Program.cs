using VkApi;
using VkApi.User.Auth;

internal class Program
{
    public static async Task Main(string username, string password)
    {
        using var provider = AuthProvider.CreateWithUserAgent();

        AuthResult result;
        try
        {
            result = await provider.LoginAsync(username, password);
        }
        catch (AuthException e)
        {
            Console.WriteLine(e.Error);
            throw;
        }
        
        Console.WriteLine(result);

        var client = new Api(result.AccessToken);

        var currentUser = await client.Users.GetAsync(new(null, null, null));
        
        Console.WriteLine(currentUser.First());
    }
}