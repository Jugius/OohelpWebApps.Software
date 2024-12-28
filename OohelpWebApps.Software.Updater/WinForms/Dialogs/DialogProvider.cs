using OohelpWebApps.Software.Updater.Services;
using OohelpWebApps.Software.Updater.Extentions;
using OohelpWebApps.Software.Updater.Common.Enums;

namespace OohelpWebApps.Software.Updater.WinForms.Dialogs;
internal class DialogProvider : Updater.DialogProvider
{
    private readonly IWinFormsUpdatableApplication _application;
    private Form DialogsOwner => _application.MainWindow;

    public DialogProvider(IWinFormsUpdatableApplication application)
    {
        _application = application;
    }
    public override void ShowException(string message, string caption) => DialogsOwner.Invoke(() =>
    {
        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
    });

    internal override void ShowMessage(string message, string caption) => DialogsOwner.Invoke(() =>
    {
        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
    });

    internal override bool ShowQuestion(string message, string caption) => (bool)this.DialogsOwner.Invoke(() =>
    {
        return MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    });
    internal override DownloadedUpdate ShowUpdateDownloadDialog(DownloadUpdateRequest updateRequest, ApiSoftwareService apiService) => (DownloadedUpdate)DialogsOwner.Invoke(() =>
    {
        var dlg = new UpdateDownloadDialog(updateRequest, apiService)
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


    private UpdateInfoDialog BuildUpdateInfoDialog(IUpdate update)
    {
        var installedRelease = update.AppInfo.Releases.FirstOrDefault(a => a.Version == _application.Version)
            ?? update.AppInfo.Releases.Where(a => a.Version > _application.Version).OrderByDescending(a => a.Version).First();
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
    internal override DeploymentOrder ShowUpdateInfoDialog(DownloadUpdateRequest request) => (DeploymentOrder)DialogsOwner.Invoke(() =>
    {
        var dialog = BuildUpdateInfoDialog(request);

        var isCritical = request.Release.Kind == ReleaseKind.Critical;

        if (isCritical)
        {
            dialog.NewFeatures = "Срочное обновление, версия " + request.Release.Version.ToFormattedString();
            dialog.UpdateStatus = "Обновление будет установлено после выхода из программы";
        }

        if (dialog.ShowDialog() == DialogResult.OK)
            return DeploymentOrder.Immediately;

        return isCritical ? DeploymentOrder.Quietly : DeploymentOrder.NoUpdate;
    });

    internal override DeploymentOrder ShowUpdateInfoDialog(DownloadedUpdate update) => (DeploymentOrder)DialogsOwner.Invoke(() =>
    {
        var dialog = BuildUpdateInfoDialog(update);
        dialog.UpdateStatus = "Обновление будет установлено после выхода из программы";

        if (update.Release.Kind == ReleaseKind.Critical)
            dialog.NewFeatures = "Срочное обновление, версия " + update.Release.Version.ToFormattedString();

        return dialog.ShowDialog() == DialogResult.OK
        ? DeploymentOrder.Immediately
        : DeploymentOrder.Quietly;
    });

}
