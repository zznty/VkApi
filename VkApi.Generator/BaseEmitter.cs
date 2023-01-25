using System.Text;

namespace VkApi.Generator;

internal abstract class BaseEmitter<T> where T : class
{
    private readonly Func<string, StringBuilder, Task> _addCallback;

    protected BaseEmitter(Func<string, StringBuilder, Task> addCallback)
    {
        _addCallback = addCallback;
    }

    protected Task AddAsync(string fileName, StringBuilder content) => _addCallback(fileName, content);

    public abstract Task EmitAsync(T data);
}