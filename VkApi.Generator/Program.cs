using System.Text;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using VKApi.Schema;

namespace VkApi.Generator;

internal static class Program
{
    private const string RepositoryUrl = "https://github.com/zznty/vk-api-schema/";
    
    public static async Task Main(DirectoryInfo targetDirectory, FileInfo project, DirectoryInfo sdkPath)
    {
        if (targetDirectory.Exists)
            targetDirectory.Delete(true);

        targetDirectory.Create();

        var filesProperties = await GetFilesPropertiesAsync(project.FullName, sdkPath.FullName);

        var schema = await VKApiSchema.ParseAsync(RepositoryUrl);

        async Task AddFile(string fileName, StringBuilder content)
        {
            await using var writer = File.CreateText(Path.Combine(targetDirectory.FullName, fileName));
            await writer.WriteLineAsync("#nullable enable");
            await writer.WriteAsync(content);
        }

        await Task.WhenAll(new ErrorEmitter(AddFile).EmitAsync(schema.Errors),
                           new MethodEmitter(filesProperties, AddFile).EmitAsync(schema.Categories));
    }

    private static async Task<IDictionary<string, IEnumerable<string>>> GetFilesPropertiesAsync(
        string file, string sdkPath)
    {
        MSBuildLocator.RegisterMSBuildPath(sdkPath);
        using var workspace = MSBuildWorkspace.Create();
        var project = await workspace.OpenProjectAsync(file);

        return await project.Documents.Where(b => b.SourceCodeKind is SourceCodeKind.Regular &&
                                                  (b.FilePath?.Contains("Responses") is true ||
                                                   b.FilePath?.Contains("Requests") is true ||
                                                   b.FilePath?.Contains("Objects") is true))
                            .ToAsyncEnumerable()
                            .SelectAwait(async b =>
                            {
                                var syntax = await b.GetSyntaxRootAsync();
                                var declaration = syntax!.ChildNodes().OfType<BaseNamespaceDeclarationSyntax>().First()
                                                         .Members.OfType<RecordDeclarationSyntax>().First();

                                return (declaration.Identifier.Text,
                                    declaration.Members.OfType<PropertyDeclarationSyntax>().Select(d => d.Identifier.Text));
                            }).ToDictionaryAsync(b => b.Item1, b => b.Item2);
    }
}