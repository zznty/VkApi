using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using CaseExtensions;
using VKApiSchemaParser.Models;
using VKApiSchemaParser.Models.Schemas;

namespace VkApi.Generator;

internal partial class MethodEmitter : BaseEmitter<IDictionary<string, ApiCategory>>
{
    private readonly IDictionary<string, IEnumerable<string>> _filesProperties;

    private const string InterfaceContent = @"namespace VkApi.Core.Abstractions;

/// <summary>
/// Abstraction for {0} category.
/// </summary>
public partial interface I{0}Category
{{

";

    private const string ClassContent = @"namespace VkApi.Core.Categories;

/// <summary>
/// Implementation for {0} category.
/// </summary>
public partial class {0}Category : I{0}Category
{{
    private readonly IApiClient _client;

    public {0}Category(IApiClient client)
    {{
        _client = client;
    }}

";

    private const string MethodContentFull = @"return _client.RequestAsync<{1}, {2}>(""{0}"", request);";
    private const string MethodContentNoRequest = @"return _client.RequestAsync<{1}>(""{0}"");";

    private const string TypeContent = @"namespace VkApi.Core.{1};

public partial record {0}(
";

    private const string EnumContentStart = @"namespace VkApi.Core.Enums;

public partial class {0} : SmartEnum<{0}, {1}>
{{
    private {0}(string name, {1} value) : base(name, value)
    {{
    }}
";

    private const string EnumEntry = @"public static readonly {0} {1} = new(nameof({1}), {2});";
    

    private readonly ConcurrentDictionary<string, StringBuilder> _additionalFiles = new();

    public MethodEmitter(IDictionary<string, IEnumerable<string>> filesProperties,
                         Func<string, StringBuilder, Task> addCallback) : base(addCallback)
    {
        _filesProperties = filesProperties;
    }

    public override async Task EmitAsync(IDictionary<string, ApiCategory> data)
    {
        await Task.WhenAll(data.Where(b => b.Value.Methods.Count > 0)
                               .Select(b => EmitCategoryAsync(b.Key, b.Value)));
        await Task.WhenAll(_additionalFiles.Select(b => AddAsync(b.Key, b.Value)));
    }

    private async Task EmitCategoryAsync(string categoryName, ApiCategory category)
    {
        var name = categoryName[..^1].ToPascalCase();

        var interfaceBuilder = new StringBuilder(string.Format(InterfaceContent, name));
        var classBuilder = new StringBuilder(string.Format(ClassContent, name));

        foreach (var method in category.Methods.Values)
        {
            EmitMethod(method, interfaceBuilder, classBuilder);
        }

        interfaceBuilder.Append('}');
        classBuilder.Append('}');

        await AddAsync($"I{name}Category.g.cs", interfaceBuilder);
        await AddAsync($"{name}Category.g.cs", classBuilder);
    }

    private void EmitMethod(ApiMethod method, StringBuilder interfaceBuilder, StringBuilder classBuilder)
    {
        var parameters = method.Parameters.ToArray();
        var responses = method.Responses.ToArray();

        var parameterType = parameters.Length > 0
            ? EmitType($"{method.Category.ToPascalCase()}{method.Name.ToPascalCase()}Request", parameters)
            : null;
        var responseType = responses is [{ Properties: { } properties }] &&
                           properties.FirstOrDefault() is { Properties: { } responseProperties } &&
                           responseProperties.Any()
            ? EmitType($"{method.Category.ToPascalCase()}{method.Name.ToPascalCase()}Response", "Responses",
                       responseProperties, true)
            : null;

        var methodHeader =
            $"{(responseType is null ? "Task" : $"Task<{responseType}>")} {method.Name.ToPascalCase()}Async({(parameterType is null ? string.Empty : $"{parameterType} request")})";

        interfaceBuilder.Append(methodHeader).AppendLine(";");

        classBuilder.Append("public ").AppendLine(methodHeader).AppendLine("{");

        if (parameterType is null)
            classBuilder.AppendFormat(MethodContentNoRequest, method.FullName, responseType ?? "Response");
        else
            classBuilder.AppendFormat(MethodContentFull, method.FullName, parameterType, responseType ?? "Response");

        classBuilder.AppendLine("}");
    }

    [GeneratedRegex(@"[\s\.,]")]
    private partial Regex InvalidCharRegex();

    private string RemoveInvalidChars(string str)
    {
        str = InvalidCharRegex().Replace(str, "_");
        return char.IsNumber(str[0]) ? $"_{str}" : str;
    }

    private string EmitType(string name, string ns, IEnumerable<ApiObject> properties, bool isResponse = false)
    {
        if (_additionalFiles.ContainsKey(name))
            return name;

        string[]? existingProperties = null;
        if (_filesProperties.TryGetValue(name, out var value))
            existingProperties = value.ToArray();

        var sb = new StringBuilder();

        sb.AppendFormat(TypeContent, name, ns);
        foreach (var o in properties)
        {
            var propertyName = RemoveInvalidChars(o.Name.ToPascalCase());
            if (existingProperties?.Contains(propertyName) is true)
                continue;

            var type = MapType(name, o.Name, o.Type, o.Items, o.Properties, o.IsRequired, o.Enum, o.EnumNames);
            if (type is null) continue;
            sb.AppendLine($"{type} {propertyName},");
        }

        if (sb[^(1 + Environment.NewLine.Length)] == ',')
            sb.Remove(sb.Length - (1 + Environment.NewLine.Length), 1);

        sb.Append(isResponse ? ") : Response;" : ");");

        _additionalFiles.TryAdd($"{name}.g.cs", sb);

        return name;
    }

    private string? MapType(string typeName, string name, ApiObjectType type, ApiObject? items,
                            IEnumerable<ApiObject> properties, bool isRequired, IEnumerable<string> enumValues,
                            IEnumerable<string> enumNames)
    {
        var result = type switch
        {
            ApiObjectType.Integer when enumValues.ToArray() is ["0", "1"] && enumNames.ToArray() is ["no", "yes"] => "bool",
            _ when enumValues.Any() && enumNames.Any() => EmitEnum($"{typeName}{name.ToPascalCase()}", type, enumValues, enumNames),
            ApiObjectType.Undefined => null,
            ApiObjectType.Multiple => "JsonElement",
            ApiObjectType.Object => EmitType($"{typeName}{name.ToPascalCase()}", "Objects", properties),
            ApiObjectType.Integer => "int",
            ApiObjectType.String => "string",
            ApiObjectType.Array when items is not null =>
                $"ICollection<{MapType($"{typeName}{name.ToPascalCase()}", "items", items.Type, items.Items, items.Properties, items.IsRequired, items.Enum, items.EnumNames) ?? "JsonElement"}>",
            ApiObjectType.Number => "double",
            ApiObjectType.Boolean => "bool",
            _ => $"JsonElement /* {type} unresolved */"
        };

        if (result is not null && !isRequired)
            return result + "?";

        return result;
    }

    private string EmitEnum(string name, ApiObjectType type, IEnumerable<string> enumValues, IEnumerable<string> enumNames)
    {
        if (_additionalFiles.ContainsKey(name))
            return name;

        var typeName = type switch
        {
            ApiObjectType.Multiple => "string",
            _ => MapType(name, $"{name}Element", type, null,
                         ImmutableArray<ApiObject>.Empty, true,
                         ImmutableArray<string>.Empty, ImmutableArray<string>.Empty)
        };

        var sb = new StringBuilder(string.Format(EnumContentStart, name, typeName));
        
        foreach (var (key,value) in enumNames.Zip(enumValues))
        {
            sb.AppendFormat(EnumEntry, name, RemoveInvalidChars(key.ToPascalCase()), typeName == "string" ? $"\"{value}\"" : value).AppendLine();
        }

        sb.Append('}');

        _additionalFiles.TryAdd($"{name}.g.cs", sb);

        return name;
    }

    private string EmitType(string name, IEnumerable<ApiMethodParameter> properties)
    {
        if (_additionalFiles.ContainsKey(name))
            return name;

        var sb = new StringBuilder();

        string[]? existingProperties = null;
        if (_filesProperties.TryGetValue(name, out var value))
            existingProperties = value.ToArray();

        sb.AppendFormat(TypeContent, name, "Requests");
        foreach (var o in properties)
        {
            var propertyName = RemoveInvalidChars(o.Name.ToPascalCase());
            if (existingProperties?.Contains(propertyName) is true)
                continue;

            var type = MapType(name, o.Name, o.Type, o.Items, ImmutableArray<ApiObject>.Empty, o.IsRequired, o.Enum,
                               o.EnumNames);
            if (type is null) continue;
            sb.AppendLine($"{type} {propertyName},");
        }

        if (sb[^(1 + Environment.NewLine.Length)] == ',')
            sb.Remove(sb.Length - (1 + Environment.NewLine.Length), 1);

        sb.Append(");");

        _additionalFiles.TryAdd($"{name}.g.cs", sb);

        return name;
    }
}