namespace VkApi.Core.Errors;

public class ApiException : Exception
{
    public ErrorContext? Context { get; }

    public ApiException(int code, string? message) : base(string.IsNullOrEmpty(message)
                                                              ? code.ToString()
                                                              : $"{code}: {message}")
    {
    }
    
    public ApiException(int code, string? message, ErrorContext context) : this(code, message)
    {
        Context = context;
    }
}