using Json.Schema;
using VKApi.Schema.Keywords;
using VKApi.Schema.Models;
using ItemsKeyword = VKApi.Schema.Keywords.ItemsKeyword;
using RequiredKeyword = Json.Schema.RequiredKeyword;

namespace VKApi.Schema.Parsers;

internal abstract class BaseSchemaParser<T>
{
    public abstract T ParseSchema(JsonSchema schema);
    protected abstract ApiObject ParseObject(JsonSchema schema, string? name);

    protected ApiObject? ParseNestedObject(JsonSchema? schema)
    {
        // TODO: Remove this check when "notifications_get_response" "items" property will NOT be an empty array
        return schema?.Keywords?.Any() is true ? ParseObject(schema, null) : null;
    }

    protected void FillObject(ApiObject obj, JsonSchema schema)
    {
        if (schema.Keywords is null)
            return;

        var typeKeywords = schema.Keywords.OfType<TypeKeyword>().ToArray();
        var type = typeKeywords.Length > 1
            ? JsonStringConstants.Multiple
            : typeKeywords.FirstOrDefault()?.Type.ToString().ToLower() ??
              (schema.Keywords.Any() ? JsonStringConstants.Object : null);

        obj.Type = ObjectTypeMapper.Map(type);

        // Format
        obj.Format = StringFormatMapper.Map(schema.Keywords.OfType<FormatKeyword>().FirstOrDefault()?.Value);

        // Properties
        var requiredProperties = schema.Keywords.OfType<RequiredKeyword>().FirstOrDefault()?.Properties ??
                                 Array.Empty<string>();

        obj.Properties = GetProperties<PropertiesKeyword>(schema, requiredProperties);
        obj.PatternProperties = GetProperties<PatternPropertiesKeyword>(schema, requiredProperties);

        obj.MinProperties = schema.Keywords.OfType<MinPropertiesKeyword>().FirstOrDefault()?.Value;
        obj.MaxProperties = schema.Keywords.OfType<MaxPropertiesKeyword>().FirstOrDefault()?.Value;
        obj.AdditionalProperties =
            schema.Keywords.OfType<AdditionalPropertiesKeyword>().FirstOrDefault()?.Schema is 
                { BoolValue: true } or { Keywords.Count: > 0 };

        // Other
        obj.Description = schema.Keywords.OfType<DescriptionKeyword>().FirstOrDefault()?.Value;
        obj.Enum = schema.Keywords.OfType<EnumKeyword>().FirstOrDefault()?.Values.Where(b => b is not null)
                         .Select(b => b!.ToString()) ?? Array.Empty<string>();
        obj.EnumNames = (schema.Keywords.OfType<EnumNamesKeyword>().FirstOrDefault()?.Names.Where(b => b is not null) ?? Array.Empty<string>())!;
        obj.Minimum = schema.Keywords.OfType<MinimumKeyword>().FirstOrDefault()?.Value;
        obj.Items = ParseNestedObject(schema.Keywords.OfType<ItemsKeyword>().FirstOrDefault()?.Items[0]);
        obj.IsRequired = schema.Keywords.OfType<RequiredKeyword>().FirstOrDefault()?.Properties.Any() is true;
        obj.WithSetters = schema.Keywords.OfType<WithSettersKeyword>().FirstOrDefault()?.Value == true;
        obj.WithoutRefs = schema.Keywords.OfType<WithoutRefsKeyword>().FirstOrDefault()?.Value == true;
        obj.AllOf = schema.Keywords.OfType<AllOfKeyword>().SelectMany(b => b.Schemas.Select(ParseNestedObject))!;
        obj.OneOf = schema.Keywords.OfType<OneOfKeyword>().SelectMany(b => b.Schemas.Select(ParseNestedObject))!;
    }

    protected static void FillError(ApiError error, JsonSchema schema)
    {
        if (schema.Keywords is null)
            return;
            
        error.Description = schema.Keywords.OfType<DescriptionKeyword>().FirstOrDefault()?.Value;
        error.Code = schema.Keywords.OfType<CodeKeyword>().FirstOrDefault()?.Code;
        error.Subcodes = schema.Keywords.OfType<SubcodesKeyword>().SelectMany(
            b => b.SubCodes.Values.Where(c => c.Keywords is not null)
                  .Select(c => c.Keywords!.OfType<SubcodeKeyword>().First().Code));
    }

    private IEnumerable<ApiObject> GetProperties<TKeyword>(JsonSchema schema,
                                                           IReadOnlyCollection<string> requiredProperties)
        where TKeyword : class, IKeyedSchemaCollector
    {
        var parsedProperties = schema.Keywords!.OfType<TKeyword>().SelectMany(t => t.Schemas
                                                                                  .Where(p => p.Value.Keywords != null)
                                                                                  .Select(p =>
                                                                                  {
                                                                                      var newObject =
                                                                                          ParseObject(p.Value, p.Key);

                                                                                      if (requiredProperties.Any())
                                                                                      {
                                                                                          newObject.IsRequired =
                                                                                              requiredProperties.Contains(
                                                                                                  newObject.Name);
                                                                                      }

                                                                                      return newObject;
                                                                                  })).ToArray();

        var duplicateProperties = parsedProperties.GroupBy(p => p.Name)
                                                  .Where(g => g.Count() > 1)
                                                  .Select(g => g.Key)
                                                  .ToArray();

        if (duplicateProperties.Any())
            throw new Exception($"Duplicate properties {string.Join(", ", duplicateProperties)}");

        return parsedProperties;
    }
}