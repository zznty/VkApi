namespace VkApi.Core.Requests;

public record CatalogGetSectionRequest(string SectionId, string? StartFrom, bool NeedBlocks = true);