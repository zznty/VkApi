using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;

namespace VKApi.Schema.Keywords;

[SchemaKeyword(Name)]
[JsonConverter(typeof(CodeKeywordJsonConverter))]
internal class CodeKeyword : IJsonSchemaKeyword, IEquatable<CodeKeyword>
{
    /// <summary>
    /// The JSON name of the keyword.
    /// </summary>
    public const string Name = "code";
    
    public int Code { get; }

    public CodeKeyword(int code)
    {
        Code = code;
    }

    public void Validate(ValidationContext context)
    {
    }

    public bool Equals(CodeKeyword other)
    {
        return Code == other.Code;
    }
}

internal class CodeKeywordJsonConverter : JsonConverter<CodeKeyword>
{
    public override CodeKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(reader.GetInt32());
    }

    public override void Write(Utf8JsonWriter writer, CodeKeyword value, JsonSerializerOptions options)
    {
        writer.WriteNumber(CodeKeyword.Name, value.Code);
    }
}