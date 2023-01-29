using System.Text;
using CaseExtensions;
using VKApi.Schema.Models;

namespace VkApi.Generator;

internal class ErrorEmitter : BaseEmitter<IDictionary<string, ApiError>>
{
    private const string Content = @"namespace VkApi.Core.Errors;

public class Api{0}Exception : ApiException
{{
    public Api{0}Exception() : base({1}, {2})
    {{
    }}

    public Api{0}Exception(ErrorContext context) : base({1}, {2}, context)
    {{
    }}
}}
        ";

    private const string FactoryContent = @"namespace VkApi.Core.Errors;

public class ApiExceptionFactory : IApiExceptionFactory
{

    public ApiException CreateExceptionFromCode(int code)
    {
        return (ApiException) Activator.CreateInstance(_exceptions[code])!;
    }

    private static readonly Dictionary<int, Type> _exceptions = new()
        {
";

    private const string ExceptionEntry = @"{{ {0}, typeof(Api{1}Exception) }},";
    
    public ErrorEmitter(Func<string, StringBuilder, Task> addCallback) : base(addCallback)
    {
    }

    public override async Task EmitAsync(IDictionary<string, ApiError> data)
    {
        var sb = new StringBuilder(FactoryContent);
        
        await Task.WhenAll(data.Values.Select(error =>
        {
            var name = error.Name["api_error_".Length..].ToPascalCase();

            sb.AppendFormat(ExceptionEntry, error.Code.GetValueOrDefault(), name).AppendLine();
            
            return AddAsync($"Api{name}Exception.g.cs",
                            new(string.Format(Content, name,
                                              error.Code.GetValueOrDefault(),
                                              error.Description is null ? null : $"\"{error.Description}\"")));
        }));

        sb.AppendLine("};").Append('}');
        
        await AddAsync("ApiExceptionFactory.g.cs", sb);
    }
}