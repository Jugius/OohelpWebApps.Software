using System.Text;
using System.Text.Json;

namespace OohelpWebApps.Software.Server.Mapping;

public class ResultsExtension
{
    public static IResult JsonWithLength<T>(T obj)
    {
        var json = JsonSerializer.Serialize(obj);
        var jsonBytes = Encoding.UTF8.GetBytes(json);

        return new JsonWithLengthResult(jsonBytes);
    }

    private class JsonWithLengthResult : IResult
    {
        private readonly byte[] _jsonBytes;

        public JsonWithLengthResult(byte[] jsonBytes)
        {
            _jsonBytes = jsonBytes;
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.ContentLength = _jsonBytes.Length;
            await httpContext.Response.Body.WriteAsync(_jsonBytes);
        }
    }
}
