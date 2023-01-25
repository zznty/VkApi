using Json.Schema;
using VKApi.Schema.Models;

namespace VKApi.Schema;

internal static class StringFormatMapper
{
    private static readonly IDictionary<Format, ApiStringFormat> _formatsMapping = new Dictionary<Format, ApiStringFormat>(new FormatComparer())
    {
        { new Format("uri"), ApiStringFormat.Uri }
    };

    public static ApiStringFormat? Map(Format? formatName)
    {
        return formatName == null || !_formatsMapping.TryGetValue(formatName, out var stringFormat)
            ? null
            : stringFormat;
    }
        
    private class FormatComparer : IEqualityComparer<Format>
    {
        public bool Equals(Format x, Format y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Key == y.Key;
        }

        public int GetHashCode(Format obj)
        {
            return obj.Key.GetHashCode();
        }
    }
}