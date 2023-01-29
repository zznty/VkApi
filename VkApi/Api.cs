using VkApi.Core.Abstractions;
using VkApi.Core.Categories;
using VkApi.Core.Errors;

namespace VkApi;

public class Api
{
    public Api(string token, string version = "5.131")
    {
        Client = new ApiClient(new ApiExceptionFactory(), new()
        {
            BaseAddress = new("https://api.vk.com/method/"),
            DefaultRequestHeaders =
            {
                Authorization = new("Bearer", token)
            }
        }, version);
        
        Account = new AccountCategory(Client);
        Ads = new AdsCategory(Client);
        Apps = new AppsCategory(Client);
        Board = new BoardCategory(Client);
        Database = new DatabaseCategory(Client);
        Docs = new DocsCategory(Client);
        Fave = new FaveCategory(Client);
        Friends = new FriendsCategory(Client);
        Gifts = new GiftsCategory(Client);
        Groups = new GroupsCategory(Client);
        Likes = new LikesCategory(Client);
        Market = new MarketCategory(Client);
        Messages = new MessagesCategory(Client);
        Newsfeed = new NewsfeedCategory(Client);
        Notes = new NotesCategory(Client);
        Notifications = new NotificationsCategory(Client);
        Pages = new PagesCategory(Client);
        Photos = new PhotosCategory(Client);
        Polls = new PollsCategory(Client);
        Search = new SearchCategory(Client);
        Stats = new StatsCategory(Client);
        Status = new StatusCategory(Client);
        Storage = new StorageCategory(Client);
        Users = new UsersCategory(Client);
        Utils = new UtilsCategory(Client);
        Video = new VideoCategory(Client);
        Donut = new DonutCategory(Client);
        Podcasts = new PodcastsCategory(Client);
        LeadForms = new LeadFormsCategory(Client);
        PrettyCards = new PrettyCardsCategory(Client);
        Store = new StoreCategory(Client);
        Stories = new StoriesCategory(Client);
        AppWidgets = new AppWidgetsCategory(Client);
        Streaming = new StreamingCategory(Client);
        Orders = new OrdersCategory(Client);
        Wall = new WallCategory(Client);
        Widgets = new WidgetsCategory(Client);
    }

    public IApiClient Client { get; }

#region Categories

    public IAccountCategory Account { get; }
    public IAdsCategory Ads { get; }
    public IAppsCategory Apps { get; }
    public IBoardCategory Board { get; }
    public IDatabaseCategory Database { get; }
    public IDocsCategory Docs { get; }
    public IFaveCategory Fave { get; }
    public IFriendsCategory Friends { get; }
    public IGiftsCategory Gifts { get; }
    public IGroupsCategory Groups { get; }
    public ILikesCategory Likes { get; }
    public IMarketCategory Market { get; }
    public IMessagesCategory Messages { get; }
    public INewsfeedCategory Newsfeed { get; }
    public INotesCategory Notes { get; }
    public INotificationsCategory Notifications { get; }
    public IPagesCategory Pages { get; }
    public IPhotosCategory Photos { get; }
    public IPollsCategory Polls { get; }
    public ISearchCategory Search { get; }
    public IStatsCategory Stats { get; }
    public IStatusCategory Status { get; }
    public IStorageCategory Storage { get; }
    public IUsersCategory Users { get; }
    public IUtilsCategory Utils { get; }
    public IVideoCategory Video { get; }
    public IDonutCategory Donut { get; }
    public IPodcastsCategory Podcasts { get; }
    public ILeadFormsCategory LeadForms { get; }
    public IPrettyCardsCategory PrettyCards { get; }
    public IStoreCategory Store { get; }
    public IStoriesCategory Stories { get; }
    public IAppWidgetsCategory AppWidgets { get; }
    public IStreamingCategory Streaming { get; }
    public IOrdersCategory Orders { get; }
    public IWallCategory Wall { get; }
    public IWidgetsCategory Widgets { get; }

#endregion
}