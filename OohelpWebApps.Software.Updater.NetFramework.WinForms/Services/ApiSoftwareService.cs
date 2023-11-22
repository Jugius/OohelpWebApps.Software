using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using OohelpWebApps.Software.Updater.Common;
using OohelpWebApps.Software.Updater.Extentions;

namespace OohelpWebApps.Software.Updater.Services;
internal class ApiSoftwareService
{
    private readonly HttpClient _httpClient;

    public ApiSoftwareService(string updatesServerPath)
    {
        _httpClient = HttpClientFactory.CreateDefaultHttpClient();
        _httpClient.BaseAddress = new Uri(updatesServerPath);
    }

    public async Task<OperationResult<ApplicationInfo>> GetApplicationInfo(string applicationName, Version version)
    {
        string query = $"/api/application/{applicationName}";
        if (version != null)
            query += $"?version={version}";

        try
        {
            var appInfo = await _httpClient.GetFromJsonAsync<ApplicationInfo>(query);
            return appInfo;
        }
        catch (HttpRequestException httpUnavailable) when (httpUnavailable.HResult == 503)
        {
            return new Exception("Сервис временно недоступен.");
        }
        catch (HttpRequestException httpUnavailable) when (httpUnavailable.HResult == 404)
        {
            return new Exception("Приложение не найдено.");
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    public async Task<OperationResult<string>> DownloadToTempFile(Guid fileId)
    {
        string tempFile = Path.GetTempFileName();
        Uri uri = new Uri($"/api/file/{fileId}", UriKind.Relative);
        try
        {
            await _httpClient.DownloadAsync(uri, tempFile);
            return tempFile;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    public async Task<string> DownloadToTempFile(Guid fileId, IProgress<int> progress = null, CancellationToken cancellationToken = default)
    {
        string tempFile = Path.GetTempFileName();
        Uri uri = new Uri($"/api/file/{fileId}", UriKind.Relative);
        await _httpClient.DownloadAsync(uri, tempFile, progress, cancellationToken);
        return tempFile;
    }

}
