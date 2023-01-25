using VKApi.Schema.Models.Schemas;

namespace VKApi.Schema.Models;

/// <summary>
/// Represents an API schema.
/// </summary>
public class ApiSchema
{
    /// <summary>
    /// Gets or sets the categories dictionary.
    /// </summary>
    public IDictionary<string, ApiCategory> Categories { get; set; }
    
    /// <summary>
    /// Gets or sets the errors dictionary.
    /// </summary>
    public IDictionary<string, ApiError> Errors { get; set; }
}