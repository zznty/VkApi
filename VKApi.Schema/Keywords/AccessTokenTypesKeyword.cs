using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;
using VKApi.Schema.Models;

namespace VKApi.Schema.Keywords;

[SchemaKeyword(Name)]
[JsonConverter(typeof(AccessTokenTypesKeywordJsonConverter))]
internal class AccessTokenTypesKeyword : IJsonSchemaKeyword, IEquatable<AccessTokenTypesKeyword>
{
    public AccessTokenTypesKeyword(IEnumerable<ApiAccessTokenType> types)
    {
        Types = types;
    }

    /// <summary>
    /// The JSON name of the keyword.
    /// </summary>
    public const string Name = "access_token_type";

    public IEnumerable<ApiAccessTokenType> Types { get; }

    public void Validate(ValidationContext context)
    {
    }

    public bool Equals(AccessTokenTypesKeyword other)
    {
        return other.Types.All(b => other.Types.Contains(b));
    }
}

internal class AccessTokenTypesKeywordJsonConverter : JsonConverter<AccessTokenTypesKeyword>
{
    public override AccessTokenTypesKeyword? Read(ref Utf8JsonReader reader, Type typeToConvert,
                                                  JsonSerializerOptions options)
    {
        return new(JsonSerializer.Deserialize<string[]>(ref reader, options)
                                 ?.Select(b => (ApiAccessTokenType)Enum.Parse(typeof(ApiAccessTokenType), b, true)) ??
                   Array.Empty<ApiAccessTokenType>());
    }

    public override void Write(Utf8JsonWriter writer, AccessTokenTypesKeyword value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(AccessTokenTypesKeyword.Name);
        writer.WriteStartArray();
        foreach (var type in value.Types)
        {
            writer.WriteStringValue(options.PropertyNamingPolicy?.ConvertName(type.ToString()) ?? type.ToString());
        }

        writer.WriteEndArray();
    }
}