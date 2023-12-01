using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using OohelpWebApps.Software.Updater.Common;
using OohelpWebApps.Software.Updater.Common.Enums;

namespace OohelpWebApps.Software.Updater.Services;
internal class ApiSoftwareService
{
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
    public async Task<OperationResult<ReleaseFile>> GetExtractorFile(RuntimeVersion extractorRuntime)
    {
        const string extractorApplicationName = "ZipExtractor";

        var extractorAppResult = await GetApplicationInfo(extractorApplicationName, null);
        if (!extractorAppResult.IsSuccess)
            return extractorAppResult.Error;

        var extractorRelease = extractorAppResult.Value.Releases
            .Where(r => r.Files.Any(f => f.Kind == FileKind.Install && f.RuntimeVersion == extractorRuntime))
            .OrderByDescending(r => r.Version)
            .FirstOrDefault();

        if (extractorRelease == null)
            return new Exception($"Не найден распаковщик для приложения на платформе {extractorRuntime}");

        return extractorRelease.Files.First(f => f.Kind == FileKind.Install && f.RuntimeVersion == extractorRuntime);
    }
    public async Task<OperationResult<string>> DownloadToTempFile(ReleaseFile releaseFile)
    {
        try
        {
            return await DownloadToTempFile(releaseFile.Id);
        }
        catch (Exception ex)
        {
            return new Exception($"Ошибка загрузки файла {releaseFile.Name}: " + ex.GetBaseException().Message);
        }
    }
    public async Task<string> DownloadToTempFile(Guid fileId, IProgress<int> progress = null, CancellationToken cancellationToken = default)
    {
        const int BufferSize = 81920;

        string tempFile = Path.GetTempFileName();
        Uri requestUri = new Uri($"file/{fileId}", UriKind.Relative);


        using var fileStream = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, bufferSize: BufferSize, useAsync: true);
        using var response = await _httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);

        var contentLength = response.Content.Headers.ContentLength;

        using var contentStream = await response.Content.ReadAsStreamAsync();
        if (progress == null || !contentLength.HasValue)
        {
            await contentStream.CopyToAsync(fileStream);
            return tempFile;
        }

        var buffer = new byte[BufferSize];
        long totalBytesRead = 0;
        int bytesRead;
        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
        {
            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
            totalBytesRead += bytesRead;
            progress.Report(bytesRead);
        }

        return tempFile;
    }

}
