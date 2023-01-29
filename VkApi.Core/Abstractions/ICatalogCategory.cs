namespace VkApi.Core.Abstractions;

public interface ICatalogCategory
{
    Task<CatalogGetAudioResponse> GetAudioAsync(CatalogGetAudioRequest request);
    Task<CatalogGetSectionResponse> GetSectionAsync(CatalogGetSectionRequest request);
}