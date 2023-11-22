using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Security.Authentication;

namespace OohelpWebApps.Software.Updater.Services;
internal class HttpClientFactory
{
    public static HttpClient CreateDefaultHttpClient(HttpMessageHandler httpMessageHandler = null)
    {
        httpMessageHandler ??= GetDefaultHttpClientHandler();

        var httpClient = new HttpClient(httpMessageHandler);

        ConfigureDefaultHttpClient(httpClient);

        return httpClient;
    }
    public static HttpClientHandler GetDefaultHttpClientHandler()
    {
        var httpClientHandler = new HttpClientHandler
        {
            SslProtocols = SslProtocols.None
        };

        if (httpClientHandler.SupportsAutomaticDecompression)
        {
            httpClientHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        }

        return httpClientHandler;
    }
    public static void ConfigureDefaultHttpClient(HttpClient httpClient)
    {
        if (httpClient == null)
            throw new ArgumentNullException(nameof(httpClient));

        httpClient.Timeout = TimeSpan.FromSeconds(30);

        httpClient.DefaultRequestHeaders.Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
