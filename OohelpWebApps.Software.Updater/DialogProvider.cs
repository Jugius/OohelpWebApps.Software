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
}
