namespace VkApi.Core.Responses;

public record CatalogGetAudioResponseCatalog(
    string DefaultSection,
    ICollection<CatalogGetAudioResponseCatalogSection> Sections,
    string PinnedSection
);

public record CatalogGetAudioResponse(
    CatalogGetAudioResponseCatalog Catalog
);

public record CatalogGetAudioResponseCatalogSection(
    string Id,
    string Title,
    string Url
);