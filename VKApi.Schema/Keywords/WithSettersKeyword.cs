using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;

namespace VKApi.Schema.Keywords;

[SchemaKeyword(Name)]
[JsonConverter(typeof(WithSettersKeywordJsonConverter))]
internal class WithSettersKeyword : IJsonSchemaKeyword, IEquatable<WithSettersKeyword>
{
    /// <summary>
    /// The JSON name of the keyword.
    /// </summary>
    public const string Name = "withSetters";
    
    public bool Value { get; }

    public WithSettersKeyword(bool value)
    {
        Value = value;
    }

    public void Validate(ValidationContext context)
    {
    }

    public bool Equals(WithSettersKeyword other)
    {
        return Value == other.Value;
    }
}

internal class WithSettersKeywordJsonConverter : JsonConverter<WithSettersKeyword>
{
    public override WithSettersKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(reader.GetBoolean());
    }

    public override void Write(Utf8JsonWriter writer, WithSettersKeyword value, JsonSerializerOptions options)
    {
        writer.WriteBoolean(WithSettersKeyword.Name, value.Value);
    }
}