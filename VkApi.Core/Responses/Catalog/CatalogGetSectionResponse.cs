namespace VkApi.Core.Responses;

public record CatalogGetSectionResponseSectionBlockAction(
    CatalogGetSectionResponseSectionBlockActionAction Action,
    string Title,
    int RefItemsCount,
    string RefLayoutName,
    string RefDataType,
    string SectionId,
    string BlockId
);

public record CatalogGetSectionResponseSectionBlockActionAction(
    string Type,
    string Target,
    string Url
);

public record CatalogGetSectionResponseAudioAds(
    string ContentId,
    string Duration,
    string AccountAgeType,
    string Puid1,
    string Puid22
);

public record CatalogGetSectionResponseAlbum(
    int Id,
    string Title,
    int OwnerId,
    string AccessKey,
    CatalogGetSectionResponseAlbumThumb Thumb
);

public record CatalogGetSectionResponseAudio(
    string Artist,
    int Id,
    int OwnerId,
    string Title,
    int Duration,
    string AccessKey,
    CatalogGetSectionResponseAudioAds Ads,
    bool IsExplicit,
    bool IsFocusTrack,
    bool IsLicensed,
    string TrackCode,
    string Url,
    int Date,
    int GenreId,
    bool ShortVideosAllowed,
    bool StoriesAllowed,
    bool StoriesCoverAllowed,
    CatalogGetSectionResponseAlbum Album,
    ICollection<CatalogGetSectionResponseAudioMainArtist> MainArtists,
    string Subtitle,
    ICollection<CatalogGetSectionResponseAudioFeaturedArtist> FeaturedArtists
);

public record CatalogGetSectionResponseAudioFollowingsUpdateInfo(
    string Title,
    string Id,
    ICollection<CatalogGetSectionResponseAudioFollowingsUpdateInfoCover> Covers
);

public record Badge(
    string Type,
    string Text
);

public record CatalogGetSectionResponseSectionBlock(
    string Id,
    string DataType,
    CatalogGetSectionResponseSectionBlockLayout Layout,
    ICollection<string> ListenEvents,
    Badge Badge,
    ICollection<string> AudiosIds,
    ICollection<string> PlaylistsIds,
    ICollection<CatalogGetSectionResponseSectionBlockAction> Actions,
    string NextFrom,
    string Url,
    ICollection<string> AudioFollowingsUpdateInfoIds
);

public record CatalogGetSectionResponseProfileCity(
    int Id,
    string Title
);

public record CatalogGetSectionResponseProfileCountry(
    int Id,
    string Title
);

public record CatalogGetSectionResponseAudioFollowingsUpdateInfoCover(
    int Width,
    int Height,
    string Photo34,
    string Photo68,
    string Photo135,
    string Photo270,
    string Photo300,
    string Photo600,
    string Photo1200
);

public record CatalogGetSectionResponseAudioFeaturedArtist(
    string Name,
    string Domain,
    string Id,
    bool IsFollowed,
    bool CanFollow
);

public record CatalogGetSectionResponseGroup(
    int Id,
    int MemberStatus,
    int MembersCount,
    string Activity,
    int Trending,
    string Name,
    string ScreenName,
    int IsClosed,
    string Type,
    int IsAdmin,
    int IsMember,
    int IsAdvertiser,
    int Verified,
    string Photo50,
    string Photo100,
    string Photo200
);

public record CatalogGetSectionResponseSectionBlockLayout(
    string Name,
    string Title,
    int? OwnerId,
    string Style,
    CatalogGetSectionResponseSectionBlockLayoutTopTitle TopTitle
);

public record CatalogGetSectionResponseAudioMainArtist(
    string Name,
    string Domain,
    string Id,
    bool IsFollowed,
    bool CanFollow
);

public record CatalogGetSectionResponsePlaylistMeta(
    string View
);

public record CatalogGetSectionResponsePlaylistPermissions(
    bool Play,
    bool Share,
    bool Edit,
    bool Follow,
    bool Delete,
    bool BoomDownload,
    bool SaveAsCopy
);

public record CatalogGetSectionResponsePlaylistPhoto(
    int Width,
    int Height,
    string Photo34,
    string Photo68,
    string Photo135,
    string Photo270,
    string Photo300,
    string Photo600,
    string Photo1200
);

public record CatalogGetSectionResponsePlaylist(
    int Id,
    int OwnerId,
    int Type,
    string Title,
    string Description,
    int Count,
    int Followers,
    int Plays,
    int CreateTime,
    int UpdateTime,
    ICollection<object> Genres,
    bool IsFollowing,
    CatalogGetSectionResponsePlaylistPhoto Photo,
    CatalogGetSectionResponsePlaylistPermissions Permissions,
    bool SubtitleBadge,
    bool PlayButton,
    string AccessKey,
    string Subtitle,
    string AlbumType,
    CatalogGetSectionResponsePlaylistMeta Meta
);

public record CatalogGetSectionResponseProfile(
    int Id,
    CatalogGetSectionResponseProfileCity City,
    CatalogGetSectionResponseProfileCountry Country,
    string Photo200,
    string Activity,
    int FollowersCount,
    ICollection<object> Career,
    int University,
    string UniversityName,
    int Faculty,
    string FacultyName,
    int Graduation,
    string EducationForm,
    string EducationStatus,
    string ScreenName,
    string Photo50,
    string Photo100,
    int Verified,
    int Trending,
    int FriendStatus,
    string FirstName,
    string LastName,
    bool CanAccessClosed,
    bool IsClosed
);

public record CatalogGetSectionResponseRecommendedPlaylist(
    int Id,
    int OwnerId,
    double Percentage,
    string PercentageTitle,
    ICollection<string> Audios,
    string Color
);

public record CatalogGetSectionResponse(
    CatalogGetSectionResponseSection Section,
    ICollection<CatalogGetSectionResponseProfile>? Profiles,
    ICollection<CatalogGetSectionResponseGroup>? Groups,
    ICollection<CatalogGetSectionResponseAlbum>? Albums,
    ICollection<CatalogGetSectionResponseAudio>? Audios,
    ICollection<CatalogGetSectionResponseRecommendedPlaylist>? RecommendedPlaylists,
    ICollection<CatalogGetSectionResponsePlaylist>? Playlists,
    ICollection<CatalogGetSectionResponseAudioFollowingsUpdateInfo>? AudioFollowingsUpdateInfo
);

public record CatalogGetSectionResponseSection(
    string Id,
    string Title,
    ICollection<CatalogGetSectionResponseSectionBlock> Blocks,
    string Url,
    ICollection<CatalogGetSectionResponseSectionBlockAction> Actions
);

public record CatalogGetSectionResponseAlbumThumb(
    int Width,
    int Height,
    string Photo34,
    string Photo68,
    string Photo135,
    string Photo270,
    string Photo300,
    string Photo600,
    string Photo1200
);

public record CatalogGetSectionResponseSectionBlockLayoutTopTitle(
    string Icon,
    string Text
);