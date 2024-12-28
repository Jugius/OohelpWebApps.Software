using System.Text;
using OohelpWebApps.Software.Updater.Common;
using OohelpWebApps.Software.Updater.Extentions;
using OohelpWebApps.Software.Updater.Services;

namespace OohelpWebApps.Software.Updater;
internal abstract class DialogProvider
{
    private readonly IUpdatableApplication _application;
    public abstract void ShowException(string message, string caption);
    internal abstract void ShowMessage(string message, string caption);
    internal abstract bool ShowQuestion(string message, string caption);

    internal abstract DeploymentOrder ShowUpdateInfoDialog(DownloadUpdateRequest request);
    internal abstract DeploymentOrder ShowUpdateInfoDialog(DownloadedUpdate update);
    internal abstract DownloadedUpdate ShowUpdateDownloadDialog(DownloadUpdateRequest updateRequest, ApiSoftwareService apiService);
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
    
    protected string GetVersionInfo(ApplicationRelease newRelease, ApplicationInfo appInfo)
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
}
