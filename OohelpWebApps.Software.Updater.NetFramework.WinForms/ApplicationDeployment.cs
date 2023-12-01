using System;
using System.Threading.Tasks;
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
    private readonly RuntimeVersion _runtimeVersion;
    private readonly DialogProvider _dialogProvider;

    private DownloadedUpdate _downloadedUpdate;

    public ApplicationDeployment(IUpdatableApplication application)
    {
        this._application = application;
        this._runtimeVersion = GetApplicationRuntimeVersion();
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

        var app = result.Value;

        if (!app.HasNewerVersion(this._application.Version))
        {
            this._dialogProvider.ShowMessage_YouUseLastVersion(method);
            return;
        }

        await OnNewApplicationVersionFound(result.Value, method);
    }

    private async Task OnNewApplicationVersionFound(ApplicationInfo appInfo, UpdateMethod method)
    {
        if (appInfo.TryGetSuitableReleaseToUpdate(_application.Version, _runtimeVersion, out ApplicationRelease release))
        {
            await ProcessNewApplicationRelease(appInfo, release, method);
            return;
        }

        if (method != UpdateMethod.Manual) return;

        if (!appInfo.TryGetSuitableReleaseToInstall(_application.Version, _runtimeVersion, out release)) return;

        var file = release.Files.GetSuitableFileToInstall(this._runtimeVersion);

        string message = $"Приложение {_application.ApplicationName} использует для работы {_runtimeVersion}.\nОбнаружена новая версия {release.Version.ToFormattedString()}, для работы которой необходим {file.RuntimeVersion}.\n\nПерейти на страницу для загрузки обновления?";

        if (this._application.DownloadPage != null &&
            _dialogProvider.ShowQuestion(message, $"Обновление {_application.ApplicationName}"))
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = this._application.DownloadPage.ToString(),
                    UseShellExecute = true
                });
        }
    }
    private async Task ProcessNewApplicationRelease(ApplicationInfo appInfo, ApplicationRelease release, UpdateMethod method)
    {
        if (IsUpdatePreparedToDeploy(release.Version))
        {
            var order = GetDeploymentOrder(this._downloadedUpdate, method, null);
            ProcessDownloadedUpdate(order);
            return;
        }

        var appFile = release.Files.GetSuitableFileToUpdate(this._runtimeVersion);
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

    private bool IsUpdatePreparedToDeploy(Version version)
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

    internal static RuntimeVersion GetApplicationRuntimeVersion()
    {
        var netVer = Environment.Version;

        int ver = netVer.Major * 10 + netVer.Minor;

        if (ver >= 80) return RuntimeVersion.Net8;
        if (ver >= 70) return RuntimeVersion.Net7;
        if (ver >= 60) return RuntimeVersion.Net6;
        if (ver >= 50) return RuntimeVersion.Net5;

        return RuntimeVersion.NetFramework;
    }
    private void ShutdownApplication()
    {
        this._application.MainWindow.Invoke(() =>
        {
            System.Windows.Forms.Application.Exit();
        });
    }
    public override string ToString()
    {
        string status = $"Application: {this._application.ApplicationName}" +
            $"\nVersion: {this._application.Version.ToFormattedString()}" +
            $"\nRuntime: {this._runtimeVersion}";

        if (this._downloadedUpdate == null)
        {
            status += "\n\nNo updates downloaded.";
        }
        else
        {
            status += $"\n\nDownloaded update version: {this._downloadedUpdate.Release.Version.ToFormattedString()}\n" +
            $"{this._downloadedUpdate}";
        }

        return status;
    }
}
