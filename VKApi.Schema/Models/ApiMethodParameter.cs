namespace VKApi.Schema.Models;

/// <summary>
/// Represents an API method parameter.
/// </summary>
public class ApiMethodParameter : IApiEnumEntity
{
    /// <summary>
    /// Gets or sets object's name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets object's type as enumerable.
    /// </summary>
    public ApiObjectType Type { get; set; }

    /// <summary>
    /// Gets or sets object's description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets object's enum values.
    /// </summary>
    public IEnumerable<string> Enum { get; set; }

    /// <summary>
    /// Gets or sets object's enum names.
    /// </summary>
    public IEnumerable<string> EnumNames { get; set; }

    /// <summary>
    /// Gets or sets parameter's minimum length.
    /// </summary>
    public uint? MinLength { get; set; }

    /// <summary>
    /// Gets or sets parameter's maximum length.
    /// </summary>
    public uint? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets minimum value.
    /// </summary>
    public decimal? Minimum { get; set; }

    /// <summary>
    /// Gets or sets maximum value.
    /// </summary>
    public decimal? Maximum { get; set; }

    /// <summary>
    /// Gets or sets default value.
    /// </summary>
    public string? Default { get; set; }

    /// <summary>
    /// Gets or sets maximum number of items in array.
    /// </summary>
    public uint? MaxItems { get; set; }

    /// <summary>
    /// Gets or sets items type if object's type is array.
    /// </summary>
    public ApiObject? Items { get; set; }

    /// <summary>
    /// Gets or sets flag whether (object) property is required or not.
    /// </summary>
    public bool IsRequired { get; set; }
}