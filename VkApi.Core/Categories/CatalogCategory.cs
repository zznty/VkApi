namespace VkApi.Core.Categories;

public class CatalogCategory : ICatalogCategory
{
    private readonly IApiClient _client;

    public CatalogCategory(IApiClient client)
    {
        _client = client;
    }

    public Task<CatalogGetAudioResponse> GetAudioAsync(CatalogGetAudioRequest request)
    {
        return _client.RequestAsync<CatalogGetAudioRequest, CatalogGetAudioResponse>("catalog.getAudio", request);
    }

    public Task<CatalogGetSectionResponse> GetSectionAsync(CatalogGetSectionRequest request)
    {
        return _client.RequestAsync<CatalogGetSectionRequest, CatalogGetSectionResponse>("catalog.getSection", request);
    }
}