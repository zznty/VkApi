using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;

namespace VKApi.Schema.Keywords;

[SchemaKeyword(Name)]
[JsonConverter(typeof(MethodsKeywordJsonConverter))]
internal class MethodsKeyword : IJsonSchemaKeyword, IKeyedSchemaCollector, IEquatable<MethodsKeyword>
{
    /// <summary>
    /// The JSON name of the keyword.
    /// </summary>
    public const string Name = "methods";

    /// <summary>
    /// The collection of schema definitions.
    /// </summary>
    public IReadOnlyDictionary<string, JsonSchema> Methods { get; }

    IReadOnlyDictionary<string, JsonSchema> IKeyedSchemaCollector.Schemas => Methods;

    /// <summary>
    /// Creates a new <see cref="MethodsKeyword"/>.
    /// </summary>
    /// <param name="values">The collection of schema definitions.</param>
    public MethodsKeyword(IReadOnlyDictionary<string, JsonSchema> values)
    {
        Methods = values ?? throw new ArgumentNullException(nameof(values));
    }

    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
    public bool Equals(MethodsKeyword? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Methods.Count != other.Methods.Count) return false;
        var byKey = Methods.Join(other.Methods,
                                 td => td.Key,
                                 od => od.Key,
                                 (td, od) => new { ThisDef = td.Value, OtherDef = od.Value })
                           .ToArray();
        if (byKey.Length != Methods.Count) return false;

        return byKey.All(g => Equals(g.ThisDef, g.OtherDef));
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as MethodsKeyword);
    }

    /// <summary>Serves as the default hash function.</summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return Methods.GetStringDictionaryHashCode();
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

internal class MethodsKeywordJsonConverter : JsonConverter<MethodsKeyword>
{
    public override MethodsKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected array");

        var schema = JsonSerializer.Deserialize<JsonSchema[]>(ref reader, options)!;
        return new MethodsKeyword(schema.ToDictionary(b => b.Keywords!.OfType<UnrecognizedKeyword>()
                                                            .First(c => c.Name == "name").Value!.ToString()));
    }

    public override void Write(Utf8JsonWriter writer, MethodsKeyword value, JsonSerializerOptions options)
    {
        writer.WriteStartArray(MethodsKeyword.Name);
        foreach (var kvp in value.Methods)
        {
            writer.WriteStartObject();
            JsonSerializer.Serialize(writer, kvp.Value, options);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}