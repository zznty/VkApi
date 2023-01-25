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
    
    public ErrorEmitter(Func<string, StringBuilder, Task> addCallback) : base(addCallback)
    {
    }

    public override Task EmitAsync(IDictionary<string, ApiError> data)
    {
        return Task.WhenAll(data.Values.Select(error =>
        {
            var name = error.Name["api_error_".Length..].ToPascalCase();
            return AddAsync($"Api{name}Exception.g.cs",
                            new(string.Format(Content, name,
                                              error.Code.GetValueOrDefault(),
                                              error.Description is null ? null : $"\"{error.Description}\"")));
        }));
    }
}