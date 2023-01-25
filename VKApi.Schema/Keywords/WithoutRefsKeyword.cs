using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;

namespace VKApi.Schema.Keywords;

[SchemaKeyword(Name)]
[JsonConverter(typeof(WithoutRefsKeywordJsonConverter))]
internal class WithoutRefsKeyword : IJsonSchemaKeyword, IEquatable<WithoutRefsKeyword>
{
    /// <summary>
    /// The JSON name of the keyword.
    /// </summary>
    public const string Name = "withoutRefs";
    
    public bool Value { get; }

    public WithoutRefsKeyword(bool value)
    {
        Value = value;
    }

    public void Validate(ValidationContext context)
    {
    }

    public bool Equals(WithoutRefsKeyword other)
    {
        return Value == other.Value;
    }
}

internal class WithoutRefsKeywordJsonConverter : JsonConverter<WithoutRefsKeyword>
{
    public override WithoutRefsKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(reader.GetBoolean());
    }

    public override void Write(Utf8JsonWriter writer, WithoutRefsKeyword value, JsonSerializerOptions options)
    {
        writer.WriteBoolean(WithoutRefsKeyword.Name, value.Value);
    }
}