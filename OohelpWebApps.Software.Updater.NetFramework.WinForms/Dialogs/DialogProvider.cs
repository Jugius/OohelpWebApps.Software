using System.Linq;
using System.Text;
using System;
using System.Windows.Forms;
using OohelpWebApps.Software.Updater.Services;
using OohelpWebApps.Software.Updater.Extentions;
using OohelpWebApps.Software.Updater.Common;
using OohelpWebApps.Software.Updater.Common.Enums;

namespace OohelpWebApps.Software.Updater.Dialogs;
internal class DialogProvider
{
    private readonly IUpdatableApplication _application;
    private Form DialogsOwner => _application.MainWindow;

    public DialogProvider(IUpdatableApplication application)
    {
        _application = application;
    }
    public void ShowException(string message, string caption) => this.DialogsOwner.Invoke(() =>
    {
        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
    });
    public void ShowException_SearchUpdateError(Exception error, UpdateMethod method)
    {
        if (method != UpdateMethod.Manual) return;

        ShowException(error.Message, "Ошибка проверки обновления");
    }
    public void ShowException_GetExtractorError(Exception error, Version updateVersion, UpdateMethod method)
    {
        if (method != UpdateMethod.Manual) return;

        string message = $"Обнаружена новая версия {updateVersion.ToFormattedString()}, " +
                    "но не удалось получить информацию о загрузке.\n" +
                    "Вы можете повторить позже.\n\n" +
                    "Ошибка: " + error.Message;

        this.ShowException(message, "Ошибка загрузки обновления");
    }

    internal void ShowMessage(string message, string caption) => this.DialogsOwner.Invoke(() =>
    {
        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
    });

    internal void ShowMessage_YouUseLastVersion(UpdateMethod method)
    {
        if (method == UpdateMethod.Manual)
        {
            ShowMessage(
                message: $"Вы используете последнюю версию {_application.ApplicationName}." +
                $"\nТекущая версия: {_application.Version.ToFormattedString()}",
                caption: "Обновление");
        }
    }
    internal DownloadedUpdate ShowUpdateDownloadDialog(DownloadUpdateRequest updateRequest, ApiSoftwareService apiService) => (DownloadedUpdate)this.DialogsOwner.Invoke(() =>
    {
        UpdateDownloadDialog dlg = new UpdateDownloadDialog(updateRequest, apiService)
        {
            Text = $"{_application.ApplicationName} Installer",
            CurrentVersion = "Текущая версия: " + _application.Version.ToFormattedString(),
            UpdateVersion = "Версия обновления: " + updateRequest.Release.Version.ToFormattedString(),

            Owner = DialogsOwner,
            Icon = DialogsOwner.Icon
        };

        var dialogResult = dlg.ShowDialog();

        if (dialogResult == DialogResult.OK)
            return dlg.DownloadedUpdate;

        if (dialogResult == DialogResult.Abort)
            ShowException(dlg.ErrorMessage, "Ошибка загрузки");

        return null;
    });

    private string GetVersionInfo(ApplicationRelease newRelease, ApplicationInfo appInfo)
    {
        var releases = appInfo.Releases.Where(a => a.Version > _application.Version && a.Version <= newRelease.Version).OrderByDescending(a => a.Version);

        StringBuilder sb = new StringBuilder();
        foreach (var release in releases)
        {
            sb.AppendLine($"Вер.: {release.Version.ToFormattedString()}, {release.ReleaseDate:dd.MM.yyyy}");

            if (release.Details == null || release.Details.Count == 0) continue;

            foreach (var group in release.Details.GroupBy(a => a.Kind))
            {
                sb.AppendLine(group.Key.ToValueString())
                  .Append(string.Join(Environment.NewLine, group.Select(a => $"  {a.Description}")))
                  .AppendLine();
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
    private UpdateInfoDialog BuildUpdateInfoDialog(IUpdate update)
    {
        var installedRelease = update.AppInfo.Releases.First(a => a.Version == _application.Version);
        return new UpdateInfoDialog
        {
            Text = $"{_application.ApplicationName} Installer",
            ApplicationName = _application.ApplicationName,
            NewFeatures = "Новые возможности в версии " + update.Release.Version.ToFormattedString(),
            UpdateVersion = update.Release.Version.ToFormattedString(),
            UpdateSize = FilesService.FormatBytes(update.ApplicationReleaseFile.Size + update.ExtractorReleaseFile.Size, 1, true),
            UpdateStatus = "Не запущено",
            UpdateDetailsUri = _application.DownloadPage,
            CurrentVersion = _application.Version.ToFormattedString(),
            LastTimeUpdated = installedRelease.ReleaseDate.ToString("dd.MM.yyyy"),
            UpdateDescription = GetVersionInfo(update.Release, update.AppInfo),

            Owner = DialogsOwner,
            Icon = DialogsOwner.Icon
        };
    }
    internal DeploymentOrder ShowUpdateInfoDialog(DownloadUpdateRequest request) => (DeploymentOrder)this.DialogsOwner.Invoke(() =>
    {
        UpdateInfoDialog dialog = BuildUpdateInfoDialog(request);

        bool isCritical = request.Release.Kind == ReleaseKind.Critical;

        if (isCritical)
        {
            dialog.NewFeatures = "Срочное обновление, версия " + request.Release.Version.ToFormattedString();
            dialog.UpdateStatus = "Обновление будет установлено после выхода из программы";
        }

        if(dialog.ShowDialog() == DialogResult.OK)
            return DeploymentOrder.Immediately;

        return isCritical ? DeploymentOrder.Quietly : DeploymentOrder.NoUpdate;
    });
    internal DeploymentOrder ShowUpdateInfoDialog(DownloadedUpdate update) => (DeploymentOrder)this.DialogsOwner.Invoke(() =>
    {
        UpdateInfoDialog dialog = BuildUpdateInfoDialog(update);
        dialog.UpdateStatus = "Обновление будет установлено после выхода из программы";

        if (update.Release.Kind == ReleaseKind.Critical)
            dialog.NewFeatures = "Срочное обновление, версия " + update.Release.Version.ToFormattedString();

        return dialog.ShowDialog() == DialogResult.OK
        ? DeploymentOrder.Immediately
        : DeploymentOrder.Quietly;
    });
    internal bool ShowQuestion(string message, string caption) => (bool)this.DialogsOwner.Invoke(() =>
    {
        return MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    });
}
