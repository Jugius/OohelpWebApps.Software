using System.Diagnostics;
using System.Text;
using Microsoft.Win32.SafeHandles;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;


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

        await Task.Run(() =>
        {
            using (var archive = ZipArchive.Open(_extractionArgs.ZipFile))
            {
                var count = archive.Entries.Count(entry => !entry.IsDirectory);
                var totalSize = archive.TotalUncompressSize;

                _logBuilder.AppendLine($"{count} файлов и папок в архиве");

                long decompressed = 0;
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    progress?.Report(new ExtractionProgress((int)((double)decompressed / (double)totalSize * 100), $"Извлечение {entry.Key}"));
                    _logBuilder.Append(entry.Key);

                    entry.WriteToDirectory(_extractionArgs.ExtractionDirectory, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                    decompressed += entry.Size;
                    progress?.Report(new ExtractionProgress((int)((double)decompressed / (double)totalSize * 100)));
                    _logBuilder.AppendLine(Ok);
                }

            }
        });
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
    //void Unpack(string file, string tempPath, CancellationToken cancellationToken)
    //{
    //    List<string> files;

    //    using (Stream stream = File.OpenRead(file))
    //    using (var archive = ArchiveFactory.Open(stream))
    //    {
    //        var fileNames = archive.Entries.Select(x => x.Key).ToList();
    //        files = fileNames.Select(x => Path.Combine(tempPath, x)).ToList();

    //        // https://github.com/RupertAvery/PSXPackager/pull/40
    //        archive.EntryExtractionBegin += (sender, args) =>
    //        {
    //            _notifier.Notify(PopstationEventEnum.DecompressStart, args.Item.Key);
    //            _currentFileDecompressedSize = args.Item.Size;
    //        };

    //        archive.EntryExtractionEnd += (sender, args) =>
    //        {
    //            _notifier.Notify(PopstationEventEnum.DecompressProgress, 100);
    //            _notifier.Notify(PopstationEventEnum.DecompressComplete, null);
    //        };

    //        archive.CompressedBytesRead += ArchiveFileOnExtracting;
    //        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
    //        {
    //            entry.WriteToDirectory(tempPath, new ExtractionOptions()
    //            {
    //                ExtractFullPath = true,
    //                Overwrite = true
    //            });
    //        }
    //        archive.CompressedBytesRead -= ArchiveFileOnExtracting;

    //    }

    //    tempFiles.AddRange(files);
    //}
}
