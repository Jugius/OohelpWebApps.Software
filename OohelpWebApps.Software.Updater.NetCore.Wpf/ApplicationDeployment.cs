using System.IO;
using OohelpWebApps.Software.Updater.Common;
using OohelpWebApps.Software.Updater.Common.Enums;
using OohelpWebApps.Software.Updater.Dialogs;
using OohelpWebApps.Software.Updater.Extentions;
using OohelpWebApps.Software.Updater.Services;

namespace OohelpWebApps.Software.Updater;
public sealed class ApplicationDeployment
{
    private const string ExtractorApplicationName = "ZipExtractor";

    private readonly ApiSoftwareService _apiSoftwareService;
    private readonly IUpdatableApplication _application;
    private readonly RuntimeVersion _runtimeVersion;
    private readonly DialogProvider _dialogProvider;

    private DownloadedUpdate _downloadedUpdate;    

    public ApplicationDeployment(IUpdatableApplication application)
    {
        this._application = application;
        this._runtimeVersion = GetRunningRuntimeVersion();
        this._apiSoftwareService = new ApiSoftwareService(application.UpdatesServerPath);
        this._dialogProvider = new DialogProvider(application.MainWindow);
    }
    /// <summary>
    /// Процедура по умолчанию, проверяем наличие обновлений, если есть то загружаем и ждем завершение приложения.
    /// </summary>    

    public async Task UpdateApplication(UpdateMethod method)
    {
        if (method == UpdateMethod.NoUpdate) return;

        bool showMessages = method == UpdateMethod.Manual;

        var result = await _apiSoftwareService.GetApplicationInfo(this._application.ApplicationName, this._application.Version);
        if (!result.IsSuccess)
        {
            if (showMessages)
                this._dialogProvider.ShowException(result.Error.Message, "Ошибка проверки обновления");

            return;
        }

        var appInfo = result.Value;

        if (appInfo.Releases.Count > 1)
        {
            await ProcessNewApplicationRelease(appInfo, method);
        }
        else
        {
            if (showMessages)
            {
                this._dialogProvider.ShowMessageYouUseLastVersion(this._application);
            }
        }        
    }

    private void FindReleaseWithNewerRuntimeVersionAndProposeDownload(ApplicationInfo appInfo, UpdateMethod method)
    {
        if (method != UpdateMethod.Manual) return;

        var release = GetNewestNextRuntimeRelease(appInfo.Releases);
        if (release == null)
        {
            this._dialogProvider.ShowMessageYouUseLastVersion(this._application);
        }
        else
        {
            var file = release.Files.FirstOrDefault(a => a.Kind == FileKind.Install);

            string message = $"Приложение {_application.ApplicationName} работает на {_runtimeVersion}. Обнаружена новая версия {release.Version.ToFormattedString()}, для работы которой необходим {file.RuntimeVersion}.\n\nПерейти на страницу для загрузки обновления?";

            if (_dialogProvider.ShowQuestion(message, $"Обновление {_application.ApplicationName}"))
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = "", UseShellExecute = true });
        }
    }
    private async Task ProcessNewApplicationRelease(ApplicationInfo appInfo, UpdateMethod method)
    {
        var release = GetNewestSameRuntimeRelease(appInfo.Releases);// ?? 

        if (release == null)
        {
            FindReleaseWithNewerRuntimeVersionAndProposeDownload(appInfo, method);
            return;
        }

        if (this._downloadedUpdate != null)
        {
            if (release.Version > this._downloadedUpdate.Release.Version) // свежезагруженный манифест новее, чем приготовленный к установке
            {
                this._downloadedUpdate.Clear();
                AppDomain.CurrentDomain.ProcessExit -= this._downloadedUpdate.OnApplicationExit;
                this._downloadedUpdate = null;
            }
            else
            {
                if (method == UpdateMethod.Automatic)
                {
                    ProcessDownloadedUpdate(false);
                }
                else
                {
                    ProcessDownloadedUpdate(null);
                }
                
                return;
            }
        }        
        
        var requestResult = await PrepareDownloadRequest(appInfo, release);
        if (!requestResult.IsSuccess)
        {
            if (method == UpdateMethod.Manual)
            {
                string message = $"Обнаружена новая версия {release.Version.ToFormattedString()}, " + 
                    "но не удалось получить информацию о загрузке.\n" +
                    "Вы можете повторить позже.\n\n" + 
                    "Ошибка: " + requestResult.Error.Message;

                this._dialogProvider.ShowException(message, "Ошибка загрузки обновления");
            }
            return;
        }

        bool showMessages = (UpdateMethod.Manual | UpdateMethod.DownloadAndUpdateOnRequest).HasFlag(method);
        bool resume = true;

        DownloadUpdateRequest downloadUpdateRequest = requestResult.Value;

        if (showMessages)
        {
            resume = this._dialogProvider.ShowUpdateInfoDialog(this._application, downloadUpdateRequest);
        }

        if (!resume) return;

        DownloadedUpdate update = await DownloadUpdate(downloadUpdateRequest, showMessages);

        if (update == null) return;

        this._downloadedUpdate = update;
        AppDomain.CurrentDomain.ProcessExit += this._downloadedUpdate.OnApplicationExit;

        
        if (method == UpdateMethod.Automatic)
        {
            ProcessDownloadedUpdate(false);
        }
        else if (method == UpdateMethod.Manual || method == UpdateMethod.DownloadAndUpdateOnRequest)
        {
            ProcessDownloadedUpdate(true);
        }
        else if (method == UpdateMethod.AutomaticDownload_UpdateOnRequest)
        {
            ProcessDownloadedUpdate(null);
        }
        
    }

    private ApplicationRelease GetNewestSameRuntimeRelease(IEnumerable<ApplicationRelease> releases)
    {
        return releases
            .Where(a => a.Files.Any(f => f.RuntimeVersion == this._runtimeVersion && f.Kind == FileKind.Update) && a.Version > this._application.Version)
            .MaxBy(a => a.Version);        
    }

    private ApplicationRelease GetNewestNextRuntimeRelease(IEnumerable<ApplicationRelease> releases)
    {
        return releases
            .Where(a => a.Files.Any(f => f.RuntimeVersion > this._runtimeVersion && f.Kind == FileKind.Install) && a.Version > this._application.Version)
            .MaxBy(a => a.Version);
    }

    private async Task<OperationResult<DownloadUpdateRequest>> PrepareDownloadRequest(ApplicationInfo appInfo, ApplicationRelease appRelease)
    {
        var appFile = appRelease.Files?.FirstOrDefault(a => a.Kind == FileKind.Update);
        
        var extractorAppResult = await _apiSoftwareService.GetApplicationInfo(ExtractorApplicationName, null);
        if (!extractorAppResult.IsSuccess)
            return new Exception("Ошибка поиска экстрактора: " + extractorAppResult.Error.Message);


        var extractorRelease = extractorAppResult.Value.Releases
            .Where(r => r.Files.Any(f => f.Kind == FileKind.Install && f.RuntimeVersion == _runtimeVersion))
            .MaxBy(r => r.Version);
            
        if (extractorRelease == null)
            return new Exception("В информации об экстракторе отсутствует адрес файла.");

        var extractorFile = extractorRelease.Files.First(f => f.Kind == FileKind.Install && f.RuntimeVersion == _runtimeVersion);

        return new DownloadUpdateRequest
        {
            AppInfo = appInfo,
            Release = appRelease,
            ApplicationReleaseFile = appFile,
            ExtractorReleaseFile = extractorFile,
        };
    }    

    private async Task<DownloadedUpdate> DownloadUpdate(DownloadUpdateRequest updateRequest, bool showMessages)
    {
        if (showMessages)
        {
            return this._dialogProvider.ShowUpdateDownloadDialog(this._application, updateRequest, this._apiSoftwareService);
            //throw new NotImplementedException();
        }

        var downloadAppTask = SaveFileToUpdateFolder(updateRequest.ApplicationReleaseFile);
        var downloadExtractorTask = SaveFileToUpdateFolder(updateRequest.ExtractorReleaseFile);

        await Task.WhenAll(downloadAppTask, downloadExtractorTask);

        if (downloadAppTask.Result.IsSuccess && downloadExtractorTask.Result.IsSuccess)
        {
            return new DownloadedUpdate
            {
                AppInfo = updateRequest.AppInfo,
                Release = updateRequest.Release,
                ApplicationReleaseFile = updateRequest.ApplicationReleaseFile,
                ExtractorReleaseFile = updateRequest.ExtractorReleaseFile,
                ApplicationPascagePath = downloadAppTask.Result.Value,
                ExtractorPath = downloadExtractorTask.Result.Value
            };
        }
        return null;
    }

    private void ProcessDownloadedUpdate(bool? restartImmediately)
    {
        bool restartApp = false;
        if (restartImmediately.HasValue)
        {
            this._downloadedUpdate.StartApplicationAfterDeployment = restartApp = restartImmediately.Value;            
        }
        else
        {
            this._downloadedUpdate.StartApplicationAfterDeployment = restartApp =
                this._dialogProvider.ShowUpdateInfoDialog(this._application, this._downloadedUpdate);
        }

        if (restartApp)
        {
            this._application.MainWindow.Dispatcher.Invoke(() =>
            {
                System.Windows.Application.Current.Shutdown();
            });
        }
    }  
    private async Task<OperationResult<string>> SaveFileToUpdateFolder(ReleaseFile releaseFile)
    {
        const string UPDATE_DIRECTORY_NAME = "updates";

        var downloadresult = await _apiSoftwareService.DownloadToTempFile(releaseFile.Id);
        if (!downloadresult.IsSuccess)
            return new Exception($"Ошибка загрузки файла {releaseFile.Name}: " + downloadresult.Error.Message);

        var hash = await FilesService.ComputeFileHash(downloadresult.Value);
        if (!string.Equals(hash, releaseFile.CheckSum, StringComparison.OrdinalIgnoreCase))
            return new Exception($"Ошибка проверки файла {releaseFile.Name}: не совпадает контрольная сумма");

        string destFolder = Path.Combine(AppContext.BaseDirectory, UPDATE_DIRECTORY_NAME);
        if (!Directory.Exists(destFolder))
            Directory.CreateDirectory(destFolder);

        try
        {
            string destFile = Path.Combine(destFolder, releaseFile.Name);

            if (File.Exists(destFile))
                File.Delete(destFile);

            File.Move(downloadresult.Value, destFile);

            return destFile;
        }
        catch (Exception ex)
        {
            return new Exception($"Ошибка переноса файла {releaseFile.Name}: " + ex.GetBaseException().Message);
        }

    } 
    public static RuntimeVersion GetRunningRuntimeVersion()
    {
        var netVer = Environment.Version;

        int ver = netVer.Major * 10 + netVer.Minor;

        if (ver >= 80) return RuntimeVersion.Net8;
        if (ver >= 70) return RuntimeVersion.Net7;
        if (ver >= 60) return RuntimeVersion.Net6;
        if (ver >= 50) return RuntimeVersion.Net5;

        return RuntimeVersion.NetFramework;
    }

    public string GetStatus()
    {
        if (this._downloadedUpdate == null)
            return "None";

        return $"Downloaded version: {this._downloadedUpdate.Release.Version.ToFormattedString()}\n" +
            $"{this._downloadedUpdate}";
    }
}
