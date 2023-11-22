using System.IO;
using System.Net.Http;

namespace OohelpWebApps.Software.Updater.Extentions;
internal static class HttpClientExtention
{
    public static int BufferSize { get; set; } = 81920;
    public static async Task DownloadAsync(this HttpClient httpClient, Uri requestUri, Stream destination, IProgress<int> progress = null, CancellationToken cancellationToken = default)
    {
        using (var response = await httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
        {
            var contentLength = response.Content.Headers.ContentLength;

            using var contentStream = await response.Content.ReadAsStreamAsync();
            if (progress == null || !contentLength.HasValue)
            {
                await contentStream.CopyToAsync(destination, cancellationToken);
                return;
            }

            var buffer = new byte[BufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress.Report(bytesRead);
            }
        }
    }
    public static async Task DownloadAsync(this HttpClient httpClient, Uri requestUri, string destinationFile, IProgress<int> progress = null, CancellationToken cancellationToken = default)
    {
        await using var fileStream = new FileStream(destinationFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
        await DownloadAsync(httpClient, requestUri, fileStream, progress, cancellationToken);
    }
}
