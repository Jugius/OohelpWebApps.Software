using Microsoft.Win32;
using OohelpWebApps.Software.Domain;
using SoftwareManager.Dialogs;
using SoftwareManager.ViewModels.Entities;
using System;
using System.Linq;
using System.Windows;

namespace SoftwareManager.Services;

public class DialogsProvider
{
    public Window DialogsOwner { get; }

    public DialogsProvider(Window dialogsOwner)
    {
        DialogsOwner = dialogsOwner;
    }

    public System.IO.FileInfo ShowOpenFileDialod()
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.Multiselect = false;
        dlg.RestoreDirectory = true;
        dlg.CheckFileExists = true;

        if (dlg.ShowDialog().GetValueOrDefault())
            return new System.IO.FileInfo(dlg.FileName);
        return null;
    }

    internal string ShowSaveAsFileDialog(string fileName = null, string filter = null)
    {
        SaveFileDialog dlg = new SaveFileDialog();
        dlg.RestoreDirectory = true;
        dlg.FileName = fileName;
        dlg.Filter = filter ?? "Все файлы|*.*";
        if (dlg.ShowDialog().GetValueOrDefault())
            return dlg.FileName;
        return null;
    }

    internal string ShowSaveAsJsonFileDialog() => ShowSaveAsFileDialog(null, "Файл JSON|*.json");
    public ApplicationInfoVM ShowApplicationInfoDialog() =>
        ShowApplicationInfoDialog(new ApplicationInfoVM { Id = Guid.NewGuid() });
    public ApplicationInfoVM ShowApplicationInfoDialog(ApplicationInfoVM appInfo)
    {
        Dialogs.ApplicationPropertiesDialog dlg = new Dialogs.ApplicationPropertiesDialog(appInfo);
        if (dlg.ShowDialog().GetValueOrDefault())
            return dlg.ApplicationInfo;
        return null;
    }
    public ApplicationReleaseVM ShowApplicationReleaseDialog(ApplicationInfoVM appInfo) =>
        ShowApplicationReleaseDialog(GenerateRelease(appInfo));
    
    internal ApplicationReleaseVM ShowApplicationReleaseDialog(ApplicationReleaseVM release)
    {
        Dialogs.ReleasePropertiesDialog dlg = new Dialogs.ReleasePropertiesDialog(release);

        if (dlg.ShowDialog().GetValueOrDefault())
            return dlg.Release;
        return null;
    }

    internal ReleaseDetailVM ShowReleaseDetailDialog(ApplicationReleaseVM release) =>
        this.ShowReleaseDetailDialog(new ReleaseDetailVM { ReleaseId = release.Id });
    internal ReleaseDetailVM ShowReleaseDetailDialog(ReleaseDetailVM detail)
    {
        var dlg = new Dialogs.DetailPropertiesDialog(detail);
        if (dlg.ShowDialog().GetValueOrDefault())
            return dlg.ReleaseDetail;
        return null;
    }
    public bool ShowDeleteQuestion(ApplicationInfoVM info) =>
        ShowQuestion($"Точно удалить все записи о приложении {info.Name}?", "Удаление приложения");
    internal bool ShowDeleteQuestion(ApplicationReleaseVM release)=>
        ShowQuestion($"Точно удалить все записи о релизе {release.Version}?", "Удаление релиза");
    internal bool ShowDeleteQuestion(ReleaseDetailVM detail) =>
        ShowQuestion($"Точно удалить запись\n{detail.Kind}: {detail.Description}?", "Удаление детали");
    internal bool ShowDeleteQuestion(ReleaseFileVM file) =>
        ShowQuestion($"Точно удалить файл: {file.Name}?", "Удаление файла");
    private static bool ShowQuestion(string message, string caption) => 
        MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    private static ApplicationReleaseVM GenerateRelease(ApplicationInfoVM appInfo)=>
        new ApplicationReleaseVM
        {
            ApplicationId = appInfo.Id,
            Kind = ReleaseKind.Minor,
            ReleaseDate = DateOnly.FromDateTime(DateTime.Now),
            Version = GenerateNextReleaseVersion(appInfo.Releases.Max(a => a.Version))
        };

    public static Version GenerateNextReleaseVersion(Version current)
    {
        if (current == null) return new Version(1, 0);
        Version newVer;

        if (current.Revision > 0)
            newVer = new Version(current.Major, current.Minor, current.Build, current.Revision + 1);
        else if (current.Build > 0)
            newVer = new Version(current.Major, current.Minor, current.Build + 1);
        else
            newVer = new Version(current.Major, current.Minor + 1);
        return newVer;
    }

    public void ShowException(string message, string caption)
    {
        string ex = string.IsNullOrEmpty(caption) ?
            message :
            $"{caption}\n\n{message}";
        MessageBox.Show(ex, caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }
    public void ShowAppSettingsDialog()
    {
        AppSettingsDialog dialog = new AppSettingsDialog(AppSettings.Instance) { Owner = this.DialogsOwner };
        if (dialog.ShowDialog().GetValueOrDefault())
            AppSettings.Save(dialog.Settings);
    }

    public void ShowException(Exception ex, string caption)
    {
        string content = ex.Message;

        string expanded = String.Empty;
        if (!string.IsNullOrEmpty(ex.StackTrace))
            expanded += ex.Message + "\n" + ex.StackTrace + "\n";

        Exception inner = ex.InnerException;
        while (inner != null)
        {
            expanded += inner.Message + "\n";
            if (!string.IsNullOrEmpty(inner.StackTrace))
                expanded += inner.StackTrace + "\n";

            inner = inner.InnerException;
        }

        Ookii.Dialogs.Wpf.TaskDialog dlg = new Ookii.Dialogs.Wpf.TaskDialog
        {
            ButtonStyle = Ookii.Dialogs.Wpf.TaskDialogButtonStyle.Standard,
            WindowTitle = caption,
            MainInstruction = caption,
            Content = content,
            ExpandedInformation = String.IsNullOrEmpty(expanded) ? null : expanded,
            MainIcon = Ookii.Dialogs.Wpf.TaskDialogIcon.Error,
            EnableHyperlinks = true,
            AllowDialogCancellation = true
        };

        dlg.Buttons.Add(new Ookii.Dialogs.Wpf.TaskDialogButton(Ookii.Dialogs.Wpf.ButtonType.Ok));

        this.DialogsOwner.Dispatcher.Invoke(() =>
        {
            _ = dlg.ShowDialog(this.DialogsOwner);
        });
    }

    internal ReleaseFileVM ShowReleaseFileDialog(ReleaseFileVM file)
    {
        Dialogs.ReleaseFilePropertiesDialog dlg = new Dialogs.ReleaseFilePropertiesDialog(file);
        if (dlg.ShowDialog().GetValueOrDefault())
            return dlg.ReleaseFile;
        return null;
    }

    internal void ShowFileInExplorer(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath)) return;
        string argument = "/select, \"" + filePath + "\"";
        System.Diagnostics.Process.Start("explorer.exe", argument);
    }
}
