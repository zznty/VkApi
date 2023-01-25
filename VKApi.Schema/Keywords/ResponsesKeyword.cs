using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;

namespace VKApi.Schema.Keywords;

[SchemaKeyword(Name)]
[JsonConverter(typeof(ResponsesKeywordJsonConverter))]
internal class ResponsesKeyword : IJsonSchemaKeyword, IKeyedSchemaCollector, IEquatable<ResponsesKeyword>
{
	/// <summary>
	/// The JSON name of the keyword.
	/// </summary>
	public const string Name = "responses";

	/// <summary>
	/// The collection of schema definitions.
	/// </summary>
	public IReadOnlyDictionary<string, JsonSchema> Responses { get; }

	IReadOnlyDictionary<string, JsonSchema> IKeyedSchemaCollector.Schemas => Responses;

	/// <summary>
	/// Creates a new <see cref="ResponsesKeyword"/>.
	/// </summary>
	/// <param name="values">The collection of schema definitions.</param>
	public ResponsesKeyword(IReadOnlyDictionary<string, JsonSchema> values)
	{
		Responses = values ?? throw new ArgumentNullException(nameof(values));
	}

	/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
	/// <param name="other">An object to compare with this object.</param>
	/// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
	public bool Equals(ResponsesKeyword? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		if (Responses.Count != other.Responses.Count) return false;
		var byKey = Responses.Join(other.Responses,
				td => td.Key,
				od => od.Key,
				(td, od) => new { ThisDef = td.Value, OtherDef = od.Value })
			.ToArray();
		if (byKey.Length != Responses.Count) return false;

		return byKey.All(g => Equals(g.ThisDef, g.OtherDef));
	}

	/// <summary>Determines whether the specified object is equal to the current object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
	public override bool Equals(object obj)
	{
		return Equals(obj as ResponsesKeyword);
	}

	/// <summary>Serves as the default hash function.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return Responses.GetStringDictionaryHashCode();
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

internal class ResponsesKeywordJsonConverter : JsonConverter<ResponsesKeyword>
{
	public override ResponsesKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException("Expected object");

		var schema = JsonSerializer.Deserialize<Dictionary<string, JsonSchema>>(ref reader, options)!;
		return new ResponsesKeyword(schema);
	}

	public override void Write(Utf8JsonWriter writer, ResponsesKeyword value, JsonSerializerOptions options)
	{
		writer.WriteStartObject(ResponsesKeyword.Name);
		foreach (var kvp in value.Responses)
		{
			JsonSerializer.Serialize(writer, kvp.Value, options);
		}
		writer.WriteEndObject();
	}
}