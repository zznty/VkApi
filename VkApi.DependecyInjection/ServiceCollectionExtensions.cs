using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VkApi.Core.Abstractions;
using VkApi.Core.Categories;
using VkApi.Core.Errors;
using VkApi.DependencyInjection.Abstractions;
using VkApi.DependencyInjection.Services;

namespace VkApi.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static TCollection AddVkAPi<TCollection>(this TCollection collection, Func<IServiceProvider, IApiVersionProvider> versionGetter,
                                                    Func<IServiceProvider, IApiTokenProvider> tokenGetter) where TCollection : IServiceCollection
    {
        collection.TryAddTransient<IApiExceptionFactory, ApiExceptionFactory>();
        collection.AddHttpClient<IApiClient, ApiClient>((s, client) =>
        {
            client.BaseAddress = new("https://api.vk.com/method/");
            client.DefaultRequestHeaders.Authorization = new("Bearer", s.GetRequiredService<IApiTokenProvider>().Token);
        });
        collection.TryAddTransient(versionGetter);
        collection.TryAddTransient(tokenGetter);
        collection.AddApiCategories();
        
        return collection;
    }

    public static TCollection AddVkAPi<TCollection>(this TCollection collection,
                                                    Func<IApiVersionProvider> versionGetter,
                                                    Func<IApiTokenProvider> tokenGetter)
        where TCollection : IServiceCollection
    {
        return collection.AddVkAPi(_ => versionGetter(), _ => tokenGetter());
    }

    public static TCollection AddVkAPi<TCollection>(this TCollection collection,
                                                    Func<string> versionGetter,
                                                    Func<string> tokenGetter)
        where TCollection : IServiceCollection
    {
        return collection.AddVkAPi(() => DefaultApiVersionProvider.CreateWithVersion(versionGetter()),
                                   () => DefaultApiTokenProvider.CreateWithToken(tokenGetter()));
    }

    public static TCollection AddVkAPi<TCollection>(this TCollection collection,
                                                    string token,
                                                    string version = "5.131")
        where TCollection : IServiceCollection
    {
        return collection.AddVkAPi(() => version, () => token);
    }

    public static TCollection AddVkAPi<TCollection>(this TCollection collection,
                                                    Func<string> tokenGetter,
                                                    string version = "5.131")
        where TCollection : IServiceCollection
    {
        return collection.AddVkAPi(tokenGetter, () => version);
    }

    private static void AddApiCategories<TCollection>(this TCollection collection) where TCollection : IServiceCollection
    {
        collection.TryAddTransient<IUsersCategory, UsersCategory>();
        collection.TryAddTransient<IFriendsCategory, FriendsCategory>();
        collection.TryAddTransient<IStatsCategory, StatsCategory>();
        collection.TryAddTransient<IMessagesCategory, MessagesCategory>();
        collection.TryAddTransient<IGroupsCategory, GroupsCategory>();
        collection.TryAddTransient<IDatabaseCategory, DatabaseCategory>();
        collection.TryAddTransient<IUtilsCategory, UtilsCategory>();
        collection.TryAddTransient<IWallCategory, WallCategory>();
        collection.TryAddTransient<IBoardCategory, BoardCategory>();
        collection.TryAddTransient<IFaveCategory, FaveCategory>();
        collection.TryAddTransient<IVideoCategory, VideoCategory>();
        collection.TryAddTransient<IAccountCategory, AccountCategory>();
        collection.TryAddTransient<IDocsCategory, DocsCategory>();
        collection.TryAddTransient<ILikesCategory, LikesCategory>();
        collection.TryAddTransient<IPagesCategory, PagesCategory>();
        collection.TryAddTransient<IAppsCategory, AppsCategory>();
        collection.TryAddTransient<IStatsCategory, StatsCategory>();
        collection.TryAddTransient<IGiftsCategory, GiftsCategory>();
        collection.TryAddTransient<IAuthCategory, AuthCategory>();
        collection.TryAddTransient<IPollsCategory, PollsCategory>();
        collection.TryAddTransient<ISearchCategory, SearchCategory>();
        collection.TryAddTransient<IStorageCategory, StorageCategory>();
        collection.TryAddTransient<IAdsCategory, AdsCategory>();
        collection.TryAddTransient<INotificationsCategory, NotificationsCategory>();
        collection.TryAddTransient<IWidgetsCategory, WidgetsCategory>();
        collection.TryAddTransient<IStreamingCategory, StreamingCategory>();
        collection.TryAddTransient<INotesCategory, NotesCategory>();
        collection.TryAddTransient<IAppWidgetsCategory, AppWidgetsCategory>();
        collection.TryAddTransient<IOrdersCategory, OrdersCategory>();
        collection.TryAddTransient<ISecureCategory, SecureCategory>();
        collection.TryAddTransient<IStoriesCategory, StoriesCategory>();
        collection.TryAddTransient<ILeadFormsCategory, LeadFormsCategory>();
        collection.TryAddTransient<IPrettyCardsCategory, PrettyCardsCategory>();
        collection.TryAddTransient<IPodcastsCategory, PodcastsCategory>();
        collection.TryAddTransient<IDonutCategory, DonutCategory>();
        collection.TryAddTransient<IDownloadedGamesCategory, DownloadedGamesCategory>();
        collection.TryAddTransient<IStatusCategory, StatusCategory>();
        collection.TryAddTransient<ICatalogCategory, CatalogCategory>();
    }
}