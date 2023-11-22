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
    private readonly System.Windows.Forms.Form _owner;

    public DialogProvider(Form owner)
    {
        _owner = owner;
    }
    public void ShowException(string message, string caption)
    {
        this._owner.Invoke(() => 
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }); 
    }

    internal void ShowMessage(string message, string caption)
    {
        this._owner.Invoke(() =>
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        });
    }
    internal DownloadedUpdate ShowUpdateDownloadDialog(IUpdatableApplication application, DownloadUpdateRequest updateRequest, ApiSoftwareService apiService) => (DownloadedUpdate)this._owner.Invoke(() =>
    {
        UpdateDownloadDialog dlg = new UpdateDownloadDialog(updateRequest, apiService)
        {
            Text = $"{application.ApplicationName} Installer",
            CurrentVersion = "Текущая версия: " + application.Version.ToFormattedString(),
            UpdateVersion = "Версия обновления: " + updateRequest.Release.Version.ToFormattedString(),

            Owner = _owner,
            Icon = application.MainWindow.Icon
        };

        var dialogResult = dlg.ShowDialog();

        if (dialogResult == DialogResult.OK)
            return dlg.DownloadedUpdate;

        if (dialogResult == DialogResult.Abort)
            ShowException(dlg.ErrorMessage, "Ошибка загрузки");

        return null;
    });

    private static string GetVersionInfo(ApplicationRelease currentRelease, ApplicationRelease newRelease, ApplicationInfo appInfo)
    {
        var releases = appInfo.Releases.Where(a => a.Version > currentRelease.Version && a.Version <= newRelease.Version).OrderByDescending(a => a.Version);
        StringBuilder sb = new StringBuilder();

        foreach (var release in releases)
        {
            sb.AppendLine($"Вер.: {release.Version.ToFormattedString()}, {release.ReleaseDate:dd.MM.yyyy}");

            if (release.Details == null || release.Details.Count == 0) continue;

            foreach (var group in release.Details.GroupBy(a => a.Kind))
            {
                sb.AppendLine(GetDetailKindValue(group.Key))
                  .AppendLine(string.Join(Environment.NewLine, group.Select(a => $"  {a.Description}")))
                  .AppendLine();
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string GetDetailKindValue(DetailKind kind) => kind switch
    {
        DetailKind.Changed => "Изменения:",
        DetailKind.Fixed => "Исправления:",
        DetailKind.Updated => "Обновления:",
        _ => kind.ToString()
    };

    internal bool ShowUpdateInfoDialog(IUpdatableApplication application, DownloadUpdateRequest request) => (bool)this._owner.Invoke(() =>
    {
        var installedRelease = request.AppInfo.Releases.First(a => a.Version == application.Version);
        UpdateInfoDialog dialog = new UpdateInfoDialog
        {
            Text = $"{application.ApplicationName} Installer",
            ApplicationName = application.ApplicationName,
            UpdateVersion = request.Release.Version.ToFormattedString(),
            UpdateSize = CalculateSize(request.ApplicationReleaseFile, request.ExtractorReleaseFile),
            UpdateStatus = "Не запущено",
            UpdateDetailsUri = application.DownloadPage,
            CurrentVersion = application.Version.ToFormattedString(),
            LastTimeUpdated = installedRelease.ReleaseDate.ToString("dd.MM.yyyy"),
            UpdateDescription = GetVersionInfo(installedRelease, request.Release, request.AppInfo),
            AttentionMessage = $"{application.ApplicationName} закроется автоматически",
            NewFeatures = $"Новые возможности в версии {request.Release.Version}",

            Owner = _owner,
            Icon = application.MainWindow.Icon
        };

        return dialog.ShowDialog() == DialogResult.OK;
    });
    internal bool ShowUpdateInfoDialog(IUpdatableApplication application, DownloadedUpdate update) => (bool)this._owner.Invoke(() =>
    {
        var installedRelease = update.AppInfo.Releases.First(a => a.Version == application.Version);
        UpdateInfoDialog dialog = new UpdateInfoDialog
        {
            Text = $"{application.ApplicationName} Installer",
            ApplicationName = application.ApplicationName,
            UpdateVersion = update.Release.Version.ToFormattedString(),
            UpdateSize = CalculateSize(update.ApplicationReleaseFile, update.ExtractorReleaseFile),
            UpdateStatus = "Готово к установке",
            UpdateDetailsUri = application.DownloadPage,
            CurrentVersion = application.Version.ToFormattedString(),
            LastTimeUpdated = installedRelease.ReleaseDate.ToString("dd.MM.yyyy"),
            UpdateDescription = GetVersionInfo(installedRelease, update.Release, update.AppInfo),
            AttentionMessage = $"{application.ApplicationName} закроется автоматически",
            NewFeatures = $"Новые возможности в версии {update.Release.Version}",

            Owner = _owner,
            Icon = application.MainWindow.Icon
        };

        return dialog.ShowDialog() == DialogResult.OK;
    });
    private static string CalculateSize(params ReleaseFile[] files)
    {
        return Extentions.FileSizeExtention.FormatBytes(files.Sum(a => a.Size), 1, true);
    }

    internal void ShowMessageYouUseLastVersion(IUpdatableApplication application)
    {
        string m = $"Вы используете последнюю версию {application.ApplicationName}.\nТекущая версия: {application.Version.ToFormattedString()}";
        this.ShowMessage(m, "Обновление");
    }

    internal bool ShowQuestion(string message, string caption)
    {
        bool result = (bool)this._owner.Invoke(() =>
        {
            return MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        });
        return result;

    }
}
