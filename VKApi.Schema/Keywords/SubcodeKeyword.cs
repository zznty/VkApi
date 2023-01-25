using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;

namespace VKApi.Schema.Keywords;

[SchemaKeyword(Name)]
[JsonConverter(typeof(SubcodeKeywordJsonConverter))]
internal class SubcodeKeyword : IJsonSchemaKeyword, IEquatable<SubcodeKeyword>
{
    /// <summary>
    /// The JSON name of the keyword.
    /// </summary>
    public const string Name = "subcode";
    
    public int Code { get; }

    public SubcodeKeyword(int code)
    {
        Code = code;
    }

    public void Validate(ValidationContext context)
    {
    }

    public bool Equals(SubcodeKeyword other)
    {
        return Code == other.Code;
    }
}

internal class SubcodeKeywordJsonConverter : JsonConverter<SubcodeKeyword>
{
    public override SubcodeKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(reader.GetInt32());
    }

    public override void Write(Utf8JsonWriter writer, SubcodeKeyword value, JsonSerializerOptions options)
    {
        writer.WriteNumber(SubcodeKeyword.Name, value.Code);
    }
}