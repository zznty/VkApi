using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;

namespace VKApi.Schema.Keywords;

[SchemaKeyword(Name)]
[JsonConverter(typeof(SubcodesKeywordJsonConverter))]
internal class SubcodesKeyword : IJsonSchemaKeyword, IKeyedSchemaCollector, IEquatable<SubcodesKeyword>
{
	/// <summary>
	/// The JSON name of the keyword.
	/// </summary>
	public const string Name = "subcodes";

	/// <summary>
	/// The collection of schema definitions.
	/// </summary>
	public IReadOnlyDictionary<string, JsonSchema> SubCodes { get; }

	IReadOnlyDictionary<string, JsonSchema> IKeyedSchemaCollector.Schemas => SubCodes;

	/// <summary>
	/// Creates a new <see cref="SubcodesKeyword"/>.
	/// </summary>
	/// <param name="values">The collection of schema definitions.</param>
	public SubcodesKeyword(IReadOnlyDictionary<string, JsonSchema> values)
	{
		SubCodes = values ?? throw new ArgumentNullException(nameof(values));
	}

	/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
	/// <param name="other">An object to compare with this object.</param>
	/// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
	public bool Equals(SubcodesKeyword? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		if (SubCodes.Count != other.SubCodes.Count) return false;
		var byKey = SubCodes.Join(other.SubCodes,
				td => td.Key,
				od => od.Key,
				(td, od) => new { ThisDef = td.Value, OtherDef = od.Value })
			.ToArray();
		if (byKey.Length != SubCodes.Count) return false;

		return byKey.All(g => Equals(g.ThisDef, g.OtherDef));
	}

	/// <summary>Determines whether the specified object is equal to the current object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
	public override bool Equals(object obj)
	{
		return Equals(obj as SubcodesKeyword);
	}

	/// <summary>Serves as the default hash function.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return SubCodes.GetStringDictionaryHashCode();
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

internal class SubcodesKeywordJsonConverter : JsonConverter<SubcodesKeyword>
{
	public override SubcodesKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray && reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException("Expected array or object");

		var schema = reader.TokenType == JsonTokenType.StartObject ?
			JsonSerializer.Deserialize<Dictionary<string, JsonSchema>>(ref reader, options)! :
			JsonSerializer.Deserialize<JsonSchema[]>(ref reader, options)!
			              .Select((s, i) => (i.ToString(), s))
			              .ToDictionary(b => b.Item1, b => b.s);
		return new SubcodesKeyword(schema);
	}

	public override void Write(Utf8JsonWriter writer, SubcodesKeyword value, JsonSerializerOptions options)
	{
		writer.WriteStartArray(SubcodesKeyword.Name);
		foreach (var kvp in value.SubCodes)
		{
			writer.WriteStartObject();
			JsonSerializer.Serialize(writer, kvp.Value, options);
			writer.WriteEndObject();
		}
		writer.WriteEndArray();
	}
}