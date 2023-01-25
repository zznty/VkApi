using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;

namespace VKApi.Schema.Keywords;

[SchemaKeyword(Name)]
[JsonConverter(typeof(ErrorsKeywordJsonConverter))]
internal class ErrorsKeyword : IJsonSchemaKeyword, IKeyedSchemaCollector, IEquatable<ErrorsKeyword>
{
    /// <summary>
    /// The JSON name of the keyword.
    /// </summary>
    public const string Name = "errors";

    /// <summary>
    /// The collection of schema definitions.
    /// </summary>
    public IReadOnlyDictionary<string, JsonSchema> Errors { get; }

    IReadOnlyDictionary<string, JsonSchema> IKeyedSchemaCollector.Schemas => Errors;

    /// <summary>
    /// Creates a new <see cref="ErrorsKeyword"/>.
    /// </summary>
    /// <param name="values">The collection of schema definitions.</param>
    public ErrorsKeyword(IReadOnlyDictionary<string, JsonSchema> values)
    {
        Errors = values ?? throw new ArgumentNullException(nameof(values));
    }

    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
    public bool Equals(ErrorsKeyword? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Errors.Count != other.Errors.Count) return false;
        var byKey = Errors.Join(other.Errors,
                                 td => td.Key,
                                 od => od.Key,
                                 (td, od) => new { ThisDef = td.Value, OtherDef = od.Value })
                           .ToArray();
        if (byKey.Length != Errors.Count) return false;

        return byKey.All(g => Equals(g.ThisDef, g.OtherDef));
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as ErrorsKeyword);
    }

    /// <summary>Serves as the default hash function.</summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return Errors.GetStringDictionaryHashCode();
    }

    public void Validate(ValidationContext context)
    {
        context.EnterKeyword(Name);
        context.Push(context.SchemaLocation.Combine(Name), true);
        context.LocalResult.Pass();
        context.Pop();
        context.ExitKeyword(Name, true);
    }
}

internal class ErrorsKeywordJsonConverter : JsonConverter<ErrorsKeyword>
{
    public override ErrorsKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray && reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected array or object");

        var schema = reader.TokenType == JsonTokenType.StartObject ?
            JsonSerializer.Deserialize<Dictionary<string, JsonSchema>>(ref reader, options)! :
            JsonSerializer.Deserialize<JsonSchema[]>(ref reader, options)!
                          .Select((s, i) => (i.ToString(), s))
                          .ToDictionary(b => b.Item1, b => b.s);
        return new ErrorsKeyword(schema);
    }

    public override void Write(Utf8JsonWriter writer, ErrorsKeyword value, JsonSerializerOptions options)
    {
        writer.WriteStartArray(ErrorsKeyword.Name);
        foreach (var kvp in value.Errors)
        {
            writer.WriteStartObject();
            JsonSerializer.Serialize(writer, kvp.Value, options);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}