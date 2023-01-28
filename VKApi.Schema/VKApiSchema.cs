using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Json.Pointer;
using Json.Schema;
using VKApi.Schema.Keywords;
using VKApi.Schema.Models;
using VKApi.Schema.Models.Schemas;
using VKApi.Schema.Parsers;

namespace VKApi.Schema;

/// <summary>
/// Represents API schema parser entry point.
/// </summary>
public static class VKApiSchema
{
    static VKApiSchema()
    {
        SchemaKeywordRegistry.Register<MethodsKeyword>();
        SchemaKeywordRegistry.Register<ParametersKeyword>();
        SchemaKeywordRegistry.Register<ResponsesKeyword>();
        SchemaKeywordRegistry.Unregister<Json.Schema.RequiredKeyword>();
        SchemaKeywordRegistry.Register<Keywords.RequiredKeyword>();
        SchemaKeywordRegistry.Register<EnumNamesKeyword>();
        SchemaKeywordRegistry.Register<WithSettersKeyword>();
        SchemaKeywordRegistry.Register<WithoutRefsKeyword>();
        SchemaKeywordRegistry.Register<AccessTokenTypesKeyword>();
        SchemaKeywordRegistry.Register<ErrorsKeyword>();
        SchemaKeywordRegistry.Register<CodeKeyword>();
        SchemaKeywordRegistry.Register<GlobalKeyword>();
        SchemaKeywordRegistry.Register<SubcodesKeyword>();
        SchemaKeywordRegistry.Register<SubcodeKeyword>();
        SchemaKeywordRegistry.Unregister<Json.Schema.ItemsKeyword>();
        SchemaKeywordRegistry.Register<Keywords.ItemsKeyword>();
    }

    /// <summary>
    /// Parses API schema and returns all its content.
    /// </summary>
    /// <param name="repositoryUrl">Base url for repository to fetch schema from.</param>
    /// <returns>API schema content.</returns>
    public static async Task<ApiSchema> ParseAsync(string repositoryUrl)
    {
        using var client = new HttpClient();
        var zip = await GetSchemaZipAsync(repositoryUrl, client);

        var registry = new SchemaRegistry(ValidationOptions.Default)
        {
            Fetch = uri =>
            {
                var entry = zip.Entries.FirstOrDefault(b => b.FullName["vk-api-schema-master/".Length..]
                                                             .Equals(
                                                                 uri.GetComponents(UriComponents.Path, UriFormat.Unescaped)
                                                                 ["VKCOM/vk-api-schema/raw/master/".Length..],
                                                                 StringComparison.OrdinalIgnoreCase));
                if (entry is null)
                    return null;

                using var stream = entry.Open();
                var schema = JsonSchema.FromStream(stream).Result;
                schema.BaseUri = uri;
                return schema;
            }
        };

        var schemas = await zip.Entries
                               .Where(b => b.Name.EndsWith(".json") &&
                                           b.Name is not "composer.json" and not "package.json"
                                                                         and not "schema.json")
                               .ToAsyncEnumerable()
                               .SelectAwait(async b =>
                               {
                                   await using var stream = b.Open();

                                   var fullName = b.FullName.Substring("vk-api-schema-master/".Length);

                                   return (FullName: fullName, Schema: await JsonSchema.FromStream(stream));
                               })
                               .ToArrayAsync();

        var resolver = new SchemaResolver(registry);

        for (var index = 0; index < schemas.Length; index++)
        {
            var (fullName, schema) = schemas[index];
            schemas[index] = (fullName, resolver.ForceResolveReferences(schema, new($"https://github.com/VKCOM/vk-api-schema/raw/master/{fullName}")));
        }

        await using var errorsFile = zip.Entries.First(b => b.Name == "errors.json").Open();
        var errorsSchema = await JsonSchema.FromStream(errorsFile);

        return new()
        {
            Categories = zip.Entries.Where(b => b.FullName.EndsWith("/"))
                            .Select(b => (
                                        Name: b.FullName.Substring(
                                            b.FullName.Substring(0, b.FullName.Length - 1).LastIndexOf('/') + 1),
                                        Category: new ApiCategory
                                        {
                                            Methods = schemas.FirstOrDefault(
                                                c => c.FullName ==
                                                     string.Concat(b.FullName.AsSpan("vk-api-schema-master/".Length),
                                                                   "methods.json")) is { Schema: { } } methods
                                                ? new MethodsSchemaParser().ParseSchema(methods.Schema)
                                                : ImmutableDictionary<string, ApiMethod>.Empty,
                                        }))
                            .ToDictionary(b => b.Name, b => b.Category),
            Errors = errorsSchema.Keywords?.OfType<ErrorsKeyword>()
                                 .SelectMany(
                                     b => b.Errors.Select(c => MethodsSchemaParser.ParseError(c.Key, c.Value)))
                                 .ToImmutableDictionary(b => b.Name) ?? ImmutableDictionary<string, ApiError>.Empty
        };
    }

    private static async Task<ZipArchive> GetSchemaZipAsync(string url, HttpClient client)
    {
        return new ZipArchive(
            await client.GetStreamAsync(new Uri(new(url), "archive/refs/heads/master.zip")),
                                                ZipArchiveMode.Read);
    }
}

internal class SchemaResolver
{
    private readonly SchemaRegistry _registry;
    private readonly Dictionary<Uri, JsonSchema> _visitedSchemas = new();
    
    public SchemaResolver(SchemaRegistry registry)
    {
        _registry = registry;
    }

    public JsonSchema ForceResolveReferences(JsonSchema schema, Uri currentUri)
    {
        if (schema.Keywords is null)
            return schema;

        Debug.WriteLine(currentUri);

        var anchors = schema.Keywords.OfType<IAnchorProvider>();
        foreach (var anchor in anchors)
        {
            anchor.RegisterAnchor(_registry, currentUri, schema);
        }

        var keywords = schema.Keywords.OfType<IRefResolvable>().OrderBy(k => ((IJsonSchemaKeyword)k).Priority());
        foreach (var keyword in keywords)
        {
            keyword.RegisterSubschemas(_registry, currentUri);
        }

        foreach (var collector in schema.Keywords.OfType<IKeyedSchemaCollector>())
        {
            var newKeywords = schema.Keywords!.Where(b => !ReferenceEquals(b, collector)).ToList();
            var schemas = collector.Schemas
                                   .Select(b => (b.Key, ForceResolveReferences(b.Value, currentUri)))
                                   .ToDictionary(b => b.Key, b => b.Item2);
            if (collector is PatternPropertiesKeyword)
                newKeywords.Add(
                    new PatternPropertiesKeyword(schemas.ToDictionary(b => new Regex(b.Key), b => b.Value)));
            else
                newKeywords.Add((IJsonSchemaKeyword)Activator.CreateInstance(collector.GetType(), schemas)!);

            schema = new(newKeywords)
            {
                BaseUri = currentUri.OriginalString.Contains('#')
                    ? new Uri(currentUri.OriginalString[..currentUri.OriginalString.IndexOf('#')])
                    : currentUri
            };
        }

        foreach (var collector in schema.Keywords!.OfType<ISchemaCollector>())
        {
            var newKeywords = schema.Keywords!.Where(b => !ReferenceEquals(b, collector)).ToList();
            var schemas = collector.Schemas?
                                   .Select(b => ForceResolveReferences(b, currentUri))
                                   .ToArray() ?? Array.Empty<JsonSchema>();
            newKeywords.Add((IJsonSchemaKeyword)Activator.CreateInstance(collector.GetType(), (object)schemas)!);

            schema = new(newKeywords)
            {
                BaseUri = currentUri.OriginalString.Contains('#')
                    ? new Uri(currentUri.OriginalString[..currentUri.OriginalString.IndexOf('#')])
                    : currentUri
            };
        }

        var refKeyword = schema.Keywords!.OfType<RefKeyword>().SingleOrDefault();
        if (refKeyword == null)
            return schema;

        Uri UpdateUri(Uri id, Uri? uri)
        {
            if (uri is null || id.IsAbsoluteUri)
                return id;
            var num = id.OriginalString.IndexOf('#') != 0 ? 1 : 0;
            if (num != 0 && currentUri.Segments.Length > 1 && currentUri.IsFile)
                uri = uri.GetParentUri();
            return new Uri(uri, id);
        }

        currentUri = UpdateUri(refKeyword.Reference, currentUri);
        var fragment = currentUri.GetComponents(UriComponents.Fragment, UriFormat.Unescaped);
        var cleanUri = new Uri(currentUri.OriginalString[..currentUri.OriginalString.IndexOf('#')]);

        Debug.WriteLine(currentUri);

        var subSchema = _registry.GetRegistration(cleanUri)?.Root
                                 .FindSubschema(JsonPointer.Parse(fragment), cleanUri).Item1 ??
                        throw new FileNotFoundException("Unable to resolve reference", currentUri.ToString());

        schema = new(schema.Keywords!.Where(b => !ReferenceEquals(b, refKeyword)).Concat(subSchema.Keywords!))
        {
            BaseUri = cleanUri
        };

        // prevent from circular reference
        if (_visitedSchemas.TryGetValue(currentUri, out var visitedSchema))
            schema = visitedSchema;
        else
            schema = _visitedSchemas[currentUri] = ForceResolveReferences(schema, cleanUri);

        _registry.RegisterAnchor(cleanUri, fragment, schema);

        return schema;
    }
}