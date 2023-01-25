using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;

namespace VKApi.Schema.Keywords;

[SchemaKeyword(Name)]
[JsonConverter(typeof(GlobalKeywordJsonConverter))]
internal class GlobalKeyword : IJsonSchemaKeyword, IEquatable<GlobalKeyword>
{
    /// <summary>
    /// The JSON name of the keyword.
    /// </summary>
    public const string Name = "global";
    
    public bool Value { get; }

    public GlobalKeyword(bool value)
    {
        Value = value;
    }

    public void Validate(ValidationContext context)
    {
    }

    public bool Equals(GlobalKeyword other)
    {
        return Value == other.Value;
    }
}

internal class GlobalKeywordJsonConverter : JsonConverter<GlobalKeyword>
{
    public override GlobalKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(reader.GetBoolean());
    }

    public override void Write(Utf8JsonWriter writer, GlobalKeyword value, JsonSerializerOptions options)
    {
        writer.WriteBoolean(GlobalKeyword.Name, value.Value);
    }
}