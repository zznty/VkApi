using System.Collections.Immutable;
using Json.Schema;
using VKApi.Schema.Keywords;
using VKApi.Schema.Models;

namespace VKApi.Schema.Parsers;

internal class MethodsSchemaParser : BaseSchemaParser<IDictionary<string, ApiMethod>>
{
    public override IDictionary<string, ApiMethod> ParseSchema(JsonSchema schema)
    {
        if (schema.Keywords is null)
            return ImmutableDictionary<string, ApiMethod>.Empty;

        return schema.Keywords.OfType<MethodsKeyword>()
                     .SelectMany(b => b.Methods.Select(c => ParseMethod(c.Key, c.Value)))
                     .ToDictionary(method => method.FullName);
    }

    protected override ApiObject ParseObject(JsonSchema schema, string? name)
    {
        var obj = InitializeObject(schema, name);
        FillObject(obj, schema);
        return obj;
    }

    private ApiObject InitializeObject(JsonSchema schema, string? name)
    {
        var obj = new ApiObject();

        if (!string.IsNullOrEmpty(name)) obj.Name = name!;

        return obj;
    }

    private ApiMethod ParseMethod(string name, JsonSchema schema)
    {
        if (schema.Keywords is null)
            return new();

        var splittedName = name.Split('.');
        var method = new ApiMethod
        {
            FullName = name,
            Category = splittedName[0],
            Name = splittedName[1],
            Description = schema.Keywords.OfType<DescriptionKeyword>().FirstOrDefault()?.Value,
            Errors = schema.Keywords.OfType<ErrorsKeyword>()
                           .SelectMany(t => t.Errors.Select(tc => ParseError(tc.Key, tc.Value))),
            AccessTokenTypes = schema.Keywords.OfType<AccessTokenTypesKeyword>().SelectMany(b => b.Types),
            Parameters = schema.Keywords.OfType<ParametersKeyword>()
                               .SelectMany(t => t.Parameters.Select(c => ParseMethodParameter(c.Key, c.Value))),
            Responses = schema.Keywords.OfType<ResponsesKeyword>()
                              .SelectMany(t => t.Responses.Select(tc => ParseObject(tc.Value, tc.Key)))
        };

        return method;
    }

    internal static ApiError ParseError(string name, JsonSchema schema)
    {
        if (schema.Keywords is null)
            return new();

        var error = new ApiError
        {
            Name = name
        };
        FillError(error, schema);

        return error;
    }

    private ApiMethodParameter ParseMethodParameter(string name, JsonSchema schema)
    {
        if (schema.Keywords is null)
            return new();
        var typeKeywords = schema.Keywords.OfType<TypeKeyword>().ToArray();
        var type = typeKeywords.Length > 1
            ? JsonStringConstants.Multiple
            : typeKeywords[0].Type.ToString().ToLower();

        return new ApiMethodParameter
        {
            Name = name,
            Type = ObjectTypeMapper.Map(type),
            Description = schema.Keywords.OfType<DescriptionKeyword>().FirstOrDefault()?.Value,
            Enum = schema.Keywords.OfType<EnumKeyword>().FirstOrDefault()?.Values.Where(b => b is not null)
                         .Select(b => b!.ToString()) ?? Array.Empty<string>(),
            EnumNames = (schema.Keywords.OfType<EnumNamesKeyword>().FirstOrDefault()?.Names
                               .Where(b => b is not null) ?? Array.Empty<string>())!,
            Minimum = schema.Keywords.OfType<MinimumKeyword>().FirstOrDefault()?.Value,
            Maximum = schema.Keywords.OfType<MaximumKeyword>().FirstOrDefault()?.Value,
            Default = schema.Keywords.OfType<DefaultKeyword>().FirstOrDefault()?.Value?.ToString(),
            MinLength = schema.Keywords.OfType<MinLengthKeyword>().FirstOrDefault()?.Value,
            MaxLength = schema.Keywords.OfType<MaxLengthKeyword>().FirstOrDefault()?.Value,
            MaxItems = schema.Keywords.OfType<MaxItemsKeyword>().FirstOrDefault()?.Value,
            Items = ParseNestedObject(schema.Keywords.OfType<ItemsKeyword>().FirstOrDefault()?.SingleSchema),
            IsRequired = schema.Keywords.OfType<Keywords.RequiredKeyword>().FirstOrDefault()?.Value is true
        };
    }
}