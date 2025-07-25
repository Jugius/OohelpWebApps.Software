using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using OohelpWebApps.Software.Updater.Common;
using OohelpWebApps.Software.Updater.Models;

namespace OohelpWebApps.Software.Updater.Services;
internal class ApiSoftwareService
{
    public event EventHandler<long?> ContentLengthUpdated;
    private readonly HttpClient _httpClient;
    public ApiSoftwareService(Uri baseUri)
    {
        _httpClient = HttpClientFactory.CreateDefaultHttpClient();        
        _httpClient.BaseAddress = new Uri(baseUri, "api/");
    }

    public async Task<OperationResult<ApplicationInfo>> GetApplicationInfo(string applicationName, Version version)
    {
        string query = $"application/{applicationName}";
        if (version != null)
            query += $"?version={version}";

        try
        {
            return await _httpClient.GetFromJsonAsync<ApplicationInfo>(query);             
        }
        catch (HttpRequestException httpUnavailable) when (httpUnavailable.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
        {
            return new Exception("Сервис временно недоступен.");
        }
        catch (HttpRequestException httpUnavailable) when (httpUnavailable.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return new Exception($"Приложение {applicationName} не найдено.");
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    public async Task<OperationResult<UpdatePackage>> DownloadPascage(ReleaseFile releaseFile)
    {
        Uri requestUri = new Uri($"update/{releaseFile.Id}", UriKind.Relative);
        try
        {
            return await _httpClient.GetFromJsonAsync<UpdatePackage>(requestUri);
        }
        catch (HttpRequestException httpUnavailable) when (httpUnavailable.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
        {
            return new Exception("Сервис временно недоступен.");
        }        
        catch (Exception ex)
        {
            return ex;
        }
    }
    public async Task<OperationResult<UpdatePackage>> DownloadPascage(ReleaseFile releaseFile, IProgress<DownloadProgress> progress, CancellationToken cancellationToken = default)
    {
        Uri requestUri = new Uri($"update/{releaseFile.Id}", UriKind.Relative);

        using var response = await _httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
        if (!response.IsSuccessStatusCode)
            return new Exception(response.ReasonPhrase);

        var contentLength = response.Content.Headers.ContentLength;
        ContentLengthUpdated?.Invoke(this, contentLength);

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var ms = new MemoryStream();

        var buffer = new byte[8192];
        long totalRead = 0;
        int bytesRead;
        int lastReported = -1;

        while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
        {
            await ms.WriteAsync(buffer.AsMemory(0, bytesRead));
            totalRead += bytesRead;

            if (contentLength.HasValue)
            {
                int percent = (int)Math.Round((double)totalRead / contentLength.Value * 100);
                if (percent != lastReported)
                {
                    lastReported = percent;
                    progress.Report(new DownloadProgress(contentLength.Value, totalRead));
                }                
            }
        }
        ms.Seek(0, SeekOrigin.Begin);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var pascage = await JsonSerializer.DeserializeAsync<UpdatePackage>(ms, options, cancellationToken);
        return pascage;
    }  
}
