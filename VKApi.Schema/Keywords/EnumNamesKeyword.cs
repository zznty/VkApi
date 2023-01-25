using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;

namespace VKApi.Schema.Keywords;

[SchemaKeyword(Name)]
[JsonConverter(typeof(EnumNamesKeywordJsonConverter))]
internal class EnumNamesKeyword : IJsonSchemaKeyword, IEquatable<EnumNamesKeyword>
{
	/// <summary>
	/// The JSON name of the keyword.
	/// </summary>
	public const string Name = "enumNames";

	private readonly HashSet<string?> _names;

	/// <summary>
	/// The collection of enum values.
	/// </summary>
	/// <remarks>
	/// Enum values aren't necessarily strings; they can be of any JSON value.
	/// </remarks>
	public IReadOnlyCollection<string?> Names => _names;

	/// <summary>
	/// Creates a new <see cref="EnumNamesKeyword"/>.
	/// </summary>
	/// <param name="names">The collection of enum values.</param>
	public EnumNamesKeyword(params string?[] names)
	{
		_names = new(names ?? throw new ArgumentNullException(nameof(names)));

		if (_names.Count != names.Length)
			throw new ArgumentException("`enumNames` requires unique values");
	}

	/// <summary>
	/// Creates a new <see cref="EnumNamesKeyword"/>.
	/// </summary>
	/// <param name="values">The collection of enum values.</param>
	public EnumNamesKeyword(IEnumerable<string?> values)
	{
		_names = new(values ?? throw new ArgumentNullException(nameof(values)));

		if (_names.Count != values.Count())
			throw new ArgumentException("`enumNames` requires unique values");
	}

	/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
	/// <param name="other">An object to compare with this object.</param>
	/// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
	public bool Equals(EnumNamesKeyword? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		// Don't need ContentsEqual here because that method considers counts.
		// We know that with a hash set, all counts are 1.
		return Names.Count == other.Names.Count &&
			   Names.All(x => other.Names.Contains(x));
	}

	/// <summary>Determines whether the specified object is equal to the current object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
	public override bool Equals(object obj)
	{
		return Equals(obj as EnumNamesKeyword);
	}

	/// <summary>Serves as the default hash function.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return Names.GetUnorderedCollectionHashCode(element => element?.GetHashCode() ?? 0);
	}

	/// <inheritdoc />
	public void Validate(ValidationContext context)
	{
	}
}

internal class EnumNamesKeywordJsonConverter : JsonConverter<EnumNamesKeyword>
{
	public override EnumNamesKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var array = JsonSerializer.Deserialize<string[]>(ref reader, options);
		return new EnumNamesKeyword(array ?? Array.Empty<string>());
	}
	public override void Write(Utf8JsonWriter writer, EnumNamesKeyword value, JsonSerializerOptions options)
	{
		writer.WritePropertyName(EnumNamesKeyword.Name);
		writer.WriteStartArray();
		foreach (var name in value.Names)
		{
			writer.WriteStringValue(name);
		}
		writer.WriteEndArray();
	}
}