using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace OohelpWebApps.Software.ZipExtractor;

public sealed class ExtractionService
{
    private readonly StringBuilder _logBuilder;
    private readonly ExtractionArgs _extractionArgs;

    public string ApplicationName { get; }

    public ExtractionService(StringBuilder logBuilder, ExtractionArgs extractionArgs)
    {
        _logBuilder = logBuilder;
        _extractionArgs = extractionArgs;
        this.ApplicationName = System.IO.Path.GetFileNameWithoutExtension(_extractionArgs.ExecutableFile);
    }

    public async Task<ExtractionResult> Extract(CancellationToken cancellationToken = default, IProgress<ExtractionProgress> progress = null)
    {
        try 
        {
            var processes = Process.GetProcessesByName(this.ApplicationName);

            if(processes.Length > 0)
            {
                _logBuilder.AppendLine($"Ожидание завершения {this.ApplicationName}.");
                progress?.Report(new ExtractionProgress($"Ожидание завершения {this.ApplicationName}."));
                await Task.WhenAll(processes.Select(process => KillProcessAsync(process))).ConfigureAwait(false);
            }
        }
        catch (Exception ex) 
        {
            _logBuilder.AppendLine(ex.Message);
            return ExtractionResult.FromException(ex);
        }

        try
        {
            progress?.Report(new ExtractionProgress($"Распаковка файлов..."));
            await ExtractFilesAsync(cancellationToken, progress).ConfigureAwait(false);
            return ExtractionResult.Success;
        }
        catch (OperationCanceledException)
        {
            _logBuilder.AppendLine("Canceled");
            return ExtractionResult.Cancel;
        }
        catch (Exception ex)
        {
            _logBuilder.AppendLine("Ошибка распаковки: " + ex.Message);
            return ExtractionResult.FromException(new Exception("Ошибка распаковки: " + ex.Message));
        }

    }

    private async Task ExtractFilesAsync(CancellationToken cancellationToken = default, IProgress<ExtractionProgress> progress = null)
    {
        const string Ok = " - OK";
        // Open an existing zip file for reading.
        using (ZipStorer zip = ZipStorer.Open(_extractionArgs.ZipFile, FileAccess.Read))
        {
            // Read the central directory collection.
            List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

            _logBuilder.AppendLine($"{dir.Count} файлов и папок в архиве");

            int fileNum = 0;
            foreach (var entry in dir)
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                string filePath = Path.Combine(_extractionArgs.ExtractionDirectory, entry.FilenameInZip);
                progress?.Report(new ExtractionProgress(fileNum++ * 100 / dir.Count, "Извлечение " + entry.FilenameInZip));
                _logBuilder.Append(entry.FilenameInZip);

                using (var stream = System.IO.File.Create(filePath))
                {
                    _ = await zip.ExtractFileAsync(entry, stream);
                }           
                
                progress?.Report(new ExtractionProgress(fileNum * 100 / dir.Count));
                _logBuilder.AppendLine(Ok);
            }
        }
    }

    private async static Task KillProcessAsync(Process process)
    {
        if (process == null || process.HasExited) return;

        TimeSpan waitSeconds = TimeSpan.FromSeconds(10);

        try
        {
            var waitResult = await WaitForExitAsync(process, waitSeconds).ConfigureAwait(false);
            if (!waitResult)
            {
                process.Kill();
            }
        }
        catch (InvalidOperationException)
        {
            return;
        }
        if (!process.HasExited)
        {
            throw new Exception($"Ошибка завершения процесса {process.ProcessName}.");
        }
    }

    
    private static Task<bool> WaitForExitAsync(Process process, TimeSpan timeout)
    {
        ManualResetEvent processWaitObject = new ManualResetEvent(false);
        processWaitObject.SafeWaitHandle = new SafeWaitHandle(process.Handle, false);

        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        RegisteredWaitHandle registeredProcessWaitHandle = null;
        registeredProcessWaitHandle = ThreadPool.RegisterWaitForSingleObject(
            processWaitObject,
            delegate (object state, bool timedOut)
            {
                if (!timedOut)
                {
                    registeredProcessWaitHandle.Unregister(null);
                }

                processWaitObject.Dispose();
                tcs.SetResult(!timedOut);
            },
            null /* state */,
            timeout,
            true /* executeOnlyOnce */);

        return tcs.Task;
    }

    public void StartExecutable()
    {
        if (!_extractionArgs.StartExecutableAfterExtraction) return;
        
        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(_extractionArgs.ExecutableFile);
            Process.Start(processStartInfo);
            _logBuilder.AppendLine($"Приложение запущено: {_extractionArgs.ExecutableFile}");
        }
        catch (Exception ex)
        {
            _logBuilder.AppendLine($"Ошибка старта приложения: {_extractionArgs.ExecutableFile}. {ex.Message}");
        }
    }
}
