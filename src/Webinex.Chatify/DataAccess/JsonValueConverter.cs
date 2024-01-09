using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Webinex.Chatify.DataAccess;

internal static class JsonValueConverter
{
    public static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}

internal class JsonValueConverter<T> : ValueConverter<T, string>
{
    private static readonly Expression<Func<T, string>> TO_PROVIDER = (x) =>
        JsonSerializer.Serialize(x, JsonValueConverter.JSON_SERIALIZER_OPTIONS);
    
    private static readonly Expression<Func<string, T>> TO_MODEL = (x) =>
        JsonSerializer.Deserialize<T>(x, JsonValueConverter.JSON_SERIALIZER_OPTIONS)!;
    
    public JsonValueConverter() : base(TO_PROVIDER, TO_MODEL)
    {
    }
}