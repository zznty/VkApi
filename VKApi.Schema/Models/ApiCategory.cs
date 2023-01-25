namespace VKApi.Schema.Models.Schemas;

/// <summary>
/// Represents an API schema category.
/// </summary>
public class ApiCategory
{
    /// <summary>
    /// Gets or sets methods dictionary.
    /// </summary>
    public IDictionary<string, ApiMethod> Methods { get; set; }
}