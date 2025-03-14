using System.Reflection;
using OohelpWebApps.Software.Updater.Common;
using OohelpWebApps.Software.Updater.Common.Enums;
using OohelpWebApps.Software.Updater.Dialogs;
using OohelpWebApps.Software.Updater.Extentions;
using OohelpWebApps.Software.Updater.Services;

namespace OohelpWebApps.Software.Updater;
public sealed class ApplicationDeployment
{
    private readonly ApiSoftwareService _apiSoftwareService;
    private readonly IUpdatableApplication _application;
    private readonly DialogProvider _dialogProvider;

    private DownloadedUpdate _downloadedUpdate;

    public ApplicationDeployment(IUpdatableApplication application)
    {
        this._application = application;
        this._apiSoftwareService = new ApiSoftwareService(application.UpdatesServer);
        this._dialogProvider = new DialogProvider(application);
    }
    /// <summary>
    /// Процедура по умолчанию, проверяем наличие обновлений, если есть то загружаем и ждем завершение приложения.
    /// </summary>    

    public async Task UpdateApplication(UpdateMethod method)
    {
        if (method == UpdateMethod.NoUpdate) return;

        var result = await _apiSoftwareService.GetApplicationInfo(this._application.ApplicationName, this._application.Version);

        if (!result.IsSuccess)
        {
            this._dialogProvider.ShowException_SearchUpdateError(result.Error, method);
            return;
        }

        var appInfo = result.Value;

        if (appInfo.HasNewerVersion(this._application.Version))
        {
            await ProcessNewApplicationVersion(appInfo, method);
        }
        else 
        {
            this._dialogProvider.ShowMessage_YouUseLastVersion(method);
        }
    }

    private async Task ProcessNewApplicationVersion(ApplicationInfo appInfo, UpdateMethod method)
    {
        if (appInfo.HasSuitableReleaseToUpdate(_application.Version))
        {
            await ProcessNewApplicationUpdateRelease(appInfo, method);
            return;
        }        

        if (appInfo.HasSuitableReleaseToInstall(_application.Version))
        {
            ProcessNewApplicationInstallRelease(appInfo, method);
            return;
        }

        this._dialogProvider.ShowMessage_YouUseLastVersion(method);        
    }
    private void ProcessNewApplicationInstallRelease(ApplicationInfo appInfo, UpdateMethod method)
    {
        if (method != UpdateMethod.Manual) return;
        if (this._application.DownloadPage == null) return;

        ApplicationRelease release = appInfo.GetSuitableReleaseToInstall(_application.Version);
        var files = release.Files.Where(file => file.Kind == FileKind.Install);        

        var file = files.FirstOrDefault(f => f.RuntimeVersion > RuntimeService.Version) 
                ?? files.FirstOrDefault(f => f.RuntimeVersion == RuntimeService.Version);

        string message = file.RuntimeVersion > RuntimeService.Version
            ? $"Текущая версия {_application.ApplicationName} {_application.Version.ToFormattedString()} работает на платформе {RuntimeService.Version}.\nОбнаружена новая версия {release.Version.ToFormattedString()}, для работы которой необходим {file.RuntimeVersion}.\n\nПерейти на страницу {_application.DownloadPage.Host} для загрузки?"
            : $"Обнаружена новая версия {_application.ApplicationName} {release.Version.ToFormattedString()}.\n\nПерейти на страницу для загрузки?";

        if (_dialogProvider.ShowQuestion(message, $"Обновление {_application.ApplicationName}"))
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = this._application.DownloadPage.ToString(),
                    UseShellExecute = true
                });
        }
    }
    private async Task ProcessNewApplicationUpdateRelease(ApplicationInfo appInfo, UpdateMethod method)
    {
        ApplicationRelease release = appInfo.GetSuitableReleaseToUpdate(_application.Version);

        if (UpdateIsInQueueToDeploy(release.Version))
        {
            var order = GetDeploymentOrder(this._downloadedUpdate, method, null);
            ProcessDownloadedUpdate(order);
            return;
        }

        var appFile = release.Files.GetSuitableFileToUpdate();
        var getExtractorFileResult = await _apiSoftwareService.GetExtractorFile(appFile.RuntimeVersion);

        if (!getExtractorFileResult.IsSuccess)
        {
            this._dialogProvider.ShowException_GetExtractorError(getExtractorFileResult.Error, release.Version, method);
            return;
        }

        DownloadUpdateRequest request = new DownloadUpdateRequest
        {
            AppInfo = appInfo,
            Release = release,
            ApplicationReleaseFile = appFile,
            ExtractorReleaseFile = getExtractorFileResult.Value
        };

        await ProcessDownloadUpdateRequest(request, method);
    }
    private async Task ProcessDownloadUpdateRequest(DownloadUpdateRequest request, UpdateMethod method)
    {
        var order = GetDeploymentOrder(request, method);
        if (order.Command == UpdateCommand.NoUpdate) return;

        DownloadedUpdate update = await DownloadUpdate(request, order.Command == UpdateCommand.UpdateImmediately);

        if (update == null) return;

        this._downloadedUpdate = update;
        AppDomain.CurrentDomain.ProcessExit += this._downloadedUpdate.OnApplicationExit;

        order = GetDeploymentOrder(this._downloadedUpdate, method, order);
        ProcessDownloadedUpdate(order);
    }    
    private void ProcessDownloadedUpdate(DeploymentOrder order)
    {
        this._downloadedUpdate.StartApplicationAfterDeployment = order.StartApplicationAfterDeployment;

        if (order.Command == UpdateCommand.UpdateImmediately)
        {
            ShutdownApplication();
        }
    }
    private async Task<DownloadedUpdate> DownloadUpdate(DownloadUpdateRequest updateRequest, bool showDialogs)
    {
        if (showDialogs)
        {
            return this._dialogProvider.ShowUpdateDownloadDialog(updateRequest, this._apiSoftwareService);
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
    private async Task<OperationResult<string>> SaveFileToUpdateFolder(ReleaseFile releaseFile)
    {
        var downloadresult = await _apiSoftwareService.DownloadToTempFile(releaseFile);
        if (!downloadresult.IsSuccess) return downloadresult.Error;

        string tempFile = downloadresult.Value;

        if (!await FilesService.VerifyChecksum(tempFile, releaseFile.CheckSum))
            return new Exception($"Ошибка проверки файла {releaseFile.Name}: не совпадает контрольная сумма");

        try
        {
            return FilesService.MoveFileToUpdateDirectory(tempFile, releaseFile.Name);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private DeploymentOrder GetDeploymentOrder(DownloadUpdateRequest request, UpdateMethod method) =>
        method == UpdateMethod.Manual || method == UpdateMethod.DownloadAndUpdateOnRequest
        ? this._dialogProvider.ShowUpdateInfoDialog(request)
        : DeploymentOrder.Quietly;


    private DeploymentOrder GetDeploymentOrder(DownloadedUpdate update, UpdateMethod method, DeploymentOrder order = null) => method switch
    {
        UpdateMethod.Manual => order ?? this._dialogProvider.ShowUpdateInfoDialog(update),

        UpdateMethod.AutomaticDownload_UpdateOnRequest => order == null
        ? DeploymentOrder.Quietly
        : this._dialogProvider.ShowUpdateInfoDialog(update),

        UpdateMethod.DownloadAndUpdateOnRequest => order == null
        ? DeploymentOrder.Quietly
        : DeploymentOrder.Immediately,

        _ => DeploymentOrder.Quietly
    };
    
    private bool UpdateIsInQueueToDeploy(Version version)
    {
        if (this._downloadedUpdate == null) return false;

        if (this._downloadedUpdate.Release.Version < version) // свежезагруженный манифест новее, чем приготовленный к установке
        {
            this._downloadedUpdate.Clear();
            AppDomain.CurrentDomain.ProcessExit -= this._downloadedUpdate.OnApplicationExit;
            this._downloadedUpdate = null;
            return false;
        }
        return true;
    }  
    


    public static Version GetApplicationVersion(Assembly assembly)
    {
        var version = assembly.GetName().Version;

        if (version.Revision > 0)
            return new Version(version.Major, version.Minor, version.Build, version.Revision);

        if (version.Build > 0)
            return new Version(version.Major, version.Minor, version.Build);

        return new Version(version.Major, version.Minor);
    }
    private void ShutdownApplication()
    {
        this._application.MainWindow.Dispatcher.Invoke(() =>
        {
            System.Windows.Application.Current.Shutdown();
        });
    }
    public override string ToString()
    {
        var downloadedInfo = this._downloadedUpdate == null
            ? "No updates downloaded."
            : $"Downloaded update version: {this._downloadedUpdate.Release.Version.ToFormattedString()}\n{this._downloadedUpdate}";

        return $"Application: {this._application.ApplicationName}" +
            $"\nVersion: {this._application.Version.ToFormattedString()}" +
            $"\nRuntime: {RuntimeService.Version}" + 
            $"\n\nUpdate In Queue To Deploy:\n{downloadedInfo}";
    }
}
