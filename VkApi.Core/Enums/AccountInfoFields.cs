namespace VkApi.Core.Enums;

public class AccountInfoField : SmartFlagEnum<AccountInfoField, string>
{
    public static readonly AccountInfoField Country = new(nameof(Country), "country");
    public static readonly AccountInfoField HttpsRequired = new(nameof(HttpsRequired), "https_required");
    public static readonly AccountInfoField OwnPostsDefault = new(nameof(OwnPostsDefault), "own_posts_default");
    public static readonly AccountInfoField NoWallReplies = new(nameof(NoWallReplies), "no_wall_replies");
    public static readonly AccountInfoField Intro = new(nameof(Intro), "intro");
    public static readonly AccountInfoField Language = new(nameof(Language), "lang");

    private AccountInfoField(string name, string value) : base(name, value)
    {
    }
}