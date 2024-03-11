using System.Threading.Tasks;
using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Files;
public class DownloadReleaseFileCommand : AsyncCommandBase
{
    public DownloadReleaseFileCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override async Task ExecuteAsync(object parameter)
    {
        if (parameter is not ReleaseFileVM file) return;

        string filePath = this.DialogProvider.ShowSaveAsFileDialog(file.Name);
        if (filePath == null) return;

        try
        {
            await ApplicationsService.DownloadFile(file, filePath);
            this.DialogProvider.SelectFileInExplorer(filePath);
        }
        catch (System.Exception ex)
        {
            DialogProvider.ShowException(ex, "Ошибка загрузки файла");
        }
    }
}
