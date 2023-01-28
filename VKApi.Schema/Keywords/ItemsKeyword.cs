using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;

namespace VKApi.Schema.Keywords;

[SchemaKeyword(Name)]
[JsonConverter(typeof(ItemsKeywordJsonConverter))]
internal class ItemsKeyword : IJsonSchemaKeyword, ISchemaCollector, IEquatable<ItemsKeyword>
{
    /// <summary>
    /// The JSON name of the keyword.
    /// </summary>
    public const string Name = "items";

    /// <summary>
    /// The collection of schema definitions.
    /// </summary>
    public IReadOnlyList<JsonSchema> Items { get; }

    IReadOnlyList<JsonSchema> ISchemaCollector.Schemas => Items;

    /// <summary>
    /// Creates a new <see cref="ItemsKeyword"/>.
    /// </summary>
    /// <param name="values">The collection of schema definitions.</param>
    public ItemsKeyword(IReadOnlyCollection<JsonSchema> values)
    {
        Items = values.ToList() ?? throw new ArgumentNullException(nameof(values));
    }

    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
    public bool Equals(ItemsKeyword? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Items.SequenceEqual(other.Items);
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as ItemsKeyword);
    }

    /// <summary>Serves as the default hash function.</summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return Items.GetUnorderedCollectionHashCode();
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

file class ItemsKeywordJsonConverter : JsonConverter<ItemsKeyword>
{
    public override ItemsKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray && reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected array or object, got " + reader.TokenType);

        var schema = reader.TokenType == JsonTokenType.StartArray
            ? JsonSerializer.Deserialize<JsonSchema[]>(ref reader, options)
            : new[] { JsonSerializer.Deserialize<JsonSchema>(ref reader, options) };
        
        return new ItemsKeyword(schema!);
    }

    public override void Write(Utf8JsonWriter writer, ItemsKeyword value, JsonSerializerOptions options)
    {
        switch (value.Items.Count)
        {
            case 0:
                writer.WriteNull(ItemsKeyword.Name);
                break;
            case 1:
                writer.WritePropertyName(ItemsKeyword.Name);
                JsonSerializer.Serialize(writer, value.Items[0], options);
                break;
            case > 1:
            {
                writer.WriteStartArray(ItemsKeyword.Name);
            
                foreach (var item in value.Items)
                {
                    writer.WriteStartObject();
                    JsonSerializer.Serialize(writer, item, options);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
                break;
            }
        }
    }
}